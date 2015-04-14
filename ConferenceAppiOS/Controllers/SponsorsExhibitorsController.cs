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
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS
{
    [Register("SponsorsExhibitorsController")]
    public class SponsorsExhibitorsController : BaseViewController
    {
        #region--Views--
        //LoadingOverlay loadingOverlay;
		UICollectionViewFlowLayout sponsorsFlowLayout;
		SponsorsCollectionView sponsorsCollectionView;
        CGRect frm;
		public static NSString cellId = new NSString("Cell");
		public static NSString headerId = new NSString("Header");
        #endregion

        #region--constants--
		static nfloat headerViewLeftMargin = -3;
		static nfloat headerHeight = 64;
		static nfloat sepLeftInset = 100;
        const int exhibitorLimit = 50;
        int exhibitorOffset = 0;
		static nfloat exhibitorTableViewXPadding = 0;
		static nfloat exhibitorTableViewYPadding = 0;
		static nfloat cellheight = 200;
		static nfloat SectionheaderHeight = 30;
        string TitleString = AppTheme.SEexhibitorsTitleString;
        #endregion

		Dictionary<string, List<BuiltExhibitor>> exhibitorSource =new Dictionary<string,List<BuiltExhibitor>>();


        public SponsorsExhibitorsController(CGRect rect)
        {
            frm = rect;
            View.Frame = rect;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = frm;
			sponsorsFlowLayout = new UICollectionViewFlowLayout()
			{
				ItemSize = new CGSize((View.Frame.Width / 3), (View.Frame.Width / 3)),
				HeaderReferenceSize = new CGSize(View.Frame.Width, SectionheaderHeight),
				ScrollDirection = UICollectionViewScrollDirection.Vertical,
				MinimumInteritemSpacing = 0, // minimum spacing between cells
				MinimumLineSpacing = 0 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
			};

			sponsorsCollectionView = new SponsorsCollectionView(sponsorsFlowLayout);

            setExhibitorHeader();

			GetAllSponsorsAndPopulateInView ();

			//ShowOverlay(sponsorsCollectionView.View);

			View.AddSubview(sponsorsCollectionView.View);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.sponsors, Helper.ToDateString(DateTime.Now));
        }

		void GetAllSponsorsAndPopulateInView(){

            LoadingView.Show(string.Empty);
			getAllExhibitorsByOrder(res => {

				InvokeOnMainThread(() =>
					{
						if (sponsorsCollectionView.items.Count > 0)
						{
							foreach (var item in res)
							{
								if (sponsorsCollectionView.items.ContainsKey(item.Key))
									sponsorsCollectionView.items[item.Key].AddRange(item.Value);
								else
									sponsorsCollectionView.items.Add(item.Key, item.Value);
							}
							sponsorsCollectionView.UpdateSource(sponsorsCollectionView.items);
							sponsorsCollectionView.CollectionView.ReloadData();//sponsorsCollectionView.reload
                            LoadingView.Dismiss();
						}
						else
						{ 
							sponsorsCollectionView.UpdateSource(res);
							sponsorsCollectionView.CollectionView.ReloadData();//sponsorsCollectionView.reload
                            LoadingView.Dismiss();
						}
					});
			});
		}

		void ReloadAllSponsors(){
			getAllExhibitorsByOrder(res => {
				InvokeOnMainThread(() =>
					{
						sponsorsCollectionView.UpdateSource(res);
						sponsorsCollectionView.CollectionView.ReloadData();
					});
			});
		}

        public override void OnDetailClosing(NSNotification notification)
        {//..
			if (sponsorsCollectionView != null && sponsorsCollectionView.selectedIndex != null)
            {
//                exhibitorTableView.DeselectRow(sponsorsDataSource.selectedIndex, false);
            }
        }

        private void getExhibitors(Action<Dictionary<string, List<BuiltExhibitor>>> callback)
        {

            DataManager.GetExhibitors(AppDelegate.Connection, exhibitorLimit, exhibitorOffset).ContinueWith(t =>
                {
                    var res = t.Result;
                    Dictionary<string, List<BuiltExhibitor>> result = res.GroupBy(p => p.name.First().ToString().ToUpper()).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
                    if (callback != null)
                    {
                        callback(result);
                    }

                    if (res.Count == exhibitorLimit)
                    {
                        exhibitorOffset += exhibitorLimit;
                        getExhibitors(callback);
                    }
                });
        }

        private void getAllExhibitorsByOrder(Action<Dictionary<string, List<BuiltExhibitor>>> callback)
        {
			DataManager.GetAllExhibitorsByOrder(AppDelegate.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                if (callback != null)
                {
						callback(res);
                }
            });
        }

        private void updateExhibitors()
        {
			DataManager.GetAllExhibitorsByOrder(AppDelegate.Connection).ContinueWith(t =>
				{
					exhibitorSource = t.Result;
					InvokeOnMainThread(() =>
						{
                            sponsorsCollectionView.UpdateSource(exhibitorSource);
							sponsorsCollectionView.CollectionView.ReloadData();
						});
				});
        }

        public override void ViewWillLayoutSubviews()
        {

            base.ViewWillLayoutSubviews();
			if (sponsorsCollectionView != null)
            {

				sponsorsFlowLayout.ItemSize = new CGSize ((View.Frame.Width / 3), (View.Frame.Width / 3));
				sponsorsCollectionView.View.Frame = new CGRect(exhibitorTableViewXPadding, exhibitorTableViewYPadding, View.Frame.Width, View.Frame.Height-(exhibitorTableViewYPadding-SectionheaderHeight));

				setExhibitorHeader();
            }
        }

        void setExhibitorHeader()
        {
            TitleHeaderView titleheaderView = new TitleHeaderView(TitleString, true, false, true, false, false, false, false, false);
            titleheaderView.searchFieldClicked = (textField) =>
            {
                textField.ResignFirstResponder();
                searchExhibitors(textField.Text);
            };
            titleheaderView.searchButtonClicked = (textField) =>
            {
                textField.ResignFirstResponder();
                if (string.IsNullOrWhiteSpace(titleheaderView.searchField.Text))
                {
                    return;
                }
                searchExhibitors(textField.Text.Trim());
            };

            titleheaderView.searchField.EditingChanged += (s, e) =>
            {
                if (titleheaderView.searchField.Text == string.Empty)
                {
					ReloadAllSponsors();
                }

            };

            titleheaderView.RefreshButtonClicked = () =>
            {
            };

            titleheaderView.AddButtonClicked = () =>
            {

            };

            titleheaderView.ShareButtonClicked = () =>
            {

            };

            titleheaderView.Frame = new CGRect(headerViewLeftMargin, 0, View.Frame.Width, headerHeight);

            View.AddSubview(titleheaderView);

			sponsorsCollectionView.View.Frame = new CGRect(exhibitorTableViewXPadding, titleheaderView.Frame.Bottom, View.Frame.Width, View.Frame.Height-(titleheaderView.Frame.Bottom));

        }

        private void searchExhibitors(string p)
        {
            DataManager.GetSearchExhibitorWithSection(AppDelegate.Connection, p).ContinueWith(t =>
                {
					var result = t.Result;
                    InvokeOnMainThread(() =>
                        {
							sponsorsCollectionView.UpdateSource(t.Result);
							sponsorsCollectionView.CollectionView.ReloadData();
                        });
                });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.exhibitor))
            {
                updateExhibitors();
            }
        }

        public void ShowSponsorDetails(string exhibitor_id)
        {
            DataManager.GetExhibitorFromId(AppDelegate.Connection, exhibitor_id).ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        ExhibitorDetailController vc = new ExhibitorDetailController(t.Result);
                        AppDelegate.instance().rootViewController.openDetail(vc, null, true);
                    });
                }
            });
        }
    }

	#region --SponsorsDataSource=--
	public class SponsorsDatasource : UICollectionViewDataSource{
		Dictionary<string, List<BuiltExhibitor>> items;
		string[] keys;
		public NSIndexPath selectedIndex;
		static NSString cellId = new NSString("Cell");
		static NSString headerId = new NSString("Header");
		public SponsorsDatasource(Dictionary<string, List<BuiltExhibitor>> items)
		{
			keys = items.Keys.ToArray();
			this.items = items;
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return keys.Length;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return items[keys[section]].Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (SponsorsCell)collectionView.DequeueReusableCell (cellId, indexPath);
			var exhibitor = items[keys[indexPath.Section]][indexPath.Row];
			cell.UpdateCell (exhibitor);

			return cell;
		}
			
//		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			selectedIndex = indexPath;
//
//			var exhibitor = items[keys[indexPath.Section]][indexPath.Row];
//			if (exhibitor != null)
//			{
//				ExhibitorDetailController vc = new ExhibitorDetailController(exhibitor);
//				AppDelegate.instance().rootViewController.openDetail(vc, null, true);
//			}
//		}
//


		public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			if (keys [indexPath.Section].Length > 0) {
				var header = (HeaderView)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
				header.updateheader (keys [indexPath.Section]);
				header.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth; 
				return header;
			} else {
				var view = (HeaderView)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
				view.Frame = CGRect.Empty;
				view.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
				return view;
			}
		}

