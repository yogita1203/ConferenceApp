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
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid
{
    [Activity(Label = "DailyHighlightsDetailsActivity")]
    public class DailyHighlightsDetailsActivity : Activity
    {
        TextView txtTitle;
        TextView txtDescription;
        TextView txtLink;
        TextView txtDate, titleTextView;
        View actionBarView;
        ImageView back_btn;
        ImageButton rightMenuBtn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Window.RequestFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.NewsDetails);

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            rightMenuBtn.Visibility = ViewStates.Invisible;

            back_btn = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
            {
                Finish();
            };

            titleTextView = actionBarView.FindViewById<TextView>(Resource.Id.titleTextView);
            titleTextView.Text = "NEWS";

            txtTitle = FindViewById<TextView>(Resource.Id.newsDetail_textview_title);
            txtTitle.SetTypeface(txtTitle.Typeface, Android.Graphics.TypefaceStyle.Bold);
            txtDate = FindViewById<TextView>(Resource.Id.newsDetail_textview_publishedDate);
            txtDate.SetTypeface(txtTitle.Typeface, Android.Graphics.TypefaceStyle.Bold);
            txtDescription = FindViewById<TextView>(Resource.Id.newsDetail_textview_descr);
            txtDescription.SetTypeface(txtDescription.Typeface, Android.Graphics.TypefaceStyle.Normal);
            txtLink = FindViewById<TextView>(Resource.Id.newsDetail_textview_readMore);
            txtLink.SetTypeface(txtLink.Typeface, Android.Graphics.TypefaceStyle.Normal);
            var image = FindViewById<ImageView>(Resource.Id.newsDetail_coverImage);

            var title = Intent.GetStringExtra("Title");
            var date = Intent.GetStringExtra("Date");
            var desc = Intent.GetStringExtra("Description");
            var link = Intent.GetStringExtra("Link");


            if (!string.IsNullOrWhiteSpace(title))
            {
                txtTitle.Visibility = ViewStates.Visible;
                txtTitle.Text = title;
            }
            else
            {
                txtTitle.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(date))
            {
                txtDate.Visibility = ViewStates.Visible;
                txtDate.Text = Helper.convertToNewsDate(date);
            }
            else
            {
                txtDate.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(desc))
            {
                txtDescription.Visibility = ViewStates.Visible;
                txtDescription.Text = desc;
            }
            else {
                txtDescription.Visibility = ViewStates.Gone;
            }
            
            txtLink.Text = link;
            image.Visibility = ViewStates.Visible;
            //ActionBar.SetHomeButtonEnabled(true);
     
            // Create your application here
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
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
    }
}