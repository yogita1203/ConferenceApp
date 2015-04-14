using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using CommonLayer;
using CommonLayer.Entities.Built;
using System.Linq;
using ConferenceAppiOS.Helpers;
using BigTed;
using BuiltSDK;

namespace ConferenceAppiOS
{
    public class AppSettingsViewController : BaseViewController
    {
		static nfloat lineViewY = 44;

        nfloat lineViewHeight = AppTheme.APSseparatorBorderWidth;
		static nfloat closeButtonWidth = 44;
        static nfloat closeButtonHeight = 44;
		static nfloat headerheight = 44;
        string titleText = AppTheme.AppSettingsText;

		static nfloat TableViewWidth = 360;
        UILabel titleLabel;
        UIButton closeButton;
        LineView horizontalLine, verticalLine;
        Dictionary<string, List<Submenus>> dataSource;
        UITableView settingsListTableView;
        public WebViewController webView;

        public AppSettingsViewController(CGRect rect)
        {
            View.Frame = rect;

            View.BackgroundColor = AppTheme.ASBackgroundColor;

            horizontalLine = new LineView(lineViewY, lineViewHeight, View);

            verticalLine = new LineView(new CGRect(TableViewWidth, lineViewY, lineViewHeight, View.Frame.Size.Height - lineViewY));

            titleLabel = new UILabel
            {
                TextColor = AppTheme.ASCellTextColor,
                Font = AppTheme.APSTitleLabelFont,
                BackgroundColor = AppTheme.APSBackgroundColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = new NSString(titleText),
                TextAlignment = UITextAlignment.Center,
                HighlightedTextColor=AppTheme.ASCellTextColor,
            };


            DataManager.GetSettingsMenu(AppDelegate.Connection).ContinueWith(t =>
            {
                List<BuiltSettingsMenu> menus = t.Result;
                List<Submenus> temp = new List<Submenus>();
                

                List<BuiltSettingsMenu> myMenus = new List<BuiltSettingsMenu>();
                try
                {
                    for (int i = 0; i < menus.Count; i++)
                    {
							for (int j = 0; j < menus[i].menu.Count; j++)
                        {
								for (int k = 0; k < menus[i].menu[j].sub_menu.Count; k++)
                            {
									if (menus[i].menu[j].sub_menu[k].name.ToLower() == "logout")
                                {
										menus[i].menu[j].sub_menu.RemoveAt(k);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch 
                {}

                int ind = 0;
                for (int i = 0; i < menus.Count; i++)
                {
                    for (int j = 0; j < menus[i].menu.Count; j++)
                    {
                        for (int k = 0; k < menus[i].menu[j].sub_menu.Count; k++)
                        {
                            for (int l = 0; l < menus[i].menu[j].sub_menu[k].link_group.Count; l++)
                            {
                                if (menus[i].menu[j].sub_menu[k].link_group[l].link.ToLower() == "{push-switch}")
                                {
                                    ind = l;
                                    break;
                                }
                            }
                                
                        }
                    }
                }

					dataSource = menus.SelectMany(p => p.menu).OrderBy(p => p.order).GroupBy(p => p.section_name).ToDictionary(p => p.Key, p => p.SelectMany(q => q.sub_menu).ToList());

                InvokeOnMainThread(() =>
                {
                    settingsListTableView.Source = new SettingsViewControllerSource(this, dataSource);
                    settingsListTableView.ReloadData();
                    int row = 0;
                    int section = 1;

                    if (menus.Count > 0)
                    {
                        settingsListTableView.SelectRow(NSIndexPath.FromRowSection(ind , section), false, UITableViewScrollPosition.None);
                        string str = Helper.getSettingsLink(dataSource[dataSource.Keys.ToArray()[section]][row].link_group);
                        if (!Helper.IsConnectedToInternet())
                        {
                            new UIAlertView("", AppTheme.APSInternetErrorText, null, AppTheme.DismissText).Show();
                            return;
                        }
                        else if (!str.Contains("{"))
                        {
                            webView.loadRequest(str);
                        }
                    }
                });
            });

            settingsListTableView = new UITableView();
            settingsListTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            settingsListTableView.Frame = new CGRect(0, lineViewY + lineViewHeight, TableViewWidth, View.Frame.Size.Height - (lineViewY + lineViewHeight));

            var footer = new UIView(new CGRect(0, 0, settingsListTableView.Frame.Width, 30));
            var versionNumber = new UILabel(new CGRect(0,0, footer.Frame.Width, 30));
            versionNumber.Center = footer.Center;

            versionNumber.TextColor = AppTheme.ASversionNumber;
            versionNumber.TextAlignment = UITextAlignment.Center;
			var info= NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            string strVersion=info.ToString();

			var build = NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"];
			string strBuild = build.ToString ();

			versionNumber.Text = AppTheme.ASstaticVersion + strVersion + " ("+strBuild+")";
            footer.AddSubviews(versionNumber);
            settingsListTableView.TableFooterView=footer;

            closeButton = UIButton.FromType(UIButtonType.Custom);
            closeButton.BackgroundColor = AppTheme.APSCloseBtnBackgroundColor;
            closeButton.SetImage(new UIImage(AppTheme.ASCrossIcon), UIControlState.Normal);
            closeButton.SetImage(new UIImage(AppTheme.ASCrossIcon), UIControlState.Selected);
            closeButton.SetImage(new UIImage(AppTheme.ASCrossIcon), UIControlState.Highlighted);
            closeButton.TouchUpInside += closeButtonClicked;

            webView = new WebViewController("");

            View.AddSubviews(webView.View, titleLabel, verticalLine, horizontalLine, closeButton, settingsListTableView);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.settings, Helper.ToDateString(DateTime.Now));
        }

        void closeButtonClicked(object sender, EventArgs e)
        {
            AppDelegate.instance().rootViewController.closeDialogue();
            AppDelegate.instance().rootViewController.ViewDidLayoutSubviews();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            closeButton.Frame = new CGRect(0, 0, closeButtonWidth, closeButtonHeight);
            titleLabel.Frame = new CGRect(0, 0, View.Frame.Width, headerheight);
            //			horizontalLine
            horizontalLine.Frame = new CGRect(0, headerheight, View.Frame.Width, lineViewHeight);
            settingsListTableView.Frame = new CGRect(0, lineViewY + lineViewHeight, TableViewWidth, View.Frame.Size.Height - (lineViewY + lineViewHeight));
            webView.View.Frame = new CGRect(TableViewWidth + lineViewHeight, lineViewY + lineViewHeight, View.Frame.Size.Width - (TableViewWidth + lineViewHeight), View.Frame.Size.Height - (lineViewY + lineViewHeight));
            verticalLine.Frame = new CGRect(TableViewWidth, lineViewY + lineViewHeight, lineViewHeight, View.Frame.Size.Height - (lineViewY + lineViewHeight));
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.settings_menu))
            {
                updateSettingsTableSource();
                return;
            }
        }

        private void updateSettingsTableSource()
        {
            DataManager.GetSettingsMenu(AppDelegate.Connection).ContinueWith(t =>
            {
                List<BuiltSettingsMenu> menus = t.Result;
                List<Submenus> temp = new List<Submenus>();


                List<BuiltSettingsMenu> myMenus = new List<BuiltSettingsMenu>();
                try
                {
                    for (int i = 0; i < menus.Count; i++)
                    {
                        for (int j = 0; j < menus[i].menu.Count; j++)
                        {
                            for (int k = 0; k < menus[i].menu[j].sub_menu.Count; k++)
                            {
                                if (menus[i].menu[j].sub_menu[k].name.ToLower() == "logout")
                                {
                                    menus[i].menu[j].sub_menu.RemoveAt(k);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                { }

                int ind = 0;
                for (int i = 0; i < menus.Count; i++)
                {
                    for (int j = 0; j < menus[i].menu.Count; j++)
                    {
                        for (int k = 0; k < menus[i].menu[j].sub_menu.Count; k++)
                        {
                            for (int l = 0; l < menus[i].menu[j].sub_menu[k].link_group.Count; l++)
                            {
                                if (menus[i].menu[j].sub_menu[k].link_group[l].link.ToLower() == "{push-switch}")
                                {
                                    ind = l;
                                    break;
                                }
                            }

                        }
                    }
                }

                dataSource = menus.SelectMany(p => p.menu).OrderBy(p => p.order).GroupBy(p => p.section_name).ToDictionary(p => p.Key, p => p.SelectMany(q => q.sub_menu).ToList());

                InvokeOnMainThread(() =>
                {
                    settingsListTableView.Source = new SettingsViewControllerSource(this, dataSource);
                    settingsListTableView.ReloadData();
                    int row = 0;
                    int section = 1;

                    if (menus.Count > 0)
                    {
                        settingsListTableView.SelectRow(NSIndexPath.FromRowSection(ind, section), false, UITableViewScrollPosition.None);
                        string str = Helper.getSettingsLink(dataSource[dataSource.Keys.ToArray()[section]][row].link_group);
                        if (!Helper.IsConnectedToInternet())
                        {
                            new UIAlertView("", AppTheme.APSInternetErrorText, null, AppTheme.DismissText).Show();
                            return;
                        }
                        else if (!str.Contains("{"))
                        {
                            webView.loadRequest(str);
                        }
                    }
                });
            });
        }
    }

    public class SettingsViewControllerSource : UITableViewSource
    {
        Dictionary<string, List<Submenus>> menuList;
        List<string> keys;
		static nfloat cellHeight = 46;
		static nfloat switchWidth = 30;
		static nfloat switchHeight = 31;
        AppSettingsViewController controller;
        NSString cellIdentifier = new NSString("SettingsCell");

        UISwitch _pushNotifiactionSwitch;
        UISwitch pushNotifiactionSwitch
        {
            get
            {
                if (_pushNotifiactionSwitch == null)
                {
                    _pushNotifiactionSwitch = new UISwitch()
                    {
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                        OnTintColor = AppTheme.ASCellSelectedBackgroundColor
                    };
                    _pushNotifiactionSwitch.AddTarget((s, e) =>
                    {
                        try
                        {
                            var installation = BuiltInstallation<BuiltInstallationData>.currentInstallation();
                            var token = installation.result.device_token;
                            DataManager.CreateInstallation(token, err =>
                            {
                                if (err != null)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                        _pushNotifiactionSwitch.SetState(!_pushNotifiactionSwitch.On, false);
                                    });
                                }
                            }, !_pushNotifiactionSwitch.On);
                        }
                        catch { }
                    }, UIControlEvent.ValueChanged);
                }
                return _pushNotifiactionSwitch;
            }
        }

        UISwitch _gamingNotifiactionSwitch;
        UISwitch gamingNotifiactionSwitch
        {
            get
            {
                if (_gamingNotifiactionSwitch == null)
                {
                    _gamingNotifiactionSwitch = new UISwitch()
                    {
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                        OnTintColor = AppTheme.ASCellSelectedBackgroundColor
                    };
                    _gamingNotifiactionSwitch.AddTarget((s, e) =>
                    {
                    }, UIControlEvent.ValueChanged);
                }
                return _gamingNotifiactionSwitch;
            }
        }

        public SettingsViewControllerSource(AppSettingsViewController appSettingsViewController, Dictionary<string, List<Submenus>> items)
        {
            controller = appSettingsViewController;
            menuList = items;
            keys = items.Keys.ToList();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
			return menuList[keys[(int)section]].Count;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
			if (menuList[keys[(int)section]].Count == 0)
            {
                return new UIView(new CGRect(0, 0, 0, 0));
            }
			UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
            {
                BackgroundColor = AppTheme.APSmenuSectionBackgroundColor,
            };
            UILabel titleLabel = new UILabel();
			titleLabel.Frame = new CGRect(15, 0, view.Frame.Width - 15, view.Frame.Height);
			titleLabel.Text = keys[(int)section];
            titleLabel.TextColor = AppTheme.ASSectionTitleColor;
            titleLabel.TextAlignment = UITextAlignment.Left;
            titleLabel.BackgroundColor = UIColor.Clear;
            titleLabel.Font = AppTheme.APSSectionTitleFont;
            view.AddSubview(titleLabel);
            return view;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
			return AppTheme.SectionHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = null;
            cell = tableView.DequeueReusableCell(cellIdentifier) as UITableViewCell;
            if (cell == null) cell = new UITableViewCell();
            string str = menuList[keys[indexPath.Section]][indexPath.Row].name;
            cell.TextLabel.Text = str;
			cell.TextLabel.Font = AppFonts.ProximaNovaRegular (18);
            cell.TextLabel.TextColor = AppTheme.ASCellTextColor;
            string link = Helper.getSettingsLink(menuList[keys[indexPath.Section]][indexPath.Row].link_group);
            if (link != "#{switch}" && !link.Contains("{"))
            {
                cell.TextLabel.HighlightedTextColor = AppTheme.ASCellSelectedTextColor;
                UIView myBackView = new UIView();
                myBackView.BackgroundColor = AppTheme.ASCellSelectedBackgroundColor;
                cell.SelectedBackgroundView = myBackView;
            }
            else
            {
                cell.TextLabel.HighlightedTextColor = AppTheme.ASCellSelectedTextColor;
                UIView myBackView = new UIView();
                myBackView.BackgroundColor = AppTheme.ASBackgroundColor;
                cell.SelectedBackgroundView = myBackView;
                if (str == "Push Notification")
                {
                    cell.TextLabel.HighlightedTextColor = AppTheme.ASCellTextColor;
                    cell.TextLabel.Frame = new CGRect(0, 0, cell.TextLabel.Frame.Width - switchWidth, cell.TextLabel.Frame.Height);
                    pushNotifiactionSwitch.Frame = new CGRect(cell.ContentView.Frame.Size.Width - switchWidth, (cell.ContentView.Frame.Height - switchHeight) / 2, switchWidth, switchHeight);
                    cell.ContentView.AddSubview(pushNotifiactionSwitch);

                    try
                    {
                        var installation = BuiltInstallation<BuiltInstallationData>.currentInstallation();
                        pushNotifiactionSwitch.On = !installation.result.disable;
                        pushNotifiactionSwitch.Enabled = true;
                    }
                    catch
                    {
                        pushNotifiactionSwitch.On = false;
                        pushNotifiactionSwitch.Enabled = false;
                    }
                }
                else if (str == "Gamification Features")
                {
                    cell.TextLabel.Frame = new CGRect(0, 0, cell.TextLabel.Frame.Width - switchWidth, cell.TextLabel.Frame.Height);
                    gamingNotifiactionSwitch.Frame = new CGRect(cell.ContentView.Frame.Size.Width - switchWidth, (cell.ContentView.Frame.Height - switchHeight) / 2, switchWidth, switchHeight);
                    gamingNotifiactionSwitch.On = true;
                    cell.ContentView.AddSubview(gamingNotifiactionSwitch);
                }

            }
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return cellHeight;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (!Helper.IsConnectedToInternet())
            {
                new UIAlertView("", "Check your connection and try again.", null, "Dismiss").Show();
                return;
            }

            string str = Helper.getSettingsLink(menuList[keys[indexPath.Section]][indexPath.Row].link_group);
            if (str != "#{switch}" && !str.Contains("{"))
            {
                controller.webView.loadRequest(str);
            }
            else
            {
                //tableView.CellAt(indexPath).TextLabel.Enabled= false;
                tableView.DeselectRow(indexPath, true);
            }
        }
  
    }
}

