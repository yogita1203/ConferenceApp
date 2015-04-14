using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreFoundation;
using Foundation;
using UIKit;
using CommonLayer;
using CommonLayer.Entities.Built;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using ConferenceAppiOS.CustomControls;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public class FeaturedSessionViewExplore : BaseViewController
    {
        public Action<bool> actionReloadFeaturedsessiontable;
        public Action actionUpdateFeaturedSesion;
        public Action actionUpdateVisibleCells;
        public Action actionUpdateTrackList;
        public Action<List<string>> actioUpdateVisibleCellsWithMyScheduleIds;
        public bool isFeaturedsessionloaded;
        public UITableView featuredTbaleView;
        public List<BuiltTracks> lstFeaturedSesionTrackList;
        public List<BuiltSessionTime> lstBuiltSessionTime;
        CGRect _frame; public UIView headerBorderBottom, headerTopBorder;
        public WhatsHappeningNewController _whatsHappeningNewController; UIView headerView;
        List<string> lst_listOfMyScheIdsInFeaturedSession; public NSIndexPath selectedIndex; UILabel lblSectionHeader;
        public LayoutEnum positionInParentView { get; set; }
        public FeaturedSessionViewExplore(CGRect _rect)
        {
            _frame = _rect;
            View.Frame = _frame;

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            actionReloadFeaturedsessiontable = Reloadfeaturedtable;
            actionUpdateFeaturedSesion = updatefeaturedSession;
            actionUpdateVisibleCells = updateVisibleCells;
            actionUpdateTrackList = updateTrackList;
            actioUpdateVisibleCellsWithMyScheduleIds = (param) =>
            {
                lst_listOfMyScheIdsInFeaturedSession = param;
                foreach (var item in featuredTbaleView.VisibleCells)
                {
                    if (item is SessionTableCell)
                    {
                        var session = ((SessionTableCell)item).model;

                        if (lst_listOfMyScheIdsInFeaturedSession.Contains(session.session_time_id))
                            ((SessionTableCell)item).UpdateCell(session, true, lstFeaturedSesionTrackList, true);
                        else
                            ((SessionTableCell)item).UpdateCell(session, false, lstFeaturedSesionTrackList, true);
                    }
                }
            };
            headerBorderBottom = new UIView();
            headerTopBorder = new UIView();
            featuredTbaleView = new UITableView();

            lstFeaturedSesionTrackList = AppSettings.AllTracks;
            builtSessionTimeListAsPerTrack((p) =>
            {
                lstBuiltSessionTime = p;
                InvokeOnMainThread(() =>
                {
                    featuredTbaleView.Source = new FeaturedSessionTableSource(this, lstBuiltSessionTime);
                    featuredTbaleView.ReloadData();
                    isFeaturedsessionloaded = true;
                });
            });

            featuredTbaleView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));

            headerView = new UIView()
            {
                BackgroundColor = AppTheme.SectionHeaderBackColor,

            };
            headerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            lblSectionHeader = new UILabel()
            {
                TextColor = AppTheme.SpeakerSessionTitleColor,
                Font = AppFonts.ProximaNovaRegular(18),
            };
            lblSectionHeader.Text = AppTheme.featuredSessionText;
            headerBorderBottom.BackgroundColor = AppTheme.WHBorderColor;
            headerTopBorder.BackgroundColor = AppTheme.WHBorderColor;
            headerView.AddSubview(headerBorderBottom);
            headerView.AddSubview(headerTopBorder);
            headerView.AddSubview(lblSectionHeader);
            View.AddSubviews(featuredTbaleView, headerView);
            UIView ss = new UIView();
            ss.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.SectionheaderHeight);
            ss.BackgroundColor = UIColor.White;
            featuredTbaleView.TableHeaderView = ss;

        }
        public void builtSessionTimeListAsPerTrack(Action<List<BuiltSessionTime>> callback)
        {
            //DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(t =>
            //{
            //    var res = t.Result;
            //    if (callback != null)
            //        callback(res);
            //});
            if (callback != null)
                callback(AppSettings.FeaturedSessions);
        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                headerView.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.SectionheaderHeight);
                lblSectionHeader.Frame = new CGRect(AppTheme.sectionheaderTextLeftPadding, 0, headerView.Frame.Width, headerView.Frame.Height);
                featuredTbaleView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
                headerTopBorder.Frame = new CGRect(0, 0, featuredTbaleView.Frame.Width, AppTheme.borderForSectionHeaderTopWidhtVar);
                headerBorderBottom.Frame = new CGRect(0, headerView.Frame.Size.Height, featuredTbaleView.Frame.Width, AppTheme.sectionBottomBorderHeight);
            }
            else
            {
                headerView.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.SectionheaderHeight);
                lblSectionHeader.Frame = new CGRect(AppTheme.sectionheaderTextLeftPadding, 0, headerView.Frame.Width, headerView.Frame.Height);
                featuredTbaleView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
                headerTopBorder.Frame = new CGRect(0, 0, featuredTbaleView.Frame.Width, AppTheme.borderForSectionHeaderTopWidhtVar);
                headerBorderBottom.Frame = new CGRect(0, headerView.Frame.Size.Height, featuredTbaleView.Frame.Width, AppTheme.sectionBottomBorderHeight);
            }

        }
        public void Reloadfeaturedtable(bool shouldLoad)
        {
            InvokeOnMainThread(() =>
            {
                if (featuredTbaleView.Source == null)
                {
                    featuredTbaleView.Source = new FeaturedSessionTableSource(this, lstBuiltSessionTime);
                    featuredTbaleView.ReloadData();
                }
                else
                {
                    (featuredTbaleView.Source as FeaturedSessionTableSource).UpdateSource(lstBuiltSessionTime);
                    featuredTbaleView.ReloadData();
                }
            });
        }

        public void updatefeaturedSession()
        {
            builtTracksFromDB((r) =>
            {
                lstFeaturedSesionTrackList = r;
                //DataManager.GetbuiltSessionTimeListOfTrack(AppDelegate.Connection).ContinueWith(t =>
                //{
                    var res = AppSettings.FeaturedSessions;
                    lstBuiltSessionTime = res;
                    InvokeOnMainThread(() =>
                    {
                        if (featuredTbaleView.Source != null)
                        {
                            (featuredTbaleView.Source as FeaturedSessionTableSource).UpdateSource(lstBuiltSessionTime);
                            featuredTbaleView.ReloadData();
                        }
                        else
                        {
                            featuredTbaleView.Source = new FeaturedSessionTableSource(this, lstBuiltSessionTime);
                            featuredTbaleView.ReloadData();
                        }
                    });
                //});
            });
        }
        public void updateVisibleCells()
        {
            foreach (var item in featuredTbaleView.VisibleCells)
            {
                if (item is SessionTableCell)
                {
                    var session = ((SessionTableCell)item).model;
                    ((SessionTableCell)item).UpdateCell(session, false, lstFeaturedSesionTrackList, true);
                }
            }
        }
        private void builtTracksFromDB(Action<List<BuiltTracks>> callback)
        {
            DataManager.GetListOfTrack(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;

                if (callback != null)
                    callback(result);
            });
        }
        private void updateTrackList()
        {
            DataManager.GetListOfTrack(AppDelegate.Connection).ContinueWith(t =>
            {
                lstFeaturedSesionTrackList = t.Result;
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

        public override void OnUpdateSessions(NSNotification notification)
        {
            lstBuiltSessionTime = AppSettings.FeaturedSessions;
            Reloadfeaturedtable(true);
        }
    }
    public class FeaturedSessionTableSource : UITableViewSource
    {
        FeaturedSessionViewExplore _featuredSessionVwExplore;
        List<BuiltSessionTime> _lstBuiltSessionTime;

        NSString noSessionsCellIdentifier = new NSString("NoSessions");
        NSString sessionCellIdentifier = new NSString("FeaturedSessions");


        public FeaturedSessionTableSource(FeaturedSessionViewExplore featuredSessionVwExplore, List<BuiltSessionTime> lstBuiltSesssionTime)
        {
            _lstBuiltSessionTime = lstBuiltSesssionTime ?? new List<BuiltSessionTime>();
            _featuredSessionVwExplore = featuredSessionVwExplore;

        }
        public void UpdateSource(List<BuiltSessionTime> lstBuiltSesssionTime)
        {
            _lstBuiltSessionTime = lstBuiltSesssionTime;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (_lstBuiltSessionTime.Count > 1)
            {
                return _lstBuiltSessionTime.Count;
            }
            else
            {
                return 1;
            }
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (_lstBuiltSessionTime.Count > 1)
            {
                return AppTheme.featuredSessionCellRowHeight;
            }
            else
            {
                return 200;
            }
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                if (_lstBuiltSessionTime.Count == 0)
                {
                }
                else
                {
                    _featuredSessionVwExplore.selectedIndex = indexPath;
                    SessionDetailController vc = new SessionDetailController(_lstBuiltSessionTime.ElementAt(indexPath.Row));

                    vc.View.Frame = new CGRect(-100, 0, 477, _featuredSessionVwExplore._whatsHappeningNewController.View.Frame.Size.Height);
                    AppDelegate.instance().rootViewController.openDetail(vc, _featuredSessionVwExplore._whatsHappeningNewController, false);
                }
            }
            catch { }
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = null;
            if (_lstBuiltSessionTime.Count > 0)
            {
                cell = tableView.DequeueReusableCell(sessionCellIdentifier) as SessionTableCell;
                if (cell == null) cell = new SessionTableCell(sessionCellIdentifier);
                BuiltSessionTime sessiontime = _lstBuiltSessionTime.ElementAt(indexPath.Row);

                #region--Add Remove Session From My Schedule
                ((SessionTableCell)cell).btnAddRemove.TouchUpInside += (s, e) =>
                {
                    if (AppSettings.ApplicationUser != null)
                    {
                        ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                        ((SessionTableCell)cell).btnAddRemove.Enabled = false;

                        if (((SessionTableCell)cell).btnAddRemove.Selected)
                        {
                            #region ---AddSession

                            DataManager.AddSessionToSchedule(AppDelegate.Connection, _lstBuiltSessionTime.ElementAt(indexPath.Row), (session_time) =>
                            {
                                var setting = new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                };

                                InvokeOnMainThread(() =>
                                {
                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
									ToastView.ShowToast(Helper.SessionAddedString, 2);
                                    NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                                });
                            }, error =>
                            {
                                InvokeOnMainThread(() =>
                                {
                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                    ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;

                                    new UIAlertView("", Helper.GetErrorMessage(error), null, AppTheme.DismissText).Show();
                                });
                            });
                            #endregion
                        }
                        else
                        {
                            #region RemoveSession

                            if (_lstBuiltSessionTime.ElementAt(indexPath.Row).BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                        _lstBuiltSessionTime.ElementAt(indexPath.Row).BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ToastView.ShowToast(Helper.CanntotUnschedule, 2);
                                ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                            }
                            else
                            {
                                var alert = new UIAlertView("", AppSettings.RemoveSessionAlertMessage, null, AppTheme.CancelTextTitle, AppTheme.ConfirmText);
                                alert.Clicked += (sender, args) =>
                                {
                                    if (args.ButtonIndex == 1)
                                    {
                                        DataManager.RemoveSessionFromSchedule(AppDelegate.Connection, _lstBuiltSessionTime.ElementAt(indexPath.Row), (err, session_time) =>
                                        {
                                            if (err == null)
                                            {
                                                var setting = new JsonSerializerSettings
                                                {
                                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                };

                                                InvokeOnMainThread(() =>
                                                {
                                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                                    ToastView.ShowToast(Helper.SessionRemovedString, 2);
                                                    NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionRemovedFromSchedule, null, dictionary);
                                                });
                                            }
                                            else
                                            {
                                                InvokeOnMainThread(() =>
                                                {
                                                    ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                                    ((SessionTableCell)cell).btnAddRemove.Selected = !((SessionTableCell)cell).btnAddRemove.Selected;
                                                });
                                            }
                                        });
                                    }
                                    else
                                    {
                                        ((SessionTableCell)cell).btnAddRemove.Enabled = true;
                                        ((SessionTableCell)cell).btnAddRemove.Selected = true;

                                    }
                                };
                                alert.Show();
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        AppDelegate.instance().ShowLogin();
                        return;
                    }

                };
                #endregion

                if (AppSettings.MySessionIds.Contains(sessiontime.session_time_id))
                {
                    ((SessionTableCell)cell).UpdateCell(sessiontime, true, _featuredSessionVwExplore.lstFeaturedSesionTrackList, true);
                }
                else
                {
                    ((SessionTableCell)cell).UpdateCell(sessiontime, false, _featuredSessionVwExplore.lstFeaturedSesionTrackList, true);
                }

            }
            else if (_lstBuiltSessionTime.Count == 0 || _lstBuiltSessionTime == null)
            {
                cell = tableView.DequeueReusableCell(noSessionsCellIdentifier) as NoSessionsCell;
                if (cell == null)
                {
                    cell = new NoSessionsCell(noSessionsCellIdentifier);
                }
                ((NoSessionsCell)cell).UpdateCell("FeaturedSessions");

            }
            return cell;
        }
    }
}