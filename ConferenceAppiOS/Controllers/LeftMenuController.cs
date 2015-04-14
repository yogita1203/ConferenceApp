using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using Foundation;
using Twitter;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;

namespace ConferenceAppiOS
{
    public class LeftMenuController : BaseViewController
    {
        static CGRect tabButtonFrame = new CGRect(0, 0, 50, 50);
        public CGRect bottonTabViewOpenFrame = new CGRect(48, 2, tabButtonFrame.Width * 3 + 40, tabButtonFrame.Height);
        public CGRect bottonTabViewClosedFrame = new CGRect(-(tabButtonFrame.Width * 3 + 40), 2, tabButtonFrame.Width * 4 + 40, tabButtonFrame.Height);

		static nfloat settingsIconSize = 20, helpIconSize = 20, toolbarIconSize = 25, leftMenuToggleButtonSize = 40;
		static nfloat NotificationIconSize = 20;
		static nfloat settingsIconLeftMargin = 15;
		static nfloat notificationIconLeftMargin = 60;
		static nfloat leftMenuToggleLeftmargin = 10;
		static nfloat userLabelLeftMargin = 15;
		static nfloat userLabelWidth = 120;
		static nfloat headerHeight = 64;
        public static string SelectedMenuTitle;

        string loginName = AppTheme.LMloginTextTitle;

        public NSIndexPath prevSelectedIndexPath;

        UIBarButtonItem tweetBarButton;

        UIBarButtonItem noteBarButton;


        UIBarButtonItem scheduleBarButton;

        CGRect selfFrame;

        Dictionary<string, BuiltSectionItems[]> tableItems;

        UIView _tableHeader;
        UIView tableHeader
        {
            get
            {
                if (_tableHeader == null)
                {
                    _tableHeader = new UIView(new CGRect(0, 0, View.Frame.Width, headerHeight))
                    {
                        BackgroundColor = AppTheme.LMHeaderBackgroudColor,
                    };

                    View.AddSubview(_tableHeader);
                }
                return _tableHeader;
            }
            set
            {
                _tableHeader = value;
            }
        }

        UITableView _tableView;
        UITableView tableView
        {
            get
            {
                if (_tableView == null)
                {
                    _tableView = new UITableView();
                    _tableView.ShowsHorizontalScrollIndicator = false;
                    _tableView.ShowsVerticalScrollIndicator = false;

                }
                return _tableView;
            }
        }

        string link = string.Empty;

        private UIView _bottonTabView;
        public UIView bottonTabView
        {
            get
            {
                if (_bottonTabView == null)
                {
                    var tweetButton = UIButton.FromType(UIButtonType.Custom);
                    tweetButton.Frame = new CGRect(20, 0, tabButtonFrame.Width, tabButtonFrame.Height);
                    tweetButton.Font = AppTheme.LMTwitterFont;
                    tweetButton.SetTitleColor(AppTheme.LMTwitterFontNormalColor, UIControlState.Normal);
                    tweetButton.SetTitleColor(AppTheme.LMTwitterFontSelectedColor, UIControlState.Selected);
                    tweetButton.SetTitleColor(AppTheme.LMTwitterFontHighlightedColor, UIControlState.Highlighted);
                    tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Normal);
                    tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Selected);
                    tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Highlighted);
                    tweetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    tweetButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    tweetButton.TouchUpInside += twitterButtonClicked;

