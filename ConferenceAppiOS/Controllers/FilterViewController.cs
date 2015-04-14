using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer;
using SQLiteNetExtensions.Extensions;
using CommonLayer.Entities.Built;
using System.Threading;

namespace ConferenceAppiOS
{
    public class FilterViewController : BaseViewController
    {
        UISwitch btnToggle;
        public bool showFilterMark = false;
        public UITableView filterTableview;
        public string preSelectedTrackName = string.Empty;
		static nfloat filterTableviewXPadding = 0;
		static nfloat filterTableviewYPadding = 0;

		static nfloat headerViewXPadding = 0;
		static nfloat headerViewYPadding = 0;
		static nfloat headerViewHeightPadding = 60;

		static nfloat lblTitleXPadding = 15;
		static nfloat lblTitleYPadding = 0;
		static nfloat lblTitleWidthPadding = 180;
		static nfloat lblTitleHeightPadding = 60;

		static nfloat btnToggleYPadding = 15;
		static nfloat btnToggleWidthPadding = 50;
		static nfloat btnToggleHeightPadding = 30;
        public NSIndexPath datePickerIndexPath;
        public UIPickerView subTrackPicker;
        const int kDatePickerTag = 99;
        public const int kDateStartRow = 1;
        public const int kDateEndRow = 2;

        FilterTableSource tabesource;

        List<BuiltTracks> lstBuiltTracks = new List<BuiltTracks>();
        public Dictionary<string, BuiltTracks[]> TrackDictionary = new Dictionary<string, BuiltTracks[]>();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            datePickerIndexPath = null;
            filterTableview = new UITableView
            {
                Frame = new CGRect(filterTableviewXPadding, filterTableviewYPadding, View.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth

            };

            SetHeaderView();
            UIView footer = new UIView(CGRect.Empty);
            filterTableview.TableFooterView = footer;
            View.AddSubview(filterTableview);

            ThreadPool.QueueUserWorkItem((s) =>
            {
                DataManager.GetTracks(AppDelegate.Connection).ContinueWith((res) =>
                {
                    var tmpTracks = res.Result;

                    //foreach (var item in tmpTracks)
                    //{
                    //    if (String.IsNullOrWhiteSpace(item.parentTrackName))
                    //    {
                    //        if (!TrackDictionary.ContainsKey(item.name))
                    //            TrackDictionary.Add(item.name, tmpTracks.Where(p => p.parentTrackName == item.name).ToArray());
                    //    }
                    //}

                    //foreach (var item in TrackDictionary)
                    //{
                    //    Console.WriteLine(item.Value.Length);
                    //}

                    //var parentTracks = tmpTracks.Where(p => String.IsNullOrWhiteSpace(p.parentTrackName)).Select(u => u.name).ToList();
                    ////TrackDictionary = tmpTracks.Where(p => !String.IsNullOrWhiteSpace(p.parentTrackName)).GroupBy(p => p.parentTrackName).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToArray());

                    //int trackIndex = parentTracks.IndexOf("No Track");
                    //parentTracks.RemoveAt(trackIndex);

                    DataManager.GetSessionTracks(AppDelegate.Connection).ContinueWith((t) =>
                        {
                            var result = t.Result;
                            foreach (var item in result)
                            {
                                    if (!TrackDictionary.ContainsKey(item))
                                        TrackDictionary.Add(item, tmpTracks.Where(p => p.parentTrackName == item).ToArray());
                            }
                            Dictionary<string, string[]> dict = new Dictionary<string, string[]>();
                            dict.Add("Tracks", result.ToArray());
                            InvokeOnMainThread(() =>
                            {
                                tabesource = new FilterTableSource(this, dict);
                                filterTableview.Source = tabesource;
                                filterTableview.ReloadData();
                            });
                        });


                });
            });
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            filterTableview.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Size.Height);
            SetHeaderView();
        }
        void SetHeaderView()
        {
            var headerView = new UIView
            {
                Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, headerViewHeightPadding),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            var lblTitle = new UILabel
            {
                Text = "Filter",
                Frame = new CGRect(lblTitleXPadding, lblTitleYPadding, View.Frame.Width - lblTitleWidthPadding, lblTitleHeightPadding),
                TextColor = UIColor.Black,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Font = AppFonts.ProximaNovaRegular(20),
            };

            btnToggle = new UISwitch()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                OnTintColor = AppTheme.btnToggle,
                Frame = new CGRect(View.Frame.Width - (btnToggleWidthPadding + lblTitleXPadding), btnToggleYPadding, btnToggleWidthPadding, btnToggleHeightPadding),
            };

