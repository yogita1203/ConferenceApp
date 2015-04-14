using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using CoreAnimation;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    [Register("SpeakerDetailController")]
    public class SpeakerDetailController : BaseViewController
    {
        string NAVIGATION_TITLE = AppTheme.SPspeakerNavigationText;
		static nfloat leftMargin = 30;
		static nfloat NAVIGATION_TITLE_HEIGHT = 30;
		static nfloat NAVIGATION_TITLE_WIDTH = 230;
		static nfloat backButtonVerticalOffset = -60;
		static nfloat BarItemFlexibleSpaceWidth = 10;
		static nfloat LabelsMargin = 10;
        BuiltSpeaker speaker;
        UITableView tableView;
        public SpeakerDetailController(BuiltSpeaker speaker)
        {
            this.speaker = speaker;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = AppTheme.SPspeakerDeatailBackColor;

            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(0, backButtonVerticalOffset), UIBarMetrics.Default);

            NavigationItem.TitleView = new UILabel(new CGRect(0, 0, NAVIGATION_TITLE_WIDTH, NAVIGATION_TITLE_HEIGHT))
            {
                Text = NAVIGATION_TITLE,
                TextAlignment = UITextAlignment.Center,
                Font = AppTheme.SPnavigationTitleFont,
                TextColor = AppTheme.SPspeakerDetailsNavigationTitle
            };

            tableView = new UITableView();

            View.AddSubviews(tableView);
            SetFooter();

            DataManager.GetSpeakerSessions(AppDelegate.Connection, speaker.session.Select(p => p.session_id).ToList()).ContinueWith(t =>
            {
                InvokeOnMainThread(() =>
                {
                    tableView.Source = new SpeakerDataSource(this, t.Result);
                    tableView.ReloadData();
                });
            });

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.speaker_detail, Helper.ToDateString(DateTime.Now));
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (tableView.TableHeaderView == null)
                SetHeader();
        }
        
        private void SetHeader()
        {
            nfloat height = 0;
            nfloat topMargin = 20;
            nfloat rightMargin = 60;
            nfloat descriptionTopMargin = 10;

			nfloat yPosition = 0;

            UIView header = new UIView(); 
            header.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            header.AutosizesSubviews = true;

            var headerDetail = new UIView();
            headerDetail.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            headerDetail.AutosizesSubviews = true;

			if (speaker.full_name != null || speaker.first_name != null || speaker.last_name != null) {
				var speakerName = new UILabel(new CGRect(leftMargin, topMargin, View.Frame.Width - rightMargin, 0))
				{
					TextColor = AppTheme.SPspeakerNameColor,
					Font = AppTheme.SPspeakerNameFont,
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth
				};

				string attributedname  = string.IsNullOrEmpty (speaker.full_name) ? string.Format ("{0} {1}", speaker.first_name, speaker.last_name) : string.Empty;
				speakerName.AttributedText = AppFonts.IncreaseLineHeight(attributedname,speakerName.Font,speakerName.TextColor);
				speakerName.SizeToFit();
				speakerName.Frame = new CGRect(leftMargin, topMargin, View.Frame.Width - rightMargin, speakerName.Frame.Height);
				yPosition = speakerName.Frame.Bottom;
				headerDetail.AddSubview(speakerName);
				height += topMargin + speakerName.Frame.Height;
			}
           
			if (speaker.company_name != null && speaker.company_name.Length > 0) {
				var companyName = new UILabel(new CGRect(leftMargin, yPosition + LabelsMargin, View.Frame.Width - rightMargin, 0))
				{
					Text = speaker.company_name,
					TextColor = AppTheme.SPcompanyNameColor,
					Font = AppTheme.SPcompanyNameFont,
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth
				};

				companyName.SizeToFit();
				companyName.Frame = new CGRect(leftMargin, yPosition + LabelsMargin, View.Frame.Width - rightMargin, companyName.Frame.Height);
				yPosition = companyName.Frame.Bottom;
				headerDetail.AddSubview(companyName);
				height += companyName.Frame.Height;
			}

			if (speaker.job_title != null && speaker.job_title.Length > 0) {

				var job_title = new UILabel(new CGRect(leftMargin,  yPosition + LabelsMargin/2, View.Frame.Width - rightMargin, 0))
				{
					Text = speaker.job_title,
					TextColor = AppTheme.SPjobTitleColor,
					Font = AppTheme.SPjobTitleFont,
					AutoresizingMask = UIViewAutoresizing.FlexibleWidth
				};

				job_title.SizeToFit();
				job_title.Frame = new CGRect(leftMargin, yPosition + LabelsMargin/2, View.Frame.Width - rightMargin, job_title.Frame.Height);
				yPosition = job_title.Frame.Bottom;
				height += job_title.Frame.Height;
				headerDetail.AddSubview(job_title);

			}

			if(speaker.bio != null && speaker.bio.Length > 0){
				var description = new UILabel(new CGRect(leftMargin, yPosition + descriptionTopMargin, View.Frame.Width - rightMargin, 0))
				{
					Lines = 0,
					TextAlignment = UITextAlignment.Left,
					LineBreakMode = UILineBreakMode.WordWrap,
					TextColor = AppTheme.SPdescriptionColor,
					Font = AppTheme.SPdescriptionFont
				};
				description.AttributedText = AppFonts.IncreaseLineHeight(speaker.bio,description.Font,description.TextColor);
                var h = Helper.getTextHeight(description.Text, description.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, description.Font, description);
				description.Frame = new CGRect(leftMargin, yPosition + descriptionTopMargin, View.Frame.Width - rightMargin, h);

				height += description.Frame.Height + descriptionTopMargin+descriptionTopMargin*3;
				headerDetail.AddSubview(description);
			}

			header.Frame = new CGRect(0, 0, View.Frame.Width, height + (descriptionTopMargin*2));
            tableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
			headerDetail.Frame = new CGRect(0, 0, View.Frame.Width, height + (descriptionTopMargin*2));

            header.AddSubviews(headerDetail);
            tableView.TableHeaderView = header;
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);
        }

        private void SetFooter()
        {
            UIView footer = new UIView(CGRect.Empty);
            tableView.TableFooterView = footer;
        }

        public class SpeakerDataSource : UITableViewSource
        {
			static nfloat rowHeight = 60;
			public NSIndexPath selectedIndex;

            SpeakerDetailController speakersViewController;
            List<BuiltSessionTime> items;
            NSString cellIdentifierSpeaker = new NSString("SpeakerCell");
            public SpeakerDataSource(SpeakerDetailController speakerController, List<BuiltSessionTime> items)
            {
                speakersViewController = speakerController;
                this.items = items;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return items.Count;
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
				return AppTheme.SectionHeight;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return rowHeight;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                nfloat sepLeftMargin = 15;
                UILabel lblSectionHeader;
                if (items != null && items.Count> 0)
                {
					UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
                    {
                        AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                        AutosizesSubviews = true,
						BackgroundColor = AppTheme.SPSectionBGColor
                    };
                    lblSectionHeader = new UILabel(new CGRect(sepLeftMargin, 0, tableView.Frame.Width - (sepLeftMargin * 2), view.Frame.Height));
                    if (items.Count > 1)
                    {
                       
                            lblSectionHeader.BackgroundColor = AppTheme.SPheaderlabelbackColor;
                            lblSectionHeader.TextColor = AppTheme.SPSectionTextColor;
                            lblSectionHeader.Text = items.Count + " " + "Sessions";
                            lblSectionHeader.Font = AppTheme.SPlabelSectionFont;
                            lblSectionHeader.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                    }
                    else
                    {
                        lblSectionHeader.BackgroundColor = AppTheme.SPheaderlabelbackColor;
                        lblSectionHeader.TextColor = AppTheme.SPSectionTextColor;
                        lblSectionHeader.Text = items.Count + " " + "Session";
                        lblSectionHeader.Font = AppTheme.SPlabelSectionFont;
                        lblSectionHeader.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                    }
					

                    var topLine = new UIView(new CGRect(sepLeftMargin, 0, view.Frame.Width, 1)) { BackgroundColor = AppTheme.SPspeakerDetailSeparatorColor };
                    var bottomLine = new UIView(new CGRect(sepLeftMargin, view.Frame.Bottom, view.Frame.Width, 1)) { BackgroundColor = AppTheme.SPspeakerDetailSeparatorColor };

                    view.AddSubviews(lblSectionHeader, topLine, bottomLine);
                    return view;
                }
                else
                {
                    return new UIView(new CGRect(0, 0, 0, 0));
                }

                
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                SpeakerSessionsCell cell = tableView.DequeueReusableCell(cellIdentifierSpeaker) as SpeakerSessionsCell;
                if (cell == null) cell = new SpeakerSessionsCell(cellIdentifierSpeaker);
                var item = items[indexPath.Row];
                var tracks=AppSettings.AllTracks;
                cell.UpdateCell(item,tracks);
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
				selectedIndex = indexPath;

                var session = items[indexPath.Row];
                SessionDetailController vc = new SessionDetailController(session);
                AppDelegate.instance().rootViewController.openDetail(vc, speakersViewController, false);
            }
        }
    }
}