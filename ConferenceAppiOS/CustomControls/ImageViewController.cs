 using CoreGraphics;
using UIKit;
using Foundation;
using SDWebImage;
using System;

namespace ConferenceAppiOS
{
    public class ImageViewController : UIViewController
    {
        UIScrollView scrollView;
        UIImageView imageView;
        CGRect frame;
		string imageUrl; string _headertext; static nfloat crossImageHeight = 25; static nfloat crossImageWidth = 25;

        public ImageViewController(CGRect rect, string imageUrl)
            : base()
        {
            this.frame = rect;
            this.imageUrl = imageUrl;
        }
        public ImageViewController(CGRect rect, string imageUrl,string headerText)
        {
            this.frame = rect;
            this.imageUrl = imageUrl;
            this._headertext = headerText;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = frame;
            this.View.BackgroundColor = UIColor.White;

            this.Title = "Scroll View";

            nfloat topBarHeight = 50;
            UIView topBar = new UIView(new CGRect(0, 0, View.Frame.Width, topBarHeight))
            {
				BackgroundColor = AppTheme.IVHeadingBackground,
            };
           
			UIView tobBarBottonLine = new UIView (new CGRect (0, topBar.Frame.Height - 6, topBar.Frame.Width, 1));
			tobBarBottonLine.BackgroundColor = AppTheme.IVLineViewBackground;

            var closeButton = UIButton.FromType(UIButtonType.Custom);
			closeButton.BackgroundColor = UIColor.White;
            closeButton.ImageView.Frame = new CGRect(5, 5, 10, 10);
			closeButton.Layer.CornerRadius = 3.0f;
			closeButton.SetTitle(AppTheme.IVcrossImage, UIControlState.Normal);
			closeButton.SetTitle(AppTheme.IVcrossImage, UIControlState.Selected);
			closeButton.SetTitle(AppTheme.IVcrossImage, UIControlState.Highlighted);
			closeButton.SetTitleColor(AppTheme.IVcrossImageNormalColor, UIControlState.Normal);
			closeButton.SetTitleColor(AppTheme.IVcrossImageSelectedColor, UIControlState.Selected);
			closeButton.SetTitleColor(AppTheme.IVcrossImageHighlightedColor, UIControlState.Highlighted);
			closeButton.Font = AppTheme.IVcrossImageFont;
            closeButton.Frame = new CGRect(15,(topBarHeight/2)-12, crossImageWidth, crossImageHeight);
            closeButton.TouchUpInside += closeButtonClicked;

			var titleLabel = new UILabel(new CGRect(closeButton.Frame.Right+10, 0, View.Frame.Width-((closeButton.Frame.Width+10)*2), topBarHeight))
            {
                BackgroundColor = UIColor.Clear,
				TextColor = UIColor.Clear.FromHexString(AppTheme.TextColor,1.0f),
				Font = AppTheme.THVTitleLabelFont,
				Text = "Preview",
                TextAlignment = UITextAlignment.Center,
            };
			titleLabel.Center = topBar.Center;
            if(_headertext !=null)
            {
				titleLabel.Text = _headertext;
            }

			topBar.AddSubviews(tobBarBottonLine,titleLabel, closeButton);

            scrollView = new UIScrollView(new CGRect(0, topBar.Frame.Bottom, View.Frame.Width, View.Frame.Height - topBarHeight));
            View.AddSubviews(topBar, scrollView);

            imageView = new UIImageView(new CGRect(0, 0, scrollView.Frame.Width, scrollView.Frame.Height))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
			imageView.SetImage(NSUrl.FromString(imageUrl));
            scrollView.ContentSize = new CGSize(View.Frame.Width, View.Frame.Width - topBarHeight);
            scrollView.AddSubview(imageView);
            scrollView.MaximumZoomScale = 3f;
            scrollView.MinimumZoomScale = 1f;
            scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { return imageView; };

            UITapGestureRecognizer doubletap = new UITapGestureRecognizer(OnDoubleTap)
            {
                NumberOfTapsRequired = 2
            };
            scrollView.AddGestureRecognizer(doubletap);
        }

        void closeButtonClicked(object sender, EventArgs e)
        {
            AppDelegate.instance().rootViewController.closeDialogue();
        }

        private void OnDoubleTap(UIGestureRecognizer gesture)
        {
            if (scrollView.ZoomScale > 1)
                scrollView.SetZoomScale(1f, true);
            else
                scrollView.SetZoomScale(2f, true);
        }
    }
}