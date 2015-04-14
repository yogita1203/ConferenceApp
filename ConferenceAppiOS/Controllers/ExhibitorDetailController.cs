using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using CoreAnimation;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using SDWebImage;
using ConferenceAppiOS.Helpers;
using CoreGraphics;

namespace ConferenceAppiOS
{
    [Register("ExhibitorDetailController")]
    public class ExhibitorDetailController : BaseViewController
    {
        string NAVIGATION_TITLE = AppTheme.EXNavigationTitleText;
		static nfloat leftMargin = 30;
		static nfloat NAVIGATION_TITLE_HEIGHT = 30;
		static nfloat NAVIGATION_TITLE_WIDTH = 230;
		static nfloat backButtonVerticalOffset = -60;
		static nfloat BarItemFlexibleSpaceWidth = 10;

        UIImageView imgLogo;
		UIScrollView scrollView;
        UIView headerDetail;
        BuiltExhibitor exhibitor;
        UIButton _exhibitorButton;
        UILabel exhibitorName;
        UILabel boothName;
        UILabel description;
        UIButton ExhibitorButton
        {
            get
            {
                if (_exhibitorButton == null)
                {
                    _exhibitorButton = new UIButton();
                    _exhibitorButton.Layer.CornerRadius = 10.0f;
                    _exhibitorButton.Layer.BorderColor = AppTheme.VIvenueLocationBtnBorderColor.CGColor;
                    _exhibitorButton.Layer.BorderWidth = 1;
                }

                return _exhibitorButton;
            }
        }

        UIButton _exhibitorImage;

        UIButton ExhibitorImage
        {
            get
            {
                if (_exhibitorImage == null)
                {
                    _exhibitorImage = new UIButton();
                }
                return _exhibitorImage;
            }
        }


        UILabel _exhibitorName;
        UILabel ExhibitorName
        {
            get
            {
                if (_exhibitorName == null)
                {
                    _exhibitorName = new UILabel();
					_exhibitorName.TextAlignment = UITextAlignment.Center;
					_exhibitorName.Font = AppTheme.EXnameFont;
					_exhibitorName.TextColor = AppTheme.EXcompanyNameColor;
                }
                return _exhibitorName;

            }

        }
        public ExhibitorDetailController(BuiltExhibitor exhibitor)
        {
            this.exhibitor = exhibitor;
        }

        public ExhibitorDetailController(string exhibitor_id)
        {
            this.exhibitor = DataManager.GetExhibitorFromId(AppDelegate.Connection, exhibitor_id).Result;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = AppTheme.EXpageBackground;

            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(0, backButtonVerticalOffset), UIBarMetrics.Default);

			NavigationItem.Title = this.exhibitor.name;

            AddSubviews();
        }

        private void AddSubviews()
        {
            nfloat height = 0;
            nfloat topMargin = 10;

			scrollView = new UIScrollView();
            scrollView.AutosizesSubviews = true;

            imgLogo = new UIImageView()
            {
				ContentMode = UIViewContentMode.Center,
				Image = UIImage.FromFile (AppTheme.EXLogoPlaceholder),
				BackgroundColor = AppTheme.ClearColor,
            };
			imgLogo.Layer.BorderColor = AppTheme.EXlineColor.CGColor;
			imgLogo.Layer.BorderWidth = 1.0f;
            imgLogo.Layer.MasksToBounds = true;

            var exhibitor_file_url = Helper.GetExhibitorImageUrl(exhibitor.exhibitor_file);
              if(!string.IsNullOrWhiteSpace(exhibitor_file_url))
              {
					NSString urlStr = new NSString (exhibitor_file_url);
					if (urlStr != null) 
					{
						imgLogo.SetImage(new NSUrl(urlStr), (t, t2, t3, t4) =>
						{
							imgLogo.ContentMode = UIViewContentMode.ScaleAspectFit;
						});
					}
              }

            headerDetail = new UIView();
            headerDetail.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            headerDetail.AutosizesSubviews = true;

            exhibitorName = new UILabel()
            {
                Text = !String.IsNullOrWhiteSpace(exhibitor.type) ? "Type: " +  exhibitor.type : exhibitor.type,
                TextColor = AppTheme.EXcompanyNameColor,
                Font = AppTheme.EXnameFont,
            };

            height += topMargin + exhibitorName.Frame.Height;

            boothName = new UILabel()
            {
                Text = "Booth: " + exhibitor.booth,
                TextColor = AppTheme.EXcompanyNameColor,
                Font = AppTheme.EXboothNameFont,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
           

            //height += boothName.Frame.Height;

            description = new UILabel()
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppTheme.EXdescriptionColor,
                Font = AppTheme.EXboothNameFont
            };
            if ( !string.IsNullOrEmpty (exhibitor.company_description))
            {
                description.AttributedText = AppFonts.IncreaseLineHeight(exhibitor.company_description, description.Font, description.TextColor);
            }

            ExhibitorButton.AddSubviews(ExhibitorImage, ExhibitorName);
			scrollView.AddSubviews(imgLogo,exhibitorName, boothName,ExhibitorButton,description);

			View.AddSubviews(scrollView);
        }

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			nfloat height = 0;
			nfloat topMargin = 10;
			nfloat rightMargin = 60;
			nfloat descriptionTopMargin = 40;
			nfloat logoHeight = 300;
			View.Frame = new CGRect (0,0,View.Frame.Size.Width,View.Frame.Size.Height);
			imgLogo.Frame = new CGRect(topMargin, topMargin, View.Frame.Width - topMargin * 2, logoHeight);
			height = imgLogo.Frame.Bottom +topMargin;

