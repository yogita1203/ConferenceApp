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
using CommonLayer;
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using Android.Util;

namespace ConferenceAppDroid
{
    [Activity(Label = "HandsOnLabsActivity")]
    public class HandsOnLabsDetailActivity : FragmentActivity
    {
        private Context context;
        private TextView handsOnTitleTextView;
        private TextView handsOnAbbreviationTextView;
        private TextView handsOnDescriptionTextView;
        private String id;
        public BuiltHandsonLabs currentHOl;
        private View actionBarView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private TextView holDetail_textview_id;
        private TextView holDetail_twitter_hash_tag;
        private RelativeLayout holSpeakerContainer;
        private TextView holDetails_speakerSectionTextView;
        private TextView holDetail_textview_id_label;
        private TextView holDetail_twitter_hash_tag_label;
        private LinearLayout hol_speaker_list_container;
        private TextView holDetail_textview_duration_label;
        private TextView holDetail_textview_duration_value;
        private TextView holDetail_textview_capacity_label;
        private TextView holDetail_textview_capacity_value;
        ImageView back_btn;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            SetContentView(Resource.Layout.activity_hands_on_labs_detail);
            id=Intent.GetStringExtra("SessionID");
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            back_btn = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
            {
                Finish();
            };

            init();
            leftMenuBtn.Visibility=ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Visible;
            titleTextView.Text = "LAB";
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

        private void init()
        {
            handsOnTitleTextView = (TextView)FindViewById(Resource.Id.handsOnTitleTextView);
            handsOnAbbreviationTextView = (TextView)FindViewById(Resource.Id.handsOnAbbreviationTextView);
            handsOnDescriptionTextView = (TextView)FindViewById(Resource.Id.handsOnDescriptionTextView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);

            holDetail_textview_id = (TextView)FindViewById(Resource.Id.holDetail_textview_value);
            holDetail_twitter_hash_tag = (TextView)FindViewById(Resource.Id.holDetail_twitter_hash_tag_value);
            holSpeakerContainer = (RelativeLayout)FindViewById(Resource.Id.holSpeakerContainer);
            holDetails_speakerSectionTextView = (TextView)FindViewById(Resource.Id.holDetails_speakerSectionTextView);
            holDetail_textview_id_label = (TextView)FindViewById(Resource.Id.holDetail_textview_id);
            holDetail_twitter_hash_tag_label = (TextView)FindViewById(Resource.Id.holDetail_twitter_hash_tag_id);
            hol_speaker_list_container = (LinearLayout)FindViewById(Resource.Id.hol_speaker_list_container);
            holDetail_textview_duration_label = (TextView)FindViewById(Resource.Id.holDetail_textview_duration_label);
            holDetail_textview_duration_value = (TextView)FindViewById(Resource.Id.holDetail_textview_duration_value);
            holDetail_textview_capacity_label = (TextView)FindViewById(Resource.Id.holDetail_textview_capacity_label);
            holDetail_textview_capacity_value = (TextView)FindViewById(Resource.Id.holDetail_textview_capacity_value);

            DataManager.GetHandsOnLabsUsingSessionId(DBHelper.Instance.Connection,id).ContinueWith(t =>
            {
                currentHOl = t.Result;
                RunOnUiThread(() =>
                {
                    setDisplay();
                });

            });

            //handsOnTitleTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetail_textview_id_label.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //holDetail_twitter_hash_tag_label.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //holDetail_textview_duration_label.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //holDetail_textview_capacity_label.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //handsOnAbbreviationTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //handsOnDescriptionTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetail_textview_id.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetail_twitter_hash_tag.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetail_textview_duration_value.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetail_textview_capacity_value.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //holDetails_speakerSectionTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
        }

        private void setDisplay()
        {
            handsOnTitleTextView.Text = currentHOl.title;
            handsOnDescriptionTextView.Text = currentHOl._abstract;

            holDetail_textview_id.Text = currentHOl.session_id;
            if (!string.IsNullOrWhiteSpace(currentHOl.hashtag))
            {
                holDetail_twitter_hash_tag_label.Visibility = ViewStates.Visible;
                holDetail_twitter_hash_tag.Visibility = ViewStates.Visible;
                holDetail_twitter_hash_tag.Text = currentHOl.hashtag;
            }
            else
            {
                holDetail_twitter_hash_tag_label.Visibility = ViewStates.Gone;
                holDetail_twitter_hash_tag.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(currentHOl.capacity))
            {
                holDetail_textview_capacity_label.Visibility = ViewStates.Visible;
                holDetail_textview_capacity_value.Visibility = ViewStates.Visible;
                holDetail_textview_capacity_value.Text = currentHOl.capacity;
            }
            else
            {
                holDetail_textview_capacity_label.Visibility = ViewStates.Gone;
                holDetail_textview_capacity_value.Visibility = ViewStates.Gone;
            }

            if (string.IsNullOrWhiteSpace(currentHOl.duration))
            {
                holDetail_textview_duration_label.Visibility = ViewStates.Visible;
                holDetail_textview_duration_value.Visibility = ViewStates.Visible;
                holDetail_textview_duration_value.Text = currentHOl.duration;
            }
            else
            {
                holDetail_textview_duration_label.Visibility = ViewStates.Gone;
                holDetail_textview_duration_value.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(currentHOl.speaker_separated))
            {
                var speaker = currentHOl.speaker_separated;
                hol_speaker_list_container.Visibility = ViewStates.Visible;
           
                if (hol_speaker_list_container.ChildCount > 0)
                {
                    hol_speaker_list_container.RemoveAllViews();
                }
                if (speaker.Contains(","))
                {
                    string[] values = speaker.Split(',');
                    if (values.Length== 1)
                    {
                        holDetails_speakerSectionTextView.Text = values.Length + "Speaker";
                    }
                    else
                    {
                        holDetails_speakerSectionTextView.Text = values.Length + " Speakers";
                    }
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView speakerNameTv = new TextView(context);
                        String speakerName = values[i];
                        speakerNameTv.Text=speakerName.Trim();
                        
                        LinearLayout.LayoutParams lineParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, dpToPx(context, 40));
                        speakerNameTv.SetPadding(dpToPx(context, 15), 0, dpToPx(context, 15), 0);
                        speakerNameTv.Gravity=GravityFlags.CenterVertical;
                        speakerNameTv.SetBackgroundResource(Resource.Color.white);

                        hol_speaker_list_container.AddView(speakerNameTv, lineParams);
                    }
                }

            }

            else  
            {
                        holSpeakerContainer.Visibility=ViewStates.Gone;
            }


        }
        public static int dpToPx(Context context, int dp)
        {
            int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
            return px;
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