using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreFoundation;
using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS;
using ConferenceAppiOS.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;
using CoreGraphics;
using CoreAnimation;
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS
{
    public class MyScheduleViewExplorer : BaseViewController
    {
        public UITableView myScheduleTableview;
        public List<BuiltTracks> lstAllbuildTracks;
        public Dictionary<string, List<BuiltSessionTime>> dictMyScheBuiltSessionTime;
        CGRect _rect;
        UIView myScheduleHeaderview;
        public List<string> listOfMyScheIdsInFeaturedSession;
        public UIView bottomBorderForHeader;
        public bool shouldShowMySchedule;
        private UILabel _lblMyscheduleHeader;
        public FeaturedSessionViewExplore _featuredSessionViewExplore;
        public Action actionNullMySchedule;
        public Action actionReloadmySchedule;
        public Action actionReloadafterSessionRemoved;
        public WhatsHappeningNewController _whatsHappeningNewController;
        public NSIndexPath selectedIndex;
		public bool LoadingData = false;
		UIActivityIndicatorView _indicator;
		public UIActivityIndicatorView indicator{
			get{
				if (_indicator == null) {
					_indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.White);
				}
				return _indicator;
			}
		}

		nfloat indicatorWidth = 40;
        public LayoutEnum positionInParentView { get; set; }
        public UILabel lblMyscheduleHeader
        {
            get
            {
                if (_lblMyscheduleHeader == null)
                {
                    _lblMyscheduleHeader = new UILabel();
                    _lblMyscheduleHeader.Frame = new CGRect(AppTheme.sectionheaderTextLeftPadding, 10, 200, 30);
                    _lblMyscheduleHeader.Font = AppFonts.ProximaNovaRegular(18);
                    _lblMyscheduleHeader.TextColor = AppTheme.SpeakerSessionTitleColor;
                    _lblMyscheduleHeader.Text = AppTheme.myScheduleText;
                }
                return _lblMyscheduleHeader;
            }
        }
        public MyScheduleViewExplorer(CGRect rect)
        {
            _rect = rect;
            View.Frame = _rect;
        }
        public MyScheduleViewExplorer(FeaturedSessionViewExplore featuredSessionViewExplore, CGRect rect)
        {
            _rect = rect;
            View.Frame = _rect;
            _featuredSessionViewExplore = featuredSessionViewExplore;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Clear;
            actionNullMySchedule = nullMyScheduleTable;
            actionReloadmySchedule = ReloadMySchedule;
            actionReloadafterSessionRemoved = ReloadAfterRemovedFromMySchedule;
            myScheduleHeaderview = new UIView();
            myScheduleHeaderview.BackgroundColor = AppTheme.SectionHeaderBackColor;
            bottomBorderForHeader = new UIView();
            bottomBorderForHeader.BackgroundColor = AppTheme.WHBorderColor;
            myScheduleHeaderview.AddSubviews(lblMyscheduleHeader, bottomBorderForHeader);
            myScheduleTableview = new UITableView();
            myScheduleTableview.BackgroundColor = AppTheme.MyScduleBgColor;
            myScheduleTableview.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));
            myScheduleTableview.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            myScheduleTableview.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            lstAllbuildTracks = AppSettings.AllTracks;
			indicator.Frame = new CGRect ((this.View.Frame.Width - indicatorWidth) / 2, (this.View.Frame.Size.Height - indicatorWidth) / 2, indicatorWidth, indicatorWidth);

            builtMyScheduleFromDB((p) =>
               {
                   dictMyScheBuiltSessionTime = p;
                   if (dictMyScheBuiltSessionTime != null && dictMyScheBuiltSessionTime.Count != 0)
                   {
                       var list = dictMyScheBuiltSessionTime.SelectMany(q => q.Value).ToList();
                       listOfMyScheIdsInFeaturedSession = list.Select(r => r.session_time_id).ToList();
                       AppSettings.MySessionIds = listOfMyScheIdsInFeaturedSession;

                       InvokeOnMainThread(() =>
                       {
						   indicator.StopAnimating();
                           myScheduleTableview.Source = new MyScheduleTableSource(this, dictMyScheBuiltSessionTime);
                           myScheduleTableview.ReloadData();
                           if (_featuredSessionViewExplore.actionReloadFeaturedsessiontable != null)
                           {
                               _featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
                           }
                       });
                   }
                   else if (dictMyScheBuiltSessionTime == null || dictMyScheBuiltSessionTime.Count == 0)
                   {
						if(LoadingData == false){
							InvokeOnMainThread(() =>
								{
									indicator.StopAnimating ();
									if (myScheduleTableview.Source == null)
									{
										myScheduleTableview.Source = new MyScheduleTableSource(this, dictMyScheBuiltSessionTime);
										myScheduleTableview.ReloadData();
									}
								});
						}else{

							indicator.StartAnimating ();
						}
                   }
               });

            View.AddSubviews(myScheduleTableview, myScheduleHeaderview);
			View.AddSubview (_indicator);
        }

        private void builtMyScheduleFromDB(Action<Dictionary<string, List<BuiltSessionTime>>> callback)
        {
            DataManager.GetMyScheduleToday(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;
                if (callback != null)
                    callback(result);

            });
        }
			
        public void ReloadMySchedule()
        {
			LoadingData = false;

            DataManager.GetMyScheduleToday(AppDelegate.Connection).ContinueWith(t =>
            {
                lstAllbuildTracks = AppSettings.AllTracks;
                var result = t.Result;
                dictMyScheBuiltSessionTime = result;
                if (dictMyScheBuiltSessionTime.Count > 0)
                {
                    AppSettings.MySessionIds = dictMyScheBuiltSessionTime.SelectMany(q => q.Value).Select(r => r.session_time_id).ToList();

                    InvokeOnMainThread(() =>
                    {
						indicator.StopAnimating ();

                        if (myScheduleTableview.Source == null)
                        {
                            myScheduleTableview.Source = new MyScheduleTableSource(this, dictMyScheBuiltSessionTime);
                    
                            myScheduleTableview.ReloadData();
                        }
                        else
                        {
                            (myScheduleTableview.Source as MyScheduleTableSource).UpdateSource(this, dictMyScheBuiltSessionTime);
                            myScheduleTableview.ReloadData();
                        }

                        if (_featuredSessionViewExplore.actionReloadFeaturedsessiontable != null)
                        {
                            _featuredSessionViewExplore.actionReloadFeaturedsessiontable(true);
                        }

                        //if (completionHandler != null)
                        //{
                        //    completionHandler();
                        //}
                    });
                }

            });
        }
        public void ReloadAfterRemovedFromMySchedule()
        {
            DataManager.GetMyScheduleToday(AppDelegate.Connection).ContinueWith(t =>
            {
                dictMyScheBuiltSessionTime = t.Result;
                InvokeOnMainThread(() =>
                {
                    if (dictMyScheBuiltSessionTime.Count > 0)
                    {
							InvokeOnMainThread(() =>
							{
								if(myScheduleTableview.Source == null)
									myScheduleTableview.Source = new MyScheduleTableSource(this, new Dictionary<string, List<BuiltSessionTime>>());

								(myScheduleTableview.Source as MyScheduleTableSource).UpdateSource(this, dictMyScheBuiltSessionTime);
		                        myScheduleTableview.ReloadData();
							});
                    }
                    else if (dictMyScheBuiltSessionTime != null && dictMyScheBuiltSessionTime.Count == 0)
                    {
                        InvokeOnMainThread(() =>
                        {
							if(myScheduleTableview.Source == null)
								myScheduleTableview.Source = new MyScheduleTableSource(this, new Dictionary<string, List<BuiltSessionTime>>());

                            (myScheduleTableview.Source as MyScheduleTableSource).UpdateSource(this, dictMyScheBuiltSessionTime);
                            myScheduleTableview.ReloadData();
                        });
                    }
                    else if (dictMyScheBuiltSessionTime == null)
                    {
                        InvokeOnMainThread(() =>
                        {
							if(myScheduleTableview.Source == null)
								myScheduleTableview.Source = new MyScheduleTableSource(this, new Dictionary<string, List<BuiltSessionTime>>());

                            (myScheduleTableview.Source as MyScheduleTableSource).UpdateSource(this, dictMyScheBuiltSessionTime);
                            myScheduleTableview.ReloadData();
                        });
                    }
                });
            });
        }

        private void updateTrackList()
        {
            DataManager.GetListOfTrack(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;
                lstAllbuildTracks = result;
            });

        }
        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            base.OnDeltaCompleted(notification, updatedUids);
            if (updatedUids != null && updatedUids.Contains(ApiCalls.track))
            {
                updateTrackList();
            }
        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
			indicator.Frame = new CGRect ((this.View.Frame.Width - indicatorWidth) / 2, (this.View.Frame.Size.Height - indicatorWidth) / 2, indicatorWidth, indicatorWidth);
            myScheduleHeaderview.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.SectionheaderHeight);
            bottomBorderForHeader.Frame = new CGRect(0, View.Frame.Height, View.Frame.Width, 1);
            myScheduleTableview.Frame = new CGRect(0, myScheduleHeaderview.Frame.Bottom, View.Frame.Width, View.Frame.Height - (myScheduleHeaderview.Frame.Height));

        }
        public void nullMyScheduleTable()
        {
            if (myScheduleTableview != null)
            {
                InvokeOnMainThread(() =>
                {
						indicator.StartAnimating();
						if(LoadingData == false){
		                    myScheduleTableview.Source = new MyScheduleTableSource(this, new Dictionary<string, List<BuiltSessionTime>>());
		                    myScheduleTableview.ReloadData();
						}
                });
            }
        }
    }
    public class MyScheduleTableSource : UITableViewSource
    {
        MyScheduleViewExplorer _mkyScheduleViewExplorer;
        NSString scheduleCellIdentifier = new NSString("Schedule");
        NSString cellIdentifierToday = new NSString("TodayTomorrowCell");
        NSString noSessionCellIdentifier = new NSString("NoSession");
        Dictionary<string, List<BuiltSessionTime>> _dictMyschedule;
		static nfloat sectionHeaderYPadding = 20;
        string[] keys;

        public MyScheduleTableSource(MyScheduleViewExplorer mkyScheduleViewExplorer, Dictionary<string, List<BuiltSessionTime>> dictMyschedule)
        {
            _dictMyschedule = dictMyschedule;
            _mkyScheduleViewExplorer = mkyScheduleViewExplorer;
            if (_dictMyschedule != null)
            { keys = _dictMyschedule.Keys.ToArray(); }
        }
        public void UpdateSource(MyScheduleViewExplorer mkyScheduleViewExplorer, Dictionary<string, List<BuiltSessionTime>> dicttableItems)
        {
            _dictMyschedule = dicttableItems;
            _mkyScheduleViewExplorer = mkyScheduleViewExplorer;
            if (_dictMyschedule != null)
            { this.keys = _dictMyschedule.Keys.ToArray(); }
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            if (_dictMyschedule != null && _dictMyschedule.Count > 0)
            {
                return keys.Length;
            }
            else if (_dictMyschedule == null)
            {
                return 1;
            }
            else
            { return 1; }
        }
        public override string TitleForHeader(UITableView tableView, nint section)
        {
            if (_dictMyschedule != null && _dictMyschedule.Count > 0)
            {
                return keys[section];
            }
            else if (_dictMyschedule == null)
            {
                return "";
            }
            else
            { return ""; }
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.myScheduleSectionHeaderHeight))
            {
                BackgroundColor = AppTheme.myScduleSectionHeaderBgColor
            };
            if (_dictMyschedule == null || _dictMyschedule.Count == 0)
            {
                view = null;
                return view;
            }
            else
            {
                var borderBottom = new CALayer();
                borderBottom.Frame = new CGRect(0, view.Frame.Size.Height, tableView.Frame.Width, 1);
                borderBottom.BackgroundColor = AppTheme.WHCellHeaderBorderColor.CGColor;

                var borderTop = new CALayer();
                borderTop.Frame = new CGRect(0, 0, tableView.Frame.Width, 1);
                borderTop.BackgroundColor = AppTheme.WHCellHeaderBorderColor.CGColor;
                view.Layer.AddSublayer(borderTop);
                view.Layer.AddSublayer(borderBottom);
                UILabel lblSectionHeader;
                lblSectionHeader = new UILabel();
                lblSectionHeader.SizeToFit();
                lblSectionHeader = new UILabel(new CGRect(AppTheme.sectionheaderTextLeftPadding, (view.Frame.Height / 2) - sectionHeaderYPadding, view.Frame.Width, view.Frame.Height))
                {
                    TextColor = AppTheme.SpeakerSessionTitleColor,
                    Font = AppFonts.ProximaNovaRegular(18),
                };

                if (_dictMyschedule == null || _dictMyschedule.Count == 0)
                { lblSectionHeader.Text = null; }
                else { lblSectionHeader.Text = keys[section].Replace(",", "").ToUpper(); }
                view.AddSubview(lblSectionHeader);
                return view;
            }
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_dictMyschedule == null)
            {
                return 0;
            }
            if (_dictMyschedule.Count == 0)
            {
                return 0;
            }
            return AppTheme.myScheduleSectionHeaderHeight;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_dictMyschedule != null && _dictMyschedule.Count > 0)
            {
				_mkyScheduleViewExplorer.indicator.StopAnimating ();
                return _dictMyschedule[keys[section]].Count();
            }
			if (_mkyScheduleViewExplorer.indicator.IsAnimating) {
				return 0;
			}
            return 1;
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
			return AppTheme.featuredSessionCellRowHeight;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                if (_dictMyschedule.Count == 0)
                {
                }
                else
                {
                    _mkyScheduleViewExplorer.selectedIndex = indexPath;
                    SessionDetailController vc = new SessionDetailController(_dictMyschedule[keys[indexPath.Section]][indexPath.Row]);
                    vc.View.Frame = new CGRect(-100, 0, 477, _mkyScheduleViewExplorer._whatsHappeningNewController.View.Frame.Size.Height);
                    AppDelegate.instance().rootViewController.openDetail(vc, _mkyScheduleViewExplorer._whatsHappeningNewController, false);
                }
            }
            catch { }
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell noSessioncell = null;
            if (_dictMyschedule == null || _dictMyschedule.Count == 0)
            {
                noSessioncell = tableView.DequeueReusableCell(noSessionCellIdentifier) as NoSessionsCell;
                if (noSessioncell == null)
                {
                    noSessioncell = new NoSessionsCell(noSessionCellIdentifier);
                }
                ((NoSessionsCell)noSessioncell).UpdateCell("");
                return noSessioncell;
            }
            else
            {
                SessionTableCell cell = null;
                cell = tableView.DequeueReusableCell(scheduleCellIdentifier) as SessionTableCell;
                if (cell == null) cell = new SessionTableCell(scheduleCellIdentifier);
                var myScheToday = _dictMyschedule[keys[indexPath.Section]][indexPath.Row];
                cell.UpdateCell(myScheToday, true, _mkyScheduleViewExplorer.lstAllbuildTracks, false);

                #region Session Add remove From My Schedule
                cell.btnAddRemove.TouchUpInside += (s, e) =>
                {
                    if (AppSettings.ApplicationUser != null)
                    {
                        ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                        ((SessionTableCell)cell).btnAddRemove.Enabled = false;

                        if (((SessionTableCell)cell).btnAddRemove.Selected)
                        {
                            DataManager.AddSessionToSchedule(AppDelegate.Connection, myScheToday, (session_time) =>
                            {
                                var setting = new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                };

                                InvokeOnMainThread(() =>
                                {
                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                    NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                                });
                            }, (error) =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                    ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                                    new UIAlertView("", Helper.GetErrorMessage(error), null, "Dismiss").Show();
                                });
                            });
                        }
                        else
                        {
                            if (myScheToday.BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                            myScheToday.BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ToastView.ShowToast(Helper.CanntotUnschedule, 2);
                                cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;
                                cell.btnAddRemove.Enabled = true;
                            }
                            else
                            {
                                var alert = new UIAlertView("", AppSettings.RemoveSessionAlertMessage, null, "Cancel", "Confirm");
                                alert.Clicked += (sender, args) =>
                                {
                                    if (args.ButtonIndex == 1)
                                    {
                                        DataManager.RemoveSessionFromSchedule(AppDelegate.Connection, myScheToday, (err, session_time) =>
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
                                            else
                                            {
                                                InvokeOnMainThread(() => 
                                                {
                                                    ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                                });
                                            }
                                        });
                                    }
                                    else
                                    {
                                        ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                                        ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                    }
                                };
                                alert.Show();
                            }
                        }
                    }
                    else
                    {
                        AppDelegate.instance().ShowLogin();
                        return;
                    }
                };
                #endregion
                return cell;
            }
        }
    }
}