using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace ConferenceAppiOS
{
	public class CustomWebViewController : UIViewController
	{
		UIWebView webView;
		UIToolbar navBar;
		string urlString;
		static nfloat toolBarHeight = 60;
		UIBarButtonItem [] items;
		UIActivityIndicatorView indicator;

		public CustomWebViewController (string url)
		{
			urlString = url;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			navBar = new UIToolbar ();
			navBar.Frame = new CGRect (0, View.Frame.Height-toolBarHeight, View.Frame.Width, toolBarHeight);
			navBar.TintColor = UIColor.DarkGray;			
			CGRect rect = View.Frame;

			items = new UIBarButtonItem [] {
				new UIBarButtonItem (new UIImage("back.png"), UIBarButtonItemStyle.Bordered, (o, e) => { webView.GoBack (); }),
				new UIBarButtonItem (new UIImage("fwd.png"), UIBarButtonItemStyle.Bordered, (o, e) => { webView.GoForward (); }),
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace, null),
				new UIBarButtonItem (UIBarButtonSystemItem.Refresh, (o, e) => { webView.Reload (); }),
				new UIBarButtonItem (UIBarButtonSystemItem.Stop, (o, e) => { 
					webView.StopLoading (); 
					// Phone: NavigationController, Pad: Modal
					if (NavigationController == null){
						DismissViewController (true, ()=> {
							View.Frame = rect;
						});
					}else{
						NavigationController.PopViewController (true);
					}
				})
			};
			navBar.Items = items;

			indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

			webView = new UIWebView ();
			webView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height-toolBarHeight);
			webView.LoadStarted += delegate {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				navBar.Items[0].Enabled = webView.CanGoBack;
				navBar.Items[1].Enabled = webView.CanGoForward;
			};

			webView.ScalesPageToFit = true;
			webView.ContentMode = UIViewContentMode.ScaleAspectFit;

			webView.LoadStarted += (sender,e) => {
				indicator.StartAnimating();
			};

			webView.LoadFinished += (sender,e) => {
				//				CGRect frame = webView.Frame;
				//				frame.Size = new SizeF(frame.Size.Width,1);
				//				webView.Frame = frame;
				//				SizeF fittingSize = webView.SizeThatFits(new SizeF(frame.Size.Width,0));//[aWebView sizeThatFits:CGSizeZero];
				//				frame.Size = new SizeF(frame.Size.Width,fittingSize.Height);
				//				webView.Frame = frame;
				indicator.StopAnimating();
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				navBar.Items[0].Enabled = webView.CanGoBack;
				navBar.Items[1].Enabled = webView.CanGoForward;


				//test
				//var contentSize = webView.ScrollView.ContentSize;
				//var viewSize = View.Bounds.Size;
				//nfloat rw = viewSize.Width / contentSize.Width;
				//webView.ScrollView.MinimumZoomScale = rw;
				//webView.ScrollView.MaximumZoomScale = rw;
				//webView.ScrollView.ZoomScale = rw;

				var newBounds = webView.Bounds;
				newBounds.Height = webView.ScrollView.ContentSize.Height;
				newBounds.Width = webView.ScrollView.ContentSize.Width;
				webView.Bounds = newBounds;
				//
			};

			webView.LoadError += (sender, e) => {
				indicator.StopAnimating();
//				Console.WriteLine(e.Error.ToString()); 
			};

			webView.ScalesPageToFit = true;
			//webView.SizeToFit();
			if (urlString.Length > 0) {
				loadRequest (urlString);
			}
			navBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			webView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			View.AddSubview (webView);
			View.AddSubview (navBar);
			View.AddSubview(indicator);

		}

		public void loadRequest(string urlString){
			NSUrl url = NSUrl.FromString(urlString);
			NSMutableUrlRequest request = new NSMutableUrlRequest (url);
			webView.LoadRequest(request);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			webView.Frame = new CGRect (0, 0, View.Frame.Width, View.Frame.Height-toolBarHeight);
			indicator.Center = new CGPoint (webView.Frame.Width/2,(webView.Frame.Height-toolBarHeight)/2);
		}
	}
}

