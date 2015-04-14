using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS.Helpers;
using CoreAnimation;

namespace ConferenceAppiOS
{
    class SessionTableCell : UITableViewCell
    {
        public BuiltSessionTime model;
        public UILabel lblSessionName, lblRoom, lbldate, lblRoomTime;
		public CALayer trackView; static nfloat btnHeight = 30, btnWidth = 30; BuiltTracks builtTracks = null;
        public UIView topView;
        bool increaseWidth = false;
        bool shouldStar = false;
        private UIButton _btnAddRemove;
        public UIButton btnAddRemove
        {
            get
            {
                if (_btnAddRemove == null)
                {
                    _btnAddRemove = UIButton.FromType(UIButtonType.Custom);
                    _btnAddRemove.BackgroundColor = AppTheme.STCbtnAddRemoveBackground;
                    _btnAddRemove.SetTitle(AppTheme.WHPlusIcon, UIControlState.Normal);
                    _btnAddRemove.SetTitle(AppTheme.WHScheduleIcon, UIControlState.Selected);
                    _btnAddRemove.SetTitleColor(AppTheme.WHPlusIconNormalColor, UIControlState.Normal);
                    _btnAddRemove.SetTitleColor(AppTheme.WHScheduleIconSelectedColor, UIControlState.Selected);
                    _btnAddRemove.SetTitleColor(AppTheme.STClblSessionNameHighlightedTextColor, UIControlState.Highlighted);
                    _btnAddRemove.Font = AppTheme.WHPlusIconNameFont;
                    ContentView.Add(btnAddRemove);
                }
                return _btnAddRemove;
            }
        }


        public bool shouldShowStarIfAvailable = true;

        public SessionTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {

            ContentView.BackgroundColor = UIColor.Clear;
            lblSessionName = new UILabel()
            {
                TextColor = AppTheme.STClblSessionTextColor,
                Font = AppFonts.ProximaNovaRegular(16),
                BackgroundColor = AppTheme.STClblSessionNameBackgroundColor,
                HighlightedTextColor = AppTheme.STClblSessionNameHighlightedTextColor
            };

            lbldate = new UILabel()
            {
                TextColor = AppTheme.STClbldateTextColor,
                BackgroundColor = AppTheme.STClbldateBackgroundColor,
                HighlightedTextColor = AppTheme.STClbldateHighlightedTextColor,
                Font = AppFonts.ProximaNovaRegular(14),
            };

            lblRoom = new UILabel()
            {
                TextColor = AppTheme.STClblRoomTextColor,
                BackgroundColor = AppTheme.STClblRoomBackgroundColor,
                HighlightedTextColor = AppTheme.STClblRoomHighlightedTextColor,
                Font = AppFonts.ProximaNovaRegular(14),
            };

            lblRoomTime = new UILabel()
            {
                TextColor = AppTheme.STClblRoomTimeTextColor,
                BackgroundColor = UIColor.Clear,
                HighlightedTextColor = AppTheme.STClblRoomTimeHighlightedTextColor,
                Font = AppFonts.ProximaNovaRegular(14),
            };

            this.SelectedBackgroundView = new UIView(this.Frame);
            this.SelectedBackgroundView.BackgroundColor = AppTheme.STCCellSelecteBackColor;
            topView = new UIView();
            trackView = new CALayer();
            topView.Layer.AddSublayer(trackView);
            topView.Add(lblSessionName);
            topView.Add(lblRoomTime);
            topView.Add(lblRoom);
            topView.Add(lbldate);
            ContentView.Add(topView);

        }

        public void UpdateCell(BuiltSessionTime builtSessionTime, bool shoulStar, List<BuiltTracks> allTracks, bool shouldIncreaseWidht)
        {
            model = builtSessionTime;
            shouldStar = shoulStar;
            this.increaseWidth = shouldIncreaseWidht;
            if (builtSessionTime.BuiltSession.track != null)
            {

                builtTracks = allTracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);
            }
            if (builtTracks == null)
            {
                builtTracks = allTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }

