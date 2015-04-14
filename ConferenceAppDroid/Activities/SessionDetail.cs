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
using ConferenceAppDroid.CustomControls;
using ConferenceAppDroid.Core;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Interfaces;
using CommonLayer.Entities.Built;
using Android.Net;

namespace ConferenceAppDroid
{
    [Activity(Label = "SessionDetail")]
    public class SessionDetail : BaseActivity, IGetTweetStatusObject
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
        public string date = string.Empty;
        public string time = string.Empty;
        public string title = string.Empty;
        public string abbreviation = string.Empty;
        public string isFromActivity = string.Empty;
        public SessionDetail()
            : base(Resource.String.ApplicationName, "")
        { }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            uid =Intent.GetStringExtra("uid");
            date = Intent.GetStringExtra("date");
            time= Intent.GetStringExtra("time");
            title = Intent.GetStringExtra("title");
            abbreviation = Intent.GetStringExtra("abbreviation");
            isFromActivity = Intent.GetStringExtra("isFromActivity");
            SetContentView(Resource.Layout.activity_session_detail_screen);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            init();
            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Visible;
            twitterImageView.Visibility = ViewStates.Visible;
            starUnstarImageView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(this, twitterImageView, "&#xf099;");
            twitterImageView.Text = lmtUnicode;

            scheduleImageView.Visibility = ViewStates.Visible;
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
                {
                    Finish();
                };
            string session_tweet = AppSettings.Instance.config.social.twitter.session_tweet_text;
            StringBuilder sb = new StringBuilder(session_tweet);
            sb.Replace("{title}", title);
            sb.Replace("{abbreviation}", abbreviation);
            var text = sb.ToString();
            twitterImageView.Click += (s, e) =>
                {
                    AppUtilities.postOnTwitter(this, "", null, this);
                };
            //titleTextView.SetTextColor(Android.Graphics.Color.White);
            titleTextView.Text = "SESSION";
            var mPager = FindViewById<CustomViewPagerWithNoScroll>(Resource.Id.pager);
            mPager.SetPagingEnabled(false);
            var sessionDetailAdapter = new SessionDetailAdapter(SupportFragmentManager);
            mPager.Adapter = sessionDetailAdapter;

            bool showStar = false;
            var result = DataManager.GetMyInterest(DBHelper.Instance.Connection).Result;
            if (result != null && result.Count > 0)
            {
                showStar = result.Any(p => p.session_time_id == uid);
            }

            if (showStar)
            {
                starUnstarImageView.SetImageResource(Resource.Drawable.ic_add_interest_selected);
            }
            else
            {
                starUnstarImageView.SetImageResource(Resource.Drawable.ic_add_interest);
            }


            if (Convert.ToDateTime(date) < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                if (scheduleImageView.Visibility== ViewStates.Visible)
                { scheduleImageView.Visibility = ViewStates.Gone;}

            }
            else if (Convert.ToDateTime(date) == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(time) < Helper.timeConverterForCurrentHourMinute())
            {
                if (scheduleImageView.Visibility == ViewStates.Visible)
                { scheduleImageView.Visibility = ViewStates.Gone; }
            }
            else if (Convert.ToDateTime(date) > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                scheduleImageView.Visibility =ViewStates.Visible;

            }

            if (AppSettings.Instance.MySessionIds.Contains(uid))
            {
                scheduleImageView.SetImageResource(Resource.Drawable.ic_schedule_row);
                //scheduleImageView.Tag = scheduleImageView.Tag.ToString() + "," + Resource.Drawable.ic_schedule_row.ToString();
            }
            else
            {
                scheduleImageView.SetImageResource(Resource.Drawable.ic_add_schedule_row);
                //scheduleImageView.Tag = scheduleImageView.Tag.ToString() + "," + Resource.Drawable.ic_add_schedule_row.ToString();
            }



