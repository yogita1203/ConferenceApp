using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace ConferenceAppDroid.Utilities
{
    public class CustomWebViewClient : WebViewClient
    {
        Dictionary<String, String> noCacheHeaders;
        Context context;
        public CustomWebViewClient(Context context, Dictionary<String, String> noCacheHeaders)
        {
            this.context = context;
            this.noCacheHeaders = noCacheHeaders;
        }
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {

            if (url != null && !(url.StartsWith("http")))
            {
                try
                {
                    Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                    context.StartActivity(Intent.CreateChooser(intent, "Choose App"));
                }
                catch (Exception e)
                {
                }
                return true;
            }
            else
            {
                view.LoadUrl(url, noCacheHeaders);
                return false;
            }
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            //if (isShowTitle)
            //{
            //    if (!webView.getTitle().contains("http"))
            //    {
            //        titleTextView.setText(webView.getTitle().toUpperCase() + "  ");
            //    }
            //}
        }

        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
        }
    }
}