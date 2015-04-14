using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using ConferenceAppiOS.Helpers;
using CoreGraphics;
using CommonLayer.Entities.Built;
using System.Globalization;
using CoreAnimation;
using CommonLayer;

namespace ConferenceAppiOS
{
    public class SessionDataCell : UITableViewCell
    {
        public UILabel lblLeftTime, lblBottomTime, lblSessionName, lblRoom;
        private UILabel _lblTopTime;

		static nfloat contentViewX = 170;

		static nfloat lblLeftTimeXPadding = 22;
		static nfloat lblLeftTimeYPadding = 15;
		static nfloat lblLeftTimeWidthPadding = 100;

		static nfloat lblTopTimeYPadding = 15;
		static nfloat lblTopTimeWidthPadding = 100;

		static nfloat lblBottomTimeYPadding = 10;
		static nfloat lblBottomTimeWidthPadding = 100;

		static nfloat TrackColorYPadding = 0;
		static nfloat TrackColorWidthPadding = 5;

		static nfloat lblSessionNameXPadding = 20;
		static nfloat lblSessionNameYPadding = 2;
		static nfloat lblSessionNameWidthPadding = 410;

		static nfloat lblRoomXPadding = 10;
		static nfloat lblRoomYPadding = 10;
		static nfloat lblRoomWidthPadding = 150;
		static nfloat lblRoomHeightPadding = 20;

		static nfloat btnStarXPadding = 10;
		static nfloat btnStarYPadding = 10;
		static nfloat btnStarWidthPadding = 20;
		static nfloat btnStarHeightPadding = 20;

		static nfloat btnAddRemoveSize = 30;

        public UILabel lblTopTime
        {
            get
            {
                if (_lblTopTime == null)
                {
                    _lblTopTime = new UILabel
                    {
                        TextColor = AppTheme.SDClblTopTime,
                        HighlightedTextColor = AppTheme.SDClblTopTimeHightlighted,
                        Font = AppFonts.ProximaNovaRegular(12),
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                    };
                }
                return _lblTopTime;
            }
            set
            {
            }
        }

        public CALayer TrackColor;
		static nfloat leftMargin = 20;
		static nfloat rightMargin = 20;
		static nfloat labelHeight = 20;
		static nfloat starWidth = 33.0f;
		static nfloat topMargin = 13;
		static nfloat paddingBetweenViews = 13;
		static nfloat ContentViewX = 170.0f;
		static nfloat topBottomtimeWidth = 65;
		static nfloat topBottomtimeHeight = 15;
		static nfloat roomLabelHeight = 25;
		static nfloat MAXRoomLabelWidth = 175;
        private bool display = false;

        public BuiltSessionTime model;
        public bool displayTime;
        private UIButton _btnAddRemove;
        public UIButton btnAddRemove
        {
            get
            {
                if (_btnAddRemove == null)
                {
                    _btnAddRemove = UIButton.FromType(UIButtonType.Custom);
                    _btnAddRemove.BackgroundColor = AppTheme.SDCbtnAddRemove;
                    _btnAddRemove.SetTitle(AppTheme.SCPlusIcon, UIControlState.Normal);
                    _btnAddRemove.SetTitle(AppTheme.SCScheduleIcon, UIControlState.Selected);
					_btnAddRemove.SetTitleColor(AppTheme.SDCbtnAddRemoveBGColor, UIControlState.Normal);
					_btnAddRemove.SetTitleColor(AppTheme.SDCSelectedbtnAddRemoveBGColor, UIControlState.Selected);
                    _btnAddRemove.Font = AppTheme.LMPlusIconNameFont;
                    ContentView.Add(_btnAddRemove);
                }
                return _btnAddRemove;
            }
        }


        public SessionDataCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            lblLeftTime = new UILabel
            {
                TextColor = AppTheme.lblLeftTime,
				BackgroundColor = AppTheme.SDClblBottomTimeBackground,
                Font = AppFonts.ProximaNovaRegular(16),
            };

            lblBottomTime = new UILabel
            {
                TextColor = AppTheme.lblBottomTime,
				BackgroundColor = AppTheme.SDClblBottomTimeBackground,
                Font = AppFonts.ProximaNovaRegular(12)
            };

            TrackColor = new CALayer();

            lblSessionName = new UILabel
            {
                Lines = 1,
                TextColor = AppTheme.lblSessionName,
				Font = AppFonts.ProximaNovaRegular(16),
				BackgroundColor = AppTheme.SDClblBottomTimeBackground,
                HighlightedTextColor = AppTheme.SDClblSessionNameHighlighted,
              
            };

            lblRoom = new UILabel
            {
				Lines = 1,
                TextColor = AppTheme.lblRoom,
                Font = AppFonts.ProximaNovaRegular(14),
				BackgroundColor = AppTheme.SDClblBottomTimeBackground,
                HighlightedTextColor = AppTheme.SDClblRoomHighlighted,
            };

            ContentView.AddSubview(lblLeftTime);
            ContentView.AddSubview(lblTopTime);
            ContentView.AddSubview(lblBottomTime);
            ContentView.Layer.AddSublayer(TrackColor);
            ContentView.AddSubview(lblSessionName);
            ContentView.AddSubview(lblRoom);


            var selectedBackgroundView = new UIView();
            selectedBackgroundView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            SelectedBackgroundView = selectedBackgroundView;

        }


