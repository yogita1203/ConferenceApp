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
using Android.Support.V4.App;
using Android.Webkit;
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid
{
    [Activity(Label = "UIWebViewForSurvey")]
    public class UIWebViewForSurvey : FragmentActivity
    {
        private Context context;
        private WebView webView;
        private View actionBarView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private ImageButton back_btn;
        private String url;
        private Dictionary<String, String> noCacheHeaders;
        private RelativeLayout surveyLoadingContainer;
        public UIWebViewForSurvey()
        {
            context = this;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);

            ActionBar.CustomView = actionBarView;
            SetContentView(Resource.Layout.activity_web_view_for_survey);
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            init();
            initWebView();

            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Gone;
            back_btn.Visibility = ViewStates.Visible;

            back_btn.Click += (s, e) =>
               {
                   Finish();
               };
            titleTextView.Text = "Survey".ToUpper();

            if (Intent.Extras != null && Intent.Extras.ContainsKey("url"))
            {
                url = Intent.GetStringExtra("url");
                OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
                if (url != null)
                {
                    webView.LoadUrl(url, noCacheHeaders);
                }
            }
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);

        }

        protected override void OnResume()
        {
            base.OnResume();
            new AppUtilities().isFromPauseResume(this, false);
        }

        protected override void OnPause()
        {
            base.OnPause();
            new AppUtilities().isFromPauseResume(this, true);
        }

        private void initWebView()
        {
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.SetRenderPriority(WebSettings.RenderPriority.High);
            webView.Settings.CacheMode = CacheModes.Default;
            webView.Settings.SetAppCacheEnabled(false);
            webView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            webView.Settings.LightTouchEnabled = false;
            webView.Settings.UseWideViewPort = true;

            noCacheHeaders = new Dictionary<String, String>(2);
            noCacheHeaders.Add("Pragma", "no-cache");
            noCacheHeaders.Add("Cache-Control", "no-cache");

            webView.SetWebChromeClient(new SurveyCustomWebChromeClient(surveyLoadingContainer));
            webView.SetWebViewClient(new SurveyCustomWebViewClient(this, noCacheHeaders, surveyLoadingContainer));
        }

        private void init()
        {
            webView = (WebView)FindViewById(Resource.Id.webView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);

            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);
            surveyLoadingContainer = (RelativeLayout)FindViewById(Resource.Id.surveyLoading);
            surveyLoadingContainer.Clickable=true;
        }

    }
}