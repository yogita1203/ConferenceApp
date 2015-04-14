using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using CoreAnimation;

namespace ConferenceAppiOS
{
    public class SessionSpeakerCell : UITableViewCell
    {
        UIButton arrow;
        UILabel nameLabel, roleLabel;
        
        public SessionSpeakerCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = AppTheme.viewBackground;

            nameLabel = new UILabel()
            {
                TextColor = AppTheme.nameLabel,
                HighlightedTextColor = AppTheme.MenuHighlightedText,
                BackgroundColor = AppTheme.SSCnameLabelBackground,
                Font = AppFonts.ProximaNovaRegular (16)
            };
        

            roleLabel = new UILabel()
            {
                TextColor = AppTheme.roleLabel,
                BackgroundColor = AppTheme.SSCroleLabelBackground,
				Font = AppFonts.ProximaNovaRegular(14)
            };

            arrow = new UIButton();
            arrow.SetTitleColor(AppTheme.SSCForwardNormalColor, UIControlState.Normal);
            arrow.SetTitle(AppTheme.SSCforward, UIControlState.Normal);
            arrow.SetTitle(AppTheme.SSCforward, UIControlState.Selected);
            arrow.Font = AppTheme.SSCForwardFont;

            ContentView.Add(nameLabel);
            ContentView.Add(roleLabel);
            //ContentView.Add(arrow);
        }

        public void UpdateCell(BuiltSessionSpeaker speaker)
        {
            nameLabel.Text = speaker.full_name;
            roleLabel.Text = speaker.roles;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            nameLabel.Frame = new CGRect(40, 10, ContentView.Frame.Width - 60, 20);
            var frame = nameLabel.Frame;
            frame.Y = nameLabel.Frame.Bottom;
            roleLabel.Frame = frame;
            arrow.Frame = new CGRect(ContentView.Frame.Right - 40, Helper.getCenterY(ContentView.Frame.Height, 20), 20, 20);
        }
    }
}