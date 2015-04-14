using System;
using CoreGraphics;

using CoreFoundation;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Linq;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using CommonLayer;
using ConferenceAppiOS.Helpers;
namespace ConferenceAppiOS
{
    public class SpeakersControllerSource : UITableViewSource
    {
        Dictionary<string, List<BuiltSpeaker>> items;
        string[] keys;
        NSString cellIdentifier = new NSString("TableCell");
		public NSIndexPath selectedIndex;

        public SpeakersControllerSource(Dictionary<string, List<BuiltSpeaker>> items)
        {
            keys = items.Keys.ToArray();
            this.items = items;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return keys.Length;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return items[keys[section]].Count;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, 1))
            {
				BackgroundColor = UIColor.Clear,//AppTheme.SPCcellSectionBGColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            var separator = new UIView(new CGRect(30, 0, view.Frame.Width - 30, 1))
            {
				BackgroundColor = AppTheme.SPCcellLineviewColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleMargins | UIViewAutoresizing.FlexibleWidth
            };
            view.AddSubview(separator);
            return view;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SpeakerCell cell = tableView.DequeueReusableCell(cellIdentifier) as SpeakerCell;
            if (cell == null) cell = new SpeakerCell(cellIdentifier);
            var speaker = items[keys[indexPath.Section]][indexPath.Row];
            cell.UpdateCell(speaker, indexPath.Row == 0 ? speaker.first_name[0].ToString() : String.Empty);
            
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 50;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                selectedIndex = indexPath;

                var speaker = items[keys[indexPath.Section]][indexPath.Row];
                if (speaker != null)
                {

	                    SpeakerDetailController vc = new SpeakerDetailController(speaker);
	                    AppDelegate.instance().rootViewController.openDetail(vc, null, true);
	                    var title = speaker.first_name + " " + speaker.last_name;
                        var config = AppDelegate.instance().config;
                        AppDelegate.instance().speakerTwtter = config.social.twitter.speaker_tweet_text;
	                    var text=AppDelegate.instance().speakerTwtter.Replace("{name}", title);
	                    AppDelegate.instance().speakertwitterText = text;
                }
            }

            catch { }
        }

        public void UpdateSource(Dictionary<string, List<BuiltSpeaker>> items)
        {
            keys = items.Keys.ToArray();
            this.items = items;
        }
    }

    public class SpeakerCell : UITableViewCell
    {
        UILabel nameInitialLabel, nameLabel, companyLabel;
        public SpeakerCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = AppTheme.SPCcellBackColor;

            var selectedBackgroundView = new UIView();
            selectedBackgroundView.BackgroundColor = AppTheme.SPCcellSelectedbackgroundColor;
            SelectedBackgroundView = selectedBackgroundView;

            nameLabel = new UILabel()
            {
                TextColor = AppTheme.SPCcellNameTextColor,
                HighlightedTextColor = AppTheme.SPCcellNameHighlightedColor,
                BackgroundColor = AppTheme.SPCcellNameBackColor,
                Font = AppTheme.SPCnameFont
            };

            nameInitialLabel = new UILabel()
            {
                TextColor =AppTheme.SPCcellInitialNameTextColor,
                HighlightedTextColor = AppTheme.SPCcellInitialNameHighlghtedColor,
                BackgroundColor = AppTheme.SPCcellInitialNameBackColor,
                Font = AppTheme.SPCinitialNameFont,
            };

            companyLabel = new UILabel()
            {
                TextColor = AppTheme.SPCcomapnyNameTextColor,
                HighlightedTextColor = AppTheme.SPCcellComapnyNameHighlghtedColor,
                BackgroundColor = AppTheme.SPCcellCompanyNameBackColor,
                Font = AppTheme.SPCcompanyNameFont,
                TextAlignment = UITextAlignment.Right
            };
					
            ContentView.Add(nameLabel);
            ContentView.Add(companyLabel);
            ContentView.Add(nameInitialLabel);
        }

        public void UpdateCell(BuiltSpeaker speaker, string nameInitial)
        {
            nameLabel.Text = speaker.first_name + " " + speaker.last_name;
            companyLabel.Text = speaker.company_name;
            nameInitialLabel.Text = nameInitial;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ContentView.Frame = new CGRect(100, 0, ContentView.Frame.Width - 100, ContentView.Frame.Height);
			SelectedBackgroundView.Frame = ContentView.Frame;
            nameLabel.Frame = new CGRect(20, 0, 200, ContentView.Frame.Height);
            nameInitialLabel.Frame = new CGRect(-70, 0, 30, ContentView.Frame.Height);
            companyLabel.Frame = new CGRect(ContentView.Frame.Width - 250, 0, 200, ContentView.Frame.Height);
        }


        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (Selected)
            {
                ContentView.BackgroundColor = AppTheme.SPCcellSelectedbackgroundColor;
            }
        }
    }

    #region--Table--
    public class SpeakersTable : UITableView
    {
		static nfloat sepLeftInset = 100;
        public SpeakersTable()
        {   
            SeparatorInset = new UIEdgeInsets(0, sepLeftInset, 0, 0);
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            BackgroundColor = AppTheme.SPCspeakersTableBackColor;
            TableFooterView = new UIView();
        }
    }
    #endregion
}