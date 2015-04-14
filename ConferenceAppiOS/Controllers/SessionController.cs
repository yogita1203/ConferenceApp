using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS.Helpers;
using Newtonsoft.Json;
using BigTed;
using ConferenceAppiOS.CustomControls;
using System.Threading.Tasks;
using System.Globalization;

namespace ConferenceAppiOS
{
    public class SessionController : BaseViewController
    {
        LoadingOverlay loadingOverlay;
        string SessionsTitle = AppTheme.SCsessionsTitleText;
        string SearchTitle = AppTheme.SCspeakesText;
        string speakerSearchText = AppTheme.SCspeakerSearchText;
		static nfloat headerViewLeftMargin = -3;
		static nfloat headerViewRightMargin = 4;
		static nfloat headerHeight = 64;
		static nfloat uisearchBarLeftMargin = 250;
		static nfloat uisearchBarTopMargin = 25;
		static nfloat uisearchBarWidth = 200;
		static nfloat uisearchBarHeight = 35;
		static nfloat btnSearchLeftMargin = 40;
		static nfloat btnSearchTopMargin = 25;
		static nfloat btnSearchSize = 30;
		static nfloat btnFilterLeftMargin = 290;
		static nfloat btnFilterTopMargin = 25;
		static nfloat btnFilterSize = 30;
		static nfloat btnSyncLeftMargin = 330;
		static nfloat btnSyncTopMargin = 25;
		static nfloat btnSyncSize = 30;
		static nfloat lblSyncLeftMargin = 120;
		static nfloat lblSyncTopMargin = 25;
		static nfloat lblSyncWidth = 120;
		static nfloat lblSyncHeight = 30;
		static nfloat titleLeftMargin = 15;
		static nfloat titleTopMargin = 25;
		static nfloat titleWidth = 250;
		static nfloat titleHeight = 30;
		static nfloat searchBarCornerRadius = 2.0f;
		static nfloat searchBarBorderWidth = 1.0f;

		static nfloat sidePanelXPadding = 0;
		static nfloat sidePanelYPadding = 0;
		static nfloat sidePaneWidthPadding = 88;

		static nfloat blankViewXPadding = 0;
		static nfloat blankViewYPadding = 0;
		static nfloat blankViewHeigthPadding = 300;

		static nfloat sidePanelFooterXPadding = 0;
		static nfloat sidePanelFooterYPadding = 0;
		static nfloat sidePanelFooterWidthPadding = 0;
		static nfloat sidePanelFooterHeightPadding = 1;

		static nfloat RightTableViewYPadding = 0;

		static nfloat sessionTableHeaderViewHeight = 120;
		static nfloat daysSegmentControlYPosition = 64;
		static nfloat daysSegmentControlHeight = 55;
		static nfloat RightTableViewFooterXPadding = 0;
		static nfloat RightTableViewFooterYPadding = 0;
		static nfloat RightTableViewFooterWidthPadding = 0;
		static nfloat RightTableViewFooterHeightPadding = 1;

        NSObject sessionAddedToSchedule;
        NSObject sessionRemovedFromSchedule;
		static nfloat speakerTableLeftMargin = 88;
        const int sessionLimit = 50;
        private int sessionOffset = 0;
        const int speakerLimit = 50;
        private int speakerOffset = 0;

        public string CurrentSelectedDate { get; set; }
        public string searchText { get; set; }
        private string SpeakerSearchText { get; set; }

        public string filterTrackName = string.Empty;

        public string subTrackName = string.Empty;
        List<string> lstDate = new List<string>();
        UITableView RightTableView;
        SpeakersTable SpeakerTableView;
        CGRect frm;
        CustomTableView sidePanel;
        FilterViewController Controller;
        SessionDataSource sessionDataSource;
        TitleHeaderView speakerTitleheaderView;
        UIView sessionheaderView;
        TitleHeaderView sessionTitleheaderView;
        SegmentView sessionDaysSegmentControl;
        NSIndexPath indexPath;

        bool sessionControllerVisible = true;

