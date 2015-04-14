using System;
using UIKit;
using CoreGraphics;

namespace ConferenceAppiOS
{
	public class NavigationBar : UIView
	{

		public UIButton _rightButton;
		public UIButton rightButton { 
			get{ 
				if (_rightButton == null) {
					_rightButton = new UIButton ();

					AddSubview (rightButton);
				}
				return _rightButton;
			}
			set{ 
				_rightButton = value;
			}
		}

		private UIButton _leftButton;
		public UIButton leftButton { 
			get{
				if(_leftButton == null){
					_leftButton = new UIButton ();

					AddSubview (_leftButton);
				}
				return _leftButton;
			}
			set{ 
				_rightButton = value;
			}
		}

		public UIView _titleView;

		public UIView titleView { 
			get{
				if(_titleView == null){
					UILabel titleLable = new UILabel ();
					titleLable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
					titleLable.Text = "VMworld 2014";
					titleLable.AdjustsFontSizeToFitWidth = true;

					UILabel subTitleLable = new UILabel ();
					subTitleLable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
					subTitleLable.Text = "Last Sync";
					subTitleLable.AdjustsFontSizeToFitWidth = true;
					_titleView = new UIView ();

					titleLable.Frame = new CGRect (0,0, _titleView.Frame.Width,15);
					_titleView.AddSubview (titleLable);
					subTitleLable.Frame = new CGRect (0,15, _titleView.Frame.Width,15);
					_titleView.AddSubview (subTitleLable);

					AddSubview (_titleView);
				}
				return _titleView;
			}
		}

		public NavigationBar (CGRect rect)
		{
			Frame = rect;

			rightButton.BackgroundColor = UIColor.White;

			leftButton.BackgroundColor = UIColor.White;

			titleView.BackgroundColor = UIColor.White;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			titleView.SizeToFit ();

			rightButton.Frame = new CGRect (20,Frame.Size.Height - 40,30,30);
			leftButton.Frame = new CGRect (Frame.Width - 40,Frame.Size.Height - 40,30,30);
			titleView.Frame = new CGRect (Frame.Width / 2 - ((titleView.Frame.Width == 0) ? 50 : titleView.Frame.Width / 2), Frame.Size.Height - 40, (titleView.Frame.Width == 0) ? 100 : titleView.Frame.Width , 30);
		}
	}
}

