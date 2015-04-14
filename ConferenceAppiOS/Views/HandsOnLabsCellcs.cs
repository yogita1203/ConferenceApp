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

namespace ConferenceAppiOS
{
    class HandsOnLabsCell : UITableViewCell
    {
        BuiltHandsonLabs model;
        public UILabel  lblHandsOnLbasName ;
        private UILabel _lblTopTime;

		static nfloat contentViewX = 60;

		static nfloat lblLeftTimeXPadding = 22;
		static nfloat lblLeftTimeYPadding = 5;
		static nfloat lblLeftTimeWidthPadding = 100;
		static nfloat lblLeftTimeHeightPadding = 50;

		static nfloat lblTopTimeYPadding = 5;
		static nfloat lblTopTimeWidthPadding = 100;
		static nfloat lblTopTimeHeightPadding = 50;

		static nfloat lblBottomTimeYPadding = 20;
		static nfloat lblBottomTimeWidthPadding = 100;
		static nfloat lblBottomTimeHeightPadding = 50;

		static nfloat TrackColorYPadding = 0;
		static nfloat TrackColorWidthPadding = 1;

		static nfloat lblHOLNameXPadding = 20;
		static nfloat lblHOLNameYPadding = 2;
		static nfloat lblHOLNameWidthPadding = 410;
		static nfloat lblHOLNameHeightPadding = 0;

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
                        TextColor = AppTheme.HOLlblTopTime,
                        HighlightedTextColor = AppTheme.HOLlblTopTimeHighlighted,
                        Font = AppFonts.ProximaNovaRegular (14),
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                    };

                }


                return _lblTopTime;
            }

            set
            {

            }
        }

       
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
			
        public HandsOnLabsCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
           
            lblHandsOnLbasName = new UILabel
            {
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0,
                TextColor = AppTheme.ADcellTextColor,
                Font = AppFonts.ProximaNovaRegular (16),
                HighlightedTextColor = AppTheme.lblHandsOnLbasNameHighlighted,

            };
            
            ContentView.AddSubview(lblHandsOnLbasName);
            
            var selectedBackgroundView = new UIView();
			selectedBackgroundView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            SelectedBackgroundView = selectedBackgroundView;
        }

        public void UpdateCell(BuiltHandsonLabs builtHandsOnLabs)
        {
            model = builtHandsOnLabs;
			lblHandsOnLbasName.AttributedText = AppFonts.IncreaseLineHeight(model.title,lblHandsOnLbasName.Font,lblHandsOnLbasName.TextColor);
        }
               
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            lblHandsOnLbasName.Frame = new CGRect(lblHOLNameXPadding, 0, this.Frame.Width - (lblHOLNameXPadding * 2), ContentView.Frame.Height);
            
        }
        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (Selected)
            {
				ContentView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            }
        }

    }
}

