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
using CommonLayer;
using ConferenceAppDroid.Utilities;
using System.Globalization;
using Android.Util;
using Android.Support.V4.App;
using Android.Support.V4.View;
using CommonLayer.Entities.Built;
using System.Threading.Tasks;
using ConferenceAppDroid.Adapters;
using Android.Graphics;
using ConferenceAppDroid.Core;
using ConferenceAppDroid.BroadcastReceivers;
using Android.Views.InputMethods;
using Android.Net;

namespace ConferenceAppDroid.Fragments
{
    public class Session : Android.Support.V4.App.Fragment
    {
        View actionBarView;
        View parentView;
        HorizontalScrollView HorizontalScrollView01;
        CustomViewPagerWithNoScroll mViewPager;
        SlidingTabLayout mSlidingTabLayout;
        List<BuiltSessionTime> lstAllSessions;
        SeparatedListAdapter adapter;
        List<BuiltTracks> tracks;
        SegmentedControlButton segmentedControlButton;
        RadioGroup sessionGroup;
        List<string> lstDate = new List<string>();
        ListView lstSession;
        public string CurrentSelectedDate { get; set; }
        public string searchText { get; set; }
        public string filterTrackName = string.Empty;
        public string subTrackName = string.Empty;
        SessionAdapter sessionAdapter;
        List<BuiltSessionTime> sessionSource = new List<BuiltSessionTime>();
        ImageView back_btn;
        TextView titleTextView, filter_tracks_title_tv, filter_sub_tracks_title_tv, searchTextView;
        RelativeLayout select_subtrack_filter_container;
        FilterDialogFragment dialog;
        MainActivity baseActivity;
        private static int TITLE_OFFSET_DIPS = 24;
        MainActivity activity;
        RelativeLayout session_loading_container;
        TextView session_NoData_TextView;
        EditText txtSearch;

