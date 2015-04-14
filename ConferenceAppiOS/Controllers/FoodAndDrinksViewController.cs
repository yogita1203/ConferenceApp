using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CommonLayer;

namespace ConferenceAppiOS
{
	public class FoodAndDrinksViewController : BaseViewController
    {
        #region--Views--
        TitleHeaderView headerView;
        UITabBarController tabBarController;
        CGRect frame;
        WebViewController foodAndDrinkWebview;
        UIView horizontalLine, verticalLine;
        #endregion

        #region--Constants--
        string TitleName = AppTheme.FDtitleTextSanFrancisco;
		static nfloat headerHeight = 64;
		static nfloat TabBarControllerWidth = 378.0f;

		static nfloat headerViewXPadding = 0;
		static nfloat headerViewYPadding = 0;

		static nfloat tabBarControllerXPadding = 0;

		static nfloat vc1XPadding = 0;
		static nfloat vc1YPadding = 0;

		static nfloat vc2XPadding = 0;
		static nfloat vc2YPadding = 0;
        #endregion

        public FoodAndDrinksViewController (CGRect rect) 
		{
			frame = new CGRect(0,0,rect.Width,rect.Height);
			View.Frame = frame;
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.Frame = frame;
			foodAndDrinkWebview = new WebViewController (string.Empty);

			headerView = new TitleHeaderView (TitleName, true, false, false, false, false,false,false,false);
			View.AddSubview (headerView);

			tabBarController = new UITabBarController ();
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, headerHeight);
            tabBarController.View.Frame = new CGRect(tabBarControllerXPadding, headerHeight, View.Frame.Width / 2, View.Frame.Height - headerHeight);
            
            var vc1 = new FoodAndDrinkController(new CGRect(vc1XPadding, vc1YPadding, tabBarController.View.Frame.Width, tabBarController.View.Frame.Height));
			vc1.linkCliked = (str) => {
				foodAndDrinkWebview.loadRequest(str);
			};

            var vc2 = new TransportationController(new CGRect(vc2XPadding, vc2YPadding, tabBarController.View.Frame.Width, tabBarController.View.Frame.Height));
			vc2.transportLinkCliked = (str) => {
				foodAndDrinkWebview.loadRequest(str);
			};

			tabBarController.ViewControllers = new UIViewController[] {vc2,vc1};
           
			tabBarController.ViewControllerSelected += (s,e) => {
				UITabBarController controller = (UITabBarController)s; 
				if(controller.SelectedIndex == 1)
					vc2.selectRowForTransportationTable();
			};
			View.AddSubview (tabBarController.View);

			horizontalLine = new LineView (CGRect.Empty);

			verticalLine = new LineView (CGRect.Empty);

			View.AddSubviews (horizontalLine, verticalLine);
			View.AddSubview (foodAndDrinkWebview.View);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, headerHeight);
            var item = tabBarController.TabBar.Subviews;
            item[0].Frame = new CGRect(0,0,0,0);
            item[1].Frame = new CGRect(0,0,0,0);
            tabBarController.View.Frame = new CGRect(tabBarControllerXPadding, headerHeight, TabBarControllerWidth, View.Frame.Height - headerHeight);
            horizontalLine.Frame = new CGRect(tabBarController.View.Frame.Width, headerHeight, View.Frame.Width - (tabBarController.View.Frame.Width + 4), AppTheme.FDseparatorBorderWidth);
            verticalLine.Frame = new CGRect(tabBarController.View.Frame.Width, headerHeight, AppTheme.FDseparatorBorderWidth, tabBarController.View.Frame.Height);
            foodAndDrinkWebview.View.Frame = new CGRect(tabBarController.View.Frame.Width + AppTheme.FDseparatorBorderWidth, headerHeight + AppTheme.FDseparatorBorderWidth, View.Frame.Width - (tabBarController.View.Frame.Width + AppTheme.FDseparatorBorderWidth), tabBarController.View.Frame.Height);
		}


	}
}