            lblSessionName.Text = builtSessionTime.BuiltSession.title;
			lbldate.Text = convertToTodayTomorrowDate(builtSessionTime.date);
			if (convertToStartEndDate (builtSessionTime.time, builtSessionTime.length).Length > 0) {
				lblRoomTime.Text = ", " + convertToStartEndDate (builtSessionTime.time, builtSessionTime.length);
			} else {
				lblRoomTime.Text = "";
			}
			if (builtSessionTime.room.Length > 0) {
				lblRoom.Text = ", " + builtSessionTime.room;
			} else {
				lblRoom.Text = "";
			}
				
			btnAddRemove.Hidden = false;
            if (shoulStar)
            {
				btnAddRemove.Selected = shoulStar;
			}
            else
            {
                btnAddRemove.Selected = shoulStar;
            }

            if (builtTracks != null)
            {
                trackView.BackgroundColor = UIColor.Clear.FromHexString(builtTracks.color, 1.0f).CGColor;
            }

            try
            {
                if (Convert.ToDateTime(builtSessionTime.date).Date < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    btnAddRemove.Hidden = true;
                }
                else if (Convert.ToDateTime(builtSessionTime.date).Date == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(builtSessionTime.time) < Helper.timeConverterForCurrentHourMinute())
                {
                    btnAddRemove.Hidden = true;
                }
                else if (Convert.ToDateTime(builtSessionTime.date).Date > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    btnAddRemove.Hidden = false;
                }
            }
            catch
            {
                btnAddRemove.Hidden = true;
            }
        }

        private static string convertToStartEndDate(string time, string length)
        {
            string date = DateTime.Parse(time).ToString("hh:mm tt");
            string endDate = DateTime.Parse(time).AddMinutes(Convert.ToDouble(length)).ToString("hh:mm tt");
            string actualDate = string.Format("{0} - {1}", date, endDate);
            return actualDate;
        }
        Func<string, string> convertToTodayTomorrowDate = (Date) =>
        {
            var date = DateTime.Parse(Date).Date;
            if (date.Day == DateTime.Now.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
            {
                Date = "Today";
                return Date;
            }
            else if (date.Day == DateTime.Now.Day + 1 && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
            {
                Date = "Tomorrow";
                return Date;
            }
            else
            {
                var date1 = DateTime.Parse(Date).ToString("ddd MMM dd");
                return date1;
            }
        };


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            nfloat leftWidth = ContentView.Frame.Width - 60;
            nfloat roomWidth = ContentView.Frame.Width / 2 - 20;
            nfloat leftCellPadding = 21;
            nfloat CellTopPadding = 16;
            nfloat sessionNameRightPadding = 85;
            topView.Frame = new CGRect(0, 0, leftWidth, 80);
            if (this.increaseWidth)
            {
                trackView.Frame = new CGRect(0, 0, 8, ContentView.Frame.Height);
            }
            else
            {
                trackView.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);
            }

            lblSessionName.Frame = new CGRect(leftCellPadding, CellTopPadding, ContentView.Frame.Width - sessionNameRightPadding, 20);
            lbldate.SizeToFit();
            lblRoom.SizeToFit();
            lblRoomTime.SizeToFit();
            lbldate.Frame = new CGRect(leftCellPadding, lblSessionName.Frame.Bottom + 1, lbldate.Frame.Size.Width, lbldate.Frame.Size.Height);

			lblRoomTime.Frame = new CGRect(lbldate.Frame.Right, lblSessionName.Frame.Bottom + 1, lblRoomTime.Frame.Size.Width, lblRoomTime.Frame.Size.Height);
			lblRoom.Frame = new CGRect(lblRoomTime.Frame.Right, lblSessionName.Frame.Bottom + 1, ContentView.Frame.Width - (lbldate.Frame.Width + lblRoomTime.Frame.Width) - 82, lblRoom.Frame.Size.Height);
			btnAddRemove.Frame = new CGRect(topView.Frame.Right + 10, ContentView.Frame.Height / 2 - 15, btnWidth, btnHeight);
      
		}

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            if (_btnAddRemove != null)
            {
                _btnAddRemove.RemoveFromSuperview();
                _btnAddRemove = null;
            }

        }
    }
}

