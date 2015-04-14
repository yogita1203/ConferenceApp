using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using CommonLayer;
using ConferenceAppiOS;
using Newtonsoft.Json;
using ConferenceAppiOS.Views;
using BigTed;
using ConferenceAppiOS.CustomControls;
using System.Globalization;

namespace ConferenceAppiOS
{
    public class ScheduleAndInterestController : BaseViewController
    {
        public enum Diff
        {
            Schedule,
            Interest
        }

        public Diff Panel { get; set; }
        CGRect frm;
        UITableView InterestTableView;
        public UITableView SchedulteTableView;
        UIView IntrestHeaderView;
        UIView headerViewSchedule;
        string InterestTitle = AppTheme.SIinterestTitle;
        List<string> lstDate = new List<string>();
        List<string> interestDate = new List<string>();

        public string CurrentSelectedDate { get; set; }
        public ScheduleAndInterestController(CGRect rect)
        {
            frm = rect;
            View.Frame = rect;
        }
        CustomTableView sidePanel;


		static nfloat headerViewLeftMargin = -3;
		static nfloat headerViewRightMargin = 4;
		static nfloat headerHeight = 64;

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

		static nfloat empyVerticalViewYPadding = 0;
		static nfloat empyVerticalViewWidthPadding = 3;

		static nfloat InterestTableViewYPadding = 0;

		static nfloat intrestTableHeaderViewHeight = 120;
		static nfloat scheduleTableHeaderViewHeight = 120;

		static nfloat InterestTableViewFooterXPadding = 0;
		static nfloat InterestTableViewFooterYPadding = 0;
		static nfloat InterestTableViewFooterWidthPadding = 0;
		static nfloat InterestTableViewFooterHeightPadding = 1;

		static nfloat ScheduleTableViewFooterXPadding = 0;
		static nfloat ScheduleTableViewFooterYPadding = 0;
		static nfloat ScheduleTableViewFooterWidthPadding = 0;
		static nfloat ScheduleTableViewFooterHeightPadding = 1;

        Dictionary<string, BuiltSessionTime[]> interestSource = new Dictionary<string, BuiltSessionTime[]>();
        Dictionary<string, BuiltSessionTime[]> scheduleSource = new Dictionary<string, BuiltSessionTime[]>();
        public NSIndexPath selectedIndex = null;
        public NSIndexPath scheduleSelectedIndex = null;
        MyScheduleSourceFull myScheduleSourceFull;
        ScheduleInterestDataSource scheduleInterestDataSource;
        public int sidePanelSelectedRow;
        NSIndexPath indexPath;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = frm;

            sidePanel = new CustomTableView(CustomTableView.TableType.schedule);
            sidePanel.Frame = new CGRect(sidePanelXPadding, sidePanelYPadding, sidePaneWidthPadding, View.Frame.Height);
            sidePanel.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            var blankView = new UIView();
            blankView.Frame = new CGRect(blankViewXPadding, blankViewYPadding, sidePanel.Frame.Width, blankViewHeigthPadding);
            blankView.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.Layer3Color, 1.0f);
            sidePanel.TableHeaderView = blankView;
            sidePanel.TableFooterView = new UIView(new CGRect(sidePanelFooterXPadding, sidePanelFooterYPadding, sidePanelFooterWidthPadding, sidePanelFooterHeightPadding));

