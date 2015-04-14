using System;
using CoreGraphics;
using UIKit;
using Foundation;
using System.IO;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class WebViewController : UIViewController
    {
		static nfloat crossImageHeight = 25; 
		static nfloat crossImageWidth = 25;
		static nfloat topBarHeight = 50;
        public UIWebView webView;
        string urlString;
        UIActivityIndicatorView indicator;
        static bool showTopBar;
        CGRect bounds;
        string title;

        public WebViewController(string url)
        {
            urlString = url;
            showTopBar = false;
        }

        public WebViewController(string url, bool _showTopBar, CGRect frame, string title)
        {
            urlString = url;
            showTopBar = _showTopBar;
            bounds = frame;
            this.title = title;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (showTopBar)
                setTopBar();

            CGRect rect = View.Frame;

            indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

            webView = new UIWebView();

            if (showTopBar)
                webView.Frame = new CGRect(0, topBarHeight, View.Frame.Width, View.Frame.Height - topBarHeight);
            else
                webView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);


			webView.ScrollView.Bounces = false;

            webView.LoadStarted += delegate
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
            };

            webView.ScalesPageToFit = true;
            webView.ContentMode = UIViewContentMode.ScaleAspectFit;

            webView.LoadStarted += (sender, e) =>
            {
                indicator.StartAnimating();
            };

            webView.LoadFinished += (sender, e) =>
            {
                indicator.StopAnimating();
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
            };

            webView.LoadError += (sender, e) =>
            {
                indicator.StopAnimating();
//                Console.WriteLine(e.Error.ToString());
            };

            webView.ScalesPageToFit = true;
            if (urlString.Length > 0)
            {
                loadRequest(urlString);
            }
            webView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            View.AddSubview(webView);
            View.AddSubview(indicator);
        }

        private void setTopBar()
        {
            View.BackgroundColor = UIColor.Gray;
            UIView topBar = new UIView(new CGRect(0, 0, bounds.Width, topBarHeight))
            {
                BackgroundColor = AppTheme.IVHeadingBackground,
            };

            UIView tobBarBottonLine = new UIView(new CGRect(0, topBar.Frame.Height - 1, topBar.Frame.Width, 1));
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
            closeButton.Frame = new CGRect(15, (topBarHeight / 2) - 12, crossImageWidth, crossImageHeight);
            closeButton.TouchUpInside += (s, e) => 
            {
                AppDelegate.instance().rootViewController.closeDialogue();
            };


            var titleLabel = new UILabel(new CGRect(closeButton.Frame.Right, 0, View.Frame.Width, topBarHeight))
            {
                BackgroundColor = UIColor.Clear,
				TextColor = UIColor.Clear.FromHexString(AppTheme.TextColor,1.0f),
                Font = AppTheme.THVTitleLabelFont,
                Text = this.title,
                TextAlignment = UITextAlignment.Center,
            };
            titleLabel.Center = topBar.Center;
       
            topBar.AddSubviews(tobBarBottonLine, titleLabel, closeButton);
            View.AddSubview(topBar);
        }

        public void loadRequest(string urlString)
        {
            if (String.IsNullOrWhiteSpace(urlString))
                return;
            NSUrl url = NSUrl.FromString(urlString.Trim());
            webView.LoadRequest(new NSUrlRequest(new NSUrl("about:blank")));
            webView.Reload();
            webView.LoadRequest(new NSUrlRequest(url));
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            if (showTopBar)
                webView.Frame = new CGRect(0, topBarHeight, View.Frame.Width, View.Frame.Height - topBarHeight);
            else
                webView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            indicator.Center = new CGPoint(webView.Frame.Width / 2, (webView.Frame.Height) / 2);
        }
    }
}

