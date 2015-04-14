using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using CoreAnimation;

using System.Threading;

namespace ConferenceAppiOS
{
    public class SessionCell : UITableViewCell
    {
        public UIView leftView;
        public UILabel lblEventTitle, lblEventAddress, lblEventTime;
        public UIButton btnAdd;
       
      
        public SessionCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            var bgColorView = new UIView();
            bgColorView.BackgroundColor = UIColor.DarkGray.ColorWithAlpha(0.6f);
            SelectedBackgroundView = bgColorView;

            
            BackgroundColor = UIColor.White;

           

            leftView = new UIView();
           

            lblEventTitle = new UILabel();

			lblEventTitle.Font = AppFonts.ProximaNovaRegular(15);
            lblEventTitle.TextColor = UIColor.Black;
           

            lblEventAddress = new UILabel();
            lblEventAddress.TextColor = UIColor.LightGray;
            
			lblEventAddress.Font = AppFonts.ProximaNovaRegular(13);

            lblEventTime = new UILabel();
            lblEventTime.TextColor = UIColor.LightGray;
           
			lblEventTime.Font = AppFonts.ProximaNovaRegular(13);

            btnAdd = UIButton.FromType(UIButtonType.Custom);
            btnAdd.BackgroundColor = UIColor.Clear;
          
            ContentView.AddSubviews(leftView, lblEventTitle, lblEventAddress, lblEventTime, btnAdd);
            
        }

       

       

       

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            leftView.Frame = new CGRect(0, 0, 5, 80);
            lblEventTitle.Frame = new CGRect(20, 10, ContentView.Frame.Width - 70, 20);
            lblEventAddress.Frame = new CGRect(20, lblEventTitle.Frame.Bottom, ContentView.Frame.Width - 70, 20);
            lblEventTime.Frame = new CGRect(20, lblEventAddress.Frame.Bottom, ContentView.Frame.Width - 70, 20);
            btnAdd.Frame = new CGRect(ContentView.Frame.Right - 40, lblEventTitle.Frame.Bottom, 25, 25);
        }

        public void UpdateCell(string title, string address, DateTime timestamp, UIColor trackColor, bool mySession)
        {
            lblEventTitle.Text = title;
            lblEventAddress.Text = address;
            lblEventTime.Text = timestamp.ToString();
            leftView.BackgroundColor = trackColor;
            btnAdd.Selected = mySession;
        }
    }
}
