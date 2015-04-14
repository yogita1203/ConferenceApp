using System;
using CoreGraphics;
using Foundation;
using UIKit;
using BigTed;
using CommonLayer;
using Newtonsoft.Json.Linq;
using ConferenceAppiOS.CustomControls;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public partial class LoginViewController : UIViewController
    {
        public bool isFullScreen = false;
		UIImageView logoView;
		UILabel licenseAgreementLabel;
		UILabel licenseAgreementLinkLabel;
		UILabel titleLabel, subTitle, loginTextLabel;
		UILabel ForgotPassword, ForgotUsername;
        UIButton loginButton, closeButton;
        UITextField userNameTextField, passwordTextField;
		UIView userNameTextFieldLeftView, passwordTextFieldLeftView;
        UIImageView userNameTextFieldIcon;
        UIButton passwordTextFieldIcon;
        LineView line;
		public UIScrollView baseScrollView;
		string forgetPassowrd = AppTheme.LIForgetPassowrd;
        string loginButtonText = AppTheme.LIloginButtonText;
        string licensetext = AppTheme.LIlicensetext;
		string licenselinktext = AppTheme.LIlicenseLinkText;
        string ERROR_MESSAGE = AppTheme.LIerrorMessgeText;

		static nfloat textViewPaddingWidth = 10;

		static nfloat closeButtonWidth = 44;
		static nfloat closeButtonHeight = 44;
		static nfloat headerheight = 44;

        nfloat lineViewHeight = AppTheme.LIseparatorBorderWidth;

		static nfloat lineViewY = 44;

		static nfloat texFieldHeight = 44;

		static nfloat loinLeftRightPadding = 30;

		static nfloat leftPadding = 0;

		static nfloat rightPadding = 0;

		static nfloat topFortextField = 15;

		static nfloat gapBetweenTextFields = 0;

		static nfloat gapBetweenTextFieldAndLoginButton = 60;

		static nfloat loginButtonHeight = 40;

		static nfloat loginTextLabelTopMarin = 18;

		static nfloat forgetLabelTop = 90;

		static nfloat licenseLabelTop = 20;

		static nfloat forgotButtonLeft = 30;

		static nfloat topPaddingForforgotbutoonsInSmallView = 35;

		static nfloat gapBetweenPasswordAndUsernameButton = 50;


        public LoginViewController(CGRect rect, bool isFullScreen = false)
        {
            this.isFullScreen = isFullScreen;
            View.Frame = rect;

        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

		UIImageView backgroundImageView {
			get;
			set;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.


			backgroundImageView = new UIImageView();
			backgroundImageView.Image = new UIImage (AppTheme.LIBackgroundImage);
			backgroundImageView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			backgroundImageView.Frame = View.Frame;

			View.BackgroundColor = UIColor.Clear;

			baseScrollView = new UIScrollView ();
			baseScrollView.BackgroundColor = UIColor.Clear;
			View.AddSubviews (backgroundImageView,baseScrollView);

            line = new LineView(lineViewY, lineViewHeight, View);

			logoView = new UIImageView () {
				Image = new UIImage(AppTheme.LILogo),
			};

//			var tTTAttributedLabel = new TTTAttributedLabel ();
//			tTTAttributedLabel.BackgroundColor = UIColor.Clear;
//			tTTAttributedLabel.Font = AppTheme.LIOtherLabelsFont;
//			tTTAttributedLabel.TextColor = AppTheme.LIOtherLabeltextColor;
//			tTTAttributedLabel.HighlightedTextColor = AppTheme.LIOtherLabeltextColor;
//			tTTAttributedLabel.Lines = 0;
//			tTTAttributedLabel.LineBreakMode = UILineBreakMode.WordWrap;
//			tTTAttributedLabel.TextAlignment = UITextAlignment.Center;
//			tTTAttributedLabel.Text = new NSString (licensetext);
//			tTTAttributedLabel.Highlighted = false;
			licenseAgreementLabel = new UILabel(); //..
            licenseAgreementLabel.TextAlignment = UITextAlignment.Center;

//			var tTTAttributedLinkLabel = new TTTAttributedLabel ();
//			tTTAttributedLinkLabel.BackgroundColor = UIColor.Clear;
//			tTTAttributedLinkLabel.Font = AppTheme.LILoginbuttonFont;
//			tTTAttributedLinkLabel.TextColor = AppTheme.LIOtherLabeltextColor;
//			tTTAttributedLinkLabel.HighlightedTextColor = AppTheme.LIOtherLabeltextColor;
//			tTTAttributedLinkLabel.Lines = 0;
//			tTTAttributedLinkLabel.LineBreakMode = UILineBreakMode.WordWrap;
//			tTTAttributedLinkLabel.TextAlignment = UITextAlignment.Center;
//			tTTAttributedLinkLabel.Text = new NSString (licenselinktext);
			licenseAgreementLinkLabel = new UILabel(); //..
			 
//            var dict = (NSMutableDictionary)licenseAgreementLabel.LinkAttributes.MutableCopy();
//            dict.Remove(new NSString("CTForegroundColor"));
//            dict.Remove(new NSString("NSUnderline"));
//            dict.Add(new NSString("CTForegroundColor"), UIColor.FromCGColor(AppTheme.LIOtherLabeltextColor.CGColor));
//            NSMutableParagraphStyle style = new NSMutableParagraphStyle();
//            style.Alignment = UITextAlignment.Center;
//            dict.Add(new NSString("NSParagraphStyleAttributeName"), style);

//			licenseAgreementLinkLabel.LinkAttributes = dict;
//            licenseAgreementLinkLabel.Delegate = new AttributedLabelDelegate();
//			licenseAgreementLinkLabel.AddLinkToURL(NSUrl.FromString("http://vm-static.built.io/ios_licenseandagreement.html"), new NSRange(0, 17));
			licenseAgreementLinkLabel.UserInteractionEnabled = true;

			ForgotUsername = new UILabel
			{
				BackgroundColor = UIColor.Clear,
				Font = AppTheme.LIOtherLabelsFont,
				TextColor = AppTheme.LIOtherLabeltextColor,
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
                Text = new NSString(AppTheme.LIForgetUsername),
				TextAlignment = UITextAlignment.Center,
			};

			ForgotUsername.UserInteractionEnabled = true;
			ForgotUsername.AddGestureRecognizer(new UITapGestureRecognizer(() =>
				{
					UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("https://www.vmworld.com/email-username!input.jspa"));
				}));


            ForgotPassword = new UILabel
            {
                BackgroundColor = AppTheme.LIforgotPwdbackcolor,
                Font = AppTheme.LIOtherLabelsFont,
                TextColor = AppTheme.LIOtherLabeltextColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = new NSString(forgetPassowrd),
				TextAlignment = UITextAlignment.Center,
            };

            ForgotPassword.UserInteractionEnabled = true;
            ForgotPassword.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("http://www.vmworld.com/emailPasswordToken!input.jspa"));
            }));

            titleLabel = new UILabel
            {
                BackgroundColor = AppTheme.LItitleLabelbackcolor,
                Font = AppTheme.LITitleFont,
                TextColor = AppTheme.LITitleFontColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = new NSString(AppTheme.LIfullScreenText),
                TextAlignment = UITextAlignment.Center,
            };

            subTitle = new UILabel
            {
                BackgroundColor = AppTheme.LIsubTitlebackcolor,
                Font = AppTheme.LITitleFont,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppTheme.LIsubTitleTextColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };


			// textfileds left view
			userNameTextFieldLeftView = new UIView {
				Frame = new CGRect(0,0,texFieldHeight,texFieldHeight),
				BackgroundColor = UIColor.Clear,

			};

			passwordTextFieldLeftView = new UIView {
				Frame = new CGRect(0,0,texFieldHeight,texFieldHeight),
				BackgroundColor = UIColor.Clear,
			};

			userNameTextFieldIcon = new UIImageView () {
				Image = new UIImage(AppTheme.LIUserIcon),
			};

			passwordTextFieldIcon = new UIButton () {
				//Image = new UIImage(AppTheme.LIPasswordIcon),
			};

            passwordTextFieldIcon.SetTitleColor(AppTheme.LVCPwdImageFontSelectedColor, UIControlState.Selected);
            passwordTextFieldIcon.SetTitleColor(AppTheme.LVCPwdImageFontHighlightedColor, UIControlState.Highlighted);
            passwordTextFieldIcon.SetTitle(AppTheme.LVCPwdImage, UIControlState.Normal);
            passwordTextFieldIcon.SetTitle(AppTheme.LVCPwdImage, UIControlState.Selected);
            passwordTextFieldIcon.SetTitle(AppTheme.LVCPwdImage, UIControlState.Highlighted);
            passwordTextFieldIcon.Font = AppTheme.LVCPwdImageFont;

			passwordTextFieldIcon.SizeToFit ();
			userNameTextFieldIcon.SizeToFit ();
			userNameTextFieldIcon.Center = userNameTextFieldLeftView.Center;
			passwordTextFieldIcon.Center = passwordTextFieldLeftView.Center;
			userNameTextFieldLeftView.AddSubview (userNameTextFieldIcon);
			passwordTextFieldLeftView.AddSubview (passwordTextFieldIcon);
			//..

            userNameTextField = new UITextField()
            {
                BackgroundColor = AppTheme.LITextViewBackgroundColor,
                Font = AppTheme.LITextFieldFont,
				TextAlignment = UITextAlignment.Left,
                TextColor = AppTheme.LITextViewFontColor,
                Placeholder = AppTheme.LIuserNameTextFieldPlaceHolder,
                ReturnKeyType = UIReturnKeyType.Next,			
                AutocapitalizationType = UITextAutocapitalizationType.None,
                Tag = 0,
				RightViewMode = UITextFieldViewMode.Always,
				LeftViewMode = UITextFieldViewMode.Always,
            };
			//userNameTextField.AccessibilityIdentifier = "LoginUsernameTextField";

            userNameTextField.ShouldReturn = (textField) =>
            {
                if (userNameTextField.IsFirstResponder)
                {
                    return passwordTextField.BecomeFirstResponder();
                }
                else
                {
                    return passwordTextField.ResignFirstResponder();
                }
            };

            userNameTextField.SetValueForKeyPath(AppTheme.LITextViewPlaceHolderFontColor, new NSString("_placeholderLabel.textColor"));
            UIView right = new UIView(new CGRect(0, 0, textViewPaddingWidth, 0));
            right.BackgroundColor = AppTheme.LIuserNameRightViewcolor;
            userNameTextField.RightView = right;

			userNameTextFieldLeftView.BackgroundColor = AppTheme.LIuserNameLeftViewcolor;
			userNameTextField.LeftView = userNameTextFieldLeftView;

            passwordTextField = new UITextField()
            {
                SecureTextEntry = true,
                BackgroundColor = AppTheme.LITextViewBackgroundColor,
                Font = AppTheme.LITextFieldFont,
				TextAlignment = UITextAlignment.Left,
                TextColor = AppTheme.LITextViewFontColor,
                Placeholder = AppTheme.LIpasswordTextFieldPlaceHolder,
                ReturnKeyType = UIReturnKeyType.Go,
                AutocapitalizationType = UITextAutocapitalizationType.None,
				RightViewMode = UITextFieldViewMode.Always,
				LeftViewMode = UITextFieldViewMode.Always
            };
			//passwordTextField.AccessibilityIdentifier = "LoginPasswordTextField";

            passwordTextField.Tag = 1;
            passwordTextField.SetValueForKeyPath(AppTheme.LITextViewPlaceHolderFontColor, new NSString("_placeholderLabel.textColor"));
            passwordTextField.ShouldReturn = delegate(UITextField textField)
            {
                if (textField.Text.Length > 0)
                {
                    textField.ResignFirstResponder();
                    login();
                    return true;
                }
                else
                {
                    return false;
                }
            };
            UIView right1 = new UIView(new CGRect(0, 0, textViewPaddingWidth, 0));
            right1.BackgroundColor = AppTheme.LIuserNameRightView1color;
			passwordTextField.RightView = right1;

			passwordTextFieldLeftView.BackgroundColor = AppTheme.LIuserNameLeftView1color;
			passwordTextField.LeftView = passwordTextFieldLeftView;

            loginButton = UIButton.FromType(UIButtonType.Custom);
            loginButton.SetTitleColor(AppTheme.LILoginbuttonTitleFontColor, UIControlState.Normal);
            loginButton.SetTitleColor(AppTheme.LILoginbuttonTitleFontColor, UIControlState.Highlighted);
            loginButton.SetTitleColor(AppTheme.LILoginbuttonTitleFontColor, UIControlState.Selected);
			loginButton.SetTitle(loginButtonText, UIControlState.Normal);
			loginButton.SetTitle(loginButtonText, UIControlState.Highlighted);
			loginButton.SetTitle(loginButtonText, UIControlState.Selected);	
			loginButton.Layer.CornerRadius = 5.0f;
			loginButton.Font = AppTheme.LILoginbuttonFont;
            loginButton.BackgroundColor = AppTheme.LILoginbuttonBackgroundColor;
            loginButton.TouchUpInside += loginButton_TouchUpInside;
			//loginButton.AccessibilityIdentifier = "LoginLoginButton";

            closeButton = UIButton.FromType(UIButtonType.Custom);
			closeButton.BackgroundColor = UIColor.Clear;
            closeButton.ImageView.Frame = new CGRect(5, 5, 10, 10);
            closeButton.SetImage(new UIImage(AppTheme.LICrossWhiteIcon), UIControlState.Normal);
            closeButton.SetImage(new UIImage(AppTheme.LICrossWhiteIcon), UIControlState.Selected);
            closeButton.SetImage(new UIImage(AppTheme.LICrossWhiteIcon), UIControlState.Highlighted);
            closeButton.TouchUpInside += closeButtonClicked;

			loginTextLabel = new UILabel ();
			loginTextLabel.Lines = 0;
			loginTextLabel.TextColor = AppTheme.LIOtherLabeltextColor;
			loginTextLabel.Font = AppTheme.LILoginTextLabelFont;
			loginTextLabel.TextAlignment = UITextAlignment.Right;
			UIStringAttributes stringAttributes = new UIStringAttributes {
				Font = AppTheme.LILoginTextLabelFont,
				ForegroundColor = AppTheme.LIOtherLabeltextColor,
				ParagraphStyle = new NSMutableParagraphStyle () { LineSpacing = 4.0f }
			};
			NSMutableAttributedString attrString = new NSMutableAttributedString (AppTheme.LoginDetailText);
			attrString.AddAttributes(stringAttributes,new NSRange(0,AppTheme.LoginDetailText.Length));
			loginTextLabel.AttributedText = attrString;

            if (isFullScreen)
            {
                titleLabel.TextColor = AppTheme.LIOtherLabeltextColor;
                titleLabel.Font = AppTheme.LItitleLabelFont;
                titleLabel.Text = AppTheme.LIfullScreenText;
				baseScrollView.AddSubviews(logoView,licenseAgreementLabel,licenseAgreementLinkLabel,
					loginButton, subTitle,ForgotUsername, ForgotPassword,
					userNameTextField, passwordTextField);
            }
            else
            {
				baseScrollView.AddSubviews(logoView, licenseAgreementLabel,licenseAgreementLinkLabel,
					loginButton, closeButton,ForgotUsername, ForgotPassword,
                userNameTextField, passwordTextField);
            }
        }

        void login()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(userNameTextField.Text))
                {
                    new UIAlertView("", AppTheme.LIusernamecannotbeblankText, null,AppTheme.DismissText).Show();
                    return;
                }
                else if (String.IsNullOrWhiteSpace(passwordTextField.Text))
                {
                    new UIAlertView("", AppTheme.LIpasswordcannotbeblankText, null, AppTheme.DismissText).Show();
                    return;
                }
            
                userNameTextField.ResignFirstResponder();
                passwordTextField.ResignFirstResponder();

                LoadingView.Show(string.Empty);
                DataManager.LoginExtension(userNameTextField.Text, passwordTextField.Text, AppDelegate.Connection, (res) =>
                    {
                        InvokeOnMainThread(() =>
                            {
								userNameTextField.Text = "";
								passwordTextField.Text = "";

                                if (res == null)
                                {
                                    LoadingView.Dismiss();
                                    AppDelegate.instance().rootViewController.closeDialogue();

                                    AppSettings.ApplicationUser = DataManager.GetCurrentUser(AppDelegate.Connection).Result.application_user;
                                    DataManager.SetCurrentUser(AppSettings.ApplicationUser);

                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.USER_LOGGED_IN, null);
                                    AppDelegate.instance().updateInstallation();

                                    DataManager.fetchAfterLoginAsync(AppDelegate.Connection, AppSettings.ApplicationUser, (result, surveyCount) =>
                                        {
                                            AppSettings.NewSurveyCount = surveyCount;
                                            InvokeOnMainThread(() =>
                                                {
                                                    NSUserDefaults.StandardUserDefaults.SetInt(surveyCount, AppSettings.SurveyCountKey);
                                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.AFTER_LOGIN_DATA_FETCHED, null);
                                                });
                                        });
                                    DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.login, Helper.ToDateString(DateTime.Now));
                                }
                                else
                                {
                                    passwordTextField.Text = string.Empty;
                                    try
                                    {
                                        string error_message = Helper.GetErrorMessage(res);
                                        LoadingView.Dismiss();
                                        new UIAlertView("", error_message, null, AppTheme.DismissText).Show();
                                    }
                                    catch
                                    {
                                        LoadingView.Dismiss();
                                        new UIAlertView("", ERROR_MESSAGE, null, AppTheme.DismissText).Show();
                                    }
                                }
                            });
                    });
            
            }
            catch{}
        }

        void loginButton_TouchUpInside(object sender, EventArgs e)
        {
            login();
        }

        void closeButtonClicked(object sender, EventArgs e)
        {
            AppDelegate.instance().rootViewController.closeDialogue();
            AppDelegate.instance().rootViewController.dialogueView.BackgroundColor = AppTheme.LIinstanceRootViewbackColor;

        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            
			baseScrollView.Frame = new CGRect (0,0,View.Frame.Width,View.Frame.Height);
            if (isFullScreen)
            {
                nfloat titleLabelYPadding = 110;
				logoView.SizeToFit ();
				logoView.Frame = new CGRect ((View.Frame.Width-logoView.Frame.Size.Width)/2,titleLabelYPadding,logoView.Frame.Size.Width,logoView.Frame.Size.Height);
				userNameTextField.Frame = new CGRect(leftPadding, logoView.Frame.Bottom + topFortextField, View.Frame.Width - (leftPadding + rightPadding), texFieldHeight);
                passwordTextField.Frame = new CGRect(leftPadding, userNameTextField.Frame.Bottom + gapBetweenTextFields, View.Frame.Width - (leftPadding + rightPadding), texFieldHeight);
				licenseAgreementLabel.Frame = new CGRect(0, passwordTextField.Frame.Bottom + licenseLabelTop, View.Frame.Width , 0);
				licenseAgreementLabel.SizeToFit();
                licenseAgreementLabel.Frame = new CGRect(0, passwordTextField.Frame.Bottom + licenseLabelTop, View.Frame.Width , licenseAgreementLabel.Frame.Size.Height);
				licenseAgreementLinkLabel.Frame = new CGRect(0, licenseAgreementLabel.Frame.Bottom, View.Frame.Width - (loinLeftRightPadding*2), 0);
				licenseAgreementLinkLabel.SizeToFit();
				licenseAgreementLinkLabel.Frame = new CGRect((View.Frame.Width - licenseAgreementLinkLabel.Frame.Size.Width)/2, licenseAgreementLabel.Frame.Bottom, View.Frame.Width - (loinLeftRightPadding*2), licenseAgreementLinkLabel.Frame.Size.Height);
				loginButton.Frame = new CGRect((loinLeftRightPadding*2) + 20, licenseAgreementLabel.Frame.Bottom + gapBetweenTextFieldAndLoginButton, View.Frame.Width - ((loinLeftRightPadding*4)+40), loginButtonHeight);
				ForgotUsername.SizeToFit();
				ForgotPassword.SizeToFit();
				ForgotUsername.Frame = new CGRect(forgotButtonLeft, loginButton.Frame.Bottom + forgetLabelTop, ForgotUsername.Frame.Size.Width, ForgotUsername.Frame.Height);
				ForgotPassword.Frame = new CGRect(ForgotUsername.Frame.Right + gapBetweenPasswordAndUsernameButton, loginButton.Frame.Bottom + forgetLabelTop, ForgotPassword.Frame.Size.Width, ForgotPassword.Frame.Height);
            }
            else
            {
				logoView.SizeToFit ();
				logoView.Frame = new CGRect ((View.Frame.Width-logoView.Frame.Size.Width)/2,lineViewY,logoView.Frame.Size.Width,logoView.Frame.Size.Height);
                closeButton.Frame = new CGRect(0, 0, closeButtonWidth, closeButtonHeight);
				userNameTextField.Frame = new CGRect(leftPadding, logoView.Frame.Bottom + topFortextField, View.Frame.Width - (leftPadding + rightPadding), texFieldHeight);
				passwordTextField.Frame = new CGRect(leftPadding, userNameTextField.Frame.Bottom, View.Frame.Width - (leftPadding + rightPadding), texFieldHeight);
				licenseAgreementLabel.Frame = new CGRect(0, passwordTextField.Frame.Bottom + licenseLabelTop, View.Frame.Width , 0);
				licenseAgreementLabel.SizeToFit();
                licenseAgreementLabel.Frame = new CGRect(0, passwordTextField.Frame.Bottom + licenseLabelTop, View.Frame.Width , licenseAgreementLabel.Frame.Size.Height);
				licenseAgreementLinkLabel.Frame = new CGRect(0, licenseAgreementLabel.Frame.Bottom, View.Frame.Width - (loinLeftRightPadding*2), 0);
				licenseAgreementLinkLabel.SizeToFit();
				licenseAgreementLinkLabel.Frame = new CGRect((View.Frame.Width - licenseAgreementLinkLabel.Frame.Size.Width)/2, licenseAgreementLabel.Frame.Bottom, View.Frame.Width - (loinLeftRightPadding*2), licenseAgreementLinkLabel.Frame.Size.Height);
				loginButton.Frame = new CGRect(loinLeftRightPadding*4, licenseAgreementLabel.Frame.Bottom + 50, View.Frame.Width - (loinLeftRightPadding*8), loginButtonHeight);
				ForgotUsername.SizeToFit();
				ForgotUsername.Frame = new CGRect(loinLeftRightPadding*2.5f, loginButton.Frame.Bottom + topPaddingForforgotbutoonsInSmallView, ForgotUsername.Frame.Size.Width, ForgotUsername.Frame.Height);
				ForgotPassword.SizeToFit();
				ForgotPassword.Frame = new CGRect(ForgotUsername.Frame.Right + gapBetweenPasswordAndUsernameButton, loginButton.Frame.Bottom + topPaddingForforgotbutoonsInSmallView, ForgotPassword.Frame.Size.Width, ForgotPassword.Frame.Height);
            }
        }
    }
}