        Dictionary<string, List<BuiltSessionTime>> sessionSource = new Dictionary<string, List<BuiltSessionTime>>();
        Dictionary<string, List<BuiltSpeaker>> speakerSource;
        SpeakersControllerSource speakersControllerSource;
        public SessionController(CGRect rect)
        {
            frm = rect;
            View.Frame = rect;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = frm;
            
            sidePanel = new CustomTableView(CustomTableView.TableType.session);
            sidePanel.Frame = new CGRect(sidePanelXPadding, sidePanelYPadding, sidePaneWidthPadding, View.Frame.Height);

            speakerTitleheaderView = new TitleHeaderView(SearchTitle, true, false, true, false, false, false, false, false);

            speakerTitleheaderView.searchField.EditingChanged += (s, e) =>
            {
                if (speakerTitleheaderView.searchField.Text == string.Empty)
                {
                    searchSpeakers(string.Empty);
                }
            };

            speakerTitleheaderView.searchFieldClicked = (textField) =>
            {
                textField.ResignFirstResponder();
                searchSpeakers(textField.Text);
            };
            speakerTitleheaderView.searchButtonClicked = (textField) =>
            {
                textField.ResignFirstResponder();
                searchSpeakers(textField.Text);
            };
            speakerTitleheaderView.FilterButtonClicked = (btnFilter) =>
            {
                try
                {
                    FilterViewController Controller = new FilterViewController();
                    Controller.View.Frame = new CGRect(0, 0, 500, 500);
                    AppDelegate.instance().rootViewController.popOverController = new UIPopoverController(Controller);
                    AppDelegate.instance().rootViewController.popOverController.PresentFromRect(btnFilter.Bounds, btnFilter, UIPopoverArrowDirection.Any, true);
                }
                catch
                { }
            };
            speakerTitleheaderView.RefreshButtonClicked = () =>
            {
            };

            speakerTitleheaderView.AddButtonClicked = () =>
            {

            };

            speakerTitleheaderView.ShareButtonClicked = () =>
            {

            };

            speakerTitleheaderView.Frame = new CGRect(4, 0, View.Frame.Width - (sidePanel.Frame.Width - headerViewRightMargin), headerHeight);
            View.AddSubview(speakerTitleheaderView); //..


            sidePanel.RowSelectedHandler = (indexPath) =>
            {
                this.indexPath = indexPath;
                if (indexPath.Row == 1)
                {
                    if (SpeakerTableView == null)
                    {
                        LoadingView.Show(String.Empty);
                        SpeakerTableView = new SpeakersTable() { };

                        sessionControllerVisible = false;
                        setSpeakersHeader();

                        getALLSpeakers(res =>
                        {
                            InvokeOnMainThread(() =>
                            {

                                if (SpeakerTableView.Source != null)
                                {
                                    foreach (var item in res)
                                    {
                                        if (speakerSource.ContainsKey(item.Key))
                                            speakerSource[item.Key].AddRange(item.Value);
                                        else
                                            speakerSource.Add(item.Key, item.Value);
                                    }

                                    (SpeakerTableView.Source as SpeakersControllerSource).UpdateSource(speakerSource);
                                    SpeakerTableView.ReloadData();
                                }
                                else
                                {
                                    speakerSource = res;
                                    speakersControllerSource = new SpeakersControllerSource(speakerSource);
                                    SpeakerTableView.Source = speakersControllerSource;
                                    SpeakerTableView.ReloadData();
                                    LoadingView.Dismiss();
                                }
                            });
                        });

                        View.AddSubview(SpeakerTableView);
                        ViewWillLayoutSubviews();
                        //ShowOverlay(SpeakerTableView);
                    }
                    else
                    {
                        sessionControllerVisible = false;
                        View.BringSubviewToFront(SpeakerTableView);
                    }

                    DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.speaker, Helper.ToDateString(DateTime.Now));
                }
                else
                {
                    this.indexPath = null;
                    sessionControllerVisible = true;
                    View.BringSubviewToFront(RightTableView);
                    DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.session, Helper.ToDateString(DateTime.Now));
                }
            };

            sidePanel.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            sidePanel.BackgroundColor = AppTheme.SCsidepanelBackColor;

            var blankView = new UIView();
            blankView.Frame = new CGRect(blankViewXPadding, blankViewYPadding, 50, blankViewHeigthPadding);
            blankView.BackgroundColor = AppTheme.SCblankViewBackColor;
            sidePanel.TableHeaderView = blankView;
            sidePanel.TableFooterView = new UIView(new CGRect(sidePanelFooterXPadding, sidePanelFooterYPadding, sidePanelFooterWidthPadding, sidePanelFooterHeightPadding));

            RightTableView = new UITableView()
            {
                Frame = new CGRect(sidePanel.Frame.Right, RightTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            LoadingView.Show(String.Empty);

               var tracks = AppSettings.AllTracks;
                       getALLSession((res) =>
                       {
                           InvokeOnMainThread(() =>
                           {
                               res = res.OrderBy(u => u.Key).ToDictionary(u => convertToDate(u.Key), u => u.Value);
                               if (RightTableView.Source != null)
                               {
                                   foreach (var item in res)
                                   {
                                       if (sessionSource.ContainsKey(item.Key))
                                           sessionSource[item.Key].AddRange(item.Value);
                                       else
                                           sessionSource.Add(item.Key, item.Value);
                                   }


                                   (RightTableView.Source as SessionDataSource).UpdateSource(sessionSource);
                                   RightTableView.ReloadData();
                               }
                               else
                               {

                                   sessionSource = res;
                                   sessionDataSource = new SessionDataSource(this, sessionSource, tracks);
                                   RightTableView.Source = sessionDataSource;
                                   RightTableView.ReloadData();
                               }
                           });
                       });

            RightTableView.TableFooterView = new UIView(new CGRect(RightTableViewFooterXPadding, RightTableViewFooterYPadding, RightTableViewFooterWidthPadding, RightTableViewFooterHeightPadding));

            View.AddSubviews(sidePanel, RightTableView); //..
            updateSegment();

            AddObserver();
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.session, Helper.ToDateString(DateTime.Now));
        }

