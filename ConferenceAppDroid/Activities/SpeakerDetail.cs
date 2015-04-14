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
using ConferenceAppDroid.CustomControls;
using Android.Support.V4.App;
using ConferenceAppDroid.Fragments;
using ConferenceAppDroid.Core;
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid
{
    [Activity(Label = "SpeakerDetail")]
    public class SpeakerDetail : FragmentActivity
    {
        View actionBarView;
        ImageView leftArrowImageView;
        ImageView rightArrowImageView;
        ImageView starUnstarImageView;
        TextView twitterImageView;
        ImageView scheduleImageView;
        ImageButton rightMenuBtn;
        ImageButton leftMenuBtn;
        TextView titleTextView;
        ImageView bottomImageView;
        ImageView back_btn;
        public string uid = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            uid = Intent.GetStringExtra("uid");
            SetContentView(Resource.Layout.activity_speaker_details);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            // Create your application here
            init();
            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Visible;
            twitterImageView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(this, twitterImageView, "&#xf099;");
            twitterImageView.Text = lmtUnicode;

            scheduleImageView.Visibility = ViewStates.Gone;
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
                {
                    Finish();
                };
            //titleTextView.SetTextColor(Android.Graphics.Color.White);
            titleTextView.Text = "SPEAKER";
            var mPager = FindViewById<CustomViewPagerWithNoScroll>(Resource.Id.pager);
            mPager.SetPagingEnabled(false);
            var speakerDetailAdapter = new SpeakerDetailAdapter(SupportFragmentManager);
            mPager.Adapter = speakerDetailAdapter;

        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }

        private void init()
        {
            leftArrowImageView = (ImageView)FindViewById(Resource.Id.leftArrowImageView);
            rightArrowImageView = (ImageView)FindViewById(Resource.Id.rightArrowImageView);
            starUnstarImageView = (ImageView)FindViewById(Resource.Id.starUnstarImageView);
            twitterImageView = (TextView)FindViewById(Resource.Id.twitterImageView);
            scheduleImageView = (ImageView)FindViewById(Resource.Id.scheduleImageView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);
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

    public class SpeakerDetailAdapter : FragmentPagerAdapter
    {

        public SpeakerDetailAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {

        }
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return new SpeakerDetailFragment();
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            Java.Lang.ICharSequence data = null;
            if (position == 0)
            {
                var temp = CharSequence.ArrayFromStringArray(new string[] { " all days" });
                data = temp[position];
                return data;
            }
            else
            {
                return data;
            }
        }
        public override int Count
        {
            get
            {
                return 1;
            }
        }


    }



}