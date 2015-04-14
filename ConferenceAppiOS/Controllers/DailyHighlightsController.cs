using BigTed;
using CommonLayer;
using CommonLayer.Entities.Built;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Net;
using SDWebImage;
using ConferenceAppiOS.Helpers;

namespace ConferenceAppiOS
{
    [Register("DailyHighlightsController")]
    public class DailyHighlightsController : BaseViewController
    {
        string TitleString = AppTheme.DHNewsText;
		static nfloat HeaderHeight = 64;
		static nfloat FilterSectionHeight = 120;
		static nfloat NewsTableWidth = 362;
        nfloat lineSize = AppTheme.DHSeparatorBorderWidth;
		static nfloat detailPageWidth = 560;
		static nfloat detailPageHeight = 660;
		static nfloat ScrollViewHeight = 175;
		static nfloat TextFieldHeight = 40;
		static nfloat TextViewHeight = 0;
		static nfloat TopBarHeight = 50;
		static nfloat Margin = 20;
		static nfloat topBarTitleLeftMargin = 20;
		static nfloat topBarTitleWidth = 100;

		static nfloat btnDeleteRightMargin = 150;
		static nfloat btnTopMargin = 10;
		static nfloat btnSize = 30;
		static nfloat btnImageEdgeInset = 5;

		static nfloat btnEditLeftMargin = 20;

        UITableView newsTable;
        UIScrollView scrollView;
        internal Action<BuiltNews> NewsChangedHandler;
        readonly UIColor pageBackground = AppTheme.DHPageBackColor;

        LineView horizontalLine;
        LineView verticalLine;

        UILabel lblTitle;
        UILabel lblDate;
        UILabel lblDescription;
		UILabel newsLink;
        UIImageView imgCoverPhoto;
        internal BuiltNews currentNews;
        NewsTableSource newsTableSource;
        public DailyHighlightsController(CGRect rect)
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

            NewsChangedHandler = newsChanged;

            horizontalLine = new LineView(new CGRect(0, HeaderHeight, View.Frame.Width, lineSize))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            verticalLine = new LineView(new CGRect(NewsTableWidth, HeaderHeight, lineSize, View.Frame.Height - HeaderHeight));

            newsTable = new UITableView()
            {
                Frame = new CGRect(0, HeaderHeight, NewsTableWidth, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleRightMargin,
            };

            setHeader();
            newsTable.TableFooterView = new UIView(CGRect.Empty);

            setTableSource();

            View.AddSubviews(newsTable, horizontalLine, verticalLine);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.news, Helper.ToDateString(DateTime.Now));
        }