        public void UpdateCell(BuiltSessionTime builtSessionTime, bool displayTime, List<BuiltTracks> parentTracks, bool scheduled)
        {
            BuiltTracks builtTracks = null;
            model = builtSessionTime;
            this.displayTime = displayTime;

            display = displayTime;

            if (builtSessionTime.BuiltSession != null)
            {
                builtTracks = parentTracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);
				lblSessionName.Text = builtSessionTime.BuiltSession.title;
			}

            if (builtTracks == null)
            {
                builtTracks = parentTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }

			lblRoom.Text = builtSessionTime.room;
            lblTopTime.Text = convertToStartDate(builtSessionTime.time);
            lblBottomTime.Text = convertToEndDate(builtSessionTime.time, builtSessionTime.length);
            if (builtTracks != null)
            {
                TrackColor.BackgroundColor = UIColor.Clear.FromHexString(builtTracks.color, 1.0f).CGColor;

            }

            if (displayTime)
            {
                lblLeftTime.Text = convertToSingleDate(builtSessionTime.time);
            }
            else
            {
                lblLeftTime.Text = string.Empty;
            }
            if (btnAddRemove.Hidden == true)
            {
                btnAddRemove.Hidden = false;
                btnAddRemove.Selected = scheduled;
            }
            else
                btnAddRemove.Selected = scheduled;

            try
            {
                if (Convert.ToDateTime(builtSessionTime.date).Date < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    if (btnAddRemove.Hidden == false)
                    { btnAddRemove.Hidden = true; }

                }
                else if (Convert.ToDateTime(builtSessionTime.date).Date ==  TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(builtSessionTime.time) < Helper.timeConverterForCurrentHourMinute())
                {
                    if (btnAddRemove.Hidden == false)
                    { btnAddRemove.Hidden = true; }
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

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ContentView.Frame = new CGRect(contentViewX, ContentView.Frame.Y, ContentView.Frame.Width - contentViewX, ContentView.Frame.Height);
            SelectedBackgroundView.Frame = ContentView.Frame;
           
			lblLeftTime.Frame = new CGRect(-(contentViewX - lblLeftTimeXPadding), lblLeftTimeYPadding, lblLeftTimeWidthPadding, 0);
			lblLeftTime.SizeToFit ();
			lblLeftTime.Frame = new CGRect(-(contentViewX - lblLeftTimeXPadding), lblLeftTimeYPadding, lblLeftTime.Frame.Size.Width, lblLeftTime.Frame.Size.Height);

            lblTopTime.Frame = new CGRect(-(contentViewX - lblTopTimeWidthPadding), lblTopTimeYPadding, lblTopTimeWidthPadding, 0);
			lblTopTime.SizeToFit ();
			lblTopTime.Frame = new CGRect(-(contentViewX - lblTopTimeWidthPadding), lblLeftTimeYPadding, lblTopTime.Frame.Size.Width, (lblLeftTime.Frame.Size.Height == 0) ? 12.0f : lblLeftTime.Frame.Size.Height);

			lblBottomTime.Frame = new CGRect(-(contentViewX - lblBottomTimeWidthPadding), lblTopTime.Frame.Bottom + lblBottomTimeYPadding, lblBottomTimeWidthPadding, 0);
			lblBottomTime.SizeToFit ();
			lblBottomTime.Frame = new CGRect(-(contentViewX - lblBottomTimeWidthPadding), lblTopTime.Frame.Bottom + lblBottomTimeYPadding, lblBottomTime.Frame.Size.Width, lblBottomTime.Frame.Size.Height);

            TrackColor.Frame = new CGRect(-TrackColorWidthPadding, TrackColorYPadding, TrackColorWidthPadding, ContentView.Frame.Height);
			lblSessionName.Frame = new CGRect(lblSessionNameXPadding, lblTopTimeYPadding, ContentView.Frame.Width-(lblSessionNameXPadding*2 + (btnAddRemoveSize+btnStarXPadding*2)), 16);
//			lblSessionName.SizeToFit ();
//			lblSessionName.Frame = new CGRect(lblSessionNameXPadding, lblTopTimeYPadding, ContentView.Frame.Width-(lblSessionNameXPadding*2 + (btnAddRemoveSize+btnStarXPadding*2)),lblSessionName.Frame.Height);

			lblRoom.Frame = new CGRect(lblSessionName.Frame.X, lblSessionName.Frame.Bottom, lblSessionName.Frame.Width, lblRoomHeightPadding);
			btnAddRemove.Frame = new CGRect(lblSessionName.Frame.Right+btnStarXPadding*2, Helper.getCenterY(ContentView.Frame.Height, btnAddRemoveSize), btnAddRemoveSize, btnAddRemoveSize);
		}


        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (Selected)
            {
                ContentView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            }
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

        public string convertToStartDate(string time)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(time);
                return Helper.ToDateTimeString(date, "hh:mm tt").ToLower();
            }
            catch
            {
                return String.Empty;
            }
        }

        public string convertToEndDate(string time, string length)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var endDate = DateTime.Parse(time).AddMinutes(Convert.ToInt32(length));
                return Helper.ToDateTimeString(endDate, "hh:mm tt").ToLower();
            }
            catch
            {
                return String.Empty;
            }
        }

        public string convertToSingleDate(string time)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(time);
                return Helper.ToDateTimeString(date, "hh tt").ToLower();
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}