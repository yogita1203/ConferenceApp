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
using CommonLayer.Entities.Built;
using ConferenceAppDroid.CustomControls;
using Android.Support.V4.App;
using ConferenceAppDroid.Utilities;
using System.Threading.Tasks;
using ConferenceAppDroid.Interfaces;
using ConferenceAppDroid.Fragments;
using CommonLayer;
using Newtonsoft.Json;

namespace ConferenceAppDroid.Activities
{
    [Activity(Label = "SpeakerDetailActivity")]
    public class SpeakerDetailActivity : FragmentActivity, ILoadFragment
    {
        private const int SPEAKER_DETAILS_HANDLER = 4;
        private View actionBarView;
        private CustomViewPagerWithNoScroll viewPager;
        private ImageView leftArrowImageView;
        private ImageView rightArrowImageView;
        private ImageView starUnstarImageView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private String id;
        private int position = 0;
        public String from;
        private Context context;
        private TextView twitterImageView;
        private String speakerSearchText;
        private int tempPosition;
        private ConferenceAppDroid.Activities.SponsorsDetail.SessionDayDetailFragmentAdapter mAdapter;
        private BuiltSpeaker speakerModel;
        private ImageButton back_btn;
        public List<BuiltSpeaker> speakerModelList;
        private int speakerCount;
        private long totalCount;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.context = this;

            // if(AppConstants.IS_PEX_EVENT){
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            /*}else{
                overridePendingTransition(R.anim.up_from_bottom, R.anim.hold);
            }*/
            SetContentView(Resource.Layout.activity_speaker_details);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = (actionBarView);
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            viewPager = (CustomViewPagerWithNoScroll)FindViewById(Resource.Id.pager);
            viewPager.SetPagingEnabled(false);
            leftArrowImageView = (ImageView)FindViewById(Resource.Id.leftArrowImageView);
            rightArrowImageView = (ImageView)FindViewById(Resource.Id.rightArrowImageView);
            starUnstarImageView = (ImageView)FindViewById(Resource.Id.starUnstarImageView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);
            twitterImageView = (TextView)FindViewById(Resource.Id.twitterImageView);

            //titleTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            // if(AppConstants.IS_PEX_EVENT){
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
                {
                    Finish();
                };
            //back_btn.setOnClickListener(onClickListener);
            /*}else{
                bottomImageView.setVisibility(View.VISIBLE);
                bottomImageView.setOnClickListener(onClickListener);
            }*/
            twitterImageView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(this, twitterImageView, "&#xf099;");
            twitterImageView.Text = lmtUnicode;

            starUnstarImageView.Visibility = ViewStates.Gone;
            //twitterImageView.setOnClickListener(onClickListener);
            titleTextView.Text = (Resources.GetString(Resource.String.speaker_text).ToUpper() + "  ");
            // Create your application here

            if (Intent.Data != null)
            {
                String data = Intent.Data.ToString();
                if (data.Contains("/speaker"))
                {
                    String _id = data.Substring(data.IndexOf("vmwareapp://speaker/") + 20, data.Length);
                    //                speakerModelList = (ArrayList<SpeakerModel>) queryOrm.getSpeakerByIdUserRef(_id);
                }
            }

          

            if (Intent.Extras != null)
            {
                id = Intent.GetStringExtra("id");
                position = Intent.GetIntExtra("position", 0);
                from = Intent.GetStringExtra("from");
                speakerSearchText = Intent.GetStringExtra("search_speaker_text");
                

                if (Intent.GetSerializableExtra("speakerList") != null)
                {
                    speakerModelList = (List<BuiltSpeaker>)Intent.GetSerializableExtra("speakerList");
                    FetchSpeakerCount().ContinueWith(t =>
                        {
                            Message msg = new Message();
                            int count = t.Result;
                            if (count !=0 )
                            {
                                try
                                {
                                    addData(count, position, SPEAKER_DETAILS_HANDLER, speakerModelList);
                                }
                                catch (Exception e)
                                { }
                            }
                        });

                }
                else
                {
                        speakerModelList = AppSettings.Instance.speakerScreen;

                        FetchSpeakerCount().ContinueWith(t =>
                        {
                            Message msg = new Message();
                            int count = t.Result;
                            if (count != 0)
                            {
                                try
                                {
                                    RunOnUiThread(() =>
                                        {
                                            addData(count, position, SPEAKER_DETAILS_HANDLER, speakerModelList);
                                        });
                                }
                                catch (Exception e)
                                { }
                            }
                        });
                }
            }

        }

         

