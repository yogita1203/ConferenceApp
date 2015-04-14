using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using CoreGraphics;

namespace ConferenceAppiOS.CustomControls
{
    class ToastView : UIView
    {
        UILabel _textLabel;

		static nfloat ToastHeight = 50.0f;
		static nfloat StatusToastHeight = 150.0f;
		static nfloat ToastGap = 10.0f;

        public ToastView(CGRect rect)
        {
            base.Frame = rect;
        }

        public UILabel textLabel
        {
            get
            {

                if (_textLabel == null)
                {
                    _textLabel = new UILabel(new CGRect(5, 5, Frame.Size.Width - 10, Frame.Size.Height - 10));
                    _textLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    _textLabel.BackgroundColor = UIColor.Clear;
                    _textLabel.TextAlignment = UITextAlignment.Center;
                    _textLabel.TextColor = UIColor.White;
                    _textLabel.Lines = 2;
					_textLabel.Font = AppFonts.ProximaNovaRegular(14);
                    _textLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
                    AddSubview(_textLabel);

                }
                return _textLabel;
            }
        }

        public static void ShowToastInView(UIView parentView, string text, nfloat duration)
        {
            int toastsAlreadyInParent = 0;

            foreach (UIView subView in parentView.Subviews)
            {
                if (subView.GetType() == typeof(ToastView))
                {
                    toastsAlreadyInParent++;
                }
            }

            var parentFrame = parentView.Frame;

            nfloat yOrigin = parentFrame.Size.Height - (70 + ToastHeight * toastsAlreadyInParent + ToastGap * toastsAlreadyInParent);

            CGRect selfFrame = new CGRect(parentFrame.X + 20, yOrigin, parentFrame.Size.Width - 40, ToastHeight);
            ToastView toast = new ToastView(selfFrame);
            toast.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
            toast.BackgroundColor = UIColor.DarkGray;
            toast.Alpha = 0.0f;
            toast.Layer.CornerRadius = 4;
            toast.textLabel.Text = text;

            parentView.AddSubview(toast);

            UIView.Animate(0.4f, () =>
            {
                toast.Alpha = 0.9f;
                toast.textLabel.Alpha = 0.9f;
            }, () =>
            {
               
            });

            toast.PerformSelector(new Selector("hideSelf"), null, duration);
        }

        public static void ShowToast(string text, nfloat duration)
        {
            //UIWindow window = UIApplication.SharedApplication.KeyWindow;
            //UIView parentView = window.RootViewController.View;
            UIView parentView = AppDelegate.instance().rootViewController.View;
            int toastsAlreadyInParent = 0;

            foreach (UIView subView in parentView.Subviews)
            {
                if (subView.GetType() == typeof(ToastView))
                {
                    toastsAlreadyInParent++;
                }
            }

            var parentFrame = parentView.Frame;

            nfloat yOrigin = parentFrame.Size.Height - (100 + ToastHeight * toastsAlreadyInParent + ToastGap * toastsAlreadyInParent);

            CGRect selfFrame = new CGRect(parentFrame.X + 20, yOrigin, parentFrame.Size.Height - 40, ToastHeight);
            ToastView toast = new ToastView(selfFrame);
            toast.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
            toast.BackgroundColor = UIColor.DarkGray;
            toast.Alpha = 0.0f;
            toast.Layer.CornerRadius = 4;
            toast.textLabel.Text = text;
            var textSize = new NSString(toast.textLabel.Text).WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(toast.textLabel.Font, UIStringAttributeKey.Font));
            var strikeWidth = textSize.Width + 60;
            toast.Frame = new CGRect((parentFrame.Width / 2) - (strikeWidth / 2), yOrigin, strikeWidth, ToastHeight);
            //parentView.AddSubview(toast);
            UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(toast);

            UIView.Animate(0.4f, () =>
            {
                toast.Alpha = 0.9f;
                toast.textLabel.Alpha = 0.9f;
            }, () =>
            {
               
            });

            toast.PerformSelector(new Selector("hideSelf"), null, duration);
        }

