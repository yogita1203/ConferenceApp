using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using CoreGraphics;
using ConferenceAppiOS.Helpers;
using CoreAnimation;

namespace ConferenceAppiOS
{
    class SpeakerSessionsCell : UITableViewCell
    {
		static nfloat leftMargin = 30;
		static nfloat height = 20;
		static nfloat imgWidth = 20;
		static nfloat sessionNameLeftMargin = 20;
		static nfloat sessionNameTopMargin = 10;
		static nfloat sessionNameRightMargin = 40; 

        UIImageView imgBack;
        public UILabel lblSessionName, lblTime;
        public CALayer TrackColor;

        public SpeakerSessionsCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            ContentView.BackgroundColor = UIColor.Clear;
            lblSessionName = new UILabel()
            {
                TextColor = AppTheme.SpeakerSessionTitleColor,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular (16)
            };

            TrackColor = new CALayer();

            lblTime = new UILabel()
            {
                TextColor = AppTheme.SpeakerSessionTimeColor,
                BackgroundColor = UIColor.Clear,
                Font = AppFonts.ProximaNovaRegular(12)
            };
			imgBack = new UIImageView(UIImage.FromBundle(AppTheme.LMRightArrowImage));
            ContentView.Add(lblSessionName);
            ContentView.Add(lblTime);
            //ContentView.Add(imgBack);
            ContentView.Layer.AddSublayer(TrackColor);
        }

        public void UpdateCell(BuiltSessionTime builtSessionTime,List<BuiltTracks> tracks)
        {
            BuiltTracks builtTracks = null;
            if (builtSessionTime.BuiltSession != null)
            {
                builtTracks = tracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);
                lblSessionName.Text = builtSessionTime.BuiltSession.title;
            }

            if (builtTracks == null)
            {
                builtTracks = tracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }
            if (builtTracks != null)
            {
                TrackColor.BackgroundColor = UIColor.Clear.FromHexString(builtTracks.color, 1.0f).CGColor;
            }
            
            lblTime.Text = convertToStartEndDate(builtSessionTime.time, builtSessionTime.length);
        }

        private static string convertToStartEndDate(string time, string length)
        {
            string date = DateTime.Parse(time).ToString("hh:mm tt");
            string endDate = DateTime.Parse(time).AddMinutes(Convert.ToInt32(length)).ToString("hh:mm tt");
            string actualDate = string.Format("{0} - {1}", date, endDate);
            return actualDate;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TrackColor.Frame = new CGRect(0, 0, 5, ContentView.Frame.Height);
            lblSessionName.Frame = new CGRect(leftMargin, sessionNameTopMargin, ContentView.Frame.Width - sessionNameRightMargin, height);
            lblTime.Frame = new CGRect(leftMargin, lblSessionName.Frame.Bottom, lblSessionName.Frame.Width, height);
            imgBack.Frame = new CGRect(lblSessionName.Frame.Right + 10, Helper.getCenterY(ContentView.Frame.Height, imgWidth), imgWidth, height);
        }
    }
}