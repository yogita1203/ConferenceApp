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
    public class CustomWebChromeClient : WebChromeClient
    {
        ProgressBar progressBar;
        public CustomWebChromeClient(ProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }
        public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        {
            return base.OnJsAlert(view, url, message, result);
        }

        public override void OnProgressChanged(WebView view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);
            if (newProgress == 0)
            {
                progressBar.Visibility=ViewStates.Visible;
            }
            else if (newProgress == 100)
            {
                progressBar.Visibility = ViewStates.Gone;
            }
        }
    }
}