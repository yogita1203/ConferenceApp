using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using CoreGraphics;

namespace ConferenceAppiOS
{
    class SpeakerDataCell : UITableViewCell
    {
         UILabel nameLabel, roleLabel;
         public SpeakerDataCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.White;
            
            nameLabel = new UILabel()
            {
                TextColor = UIColor.Black,
                HighlightedTextColor = AppTheme.MenuHighlightedText,
                BackgroundColor = UIColor.Clear,
				Font = AppFonts.ProximaNovaRegular(18)
            };

            roleLabel = new UILabel()
            {
                TextColor = UIColor.Black,
               
                BackgroundColor = UIColor.Clear
            };

            ContentView.Add(nameLabel);
            ContentView.Add(roleLabel);
        }

         public void UpdateCell(BuiltSpeaker speaker)
         {
             nameLabel.Text = string.Format("{0} {1}", speaker.first_name, speaker.last_name);
             roleLabel.Text = speaker.company_name;
         }

         public override void LayoutSubviews()
         {
             base.LayoutSubviews();
             nameLabel.Frame = new CGRect(40, 20, ContentView.Frame.Width - 60, 30);
             var frame = nameLabel.Frame;
             frame.Y = nameLabel.Frame.Bottom;
             roleLabel.Frame = frame;
         }
    }
}