//		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			selectedIndex = indexPath;
//			var cell = collectionView.CellForItem (indexPath);
//			cell.ContentView.BackgroundColor = UIColor.Yellow;
//		}

//		public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			var cell = collectionView.CellForItem (indexPath);
//			cell.ContentView.BackgroundColor = UIColor.White;
//		}

//		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
//		{
//			return true;
//		}

		internal void UpdateSource(Dictionary<string, List<BuiltExhibitor>> exhibitorSource)
		{
			keys = items.Keys.ToArray();
			this.items = exhibitorSource;
		}
	}
	#endregion

   	#region --HeaderView-- 
	public class HeaderView : UICollectionReusableView {
        string title = string.Empty;
			public UIButton userLabel;
		[Export ("initWithFrame:")]
		public HeaderView (CGRect frame) : base (frame)
		{
            BackgroundColor = AppTheme.LMmenuSectionBackgroundColor;
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            userLabel = new UIButton(UIButtonType.Custom);
            userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Normal);
            userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Highlighted);
            userLabel.SetTitleColor(AppTheme.LMMenuSectionFontColor, UIControlState.Selected);
            userLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            userLabel.TitleLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            userLabel.TitleLabel.TextAlignment = UITextAlignment.Left;
            userLabel.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            userLabel.BackgroundColor = BackgroundColor;
            //userLabel.Font = AppTheme.SESectionTitleFont;
            AddSubview(userLabel);
		}

		public void updateheader(string title){
            this.title = title;
			userLabel.SetTitle (title, UIControlState.Normal);
			userLabel.SetTitle (title, UIControlState.Highlighted);
			userLabel.SetTitle (title, UIControlState.Selected);
		}

		public override void LayoutSubviews ()
		{
                userLabel.BackgroundColor = BackgroundColor;
                userLabel.Frame = new CGRect(15, 0, Frame.Width - 15, 30);
			
		}
      
	}

   

	#endregion

}