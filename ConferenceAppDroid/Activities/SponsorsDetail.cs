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
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Utilities;
using Android.Support.V4.App;
using Java.IO;
using ConferenceAppDroid.Fragments;
using ConferenceAppDroid.Interfaces;
using CommonLayer;

namespace ConferenceAppDroid.Activities
{
    [Activity(Label = "SponsorsDetail")]
    public class SponsorsDetail : FragmentActivity, ILoadFragment
    {
        private Context context;
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
        private int position;
        private String fromScreen;
        private BuiltExhibitor sponsorModelList;
        private TextView twitterImageView;
        private String sponsorSearchText;
        private int tempPosition;
        private long totalCount;
        private SessionDayDetailFragmentAdapter mAdapter;
        private const int SPONSOR_DETAILS_HANDLER = 4;
        private ImageButton back_btn;
        public List<BuiltExhibitor> lstExhibitor = new List<BuiltExhibitor>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            context = this;


            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = (actionBarView);
            SetContentView(Resource.Layout.activity_session_detail_screen);
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            init();

            starUnstarImageView.Visibility = ViewStates.Gone;
            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
            {
                Finish();
            };
            twitterImageView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(this, twitterImageView, "&#xf099;");
            twitterImageView.Text = lmtUnicode;

            if (Intent.Data != null)
            {
                String data = Intent.Data.ToString();
                if (data.Contains("/exhibitor"))
                {
                    String _id = data.Substring(data.IndexOf("vmwareapp://exhibitor/") + 22, data.Length);
                    //                sponsorModelList = queryOrm.getExhibitorById(_id);
                }
            }

            if (Intent.Extras != null)
            {
                id = Intent.GetStringExtra("id");
                position = Intent.GetIntExtra("position", 0);
                fromScreen = Intent.GetStringExtra("from");
                sponsorSearchText = Intent.GetStringExtra("sponsor_search_text");


                getAllExhibitorsByOrder(res =>
                {
                    lstExhibitor = res;
                    if (fromScreen != null && fromScreen.Equals((AppConstants.SPONSORS_BY_CONTRIBUTION), StringComparison.InvariantCultureIgnoreCase))
                    {
                        new FetchSponsorsCountAsync(this).Execute();
                        //                sponsorModelList = SponsorByContribution.hashMap.get(id);

                    }
                    else if (fromScreen != null && fromScreen.Equals((AppConstants.APPUTILITIES), StringComparison.InvariantCultureIgnoreCase))
                    {
                        new FetchSponsorsCountAsync(this).Execute();
                    }
                });


            }
            // Create your application here
        }