        private void addData(int count, int position, int whichType, List<BuiltSpeaker> speakerModelList)
        {
            List<BuiltSpeaker> speakerList = speakerModelList;
            tempPosition = position;
            this.totalCount = count;
            switch (whichType)
            {
                case SPEAKER_DETAILS_HANDLER:


                    mAdapter = new ConferenceAppDroid.Activities.SponsorsDetail.SessionDayDetailFragmentAdapter(context, SupportFragmentManager);
                    viewPager.Adapter = (mAdapter);
                    updateUi(position, totalCount, speakerList);


                    leftArrowImageView.Visibility = ViewStates.Visible;
                    rightArrowImageView.Visibility = ViewStates.Visible;
                    leftArrowImageView.SetBackgroundResource(Resource.Drawable.selector_left_blue_tracks_session_detail);
                    rightArrowImageView.SetBackgroundResource(Resource.Drawable.selector_right_blue_tracks_session_detail);

                    leftArrowImageView.Click += (s, e) =>
                    {
                        if (tempPosition != 0)
                        {
                            mAdapter.clear();
                            tempPosition--;
                            updateUi(tempPosition, totalCount, speakerList);
                            AppConstants.TEMP_POSITION = tempPosition;
                        }
                    };

                    rightArrowImageView.Click += (s, e) =>
                        {
                            if (tempPosition < totalCount - 1)
                            {
                                mAdapter.clear();
                                tempPosition++;
                                updateUi(tempPosition, totalCount, speakerList);
                                AppConstants.TEMP_POSITION = tempPosition;
                            }
                        };

                    break;
            }
        }

        private void updateUi(int speakerPosition, long countTotal, List<BuiltSpeaker> speakerList)
        {
            Bundle bundle = new Bundle();
            if (speakerList != null)
            {

                //bundle.PutSerializable("speakerList", (java.io.Serializable)speakerList);
            }
            mAdapter.addSpeakerFragment(SpeakerDetailFragment.newInstance(this), speakerPosition, from, id, countTotal, speakerList);
        }
        Task<int> FetchSpeakerCount()
        {
            return Task.Run<int>(() =>
            {
                if (from != null && from.Equals((AppConstants.SESSION_DETAIL_PAGER_ADAPTER), StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        if (speakerModelList != null && speakerModelList.Count > 0)
                        {
                            speakerCount = speakerModelList.Count;
                        }
                    }
                    catch (Exception e)
                    {
                    }

                }
                else if (from != null && from.Equals((AppConstants.MY_INTEREST_SPEAKER_FRAGMENT), StringComparison.InvariantCultureIgnoreCase))
                {
                }
                else if (from != null && from.Equals((AppConstants.APPUTILITIES), StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        if (speakerModelList != null && speakerModelList.Count > 0)
                        {
                            speakerCount = speakerModelList.Count;
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    speakerCount = speakerModelList.Count; ;
                    //speakerCount = orm.searchSpeakersCount(AppConstants.SPEAKER_SEARCH_TEXT);
                }
                return speakerCount;

            });


        }

        public void loadFragment(bool load, string fragmentName, object @object)
        {
            speakerModel = (BuiltSpeaker)@object;
            if (tempPosition == 0 && totalCount == 1)
            {
                leftArrowImageView.SetBackgroundResource(Resource.Drawable.bg_shape_left_blue_tracks_disabled);
                rightArrowImageView.SetBackgroundResource(Resource.Drawable.bg_shape_right_blue_tracks_disabled);
            }
            else if (tempPosition > 0 && (tempPosition + 1) == totalCount)
            {
                leftArrowImageView.SetBackgroundResource(Resource.Drawable.selector_left_blue_tracks_session_detail);
                rightArrowImageView.SetBackgroundResource(Resource.Drawable.bg_shape_right_blue_tracks_disabled);
            }
            else if (tempPosition == 0 && totalCount > 0)
            {
                leftArrowImageView.SetBackgroundResource(Resource.Drawable.bg_shape_left_blue_tracks_disabled);
                rightArrowImageView.SetBackgroundResource(Resource.Drawable.selector_right_blue_tracks_session_detail);
            }
            else if (tempPosition > 0 && totalCount > 0)
            {
                leftArrowImageView.SetBackgroundResource(Resource.Drawable.selector_left_blue_tracks_session_detail);
                rightArrowImageView.SetBackgroundResource(Resource.Drawable.selector_right_blue_tracks_session_detail);
            }

        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutString("from", from);
            outState.PutInt("position", position);
            outState.PutString("search_speaker_text", speakerSearchText);
            outState.PutSerializable("speakerList", (Java.IO.ISerializable)Intent.GetSerializableExtra("speakerList"));
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            if (savedInstanceState != null)
            {
                id = savedInstanceState.GetString("id");
                from = savedInstanceState.GetString("from");
                position = savedInstanceState.GetInt("position");
                speakerSearchText = savedInstanceState.GetString("search_speaker_text");

                Message msg = new Message();
                Bundle bundle = new Bundle();
                bundle.PutInt("speakerPosition", position);
                bundle.PutString("id", id);

                if (savedInstanceState.GetSerializable("speakerList") != null)
                {
                    List<BuiltSpeaker> speakerModelList = (List<BuiltSpeaker>)savedInstanceState.GetSerializable("speakerList");
                    //FetchSpeakerCount fetchSpeakerCount = new FetchSpeakerCount();
                    //fetchSpeakerCount.execute(speakerModelList);
                }
                else
                {
                    //FetchSpeakerCount fetchSpeakerCount = new FetchSpeakerCount();
                    //fetchSpeakerCount.execute();
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            new AppUtilities().isFromPauseResume(this, true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            new AppUtilities().isFromPauseResume(this, false);
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }
    }
}