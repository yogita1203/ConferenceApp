using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using CoreGraphics;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using CommonLayer;
using ConferenceAppiOS;
using Newtonsoft.Json;
using ConferenceAppiOS.Views;
using CoreAnimation;
using CoreGraphics;


namespace ConferenceAppiOS
{
    public class ProgramFaqController : BaseViewController
    {
        public enum panelEnum
        {
            Agenda,
            Programs,
            HandsOnLabs
        }
       
        public panelEnum panel { get; set; }
        public string AgendaSearchText { get; set; }

        public string HandsOnLabsSearchText { get; set; }

        public string ProgramSearchText { get; set; }
        public string CurrentSelectedAgendaDate { get; set; }

        #region--views--
        UITableView programTableview;
        LoadingOverlay loadingOverlay;
        CGRect rectBase;
        #endregion

        #region-- Constants
        string AgendaTitle = AppTheme.PFagendaTitle;
        string ProgramTitle = AppTheme.PFprogramTitle;
        string HandsOnLabsTitle = AppTheme.PFHOlTitle;
		static nfloat agendaTableHeaderViewHeight = 120;
		static nfloat headerViewLeftMargin = 0;
		static nfloat headerViewRightMargin = 4;
		static nfloat headerHeight = 64;
		static nfloat daysSegmentControlYPosition = 65;
		static nfloat daysSegmentControlHeight = 55;
		static nfloat sidePanelXPadding = 0;
		static nfloat sidePanelYPadding = 0;
		static nfloat sidePaneWidthPadding = 88;
		static nfloat blankViewXPadding = 0;
		static nfloat blankViewYPadding = 0;
		static nfloat blankViewHeigthPadding = 300;
		static nfloat sidePanelFooterXPadding = 0;
		static nfloat sidePanelFooterYPadding = 0;
		static nfloat sidePanelFooterWidthPadding = 0;
		static nfloat sidePanelFooterHeightPadding = 1;
		static nfloat empyVerticalViewYPadding = 0;
		static nfloat empyVerticalViewWidthPadding = 3;
		static nfloat agendaTableViewYPadding = 0;
		static nfloat agendaTableViewFooterXPadding = 0;
		static nfloat agendaTableViewFooterYPadding = 0;
		static nfloat agendaTableViewFooterWidthPadding = 0;
		static nfloat agendaTableViewFooterHeightPadding = 1;
		static nfloat HOLTableViewYPadding = 0;
		static nfloat HOLTableViewFooterXPadding = 0;
		static nfloat HOLTableViewFooterYPadding = 0;
		static nfloat HOLTableViewFooterWidthPadding = 0;
		static nfloat HOLTableViewFooterHeightPadding = 1;
		static nfloat programTableViewWidth = 400;
		static nfloat programTableViewWidthLeftOpened = 335;
		static nfloat programTableViewYPadding = 0;
		static nfloat programTableViewFooterXPadding = 0;
		static nfloat programTableViewFooterYPadding = 0;
		static nfloat programTableViewFooterWidthPadding = 0;
		static nfloat programTableViewFooterHeightPadding = 1;
		static nfloat programTableVwRightBorderWidth = 1;
		static nfloat programTableTopBorderWidth = 1;
		static nfloat topheaderProgrmDescHeight = 44;
		static nfloat handsOnlabsTableViewYPadding = 0;
		static nfloat sepLeftInset = 10;
		static nfloat progrmLeftInset = 20;
		static nfloat programTableHeaderViewHeight = 65;
		static nfloat holTitleBorderHeight = 3;
		static nfloat sidePanelTotalRowHeightFromAllCell = 300;
        nfloat myWidth;
        #endregion

        Dictionary<string, BuiltAgendaItem[]> agendaListDict = new Dictionary<string, BuiltAgendaItem[]>();
        Dictionary<string, List<BuiltHandsonLabs>> handsOnLabsListDict = new Dictionary<string, List<BuiltHandsonLabs>>();
        List<string> lstAgendaDate = new List<string>();
        public List<BuiltHandsonLabs> lsthandsOnLabsBase;
        public List<BuiltOthers> lstPrograms;

        #region-- Webview for Programs

