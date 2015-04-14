using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class SocialMediaCell : UITableViewCell
    {
        private UILabel _linkTitle;
        public UILabel LinkTitle
        {
            get
            {
                if (_linkTitle == null)
                {
                    _linkTitle = new UILabel
                    {
                        HighlightedTextColor = UIColor.White,
                        TextColor = AppTheme.LinkTitle,
						Font = AppFonts.ProximaNovaRegular(16),
                    };
                    ContentView.Add(_linkTitle);
                }
                return _linkTitle;
            }
        }

        public SocialMediaCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            
        }

        public void updateCell(BuiltImportantLinks builtImportantLinks)
        {
            LinkTitle.Text = builtImportantLinks.title;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            LinkTitle.Frame = new CGRect(20, ContentView.Frame.Height / 2 - 25, ContentView.Frame.Width - 40, 50);
            
        }
        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (Selected)
            {
				ContentView.BackgroundColor = AppTheme.CellSelectedbackgroundColor;
            }
        }

    }
}