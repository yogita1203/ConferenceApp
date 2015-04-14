using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ConferenceAppiOS
{
	public partial class TabbedCarousel : UIViewController
	{
		UIScrollView _tabbedScrollView;
		UIScrollView tabbedScrollView{
			get{
				if (_tabbedScrollView == null) {
					_tabbedScrollView = new UIScrollView ();
					View.AddSubview (_tabbedScrollView);
				}
				return _tabbedScrollView;
			}
		}

		UIScrollView _baseScrollView;
		UIScrollView baseScrollView{
			get{
				if (_baseScrollView == null) {
					_baseScrollView = new UIScrollView ();
					View.AddSubview (_baseScrollView);
				}
				return _baseScrollView;
			}
		}


		public TabbedCarousel ()

		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

		}
	}
}

