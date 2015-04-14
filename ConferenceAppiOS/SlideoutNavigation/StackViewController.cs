using System;
using CoreGraphics;
using Foundation;
using UIKit;
using ObjCRuntime;
using CoreAnimation;
using System.Collections.Generic;

namespace ConferenceAppiOS
{
	public partial class StackViewController : UIViewController
	{

		UILabel refreshLabel;
		NavigationBar navigationView;
		nfloat navigationHeight = 0.0f;
		public UIView slideViews;
		public UIView borderViews;

		private UIView viewAtLeft;
		private UIView viewAtRight;
		private UIView viewAtLeft2;
		private UIView viewAtRight2;	
		private UIView viewAtRightAtTouchBegan;
		private UIView viewAtLeftAtTouchBegan;

		public List<UIViewController> viewControllersStack;

		private NSString dragDirection;

		nfloat viewXPosition;		
		nfloat displacementPosition;
		nfloat lastTouchPoint;
		nfloat difference;
		public nfloat slideStartPosition;

		private CGPoint positionOfViewAtRightAtTouchBegan;
		private CGPoint positionOfViewAtLeftAtTouchBegan;

		const int SLIDE_VIEWS_MINUS_X_POSITION = -130;
		const int SLIDE_VIEWS_START_X_POS = 0;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public StackViewController (bool navigationbar)
		{
			if (navigationbar) {
				navigationHeight = 66.0f;

				refreshLabel = new UILabel ();
				refreshLabel.Font = AppFonts.ProximaNovaRegular (14);
				refreshLabel.TextAlignment = UITextAlignment.Center;
				refreshLabel.TextColor = UIColor.White;
				refreshLabel.Frame = new CGRect(0,0,View.Frame.Size.Width,navigationHeight);
				View.AddSubview (refreshLabel);

				navigationView = new NavigationBar (new CGRect(0,0,View.Frame.Size.Width,navigationHeight));
				navigationView.BackgroundColor = UIColor.Gray;
				navigationView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				View.AddSubview (navigationView);
				navigationView.rightButton.TouchUpInside += RightButtonClicked;
				navigationView.leftButton.TouchUpInside += LeftButtonClicked;
			}

			viewControllersStack = new List<UIViewController> ();
			borderViews = new UIView (new CGRect (SLIDE_VIEWS_MINUS_X_POSITION - 2, -2, 2, this.View.Frame.Size.Height));
			borderViews.BackgroundColor = UIColor.Clear;

			UIView verticalLineView1 = new UIView (new CGRect (0, 0, 1, borderViews.Frame.Size.Height));
			verticalLineView1.BackgroundColor = UIColor.White;
			verticalLineView1.Tag = 1;
			verticalLineView1.Hidden = true;
			borderViews.Add (verticalLineView1);

			UIView verticalLineView2 = new UIView (new CGRect (0, 0, 2, borderViews.Frame.Size.Height));
			verticalLineView2.BackgroundColor = UIColor.Gray;
			verticalLineView2.Tag = 2;
			verticalLineView2.Hidden = true;
			borderViews.AddSubview (verticalLineView2);

			View.AddSubview (borderViews);

			slideViews = new UIView (new CGRect (0, 0, View.Frame.Size.Width, View.Frame.Size.Height));
			slideViews.BackgroundColor = UIColor.Clear;
			View.BackgroundColor = UIColor.LightTextColor;
			View.Frame = slideViews.Frame;

			View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;

			viewXPosition = 0;
			lastTouchPoint = -1;


			dragDirection = new NSString ("");

			viewAtLeft2=null;
			viewAtLeft=null;
			viewAtRight=null;
			viewAtRight2=null;
			viewAtRightAtTouchBegan = null;

			UIPanGestureRecognizer panRecognizer = new UIPanGestureRecognizer ();

			panRecognizer.AddTarget (() => handlePanFrom (panRecognizer));
			panRecognizer.MaximumNumberOfTouches = 1;
			panRecognizer.DelaysTouchesBegan = true;
			panRecognizer.DelaysTouchesEnded = true;
			panRecognizer.CancelsTouchesInView = true;
			View.GestureRecognizers = new UIGestureRecognizer[]{ panRecognizer };

			View.AddSubview (slideViews);

		}

		void RightButtonClicked (object sender, EventArgs ea) {
//			Console.WriteLine ("RightButtonClicked");
		}

