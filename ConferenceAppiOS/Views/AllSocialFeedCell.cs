using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using SDWebImage;
using CoreGraphics;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class AllSocialFeedCell : UITableViewCell
    {
        public AllSocialModel model;

        private UIImageView _thumbnailImageView;
        public UIImageView thumbnailImageView
        {
            get
            {
                if (_thumbnailImageView == null)
                {
                    _thumbnailImageView = new UIImageView();
                    _thumbnailImageView.Layer.MasksToBounds = true;

                    ContentView.Add(_thumbnailImageView);
                }

                return _thumbnailImageView;
            }
        }

        private UILabel _titleLabel;
        public UILabel titleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel();
                    _titleLabel.TextColor = AppTheme.SFCtitleLabel;
                    _titleLabel.Font = AppFonts.ProximaNovaRegular (16);
                }
                ContentView.AddSubview(_titleLabel);
                return _titleLabel;
            }
        }

        private UIButton _socialTypeLogo;
        public UIButton socialTypeLogo
        {
            get
            {
                if (_socialTypeLogo == null)
                {
                    _socialTypeLogo = new UIButton();
                    _socialTypeLogo.Layer.MasksToBounds = true;

                    ContentView.Add(_socialTypeLogo);
                }

                return _socialTypeLogo;
            }
        }

        private UILabel _subTitleLabel;
        public UILabel subTitleLabel
        {
            get
            {
                if (_subTitleLabel == null)
                {
                    _subTitleLabel = new UILabel();
                }
                ContentView.AddSubview(_subTitleLabel);
                return _subTitleLabel;
            }
        }

        private UILabel _timeLabel;
        public UILabel timeLabel
        {
            get
            {
                if (_timeLabel == null)
                {
                    _timeLabel = new UILabel();
                    _timeLabel.Font = AppFonts.ProximaNovaRegular (12);
                }
                ContentView.AddSubview(_timeLabel);
                return _timeLabel;
            }
        }

		public UILabel _feedDescriptionLabel;
		public UILabel feedDescriptionLabel
        {
            get
            {
                if (_feedDescriptionLabel == null)
                {
					_feedDescriptionLabel = new UILabel();
//                    _feedDescriptionLabel.Lines = 0;
//                    _feedDescriptionLabel.TextColor = AppTheme.SMfeedDescTextColor;
//                    _feedDescriptionLabel.Font = AppFonts.ProximaNovaRegular(14);
//                    _feedDescriptionLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
//                    _feedDescriptionLabel.LineBreakMode = UILineBreakMode.WordWrap;
//..                    _feedDescriptionLabel.DataDetectorTypes = NSTextCheckingTypes.NSTextCheckingTypeLink;
                    ContentView.AddSubview(_feedDescriptionLabel);

                }
                return _feedDescriptionLabel;
            }
        }

        private UIImageView _feedContentImageView;
        public UIImageView feedContentImageView
        {
            get
            {
                if (_feedContentImageView == null)
                {
                    _feedContentImageView = new UIImageView();
                    _feedContentImageView.Layer.MasksToBounds = true;

                    ContentView.Add(_feedContentImageView);
                }

                return _feedContentImageView;
            }
        }

        private UIButton _commentButton;
        public UIButton commentButton
        {
            get
            {
                if (_commentButton == null)
                {
                    _commentButton = new UIButton();
                    ContentView.Add(_commentButton);
                }
                return _commentButton;
            }
        }


        private UILabel _commentCountLabel;
        public UILabel commentCountLabel
        {
            get
            {
                if (_commentCountLabel == null)
                {
                    _commentCountLabel = new UILabel();
                    _commentCountLabel.TextColor = AppTheme.SFCcommentCountLabel;
                    _commentCountLabel.BackgroundColor = UIColor.Clear;
                    _commentCountLabel.Font = AppFonts.ProximaNovaRegular (12);
                }
                ContentView.AddSubview(_commentCountLabel);
                return _commentCountLabel;
            }
        }

        private UIButton _likeButton;
        public UIButton likeButton
        {
            get
            {
                if (_likeButton == null)
                {
                    _likeButton = new UIButton();
                    ContentView.Add(_likeButton);
                }
                return _likeButton;
            }
        }


        private UILabel _likeCountLabel;
        public UILabel likeCountLabel
        {
            get
            {
                if (_likeCountLabel == null)
                {
                    _likeCountLabel = new UILabel();
                    _likeCountLabel.TextColor = AppTheme.SFClikeCountLabel;
                    _likeCountLabel.BackgroundColor = UIColor.Clear;
                    _likeCountLabel.Font = AppFonts.ProximaNovaRegular (12);
                }
                ContentView.AddSubview(_likeCountLabel);
                return _likeCountLabel;
            }
        }

        private UIImageView _videoThumbnailImageView;
        public UIImageView videoThumbnailImageView
        {
            get
            {
                if (_videoThumbnailImageView == null)
                {
                    _videoThumbnailImageView = new UIImageView();
                    _videoThumbnailImageView.Layer.MasksToBounds = true;

                    ContentView.AddSubview(_videoThumbnailImageView);
                }

                return _videoThumbnailImageView;
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


        public AllSocialFeedCell(NSString key)
            : base(UITableViewCellStyle.Value1, key)
        {
            BackgroundColor = UIColor.Clear;
            SelectedBackgroundView = new UIView
            {
                BackgroundColor = AppTheme.NotesCellSelectedColor
            };
        }

		public static nfloat thumbnailImageViewXPadding = 20;
		public static nfloat thumbnailImageViewYPadding = 15;
		public static nfloat thumbnailImageViewWidthPadding = 40;
		public static nfloat thumbnailImageViewHeightPadding = 40;

		public static nfloat titleLabelXPadding = 20;
		public static nfloat titleLabelYPadding= 15;
		public static nfloat titleLabelWidthPadding= 215;
		public static nfloat titleLabelHeightPadding = 20;

		public static nfloat timeLabelXPadding = 50;
		public static nfloat timeLabelYPadding = 15;
		public static nfloat timeLabelWidthPadding = 30;
		public static nfloat timeLabelHeightPadding = 20;

		public static nfloat socialTypeLogoXPadding = 20;
		public static nfloat socialTypeLogoYPadding = 3;
		public static nfloat socialTypeLogoWidthPadding = 20;
		public static nfloat socialTypeLogoHeightPadding = 20;

		public static nfloat subTitleLabelXPadding = 3;
		public static nfloat subTitleLabelYPadding = 5;
		public static nfloat subTitleLabelWidthPadding = 198;
		public static nfloat subTitleLabelHeightPadding = 15;

		public static nfloat commentButtonYPadding = 10;
		public static nfloat commentButtonWidthPadding = 20;
		public static nfloat commentButtonHeightPadding =20;

		public static nfloat likeButtonYPadding = 5;
		public static nfloat likeButtonWidthPadding = 20;
		public static nfloat likeButtonHeightPadding = 20;


		public static nfloat commentCountLabelXPadding = 5;
		public static nfloat commentCountLabelYPadding = 10;
		public static nfloat commentCountLabelWidthPadding = 50;
		public static nfloat commentCountLabelHeightPadding = 20;


		public static nfloat likeCountLabelXPadding = 5;
		public static nfloat likeCountLabelYPadding = 5;
		public static nfloat likeCountLabelWidthPadding = 50;
		public static nfloat likeCountLabelHeightPadding = 20;

		public static nfloat videoThumbnailImageViewXPadding = 20;
		public static nfloat videoThumbnailImageViewYPadding = 10;

		public static nfloat dottedImageViewXPadding = 50;
		public static nfloat dottedImageViewYPadding = 15;
		public static nfloat dottedImageViewWidthPadding = 30;
		public static nfloat dottedImageViewHeightPadding = 30;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //profile img
            thumbnailImageView.Frame = new CGRect(thumbnailImageViewXPadding, thumbnailImageViewYPadding, thumbnailImageViewWidthPadding, thumbnailImageViewHeightPadding);

            //user firstname
            titleLabel.Frame = new CGRect(thumbnailImageView.Frame.Right + titleLabelXPadding, titleLabelYPadding, titleLabelWidthPadding, titleLabelHeightPadding);

            // time
            timeLabel.Frame = new CGRect(ContentView.Frame.Width - timeLabelXPadding, timeLabelYPadding, timeLabelWidthPadding, timeLabelHeightPadding);

            //twitter image
            socialTypeLogo.Frame = new CGRect(thumbnailImageView.Frame.Right + socialTypeLogoXPadding, titleLabel.Frame.Bottom + socialTypeLogoYPadding, socialTypeLogoWidthPadding, socialTypeLogoHeightPadding);

            //user handler name
            subTitleLabel.Frame = new CGRect(socialTypeLogo.Frame.Right + subTitleLabelXPadding, titleLabel.Frame.Bottom + subTitleLabelYPadding, subTitleLabelWidthPadding, subTitleLabelHeightPadding);

            //retweet img
            commentButton.Frame = new CGRect(thumbnailImageView.Frame.Left, thumbnailImageView.Frame.Bottom + commentButtonYPadding, commentButtonWidthPadding, commentButtonHeightPadding);

            //favourite img
            likeButton.Frame = new CGRect(thumbnailImageView.Frame.Left, commentButton.Frame.Bottom + likeButtonYPadding, likeButtonWidthPadding, likeButtonHeightPadding);


            // retweetcount
            commentCountLabel.Frame = new CGRect(commentButton.Frame.Right + commentCountLabelXPadding, thumbnailImageView.Frame.Bottom + commentCountLabelYPadding, commentCountLabelWidthPadding, commentCountLabelHeightPadding);


            //favouritecout
            likeCountLabel.Frame = new CGRect(likeButton.Frame.Right + likeCountLabelXPadding, commentCountLabel.Frame.Bottom + likeCountLabelYPadding, likeCountLabelWidthPadding, likeCountLabelHeightPadding);


            // user content
            feedDescriptionLabel.Frame = new CGRect(thumbnailImageView.Frame.Right + 20, thumbnailImageView.Frame.Bottom + 10, ContentView.Frame.Width - (thumbnailImageView.Frame.Right + 100), 0);
            var frame = feedDescriptionLabel.Frame;
            feedDescriptionLabel.SizeToFit();
            frame.Height = feedDescriptionLabel.Frame.Height;
            feedDescriptionLabel.Frame = frame;

            //user imgView
            videoThumbnailImageView.Frame = new CGRect(thumbnailImageView.Frame.Right + videoThumbnailImageViewXPadding, feedDescriptionLabel.Frame.Bottom + videoThumbnailImageViewYPadding, Convert.ToInt32(model.media_width), Convert.ToInt32(model.media_height));
        
            // dotted img
            if (!string.IsNullOrEmpty(model.media_url))
            {
                dottedImageView.Frame = new CGRect(ContentView.Frame.Width - dottedImageViewXPadding, videoThumbnailImageView.Frame.Bottom - dottedImageViewYPadding, dottedImageViewWidthPadding, dottedImageViewHeightPadding);
            }
            else
            {
                dottedImageView.Frame = new CGRect(ContentView.Frame.Width - dottedImageViewXPadding, likeCountLabel.Frame.Bottom - dottedImageViewYPadding, dottedImageViewWidthPadding, dottedImageViewHeightPadding);
            }
         

        }

        public void updateCell(BuiltTwitter socialModel, bool shouldIncrement, string requestUrl)
        {
            model=socialModel.getALLSocialModel();

			thumbnailImageView.SetImage(NSUrl.FromString(model.user_image_url), UIImage.FromBundle(AppTheme.anonymousUser));
            commentCountLabel.Text = model.commentsCount;
            likeCountLabel.Text = model.likesCount;
            titleLabel.Text = socialModel.username;
            subTitleLabel.Text = string.Format("@{0}", socialModel.username);
            feedDescriptionLabel.Text = new NSString(socialModel.content_text);
//            feedDescriptionLabel.Delegate = new AttributedLabelDelegate();

            if (socialModel.social_type.ToLower() == AppTheme.SMfacebookText.ToLower())
            {
                socialTypeLogo.SetImage(UIImage.FromBundle(AppTheme.SMInstagramLogo), UIControlState.Normal);
                commentButton.SetImage(UIImage.FromBundle(AppTheme.SMInstagramComment), UIControlState.Normal);
                likeButton.SetImage(UIImage.FromBundle(AppTheme.SMInstagramLove), UIControlState.Normal);
                dottedImageView.SetImage(UIImage.FromBundle(AppTheme.SMdottedImage), UIControlState.Normal);
            }
            else if (socialModel.social_type.ToLower() == AppTheme.SMinstagramText.ToLower())
            {
                // Instagarm Image
                socialTypeLogo.SetTitleColor(AppTheme.SMInstagramNormalColor, UIControlState.Normal);
                socialTypeLogo.SetTitleColor(AppTheme.SMInstagramFontSelectedColor, UIControlState.Selected);
                socialTypeLogo.SetTitleColor(AppTheme.SMInstagramFontHighlightedColor, UIControlState.Highlighted);
                socialTypeLogo.SetTitle(AppTheme.SMInstagramImage, UIControlState.Normal);
                socialTypeLogo.SetTitle(AppTheme.SMInstagramImage, UIControlState.Selected);
                socialTypeLogo.SetTitle(AppTheme.SMInstagramImage, UIControlState.Highlighted);
                socialTypeLogo.Font = AppTheme.SMInstagramFont;

                // Like Image
                likeButton.SetTitleColor(AppTheme.SMInstagramLikeNormalColor, UIControlState.Normal);
                likeButton.SetTitleColor(AppTheme.SMInstagramLikeFontSelectedColor, UIControlState.Selected);
                likeButton.SetTitleColor(AppTheme.SMInstagramLikeFontHighlightedColor, UIControlState.Highlighted);
                likeButton.SetTitle(AppTheme.SMInstagramLike, UIControlState.Normal);
                likeButton.SetTitle(AppTheme.SMInstagramLike, UIControlState.Selected);
                likeButton.SetTitle(AppTheme.SMInstagramLike, UIControlState.Highlighted);
                likeButton.Font = AppTheme.SMInstagramLikeFont;

                // Comment Image
                commentButton.SetTitleColor(AppTheme.SMInstagramCommentNormalColor, UIControlState.Normal);
                commentButton.SetTitleColor(AppTheme.SMInstagramCommentFontSelectedColor, UIControlState.Selected);
                commentButton.SetTitleColor(AppTheme.SMInstagramCommentFontHighlightedColor, UIControlState.Highlighted);
                commentButton.SetTitle(AppTheme.SMInstagramComment, UIControlState.Normal);
                commentButton.SetTitle(AppTheme.SMInstagramComment, UIControlState.Selected);
                commentButton.SetTitle(AppTheme.SMInstagramComment, UIControlState.Highlighted);
                commentButton.Font = AppTheme.SMInstagramCommentFont;

                // Dotted Image
                dottedImageView.SetTitleColor(AppTheme.SMDottedImageNormalColor, UIControlState.Normal);
                dottedImageView.SetTitleColor(AppTheme.SMDottedImageFontSelectedColor, UIControlState.Selected);
                dottedImageView.SetTitleColor(AppTheme.SMDottedImageFontHighlightedColor, UIControlState.Highlighted);
                dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Normal);
                dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Selected);
                dottedImageView.SetTitle(AppTheme.SMDottedImage, UIControlState.Highlighted);
                dottedImageView.Font = AppTheme.SMDottedImageFont;
            }

            else if (socialModel.social_type.ToLower() == AppTheme.SMyoutubeText.ToLower())
            {
                socialTypeLogo.SetImage(UIImage.FromBundle("instagram_icon.png"), UIControlState.Normal);
                commentButton.SetImage(UIImage.FromBundle("ic_instagram_comment.png"), UIControlState.Normal);
                likeButton.SetImage(UIImage.FromBundle("instagram_love.png"), UIControlState.Normal);
                dottedImageView.SetImage(UIImage.FromBundle(AppTheme.SMdottedImage), UIControlState.Normal);
            }
        }
    }
}     