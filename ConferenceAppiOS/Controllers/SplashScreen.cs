using System.Collections.Generic;
using CoreGraphics;
using System.Threading.Tasks;
using UIKit;
using CoreGraphics;
using Foundation;
using CommonLayer;
using System;
using ConferenceAppiOS.Helpers;
using ConferenceAppiOS.CustomControls;
using SDWebImage;
using CommonLayer.Entities.Built;
using System.Linq;

namespace ConferenceAppiOS
{
    public partial class SplashScreen : BaseViewController
    {
        const string BACKGROUND_IMAGE_NAME = "Default-Landscape.png";
        public Action DataLoaded;

        public SplashScreen()
        { }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        LoadingOverlay loadingOverlay;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            UIImageView bgImageView = new UIImageView(UIImage.FromBundle(BACKGROUND_IMAGE_NAME));
            bgImageView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            bgImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            bgImageView.Frame = View.Bounds;
            bgImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            View.AddSubview(bgImageView);

            nfloat topMargin = View.Frame.Height / 2.5f;
            loadingOverlay = new LoadingOverlay(new CGRect(0,topMargin,View.Frame.Width, View.Frame.Height - topMargin), true, true);
            View.Add(loadingOverlay);
			AppSettings.NewSurveyCount = (int)NSUserDefaults.StandardUserDefaults.IntForKey(new NSString(AppSettings.SurveyCountKey));
            var dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(TitleHeaderView.timeStamp);
            if (String.IsNullOrWhiteSpace(dateTime))
                NSUserDefaults.StandardUserDefaults.SetString(Helper.ToDateString(DateTime.Now), TitleHeaderView.timeStamp);
            DataManager.GetConfig(AppDelegate.Connection).ContinueWith(t =>
            {
                DataManager.CreateAllTables(AppDelegate.Connection);
                AppDelegate.instance().config = t.Result;
                AppDelegate.instance().hashtags = t.Result.social.all_feeds.hashtags;

                DataManager.GetCurrentUser(AppDelegate.Connection).ContinueWith((t2) =>
                {
                    if (t2.Result != null)
                    {
                        AppSettings.ApplicationUser = t2.Result.application_user;
                        DataManager.SetCurrentUser(AppSettings.ApplicationUser);
                    }

                    DataManager.GetTracks(AppDelegate.Connection).ContinueWith(tracks =>
                    {
                        AppSettings.AllTracks = tracks.Result ?? new List<BuiltTracks>();
                        DataManager.getMySurveyExtension(AppDelegate.Connection, (res, count) =>
                        {
                            AppSettings.NewSurveyCount = count;

                            DataManager.GetListOfIntroData(AppDelegate.Connection).ContinueWith(intro =>
                            {
                                try
                                {
                                    AppSettings.BuiltIntroList = intro.Result;
                                    var introBgString = NSUserDefaults.StandardUserDefaults.StringForKey(AppSettings.IntroBgKey);
                                    if (!String.IsNullOrWhiteSpace(introBgString))
                                        AppSettings.WebViewImageString = new NSString(introBgString);

                                    if (AppSettings.WebViewImageString == null)
                                    {
                                        var result = intro.Result.FirstOrDefault();
                                        var imageData = NSData.FromUrl(NSUrl.FromString(result.bg_image.url));
                                        AppSettings.WebViewImageString = (NSString)imageData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                                        NSUserDefaults.StandardUserDefaults.SetString(AppSettings.WebViewImageString.ToString(), AppSettings.IntroBgKey);
                                    }
                                }
                                catch { }

                                DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                                {
                                    AppSettings.FeaturedSessions = fs.Result;

                                    InvokeOnMainThread(() =>
                                    {
                                        NSUserDefaults.StandardUserDefaults.SetInt(count, AppSettings.SurveyCountKey);
                                        loadingOverlay.Hide();
                                    });
                                    if (DataLoaded != null)
                                    {
                                        DataLoaded();
                                    }
                                });
                            });
                        }, AppSettings.NewSurveyCount);
                    });
                });
            });
        }
    }
}