            starUnstarImageView.Click += (s, e) =>
                {

                    if (AppSettings.Instance.ApplicationUser != null)
                    {
                        ProgressDialog progressDialog = new ProgressDialog(this);
                        progressDialog.SetMessage("Please wait…");
                        progressDialog.Show();
                        DataManager.toggleExtension(uid, DBHelper.Instance.Connection, (call) =>
                        {
                            if (call.ToLower() == "added")
                            {
                                RunOnUiThread(() =>
                                {
                                    progressDialog.Dismiss();
                                    Toast.MakeText(this, Helper.InterestAddedString, ToastLength.Long).Show();
                                    starUnstarImageView.SetImageResource(Resource.Drawable.ic_add_interest_selected);

                                });
                            }
                            else
                            {
                                RunOnUiThread(() =>
                                {
                                    progressDialog.Dismiss();
                                    //Helper.showProgress(this, false); 
                                    Toast.MakeText(this, Helper.InterestRemovedString, ToastLength.Long).Show();
                                    starUnstarImageView.SetImageResource(Resource.Drawable.ic_add_interest);
                                    if (!string.IsNullOrWhiteSpace(isFromActivity) && isFromActivity.Equals("myinterest", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        Intent intent = new Intent(InterestReceiver.action);
                                        SendBroadcast(intent);
                                        Finish();
                                    }
                                });
                            }
                        }, (error) =>
                        {
                            Console.WriteLine();
                        });
                    }
                    else
                     {
                         new Android.App.AlertDialog.Builder(this).SetTitle("Login Required").SetMessage("You need to be logged-in to access this feature. Do you want to login?")
                                 .SetPositiveButton("Yes", (sender, args) =>
                                 {
                                     Intent intent = new Intent(this, typeof(LoginActivity));
                                     StartActivity(intent);
                                 })
                                 .SetNegativeButton("No", (sender, args) => { }).Show();
                    }
                };

            scheduleImageView.Click += (s, e) =>
                {
                    //    if (AppSettings.Instance.ApplicationUser != null)
                    //    {
                    //        ConnectivityManager connectivityManager = (ConnectivityManager)this.GetSystemService(Context.ConnectivityService);

                    //        if (connectivityManager.ActiveNetworkInfo == null)
                    //        {
                    //            Toast.MakeText(this, this.Resources.GetString(Resource.String.no_internet_connection_text), ToastLength.Long).Show();
                    //            return;
                    //        }

                    //        Helper.showProgress(this, true);
                    //        if (scheduleImageView.Tag.ToString().Split(',').Last() == Resource.Drawable.ic_add_schedule_row.ToString())
                    //        {
                    //            DataManager.AddSessionToSchedule(DBHelper.Instance.Connection, item, (session) =>
                    //            {
                    //                if (!AppSettings.Instance.MySessionIds.Contains(session.session_time_id))
                    //                {
                    //                    AppSettings.Instance.MySessionIds.Add(session.session_time_id);
                    //                }
                    //                context.RunOnUiThread(() =>
                    //                {
                    //                    Helper.showProgress(context, false);
                    //                    Toast.MakeText(context, (Helper.SessionAddedString), ToastLength.Long).Show();
                    //                    session_star_image_btn.SetImageResource(Resource.Drawable.ic_schedule_row);
                    //                    session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_schedule_row.ToString();
                    //                });
                    //            }, error =>
                    //            {
                    //                context.RunOnUiThread(() =>
                    //                {
                    //                    Helper.showProgress(context, false);
                    //                    Toast.MakeText(context, (Helper.GetErrorMessage(error)), ToastLength.Long).Show();
                    //                });
                    //            });
                    //        }
                    //        else
                    //        {
                    //            if (item.BuiltSession.type.Equals(AppSettings.Instance.GeneralSession, StringComparison.InvariantCultureIgnoreCase) || item.BuiltSession.type.Equals(AppSettings.Instance.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                    //            {
                    //                Toast.MakeText(this, (Helper.CanntotUnschedule), ToastLength.Long).Show();
                    //            }
                    //            else
                    //            {

                    //                DataManager.RemoveSessionFromSchedule(DBHelper.Instance.Connection, item, (err, session) =>
                    //                {
                    //                    if (AppSettings.Instance.MySessionIds.Contains(session.session_time_id))
                    //                    {
                    //                        AppSettings.Instance.MySessionIds.Remove(session.session_time_id);
                    //                    }
                    //                    if (err == null)
                    //                    {
                    //                        context.RunOnUiThread(() =>
                    //                        {
                    //                            Helper.showProgress(context, false);
                    //                            Toast.MakeText(context, (Helper.SessionRemovedString), ToastLength.Long).Show();
                    //                            session_star_image_btn.SetImageResource(Resource.Drawable.ic_add_schedule_row);
                    //                            session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_add_schedule_row.ToString();
                    //                        });
                    //                    }
                    //                });
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        new Android.App.AlertDialog.Builder(this).SetTitle("Login Required").SetMessage("You need to be logged-in to access this feature. Do you want to login?")
                    //                                                           .SetPositiveButton("Yes", (sender, args) =>
                    //                                                           {
                    //                                                               Intent intent = new Intent(this, typeof(LoginActivity));
                    //                                                               this.StartActivity(intent);
                    //                                                           })
                    //                                                           .SetNegativeButton("No", (sender, args) => { }).Show();
                    //    }
                    //};
                };
        }

        public override void OnBackPressed()
        {
            Finish();
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

        public void setTweetStatusObject(Twitter4j.IStatus result, int position, async.TweetActionAsync.ActionType type)
        {
            Console.WriteLine();
        }
    }

    public class SessionDetailAdapter : FragmentStatePagerAdapter
    {
        
        public SessionDetailAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {

        }
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return new SessionDetailFragment();
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

