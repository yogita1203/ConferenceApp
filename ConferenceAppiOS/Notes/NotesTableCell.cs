using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS.Notes
{
    internal class NotesTableCell : UITableViewCell
    {
		static nfloat Margin = 15;
		static nfloat TopMargin = 10;
		static nfloat LabelHeight = 20;
        
		static nfloat TitleRightMargin = 150;

        UILabel _titleLabel;
        public UILabel TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel()
                    {
                        TextColor = AppTheme.NTnotesCellTitleColor,
                        HighlightedTextColor = AppTheme.NTcellHighlightedTextColor,
                        BackgroundColor = AppTheme.NTcellTitleBackColor,
                        Font = AppTheme.NTtitleFont,
                    };
                }
                return _titleLabel;
            }
        }
			
        UILabel _timeLabel;
        public UILabel TimeLabel
        {
            get
            {
                if (_timeLabel == null)
                {
                    _timeLabel = new UILabel()
                    {
                        TextColor = AppTheme.NotesCellTimeColor,
                        HighlightedTextColor = AppTheme.NTcellHighlightedTextColor,
                        BackgroundColor = AppTheme.NTcellTitleBackColor,
                        Font = AppTheme.NTtimeLabelFont,
						TextAlignment = UITextAlignment.Left
                    };
                }
                return _timeLabel;
            }
        }

        public NotesTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            ContentView.AddSubviews(TitleLabel, TimeLabel);

            SelectedBackgroundView = new UIView
            {
                BackgroundColor = AppTheme.NTCellSelectedColor
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
			TitleLabel.SizeToFit ();
			TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin*2), TitleLabel.Frame.Size.Height);
			TimeLabel.SizeToFit ();
			TimeLabel.Frame = new CGRect(Margin, TitleLabel.Frame.Bottom +TopMargin/2, ContentView.Frame.Width - (Margin*2), TimeLabel.Frame.Size.Height);
        }

        public void UpdateCell(BuiltNotes note)
        {
            TitleLabel.Text = note.title;
            TimeLabel.Text = Convert.ToDateTime(note.updated_at).ToString("MMM d, h:mm tt");
        }
    }
}