            sidePanel.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.Layer3Color, 1.0f);
            //Empty Vertical View
            var empyVerticalView = new UIView
            {
                BackgroundColor = UIColor.FromRGB(73, 73, 73),
                Frame = new CGRect(sidePanel.Frame.Right, empyVerticalViewYPadding, empyVerticalViewWidthPadding, View.Frame.Height),
            };

            InterestTableView = new UITableView()
            {
                Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            IntrestHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width - sidePanel.Frame.Width, intrestTableHeaderViewHeight));

            SchedulteTableView = new UITableView()
            {
                Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            headerViewSchedule = new UIView();
            var tracks = AppSettings.AllTracks;
            getALLSchedule((res) =>
            {
                scheduleSource = res;
                scheduleSource = res.OrderBy(u => u.Key).ToDictionary(u => convertToDate(u.Key), u => u.Value);
                InvokeOnMainThread(() =>
                {
                    myScheduleSourceFull = new MyScheduleSourceFull(this, scheduleSource, tracks);
                    SchedulteTableView.Source = myScheduleSourceFull;
                    SetScheduleHeaderView();
                    SchedulteTableView.ReloadData();
                    sidePanelSelectedRow = 0;
                });
            });

            sidePanel.RowSelectedHandler = (indexPath) =>
            {
                this.indexPath = indexPath;
                if (indexPath.Row == 1)
                {
                    sidePanelSelectedRow = indexPath.Row;
                    if (InterestTableView != null)
                    {
                        var track = AppSettings.AllTracks;
                        getALLInterests(res =>
                            {
                                interestSource = res;
                                interestSource = res.OrderBy(u => u.Key).ToDictionary(u => convertToDate(u.Key), u => u.Value);
                                InvokeOnMainThread(() =>
                                    {
                                        scheduleInterestDataSource = new ScheduleInterestDataSource(this, interestSource, track);
                                        setInterestHeaderView();

                                        if (interestDate != null && interestDate.Count > 0)
                                        {
                                            IntrestHeaderView.Frame = new CGRect(sidePanel.Frame.Right, 0, View.Frame.Width - sidePanel.Frame.Width, intrestTableHeaderViewHeight);
                                        }
                                        else
                                        {
                                            IntrestHeaderView.Frame = new CGRect(InterestTableView.Frame.X + 4, 0, InterestTableView.Frame.Width - 8, headerHeight);
                                        }
                                        //												InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height - IntrestHeaderView.Frame.Height);
                                        InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height);

                                        InterestTableView.Source = scheduleInterestDataSource;
                                        InterestTableView.ReloadData();
                                    });
                            });

                        setInterestHeaderView();
                        View.BringSubviewToFront(InterestTableView);
                        View.BringSubviewToFront(IntrestHeaderView);
                        View.BringSubviewToFront(sidePanel);

                        View.AddSubview(InterestTableView);

                        View.AddSubview(IntrestHeaderView);

                        if (interestDate != null && interestDate.Count > 0)
                        {
                            IntrestHeaderView.Frame = new CGRect(sidePanel.Frame.Right, 0, View.Frame.Width - sidePanel.Frame.Width, intrestTableHeaderViewHeight);
                        }
                        else
                        {
                            IntrestHeaderView.Frame = new CGRect(InterestTableView.Frame.X + 4, 0, InterestTableView.Frame.Width - 8, headerHeight);
                        }
                        //						InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height - IntrestHeaderView.Frame.Height);
                        InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height);

                    }
                    else
                    {
                        if (interestDate.Count > 0)
                        {
                            View.BringSubviewToFront(InterestTableView);
                            View.BringSubviewToFront(IntrestHeaderView);
                            View.BringSubviewToFront(sidePanel);
                        }
                        else
                        {
                            View.BringSubviewToFront(InterestTableView);
                            setInterestHeaderView();
                            View.BringSubviewToFront(IntrestHeaderView);
                            View.BringSubviewToFront(sidePanel);
                        }
                    }
                }
                else
                {

                    getALLSchedule((res) =>
                    {
                        scheduleSource = res;
                        scheduleSource = res.OrderBy(u => u.Key).ToDictionary(u => convertToDate(u.Key), u => u.Value);
                        InvokeOnMainThread(() =>
                        {
                            (SchedulteTableView.Source as MyScheduleSourceFull).UpdateSource(scheduleSource);
                            SchedulteTableView.ReloadData();
                            this.indexPath = null;
                            sidePanelSelectedRow = 0;
                            View.BringSubviewToFront(SchedulteTableView);
                            SetScheduleHeaderView();
                            View.BringSubviewToFront(headerViewSchedule);
                            View.BringSubviewToFront(sidePanel);
                        });
                    });
                }
            };

            InterestTableView.TableFooterView = new UIView(new CGRect(InterestTableViewFooterXPadding, InterestTableViewFooterYPadding, InterestTableViewFooterWidthPadding, InterestTableViewFooterHeightPadding));
            SchedulteTableView.TableFooterView = new UIView(new CGRect(ScheduleTableViewFooterXPadding, ScheduleTableViewFooterYPadding, ScheduleTableViewFooterWidthPadding, ScheduleTableViewFooterHeightPadding));

            View.AddSubviews(sidePanel, empyVerticalView, InterestTableView, SchedulteTableView);
            AddObserver();
            InterestStarAddObserver();
            SetScheduleHeaderView();

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.my_schedule, Helper.ToDateString(DateTime.Now));
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);

            if (Panel == Diff.Interest)
            {
                if (scheduleInterestDataSource != null && scheduleInterestDataSource.selectedIndex != null)
                {
                    InterestTableView.DeselectRow(scheduleInterestDataSource.selectedIndex, false);
                }

                if (selectedIndex != null)
                {
                    bool tempbool = false;
                    var sessionTemp = ((SessionDataCell)InterestTableView.CellAt(selectedIndex)).model;
                    var result = DataManager.GetMyInterest(AppDelegate.Connection).Result;
                    if (result != null && result.Count > 0)
                    {
                        tempbool = DataManager.GetMyInterest(AppDelegate.Connection).Result.Any(p => p.session_time_id == sessionTemp.BuiltSession.session_id);
                    }

                    if (!tempbool)
                    {
                        List<string> keys = new List<string>();
                        int sectionIndex = selectedIndex.Section;

                        foreach (var key in interestSource.Keys)
                        {
                            keys.Add(key);
                        }
                        var lst = interestSource[keys[sectionIndex]].ToList();
                        lst.Remove(sessionTemp);

                        if (lst.Count > 0)
                        {
                            interestSource[keys[sectionIndex]] = lst.ToArray();
                            InterestTableView.DeleteRows(new[] { selectedIndex }, UITableViewRowAnimation.Fade);
                        }
                        else
                        {
                            interestDate.RemoveRange(0, interestDate.Count);
                            interestSource[keys[sectionIndex]] = lst.ToArray();
                            InterestTableView.DeleteRows(new[] { selectedIndex }, UITableViewRowAnimation.Fade);
                            interestSource.Remove(keys[sectionIndex]);
                            (InterestTableView.Source as ScheduleInterestDataSource).UpdateSource(interestSource);
                            InterestTableView.ReloadData();
                        }

                    }
                    selectedIndex = null;
                }
            }
            else
            {
                if (Panel == Diff.Schedule)
                {
                    if (myScheduleSourceFull != null && myScheduleSourceFull.selectedIndex != null)
                    {
                        SchedulteTableView.DeselectRow(myScheduleSourceFull.selectedIndex, false);

                    }

                    if (scheduleSelectedIndex != null)
                    {
                        var scheduleTemp = ((SessionDataCell)SchedulteTableView.CellAt(scheduleSelectedIndex)).model;

                        DataManager.GetMyScheduleTodayArray(AppDelegate.Connection).ContinueWith(t =>
                            {
                                bool isSchedulePresent = false;
                                var schedule = t.Result;
                                if (schedule != null)
                                {
                                    isSchedulePresent = schedule.Any(p => p.session_time_id == scheduleTemp.session_time_id);
                                    if (!isSchedulePresent)
                                    {
                                        List<string> keys = new List<string>();
                                        int sectionIndex = scheduleSelectedIndex.Section;

                                        foreach (var key in scheduleSource.Keys)
                                        {
                                            keys.Add(key);
                                        }
                                        var lst = scheduleSource[keys[sectionIndex]].ToList();
                                        lst.Remove(scheduleTemp);
                                        if (lst.Count > 0)
                                        {
                                            scheduleSource[keys[sectionIndex]] = lst.ToArray();
                                            InvokeOnMainThread(() =>
                                                {
                                                    SchedulteTableView.DeleteRows(new[] { scheduleSelectedIndex }, UITableViewRowAnimation.Fade);
                                                });



                                        }
                                        else
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                scheduleSource[keys[sectionIndex]] = lst.ToArray();
                                                SchedulteTableView.DeleteRows(new[] { scheduleSelectedIndex }, UITableViewRowAnimation.Fade);
                                                scheduleSource.Remove(keys[sectionIndex]);
                                                (SchedulteTableView.Source as MyScheduleSourceFull).UpdateSource(scheduleSource);
                                                SchedulteTableView.ReloadData();
                                            });

                                        }

                                    }
                                    scheduleSelectedIndex = null;
                                }
                            });
                    }
                }
            }

        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewWillLayoutSubviews()
        {
            sidePanel.Frame = new CGRect(sidePanelXPadding, sidePanelYPadding, sidePaneWidthPadding, View.Frame.Height);

            if (interestDate != null && interestDate.Count > 0)
            {
                IntrestHeaderView.Frame = new CGRect(sidePanel.Frame.Right, 0, View.Frame.Width - sidePanel.Frame.Width, intrestTableHeaderViewHeight);
            }
            else
            {
                IntrestHeaderView.Frame = new CGRect(InterestTableView.Frame.X + 4, 0, InterestTableView.Frame.Width - 8, headerHeight);
            }
            //			InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height - IntrestHeaderView.Frame.Height);
            InterestTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height);


            if (lstDate != null && lstDate.Count > 0)
            {
                headerViewSchedule.Frame = new CGRect(SchedulteTableView.Frame.X, 0, SchedulteTableView.Frame.Width, scheduleTableHeaderViewHeight);
            }
            else
            {
                headerViewSchedule.Frame = new CGRect(SchedulteTableView.Frame.X, 0, SchedulteTableView.Frame.Width, headerHeight);
            }
            SchedulteTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding + 5, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height);

            //			SchedulteTableView.Frame = new CGRect(sidePanel.Frame.Right, InterestTableViewYPadding+5, View.Frame.Width - sidePanel.Frame.Width, View.Frame.Height-headerViewSchedule.Frame.Size.Height);
        }

        void setInterestHeaderView()
        {
            IntrestHeaderView.Frame = new CGRect(0, 0, View.Frame.Width - sidePanel.Frame.Width, intrestTableHeaderViewHeight);
            List<UIView> views = new List<UIView>();
            foreach (var vw in IntrestHeaderView.Subviews)
            {
                vw.RemoveFromSuperview();
                vw.Dispose();
            }


            TitleHeaderView titleheaderView = new TitleHeaderView(InterestTitle, true, false, false, false, false, false, false, false);

            titleheaderView.Frame = new CGRect(headerViewLeftMargin + 4, 0, View.Frame.Width - (sidePanel.Frame.Width - (headerViewRightMargin + 4)), headerHeight);
            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
            IntrestHeaderView.AddSubview(titleheaderView);

            nfloat daysSegmentControlXPadding = 0;
            nfloat daysSegmentControlYPadding = 64;
            nfloat daysSegmentControlHeightPadding = 55;
            IntrestHeaderView.Frame = new CGRect(InterestTableView.Frame.X, 0, InterestTableView.Frame.Width, IntrestHeaderView.Frame.Height);

            if (interestDate != null && interestDate.Count > 0)
            {
                var dates = interestDate.Select(p => convertToDateInterest(p)).ToList();
                SegmentView daysSegmentInterestControl = new SegmentView(dates);
                daysSegmentInterestControl.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                daysSegmentInterestControl.Frame = new CGRect(daysSegmentControlXPadding, daysSegmentControlYPadding, IntrestHeaderView.Frame.Width, daysSegmentControlHeightPadding);
                daysSegmentInterestControl.BackgroundColor = AppTheme.SIsegmentbackColor;
                daysSegmentInterestControl.segmentViewViewDelegate += daysSegmentInterestControl_segmentViewViewDelegate;
                daysSegmentInterestControl.selectedSegmentIndex = 0;
                daysSegmentInterestControl.textColor = AppTheme.SIsegmentTitletextColor;
                daysSegmentInterestControl.subHeadingTextColor = AppTheme.SIsegmentSubTitletextColor;
                daysSegmentInterestControl.font = AppTheme.SIsegmentFont;
                daysSegmentInterestControl.selectedBoxColor = AppTheme.SIsegmentSelectedtabColor;
                daysSegmentInterestControl.selectedBoxColorOpacity = 1.0f;
                daysSegmentInterestControl.selectedBoxBorderWidth = 0.0f;
                daysSegmentInterestControl.selectionIndicatorHeight = 4.0f;
                daysSegmentInterestControl.selectedTextColor = AppTheme.SIsegmentTitletextColor;
                daysSegmentInterestControl.selectionIndicatorColor = AppTheme.SIsegmentSelectedtabBottomColor;
                daysSegmentInterestControl.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                daysSegmentInterestControl.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                daysSegmentInterestControl.multiLineSupport = true;
                daysSegmentInterestControl.multiLineFont = AppTheme.SImultiLineFont;
                daysSegmentInterestControl.separatorColor = AppTheme.SIsegmentSeparatorColor;
                daysSegmentInterestControl.Layer.CornerRadius = 0.0f;
                daysSegmentInterestControl.Layer.BorderColor = AppTheme.SIsegmentSeparatorColor.CGColor;
                daysSegmentInterestControl.Layer.BorderWidth = 1.0f;
                IntrestHeaderView.AddSubview(daysSegmentInterestControl);
            }
            else
            {
                var propertyinfo = IntrestHeaderView.Subviews;
                foreach (var item in propertyinfo)
                {
                    var i = item as SegmentView;
                    if (i != null)
                    {
                        i.RemoveFromSuperview();
                    }
                }
                IntrestHeaderView.Frame = new CGRect(InterestTableView.Frame.X + 4, 0, InterestTableView.Frame.Width - 8, headerHeight);
                titleheaderView.showBaseLine = true;
                titleheaderView.SetNeedsLayout();
            }

            IntrestHeaderView.BackgroundColor = titleheaderView.BackgroundColor;

            View.AddSubview(IntrestHeaderView);
            InterestTableView.TableHeaderView = new UIView(new CGRect(0, 0, InterestTableView.Frame.Width, IntrestHeaderView.Frame.Height));
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

        NSObject sessionAddedToSchedule;
        NSObject sessionRemovedFromSchedule;
        public void AddObserver()
        {
			sessionAddedToSchedule = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.SessionAddedToSchedule), (notification) =>
            {
                try
                {
                    var data = notification.UserInfo.ValueForKey(new NSString("session_time")).ToString();
                    var session_time = JsonConvert.DeserializeObject<BuiltSessionTime>(data);

                    if (!AppSettings.MySessionIds.Contains(session_time.session_time_id))
                    {
                        AppSettings.MySessionIds.Add(session_time.session_time_id);
                    }

                    //if (scheduleSelectedIndex != null)
                    //{
                    //    var scheduleTemp = ((SessionDataCell)SchedulteTableView.CellAt(scheduleSelectedIndex));
                    //}

                    if (sidePanelSelectedRow == 0)
                    {
                        foreach (var item in SchedulteTableView.VisibleCells)
                        {
                            if (item is SessionDataCell)
                            {
                                var session = ((SessionDataCell)item).model;

                                if (session.session_time_id != session_time.session_time_id)
                                    continue;

                                if (AppSettings.MySessionIds.Contains(session.session_time_id))
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (SchedulteTableView.Source as MyScheduleSourceFull).parentTracks, true);
                                else
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (SchedulteTableView.Source as MyScheduleSourceFull).parentTracks, true);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in InterestTableView.VisibleCells)
                        {
                            if (item is SessionDataCell)
                            {
                                var session = ((SessionDataCell)item).model;

                                if (session.session_time_id != session_time.session_time_id)
                                    continue;

                                if (AppSettings.MySessionIds.Contains(session.session_time_id))
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (InterestTableView.Source as ScheduleInterestDataSource).parentTracks, true);
                                else
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (InterestTableView.Source as ScheduleInterestDataSource).parentTracks, false);
                            }
                        }

                    }
                }
                catch { }
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
                    //if (scheduleSelectedIndex != null)
                    //{
                    //    var scheduleTemp = ((SessionDataCell)SchedulteTableView.CellAt(scheduleSelectedIndex));
                    //}

                    if (sidePanelSelectedRow == 0)
                    {
                        foreach (var item in SchedulteTableView.VisibleCells)
                        {
                            if (item is SessionDataCell)
                            {
                                var session = ((SessionDataCell)item).model;

                                if (AppSettings.MySessionIds.Contains(session.session_time_id))
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (SchedulteTableView.Source as MyScheduleSourceFull).parentTracks, true);
                                else
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (SchedulteTableView.Source as MyScheduleSourceFull).parentTracks, true);

                            }
                        }

                    }
                    else
                    {
                        foreach (var item in InterestTableView.VisibleCells)
                        {
                            if (item is SessionDataCell)
                            {
                                var session = ((SessionDataCell)item).model;

                                if (AppSettings.MySessionIds.Contains(session.session_time_id))
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (InterestTableView.Source as ScheduleInterestDataSource).parentTracks, true);
                                else
                                    ((SessionDataCell)item).UpdateCell(session, ((SessionDataCell)item).displayTime, (InterestTableView.Source as ScheduleInterestDataSource).parentTracks, false);
                            }
                        }
                    }
                }
                catch
                { }
            });
        }

        public void RemoveObserver()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(sessionAddedToSchedule);
            NSNotificationCenter.DefaultCenter.RemoveObserver(sessionRemovedFromSchedule);
        }

        NSObject starAddedToInterest;
        NSObject starRemovedFromInterest;
        public void InterestStarAddObserver()
        {
			starAddedToInterest = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.StarAddedToInterest), (notification) =>
                {
                    var data = notification.UserInfo.ValueForKey(new NSString("Star_Selected")).ToString();
                    InvokeOnMainThread(() =>
                        {
                            try
                            {
                                var interestCell = ((SessionDataCell)InterestTableView.CellAt(selectedIndex));
                            }
                            catch { }

                        });
                });

			starRemovedFromInterest = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.StarRemovedFromInterest), (notification) =>
            {
                var data = notification.UserInfo.ValueForKey(new NSString("Star_UnSelected")).ToString();
                InvokeOnMainThread(() =>
                {
                    try
                    {
                        var interestCell = ((SessionDataCell)InterestTableView.CellAt(selectedIndex));
                    }
                    catch { }

                });
            });
        }

        public void InterestStarRemoveObserver()
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(starAddedToInterest);
            NSNotificationCenter.DefaultCenter.RemoveObserver(starRemovedFromInterest);
        }

        void SetScheduleHeaderView()
        {
            headerViewSchedule.Frame = new CGRect(SchedulteTableView.Frame.X, 0, SchedulteTableView.Frame.Width, scheduleTableHeaderViewHeight);
            TitleHeaderView titleheaderView = new TitleHeaderView(string.Empty, true, false, false, false, false, false, false, false);
            headerViewSchedule.BackgroundColor = titleheaderView.BackgroundColor;
            titleheaderView.Frame = new CGRect(headerViewLeftMargin, 0, View.Frame.Width - (sidePanel.Frame.Width - headerViewRightMargin), headerHeight);
            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
            headerViewSchedule.AddSubview(titleheaderView);

            if (lstDate != null && lstDate.Count > 0)
            {
                var dates = lstDate.Select(p => convertToDateInterest(p)).ToList();
                SegmentView daysSegmentControlSchedule = new SegmentView(dates);
                daysSegmentControlSchedule.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                daysSegmentControlSchedule.Frame = new CGRect(0, 64, headerViewSchedule.Frame.Width, 55);
                daysSegmentControlSchedule.segmentViewViewDelegate += daysSegmentControlSchedule_segmentViewViewDelegate;
                daysSegmentControlSchedule.BackgroundColor = AppTheme.SIsegmentbackColor;
                daysSegmentControlSchedule.selectedSegmentIndex = 0;
                daysSegmentControlSchedule.textColor = AppTheme.SIsegmentTitletextColor;
                daysSegmentControlSchedule.subHeadingTextColor = AppTheme.SIsegmentSubTitletextColor;
                daysSegmentControlSchedule.font = AppTheme.SegmentFont;
                daysSegmentControlSchedule.selectedBoxColor = AppTheme.SIsegmentSelectedtabColor;
                daysSegmentControlSchedule.selectedBoxColorOpacity = 1.0f;
                daysSegmentControlSchedule.selectedBoxBorderWidth = 0.0f;
                daysSegmentControlSchedule.selectionIndicatorHeight = 4.0f;
                daysSegmentControlSchedule.selectedTextColor = AppTheme.SIsegmentTitletextColor;
                daysSegmentControlSchedule.selectionIndicatorColor = AppTheme.SIsegmentSelectedtabBottomColor;
                daysSegmentControlSchedule.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                daysSegmentControlSchedule.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                daysSegmentControlSchedule.multiLineSupport = true;
                daysSegmentControlSchedule.multiLineFont = AppTheme.SImultiLineFont;
                daysSegmentControlSchedule.separatorColor = AppTheme.SIsegmentSeparatorColor;
                daysSegmentControlSchedule.Layer.CornerRadius = 0.0f;
                daysSegmentControlSchedule.Layer.BorderColor = AppTheme.SIsegmentSeparatorColor.CGColor;
                daysSegmentControlSchedule.Layer.BorderWidth = 1.0f;

                headerViewSchedule.AddSubview(daysSegmentControlSchedule);
            }
            else
            {
                var propertyinfo = headerViewSchedule.Subviews;
                foreach (var item in propertyinfo)
                {
                    var i = item as SegmentView;
                    if (i != null)
                    {
                        i.RemoveFromSuperview();
                    }
                }
                titleheaderView.showBaseLine = true;
                titleheaderView.SetNeedsLayout();
                headerViewSchedule.Frame = new CGRect(SchedulteTableView.Frame.X, 0, SchedulteTableView.Frame.Width, headerHeight);
            }
            View.AddSubview(headerViewSchedule);
            if (sidePanel != null)
                View.BringSubviewToFront(sidePanel);
            SchedulteTableView.TableHeaderView = new UIView(new CGRect(0, 0, SchedulteTableView.Frame.Width, headerViewSchedule.Frame.Height));
        }

        void daysSegmentControlSchedule_segmentViewViewDelegate(int index)
        {
            try
            {
                var date = lstDate[index];
                if (date.ToLower().Contains(AppTheme.SIsingleDayText))
                {
                    CurrentSelectedDate = date;
                    filterScheduleUsingLinq(date);
                }
                else
                {
                    var dt = lstDate.Where(p => p == date).FirstOrDefault();
                    filterScheduleUsingLinq(date);
                }
            }
            catch { }
        }

        void daysSegmentInterestControl_segmentViewViewDelegate(int index)
        {
            try
            {
                var date = interestDate[index];
                if (date.ToLower().Contains(AppTheme.SIsingleDayText))
                {
                    CurrentSelectedDate = date;
                    filterInterestUsingLinq(date);
                }
                else
                {
                    var dt = interestDate.Where(p => p == date).FirstOrDefault();
                    filterInterestUsingLinq(date);
                }
            }
            catch { }

        }

        private void filterInterestUsingLinq(string dateTime)
        {
            DataManager.GetMyInterest(AppDelegate.Connection).ContinueWith(t =>
              {
                  var allInterest = t.Result;
                  CurrentSelectedDate = convertToBuiltDate(dateTime);
                  var ids = allInterest.Select(p => p.session_time_id).ToList();
                  DataManager.GetSessionTimeFromSessionIds(AppDelegate.Connection, ids).ContinueWith(s =>
                  {
                      var data = s.Result;
                      if (!string.IsNullOrEmpty(this.CurrentSelectedDate) && this.CurrentSelectedDate.ToLower().Contains(AppTheme.SIsingleDayText))
                      {
                          interestSource = data.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => convertToDateInterest(p.Key), p => p.ToArray().OrderBy(q => q.time).ToArray());
                          InvokeOnMainThread(() =>
                          {
                              (InterestTableView.Source as ScheduleInterestDataSource).UpdateSource(interestSource);
                              InterestTableView.ReloadData();
                          });
                          return;
                      }

                      if (!string.IsNullOrEmpty(this.CurrentSelectedDate))
                      {
                          var expectedResult = data.Where(q => q.date == CurrentSelectedDate);
                          interestSource = expectedResult.ToList().GroupBy(p => p.time).OrderBy(p => p.Key).ToDictionary(p => convertTimeFormat(p.Key), p => p.ToArray());
                          InvokeOnMainThread(() =>
                          {
                              (InterestTableView.Source as ScheduleInterestDataSource).UpdateSource(interestSource);
                              InterestTableView.ReloadData();
                          });
                          return;
                      }
                  });
              });
        }

        public string convertTimeFormat(string time)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(time);
                return Helper.ToDateTimeString(date, "hh:mm tt");
            }
            catch
            {
                return String.Empty;
            }
        }

        public string convertToBuiltDate(string p)
        {
            if (String.IsNullOrWhiteSpace(p))
                return String.Empty;

            if (p.ToLower().Contains("days"))
            {
                var allDate = p;
                return allDate;
            }

            try
            {
                var date = DateTime.Parse(p);
                return Helper.ToDateTimeString(date, "yyyy-MM-dd");
            }
            catch
            {
                return String.Empty;
            }
        }

        private void filterScheduleUsingLinq(string date)
        {
            DataManager.GetMyScheduleToday(AppDelegate.Connection).ContinueWith(res =>
           {
               var expectedResult = res.Result;
               List<BuiltSessionTime> lstBuiltSesssiontime = null;

               lstBuiltSesssiontime = expectedResult.Values.SelectMany(p => p).ToList();

               CurrentSelectedDate = convertToBuiltDate(date);
               if (!string.IsNullOrEmpty(this.CurrentSelectedDate) && this.CurrentSelectedDate.ToLower().Contains(AppTheme.SIsingleDayText))
               {
                   Dictionary<string, BuiltSessionTime[]> filteredList = new Dictionary<string, BuiltSessionTime[]>();
                   expectedResult = lstBuiltSesssiontime.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => convertToDate(p.Key), p => p.OrderBy(q => q.time).ToList());
                   scheduleSource = expectedResult.ToDictionary(p => p.Key, p => p.Value.ToArray());
                   InvokeOnMainThread(() =>
                   {
                       (SchedulteTableView.Source as MyScheduleSourceFull).UpdateSource(scheduleSource);
                       SchedulteTableView.ReloadData();
                   });
                   return;
               }
               if (!string.IsNullOrEmpty(this.CurrentSelectedDate))
               {
                   expectedResult = lstBuiltSesssiontime.Where(q => q.date == CurrentSelectedDate).OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => convertToDate(p.Key), p => p.OrderBy(q => q.time).ToList());
                   scheduleSource = expectedResult.ToDictionary(p => p.Key, p => p.Value.ToArray());
                   InvokeOnMainThread(() =>
                   {
                       (SchedulteTableView.Source as MyScheduleSourceFull).UpdateSource(scheduleSource);
                       SchedulteTableView.ReloadData();
                   });
               }

           });
        }

        private void getALLInterests(Action<Dictionary<string, BuiltSessionTime[]>> callback)
        {
            Func<string, string> convertToDate = (Date) =>
            {
                var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
                return date;
            };
            List<BuiltSessionTime> temp = new List<BuiltSessionTime>();
            Dictionary<string, BuiltSessionTime[]> filteredList = new Dictionary<string, BuiltSessionTime[]>();
            DataManager.GetMyInterest(AppDelegate.Connection).ContinueWith(t =>
                {
                    var allInterest = t.Result;
                    if (allInterest != null && allInterest.Count > 0)
                    {
                        var ids = allInterest.Select(p => p.session_time_id).ToList();
                        DataManager.GetSessionTimeFromSessionIds(AppDelegate.Connection, ids).ContinueWith(s =>
                        {
                            var data = s.Result;
                            var result = data.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => (p.Key), p => p.ToArray().OrderBy(q => q.time).ToArray());
                            if (interestDate == null) interestDate = new List<string>();
                            interestDate = result.Select(q => q.Key).Distinct().ToList();
                            if (interestDate.Count > 0)
                            {
                                interestDate.Insert(0, AppTheme.SIalldaysText);
                            }

                            if (callback != null)
                            {
                                callback(result);
                            }
                        });
                    }
                    else
                    {
                        if (callback != null)
                        {
                            callback(new Dictionary<string, BuiltSessionTime[]>());
                        }
                    }
                });
        }

        private void getALLSchedule(Action<Dictionary<string, BuiltSessionTime[]>> callback)
        {
            Func<string, string> convertToDate = (Date) =>
            {
                var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
                return date;
            };
            Dictionary<string, BuiltSessionTime[]> filteredList = new Dictionary<string, BuiltSessionTime[]>();
            DataManager.GetMyScheduleTodayArray(AppDelegate.Connection).ContinueWith(t =>
            {
                var allSessions = t.Result;
                if (allSessions != null && allSessions.Length > 0)
                {
                    var dictResult = allSessions.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToArray().OrderBy(q => q.time).ToArray());
                    foreach (var item in dictResult)
                    {
                        var firstitem = item.Value;
                        var newItem = firstitem.OrderBy(p => p.time).ToArray();
                        filteredList.Add(item.Key, newItem);
                    }

                    if (lstDate == null) lstDate = new List<string>();
                    lstDate = dictResult.Select(s => s.Key).Distinct().ToList();
                    lstDate.Insert(0, AppTheme.SIalldaysText);
                    if (callback != null)
                    {
                        callback(filteredList);

                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(new Dictionary<string, BuiltSessionTime[]>());
                    }
                }

            });
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
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
                        updateInterestTableSource();
                        return;
                    }
                }
            }
            else
            {
                if (!shouldUpdate)
                    return;
                if (updatedUids != null && updatedUids.Contains(ApiCalls.session))
                {
                    updateTableSource();
                }
            }
        }

        public override void OnAfterLoginDataFetched(NSNotification notification)
        {
            updateTableSource();
        }

        private void updateInterestTableSource()
        {
            var p = AppSettings.AllTracks;
            getALLInterests(res =>
            {
                interestSource = res;
                InvokeOnMainThread(() =>
                {
                    scheduleInterestDataSource = new ScheduleInterestDataSource(this, interestSource, p);
                    InterestTableView.Source = scheduleInterestDataSource;
                    setInterestHeaderView();
                    InterestTableView.ReloadData();
                });
            });
        }

        private void updateTableSource()
        {
            shouldUpdate = false;
            var tracks = AppSettings.AllTracks;
            getALLSchedule((res) =>
            {
                scheduleSource = res;
                InvokeOnMainThread(() =>
                {
                    myScheduleSourceFull = new MyScheduleSourceFull(this, scheduleSource, tracks);
                    SchedulteTableView.Source = myScheduleSourceFull;
                    SetScheduleHeaderView();
                    SchedulteTableView.ReloadData();
                    shouldUpdate = true;
                    sidePanelSelectedRow = 0;
                });
            });
        }

        public string convertToDate(string Date)
        {
            if (String.IsNullOrWhiteSpace(Date))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(Date).Date;
                if (date.Day == DateTime.Now.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Today";
                    return Date;
                }
                else if (date.Day == DateTime.Now.Day + 1 && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Tomorrow";
                    return Date;
                }
                else
                {
                    return Helper.ToDateTimeString(date, "ddd, MMM dd");
                }
            }
            catch
            {
                return String.Empty;
            }
        }
    }


    public class ScheduleInterestDataSource : UITableViewSource
    {
        ScheduleAndInterestController scheduleAndInterestController;
        Dictionary<string, BuiltSessionTime[]> indexedTableItems;
        NSString cellIdentifier = new NSString("TableCell");
        string[] keys;
        public List<BuiltTracks> parentTracks;
        Dictionary<string, List<string>> lstTime = new Dictionary<string, List<string>>();
        Dictionary<string, Int32> lstTimeIndexPosition = new Dictionary<string, Int32>();
        bool displayTime = false;
		static nfloat defaultCellHeight = 75.0f;//63.0f;
		static nfloat defaultCellSpaceExceptSessionName = 280.0f;
        public NSIndexPath selectedIndex;
        NSString emptyCellIdentifier = new NSString("EmptyTableCell");

        public ScheduleInterestDataSource(ScheduleAndInterestController scheduleAndInterestController, Dictionary<string, BuiltSessionTime[]> items, List<BuiltTracks> sessionTracks)
        {
            this.scheduleAndInterestController = scheduleAndInterestController;
            indexedTableItems = items;
            keys = items.Keys.ToArray();
            parentTracks = sessionTracks;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
            {
                return 1;
            }
            else
            {
                return keys.Length;
            }
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
            {
                return 1;
            }
            else
            {
                return indexedTableItems[keys[section]].Length;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat viewXPadding = 0;
            nfloat viewYPadding = 0;

            UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, 40))
            {
                BackgroundColor = AppTheme.SIinterestHeaderViewbackColor,
            };

            nfloat lblSectionHeaderXPadding = 15;
            nfloat lblSectionHeaderYPadding = 0;
            nfloat lblSectionHeaderWidthPadding = 30;
            nfloat lblSectionHeaderHeightPadding = 30;

            UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, lblSectionHeaderYPadding, tableView.Frame.Width - lblSectionHeaderWidthPadding, view.Frame.Height))
            {
                BackgroundColor = AppTheme.SIinterestHeaderLabelbackColor,
                TextColor = AppTheme.SIinterestHeaderLabelTextColor,
                Font = AppTheme.SIintrerestHeaderLabelFont
            };
            string headerTitle = keys[section].Replace(",", "").ToUpper();
            lblSectionHeader.Text = headerTitle;

            nfloat separatorXPadding = 0;
            nfloat separatorYPadding = 1;
            nfloat separatorHeightPadding = 1;
            var separator = new UIView(new CGRect(separatorXPadding, view.Frame.Bottom - separatorYPadding, view.Frame.Width, separatorHeightPadding));
            separator.BackgroundColor = AppTheme.SImenuSectionSeparator;

            view.AddSubviews(lblSectionHeader, separator);
            return view;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
                return 0;

            return 40;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
            {
                NoSessionsCell emptyCell = null;
                emptyCell = tableView.DequeueReusableCell(emptyCellIdentifier) as NoSessionsCell;
                if (emptyCell == null) emptyCell = new NoSessionsCell(emptyCellIdentifier);
                emptyCell.UpdateCell(AppTheme.noInterestCell);
                emptyCell.nameLabel.Text = AppTheme.noInterestCell;
                return emptyCell;
            }

            SessionDataCell cell = tableView.DequeueReusableCell(cellIdentifier) as SessionDataCell;
            if (cell == null) cell = new SessionDataCell(cellIdentifier);
            var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
            string time = DateTime.Parse(item.time).ToString("hh tt");

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
                cell.btnAddRemove.Selected = !cell.btnAddRemove.Selected;
                cell.btnAddRemove.Enabled = false;

                if (cell.btnAddRemove.Selected)
                {
                    DataManager.AddSessionToSchedule(AppDelegate.Connection, item, (session_time) =>
                                        {
                                            var setting = new JsonSerializerSettings
                                                             {
                                                                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                             };

                                            InvokeOnMainThread(() =>
                                            {
                                                cell.btnAddRemove.Enabled = true;
                                                ToastView.ShowToast(Helper.SessionAddedString, 2);
                                                NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(session_time, setting)), new NSString("session_time"));
                                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                                            });
                                        }, error =>
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                cell.btnAddRemove.Enabled = true;
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
                        cell.btnAddRemove.Enabled = true;
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
                                            cell.btnAddRemove.Enabled = true;
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
                                cell.btnAddRemove.Enabled = true;
                            }
                        };
                        alert.Show();
                    }
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
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
            {
                return 75;
            }
            else
            {
                return defaultCellHeight;
            }
        }

        public void UpdateSource(Dictionary<string, BuiltSessionTime[]> indexedTableItems)
        {
            this.indexedTableItems = indexedTableItems;
            this.keys = indexedTableItems.Keys.ToArray();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexedTableItems == null || (indexedTableItems != null && indexedTableItems.Count == 0))
                return;
            try
            {
                nfloat vcXPadding = 0;
                nfloat vcYPadding = 0;
                nfloat vcWidthPadding = 477;
                selectedIndex = indexPath;
                scheduleAndInterestController.Panel = ScheduleAndInterestController.Diff.Interest;
                scheduleAndInterestController.selectedIndex = indexPath;
                SessionDetailController vc = new SessionDetailController(indexedTableItems[keys[indexPath.Section]][indexPath.Row]);
                vc.View.Frame = new CGRect(vcXPadding, vcYPadding, vcWidthPadding, scheduleAndInterestController.View.Frame.Size.Height);
                AppDelegate.instance().rootViewController.openDetail(vc, scheduleAndInterestController, true);
            }
            catch { }
        }
    }



    public class MyScheduleSourceFull : UITableViewSource
    {
        ScheduleAndInterestController scheduleAndInterestController;
        bool displayTime = false;
        Dictionary<string, BuiltSessionTime[]> indexedTableItems;
        string[] keys;
		static nfloat defaultCellHeight = 75.0f;//63.0f;
		static nfloat defaultCellSpaceExceptSessionName = 280.0f;
        public NSIndexPath selectedIndex;

        NSString cellIdentifier = new NSString("TableCell");
        NSString emptyCellIdentifier = new NSString("EmptyTableCell");
        Dictionary<string, List<string>> lstTime = new Dictionary<string, List<string>>();
        Dictionary<string, Int32> lstTimeIndexPosition = new Dictionary<string, Int32>();
        public List<BuiltTracks> parentTracks;
        public MyScheduleSourceFull(ScheduleAndInterestController scheduleAndInterestController, Dictionary<string, BuiltSessionTime[]> items, List<BuiltTracks> sessionTracks)
        {
            this.scheduleAndInterestController = scheduleAndInterestController;
            indexedTableItems = items;
            keys = items.Keys.ToArray();
            parentTracks = sessionTracks;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (indexedTableItems == null || indexedTableItems.Count == 0)
            {
                return 1;
            }
            return keys.Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (indexedTableItems == null || indexedTableItems.Count == 0)
                return 1;

            return indexedTableItems[keys[section]].Length;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            if (keys.Length == 0)
            {
                return new UIView();
            }
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 40))
            {
                BackgroundColor = AppTheme.SIscheduleHeaderViewbackColor,
            };

            UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, tableView.Frame.Width - 30, view.Frame.Height))
            {
                BackgroundColor = AppTheme.SIscheduleHeaderLabelbackColor,
                TextColor = AppTheme.SIscheduleHeaderLabelTextColor,
                Font = AppTheme.SIscheduleHeaderLabelFont
            };

            string headerTitle = keys[section].Replace(",", "").ToUpper();
            lblSectionHeader.Text = headerTitle;

            var separator = new UIView(new CGRect(0, view.Frame.Bottom - 1, view.Frame.Width, 1));
            separator.BackgroundColor = AppTheme.SImenuSectionSeparator;

            view.AddSubviews(lblSectionHeader, separator);
            return view;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (indexedTableItems == null || indexedTableItems.Count == 0)
            {
                return 0;
            }
            if (String.IsNullOrEmpty(keys[section]))
                return 0;

            //			Console.WriteLine ("tableView width: "+ tableView.Frame.Width);
            return 40;
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexedTableItems == null || indexedTableItems.Count == 0)
            {
                NoSessionsCell emptyCell = null;
                emptyCell = tableView.DequeueReusableCell(emptyCellIdentifier) as NoSessionsCell;
                if (emptyCell == null) emptyCell = new NoSessionsCell(emptyCellIdentifier);
                emptyCell.UpdateCell(AppTheme.SInoScheduleCellText);
                emptyCell.nameLabel.Text = AppTheme.SInoScheduleCellText;
                return emptyCell;
            }

            SessionDataCell cell = tableView.DequeueReusableCell(cellIdentifier) as SessionDataCell;
            if (cell == null) cell = new SessionDataCell(cellIdentifier);
            var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
            string time = DateTime.Parse(item.time).ToString("hh tt");

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

            cell.btnAddRemove.TouchUpInside += (s, e) =>
            {
                if (item.BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                        item.BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                {
                    ToastView.ShowToast(Helper.CanntotUnschedule, 2);
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
                                        if (AppSettings.MySessionIds.Contains(session_time.session_time_id))
                                        {
                                            AppSettings.MySessionIds.Remove(session_time.session_time_id);
                                        } 

                                        try
                                        {
                                            var lst = indexedTableItems[keys[indexPath.Section]].ToList();
                                            var removeItem = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
                                            lst.Remove(removeItem);

                                            if (lst.Count > 0)
                                            {
                                                indexedTableItems[keys[indexPath.Section]] = lst.ToArray();
                                                scheduleAndInterestController.SchedulteTableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
                                            }
                                            else
                                            {
                                                indexedTableItems[keys[indexPath.Section]] = lst.ToArray();
                                                scheduleAndInterestController.SchedulteTableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
                                                indexedTableItems.Remove(keys[indexPath.Section]);
                                                (scheduleAndInterestController.SchedulteTableView.Source as MyScheduleSourceFull).UpdateSource(indexedTableItems);
                                                scheduleAndInterestController.SchedulteTableView.ReloadData();
                                            }
                                        }
                                        catch { }
                                    });
                                }
                            });
                        }
                    };
                    alert.Show();
                }
            };


            if (lstTimeIndexPosition[keys[indexPath.Section] + time] != null)
            {
                var indexPathValue = lstTimeIndexPosition[keys[indexPath.Section] + time];
                if (indexPathValue == indexPath.Row)
                {
                    displayTime = true;
                }
            }



            var menuText = item.date.Split('|');
            if (menuText.Length > 1)
                cell.UpdateCell(item, displayTime, parentTracks, true);
            else
                cell.UpdateCell(item, displayTime, parentTracks, true);

            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            //			Console.WriteLine ("tableView width:" + tableView.Frame.Width);

            if (indexedTableItems == null || indexedTableItems.Count == 0)
            {
                return 75;
            }
            else
            {
                //				var item = indexedTableItems [keys [indexPath.Section]] [indexPath.Row];
                //				NSString str = (NSString)item.BuiltSession.title;
                //				SizeF size = str.StringSize (AppFonts.ProximaNovaRegular(18), new SizeF (tableView.Frame.Width - defaultCellSpaceExceptSessionName, 999), UILineBreakMode.WordWrap);
                //				size.Height = size.Height + 40;
                //				if (size.Height < defaultCellHeight) {
                return defaultCellHeight;
                //				}
                //				return size.Height + 40;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexedTableItems == null || indexedTableItems.Count == 0)
            {
            }
            else
            {
                try
                {
                    nfloat vcXPadding = 0;
                    nfloat vcYPadding = 0;
                    nfloat vcWidthPadding = 477;
                    selectedIndex = indexPath;

                    scheduleAndInterestController.scheduleSelectedIndex = indexPath;
                    scheduleAndInterestController.Panel = ScheduleAndInterestController.Diff.Schedule;
                    SessionDetailController vc = new SessionDetailController(indexedTableItems[keys[indexPath.Section]][indexPath.Row]);
                    vc.View.Frame = new CGRect(vcXPadding, vcYPadding, vcWidthPadding, scheduleAndInterestController.View.Frame.Size.Height);
                    AppDelegate.instance().rootViewController.openDetail(vc, scheduleAndInterestController, true);
                }
                catch { }
            }
        }

        public void UpdateSource(Dictionary<string, BuiltSessionTime[]> indexedTableItems)
        {
            this.indexedTableItems = indexedTableItems;
            this.keys = indexedTableItems.Keys.ToArray();
        }
    }
}
