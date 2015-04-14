using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreGraphics;

namespace ConferenceAppiOS
{
    public class LoadingOverlay : UIView
    {
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;
        public LoadingOverlay(CGRect frame, bool largeSize = false, bool showText = false)
            : base(frame)
        {
            nfloat labelHeight = 25;
            nfloat labelWidth = Frame.Width - 20;
			nfloat toppaddingForSpinner = 40;
            BackgroundColor = UIColor.Clear;
            AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;   

			if (showText)
			{
				// create and configure the "Loading Data" label
				loadingLabel = new UILabel();
				loadingLabel.Frame = new CGRect (
					centerX - (labelWidth / 2),
					centerY - 30,
					labelWidth,
					labelHeight
				);
				loadingLabel.BackgroundColor = UIColor.Clear;
				loadingLabel.TextColor = UIColor.Clear.FromHexString(AppTheme.TextColor,1.0f);
				loadingLabel.Text = "Loading...";
				loadingLabel.TextAlignment = UITextAlignment.Center;
				loadingLabel.Font = AppFonts.ProximaNovaRegular (25);
				loadingLabel.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				AddSubview(loadingLabel);
			}

			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			activitySpinner.Transform = CGAffineTransform.MakeScale(1.80f,1.80f); 
			activitySpinner.Frame = new CGRect(
				centerX - (activitySpinner.Frame.Width / 2),
				centerY + toppaddingForSpinner,
				activitySpinner.Frame.Width,
				activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            
        }
        public void Hide()
        {
            UIView.Animate(
                0.5,
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
}