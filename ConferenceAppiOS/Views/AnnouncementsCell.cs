using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;
using System.IO;
using System.ComponentModel;
using System.Web;
using SDWebImage;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Views;

namespace ConferenceAppiOS
{
    public class AnnouncementCell : UITableViewCell
    {

        UIScrollView AnnoucmentsScrollView;

        public Action<int> pageChangedCallBack;
        PageControl pageControl;

		static nfloat AnnouncmntCellHght = 200; static nfloat pageControlBottomPadding = 60;
        UILabel lblSyncIndicator; CGSize pgControlSizef;
        string fileName = "intro.html";
        string localHtmlUrl;
        UIWebView webView;
        public AnnouncementCell(NSString cellId)
            : base(UITableViewCellStyle.Default, cellId)
        {
            AnnoucmentsScrollView = new UIScrollView();
            pageControl = new PageControl(PageControlType.OnFullOffFull, new CGRect((ContentView.Frame.Size.Width - 200) / 2, AnnoucmentsScrollView.Frame.Bottom - 50, 200, 50));
            localHtmlUrl = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);
            AnnoucmentsScrollView.ShowsVerticalScrollIndicator = false;
            AnnoucmentsScrollView.BackgroundColor = UIColor.White;
            AnnoucmentsScrollView.PagingEnabled = true;
            AnnoucmentsScrollView.Scrolled += AnnoucmentsScrollView_Scrolled;
            pgControlSizef = new CGSize();
            pageControl.OnColor = AppTheme.pageControlOnColor;

            pageControl.OffColor = AppTheme.pageControlOffColor;
            pageControl.HidesForSinglePage = true;
            lblSyncIndicator = new UILabel();
            lblSyncIndicator.Text = "9m ago";
            lblSyncIndicator.TextColor = AppTheme.AClblSyncIndicatorTextColor;
            lblSyncIndicator.Font = AppFonts.ProximaNovaRegular(15);
            ContentView.AddSubviews(AnnoucmentsScrollView, pageControl, lblSyncIndicator);
        }



