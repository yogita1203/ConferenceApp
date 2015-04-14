using CommonLayer.Entities.Built;
using CoreGraphics;
using Foundation;
using UIKit;
using SDWebImage;
using System;
using CoreGraphics;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS.Notes
{
	internal class NotePhotoCell : UIView
    {
		static nfloat Margin = 10;
		public static nfloat ImageSize = 160;
        UIImageView _notePhoto;
		static nfloat ButtonSize = 30;
		static nfloat ButtonImageEdgeInsets = 5;
        //const string CrossImageString = "cross.png";
        string imageUrl;
        public UIImageView NotePhoto
        {
            get
            {
                if (_notePhoto == null)
                {
                    _notePhoto = new UIImageView()
                    {
                        BackgroundColor = UIColor.Black.ColorWithAlpha(0.1f),
                    };
                }

                return _notePhoto;
            }
            set
            {
                _notePhoto = value;
            }
        }

        UIButton _btnRemove;
        public UIButton BtnRemove
        {
            get
            {
                if (_btnRemove == null)
                {
                    _btnRemove = UIButton.FromType(UIButtonType.Custom);
					_btnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Normal);
					_btnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Selected);
					_btnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Highlighted);
					_btnRemove.SetTitleColor(AppTheme.NTcrossImageNormalColor, UIControlState.Normal);
					_btnRemove.SetTitleColor(AppTheme.NTcrossImageSelectedColor, UIControlState.Selected);
					_btnRemove.SetTitleColor(AppTheme.NTcrossImageHighlightedColor, UIControlState.Highlighted);
                    _btnRemove.ImageEdgeInsets = new UIEdgeInsets(ButtonImageEdgeInsets, ButtonImageEdgeInsets, ButtonImageEdgeInsets, ButtonImageEdgeInsets);
					_btnRemove.BackgroundColor = UIColor.White;
					_btnRemove.Font = AppTheme.NTcrossImageFont;
					_btnRemove.Layer.CornerRadius = 15.0f;
					_btnRemove.Layer.BorderColor = AppTheme.NTLineColor.CGColor;
					_btnRemove.Layer.BorderWidth = 1.0f;
                }
                return _btnRemove;
            }
            set
            {
                _btnRemove = value;
            }
        }

		public NotePhotoCell(CGRect rectangle)
        {
			Frame = rectangle;
            BackgroundColor = UIColor.Clear;
            AddSubviews(NotePhoto, BtnRemove);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
			_notePhoto.Layer.BorderColor = AppTheme.NTLineColor.CGColor;
			_notePhoto.Layer.BorderWidth = 1.0f;
            _notePhoto.Frame = new CGRect(Margin, Margin, ImageSize, ImageSize);
            _btnRemove.Frame = new CGRect(0, 0, ButtonSize, ButtonSize);
        }


        public void UpdateCell(NotePhotos notePhoto, bool isEditable)
        {
            var urlString = notePhoto.url;
            if (AppSettings.ApplicationUser != null)
                urlString += "?AUTHTOKEN=" + AppSettings.ApplicationUser.authtoken;
            imageUrl = urlString;

			NotePhoto.SetImage(NSUrl.FromString(urlString));
			BtnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Normal);
			BtnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Selected);
			BtnRemove.SetTitle(AppTheme.NTcrossImage, UIControlState.Highlighted);
			BtnRemove.SetTitleColor(AppTheme.NTcrossImageNormalColor, UIControlState.Normal);
			BtnRemove.SetTitleColor(AppTheme.NTcrossImageSelectedColor, UIControlState.Selected);
			BtnRemove.SetTitleColor(AppTheme.NTcrossImageHighlightedColor, UIControlState.Highlighted);
			BtnRemove.Font = AppTheme.NTcrossImageFont;
            if (isEditable)
            {
                BtnRemove.Hidden = false;
                BtnRemove.Enabled = true;
            }
            else
            {
                BtnRemove.Hidden = true;
                BtnRemove.Enabled = false;
            }

            UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                {
                    ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), imageUrl);
                    AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                });
            singleTap.NumberOfTapsRequired = 1;
            NotePhoto.UserInteractionEnabled = true;
            NotePhoto.AddGestureRecognizer(singleTap);
        }
    }
}