        public override void ViewWillLayoutSubviews()
        {
            nfloat height = 0;
            nfloat imgHeight = 0;
            base.ViewWillLayoutSubviews();
            verticalLine.Frame = new CGRect(NewsTableWidth, HeaderHeight, lineSize, View.Frame.Size.Height - HeaderHeight);
            newsTable.Frame = new CGRect(0, HeaderHeight, NewsTableWidth, View.Frame.Height);
            if (scrollView != null)
            {
                scrollView.Frame = new CGRect(verticalLine.Frame.Right, HeaderHeight + lineSize, View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));

                if (currentNews.cover_image != null)
                {
                    if (imgCoverPhoto != null)
                    {
                        imgCoverPhoto.Frame = new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), 300);
                        imgCoverPhoto.BackgroundColor = AppTheme.DHImageCoverPhotoBackColor;
                        imgCoverPhoto.ContentMode = UIViewContentMode.Center;
                        imgCoverPhoto.Layer.MasksToBounds = true;
                        imgCoverPhoto.SetImage(NSUrl.FromString(currentNews.cover_image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder));
                        imgHeight += imgCoverPhoto.Frame.Bottom;

                    }
                    else
                    {
                        imgCoverPhoto = new UIImageView();
                        imgCoverPhoto.BackgroundColor = AppTheme.DHImageCoverPhotoBackColor;
                        imgCoverPhoto.SetImage(NSUrl.FromString(currentNews.cover_image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder));
                        imgCoverPhoto.ContentMode = UIViewContentMode.Center;
                        imgCoverPhoto.Layer.MasksToBounds = true;
                        imgCoverPhoto.Frame = new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), 300);
                        imgHeight += imgCoverPhoto.Frame.Bottom;
                    }
                }

                if (imgHeight == 0)
                {
                    lblTitle.Frame = new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), TextFieldHeight);
                    lblTitle.SizeToFit();
                    lblTitle.Frame = new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), lblTitle.Frame.Height);
                }
                else
                {
                    lblTitle.Frame = new CGRect(Margin, imgHeight + 20, scrollView.Frame.Width - (Margin * 2), TextFieldHeight);
                    lblTitle.SizeToFit();
                    lblTitle.Frame = new CGRect(Margin, imgHeight + 20, scrollView.Frame.Width - (Margin * 2), lblTitle.Frame.Height);
                }


                height += lblTitle.Frame.Bottom;

                if (currentNews != null && !string.IsNullOrEmpty(currentNews.published_date))
                {
					if(lblDate != null){
                    lblDate.Frame = new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), lblDate.Frame.Size.Height);
                    height = lblDate.Frame.Bottom;
					}
                }

                lblDescription.Frame = new CGRect(Margin, height + Margin, scrollView.Frame.Width - (Margin * 2), TextViewHeight);
                var frame = lblDescription.Frame;
                lblDescription.SizeToFit();
                frame.Height = lblDescription.Frame.Height;
                lblDescription.Frame = frame;

                newsLink.Frame = new CGRect(lblDescription.Frame.X, lblDescription.Frame.Bottom + Margin, scrollView.Frame.Width - (lblDescription.Frame.X * 2), TextViewHeight);
                var linkFrame = newsLink.Frame;
                newsLink.SizeToFit();
                linkFrame.Height = newsLink.Frame.Height;
                newsLink.Frame = linkFrame;
                scrollView.ContentSize = new CGSize(View.Frame.Width - verticalLine.Frame.Right, newsLink.Frame.Bottom + Margin);

            }
        }

        private void setTableSource()
        {
            DataManager.GetNews(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;

                if (result != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        newsTableSource = new NewsTableSource(this, result);
                        newsTable.Source = newsTableSource;
                        newsTable.ReloadData();

                        if (result.Count > 0)
                            newsTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                        currentNews = result.FirstOrDefault();
                        setNewsDetailsView();
                        ViewWillLayoutSubviews();
                    });
                }
            });
        }

        public override void OnDetailClosing(NSNotification notification)
        {
            base.OnDetailClosing(notification);
            if (newsTableSource != null && newsTableSource.selectedIndex != null)
            {
                newsTable.DeselectRow(newsTableSource.selectedIndex, false);
            }
        }

        private void updateTableSource()
        {
            DataManager.GetNews(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;

                if (result != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        (newsTable.Source as NewsTableSource).UpdateSource(result);
                        newsTable.ReloadData();

                        if (result.Count > 0)
                            newsTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                        currentNews = result.FirstOrDefault();
                        updateNewsDetailsView();
                        ViewWillLayoutSubviews();
                    });
                }
            });
        }

        private void newsChanged(BuiltNews news)
        {
            currentNews = news;
            updateNewsDetailsView();
            ViewWillLayoutSubviews();
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

        void setNewsDetailsView()
        {
            nfloat height = 0;
            nfloat dateheight = 0;
            scrollView = new UIScrollView();
            scrollView.Frame = new CGRect(verticalLine.Frame.Right, HeaderHeight + lineSize, View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));
            scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
            scrollView.BackgroundColor = AppTheme.DHnewsdetailsbackColor;

            lblTitle = new UILabel(new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), TextFieldHeight))
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Font = AppTheme.DHnewsTitleFont,
                TextColor = AppTheme.DHCellTitleColor
            };
            height += lblTitle.Frame.Bottom;
            dateheight += lblTitle.Frame.Bottom;
            
            var actualValue = convertToNewsDate(currentNews.published_date);
            var minValue = DateTime.MinValue.ToString("MMMM dd, yyyy");

            if (actualValue != minValue)
            {
                lblDate = new UILabel(new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), TextFieldHeight));
                lblDate.Font = AppTheme.DHdescriptionFont;
                lblDate.TextColor = AppTheme.DHcellDescriptionColor;
                lblDate.SizeToFit();
                lblDate.Frame = new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), lblDate.Frame.Size.Height);
                lblDate.Text = convertToNewsDate(currentNews.published_date);
                scrollView.AddSubview(lblDate);
                dateheight += lblDate.Frame.Bottom;
            }


            if (dateheight.Equals(height))
            {
                lblDescription = new UILabel(new CGRect(Margin, height + Margin, scrollView.Frame.Width - (Margin * 2), TextViewHeight))
                {
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                    Font = AppTheme.DHdescriptionFont,
                    TextColor = AppTheme.DHCellTitleColor
                };
            }

            else
            {
                lblDescription = new UILabel(new CGRect(Margin, dateheight + Margin, scrollView.Frame.Width - (Margin * 2), TextViewHeight))
                {
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                    Font = AppTheme.DHdescriptionFont,
                    TextColor = AppTheme.DHCellTitleColor
                };
            }

            if (currentNews != null)
            {
                lblTitle.AttributedText = AppFonts.IncreaseLineHeight(currentNews.title, lblTitle.Font, lblTitle.TextColor);
                lblTitle.SizeToFit();
                lblTitle.Frame = new CGRect(Margin, Margin, scrollView.Frame.Width - (Margin * 2), lblTitle.Frame.Height);
				nfloat yPosition = lblTitle.Frame.Bottom + Margin;
				if (lblDate != null) {
					lblDate.SizeToFit();
					lblDate.Frame = new CGRect(Margin, lblTitle.Frame.Bottom + (Margin / 2), scrollView.Frame.Width - (Margin * 2), lblDate.Frame.Size.Height);
					yPosition = lblDate.Frame.Bottom + Margin;
				}
                
                lblDescription.AttributedText = AppFonts.IncreaseLineHeight(currentNews.desc, lblDescription.Font, lblDescription.TextColor);
                lblDescription.SizeToFit();
				lblDescription.Frame = new CGRect(Margin, yPosition, scrollView.Frame.Width - (Margin * 2), TextViewHeight);

                var frame = lblDescription.Frame;
                lblDescription.SizeToFit();
                frame.Height = lblDescription.Frame.Height;
                lblDescription.Frame = frame;

				newsLink = new UILabel();
                newsLink.Lines = 0;
                newsLink.TextColor = AppTheme.DHNewsLinkTextColor;
                newsLink.Font = AppTheme.DHNewsLinkFont;
                newsLink.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                newsLink.LineBreakMode = UILineBreakMode.WordWrap;
                newsLink.Frame = new CGRect(lblDescription.Frame.X, lblDescription.Frame.Bottom + Margin, scrollView.Frame.Width - (lblDescription.Frame.X * 2), TextViewHeight);
//                newsLink.Delegate = new AttributedLabelDelegate();


                if (currentNews.link != null)
                {
                    newsLink.Text = new NSString(currentNews.link.title);

                    try
                    {
//                        var url = NSUrl.FromString(currentNews.link.href);
//                        var range = new NSRange(0, currentNews.link.title.Length);
//                        newsLink.AddLinkToURL(url, range);
                    }
                    catch
                    { }

                    var linkFrame = newsLink.Frame;
                    newsLink.SizeToFit();
                    linkFrame.Height = newsLink.Frame.Height;
                    newsLink.Frame = linkFrame;
                }


                if (currentNews.cover_image != null)
                {
                    imgCoverPhoto = new UIImageView(new CGRect(Margin, newsLink.Frame.Bottom + Margin, scrollView.Frame.Width - (Margin * 2), 300))
                    {
                        BackgroundColor = AppTheme.DHImageCoverPhotoBackColor,
                        ContentMode = UIViewContentMode.Center,

                    };
                    imgCoverPhoto.Layer.MasksToBounds = true;
                    imgCoverPhoto.SetImage(NSUrl.FromString(currentNews.cover_image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder));
                    scrollView.AddSubview(imgCoverPhoto);
                }

                scrollView.AddSubviews(lblTitle,  lblDescription, newsLink);

                if (imgCoverPhoto == null)
                {
                    scrollView.ContentSize = new CGSize(View.Frame.Width - verticalLine.Frame.Right, newsLink.Frame.Bottom);
                }
                else
                {
                    scrollView.ContentSize = new CGSize(View.Frame.Width - verticalLine.Frame.Right, imgCoverPhoto.Frame.Bottom);
                }

                View.AddSubview(scrollView);

                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.news_details, Helper.ToDateString(DateTime.Now));
            }

        }

        void updateNewsDetailsView()
        {
            nfloat height = 0;
            if (imgCoverPhoto != null)
            {
                imgCoverPhoto.RemoveFromSuperview();
                imgCoverPhoto = null;
                imgCoverPhoto = new UIImageView();
                imgCoverPhoto.ContentMode = UIViewContentMode.Center;
                imgCoverPhoto.Layer.MasksToBounds = true;
                imgCoverPhoto.BackgroundColor = AppTheme.DHImageCoverPhotoBackColor;
                height += imgCoverPhoto.Frame.Y;
                scrollView.AddSubview(imgCoverPhoto);
            }

            if (lblDate != null)
            {
                lblDate.RemoveFromSuperview();
                lblDate = null;
                lblDate = new UILabel(new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), TextFieldHeight));
                lblDate.Font = AppTheme.DHdescriptionFont;
                lblDate.TextColor = AppTheme.DHcellDescriptionColor;
                lblDate.SizeToFit();
                lblDate.Frame = new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), lblDate.Frame.Size.Height);
                scrollView.AddSubview(lblDate);
            }

            if (currentNews != null)
            {
                if (currentNews.cover_image != null)
                {
                    if (imgCoverPhoto == null)
                    {
                        imgCoverPhoto = new UIImageView();
                        imgCoverPhoto.SetImage(NSUrl.FromString(currentNews.cover_image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder));
                        //imgCoverPhoto.Frame = new CGRect(Margin, newsLink.Frame.Bottom + Margin, scrollView.Frame.Width - (Margin * 2), 300);
                        scrollView.ContentSize = new CGSize(View.Frame.Width - verticalLine.Frame.Right, imgCoverPhoto.Frame.Bottom);
                        scrollView.AddSubview(imgCoverPhoto);
                    }

                }
                if (currentNews.published_date != null)
                {
                    if (lblDate != null)
                    {
                        lblDate = new UILabel(new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), TextFieldHeight));
                        lblDate.Text = convertToNewsDate(currentNews.published_date);
                        lblDate.Font = AppTheme.DHdescriptionFont;
                        lblDate.TextColor = AppTheme.DHcellDescriptionColor;
                        lblDate.SizeToFit();
                        lblDate.Frame = new CGRect(Margin, height + Margin / 2, scrollView.Frame.Width - (Margin * 2), lblDate.Frame.Size.Height);
                        scrollView.AddSubview(lblDate);
                    }
                }
                lblTitle.SizeToFit();
                lblTitle.AttributedText = AppFonts.IncreaseLineHeight(currentNews.title, lblTitle.Font, lblTitle.TextColor);

                lblDescription.Text = currentNews.desc;


                var frame = lblDescription.Frame;
                lblDescription.SizeToFit();
                frame.Height = lblDescription.Frame.Height;
                lblDescription.Frame = frame;

                if (currentNews.link != null)
                {
                    newsLink.Text = new NSString(currentNews.link.title);

                    try
                    {
                        var url = NSUrl.FromString(currentNews.link.href);
                        var range = new NSRange(0, currentNews.link.title.Length);
//                        newsLink.AddLinkToURL(url, range);
                    }
                    catch
                    { }
                }

                newsLink.Frame = new CGRect(lblDescription.Frame.X, lblDescription.Frame.Bottom + Margin, scrollView.Frame.Width - (lblDescription.Frame.X * 2), TextViewHeight);
                var linkFrame = newsLink.Frame;
                newsLink.SizeToFit();
                linkFrame.Height = newsLink.Frame.Height;
                newsLink.Frame = linkFrame;


                if (currentNews.cover_image != null)
                {
                    if (imgCoverPhoto == null)
                    {
                        imgCoverPhoto = new UIImageView();
                        imgCoverPhoto.SetImage(NSUrl.FromString(currentNews.cover_image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder));
                        //imgCoverPhoto.Frame = new CGRect(Margin, newsLink.Frame.Bottom + Margin, scrollView.Frame.Width - (Margin * 2), 300);
                        scrollView.ContentSize = new CGSize(View.Frame.Width - verticalLine.Frame.Right, imgCoverPhoto.Frame.Bottom);
                        scrollView.AddSubview(imgCoverPhoto);
                    }

                }

                DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.news_details, Helper.ToDateString(DateTime.Now));
            }
        }

        public override void ViewDidLayoutSubviews()
        {


        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.news))
            {
                updateTableSource();
            }
        }

        public string convertToNewsDate(string dateTime)
        {
            if (String.IsNullOrWhiteSpace(dateTime))
                return String.Empty;
            try
            {
                var date = DateTime.Parse(dateTime);
                return Helper.ToDateTimeString(date, "MMMM dd, yyyy");
            }
            catch
            {
                return String.Empty;
            }
        }
    }

    internal class NewsTableSource : UITableViewSource
    {
        DailyHighlightsController dailyHighlightsController;
        List<BuiltNews> news;
        NSString cellIdentifier = new NSString("NewsCell");
        public NSIndexPath selectedIndex;
        public NewsTableSource(DailyHighlightsController dailyHighlightsController, List<BuiltNews> news)
        {
            this.dailyHighlightsController = dailyHighlightsController;
            this.news = news;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            NewsTableCell cell = tableView.DequeueReusableCell(cellIdentifier) as NewsTableCell;
            if (cell == null)
            {
                cell = new NewsTableCell(cellIdentifier);
            }
            var item = news[indexPath.Row];
            cell.UpdateCell(item);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            if (dailyHighlightsController.NewsChangedHandler != null)
                dailyHighlightsController.NewsChangedHandler(news[indexPath.Row]);
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return news.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            NSString str = new NSString("");
            var singlenews = news[indexPath.Row];
            if (!string.IsNullOrWhiteSpace(singlenews.title))
            {
                str = (NSString)singlenews.title;
            }

            CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(18), new CGSize(tableView.Frame.Width - 30, 36), UILineBreakMode.WordWrap);
            var height = size.Height;
            if (singlenews.published_date != null)
            {
                return height+45;
            }
            if (height < 50)
            {
                return 50;
            }
            else
            {
                return height+20;
            }
        }

        internal void UpdateSource(List<BuiltNews> news)
        {
            this.news = news;
        }
    }

    internal class NewsTableCell : UITableViewCell
    {
		static nfloat Margin = 15;
		static nfloat TopMargin = 10;
		static nfloat LabelHeight = 20;
        BuiltNews model;
        UILabel _titleLabel;
        public UILabel TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel()
                    {
                        Lines=2,
                        LineBreakMode=UILineBreakMode.TailTruncation,
                        TextColor = AppTheme.DHCellTitleColor,
                        HighlightedTextColor = AppTheme.DHHighLightedTextColor,
                        BackgroundColor = AppTheme.DHTitleBackColor,
                        Font = AppTheme.DHtitleLabelFont,
                    };
                }
                return _titleLabel;
            }
        }

        UILabel _titledate;
        public UILabel TitleDate
        {
            get
            {
                if (_titledate == null)
                {
                    _titledate = new UILabel()
                    {
                        TextColor = AppTheme.DHcellDescriptionColor,
                        HighlightedTextColor = AppTheme.DHdescriptionHighLightedTextColor,
                        BackgroundColor = AppTheme.DHDescBackColor,
                        Font = AppTheme.DHdescLabelFont,
                    };
                }
                return _titledate;
            }
        }

        public NewsTableCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            ContentView.AddSubviews(TitleLabel, TitleDate);

            SelectedBackgroundView = new UIView
            {
                BackgroundColor = AppTheme.DHnotesCellSelectedBackColor
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (model != null && model.published_date != null)
            {
                var actualValue=convertToNewsDate(model.published_date);
                var minValue = DateTime.MinValue.ToString("MMMM dd, yyyy");
                if (!actualValue.Equals(minValue))
                {

                    TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin * 2), 0);
                    TitleLabel.SizeToFit();
                    TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin * 2), TitleLabel.Frame.Height);
                    TitleDate.Text = convertToNewsDate(model.published_date);
                    TitleDate.SizeToFit();
                    TitleDate.Frame = new CGRect(Margin, TitleLabel.Frame.Bottom + (TopMargin / 2), ContentView.Frame.Width - (Margin * 2), TitleDate.Frame.Height);
                    this.ContentView.AddSubview(TitleDate);
                }
                else
                {
                    TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin * 2), 0);
                    TitleLabel.SizeToFit();
                    TitleLabel.Frame = new CGRect(Margin, (ContentView.Frame.Height - TitleLabel.Frame.Height) / 2, ContentView.Frame.Width - (Margin * 2), TitleLabel.Frame.Height);
                }
                
            }
            else
            {
                TitleLabel.Frame = new CGRect(Margin, TopMargin, ContentView.Frame.Width - (Margin * 2), 0);
                TitleLabel.SizeToFit();
				TitleLabel.Frame = new CGRect(Margin, (ContentView.Frame.Height - TitleLabel.Frame.Height)/2, ContentView.Frame.Width - (Margin * 2), TitleLabel.Frame.Height);

            }
        }

        public void UpdateCell(BuiltNews news)
        {
            model = news;
            TitleLabel.Text = news.title;
        }

        public override void PrepareForReuse()
        {
            if (_titledate != null)
            {
                _titledate.RemoveFromSuperview();
                _titledate = null;
            }
        }

        public string convertToNewsDate(string dateTime)
        {
            if (String.IsNullOrWhiteSpace(dateTime))
                return String.Empty;
            try
            {
                var date = DateTime.Parse(dateTime);
                return Helper.ToDateTimeString(date, "MMMM dd, yyyy");
            }
            catch
            {
                return String.Empty;
            }
        }
    }

//    class AttributedLabelDelegate : TTTAttributedLabelDelegate
//    {
//        public override void DidSelectLinkWithURL(TTTAttributedLabel label, NSUrl url)
//        {
//            UIApplication.SharedApplication.OpenUrl(url);
//        }
//	}//..
}