using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using SQLiteNetExtensions.Extensions;
using CoreGraphics;
using CommonLayer.Entities.Built;
using CommonLayer;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using MessageUI;
using Twitter;
using Accounts;
using Social;
using BigTed;
using SDWebImage;
using System.Threading.Tasks;
using ConferenceAppiOS.Helpers;
using CoreGraphics;


namespace ConferenceAppiOS
{

    public class SocialMediaController : BaseViewController
    {
        LoadingOverlay loadingOverlay;
        private string SocialMediaTitle = AppTheme.SMsocialMediaText;
		static nfloat headerViewLeftMargin = -3;
		static nfloat SocialMediaTableHeaderViewHeight = 120;
		static nfloat headerHeight = 64;
        public UITableView impLinksTableView;
		static nfloat impLinksTableViewWidth = 362;
        public WebViewController webView;
        Dictionary<string, BuiltImportantLinks[]> impLinkSource;
        CGRect frm;
        LineView verticalLine;
        LineView horizontalLine;
        nfloat lineViewHeight = AppTheme.SMseparatorBorderWidth;
        TitleHeaderView titleheaderView;
        public BaseTableViewController AllFeedsTableView;
		static nfloat titleheaderViewXPadding = 0;
		static nfloat titleheaderViewXPaddingYPadding = 0;
		static nfloat horizontalLineXPadding = 0;
        nfloat horizontalHeightPadding = AppTheme.SMseparatorBorderWidth;

		static nfloat impLinksTableViewXPadding = 0;
        int webViewPad = 4;

        public UIActionSheet actionSheet;
        public UIActionSheet socialActionSheet;
        List<string> lstActionSheet;
        List<string> sociallstActionSheet;
        string emailSubject = string.Empty;
        public Action<BuiltTwitter, NSIndexPath> MessageBody;
        public string messageContent = string.Empty;
        public string hashTags = string.Empty;
        TWTweetComposeViewController tweet;
        MFMessageComposeViewController message;
        public BuiltTwitter currentTwitterDataModel { get; set; }
        public NSIndexPath currentIndexPath;
        List<BuiltTwitter> twitterSource = new List<BuiltTwitter>();
        UIRefreshControl refreshControl;
        public UIView spinnerView;
        public string selfieHastTag = string.Empty;
        public string photoHasTag = string.Empty;
        public NSOperationQueue imageDownloadingQueue;
        public NSCache imagesCache;
        string Twitter = AppTheme.SMtwitterAllFeedsText;
        static string INSTAGRAM = AppTheme.SMinstagramText;
        static string FACEBOOK = AppTheme.SMfacebookText;
        static string YOUTUBE = AppTheme.SMyoutubeText;
        string ACTIONSHEETTITLE = AppTheme.SMYActionSheetTitleText;

        ImpLinksTableSource impLinksTableSource;

        AllFeedsDataSource allFeedsDataSource;

		private static readonly UIImagePickerController _imagePicker = new UIImagePickerController();
		static UIImagePickerController
		imagePicker
        {
            get
            {
                    //if (_imagePicker == null)
                    //{
                    //    _imagePicker = new UIImagePickerController();
                    //    _imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    //}
                return _imagePicker;

            }
        }

		private static UIImagePickerController _imagePickerSelfie = new UIImagePickerController();
		UIImagePickerController imagePickerSelfie
        {
            get
            {
                //if (_imagePickerSelfie == null)
                //{
                //    _imagePickerSelfie = new UIImagePickerController();
                //    _imagePickerSelfie.SourceType = UIImagePickerControllerSourceType.Camera;
                //    _imagePickerSelfie.CameraDevice = UIImagePickerControllerCameraDevice.Front;
                //}
                return _imagePickerSelfie;

            }
        }

        public enum ActionType
        {
            RETWEET = 0,
            FAVOURITE,
            REPLY
        }

        public enum SocialAction
        {
            TAKE_A_SELFIE = 0,
            OPEN_A_CAMERA,
            NEW_TWEET
        }

        public SocialAction socialAction;

        public ActionType actionType;

        public SocialMediaController(CGRect rect)
        {
            frm = rect;
            View.Frame = rect;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = frm;
            imageDownloadingQueue = new NSOperationQueue();
            imageDownloadingQueue.MaxConcurrentOperationCount = 4;
            imagesCache = new NSCache();
            titleheaderView = new TitleHeaderView(SocialMediaTitle, true, false, false, false, false, true, true, true);
            titleheaderView.RefreshButtonClicked = () =>
            { };
            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;

            titleheaderView.cameraButtonClicked = () =>
                {
                    openCamera();
                };

            titleheaderView.frontCameraButtonClicked = () =>
            {
                takeSelfie();
            };

            titleheaderView.postButtonClicked = () =>
                {
                    ShowTwitterPostDialog(null);
                };

            //Horizontal Divider
            horizontalLine = new LineView(CGRect.Empty);

            //Twitter Authentication
            authenticateWithTwitter();

            //ImportantLinksTableView
            impLinksTableView = new UITableView
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth
            };
            impLinksTableView.TableFooterView = new UIView(CGRect.Empty);

