using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS.Views
{
    public class VenueCenterCell : UITableViewCell
    {
        UILabel nameLabel, infoLabel;
		static nfloat nameLabelYpadding = 10;
		static nfloat nameLabelLeftPadding = 15;
		static nfloat infoLabelLeftPadding = 15;
		static nfloat infoTopPadding = 10;
		static nfloat nameLabelRightPadding = 20;
        public VenueCenterCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = AppTheme.VCCContentviewBackgroundColor;

            nameLabel = new UILabel()
            {
                TextColor = AppTheme.VCCnameLabelTextColor,
				Font = AppTheme.FDTitleTextFont,
                HighlightedTextColor = AppTheme.VCCnameLabelHighlightedTextColor,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            infoLabel = new UILabel()
            {
                Lines = 2,
                LineBreakMode = UILineBreakMode.TailTruncation,
                TextColor = AppTheme.VCCinfoLabelTextColor,
				Font = AppTheme.VNDetailTextFont,
                HighlightedTextColor = AppTheme.VCCinfoLabelHighlightedTextColor,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            this.SelectedBackgroundView = new UIView(this.Frame);
            this.SelectedBackgroundView.BackgroundColor = AppTheme.VCCCellBackgroundColor;
            ContentView.AddSubviews(nameLabel,infoLabel);

        }

        public void UpdateCell(BuiltVenue venue)
        {
            nameLabel.Text = venue.name;
            infoLabel.Text = venue.info;
            nameLabel.LineBreakMode = UILineBreakMode.WordWrap;
            nameLabel.Lines = 0;
        }

        public override void LayoutSubviews()
        {
           
            base.LayoutSubviews();
            nameLabel.Frame = new CGRect(nameLabelLeftPadding, nameLabelYpadding, ContentView.Frame.Width - nameLabelRightPadding, 0);
            nameLabel.SizeToFit();
            nameLabel.Frame = new CGRect(nameLabelLeftPadding, nameLabelYpadding, ContentView.Frame.Width - (nameLabelLeftPadding+nameLabelRightPadding), nameLabel.Frame.Height);
            infoLabel.Frame = new CGRect(infoLabelLeftPadding, nameLabel.Frame.Bottom, ContentView.Frame.Width - (nameLabelRightPadding + infoLabelLeftPadding), 0);
            infoLabel.SizeToFit();
			infoLabel.Frame = new CGRect(infoLabelLeftPadding, nameLabel.Frame.Bottom + infoTopPadding, ContentView.Frame.Width - (nameLabelRightPadding + infoLabelLeftPadding), infoLabel.Frame.Height);

        }
    }
}