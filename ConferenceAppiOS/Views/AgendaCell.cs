using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using ConferenceAppiOS.Helpers;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class AgendaCell : UITableViewCell
    {
        public UILabel lblLeftTime, lblBottomTime, lblSessionName, lblRoom;
        private UILabel _lblTopTime;

		static nfloat contentViewX = 170;

		static nfloat lblLeftTimeXPadding = 22;
		static nfloat lblLeftTimeYPadding = 15;
		static nfloat lblLeftTimeWidthPadding = 100;
		static nfloat lblLeftTimeHeightPadding = 50;

		static nfloat lblTopTimeYPadding = 15;
		static nfloat lblTopTimeWidthPadding = 100;
		static nfloat lblTopTimeHeightPadding = 50;

		static nfloat lblBottomTimeYPadding = 10;
		static nfloat lblBottomTimeWidthPadding = 100;
		static nfloat lblBottomTimeHeightPadding = 50;

		static nfloat TrackColorYPadding = 0;
		static nfloat TrackColorWidthPadding = 1;

		static nfloat lblSessionNameXPadding = 20;
		static nfloat lblSessionNameYPadding = 2;
		static nfloat lblSessionNameWidthPadding = 410;
		static nfloat lblSessionNameHeightPadding = 0;

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
                        TextColor = AppTheme.AClblTopTime,
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

        public UIView TrackColor;
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

        public BuiltAgendaItem model;
        public bool displayTime;
        private UIButton _btnAddRemove;
        public UIButton btnAddRemove
        {
            get
            {
                if (_btnAddRemove == null)
                {
                    _btnAddRemove = UIButton.FromType(UIButtonType.Custom);
                    _btnAddRemove.SetBackgroundImage(UIImage.FromBundle(AppTheme.sessionAddImage), UIControlState.Normal);
                    _btnAddRemove.SetBackgroundImage(UIImage.FromBundle(AppTheme.sessionScheduledImage), UIControlState.Selected);
                    ContentView.Add(btnAddRemove);
                }
                return _btnAddRemove;
            }
        }


        public AgendaCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            lblLeftTime = new UILabel
            {
                TextColor = AppTheme.AClblLeftTime,
				Font = AppFonts.ProximaNovaRegular(16),

            };


            lblBottomTime = new UILabel
            {
                TextColor = AppTheme.lblBottomTime,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular (12)

            };

            TrackColor = new UIView();
            TrackColor.BackgroundColor = AppTheme.ACTrackColor;

            lblSessionName = new UILabel
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextColor = AppTheme.AClblSessionName,
                Font = AppFonts.ProximaNovaRegular (16),
                HighlightedTextColor = UIColor.White,
            };
            lblRoom = new UILabel
            {
				LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppTheme.AClblRoom,
				Font = AppFonts.ProximaNovaRegular(14),
                HighlightedTextColor = UIColor.White,
				Lines = 0,
            };
            ContentView.AddSubview(lblLeftTime);
            ContentView.AddSubview(lblTopTime);
            ContentView.AddSubview(lblBottomTime);
            ContentView.AddSubview(TrackColor);
            ContentView.AddSubview(lblSessionName);
            ContentView.AddSubview(lblRoom);
            SelectionStyle = UITableViewCellSelectionStyle.None;

            var selectedBackgroundView = new UIView();
            selectedBackgroundView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            SelectedBackgroundView = selectedBackgroundView;

        }


        public void UpdateCell(BuiltAgendaItem builtAgendaItem, bool displayTime)
        {
            model = builtAgendaItem;
            this.displayTime = displayTime;            

            lblSessionName.Text = builtAgendaItem.name;
            lblRoom.Text = builtAgendaItem.location;
            lblTopTime.Text = builtAgendaItem.start_time;
            lblBottomTime.Text =builtAgendaItem.end_time;
            

            if (displayTime)
            {
                if (!String.IsNullOrWhiteSpace(builtAgendaItem.start_time))
                    lblLeftTime.Text = convertToSingleDate(builtAgendaItem.start_time);
                else
                    lblLeftTime.Text = string.Empty;
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

			lblLeftTime.Frame = new CGRect(-(contentViewX - lblLeftTimeXPadding), lblLeftTimeYPadding, lblLeftTimeWidthPadding, 0);
            lblLeftTime.SizeToFit();
			lblLeftTime.Frame = new CGRect(-(contentViewX - lblLeftTimeXPadding), lblLeftTimeYPadding, lblLeftTime.Frame.Size.Width, lblLeftTime.Frame.Size.Height);


			lblTopTime.Frame = new CGRect(-(contentViewX - lblTopTimeWidthPadding), lblTopTimeYPadding, lblTopTimeWidthPadding, 0);
            lblTopTime.SizeToFit();
			lblTopTime.Frame = new CGRect(-(contentViewX - lblTopTimeWidthPadding), lblLeftTimeYPadding, lblTopTime.Frame.Size.Width, (lblLeftTime.Frame.Size.Height == 0) ? 12.0f : lblLeftTime.Frame.Size.Height);

			lblBottomTime.Frame = new CGRect(-(contentViewX - lblBottomTimeWidthPadding), lblTopTime.Frame.Bottom + lblBottomTimeYPadding, lblBottomTimeWidthPadding, 0);
            lblBottomTime.SizeToFit();
			lblBottomTime.Frame = new CGRect(-(contentViewX - lblBottomTimeWidthPadding), lblTopTime.Frame.Bottom + lblBottomTimeYPadding, lblBottomTime.Frame.Size.Width, lblBottomTime.Frame.Size.Height);

			TrackColor.Frame = new CGRect(-TrackColorWidthPadding, TrackColorYPadding, TrackColorWidthPadding, ContentView.Frame.Height);
			lblSessionName.Frame = new CGRect(lblSessionNameXPadding, lblTopTimeYPadding, ContentView.Frame.Width - (lblSessionNameXPadding * 2 + (btnAddRemoveSize + btnStarXPadding * 2)), 0);
			lblRoom.Frame = new CGRect(lblSessionName.Frame.X, lblSessionName.Frame.Bottom, lblSessionName.Frame.Width, 0);
			lblSessionName.SizeToFit();
			lblRoom.SizeToFit ();

			var height = lblSessionName.Frame.Height + lblRoom.Frame.Size.Height;

            if (!string.IsNullOrWhiteSpace(lblRoom.Text))
            {
				lblSessionName.Frame = new CGRect(lblSessionNameXPadding, lblTopTimeYPadding, ContentView.Frame.Width - (lblSessionNameXPadding * 2 + (btnAddRemoveSize + btnStarXPadding * 2)), lblSessionName.Frame.Height);
				lblRoom.Frame = new CGRect(lblSessionName.Frame.X, lblSessionName.Frame.Bottom+(lblRoomYPadding/2), lblSessionName.Frame.Width, lblRoom.Frame.Size.Height);
            }
            else
            {
             
                if (height > 16)
                {
					lblSessionName.Frame = new CGRect(lblSessionNameXPadding, lblTopTimeYPadding, ContentView.Frame.Width - (lblSessionNameXPadding * 2 + (btnAddRemoveSize + btnStarXPadding * 2)), lblSessionName.Frame.Height);
					lblRoom.Frame = new CGRect(lblSessionName.Frame.X, lblSessionName.Frame.Bottom+(lblRoomYPadding/2), lblSessionName.Frame.Width, lblRoom.Frame.Size.Height);

				}
                else
                {
					lblSessionName.Frame = new CGRect(lblSessionNameXPadding, (ContentView.Frame.Height / 2) - (lblSessionName.Frame.Height / 2), ContentView.Frame.Width - (lblSessionNameXPadding * 2 + (btnAddRemoveSize + btnStarXPadding * 2)), lblSessionName.Frame.Height);
					lblRoom.Frame = new CGRect(lblSessionName.Frame.X, lblSessionName.Frame.Bottom+(lblRoomYPadding/2), lblSessionName.Frame.Width, lblRoom.Frame.Size.Height);
				}
            }      
        }


        public override void SetSelected(bool selected, bool animated)
        {
			base.SetSelected(false, false);
            if (Selected)
            {
				ContentView.BackgroundColor = AppTheme.NotesCellSelectedColor;
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