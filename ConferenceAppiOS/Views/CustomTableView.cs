using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
    public class Keys
    {
        public const string Text = "Text";
        public const string SelectedImage = "SelectedImage";
        public const string NormalImage = "NormalImage";
    }
    public class CustomTableView : UITableView
    {
        public enum TableType
        {
            session,
            schedule,
            AgendaProgramHandsonLabsEnum
        }

        public Action<NSIndexPath> RowSelectedHandler;


        public CustomTableView(TableType tableType)
        {

            if (tableType == TableType.session)
            {
                Dictionary<string, string> lstFirst = new Dictionary<string, string>();
                lstFirst.Add(Keys.Text, AppTheme.CTsessionsTitleText);
				lstFirst.Add(Keys.NormalImage, AppTheme.SctimeImage);
                Dictionary<string, string> lstSecond = new Dictionary<string, string>();
                lstSecond.Add(Keys.Text, AppTheme.CTspeakesText);
				lstSecond.Add(Keys.NormalImage, AppTheme.ScgroupImage);
                List<Dictionary<string, string>> lstSource = new List<Dictionary<string, string>>();
                lstSource.Add(lstFirst);
                lstSource.Add(lstSecond);
                Source = new SidePanelDataSource(this, lstSource.ToArray());
				SelectRow(NSIndexPath.FromRowSection(0,0), true, UITableViewScrollPosition.None);
            }
            else if (tableType == TableType.schedule)
            {
                Dictionary<string, string> lstFirst = new Dictionary<string, string>();
                lstFirst.Add(Keys.Text, AppTheme.Scheduletext);
				lstFirst.Add(Keys.NormalImage, AppTheme.ScheduleBigIcon);
                Dictionary<string, string> lstSecond = new Dictionary<string, string>();
                lstSecond.Add(Keys.Text, AppTheme.SIinterestTitle);
				lstSecond.Add(Keys.NormalImage, AppTheme.IntrestBigIcon);
                List<Dictionary<string, string>> lstSource = new List<Dictionary<string, string>>();
                lstSource.Add(lstFirst);
                lstSource.Add(lstSecond);
                Source = new SidePanelDataSource(this, lstSource.ToArray());
				SelectRow(NSIndexPath.FromRowSection(0,0), true, UITableViewScrollPosition.None);
            }
            else if (tableType == TableType.AgendaProgramHandsonLabsEnum)
            {
                Dictionary<string, string> lstFirst = new Dictionary<string, string>();
                lstFirst.Add(Keys.Text, AppTheme.AGagendaTitle);
                lstFirst.Add(Keys.NormalImage, AppTheme.APNewImage);
                lstFirst.Add(Keys.SelectedImage, AppTheme.APNewImage);
                Dictionary<string, string> lstSecond = new Dictionary<string, string>();
                lstSecond.Add(Keys.Text, AppTheme.PFprogramTitle);
                lstSecond.Add(Keys.NormalImage, AppTheme.programImage);
                lstSecond.Add(Keys.SelectedImage, AppTheme.programImage);
                Dictionary<string, string> lstThird = new Dictionary<string, string>();
                lstThird.Add(Keys.Text, AppTheme.PFHOlTitle);
                lstThird.Add(Keys.NormalImage, AppTheme.handOnlabsImage);
                lstThird.Add(Keys.SelectedImage, AppTheme.handOnlabsImage);
                List<Dictionary<string, string>> lstSource = new List<Dictionary<string, string>>();
                lstSource.Add(lstFirst);
                lstSource.Add(lstSecond);
                lstSource.Add(lstThird);
                Source = new SidePanelDataSource(this, lstSource.ToArray());
                SelectRow(NSIndexPath.FromRowSection(0, 0), true, UITableViewScrollPosition.None);
            }
        }
    }

    public class SidePanelDataSource : UITableViewSource
    {
        Dictionary<string, string>[] dict;
        NSString cellIdentifier = new NSString("TableCell");
        CustomTableView customTableView;
		public NSIndexPath selectedIndex;

        public SidePanelDataSource(CustomTableView tableView, Dictionary<string, string>[] dictionary)
        {
            this.customTableView = tableView;
            this.dict = dictionary;
        }
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 88;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var data = dict[indexPath.Row];
			selectedIndex = indexPath;
            if (customTableView.RowSelectedHandler != null)
            {
                customTableView.RowSelectedHandler(indexPath);
            }
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SidePanelCell cell = tableView.DequeueReusableCell(cellIdentifier) as SidePanelCell;
            if (cell == null) cell = new SidePanelCell(cellIdentifier);

            var data = dict[indexPath.Row];
            cell.UpdateCell(data);
			cell.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.Layer3Color,1.0f);

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return this.dict.Count();
        }

    }

    public class SidePanelCell : UITableViewCell
    {
        public static Action<BuiltTracks> action;
        public UILabel lblName { get; set; }
        public UILabel imgIcon { get; set; }

        Dictionary<string, string> celldata;


        public SidePanelCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            lblName = new UILabel()
            {
				Font = AppFonts.ProximaNovaRegular(15),
				TextColor = UIColor.Clear.FromHexString(AppTheme.SecondaryColor,1.0f),
                TextAlignment = UITextAlignment.Center,
                LineBreakMode = UILineBreakMode.WordWrap,
				BackgroundColor = UIColor.Clear
            };

            imgIcon = new UILabel()
            {
				TextColor = AppTheme.LMmenuNormalText,
                HighlightedTextColor = AppTheme.MenuHighlightedText,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Center,
                Font = AppTheme.SPIconFontSize,
            };

			lblName.HighlightedTextColor = UIColor.Clear.FromHexString(AppTheme.Layer1Color,1.0f);
            lblName.Lines = 2;
            imgIcon.TextColor = UIColor.Clear.FromHexString(AppTheme.SecondaryColor, 1.0f);
            this.SelectedBackgroundView = new UIView(this.Frame);
			this.SelectedBackgroundView.BackgroundColor = UIColor.Clear.FromHexString(AppTheme.SecondaryColor,1.0f);

            ContentView.Add(lblName);
            ContentView.Add(imgIcon);

        }

        public void UpdateCell(Dictionary<string, string> cellData)
        {
            this.celldata = cellData;
            lblName.Text = cellData[Keys.Text];
            imgIcon.Text = cellData[Keys.NormalImage];
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imgIcon.Frame = new CoreGraphics.CGRect(ContentView.Frame.Width / 2 - 25, ContentView.Frame.Height / 2 - 30, 50, 40);
            lblName.Frame = new CoreGraphics.CGRect(0, imgIcon.Frame.Bottom + 5, Frame.Width, 0);
            lblName.SizeToFit();
            lblName.Frame = new CoreGraphics.CGRect(0, imgIcon.Frame.Bottom + 5, Frame.Width, lblName.Frame.Height);
        }
    }
}