        [Export("hideSelf")]
        void hideSelf()
        {
            UIView.Animate(0.4, () =>
            {
                this.Alpha = 0;
                this.textLabel.Alpha = 0;
            }, () =>
            {
                this.RemoveFromSuperview();
            });
        }
    }

    class HUDView : UIView
    {
        UILabel _textLabel;
        public UIImageView _imgStatus;

		static nfloat ToastHeight = 100.0f;
		static nfloat ToastGap = 10.0f;

        public HUDView(CGRect rect)
        {
            base.Frame = rect;
        }

        public UIImageView imgStatus
        {
            get
            {

                if (_imgStatus == null)
                {
                    _imgStatus = new UIImageView() 
                    {
                        BackgroundColor = UIColor.Clear
                    };
                    _imgStatus.Frame = new CGRect((Frame.Size.Width / 2) - 30, 20, 40, 40);
                    _imgStatus.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                }
                AddSubview(_imgStatus);
                return _imgStatus;
            }
        }

        public UILabel textLabel
        {
            get
            {

                if (_textLabel == null)
                {
                    _textLabel = new UILabel(new CGRect(5, _imgStatus.Frame.Bottom + 10, Frame.Size.Width - 10, Frame.Size.Height - 80));
                    _textLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    _textLabel.BackgroundColor = UIColor.Clear;
                    _textLabel.TextAlignment = UITextAlignment.Center;
                    _textLabel.TextColor = UIColor.White;
                    _textLabel.Lines = 2;
					_textLabel.Font = AppFonts.ProximaNovaRegular(14);
                    _textLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
                    AddSubview(_textLabel);
                }
                return _textLabel;
            }
        }

        public static void ShowSuccess(string text, nfloat duration)
        {
            //UIWindow window = UIApplication.SharedApplication.KeyWindow;
            //UIView parentView = window.RootViewController.View;
            UIView parentView = AppDelegate.instance().rootViewController.View;

            var parentFrame = parentView.Frame;
            CGRect selfFrame = new CGRect(parentFrame.X + 20, 0, parentFrame.Size.Height - 40, ToastHeight);
            HUDView toast = new HUDView(selfFrame);
            toast.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
            toast.BackgroundColor = UIColor.DarkGray;
            toast.Alpha = 0.0f;
            toast.Layer.CornerRadius = 4;
            toast.imgStatus.Image = UIImage.FromBundle(AppTheme.checkMarkImg);
            toast.textLabel.Text = text;

            var textSize = new NSString(toast.textLabel.Text).WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(toast.textLabel.Font, UIStringAttributeKey.Font));
            var strikeWidth = textSize.Width + 60;
            toast.Frame = new CGRect((parentFrame.Height / 2) - (strikeWidth / 2), (parentFrame.Width / 2) - ToastHeight / 2, strikeWidth, ToastHeight);

            parentView.AddSubview(toast);

            UIView.Animate(0.4f, () =>
            {
                toast.Alpha = 0.9f;
                toast.textLabel.Alpha = 0.9f;
            }, () =>
            {
            });

            toast.PerformSelector(new Selector("hideSelf"), null, duration);
        }

        public static void ShowError(string text, nfloat duration)
        {
            //UIWindow window = UIApplication.SharedApplication.KeyWindow;
            //UIView parentView = window.RootViewController.View;
            UIView parentView = AppDelegate.instance().rootViewController.View;
            
            var parentFrame = parentView.Frame;
            CGRect selfFrame = new CGRect(parentFrame.X + 20, 0, parentFrame.Size.Height - 40, ToastHeight);
            HUDView toast = new HUDView(selfFrame);
            toast.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
            toast.BackgroundColor = UIColor.DarkGray;
            toast.Alpha = 0.0f;
            toast.Layer.CornerRadius = 4;
            toast.imgStatus.Image = UIImage.FromBundle("x_alt.png");
            toast.textLabel.Text = text;

            var textSize = new NSString(toast.textLabel.Text).WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(toast.textLabel.Font, UIStringAttributeKey.Font));
            var strikeWidth = textSize.Width + 60;
            toast.Frame = new CGRect((parentFrame.Height / 2) - (strikeWidth / 2), (parentFrame.Width / 2) - ToastHeight / 2, strikeWidth, ToastHeight);

            parentView.AddSubview(toast);

            UIView.Animate(0.4f, () =>
            {
                toast.Alpha = 0.9f;
                toast.textLabel.Alpha = 0.9f;
            }, () =>
            {
            });

            toast.PerformSelector(new Selector("hideSelf"), null, duration);
        }

        [Export("hideSelf")]
        void hideSelf()
        {
            UIView.Animate(0.4, () =>
            {
                this.Alpha = 0;
                this.textLabel.Alpha = 0;
                this.imgStatus.Alpha = 0;
            }, () =>
            {
                this.RemoveFromSuperview();
            });
        }
    }

    class LoadingView : UIView
    {
        UILabel _textLabel;
        UIActivityIndicatorView _activitySpinner;

		static nfloat ToastHeight = 100.0f;
		static nfloat ToastGap = 10.0f;

        public LoadingView(CGRect rect)
        {
            base.Frame = rect;
        }

        public UIActivityIndicatorView ActivitySpinner
        {
            get
            {
                if (_activitySpinner == null)
                {
                    _activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                    _activitySpinner.Frame = new CGRect((Frame.Size.Width / 2) - 30, 20, 40, 40);
                    _activitySpinner.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
                    CGAffineTransform transform = CGAffineTransform.MakeScale(1.5f, 1.5f);
                    _activitySpinner.Transform = transform;
                    AddSubview(_activitySpinner);
                }
                return _activitySpinner;
            }
        }

        public UILabel textLabel
        {
            get
            {

                if (_textLabel == null)
                {
                    _textLabel = new UILabel(new CGRect(5, _activitySpinner.Frame.Bottom + 10, Frame.Size.Width - 10, Frame.Size.Height - 80));
                    _textLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                    _textLabel.BackgroundColor = UIColor.Clear;
                    _textLabel.TextAlignment = UITextAlignment.Center;
                    _textLabel.TextColor = UIColor.White;
                    _textLabel.Lines = 2;
					_textLabel.Font = AppFonts.ProximaNovaRegular(14);
                    _textLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
                    AddSubview(_textLabel);
                }
                return _textLabel;
            }
        }

        static LoadingView currentView;
        public static void Show(string text)
        {
//            UIWindow window = UIApplication.SharedApplication.KeyWindow;
			UIView parentView = AppDelegate.instance().rootViewController.View;

            var parentFrame = parentView.Frame;
            CGRect selfFrame = new CGRect(parentFrame.X + 20, 0, parentFrame.Size.Height - 40, ToastHeight);

            if (currentView == null)
            {
                currentView = new LoadingView(selfFrame);
                currentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
                currentView.BackgroundColor = AppTheme.LoaderBackgroundColor;
                currentView.Alpha = 0.0f;
                currentView.Layer.CornerRadius = 4;
            }


            if (String.IsNullOrWhiteSpace(text))
            {
                var frame = currentView.ActivitySpinner.Frame;
                frame.Y = 0;
                frame.Height = ToastHeight;
                currentView.ActivitySpinner.Frame = frame;
            }
            else
            {
                currentView.ActivitySpinner.Frame = new CGRect((currentView.Frame.Size.Width / 2) - 30, 20, 40, 40);
            }

            currentView.ActivitySpinner.StartAnimating();
            currentView.textLabel.Text = text;

            var textSize = new NSString(currentView.textLabel.Text).WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(currentView.textLabel.Font, UIStringAttributeKey.Font));
            var strikeWidth = textSize.Width + 120;
			currentView.Frame = new CGRect((parentFrame.Width / 2) - (strikeWidth / 2), (parentFrame.Height / 2) - ToastHeight / 2, strikeWidth, ToastHeight);

            //parentView.AddSubview(currentView);
            UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(currentView);

            UIView.Animate(0.4f, () =>
            {
                currentView.Alpha = 0.9f;
                currentView.textLabel.Alpha = 0.9f;
                currentView.ActivitySpinner.Alpha = 0.9f;
            }, () =>
            {
                parentView.UserInteractionEnabled = false;
            });
        }

		public static void ShowInView(string text, UIView baseView)
		{
			UIView parentView = AppDelegate.instance().rootViewController.View;

			var parentFrame = parentView.Frame;
			CGRect selfFrame = new CGRect(parentFrame.X + 20, (parentFrame.Size.Height - ToastHeight)/2, parentFrame.Size.Width - 40, ToastHeight);

			if (currentView == null)
			{
				currentView = new LoadingView(selfFrame);
				currentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;
				currentView.BackgroundColor = AppTheme.LoaderBackgroundColor;
				currentView.Alpha = 0.0f;
				currentView.Layer.CornerRadius = 4;
			}


			if (String.IsNullOrWhiteSpace(text))
			{
				var frame = currentView.ActivitySpinner.Frame;
				frame.Y = 0;
				frame.Height = ToastHeight;
				currentView.ActivitySpinner.Frame = frame;
			}
			else
			{
				currentView.ActivitySpinner.Frame = new CGRect((currentView.Frame.Size.Width / 2) - 30, 20, 40, 40);
			}

			currentView.ActivitySpinner.StartAnimating();
			currentView.textLabel.Text = text;

			var textSize = new NSString(currentView.textLabel.Text).WeakGetSizeUsingAttributes(NSDictionary.FromObjectAndKey(currentView.textLabel.Font, UIStringAttributeKey.Font));
			var strikeWidth = textSize.Width + 120;
			currentView.Frame = new CGRect((parentFrame.Width / 2) - (strikeWidth / 2), (parentFrame.Height / 2) - ToastHeight / 2, strikeWidth, ToastHeight);

			parentView.AddSubview(currentView);

			UIView.Animate(0.4f, () =>
				{
					currentView.Alpha = 0.9f;
					currentView.textLabel.Alpha = 0.9f;
					currentView.ActivitySpinner.Alpha = 0.9f;
				}, () =>
				{
					parentView.UserInteractionEnabled = false;
				});
		}

		public static void DismissFromView (UIView baseview)
		{
			UIView parentView = baseview;
			UIView.Animate(0.4, () =>
				{
					if (currentView != null)
					{
						currentView.ActivitySpinner.StopAnimating();
						currentView.Alpha = 0;
						currentView.textLabel.Alpha = 0;
						currentView.ActivitySpinner.Alpha = 0;
					}
				}, () =>
				{
					parentView.UserInteractionEnabled = true;
					if (currentView != null)
						currentView.RemoveFromSuperview();
				});
		}

        public static void Dismiss()
        {
			UIView parentView = AppDelegate.instance().rootViewController.View;
            UIView.Animate(0.4, () =>
            {
                if (currentView != null)
                {
                    currentView.ActivitySpinner.StopAnimating();
                    currentView.Alpha = 0;
                    currentView.textLabel.Alpha = 0;
                    currentView.ActivitySpinner.Alpha = 0;
                }
            }, () =>
            {
                parentView.UserInteractionEnabled = true;
                if (currentView != null)
                    currentView.RemoveFromSuperview();
            });
        }
    }
}