			exhibitorName.Frame = new CGRect(topMargin, height, View.Frame.Width - rightMargin, 0);
			exhibitorName.SizeToFit();
			exhibitorName.Frame = new CGRect(topMargin, height, View.Frame.Width - rightMargin, exhibitorName.Frame.Height);
			height = exhibitorName.Frame.Bottom +topMargin;

			if (!string.IsNullOrEmpty(exhibitor.booth))
			{
				boothName.Frame = new CGRect(topMargin, height, View.Frame.Width - rightMargin, 0);
				boothName.SizeToFit ();
				boothName.Frame = new CGRect(topMargin, height, View.Frame.Width - rightMargin, boothName.Frame.Height);
				height = boothName.Frame.Bottom + topMargin;
			}

			if (!string.IsNullOrEmpty(exhibitor.url))
			{

				ExhibitorButton.Frame = new CGRect(topMargin, height, View.Frame.Width - rightMargin, 0);
				ExhibitorImage.Frame = new CGRect(15, 15, 30, 30);
                ExhibitorName.Frame = new CGRect(ExhibitorImage.Frame.X+10, 15, 200, 0);
				ExhibitorName.SizeToFit ();


				if ((ExhibitorName.Frame.Size.Height + 20) > (ExhibitorImage.Frame.Size.Height + 20)) {
					ExhibitorButton.Frame = new CGRect(topMargin, height, View.Frame.Width - (topMargin + 10), (ExhibitorName.Frame.Size.Height + 20));
				} else {

					ExhibitorButton.Frame = new CGRect(topMargin, height, View.Frame.Width - (topMargin + 10),(ExhibitorImage.Frame.Size.Height + 20));
				}

                ExhibitorImage.Frame = new CGRect(15, (ExhibitorButton.Frame.Height/2)-15, 30, 30);
                ExhibitorName.Frame = new CGRect(ExhibitorImage.Frame.X+10, 0, 200, ExhibitorButton.Frame.Size.Height);


				description.Frame = new CGRect(topMargin, ExhibitorButton.Frame.Bottom +10 , View.Frame.Width - (rightMargin), 0);

                var h = Helper.getTextHeight(description.Text, description.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, description.Font, description);
                //description.SizeToFit();
                var rect = description.Frame;

                //rect.Height = description.Frame.Height;
                rect.Height = h;
                description.Frame = rect;
				SetData();

				height = description.Frame.Bottom + topMargin;

			}
			else
			{
				description.Frame = new CGRect(topMargin, height, View.Frame.Width - (rightMargin), 0);
                var h = Helper.getTextHeight(description.Text, description.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, description.Font, description);
                var rect = description.Frame;

                rect.Height = h;
                description.Frame = rect;
				height = description.Frame.Bottom + topMargin;
			}

			scrollView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
			scrollView.ContentSize = new CGSize (View.Frame.Width,height + descriptionTopMargin);

		}

        private void SetData()
        {
            string url = exhibitor.url;
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) url = "http://" + url;
                ExhibitorImage.SetTitle(AppTheme.SECsponsorImage, UIControlState.Normal);
                ExhibitorImage.SetTitleColor(AppTheme.SECsponsorImageNormalColor, UIControlState.Normal);
                ExhibitorImage.Font = AppTheme.SECsponsorImageFont;
                ExhibitorName.Text = AppTheme.SPCExhibitorText;
				description.AttributedText = AppFonts.IncreaseLineHeight (exhibitor.company_description, description.Font, description.TextColor);
                ExhibitorButton.TouchUpInside += (s, e) =>
                {
                    UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(url));
                };
            }
        }
    }
}