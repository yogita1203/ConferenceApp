using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ConferenceAppDroid.Core;
using SlidingMenuSharp;
using Android.Graphics;
using System.IO;
using SQLite.Net;
using SQLite.Net.Platform.XamarinAndroid;
using ConferenceAppDroid.Utilities;
using CommonLayer;
using Fragment = Android.Support.V4.App.Fragment;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Fragments;

namespace ConferenceAppDroid
{
    [Activity(Label = "ConferenceAppDroid")]
    //[Activity(Label = "Properties", Theme = "@style/ExampleTheme")]
    public class MainActivity : BaseActivity
    {
        public string exploreTrack { get; set; }
        protected Fragment contentFrag;
        MenuChangeReceiver menuChangeReceiver;
        public LoginReceivers loginReceiver;
        ExploreTrackReceiver exploreTrackReceiver;
        public InterestReceiver interestReceiver;
        public NotesReceiver notesReceiver;
        public DeltaStartedReceiver deltaStartedReceiver;
        public BackgroundUpdatesStartReceiver backgroundUpdatesStartReceiver;
        public DeltaCompletedReceiver deltaCompletedReceiver;
        public UpdateSessionsReceiver updateSessionsReceiver;
       

        string position;
        string url;
        View actionBarView;
        TextView titleTextView, left_menu_textview, searchTextView;
        ImageButton rightMenuBtn;
        public MainActivity()
            : base(Resource.String.ApplicationName, "Whats Happening")
        { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            titleTextView = actionBarView.FindViewById<TextView>(Resource.Id.titleTextView);

            left_menu_textview = actionBarView.FindViewById<TextView>(Resource.Id.left_menu_textview);
            left_menu_textview.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(this,left_menu_textview, "&#xf0c9;");
            left_menu_textview.Text = lmtUnicode;
            left_menu_textview.Click += left_menu_textview_Click;

            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            rightMenuBtn.Visibility = ViewStates.Invisible;


            searchTextView = actionBarView.FindViewById<TextView>(Resource.Id.searchTextView);
            //searchTextView.Visibility = ViewStates.Visible;
            //Typeface custom_font = Typeface.CreateFromAsset(Assets, "Fonts/FontAwesome.ttf");
            //searchTextView.Typeface = custom_font;
            //var str ="&#xf002";
            // var val = str.Substring(3).Replace(";", "");
            //int code = int.Parse(val, System.Globalization.NumberStyles.HexNumber);
            //string unicodeString = char.ConvertFromUtf32(code).ToString();
            //searchTextView.Text = unicodeString;

            SetSlidingActionBarEnabled(true);

            SetContentView(Resource.Layout.Main);

            ReplaceFragment(savedInstanceState);

            menuChangeReceiver = new MenuChangeReceiver();
            menuChangeReceiver.OnBroadcastReceive += menuChangeReceiver_OnBroadcastReceive;
            RegisterReceiver(menuChangeReceiver, new IntentFilter(MenuChangeReceiver.action));

            loginReceiver = new LoginReceivers();
            loginReceiver.OnBroadcastReceive += loginReceiver_OnBroadcastReceive;
            RegisterReceiver(loginReceiver, new IntentFilter(LoginReceivers.action));

            interestReceiver = new InterestReceiver();
            notesReceiver = new NotesReceiver();
            deltaStartedReceiver = new DeltaStartedReceiver();
            deltaCompletedReceiver = new DeltaCompletedReceiver();
            updateSessionsReceiver = new UpdateSessionsReceiver();
            backgroundUpdatesStartReceiver = new BackgroundUpdatesStartReceiver();
            backgroundUpdatesStartReceiver.OnBroadcastReceive += backgroundUpdatesStartReceiver_OnBroadcastReceive;
            RegisterReceiver(backgroundUpdatesStartReceiver, new IntentFilter(BackgroundUpdatesStartReceiver.action));

            SlidingMenu.Mode = MenuMode.Left;
            SlidingMenu.ShadowDrawableRes = Resource.Drawable.shadow;
            SlidingMenu.TouchModeAbove = TouchMode.Margin;
            //var scrollScale = FindViewById<SeekBar>(Resource.Id.scroll_scale);
            //scrollScale.Max = 1000;
            //scrollScale.Progress = 333;
            //scrollScale.StopTrackingTouch += (sender, args) =>
            //{
            //    SlidingMenu.BehindScrollScale = (float)args.SeekBar.Progress / args.SeekBar.Max;
            //};
            SlidingMenu.BehindScrollScale = 1;

            //var behindWidth = FindViewById<SeekBar>(Resource.Id.behind_width);
            //behindWidth.Max = 1000;
            //behindWidth.Progress = 750;
            //behindWidth.StopTrackingTouch += (sender, args) =>
            //{
            //    var percent = (float)args.SeekBar.Progress / args.SeekBar.Max;
            //    SlidingMenu.BehindWidth = (int)(percent * SlidingMenu.Width);
            //    SlidingMenu.RequestLayout();
            //};

            //var shadowEnabled = FindViewById<CheckBox>(Resource.Id.shadow_enabled);
            //shadowEnabled.Checked = true;
            //shadowEnabled.CheckedChange += (sender, args) =>
            //{
            //    if (args.IsChecked)
            //        SlidingMenu.ShadowDrawableRes = SlidingMenu.Mode == MenuMode.Left
            //                                            ? Resource.Drawable.shadow
            //                                            : Resource.Drawable.shadowright;
            //    else
            //        SlidingMenu.ShadowDrawable = null;
            //};

            //var shadowWidth = FindViewById<SeekBar>(Resource.Id.shadow_width);
            //shadowWidth.Max = 1000;
            //shadowWidth.Progress = 75;
            //shadowWidth.StopTrackingTouch += (sender, args) =>
            //{
            //    var percent = (float)args.SeekBar.Progress / args.SeekBar.Max;
            //    var width = (int)(percent * SlidingMenu.Width);
            //    SlidingMenu.ShadowWidth = width;
            //    SlidingMenu.Invalidate();
            //};

            //var fadeEnabled = FindViewById<CheckBox>(Resource.Id.fade_enabled);
            //fadeEnabled.Checked = true;
            //fadeEnabled.CheckedChange += (sender, args) =>
            //{
            //    SlidingMenu.FadeEnabled = args.IsChecked;
            //};

            //var fadeDeg = FindViewById<SeekBar>(Resource.Id.fade_degree);
            //fadeDeg.Max = 1000;
            //fadeDeg.Progress = 666; //NUMBER OF THE BEAST!
            //fadeDeg.StopTrackingTouch += (sender, args) =>
            //{
            //    SlidingMenu.FadeDegree = (float)args.SeekBar.Progress / args.SeekBar.Max;
            //};
        }

