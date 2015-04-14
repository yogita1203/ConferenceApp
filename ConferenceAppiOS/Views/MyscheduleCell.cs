using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using CoreGraphics;
using CoreAnimation;

namespace ConferenceAppiOS.Views
{
    public class MyscheduleCell : UITableViewCell
    {
        public UILabel lblLeftTime, lblBottomTime, lblSessionName, lblRoom;
        private UILabel _lblTopTime;

		static nfloat contentViewX = 170;
		static nfloat lblLeftTimeXPadding = 22;
		static nfloat lblLeftTimeYPadding = 5;
		static nfloat lblLeftTimeWidthPadding = 100;
		static nfloat lblLeftTimeHeightPadding = 50;

		static nfloat lblTopTimeXPadding = 65;
		static nfloat lblTopTimeYPadding = 5;
		static nfloat lblTopTimeWidthPadding = 100;
		static nfloat lblTopTimeHeightPadding = 50;

		static nfloat lblBottomTimeXPadding = 15;
		static nfloat lblBottomTimeYPadding = 20;
		static nfloat lblBottomTimeWidthPadding = 100;
		static nfloat lblBottomTimeHeightPadding = 50;

		static nfloat TrackColorXPadding = 95;
		static nfloat TrackColorYPadding = 0;
		static nfloat TrackColorWidthPadding = 5;

		static nfloat lblSessionNameXPadding = 20;
		static nfloat lblSessionNameYPadding = 2;
		static nfloat lblSessionNameWidthPadding = 410;
		static nfloat lblSessionNameHeightPadding = 0;

		static nfloat lblRoomXPadding = 10;
		static nfloat lblRoomYPadding = 10;
		static nfloat lblRoomWidthPadding = 150;
		static nfloat lblRoomHeightPadding = 20;

		static nfloat btnCalenderXPadding = 10;
		static nfloat btnCalenderYPadding = 10;
		static nfloat btnCalenderWidthPadding = 20;
		static nfloat btnCalenderHeightPadding = 20;
        public UILabel lblTopTime
        {
            get
            {
                if (_lblTopTime == null)
                {
                    _lblTopTime = new UILabel
                      {
                          TextColor = AppTheme.MSClblTopTime,
                          Font = AppFonts.ProximaNovaRegular (12),
                          AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                      };
                }

                return _lblTopTime;
            }

            set
            {

            }
        }
        public BuiltSessionTime model;
        public bool displayTime;
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
        public MyscheduleCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            lblLeftTime = new UILabel
            {
				TextColor = AppTheme.MSCellNormalTextColor,
                Font = AppFonts.ProximaNovaRegular(16),

            };


            lblBottomTime = new UILabel
            {
				TextColor = AppTheme.MSCellNormalTextColor,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular (12)

            };

            TrackColor = new CALayer();

            lblSessionName = new UILabel
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
				TextColor = AppTheme.MSCellSessionNameTextColor,
                Font = AppFonts.ProximaNovaRegular (16),
                HighlightedTextColor = UIColor.White,
            };
            lblRoom = new UILabel
            {
				TextColor =  AppTheme.MSCellNormalTextColor,
				Font = AppFonts.ProximaNovaRegular(14),
                HighlightedTextColor = UIColor.White,

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


        public void UpdateCell(BuiltSessionTime builtSessionTime, bool displayTime, List<BuiltTracks> parentTracks)
        {
            model = builtSessionTime;
            display = displayTime;

            var builtTracks = parentTracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);

            if (builtTracks == null)
            {
                builtTracks = parentTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }

            lblSessionName.Text = builtSessionTime.BuiltSession.title;
            lblRoom.Text = builtSessionTime.room;
            lblTopTime.Text = convertToStartDate(builtSessionTime.time);
            lblBottomTime.Text = convertToEndDate(builtSessionTime.time, builtSessionTime.length);
            if (builtTracks != null)
            {
				TrackColor.BackgroundColor = UIColor.Clear.FromHexString((builtTracks.color),1.0f).CGColor;
            }

            if (displayTime)
            {
                lblLeftTime.Text = convertToSingleDate(builtSessionTime.time);
            }
            else
            {
                lblLeftTime.Text = string.Empty;
            }
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ContentView.Frame = new CGRect(contentViewX, ContentView.Frame.Y, ContentView.Frame.Width - contentViewX, ContentView.Frame.Height);
			SelectedBackgroundView.Frame = ContentView.Frame;
			lblLeftTime.Frame = new CGRect(-(contentViewX - lblLeftTimeXPadding), lblLeftTimeYPadding, lblLeftTimeWidthPadding, lblLeftTimeHeightPadding);
            lblTopTime.Frame = new CGRect(-(contentViewX - lblTopTimeWidthPadding), lblTopTimeYPadding, lblTopTimeWidthPadding, lblTopTimeHeightPadding);

            var frame = lblTopTime.Frame;
            frame.Y = lblTopTime.Frame.Bottom;

            lblBottomTime.Frame = new CGRect(-(contentViewX - lblBottomTimeWidthPadding), lblTopTime.Frame.Y + lblBottomTimeYPadding, lblBottomTimeWidthPadding, lblBottomTimeHeightPadding);
            TrackColor.Frame = new CGRect(-TrackColorWidthPadding, TrackColorYPadding, TrackColorWidthPadding, ContentView.Frame.Height);
            lblSessionName.Frame = new CGRect(TrackColor.Frame.Right + lblSessionNameXPadding, ContentView.Frame.Height / lblSessionNameYPadding, this.Frame.Width - lblSessionNameWidthPadding, lblSessionNameHeightPadding);
            var h = Helper.getTextHeight(lblSessionName.Text, lblSessionName.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, lblSessionName.Font, lblSessionName);
            var rect = lblSessionName.Frame;
            rect.Height = h;
            rect.Y = rect.Y - h / 2;
            lblSessionName.Frame = rect;
            lblRoom.Frame = new CGRect(lblSessionName.Frame.Right + lblRoomXPadding, ContentView.Frame.Height / 2 - lblRoomYPadding, lblRoomWidthPadding, lblRoomHeightPadding);
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