using System;
using UIKit;
using CoreGraphics;

namespace ConferenceAppiOS
{
	public class LineView : UIView
	{
		public UIColor backgroundColor;
		public CGRect frm;
		public LineView (nfloat yPosition, nfloat height, UIView parentView)
		{
			frm = new CGRect (0, yPosition == null ? 0 : yPosition, parentView.Frame.Width, height == null ? 0 : height);
			BackgroundColor = AppTheme.LVBackgroundViewColor;

		}

		public LineView(CGRect frame){
			Frame = frame;
			BackgroundColor = UIColor.Clear.FromHexString(AppTheme.LineColor,1.0f);
		}
	}
}

