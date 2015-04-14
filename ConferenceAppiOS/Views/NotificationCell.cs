using System;
using CoreGraphics;
using Foundation;
using UIKit;
using SDWebImage;
using ConferenceAppiOS.Helpers;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
	public class NotificationCell : UITableViewCell
	{
		nfloat leftMargin = 21;
		nfloat rightMargin = 21;
		nfloat topMargin = 16;

		BuiltEventNotifications notification;
		UILabel headingLabel, subheadingLabel;
		public NotificationCell(NSString cellId)
			: base(UITableViewCellStyle.Default, cellId)
		{
            BackgroundColor = AppTheme.NVbackgroundColor;

			var selectedBackgroundView = new UIView();
            selectedBackgroundView.BackgroundColor = AppTheme.NVbackgroundColor;
			SelectedBackgroundView = selectedBackgroundView;

			headingLabel = new UILabel()
			{
                TextColor = AppTheme.NVcellTextColor,
                HighlightedTextColor = AppTheme.NVcellTextColor,
				BackgroundColor = UIColor.Clear,
				Font = AppTheme.NVcellTextFont,

			};

			subheadingLabel = new UILabel()
			{
                TextColor = AppTheme.NVdescriptionTextColor,
                HighlightedTextColor = AppTheme.NVdescriptionTextColor,
				BackgroundColor = UIColor.Clear,
                Font = AppTheme.NVdescriptionFont,
			};

			ContentView.Add(headingLabel);
			ContentView.Add(subheadingLabel);
		}

		public void UpdateCell(BuiltEventNotifications model)
		{
			notification = model;
			headingLabel.Text = notification.title;
			subheadingLabel.Text = notification.desc;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			headingLabel.Frame = new CGRect (leftMargin,topMargin,ContentView.Frame.Size.Width-(leftMargin+rightMargin),0);
			headingLabel.Lines = 0;
			headingLabel.SizeToFit ();
			headingLabel.Frame = new CGRect (leftMargin,topMargin,ContentView.Frame.Size.Width-(leftMargin+rightMargin),headingLabel.Frame.Size.Height);
			subheadingLabel.Frame = new CGRect (leftMargin,headingLabel.Frame.Bottom,ContentView.Frame.Size.Width-(leftMargin+rightMargin),0);
			subheadingLabel.Lines = 0;
			subheadingLabel.SizeToFit ();
			subheadingLabel.Frame = new CGRect (leftMargin,headingLabel.Frame.Bottom+topMargin/2,ContentView.Frame.Size.Width-(leftMargin+rightMargin),subheadingLabel.Frame.Size.Height);
		}
	}
}