		void LeftButtonClicked (object sender, EventArgs ea) {
			if (refreshLabel.Frame.Y == 0) {
				
			} else {
			
			}
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		[Obsolete ("Deprecated in iOS 6.0")]
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			for (int index = 0 ; index < viewControllersStack.Count; index++) {
				UIViewController subController = viewControllersStack [index];
				if(subController !=  null)
					subController.ViewDidLoad ();
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.White;
		}

		[Obsolete ("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation")]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			bool isViewOutOfScreen = false; 
			int viewControllersStackCount = (viewControllersStack == null) ? 0 : viewControllersStack.Count; 
			for (int index = 0; index < viewControllersStackCount; index++) {
				UIViewController subController = viewControllersStack [index];
				if (viewAtRight != null && viewAtRight.Equals(subController.View)) {
					if (viewAtRight.Frame.X <= ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width))) {
						subController.View.Frame = new CGRect(View.Frame.Size.Width - subController.View.Frame.Size.Width, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
					}else{
						subController.View.Frame = new CGRect (((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width), (subController == null) ? 0 : subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);				
					}
					isViewOutOfScreen = true;
				}
				else if (viewAtLeft != null && viewAtLeft.Equals(subController.View)) {
					if (viewAtLeft2 == null) {
						if(viewAtRight == null){	
							subController.View.Frame = new CGRect (SLIDE_VIEWS_START_X_POS, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
						}else{
							subController.View.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
							if (viewAtRight != null) 
								viewAtRight.Frame = new CGRect(SLIDE_VIEWS_MINUS_X_POSITION + subController.View.Frame.Size.Width, ((viewAtRight == null) ? 0 : viewAtRight.Frame.Y), ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width), ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Height));
						}
					}
					else if (((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION || ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) == SLIDE_VIEWS_START_X_POS) {
						subController.View.Frame = new CGRect(subController.View.Frame.X, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
					}
					else {
						if ((((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)) == View.Frame.Size.Width) {
							subController.View.Frame = new CGRect(View.Frame.Size.Width - subController.View.Frame.Size.Width, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
						}else{
							subController.View.Frame = new CGRect(((viewAtLeft2 == null) ? 0 : viewAtLeft2.Frame.X) + ((viewAtLeft2 == null) ? 0 : viewAtLeft2.Frame.Size.Width), subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
						}
					}
				}
				else if(!isViewOutOfScreen){
					subController.View.Frame = new CGRect(subController.View.Frame.X, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
				}
				else {
					subController.View.Frame = new CGRect(View.Frame.Size.Width, subController.View.Frame.Y, subController.View.Frame.Size.Width, View.Frame.Size.Height);
				}

			}
			foreach (UIViewController subController in viewControllersStack) {
				subController.WillAnimateRotation(toInterfaceOrientation,duration);
				if (!((viewAtRight != null && viewAtRight.Equals(subController.View) || (viewAtLeft != null && viewAtLeft.Equals(subController.View)) || (viewAtLeft2 != null && viewAtLeft2.Equals(subController.View))))) {
					subController.View.Hidden = true;
				}

			}   

		}


		public void didRotateFromInterfaceOrientation(UIInterfaceOrientation fromInterfaceOrientation) {	
			foreach (UIViewController subController in viewControllersStack) {
				subController.DidRotate(fromInterfaceOrientation);
			}
			if (viewAtLeft != null) {
				viewAtLeft.Hidden = false;
			}
			if (viewAtRight != null) {
				viewAtRight.Hidden = false;
			}	
			if (viewAtLeft2 != null) {
				viewAtLeft2.Hidden =  false;
			}	
		}


		[Export ("bounceBack:finished:context:")]
		public void bounceBack(string animationID, NSNumber finished, IntPtr context) {	

			bool isBouncing = false;

			if(dragDirection.Equals("") && finished.BoolValue){
				if(viewAtLeft != null)
					viewAtLeft.Layer.RemoveAllAnimations ();
				if(viewAtRight != null)
					viewAtRight.Layer.RemoveAllAnimations ();
				if(viewAtRight2 != null)
					viewAtRight2.Layer.RemoveAllAnimations ();
				if(viewAtLeft2 != null)
					viewAtLeft2.Layer.RemoveAllAnimations ();

				if (animationID.Equals(new NSString("LEFT-WITH-LEFT")) && ((viewAtLeft2 == null) ? 0.0f : viewAtLeft2.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION) {
					CABasicAnimation bounceAnimation = CABasicAnimation.FromKeyPath ("position.x");
					bounceAnimation.Duration = 0.2;
					bounceAnimation.From = NSNumber.FromNFloat ((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X);
					bounceAnimation.To = NSNumber.FromNFloat ((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X - 10);
					bounceAnimation.RepeatCount = 0;
					bounceAnimation.AutoReverses = true;
					bounceAnimation.FillMode = "kCAFillModeBackwards";
					bounceAnimation.RemovedOnCompletion = true;
					bounceAnimation.Additive = false;
					if(viewAtLeft != null)
						viewAtLeft.Layer.AddAnimation (bounceAnimation, "bounceAnimation");
					if(viewAtRight != null)
						viewAtRight.Hidden = false;

					CABasicAnimation bounceAnimationForRight = CABasicAnimation.FromKeyPath ("position.x");
					bounceAnimationForRight.Duration = 0.2;
					bounceAnimationForRight.From = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X);
					bounceAnimationForRight.To = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X - 20);
					bounceAnimationForRight.RepeatCount = 0;
					bounceAnimationForRight.AutoReverses = true;
					bounceAnimationForRight.FillMode = "kCAFillModeBackwards";
					bounceAnimationForRight.RemovedOnCompletion = true;
					bounceAnimationForRight.Additive = false;
					if(viewAtRight != null)
						viewAtRight.Layer.AddAnimation (bounceAnimationForRight, "bounceAnimationRight");

				}else if (animationID.Equals(new NSString("LEFT-WITH-RIGHT"))  && (((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION)) {
                   
					CABasicAnimation bounceAnimation = CABasicAnimation.FromKeyPath("position.x");
					bounceAnimation.Duration = 0.2;
					bounceAnimation.From = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X);
					bounceAnimation.To = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X - 10);
					bounceAnimation.RepeatCount = 0;
					bounceAnimation.AutoReverses = true;
					bounceAnimation.FillMode = "kCAFillModeBackwards";
					bounceAnimation.RemovedOnCompletion = true;
					bounceAnimation.Additive = false;
					if(viewAtRight != null)
						viewAtRight.Layer.AddAnimation(bounceAnimation,"bounceAnimation");
					if(viewAtRight2 != null)
						viewAtRight2.Hidden = false;

					CABasicAnimation bounceAnimationForRight2 = CABasicAnimation.FromKeyPath("position.x");
					bounceAnimationForRight2.Duration = 0.2;
					bounceAnimationForRight2.From = NSNumber.FromNFloat((viewAtRight2 == null)? 0.0f : viewAtRight2.Center.X);
					bounceAnimationForRight2.To = NSNumber.FromNFloat((viewAtRight2 == null)? 0.0f : viewAtRight2.Center.X - 20);
					bounceAnimationForRight2.RepeatCount = 0;
					bounceAnimationForRight2.AutoReverses = true;
					bounceAnimationForRight2.FillMode = "kCAFillModeBackwards";
					bounceAnimationForRight2.RemovedOnCompletion = true;
					bounceAnimationForRight2.Additive = false;
					if(viewAtRight2 != null)
						viewAtRight2.Layer.AddAnimation (bounceAnimationForRight2, "bounceAnimationRight2");

				}else if (animationID.Equals(new NSString("RIGHT-WITH-RIGHT"))) {
					CABasicAnimation bounceAnimationLeft = CABasicAnimation.FromKeyPath("position.x");
					bounceAnimationLeft.Duration = 0.2;
					bounceAnimationLeft.From = NSNumber.FromNFloat((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X);
					bounceAnimationLeft.To = NSNumber.FromNFloat((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X + 10);
					bounceAnimationLeft.RepeatCount = 0;
					bounceAnimationLeft.AutoReverses = true;
					bounceAnimationLeft.FillMode = "kCAFillModeBackwards";
					bounceAnimationLeft.RemovedOnCompletion = true;
					bounceAnimationLeft.Additive = false;
					if(viewAtLeft != null)
						viewAtLeft.Layer.AddAnimation(bounceAnimationLeft, "bounceAnimationLeft");

					CABasicAnimation bounceAnimationRight = CABasicAnimation.FromKeyPath("position.x");
					bounceAnimationRight.Duration = 0.2;
					bounceAnimationRight.From = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X);
					bounceAnimationRight.To = NSNumber.FromNFloat((viewAtRight == null)? 0.0f : viewAtRight.Center.X + 10);
					bounceAnimationRight.RepeatCount = 0;
					bounceAnimationRight.AutoReverses = true;
					bounceAnimationRight.FillMode = "kCAFillModeBackwards";
					bounceAnimationRight.RemovedOnCompletion = true;
					bounceAnimationRight.Additive = false;
					if(viewAtRight != null)
						viewAtRight.Layer.AddAnimation(bounceAnimationRight,"bounceAnimationRight");

				}else if (animationID.Equals("RIGHT-WITH-LEFT")) {
					CABasicAnimation bounceAnimationLeft = CABasicAnimation.FromKeyPath("position.x");
					bounceAnimationLeft.Duration = 0.2;
					bounceAnimationLeft.From = NSNumber.FromNFloat((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X);
					bounceAnimationLeft.To = NSNumber.FromNFloat((viewAtLeft == null)? 0.0f : viewAtLeft.Center.X + 10);
					bounceAnimationLeft.RepeatCount = 0;
					bounceAnimationLeft.AutoReverses = true;
					bounceAnimationLeft.FillMode = "kCAFillModeBackwards";
					bounceAnimationLeft.RemovedOnCompletion = true;
					bounceAnimationLeft.Additive = false;
					if(viewAtLeft != null)
						viewAtLeft.Layer.AddAnimation(bounceAnimationLeft, "bounceAnimationLeft");

					if (viewAtLeft2 != null) {
						viewAtLeft2.Hidden = false;
						int viewAtLeft2Position = Array.IndexOf(slideViews.Subviews,viewAtLeft2);
						if (viewAtLeft2Position > 0) {
							UIView vw = slideViews.Subviews.GetValue(viewAtLeft2Position -1) as UIView;
							vw.Hidden = false;
						}
						CABasicAnimation bounceAnimationLeft2 = CABasicAnimation.FromKeyPath("position.x");
						bounceAnimationLeft2.Duration = 0.2;
						bounceAnimationLeft2.From = NSNumber.FromNFloat(viewAtLeft2.Center.X);
						bounceAnimationLeft2.To = NSNumber.FromNFloat(viewAtLeft2.Center.X + 10);
						bounceAnimationLeft2.RepeatCount = 0;
						bounceAnimationLeft2.AutoReverses = true;
						bounceAnimationLeft2.FillMode = "kCAFillModeBackwards";
						bounceAnimationLeft2.RemovedOnCompletion = true;
						bounceAnimationLeft2.Additive = false;
						if(viewAtLeft2 != null)
							viewAtLeft2.Layer.AddAnimation (bounceAnimationLeft2, "bounceAnimationviewAtLeft2");
						this.PerformSelector (new Selector ("callArrangeVerticalBar"), null, 0.4);
						isBouncing = true;
					}

				}

			}
			arrangeVerticalBar();	
			if (Array.IndexOf(slideViews.Subviews,viewAtLeft2) == 1 && isBouncing) {
				if(borderViews.ViewWithTag (2) != null)
					borderViews.ViewWithTag (2).Hidden = true;
			}
		}


		[Export ("callArrangeVerticalBar")]
		public void callArrangeVerticalBar(){
			arrangeVerticalBar ();
		}

		public void addViewInSlider(UIViewController controller, UIViewController invokeByController, bool isStackStartView){

			if (isStackStartView) {

				slideStartPosition = SLIDE_VIEWS_START_X_POS;
				viewXPosition = slideStartPosition;

				foreach (UIView subview in slideViews.Subviews) {
					subview.RemoveFromSuperview ();
				}
				if(borderViews.ViewWithTag (3) != null)
					borderViews.ViewWithTag (3).Hidden = true;
				if(borderViews.ViewWithTag (2) != null)
					borderViews.ViewWithTag (2).Hidden = true;
				if(borderViews.ViewWithTag (1) != null)
					borderViews.ViewWithTag (1).Hidden = true;
				viewControllersStack.Clear();
			}


			if(viewControllersStack.Count > 1){
				int indexOfViewController = viewControllersStack.IndexOf(invokeByController)+1;

				if (invokeByController.ParentViewController != null) { // bool check by null //..
					indexOfViewController = viewControllersStack.IndexOf(invokeByController.ParentViewController)+1;
				}

				int viewControllerCount = (int)viewControllersStack.Count;
				for (int i = indexOfViewController; i < viewControllerCount; i++) {
					UIView vw1 = slideViews.ViewWithTag (i) as UIView;
					vw1.RemoveFromSuperview ();
					viewControllersStack.RemoveAt (indexOfViewController);
					viewXPosition = View.Frame.Size.Width - controller.View.Frame.Size.Width;
				}
			}else if(viewControllersStack.Count == 0) {
				foreach (UIView subview in slideViews.Subviews) {
					subview.RemoveFromSuperview ();
				}	
				viewControllersStack.Clear ();
				if(borderViews.ViewWithTag (3) != null)
					borderViews.ViewWithTag (3).Hidden = true;
				if(borderViews.ViewWithTag (2) != null)
					borderViews.ViewWithTag (2).Hidden = true;
				if(borderViews.ViewWithTag (1) != null)
					borderViews.ViewWithTag (1).Hidden = true;
			}

			if (slideViews.Subviews.Length != 0) {
				UIViewWithShadow verticalLineView = new UIViewWithShadow (UIViewWithShadow.ShadowSide.left);
				verticalLineView.Frame = new CGRect (-40, 0, 40 , controller.View.Frame.Size.Height);
				verticalLineView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				verticalLineView.ClipsToBounds = false;
				controller.View.AddSubview (verticalLineView);
			}

			viewControllersStack.Add (controller);
			if (invokeByController !=null) {
				viewXPosition = invokeByController.View.Frame.X + invokeByController.View.Frame.Size.Width;			
			}
			if (slideViews.Subviews.Length == 0) {
				slideStartPosition = SLIDE_VIEWS_START_X_POS;
				viewXPosition = slideStartPosition;
				if (controller.View.Frame.Width > View.Frame.Width) {
					difference = controller.View.Frame.Width - View.Frame.Width;
					if (difference > Math.Abs (SLIDE_VIEWS_MINUS_X_POSITION)) {
						viewXPosition = SLIDE_VIEWS_MINUS_X_POSITION;
						controller.View.Frame = new CGRect (viewXPosition, navigationHeight, 954, View.Frame.Size.Height - navigationHeight);
					} else {
						slideStartPosition = -(difference);
						viewXPosition = -(difference);

					}
				} else {
					difference = 0;
				}
			}

			controller.View.Frame = new CGRect(viewXPosition, navigationHeight, controller.View.Frame.Size.Width, View.Frame.Size.Height-navigationHeight);
			controller.View.Tag = (int)(viewControllersStack.Count-1);
			controller.ViewWillAppear (false);
			controller.ViewDidAppear (false);
			slideViews.AddSubview (controller.View);

			if (slideViews.Subviews.Length > 0) {
				if (slideViews.Subviews.Length==1) {
					viewAtLeft = slideViews.Subviews.GetValue(slideViews.Subviews.Length-1) as UIView;
					viewAtLeft2 = null;
					viewAtRight = null;
					viewAtRight2 = null;
					if(navigationView != null)
						navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);

				}else 
					if (slideViews.Subviews.Length==2){
					viewAtRight = slideViews.Subviews.GetValue (slideViews.Subviews.Length - 1) as UIView;
					viewAtLeft = slideViews.Subviews.GetValue (slideViews.Subviews.Length - 2) as UIView;
					viewAtLeft2 = null;
					viewAtRight2 = null;

					UIView.BeginAnimations (null);
					UIView.SetAnimationTransition (UIViewAnimationTransition.None, viewAtLeft, true);
					UIView.SetAnimationBeginsFromCurrentState (false);
					if (viewAtLeft != null) {
						viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
						if(viewAtLeft2 != null){
						if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
						} else {
						if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
						}
					}
					if (viewAtRight != null) 
						viewAtRight.Frame = new CGRect (View.Frame.Size.Width - viewAtRight.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtRight.Frame.Y, (viewAtLeft == null) ? 0 : viewAtRight.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtRight.Frame.Size.Height);
					UIView.CommitAnimations ();
					slideStartPosition = SLIDE_VIEWS_MINUS_X_POSITION;

				}else {

					viewAtRight = slideViews.Subviews.GetValue (slideViews.Subviews.Length - 1) as UIView;
					viewAtLeft = slideViews.Subviews.GetValue(slideViews.Subviews.Length - 2) as UIView;
					viewAtLeft2 = slideViews.Subviews.GetValue(slideViews.Subviews.Length-3) as UIView;
					viewAtLeft2.Hidden = false;
					viewAtRight2 = null;
					UIView.BeginAnimations (null);
					UIView.SetAnimationTransition (UIViewAnimationTransition.None, viewAtLeft, true);
					UIView.SetAnimationBeginsFromCurrentState (false);

					if (viewAtLeft2.Frame.X != SLIDE_VIEWS_MINUS_X_POSITION) {
						viewAtLeft2.Frame = new CGRect(SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft2 == null) ? 0 : viewAtLeft2.Frame.Y, (viewAtLeft2 == null) ? 0 : viewAtLeft2.Frame.Size.Width, (viewAtLeft2 == null) ? 0 : viewAtLeft2.Frame.Size.Height);
					}
					if (viewAtLeft != null) {
						viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
						if(viewAtLeft2 != null){
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
						} else {
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
						}
					}

					if (viewAtRight != null) 
						viewAtRight.Frame = new CGRect (View.Frame.Size.Width - ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width), ((viewAtRight == null) ? 0 : viewAtRight.Frame.Y), (viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width, (viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Height);
					UIView.SetAnimationDelegate (this);
					UIView.SetAnimationDidStopSelector(new Selector("bounceBack:finished:context:"));
					UIView.CommitAnimations();
					slideStartPosition = SLIDE_VIEWS_MINUS_X_POSITION;	
					if(slideViews.Subviews.Length > 3){
						UIView a = slideViews.Subviews.GetValue(slideViews.Subviews.Length - 4) as UIView;
						a.Hidden =  true;
					}


				}
			}
		}


		public void handlePanFrom (UIPanGestureRecognizer recognizer){
			//			handlepan 
			CGPoint translatedPoint = recognizer.TranslationInView(this.View);
			if (recognizer.State == UIGestureRecognizerState.Began) {
				displacementPosition = 0;
				if(viewAtRight != null)
					positionOfViewAtRightAtTouchBegan = new CGPoint((viewAtRight == null)?0:viewAtRight.Frame.X,(viewAtRight == null)?0:viewAtRight.Frame.Y);
				positionOfViewAtLeftAtTouchBegan = new CGPoint((viewAtLeft == null)?0:viewAtLeft.Frame.X,(viewAtLeft == null)?0:viewAtLeft.Frame.Y);
				viewAtRightAtTouchBegan = viewAtRight;
				viewAtLeftAtTouchBegan = viewAtLeft;
				if(viewAtLeft != null)
					viewAtLeft.Layer.RemoveAllAnimations();
				if(viewAtRight != null)
					viewAtRight.Layer.RemoveAllAnimations();
				if(viewAtRight2 != null)
					viewAtRight2.Layer.RemoveAllAnimations();
				if(viewAtLeft2 != null)
					viewAtLeft2.Layer.RemoveAllAnimations();

				if (viewAtLeft2 != null) {
					int viewAtLeft2Position =  Array.IndexOf(slideViews.Subviews,viewAtLeft2);
					if (viewAtLeft2Position > 0) {

						UIView tmp = slideViews.Subviews.GetValue(viewAtLeft2Position - 1) as UIView;
						tmp.Hidden = false;
					}
				}

				arrangeVerticalBar();
			}

			//..
			CGPoint location =  recognizer.LocationInView(this.View);

			if (lastTouchPoint != -1) {

				if (location.X < lastTouchPoint) {			

					if (dragDirection.Equals(new NSString("RIGHT"))) {
						positionOfViewAtRightAtTouchBegan = new CGPoint((viewAtRight==null) ? 0 : viewAtRight.Frame.X ,(viewAtRight == null)? 0: viewAtRight.Frame.Y);
						positionOfViewAtLeftAtTouchBegan = new CGPoint((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X,(viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y);
						displacementPosition = translatedPoint.X * -1;
					}				

					dragDirection = new NSString("LEFT");

					if (viewAtRight != null) {

						if (viewAtLeft.Frame.X <= SLIDE_VIEWS_MINUS_X_POSITION) {	
							if(Array.IndexOf(slideViews.Subviews,viewAtRight) < slideViews.Subviews.Length - 1){
								viewAtLeft2 = viewAtLeft;
								viewAtLeft = viewAtRight;
								if(viewAtRight2!=null)
									viewAtRight2.Hidden = false;
								viewAtRight = viewAtRight2;
								if(Array.IndexOf(slideViews.Subviews,viewAtRight) < slideViews.Subviews.Length -1){
									viewAtRight2 = slideViews.Subviews.GetValue(Array.IndexOf(slideViews.Subviews,viewAtRight)+1) as UIView;
								}else {
									viewAtRight2 = null;
								}							
								positionOfViewAtRightAtTouchBegan = new CGPoint(((viewAtRight == null) ? 0 : viewAtRight.Frame.X),((viewAtRight ==  null) ? 0 : viewAtRight.Frame.Y));
								positionOfViewAtLeftAtTouchBegan = new CGPoint((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X,(viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y);
								displacementPosition = translatedPoint.X * -1;							
								if (Array.IndexOf(slideViews.Subviews,viewAtLeft2) > 1) {
									UIView tmp = slideViews.Subviews.GetValue(Array.IndexOf(slideViews.Subviews,viewAtLeft2) - 2) as UIView;
									tmp.Hidden = true;
								}

							}

						}


							if(viewAtLeft.Frame.X == SLIDE_VIEWS_MINUS_X_POSITION && viewAtRight.Frame.X + viewAtRight.Frame.Size.Width > View.Frame.Size.Width){
								if((positionOfViewAtLeftAtTouchBegan.X + translatedPoint.X + displacementPosition + viewAtRight.Frame.Size.Width) <= View.Frame.Size.Width) {
									if (viewAtRight != null) 
										viewAtRight.Frame = new CGRect(View.Frame.Size.Width - ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width) , (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y,(viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Height);
								}else {

									viewAtRight.Frame = new CGRect (positionOfViewAtRightAtTouchBegan.X + translatedPoint.X + displacementPosition, viewAtRight.Frame.Y, viewAtRight.Frame.Width, viewAtRight.Frame.Height);
									if (viewAtLeft2 != null) {
									if(navigationView != null)
										navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
									if(navigationView != null)
										navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
							}
							else
								if((Array.IndexOf(slideViews.Subviews,viewAtRight) == slideViews.Subviews.Length-1) && ((viewAtRight == null)? 0.0f : viewAtRight.Frame.X) <= (View.Frame.Size.Width - ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width))){
						
//									Console.WriteLine ("2: SLIDE_VIEWS_MINUS_X_POSITION :positionOfViewAtRightAtTouchBegan.X " + positionOfViewAtRightAtTouchBegan.X);

									viewAtRight.Frame = new CGRect (positionOfViewAtRightAtTouchBegan.X + translatedPoint.X + displacementPosition, viewAtRight.Frame.Y, viewAtRight.Frame.Width, viewAtRight.Frame.Height);
									if (viewAtLeft2 != null) {
									if(navigationView != null)
										navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
									if(navigationView != null)
										navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
//								}
							}
							else{						
								if (positionOfViewAtLeftAtTouchBegan.X + translatedPoint.X + displacementPosition <= SLIDE_VIEWS_MINUS_X_POSITION) {
									if (viewAtLeft != null) {
										viewAtLeft.Frame =  new CGRect(SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Width, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Height);
										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}
								}else {
									if (viewAtLeft != null) {
										viewAtLeft.Frame =  new CGRect(positionOfViewAtLeftAtTouchBegan.X + translatedPoint.X + displacementPosition, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Width, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Height);
										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}
								}	
								if (viewAtRight != null) 
									viewAtRight.Frame =  new CGRect((((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X) + ((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Width)), (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Width, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Height);
								if (viewAtLeft.Frame.X == SLIDE_VIEWS_MINUS_X_POSITION) {
									positionOfViewAtRightAtTouchBegan = new CGPoint((viewAtRight == null)? 0.0f : viewAtRight.Frame.X,(viewAtRight == null)? 0.0f : viewAtRight.Frame.Y);
									positionOfViewAtLeftAtTouchBegan = new CGPoint((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X,(viewAtLeft == null)? 0.0f :viewAtLeft.Frame.Y);
									displacementPosition = translatedPoint.X * -1;
								}
							}
//						}

					}else {
						if (viewAtLeft != null) {
							viewAtLeft.Frame =  new CGRect(positionOfViewAtLeftAtTouchBegan.X + translatedPoint.X + displacementPosition , (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Width, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Height);
							if (viewAtLeft2 != null) {
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
							} else {
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
							}
						}
					}
					arrangeVerticalBar();

				}else if (location.X > lastTouchPoint) {	

					if(dragDirection.Equals(new NSString("LEFT"))){
						positionOfViewAtRightAtTouchBegan = new CGPoint(((viewAtRight == null) ? 0 : viewAtRight.Frame.X),((viewAtRight == null) ? 0 : viewAtRight.Frame.Y));
						positionOfViewAtLeftAtTouchBegan = new CGPoint(((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X),((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y));
						displacementPosition = translatedPoint.X;
					}	

					dragDirection = new NSString("RIGHT");

					if (viewAtLeft != null) {
						nfloat widthForRight = ((viewAtRight == null) ? 0.0f : viewAtRight.Frame.X);
						if (widthForRight >= View.Frame.Size.Width) {
							if(Array.IndexOf(slideViews.Subviews,viewAtLeft) > 0) {
								if(viewAtRight2 != null)
									viewAtRight2.Hidden = true;
								viewAtRight2 =  viewAtRight;
								viewAtRight = viewAtLeft;
								viewAtLeft = viewAtLeft2;			
								if(Array.IndexOf(slideViews.Subviews,viewAtLeft) > 0){
									viewAtLeft2 =  slideViews.Subviews.GetValue(Array.IndexOf(slideViews.Subviews,viewAtLeft) - 1) as UIView;
									viewAtLeft2.Hidden = false;
								}
								else{
									viewAtLeft2 = null;
								}
								positionOfViewAtRightAtTouchBegan = new CGPoint(((viewAtRight == null) ? 0 : viewAtRight.Frame.X),((viewAtRight == null) ? 0 : viewAtRight.Frame.Y));
								positionOfViewAtLeftAtTouchBegan = new CGPoint(((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X), ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y));
								displacementPosition = translatedPoint.X;
								arrangeVerticalBar();
							}
						}

						if((((viewAtRight == null)?0:viewAtRight.Frame.X) < ((((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)))) && ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION){						
							if ((positionOfViewAtRightAtTouchBegan.X + translatedPoint.X - displacementPosition) >= (((viewAtLeft == null)?0:viewAtLeft.Frame.X) + ((viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width))) {
								if (viewAtRight != null)
									viewAtRight.Frame = new CGRect((((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)), (viewAtRight == null)? 0 : viewAtRight.Frame.Y, (viewAtRight == null)? 0 :viewAtRight.Frame.Size.Width, (viewAtRight == null)? 0 : viewAtRight.Frame.Size.Height);
							}else {
								if (viewAtRight != null)
									viewAtRight.Frame = new CGRect(positionOfViewAtRightAtTouchBegan.X + translatedPoint.X - displacementPosition, (viewAtRight == null)? 0 : viewAtRight.Frame.Y, (viewAtRight == null)? 0 : viewAtRight.Frame.Size.Width, (viewAtRight == null)? 0 : viewAtRight.Frame.Size.Height);
							}

						}
						else if (Array.IndexOf(slideViews.Subviews,viewAtLeft) == 0) {
							if (viewAtRight == null) {
								if (viewAtLeft != null) {
									viewAtLeft.Frame =  new CGRect(positionOfViewAtLeftAtTouchBegan.X + translatedPoint.X - displacementPosition, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Width, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
							}
							else{
								if (viewAtRight != null) 
									viewAtRight.Frame =  new CGRect(positionOfViewAtRightAtTouchBegan.X + translatedPoint.X - displacementPosition, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Width, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Height);
								if ((((viewAtRight == null)? 0 :viewAtRight.Frame.X) - ((viewAtLeft == null)? 0 : viewAtLeft.Frame.Width)) < SLIDE_VIEWS_MINUS_X_POSITION) {
									if (viewAtLeft != null) {
										viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}
								}else{
									if (viewAtLeft != null) {
										viewAtLeft.Frame =  new CGRect(((viewAtRight == null) ? 0 : viewAtRight.Frame.X) - ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width), (viewAtLeft == null)? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null)? 0 : viewAtLeft.Frame.Size.Height);
										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}
								}
							}
						}else{
							if ((positionOfViewAtRightAtTouchBegan.X + translatedPoint.X - displacementPosition) >= View.Frame.Size.Width) {
								if (viewAtRight != null) 
									viewAtRight.Frame =  new CGRect(View.Frame.Size.Width, (viewAtRight == null) ? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null) ? 0.0f : viewAtRight.Frame.Size.Width, (viewAtRight == null) ? 0.0f : viewAtRight.Frame.Size.Height);
							}else {
								if (viewAtRight != null) 
									viewAtRight.Frame =  new CGRect(positionOfViewAtRightAtTouchBegan.X + translatedPoint.X - displacementPosition, (viewAtRight == null) ? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null) ? 0.0f :  viewAtRight.Frame.Size.Width, (viewAtRight == null) ? 0.0f : viewAtRight.Frame.Size.Height);
							}
							if ((((viewAtRight == null)? 0 : viewAtRight.Frame.X) - ((viewAtLeft == null)? 0 : viewAtLeft.Frame.Width)) < SLIDE_VIEWS_MINUS_X_POSITION) {
								if (viewAtLeft != null) {
									viewAtLeft.Frame =  new CGRect(SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Width, (viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
							}
							else{
								if (viewAtLeft != null) {
									viewAtLeft.Frame =  new CGRect(((viewAtRight == null)? 0 : viewAtRight.Frame.X) - ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width), (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, ((viewAtLeft == null)? 0 : viewAtLeft.Frame.Size.Height));
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
							}
							if (viewAtRight != null) {
								if (viewAtRight.Frame.X >= View.Frame.Size.Width) {
									positionOfViewAtRightAtTouchBegan = new CGPoint((viewAtRight !=  null) ? 0 : viewAtRight.Frame.X, (viewAtRight !=  null) ? 0 : viewAtRight.Frame.Y);
									if(viewAtLeft != null)
										positionOfViewAtLeftAtTouchBegan = new CGPoint((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X,(viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Y);
									displacementPosition = translatedPoint.X;
								}
							}
							arrangeVerticalBar();
						}

					}
					arrangeVerticalBar();
				}
			}

			lastTouchPoint = location.X;

			// STATE END	
			if (recognizer.State == UIGestureRecognizerState.Ended) {
				if (dragDirection.Equals(new NSString("LEFT"))) {
					if (viewAtRight != null) {
						if (Array.IndexOf(slideViews.Subviews,viewAtLeft) == 0 && !(((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION || (((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X) == SLIDE_VIEWS_START_X_POS))) {
							UIView.BeginAnimations(null);
							UIView.SetAnimationDuration(0.2);
							UIView.SetAnimationTransition(UIViewAnimationTransition.None,new UIView(),true);
							UIView.SetAnimationBeginsFromCurrentState(true);
							if (((viewAtLeft != null) ? 0 : viewAtLeft.Frame.X) < SLIDE_VIEWS_START_X_POS && viewAtRight != null) {
								if (viewAtLeft != null) {
									viewAtLeft.Frame =  new CGRect(SLIDE_VIEWS_MINUS_X_POSITION, viewAtLeft.Frame.Y, viewAtLeft.Frame.Width, viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
								if(viewAtRight != null)
									viewAtRight.Frame = new CGRect(SLIDE_VIEWS_MINUS_X_POSITION + viewAtLeft.Frame.Size.Width, viewAtRight.Frame.Y, viewAtRight.Frame.Size.Width,viewAtRight.Frame.Size.Height);
							}
							else{
								//Drop Card View Animation
								UIView dropView =  slideViews.Subviews.GetValue(0) as UIView;
								nfloat tempWidthForCalculation = (dropView.Frame.Size.Width > (View.Frame.Width/2)) ? View.Frame.Width/2 : dropView.Frame.Size.Width; 
								if ((dropView.Frame.X+200) >= tempWidthForCalculation) {

									int viewControllerCount = viewControllersStack.Count;

									if (viewControllerCount > 1) {
										for (int i = 1; i < viewControllerCount; i++) {
											viewXPosition = View.Frame.Size.Width - slideViews.ViewWithTag(i).Frame.Size.Width;
											slideViews.ViewWithTag(i).RemoveFromSuperview();
											viewControllersStack.RemoveAt(viewControllersStack.Count -1);
										} 

										if(borderViews.ViewWithTag(3) != null)
											borderViews.ViewWithTag(3).Hidden = true;
										if(borderViews.ViewWithTag(2) != null)
											borderViews.ViewWithTag(2).Hidden = true;
										if(borderViews.ViewWithTag(1) != null)
											borderViews.ViewWithTag(1).Hidden = true;
									}

									// Removes the selection of row for the first slide view
									UIView temp = slideViews.Subviews.GetValue(0) as UIView;
									foreach (var tableView in temp.Subviews) { //.. change UIView to var
										if(tableView is UITableView){
											UITableView table = tableView as UITableView;
											NSIndexPath selectedRow = table.IndexPathForSelectedRow;
											NSIndexPath[] indexPaths = new NSIndexPath[]{selectedRow};
											table.ReloadRows(indexPaths,UITableViewRowAnimation.None);
										}
									}

									viewAtLeft2 = null;
									viewAtRight =  null;
									viewAtRight2 = null;							 
								}
								if (viewAtLeft != null) {
									viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_START_X_POS, (viewAtLeft == null) ? 0.0f : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0.0f : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0.0f : viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}

								if (viewAtRight != null)
									viewAtRight.Frame =  new CGRect(SLIDE_VIEWS_START_X_POS + ((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.Size.Width), (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width,(viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Height);

							}
							UIView.CommitAnimations();
						}
						else if (((	(viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION) && (((	(viewAtRight == null)? 0.0f : viewAtRight.Frame.X) + ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width) > View.Frame.Size.Width))){
							UIView.BeginAnimations(null);
							UIView.SetAnimationDuration(0.2);
							UIView.SetAnimationTransition(UIViewAnimationTransition.None,viewAtRight,true);
							UIView.SetAnimationBeginsFromCurrentState(true);
							if(viewAtRight != null)
								viewAtRight.Frame = new CGRect(View.Frame.Size.Width - ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width), (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width,(viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Height);
							UIView.CommitAnimations();

						}	
						else if (((viewAtLeft == null)? 0.0f : viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION && ((((viewAtRight == null)? 0.0f : viewAtRight.Frame.X) + ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width)) < View.Frame.Size.Width)) {
							UIView.BeginAnimations("RIGHT-WITH-RIGHT");
							UIView.SetAnimationDuration(0.2);
							UIView.SetAnimationTransition(UIViewAnimationTransition.None,new UIView(),true);
							UIView.SetAnimationBeginsFromCurrentState(true);
							if (viewAtRight != null) 
								viewAtRight.Frame =  new CGRect(View.Frame.Size.Width - ((viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width), (viewAtRight == null)? 0.0f : viewAtRight.Frame.Y, (viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Width,(viewAtRight == null)? 0.0f : viewAtRight.Frame.Size.Height);
							UIView.SetAnimationDelegate(this);
							UIView.SetAnimationDidStopSelector(new Selector("bounceBack:finished:context:"));
							UIView.CommitAnimations();
//							Console.WriteLine ("RIGHT-WITH-RIGHT");
						}
						else if (((viewAtLeft == null) ? 0.0f : viewAtLeft.Frame.X) > SLIDE_VIEWS_MINUS_X_POSITION) {
							UIView.SetAnimationDuration(0.2);
							UIView.SetAnimationTransition(UIViewAnimationTransition.None,new UIView(),true);
							UIView.SetAnimationBeginsFromCurrentState(true);
							if (((((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)) > View.Frame.Size.Width) && (((viewAtLeft == null) ? 0 : viewAtLeft.Frame.X) < (View.Frame.Size.Width - (viewAtLeft.Frame.Size.Width)/2))) {
								UIView.BeginAnimations("LEFT-WITH-LEFt");
								if (viewAtLeft != null) {
									viewAtLeft.Frame =  new CGRect(View.Frame.Size.Width - ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width), ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y), (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0,(nfloat) (View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat) (View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
//								Console.WriteLine ("LEFT-WITH-LEFt");

								//Show bounce effect
								if (viewAtRight != null) 
									viewAtRight.Frame =  new CGRect(View.Frame.Size.Width, (viewAtRight == null) ? 0 : viewAtRight.Frame.Y,  (viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width, (viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Height);
							}
							else {
//								Console.WriteLine ("LEFT-WITH-RIGHT");

								UIView.BeginAnimations("LEFT-WITH-RIGHT");
								if (viewAtLeft != null) {
									viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0.0f : viewAtLeft.Frame.Y, viewAtLeft.Frame.Size.Width, viewAtLeft.Frame.Size.Height);
									if (viewAtLeft2 != null) {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
									} else {
										if(navigationView != null)
											navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
									}
								}
								if ((positionOfViewAtLeftAtTouchBegan.X + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)) <= View.Frame.Size.Width) {
									if (viewAtRight != null) 
										viewAtRight.Frame =  new CGRect((View.Frame.Size.Width - ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width)), ((viewAtRight == null) ? 0.0f : viewAtRight.Frame.Y), ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width),(viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Height);
								}
								else{
									if (viewAtRight != null) 
										viewAtRight.Frame =  new CGRect((SLIDE_VIEWS_MINUS_X_POSITION + ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width)),  ((viewAtRight == null) ? 0 : viewAtRight.Frame.Y), (viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width,(viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Height);
								}

								//Show bounce effect
								if(viewAtRight2 != null)
									viewAtRight2.Frame = new CGRect((((viewAtRight == null) ? 0 : viewAtRight.Frame.X) + ((viewAtRight == null) ? 0 : viewAtRight.Frame.Size.Width)), ((viewAtRight2 == null) ? 0 : viewAtRight2.Frame.Y), (viewAtRight2 == null) ? 0 : viewAtRight2.Frame.Size.Width, viewAtRight2.Frame.Size.Height);
							}
							UIView.SetAnimationDelegate(this);
							UIView.SetAnimationDidStopSelector(new Selector("bounceBack:finished:context:"));
							UIView.CommitAnimations();
						}

					}
					else{
						UIView.BeginAnimations(null);
						UIView.SetAnimationDuration(0.2);
						if (viewAtLeft != null) {
							UIView.SetAnimationBeginsFromCurrentState (true);
							UIView.SetAnimationTransition (UIViewAnimationTransition.None, viewAtLeft, true);
							//..
							if (viewAtLeft != null) {
								if (viewAtLeft.Frame.Width > View.Frame.Width) {
									difference = viewAtLeft.Frame.Width - View.Frame.Width;
									if (difference > Math.Abs (SLIDE_VIEWS_MINUS_X_POSITION)) {
										viewXPosition = SLIDE_VIEWS_MINUS_X_POSITION;
										viewAtLeft.Frame = new CGRect (viewXPosition, navigationHeight, 954, View.Frame.Size.Height - navigationHeight);
									} else {
										slideStartPosition = -(difference);
										viewXPosition = -(difference);
									}
									viewAtLeft.Frame = new CGRect (viewXPosition, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height); 

								} else {
									difference = 0;
									viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_START_X_POS, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height); 
								}
							}
							//...
							if (viewAtLeft2 != null) {
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
							} else {
								if(navigationView != null)
									navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
							}
						}
						UIView.CommitAnimations();
					}

				}else
					if (dragDirection.Equals(new NSString("RIGHT"))){
						if (viewAtLeft != null) {
							if (Array.IndexOf(slideViews.Subviews,viewAtLeft) == 0 && !(((viewAtLeft == null)?0: viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION || ((viewAtLeft == null)?0:viewAtLeft.Frame.X) == SLIDE_VIEWS_START_X_POS)) {
								UIView.BeginAnimations(null);
								UIView.SetAnimationDuration(0.2);
								UIView.SetAnimationBeginsFromCurrentState(true);
								UIView.SetAnimationTransition(UIViewAnimationTransition.None,new UIView(),true);
								if (((viewAtLeft == null)?0: viewAtLeft.Frame.X) > SLIDE_VIEWS_MINUS_X_POSITION || viewAtRight == null) {

									//Drop Card View Animation
									UIView temp = slideViews.Subviews.GetValue(0) as UIView;
									nfloat tempWidthForCalculation = (temp.Frame.Size.Width > (View.Frame.Width/2)) ? View.Frame.Width/2 : temp.Frame.Size.Width; 
									if ((temp.Frame.X+200) >= tempWidthForCalculation) {
										int viewControllerCount = viewControllersStack.Count;
										if (viewControllerCount > 1) {
											for (int i = 1; i < viewControllerCount; i++) {
												UIView v = slideViews.ViewWithTag(i) as UIView;
												viewXPosition = View.Frame.Size.Width - v.Frame.Size.Width;
												v.RemoveFromSuperview();
												viewControllersStack.RemoveAt(viewControllersStack.Count - 1);
											}
											if(borderViews.ViewWithTag (3) != null)
												borderViews.ViewWithTag (3).Hidden =  true;
											if(borderViews.ViewWithTag (2) != null)
												borderViews.ViewWithTag (2).Hidden =  true;
											if(borderViews.ViewWithTag (1) != null)
												borderViews.ViewWithTag (1).Hidden =  true;
										}

										// Removes the selection of row for the first slide view
										UIView tmp1 = slideViews.Subviews.GetValue(0) as UIView;
										foreach (var tableView in tmp1.Subviews) { //.. change UIView to var
											if(tableView is UITableView){
												UITableView table = tableView as UITableView;
												NSIndexPath selectedRow = table.IndexPathForSelectedRow;
												if (selectedRow != null) {
													NSIndexPath[] indexPaths = new NSIndexPath[]{selectedRow};
													table.ReloadRows(indexPaths,UITableViewRowAnimation.None);
												}
											}
										}

										viewAtLeft2 = null;
										viewAtRight = null;
										viewAtRight2 = null;							 
									}
									if (viewAtLeft != null){ 
										//..
										if (viewAtLeft.Frame.Width > View.Frame.Width) {
											difference = viewAtLeft.Frame.Width - View.Frame.Width;
											if (difference > Math.Abs (SLIDE_VIEWS_MINUS_X_POSITION)) {
												viewXPosition = SLIDE_VIEWS_MINUS_X_POSITION;
												viewAtLeft.Frame = new CGRect (viewXPosition, navigationHeight, 954, View.Frame.Size.Height - navigationHeight);
											} else {
												slideStartPosition = -(difference);
												viewXPosition = -(difference);
											}
											viewAtLeft.Frame = new CGRect (viewXPosition, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height); 

										} else {//...
											difference = 0;
											viewAtLeft.Frame =  new CGRect(SLIDE_VIEWS_START_X_POS, (viewAtLeft == null)?0: viewAtLeft.Frame.Y, (viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width, (viewAtLeft == null)?0:viewAtLeft.Frame.Size.Height);
										}

										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}	
									if (viewAtRight != null) {
										viewAtRight.Frame =  new CGRect(SLIDE_VIEWS_START_X_POS + ((viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width), ((viewAtRight == null)?0:viewAtRight.Frame.Y), ((viewAtRight == null)?0:viewAtRight.Frame.Size.Width),((viewAtRight == null)?0:viewAtRight.Frame.Size.Height));
									}
								}
								else{
									if (viewAtLeft != null) {
										viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
										if (viewAtLeft2 != null) {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
										} else {
											if(navigationView != null)
												navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
										}
									}

									if (viewAtRight != null) 
										viewAtRight.Frame = new CGRect(SLIDE_VIEWS_MINUS_X_POSITION + ((viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width), (viewAtRight == null)?0:viewAtRight.Frame.Y, (viewAtRight == null)?0:viewAtRight.Frame.Size.Width,(viewAtRight == null)?0:viewAtRight.Frame.Size.Height);
								}
								UIView.CommitAnimations();
							}
							else if (((viewAtRight == null)?0:viewAtRight.Frame.X) < View.Frame.Size.Width) {
								if((((viewAtRight == null)?0:viewAtRight.Frame.X) < (((viewAtLeft == null)?0:viewAtLeft.Frame.X) + viewAtLeft.Frame.Size.Width)) && (((viewAtRight == null)?0:viewAtRight.Frame.X) < (View.Frame.Size.Width - ((viewAtRight == null)?0:viewAtRight.Frame.Size.Width/2)))){
									UIView.BeginAnimations ("RIGHT-WITH-RIGHT");
									UIView.SetAnimationDuration (0.2);
									UIView.SetAnimationBeginsFromCurrentState (true);
									UIView.SetAnimationTransition (UIViewAnimationTransition.None, new UIView(), true);
									if (viewAtRight != null) 
										viewAtRight.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION + ((viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width), ((viewAtRight == null)?0:viewAtRight.Frame.Y), ((viewAtRight == null)?0:viewAtRight.Frame.Size.Width), (viewAtRight == null)?0:viewAtRight.Frame.Size.Height);
									UIView.SetAnimationDelegate (this);
									UIView.SetAnimationDidStopSelector (new Selector ("bounceBack:finished:context:"));
									UIView.CommitAnimations ();
//									Console.WriteLine ("RIGHT-WITH-RIGHT");
								}				
								else{
//									Console.WriteLine ("RIGHT-WITH-LEFT");
									UIView.BeginAnimations ("RIGHT-WITH-LEFT");
									UIView.SetAnimationDuration (0.2);
									UIView.SetAnimationBeginsFromCurrentState (true);
									UIView.SetAnimationTransition (UIViewAnimationTransition.None, new UIView(), true);
									if(Array.IndexOf(slideViews.Subviews,viewAtLeft) > 0){ 
										if ((positionOfViewAtRightAtTouchBegan.X  + ((viewAtRight == null)?0:viewAtRight.Frame.Size.Width)) <= View.Frame.Size.Width) {	
											if (viewAtLeft != null) {
												viewAtLeft.Frame = new CGRect (View.Frame.Size.Width - ((viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width), (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
												if (viewAtLeft2 != null) {
													if(navigationView != null)
														navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
												} else {
													if(navigationView != null)
														navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
												}
											}
										}
										else{							
											if (viewAtLeft != null) {
												viewAtLeft.Frame = new CGRect(SLIDE_VIEWS_MINUS_X_POSITION + ((viewAtLeft2 == null)?0:viewAtLeft2.Frame.Size.Width), (viewAtLeft == null)?0:viewAtLeft.Frame.Y, (viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width, (viewAtLeft == null)?0:viewAtLeft.Frame.Size.Height);
												if (viewAtLeft2 != null) {
													if(navigationView != null)
														navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
												} else {
													if(navigationView != null)
														navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (nfloat)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
												}
											}
										}
										if (viewAtRight != null) 
											viewAtRight.Frame = new CGRect (View.Frame.Size.Width, (viewAtRight == null)?0:viewAtRight.Frame.Y, (viewAtRight == null)?0:viewAtRight.Frame.Size.Width,(viewAtRight == null)?0:viewAtRight.Frame.Size.Height);
									}else{
										if (viewAtLeft != null) {
											viewAtLeft.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Y, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Width, (viewAtLeft == null) ? 0 : viewAtLeft.Frame.Size.Height);
											if (viewAtLeft2 != null) {
												if(navigationView != null)
													navigationView.Frame = new CGRect (viewAtLeft2.Frame.X, 0,(float) (View.Frame.Size.Width + Math.Abs (viewAtLeft2.Frame.X)), navigationHeight);
											} else {
												if(navigationView != null)
													navigationView.Frame = new CGRect (viewAtLeft.Frame.X, 0, (float)(View.Frame.Size.Width + Math.Abs (viewAtLeft.Frame.X)), navigationHeight);
											}
										}
										if (viewAtRight != null) 
											viewAtRight.Frame = new CGRect (SLIDE_VIEWS_MINUS_X_POSITION + ((viewAtLeft == null)?0:viewAtLeft.Frame.Size.Width), (viewAtRight == null)?0:viewAtRight.Frame.Y, (viewAtRight == null)?0:viewAtRight.Frame.Size.Width,(viewAtRight == null)?0:viewAtRight.Frame.Size.Height);
									}
									UIView.SetAnimationDelegate (this);
									UIView.SetAnimationDidStopSelector (new Selector ("bounceBack:finished:context:"));
									UIView.CommitAnimations ();
								}

							}
						}			
					}
				lastTouchPoint = -1;
				dragDirection = new NSString("");
			}	
		}


		public void arrangeVerticalBar(){
			if (slideViews.Subviews.Length > 2) {
				borderViews.ViewWithTag (2).Hidden = true;
				borderViews.ViewWithTag (1).Hidden = true;
				int stackCount = 0;
				if (viewAtLeft != null) {
					stackCount = Array.IndexOf (slideViews.Subviews, viewAtLeft);
				}

				if (viewAtLeft != null && (((viewAtLeft == null)?0: viewAtLeft.Frame.X) == SLIDE_VIEWS_MINUS_X_POSITION)) {
					stackCount += 1;
				}

				if (stackCount == 2) {
					borderViews.ViewWithTag (2).Hidden = false;
				}
				if (stackCount >= 3) {
					borderViews.ViewWithTag (2).Hidden = false;
					borderViews.ViewWithTag (1).Hidden = false;
				}
			}
		}
	}
}