        void backgroundUpdatesStartReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            LeftMenuFragment mSomeFragment = (LeftMenuFragment)SupportFragmentManager.FindFragmentByTag("left_fragment");

            DataManager.CheckIfLatestDataAvailable(DBHelper.Instance.Connection, (res) =>
            {
                if (res)
                {
                    //TitleHeaderView.previousTitle = TitleHeaderView.refreshTitleSelected;
                    RunOnUiThread(() =>
                    {
                        mSomeFragment.updateOnSync();
                        //NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.LATEST_DATA_AVAILABLE, null);
                    });
                }
                else
                {
                    //TitleHeaderView.previousTitle = TitleHeaderView.upToDate;
                    RunOnUiThread(() =>
                    {
                        mSomeFragment.changeSyncInfoText("updated......");
                        mSomeFragment.startRefreshAnimation();
                    });
                }
            });
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (loginReceiver != null)
            {
                UnregisterReceiver(loginReceiver);
            }
            if (menuChangeReceiver != null)
            {
                UnregisterReceiver(menuChangeReceiver);
            }
        }

        private void loginReceiver_OnBroadcastReceive(Context arg1, Android.Content.Intent arg2)
        {
            LeftMenuFragment mSomeFragment = (LeftMenuFragment)SupportFragmentManager.FindFragmentByTag("left_fragment");
            mSomeFragment.loginReceiver_OnBroadcastReceive();
        }

        void left_menu_textview_Click(object sender, EventArgs e)
        {
            Toggle();
        }

        void menuChangeReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            position=arg2.GetIntExtra("position",10).ToString();
                url =arg2.GetStringExtra("url");
                ReplaceFragment(Bundle.Empty);
            
        }

        public void ReplaceFragment(Bundle savedInstanceState)
        {
            if (null == savedInstanceState)
            {
                setTitleText(Title);
                var t = SupportFragmentManager.BeginTransaction();
                //contentFrag = new MenuFragment();
                contentFrag = new whatsHappenning();
                t.Replace(Resource.Id.content_frame, contentFrag);
                t.Commit();
            }
            else
            {
                setTitleText(Title);
                var str=Helper.getStringToShowFragment(url);

                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (str.Equals("sessions", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new SessionsNSpeakers(false));
                        t.Commit();
                    }
                    if (str.Equals("news", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new DailyHighlights());
                        t.Commit();
                    }
                    if (str.Equals("others", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new ProgramsFaq());
                        t.Commit();
                    }
                    if (str.Equals("locationmoscone", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new Venue());
                        t.Commit();
                    }
                    if (str.Equals("locationsfo", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new SanFrancisco());
                        t.Commit();
                    }
                    if (str.Equals("explore", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new whatsHappenning());
                        t.Commit();
                    }

                    if (str.Equals("schedule", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new ScheduleInterest());
                        t.Commit();
                    }

                    if (str.Equals("survey", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new SurveysFragment());
                        t.Commit();
                    }

                    if (str.Equals("social", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new SocialFragment());
                        t.Commit();
                    }
                    if (str.Equals("notes", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new MyNotes());
                        t.Commit();
                    }
                    if (str.Equals("agenda", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new Agenda());
                        t.Commit();
                    }
                    if (str.Equals("sponsors", StringComparison.InvariantCultureIgnoreCase))
                    {
                        searchTextView.Visibility = ViewStates.Gone;
                        var t = SupportFragmentManager.BeginTransaction();
                        contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                        t.Replace(Resource.Id.content_frame, new SponsorsFragment());
                        t.Commit();
                    }
                }
                
            }
                

            //FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            //fragmentTx.Replace(Resource.Id.content_frame, fragment);
            //// Add the transaction to the back stack.
            //fragmentTx.AddToBackStack(null);
            //// Commit the transaction.
            //fragmentTx.Commit();
        }

        public void showSessionFragment()
        {
            var t = SupportFragmentManager.BeginTransaction();
            contentFrag = SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
            t.Replace(Resource.Id.content_frame, new SessionsNSpeakers(true));
            t.Commit();
        }
        public void setTitleText(String menuName)
        {
            titleTextView.Text=menuName.ToUpper();
            titleTextView.Invalidate();
        }

        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                FragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
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
    }

    class ProgramFaqAdapter : ArrayAdapter<object>
    {
        public ProgramFaqAdapter() : base(null, 0)
        {

        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            return base.GetView(position, convertView, parent);
        }
    }
}

