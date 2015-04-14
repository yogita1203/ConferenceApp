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
using CoreAnimation;
using CoreGraphics;


namespace ConferenceAppiOS
{
    public class AgendaProgramsHandsOnLabs : BaseViewController
    {
        #region--Views--
        LoadingOverlay loadingOverlay;
        UITableView agendaTableView;
        public WebViewController webView;
              
        CGRect rectBase;
        #endregion

        #region-- Constants
        string AgendaTitle = AppTheme.AGagendaTitle;       
		static nfloat agendaTableHeaderViewHeight = 120;
		static nfloat headerViewLeftMargin = 0;
		static nfloat headerViewRightMargin = 4;
		static nfloat headerHeight = 64;
		static nfloat daysSegmentControlYPosition = 64;
		static nfloat daysSegmentControlHeight = 55;
		static nfloat sidePanelXPadding = 0;
		static nfloat sidePanelYPadding = 0;
		static nfloat sidePaneWidthPadding = 88;
		static nfloat sidePanelFooterXPadding = 0;
		static nfloat sidePanelFooterYPadding = 0;
		static nfloat sidePanelFooterWidthPadding = 0;
		static nfloat sidePanelFooterHeightPadding = 1;
		static nfloat empyVerticalViewYPadding = 0;
		static nfloat empyVerticalViewWidthPadding = 3;
		static nfloat agendaTableViewYPadding = 0;
		static nfloat agendaTableViewFooterXPadding = 0;
		static nfloat agendaTableViewFooterYPadding = 0;
		static nfloat agendaTableViewFooterWidthPadding = 0;
		static nfloat agendaTableViewFooterHeightPadding = 1;
		static nfloat HOLTableViewYPadding = 0;
		static nfloat HOLTableViewFooterXPadding = 0;
		static nfloat HOLTableViewFooterYPadding = 0;
		static nfloat HOLTableViewFooterWidthPadding = 0;
		static nfloat HOLTableViewFooterHeightPadding = 1;
		static nfloat programTableViewWidth = 400;
		static nfloat programTableViewWidthLeftOpened = 335;
		static nfloat programTableViewYPadding = 0;
		static nfloat programTableViewFooterXPadding = 0;
		static nfloat programTableViewFooterYPadding = 0;
		static nfloat programTableViewFooterWidthPadding = 0;
		static nfloat programTableViewFooterHeightPadding = 1;
		static nfloat programTableVwRightBorderWidth = 4;
		static nfloat programTableTopBorderWidth = 3;
		static nfloat topheaderProgrmDescHeight = 44;
		static nfloat handsOnlabsTableViewYPadding = 0;
		static nfloat sepLeftInset = 10;
		static nfloat progrmLeftInset = 20;
		static nfloat handsOnLabsTableHeaderViewHeight = 65;
		static nfloat programTableHeaderViewHeight = 69;
		static nfloat holTitleBorderHeight = 3;
		static nfloat sidePanelTotalRowHeightFromAllCell = 300;
        nfloat myWidth = 0;
		static nfloat imgRightPadding = 50;
		static nfloat lblLeftPadding = 10;
		static nfloat imgLeftPadding = 25;
		static nfloat gapMargin = 20;
		static nfloat lblprogramHeaderYPadding = 10;
		static nfloat lblprogramHeaderRightPadding = 50;
		static nfloat AgendaTableViewYPadding = 0;
        #endregion

        Dictionary<string, BuiltAgendaItem[]> agendaListDict = new Dictionary<string, BuiltAgendaItem[]>();
         
        List<string> lstAgendaDate = new List<string>();
        List<DateTime> AgendaDates = new List<DateTime>(); 
        public List<BuiltHandsonLabs> lsthandsOnLabsBase;
        public List<BuiltOthers> lstPrograms; SegmentView daysSegmentControl;
        public string AgendaSearchText { get; set; }
        public string CurrentSelectedAgendaDate { get; set; }
		UIView headerView;

        public AgendaProgramsHandsOnLabs(CGRect rect)
        {
            rectBase = rect;
            View.Frame = rectBase;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = rectBase;
            myWidth = View.Frame.Width;

            agendaTableView = new UITableView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            ShowOverlay(agendaTableView);

            getAgenda((res) =>
            {
                agendaListDict = res;
                InvokeOnMainThread(() =>
                {
                    agendaTableView.Source = new AgendaDataSource(this, agendaListDict);
                    setAgendaTableHeaderView();
                    agendaTableView.ReloadData();
                    loadingOverlay.Hide();
                });
            });

            agendaTableView.TableFooterView = new UIView(new CGRect(agendaTableViewFooterXPadding, agendaTableViewFooterYPadding, agendaTableViewFooterWidthPadding, agendaTableViewFooterHeightPadding));
           
            View.AddSubviews(agendaTableView);
            setAgendaTableHeaderView();

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.agenda, Helper.ToDateString(DateTime.Now));
        }