        private void getAllExhibitorsByOrder(Action<List<BuiltExhibitor>> callback)
        {
            DataManager.GetAllExhibitorsByOrderInAndroid(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                totalCount = t.Result.Count;
                if (callback != null)
                {
                    callback(res);
                }
            });
        }



        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }

        private void init()
        {
            viewPager = (CustomViewPagerWithNoScroll)FindViewById(Resource.Id.pager);
            viewPager.SetPagingEnabled(false);
            leftArrowImageView = (ImageView)FindViewById(Resource.Id.leftArrowImageView);
            rightArrowImageView = (ImageView)FindViewById(Resource.Id.rightArrowImageView);
            starUnstarImageView = (ImageView)FindViewById(Resource.Id.starUnstarImageView);
            twitterImageView = (TextView)FindViewById(Resource.Id.twitterImageView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);

            //titleTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));




        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        private void updateUi(int sponsorPosition, long countTotal, List<BuiltExhibitor> sponsorList)
        {
            Bundle bundle = new Bundle();
            if (sponsorList != null)
            {

                //bundle.PutSerializable("speakerList", (Java.IO.ISerializable)sponsorList);
            }
            //SpeakerDetailFragment_v2 speakerDetailFragment_v2 = new SpeakerDetailFragment_v2();
            mAdapter.addFragment(SponsorDetailFragment.newInstance(context, sponsorPosition, fromScreen, id, countTotal, sponsorList, sponsorSearchText));

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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("id", id);
            outState.PutString("from", fromScreen);
            outState.PutInt("position", position);
            outState.PutString("sponsor_search_text", sponsorSearchText);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            if (savedInstanceState != null)
            {
                id = savedInstanceState.GetString("id");
                fromScreen = savedInstanceState.GetString("from");
                position = savedInstanceState.GetInt("position");
                sponsorSearchText = savedInstanceState.GetString("sponsor_search_text");

                if (fromScreen != null && fromScreen.Equals((AppConstants.SPONSORS_BY_CONTRIBUTION), StringComparison.InvariantCultureIgnoreCase))
                {
                    //new FetchSponsorsCountAsync().execute();
                }
                else if (fromScreen != null && fromScreen.Equals(AppConstants.APPUTILITIES, StringComparison.InvariantCultureIgnoreCase))
                {
                    //new FetchSponsorsCountAsync().execute();
                }
            }
        }

        public void loadFragment(bool load, string fragmentName, object @object)
        {
            sponsorModelList = (BuiltExhibitor)@object;
            titleTextView.Text = (sponsorModelList.name.ToUpper());
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

        private void addData(long totalCount, int position, int whichType, Object obj)
        {
            //        final List<SpeakerModel> speakerList = (List<SpeakerModel>) obj;
            tempPosition = position;
            this.totalCount = totalCount;
            switch (whichType)
            {
                case SPONSOR_DETAILS_HANDLER:


                    mAdapter = new SessionDayDetailFragmentAdapter(context,SupportFragmentManager);
                    viewPager.Adapter = (mAdapter);
                    updateUi(position, totalCount, lstExhibitor);


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
                                updateUi(tempPosition, totalCount, lstExhibitor);
                                AppConstants.TEMP_POSITION = tempPosition;
                            }
                        };
                    rightArrowImageView.Click += (s, e) =>
                        {
                            if (tempPosition < totalCount - 1)
                            {
                                mAdapter.clear();
                                tempPosition++;
                                updateUi(tempPosition, totalCount, lstExhibitor);
                                AppConstants.TEMP_POSITION = tempPosition;
                            }
                        };

                    break;


            }

        }


        private class FetchSponsorsCountAsync : AsyncTask
        {
            SponsorsDetail sponsorsDetail;
            public FetchSponsorsCountAsync(SponsorsDetail sponsorsDetail)
            {
                this.sponsorsDetail=sponsorsDetail;
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                if (sponsorsDetail.fromScreen != null && sponsorsDetail.fromScreen.Equals(AppConstants.SPONSORS_BY_CONTRIBUTION,StringComparison.InvariantCultureIgnoreCase))
                {

                    sponsorsDetail.totalCount = 90;
                    //sponsorsDetail.totalCount = queryOrm.getAllSponsorsByContributionCount(sponsorSearchText);
                }
                else if (sponsorsDetail.fromScreen != null && sponsorsDetail.fromScreen.Equals(AppConstants.APPUTILITIES,StringComparison.InvariantCultureIgnoreCase))
                {
                    sponsorsDetail.totalCount = 1;
                }

                return sponsorsDetail.totalCount;

            }
            protected override void OnPostExecute(Java.Lang.Object result)
            {
                if (result != null)
                {
                    Message msg = new Message();
                    if (result != null)
                    {
                        msg.What = SPONSOR_DETAILS_HANDLER;
                        //                    msg.obj = speakerModelList;
                        Bundle bundle = new Bundle();
                        bundle.PutLong("sponsorCount", sponsorsDetail.totalCount);
                        msg.Data=(bundle);
                        var t=new Handler(new MyHandler(sponsorsDetail));
                        t.SendMessage(msg);
                    }
                }
            }
    }


        public class MyHandler : Java.Lang.Object, Android.OS.Handler.ICallback
        {
            SponsorsDetail sponsorsDetail;
            public MyHandler(SponsorsDetail sponsorsDetail)
            {
                this.sponsorsDetail=sponsorsDetail;
            }
            public new bool HandleMessage(Message msg)
            {
                if (msg.What == SPONSOR_DETAILS_HANDLER)
                {
                    try
                    {
                        if (msg.Data!= null)
                        {
                            Bundle bundle = msg.Data;

                            sponsorsDetail.addData(bundle.GetLong("sponsorCount"), sponsorsDetail.position, SPONSOR_DETAILS_HANDLER, msg.Obj);

                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                return false;
            }

        }

        


        public class SessionDayDetailFragmentAdapter : FragmentStatePagerAdapter
        {
            private List<Android.Support.V4.App.Fragment> views = new List<Android.Support.V4.App.Fragment>();
            public SessionDayDetailFragmentAdapter(Context context, Android.Support.V4.App.FragmentManager fm)
                : base(fm)
            {

                views = new List<Android.Support.V4.App.Fragment>();
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                return views[position];
            }

            public override int GetItemPosition(Java.Lang.Object @object)
            {
                return PositionNone;
            }

            public void addFragment(Android.Support.V4.App.Fragment v)
            {
                views.Add(v);
                NotifyDataSetChanged();
            }

            public void addSpeakerFragment(Android.Support.V4.App.Fragment v, int speakerPosition, String from, String id, long countTotal, List<BuiltSpeaker> speakerModelList)
            {
                Bundle bundle = new Bundle();
                bundle.PutInt("speakerPosition", speakerPosition);
                bundle.PutString("from", from);
                bundle.PutString("id", id);
                bundle.PutLong("countTotal", countTotal);
                //bundle.PutSerializable("speakerList", (ISerializable)speakerModelList);
                v.Arguments = (bundle);
                views.Add(v);
                NotifyDataSetChanged();
            }
            public void clear()
            {
                for (int i = 0; i < views.Count; i++)
                {
                    views.Clear();
                }
            }

            public override int Count
            {
                get { return views.Count; }
            }

            public override void DestroyItem(View container, int position, Java.Lang.Object @object)
            {
                View view = (View)@object;
                ((Android.Support.V4.View.ViewPager)container).RemoveView(view);
                views.RemoveAt(position);
            }


        }
    }
}