using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using SDWebImage;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public class TwitterSocialCell : UITableViewCell
    {
        public bool isImage;

        private UIImageView _thumbnailImageView;

        private UIImageView _userImageView;
        public UIImageView userImageView
        {
            get
            {
                if (_userImageView == null)
                {
                    _userImageView = new UIImageView();
                    ContentView.Add(_userImageView);
                }
                return _userImageView;
            }

        }

        public UIImageView thumbnailImageView
        {
            get
            {
                if (_thumbnailImageView == null)
                {
                    _thumbnailImageView = new UIImageView();
                    _thumbnailImageView.Layer.CornerRadius = 5.0f;
                    _thumbnailImageView.Layer.MasksToBounds = true;
                    ContentView.Add(_thumbnailImageView);
                }
                return _thumbnailImageView;
            }
        }

        private UILabel _titlelabel;
        public UILabel titlelabel
        {
            get
            {
                if (_titlelabel == null)
                {
                    _titlelabel = new UILabel();
                    _titlelabel.HighlightedTextColor = UIColor.White;
                    _titlelabel.TextColor = AppTheme.titlelabel;
                    _titlelabel.Font = AppFonts.ProximaNovaRegular (16);
                }
                ContentView.AddSubview(_titlelabel);
                return _titlelabel;
            }
        }

        private UILabel _twitterhandleLabel;
        public UILabel twitterhandleLabel
        {
            get
            {
                if (_twitterhandleLabel == null)
                {
                    _twitterhandleLabel = new UILabel();
                    _twitterhandleLabel.Text = "";
					_twitterhandleLabel.Font = AppFonts.ProximaNovaRegular (14);

                    ContentView.AddSubview(_twitterhandleLabel);
                }
                return _twitterhandleLabel;
            }
        }

        private UIButton _retweetImageButton;
        public UIButton retweetImageButton
        {
            get
            {
                if (_retweetImageButton == null)
                {
                    _retweetImageButton = new UIButton();
                    ContentView.Add(_retweetImageButton);
                }
                return _retweetImageButton;
            }
        }

        private UIButton _favouriteImageButton;
        public UIButton favouriteImageButton
        {
            get
            {
                if (_favouriteImageButton == null)
                {
                    _favouriteImageButton = new UIButton();
                    ContentView.Add(_favouriteImageButton);
                }
                return _favouriteImageButton;
            }
        }

        private UIButton _replyImageButton;
        public UIButton replyImageButton
        {
            get
            {
                if (_replyImageButton == null)
                {
                    _replyImageButton = new UIButton();
                  
                }
                return _replyImageButton;
            }
        }

		public UILabel _twitterContent_text;
		public UILabel twitterContentText
        {
            get
            {
                if (_twitterContent_text == null)
                {
					_twitterContent_text = new UILabel();
					_twitterContent_text.Lines = 0;
                    _twitterContent_text.TextColor = AppTheme.SMtwitterSocialCellTextColor;
                    _twitterContent_text.Font = AppFonts.ProximaNovaRegular(14);
                    _twitterContent_text.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                    _twitterContent_text.LineBreakMode = UILineBreakMode.WordWrap;
//                    _twitterContent_text.DataDetectorTypes = NSTextCheckingTypes.NSTextCheckingTypeLink;
//                    _twitterContent_text.Delegate = new AttributedLabelDelegate();
                    ContentView.AddSubview(_twitterContent_text);

                }
                return _twitterContent_text;
            }
        }

        private UIButton _twitterImageView;
        public UIButton twitterImageView
        {
            get
            {
                if (_twitterImageView == null)
                {
                    _twitterImageView = new UIButton();
                    ContentView.AddSubview(_twitterImageView);
                }
                return _twitterImageView;
            }
        }

        private UILabel _retweetCountLabel;
        public UILabel retweetCountLabel
        {
            get
            {
                if (_retweetCountLabel == null)
                {
                    _retweetCountLabel = new UILabel();
                    _retweetCountLabel.TextColor = AppTheme.retweetCountLabel;
                    _retweetCountLabel.BackgroundColor = UIColor.Clear;
                    _retweetCountLabel.Font = AppFonts.ProximaNovaRegular (12);
                    ContentView.AddSubview(_retweetCountLabel);
                }
                return _retweetCountLabel;
            }
        }

        private UILabel _favouriteCount;
        public UILabel favouriteCount
        {
            get
            {
                if (_favouriteCount == null)
                {
                    _favouriteCount = new UILabel();
                    _favouriteCount.TextColor = AppTheme.favouriteCount;
                    _favouriteCount.BackgroundColor = UIColor.Clear;
                    _favouriteCount.Font = AppFonts.ProximaNovaRegular (12);
                    ContentView.AddSubview(_favouriteCount);
                }
                return _favouriteCount;
            }
        }

        private UIButton _dottedImageView;
        public UIButton dottedImageView
        {
            get
            {
                if (_dottedImageView == null)
                {
                    _dottedImageView = new UIButton();
                    ContentView.Add(_dottedImageView);
                }
                return _dottedImageView;
            }
        }

        public UIButton replyButton
        {
            get;
            set;
        }

        public UIButton retweetButton
        {
            get;
            set;
        }

        public UIButton favouriteButton
        {
            get;
            set;
        }

        private UILabel _timeLabel;
        public UILabel timeLabel
        {
            get
            {
                if (_timeLabel == null)
                {
                    _timeLabel = new UILabel();
                    _timeLabel.Text = "";
                    _timeLabel.TextColor = AppTheme.timeLabel;
                    _timeLabel.BackgroundColor = UIColor.Clear;
                    _timeLabel.Font = AppFonts.ProximaNovaRegular (12);
                    ContentView.AddSubview(_timeLabel);
                }
                return _timeLabel;
            }
        }

        public BuiltTwitter model;
        public TwitterSocialCell(NSString key)
            : base(UITableViewCellStyle.Value1, key)
        { }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //profile img
            thumbnailImageView.Frame = new CGRect(20, 15, 40, 40);

            //user firstname
            titlelabel.Frame = new CGRect(thumbnailImageView.Frame.Right + 20, 15, 215, 20);

            // time
            timeLabel.Frame = new CGRect(ContentView.Frame.Width - 50, 15, 30, 20);

            //twitter image
            twitterImageView.Frame = new CGRect(thumbnailImageView.Frame.Right + 20, titlelabel.Frame.Bottom + 3, 20, 20);

            //user handler name
            twitterhandleLabel.Frame = new CGRect(twitterImageView.Frame.Right + 3, titlelabel.Frame.Bottom + 5, 198, 0);
            twitterhandleLabel.SizeToFit();
            twitterhandleLabel.Frame = new CGRect(twitterImageView.Frame.Right + 3, titlelabel.Frame.Bottom + 5, 198, twitterhandleLabel.Frame.Height);

            //retweet img
            retweetImageButton.Frame = new CGRect(thumbnailImageView.Frame.Left, thumbnailImageView.Frame.Bottom + 10, 20, 20);

            //favourite img
            favouriteImageButton.Frame = new CGRect(thumbnailImageView.Frame.Left, retweetImageButton.Frame.Bottom + 5, 20, 20);

            //reply img
            replyImageButton.Frame = new CGRect(thumbnailImageView.Frame.Left, favouriteImageButton.Frame.Bottom + 5, 20, 20);

            // retweetcount
            retweetCountLabel.Frame = new CGRect(retweetImageButton.Frame.Right + 5, thumbnailImageView.Frame.Bottom + 10, 50, 20);

            //favouritecout
            favouriteCount.Frame = new CGRect(favouriteImageButton.Frame.Right + 5, retweetCountLabel.Frame.Bottom + 5, 50, 20);

            

            // user content
            twitterContentText.Frame = new CGRect(thumbnailImageView.Frame.Right + 20, thumbnailImageView.Frame.Bottom + 10, ContentView.Frame.Width - (thumbnailImageView.Frame.Right + 100), 0);
            var frame = twitterContentText.Frame;
            twitterContentText.SizeToFit();
            frame.Height = twitterContentText.Frame.Height;
            twitterContentText.Frame = frame;

            //user imgView
            userImageView.Frame = new CGRect(thumbnailImageView.Frame.Right + 20, twitterContentText.Frame.Bottom + 10, twitterContentText.Frame.Width, 160);

            // dotted img
            if(model!=null && model.entities!=null)
            {
                if (model.entities.media != null && model.entities.media.Count > 0)
                {
                    dottedImageView.Frame = new CGRect(ContentView.Frame.Width - 50, userImageView.Frame.Bottom - 15, 30, 30);
                }
                else
                {
                    dottedImageView.Frame = new CGRect(ContentView.Frame.Width - 50, favouriteCount.Frame.Bottom - 15, 30, 30);
                }
            }
            
        }

        public void updateCell(BuiltTwitter builtTwitter,bool shouldIncrement,string requestUrl)
        {
            model = builtTwitter;
            var result = builtTwitter.social_object_dict;
            var twitterUser = builtTwitter.user;
            var entities = builtTwitter.entities;
            
            retweetImageButton.SetTitleColor(AppTheme.SMRetweetNormalColor, UIControlState.Normal);
            retweetImageButton.SetTitleColor(AppTheme.SMRetweetFontSelectedColor, UIControlState.Selected);
            retweetImageButton.SetTitleColor(AppTheme.SMRetweetFontHighlightedColor, UIControlState.Highlighted);
            retweetImageButton.SetTitle(AppTheme.SMRetweetImage, UIControlState.Normal);
            retweetImageButton.SetTitle(AppTheme.SMRetweetImage, UIControlState.Selected);
            retweetImageButton.SetTitle(AppTheme.SMRetweetImage, UIControlState.Highlighted);
            retweetImageButton.Font = AppTheme.SMRetweetFont;

            favouriteImageButton.SetTitleColor(AppTheme.SMFavouriteNormalColor, UIControlState.Normal);
            favouriteImageButton.SetTitleColor(AppTheme.SMFavouriteFontSelectedColor, UIControlState.Selected);
            favouriteImageButton.SetTitleColor(AppTheme.SMFavouriteFontHighlightedColor, UIControlState.Highlighted);
            favouriteImageButton.SetTitle(AppTheme.SMFavouriteImage, UIControlState.Normal);
            favouriteImageButton.SetTitle(AppTheme.SMFavouriteImage, UIControlState.Selected);
            favouriteImageButton.SetTitle(AppTheme.SMFavouriteImage, UIControlState.Highlighted);
            favouriteImageButton.Font = AppTheme.SMFavouriteFont;

            //profile img
            if (twitterUser != null)
            {
                if (twitterUser.profile_image_url != null)
                {
					thumbnailImageView.SetImage(NSUrl.FromString(twitterUser.profile_image_url), UIImage.FromBundle(AppTheme.anonymousUser));
                }
            }
       
           
            titlelabel.Text = twitterUser.name;
            timeLabel.Text = Helper.SocialToTimeAgo(Convert.ToDateTime(model.created_at));             
           
            if (!string.IsNullOrEmpty(requestUrl))
            {
                if (requestUrl.ToLower().Contains("create"))
                {
                    if (result.ContainsKey("favorite_count") && result["favorite_count"].ToString() != null)
                    {
                        var text = result["favorite_count"].ToString();
                        var temp = Convert.ToInt32(text);
                        temp++;
                        favouriteCount.Text = Convert.ToString(temp);
                    }

                    if (requestUrl.ToLower().Contains("destroy"))
                    {
                        var text = result["favorite_count"].ToString();
                        var temp = Convert.ToInt32(text);
                        temp--;
                        favouriteCount.Text = Convert.ToString(temp);
                    }
                }
            }
            else
            {
                if (result.ContainsKey("favorite_count"))
                {
                    favouriteCount.Text = result["favorite_count"].ToString();
                }
                
            }

            if (!string.IsNullOrEmpty(requestUrl))
            {
                if (requestUrl.ToLower().Contains("retweet"))
                {
                    if (result.ContainsKey("retweet_count") && result["retweet_count"].ToString() != null)
                    {
                        var text = result["retweet_count"].ToString();
                        var temp = Convert.ToInt32(text);
                        temp++;
                        retweetCountLabel.Text = Convert.ToString(temp);
                    }

                    if (requestUrl.ToLower().Contains("destroy"))
                    {
                        var text = result["retweet_count"].ToString();
                        var temp = Convert.ToInt32(text);
                        temp--;
                        retweetCountLabel.Text = Convert.ToString(temp);
                    }
                }
            }
            else
            {
                if (result.ContainsKey("retweet_count"))
                {
                    retweetCountLabel.Text = result["retweet_count"].ToString();
                }
                
            }

			twitterContentText.Text = builtTwitter.content_text;

            // user hanler name
            if (twitterUser != null)
            {
                if (twitterUser.screen_name != null)
                {
                    twitterhandleLabel.Text = string.Format("@{0}", twitterUser.screen_name);
                }
            }

            //twitterImageView.SetImage(UIImage.FromBundle("twitter.png"), UIControlState.Normal);
            twitterImageView.SetTitleColor(AppTheme.SMTwitterNormalColor, UIControlState.Normal);
            twitterImageView.SetTitleColor(AppTheme.SMTwitterFontSelectedColor, UIControlState.Selected);
            twitterImageView.SetTitleColor(AppTheme.SMTwitterFontHighlightedColor, UIControlState.Highlighted);
            twitterImageView.SetTitle(AppTheme.SMTwitterImage, UIControlState.Normal);
            twitterImageView.SetTitle(AppTheme.SMTwitterImage, UIControlState.Selected);
            twitterImageView.SetTitle(AppTheme.SMTwitterImage, UIControlState.Highlighted);
            twitterImageView.Font = AppTheme.SMTwitterFont;

            // user media

            if(entities!=null)
            {
                if (entities.media != null && entities.media.Count > 0)
                {
                    userImageView.Layer.BorderColor = AppTheme.EXlineColor.CGColor;
                    userImageView.Layer.BorderWidth = 1.0f;
                    userImageView.ContentMode = UIViewContentMode.Center;
					userImageView.BackgroundColor = AppTheme.SMsectionHeaderBackColor;
                    userImageView.Layer.MasksToBounds = true;
                }
            }

            dottedImageView.SetTitleColor(AppTheme.SMDottedImageNormalColor, UIControlState.Normal);
            dottedImageView.SetTitleColor(AppTheme.SMDottedImageFontSelectedColor, UIControlState.Selected);
            dottedImageView.SetTitleColor(AppTheme.SMDottedImageFontHighlightedColor, UIControlState.Highlighted);
            dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Normal);
            dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Selected);
            dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Highlighted);
            dottedImageView.Font = AppTheme.SMDottedImageFont;

         

        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            if (_userImageView != null)
            {
                _userImageView.RemoveFromSuperview();
                _userImageView = null;
            }

            if (_retweetCountLabel != null)
            {
                _retweetCountLabel.RemoveFromSuperview();
                _retweetCountLabel = null;
            }

            if (_retweetImageButton != null)
            {
                _retweetImageButton.RemoveFromSuperview();
                _retweetImageButton = null;
            }

            if (_dottedImageView != null)
            {
                _dottedImageView.RemoveFromSuperview();
                _dottedImageView = null;
            }
        }
    }
}