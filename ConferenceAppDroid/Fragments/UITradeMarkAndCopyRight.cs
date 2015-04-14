using Android.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using CommonLayer.Entities.Built;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.Fragments
{
    public class UITradeMarkAndCopyRight:Android.App.Activity
    {
        private View actionBarView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private ImageButton back_btn;
        private ProgressDialog progressDialog;
        private String tradeMarkAndCopyRight;
        private WebView webView;
        private BuiltLegal legalModel;
        private RelativeLayout loadingContainer;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_terms_and_condition);
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView=(actionBarView);
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            init();
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Gone;
            leftMenuBtn.Visibility = ViewStates.Gone;
            back_btn.Visibility = ViewStates.Visible;

            loadingContainer = (RelativeLayout)FindViewById(Resource.Id.loadingContainer);
            loadingContainer.Visibility=ViewStates.Visible;
            TextView loadingText = (TextView)loadingContainer.FindViewById(Resource.Id.loadingText);
            //loadingText.setTypeface(Typeface.createFromAsset(getAssets(), AppConstants.FONT_MEDIUM));
            loadingText.SetTextColor(Resources.GetColor(Resource.Color.white));
        }

        private void init()
        {
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);
            //titleTextView.setTypeface(Typeface.createFromAsset(this.getAssets(), AppConstants.FONT));
            webView = (WebView)FindViewById(Resource.Id.webView);
        }
        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }
    }
}