            subTrackPicker = new UIPickerView(CGRect.Empty);
            subTrackPicker.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            subTrackPicker.ShowSelectionIndicator = true;
            subTrackPicker.Tag = kDatePickerTag;

            headerView.AddSubviews(lblTitle);
            filterTableview.TableHeaderView = headerView;
        }

        public void displayInlineDatePickerForRowAtIndexPath(NSIndexPath indexPath)
        {


            this.filterTableview.BeginUpdates();

            bool before = false;
            if (this.datePickerIndexPath != null)
            {
                before = this.datePickerIndexPath.Row < indexPath.Row;
            }

            var row = (this.datePickerIndexPath == null) ? 0 : this.datePickerIndexPath.Row;
            bool sameCellClicked = (row - 1) == indexPath.Row;

            if (this.datePickerIndexPath != null)
            {

                List<string> strings = tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]].ToList();
                strings.RemoveAt(row);
                tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]] = strings.ToArray();
                this.filterTableview.DeleteRows(new NSIndexPath[] { NSIndexPath.FromRowSection(row, 0) }, UITableViewRowAnimation.Fade);
                this.datePickerIndexPath = null;
            }


            if (!sameCellClicked)
            {
                Int32 rowToReveal = (before ? indexPath.Row - 1 : indexPath.Row);
                NSIndexPath indexPathToReveal = NSIndexPath.FromRowSection(rowToReveal, 0);
                if (tabesource.subtrackArray.Count() > 0)
                {
                    tabesource.cell.btncheck.Selected = true;
                    this.toggleDatePickerForSelectedIndexPath(indexPathToReveal);
                    this.datePickerIndexPath = NSIndexPath.FromRowSection(indexPathToReveal.Row + 1, 0);
                }

            }
            else
            {
                preSelectedTrackName = "";
            }
            this.filterTableview.DeselectRow(indexPath, true);

            this.filterTableview.EndUpdates();

            this.updateDatePicker();
        }

        void toggleDatePickerForSelectedIndexPath(NSIndexPath indexPath)
        {
            this.filterTableview.BeginUpdates();

            NSIndexPath[] indexPaths = new[] { NSIndexPath.FromRowSection(indexPath.Row + 1, 0) };

            if (this.hasPickerForIndexPath(indexPath))
            {
                List<string> stringss = tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]].ToList();
                stringss.RemoveAt(indexPath.Row + 1);
                tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]] = stringss.ToArray();

                this.filterTableview.DeleteRows(indexPaths, UITableViewRowAnimation.Fade);
            }
            else
            {
                List<string> stringss = tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]].ToList();
                stringss.Insert(indexPath.Row + 1, "");
                tabesource.indexedTableItems[tabesource.indexedTableItems.Keys.ToArray()[0]] = stringss.ToArray();

                this.filterTableview.InsertRows(indexPaths, UITableViewRowAnimation.Fade);
            }

            this.filterTableview.EndUpdates();
        }

        bool hasPickerForIndexPath(NSIndexPath indexPath)
        {
            bool hasDatePicker = false;


            Int32 targetedRow = indexPath.Row;
            targetedRow++;
            UITableViewCell checkDatePickerCell = this.filterTableview.CellAt(NSIndexPath.FromRowSection(targetedRow, 0));

            if (checkDatePickerCell == null)
                return hasDatePicker;

            UIPickerView checkDatePicker = (UIPickerView)checkDatePickerCell.ViewWithTag(kDatePickerTag);

            hasDatePicker = (checkDatePicker != null);
            return hasDatePicker;
        }
        void updateDatePicker()
        {
            if (this.datePickerIndexPath != null)
            {
                UITableViewCell associatedDatePickerCell = this.filterTableview.CellAt(this.datePickerIndexPath);
                if (associatedDatePickerCell != null)
                {
                    subTrackPicker = (UIPickerView)associatedDatePickerCell.ViewWithTag(kDatePickerTag);
                    if (subTrackPicker != null)
                    {
                        subTrackPicker.ReloadAllComponents();
                    }
                }
            }
        }

    }
    public class FilterTableSource : UITableViewSource
    {

        FilterViewController controller;
        public Dictionary<string, string[]> indexedTableItems;
        public BuiltTracks[] subtrackArray;
        public static Action<string> action;
        NSString cellIdentifier = new NSString("TableCell");
        NSString cellPickerIdentifier = new NSString("CellPicker");

		static nfloat defaultCellSpaceExceptSessionName = 55.0f;
		static nfloat viewXPadding = 0;
		static nfloat viewYPadding = 0;
		static nfloat viewHeightPadding = 60;

		static nfloat lblSectionHeaderXPadding = 15;
		static nfloat lblSectionHeaderYPadding = 10;
		static nfloat lblSectionHeaderHeightPadding = 20;
        public FilterTableCell cell;
        public bool showSubTrackPicker = false;
        public string tracks;

        public FilterTableSource(FilterViewController controller, Dictionary<string, string[]> tracks)
        {
            this.controller = controller;
            indexedTableItems = tracks;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return indexedTableItems.Keys.ToArray().Length;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return indexedTableItems[indexedTableItems.Keys.ToArray()[section]].Length;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {

            UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, viewHeightPadding))
            {
                BackgroundColor = AppTheme.FLSectionBGColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };
            UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, lblSectionHeaderYPadding, tableView.Frame.Width, lblSectionHeaderHeightPadding))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                BackgroundColor = UIColor.Clear,
                TextColor = AppTheme.FVClblSectionHeader,
                Font = AppFonts.ProximaNovaRegular(20),
                Text = indexedTableItems.Keys.ToArray()[section]
            };
            view.AddSubview(lblSectionHeader);
            return view;
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 40;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (this.indexPathHasPicker(indexPath))
            {
                UITableViewCell cell = null;
                NSString cellID = this.cellPickerIdentifier;
                cell = tableView.DequeueReusableCell(cellID) as UITableViewCell;
                if (cell == null) cell = new UITableViewCell();
                cell.ContentView.AddSubview(this.controller.subTrackPicker);
                return cell;
            }
            else
            {
                cell = tableView.DequeueReusableCell(cellIdentifier) as FilterTableCell;
                if (cell == null) cell = new FilterTableCell(cellIdentifier);
                int row = indexPath.Row;
                var menuText = indexedTableItems[indexedTableItems.Keys.ToArray()[indexPath.Section]][row];
                if (menuText.Length > 1)
                {
                    cell.UpdateCell(menuText);
                }
                if (cell.lblTrackName.Text == controller.preSelectedTrackName)
                {
                    cell.btncheck.Selected = true;
                    showSubTrackPicker = true;
                }
                else
                {
                    cell.btncheck.Selected = false;
                }

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.Accessory = UITableViewCellAccessory.None;
                return cell;
            }
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (this.indexPathHasPicker(indexPath))
            {
                return this.controller.subTrackPicker.Frame.Height;
            }
            NSString str = new NSString("");
            var temp = indexedTableItems["Tracks"];
            var item = temp[indexPath.Row];
            if (!string.IsNullOrEmpty(item))
            {
                str = (NSString)item;
            }

            CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(17), new CGSize(tableView.Frame.Width - defaultCellSpaceExceptSessionName, 999), UILineBreakMode.WordWrap);
            var height = size.Height;
            height += 30;

            if (height > 47)
            {
                return height;
            }
            else
            {
                return 47;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.CellAt(indexPath);
            if (cell.GetType() == typeof(FilterTableCell))
            {
                FilterTableCell celltmp = ((FilterTableCell)tableView.CellAt(indexPath));
                var ParentTrackName = celltmp.lblTrackName.Text;
                subtrackArray = this.controller.TrackDictionary[ParentTrackName];
                if (action != null)
                {
                    foreach (UITableViewCell cells in tableView.VisibleCells)
                    {
                        if (cells.GetType() == typeof(FilterTableCell))
                        {
                            FilterTableCell filterCell = (FilterTableCell)cells;
                            filterCell.btncheck.Selected = false;
                        }
                    }
                    tracks = celltmp.lblTrackName.Text;
                    if (tracks != controller.preSelectedTrackName)
                    {
                        celltmp.btncheck.Selected = true;
                        controller.preSelectedTrackName = tracks;
                        action(tracks);
                    }
                    else
                    {
                        if (subtrackArray.Count() <= 0)
                        {
                            celltmp.btncheck.Selected = false;
                            controller.preSelectedTrackName = string.Empty;
                        }
                        action("");
                    }
                }


                if (ParentTrackName == controller.preSelectedTrackName)
                {
                    if (this.controller.TrackDictionary.Count > 0)
                    {

                        var subTrackList = subtrackArray.SelectMany(p => new List<string> { p.name }).ToList();
						this.controller.subTrackPicker.DataSource = new subTrackPickerSource(this.controller, subTrackList);
                        this.controller.displayInlineDatePickerForRowAtIndexPath(indexPath);
                    }
                }
            }
            else
            {
                tableView.DeselectRow(indexPath, true);
            }
        }

        bool indexPathHasDate(NSIndexPath indexPath)
        {
            bool hasDate = false;

            if ((indexPath.Row == FilterViewController.kDateStartRow) ||
                (indexPath.Row == FilterViewController.kDateEndRow || ((this.controller.datePickerIndexPath != null) && (indexPath.Row == FilterViewController.kDateEndRow + 1))))
            {
                hasDate = true;
            }

            return hasDate;
        }

        bool indexPathHasPicker(NSIndexPath indexPath)
        {
            return (this.hasInlineDatePicker() && this.controller.datePickerIndexPath.Row == indexPath.Row);
        }

        bool hasInlineDatePicker()
        {
            return (this.controller.datePickerIndexPath != null);
        }
    }

    public class subTrackPickerSource : UIPickerViewModel
    {
        public static Action<string> subTrackHandler;
        const string defaultTilte = "Sub-Tracks";
        FilterViewController filterViewController;
        static string[] names = new string[] {
			"Brian Kernighan",
			"Dennis Ritchie",
			"Ken Thompson",
			"Kirk McKusick",
			"Rob Pike",
			"Dave Presotto",
			"Steve Johnson"
		};

        List<string> subTracks = new List<string>();
        public subTrackPickerSource(FilterViewController filterViewController, List<string> subtracks)
        {
            this.filterViewController = filterViewController;
            this.subTracks = subtracks;

        }
        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return subTracks.Count + 1;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            if (component == 0)
            {
                if (row == 0)
                {
                    return defaultTilte;
                }
                else
                {
					return subTracks[(int)row - 1];
                }
            }
            else
            {
                return row.ToString();
            }
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            if (row != 0)
            {
                var obj = String.Format("{0}",
					subTracks[(int)row - 1]);
                if (subTrackHandler != null)
                    subTrackHandler(obj);
            }
        }
        public override nfloat GetComponentWidth(UIPickerView picker, nint component)
        {
            if (component == 0)
                return 240f;
            else
                return 40f;
        }
        public override nfloat GetRowHeight(UIPickerView picker, nint component)
        {
            return 40f;
        }
    }

    public class FilterTableCell : UITableViewCell
    {
        public UILabel lblTrackName;
        public UIButton btncheck;

        public BuiltTracks tracksData;

		static nfloat btncheckXPadding = 40;
		static nfloat btncheckYPadding = 15;
		static nfloat btncheckWidhtPadding = 30;
		static nfloat btncheckHeightPadding = 30;

		static nfloat lblTrackNameXPadding = 15;
		static nfloat lblTrackNameYPadding = 10;
		static nfloat lblTrackNameWidthPadding = 15;
		static nfloat lblTrackNameHeightPadding = 20;

        public FilterTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            lblTrackName = new UILabel()
            {
                Lines = 0,
                Font = AppFonts.ProximaNovaSemiBold(17),
                BackgroundColor = UIColor.Clear,
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            btncheck = new UIButton()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,

            };
            btncheck.BackgroundColor = UIColor.Clear;
            btncheck.SetTitle(AppTheme.FTNormalCheckBox, UIControlState.Normal);
            btncheck.SetTitle(AppTheme.FTSelectedCheckBox, UIControlState.Selected);
            btncheck.SetTitleColor(AppTheme.FTNormalCheckBoxNormalColor, UIControlState.Normal);
            btncheck.Font = AppTheme.FTNormalCheckBoxFont;
            btncheck.UserInteractionEnabled = false;

            ContentView.Add(lblTrackName);
            ContentView.Add(btncheck);
        }

        public void UpdateCell(string builtTracks)
        {
            lblTrackName.Text = builtTracks;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            btncheck.Frame = new CGRect(ContentView.Frame.Width - btncheckXPadding, ContentView.Frame.Height / 2 - btncheckYPadding, btncheckWidhtPadding, btncheckHeightPadding);
            lblTrackName.Frame = new CGRect(lblTrackNameXPadding, 15, btncheck.Frame.X - lblTrackNameWidthPadding, 0);
            lblTrackName.SizeToFit();
            lblTrackName.Frame = new CGRect(lblTrackNameXPadding, 15, btncheck.Frame.X - lblTrackNameWidthPadding, lblTrackName.Frame.Height);
        }
    }
}


