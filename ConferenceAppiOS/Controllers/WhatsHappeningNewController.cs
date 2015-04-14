using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using Foundation;
using UIKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Threading;

namespace ConferenceAppiOS
{
    public enum LayoutEnum
    {
        topPosition,
        RightPosition,
        bottomPosition,
        LeftPosition
    }

    public class WhatsHappeningNewController : BaseViewController
    {
        CGRect rect;

        UIScrollView _baseScrollView;
        UIScrollView baseScrollView
        {
            get
            {
                if (_baseScrollView == null)
                {
                    _baseScrollView = new UIScrollView();
                    _baseScrollView.ShowsHorizontalScrollIndicator = false;
                    _baseScrollView.ShowsVerticalScrollIndicator = false;
                    _baseScrollView.BackgroundColor = AppTheme.WHScrollViewbackGround;
                    View.AddSubview(_baseScrollView);
                }
                return _baseScrollView;
            }
        }

        IntroViewExplore introViewExploreViewObj;
      
        public List<string> listOfMyScheIdsInFeaturedSession;
        FeaturedSessionViewExplore featuredSessionViewExplore;
        MyScheduleViewExplorer myScheduleViewExplorer;
        NextUpViewExplore nextUpViewExplore, onNowView;
        LoginViewController loginViewController;
        UIView dividerView, topBorderView;
        TitleHeaderView titleheaderView;

        BuiltConfig config;

		static nfloat featuredSessionTableHeight = 350;
		static nfloat rightWidthForLoginAndSceduleTable = 347;
		nfloat totalwidth = 0; static nfloat nextUpViewheight = 138;
        NSObject showObserver;
        NSObject hideObserver;
        internal bool showOnNow = true, showNextUp = true;

        public WhatsHappeningNewController(CGRect r)
        {
            rect = r;

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = AppTheme.WHScrollViewbackGround;

            config = AppDelegate.instance().config;
            AppDelegate.instance().sessionTwtter = config.social.twitter.session_tweet_text;

            SetHeaderView();

            topBorderView = new UIView();

            topBorderView.BackgroundColor = AppTheme.WHScrollViewbackGround;

            introViewExploreViewObj = new IntroViewExplore(rect);
            introViewExploreViewObj.positionInParentView = LayoutEnum.LeftPosition;

            featuredSessionViewExplore = new FeaturedSessionViewExplore(rect);
            featuredSessionViewExplore._whatsHappeningNewController = this;
            featuredSessionViewExplore.positionInParentView = LayoutEnum.LeftPosition;

            onNowView = new NextUpViewExplore(rect, NextUpViewExplore.ViewType.OnNow);
            onNowView.positionInParentView = LayoutEnum.LeftPosition;
            onNowView._whatsHappeningNewController = this;
            onNowView.OnNowLoaded = () =>
            {
                baseScrollView.AddSubviews(onNowView.View);
                ViewWillLayoutSubviews();
                nextUpViewExplore = new NextUpViewExplore(rect, NextUpViewExplore.ViewType.NextUp);
                nextUpViewExplore.positionInParentView = LayoutEnum.LeftPosition;
                nextUpViewExplore._whatsHappeningNewController = this;
                nextUpViewExplore.NextUpLoaded = () =>
                {
                    baseScrollView.AddSubviews(nextUpViewExplore.View);
                    ViewWillLayoutSubviews();
                };
            };

            dividerView = new UIView();
            dividerView.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.Layer1SubviewsColor, 1.0f);
            dividerView.Layer.BorderColor = AppTheme.WHcellBottomLineColor.CGColor;
            dividerView.Layer.BorderWidth = 1.0f;

            baseScrollView.AddSubviews(introViewExploreViewObj.View, featuredSessionViewExplore.View);

            View.AddSubviews(topBorderView, dividerView);

