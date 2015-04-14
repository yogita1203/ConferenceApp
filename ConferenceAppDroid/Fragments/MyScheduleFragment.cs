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
using CommonLayer;
using ConferenceAppDroid.Utilities;
using ConferenceAppDroid.CustomControls;

namespace ConferenceAppDroid.Fragments
{
    public class MyScheduleFragment : Android.Support.V4.App.Fragment
    {
        List<string> lstDate = new List<string>();
        RadioGroup sessionGroup;
        ListView lstSession;
        List<BuiltTracks> tracks;
        List<BuiltSessionTime> lstAllSessions;
        List<BuiltSessionTime> sessionSource;
        SegmentedControlButton segmentedControlButton;
        SessionAdapter sessionAdapter;
        RelativeLayout select_filter_container;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var parentView = inflater.Inflate(Resource.Layout.sessionLayout, null);
            sessionGroup = parentView.FindViewById<RadioGroup>(Resource.Id.sessionGroup);
            lstSession = parentView.FindViewById<ListView>(Resource.Id.sessionListView);
            var session_loading_container = parentView.FindViewById<RelativeLayout>(Resource.Id.session_loading_container);
            session_loading_container.Visibility = ViewStates.Gone;
            select_filter_container = parentView.FindViewById<RelativeLayout>(Resource.Id.select_filter_container);
            select_filter_container.Visibility = ViewStates.Gone;
            tracks = AppSettings.Instance.AllTracks;
            getALLSchedule(res =>
            {
                lstAllSessions = res;
                sessionSource = res;
                Activity.RunOnUiThread(() =>
                {
                    setHeader();
                    sessionAdapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, lstAllSessions, tracks, Screens.Session);
                    lstSession.Adapter = sessionAdapter;
                });
            });

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


            return parentView;
        }

        void setHeader()
        {
            if (lstDate != null && lstDate.Count > 0)
            {
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
                            segmentedControlButton.Text = dates[i];
                            segmentedControlButton.Id = i;
                            segmentedControlButton.Checked = true;
                            sessionGroup.AddView(segmentedControlButton);
                        }
                        else
                        {
                            segmentedControlButton = (SegmentedControlButton)LayoutInflater.From(Activity).Inflate(Resource.Layout.temp, null);
                            segmentedControlButton.Id = i;
                            segmentedControlButton.Text = dates[i];
                            segmentedControlButton.Checked = false;
                            sessionGroup.AddView(segmentedControlButton);
                        }

                    }
                    sessionGroup.CheckedChange += sessionGroup_CheckedChange;
                });

            
                lstSession.AnimationCacheEnabled = false;
                lstSession.ScrollingCacheEnabled = false;
            }
        }

        private void sessionGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            Console.WriteLine();
        }

        private void getALLSchedule(Action<List<BuiltSessionTime>> callback)
        {
            Func<string, string> convertToDate = (Date) =>
            {
                var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
                return date;
            };
            List<BuiltSessionTime> filteredList = new List<BuiltSessionTime>();
            DataManager.GetMyScheduleTodayArray(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var allSessions = t.Result;
                if (allSessions != null && allSessions.Length > 0)
                {
                    filteredList= allSessions.OrderBy(p => p.time).ToList();
                    if (lstDate == null) lstDate = new List<string>();
                    lstDate = filteredList.Select(s => s.date).Distinct().ToList();
                    lstDate.Insert(0, "All,Days");
                    if (callback != null)
                    {
                        callback(filteredList);

                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback(new List<BuiltSessionTime>());
                    }
                }

            });
        }
    }
}