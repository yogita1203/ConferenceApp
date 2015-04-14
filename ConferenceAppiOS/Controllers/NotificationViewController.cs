using System;
using UIKit;
using CommonLayer;
using System.Collections.Generic;
using CoreGraphics;
using CommonLayer.Entities.Built;
using Foundation;
using ConferenceAppiOS.Helpers;
using System.Linq;

namespace ConferenceAppiOS
{
    public class NotificationViewController : BaseViewController
    {
		static nfloat lineViewY = 44;

        nfloat lineViewHeight = AppTheme.NVSeparatorBorderWidth;
		static nfloat closeButtonWidth = 44;
		static nfloat closeButtonHeight = 44;
		static nfloat headerheight = 44;
        string titleText = AppTheme.NVnotificationtitleText;
        UIButton closeButton;
        UILabel titleLabel;
        UITableView notificationTable;
        List<BuiltEventNotifications> notificationSource;
        LineView horizontalLine;
        public NotificationViewController(CGRect rect)
        {
            View.Frame = rect;
        }

        public override void LoadView()
        {
            base.LoadView();

            View.BackgroundColor = AppTheme.NVbackgroundColor;

            horizontalLine = new LineView(lineViewY, lineViewHeight, View);

            titleLabel = new UILabel()
            {
                TextColor = AppTheme.NVtitleColor,
                Font = AppTheme.NVtitleLabelFont,
                BackgroundColor = AppTheme.NVtitleBackgroundColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = new NSString(titleText),
                TextAlignment = UITextAlignment.Center,
            };

            closeButton = UIButton.FromType(UIButtonType.Custom);
            closeButton.BackgroundColor = AppTheme.NVCloseBtnBackColor;
            closeButton.SetImage(new UIImage(AppTheme.NVCrossIcon), UIControlState.Normal);
            closeButton.SetImage(new UIImage(AppTheme.NVCrossIcon), UIControlState.Selected);
            closeButton.SetImage(new UIImage(AppTheme.NVCrossIcon), UIControlState.Highlighted);
            closeButton.TouchUpInside += closeButtonClicked;

            DataManager.GetEventNotifications(AppDelegate.Connection).ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    notificationSource = t.Result.GroupBy(p => p.uid).Select(p => p.First()).ToList();
                    notificationSource = notificationSource.OrderByDescending(p => p.updated_at).ToList();
                    InvokeOnMainThread(() =>
                    {
                        notificationTable.Source = new NotificationDataSource(notificationSource);
                        notificationTable.ReloadData();
                    });
                }
            });

            notificationTable = new UITableView();
            notificationTable.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            notificationTable.Frame = new CGRect(0, lineViewY + lineViewHeight, View.Frame.Size.Width, View.Frame.Size.Height - (lineViewY + lineViewHeight));
            notificationTable.TableFooterView = new UIView();

            View.AddSubviews(notificationTable, titleLabel, horizontalLine, closeButton);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.notification, Helper.ToDateString(DateTime.Now));
        }

        public void RefreshNotifications()
        {
            DataManager.RefreshNotifiactions(AppDelegate.Connection, res =>
            {
                if (res)
                    updateSource();
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.event_notifications))
            {
                updateSource();
            }
        }

        void updateSource()
        {
            DataManager.GetEventNotifications(AppDelegate.Connection).ContinueWith(t =>
            {
                if (t.Result != null)
                {
                    notificationSource = t.Result.GroupBy(p => p.uid).Select(p => p.First()).ToList();
                    notificationSource = notificationSource.OrderByDescending(p => p.updated_at).ToList();
                    InvokeOnMainThread(() =>
                    {
                        (notificationTable.Source as NotificationDataSource).updateSource(notificationSource);
                        notificationTable.ReloadData();
                    });
                }
            });
        }

        void closeButtonClicked(object sender, EventArgs e)
        {
            AppDelegate.instance().rootViewController.menuViewController.ShowNotificationBadge(false);
            AppDelegate.instance().rootViewController.closeDialogue();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            closeButton.Frame = new CGRect(0, 0, closeButtonWidth, closeButtonHeight);
            titleLabel.Frame = new CGRect(0, 0, View.Frame.Width, headerheight);
            horizontalLine.Frame = new CGRect(0, headerheight, View.Frame.Width, lineViewHeight);
            notificationTable.Frame = new CGRect(0, lineViewY + lineViewHeight, View.Frame.Size.Width, View.Frame.Size.Height - (lineViewY + lineViewHeight));
        }

    }

    #region --Menu table source--
    public class NotificationDataSource : UITableViewSource
    {
        nfloat rowHeight = 41;
        nfloat leftMargin = 21;
        nfloat rightMargin = 21;
        nfloat topMargin = 16;
        public NSIndexPath selectedIndex;

        List<BuiltEventNotifications> tableItems;
        NSString cellIdentifier = new NSString("NotificationCell");
        public NotificationDataSource(List<BuiltEventNotifications> items)
        {
            tableItems = items;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            nfloat height = topMargin * 2;

            BuiltEventNotifications builtEventNotifications = tableItems[indexPath.Row];

            if (builtEventNotifications != null)
            {
                NSString descriptionString = new NSString(builtEventNotifications.desc);

                CGSize descriptionSize = descriptionString.StringSize(AppTheme.NVdescriptionFont, new CGSize(tableView.Frame.Width - (leftMargin + rightMargin), 999), UILineBreakMode.WordWrap);
                height += descriptionSize.Height;

                NSString titleString = new NSString(builtEventNotifications.title);

                CGSize titleSize = titleString.StringSize(AppTheme.NVcellTextFont, new CGSize(tableView.Frame.Width - (leftMargin + rightMargin), 999), UILineBreakMode.WordWrap);
                height += titleSize.Height;
            }
            if (height < rowHeight)
                return rowHeight;
            else
                return height;

        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            NotificationCell cell = tableView.DequeueReusableCell(cellIdentifier) as NotificationCell;

            if (cell == null)
                cell = new NotificationCell(cellIdentifier);

            BuiltEventNotifications builtEventNotifications = tableItems[indexPath.Row];
            cell.UpdateCell(builtEventNotifications);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            NotificationCell cell = tableView.CellAt(indexPath) as NotificationCell;
            cell.Selected = false;

            var item = tableItems[indexPath.Row];
            var url = item.url;
            if (url == "vmwareapp://leftmenu/notes")
            {
                if (AppSettings.ApplicationUser != null)
                {
                    AppDelegate.instance().openController(NSUrl.FromString(item.url));
                    AppDelegate.instance().rootViewController.closeDialogue();
                }
                else
                {
                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (s, arg) =>
                    {
                        if (arg.ButtonIndex.ToString() == "1")
                        {
                            AppDelegate.instance().ShowLogin();
                            return;
                        }
                        else
                        { }

                    };
                    alertView.Show();
                }

            }
            else if (url == "vmwareapp://leftmenu/schedule")
            {
                if (AppSettings.ApplicationUser != null)
                {
                    AppDelegate.instance().openController(NSUrl.FromString(item.url));
                    AppDelegate.instance().rootViewController.closeDialogue();
                }
                else
                {
                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (s, arg) =>
                    {
                        if (arg.ButtonIndex.ToString() == "1")
                        {
                            AppDelegate.instance().ShowLogin();
                            return;
                        }
                        else
                        { }

                    };
                    alertView.Show();
                }
            }

            else
            {
                AppDelegate.instance().openController(NSUrl.FromString(item.url));
                AppDelegate.instance().rootViewController.closeDialogue();
            }

        }

        public void updateSource(List<BuiltEventNotifications> items)
        {
            tableItems = items;
        }
    }

    #endregion


}