        void ShowOverlay(UIView view)
        {
            var bounds = agendaTableView.Bounds;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
            }
            this.loadingOverlay = new LoadingOverlay(bounds);
            view.Add(this.loadingOverlay);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

			headerView = new UIView(new CGRect(0, 0, View.Frame.Width, agendaTableHeaderViewHeight));

            agendaTableView.Frame = new CGRect(0, AgendaTableViewYPadding, View.Frame.Width, View.Frame.Height);
            if (agendaTableView != null && agendaTableView.Source != null)
            {
                setAgendaTableHeaderView();
            }
        }
        void setAgendaTableHeaderView()
        {
            headerView = new UIView(new CGRect(0, 0, View.Frame.Width, agendaTableHeaderViewHeight));

            TitleHeaderView titleheaderView = new TitleHeaderView(AgendaTitle, true, false, false, false, false, false, false, false);
            headerView.BackgroundColor = titleheaderView.BackgroundColor;

            titleheaderView.searchFieldClicked = (textField) =>
            {
                AgendaSearchText = textField.Text;
                if (CurrentSelectedAgendaDate == null)
                {
                    CurrentSelectedAgendaDate = AppTheme.AGalldaysText;
                }
                filterAgenda(CurrentSelectedAgendaDate, textField.Text);
            };

            titleheaderView.searchButtonClicked = (textField) =>
            {
                AgendaSearchText = textField.Text;
                if (CurrentSelectedAgendaDate == null)
                {
                    CurrentSelectedAgendaDate = AppTheme.AGalldaysText;
                }
                filterAgenda(CurrentSelectedAgendaDate, textField.Text);
            };


            titleheaderView.RefreshButtonClicked = () =>
            {

            };

            titleheaderView.Frame = new CGRect(headerViewLeftMargin, 0, View.Frame.Width - headerViewRightMargin, headerHeight);
            headerView.AddSubview(titleheaderView);

            if (lstAgendaDate != null && lstAgendaDate.Count > 0)
            {
                var dates = lstAgendaDate.Select(p =>  getDateInAgendaFormat(p)).ToList();
                daysSegmentControl = new SegmentView(dates);
                daysSegmentControl.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                daysSegmentControl.Frame = new CGRect(-4, daysSegmentControlYPosition, headerView.Frame.Width + 8, daysSegmentControlHeight);
                daysSegmentControl.BackgroundColor = AppTheme.AGsegmentbackColor;
                daysSegmentControl.segmentViewViewDelegate += daysSegmentControl_segmentViewViewDelegate;
                daysSegmentControl.selectedSegmentIndex = 0;
                daysSegmentControl.textColor = AppTheme.AGsegmentTitletextColor;
				daysSegmentControl.subHeadingTextColor = AppTheme.AGsegmentSubTitletextColor;
                daysSegmentControl.font = AppTheme.AGsegmentFont;
                daysSegmentControl.selectedBoxColor = AppTheme.AGsegmentSelectedtabColor;
                daysSegmentControl.selectedBoxColorOpacity = 1.0f;
                daysSegmentControl.selectedBoxBorderWidth = 0.0f;
                daysSegmentControl.selectionIndicatorHeight = 4.0f;
                daysSegmentControl.selectedTextColor = AppTheme.AGsegmentTitletextColor;
                daysSegmentControl.selectionIndicatorColor = AppTheme.AGsegmentSelectedtabBottomColor;
                daysSegmentControl.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                daysSegmentControl.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                daysSegmentControl.multiLineSupport = true;
                daysSegmentControl.multiLineFont = AppTheme.AGmultiLineFont;
                daysSegmentControl.separatorColor = AppTheme.AGsegmentSeparatorColor;
				daysSegmentControl.Layer.CornerRadius = 0.0f;
				daysSegmentControl.Layer.BorderColor = AppTheme.SIsegmentSeparatorColor.CGColor;
				daysSegmentControl.Layer.BorderWidth = 1.0f;
                headerView.AddSubviews(daysSegmentControl);
            }
            View.AddSubview(headerView);

            agendaTableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width, agendaTableHeaderViewHeight));
        }
        void daysSegmentControl_segmentViewViewDelegate(int index)
        {
            var date = lstAgendaDate[index];
            if (date.ToLower().Contains(AppTheme.AGsingleDayText))
            {
                CurrentSelectedAgendaDate = date;
                filterAgenda(date);
            }
            else
            {
                var dt = lstAgendaDate.Where(p => p == date).FirstOrDefault();
                filterAgenda(dt);
            }
        }

        public static string getDateInAgendaFormat(string p)
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
                return convertToDate(p);
            }
            catch
            {
                return String.Empty;
            }
        }

        private void filterAgenda(string dateTime, string searchData = null)
        {
            DataManager.GetAllAgendaItem(AppDelegate.Connection).ContinueWith(res =>
            {
                var expectedResult = res.Result.AsEnumerable();
                CurrentSelectedAgendaDate = convertToBuiltDate(dateTime);


                if (!string.IsNullOrEmpty(this.CurrentSelectedAgendaDate) && this.CurrentSelectedAgendaDate.ToLower().Contains(AppTheme.AGsingleDayText))
                {
                    if (!string.IsNullOrEmpty(this.AgendaSearchText))
                    {
                        expectedResult = expectedResult.Where(t => t.name.ToLower().Contains(AgendaSearchText.ToLower()) || t.agenda_item_id.ToLower() == AgendaSearchText.ToLower() || t.location.ToLower().Contains(AgendaSearchText.ToLower()));
                    }
                    agendaListDict = expectedResult.GroupBy(p => Convert.ToDateTime(p.BuiltAgenda.agenda_date).Date).OrderBy(p => p.Key).ToDictionary(p => p.Key.ToString("ddd, MMM dd"), p => p.ToArray());

                    InvokeOnMainThread(() =>
                    {
                        (agendaTableView.Source as AgendaDataSource).UpdateSource(agendaListDict);
                        agendaTableView.ReloadData();
                    });
                    return;
                }
                if (!string.IsNullOrEmpty(this.CurrentSelectedAgendaDate))
                {
                    expectedResult = expectedResult.Where(q => convertToBuiltDate(q.BuiltAgenda.agenda_date) == CurrentSelectedAgendaDate);
                }
                if (!string.IsNullOrEmpty(this.AgendaSearchText))
                {
                    expectedResult = expectedResult.Where(t => t.name.ToLower().Contains(AgendaSearchText.ToLower()) || t.agenda_item_id.ToLower() == AgendaSearchText.ToLower() || t.location.ToLower().Contains(AgendaSearchText.ToLower()));
                }
                agendaListDict = expectedResult.ToList().GroupBy(p => p.start_time).OrderBy(p => hoursFormat(p.Key)).ToDictionary(p => convertTimeFormat(p.Key), p => p.ToArray());
                InvokeOnMainThread(() =>
                {
                    (agendaTableView.Source as AgendaDataSource).UpdateSource(agendaListDict);
                    agendaTableView.ReloadData();
                });
            });
            return;
        }

        public string hoursFormat(string time)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(time);
                return Helper.ToDateTimeString(date, "HH:mm tt");
            }
            catch
            {
                return String.Empty;
            }
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

        private static string convertToDate(string Date)
        {
            var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
            return date;
        }
        private void getAgenda(Action<Dictionary<string, BuiltAgendaItem[]>> callback)
        {
            Dictionary<string, BuiltAgendaItem[]> filteredAgendaList = new Dictionary<string, BuiltAgendaItem[]>();
            DataManager.GetAllAgendaItem(AppDelegate.Connection).ContinueWith(t =>
            {
                var res = t.Result;

                var allAgenda = res.GroupBy(p => Convert.ToDateTime(p.BuiltAgenda.agenda_date).Date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.OrderBy(q => q.agenda_item_id).ToArray());

                foreach (var item in allAgenda)
                {
                    var firstitem = item.Value;
                    //var newItem = firstitem.OrderBy(p => p.start_time).ToArray();
                    filteredAgendaList.Add(item.Key.ToString("ddd, MMM dd"), firstitem);
                }

                if (lstAgendaDate == null) lstAgendaDate = new List<string>();
                lstAgendaDate = allAgenda.Select(s => s.Key.ToString("yyyy-MM-dd")).Distinct().ToList();
                lstAgendaDate.Insert(0, AppTheme.AGalldaysText);
                if (callback != null)
                {
                    callback(filteredAgendaList);
                }
            });
        }

        void updateSource()
        {
            Dictionary<string, BuiltAgendaItem[]> filteredAgendaList = new Dictionary<string, BuiltAgendaItem[]>();
            DataManager.GetAllAgendaItem(AppDelegate.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                var allAgenda = res.OrderBy(p => p.start_time).GroupBy(p =>Convert.ToDateTime(p.BuiltAgenda.agenda_date).Date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToArray().OrderBy(q => q.agenda_item_id).ToArray());

                foreach (var item in allAgenda)
                {
                    var firstitem = item.Value;
                    var newItem = firstitem.OrderBy(p => p.agenda_item_id).ToArray();
                    filteredAgendaList.Add(item.Key.ToString("ddd, MMM dd"), newItem);
                }

                if (lstAgendaDate == null) lstAgendaDate = new List<string>();
                lstAgendaDate = allAgenda.Select(s => s.Key.ToString("yyyy-MM-dd")).Distinct().ToList();
                lstAgendaDate.Insert(0, AppTheme.AGalldaysText);

                agendaListDict = filteredAgendaList;
                InvokeOnMainThread(() =>
                {
                    if (lstAgendaDate != null && lstAgendaDate.Count > 0)
                    {
                    }
                    (agendaTableView.Source as AgendaDataSource).UpdateSource(agendaListDict);
                    agendaTableView.ReloadData();
                });
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.agenda))
            {
                updateSource();
            }
        }
    }

    #region--AgendaDataSource
    public class AgendaDataSource : UITableViewSource
    {
        AgendaProgramsHandsOnLabs AgendaProgramsHandsOnLabsViewController;
        bool displayTime = false;
        Dictionary<string, BuiltAgendaItem[]> indexedTableItems;
        string[] keys;
		static nfloat defaultCellHeight = 63.0f;
		static nfloat defaultCellSpaceExceptSessionName = 410.0f;

        NSString cellIdentifier = new NSString("TableCell");
        Dictionary<string, List<string>> lstTime = new Dictionary<string, List<string>>();
        Dictionary<string, Int32> lstTimeIndexPosition = new Dictionary<string, Int32>();

        public AgendaDataSource(AgendaProgramsHandsOnLabs AgendaProgramsHandsOnLabsViewController, Dictionary<string, BuiltAgendaItem[]> dictitems)
        {
            this.AgendaProgramsHandsOnLabsViewController = AgendaProgramsHandsOnLabsViewController;
            indexedTableItems = dictitems;
            keys = dictitems.Keys.ToArray();

        }
        public void UpdateSource(Dictionary<string, BuiltAgendaItem[]> indexedTableItems)
        {
            this.indexedTableItems = indexedTableItems;
            this.keys = indexedTableItems.Keys.ToArray();
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
            return indexedTableItems[keys[section]].Length;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            nfloat viewXPadding = 0;
			nfloat viewYPadding = 0;
			nfloat viewHeightPadding = 40;

			UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, viewHeightPadding))
            {
                BackgroundColor = AppTheme.AGheaderViewBackcolor,
            };

			nfloat lblSectionHeaderXPadding = 15;
			nfloat lblSectionHeaderYPadding = 0;
			nfloat lblSectionHeaderWidthPadding = 30;

			UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, lblSectionHeaderYPadding, tableView.Frame.Width - lblSectionHeaderWidthPadding, view.Frame.Height))
            {
                BackgroundColor = AppTheme.AGheaderLabelBackcolor,
                TextColor = AppTheme.AGheaderTextColor,
				Text = keys[section].ToUpper().Replace(",", String.Empty),
                Font = AppTheme.AGheaderLabelFont
            };

			nfloat separatorXPadding = 30;
			nfloat separatorYPadding = 1;
			nfloat separatorHeightPadding = 1;
            var separator = new UIView(new CGRect(separatorXPadding, view.Frame.Bottom - separatorYPadding, view.Frame.Width - 30, separatorHeightPadding));
            separator.BackgroundColor = AppTheme.AGmenuSectionSeparator;
            separator.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleMargins;

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
            AgendaCell cell = tableView.DequeueReusableCell(cellIdentifier) as AgendaCell;
            if (cell == null) cell = new AgendaCell(cellIdentifier);
            var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
            string time = String.Empty;
            
            if (!String.IsNullOrWhiteSpace(item.start_time))
                time = DateTime.Parse(item.start_time).ToString("hh tt");

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

            cell.UpdateCell(item, displayTime);

            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = indexedTableItems[keys[indexPath.Section]][indexPath.Row];
            NSString str = (NSString)item.name;

			nfloat height = 0;
			CGSize namesize = str.StringSize(AppFonts.ProximaNovaRegular(16), new CGSize(tableView.Frame.Width - defaultCellSpaceExceptSessionName, 999), UILineBreakMode.WordWrap);
			height += namesize.Height;
			NSString locationstr = (NSString)item.location;

			CGSize locationsize = new CGSize(0,0);
			if(locationstr.Length > 0)
				locationsize = locationstr.StringSize(AppFonts.ProximaNovaRegular(16), new CGSize(tableView.Frame.Width - defaultCellSpaceExceptSessionName, 999), UILineBreakMode.WordWrap);

			height += locationsize.Height;

			if (height < defaultCellHeight)
            {
                return defaultCellHeight;
            }

			return height + 20;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            AgendaCell cell = (AgendaCell)tableView.CellAt(indexPath);
            cell.Selected = false;
       }

    }
    #endregion
}