 using System;
using CoreGraphics;

using Foundation;
using UIKit;
using System.Collections.Generic;
using CommonLayer;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{

    public static class ViewExtensions
    {

        public static UIView FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }
            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = subView.FindFirstResponder();
                if (firstResponder != null)
                    return firstResponder;
            }
            return null;
        }

        public static UIView FindSuperviewOfType(this UIView view, UIView stopAt, Type type)
        {
            if (view.Superview != null)
            {
                if (type.IsAssignableFrom(view.Superview.GetType()))
                {
                    return view.Superview;
                }

                if (view.Superview != stopAt)
                    return view.Superview.FindSuperviewOfType(stopAt, type);
            }

            return null;
        }
    }



    public class UIViewExt : UIView
    {
        public UIViewExt()
        {

        }
        
    }

    public enum DialogAlign
    {
        topLeft,
        topRight,
        bottomLeft,
        bottomRight,
        center
    }

    public partial class RootViewController : UIViewController
    {

        NSObject showObserver;
        NSObject hideObserver;
		UIView statusView;
        UIViewExt rootView;
        UIView leftMenuView;
		UIView rightShadowForLeftMenuView;

        UIViewController rightViewController;

		UIViewWithShadow leftShadowForDetailStackView;

        public UIScrollView dialogueView;

        public UIView rightSlideView;
        UIView rightSlideOpacityView;
        public UIView detailStackView;
        nfloat leftViewMAXWidth = 220.0f;
        nfloat leftViewMINWidth = 50.0f;

        nfloat detailMAXWidth = 362.0f;
        public bool leftMenuOpened;

		public bool browserViewOpened;

        public LeftMenuController menuViewController;
        UINavigationController menuViewNavigationController;

        public StackViewController stackScrollViewController { get; set; }

        bool navigationBarEnable;

        public UIPopoverController popOverController { get; set; }

        public RootViewController(bool navigationEnable)
        {
            navigationBarEnable = navigationEnable;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = true;
        }

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			browserViewOpened = false;

			SetNeedsStatusBarAppearanceUpdate ();

            leftMenuOpened = false;

			statusView = new UIView ();
			statusView.Frame = new CGRect (0, 0, View.Frame.Size.Width, 20);
			statusView.BackgroundColor = UIColor.Clear.FromHexString (AppTheme.Layer3Color, 1.0f);

            rootView = new UIViewExt();
            rootView.Frame = new CGRect(0, 0, View.Frame.Size.Width, View.Frame.Size.Height);
            rootView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			rootView.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.Layer1Color,1.0f);
            leftMenuView = new UIView(new CGRect(0, 0, leftViewMINWidth, View.Frame.Size.Height));
            leftMenuView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            leftMenuView.BackgroundColor = UIColor.White;
            leftMenuView.AutosizesSubviews = true;


            dialogueView = new UIScrollView();
            dialogueView.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0.5f);
            dialogueView.Hidden = true;

			rightShadowForLeftMenuView = new UIView();
            rightShadowForLeftMenuView.Frame = new CGRect(leftMenuView.Frame.Size.Width - 1, 0, 4, leftMenuView.Frame.Size.Height);
            rightShadowForLeftMenuView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
			rightShadowForLeftMenuView.BackgroundColor = UIColor.Clear.FromHexString (AppTheme.Layer3Color, 1.0f);
            rightShadowForLeftMenuView.ClipsToBounds = false;
            leftMenuView.AddSubview(rightShadowForLeftMenuView);

            menuViewController = new LeftMenuController(new CGRect(0, 0, leftMenuView.Frame.Size.Width, leftMenuView.Frame.Size.Height));
            menuViewNavigationController = new UINavigationController(menuViewController);
            menuViewNavigationController.View.Frame = new CGRect(0, 0, leftMenuView.Frame.Size.Width, leftMenuView.Frame.Size.Height);
            menuViewNavigationController.View.BackgroundColor = UIColor.White;
            menuViewNavigationController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            menuViewNavigationController.View.AutosizesSubviews = true;
            menuViewNavigationController.ViewWillAppear(false);
            menuViewNavigationController.ViewDidAppear(false);

		
            menuViewNavigationController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            leftMenuView.AddSubview(menuViewNavigationController.View);

            rightSlideView = new UIView(new CGRect(leftMenuView.Frame.Size.Width, 0, rootView.Frame.Size.Width - leftMenuView.Frame.Size.Width, rootView.Frame.Size.Height));
			rightSlideView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleRightMargin;
            rightSlideView.BackgroundColor = UIColor.White;
            rightSlideView.AutosizesSubviews = true;

            rightSlideOpacityView = new UIView(rightSlideView.Frame);
            rightSlideOpacityView.BackgroundColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.5f);
            rightSlideOpacityView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(removeDetailByTap)
            {
                NumberOfTapsRequired = 1
            };
            rightSlideOpacityView.Alpha = 0.0f;
            rightSlideOpacityView.AddGestureRecognizer(tapGesture);

            detailStackView = new UIView(new CGRect(rootView.Frame.Size.Width, 0, detailMAXWidth, View.Frame.Size.Height));
            detailStackView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            detailStackView.BackgroundColor = UIColor.Black;

            leftShadowForDetailStackView = new UIViewWithShadow(UIViewWithShadow.ShadowSide.left);
            leftShadowForDetailStackView.Frame = new CGRect(-40, 0, 40, detailStackView.Frame.Size.Height);
            leftShadowForDetailStackView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            leftShadowForDetailStackView.ClipsToBounds = false;
            detailStackView.AddSubview(leftShadowForDetailStackView);

            rootView.AddSubview(rightSlideView);
            rootView.AddSubview(rightSlideOpacityView);

            rootView.AddSubview(detailStackView);

            rootView.AddSubview(dialogueView);

			rootView.AddSubview (menuViewController.bottonTabView);

			rootView.AddSubview(leftMenuView);

			this.View.BackgroundColor = UIColor.White;
			this.View.AddSubviews(rootView,statusView);



            leftMenuOpened = false;
            arrangeleftMenu(leftMenuOpened);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
            {
            }

        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
			statusView.Frame = new CGRect (0, 0, View.Frame.Size.Width, 20);
            rootView.Frame = new CGRect(0, 0, View.Frame.Size.Width, View.Frame.Size.Height);

			CGRect openFrame = menuViewController.bottonTabViewOpenFrame;
			openFrame.Y = rootView.Frame.Height - 45;
			menuViewController.bottonTabViewOpenFrame = openFrame;

			CGRect closedFrame = menuViewController.bottonTabViewClosedFrame;
			closedFrame.Y = rootView.Frame.Height - 45;
			closedFrame.X = -(menuViewController.bottonTabViewClosedFrame.Width-50);
			menuViewController.bottonTabViewClosedFrame = closedFrame;


            dialogueView.Frame = new CGRect(0, 0, rootView.Frame.Size.Width, rootView.Frame.Size.Height);
            arrangeleftMenu(leftMenuOpened);
            if (navigationBarEnable && menuViewNavigationController != null)
                menuViewNavigationController.View.Frame = new CGRect(0, 0, leftMenuView.Frame.Size.Width, leftMenuView.Frame.Size.Height);

			if (browserViewOpened)
			{
				rightSlideView.Frame = new CGRect(leftMenuView.Frame.Size.Width, 20, rootView.Frame.Size.Width - leftMenuView.Frame.Size.Width, rootView.Frame.Size.Height-20);
			}else{
				rightSlideView.Frame = new CGRect(leftMenuView.Frame.Size.Width, 0, rootView.Frame.Size.Width - leftMenuView.Frame.Size.Width, rootView.Frame.Size.Height);
			}

            if (rightViewController != null)
            {
				if (browserViewOpened) {
					rightViewController.View.Frame = new CGRect(0, 20, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height-20);
				} else {
					rightViewController.View.Frame = new CGRect(0, 0, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height);
				}
                rightViewController.ViewWillLayoutSubviews();
            }
            rightSlideOpacityView.Frame = rightSlideView.Frame;

            detailStackView.Frame = new CGRect(rootView.Frame.Size.Width, 0, detailMAXWidth, View.Frame.Size.Height);

        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
			statusView.Frame = new CGRect (0, 0, View.Frame.Size.Width, 20);

            rootView.Frame = new CGRect(0, 0, View.Frame.Size.Width, View.Frame.Size.Height);
            dialogueView.Frame = new CGRect(0, 0, rootView.Frame.Size.Width, rootView.Frame.Size.Height);
            arrangeleftMenu(leftMenuOpened);
            if (menuViewNavigationController != null)
                menuViewNavigationController.View.Frame = new CGRect(0, 0, leftMenuView.Frame.Size.Width, leftMenuView.Frame.Size.Height);

				rightSlideView.Frame = new CGRect(leftMenuView.Frame.Size.Width, 0, rootView.Frame.Size.Width - leftMenuView.Frame.Size.Width, rootView.Frame.Size.Height);


            if (rightViewController != null)
            {
				if (browserViewOpened) {
					rightViewController.View.Frame = new CGRect(0, 20, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height-20);
				} else {
					rightViewController.View.Frame = new CGRect(0, 0, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height);
				}
                rightViewController.ViewWillLayoutSubviews();
            }

            rightSlideOpacityView.Frame = rightSlideView.Frame;

            detailStackView.Frame = new CGRect(rootView.Frame.Size.Width, 0, detailMAXWidth, View.Frame.Size.Height);

        }

        public void MenuClicked()
        {

			menuViewController.AnimateTabView ();

            UIView.Animate(0.2, () =>
            {
                if (leftMenuView.Frame.Width == leftViewMAXWidth)
                {
                    leftMenuOpened = false;
                    arrangeleftMenu(leftMenuOpened);
                    rightShadowForLeftMenuView.Frame = new CGRect(leftMenuView.Frame.Size.Width - 1, 0, 4, leftMenuView.Frame.Size.Height);
                }
                else
                {
                    leftMenuOpened = true;
                    arrangeleftMenu(leftMenuOpened);
                }
                realignRight();
            });

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.left_menu_icon, Helper.ToDateString(DateTime.Now));
        }

        void arrangeleftMenu(bool opened)
        {
            if (!leftMenuOpened)
            {
                leftMenuView.Frame = new CGRect(0, 0, leftViewMINWidth, View.Frame.Size.Height);
                rightShadowForLeftMenuView.Frame = new CGRect(leftMenuView.Frame.Size.Width - 1, 0, 4, leftMenuView.Frame.Size.Height);
                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.left_menu_slide_close, Helper.ToDateString(DateTime.Now));
            }
            else
            {
                leftMenuView.Frame = new CGRect(0, 0, leftViewMAXWidth, View.Frame.Size.Height);
                rightShadowForLeftMenuView.Frame = new CGRect(leftMenuView.Frame.Size.Width - 1, 0, 4, leftMenuView.Frame.Size.Height);
                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.left_menu_slide_open, Helper.ToDateString(DateTime.Now));
            }
        }

        public void realignRight()
        {
            rightSlideView.Frame = new CGRect(leftMenuView.Frame.Size.Width, 0, rootView.Frame.Size.Width - leftMenuView.Frame.Size.Width, rootView.Frame.Size.Height);
            rightSlideView.BackgroundColor = UIColor.White;
            rightSlideOpacityView.Frame = rightSlideView.Frame;
            foreach (UIView vw in rightSlideView.Subviews)
            {
                vw.Frame = new CGRect(0, 0, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height);
            }
        }

        public void openFromMenu(UIViewController rightController)
        {
			browserViewOpened = false;

            foreach (UIView subview in rightSlideView.Subviews)
            {
                subview.RemoveFromSuperview();
            }
            removeDetail();

            rightViewController = rightController;
			if (browserViewOpened) {
				rightViewController.View.Frame = new CGRect(0, 20, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height-20);
			} else {
				rightViewController.View.Frame = new CGRect(0, 0, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height);
			}
			rightViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleRightMargin;
            rightViewController.ViewWillAppear(false);
            rightViewController.ViewDidAppear(false);
            rightSlideView.AddSubview(rightViewController.View);
            ViewWillLayoutSubviews();
        }

		public void openFromMenuForFullscreenWeb(UIViewController rightController)
		{
			browserViewOpened = true;

			foreach (UIView subview in rightSlideView.Subviews)
			{
				subview.RemoveFromSuperview();
			}
			removeDetail();

			rightViewController = rightController;
			rightViewController.View.Frame = new CGRect(0, 20, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height-20);
//			rightViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			rightViewController.ViewWillAppear(false);
			rightViewController.ViewDidAppear(false);
			rightSlideView.AddSubview(rightViewController.View);
			ViewWillLayoutSubviews();

			rightViewController.View.Frame = new CGRect(0, 20, rightSlideView.Frame.Size.Width, rightSlideView.Frame.Size.Height-20);
		}

        public void openInDialougueView(UIViewController controller, DialogAlign align)
        {
            CGPoint xy = CGPoint.Empty;
            switch (align)
            {
                case DialogAlign.topLeft:
                    {
                        xy = new CGPoint(10, 10);
                        break;
                    }
                case DialogAlign.topRight:
                    {
                        xy = new CGPoint(dialogueView.Frame.Width - (controller.View.Frame.Width + 10), dialogueView.Frame.Y + 10);
                        break;
                    }
                case DialogAlign.bottomLeft:
                    {
                        xy = new CGPoint(10, dialogueView.Frame.Height - (controller.View.Frame.Height + 10));
                        break;
                    }
                case DialogAlign.bottomRight:
                    {
                        xy = new CGPoint(dialogueView.Frame.Width - (controller.View.Frame.Width + 10), dialogueView.Frame.Height - (controller.View.Frame.Height + 10));
                        break;
                    }
                case DialogAlign.center:
                    {
                        xy = new CGPoint(dialogueView.Frame.Width / 2 - controller.View.Frame.Width / 2, dialogueView.Frame.Height / 2 - controller.View.Frame.Height / 2);
                        break;
                    }
                default:
                    {
                        xy = new CGPoint(10, 10);
                        break;
                    }
            }
            opendialog(xy, controller.View);
        }

        public void openInDialougueView(UIViewController controller, CGPoint origin)
        {
            opendialog(origin, controller.View);
        }


        void registerForKeyboardNotifications()
        {
            showObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (notification) =>
            {
                KeyboardWillShowNotification(notification);
            });

            hideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) =>
            {
                KeyboardWillHideNotification(notification);
            });
        }

        void deregisterFromKeyboardNotifications()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(showObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(hideObserver);
        }

        protected virtual UIView KeyboardGetActiveView()
        {
            return this.View.FindFirstResponder();
        }

        protected virtual void KeyboardWillShowNotification(NSNotification notification)
        {
            UIView activeView = KeyboardGetActiveView();

            if (activeView == null)
                return;
            UIScrollView scrollView = dialogueView;

            UIScrollView tmpScrollView = activeView.FindSuperviewOfType(this.View, typeof(UIScrollView)) as UIScrollView;
            if (tmpScrollView == null)
                return;
          
            CGRect keyboardBounds = UIKeyboard.BoundsFromNotification(notification);


            UIEdgeInsets contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardBounds.Size.Height, 0.0f);
            scrollView.ContentInset = contentInsets;
            scrollView.ScrollIndicatorInsets = contentInsets;
            CGRect viewRectAboveKeyboard = new CGRect(this.View.Frame.Location, new CGSize(this.View.Frame.Width, this.View.Frame.Size.Height - keyboardBounds.Size.Height));
            CGRect activeFieldAbsoluteFrame = activeView.Superview.ConvertRectToView(activeView.Frame, this.View);
            if (!viewRectAboveKeyboard.Contains(activeFieldAbsoluteFrame) || true)
            {
                CGPoint scrollPoint = CGPoint.Empty;
                scrollPoint = new CGPoint(0.0f, 150.0f);
                if (scrollPoint.Y < dialogueView.ContentSize.Height)
                {
                    dialogueView.SetContentOffset(scrollPoint, true);
                }
                else
                {
                    dialogueView.SetContentOffset(scrollPoint, true);
                }
            }
        }

        protected virtual void KeyboardWillHideNotification(NSNotification notification)
        {
            UIView activeView = KeyboardGetActiveView();

            if (activeView == null)
                return;

            UIScrollView scrollView = activeView.FindSuperviewOfType(this.View, typeof(UIScrollView)) as UIScrollView;
            if (scrollView == null)
                return;
            double animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);
            UIEdgeInsets contentInsets = new UIEdgeInsets(0.0f, 0.0f, 0.0f, 0.0f);
            UIView.Animate(animationDuration, delegate
            {
                dialogueView.ContentInset = contentInsets;
                dialogueView.ScrollIndicatorInsets = contentInsets;
            });
        }


        void opendialog(CGPoint origin, UIView dialogView)
        {
            dialogView.Frame = new CGRect(origin.X, origin.Y, dialogView.Frame.Width, dialogView.Frame.Height);

            UIView viw = dialogueView.ViewWithTag(999);
            if (viw == null)
                dialogView.Tag = 999;
            else
                dialogView.Tag = 1000;
            dialogueView.AddSubview(dialogView);
            dialogueView.Hidden = false;
            registerForKeyboardNotifications();
            rootView.BringSubviewToFront(dialogueView);
        }

        public void closeDialogue()
        {
            UIView iviw = dialogueView.ViewWithTag(1000);
            if (iviw != null)
            {
                iviw.RemoveFromSuperview();
                iviw = null;
                return;
            }

            UIView viw = dialogueView.ViewWithTag(999);
            if (viw == null)
                return;
            viw.RemoveFromSuperview();
            viw = null;
            dialogueView.Hidden = true;
            deregisterFromKeyboardNotifications();
            rootView.SendSubviewToBack(dialogueView);
        }

        void removeDetailByTap(UIGestureRecognizer recognizer)
        {
            removeDetail();
        }

        public void removeDetail()
        {

            if (navigationBarEnable)
            {
                menuViewNavigationController = null;
            }
            else
            {
                stackScrollViewController = null;
            }

            if (detailStackView.Frame.X == rootView.Frame.Size.Width - detailMAXWidth)
            {
                UIView.Animate(0.25, () =>
                {
                    rightSlideOpacityView.Alpha = 0.0f;
						leftShadowForDetailStackView.Alpha = 0.0f;
                    detailStackView.Frame = new CGRect(rootView.Frame.Size.Width, 0, detailMAXWidth, View.Frame.Size.Height);
                });
            }
            rootView.SendSubviewToBack(rightSlideOpacityView);
            NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.DETAIL_CLOSING, null);
        }

        public void openDetail(UIViewController detailController, UIViewController fromController, bool isStartSatck)
        {
            rootView.BringSubviewToFront(rightSlideOpacityView);
            rootView.BringSubviewToFront(detailStackView);
			rootView.BringSubviewToFront (menuViewController.bottonTabView);
			rootView.BringSubviewToFront (leftMenuView);
            detailController.View.Frame = new CGRect(0, 0, detailStackView.Frame.Size.Width, detailStackView.Frame.Size.Height);
            if (navigationBarEnable)
            {
                if (menuViewNavigationController == null)
                {
                    menuViewNavigationController = new UINavigationController(detailController);
                    detailController.NavigationController.NavigationBar.Translucent = false;
                    menuViewNavigationController.View.Frame = new CGRect(0, 0, detailStackView.Frame.Size.Width, detailStackView.Frame.Size.Height);
                    detailStackView.AddSubview(menuViewNavigationController.View);
                }
                else
                {
					if(fromController!=null)
					if(fromController.NavigationController!=null)
                    fromController.NavigationController.PushViewController(detailController, true);
                }

            }
            else
            {
                if (isStartSatck)
                {
                    stackScrollViewController = new StackViewController(false);
                    stackScrollViewController.View.Frame = new CGRect(0, 0, detailStackView.Frame.Size.Width, detailStackView.Frame.Size.Height);
                    stackScrollViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    stackScrollViewController.ViewWillAppear(false);
                    stackScrollViewController.ViewDidAppear(false);
                    stackScrollViewController.View.BackgroundColor = UIColor.LightTextColor;
                    detailStackView.AddSubview(stackScrollViewController.View);
                    stackScrollViewController.addViewInSlider(detailController, fromController, true);
                }
                else
                {
                    stackScrollViewController.addViewInSlider(detailController, fromController, false);
                }
            }
            UIView.Animate(0.25, () =>
            {
                rightSlideOpacityView.Alpha = 1.0f;
					leftShadowForDetailStackView.Alpha = 1.0f;
                detailStackView.Frame = new CGRect(rootView.Frame.Size.Width - detailMAXWidth, 0, detailMAXWidth, View.Frame.Size.Height);
            });
        }

		public void OpenWebViewInDetail(NSString url)
        {
        }
    }
}