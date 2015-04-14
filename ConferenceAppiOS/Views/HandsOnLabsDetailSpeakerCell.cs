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
    public class HandsOnLabsDetailSpeakerCell : UITableViewCell
    {
       
        UILabel nameLabel;
        public HandsOnLabsDetailSpeakerCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
			BackgroundColor = AppTheme.HOLDSCBGColor;
			SelectionStyle = UITableViewCellSelectionStyle.None;
            nameLabel = new UILabel()
            {
				TextColor = AppTheme.HOLDSCnameLabelColor,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular (16)
            };

            ContentView.Add(nameLabel);           
           
        }

        public void UpdateCell(string speakerName)
        {
            if(speakerName !=null)
            nameLabel.Text = speakerName;
          
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            nameLabel.Frame = new CGRect(30, 20, ContentView.Frame.Width - 60, 20);
            
        }
    }
}