using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using CommonLayer;
using ConferenceAppiOS.Helpers;
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS
{
    public class Settingscontroller : BaseViewController
    {
        List<string> dataSource;
        UITableView settingTableView;
        public Settingscontroller()
        {
            dataSource = new List<string> {
				AppTheme.AppSettingsText,
			};

            var user = AppSettings.ApplicationUser;
            if (user != null)
            {
                dataSource.Add(AppTheme.LogoutText);
            }

            View.BackgroundColor = AppTheme.ASBackgroundColor;

            settingTableView = new UITableView();
            settingTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            settingTableView.Frame = new CGRect(View.Frame.X, View.Frame.Y, View.Frame.Size.Width, View.Frame.Size.Height);
            settingTableView.TableFooterView = new UIView();
            settingTableView.Source = new SettingscontrollerSource(dataSource);
            View.AddSubview(settingTableView);
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            settingTableView.Frame = new CGRect(0, 0, View.Frame.Size.Width, View.Frame.Size.Height);
        }
    }

    public class SettingscontrollerSource : UITableViewSource
    {
        List<string> menuList;
        public NSIndexPath selectedIndex;

        NSString cellIdentifier = new NSString("SettingsCell");
        public SettingscontrollerSource(List<string> items)
        {
            menuList = items;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return menuList.Count;
        }


        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = null;
            cell = tableView.DequeueReusableCell(cellIdentifier) as UITableViewCell;
            if (cell == null) cell = new UITableViewCell();
            string str = menuList[indexPath.Row];
            cell.TextLabel.Text = menuList[indexPath.Row];
            cell.TextLabel.TextColor = AppTheme.ASBlueColortext;
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 44;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            var str = menuList[indexPath.Row];
            if (str != null)
            {
                AppDelegate.instance().rootViewController.popOverController.Dismiss(true);
                if (str == AppTheme.LogoutText)
                {
                    var alert = new UIAlertView("", AppTheme.SureToLogoutText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alert.Clicked += (sender, args) =>
                    {
                        if (args.ButtonIndex == 1)
                        {
                            DataManager.UpdateDataOnLogout(AppDelegate.Connection).ContinueWith(t =>
                            {
                                AppSettings.ApplicationUser = null;
                                DataManager.SetCurrentUser(null);
                                AppSettings.MySessionIds = null;
                                AppSettings.NewSurveyCount = 0;

                                InvokeOnMainThread(() =>
                                {
                                    NSUserDefaults.StandardUserDefaults.SetInt(0, AppSettings.SurveyCountKey);
                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.USER_LOGGED_OUT, null);
                                });
                            });
                        }
                    };
                    alert.Show();
                }
                else if (str == AppTheme.AppSettingsText)
                {
                    AppSettingsViewController appSettingsViewController = new AppSettingsViewController(new CGRect(0, 0, 878, 651));
                    AppDelegate.instance().rootViewController.openInDialougueView(appSettingsViewController, DialogAlign.center);
                }
            }
        }
      
        }
    }



