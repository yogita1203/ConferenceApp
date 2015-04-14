using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public class NoSessionsCell : UITableViewCell
    {

        public UILabel nameLabel, roleLabel;
		UIView lineView;
        public NoSessionsCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
			BackgroundColor = AppTheme.NSCviewBackground;
            nameLabel = new UILabel()
            {
                TextColor = AppTheme.NSCnameLabel,
                BackgroundColor = AppTheme.NSCnameLabelBackgroundColor,
				Font = AppFonts.ProximaNovaRegular (16),
            };
			lineView = new UIView ();
			lineView.BackgroundColor = UIColor.Clear.FromHexString (AppTheme.LineColor, 1.0f);
			ContentView.Add(lineView);
			SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.Add(nameLabel);
        }

        public void UpdateCell(string fromWhichtable)
        {
           if (fromWhichtable == "FeaturedSessions")
           {
               nameLabel.Text = AppTheme.noSessionCellTextForFeaturedSess;
           }else{
               nameLabel.Text = AppTheme.noSessionCellTextForMySchedule;
           }
            if(fromWhichtable ==AppTheme.noInterestCell)
            {
                nameLabel.Text = AppTheme.noInterestCell;
            }
            if (fromWhichtable == AppTheme.NextUpTextForNoSession)
            {
                nameLabel.Text = AppTheme.NoNextUpTextForNoSession;
            }

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
			lineView.Frame = new CGRect (0, 0, ContentView.Frame.Width, 1.0f);
            nameLabel.SizeToFit();
            nameLabel.Frame = new CGRect(ContentView.Frame.Width/2-(nameLabel.Frame.Size.Width)/2, ContentView.Frame.Height/2-(nameLabel.Frame.Height/2), nameLabel.Frame.Width,nameLabel.Frame.Height);
        }
    }
}