        void updateSegment()
        {
            DataManager.GetUniqueSessionDates(AppDelegate.Connection).ContinueWith(t =>
            {
                lstDate = t.Result;
                if (lstDate == null) lstDate = new List<string>();
                lstDate.Insert(0, AppTheme.SCallDaysText);

                InvokeOnMainThread(() =>
                {
                    if (sessionheaderView != null)
                    {
                        sessionheaderView.RemoveFromSuperview();
                        sessionTitleheaderView.RemoveFromSuperview();
                        sessionDaysSegmentControl.RemoveFromSuperview();
                    }
                    sessionheaderView = new UIView(new CGRect(RightTableView.Frame.X + 2, 0, View.Frame.Width - sidePanel.Frame.Width, sessionTableHeaderViewHeight));
                    sessionheaderView.ClipsToBounds = true;
                    sessionTitleheaderView = new TitleHeaderView(SessionsTitle, true, true, true, false, false, false, false, false);
                    sessionheaderView.BackgroundColor = sessionTitleheaderView.BackgroundColor;
                    sessionTitleheaderView.searchField.EditingChanged += (s, e) =>
                    {
                        if (sessionTitleheaderView.searchField.Text == string.Empty)
                        {
                            this.searchText = string.Empty;
                            if (CurrentSelectedDate == null)
                            {
                                CurrentSelectedDate = AppTheme.SCallDaysText;
                            }
                            filterUsingLinq(CurrentSelectedDate, string.Empty);
                        }

                    };
                    sessionTitleheaderView.searchFieldClicked = (textField) =>
                    {
                        textField.ResignFirstResponder();
                        searchText = textField.Text;
                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = AppTheme.SCallDaysText;
                        }
                        filterUsingLinq(CurrentSelectedDate, textField.Text);
                    };

                    sessionTitleheaderView.searchButtonClicked = (textField) =>
                    {
                        textField.ResignFirstResponder();
                        searchText = textField.Text;
                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = AppTheme.SCallDaysText;
                        }
                        filterUsingLinq(CurrentSelectedDate, textField.Text);
                    };

                    sessionTitleheaderView.FilterButtonClicked = (btnFilter) =>
                    {
                        try
                        {
                            Controller = new FilterViewController();
                            Controller.preSelectedTrackName = filterTrackName;
                            Controller.View.Frame = new CGRect(0, 0, 500, 500);
                            AppDelegate.instance().rootViewController.popOverController = new UIPopoverController(Controller);
                            AppDelegate.instance().rootViewController.popOverController.PresentFromRect(btnFilter.Bounds, btnFilter, UIPopoverArrowDirection.Any, true);
                        }
                        catch
                        { }
                    };
                    sessionTitleheaderView.RefreshButtonClicked = () =>
                    {

                    };

                    sessionTitleheaderView.AddButtonClicked = () =>
                    {

                    };

                    sessionTitleheaderView.ShareButtonClicked = () =>
                    {

                    };

                    sessionTitleheaderView.Frame = new CGRect(headerViewLeftMargin + 4, 0, View.Frame.Width - (sidePanel.Frame.Width - (headerViewRightMargin + 4)), headerHeight);
                    sessionheaderView.AddSubview(sessionTitleheaderView);

                    FilterTableSource.action = (s) =>
                    {

                        if (filterTrackName.Contains(s))
                        {
                            filterTrackName = string.Empty;
                            subTrackName = string.Empty;
                        }
                        else
                        {
                            filterTrackName = s;
                            subTrackName = string.Empty;

                        }

                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = AppTheme.SCallDaysText;
                        }

                        filterUsingLinq(CurrentSelectedDate, filterTrackName);
                    };

                    subTrackPickerSource.subTrackHandler = (p) =>
                    {

                        if (filterTrackName.Contains(p))
                        {
                            subTrackName = string.Empty;
                        }
                        else
                        {
                            subTrackName = p;

                        }
                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = AppTheme.SCallDaysText;
                        }
                        filterUsingLinq(CurrentSelectedDate, subTrackName);
                    };

                    if (lstDate != null && lstDate.Count > 0)
                    {
                        var dates = lstDate.Select(p => convertToDateInterest(p)).ToList();

                        sessionDaysSegmentControl = new SegmentView(dates);
                        sessionDaysSegmentControl.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                        sessionDaysSegmentControl.Frame = new CGRect(-3, daysSegmentControlYPosition, sessionheaderView.Frame.Width + 6, daysSegmentControlHeight);
                        sessionDaysSegmentControl.BackgroundColor = AppTheme.SCsegmentbackColor;
                        sessionDaysSegmentControl.segmentViewViewDelegate += daysSegmentControl_segmentViewViewDelegate;
                        sessionDaysSegmentControl.selectedSegmentIndex = 0;
                        sessionDaysSegmentControl.textColor = AppTheme.SCsegmentTitletextColor;
                        sessionDaysSegmentControl.subHeadingTextColor = AppTheme.SCsegmentSubTitletextColor;
                        sessionDaysSegmentControl.font = AppTheme.SCsegmentFont;
                        sessionDaysSegmentControl.selectedBoxColor = AppTheme.SCsegmentSelectedtabColor;
                        sessionDaysSegmentControl.selectedBoxColorOpacity = 1.0f;
                        sessionDaysSegmentControl.selectedBoxBorderWidth = 0.0f;
                        sessionDaysSegmentControl.selectionIndicatorHeight = 4.0f;
                        sessionDaysSegmentControl.selectedTextColor = AppTheme.SCsegmentTitletextColor;
                        sessionDaysSegmentControl.selectionIndicatorColor = AppTheme.SCsegmentSelectedtabBottomColor;
                        sessionDaysSegmentControl.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                        sessionDaysSegmentControl.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                        sessionDaysSegmentControl.multiLineSupport = true;
                        sessionDaysSegmentControl.multiLineFont = AppTheme.SCmultiLineFont;
                        sessionDaysSegmentControl.separatorColor = AppTheme.SCsegmentSeparatorColor;
                        sessionDaysSegmentControl.Layer.CornerRadius = 0.0f;
                        sessionDaysSegmentControl.Layer.BorderColor = AppTheme.SCsegmentSeparatorColor.CGColor;
                        sessionDaysSegmentControl.Layer.BorderWidth = 1.0f;
                        sessionheaderView.AddSubviews(sessionDaysSegmentControl);
                    }

                    View.AddSubview(sessionheaderView);

                    SessionTableHeaderView();
                });
            });
        }

        public string convertToDateInterest(string Date)
        {
            if (String.IsNullOrEmpty(Date))
                return String.Empty;

            if (Date.ToLower().Contains("days"))
            {
                return Date;
            }
            if (Date.Contains("-"))
            {
                try
                {
                    var date = DateTime.Parse(Date);
                    return Helper.ToDateTimeString(date, "ddd, MMM dd");
                }
                catch
                {
                    return String.Empty;
                }
            }
            else
            {
                try
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    var format = "ddd MMM dd yyyy";
                    var date = DateTime.ParseExact(Date, format, provider).ToString("ddd, MMM dd");
                    return date;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public void ShowSessionDetailControllerById(string session)
        {
            DataManager.GetSessionTimeFromId(AppDelegate.Connection, session).ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    nfloat vcXPadding = 0;
                    nfloat vcYPadding = 0;
                    nfloat vcWidthPadding = 477;

                    InvokeOnMainThread(() =>
                    {
                        SessionDetailController vc = new SessionDetailController(t.Result);
                        vc.View.Frame = new CGRect(vcXPadding, vcYPadding, vcWidthPadding, this.View.Frame.Size.Height);
                        AppDelegate.instance().rootViewController.openDetail(vc, this, true);
                    });
                }
            });
        }

        public void ShowSpeakerDetailControllerById(string speaker)
        {
            DataManager.GetSpeakerFromId(AppDelegate.Connection, speaker).ContinueWith(t =>
                {
                    if (t.Result != null)
                    {
                     	nfloat vcXPadding = 0;
                        nfloat vcYPadding = 0;
                        nfloat vcWidthPadding = 477;
                        InvokeOnMainThread(() =>
                            {
                                SpeakerDetailController vc = new SpeakerDetailController(t.Result);
                                vc.View.Frame = new CGRect(vcXPadding, vcYPadding, vcWidthPadding, this.View.Frame.Size.Height);
                                AppDelegate.instance().rootViewController.openDetail(vc, this, true);
                            });
                    }
                });
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            AppDelegate.instance().sessiontwitterText = string.Empty;
            AppDelegate.instance().speakertwitterText = string.Empty;
            if (speakersControllerSource != null && speakersControllerSource.selectedIndex != null)
            {
                SpeakerTableView.DeselectRow(speakersControllerSource.selectedIndex, false);

            }
            if (sessionDataSource != null && sessionDataSource.selectedIndex != null)
            {
                RightTableView.DeselectRow(sessionDataSource.selectedIndex, false);
            }

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            RemoveObserver();
        }

        public void AddObserver()
        {
			sessionAddedToSchedule = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.SessionAddedToSchedule), (notification) =>
            {
                var data = notification.UserInfo.ValueForKey(new NSString("session_time")).ToString();
                var session_time = JsonConvert.DeserializeObject<BuiltSessionTime>(data);

                if (!AppSettings.MySessionIds.Contains(session_time.session_time_id))
                {
                    AppSettings.MySessionIds.Add(session_time.session_time_id);
                }

                foreach (var item in RightTableView.VisibleCells)
                {
                    if (item is SessionDataCell)
                    {
                        var session = ((SessionDataCell)item).model;

                        if (session.session_time_id != session_time.session_time_id)
                            continue;

                        if (AppSettings.MySessionIds.Contains(session.session_time_id))
                            ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (RightTableView.Source as SessionDataSource).parentTracks, true);
                        else
                            ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (RightTableView.Source as SessionDataSource).parentTracks, false);
                    }
                }
				});

			sessionRemovedFromSchedule = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.SessionRemovedFromSchedule), (notification) =>
            {
                try
                {
                    var data = notification.UserInfo.ValueForKey(new NSString("session_time")).ToString();
                    var session_time = JsonConvert.DeserializeObject<BuiltSessionTime>(data);

                    if (AppSettings.MySessionIds.Contains(session_time.session_time_id))
                    {
                        AppSettings.MySessionIds.Remove(session_time.session_time_id);
                    }

                    foreach (var item in RightTableView.VisibleCells)
                    {
                        if (item is SessionDataCell)
                        {
                            var session = ((SessionDataCell)item).model;

                            if (AppSettings.MySessionIds.Contains(session.session_time_id))
                                ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (RightTableView.Source as SessionDataSource).parentTracks, true);
                            else
                                ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (RightTableView.Source as SessionDataSource).parentTracks, false);
                        }
                    }
                }
                catch { }
				});
        }

        public void RemoveObserver()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(sessionAddedToSchedule);
            NSNotificationCenter.DefaultCenter.RemoveObserver(sessionRemovedFromSchedule);
        }

        void SessionTableHeaderView()
        {
            if (speakerTitleheaderView != null)
                speakerTitleheaderView.Hidden = true;


            if (RightTableView != null)
                RightTableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width - sidePanel.Frame.Width, sessionTableHeaderViewHeight));
            if (sessionheaderView != null)
                sessionheaderView.Frame = new CGRect(RightTableView.Frame.X + 2, 0, RightTableView.Frame.Width - 2, sessionTableHeaderViewHeight);

            if (sessionheaderView != null)
            {
                sessionheaderView.Hidden = false;
                sessionDaysSegmentControl.Hidden = false;
                sessionTitleheaderView.Frame = new CGRect(headerViewLeftMargin + 2, 0, View.Frame.Width - (sidePanel.Frame.Width - headerViewRightMargin), headerHeight);
                View.BringSubviewToFront(sessionheaderView);
            }
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            nfloat sidePanelXPadding = 0;
            nfloat sidePanelYPadding = 0;
            nfloat sidePanelWidthPadding = 88;

            nfloat RightTableViewYPadding = 0;
            nfloat SpeakerTableViewYPadding = 0;

            sidePanel.Frame = new CGRect(sidePanelXPadding, sidePanelYPadding, sidePanelWidthPadding, View.Frame.Height);
            RightTableView.Frame = new CGRect(speakerTableLeftMargin, RightTableViewYPadding, View.Frame.Width - speakerTableLeftMargin, View.Frame.Height);
            if (SpeakerTableView != null)
            {
                SpeakerTableView.Frame = new CGRect(speakerTableLeftMargin, SpeakerTableViewYPadding, View.Frame.Width - speakerTableLeftMargin, View.Frame.Height);
                setSpeakersHeader();
            }

            if (RightTableView != null && RightTableView.Source != null)
            {
                if (sessionControllerVisible)
                    SessionTableHeaderView();
            }
        }

        List<BuiltSessionTime> lstAllSessions = new List<BuiltSessionTime>();
        private void getALLSession(Action<Dictionary<string, List<BuiltSessionTime>>> callback)
        {
            DataManager.GetSessions(AppDelegate.Connection, sessionLimit, sessionOffset).ContinueWith(t =>
            {
                var res = t.Result;
                lstAllSessions.AddRange(res);
                var allSessions = res.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.OrderBy(q => q.time).ToList());
                if (callback != null)
                {
                    callback(allSessions);
                }

                if (res.Count != 0)
                {
                    sessionOffset += sessionLimit;
                    getALLSession(callback);
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        LoadingView.Dismiss();
                    });
                }
            });
        }

        int limit = 50;
        int offset = 0;
        private void getALLRefreshSession(Action<Dictionary<string, List<BuiltSessionTime>>> callback)
        {
            DataManager.GetSessionTime(AppDelegate.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                lstAllSessions = res;
                var allSessions = res.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.OrderBy(q => q.time).ToList());
                if (callback != null)
                {
                    callback(allSessions);
                }
            });
        }

        private void getALLSpeakers(Action<Dictionary<string, List<BuiltSpeaker>>> callback)
        {
            DataManager.GetSpeakers(AppDelegate.Connection, speakerLimit, speakerOffset).ContinueWith(t =>
            {
                var res = t.Result;
                Dictionary<string, List<BuiltSpeaker>> result = res.GroupBy(p => p.first_name.First().ToString().ToUpper()).ToDictionary(p => p.Key, p => p.ToList());
                if (callback != null)
                {
                    callback(result);
                }

                if (res.Count == speakerLimit)
                {
                    speakerOffset += speakerLimit;
                    getALLSpeakers(callback);
                }
            });
        }

        private void getALLRefreshSpeakers(Action<Dictionary<string, List<BuiltSpeaker>>> callback)
        {
            DataManager.GetSpeakers(AppDelegate.Connection, speakerLimit, speakerOffset).ContinueWith(t =>
            {
                var res = t.Result;
                Dictionary<string, List<BuiltSpeaker>> result = res.GroupBy(p => p.first_name.First().ToString().ToUpper()).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        void daysSegmentControl_segmentViewViewDelegate(int index)
        {
            var date = lstDate[index];
            if (date.ToLower().Contains(AppTheme.SCsingleDayText))
            {
                CurrentSelectedDate = date;
                filterUsingLinq(date);
            }
            else
            {
                var dt = lstDate.Where(p => p == date).FirstOrDefault();
                filterUsingLinq(dt);
            }

        }

        private static string convertToBuiltDate(string p)
        {
            if (p.ToLower().Contains(AppTheme.SCsingleDayText))
            {
                var allDate = p;
                return allDate;
            }

            var date = DateTime.Parse(p).ToString("yyyy-MM-dd");
            return date;
        }

        private static string convertTimeFormat(string time)
        {
            try
            {
                var date = DateTime.Parse(time).ToString("hh:mm tt");
                return date;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string convertToDate(string Date)
        {
            if (String.IsNullOrWhiteSpace(Date))
                return String.Empty;
            var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
            return date;
        }

        Dictionary<string, List<BuiltSessionTime>> source;
        public void filterUsingLinq(string dateTime, string searchData = null)
        {
            source = null;
            offset = 0;
            getSessions(dateTime, res =>
            {
                if (source == null)
                {
                    source = res;
                }
                else
                {
                    foreach (var item in res)
                    {
                        if (source.ContainsKey(item.Key))
                            source[item.Key].AddRange(item.Value);
                        else
                            source.Add(item.Key, item.Value);
                    }
                }

                InvokeOnMainThread(() =>
                {
                    (RightTableView.Source as SessionDataSource).UpdateSource(source);
                    RightTableView.ReloadData();
                });


            });
        }

        public void getSessions(string dateTime, Action<Dictionary<string, List<BuiltSessionTime>>> callback)
        {
            Task.Run(() =>
                {
                    var expectedResult = lstAllSessions.AsEnumerable();
                    CurrentSelectedDate = convertToBuiltDate(dateTime);

                    if (!string.IsNullOrEmpty(this.CurrentSelectedDate) && this.CurrentSelectedDate.ToLower().Contains(AppTheme.SCsingleDayText))
                    {
                        if (!string.IsNullOrEmpty(filterTrackName))
                        {
                            expectedResult = expectedResult.Where(p => p.BuiltSession.track == filterTrackName);
                        }
                        if (!string.IsNullOrEmpty(subTrackName))
                        {
                            expectedResult = expectedResult.Where(p => p.BuiltSession.sub_track_separated == subTrackName);
                        }
                        if (!string.IsNullOrEmpty(this.searchText))
                        {
                            expectedResult = expectedResult.Where(t => t.BuiltSession.title.ToLower().Contains(searchText.ToLower()) || t.BuiltSession.session_id.ToLower() == searchText.ToLower() || t.BuiltSession.abbreviation.ToLower().Contains(searchText.ToLower()));
                        }

                        sessionSource = expectedResult.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => convertToDate(p.Key), p => p.ToArray().OrderBy(q => q.time).ToList());
                        if (callback != null)
                        {
                            callback(sessionSource);
                        }

                        return;
                    }


                    if (!string.IsNullOrEmpty(this.CurrentSelectedDate))
                    {
                        expectedResult = expectedResult.Where(q => q.date == CurrentSelectedDate);
                    }

                    if (!string.IsNullOrEmpty(this.searchText))
                    {
                        expectedResult = expectedResult.Where(t => t.BuiltSession.title.ToLower().Contains(searchText.ToLower()) || t.BuiltSession.session_id.ToLower() == searchText.ToLower() || t.BuiltSession.abbreviation.ToLower().Contains(searchText.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filterTrackName))
                    {
                        expectedResult = expectedResult.Where(p => p.BuiltSession.track == filterTrackName);
                    }

                    if (!string.IsNullOrEmpty(subTrackName))
                    {
                        expectedResult = expectedResult.Where(p => p.BuiltSession.sub_track_separated == subTrackName);
                    }


                    sessionSource = expectedResult.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => convertToDate(p.Key), p => p.ToArray().OrderBy(q => q.time).ToList());
                    if (callback != null)
                    {
                        callback(sessionSource);
                    }
                });
        }


        bool shouldUpdate = true;
        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (indexPath != null)
            {
                if (indexPath.Row == 1)
                {
                    if (updatedUids != null && updatedUids.Contains(ApiCalls.speaker))
                    {
                        updateSpeakerTableSource();
                        return;
                    }
                }
            }
            else
            {
                if (!shouldUpdate)
                    return;
                if (updatedUids == null)
                    return;
                //if (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.track))
                //{
                //    updateTableSource();
                //}
            }
        }

        public override void OnUpdateSessions(NSNotification notification)
        {
            updateTableSource();
        }

        private void updateSpeakerTableSource()
        {
            getALLRefreshSpeakers(res =>
            {
                InvokeOnMainThread(() =>
                {

                    if (SpeakerTableView.Source != null)
                    {
                        foreach (var item in res)
                        {
                            if (speakerSource.ContainsKey(item.Key))
                                speakerSource[item.Key] = item.Value;
                            else
                                speakerSource.Add(item.Key, item.Value);
                        }

                        (SpeakerTableView.Source as SpeakersControllerSource).UpdateSource(speakerSource);
                        SpeakerTableView.ReloadData();
                    }
                    else
                    {
                        speakerSource = res;
                        speakersControllerSource = new SpeakersControllerSource(speakerSource);
                        SpeakerTableView.Source = speakersControllerSource;
                        SpeakerTableView.ReloadData();
                        loadingOverlay.Hide();
                    }
                });
            });
        }

        private void updateTableSource()
        {
            shouldUpdate = false;
            var tracks=AppSettings.AllTracks;
            //DataManager.GetTracks(AppDelegate.Connection).ContinueWith((t) =>
            //    {
                    //AppSettings.AllTracks = tracks;
                    getALLRefreshSession((res) =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            updateSegment();
                            res = res.OrderBy(u => u.Key).ToDictionary(u => convertToDate(u.Key), u => u.Value);
                            if (RightTableView.Source != null)
                            {
                                foreach (var item in res)
                                {
                                    if (sessionSource.ContainsKey(item.Key))
                                        sessionSource[item.Key] = item.Value;
                                    else
                                        sessionSource.Add(item.Key, item.Value);
                                }

                                (RightTableView.Source as SessionDataSource).UpdateSource(sessionSource, tracks);
                                RightTableView.ReloadData();
                                shouldUpdate = true;
                            }
                            else
                            {
                                sessionSource = res;
                                sessionDataSource = new SessionDataSource(this, sessionSource, tracks);
                                RightTableView.Source = sessionDataSource;
                                RightTableView.ReloadData();
                                loadingOverlay.Hide();
                                shouldUpdate = true;
                            }
                        });
                    });
        }

        void setSpeakersHeader()
        {

            if (sessionheaderView != null)
            {
                sessionheaderView.Hidden = true;
                sessionDaysSegmentControl.Hidden = true;
            }
            if (SpeakerTableView != null)
                SpeakerTableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width - (sidePanel.Frame.Width), headerHeight));
            if (speakerTitleheaderView != null)
                speakerTitleheaderView.Frame = new CGRect(SpeakerTableView.Frame.X + 4, 0, SpeakerTableView.Frame.Width - 4, headerHeight);

            if (speakerTitleheaderView != null)
            {
                View.BringSubviewToFront(speakerTitleheaderView);
                speakerTitleheaderView.Hidden = false;
            }
        }

        private void searchSpeakers(string p)
        {
            DataManager.GetSearchSpeakersWithSection(AppDelegate.Connection, p).ContinueWith(t =>
            {
                InvokeOnMainThread(() =>
                {
                    SpeakerTableView.Source = new SpeakersControllerSource(t.Result);
                    SpeakerTableView.ReloadData();
                });
            });
        }

        void ShowOverlay(UIView view)
        {
            var bounds = RightTableView.Bounds;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
            }
            this.loadingOverlay = new LoadingOverlay(bounds);
            view.Add(this.loadingOverlay);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            lstAllSessions = null;
            LoadingView.Dismiss();
        }

        internal void HideKeyboard()
        {
            sessionTitleheaderView.searchField.ResignFirstResponder();
        }
    }

    public class SessionDataSource : UITableViewSource
    {
        SessionController sessionViewController;
        bool displayTime = false;
        public Dictionary<string, List<BuiltSessionTime>> indexedTableItems;
        string[] keys;
		static nfloat defaultCellHeight = 75.0f;
		static nfloat defaultCellSpaceExceptSessionName = 260.0f;
        public NSIndexPath selectedIndex;
        NSString cellIdentifier = new NSString("TableCell");
        Dictionary<string, List<string>> lstTime = new Dictionary<string, List<string>>();
        Dictionary<string, Int32> lstTimeIndexPosition = new Dictionary<string, Int32>();
        public List<BuiltTracks> parentTracks;

        public SessionDataSource(SessionController sessionViewController, Dictionary<string, List<BuiltSessionTime>> items, List<BuiltTracks> sessionTracks)
        {
            this.sessionViewController = sessionViewController;
            indexedTableItems = items;
            keys = items.Keys.ToArray();
            parentTracks = sessionTracks;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return indexedTableItems[keys[section]].Count;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat viewXPadding = 0;
            nfloat viewYPadding = 0;
            nfloat viewHeightPadding = 40;

            UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, viewHeightPadding))
            {
                BackgroundColor = AppTheme.SCtableHeaderbackColor,
            };

            nfloat lblSectionHeaderXPadding = 15;
            nfloat lblSectionHeaderYPadding = 0;
            nfloat lblSectionHeaderWidthPadding = 30;
            UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, lblSectionHeaderYPadding, tableView.Frame.Width - lblSectionHeaderWidthPadding, view.Frame.Size.Height))
            {
                BackgroundColor = AppTheme.SCheaderlabelbackColor,
                TextColor = AppTheme.SCheaderlabelTextColor,
                Font = AppTheme.SCSectionTextFont
            };
            string headerTitle = keys[section].Replace(",", "");
            lblSectionHeader.Text = headerTitle.ToUpper();

            nfloat separatorXPadding = 30;
            nfloat separatorYPadding = 1;
            nfloat separatorHeightPadding = 1;
            var separator = new UIView(new CGRect(separatorXPadding, view.Frame.Bottom - separatorYPadding, view.Frame.Width - 30, separatorHeightPadding));
            separator.BackgroundColor = AppTheme.SCmenuSectionSeparator;

            view.AddSubviews(lblSectionHeader, separator);
            return view;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (String.IsNullOrEmpty(keys[section]))
                return 0;
            return 40;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SessionDataCell cell = tableView.DequeueReusableCell(cellIdentifier) as SessionDataCell;
            if (cell == null) cell = new SessionDataCell(cellIdentifier);
            var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];

            string time = String.Empty;
            if (!String.IsNullOrWhiteSpace(item.time))
                time = DateTime.Parse(item.time).ToString("hh tt");

            if (!lstTime.ContainsKey(keys[indexPath.Section]))
            {
                displayTime = true;
                lstTime.Add(keys[indexPath.Section], new List<string> { time });
                lstTimeIndexPosition.Add(keys[indexPath.Section] + time, indexPath.Row);
            }
            else
            {
                var lst = lstTime[keys[indexPath.Section]];
                if (!lst.Contains((time)))
                {
                    displayTime = true;
                    lst.Add(time);
                    lstTime[keys[indexPath.Section]] = lst;
                    lstTimeIndexPosition.Add(keys[indexPath.Section] + time, indexPath.Row);
                }
                else
                {
                    displayTime = false;
                }
            }


            if (lstTimeIndexPosition[keys[indexPath.Section] + time] != null)
            {
                var indexPathValue = lstTimeIndexPosition[keys[indexPath.Section] + time];
                if (indexPathValue == indexPath.Row)
                {
                    displayTime = true;
                }
            }


            cell.btnAddRemove.TouchUpInside += (s, e) =>
            {
                if (AppSettings.ApplicationUser != null)
                {
                    cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;

                    if (cell.btnAddRemove.Selected)
                    {
                        DataManager.AddSessionToSchedule(AppDelegate.Connection, item, (session_time) =>
                        {

                            AppSettings.MySessionIds.Add(session_time.session_time_id);

                            var setting = new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            };

                            InvokeOnMainThread(() =>
                            {
                                ToastView.ShowToast(Helper.SessionAddedString, 2);
                                NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                            });
                        }, error =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;
                                new UIAlertView("", Helper.GetErrorMessage(error), null, AppTheme.DismissText).Show();
                            });
                        });
                    }
                    else
                    {
                        if (item.BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                        item.BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ToastView.ShowToast(Helper.CanntotUnschedule, 2);
                            cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;
                        }
                        else
                        {
                            var alert = new UIAlertView("", AppSettings.RemoveSessionAlertMessage, null, AppTheme.CancelTextTitle, AppTheme.ConfirmText);
                            alert.Clicked += (sender, args) =>
                            {
                                if (args.ButtonIndex == 1)
                                {
                                    DataManager.RemoveSessionFromSchedule(AppDelegate.Connection, item, (err, session_time) =>
                                    {
                                        if (err == null)
                                        {
                                            var setting = new JsonSerializerSettings
                                            {
                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                            };

                                            InvokeOnMainThread(() =>
                                            {
                                                ToastView.ShowToast(Helper.SessionRemovedString, 2);
                                                NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionRemovedFromSchedule, null, dictionary);
                                            });
                                        }
                                    });
                                }
                                else
                                {
                                    cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;
                                }
                            };
                            alert.Show();
                        }
                    }
                }
                else
                {

                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (sender, arg) =>
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
            };
            if (AppSettings.MySessionIds != null)
            {
                if (AppSettings.MySessionIds.Contains(item.session_time_id))
                    cell.UpdateCell(item, displayTime, parentTracks, true);
                else
                    cell.UpdateCell(item, displayTime, parentTracks, false);
            }
            else
                cell.UpdateCell(item, displayTime, parentTracks, false);


            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return defaultCellHeight;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            sessionViewController.HideKeyboard();
            nfloat vcXPadding = 0;
			nfloat vcYPadding = 0;
			nfloat vcWidthPadding = 477;
            selectedIndex = indexPath;
            var session = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
            SessionDetailController vc = new SessionDetailController(session);
            vc.View.Frame = new CGRect(vcXPadding, vcYPadding, vcWidthPadding, sessionViewController.View.Frame.Size.Height);
            AppDelegate.instance().rootViewController.openDetail(vc, sessionViewController, true);
            StringBuilder sb = new StringBuilder(AppDelegate.instance().sessionTwtter);
            sb.Replace("{title}", session.BuiltSession.title);
            sb.Replace("{abbreviation}", session.BuiltSession.abbreviation);
            var text = sb.ToString();
            AppDelegate.instance().sessiontwitterText = text;
        }

        public void UpdateSource(Dictionary<string, List<BuiltSessionTime>> indexedTableItems, List<BuiltTracks> tracks = null)
        {
            this.indexedTableItems = indexedTableItems;
            this.keys = indexedTableItems.Keys.ToArray();
            if (tracks != null)
            {
                parentTracks = tracks;
            }
        }
    }
}