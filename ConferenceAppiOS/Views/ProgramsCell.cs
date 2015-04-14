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
    public class ProgramsCell : UITableViewCell
    {
        UILabel nameLabel;
        public ProgramsCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.White;

            nameLabel = new UILabel()
            {
                TextColor = AppTheme.ADcellTextColor,
                HighlightedTextColor = AppTheme.ADcellTextColorselcted,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular(16)
            };


            this.SelectedBackgroundView = new UIView(this.Frame);
            this.SelectedBackgroundView.BackgroundColor = AppTheme.PCCellBackgroundColor;
            ContentView.Add(nameLabel);
            
        }

        public void UpdateCell(BuiltOthers program)
        {
            nameLabel.Text = program.title;
           
        }

        public override void LayoutSubviews()
        {  
            nfloat nameLabeHeight = 20;
            nfloat nameLabelLeftPadding = 20;
            nfloat nameLabelRightPadding = 60;
            base.LayoutSubviews();
            nameLabel.Frame = new CGRect(nameLabelLeftPadding, (ContentView.Frame.Size.Height / 2) - (nameLabel.Frame.Height / 2), ContentView.Frame.Width - nameLabelRightPadding, 0);
            nameLabel.SizeToFit();
            nameLabel.Frame = new CGRect(nameLabelLeftPadding, (ContentView.Frame.Size.Height / 2) - (nameLabel.Frame.Height / 2), ContentView.Frame.Width - nameLabelRightPadding, nameLabeHeight);
            
        }
    }
}