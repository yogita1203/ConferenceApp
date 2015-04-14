using System;
using CoreGraphics;

using CoreFoundation;
using UIKit;
using Foundation;
using CoreData;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using System.Linq;
using CoreAnimation;
using System.Collections.Generic;
using ConferenceAppiOS.Helpers;
using CommonLayer;
using Newtonsoft.Json;
namespace ConferenceAppiOS
{
    public class HandsOnLabsDetailController : BaseViewController
    {
        UITableView tableView;
        const string NAVIGATION_TEXT = "";
		static nfloat handsOnLabsNameLeftMargin = 30;
		static nfloat handsOnLabsnameTopMargin = 20;
		static nfloat handsOnLabsNameRightMargin = 60;
		static nfloat holCategoryLeftMargin = 30;
		static nfloat holCategoryTopMargin = 20;
		static nfloat holCategoryRightMargin = 30;
		static nfloat leftMargin = 30;
		static nfloat holdescriptionAbstractTopMargin = 20;
		static nfloat holDurationLeftMargin = 30;
		static nfloat holDurationTopMargin = 10;
		static nfloat holDurationRightMargin = 30;
		static nfloat lineViewLeftPadding = 30;
		static nfloat lineViewHeight = 1;
		static nfloat lineViewTopMargin = 20;
		static nfloat sepLeftInset = 30;
        List<string> lstspeakerSeparated;

		HOLDetailNewSource hOLDetailNewSource;
        BuiltHandsonLabs builtHandsOnLabs;
        public HandsOnLabsDetailController(BuiltHandsonLabs builtHandsOnLabs)
        {
            this.builtHandsOnLabs = builtHandsOnLabs;
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = AppTheme.HOLpageBackColor;

            base.ViewDidLoad();
			Title = NAVIGATION_TEXT;

            tableView = new UITableView();
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            if(builtHandsOnLabs.speaker_separated !=null)
            {
                lstspeakerSeparated = new List<string>();
                lstspeakerSeparated = builtHandsOnLabs.speaker_separated.Split('|').ToList();
            }
            var dictAnotherSections = GetHOLSectionDetails();
			hOLDetailNewSource = new HOLDetailNewSource (this, lstspeakerSeparated, dictAnotherSections); 
			tableView.Source = hOLDetailNewSource;
            

        }
        private Dictionary<string, string> GetHOLSectionDetails()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(builtHandsOnLabs.session_id))
                result.Add("ID", builtHandsOnLabs.session_id);
            if (!String.IsNullOrEmpty(builtHandsOnLabs.hashtag))
                result.Add("Twitter Hashtag", builtHandsOnLabs.hashtag);
            if (!String.IsNullOrEmpty(builtHandsOnLabs.duration))
                result.Add("Duration", builtHandsOnLabs.duration);
            if (!String.IsNullOrEmpty(builtHandsOnLabs.capacity))
                result.Add("Capacity", builtHandsOnLabs.capacity);
            