                    var scheduleButton = UIButton.FromType(UIButtonType.Custom);
                    scheduleButton.Frame = tabButtonFrame;
                    scheduleButton.Font = AppTheme.LMScheduleIconFont;
                    scheduleButton.SetTitleColor(AppTheme.LMScheduleNormalColor, UIControlState.Normal);
                    scheduleButton.SetTitleColor(AppTheme.LMScheduleSelectedColor, UIControlState.Selected);
                    scheduleButton.SetTitleColor(AppTheme.LMScheduleHighlightedColor, UIControlState.Highlighted);
                    scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Normal);
                    scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Selected);
                    scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Highlighted);
                    scheduleButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    scheduleButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;

                    scheduleButton.Frame = new CGRect(tweetButton.Frame.Right, 0, tabButtonFrame.Width, tabButtonFrame.Height);

                    scheduleButton.TouchUpInside += scheduleButtonClicked;

                    var notesButton = UIButton.FromType(UIButtonType.Custom);
                    notesButton.Frame = new CGRect(scheduleButton.Frame.Right, 0, tabButtonFrame.Width, tabButtonFrame.Height);
                    notesButton.Font = AppTheme.LMNotesIconFont;
                    notesButton.SetTitleColor(AppTheme.LMNotesIconFontNormalColor, UIControlState.Normal);
                    notesButton.SetTitleColor(AppTheme.LMNotesIconFontSelectedColor, UIControlState.Selected);
                    notesButton.SetTitleColor(AppTheme.LMNotesIconFontHighlightedColor, UIControlState.Highlighted);
                    notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Normal);
                    notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Selected);
                    notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Highlighted);
                    notesButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    notesButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    notesButton.TouchUpInside += notesButtonClicked;

                    _bottonTabView = new UIView();
                    _bottonTabView.Layer.ShadowOffset = new CGSize(-2, 0);
                    _bottonTabView.Layer.ShadowOffset = new CGSize(0, 2);
                    _bottonTabView.Layer.ShadowRadius = 2.0f;
                    _bottonTabView.Layer.ShadowColor = AppTheme.LMbottomTabViewShadowColor.CGColor;
                    _bottonTabView.Layer.BorderColor = AppTheme.LMbottomTabViewBorderColor.CGColor;
                    _bottonTabView.Layer.BorderWidth = 1.0f;
                    _bottonTabView.BackgroundColor = AppTheme.LMbottomTabViewBackColor;
                    _bottonTabView.AddSubviews(tweetButton, scheduleButton, notesButton);
                }
                return _bottonTabView;
            }
        }

        UIButton _plusButton;
        UIButton plusButton
        {
            get
            {
                if (_plusButton == null)
                {
                    _plusButton = UIButton.FromType(UIButtonType.Custom);
                    _plusButton.BackgroundColor = UIColor.White;
                    _plusButton.SetTitle(AppTheme.LMPlusIcon, UIControlState.Normal);
                    _plusButton.SetTitle(AppTheme.LMPlusIcon, UIControlState.Selected);
                    _plusButton.SetTitle(AppTheme.LMPlusIcon, UIControlState.Highlighted);
                    _plusButton.SetTitleColor(AppTheme.LMPlusIconNormalColor, UIControlState.Normal);
                    _plusButton.SetTitleColor(AppTheme.LMPlusIconSelectedColor, UIControlState.Selected);
                    _plusButton.SetTitleColor(AppTheme.LMPlusIconHighlightedColor, UIControlState.Highlighted);
                    _plusButton.Font = AppTheme.LMPlusIconNameFont;
                    _plusButton.TouchUpInside += plusButtonClicked;
                }
                return _plusButton;
            }
        }

        UIView _customtoolBar;
        UIView customtoolBar
        {
            get
            {
                if (_customtoolBar == null)
                {
                    _customtoolBar = new UIView();
                    bottonTabView.Frame = bottonTabViewClosedFrame;
                    plusButton.Frame = new CGRect(0, 0, tabButtonFrame.Width, tabButtonFrame.Height);
                    _customtoolBar.BackgroundColor = AppTheme.LMbottomTabViewBackColor;
                    _customtoolBar.AddSubview(plusButton);
                }
                return _customtoolBar;
            }
        }

        UILabel imgBadge;
        bool showBadge;

        public Action<NSIndexPath> MenuSelectedHandler;
        public LeftMenuController(CGRect rect)
        {
            selfFrame = rect;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void OnObserverNotification(NSNotification notification)
        {
            //base.OnObserverNotification(notification);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = selfFrame;
            View.BackgroundColor = AppTheme.LMBackgroundColor;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            NavigationController.NavigationBarHidden = true;
            prevSelectedIndexPath = NSIndexPath.FromRowSection(0, 0);
            observerName = CommonLayer.TableObserverNames.menu.ToString();
            SetHeader();
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.BackgroundColor = AppTheme.LMBackgroundColor;
            var tweetButton = UIButton.FromType(UIButtonType.Custom);
            tweetButton.Frame = tabButtonFrame;
            tweetButton.Font = AppTheme.LMTwitterFont;
            tweetButton.SetTitleColor(AppTheme.LMTwitterFontNormalColor, UIControlState.Normal);
            tweetButton.SetTitleColor(AppTheme.LMTwitterFontSelectedColor, UIControlState.Selected);
            tweetButton.SetTitleColor(AppTheme.LMTwitterFontHighlightedColor, UIControlState.Highlighted);
            tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Normal);
            tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Selected);
            tweetButton.SetTitle(AppTheme.LMTwitterIcon, UIControlState.Highlighted);
            tweetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            tweetButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            tweetButton.TouchUpInside += twitterButtonClicked;

            var notesButton = UIButton.FromType(UIButtonType.Custom);
            notesButton.Frame = tabButtonFrame;
            notesButton.Font = AppTheme.LMNotesIconFont;
            notesButton.SetTitleColor(AppTheme.LMNotesIconFontNormalColor, UIControlState.Normal);
            notesButton.SetTitleColor(AppTheme.LMNotesIconFontSelectedColor, UIControlState.Selected);
            notesButton.SetTitleColor(AppTheme.LMNotesIconFontHighlightedColor, UIControlState.Highlighted);
            notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Normal);
            notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Selected);
            notesButton.SetTitle(AppTheme.LMNotesIcon, UIControlState.Highlighted);
            notesButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            notesButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            notesButton.TouchUpInside += notesButtonClicked;

            var scheduleButton = UIButton.FromType(UIButtonType.Custom);
            scheduleButton.Frame = tabButtonFrame;
            scheduleButton.Font = AppTheme.LMScheduleIconFont;
            scheduleButton.SetTitleColor(AppTheme.LMScheduleNormalColor, UIControlState.Normal);
            scheduleButton.SetTitleColor(AppTheme.LMScheduleSelectedColor, UIControlState.Selected);
            scheduleButton.SetTitleColor(AppTheme.LMScheduleHighlightedColor, UIControlState.Highlighted);
            scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Normal);
            scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Selected);
            scheduleButton.SetTitle(AppTheme.LMScheduleIcon, UIControlState.Highlighted);
            scheduleButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            scheduleButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
            scheduleButton.TouchUpInside += scheduleButtonClicked;

            tweetBarButton = new UIBarButtonItem(tweetButton);

            noteBarButton = new UIBarButtonItem(notesButton);

            scheduleBarButton = new UIBarButtonItem(scheduleButton);

            this.NavigationController.ToolbarHidden = false;

            DataManager.GetLeftMenu(AppDelegate.Connection).ContinueWith(t =>
                {
                    InvokeOnMainThread(() =>
                        {
                            t.Result.Keys.ToArray();
                            tableView.Source = new MenuTableSource(this, t.Result);
                            NSIndexPath indexPath = NSIndexPath.FromRowSection(0, 0);
                            tableItems = t.Result;

                            if (AppSettings.ApplicationUser == null)
                            {
                                tableItems.Remove(String.Empty);
                            }
                            else
                            {
                                if (!AppSettings.IsGaming())
                                {

                                }
                                if (!AppSettings.IsSurvey())
                                {

                                }
                            }

                            for (int section = 0; section < tableItems.Keys.Count; section++)
                            {
                                for (int row = 0; row < tableItems[tableItems.Keys.ToArray()[section]].Length; row++)
                                {
                                    var item = tableItems[tableItems.Keys.ToArray()[section]][row];
                                    if (item.link == "vmwareapp://leftmenu/explore")
                                    {
                                        indexPath = NSIndexPath.FromRowSection(row, section);
                                        prevSelectedIndexPath = indexPath;
                                    }
                                }
                            }
                            tableView.Source = new MenuTableSource(this, tableItems);
                            tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
                            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);

                        });
                });
            customtoolBar.Hidden = true;
            bottonTabView.Hidden = false;

            View.AddSubview(tableView);

            this.NavigationController.Toolbar.AddSubview(customtoolBar);
        }

        void updateLeftMenu()
        {
            DataManager.GetLeftMenu(AppDelegate.Connection).ContinueWith(t =>
            {
                InvokeOnMainThread(() =>
                {
                    if (AppSettings.ApplicationUser == null)
                        t.Result.Remove(String.Empty);

                    tableItems = t.Result;
                    (tableView.Source as MenuTableSource).UpdateSource(t.Result);
                    tableView.ReloadData();
                });
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.left_menu))
            {
                updateLeftMenu();
            }

            if (updatedUids != null && updatedUids.Contains(ApiCalls.config))
            {
                DataManager.GetConfig(AppDelegate.Connection).ContinueWith(t =>
                {
                    AppDelegate.instance().config = t.Result;
                });
            }

            if (updatedUids != null && updatedUids.Contains(ApiCalls.track))
            {
                DataManager.GetTracks(AppDelegate.Connection).ContinueWith((t) =>
                {
                    AppSettings.AllTracks = t.Result;

                    if (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended))
                    {
                        DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                        {
                            AppSettings.FeaturedSessions = fs.Result;
                            InvokeOnMainThread(() =>
                            {
                                NSNotificationCenter.DefaultCenter.PostNotificationName(UpdateSessions, null);
                            });
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() =>
                        {
                            NSNotificationCenter.DefaultCenter.PostNotificationName(UpdateSessions, null);
                        });
                    }
                });
            }
            else if (updatedUids != null && (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended)))
            {
                DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(fs =>
                {
                    AppSettings.FeaturedSessions = fs.Result;
                    InvokeOnMainThread(() =>
                    {
                        NSNotificationCenter.DefaultCenter.PostNotificationName(UpdateSessions, null);
                    });
                });
            }
        }

        void setToolbars()
        {
            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                customtoolBar.Hidden = true;
                bottonTabView.Hidden = false;
                this.NavigationController.Toolbar.TintColor = AppTheme.LMbottomTabViewBackColor;
                this.NavigationController.Toolbar.Translucent = false;
                var toolbar = new UIBarButtonItem[] {
					tweetBarButton
					, new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace) { Width = 50 }
					, scheduleBarButton
					, new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace) { Width = 50 }
					, noteBarButton

				};
                this.SetToolbarItems(toolbar, false);
                this.NavigationController.ToolbarHidden = false;
            }
            else
            {
                customtoolBar.Frame = new CGRect(0, 0, this.NavigationController.Toolbar.Frame.Width, this.NavigationController.Toolbar.Frame.Height);
                plusButton.Frame = new CGRect(0, 0, View.Frame.Width, tabButtonFrame.Height);
                customtoolBar.Hidden = false;
                bottonTabView.Hidden = true;
                this.SetToolbarItems(new UIBarButtonItem[] {
				}, false);
            }
        }


        void plusButtonClicked(object sender, EventArgs e)
        {
            plusButton.Selected = !plusButton.Selected;
            if (bottonTabView.Frame == bottonTabViewClosedFrame)
                bottonTabView.Hidden = false;


            UIView.Animate(0.25, () =>
                {
                    if (bottonTabView.Frame == bottonTabViewClosedFrame)
                    {
                        bottonTabView.Frame = bottonTabViewOpenFrame;
                    }
                    else
                    {
                        bottonTabView.Frame = bottonTabViewClosedFrame;
                        bottonTabView.Hidden = true;
                    }
                });
        }

        public void AnimateTabView()
        {
            if (bottonTabView.Frame == bottonTabViewOpenFrame)
            {
                plusButton.Selected = false;
                bottonTabView.Hidden = true;
                bottonTabView.Frame = bottonTabViewClosedFrame;
            }
        }

        void twitterButtonClicked(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(AppDelegate.instance().sessiontwitterText))
            {
                if (!string.IsNullOrEmpty(AppDelegate.instance().sessiontwitterText))
                {
                    ShowTwitterPostDialog(AppDelegate.instance().sessiontwitterText);
                }
                else
                {
                    ShowTwitterPostDialog(AppDelegate.instance().hashtags);
                }
            }
            else if (!string.IsNullOrEmpty(AppDelegate.instance().speakertwitterText))
            {
                if (!string.IsNullOrEmpty(AppDelegate.instance().speakertwitterText))
                {
                    ShowTwitterPostDialog(AppDelegate.instance().speakertwitterText);
                }
                else
                {
                    ShowTwitterPostDialog(AppDelegate.instance().hashtags);
                }
            }

            else
            {
                ShowTwitterPostDialog(AppDelegate.instance().hashtags);
            }
        }

        private void ShowTwitterPostDialog(string hashTag)
        {
            if (TWTweetComposeViewController.CanSendTweet)
            {
                var tweet = new TWTweetComposeViewController();
                tweet.SetInitialText(hashTag);
                tweet.SetCompletionHandler((TWTweetComposeViewControllerResult r) =>
                {
                    DismissViewController(true, null);
                    if (r == TWTweetComposeViewControllerResult.Cancelled)
                    {

                    }
                    else
                    {
                        DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.send_tweet, Helper.ToDateString(DateTime.Now));
                    }
                });

                PresentViewController(tweet, true, null);
            }
            else
            {
                try
                {
                    UIAlertView alertView = new UIAlertView("", AppTheme.LMTwitterNotConfiguredText, null, AppTheme.CancelTextTitle);
                    alertView.Show();

                }
                catch { }
            }
        }

        void notesButtonClicked(object sender, EventArgs e)
        {
            if (AppSettings.ApplicationUser != null)
            {
                NSIndexPath indexPath = NSIndexPath.FromRowSection(0, 0);
                for (int section = 0; section < tableItems.Keys.Count; section++)
                {
                    for (int row = 0; row < tableItems[tableItems.Keys.ToArray()[section]].Length; row++)
                    {
                        var item = tableItems[tableItems.Keys.ToArray()[section]][row];
                        if (item.link == "vmwareapp://leftmenu/notes")
                        {

                            indexPath = NSIndexPath.FromRowSection(row, section);

                            if (prevSelectedIndexPath.Section == indexPath.Section &&
                                prevSelectedIndexPath.Row == indexPath.Row)
                                break;

                            LeftMenuCell prevcell = tableView.CellAt(NSIndexPath.FromRowSection(prevSelectedIndexPath.Row, prevSelectedIndexPath.Section)) as LeftMenuCell;
                            if (prevcell != null)
                            {
                                prevcell.Selected = false;
                                prevcell.selected = false;
                            }


                            prevSelectedIndexPath = indexPath;
                            tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                            LeftMenuController.SelectedMenuTitle = item.menuname;

                            prevSelectedIndexPath = indexPath;

                            LeftMenuCell cell = tableView.CellAt(NSIndexPath.FromRowSection(prevSelectedIndexPath.Row, prevSelectedIndexPath.Section)) as LeftMenuCell;
                            cell.Selected = true;
                            cell.selected = true;

                            NSUrl url = NSUrl.FromString("vmwareapp://leftmenu/notes");
                            AppDelegate.instance().openController(url);
                            break;
                        }


                    }
                }
            }
            else
            {
                UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                alertView.Clicked += (s, arg) =>
                {
                    if (arg.ButtonIndex.ToString() == "1")
                    {
                        AppDelegate.instance().ShowLogin();
                        return;
                    }
                    else
                    { }

                };
                alertView.Show();
            }
        }

        void scheduleButtonClicked(object sender, EventArgs e)
        {
            if (AppSettings.ApplicationUser != null)
            {
                NSIndexPath indexPath = NSIndexPath.FromRowSection(0, 0);
                for (int section = 0; section < tableItems.Keys.Count; section++)
                {
                    for (int row = 0; row < tableItems[tableItems.Keys.ToArray()[section]].Length; row++)
                    {
                        var item = tableItems[tableItems.Keys.ToArray()[section]][row];
                        if (item.link == "vmwareapp://leftmenu/schedule")
                        {
                            indexPath = NSIndexPath.FromRowSection(row, section);

                            if (prevSelectedIndexPath.Section == indexPath.Section &&
                                prevSelectedIndexPath.Row == indexPath.Row)
                                break;

                            LeftMenuCell prevcell = tableView.CellAt(NSIndexPath.FromRowSection(prevSelectedIndexPath.Row, prevSelectedIndexPath.Section)) as LeftMenuCell;
                            if (prevcell != null)
                            {
                                prevcell.Selected = false;
                                prevcell.selected = false;
                            }


                            prevSelectedIndexPath = indexPath;
                            tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                            LeftMenuController.SelectedMenuTitle = item.menuname;


                            prevSelectedIndexPath = indexPath;

                            LeftMenuCell cell = tableView.CellAt(NSIndexPath.FromRowSection(prevSelectedIndexPath.Row, prevSelectedIndexPath.Section)) as LeftMenuCell;
                            cell.Selected = true;
                            cell.selected = true;

                            NSUrl url = NSUrl.FromString("vmwareapp://leftmenu/schedule");
                            AppDelegate.instance().openController(url);
                            break;
                        }
                    }
                }

            }
            else
            {
                UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                alertView.Clicked += (s, arg) =>
                {
                    if (arg.ButtonIndex.ToString() == "1")
                    {
                        AppDelegate.instance().ShowLogin();
                        return;
                    }
                    else
                    { }

                };
                alertView.Show();
            }
        }

        void qrCodeButtonClicked(object sender, EventArgs e)
        {

        }

        private void SetHeader()
        {
            if (tableHeader != null)
            {
                tableHeader.RemoveFromSuperview();
                tableHeader = null;
            }

            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {

                tableHeader.Frame = new CGRect(0, 0, View.Frame.Width, headerHeight);

                UIButton settingsButton = UIButton.FromType(UIButtonType.Custom);
                settingsButton.Font = AppTheme.LMSettingFont;
                settingsButton.SetTitleColor(AppTheme.LMSettingFontNormalColor, UIControlState.Normal);
                settingsButton.SetTitleColor(AppTheme.LMSettingFontSelectedColor, UIControlState.Selected);
                settingsButton.SetTitleColor(AppTheme.LMSettingFontHighlightedColor, UIControlState.Highlighted);
                settingsButton.SetTitle(AppTheme.LMSettingsIcon, UIControlState.Normal);
                settingsButton.SetTitle(AppTheme.LMSettingsIcon, UIControlState.Selected);
                settingsButton.SetTitle(AppTheme.LMSettingsIcon, UIControlState.Highlighted);
                settingsButton.Frame = new CGRect(settingsIconLeftMargin, Helper.getCenterY(tableHeader.Frame.Height + 20, settingsIconSize), settingsIconSize, settingsIconSize);
                settingsButton.TouchUpInside += (object sender, EventArgs e) =>
                {
                    if (AppSettings.ApplicationUser != null)
                    {
                        Settingscontroller Controller = new Settingscontroller();
                        var height = 88;
                        if (AppSettings.ApplicationUser == null)
                            height = height / 2;
                        Controller.View.Frame = new CGRect(0, 0, 260, height);
                        AppDelegate.instance().rootViewController.popOverController = new UIPopoverController(Controller);
                        AppDelegate.instance().rootViewController.popOverController.PopoverContentSize = new CGSize(260, height);
                        AppDelegate.instance().rootViewController.popOverController.PresentFromRect(settingsButton.Bounds, settingsButton, UIPopoverArrowDirection.Left, true);
                    }
                    else
                    {
                        AppSettingsViewController appSettingsViewController = new AppSettingsViewController(new CGRect(0, 0, 878, 651));
                        AppDelegate.instance().rootViewController.openInDialougueView(appSettingsViewController, DialogAlign.center);
                    }

                };

                //Notification Button
                UIButton notificationButton = UIButton.FromType(UIButtonType.Custom);
                notificationButton.Font = AppTheme.LMNotificationFont;
                notificationButton.SetTitleColor(AppTheme.LMNotificationNormalColor, UIControlState.Normal);
                notificationButton.SetTitleColor(AppTheme.LMSettingFontSelectedColor, UIControlState.Selected);
                notificationButton.SetTitleColor(AppTheme.LMSettingFontHighlightedColor, UIControlState.Highlighted);
                notificationButton.SetTitle(AppTheme.LMNotificationIcon, UIControlState.Normal);
                notificationButton.SetTitle(AppTheme.LMNotificationIcon, UIControlState.Selected);
                notificationButton.SetTitle(AppTheme.LMNotificationIcon, UIControlState.Highlighted);
                notificationButton.Frame = new CGRect(notificationIconLeftMargin, Helper.getCenterY(tableHeader.Frame.Height + 20, settingsIconSize), NotificationIconSize, NotificationIconSize);
                notificationButton.TouchUpInside += (object sender, EventArgs e) =>
                {
                    NotificationViewController notificationViewController = new NotificationViewController(new CGRect(0, 0, 361, 655));
                    AppDelegate.instance().rootViewController.openInDialougueView(notificationViewController, DialogAlign.center);
                    //					notificationButton.Selected = false;
                    //                    showBadge = false;
                    //                    if (imgBadge != null)
                    //                        imgBadge.Hidden = true;
                };

                if (showBadge)
                {
                    if (imgBadge == null)
                    {
                        imgBadge = new UILabel()
                        {
                            Text = AppTheme.LMBadgeIcon,
                            TextColor = AppTheme.MenuBadgeColor,
                            BackgroundColor = UIColor.Clear,
                            TextAlignment = UITextAlignment.Right,
                            Font = AppTheme.LMBadgeFontSize,
                        };
                    }
                    imgBadge.Frame = new CGRect(2, -12, 25, 25);
                    imgBadge.Hidden = false;
                    notificationButton.Selected = true;
                    notificationButton.AddSubview(imgBadge);
                }
                else
                {
                    if (imgBadge != null)
                        imgBadge.Hidden = true;

                    notificationButton.Selected = false;
                }

                tableHeader.AddSubviews(settingsButton, notificationButton);

                UIButton menuClosedButton = new UIButton();
                menuClosedButton.BackgroundColor = UIColor.Clear;
                menuClosedButton.Font = AppTheme.LMLeftArrowIconFont;
                menuClosedButton.SetTitleColor(AppTheme.LMLeftArrowIconNormalColor, UIControlState.Normal);
                menuClosedButton.SetTitleColor(AppTheme.LMLeftArrowIconSelectedColor, UIControlState.Selected);
                menuClosedButton.SetTitleColor(AppTheme.LMLeftArrowIconHighlightedColor, UIControlState.Highlighted);
                menuClosedButton.SetTitle(AppTheme.LMLeftArrowIcon, UIControlState.Normal);
                menuClosedButton.SetTitle(AppTheme.LMLeftArrowIcon, UIControlState.Selected);
                menuClosedButton.SetTitle(AppTheme.LMLeftArrowIcon, UIControlState.Highlighted);
                menuClosedButton.Frame = new CGRect(tableHeader.Frame.Width - (leftMenuToggleButtonSize), Helper.getCenterY(tableHeader.Frame.Height + 20, leftMenuToggleButtonSize), leftMenuToggleButtonSize, leftMenuToggleButtonSize);
                menuClosedButton.TouchUpInside += menuClicked;
                tableHeader.AddSubview(menuClosedButton);

                UIView separator = new UIView(new CGRect(0, tableHeader.Frame.Bottom, tableHeader.Frame.Width, 1))
                {
                    BackgroundColor = AppTheme.LMmenuHeaderSeparator
                };

                if (AppSettings.ApplicationUser == null)
                {

                    var userLabel = new UIButton(UIButtonType.Custom)
                    {
                        Frame = new CGRect(10, 60, 100, 30),
                    };

                    userLabel.TouchUpInside += (s, e) =>
                    {
                        if (AppSettings.ApplicationUser == null)
                        {
                            AppDelegate.instance().ShowLogin();
                        }
                    };

                    UIView loginView = new UIView();
                    loginView.BackgroundColor = AppTheme.LIBackgroundColor;
                    loginView.Frame = new CGRect(0, separator.Frame.Bottom, tableHeader.Frame.Width, 0);

                    UILabel LoginTextLabel = new UILabel(new CGRect(15, 20, loginView.Frame.Width - 20, 0));
                    LoginTextLabel.Lines = 0;
                    LoginTextLabel.Text = AppTheme.LMloginDetailText;//"Login to your VMworld account to access your personal schedule and notes, take surveys, and play the VMworld Game.";
                    LoginTextLabel.TextColor = AppTheme.LIOtherLabeltextColor;
                    LoginTextLabel.SizeToFit();
                    LoginTextLabel.Frame = new CGRect(15, 20, loginView.Frame.Width - 20, LoginTextLabel.Frame.Height);

                    userLabel.SetTitleColor(AppTheme.LILoginbuttonTitleFontColorInLeftColor, UIControlState.Normal);
                    userLabel.SetTitleColor(AppTheme.LILoginbuttonTitleFontColorInLeftColor, UIControlState.Highlighted);
                    userLabel.SetTitleColor(AppTheme.LILoginbuttonTitleFontColorInLeftColor, UIControlState.Selected);
                    userLabel.SetTitle(loginName, UIControlState.Normal);
                    userLabel.SetTitle(loginName, UIControlState.Highlighted);
                    userLabel.SetTitle(loginName, UIControlState.Selected);
                    userLabel.BackgroundColor = AppTheme.LILoginbuttonBackgroundColorInLeftmenu;
                    userLabel.Layer.CornerRadius = 5.0f;
                    userLabel.Frame = new CGRect(15, LoginTextLabel.Frame.Bottom + 20, loginView.Frame.Width - 30, 35);

                    loginView.Frame = new CGRect(0, separator.Frame.Bottom, tableHeader.Frame.Width, userLabel.Frame.Bottom + 20);

                    loginView.AddSubview(userLabel);
                    loginView.AddSubview(LoginTextLabel);
                    tableHeader.AddSubview(loginView);

                    tableHeader.Frame = new CGRect(0, 0, View.Frame.Width, headerHeight + loginView.Frame.Height);

                }
                else
                {

                    //					tableHeader.Frame = new CGRect (0, 0, View.Frame.Width, headerHeight + loginView.Frame.Height);
                }
                tableHeader.AddSubview(separator);

                nfloat dummyHeaderHeight = tableHeader.Frame.Height;
                if (AppTheme.StatusBar)
                    dummyHeaderHeight -= 20;

                UIView tableheaderBlankView = new UIView(new CGRect(0, 0, tableHeader.Frame.Width, dummyHeaderHeight));

                tableView.TableHeaderView = tableheaderBlankView;

                View.BringSubviewToFront(tableHeader);
            }
            else
            {
                tableHeader.Frame = new CGRect(0, 0, View.Frame.Width, headerHeight);

                UIButton menuClosedButton = new UIButton();
                menuClosedButton.Font = AppTheme.LMRightArrowIconFont;
                menuClosedButton.SetTitleColor(AppTheme.LMRightArrowIconNormalColor, UIControlState.Normal);
                menuClosedButton.SetTitleColor(AppTheme.LMRightArrowIconSelectedColor, UIControlState.Selected);
                menuClosedButton.SetTitleColor(AppTheme.LMRightArrowIconHighlightedColor, UIControlState.Highlighted);
                menuClosedButton.SetTitle(AppTheme.LMRightArrowIcon, UIControlState.Normal);
                menuClosedButton.SetTitle(AppTheme.LMRightArrowIcon, UIControlState.Selected);
                menuClosedButton.SetTitle(AppTheme.LMRightArrowIcon, UIControlState.Highlighted);
                menuClosedButton.Frame = new CGRect(Helper.getCenterX(tableHeader.Frame.Width, leftMenuToggleButtonSize), 20, leftMenuToggleButtonSize, leftMenuToggleButtonSize);
                menuClosedButton.TouchUpInside += menuClicked;
                tableHeader.AddSubview(menuClosedButton);

                UIView separator = new UIView(new CGRect(0, tableHeader.Frame.Bottom, tableHeader.Frame.Width, 1))
                {
                    BackgroundColor = AppTheme.MenuHeaderSeparator
                };
                tableHeader.AddSubview(separator);
                nfloat dummyHeaderHeight = tableHeader.Frame.Height;
                if (AppTheme.StatusBar)
                    dummyHeaderHeight -= 20;

                UIView tableheaderBlankView = new UIView(new CGRect(0, 0, tableHeader.Frame.Width, dummyHeaderHeight));
                tableView.TableHeaderView = tableheaderBlankView;

                View.BringSubviewToFront(tableHeader);
            }
        }

        void menuClicked(object sender, EventArgs e)
        {
            AppDelegate.instance().rootViewController.MenuClicked();
            SetHeader();
            tableView.ReloadData();
            LeftMenuCell cell = tableView.CellAt(NSIndexPath.FromRowSection(prevSelectedIndexPath.Row, prevSelectedIndexPath.Section)) as LeftMenuCell;
            if (cell != null)
            {
                cell.Selected = true;
            }

        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            tableView.Frame = new CGRect(0, 0, View.Frame.Size.Width, View.Frame.Size.Height);
            setToolbars();
            SetHeader();
        }

        public override void OnUserLoggedIn(NSNotification notification)
        {
            DataManager.GetLeftMenu(AppDelegate.Connection).ContinueWith(t =>
                {
                    InvokeOnMainThread(() =>
                        {
                            Dictionary<string, BuiltSectionItems[]> tableItemss = t.Result;
                            if (AppSettings.ApplicationUser == null)
                                tableItemss.Remove(String.Empty);

                            tableItems = tableItemss;

                            (tableView.Source as MenuTableSource).UpdateSource(tableItemss);
                            tableView.ReloadData();
                            NSIndexPath indexPath = NSIndexPath.FromRowSection(0, 0);
                            for (int section = 0; section < tableItems.Keys.Count; section++)
                            {
                                for (int row = 0; row < tableItems[tableItems.Keys.ToArray()[section]].Length; row++)
                                {
                                    var item = tableItems[tableItems.Keys.ToArray()[section]][row];
                                    if (item.link == "vmwareapp://leftmenu/explore")
                                    {
                                        LeftMenuController.SelectedMenuTitle = item.menuname;
                                        indexPath = NSIndexPath.FromRowSection(row, section);
                                        prevSelectedIndexPath = indexPath;
                                    }
                                }
                            }
                            tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                            SetHeader();

                        });
                });
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            LeftMenuController.SelectedMenuTitle = AppTheme.WhatsHappeningText;
            DataManager.GetLeftMenu(AppDelegate.Connection).ContinueWith(t =>
                {
                    InvokeOnMainThread(() =>
                        {
                            Dictionary<string, BuiltSectionItems[]> tableItemss = t.Result;
                            t.Result.Remove(String.Empty);

                            tableItems = tableItemss;

                            (tableView.Source as MenuTableSource).UpdateSource(tableItems);
                            tableView.ReloadData();
                            try
                            {
                                NSIndexPath indexPath = NSIndexPath.FromRowSection(0, 0);
                                for (int section = 0; section < tableItems.Keys.Count; section++)
                                {
                                    for (int row = 0; row < tableItems[tableItems.Keys.ToArray()[section]].Length; row++)
                                    {
                                        var item = tableItems[tableItems.Keys.ToArray()[section]][row];
                                        if (item.link == "vmwareapp://leftmenu/explore")
                                        {
                                            LeftMenuController.SelectedMenuTitle = item.menuname;
                                            indexPath = NSIndexPath.FromRowSection(row, section);
                                            prevSelectedIndexPath = indexPath;
                                        }
                                    }
                                }

                                tableView.SelectRow(indexPath, true, UITableViewScrollPosition.None);
                            }
                            catch(Exception ex) 
                            {
                                new UIAlertView("Error", ex.Message, null, "Dismiss").Show();
                            }
                            SetHeader();
                        });
                });
        }

        public override void OnAfterLoginDataFetched(NSNotification notification)
        {
            ReloadSurveyRow();
        }

        public override void OnReloadLeftMenu(NSNotification notification)
        {
            ReloadSurveyRow();
        }

        public void ReloadSurveyRow()
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    var indexPath = NSIndexPath.FromRowSection(Array.IndexOf(tableItems[""], tableItems[""].FirstOrDefault(p => p.menuname.ToLower().Contains("survey"))), 0);
                    tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
                }
                catch { }
            });
        }

        public void ShowNotificationBadge(bool value)
        {
            showBadge = value;
            SetHeader();
        }
    }

    #region --Menu table source--
    public class MenuTableSource : UITableViewSource
    {
        nfloat rowHeightInShortWidth = 50;
        nfloat rowHeight = 47;

        nfloat sectionHeightInShortWidth = 5;

        LeftMenuController leftMenuController;
        Dictionary<string, BuiltSectionItems[]> tableItems;
        string[] keys;
        NSString cellIdentifier = new NSString("TableCell");
        public MenuTableSource(LeftMenuController leftMenuControllr, Dictionary<string, BuiltSectionItems[]> items)
        {
            leftMenuController = leftMenuControllr;
            tableItems = items;
            keys = items.Keys.ToArray();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Length;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems[keys[section]].Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (String.IsNullOrEmpty(keys[section]) && AppSettings.ApplicationUser == null)
                return 0;

            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                return AppTheme.SectionHeight;
            }
            else
            {
                if (section == 0)
                    return 1;
                return sectionHeightInShortWidth;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (!AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                return rowHeightInShortWidth;
            }
            else
            {
                return rowHeight;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIButton userLabel;
            UIView view;
            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
                {
                    BackgroundColor = AppTheme.LMmenuSectionBackgroundColor,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };

                userLabel = new UIButton(UIButtonType.Custom);
                userLabel.Frame = new CGRect(15, 0, view.Frame.Width - 15, view.Frame.Height);
                userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Normal);
                userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Highlighted);
                userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Selected);
                userLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                userLabel.TitleLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                userLabel.TitleLabel.TextAlignment = UITextAlignment.Left;
                userLabel.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                if (section == 0 && AppSettings.ApplicationUser != null)
                {
                    var user = AppSettings.ApplicationUser;
                    if (user != null)
                    {
                        userLabel.SetTitle(user.first_name, UIControlState.Normal);
                        userLabel.SetTitle(user.first_name, UIControlState.Highlighted);
                        userLabel.SetTitle(user.first_name, UIControlState.Selected);

                    }
                }
                else
                {
                    userLabel.SetTitle(keys[section], UIControlState.Normal);
                    userLabel.SetTitle(keys[section], UIControlState.Highlighted);
                    userLabel.SetTitle(keys[section], UIControlState.Selected);
                }
                userLabel.BackgroundColor = AppTheme.LMmenuSectionBackgroundColor;
                userLabel.Font = AppTheme.LMSectionTitleFont;
                view.AddSubviews(userLabel);
                return view;
            }
            else
            {
                if (section == 0)
                    return new UIView(new CGRect(0, 0, tableView.Frame.Width, 1));

                UIView separator = new UIView(new CGRect(0, 0, tableView.Frame.Width, 5));
                separator.BackgroundColor = AppTheme.MenuSectionBackgroundColor;
                return separator;
            }
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            LeftMenuCell cell = tableView.DequeueReusableCell(cellIdentifier) as LeftMenuCell;
            if (cell == null) cell = new LeftMenuCell(cellIdentifier);
            var item = tableItems[keys[indexPath.Section]][indexPath.Row];

            var menuText = item.menuname.Split('|');
            if (menuText.Length > 1)
                cell.UpdateCell(menuText[0], menuText[1], item.icon_name == null ? "" : item.icon_name, item.icon_code == null ? "" : item.icon_code);
            else
                cell.UpdateCell(menuText[0], String.Empty, item.icon_name == null ? "" : item.icon_name, item.icon_code == null ? "" : item.icon_code);

            if (indexPath.Row == leftMenuController.prevSelectedIndexPath.Row && indexPath.Section == leftMenuController.prevSelectedIndexPath.Section)
            {
                cell.Selected = true;
                cell.selected = true;
            }
            else
            {
                cell.Selected = false;
                cell.selected = false;
            }
            return cell;
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);
            if (cell != null)
            {
                if (cell.Selected)
                    cell.Selected = false;

            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (leftMenuController.prevSelectedIndexPath.Section == indexPath.Section &&
                leftMenuController.prevSelectedIndexPath.Row == indexPath.Row)
                return;

            LeftMenuCell prevcell = tableView.CellAt(NSIndexPath.FromRowSection(leftMenuController.prevSelectedIndexPath.Row, leftMenuController.prevSelectedIndexPath.Section)) as LeftMenuCell;
            if (prevcell != null)
            {
                prevcell.Selected = false;
                prevcell.selected = false;
            }

            var item = tableItems[keys[indexPath.Section]][indexPath.Row];

            LeftMenuController.SelectedMenuTitle = item.menuname;
            NSUrl url = NSUrl.FromString(item.link);
            AppDelegate.instance().openController(url);
            leftMenuController.prevSelectedIndexPath = indexPath;

            LeftMenuCell cell = tableView.CellAt(NSIndexPath.FromRowSection(leftMenuController.prevSelectedIndexPath.Row, leftMenuController.prevSelectedIndexPath.Section)) as LeftMenuCell;
            cell.Selected = true;
            cell.selected = true;
        }

        public void UpdateSource(Dictionary<string, BuiltSectionItems[]> items)
        {
            tableItems = items;
            keys = tableItems.Keys.ToArray();
        }
    }

    #endregion
}
