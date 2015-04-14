using System;
using CoreGraphics;

using Foundation;
using UIKit;
using SDWebImage;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
	public class LeftMenuCell : UITableViewCell
	{
        UILabel imgIcon, imgBadge;
		UILabel headingLabel, subheadingLabel, lblSurveyCount;
		public bool selected;
		public LeftMenuCell(NSString cellId)
			: base(UITableViewCellStyle.Default, cellId)
		{
			BackgroundColor = AppTheme.MenuNormalCell;
			selected = false;
			var selectedBackgroundView = new UIView();
			selectedBackgroundView.BackgroundColor = AppTheme.MenuHighlightedCell;
			SelectedBackgroundView = selectedBackgroundView;

			headingLabel = new UILabel()
			{
				TextColor = AppTheme.LMmenuNormalText,
				HighlightedTextColor = AppTheme.MenuHighlightedText,
				BackgroundColor = UIColor.Clear,
				Font = AppFonts.ProximaNovaSemiBold(16)
			};

			subheadingLabel = new UILabel()
			{
				TextColor = AppTheme.LMmenuNormalText,
				HighlightedTextColor = AppTheme.MenuHighlightedText,
				BackgroundColor = UIColor.Clear
			};

            lblSurveyCount = new UILabel()
            {
                TextColor = AppTheme.MenuHighlightedText, 
                HighlightedTextColor = AppTheme.MenuHighlightedText,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular(14),
                TextAlignment = UITextAlignment.Center
            };
			lblSurveyCount.Layer.CornerRadius = 3.0f;

            lblSurveyCount.Layer.BackgroundColor = AppTheme.MenuHighlightedCell.CGColor;

			imgIcon = new UILabel () {
				TextColor = AppTheme.LMmenuNormalText,
				HighlightedTextColor = AppTheme.MenuHighlightedText,
				BackgroundColor = UIColor.Clear,
				TextAlignment = UITextAlignment.Center,
				Font = AppTheme.LMIconFontSize,
			};


            imgBadge = new UILabel()
            {
                Text = AppTheme.LMBadgeIcon,
				TextColor = AppTheme.MenuBadgeColor,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Right,
				Font = AppTheme.LMBadgeFontSize,
            };

			ContentView.Add(imgIcon);
            ContentView.AddSubview(imgBadge);
			ContentView.Add(headingLabel);
			ContentView.Add(subheadingLabel);
            ContentView.Add(lblSurveyCount);
		}

		public void UpdateCell(string caption, string subtitle, string iconName, string iconCode)
		{
			headingLabel.Text = caption;
            headingLabel.AccessibilityLabel = caption;
			subheadingLabel.Text = subtitle;			 
			imgIcon.Text = FontAwesomeXamarin.FontAwesome.FontAwesomeIconStringForIconIdentifier(iconName);
            if (caption.ToLower().Contains("survey"))
                lblSurveyCount.Text = AppSettings.NewSurveyCount != 0 ? AppSettings.NewSurveyCount + " NEW" : String.Empty;
            else
                lblSurveyCount.Text = String.Empty;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (AppDelegate.instance().rootViewController.leftMenuOpened) {
                imgBadge.Hidden = true;
				headingLabel.Hidden = false;
				subheadingLabel.Hidden = false;
                lblSurveyCount.Hidden = false;
				imgIcon.Font = AppTheme.LMIconFontSize;

				imgIcon.Frame = new CGRect(10, (ContentView.Frame.Height / 2) - 12.5f, 25, 25);
				if (!String.IsNullOrEmpty(subheadingLabel.Text))
					headingLabel.Frame = new CGRect(imgIcon.Frame.Right + 15, 10, ContentView.Frame.Width - 50, 15);
				else
					headingLabel.Frame = new CGRect(imgIcon.Frame.Right + 15, 0, ContentView.Frame.Width - 50, ContentView.Frame.Height);

                if (!String.IsNullOrWhiteSpace(lblSurveyCount.Text))
                    lblSurveyCount.Frame = new CGRect(ContentView.Frame.Width - 70, Helper.getCenterY(ContentView.Frame.Height, 25), 60, 25); 
                else
                    lblSurveyCount.Frame = CGRect.Empty;

				var frame = headingLabel.Frame;
				frame.Y = headingLabel.Frame.Bottom + 5;
				subheadingLabel.Frame = frame;
			} else {
				imgIcon.Font = AppTheme.LMIconFontSize;
				imgIcon.Frame = new CGRect(Helper.getCenterX(ContentView.Frame.Width,25),Helper.getCenterY(ContentView.Frame.Height,25), 25, 25);
				headingLabel.Hidden = true;
				subheadingLabel.Hidden = true;
                lblSurveyCount.Hidden = true;

                if (!String.IsNullOrWhiteSpace(lblSurveyCount.Text))
                {
                    var frame = imgIcon.Frame;
                    frame.Y -= 10;
					frame.X += 5;
                    imgBadge.Hidden = false;
                    imgBadge.Frame = frame;
                }
                else
                {
                    imgBadge.Hidden = true;
                    imgBadge.Frame = CGRect.Empty;
                }
			}
			if (selected) {
				Selected = true;
			} else {
				Selected = false;
			}

		}

	}

}