        public WebViewController webView;
        UIView parentProgramView;
        UIView progrmVerticalLine;
        UIView topheaderProgrmDesc;
        UIView programheaderView;
        UILabel lblprogramHeader;
		static nfloat lblProgramHeaderWidthPadding = 0;
		static nfloat lblProgramHeaderHeightPadding = 20;
		static nfloat imgRightPadding = 50;
		static nfloat lblLeftPadding = 10;
		static nfloat imgLeftPadding = 25;
		static nfloat gapMargin = 20;
		static nfloat lblprogramHeaderYPadding = 10;
		static nfloat lblprogramHeaderRightPadding = 50;

		static nfloat blankViewWidthPadding = 50;

		static nfloat programBottomBorderXPadding = 20;

		static nfloat programBottomBorderWidthPadding = 20;
		static nfloat programBottomBorderHeightPadding = 1;

		static nfloat parentProgramViewXPadding = 0;
		static nfloat parentProgramViewYPadding = 0;

		static nfloat programheaderViewXPadding = 0;
		static nfloat programheaderViewYPadding = 0;

		static nfloat programTableviewXPadding = 0;

		static nfloat lblprogramHeaderHeightPadding = 20;
        #endregion

        
        public ProgramFaqController(CGRect rect)
        {
            rectBase = rect;
            View.Frame = rectBase;
            webView = new WebViewController("");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = rectBase;
            myWidth = View.Frame.Width;
            #region --Right header and Label
            topheaderProgrmDesc = new UIView();
            lblprogramHeader = new UILabel();

            lblprogramHeader.Frame = new CGRect(lblLeftPadding, lblprogramHeaderYPadding, lblProgramHeaderWidthPadding, lblProgramHeaderHeightPadding);
            lblprogramHeader.SizeToFit();
            lblprogramHeader.Frame = new CGRect(lblLeftPadding, lblprogramHeaderYPadding, View.Frame.Width - lblprogramHeaderRightPadding, lblProgramHeaderHeightPadding);
            lblprogramHeader.LineBreakMode = UILineBreakMode.WordWrap;
            lblprogramHeader.Lines = 2;
            lblprogramHeader.Font = AppTheme.PFprogramHeaderfont;
            lblprogramHeader.TextColor = AppTheme.PFprogramTitleColor;

            topheaderProgrmDesc.BackgroundColor = AppTheme.PFtopHeaderBackColor;
            topheaderProgrmDesc.AddSubviews(lblprogramHeader);
            #endregion

            var blankView = new UIView();
            blankView.Frame = new CGRect(blankViewXPadding, blankViewYPadding, blankViewWidthPadding, (View.Frame.Height / 2) - (sidePanelTotalRowHeightFromAllCell / 2));
            blankView.BackgroundColor = AppTheme.PFblankViewBackColor;

            programTableview = new UITableView()
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            parentProgramView = new UIView();

            ShowOverlay(programTableview);


            getprograms(res =>
            {
                lstPrograms=res.OrderBy(p => p.sequence).ToList();

                InvokeOnMainThread(() =>
                {
                    programTableview.Source = new ProgramDataSource(this, lstPrograms);
                    if (lstPrograms[0].title.Equals("FAQs", StringComparison.InvariantCultureIgnoreCase))
                    {
                        setRightHeaderAndWebView(0);
                    }
                    else if ((lstPrograms[0].title.Equals(AppTheme.PFHOlTitle.ToLower(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        AppDelegate.instance().rootViewController.openDetail(new HOLController(AppDelegate.instance().rootViewController.rightSlideView.Frame), this, true);
                    }
                    else
                    {
                        setRightHeaderAndWebView(0);
                    }
                    
                       

                    programTableview.ReloadData();
                    loadingOverlay.Hide();

                    if (lstPrograms.Count > 0)
                        programTableview.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                    View.BringSubviewToFront(parentProgramView);
                });
            });


            UIView programBottomBorder = new UIView(new CGRect(programBottomBorderXPadding, programTableViewFooterYPadding, View.Frame.Width - programBottomBorderWidthPadding, programBottomBorderHeightPadding));
            programBottomBorder.BackgroundColor = programTableview.SeparatorColor;
            programTableview.TableFooterView = programBottomBorder;
            programTableview.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            programTableview.SeparatorInset = new UIEdgeInsets(0, progrmLeftInset, 0, 0);
            programTableview.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            progrmVerticalLine = new UIView();
            progrmVerticalLine.BackgroundColor = AppTheme.PFverticalLineBackColor;
            View.AddSubviews(parentProgramView);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.others, Helper.ToDateString(DateTime.Now));
        }
        public void setRightHeaderAndWebView(int index)
        {
            webView.webView.LoadRequest( new NSUrlRequest(new NSUrl("about:blank")));
            webView.webView.Reload();
            //webView.loadRequest(lstPrograms.ElementAt(index).url);
            webView.webView.LoadRequest(new NSUrlRequest(new NSUrl(lstPrograms.ElementAt(index).url)));
            lblprogramHeader.Text = lstPrograms.ElementAt(index).title;

        }

        void ShowOverlay(UIView view)
        {
            var bounds = view.Bounds; 
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new CGSize(bounds.Size.Height, bounds.Size.Width);
            }
            this.loadingOverlay = new LoadingOverlay(bounds);
            view.Add(this.loadingOverlay);
        }
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (programTableview != null)
            {
                if (AppDelegate.instance().rootViewController.leftMenuOpened)
                {
                    setProgramsHeaderView();
                    parentProgramView.Frame = new CGRect(parentProgramViewXPadding, parentProgramViewYPadding, View.Frame.Width, View.Frame.Height);
                    programheaderView.Frame = new CGRect(programheaderViewXPadding, programheaderViewYPadding, View.Frame.Width, programTableHeaderViewHeight);
                    programTableview.Frame = new CGRect(programTableviewXPadding, programheaderView.Frame.Bottom, programTableViewWidthLeftOpened, View.Frame.Height - programTableHeaderViewHeight);
                    progrmVerticalLine.Frame = new CGRect(programTableview.Frame.Right, programheaderView.Frame.Bottom, programTableVwRightBorderWidth, View.Frame.Height);
                    lblprogramHeader.Frame = new CGRect(lblLeftPadding, lblprogramHeaderYPadding, myWidth / 2 - lblprogramHeaderRightPadding, lblprogramHeaderHeightPadding);
                    topheaderProgrmDesc.Frame = new CGRect(progrmVerticalLine.Frame.Right, programheaderView.Frame.Bottom, View.Frame.Width - (programTableview.Frame.Width + progrmVerticalLine.Frame.Width), topheaderProgrmDescHeight);
                    //webView.View.Frame = new CGRect(progrmVerticalLine.Frame.Right, topheaderProgrmDesc.Frame.Bottom, View.Frame.Width - (programTableview.Frame.Width + progrmVerticalLine.Frame.Width), View.Frame.Height - topheaderProgrmDesc.Frame.Height);
                    webView.View.Frame = new CGRect(progrmVerticalLine.Frame.Right, topheaderProgrmDesc.Frame.Bottom, View.Frame.Width - (programTableview.Frame.Width + progrmVerticalLine.Frame.Width), View.Frame.Height - topheaderProgrmDesc.Frame.Bottom);
                }
                else
                {
                    setProgramsHeaderView();

					parentProgramView.Frame = new CGRect(parentProgramViewXPadding, parentProgramViewYPadding, View.Frame.Width, View.Frame.Height);
					programheaderView.Frame = new CGRect(programheaderViewXPadding, programheaderViewYPadding, View.Frame.Width, programTableHeaderViewHeight);
					programTableview.Frame = new CGRect(programTableviewXPadding, programheaderView.Frame.Bottom, programTableViewWidth, View.Frame.Height - programTableHeaderViewHeight);
					progrmVerticalLine.Frame = new CGRect(programTableview.Frame.Right, programheaderView.Frame.Bottom, programTableVwRightBorderWidth, View.Frame.Height);
					topheaderProgrmDesc.Frame = new CGRect(progrmVerticalLine.Frame.Right, programheaderView.Frame.Bottom, View.Frame.Width - (programTableview.Frame.Width + progrmVerticalLine.Frame.Width), topheaderProgrmDescHeight);
					webView.View.Frame = new CGRect(progrmVerticalLine.Frame.Right, topheaderProgrmDesc.Frame.Bottom, View.Frame.Width - (programTableview.Frame.Width + progrmVerticalLine.Frame.Width), View.Frame.Height - topheaderProgrmDesc.Frame.Bottom);
                }

            }

        }
        void setProgramsHeaderView()
        {
            programheaderView = new UIView();
            TitleHeaderView ProgramtitleheaderView = new TitleHeaderView(ProgramTitle, true, false, false, false, false, false, false, false);
            ProgramtitleheaderView.BackgroundColor = AppTheme.PFtitleHeaderViewBackColor;
            programheaderView.BackgroundColor =AppTheme.PFprogramHeaderViewBackColor;
            ProgramtitleheaderView.Frame = new CGRect(headerViewLeftMargin, 0, View.Frame.Width - headerViewRightMargin, headerHeight);
            ProgramtitleheaderView.searchFieldClicked = (textField) =>
            {
                ProgramSearchText = textField.Text;
            };
            UIView borderTop = new UIView();
            borderTop.Frame = new CGRect(0, ProgramtitleheaderView.Frame.Bottom, View.Frame.Width, programTableTopBorderWidth);
            borderTop.BackgroundColor = AppTheme.PFtopBorderLineColor;
            programheaderView.AddSubviews(ProgramtitleheaderView, borderTop);
            parentProgramView.AddSubviews(programheaderView, topheaderProgrmDesc, programTableview, progrmVerticalLine, webView.View);
        }

