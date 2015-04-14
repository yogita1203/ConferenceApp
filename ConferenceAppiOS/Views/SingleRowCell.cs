
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;

namespace ConferenceAppiOS
{
    public class SingleRowCell : UITableViewCell
    {
        public UILabel textLabel;
        public SingleRowCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.White;
            SelectionStyle = UITableViewCellSelectionStyle.None;

            textLabel = new UILabel()
            {
				TextColor = AppTheme.SRCTextColor,
                Font = AppFonts.ProximaNovaRegular (16),
                BackgroundColor = UIColor.Clear
            };

            ContentView.Add(textLabel);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            textLabel.Frame = new CGRect(30, 0, ContentView.Frame.Width - 50, 40);
        }
    }
}