            return result;
        }


        void setHeader()
        {
            nfloat height = 0;
            UIView header = new UIView();
            header.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            header.AutosizesSubviews = true;

            var headerDetail = new UIView(); 
            headerDetail.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            headerDetail.AutosizesSubviews = true;
            var handsOnLabsName = new UILabel(new CGRect(handsOnLabsNameLeftMargin, handsOnLabsnameTopMargin, View.Frame.Width - handsOnLabsNameRightMargin, 0))
            {
                Text = builtHandsOnLabs.title,
                TextColor = AppTheme.HOLnameTextColor,
                Font = AppTheme.HOLnameFont,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            handsOnLabsName.SizeToFit();
            handsOnLabsName.Frame = new CGRect(handsOnLabsNameLeftMargin, handsOnLabsnameTopMargin, View.Frame.Width - handsOnLabsNameRightMargin, handsOnLabsName.Frame.Height);
            height += handsOnLabsName.Frame.Height + handsOnLabsnameTopMargin;
            var lineViewTop = new UIView(new CGRect(lineViewLeftPadding, handsOnLabsName.Frame.Bottom + lineViewTopMargin, View.Frame.Width, lineViewHeight))
            {
				BackgroundColor = UIColor.Clear.FromHexString(AppTheme.LineColor,1.0f),
            };

            height += lineViewTopMargin + lineViewTop.Frame.Height;

            var holdescriptionAbstract = new UILabel(new CGRect(leftMargin, lineViewTop.Frame.Bottom + holdescriptionAbstractTopMargin, View.Frame.Width - (leftMargin * 2), 0))
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppTheme.HOLDescTextColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Font = AppTheme.HOLDescTextFont
            };

			holdescriptionAbstract.AttributedText = AppFonts.IncreaseLineHeight (builtHandsOnLabs._abstract, holdescriptionAbstract.Font, holdescriptionAbstract.TextColor);

            var h = Helper.getTextHeight(holdescriptionAbstract.Text, holdescriptionAbstract.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, holdescriptionAbstract.Font, holdescriptionAbstract);
            var rect = holdescriptionAbstract.Frame;
            rect.Height = h;
            holdescriptionAbstract.Frame = rect;
            height += holdescriptionAbstract.Frame.Height + holdescriptionAbstractTopMargin;
            header.Frame = new CGRect(0, 0, View.Frame.Width, height+20);
            tableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            headerDetail.Frame = new CGRect(0, 0, View.Frame.Width, height - 10);

            headerDetail.AddSubviews(handsOnLabsName, lineViewTop,holdescriptionAbstract);
            header.AddSubviews(headerDetail);
            View.AddSubviews(tableView);

			tableView.TableHeaderView = header;
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            tableView.SeparatorInset = new UIEdgeInsets(0, sepLeftInset, 0, 0);
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            SetFooter();

        }
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (tableView.TableHeaderView == null)
                setHeader();
        }
        private void SetFooter()
        {
            UIView footer = new UIView(CGRect.Empty);
            tableView.TableFooterView = footer;
        }
    }

    public class HOLDetailNewSource : UITableViewSource
    {
        Dictionary<string, string> others;
        List<string> speakers;
        string[] keys;
		public NSIndexPath selectedIndex;
        HandsOnLabsDetailController holDetailcontroller;
        NSString cellIdentifierSpeaker = new NSString("SpeakerCell");
        NSString cellIdentifierOther = new NSString("OtherCell");
        public HOLDetailNewSource(HandsOnLabsDetailController holDetail, List<string> speakers, Dictionary<string, string> others)
        {
            holDetailcontroller = holDetail;
            this.speakers = speakers;
            this.others = others;
            keys = others.Keys.ToArray();
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            if (keys.Length == 0)
            {
                if (speakers == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (speakers == null)
                {
                    return keys.Length;
                }
                else
                {
                    return keys.Length + 1;
                }
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {

            if (keys.Length == 0 || section >= keys.Length)
                return speakers.Count;
            else
                return others[keys[section]].Split('|').Length;
        }

       

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
			return AppTheme.SectionHeight;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
			if (keys.Length == 0 || indexPath.Section >= keys.Length) {
				return 60;
			} else {
				return 40;
			}
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            string sectionTitle = null;
            if (keys.Length == 0 || section >= keys.Length)
                sectionTitle = speakers.Count + AppTheme.HOLspeakersTextTitleText;
            else
                sectionTitle = keys[section];

			UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                AutosizesSubviews = true,
				BackgroundColor = AppTheme.HOLheaderBackColor
            };

			UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, tableView.Frame.Width - 30, view.Frame.Height))
            {
                BackgroundColor = UIColor.Clear,
				TextColor = AppTheme.HOLheaderLabelTitleColor,
                Text = sectionTitle,
				Font = AppTheme.HOLheaderLabelFont,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

			var topLine = new UIView(new CGRect(15, 0, view.Frame.Width, 1)) { BackgroundColor = AppTheme.HOLheaderBorderColor };
			var bottomLine = new UIView(new CGRect(15, 29, view.Frame.Width, 1)) { BackgroundColor = AppTheme.HOLheaderBorderColor };

            view.AddSubviews(lblSectionHeader, topLine, bottomLine);
            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (keys.Length == 0 || indexPath.Section >= keys.Length)
            {
                HandsOnLabsDetailSpeakerCell cell = tableView.DequeueReusableCell(cellIdentifierSpeaker) as HandsOnLabsDetailSpeakerCell;
                if (cell == null) cell = new HandsOnLabsDetailSpeakerCell(cellIdentifierSpeaker);
                var item = speakers[indexPath.Row];
                cell.UpdateCell(item);
                return cell;
            }
            else
            {
                SingleRowCell cell = tableView.DequeueReusableCell(cellIdentifierOther) as SingleRowCell;
                if (cell == null) cell = new SingleRowCell(cellIdentifierOther);

                var items = others[keys[indexPath.Section]].Split('|');
                cell.textLabel.Text = items[indexPath.Row];
                return cell;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
			selectedIndex = indexPath;
        }
    }
    public class HandsOnLabsDetailSource : UITableViewSource
    {
       
        List<string> speakers;         
        HandsOnLabsDetailController HandsOnLabsDetailController;
		public NSIndexPath selectedIndex;
        NSString cellIdentifierSpeaker = new NSString("SpeakerCell");
        NSString cellIdentifierOther = new NSString("OtherCell");
        public HandsOnLabsDetailSource(HandsOnLabsDetailController HandsOnLabsDetail, List<string> speakers)
        {
            HandsOnLabsDetailController = HandsOnLabsDetail;
            this.speakers = speakers;
           
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview,nint section)
        {
            return speakers.Count;
        }
        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
			return AppTheme.SectionHeight;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 60;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            string sectionTitle = null;
            if (speakers.Count == 0)
                sectionTitle = AppTheme.HOLnoSpeakersTextTitleText;
            else if (speakers.Count == 1)
                sectionTitle = AppTheme.HOLsimpleSpeakersTitleText;
            else
                sectionTitle = speakers.Count + AppTheme.HOLspeakersTextTitleText;

			UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                AutosizesSubviews = true,
				BackgroundColor = AppTheme.HOLheaderBackColor
            };

			UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, tableView.Frame.Width - 30, view.Frame.Height))
            {
                BackgroundColor = UIColor.Clear,
				TextColor = AppTheme.HOLheaderLabelTitleColor,
                Text = sectionTitle,
				Font = AppTheme.HOLheaderLabelFont,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

			var topLine = new UIView(new CGRect(15, 0, view.Frame.Width, 1)) { BackgroundColor = AppTheme.HOLheaderBorderColor };
			var bottomLine = new UIView(new CGRect(15, view.Frame.Bottom-1, view.Frame.Width, 1)) { BackgroundColor = AppTheme.HOLheaderBorderColor };

            view.AddSubviews(lblSectionHeader, topLine, bottomLine);
            return view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
                HandsOnLabsDetailSpeakerCell cell = tableView.DequeueReusableCell(cellIdentifierSpeaker) as HandsOnLabsDetailSpeakerCell;
                if (cell == null) cell = new HandsOnLabsDetailSpeakerCell(cellIdentifierSpeaker);
                var item = speakers[indexPath.Row];
                cell.UpdateCell(item);
				cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;           
        }

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			selectedIndex = indexPath;

		}
    }
}