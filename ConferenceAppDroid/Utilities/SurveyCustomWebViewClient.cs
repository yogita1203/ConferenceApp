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
    public class SurveyCustomWebViewClient : WebViewClient
    {
        Dictionary<String, String> noCacheHeaders;
        Context context;
        RelativeLayout surveyLoadingContainer;
        public SurveyCustomWebViewClient(Context context, Dictionary<String, String> noCacheHeaders, RelativeLayout surveyLoadingContainer)
        {
            this.surveyLoadingContainer = surveyLoadingContainer;
            this.context = context;
            this.noCacheHeaders = noCacheHeaders;
        }
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {

            if (url != null && !(url.StartsWith("http") && !url.Contains("/thankyou.ww")))
            {
                view.LoadUrl(url, noCacheHeaders);
            }
            else
            {
                surveyLoadingContainer.Visibility=ViewStates.Gone;
            }
            return false;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            surveyLoadingContainer.Visibility = ViewStates.Gone;
        }
        public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            surveyLoadingContainer.Visibility = ViewStates.Visible;
        }
    }
}