        private static string convertTimeFormat(string time)
        {
            var date = DateTime.Parse(time).ToString("hh:mm tt");
            return date;
        }


        private void getprograms(Action<List<BuiltOthers>> callback)
        {
            DataManager.GetListOfProgramsFromOthers(AppDelegate.Connection).ContinueWith(t =>
            {
                var response = t.Result;
                lstPrograms = response;

                if (callback != null)
                {
                    callback(lstPrograms);
                }
            });
        }

        private void updatePrograms()
        {
            DataManager.GetListOfProgramsFromOthers(AppDelegate.Connection).ContinueWith(t =>
            {
                lstPrograms = t.Result;

                InvokeOnMainThread(() =>
                    {
                        (programTableview.Source as ProgramDataSource).UpdateSource(lstPrograms);
                        setRightHeaderAndWebView(0);
                        programTableview.ReloadData();
                    });
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.others))
                updatePrograms();
        }
    }


    #region--ProgramDataSource--
    public class ProgramDataSource : UITableViewSource
    {
        ProgramFaqController AgendaProgramsHandsOnLabsViewController;
        List<BuiltOthers> lstbuiltOthers;
        NSString cellIdentifier = new NSString("TableCell");
		static nfloat lblProgramRowHeight = 44;
         string title = AppTheme.PFHOlTitle.ToLower();
        public ProgramDataSource(ProgramFaqController AgendaProgramsHandsOnLabsViewController, List<BuiltOthers> lstbuiltOthers)
        {
            this.AgendaProgramsHandsOnLabsViewController = AgendaProgramsHandsOnLabsViewController;
            this.lstbuiltOthers = lstbuiltOthers;

        }
        public void UpdateSource(List<BuiltOthers> lstbuiltOthers)
        {
            this.lstbuiltOthers = lstbuiltOthers;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return lstbuiltOthers.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ProgramsCell cell = tableView.DequeueReusableCell(cellIdentifier) as ProgramsCell;
            if (cell == null) cell = new ProgramsCell(cellIdentifier);
            var item = lstbuiltOthers.ElementAt(indexPath.Row);
            cell.UpdateCell(item);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {

            return lblProgramRowHeight;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (lstbuiltOthers[indexPath.Row].title.Equals(title, StringComparison.InvariantCultureIgnoreCase))
            {
                AppDelegate.instance().rootViewController.openDetail(new HOLController(AppDelegate.instance().rootViewController.rightSlideView.Frame), AgendaProgramsHandsOnLabsViewController, true);
            }
            else
            {
                this.AgendaProgramsHandsOnLabsViewController.setRightHeaderAndWebView(indexPath.Row);
            }

        }

    }

    #endregion
}