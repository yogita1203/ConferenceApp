using System;
using UIKit;
using Foundation;
using CommonLayer.Entities.Built;
using CoreGraphics;
using SDWebImage;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
	public class SponsorsCell : UICollectionViewCell
	{
		UIImageView _imgLogo;
		UIImageView imgLogo{
			get{
				if(_imgLogo == null){
					_imgLogo = new UIImageView();
					_imgLogo.ContentMode = UIViewContentMode.Center;
					_imgLogo.BackgroundColor = AppTheme.ClearColor;
					_imgLogo.Layer.BorderColor = AppTheme.EXlineColor.CGColor;
					_imgLogo.Layer.BorderWidth = 1.0f;
					_imgLogo.Image = UIImage.FromFile (AppTheme.EXLogoPlaceholder);
					ContentView.AddSubview(_imgLogo);
				}
				return _imgLogo;
			}
		}

		UILabel lblName;
		static nfloat imageTop = 20;
		static nfloat leftmargin = 5;
		static nfloat titleHeight = 30;

		[Export("initWithFrame:")]
		public SponsorsCell(CGRect frame)
			: base(frame)
		{
			ContentView.BackgroundColor = AppTheme.EXpageBackground;

			lblName	 = new UILabel
			{
				Font = AppFonts.ProximaNovaRegular (15),
				BackgroundColor = UIColor.Clear,
				TextColor = AppTheme.EXnameTextColor,
				TextAlignment = UITextAlignment.Center
			};
			ContentView.AddSubview(lblName);
		}


		public void UpdateCell(BuiltExhibitor exhibitor)
		{
			lblName.Text = exhibitor.name;

			var exhibitor_file_url = Helper.GetExhibitorImageUrl (exhibitor.exhibitor_file);
			if (!string.IsNullOrWhiteSpace (exhibitor_file_url)) {
				string urlstring = exhibitor_file_url;
				urlstring = urlstring.Replace (" ", "");
				imgLogo.SetImage(new NSUrl(urlstring),UIImage.FromFile (AppTheme.EXLogoPlaceholder), (t, t2, t3, t4) =>
					{
						if(t2 != null){
							imgLogo.ContentMode = UIViewContentMode.Center;
						}else{
							imgLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
						}
					});

			} 
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			imgLogo.Frame = new CGRect(leftmargin*2,imageTop, ContentView.Frame.Width - leftmargin*4, ContentView.Frame.Height-imageTop*4);
			lblName.Frame = new CGRect(leftmargin*2, imgLogo.Frame.Bottom + leftmargin*2, ContentView.Frame.Width - leftmargin*4, titleHeight);
		}

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
			imgLogo.Image = UIImage.FromFile (AppTheme.EXLogoPlaceholder);
			imgLogo.ContentMode = UIViewContentMode.Center;
        }
	}
}

