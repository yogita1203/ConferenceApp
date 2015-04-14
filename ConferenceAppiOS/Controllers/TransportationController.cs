using System;
using CoreGraphics;
using CommonLayer.Entities.Built;
using Foundation;
using UIKit;
using CommonLayer;
using System.Collections.Generic;
using System.Linq;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    public partial class TransportationController : BaseViewController
    {
        #region--Constants--
		static nfloat categorySegmentInterestControlHeight = 55;

		static nfloat headerViewXPadding =0;
		static nfloat headerViewYPadding = 0;

		static nfloat transportationTableXPadding = 0;
		static nfloat transportationTableYPadding = 0;

        #endregion

        #region--Views--
        UIView headerView;
        UITableView transportationTable;
        #endregion

        
        Dictionary<string, BuiltTransportation[]> foodNdrinks;
        TransportationTableDataSource dataSource;
        public Action<string> transportLinkCliked;

        public TransportationController(CGRect rect)
        {
            View.Frame = rect;
            TabBarItem.Title = AppTheme.TStransportationText;
            TabBarItem.Image = UIImage.FromFile("transport.png");

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            transportationTable = new UITableView();
            transportationTable.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            transportationTable.SeparatorInset = new UIEdgeInsets(0, 20, 0, 0);
            headerView = new UIView();
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, categorySegmentInterestControlHeight);
            transportationTable.Frame = new CGRect(transportationTableXPadding, transportationTableYPadding, View.Frame.Size.Width / 2, View.Frame.Size.Height);

            transportationTable.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width, categorySegmentInterestControlHeight));
            transportationTable.TableFooterView = new UIView(new CGRect(0, 0, View.Frame.Width, 1));


            DataManager.GetTransportation(AppDelegate.Connection).ContinueWith(t =>
                {
                    foodNdrinks = t.Result;
                    InvokeOnMainThread(() =>
                        {
                            SegmentView categorySegmentInterestControl = new SegmentView(foodNdrinks.Keys.ToList());
                            categorySegmentInterestControl.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                            categorySegmentInterestControl.Frame = new CGRect(-4, 0, headerView.Frame.Width + 12, categorySegmentInterestControlHeight);
                            categorySegmentInterestControl.segmentViewViewDelegate += selectedtab;
                            categorySegmentInterestControl.BackgroundColor = AppTheme.TSsegmentbackColor;
                            categorySegmentInterestControl.selectedSegmentIndex = 0;
                            categorySegmentInterestControl.textColor = AppTheme.TSsegmentTitletextColor;
							categorySegmentInterestControl.subHeadingTextColor = AppTheme.TSsegmentSubTitletextColor;
                            categorySegmentInterestControl.font = AppTheme.TSsegmentFont;
                            categorySegmentInterestControl.selectedBoxColor = AppTheme.TSsegmentSelectedtabColor;
                            categorySegmentInterestControl.selectedBoxColorOpacity = 1.0f;
                            categorySegmentInterestControl.selectedBoxBorderWidth = 0.0f;
                            categorySegmentInterestControl.selectionIndicatorHeight = 4.0f;
                            categorySegmentInterestControl.selectedTextColor = AppTheme.TSsegmentTitletextColor;
                            categorySegmentInterestControl.selectionIndicatorColor = AppTheme.TSsegmentSelectedtabBottomColor;
                            categorySegmentInterestControl.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                            categorySegmentInterestControl.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                            categorySegmentInterestControl.multiLineSupport = false;
                            categorySegmentInterestControl.multiLineFont = AppTheme.TSmultiLineFont;
                            categorySegmentInterestControl.separatorColor = AppTheme.TSsegmentSeparatorColor;
                            categorySegmentInterestControl.Layer.CornerRadius = 0.0f;
                            categorySegmentInterestControl.Layer.BorderColor = AppTheme.TSsegmentSeparatorColor.CGColor;
                            categorySegmentInterestControl.Layer.BorderWidth = 1.0f;
                            headerView.AddSubview(categorySegmentInterestControl);
                            dataSource = new TransportationTableDataSource(this, foodNdrinks);
                            transportationTable.Source = dataSource;
                            transportationTable.ReloadData();
                            selectRowForTransportationTable();
                        });
                });

            View.AddSubview(transportationTable);
            View.AddSubview(headerView);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.transportation, Helper.ToDateString(DateTime.Now));
        }

        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public void selectRowForTransportationTable()
           {
			if (transportationTable != null && foodNdrinks != null) {
				transportationTable.SelectRow(NSIndexPath.FromRowSection(0,0),true,UITableViewScrollPosition.None);
				if (foodNdrinks.Count > 0) {
					var item = foodNdrinks[foodNdrinks.Keys.ToArray()[dataSource.selectedTab]][0];
					if (transportLinkCliked != null)
					{
                        var link = Helper.getTransportationLink(item.link_group);
                        transportLinkCliked(link);
					}
				}
			}
		}

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, categorySegmentInterestControlHeight);
            transportationTable.Frame = new CGRect(transportationTableXPadding, transportationTableYPadding, View.Frame.Size.Width, View.Frame.Size.Height);
        }

        void selectedtab(int index)
        {
            dataSource.selectedTab = index;
            transportationTable.ReloadData();
            selectRowForTransportationTable();
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);
            if (dataSource != null && dataSource.selectedIndex != null)
            {
                transportationTable.DeselectRow(dataSource.selectedIndex, false);
            }
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.transportation))
            {
                updateTransportationSource();
                return;
            }
        }

        private void updateTransportationSource()
        {
            DataManager.GetTransportation(AppDelegate.Connection).ContinueWith(t =>
             {
                 foodNdrinks = t.Result;
                 InvokeOnMainThread(() =>
                 {
                     (transportationTable.Source as TransportationTableDataSource).UpdateSource(foodNdrinks);
                     transportationTable.ReloadData();
                     selectRowForTransportationTable();
                 });
             });
        }
    }


    #region--TransportationDataSource
    public class TransportationTableDataSource : UITableViewSource
    {
        TransportationController transportationController;
        public NSIndexPath selectedIndex;
        Dictionary<string, BuiltTransportation[]> items;
        string[] keys;
		static nfloat defaultCellHeight = 70.0f;
		static nfloat leftrightMargin = 20.0f;
        public int selectedTab = 0;
		static nfloat defaultCellSpaceExceptSessionName = 60;
        NSString cellIdentifier = new NSString("TransportaionCell");

        public TransportationTableDataSource(TransportationController transportationController, Dictionary<string, BuiltTransportation[]> dictionary)
        {
            this.transportationController = transportationController;
            items = dictionary;
            keys = dictionary.Keys.ToArray();

        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return items[keys[selectedTab]].Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            FoodAndDrinkCell cell = tableView.DequeueReusableCell(cellIdentifier) as FoodAndDrinkCell;
            if (cell == null) cell = new FoodAndDrinkCell(cellIdentifier);
            cell.cellForFoodAndDrinks = false;
            var item = items[keys[selectedTab]][indexPath.Row];
            cell.updateTranspotationCell(item);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = items[keys[selectedTab]][indexPath.Row];
            NSString str = (NSString)item.short_desc;
			NSString str1 = (NSString)item.name;

			CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(14), new CGSize((tableView.Frame.Width - 90) , 999), UILineBreakMode.WordWrap);
			CGSize size1 = str1.StringSize(AppFonts.ProximaNovaRegular(18), new CGSize((tableView.Frame.Width - 90) , 999), UILineBreakMode.WordWrap);
			size.Height = size.Height + size1.Height + 30;

			if (size.Height < defaultCellHeight)
			{
				return defaultCellHeight+10;
			}


            return size.Height;
        }
    
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			selectedIndex = indexPath;
			var item = items[keys[selectedTab]][indexPath.Row];
            if (transportationController.transportLinkCliked != null)
			{
				var link = Helper.getTransportationLink(item.link_group);
                transportationController.transportLinkCliked(link);
			}
        }

        internal void UpdateSource(Dictionary<string, BuiltTransportation[]> foodNdrinks)
        {
            this.items = foodNdrinks;
        }
    }
    #endregion
}

