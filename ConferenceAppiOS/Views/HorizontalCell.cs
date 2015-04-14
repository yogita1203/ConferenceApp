using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using ConferenceAppiOS.Helpers;
using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using CoreAnimation;

namespace ConferenceAppiOS
{
	public class HorizontalCell : UIView
    {
        public BuiltSessionTime model;
        public UILabel lblSessionName, lblRoom, lbldate, lblRoomTime;
        public UIView trackView, topView, wholeView; BuiltTracks builtTracks = null;
		static nfloat leftPaddingForCell = 12; public CALayer TrackColor;
		public HorizontalCell(CGRect rectangle)
        {
			Frame = rectangle;

            BackgroundColor = UIColor.White;
            TrackColor = new CALayer();
            lblSessionName = new UILabel()
            {
				Font = AppFonts.ProximaNovaRegular (15),
                BackgroundColor = UIColor.Clear,
                HighlightedTextColor = AppTheme.HClblSessionNameHighlightedTextColor,
				Tag = 1
            };
            lbldate = new UILabel()
            {
                TextColor = AppTheme.HClbldateTextColor,
				BackgroundColor = UIColor.Clear,
                HighlightedTextColor = AppTheme.HClbldateHighlightedTextColor,
				Font = AppFonts.ProximaNovaRegular (12),
				Tag = 2
            };
            lblRoom = new UILabel()
            {
                TextColor = AppTheme.HClblRoomTextColor,
				BackgroundColor = UIColor.Clear,
                HighlightedTextColor = AppTheme.HClblRoomHighlightedTextColor,
				Font = AppFonts.ProximaNovaRegular (12),
				Tag = 3
            };
            lblRoomTime = new UILabel()
            {
                TextColor = AppTheme.HClblRoomTimeTextColor,
				BackgroundColor = UIColor.Clear,
                HighlightedTextColor = AppTheme.HClblRoomTimeHighlightedTextColor,
				Font = AppFonts.ProximaNovaRegular (12),
				Tag = 4
            };
            
            topView = new UIView();
            trackView = new UIView();
            topView.AddSubviews(lblSessionName, lbldate, lblRoom, lblRoomTime);
            wholeView = new UIView();
            wholeView.AddSubview(topView);
            wholeView.Layer.AddSublayer(TrackColor);	
			wholeView.Layer.CornerRadius = 1.0f;
			wholeView.Layer.BorderColor = UIColor.Clear.FromHexString (AppTheme.LineColor, 1.0f).CGColor;
			wholeView.Layer.BorderWidth = 1.0f;
			wholeView.BackgroundColor = AppTheme.NUCellBackground;
            AddSubview(wholeView);

        }
        public void UpdateCell(BuiltSessionTime builtSessionTime, List<BuiltTracks> allTracks)
        {
            model = builtSessionTime;
			if (builtSessionTime.BuiltSession != null && builtSessionTime.BuiltSession.track != null)
            {
                builtTracks = allTracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);
            }
            if (builtTracks == null)
            {
                builtTracks = allTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }
            lblSessionName.Text = builtSessionTime.BuiltSession.title;
			lbldate.Text = Helper.convertToTodayTomorrowDate(builtSessionTime.date) + ", "+Helper.convertToStartEndDate(builtSessionTime.time, builtSessionTime.length);
			lblRoomTime.Text = builtSessionTime.room;
            if (builtTracks != null)
            {
                TrackColor.BackgroundColor = UIColor.Clear.FromHexString(builtTracks.color, 1.0f).CGColor; 
            }
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat leftWidth = Frame.Width - 20;
            nfloat leftCellPadding = 21;
            nfloat CellTopPadding = 10;
            nfloat sessionNameRightPadding = 40;
            nfloat trackViewheight = 70;

            topView.Frame = new CGRect(0, 0, leftWidth, trackViewheight);
            TrackColor.Frame = new CGRect(1, 1, 5, trackViewheight-1);
            lblSessionName.Frame = new CGRect(leftCellPadding, CellTopPadding, Frame.Width - sessionNameRightPadding, 20);
			lblRoomTime.Frame = new CGRect(leftCellPadding, lblSessionName.Frame.Bottom + 1, Frame.Width - sessionNameRightPadding, 0);
			lbldate.Frame = new CGRect(leftCellPadding, lblRoomTime.Frame.Bottom + 1, Frame.Width - sessionNameRightPadding, 0);
            lbldate.SizeToFit();
            lblRoomTime.SizeToFit();
			lblRoomTime.Frame = new CGRect(leftCellPadding, lblSessionName.Frame.Bottom + 1, Frame.Width - sessionNameRightPadding, lblRoomTime.Frame.Size.Height);
			lbldate.Frame = new CGRect(leftCellPadding, lblRoomTime.Frame.Bottom + 1, Frame.Width - sessionNameRightPadding, lbldate.Frame.Size.Height);

            wholeView.Frame = new CGRect(0, 0, 340, trackViewheight);

        }
    }    
}