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
    public partial class FoodAndDrinkController : BaseViewController
    {
        Dictionary<string, BuiltSFFoodNDrink[]> foodNdrinks;
        public Action<string> linkCliked;

        #region--Views--
        UIView headerView;
        UITableView foodAndDrinkTable;
        FoodAndDrinkTableDataSource dataSource;
        #endregion

        #region--constants--
		static nfloat categorySegmentInterestControlHeight = 55;

        string title = AppTheme.FDtitleText;

		static nfloat headerViewXPadding = 0;
		static nfloat headerViewYPadding = 0;

		static nfloat foodAndDrinkTableXPadding = 0;
		static nfloat foodAndDrinkTableYPadding = 0;

		static nfloat foodAndDrinkTableHeaderViewXPadd = 0;
		static nfloat foodAndDrinkTableHeaderViewYPadd = 0;

		static nfloat foodAndDrinkTableFooterViewXPadd = 0;
		static nfloat foodAndDrinkTableFooterViewYPadd = 0;
		static nfloat foodAndDrinkTableFooterViewHeight = 1;
        #endregion

        public FoodAndDrinkController(CGRect rect)
        {
            View.Frame = rect;
            TabBarItem.Title = title;
            TabBarItem.Image = UIImage.FromFile(AppTheme.FDTabImage);

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            foodAndDrinkTable = new UITableView();
            foodAndDrinkTable.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            foodAndDrinkTable.SeparatorInset = new UIEdgeInsets(0, 20, 0, 0);
            headerView = new UIView();

            
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, categorySegmentInterestControlHeight);
            foodAndDrinkTable.Frame = new CGRect(foodAndDrinkTableXPadding, foodAndDrinkTableYPadding, View.Frame.Size.Width / 2, View.Frame.Size.Height);

          

            foodAndDrinkTable.TableHeaderView = new UIView(new CGRect(foodAndDrinkTableHeaderViewXPadd, foodAndDrinkTableHeaderViewYPadd, View.Frame.Width, categorySegmentInterestControlHeight));
            foodAndDrinkTable.TableFooterView = new UIView(new CGRect(foodAndDrinkTableFooterViewXPadd, foodAndDrinkTableFooterViewYPadd, View.Frame.Width, foodAndDrinkTableFooterViewHeight));


            DataManager.GetFoodNDrink(AppDelegate.Connection).ContinueWith(t =>
            {
                foodNdrinks = t.Result;

                InvokeOnMainThread(() =>
                {
                    SegmentView categorySegmentInterestControl = new SegmentView(foodNdrinks.Keys.ToList());
                    categorySegmentInterestControl.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
                    categorySegmentInterestControl.Frame = new CGRect(-4, 0, headerView.Frame.Width + 12, categorySegmentInterestControlHeight);
                    categorySegmentInterestControl.segmentViewViewDelegate += selectedtab;
                    categorySegmentInterestControl.BackgroundColor = AppTheme.FDsegmentViewBackColor;
                    categorySegmentInterestControl.selectedSegmentIndex = 0;
                    categorySegmentInterestControl.textColor = AppTheme.FDsegmentTitletextColor;
					categorySegmentInterestControl.subHeadingTextColor = AppTheme.FDsegmentSubTitletextColor;
                    categorySegmentInterestControl.font = AppTheme.FDsegmentFont;
                    categorySegmentInterestControl.selectedBoxColor = AppTheme.FDsegmentSelectedtabColor;
                    categorySegmentInterestControl.selectedBoxColorOpacity = 1.0f;
                    categorySegmentInterestControl.selectedBoxBorderWidth = 0.0f;
                    categorySegmentInterestControl.selectionIndicatorHeight = 4.0f;
                    categorySegmentInterestControl.selectedTextColor = AppTheme.FDsegmentTitletextColor;
                    categorySegmentInterestControl.selectionIndicatorColor = AppTheme.FDsegmentSelectedtabBottomColor;
                    categorySegmentInterestControl.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
                    categorySegmentInterestControl.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
                    categorySegmentInterestControl.multiLineSupport = false;
                    categorySegmentInterestControl.multiLineFont = AppTheme.FDmultiLineFont;
                    categorySegmentInterestControl.separatorColor = AppTheme.FDsegmentSeparatorColor;
                    categorySegmentInterestControl.Layer.CornerRadius = 0.0f;
                    categorySegmentInterestControl.Layer.BorderColor = AppTheme.FDsegmentSeparatorColor.CGColor;
                    categorySegmentInterestControl.Layer.BorderWidth = 1.0f;
                    headerView.AddSubview(categorySegmentInterestControl);
                    dataSource = new FoodAndDrinkTableDataSource(this, foodNdrinks);
                    foodAndDrinkTable.Source = dataSource;
                    foodAndDrinkTable.ReloadData();
                    selectRow();
                });
            });

            View.AddSubview(foodAndDrinkTable);
            View.AddSubview(headerView);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.food_drink, Helper.ToDateString(DateTime.Now));
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            headerView.Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, categorySegmentInterestControlHeight);
            foodAndDrinkTable.Frame = new CGRect(foodAndDrinkTableXPadding, foodAndDrinkTableYPadding, View.Frame.Size.Width, View.Frame.Size.Height);
        }

        void selectedtab(int index)
        {
            dataSource.selectedTab = index;
            foodAndDrinkTable.ReloadData();
            selectRow();
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);
            if (dataSource != null && dataSource.selectedIndex != null)
            {
                foodAndDrinkTable.DeselectRow(dataSource.selectedIndex, false);
            }
        }
        void selectRow()
        {
            if (foodAndDrinkTable != null && foodNdrinks != null)
            {
                foodAndDrinkTable.SelectRow(NSIndexPath.FromRowSection(0, 0), true, UITableViewScrollPosition.None);
                if (foodNdrinks.Count > 0)
                {
                    var item = foodNdrinks[foodNdrinks.Keys.ToArray()[dataSource.selectedTab]][0];
                    if (linkCliked != null)
                    {
                        var link = Helper.getFoodDrinkLink(item.link_group);
                        linkCliked(link);
                    }
                }
            }
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.food_n_drink))
            {
                updateFoodNDrinkSource();
                return;
            }
        }

        private void updateFoodNDrinkSource()
        {
            DataManager.GetFoodNDrink(AppDelegate.Connection).ContinueWith(t =>
          {
              foodNdrinks = t.Result;
              InvokeOnMainThread(() => {
                  (foodAndDrinkTable.Source as FoodAndDrinkTableDataSource).UpdateSource(foodNdrinks);
                  foodAndDrinkTable.ReloadData();
                  selectRow();
              });
              
          });
        }
    }


    #region--FoodAndDrinkDataSource--
    public class FoodAndDrinkTableDataSource : UITableViewSource
    {
        FoodAndDrinkController foodAndDrinkController;
        Dictionary<string, BuiltSFFoodNDrink[]> items;
        string[] keys;
		static nfloat defaultCellHeight = 70.0f;
		static nfloat leftrightMargin = 20.0f;
		static nfloat defaultCellSpaceExceptSessionName = 60;
        public NSIndexPath selectedIndex;

        public int selectedTab = 0;
        NSString cellIdentifier = new NSString("FoodAndDrinkCell");

        public FoodAndDrinkTableDataSource(FoodAndDrinkController AgendaProgramsHandsOnLabsViewController, Dictionary<string, BuiltSFFoodNDrink[]> dictionary)
        {
            this.foodAndDrinkController = AgendaProgramsHandsOnLabsViewController;
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
            cell.cellForFoodAndDrinks = true;
            var item = items[keys[selectedTab]][indexPath.Row];
            cell.updateCell(item);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = items[keys[selectedTab]][indexPath.Row];
            NSString str = (NSString)item.address;
			CGSize size = str.StringSize(AppTheme.FDDetailTextFont, new CGSize(tableView.Frame.Width , 999), UILineBreakMode.WordWrap);
            if (size.Height < defaultCellHeight)
            {
                return defaultCellHeight;
            }

            return size.Height+20;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;

            var item = items[keys[selectedTab]][indexPath.Row];
            if (foodAndDrinkController.linkCliked != null)
            {
                var link = Helper.getFoodDrinkLink(item.link_group);
                foodAndDrinkController.linkCliked(link);

            }
        }


        internal void UpdateSource(Dictionary<string, BuiltSFFoodNDrink[]> foodNdrinks)
        {
            this.items = foodNdrinks;
        }
    }
    #endregion
}


