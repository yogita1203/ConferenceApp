using System;
using UIKit;
using Foundation;
using CommonLayer.Entities.Built;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace ConferenceAppiOS
{
    public class SponsorsCollectionView : UICollectionViewController    
	{
		static NSString cellId = new NSString("Cell");
		static NSString headerId = new NSString("Header");
		public Dictionary<string, List<BuiltExhibitor>> items;
		List<string>keys;
		public NSIndexPath selectedIndex;

		public SponsorsCollectionView  (UICollectionViewLayout layout) : base (layout)
		{
           
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CollectionView.BackgroundColor = AppTheme.EXpageBackground;
			CollectionView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			keys = new List<string>();
			items = new Dictionary<string, List<BuiltExhibitor>> ();
			CollectionView.RegisterClassForCell (typeof(SponsorsCell), cellId);
			CollectionView.RegisterClassForSupplementaryView (typeof(HeaderView), UICollectionElementKindSection.Header, headerId);
		}


		public void UpdateSource(Dictionary<string, List<BuiltExhibitor>> exhibitorSource)
		{
			if (exhibitorSource != null) {
				items = exhibitorSource;
				keys = items.Keys.ToList();

                this.CollectionView.Delegate = new customDelegate(keys, items);
			}
		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return keys.Count;
		}
			
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return items[keys[(int)section]].Count;
		}

        

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (SponsorsCell)collectionView.DequeueReusableCell (cellId, indexPath);
			var exhibitor = items[keys[indexPath.Section]][indexPath.Row];
			cell.UpdateCell (exhibitor);
			return cell;
		}

		public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			if (keys [indexPath.Section].Length > 0) {
				var headerView = (HeaderView) collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
				headerView.updateheader (keys [indexPath.Section]);
				return headerView;
			} else {
                var headerView = (HeaderView)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
                headerView.BackgroundColor = AppTheme.EXpageBackground;
                headerView.updateheader (keys [indexPath.Section]);
                return headerView;
			}
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			selectedIndex = indexPath;
			var exhibitor = items[keys[indexPath.Section]][indexPath.Row];
			if (exhibitor != null)
			{
				ExhibitorDetailController vc = new ExhibitorDetailController(exhibitor);
				AppDelegate.instance().rootViewController.openDetail(vc, null, true);
			}
		}
			
		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = AppTheme.EXpageBackground;
		}

		public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem (indexPath);
			cell.ContentView.BackgroundColor = AppTheme.EXpageBackground;
		}

		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

      

	}

    class customDelegate : UICollectionViewDelegateFlowLayout
    {

        List<string> keys;
        public Dictionary<string, List<BuiltExhibitor>> items;
        public customDelegate(List<string> keys, Dictionary<string, List<BuiltExhibitor>> items)
        {
            this.keys = keys;
            this.items=items;
        }
        public NSIndexPath selectedIndex;
        public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
			if (this.keys[(int)section] == string.Empty)
            {
                return new CGSize(0, 0);
            }
            else
            {
                return new CGSize(collectionView.Frame.Width, 30);
            }
        }
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            var exhibitor = items[keys[indexPath.Section]][indexPath.Row];
            if (exhibitor != null)
            {
                ExhibitorDetailController vc = new ExhibitorDetailController(exhibitor);
                AppDelegate.instance().rootViewController.openDetail(vc, null, true);
            }
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = AppTheme.EXpageBackground;
        }
        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = AppTheme.EXpageBackground;
        }

        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }
    }
}

