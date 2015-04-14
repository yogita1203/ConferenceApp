using System;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace ConferenceAppiOS
{
    public class HorizontalTableView : UIView
    {
        UITableView _TableView;
        public UITableView TableView
        {
            get
            {
                if (_TableView == null)
                {
                    _TableView = new UITableView();
                    _TableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                    _TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                    _TableView.SeparatorColor = UIColor.Clear;
                    float rotationValue = (float)Math.PI * 1.5f;
                    _TableView.Transform = CGAffineTransform.MakeRotation(rotationValue);
                    AddSubview(_TableView);
                }
                return _TableView;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
			TableView.Frame = new CGRect(0, 5, Frame.Size.Width, Frame.Size.Height - 10);
        }

        public HorizontalTableView()
        {
			TableView.Frame = new CGRect(0, 5, Frame.Size.Width, Frame.Size.Height - 10);
        }
    }
}

