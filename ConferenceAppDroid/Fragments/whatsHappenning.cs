using Android.Content;
using Android.Views;
using Android.Widget;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.Fragments
{
    public class whatsHappenning : Android.Support.V4.App.Fragment
    {
        private View parentView;
        private ListView listView;
        private ExploreHeaderComponent exploreHeaderComponent;
        private Context context;
        BuiltConfig config;
        List<BuiltSessionTime> featuredSession;
        SessionAdapter adapter;
        List<BuiltTracks> builtTracks;
        MainActivity activity;
        public Dictionary<string, List<BuiltSessionTime>> dictMyScheBuiltSessionTime;
        List<BuiltSessionTime> lstOnNOwSessions;
        List<BuiltSessionTime> lstNextUpSessions;

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.fragment_explore, container, false);
            listView = (ListView)parentView.FindViewById(Android.Resource.Id.List);
            this.context = Activity;
            exploreHeaderComponent = new ExploreHeaderComponent(Activity);
            config = AppSettings.Instance.config;
            builtTracks = AppSettings.Instance.AllTracks;
            upadateExplore();

            activity = ((MainActivity)Activity);
            activity.configCompletedReceiver.OnBroadcastReceive += configCompletedReceiver_OnBroadcastReceive;
            activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive; ;
            activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));
            activity.RegisterReceiver(activity.configCompletedReceiver, new IntentFilter(ConfigCompletedReceiver.action));
            return parentView;
        }

        void configCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            config = AppSettings.Instance.config;
            upadateExplore();
        }


        void deltaCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            var updatedUids = arg2.GetStringExtra("uids").Split('|');
            if (updatedUids != null && (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended)))
            {
                builtMyScheduleFromDB((p) =>
                {
                    dictMyScheBuiltSessionTime = p;
                    if (dictMyScheBuiltSessionTime != null && dictMyScheBuiltSessionTime.Count != 0)
                    {
                        AppSettings.Instance.MySessionIds = dictMyScheBuiltSessionTime.SelectMany(q => q.Value).Select(r => r.session_time_id).ToList();
                    }
                });
            }
            else if (updatedUids.Contains(ApiCalls.recommended))
            {
                DataManager.GetbuiltSessionTimeListOfTrack(DBHelper.Instance.Connection).ContinueWith(fs =>
                {
                    AppSettings.Instance.FeaturedSessions = fs.Result;
                });
            }
            //config = AppSettings.Instance.config;
            //upadateExplore();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (activity.deltaCompletedReceiver != null)
            {
                activity.UnregisterReceiver(activity.deltaCompletedReceiver);
            }
            if (activity.configCompletedReceiver!= null)
            {
                activity.UnregisterReceiver(activity.configCompletedReceiver);
            }
        }
        

        void upadateExplore()
        {
            builtMyScheduleFromDB((p) =>
             {
                 dictMyScheBuiltSessionTime = p;
                 if (dictMyScheBuiltSessionTime != null && dictMyScheBuiltSessionTime.Count != 0)
                 {
                     AppSettings.Instance.MySessionIds = dictMyScheBuiltSessionTime.SelectMany(q => q.Value).Select(r => r.session_time_id).ToList();
                 }
             });


                   if (config.explore.show_banner)
                   {
                       DataManager.GetListOfIntroData(DBHelper.Instance.Connection).ContinueWith(t =>
                       {
                           Activity.RunOnUiThread(() =>
                           {
                               if (listView.HeaderViewsCount > 0)
                               {
                                   listView.Adapter = null;
                                   listView.RemoveHeaderView(exploreHeaderComponent.getView());

                               }
                               listView.AddHeaderView(exploreHeaderComponent.getView());
                               listView.Adapter = null;
                               exploreHeaderComponent.showIntro(t.Result.FirstOrDefault());
                           });

                       });
                   }
                   else
                   {
                       exploreHeaderComponent.hideIntro();
                   }

                   if (config.explore.show_tracks)
                   {
                       DataManager.GetListOfAllTrack(DBHelper.Instance.Connection).ContinueWith(t =>
                       {
                           Activity.RunOnUiThread(() =>
                           {
                               exploreHeaderComponent.showTracksHeaderScrollView(t.Result);
                           });
                       });
                   }
                   else
                   {
                       exploreHeaderComponent.hideTracksHeaderScrollView();
                   }

                   if (config.explore.show_sponsors)
                   {
                       DataManager.GetListOfExhibitorTypes(DBHelper.Instance.Connection).ContinueWith(t =>
                       {
                           Activity.RunOnUiThread(() =>
                           {
                               exploreHeaderComponent.showSponsorsHeaderScrollView(t.Result);

                           });
                       });
                   }
                   else
                   {
                       exploreHeaderComponent.hideSponsorsHeaderScrollView();
                   }

                   if (config.explore.show_sessions)
                   {
                       DataManager.GetOnNowSessions(DBHelper.Instance.Connection, 10).ContinueWith(t =>
                       {
                           lstOnNOwSessions=t.Result;
                           DataManager.GetNextUpSessions(DBHelper.Instance.Connection, 10, AppSettings.Instance.ApplicationUser).ContinueWith(u =>
                           {
                               lstNextUpSessions=u.Result;
                               Activity.RunOnUiThread(() =>
                               {
                                   exploreHeaderComponent.showOnNowAndNextUpPagerHeaderView(lstOnNOwSessions, lstNextUpSessions);
                               });
                           });
                       });
                   }
                   else
                   {
                       exploreHeaderComponent.hideOnNowAndNextUpPagerHeaderView();
                   }

                   if (config.explore.show_recommended_sessions)
                   {
                       featuredSession = AppSettings.Instance.FeaturedSessions;
                       if (featuredSession.Count > 0)
                       {
                           if (adapter != null)
                           {
                               adapter.Clear();
                               adapter.AddAll(featuredSession);
                               listView.Adapter = adapter;
                               adapter.NotifyDataSetChanged();
                           }
                           else
                           {
                               exploreHeaderComponent.popularSessionlabelTextView.Visibility = ViewStates.Visible;
                               adapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, featuredSession, builtTracks, Screens.SpeakerSession);
                               listView.Adapter = adapter;
                           }

                       }
                       else
                       {
                           exploreHeaderComponent.popularSessionlabelTextView.Visibility = ViewStates.Gone;
                       }
                   }
                   exploreHeaderComponent.view.ForceLayout();


        }

        private void builtMyScheduleFromDB(Action<Dictionary<string, List<BuiltSessionTime>>> callback)
        {
            DataManager.GetMyScheduleToday(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var result = t.Result;
                if (callback != null)
                    callback(result);

            });
        }

        public void RefreshNextUp(Action completionHandler)
        {
            DataManager.GetNextUpSessions(DBHelper.Instance.Connection, 10, AppSettings.Instance.ApplicationUser).ContinueWith(t =>
            {
                lstNextUpSessions = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();
                if (completionHandler != null)
                {
                    completionHandler();
                }
            });
        }

        public void RefreshOnNow(Action completionHandler)
        {
            DataManager.GetOnNowSessions(DBHelper.Instance.Connection, 10).ContinueWith(t =>
            {
                lstOnNOwSessions= t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();
                if (completionHandler != null)
                {
                    completionHandler();
                }
            });
        }

    }
}
