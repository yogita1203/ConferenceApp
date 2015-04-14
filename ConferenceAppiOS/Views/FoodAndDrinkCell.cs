using System;
using CoreGraphics;
using UIKit;
using Foundation;
using CommonLayer.Entities.Built;
using SDWebImage;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public class FoodAndDrinkCell : UITableViewCell
    {
        public bool cellForFoodAndDrinks;
		static nfloat imageViewX = 20;
		static nfloat imageViewY = 10;
		static nfloat imageViewWidth = 50;
		static nfloat imageViewHeight = 50;

		static nfloat titleLabelY = 10;
		static nfloat titleLabelHeight = 20;

		static nfloat marginBetweenViews = 10;

        BuiltSFFoodNDrink builtSFFoodNDrink;
        BuiltTransportation builtSFTransportation;
        UILabel _titleLabel;
        UILabel titleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel
                    {
						TextColor = AppTheme.FDTitleTextFontColor,
                        Font = AppTheme.FDTitleTextFont,
                        HighlightedTextColor = UIColor.White,
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                    };
                    ContentView.AddSubview(_titleLabel);
                }
                return _titleLabel;
            }
        }

        UILabel _detailLabel;
        UILabel detailLabel
        {
            get
            {
                if (_detailLabel == null)
                {
                    _detailLabel = new UILabel
                    {
						TextColor = AppTheme.FDDetailTextFontColor,
                        Font = AppTheme.FDDetailTextFont,
                        Lines = 0,
                        LineBreakMode = UILineBreakMode.WordWrap,
                        HighlightedTextColor = UIColor.White,
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                    };
                    ContentView.AddSubview(_detailLabel);
                }
                return _detailLabel;
            }
        }

        UIImageView _iconImageView;
        UIImageView iconImageView
        {
            get
            {
                if (_iconImageView == null)
                {
                    _iconImageView = new UIImageView();
                    _iconImageView.Layer.BorderWidth = 1.0f;
					_iconImageView.Layer.BorderColor = UIColor.Clear.FromHexString(AppTheme.LineColor,1.0f).CGColor;
                    _iconImageView.BackgroundColor = AppTheme.iconImageView;
                    ContentView.AddSubview(_iconImageView);
                }
                return _iconImageView;
            }
        }


        public FoodAndDrinkCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {

        }

        public void updateTranspotationCell(BuiltTransportation transportation)
        {
            builtSFTransportation = transportation;
			titleLabel.AttributedText = AppFonts.IncreaseLineHeight (builtSFTransportation.name, titleLabel.Font, titleLabel.TextColor);
			detailLabel.AttributedText = AppFonts.IncreaseLineHeight (builtSFTransportation.short_desc, detailLabel.Font, detailLabel.TextColor);
            if (builtSFTransportation.icon != null)
            {
                iconImageView.SetImage(NSUrl.FromString(builtSFTransportation.icon.url));
            }
        }

        public void updateCell(BuiltSFFoodNDrink foodAndDrinkModel)
        {

            builtSFFoodNDrink = foodAndDrinkModel;
			titleLabel.AttributedText = AppFonts.IncreaseLineHeight (builtSFFoodNDrink.venue_name, titleLabel.Font, titleLabel.TextColor);
			detailLabel.AttributedText = AppFonts.IncreaseLineHeight (builtSFFoodNDrink.address, detailLabel.Font, detailLabel.TextColor);

            if (builtSFFoodNDrink.icon != null)
            {
                iconImageView.SetImage(NSUrl.FromString(builtSFFoodNDrink.icon.url));
            }
			
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            iconImageView.Frame = new CGRect(imageViewX, imageViewY, imageViewWidth, imageViewHeight);
            titleLabel.Frame = new CGRect(iconImageView.Frame.Right + marginBetweenViews, titleLabelY, ContentView.Frame.Width - (imageViewWidth + imageViewX + marginBetweenViews*2), titleLabelHeight);
			detailLabel.Frame = new CGRect(titleLabel.Frame.X, titleLabel.Frame.Bottom+(titleLabelY/2), titleLabel.Frame.Width, 0);
            var h = Helper.getTextHeight(detailLabel.Text, detailLabel.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, detailLabel.Font, detailLabel);
            var rect = detailLabel.Frame;
            rect.Height = h;
            detailLabel.Frame = rect;
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

			iconImageView.Image = null;
        }
    }
}

