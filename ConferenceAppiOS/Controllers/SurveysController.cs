using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.CustomControls;
using ConferenceAppiOS.Helpers;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;

namespace ConferenceAppiOS
{
    [Register("SurveysController")]
    public class SurveysController : BaseViewController
    {
        string TitleString = "Surveys";
		static nfloat HeaderHeight = 64;
        UIColor pageBackground = UIColor.White;
        UITableView surveyTable;
        public SurveysController(CGRect rect)
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

            // Perform any additional setup after loading the view
            View.BackgroundColor = pageBackground;

            surveyTable = new UITableView()
            {
                Frame = new CGRect(0, HeaderHeight, View.Frame.Width, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleRightMargin,
            };

            setHeader();
            surveyTable.TableFooterView = new UIView(CGRect.Empty);

            setTableSource();

            View.AddSubviews(surveyTable);
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.survey, Helper.ToDateString(DateTime.Now));
        }

        private void setTableSource()
        {
            LoadingView.Show(string.Empty);
            DataManager.getMySurveyExtension(AppDelegate.Connection, (t, count) =>
            {
                var result = t;
                AppSettings.NewSurveyCount = 0;
                InvokeOnMainThread(() =>
                    {
                        LoadingView.Dismiss();
                        surveyTable.Source = new SurveyTableSource(result);
                        surveyTable.ReloadData();
                        NSUserDefaults.StandardUserDefaults.SetInt(0, AppSettings.SurveyCountKey);
                        NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.ReloadLeftMenu, null);
                    });
            }, AppSettings.NewSurveyCount); 
        }

        void setHeader()
        {
            TitleHeaderView titleheaderView = new TitleHeaderView(TitleString, true, false, false, false, false, false, false, false)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            titleheaderView.Frame = new CGRect(0, 0, View.Frame.Width, HeaderHeight);
            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
            View.AddSubview(titleheaderView);
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
        }
    }

    internal class SurveyTableSource : UITableViewSource
    {
        List<SurveyExtension> surveys;
        NSString cellIdentifier = new NSString("SurveyCell");
        NSString defaultCellIdentifier = new NSString("DefaultCell");
        string noData = @"There are no surveys to fill out at this time. Please check again later.";
        public NSIndexPath selectedIndex;
        public SurveyTableSource(List<SurveyExtension> surveys)
        {
            this.surveys = surveys;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (surveys.Count > 0)
            {
                SurveyCell cell = tableView.DequeueReusableCell(cellIdentifier) as SurveyCell;
                if (cell == null) cell = new SurveyCell(cellIdentifier);
                var item = surveys[indexPath.Row];
                cell.UpdateCell(item);
                return cell;
            }
            else
            {
                UITableViewCell cell = tableView.DequeueReusableCell(defaultCellIdentifier) as UITableViewCell;
                if (cell == null)
                    cell = new UITableViewCell(UITableViewCellStyle.Default, defaultCellIdentifier);

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.TextLabel.TextAlignment = UITextAlignment.Center;
                cell.TextLabel.Text = noData;
                cell.TextLabel.TextColor = AppTheme.NTnotesCellTitleColor;
                cell.TextLabel.BackgroundColor = UIColor.Clear;
                cell.TextLabel.Font = AppFonts.ProximaNovaRegular(16);
                return cell;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            var survey = surveys[indexPath.Row];
            WebViewController vc = new WebViewController(survey.mobile_url, true, new CGRect(0, 0, 700, 700), survey.name);
            vc.View.Frame = new CGRect(0, 0, 700, 700);
            AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.survey_detail, Helper.ToDateString(DateTime.Now));
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (surveys.Count > 0)
                return surveys.Count;
            else
                return 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return AppTheme.DHcellRowHeight;
        }
    }

    internal class SurveyCell : UITableViewCell
    {
		static nfloat Margin = 15;
		static nfloat TopMargin = 10;
		static nfloat LabelHeight = 20;
        UILabel _titleLabel;
        public UILabel TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel()
                    {
                        TextColor = AppTheme.DHCellTitleColor,
                        HighlightedTextColor = AppTheme.DHHighLightedTextColor,
                        BackgroundColor = AppTheme.DHTitleBackColor,
                        Font = AppTheme.DHtitleLabelFont,
                    };
                }
                return _titleLabel;
            }
        }

        UILabel _descriptionLabel;
        public UILabel DescriptionLabel
        {
            get
            {
                if (_descriptionLabel == null)
                {
                    _descriptionLabel = new UILabel()
                    {
                        TextColor = AppTheme.DHcellDescriptionColor,
                        HighlightedTextColor = AppTheme.DHdescriptionHighLightedTextColor,
                        BackgroundColor = AppTheme.DHDescBackColor,
                        Font = AppTheme.DHdescLabelFont,
                    };
                }
                return _descriptionLabel;
            }
        }

        public SurveyCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.AddSubviews(TitleLabel, DescriptionLabel);

            SelectedBackgroundView = new UIView
            {
                BackgroundColor = AppTheme.DHnotesCellSelectedBackColor
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            TitleLabel.SizeToFit();

            if (!String.IsNullOrWhiteSpace(DescriptionLabel.Text))
            {
                TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin * 2), TitleLabel.Frame.Height);
                DescriptionLabel.Frame = new CGRect(Margin, TitleLabel.Frame.Bottom + (TopMargin / 2), ContentView.Frame.Width - (Margin * 2), LabelHeight);
            }
            else
            {
                TitleLabel.Frame = new CGRect(Margin, 0, ContentView.Frame.Width - (Margin * 2), ContentView.Frame.Height);
                DescriptionLabel.Frame = CGRect.Empty;
            }
        }

        public void UpdateCell(SurveyExtension survey)
        {
            TitleLabel.Text = survey.name;
            DescriptionLabel.Text = survey.session_title;
        }
    }
}