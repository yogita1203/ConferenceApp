using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using SQLiteNetExtensions.Extensions;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
   public class SubTrackController:BaseViewController
    {
       public string preSelectedSubTrackName = string.Empty;
		static nfloat filterTableviewXPadding = 0;
		static nfloat filterTableviewYPadding = 0;

		static nfloat headerViewXPadding = 0;
		static nfloat headerViewYPadding = 0;
		static nfloat headerViewHeightPadding = 60;

		static nfloat lblTitleXPadding = 15;
		static nfloat lblTitleYPadding = 0;
		static nfloat lblTitleWidthPadding = 180;
		static nfloat lblTitleHeightPadding = 60;

       public override void ViewDidLoad()
       {
           base.ViewDidLoad();

           var subTrackTableview = new UITableView
           {
               Frame = new CGRect(filterTableviewXPadding, filterTableviewYPadding, View.Frame.Width, View.Frame.Height),
               AutoresizingMask = UIViewAutoresizing.FlexibleWidth

           };


           var headerView = new UIView
           {
               Frame = new CGRect(headerViewXPadding, headerViewYPadding, View.Frame.Width, headerViewHeightPadding),
               AutoresizingMask = UIViewAutoresizing.FlexibleWidth
           };



           var lblTitle = new UILabel
           {
               Text = "SubTrack",
               Frame = new CGRect(lblTitleXPadding, lblTitleYPadding, View.Frame.Width - lblTitleWidthPadding, lblTitleHeightPadding),
               TextColor = UIColor.Black,
               AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
               Font = UIFont.SystemFontOfSize(20),
           };

           headerView.AddSubviews(lblTitle);
           subTrackTableview.TableHeaderView = headerView;
           UIView footer = new UIView(CGRect.Empty);
           subTrackTableview.TableFooterView = footer;

           string trackname=string.Empty;
           BuiltTracks temp = null;

           var tracks = AppDelegate.Connection.GetAllWithChildren<BuiltTracks>().Where(p => p.parentTrackName != null).ToList();
           var temptracks = tracks.Where(p => p.parentTrackName == trackname).ToArray();  
           Dictionary<string,BuiltTracks[]> dict = new Dictionary<string,BuiltTracks[]>();
           dict.Add(trackname,temptracks);
           subTrackTableview.Source = new SubTrackTableSource(this,dict);
           //tracks.GroupBy(p => p.parentTrackName);
           View.AddSubview(subTrackTableview);
       }

       public class SubTrackTableSource : UITableViewSource
       {
           SubTrackController controller;
           Dictionary<string, BuiltTracks[]> indexedTableItems;
           string[] keys;
           NSString cellIdentifier = new NSString("TableCell");

			static nfloat viewXPadding = 0;
			static nfloat viewYPadding = 0;
			static nfloat viewHeightPadding = 60;

			static nfloat lblSectionHeaderXPadding = 15;
			static nfloat lblSectionHeaderYPadding = 10;
			static nfloat lblSectionHeaderHeightPadding = 20;

           public SubTrackTableSource(SubTrackController controller, Dictionary<string, BuiltTracks[]> tracks)
           {
               this.controller = controller;
               indexedTableItems = tracks;
               keys = tracks.Keys.ToArray();
           }
           public override nint NumberOfSections(UITableView tableView)
           {
               return keys.Length;
           }
           public override nint RowsInSection(UITableView tableview, nint section)
           {
               return indexedTableItems[keys[section]].Length;
           }
           public override UIView GetViewForHeader(UITableView tableView, nint section)
           {

               UIView view = new UIView(new CGRect(viewXPadding, viewYPadding, tableView.Frame.Width, viewHeightPadding))
               {
					BackgroundColor = UIColor.FromRGB(230, 230, 230),
                   AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
               };
               UILabel lblSectionHeader = new UILabel(new CGRect(lblSectionHeaderXPadding, lblSectionHeaderYPadding, tableView.Frame.Width, lblSectionHeaderHeightPadding))
               {
                   AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                   BackgroundColor = UIColor.Clear,
                   TextColor = UIColor.FromRGB(139, 139, 139),
                   Font = UIFont.SystemFontOfSize(20),
                   Text = keys[section]
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
               SubTrackTableCell  cell = tableView.DequeueReusableCell(cellIdentifier) as SubTrackTableCell ;
               if (cell == null) cell = new SubTrackTableCell (cellIdentifier);
               var tracks = indexedTableItems[keys[indexPath.Section]][indexPath.Row];

               var menuText = tracks.name.Split('|');
               if (menuText.Length > 1)
               {
                   cell.UpdateCell(tracks);
               }
               else
               {
                   cell.UpdateCell(tracks);
               }

               if (cell.lblTrackName.Text == controller.preSelectedSubTrackName)
               {
                   cell.btncheck.SetBackgroundImage(UIImage.FromBundle(AppTheme.FLcheckedBox), UIControlState.Normal);
               }
               else
               {
                   cell.btncheck.SetBackgroundImage(UIImage.FromBundle(AppTheme.FLUncheckedBox), UIControlState.Normal);
               }

               cell.SelectionStyle = UITableViewCellSelectionStyle.None;
               cell.Accessory = UITableViewCellAccessory.None;
               return cell;
           }

           public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
           {
               return 50;
           }

       }

       public class SubTrackTableCell : UITableViewCell
       {
           public UILabel lblTrackName;
           public UILabel lblFirstTime;
           public UILabel lblTopTime;
           public UILabel lblBottomTime;
           public UILabel lblRoom;
           public UIButton btncheck;
           public static Action<BuiltTracks> subTrackHandler;
           public BuiltTracks tracksData;

			static nfloat btncheckXPadding = 40;
			static nfloat btncheckYPadding = 15;
			static nfloat btncheckWidhtPadding = 30;
			static nfloat btncheckHeightPadding = 30;

			static nfloat lblTrackNameXPadding = 15;
			static nfloat lblTrackNameYPadding = 10;
			static nfloat lblTrackNameWidthPadding = 15;
			static nfloat lblTrackNameHeightPadding = 20;

           public SubTrackTableCell(NSString cellId)
               : base(UITableViewCellStyle.Default, cellId)
           {
               lblTrackName = new UILabel()
               {
                   Font = UIFont.BoldSystemFontOfSize(17),
                   BackgroundColor = UIColor.Clear,
                   AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
               };

               btncheck = new UIButton()
               {
                   AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
               };


               btncheck.TouchUpInside += delegate
               {
                   btncheck.Selected = (!btncheck.Selected);
                   if (subTrackHandler != null)
                   {
                       subTrackHandler(this.tracksData);
                   }
               };
               ContentView.Add(lblTrackName);
               ContentView.Add(btncheck);
           }

           public void UpdateCell(BuiltTracks builtTracks)
           {
               this.tracksData = builtTracks;
               lblTrackName.Text = builtTracks.name;
           }

           public override void LayoutSubviews()
           {
               base.LayoutSubviews();
               btncheck.Frame = new CGRect(ContentView.Frame.Width - btncheckXPadding, ContentView.Frame.Height / 2 - btncheckYPadding, btncheckWidhtPadding, btncheckHeightPadding);
               lblTrackName.Frame = new CGRect(lblTrackNameXPadding, ContentView.Frame.Height / 2 - lblTrackNameYPadding, btncheck.Frame.X - lblTrackNameWidthPadding, lblTrackNameHeightPadding);


           }
       }


    }


}