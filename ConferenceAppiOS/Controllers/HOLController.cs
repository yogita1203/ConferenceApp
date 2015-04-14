using BigTed;
using CommonLayer;
using CommonLayer.Entities.Built;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Net;
using SDWebImage;
using CoreAnimation;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    [Register("HOLController")]
    public class HOLController : BaseViewController
    {
        string TitleString = AppTheme.HOLTextTitle;
		static nfloat HeaderHeight = 64;
		static nfloat FilterSectionHeight = 120;
		static nfloat NewsTableWidth = 362;
		static nfloat lineSize = 4;
		static nfloat detailPageWidth = 560;
		static nfloat detailPageHeight = 660;
		static nfloat ScrollViewHeight = 175;
		static nfloat TextFieldHeight = 40;
		static nfloat TextViewHeight = 0;
		static nfloat TopBarHeight = 50;
		static nfloat Margin = 20;
		static nfloat topBarTitleLeftMargin = 20;
		static nfloat topBarTitleWidth = 100;

		static nfloat btnDeleteRightMargin = 150;
		static nfloat btnTopMargin = 10;
		static nfloat btnSize = 30;
		static nfloat btnImageEdgeInset = 5;

		static nfloat btnEditLeftMargin = 20;

        UITableView holTable;
       
        HandsOnLabsDataSource handsOnLabsDataSource;

        public HOLController(CGRect rect)
        {
            View.Frame = rect;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = TitleString;

            // Perform any additional setup after loading the view
			View.BackgroundColor = AppTheme.HOLpageBackColor;

            holTable = new UITableView()
            {
                Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            holTable.TableFooterView = new UIView(CGRect.Empty);

            setTableSource();

            View.AddSubviews(holTable);
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.hands_on_labs, Helper.ToDateString(DateTime.Now));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            DeselectRow();
        }

        void DeselectRow()
        {
            if (handsOnLabsDataSource != null && handsOnLabsDataSource.selectedPath != null)
            {
                HandsOnLabsCell cell = (HandsOnLabsCell)holTable.CellAt(handsOnLabsDataSource.selectedPath);
                if (cell.Selected)
                    cell.Selected = false;
            }
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            holTable.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
        }

        private void setTableSource()
        {
            getHandsOnLabs(res =>
            {
                var result = res;

                if (result != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        if (handsOnLabsDataSource == null)
                        {
                            handsOnLabsDataSource = new HandsOnLabsDataSource(this, result);
                            holTable.Source = handsOnLabsDataSource;
                        }
                        else
                        {
                            (handsOnLabsDataSource as HandsOnLabsDataSource).UpdateSource(result);
                        }
                       
                        holTable.ReloadData();
                    });
                }
            });
        }

        private void getHandsOnLabs(Action<Dictionary<string, List<BuiltHandsonLabs>>> callback)
        {
            DataManager.GetSectionedhandsOnLabs(AppDelegate.Connection).ContinueWith(t =>
            {
                var response = t.Result;

                if (callback != null)
                {
                    callback(response);
                }
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.hol))
            {
                setTableSource();
            }
        }
    }

    public class HandsOnLabsDataSource : UITableViewSource
    {
        HOLController AgendaProgramsHandsOnLabsViewController;
        Dictionary<string, List<BuiltHandsonLabs>> handsOnLabsListDictsrc = new Dictionary<string, List<BuiltHandsonLabs>>();
        NSString cellIdentifier = new NSString("TableCell");
		static nfloat defaultRowHeight = 40.0f;
        public NSIndexPath selectedPath;
		static nfloat defaultCellSpaceExceptHOLTitleName = 410.0f;
        string[] keys;
        public HandsOnLabsDataSource(HOLController AgendaProgramsHandsOnLabsViewController, Dictionary<string, List<BuiltHandsonLabs>> handsOnLabsListDictsrc)
        {
            this.AgendaProgramsHandsOnLabsViewController = AgendaProgramsHandsOnLabsViewController;
            this.handsOnLabsListDictsrc = handsOnLabsListDictsrc;
            keys = handsOnLabsListDictsrc.Keys.ToArray();

        }
        public void UpdateSource(Dictionary<string, List<BuiltHandsonLabs>> handsOnLabsListDictsrc)
        {
            this.handsOnLabsListDictsrc = handsOnLabsListDictsrc;
            this.keys = handsOnLabsListDictsrc.Keys.ToArray();
        }
       
   
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return handsOnLabsListDictsrc[keys[section]].Count;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {

			UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
            {
                BackgroundColor = AppTheme.HOLheaderBackColor,
            };
            view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            var border = new CALayer();
            border.Frame = new CGRect(0, 0, view.Frame.Width, 1);
            border.BackgroundColor = AppTheme.HOLheaderBorderColor.CGColor;
            var borderBottom = new CALayer();
            borderBottom.Frame = new CGRect(0, view.Frame.Size.Height, tableView.Frame.Width, 1);
            borderBottom.BackgroundColor = AppTheme.HOLheaderBorderColor.CGColor;
            view.Layer.AddSublayer(borderBottom);
            if (section == 0)
            {
				UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, 200, AppTheme.SectionHeight))
                {
                    Text = TitleForHeader(tableView, section),
                    TextColor = AppTheme.HOLheaderLabelTitleColor,
                    Font = AppTheme.HOLheaderLabelFont  
                };
                view.AddSubview(lblSectionHeader);
                return view;
            }
            else
            {
				UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, 200, AppTheme.SectionHeight))
                {
                    TextColor = AppTheme.HOLheaderLabelTitleColor,
                    Text = TitleForHeader(tableView, section),
                    Font = AppTheme.HOLheaderLabelFont,
                };
                var borderFeat = new CALayer();
                borderFeat.Frame = new CGRect(0, 0, view.Frame.Width, 1);
                borderFeat.BackgroundColor = AppTheme.HOLheaderBorderColor.CGColor;
                view.Layer.AddSublayer(borderFeat);
                view.AddSubviews(lblSectionHeader);
                return view;
            }
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
			return AppTheme.SectionHeight;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Length;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return keys[section];
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            HandsOnLabsCell cell = tableView.DequeueReusableCell(cellIdentifier) as HandsOnLabsCell;
            if (cell == null) cell = new HandsOnLabsCell(cellIdentifier);
            var item = handsOnLabsListDictsrc[keys[indexPath.Section]][indexPath.Row];
            cell.UpdateCell(item);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = handsOnLabsListDictsrc[keys[indexPath.Section]][indexPath.Row];
            NSString str = (NSString)item.title;
			CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(18), new CGSize(tableView.Frame.Width - 80, 999), UILineBreakMode.WordWrap);
			size.Height = size.Height + 20;
			if (size.Height < defaultRowHeight)
            {
                return defaultRowHeight;
            }

			return size.Height;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat holdcXPadding = 0;
            nfloat holdcYPadding = 0;
            nfloat holdcWidthPadding = 477;
            selectedPath = indexPath;
            HandsOnLabsDetailController holdcs = new HandsOnLabsDetailController(handsOnLabsListDictsrc[keys[indexPath.Section]][indexPath.Row]);
            holdcs.View.Frame = new CGRect(holdcXPadding, holdcYPadding, holdcWidthPadding, AgendaProgramsHandsOnLabsViewController.View.Frame.Size.Height);
            AppDelegate.instance().rootViewController.openDetail(holdcs, AgendaProgramsHandsOnLabsViewController, true);
        }
    }
}