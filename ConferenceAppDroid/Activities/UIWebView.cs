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
    [Activity(Label = "UIWebView")]
    public class UIWebView : FragmentActivity
    {
        private Context context;
        private WebView webView;
        private View actionBarView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private ImageButton back_btn;
        private String title;
        private ProgressBar progressBar;
        private String url;
        private bool isModal = false;
        private bool isPush = false;
        private Dictionary<String, String> noCacheHeaders;
        private bool isShowTitle = false;
        public UIWebView()
        {
            context = this;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_web_view);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            // Create your application here
            init();
            initWebView();

            leftMenuBtn.Visibility=ViewStates.Gone;
            rightMenuBtn.Visibility=ViewStates.Invisible;
            bottomImageView.Visibility=ViewStates.Gone;
            back_btn.Visibility=ViewStates.Visible;
            back_btn.Click += (s, e) =>
                {
                    OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
                    Finish();
                };

            if (Intent.Extras!= null)
            {
                isShowTitle = Intent.Extras.GetBoolean("isShowTitle", false);
                if (Intent.Extras.ContainsKey("orientation"))
                {
                    String orientation = Intent.Extras.GetString("orientation");
                    if (orientation.Equals(("push"),StringComparison.InvariantCultureIgnoreCase))
                    {
                        isPush = true;
                        OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
                    }
                    else if (orientation.Equals(("modal"),StringComparison.InvariantCultureIgnoreCase))
                    {
                        isModal = true;
                        OverridePendingTransition(Resource.Animation.slide_up, Resource.Animation.hold);
                    }
                    else
                    {
                        OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
                    }
                }
                else
                {
                    OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
                }

                if (Intent.Extras.ContainsKey("url")) 
                {
                url = Intent.GetStringExtra("url");
                if (url != null) {
                    webView.LoadUrl(url, noCacheHeaders);
                }


                if (Intent.Extras.ContainsKey("title")) 
                {
                    title = Intent.GetStringExtra("title");
                    if (title != null)
                    {
                        titleTextView.Text=title.ToUpper()+ " ";
                    } 
                    else
                    {
                        if (1==1)
                        {
                            titleTextView.Text="VMworld 14".ToUpper() + "  ";
                        } else
                        {
                            titleTextView.Text="VMWorld 2014".ToUpper();
                        }
                    }
                }
                else 
                {
                    if (1==1) {
                        titleTextView.Text="VMworld 14".ToUpper()+ "  ";
                    } else {
                        titleTextView.Text="VMWorld 2014".ToUpper();
                    }
                }
            }
            }

                    if (Intent.Data != null) {
            String data = Intent.Data.ToString();
            if (data.Contains("com.builtio.vmworld.event://vmwareapp/webview/vmwarelink")) 
            {
                Helper.isWebViewCalled = true;
                this.url = "http://www.vmware.com/go/patents";
                webView.LoadUrl(url, noCacheHeaders);
            }
        }
        }
        private void init()
        {
            webView = (WebView)FindViewById(Resource.Id.webView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);
            progressBar = (ProgressBar)FindViewById(Resource.Id.progrss);
        }

        private void initWebView()
        {
            webView.Settings.JavaScriptEnabled=true;
            //webView.Settings.UserAgentString=AppSettings.getUserAgent(context));
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

            webView.SetWebChromeClient(new CustomWebChromeClient(progressBar));
            webView.SetWebViewClient(new CustomWebViewClient(this, noCacheHeaders));


        }
        public override void Finish()
        {
            Helper.isWebViewCalled = false;
            base.Finish();
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

    //    protected override void onPause() 
    //       {
    //    base.OnPause();
    //    if (isModal) {
    //        isModal = false;
    //        OverridePendingTransition(Resource.Animation.hold, Resource.Animation.slide_down);
    //    } else if (isPush) {
    //        isPush = false;
    //        OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
    //    } else {
    //        OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
    //    }
    //    new AppUtilities().isFromPauseResume(UIWebView.this, true);
    //}

    //    protected override void onResume() {
    //    base.OnResume();
    //    new AppUtilities().isFromPauseResume(UIWebView.this, false);
    //}
    }
}