        bool webViewShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            return true;
        }


        public bool myHandler(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navType)
        {
            return true;

        }

        public class MyWebViewDelegate : UIWebViewDelegate
        {
            public string htmlSrtingToLoad = null;
            public string bgImageUrl = null;
            public string bgcolor = null;
            public string textcolor = null;
            public NSData imageData;
            public NSString base64String;

            public override bool ShouldStartLoad(UIWebView webView,
                NSUrlRequest request,
                UIWebViewNavigationType navigationType)
            {
                webView.ScalesPageToFit = true;
                webView.ContentMode = UIViewContentMode.ScaleAspectFit;

                return true;
            }
            public override void LoadStarted(UIWebView webView)
            {

                webView.ScalesPageToFit = true;
                webView.ContentMode = UIViewContentMode.ScaleAspectFit;
            }

            public override void LoadingFinished(UIWebView webView)
            {

                if (!String.IsNullOrEmpty(htmlSrtingToLoad))
                {
                    string encodedHtml = HttpUtility.JavaScriptStringEncode(htmlSrtingToLoad);
                    webView.EvaluateJavascript("prepareIntroHTML('" + encodedHtml + "','" + bgcolor + "','" + base64String + "','" + textcolor + "')");
                    webView.ScalesPageToFit = true;
                    webView.ContentMode = UIViewContentMode.ScaleToFill;
                }
            }

            public void showHtml(BuiltIntro builtintro)
            {
                if (builtintro.desc != null)
                {
                    htmlSrtingToLoad = builtintro.desc;
                }
                if (builtintro.bg_image != null)
                {
                    bgImageUrl = builtintro.bg_image.url;
                    //						NSData * data = [[NSData alloc] initWithContentsOfURL: [NSURL URLWithString: intro.bg_image_url]];
                    //						NSString *base64String = [data base64EncodedStringWithOptions:0];
                }
                if (builtintro.bg_color != null)
                {
                    bgcolor = builtintro.bg_color;
                }
                if (builtintro.text_color != null)
                {
                    textcolor = builtintro.text_color;
                }
            }

            public override void LoadFailed(UIWebView webView, NSError error)
            {
            }

        }

        public void AddHtmlString(BuiltIntro builtintro)
        {
            webView = new UIWebView();
            webView.ScrollView.ScrollEnabled = false;
            webView.ScalesPageToFit = true;
            webView.ContentMode = UIViewContentMode.ScaleAspectFit;
            webView.Delegate = new MyWebViewDelegate();
            showHtml(builtintro);
            webView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            webView.BackgroundColor = AppTheme.introBackGroundColor;
            AnnoucmentsScrollView.AddSubview(webView);
        }

        public void showHtml(BuiltIntro builtintro)
        {
            MyWebViewDelegate webviewDelegate = (MyWebViewDelegate)webView.Delegate;
            if (builtintro.desc != null)
            {
                webviewDelegate.htmlSrtingToLoad = builtintro.desc;
            }
            if (builtintro.bg_color != null)
            {
                webviewDelegate.bgcolor = builtintro.bg_color;
            }
            if (builtintro.text_color != null)
            {
                webviewDelegate.textcolor = builtintro.text_color;
            }
            if (builtintro.bg_image != null)
            {
                //webviewDelegate.imageData = NSData.FromUrl(NSUrl.FromString(builtintro.bg_image.url));
                //webviewDelegate.base64String = (NSString)webviewDelegate.imageData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                webviewDelegate.base64String = AppSettings.WebViewImageString;
                webView.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
            }
            else
            {
                webView.LoadRequest(new NSUrlRequest(new NSUrl(localHtmlUrl, false)));
            }

        }

        public void RemoveAllViews()
        {
            foreach (var item in AnnoucmentsScrollView.Subviews)
            {
                if (item != null)
                    item.RemoveFromSuperview();
            }
        }

        public Action sdWebImageHandler;

        public void AnnoucmentsScrollView_Scrolled(object sender, EventArgs e)
        {

            int pageNo = (int)Math.Floor(AnnoucmentsScrollView.ContentOffset.X / AnnoucmentsScrollView.Frame.Size.Width);
            pageControl.CurrentPage = pageNo;

        }


        public delegate void ScrollEventHandler(object sender, EventArgs args);

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (AppDelegate.instance().rootViewController.leftMenuOpened)
            {
                AnnoucmentsScrollView.Frame = new CGRect(0, 0 + AppTheme.sectionBottomBorderHeight, ContentView.Frame.Width, AnnouncmntCellHght);
                nfloat x = 0;
                int i = 0;
                foreach (UIView uv in AnnoucmentsScrollView.Subviews)
                {
                    if (uv.GetType() == typeof(UIWebView))
                    {
                        uv.Frame = new CGRect(x, 0, ContentView.Frame.Width, AnnoucmentsScrollView.Frame.Height);

                        ((UIWebView)uv).ScalesPageToFit = true;
                        ((UIWebView)uv).ContentMode = UIViewContentMode.ScaleAspectFit;

                        x += ContentView.Frame.Width;
                        i++;
                    }
                }

                nfloat pgcontrolWidth = 30 * i;

                pageControl.Pages = i;
                pgControlSizef = pageControl.SizeForNumberOfPages(i);
                pageControl.Frame = new CGRect((ContentView.Frame.Size.Width / 2) - (pgControlSizef.Width / 2), AnnoucmentsScrollView.Frame.Bottom - pageControlBottomPadding, pgControlSizef.Width, pgControlSizef.Height);
                /////sync indicator.. Currently not used//////lblSyncIndicator.Frame = new CGRect(pageControl.Frame.Right + 50, AnnoucmentsScrollView.Frame.Bottom - 60, 80, 50);
                AnnoucmentsScrollView.ContentSize = new CGSize(x, AnnouncmntCellHght);
            }
            else
            {
                AnnoucmentsScrollView.Frame = new CGRect(0, 0, ContentView.Frame.Width, AnnouncmntCellHght);
                nfloat x = 0;
                int i = 0;
                foreach (UIView uv in AnnoucmentsScrollView.Subviews)
                {
                    if (uv.GetType() == typeof(UIWebView))
                    {
                        uv.Frame = new CGRect(x, 0 + AppTheme.sectionBottomBorderHeight, ContentView.Frame.Width, AnnoucmentsScrollView.Frame.Height);
                        ((UIWebView)uv).ScalesPageToFit = true;
                        //// ((UIWebView)uv).ContentMode = UIViewContentMode.ScaleAspectFit;
                        x += ContentView.Frame.Width;
                        i++;
                    }
                }

                nfloat pgcontrolWidth = 30 * i;

                pageControl.Pages = i;
                pgControlSizef = pageControl.SizeForNumberOfPages(i);
                pageControl.Frame = new CGRect((ContentView.Frame.Size.Width / 2) - (pgControlSizef.Width / 2), AnnoucmentsScrollView.Frame.Bottom - pageControlBottomPadding, pgControlSizef.Width, pgControlSizef.Height);
                /////sync indicator.. Currently not used//////lblSyncIndicator.Frame = new CGRect(pageControl.Frame.Right + 50, AnnoucmentsScrollView.Frame.Bottom - 60, 80, 50);
                AnnoucmentsScrollView.ContentSize = new CGSize(x, AnnouncmntCellHght);
            }



        }




    }

}