        public Session(MainActivity activity)
        {
            this.baseActivity = activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var actionbar = Activity.ActionBar.CustomView;
            searchTextView = actionbar.FindViewById<TextView>(Resource.Id.searchTextView);
            searchTextView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(Activity, searchTextView, "&#xf002");
            searchTextView.Text = lmtUnicode;



            parentView = inflater.Inflate(Resource.Layout.sessionLayout, null);
            var layout = parentView.FindViewById<RelativeLayout>(Resource.Id.session_search_container);
            txtSearch = parentView.FindViewById<EditText>(Resource.Id.session_search_edit_text);
            var btnCancel = parentView.FindViewById<Button>(Resource.Id.session_search_cancel_container_btn);
            var btnSerchCancel = parentView.FindViewById<Button>(Resource.Id.session_search_cancel_btn);
            var searchIcon = parentView.FindViewById<Button>(Resource.Id.session_search_btn);
            searchTextView.Click += (s, e) =>
            {
                if (layout.Visibility == ViewStates.Gone)
                {
                    layout.Visibility = ViewStates.Visible;
                    searchIcon.Visibility = ViewStates.Visible;
                    btnSerchCancel.Visibility = ViewStates.Visible;

                }
                else
                {
                    txtSearch.Text = string.Empty;
                    layout.Visibility = ViewStates.Gone;
                    searchIcon.Visibility = ViewStates.Gone;
                    btnSerchCancel.Visibility = ViewStates.Gone;
                    hideSoftInputFromWindow();
                }
            };

            sessionGroup = parentView.FindViewById<RadioGroup>(Resource.Id.sessionGroup);
            HorizontalScrollView01 = parentView.FindViewById<HorizontalScrollView>(Resource.Id.HorizontalScrollView01);
            session_NoData_TextView = parentView.FindViewById<TextView>(Resource.Id.session_NoData_TextView);
            lstSession = parentView.FindViewById<ListView>(Resource.Id.sessionListView);
            filter_tracks_title_tv = parentView.FindViewById<TextView>(Resource.Id.filter_tracks_title_tv);
            select_subtrack_filter_container = parentView.FindViewById<RelativeLayout>(Resource.Id.select_subtrack_filter_container);
            filter_sub_tracks_title_tv = parentView.FindViewById<TextView>(Resource.Id.filter_sub_tracks_title_tv);
            session_loading_container = parentView.FindViewById<RelativeLayout>(Resource.Id.session_loading_container);
            session_loading_container.Visibility = ViewStates.Visible;

            GetUniqueSessionDates();
            tracks = AppSettings.Instance.AllTracks;


            //if (!string.IsNullOrWhiteSpace(baseActivity.exploreTrack))
            //{

            //    filterTrackName = baseActivity.exploreTrack;
            //    if (CurrentSelectedDate == null)
            //    {
            //        CurrentSelectedDate = "All,Days";
            //    }
            //    filter_tracks_title_tv.Text = baseActivity.exploreTrack;
            //    if (AppSettings.Instance.TrackDictionary[filterTrackName].Count() > 0)
            //    {
            //        subTrackName = string.Empty;
            //        filter_sub_tracks_title_tv.Text = "Select Sub-track";
            //        select_subtrack_filter_container.Visibility = ViewStates.Visible;
            //        filter_sub_tracks_title_tv.Visibility = ViewStates.Visible;
            //    }

            //    filterUsingLinq(CurrentSelectedDate, filterTrackName);

            //    //dialog.selectedTrackName = baseActivity.exploreTrack;
            //    //filterTrackName = baseActivity.exploreTrack;


            //}



            getALLSession(res =>
                {
                    lstAllSessions = res;
                    sessionSource = res;

                    if (!string.IsNullOrWhiteSpace(baseActivity.exploreTrack))
                    {

                        filterTrackName = baseActivity.exploreTrack;
                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = "All,Days";
                        }

                        Activity.RunOnUiThread(() =>
                            {
                                if (AppSettings.Instance.TrackDictionary[filterTrackName].Count() > 0)
                                {
                                    subTrackName = string.Empty;
                                    filter_sub_tracks_title_tv.Text = "Select Sub-track";
                                    select_subtrack_filter_container.Visibility = ViewStates.Visible;
                                    filter_sub_tracks_title_tv.Visibility = ViewStates.Visible;
                                }
                            });

                        filterUsingLinq(CurrentSelectedDate, filterTrackName);

                        //dialog.selectedTrackName = baseActivity.exploreTrack;
                        //filterTrackName = baseActivity.exploreTrack;


                    }
                    else
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            session_loading_container.Visibility = ViewStates.Gone;
                            sessionAdapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, lstAllSessions, tracks, Screens.Session);
                            lstSession.Adapter = sessionAdapter;
                        });
                    }
                });
            lstSession.AnimationCacheEnabled = false;
            lstSession.ScrollingCacheEnabled = false;
            lstSession.ItemClick += (s, e) =>
                {

                    var currentSession = sessionSource[e.Position];
                    Intent intent = new Intent(Activity, (typeof(SessionDetail)));
                    intent.PutExtra("uid", currentSession.BuiltSession.session_id);
                    intent.PutExtra("date", currentSession.date);
                    intent.PutExtra("time", currentSession.time);
                    intent.PutExtra("title", currentSession.BuiltSession.title);
                    intent.PutExtra("abbreviation", currentSession.BuiltSession.abbreviation);

                    StartActivity(intent);

                };


            btnCancel.Click += (s, e) =>
            {
                //this.searchText = string.Empty;
                layout.Visibility = ViewStates.Gone;
                txtSearch.Text = string.Empty;
                this.searchText = string.Empty;
                searchIcon.Visibility = ViewStates.Visible;
                btnSerchCancel.Visibility = ViewStates.Gone;
                if (CurrentSelectedDate == null)
                {
                    CurrentSelectedDate = "All,Days";
                }
                filterUsingLinq(CurrentSelectedDate, string.Empty);
                layout.Visibility = ViewStates.Gone;
                hideSoftInputFromWindow();
            };
            txtSearch.EditorAction += (sender, e) =>
            {

                if (e.ActionId == Android.Views.InputMethods.ImeAction.Search)
                {
                    hideSoftInputFromWindow();
                    if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        searchText = txtSearch.Text;
                        if (CurrentSelectedDate == null)
                        {
                            CurrentSelectedDate = "All,Days";
                        }
                        filterUsingLinq(CurrentSelectedDate, txtSearch.Text);
                    }
                    else
                    {
                        
                        Toast.MakeText(Activity, "Search cannot be blank.", ToastLength.Long);
                        return;
                    }

                }
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (e.Text.Count() > 0)
                {
                    searchIcon.Visibility = ViewStates.Gone;
                    btnSerchCancel.Visibility = ViewStates.Visible;
                }
            };
            btnSerchCancel.Click += (s, e) =>
            {
                hideSoftInputFromWindow();
                txtSearch.Text = string.Empty;
                this.searchText = string.Empty;
                searchIcon.Visibility = ViewStates.Visible;
                btnSerchCancel.Visibility = ViewStates.Gone;
                if (CurrentSelectedDate == null)
                {
                    CurrentSelectedDate = "All,Days";
                }
                filterUsingLinq(CurrentSelectedDate, string.Empty);

            };


            filter_tracks_title_tv.Click += (s, e) =>
                {
                    var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
                    dialog = new FilterDialogFragment(filter_tracks_title_tv.Text, FilterDialogFragment.Tracks.parentTracks); // creating new object
                    dialog.selectedTrackName = filter_tracks_title_tv.Text;
                    dialog.Show(fragmentTransaction, "dialog");
                };
            filter_sub_tracks_title_tv.Click += (s, e) =>
                {
                    var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
                    if (dialog == null)
                    {
                        dialog = new FilterDialogFragment(filter_sub_tracks_title_tv.Text, FilterDialogFragment.Tracks.subTracks); // creating new object
                        Console.WriteLine(dialog.selectedTrackName);
                        dialog.selectedTrackName = filter_tracks_title_tv.Text;
                        dialog.selectedSubtrackName = filter_sub_tracks_title_tv.Text;
                        dialog.Show(fragmentTransaction, "dialog");
                    }
                    else
                    {

                        dialog.tracks = FilterDialogFragment.Tracks.subTracks;
                        Console.WriteLine(dialog.selectedTrackName);
                        dialog.selectedSubtrackName = filter_sub_tracks_title_tv.Text;
                        dialog.Show(fragmentTransaction, "dialog");
                    }
                };


            FilterDialogFragment.action = (s) =>
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    filter_tracks_title_tv.Text = "All Tracks";
                    subTrackName = s;
                    filter_sub_tracks_title_tv.Text = "Select Sub-track";
                    select_subtrack_filter_container.Visibility = ViewStates.Gone;
                    filter_sub_tracks_title_tv.Visibility = ViewStates.Gone;

                }
                else
                {
                    filter_tracks_title_tv.Text = s;
                    subTrackName = string.Empty;
                    filter_sub_tracks_title_tv.Text = "Select Sub-track";
                    if (AppSettings.Instance.TrackDictionary[dialog.selectedTrackName].Count() > 0)
                    {
                        select_subtrack_filter_container.Visibility = ViewStates.Visible;
                        filter_sub_tracks_title_tv.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        select_subtrack_filter_container.Visibility = ViewStates.Gone;
                        filter_sub_tracks_title_tv.Visibility = ViewStates.Gone;
                    }
                }
                if (filterTrackName.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    return;

                }
                else
                {
                    filterTrackName = s;
                }



                if (CurrentSelectedDate == null)
                {
                    CurrentSelectedDate = "All,Days";
                }
                filterUsingLinq(CurrentSelectedDate, filterTrackName);
            };

            FilterDialogFragment.subtrackHandler = (s) =>
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        filter_sub_tracks_title_tv.Text = "Select Sub-track";
                    }
                    else
                    {
                        filter_sub_tracks_title_tv.Text = s;

                    }
                    if (subTrackName.Equals(s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return;

                    }
                    else
                    {
                        subTrackName = s;
                    }
                    if (CurrentSelectedDate == null)
                    {
                        CurrentSelectedDate = "All,Days";
                    }
                    filterUsingLinq(CurrentSelectedDate, subTrackName);
                };

            activity = ((MainActivity)Activity);
            activity.updateSessionsReceiver.OnBroadcastReceive += updateSessionsReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.updateSessionsReceiver, new IntentFilter(UpdateSessionsReceiver.action));

            return parentView;
        }

        public void hideSoftInputFromWindow()
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(txtSearch.WindowToken, 0);
        }

        void updateSessionsReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            updateSessions();
        }

        void GetUniqueSessionDates()
        {
            DataManager.GetUniqueSessionDates(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                lstDate = t.Result;
                if (lstDate != null && lstDate.Count > 0)
                {
                    lstDate.Insert(0, "All,Days");
                    var dates = lstDate.Select(p => Helper.convertToDateInterest(p)).ToList();

                    Activity.RunOnUiThread(() =>
                    {
                        if (sessionGroup.ChildCount > 0)
                        {
                            sessionGroup.RemoveAllViews();
                        }
                        for (int i = 0; i < lstDate.Count; i++)
                        {
                            if (i == 0)
                            {
                                segmentedControlButton = (SegmentedControlButton)LayoutInflater.From(Activity).Inflate(Resource.Layout.temp, null);
                                segmentedControlButton.isFromSessionScreen = true;
                                segmentedControlButton.Text = dates[i];
                                segmentedControlButton.Id = i;
                                segmentedControlButton.Checked = true;
                                sessionGroup.AddView(segmentedControlButton);
                            }
                            else
                            {
                                segmentedControlButton = (SegmentedControlButton)LayoutInflater.From(Activity).Inflate(Resource.Layout.temp, null);
                                segmentedControlButton.isFromSessionScreen = true;
                                segmentedControlButton.Id = i;
                                segmentedControlButton.Text = dates[i];
                                segmentedControlButton.Checked = false;
                                sessionGroup.AddView(segmentedControlButton);
                            }

                        }
                        sessionGroup.CheckedChange += sessionGroup_CheckedChange;
                    });
                }
            });
        }

        private void updateSessions()
        {
            //GetUniqueSessionDates();
            getALLRefreshSession(res =>
            {
                lstAllSessions = res;
                sessionSource = res;
                Activity.RunOnUiThread(() =>
                {

                    sessionAdapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, lstAllSessions, tracks, Screens.Session);
                    lstSession.Adapter = sessionAdapter;
                });
            });
        }

        private void getALLRefreshSession(Action<List<BuiltSessionTime>> callback)
        {
            tracks = AppSettings.Instance.AllTracks;
            DataManager.GetSessionTime(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                lstAllSessions = res;
                var allSessions = res = res.OrderBy(p => p.time).OrderBy(p => p.date).ToList();
                //var allSessions = res.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.OrderBy(q => q.time).ToList());
                if (callback != null)
                {
                    callback(allSessions);
                }
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (activity.updateSessionsReceiver != null)
            {
                activity.UnregisterReceiver(activity.updateSessionsReceiver);
            }
            if (activity.deltaCompletedReceiver != null)
            {
                activity.UnregisterReceiver(activity.deltaCompletedReceiver);
            }
        }

        void sessionGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            scrollToTab(e.CheckedId, 2);
            var current = View.FindViewById<RadioButton>(e.CheckedId);
            int i = sessionGroup.IndexOfChild(current);

            var date = lstDate[i];
            if (date.ToLower().Contains("days"))
            {
                CurrentSelectedDate = date;
                filterUsingLinq(date);
            }
            else
            {
                var dt = lstDate.Where(p => p == date).FirstOrDefault();
                filterUsingLinq(dt);
            }
        }

        private void filterUsingLinq(string dateTime, string searchData = null)
        {
            getSessions(dateTime, res =>
            {
                sessionSource = res;
                Activity.RunOnUiThread(() =>
                {
                    if (!string.IsNullOrWhiteSpace(baseActivity.exploreTrack))
                    {
                        filter_tracks_title_tv.Text = baseActivity.exploreTrack;
                        baseActivity.exploreTrack = string.Empty;
                    }

                    if (session_loading_container.Visibility == ViewStates.Visible)
                    {
                        session_loading_container.Visibility = ViewStates.Gone;
                    }
                    if (sessionSource.Count > 0)
                    {
                        session_NoData_TextView.Visibility = ViewStates.Gone;
                        if (sessionAdapter != null)
                        {
                            sessionAdapter.Clear();
                            sessionAdapter.AddAll(sessionSource);
                            lstSession.Adapter = sessionAdapter;
                            sessionAdapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            sessionAdapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, sessionSource, tracks, Screens.Session);
                            lstSession.Adapter = sessionAdapter;
                        }
                    }
                    else
                    {
                        session_NoData_TextView.Visibility = ViewStates.Visible;
                        session_NoData_TextView.Text = "No Session Found";
                        sessionAdapter.Clear();
                        sessionAdapter.AddAll(sessionSource);
                        lstSession.Adapter = sessionAdapter;
                        sessionAdapter.NotifyDataSetChanged();

                    }
                });
            });
        }

        private void scrollToTab(int tabIndex, int positionOffset)
        {
            int tabStripChildCount = sessionGroup.ChildCount;
            if (tabStripChildCount == 0 || tabIndex < 0 || tabIndex >= tabStripChildCount)
            {
                return;
            }

            View selectedChild = sessionGroup.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int targetScrollX = selectedChild.Left + positionOffset;

                if (tabIndex > 0 || positionOffset > 0)
                {
                    // If we're not at the first child and are mid-scroll, make sure we obey the offset
                    var mTitleOffset = (int)(TITLE_OFFSET_DIPS * Resources.DisplayMetrics.Density);
                    targetScrollX -= mTitleOffset;
                }

                HorizontalScrollView01.ScrollTo(targetScrollX, 0);
            }
        }

        private void loadViews()
        {
            mViewPager = (CustomViewPagerWithNoScroll)parentView.FindViewById(Resource.Id.sessionDaysViewPager);
            mViewPager.SetPagingEnabled(false);
            mViewPager.Adapter = new SessionDetailAdapter(Activity.SupportFragmentManager);
            mSlidingTabLayout = (SlidingTabLayout)parentView.FindViewById(Resource.Id.sliding_tabs);
            mSlidingTabLayout.setCustomTabView(Resource.Layout.view_session_day_top_header, Resource.Id.day_of_session_tv, Resource.Id.date_of_session_tv);
            mSlidingTabLayout.setViewPager(mViewPager);
        }

        private void getALLSession(Action<List<BuiltSessionTime>> callback)
        {
            DataManager.GetSessionTime(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var res = t.Result;
                res = res.OrderBy(p => p.time).OrderBy(p => p.date).ToList();
                //var allSessions = res.OrderBy(p => p.time).GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.OrderBy(q => q.time).ToList());
                if (callback != null)
                {
                    callback(res);
                }
            });
        }

        //   SeparatedListAdapter CreateAdapter<T>(Dictionary<string, List<T>> sortedObjects)
        //where T : IHasLabel, IComparable<T>
        //   {
        //       var adapter = new SeparatedListAdapter(Activity);
        //       foreach (var e in sortedObjects)
        //       {
        //           var label = e.Key;
        //           var section = e.Value;
        //           //adapter.AddSection(label, new ArrayAdapter<T>(Activity, Resource.Layout.menu_row, section));
        //           adapter.AddSection(label, new SessionAdapter<T>(Activity, section,tracks));
        //       }
        //       return adapter;
        //   }

        private int dpToPx(Context context, int p)
        {
            int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, p, context.Resources.DisplayMetrics);
            return px;
        }

        public static string ToDateTimeString(DateTime dt, string format)
        {
            if (String.IsNullOrWhiteSpace(format))
                return String.Empty;
            else
                return dt.ToString(format);
        }

        public void getSessions(string dateTime, Action<List<BuiltSessionTime>> callback)
        {
            Task.Run(() =>
            {
                var expectedResult = lstAllSessions.AsEnumerable();
                CurrentSelectedDate = convertToBuiltDate(dateTime);

                if (!string.IsNullOrEmpty(this.CurrentSelectedDate) && this.CurrentSelectedDate.ToLower().Contains("days"))
                {
                    if (!string.IsNullOrEmpty(filterTrackName))
                    {
                        expectedResult = expectedResult.Where(p => p.BuiltSession.track == filterTrackName);
                    }
                    if (!string.IsNullOrEmpty(subTrackName))
                    {
                        expectedResult = expectedResult.Where(p => p.BuiltSession.sub_track_separated == subTrackName);
                    }
                    if (!string.IsNullOrEmpty(this.searchText))
                    {
                        expectedResult = expectedResult.Where(t => t.BuiltSession.title.ToLower().Contains(searchText.ToLower()) || t.BuiltSession.session_id.ToLower() == searchText.ToLower() || t.BuiltSession.abbreviation.ToLower().Contains(searchText.ToLower()));
                    }

                    sessionSource = expectedResult.OrderBy(p => p.time).OrderBy(p => p.date).ToList();
                    if (callback != null)
                    {
                        callback(sessionSource);
                    }

                    return;
                }


                if (!string.IsNullOrEmpty(this.CurrentSelectedDate))
                {
                    expectedResult = expectedResult.Where(q => q.date == CurrentSelectedDate);
                }
                if (!string.IsNullOrEmpty(this.searchText))
                {
                    expectedResult = expectedResult.Where(t => t.BuiltSession.title.ToLower().Contains(searchText.ToLower()) || t.BuiltSession.session_id.ToLower() == searchText.ToLower() || t.BuiltSession.abbreviation.ToLower().Contains(searchText.ToLower()));
                }
                if (!string.IsNullOrEmpty(filterTrackName))
                {
                    expectedResult = expectedResult.Where(p => p.BuiltSession.track == filterTrackName);
                }
                if (!string.IsNullOrEmpty(subTrackName))
                {
                    expectedResult = expectedResult.Where(p => p.BuiltSession.sub_track_separated == subTrackName);
                }
                sessionSource = expectedResult.OrderBy(p => p.time).OrderBy(p => p.date).ToList();
                if (callback != null)
                {
                    callback(sessionSource);
                }
            });
        }


        private static string convertToBuiltDate(string p)
        {
            if (p.ToLower().Contains("days"))
            {
                var allDate = p;
                return allDate;
            }

            var date = DateTime.Parse(p).ToString("yyyy-MM-dd");
            return date;
        }
    }


    public class SessionAdapter : ArrayAdapter<BuiltSessionTime>
    {
        View parentView;
        Android.App.Activity context;
        List<BuiltSessionTime> data;
        List<BuiltTracks> parentTracks;
        TextView sessionName;
        TextView address;
        TextView dateTime;
        ImageView trackColor;
        RelativeLayout section;
        TextView sectionTitle;
        ImageView session_star_image_btn;
        public Screens screens { get; set; }

        public SessionAdapter(Android.App.Activity context, int resource, List<BuiltSessionTime> items, List<BuiltTracks> tracks, Screens screens)
            : base(context, resource, items)
        {
            this.context = context;
            this.data = items;
            this.parentTracks = tracks;
            this.screens = screens;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            BuiltTracks builtTracks = null;
            parentView = convertView;
            if (null == convertView)
            {
                parentView = LayoutInflater.From(context).Inflate(Resource.Layout.row_all_session, null);
                session_star_image_btn = parentView.FindViewById<ImageView>(Resource.Id.session_star_image_btn);
                sessionName = parentView.FindViewById<TextView>(Resource.Id.session_title_name_tv);
                address = parentView.FindViewById<TextView>(Resource.Id.session_address_tv);
                dateTime = parentView.FindViewById<TextView>(Resource.Id.session_date_time_tv);
                trackColor = parentView.FindViewById<ImageView>(Resource.Id.session_track_color);
                section = parentView.FindViewById<RelativeLayout>(Resource.Id.section);
                sectionTitle = parentView.FindViewById<TextView>(Resource.Id.sectionTitle);
                viewHolder = new ViewHolder(sessionName, address, dateTime, trackColor, section, sectionTitle, session_star_image_btn);

                parentView.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)parentView.Tag;
                sessionName = viewHolder.sessionName;
                address = viewHolder.address;
                dateTime = viewHolder.dateTime;
                trackColor = viewHolder.trackColor;
                section = viewHolder.section;
                sectionTitle = viewHolder.sectionTitle;
                session_star_image_btn = viewHolder.session_star_image_btn;
            }


            session_star_image_btn.Tag = position.ToString();
            var item = GetItem(position);

            if (this.screens == Screens.Session)
            {
                section.Visibility = ViewStates.Visible;
                sectionTitle.Text = Helper.convertToDate(item.date);
                if (position != 0)
                {
                    var previousItem = GetItem(position - 1);
                    if (!previousItem.date.Equals(item.date, StringComparison.InvariantCultureIgnoreCase))
                    {

                        section.Visibility = ViewStates.Visible;
                        sectionTitle.Text = Helper.convertToDate(item.date);
                    }
                    else
                    {
                        section.Visibility = ViewStates.Gone;
                    }
                }
            }

            var firstName = item.BuiltSession.title;
            var companyName = item.room;
            var sessionTime = Helper.convertToDate(item.date) + ", " + Helper.convertToStartEndDate(item.time, item.length);
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                sessionName.Text = firstName;
            }

            if (!String.IsNullOrWhiteSpace(companyName))
            {
                address.Text = companyName;
            }

            dateTime.Text = sessionTime;



            if (item.BuiltSession != null)
            {
                builtTracks = parentTracks.FirstOrDefault(p => p.name == item.BuiltSession.track);
            }
            if (builtTracks == null)
            {
                builtTracks = parentTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
            }
            if (builtTracks != null)
            {
                if (builtTracks.color != null && builtTracks.color.Length > 0)
                {
                    if (builtTracks.color.Contains("#"))
                    {
                        trackColor.SetBackgroundColor(Color.ParseColor(builtTracks.color));
                    }
                    else
                    {
                        trackColor.SetBackgroundColor(Color.ParseColor("#" + builtTracks.color));
                    }
                }
            }

            if (AppSettings.Instance.MySessionIds.Contains(item.session_time_id))
            {
                session_star_image_btn.SetImageResource(Resource.Drawable.ic_schedule_row);
                session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_schedule_row.ToString();
            }
            else
            {
                session_star_image_btn.SetImageResource(Resource.Drawable.ic_add_schedule_row);
                session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_add_schedule_row.ToString();
            }

            try
            {
                if (Convert.ToDateTime(item.date).Date < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    if (session_star_image_btn.Visibility == ViewStates.Visible)
                    { session_star_image_btn.Visibility = ViewStates.Gone; }

                }
                else if (Convert.ToDateTime(item.date).Date == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(item.time) < Helper.timeConverterForCurrentHourMinute())
                {
                    if (session_star_image_btn.Visibility == ViewStates.Visible)
                    { session_star_image_btn.Visibility = ViewStates.Gone; }
                }
                else if (Convert.ToDateTime(item.date).Date > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    session_star_image_btn.Visibility = ViewStates.Visible;
                }
            }
            catch
            {
                session_star_image_btn.Visibility = ViewStates.Gone;
            }

           

            session_star_image_btn.Click += (s, e) =>
                {
                    if (AppSettings.Instance.ApplicationUser != null)
                    {
                        ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

                        if (connectivityManager.ActiveNetworkInfo == null)
                        {
                            Toast.MakeText(context, context.Resources.GetString(Resource.String.no_internet_connection_text), ToastLength.Long).Show();
                            return;
                        }

                        Helper.showProgress(context, true);
                        if (session_star_image_btn.Tag.ToString().Split(',').Last() == Resource.Drawable.ic_add_schedule_row.ToString())
                        {
                            DataManager.AddSessionToSchedule(DBHelper.Instance.Connection, item, (session) =>
                            {
                                if (!AppSettings.Instance.MySessionIds.Contains(session.session_time_id))
                                {
                                    AppSettings.Instance.MySessionIds.Add(session.session_time_id);
                                }
                                context.RunOnUiThread(() =>
                                {
                                    Helper.showProgress(context, false);
                                    Toast.MakeText(context, (Helper.SessionAddedString), ToastLength.Long).Show();
                                    session_star_image_btn.SetImageResource(Resource.Drawable.ic_schedule_row);
                                    session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_schedule_row.ToString();
                                });
                            }, error =>
                            {
                                context.RunOnUiThread(() =>
                                {
                                    Helper.showProgress(context, false);
                                    Toast.MakeText(context, (Helper.GetErrorMessage(error)), ToastLength.Long).Show();
                                });
                            });
                        }
                        else
                        {
                            if (item.BuiltSession.type.Equals(AppSettings.Instance.GeneralSession, StringComparison.InvariantCultureIgnoreCase) || item.BuiltSession.type.Equals(AppSettings.Instance.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                            {
                                Toast.MakeText(context, (Helper.CanntotUnschedule), ToastLength.Long).Show();
                            }
                            else
                            {

                                DataManager.RemoveSessionFromSchedule(DBHelper.Instance.Connection, item, (err, session) =>
                                {
                                    if (AppSettings.Instance.MySessionIds.Contains(session.session_time_id))
                                    {
                                        AppSettings.Instance.MySessionIds.Remove(session.session_time_id);
                                    }
                                    if (err == null)
                                    {
                                        context.RunOnUiThread(() =>
                                        {
                                            Helper.showProgress(context, false);
                                            Toast.MakeText(context, (Helper.SessionRemovedString), ToastLength.Long).Show();
                                            session_star_image_btn.SetImageResource(Resource.Drawable.ic_add_schedule_row);
                                            session_star_image_btn.Tag = session_star_image_btn.Tag.ToString() + "," + Resource.Drawable.ic_add_schedule_row.ToString();
                                        });
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        new Android.App.AlertDialog.Builder(context).SetTitle("Login Required").SetMessage("You need to be logged-in to access this feature. Do you want to login?")
                                                                           .SetPositiveButton("Yes", (sender, args) =>
                                                                           {
                                                                               Intent intent = new Intent(context, typeof(LoginActivity));
                                                                               context.StartActivity(intent);
                                                                           })
                                                                           .SetNegativeButton("No", (sender, args) => { }).Show();
                    }
                };


            return parentView;
        }


        public class ViewHolder : Java.Lang.Object
        {
            public TextView sessionName;
            public TextView address;
            public TextView dateTime;
            public ImageView trackColor;
            public RelativeLayout section;
            public TextView sectionTitle;
            public ImageView session_star_image_btn;

            public ViewHolder(TextView sessionName, TextView address, TextView dateTime, ImageView trackColor, RelativeLayout section, TextView sectionTitle, ImageView session_star_image_btn)
            {
                this.sessionName = sessionName;
                this.address = address;
                this.dateTime = dateTime;
                this.trackColor = trackColor;
                this.section = section;
                this.sectionTitle = sectionTitle;
                this.session_star_image_btn = session_star_image_btn;
            }
        }

    }
}