            //Refresh control

            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, e) =>
            {
                if (refreshControl.Refreshing)
                {
                    int skip = 0;
                    int limit = 10;
                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        DataManager.GetConfig(AppDelegate.Connection).ContinueWith(r =>
                        {
                            var urltemp = r.Result.social.all_feeds.url;
                            var constraint = string.Format("&skip={0}&limit={1}", skip, limit);

									NSUrlRequest url = new NSUrlRequest(NSUrl.FromString(r.Result.social.all_feeds.url + constraint));
                            NSUrlConnection.SendAsynchronousRequest(url, NSOperationQueue.MainQueue, (res, data, errro) =>
                            {
                                var resultTmp = data.ToString();
                                Newtonsoft.Json.Linq.JObject dataTmp = Newtonsoft.Json.Linq.JObject.Parse(resultTmp.Replace("\r\n", ""));
                                twitterSource = JsonConvert.DeserializeObject<List<BuiltTwitter>>(dataTmp.Property("objects").Value.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                InvokeOnMainThread(() =>
                                {
                                    refreshControl.EndRefreshing();
                                    (AllFeedsTableView.TableView.Source as AllFeedsDataSource).UpdateSource(twitterSource);
                                    AllFeedsTableView.TableView.ReloadData();
                                });
                            });
                        });
                    });
                }

            };


            //AllFeedsTableView
            AllFeedsTableView = new BaseTableViewController
            {
            };
            AllFeedsTableView.TableView.TableFooterView = new UIView(CGRect.Empty);
            AllFeedsTableView.TableView.SetContentOffset(new CGPoint(0, AllFeedsTableView.TableView.ContentSize.Height - AllFeedsTableView.TableView.Frame.Size.Height), true);
            if (refreshControl != null)
            {
                AllFeedsTableView.RefreshControl = refreshControl;
            }

            //Loading Overlay

            MessageBody = (res, index) =>
                {
                    currentIndexPath = index;
                    currentTwitterDataModel = res;
                    if (res.social_type.ToLower() == INSTAGRAM || res.social_type.ToLower() == FACEBOOK || res.social_type.ToLower() == YOUTUBE)
                    {
                        messageContent = res.getALLSocialModel().content_text;
                        socialActionSheet.ShowInView(View);
                        return;
                    }
                    messageContent = res.content_text;
                    actionSheet.ShowInView(View);
                };

            //spinner bottom view
            spinnerView = new UIView();
            spinnerView.Hidden = true;


            sociallstActionSheet = new List<string>();
            sociallstActionSheet.AddRange(new List<string> { AppTheme.EmailText, AppTheme.MessageText, AppTheme.CancelTextTitle });
            socialActionSheet = new UIActionSheet(ACTIONSHEETTITLE);
            foreach (var item in sociallstActionSheet)
            {
                socialActionSheet.AddButton(item);
            }

            socialActionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
         {
             if (socialActionSheet.CancelButtonIndex != b.ButtonIndex)
             {
					var item = sociallstActionSheet[(int)b.ButtonIndex];
                 if (item.ToLower() == AppTheme.EmailText.ToLower())
                 {
                     ShareViaEmail();
                     return;
                 }
                 if (item.ToLower() == AppTheme.MessageText.ToLower())
                 {
                     doMessage();
                     return;
                 }
             }
         };

            //Action sheet
            lstActionSheet = new List<string>();
            lstActionSheet.AddRange(new List<string> { AppTheme.Retweettext, AppTheme.Replytext, AppTheme.Favouritetext, AppTheme.EmailText, AppTheme.MessageText, AppTheme.CancelTextTitle });
            actionSheet = new UIActionSheet(ACTIONSHEETTITLE);
            foreach (var item in lstActionSheet)
            {
                actionSheet.AddButton(item);
            }

            actionSheet.Clicked += delegate(object a, UIButtonEventArgs b)
            {
                if (actionSheet.CancelButtonIndex != b.ButtonIndex)
                {
					var item = lstActionSheet[(int)b.ButtonIndex];
                    if (item.ToLower() == AppTheme.EmailText.ToLower())
                    {
                        ShareViaEmail();
                        return;
                    }
                    if (item.ToLower() == AppTheme.CancelTextTitle.ToLower())
                    {
                        return;
                    }

                    if (item.ToLower() == AppTheme.MessageText.ToLower())
                    {
                        doMessage();
                        return;
                    }
                    if (item.ToLower() == AppTheme.Favouritetext.ToLower())
                    {
                        dofavourite();
                        return;
                    }

                    if (item.ToLower() == AppTheme.Retweettext.ToLower())
                    {
                        retweet();
                        return;
                    }
                    if (item.ToLower() == AppTheme.Replytext.ToLower())
                    {
                        openReply();
                        return;
                    }
                }
            };


            //ImpLinks source
            DataManager.GetImpLinks(AppDelegate.Connection).ContinueWith(p =>
                {
                    DataManager.GetConfig(AppDelegate.Connection).ContinueWith(i =>
                    {
                        var sequence = i.Result.important_links.imp_link_ordering.OrderBy(u => u.link_sequence).ToArray();

                        Func<string, int> getIndex = (s) =>
                          {
                              var temp = sequence.FirstOrDefault(res => res.link_category == s);
                              if (temp == null)
                              {
                                  return 0;
                              }
                              else
                              {
                                  return temp.link_sequence;
                              }


                          };


                        var result = p.Result;
                        var defaultCellName = (new BuiltImportantLinks[] { new BuiltImportantLinks { title = Twitter } });
                        impLinkSource = result.GroupBy(q => q.category).OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.ToArray());
                        var keys = impLinkSource.Keys.ToArray();
                        for (int r = 0; r < keys.Length; r++)
                        {
                            var item = impLinkSource[keys[r]].ToArray();
                            impLinkSource[keys[r]] = item.OrderBy(u => u.sequence).ToArray();
                        }
                        impLinkSource.Add(string.Empty, defaultCellName);
                        impLinkSource = impLinkSource.OrderBy(o => getIndex(o.Key)).ToDictionary(o => o.Key, o => o.Value);
                        InvokeOnMainThread(() =>
                        {
                            impLinksTableSource = new ImpLinksTableSource(this, impLinkSource);
                            impLinksTableView.Source = impLinksTableSource;
                            impLinksTableView.ReloadData();
                            if (impLinkSource.Count > 0)
                            {
                                impLinksTableView.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);
                            }


                        });
                    });
                });

            #region --AllFeeds DataSource--

            refreshControl.BeginRefreshing();
            getAllFeedsUsingUrl(res =>
            {
                twitterSource = res;
                InvokeOnMainThread(() =>
                {
                    refreshControl.EndRefreshing();

                    allFeedsDataSource = new AllFeedsDataSource(this, twitterSource);
                    AllFeedsTableView.TableView.Source = allFeedsDataSource;
                    AllFeedsTableView.TableView.ReloadData();
                });

            });



            #endregion


            //verticalDividerBetween tableview and webview
            verticalLine = new LineView(CGRect.Empty);

            //webView
            webView = new WebViewController("");
            View.AddSubviews(actionSheet, titleheaderView, impLinksTableView, verticalLine, horizontalLine, webView.View, AllFeedsTableView.TableView, spinnerView);
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.social, Helper.ToDateString(DateTime.Now));
        }


        public override void OnDetailClosing(NSNotification notification)
        {
            if (impLinksTableSource != null && impLinksTableSource.selectedIndex != null)
            {
                impLinksTableView.DeselectRow(impLinksTableSource.selectedIndex, false);
            }
            if (allFeedsDataSource != null && allFeedsDataSource.selectedIndex != null)
            {
                TwitterSocialCell cell = (TwitterSocialCell)AllFeedsTableView.TableView.CellAt(allFeedsDataSource.selectedIndex);
                if (cell != null)
                {
                    if (cell.Selected)
                        cell.Selected = false;
                }
            }

        }

        void authenticateWithTwitter()
        {
            ACAccountStore account = new ACAccountStore();
            ACAccountType accountsType = account.FindAccountType(ACAccountType.Twitter);
            ThreadPool.QueueUserWorkItem((res) =>
            {
                account.RequestAccess(accountsType, null, (granted, err) =>
                {
                    if (granted)
                    {
                        var arrayOfAccounts = account.FindAccounts(accountsType);
                        if (arrayOfAccounts.Length > 0)
                        {
                            ACAccount twitterAccount = arrayOfAccounts.LastOrDefault();
                            AppDelegate.instance().twitterAccount = twitterAccount;
                        }

                    }
                });
            });
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            titleheaderView.Frame = new CGRect(titleheaderViewXPadding, titleheaderViewXPaddingYPadding, View.Frame.Width, headerHeight);
            horizontalLine.Frame = new CGRect(horizontalLineXPadding, titleheaderView.Frame.Height, View.Frame.Width, horizontalHeightPadding);
            impLinksTableView.Frame = new CGRect(impLinksTableViewXPadding, titleheaderView.Frame.Bottom, impLinksTableViewWidth, View.Frame.Height - verticalLine.Frame.Y);
            verticalLine.Frame = new CGRect(impLinksTableViewWidth, titleheaderView.Frame.Bottom, lineViewHeight, View.Frame.Size.Height - titleheaderView.Frame.Bottom);
            webView.View.Frame = new CGRect(verticalLine.Frame.Right, (titleheaderView.Frame.Bottom + webViewPad), View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (titleheaderView.Frame.Bottom - webViewPad));
            if (AllFeedsTableView != null)
            {
                AllFeedsTableView.TableView.Frame = new CGRect(verticalLine.Frame.Right, (titleheaderView.Frame.Bottom + webViewPad), View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (titleheaderView.Frame.Bottom - webViewPad));
            }
            spinnerView.Frame = new CGRect(verticalLine.Frame.Right, AllFeedsTableView.TableView.Frame.Height - 50, View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (titleheaderView.Frame.Bottom - webViewPad));

        }

        public void getAllFeedsUsingUrl(Action<List<BuiltTwitter>> callback)
        {
            List<BuiltTwitter> result = new List<BuiltTwitter>();
            int skip = 0;
            int limit = 10;
            ThreadPool.QueueUserWorkItem(c =>
                {
                    DataManager.GetConfig(AppDelegate.Connection).ContinueWith(r =>
                    {
                        var urltemp = r.Result.social.all_feeds.url;
                        if (!Reachability.IsHostReachable(urltemp))
                        {
                            emailSubject = r.Result.social.all_feeds.email_subject;
                            hashTags = r.Result.social.all_feeds.hashtags;
                            selfieHastTag = r.Result.social.all_feeds.selfie_hashtags;
                            photoHasTag = r.Result.social.all_feeds.photo_hashtags;
                            var constraint = string.Format("&skip={0}&limit={1}", skip, limit);
								NSUrlRequest url = new NSUrlRequest(NSUrl.FromString(r.Result.social.all_feeds.url + constraint));
                            NSUrlConnection.SendAsynchronousRequest(url, NSOperationQueue.MainQueue, (res, data, errro) =>
                            {
                                try
                                {
                                    var resultTmp = data.ToString();
                                    Newtonsoft.Json.Linq.JObject dataTmp = Newtonsoft.Json.Linq.JObject.Parse(resultTmp.Replace("\r\n", ""));
                                    result = JsonConvert.DeserializeObject<List<BuiltTwitter>>(dataTmp.Property("objects").Value.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                    var instagram = result.Where(p => p.social_type.ToLower() == AppTheme.SMinstagramText.ToLower()).ToList();
                                    if (callback != null)
                                        callback(result);
                                }
                                catch { }
                            });
                        }

                        else
                        {
                            if (callback != null)
                                callback(new List<BuiltTwitter>());
                        }

                    });
                });
        }

        void ShowOverlay(UIView view)
        {
            var bounds = spinnerView.Frame;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
            }

            this.loadingOverlay = new LoadingOverlay(bounds);
            view.Add(this.loadingOverlay);
        }

        private void ShareViaEmail()
        {
            try
            {
                if (MFMailComposeViewController.CanSendMail)
                {
                    DataManager.GetConfig(AppDelegate.Connection).ContinueWith(i =>
                  {
                      BuiltAllFeeds allFeeds = i.Result.social.all_feeds;

                      string link = string.Empty;
                      if (currentTwitterDataModel.social_type.ToLower() == "instagram")
                      {
                          link = string.Format("https://instagram.com");
                          string socialPayload;
                          socialPayload = string.Format("{0} {1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.getALLSocialModel().content_text, link);
                          InvokeOnMainThread(() =>
                          {
                              Dictionary<string, object> dataTmp = new Dictionary<string, object>();
                              dataTmp.Add("subject", allFeeds.email_subject);
                              dataTmp.Add("recipients", "");
                              dataTmp.Add("payload", socialPayload);
                              sendEmail(dataTmp);
                              return;
                          });
                      }
                      else
                      {
                          if (!string.IsNullOrEmpty(this.currentTwitterDataModel.user.screen_name) && this.currentTwitterDataModel.social_object_dict.ContainsKey("id_str"))
                          {
                              link = string.Format("https://twitter.com/{0}/status/{1}", this.currentTwitterDataModel.user.screen_name, this.currentTwitterDataModel.social_object_dict["id_str"]);
                          }


                          string payload;
                          payload = string.Format("{0} {1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.content_text, link);

                          InvokeOnMainThread(() =>
                          {
                              Dictionary<string, object> dataTmp = new Dictionary<string, object>();
                              dataTmp.Add("subject", allFeeds.email_subject);
                              dataTmp.Add("recipients", "");
                              dataTmp.Add("payload", payload);
                              sendEmail(dataTmp);
                          });
                      }

                  });
                }
                else
                {
                    sendEmail(null);
                }

            }
            catch { }
        }

        void sendEmail(Dictionary<string, object> data)
        {
            if (data != null)
            {
                MFMailComposeViewController mailCont = new MFMailComposeViewController();
                mailCont.SetSubject(data["subject"].ToString());
                mailCont.SetMessageBody(data["payload"].ToString(), false);
                mailCont.Finished += (sender, e) =>
                {
                    var result = e.Result.ToString();
                    CGRect rect = View.Frame;
                    mailCont.DismissViewController(true, null);
                    View.Frame = rect;
                    return;

                };
                this.PresentViewController(mailCont, true, null);

            }
            else
            {
                UIAlertView alertView = new UIAlertView(string.Empty, AppTheme.NotConfiguredmailText, null, AppTheme.CancelTextTitle);
                alertView.Show();
            }

        }

        public void ShowTwitterPostDialog(UIImage postimage)
        {
            if (Helper.IsConnectedToInternet())
            {
                if (postimage == null)
                {
                    if (TWTweetComposeViewController.CanSendTweet)
                    {
                        tweet = new TWTweetComposeViewController();
                        tweet.SetInitialText(hashTags);
                        tweet.SetCompletionHandler((TWTweetComposeViewControllerResult r) =>
                        {
                            DismissViewController(true, null); // hides the tweet
                            if (r == TWTweetComposeViewControllerResult.Cancelled)
                            {
                                return;
                            }
                            else
                            {
                                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.send_tweet, Helper.ToDateString(DateTime.Now));
                                return;
                            }
                        });
                        PresentViewController(tweet, true, null);
                    }
                    else
                    {
                        try
                        {
                            InvokeOnMainThread(() => {
                                UIAlertView alertView = new UIAlertView("", AppTheme.SMtwitterNotConfiguredText, null, AppTheme.CancelTextTitle);
                                alertView.Show();
                            });
                            
                        }
                        catch { }
                    }
                }

                else
                {
                    if (TWTweetComposeViewController.CanSendTweet)
                    {
                        tweet = new TWTweetComposeViewController();
                        if (socialAction == SocialAction.TAKE_A_SELFIE)
                        {
                            tweet.SetInitialText(selfieHastTag);
                        }
                        else
                        {
                            tweet.SetInitialText(photoHasTag);
                        }

                        tweet.AddImage(postimage);
                        tweet.SetCompletionHandler((TWTweetComposeViewControllerResult r) =>
                        {
                            DismissViewController(true, null); // hides the tweet
                            if (r == TWTweetComposeViewControllerResult.Cancelled)
                            {
                                return;
                            }
                            else
                            {
                                return;
                            }
                        });
                        PresentViewController(tweet, true, null);
                    }


                    else
                    {
                        try
                        {
                            InvokeOnMainThread(() => {
                                UIAlertView alertView = new UIAlertView("", AppTheme.SMtwitterNotConfiguredText, null, AppTheme.CancelTextTitle);
                                alertView.Show();
                            });
                            
                        }
                        catch { }
                    }
                }
            }
            else
            {
                InvokeOnMainThread(() => {
                    UIAlertView alertView = new UIAlertView("", AppTheme.SMinternetErrorText, null, AppTheme.DismissText);
                    alertView.Show();
                });
                
            }

        }

        private void ShowMessageDialog()
        {
            if (MFMessageComposeViewController.CanSendText)
            {
                message = new MFMessageComposeViewController();
                message.Body = hashTags + messageContent;
                PresentViewController(message, true, null);
            }
            else
            {

                UIAlertView alert = new UIAlertView() { Title = AppTheme.MessageText, Message = AppTheme.CantSendMsgtext }; alert.AddButton(AppTheme.OkText);
                alert.Show();
            }

        }

        void openReply()
        {
            ACAccountStore accountStore = new ACAccountStore();
            // Get the list of Twitter accounts and checks
            ACAccountType accountsType = accountStore.FindAccountType(ACAccountType.Twitter);
            ACAccount[] accountsArray = accountStore.FindAccounts(accountsType);
            if (accountsArray.Length > 0)
            {
                REComposeSheetView vc = new REComposeSheetView(new CGRect(0, 0, AppTheme.LoginWidth, AppTheme.LoginHeight), currentTwitterDataModel);
                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                vc.textData = (s) =>
                    {
                        doReply(s);
                    };
            }
            else
            {
                AppDelegate.instance().twitterAccount = null;
                UIAlertView alert = new UIAlertView(AppTheme.NoTwitterAccntsText, AppTheme.NoTwitterAccntsTextDescription, null, AppTheme.OkText);
                alert.Show();
            }

        }

        void doRetweet()
        {
            string requestURL;

            if (Convert.ToBoolean(this.currentTwitterDataModel.social_object_dict["retweeted"]))
            {
                requestURL = string.Format("https://api.twitter.com/1.1/statuses/destroy/{0}.json", this.currentTwitterDataModel.social_object_dict["id_str"]);

            }
            else
            {
                requestURL = string.Format("https://api.twitter.com/1.1/statuses/retweet/{0}.json", this.currentTwitterDataModel.social_object_dict["id_str"]);
            }

			SLRequest postRequest = SLRequest.Create(SLServiceKind.Twitter, SLRequestMethod.Post,NSUrl.FromString(requestURL), null);
            postRequest.Account = AppDelegate.instance().twitterAccount;
            ThreadPool.QueueUserWorkItem((res) =>
            {
                postRequest.PerformRequest((data, response, errror) =>
                           {
                               if (data != null)
                               {
                                   var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString());
                                   var currentCell = currentTwitterDataModel;
                                   var i = twitterSource.FirstOrDefault(p => p.uid == currentCell.uid);
                                   var index = twitterSource.IndexOf(i);


                                   if (jsonDict != null)
                                   {

                                       if (jsonDict.ContainsKey("errors"))
                                       {
                                           if (jsonDict["errors"] != null)
                                           {
                                               if (jsonDict["errors"].GetType() == typeof(NSArray))
                                               {
                                                   var ns = jsonDict["errors"] as NSArray;
                                                   if (ns.ValueAt(0).ToString() == "Bad Authentication data")
                                                   {
                                                       InvokeOnMainThread(() =>
                                                       {
                                                           UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Retweettext, AppTheme.TwitterNotConfiguredInSettingsText, null, AppTheme.OkText);
                                                           cantRetweetAlert.Show();
                                                       });
                                                   }
                                                   else
                                                   {
                                                       InvokeOnMainThread(() =>
                                                       {
                                                           UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Retweettext, AppTheme.CantRetweetText, null, AppTheme.OkText);
                                                           cantRetweetAlert.Show();
                                                       });
                                                   }
                                               }
                                               else
                                               {
                                                   InvokeOnMainThread(() =>
                                                   {
                                                       UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Retweettext, AppTheme.CantRetweetText, null, AppTheme.OkText);
                                                       cantRetweetAlert.Show();
                                                   });

                                               }
                                           }
                                       }


                                       else
                                       {
                                           if (jsonDict["created_at"] != null)
                                           {
                                               twitterSource[index].social_object = jsonDict["retweeted_status"].ToString();
                                               InvokeOnMainThread(() =>
                                               {
                                                   (AllFeedsTableView.TableView.Source as AllFeedsDataSource).UpdateSource(twitterSource);
                                                   var cell = ((TwitterSocialCell)AllFeedsTableView.TableView.CellAt(currentIndexPath));
                                                   cell.updateCell(twitterSource[index], true, requestURL);
                                                   cell.SetNeedsLayout();

                                               });
                                           }

                                       }
                                   }
                               }
                           });
            });
        }

        void doMessage()
        {
            if (MFMessageComposeViewController.CanSendText)
            {
                DataManager.GetConfig(AppDelegate.Connection).ContinueWith(i =>
               {
                   BuiltAllFeeds allFeeds = i.Result.social.all_feeds;
                   string payload;
                   if ((this.currentTwitterDataModel.entities.media.Count > 0 || this.currentTwitterDataModel.entities.urls.Count > 0) && (this.currentTwitterDataModel.entities.media[0].url != null && this.currentTwitterDataModel.entities.media[0].url.Length > 0))
                   {
                       payload = string.Format("{1} {2} \n{3}", allFeeds.hashtags, this.currentTwitterDataModel.content_text, this.currentTwitterDataModel.entities.media[0].url);
                   }
                   else
                   {
                       payload = string.Format("{1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.content_text);
                   }
                   InvokeOnMainThread(() =>
                   {
                       Dictionary<string, object> data = new Dictionary<string, object>();
                       data.Add("recipients", "");
                       data.Add("payload", payload);
                       sendMessage(data);
                   });
               });
            }
            else
            {
                sendMessage(null);
            }
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
        }

        private void sendMessage(Dictionary<string, object> data)
        {
            if (data != null)
            {
                MFMessageComposeViewController picker = new MFMessageComposeViewController();
                picker.Body = data["payload"].ToString();

                this.PresentViewController(picker, true, null);
            }
            else
            {
                InvokeOnMainThread(() =>
                {
                    UIAlertView alertView = new UIAlertView("", AppTheme.DivicesCantsendmessText, null, AppTheme.OkText);
                    alertView.Show();
                });

            }
        }

        void dofavourite()
        {
            ACAccountStore accountStore = new ACAccountStore();
            // Get the list of Twitter accounts and checks
            ACAccountType accountsType = accountStore.FindAccountType(ACAccountType.Twitter);
            ACAccount[] accountsArray = accountStore.FindAccounts(accountsType);
            if (accountsArray.Length > 0)
            {
                actionType = ActionType.FAVOURITE;

                NSString requestURL;
                if (this.currentTwitterDataModel.favourite)
                {
                    requestURL = (NSString)"https://api.twitter.com/1.1/favorites/destroy.json";
                }
                else
                {
                    requestURL = (NSString)"https://api.twitter.com/1.1/favorites/create.json";
                }

                NSDictionary param = new NSDictionary("id", currentTwitterDataModel.social_object_dict["id_str"]);
				SLRequest postRequest = SLRequest.Create(SLServiceKind.Twitter, SLRequestMethod.Post, NSUrl.FromString(requestURL), param);
                postRequest.Account = AppDelegate.instance().twitterAccount;
                ThreadPool.QueueUserWorkItem((re) =>
                    {
                        postRequest.PerformRequest((data, response, errror) =>
                        {
                            if (data != null)
                            {
                                var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString());
                                var currentCell = currentTwitterDataModel;
                                var i = twitterSource.FirstOrDefault(p => p.uid == currentCell.uid);
                                var index = twitterSource.IndexOf(i);

                                if (jsonDict != null)
                                {
                                    if (jsonDict.ContainsKey("errors"))
                                    {
                                        if (jsonDict["errors"] != null)
                                        {
                                            if (jsonDict["errors"].GetType() == typeof(NSArray))
                                            {
                                                var ns = jsonDict["errors"] as NSArray;
                                                if (ns.ValueAt(0).ToString() == "Bad Authentication data")
                                                {
                                                    InvokeOnMainThread(() =>
                                                    {
                                                        UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Favouritetext, AppTheme.TwitterNotConfiguredInSettingsText, null, AppTheme.OkText);
                                                        cantRetweetAlert.Show();
                                                    });
                                                }
                                                else
                                                {
                                                    InvokeOnMainThread(() =>
                                                    {
                                                        UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Favouritetext, AppTheme.CantFavouriteText, null, AppTheme.OkText);
                                                        cantRetweetAlert.Show();
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                InvokeOnMainThread(() =>
                                                {
                                                    UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Favouritetext, AppTheme.CantFavouriteText, null, AppTheme.OkText);
                                                    cantRetweetAlert.Show();
                                                });

                                            }
                                        }
                                    }


                                    else
                                    {
                                        if (jsonDict["created_at"] != null)
                                        {
                                            if (jsonDict.ContainsKey("retweeted_status"))
                                            {
                                                twitterSource[index].social_object = jsonDict["retweeted_status"].ToString();
                                            }
                                            InvokeOnMainThread(() =>
                                            {
                                                (AllFeedsTableView.TableView.Source as AllFeedsDataSource).UpdateSource(twitterSource);
                                                var cell = ((TwitterSocialCell)AllFeedsTableView.TableView.CellAt(currentIndexPath));
                                                cell.updateCell(twitterSource[index], true, requestURL);
                                                cell.SetNeedsLayout();

                                            });
                                        }
                                    }

                                }
                            }
                        });
                    });
            }
            else
            {
                AppDelegate.instance().twitterAccount = null;
                UIAlertView alert = new UIAlertView() { Title =AppTheme.NoTwitterAccntsText, Message = AppTheme.NoTwitterAccntsTextDescription }; alert.AddButton(AppTheme.OkText);
                alert.Show();
            }


        }

        void doReply(string text)
        {
            ACAccountStore accountStore = new ACAccountStore();
            // Get the list of Twitter accounts and checks
            ACAccountType accountsType = accountStore.FindAccountType(ACAccountType.Twitter);
            ACAccount[] accountsArray = accountStore.FindAccounts(accountsType);

            if (accountsArray.Length > 0)
            {
                if (text.Length > 140)
                {
                    UIAlertView charExceedAlertView = new UIAlertView(AppTheme.Replytext, AppTheme.PostCharacterExceedText, null, AppTheme.OkText, null, null);
                    charExceedAlertView.Show();

                    return;
                }

                if (text.Length < 1)
                {
                    UIAlertView charExceedAlertView = new UIAlertView(AppTheme.Replytext, AppTheme.PostCharacterExceedText, null, AppTheme.OkText, null);
                    charExceedAlertView.Show();
                    return;
                }
                NSString result = null;
                if (!string.IsNullOrEmpty(currentTwitterDataModel.username.ToString()))
                {
                    result = (NSString)text.Replace(string.Format("{0}", currentTwitterDataModel.username), "");
                }


                bool containsMention = false;
                if (text.Contains(currentTwitterDataModel.username))
                {
                    containsMention = true;
                }
                if (!containsMention)
                {
                    UIAlertView noMentionsAlertView = new UIAlertView(AppTheme.Replytext, AppTheme.ReplyShouldContainUsrname, null, AppTheme.OkText, null);
                    noMentionsAlertView.Show();

                    return;
                }


                if (containsMention && result.ToString().Replace(" ", "").Length < 1)
                {
                    UIAlertView noMentionsAlertView = new UIAlertView(AppTheme.Replytext, AppTheme.ReplyTextCantEmpty, null, AppTheme.OkText, null, null);
                    noMentionsAlertView.Show();

                    return;
                }


                NSString postTo = new NSString(text);
                NSMutableDictionary dictionary = new NSMutableDictionary();
                dictionary.Add(new NSString("status"), postTo);
                if (Convert.ToBoolean(this.currentTwitterDataModel.social_object_dict["retweeted"]))
                {
                    dictionary.Add(new NSString(string.Format("{0}", currentTwitterDataModel.social_object_dict["id"].ToString())), new NSString("in_reply_to_status_id"));
                }
                else
                {
                    dictionary.Add(new NSString("in_reply_to_status_id"), new NSString(string.Format("{0}", currentTwitterDataModel.social_object_dict["id_str"].ToString())));
                }
                NSString requestURL = new NSString("https://api.twitter.com/1.1/statuses/update.json");
				SLRequest postRequest = SLRequest.Create(SLServiceKind.Twitter, SLRequestMethod.Post, NSUrl.FromString(requestURL), dictionary);

                postRequest.Account = AppDelegate.instance().twitterAccount;
                ThreadPool.QueueUserWorkItem((re) =>
                   {
                       postRequest.PerformRequest((responseData, response, errror) =>
                     {
                         if (responseData != null)
                         {
                             var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseData.ToString());
                             if (jsonDict != null)
                             {
                                 if (jsonDict.ContainsKey("errors"))
                                 {
                                     if (jsonDict["errors"] != null)
                                     {
                                         if (jsonDict["errors"].GetType() == typeof(NSArray))
                                         {
                                             var ns = jsonDict["errors"] as NSArray;
                                             if (ns.ValueAt(0).ToString() == "Bad Authentication data")
                                             {
                                                 InvokeOnMainThread(() =>
                                                 {
                                                     UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Replytext, AppTheme.TwitterNotConfiguredInSettingsText, null, AppTheme.OkText, null, null);
                                                     cantRetweetAlert.Show();
                                                 });
                                             }
                                         }
                                         else
                                         {
                                             InvokeOnMainThread(() =>
                                          {

                                              new UIAlertView("", AppTheme.ReplyFailedText, null,AppTheme.DismissText).Show();
                                          });
                                         }
                                     }
                                 }
                                 else
                                 {
                                     InvokeOnMainThread(() =>
                                         {
                                         });

                                 }
                             }
                             else
                             {
                                 InvokeOnMainThread(() =>
                                       {
                                           new UIAlertView("", AppTheme.ReplyFailedText, null, AppTheme.DismissText).Show();
                                       });
                             }
                         }

                     });
                   });

            }

            else
            {
                AppDelegate.instance().twitterAccount = null;
                UIAlertView alertView = new UIAlertView(AppTheme.NoTwitterAccntsText, AppTheme.NoTwitterAccntsTextDescription, null, AppTheme.OkText, null, null);
                alertView.Show();
            }
        }

        void retweet()
        {
            ACAccountStore accountStore = new ACAccountStore();
            // Get the list of Twitter accounts and checks
            ACAccountType accountsType = accountStore.FindAccountType(ACAccountType.Twitter);
            ACAccount[] accountsArray = accountStore.FindAccounts(accountsType);
            if (accountsArray.Length > 0)
            {
                actionType = ActionType.RETWEET;

                var item = (NSDictionary)AppDelegate.instance().twitterAccount.ValueForKey(new NSString("properties"));
                NSString userID = (NSString)item["user_id"];

                if (userID != (NSString)this.currentTwitterDataModel.tweetOwnerUserID)
                {
                    bool hasUserMention = this.currentTwitterDataModel.entities.user_mentions.Any(p => p.screen_name == AppDelegate.instance().twitterAccount.Username);
                    if (this.currentTwitterDataModel.social_object_dict.ContainsKey("retweeted_status"))
                    {
                        if (this.currentTwitterDataModel.social_object_dict["retweeted_status"] != null && hasUserMention)
                        {
                            UIAlertView cantRetweetAlert = new UIAlertView() { Title =AppTheme.Retweettext, Message = AppTheme.CantRetweetOwnStatusText }; cantRetweetAlert.AddButton(AppTheme.OkText);
                            cantRetweetAlert.Show();

                            return;
                        }
                    }

                    UIAlertView retweetAlert = new UIAlertView(AppTheme.Retweettext, AppTheme.RetweetThistoUrFollowerText, null, AppTheme.CancelTextTitle,  AppTheme.Retweettext);
                    retweetAlert.Clicked += (s, e) =>
                        {
                            if (e.ButtonIndex.ToString() == "1")
                            {
//                                Console.WriteLine(e.ButtonIndex.ToString());
                                doRetweet();
                            }

                        };
                    retweetAlert.Show();
                }
                else
                {
                    UIAlertView cantRetweetAlert = new UIAlertView(AppTheme.Retweettext, AppTheme.YouCantRetweetOwnStatusText, null, AppTheme.OkText);
                    cantRetweetAlert.Show();
                }
            }

            else
            {
                AppDelegate.instance().twitterAccount = null;
                UIAlertView alert = new UIAlertView(AppTheme.NoTwitterAccntsText, AppTheme.NoTwitterAccntsTextDescription, null, AppTheme.OkText);
                alert.Show();
            }

        }

        int skip = 0;
        int limit = 10;
        public void refresh()
        {
            List<BuiltTwitter> result = new List<BuiltTwitter>();

            skip += 10;

            ThreadPool.QueueUserWorkItem(c =>
                          {
                              DataManager.GetConfig(AppDelegate.Connection).ContinueWith(r =>
                              {
                                  var urltemp = r.Result.social.all_feeds.url;
                                  var constraint = string.Format("&skip={0}&limit={1}", skip, limit);
							NSUrlRequest url = new NSUrlRequest(NSUrl.FromString(r.Result.social.all_feeds.url + constraint));
                                  NSUrlConnection.SendAsynchronousRequest(url, NSOperationQueue.MainQueue, (res, data, errro) =>
                                     {
                                         var resultTmp = data.ToString();
                                         Newtonsoft.Json.Linq.JObject dataTmp = Newtonsoft.Json.Linq.JObject.Parse(resultTmp.Replace("\r\n", ""));
                                         result = JsonConvert.DeserializeObject<List<BuiltTwitter>>(dataTmp.Property("objects").Value.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                                         twitterSource.AddRange(result);
                                         InvokeOnMainThread(() =>
                                         {
                                             loadingOverlay.Hide();
                                             (AllFeedsTableView.TableView.Source as AllFeedsDataSource).UpdateSource(twitterSource);
                                             AllFeedsTableView.TableView.ReloadData();
                                         });
                                     });
                              });
                          });
        }

        void takeSelfie()
        {
                if (UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Front))
                {
                    this.socialAction = SocialAction.TAKE_A_SELFIE;
                        InvokeOnMainThread(()=>{
                        imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                        imagePicker.CameraDevice = UIImagePickerControllerCameraDevice.Front;
                        imagePicker.Delegate = new CustomImagePickerDelegate(this);
                        this.PresentViewController(imagePicker, true, null);
                    });
                    
                    DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.camera_opened, Helper.ToDateString(DateTime.Now));
                }

                else
                {
                    InvokeOnMainThread(() => {
                        UIAlertView noFrontCameraAlert = new UIAlertView(AppTheme.CameraText, AppTheme.DoesntSupportFrontCameraText, null, AppTheme.OkText);
                        noFrontCameraAlert.Show();
                    });
                    
                }
            
        }

        void openCamera()
        {
            if (UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Rear))
            {
                this.socialAction = SocialAction.OPEN_A_CAMERA;
                InvokeOnMainThread(() =>
                {
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.CameraDevice = UIImagePickerControllerCameraDevice.Rear;
                    imagePicker.Delegate = new CustomImagePickerDelegate(this);
                    PresentViewController(imagePicker, true, null);
                 
                });
                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.camera_opened, Helper.ToDateString(DateTime.Now));
            }

            else
            {
                InvokeOnMainThread(() =>
                {
                    UIAlertView noFrontCameraAlert = new UIAlertView(AppTheme.CameraText, AppTheme.DoesntSupportCameraText, null, AppTheme.OkText);
                    noFrontCameraAlert.Show();
                });

            }
            
        }
			

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.imp_links))
            {
                updateSocialTableSource();
                return;
            }
        }

        private void updateSocialTableSource()
        {
            DataManager.GetImpLinks(AppDelegate.Connection).ContinueWith(p =>
            {
                DataManager.GetConfig(AppDelegate.Connection).ContinueWith(i =>
                {
                    var sequence = i.Result.important_links.imp_link_ordering.OrderBy(u => u.link_sequence).ToArray();

                    Func<string, int> getIndex = (s) =>
                    {
                        var temp = sequence.FirstOrDefault(res => res.link_category == s);
                        if (temp == null)
                        {
                            return 0;
                        }
                        else
                        {
                            return temp.link_sequence;
                        }

                    };


                    var result = p.Result;
                    var defaultCellName = (new BuiltImportantLinks[] { new BuiltImportantLinks { title = Twitter } });
                    impLinkSource = result.GroupBy(q => q.category).OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.ToArray());
                    var keys = impLinkSource.Keys.ToArray();
                    for (int r = 0; r < keys.Length; r++)
                    {
                        var item = impLinkSource[keys[r]].ToArray();
                        impLinkSource[keys[r]] = item.OrderBy(u => u.sequence).ToArray();
                    }
                    impLinkSource.Add(string.Empty, defaultCellName);
                    impLinkSource = impLinkSource.OrderBy(o => getIndex(o.Key)).ToDictionary(o => o.Key, o => o.Value);
                    InvokeOnMainThread(() =>
                    {
                        impLinksTableSource = new ImpLinksTableSource(this, impLinkSource);
                        impLinksTableView.Source = impLinksTableSource;
                        impLinksTableView.ReloadData();
                        if (impLinkSource.Count > 0)
                        {
                            impLinksTableView.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);
                        }


                    });
                });
            });
        }

        public class ImpLinksTableSource : UITableViewSource
        {
            SocialMediaController socialMediaController;
            string[] keys;
            Dictionary<string, BuiltImportantLinks[]> indexedTableItems;
            NSString cellIdentifier = new NSString("TableCell");
            public NSIndexPath selectedIndex;

            public ImpLinksTableSource(SocialMediaController socialMediaController, Dictionary<string, BuiltImportantLinks[]> indexedTableItems)
            {
                this.socialMediaController = socialMediaController;
                this.indexedTableItems = indexedTableItems;
                this.keys = indexedTableItems.Keys.ToArray();
            }
            public override nint NumberOfSections(UITableView tableView)
            {
                return keys.Length;
            }
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                if (String.IsNullOrEmpty(keys[section]))
                    return 0;
				return AppTheme.SectionHeight;
            }
            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return 40;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return indexedTableItems[keys[section]].Length;
            }
            public override string TitleForHeader(UITableView tableView, nint section)
            {
                return keys[section];
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                nfloat topSeparatorXPadding = 0;
                nfloat topSeparatorYPadding = 1;

                nfloat lblSectionHeaderXPadding = 15;              
                nfloat lblSectionHeaderWidthPadding = 30;

                nfloat viewXPadding = 0;
                nfloat viewYPadding = 0;

                if (String.IsNullOrEmpty(keys[section]))
                {
                    var emptyHeaderView = new UIView(CGRect.Empty);
                    return emptyHeaderView;
                }
                else
                {
					UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, AppTheme.SectionHeight))
                    {
						BackgroundColor = AppTheme.SMsectionHeaderBackColor,
                    };

                    var topSeparator = new UIView(new CGRect(topSeparatorXPadding, view.Frame.Top - 1, view.Frame.Width, topSeparatorYPadding));
                    topSeparator.BackgroundColor = AppTheme.SMmenuSectionSeparator;

					UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, topSeparator.Frame.Bottom, tableView.Frame.Width - lblSectionHeaderWidthPadding * 2, view.Frame.Size.Height))
                    {
                        TextColor = AppTheme.SMsectionTitleColor,
                        Font = AppTheme.SMsectionLabelSectionFont,
                        Text = keys[section],
                    };
                    var separator = new UIView(new CGRect(0, view.Frame.Bottom - 1, view.Frame.Width, 1));
                    separator.BackgroundColor = AppTheme.SMmenuSectionSeparator;

                    view.AddSubviews(lblSectionHeader, separator, topSeparator);
                    return view;
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                SocialMediaCell cell = tableView.DequeueReusableCell(cellIdentifier) as SocialMediaCell;
                if (cell == null) cell = new SocialMediaCell(cellIdentifier);
                var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
                cell.updateCell(item);
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                selectedIndex = indexPath;

                if (indexedTableItems[keys[indexPath.Section]][indexPath.Row].link != null)
                {
                    string str = indexedTableItems[keys[indexPath.Section]][indexPath.Row].link.href;
                    if (str != "#{switch}")
                    {
                        socialMediaController.webView.loadRequest(str);
                        socialMediaController.View.BringSubviewToFront(socialMediaController.webView.View);
                    }
                }
                else
                {
                    if (indexPath.Row == 0)
                    {
                        socialMediaController.View.BringSubviewToFront(socialMediaController.AllFeedsTableView.TableView);
                    }
                }
            }
        }

        public class AllFeedsDataSource : UITableViewSource
        {
            List<BuiltTwitter> builtTwitter;
            SocialMediaController socialMediaController;
            NSString cellIdentifier = new NSString("TableCell");
            NSString socialCellIdentifier = new NSString("SocialCell");
			static nfloat defaultCellHeight = 150;
            public NSIndexPath selectedIndex;

			static nfloat defaultCellSpaceExceptSessionName = 240;
            public AllFeedsDataSource(SocialMediaController socialMediaController, List<BuiltTwitter> builtTwitter)
            {
                this.builtTwitter = builtTwitter;
                this.socialMediaController = socialMediaController;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return builtTwitter.Count;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                nfloat height = 0;
                var item = builtTwitter[indexPath.Row];
                NSString str = (NSString)item.content_text.Trim();
                CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(14), new CGSize(tableView.Frame.Width - defaultCellSpaceExceptSessionName, 999), UILineBreakMode.WordWrap);
                height += size.Height;

                if (item.social_type.ToLower() == AppTheme.SMinstagramText.ToLower() || item.social_type.ToLower() == AppTheme.SMfacebookText.ToLower() || item.social_type.ToLower() == AppTheme.SMyoutubeText.ToLower())
                {
                    if (item.getALLSocialModel().media_url != null)
                    {
                        var tmpHeight = Convert.ToInt32(item.getALLSocialModel().media_height);
                        return height += (tmpHeight + 89);
                    }
                    else
                    {
                        if (height > 150)
                        {
                            return height;
                        }
                        else
                        {
                            return 150;
                        }

                    }
                }
                else
                {
                    if (item.entities != null)
                    {
                        if (item.entities.media != null && item.entities.media.Count > 0)
                        {
                            return height += 270;
                        }
                        else
                        {
                            if (height > 150)
                            {
                                return height;
                            }
                            else
                            {
                                return height+100;
                            }
                        }
                    }
                    else
                    {
                        if (height > 150)
                        {
                            return height;
                        }
                        else
                        {
                            return height + 85;
                        }

                    }
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var item = builtTwitter[indexPath.Row];

                if (item.social_type.ToLower() == INSTAGRAM || item.social_type.ToLower() == FACEBOOK || item.social_type.ToLower() == YOUTUBE)
                {
                    AllSocialFeedCell allSocialFeedCell = tableView.DequeueReusableCell(socialCellIdentifier) as AllSocialFeedCell;
                    if (allSocialFeedCell == null) allSocialFeedCell = new AllSocialFeedCell(socialCellIdentifier);
                    allSocialFeedCell.updateCell(item, false, string.Empty);
                    if (!string.IsNullOrEmpty(item.getALLSocialModel().media_url))
                    {
                        NSString imageUrlString = (NSString)item.getALLSocialModel().media_url;
                        UIImage cachedImage = (UIImage)socialMediaController.imagesCache.ObjectForKey(imageUrlString);
                        if (cachedImage != null)
                        {
                            allSocialFeedCell.videoThumbnailImageView.Image = cachedImage;
                            UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                            {
                                ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), imageUrlString);
                                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                            });
                            singleTap.NumberOfTapsRequired = 1;
                            allSocialFeedCell.videoThumbnailImageView.UserInteractionEnabled = true;
                            allSocialFeedCell.videoThumbnailImageView.AddGestureRecognizer(singleTap);
                        }
                        else
                        {
                            allSocialFeedCell.videoThumbnailImageView.Image = UIImage.FromBundle(AppTheme.SMplaceholderImage);
                            socialMediaController.imageDownloadingQueue.AddOperation(() =>
                            {
                                UIImage image = null;
									NSUrlRequest url = new NSUrlRequest(NSUrl.FromString(imageUrlString));
                                NSUrlConnection.SendAsynchronousRequest(url, NSOperationQueue.MainQueue, (res, data, errro) =>
                                {
                                    if (data != null)
                                    {
                                        image = UIImage.LoadFromData(data);
                                    }
                                    if (image != null)
                                    {


                                        socialMediaController.imagesCache.SetObjectforKey(image, (NSString)imageUrlString);
                                        InvokeOnMainThread(() =>
                                        {
                                            allSocialFeedCell.videoThumbnailImageView.Image = image;
                                            UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                                            {
                                                ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), imageUrlString);
                                                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                                            });
                                            singleTap.NumberOfTapsRequired = 1;
                                            allSocialFeedCell.videoThumbnailImageView.UserInteractionEnabled = true;
                                            allSocialFeedCell.videoThumbnailImageView.AddGestureRecognizer(singleTap);
                                        });
                                    }
                                });
                            });
                        }
                    }

                    allSocialFeedCell.dottedImageView.TouchUpInside += (s, e) =>
                    {
                        if (socialMediaController.MessageBody != null)
                        {
                            socialMediaController.MessageBody(builtTwitter[indexPath.Row], indexPath);
                        }
                    };
                    allSocialFeedCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    return allSocialFeedCell;
                }
                else
                {
                    TwitterSocialCell builtSocialTwitterCell = tableView.DequeueReusableCell(cellIdentifier) as TwitterSocialCell;
                    if (builtSocialTwitterCell == null)
                    {
                        builtSocialTwitterCell = new TwitterSocialCell(cellIdentifier);
                    }
                    builtSocialTwitterCell.updateCell(item, false, string.Empty);
                    NSString imageUrlString = new NSString(string.Empty);
                    if (item.entities != null)
                    {
                        if (item.entities.media != null && item.entities.media.Count > 0)
                        {
                            imageUrlString = (NSString)item.entities.media[0].media_url;
                            UIImage cachedImage = (UIImage)socialMediaController.imagesCache.ObjectForKey(imageUrlString);
                            if (cachedImage != null)
                            {
                                builtSocialTwitterCell.userImageView.Image = cachedImage;
                                UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                                {
                                    ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), imageUrlString);
                                    AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                                });
                                singleTap.NumberOfTapsRequired = 1;
                                builtSocialTwitterCell.userImageView.UserInteractionEnabled = true;
                                builtSocialTwitterCell.userImageView.AddGestureRecognizer(singleTap);
                            }
                            else
                            {
                                builtSocialTwitterCell.userImageView.Image = UIImage.FromBundle(AppTheme.EXLogoPlaceholder);
                                socialMediaController.imageDownloadingQueue.AddOperation(() =>
                                {
                                    UIImage image = null;
										NSUrlRequest url = new NSUrlRequest(NSUrl.FromString(imageUrlString));
                                    NSUrlConnection.SendAsynchronousRequest(url, NSOperationQueue.MainQueue, (res, data, errro) =>
                                    {
                                        if (data != null)
                                        {
                                            image = UIImage.LoadFromData(data);
                                        }
                                        if (image != null)
                                        {

                                            socialMediaController.imagesCache.SetObjectforKey(image, (NSString)imageUrlString);
                                            InvokeOnMainThread(() =>
                                            {
                                                builtSocialTwitterCell.userImageView.Image = image;
                                                UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                                                {
                                                    ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), imageUrlString);
                                                    AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                                                });
                                                singleTap.NumberOfTapsRequired = 1;
                                                builtSocialTwitterCell.userImageView.UserInteractionEnabled = true;
                                                builtSocialTwitterCell.userImageView.AddGestureRecognizer(singleTap);
                                            });
                                        }

                                    });



                                });
                            }
                        }
                    }


                    builtSocialTwitterCell.dottedImageView.TouchUpInside += (s, e) =>
                    {
                        if (socialMediaController.MessageBody != null)
                        {
                            socialMediaController.MessageBody(builtTwitter[indexPath.Row], indexPath);
                        }
                    };

                    builtSocialTwitterCell.SelectionStyle = UITableViewCellSelectionStyle.None;

                    return builtSocialTwitterCell;
                }

            }

            public void UpdateSource(List<BuiltTwitter> indexedTableItems)
            {
                this.builtTwitter = indexedTableItems;
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                socialMediaController.spinnerView.Hidden = true;
                if (indexPath.Row == builtTwitter.Count - 1)
                {
                    socialMediaController.spinnerView.Hidden = false;
                    socialMediaController.ShowOverlay(socialMediaController.spinnerView);
                    socialMediaController.refresh();

                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                selectedIndex = indexPath;

            }

        }
    }

    public class CustomImagePickerDelegate : UIImagePickerControllerDelegate
    {
        SocialMediaController controller;
        public CustomImagePickerDelegate(SocialMediaController controller)
        {
            this.controller = controller;
        }
        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            UIAlertView alertView = new UIAlertView("", AppTheme.DoYouWantToPostText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
            alertView.Clicked += (sender, arg) =>
            {
                if (arg.ButtonIndex.ToString() == "1")
                {
					InvokeOnMainThread(() =>
					{
						picker.DismissViewController(true, null);
						AppDelegate.instance().rootViewController.ViewDidLayoutSubviews();
	                    this.controller.ShowTwitterPostDialog(info[UIImagePickerController.OriginalImage] as UIImage);
					});
                    return;
                }
                else
                {
					InvokeOnMainThread(() =>
					{
						picker.DismissViewController(true, null);
						AppDelegate.instance().rootViewController.ViewDidLayoutSubviews();
					});
                    return;
                }

            };
            alertView.Show();
        }

        public override void Canceled(UIImagePickerController picker)
        {
            picker.DismissViewController(true, null);
            AppDelegate.instance().rootViewController.ViewDidLayoutSubviews();
        }
    }
}