            if (AppSettings.ApplicationUser != null)
            {
                myScheduleViewExplorer = new MyScheduleViewExplorer(rect);
                myScheduleViewExplorer._featuredSessionViewExplore = featuredSessionViewExplore;
                myScheduleViewExplorer._whatsHappeningNewController = this;
                myScheduleViewExplorer.positionInParentView = LayoutEnum.RightPosition;

                deregisterFromKeyboardNotifications();
                View.AddSubview(myScheduleViewExplorer.View);
            }
            else
            {
                loginViewController = new LoginViewController(rect, true);

                registerForKeyboardNotifications();
                View.AddSubview(loginViewController.View);
            }
            AddObserver();

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.explore, Helper.ToDateString(DateTime.Now));
        }


        void registerForKeyboardNotifications()
        {
            showObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (notification) =>
            {
                KeyboardWillShowNotification(notification);
            });

            hideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) =>
            {
                KeyboardWillHideNotification(notification);
            });
        }

        void deregisterFromKeyboardNotifications()
        {
            if (showObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(showObserver);
            if (hideObserver != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(hideObserver);
        }

        protected virtual void KeyboardWillShowNotification(NSNotification notification)
        {
            UIView activeView = KeyboardGetActiveView();

            if (activeView == null)
                return;
            UIScrollView scrollView = loginViewController.baseScrollView;

            UIScrollView tmpScrollView = activeView.FindSuperviewOfType(this.View, typeof(UIScrollView)) as UIScrollView;
            if (tmpScrollView == null)
                return;

            CGRect keyboardBounds = UIKeyboard.BoundsFromNotification(notification);

            UIEdgeInsets contentInsets = new UIEdgeInsets(0.0f, 0.0f, keyboardBounds.Size.Height, 0.0f);
            scrollView.ContentInset = contentInsets;
            scrollView.ScrollIndicatorInsets = contentInsets;
            CGRect viewRectAboveKeyboard = new CGRect(this.View.Frame.Location, new CGSize(this.View.Frame.Width, this.View.Frame.Size.Height - keyboardBounds.Size.Height));
            CGRect activeFieldAbsoluteFrame = activeView.Superview.ConvertRectToView(activeView.Frame, this.View);
            if (!viewRectAboveKeyboard.Contains(activeFieldAbsoluteFrame) || true)
            {
                CGPoint scrollPoint = CGPoint.Empty;
                scrollPoint = new CGPoint(0.0f, 150.0f);
                if (scrollPoint.Y < loginViewController.baseScrollView.ContentSize.Height)
                {
                    loginViewController.baseScrollView.SetContentOffset(scrollPoint, true);
                }
                else
                {
                    loginViewController.baseScrollView.SetContentOffset(scrollPoint, true);
                }
            }
        }

        protected virtual UIView KeyboardGetActiveView()
        {
            return this.View.FindFirstResponder();
        }

        protected virtual void KeyboardWillHideNotification(NSNotification notification)
        {
            UIView activeView = KeyboardGetActiveView();

            if (activeView == null)
                return;

            UIScrollView scrollView = activeView.FindSuperviewOfType(this.View, typeof(UIScrollView)) as UIScrollView;
            if (scrollView == null)
                return;

            double animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);
            UIEdgeInsets contentInsets = new UIEdgeInsets(0.0f, 0.0f, 0.0f, 0.0f);
            UIView.Animate(animationDuration, delegate
            {
                loginViewController.baseScrollView.ContentInset = contentInsets;
                loginViewController.baseScrollView.ScrollIndicatorInsets = contentInsets;
            });
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);
            if (myScheduleViewExplorer != null && myScheduleViewExplorer.selectedIndex != null)
            {
                myScheduleViewExplorer.myScheduleTableview.DeselectRow(myScheduleViewExplorer.selectedIndex, false);
            }
            if (featuredSessionViewExplore != null && featuredSessionViewExplore.selectedIndex != null)
            {
                featuredSessionViewExplore.featuredTbaleView.DeselectRow(featuredSessionViewExplore.selectedIndex, false);

            }
        }

        public override void ViewWillLayoutSubviews()
        {
            nfloat Y = 0, X = 0;

            totalwidth = View.Frame.Width;

            titleheaderView.Frame = new CGRect(X, Y, View.Frame.Width, AppTheme.WHtitleHeaderViewHeight);
            Y += titleheaderView.Frame.Height;
            topBorderView.Frame = new CGRect(X, Y, View.Frame.Width, AppTheme.WHborderForSectionHeaderTopWidhtVar);
            Y += topBorderView.Frame.Height;

            nfloat leftWidth, rightWidth;
            leftWidth = totalwidth - (rightWidthForLoginAndSceduleTable + AppTheme.WHmiddleborderWidth);
            rightWidth = rightWidthForLoginAndSceduleTable;
            baseScrollView.Frame = new CGRect(X, Y, leftWidth, (View.Frame.Height - Y));

            dividerView.Frame = new CGRect(baseScrollView.Frame.Right, topBorderView.Frame.Bottom, 10, View.Frame.Height - (titleheaderView.Frame.Height + topBorderView.Frame.Height));

            Y = 0;

            if (AppSettings.ApplicationUser != null)
            {
                myScheduleViewExplorer.View.Frame = new CGRect(dividerView.Frame.Right, topBorderView.Frame.Bottom, rightWidth, View.Frame.Height - (titleheaderView.Frame.Height + topBorderView.Frame.Height));
            }
            else
            {
                loginViewController.View.Frame = new CGRect(dividerView.Frame.Right, topBorderView.Frame.Bottom, rightWidth, View.Frame.Height - (titleheaderView.Frame.Height + topBorderView.Frame.Height));
            }

            if (config.explore.show_banner)
            {//Show Banner
                introViewExploreViewObj.View.Frame = new CGRect(0, Y, leftWidth, (AppTheme.WHannounceMentCellHeight + AppTheme.WHsectionheaderHeight));
                Y += introViewExploreViewObj.View.Frame.Height;
            }
            else
            {
                if (introViewExploreViewObj != null)
                    introViewExploreViewObj.View.Frame = CGRect.Empty;
            }

            if (config.explore.show_sessions)
            {
                if (showOnNow)
                {
                    onNowView.View.Frame = new CGRect(0, Y, leftWidth, nextUpViewheight);
                    Y += onNowView.View.Frame.Height;
                }
                else
                {
                    if (onNowView != null)
                        onNowView.View.Frame = CGRect.Empty;
                }

                if (showNextUp && nextUpViewExplore != null)
                {
                    nextUpViewExplore.View.Frame = new CGRect(0, Y, leftWidth, nextUpViewheight);
                    Y += nextUpViewExplore.View.Frame.Height;
                }
                else
                {
                    if (nextUpViewExplore != null)
                        nextUpViewExplore.View.Frame = CGRect.Empty;
                }
            }
            else
            {
                if (onNowView != null)
                    onNowView.View.Frame = CGRect.Empty;

                if (nextUpViewExplore != null)
                    nextUpViewExplore.View.Frame = CGRect.Empty;
            }

            if (config.explore.show_recommended_sessions)
            {//Show Sessions
                nfloat tableheight = 0;
                if (featuredSessionViewExplore != null && featuredSessionViewExplore.lstBuiltSessionTime != null)
                {
                    tableheight = featuredSessionViewExplore.lstBuiltSessionTime.Count * AppTheme.featuredSessionCellRowHeight;
                    tableheight += AppTheme.SectionheaderHeight;
                    if (tableheight > featuredSessionTableHeight)
                    {
                        tableheight = featuredSessionTableHeight;
                    }
                }
                featuredSessionViewExplore.View.Frame = new CGRect(0, Y, leftWidth, tableheight);
                Y += featuredSessionViewExplore.View.Frame.Height;
            }
            else
            {
                if (featuredSessionViewExplore != null)
                    featuredSessionViewExplore.View.Frame = CGRect.Empty;
            }

            if (Y < View.Frame.Height)
                Y = View.Frame.Height;

            baseScrollView.ContentSize = new CGSize(leftWidth, Y);

        }

        public override void OnUserLoggedIn(NSNotification notification)
        {
            base.OnUserLoggedIn(notification);
            if (loginViewController.View != null)
            { loginViewController.View.RemoveFromSuperview(); }

            if (myScheduleViewExplorer == null)
            {
                myScheduleViewExplorer = new MyScheduleViewExplorer(featuredSessionViewExplore, rect);
                myScheduleViewExplorer.LoadingData = true;
                myScheduleViewExplorer._featuredSessionViewExplore = featuredSessionViewExplore;
                myScheduleViewExplorer._whatsHappeningNewController = this;
                myScheduleViewExplorer.positionInParentView = LayoutEnum.RightPosition;
            }

            if (myScheduleViewExplorer.View != null)
            {
                myScheduleViewExplorer.LoadingData = true;
                myScheduleViewExplorer.indicator.StartAnimating();
                View.AddSubviews(myScheduleViewExplorer.View);
            }
        }
        public override void OnUserLoggedOut(NSNotification notification)
        {
            base.OnUserLoggedOut(notification);

            if (myScheduleViewExplorer.View != null)
            {
                myScheduleViewExplorer.LoadingData = false;
                myScheduleViewExplorer.indicator.StartAnimating();
                myScheduleViewExplorer.actionNullMySchedule();
                myScheduleViewExplorer.View.RemoveFromSuperview();

                if (loginViewController == null)
                {
                    loginViewController = new LoginViewController(rect, true);
                }

                View.AddSubview(loginViewController.View);
            }
            featuredSessionViewExplore.actionUpdateVisibleCells();
        }
        public override void OnAfterLoginDataFetched(NSNotification notification)
        {
            base.OnAfterLoginDataFetched(notification);
            if (myScheduleViewExplorer != null)
            {
                //myScheduleViewExplorer.ReloadMySchedule(() => 
                //{
                //    if (featuredSessionViewExplore != null)
                //    {
                //        featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
                //    }
                //});
                myScheduleViewExplorer.actionReloadmySchedule();
            }
        }
        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.intro))
            {
                introViewExploreViewObj.actionReloadIntrotable();
            }

            //if (updatedUids != null && updatedUids.Contains(ApiCalls.track))
            //{
            //    featuredSessionViewExplore.actionUpdateTrackList();
            //}

            if (updatedUids != null && updatedUids.Contains(ApiCalls.config))
            {
                DataManager.GetConfig(AppDelegate.Connection).ContinueWith(t =>
                {
                    AppDelegate.instance().config = t.Result;
                    config = AppDelegate.instance().config;
                    InvokeOnMainThread(() =>
                    {
                        ViewWillLayoutSubviews();
                    });
                });
            }

            if (updatedUids != null && (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended)))
            {
                if (updatedUids.Contains(ApiCalls.session))
                {
                    if (onNowView != null)
                    {
                        onNowView.RefreshOnNow(() =>
                        {
                            if (nextUpViewExplore != null)
                            {
                                nextUpViewExplore.RefreshNextUp(() =>
                                {
                                    if (updatedUids.Contains(ApiCalls.recommended))
                                    {
                                        DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                                        {
                                            AppSettings.FeaturedSessions = fs.Result;
                                            featuredSessionViewExplore.actionUpdateFeaturedSesion();
                                        });
                                    }
                                });
                            }
                        });
                    }
                    else
                    {
                        if (nextUpViewExplore != null)
                        {
                            nextUpViewExplore.RefreshNextUp(() =>
                            {
                                if (updatedUids.Contains(ApiCalls.recommended))
                                {
                                    DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                                    {
                                        AppSettings.FeaturedSessions = fs.Result;
                                        featuredSessionViewExplore.actionUpdateFeaturedSesion();
                                    });
                                }
                            });
                        }
                        else
                        {
                            if (updatedUids.Contains(ApiCalls.recommended))
                            {
                                DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                                {
                                    AppSettings.FeaturedSessions = fs.Result;
                                    featuredSessionViewExplore.actionUpdateFeaturedSesion();
                                });
                            }
                        }
                    }

                    if (myScheduleViewExplorer != null)
                    {
                        if (myScheduleViewExplorer.actionReloadmySchedule != null)
                        {
                            myScheduleViewExplorer.actionReloadmySchedule();
                        }
                    }
                }
                else if (updatedUids.Contains(ApiCalls.recommended))
                {
                    DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                    {
                        AppSettings.FeaturedSessions = fs.Result;
                        featuredSessionViewExplore.actionUpdateFeaturedSesion();
                    });
                }
            }

            //if (updatedUids.Contains(ApiCalls.recommended))
            //{
            //    DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
            //    {
            //        AppSettings.FeaturedSessions = fs.Result;
            //        featuredSessionViewExplore.actionUpdateFeaturedSesion();
            //    });
            //}

            ViewWillLayoutSubviews();
        }

        //public override void OnUpdateSessions(NSNotification notification)
        //{
        //    if (onNowView != null)
        //    {
        //        onNowView.RefreshOnNow(() =>
        //        {
        //            if (nextUpViewExplore != null)
        //            {
        //                nextUpViewExplore.RefreshNextUp(() =>
        //                {
        //                    if (myScheduleViewExplorer != null)
        //                    {
        //                        myScheduleViewExplorer.ReloadMySchedule(() =>
        //                        {
        //                            if (featuredSessionViewExplore != null)
        //                            {
        //                                featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
        //                            }
        //                        });
        //                    }
        //                });
        //            }
        //        });
        //    }
        //    else if (nextUpViewExplore != null)
        //    {
        //        nextUpViewExplore.RefreshNextUp(() =>
        //        {
        //            if (myScheduleViewExplorer != null)
        //            {
        //                myScheduleViewExplorer.ReloadMySchedule(() =>
        //                {
        //                    if (featuredSessionViewExplore != null)
        //                    {
        //                        featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
        //                    }
        //                });
        //            }
        //        });
        //    }
        //    else if (myScheduleViewExplorer != null)
        //    {
        //        myScheduleViewExplorer.ReloadMySchedule(() =>
        //        {
        //            if (featuredSessionViewExplore != null)
        //            {
        //                featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
        //            }
        //        });
        //    }
        //    else if (featuredSessionViewExplore != null)
        //    {
        //        featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
        //    }
        //}

        NSObject sessionAddedToSchedule;
        NSObject sessionRemovedFromSchedule;
        public void AddObserver()
        {
			sessionAddedToSchedule = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.SessionAddedToSchedule), (notification) =>
            {
                var data = notification.UserInfo.ValueForKey(new NSString("session_time")).ToString();
                var session_time = JsonConvert.DeserializeObject<BuiltSessionTime>(data);
                if (AppSettings.MySessionIds == null)
                {
                    AppSettings.MySessionIds.Add(session_time.session_time_id);
                }
                else if (!AppSettings.MySessionIds.Contains(session_time.session_time_id))
                {
                    AppSettings.MySessionIds.Add(session_time.session_time_id);
                }
                featuredSessionViewExplore.actioUpdateVisibleCellsWithMyScheduleIds(AppSettings.MySessionIds);
                if (myScheduleViewExplorer != null)
                    //myScheduleViewExplorer.ReloadMySchedule(() => 
                    //{
                    //    if (featuredSessionViewExplore != null)
                    //    {
                    //        featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
                    //    }
                    //});

                    myScheduleViewExplorer.actionReloadmySchedule();
            });

			sessionRemovedFromSchedule = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.SessionRemovedFromSchedule), (notification) =>
            {
                try
                {
                    var data = notification.UserInfo.ValueForKey(new NSString("session_time")).ToString();
                    var session_time = JsonConvert.DeserializeObject<BuiltSessionTime>(data);

                    if (AppSettings.MySessionIds == null)
                    {
                        listOfMyScheIdsInFeaturedSession.Remove(session_time.session_time_id);
                    }
                    else if (AppSettings.MySessionIds.Contains(session_time.session_time_id))
                    {
                        AppSettings.MySessionIds.Remove(session_time.session_time_id);
                    }
                    featuredSessionViewExplore.actioUpdateVisibleCellsWithMyScheduleIds(AppSettings.MySessionIds);

                    if (myScheduleViewExplorer != null)
                        myScheduleViewExplorer.actionReloadafterSessionRemoved();
                }
                catch { }
            });
        }

        void SetHeaderView()
        {
            titleheaderView = new TitleHeaderView(AppTheme.WhatsHappeningText, true, false, false, false, false, false, false, false);
            titleheaderView.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.WHtitleHeaderViewHeight);
            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin;
            View.AddSubview(titleheaderView);
        }
    }
}