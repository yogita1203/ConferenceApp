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
using ConferenceAppDroid.Fragments;
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid
{
    [Activity(Label = "HandsOnLabsActivity")]
    public class HandsOnLabsActivity : FragmentActivity
    {
        View actionBarView;
        private String title;
        ImageView back_btn;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_uihands_on_labs);

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            back_btn = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
            {
                Finish();
            };

            (actionBarView.FindViewById(Resource.Id.left_menu_btn)).Visibility=ViewStates.Gone;
            (actionBarView.FindViewById(Resource.Id.right_menu_btn)).Visibility = ViewStates.Invisible;
            (actionBarView.FindViewById(Resource.Id.bottomImageView)).Visibility = ViewStates.Gone;
            (actionBarView.FindViewById(Resource.Id.back_btn)).Visibility = ViewStates.Visible;
            TextView titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            // Create your application here
            title=Intent.GetStringExtra("title");
            titleTextView.Visibility = ViewStates.Visible;
            titleTextView.Text = title.ToUpper();
             OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
             if (bundle == null)
             {
                 var fragmentTransaction = SupportFragmentManager.BeginTransaction();
                 fragmentTransaction.Add(Resource.Id.container, new HandsOnLabsFragment(), "hol").Commit();
             }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
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

    }
}