using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreFoundation;
using Foundation;
using UIKit;

using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS;
using ConferenceAppiOS.Helpers;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;
using CoreGraphics;

namespace ConferenceAppiOS
{
    public class IntroViewExplore : BaseViewController
    {
        UITableView introTableView;

        List<BuiltIntro> lstIntroData; public UIView borderBottom;
        CGRect _rect;
        public LayoutEnum positionInParentView { get; set; }
        public Action actionReloadIntrotable;
        public IntroViewExplore(CGRect rect)
        {
            _rect = rect;
            View.Frame = _rect;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            actionReloadIntrotable = ReloadIntrodata;

            borderBottom = new UIView();
            borderBottom.BackgroundColor = AppTheme.WHBorderColor;
            introTableView = new UITableView();
            introTableView.ScrollEnabled = false;
            introTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            lstIntroData = AppSettings.BuiltIntroList;
            introTableView.Source = new introTableSource(this, lstIntroData);
            introTableView.ReloadData();

            View.AddSubview(introTableView);
        }
        public override void ViewWillLayoutSubviews()
        {
            introTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            borderBottom.Frame = new CGRect(0, AppTheme.SectionheaderHeight, introTableView.Frame.Width, AppTheme.sectionBottomBorderHeight);
        }
        public void ReloadIntrodata()
        {
            DataManager.GetListOfIntroData(AppDelegate.Connection).ContinueWith(t =>
            {
                var previousIntro = AppSettings.BuiltIntroList.FirstOrDefault();
                AppSettings.BuiltIntroList = t.Result;
                lstIntroData = AppSettings.BuiltIntroList;

                try
                {
                    var imgUrl = String.Empty;
                    if (previousIntro != null && previousIntro.bg_image != null)
                    {
                        imgUrl = previousIntro.bg_image.url;
                    }

                    var result = t.Result.FirstOrDefault();
                    if (result != null && result.bg_image != null && imgUrl != result.bg_image.url)
                    {
                        var imageData = NSData.FromUrl(NSUrl.FromString(result.bg_image.url));
                        AppSettings.WebViewImageString = (NSString)imageData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                    }
                }
                catch { }

                InvokeOnMainThread(() =>
                   {
                       if (introTableView.Source != null)
                       {
                           (introTableView.Source as introTableSource).UpdateSource(this, lstIntroData);
                           introTableView.ReloadData();
                       }
                       else
                       {
                           introTableView.Source = new introTableSource(this, lstIntroData);
                           introTableView.ReloadData();
                       }
                   });
            });
        }
    }
    public class introTableSource : UITableViewSource
    {
        IntroViewExplore _introViewExploreObj;
        List<BuiltIntro> _lstIntroData;
        NSString introCellIdentifier = new NSString("Intro");
		static nfloat sectionHeaderYPadding = 15;
        public introTableSource(IntroViewExplore introViewExploreObj, List<BuiltIntro> lstIntroData)
        {
            _lstIntroData = lstIntroData;
            _introViewExploreObj = introViewExploreObj;
        }
        public void UpdateSource(IntroViewExplore introViewExploreObj, List<BuiltIntro> lstIntroData)
        {

            _lstIntroData = lstIntroData;
            _introViewExploreObj = introViewExploreObj;
        }
        public override nint NumberOfSections(UITableView tableView)
        {
            return AppTheme.noOfsectionsForIntroTable;
        }
        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return AppTheme.announcementText;
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return AppTheme.SectionheaderHeight;
        }
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                if (_lstIntroData != null)
                {
                    return 1;
                }
            }
            return 0;
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                return AppTheme.announceMentCellHeight;
            }
            return 0;
        }
        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionheaderHeight))
            {
                BackgroundColor = AppTheme.SectionHeaderBackColor,
            };
            view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            if (_introViewExploreObj != null)
            {
                view.AddSubview(_introViewExploreObj.borderBottom);
            }

            if (section == 0)
            {
                UILabel lblSectionHeader = new UILabel(new CGRect(AppTheme.sectionheaderTextLeftPadding, 0, view.Frame.Width, view.Frame.Height))
                {
                    Text = TitleForHeader(tableView, section),
                    TextColor = AppTheme.SpeakerSessionTitleColor,
                    Font = AppFonts.ProximaNovaRegular(18),
                };
                view.AddSubview(lblSectionHeader);
            }
            return view;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AnnouncementCell cell = null;
            if (indexPath.Section == 0)
            {
                cell = tableView.DequeueReusableCell(introCellIdentifier) as AnnouncementCell;
                if (cell == null) cell = new AnnouncementCell(introCellIdentifier);
                cell.RemoveAllViews();
                int obj = _lstIntroData.Count();
                for (int j = 0; j < obj; j++)
                {
                    BuiltIntro builtIntro = _lstIntroData.ElementAt(j);
                    cell.AddHtmlString(builtIntro);
                }
            }
            return cell;
        }
    }
}