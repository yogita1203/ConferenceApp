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
using System.Threading.Tasks;
using ConferenceAppDroid.CustomControls;
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Fragments
{
   public class MyInterestSessionFragment: Android.Support.V4.App.Fragment
    {
       List<string> lstDate = new List<string>();
       RadioGroup sessionGroup;
       TextView txtNoData;
       ListView lstInterest;
       SessionAdapter adapter;
       List<BuiltTracks> lstTracks;
       List<BuiltSessionTime> interestSource;
       RelativeLayout select_filter_container;
       SegmentedControlButton segmentedControlButton;
       BuiltSessionTime currentSession;
       MainActivity activity;
       public string CurrentSelectedDate { get; set; }
       public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
       {
           var parentView = inflater.Inflate(Resource.Layout.sessionLayout, null);
           sessionGroup = parentView.FindViewById<RadioGroup>(Resource.Id.sessionGroup);
           lstInterest = parentView.FindViewById<ListView>(Resource.Id.sessionListView);
           var session_loading_container = parentView.FindViewById<RelativeLayout>(Resource.Id.session_loading_container);
           session_loading_container.Visibility = ViewStates.Gone;
           select_filter_container = parentView.FindViewById<RelativeLayout>(Resource.Id.select_filter_container);
           select_filter_container.Visibility = ViewStates.Gone;

           lstTracks = AppSettings.Instance.AllTracks;
                   getALLInterests(res =>
                   {
                       interestSource = res;
                       adapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, res,lstTracks,Screens.Session);
                       Activity.RunOnUiThread(() =>
                       {
                           setHeader();
                           lstInterest.Adapter = adapter;
                       });
                   });


                   lstInterest.ItemClick += (s, e) =>
                   {
                       currentSession = interestSource[e.Position];
                       Intent intent = new Intent(Activity,typeof(SessionDetail));
                       intent.PutExtra("uid", currentSession.BuiltSession.session_id);
                       intent.PutExtra("date", currentSession.date);
                       intent.PutExtra("time", currentSession.time);
                       intent.PutExtra("isFromActivity", "MyInterest");
                       intent.PutExtra("title", currentSession.BuiltSession.title);
                       intent.PutExtra("abbreviation", currentSession.BuiltSession.abbreviation);
                       StartActivity(intent);
                   };


                   activity=((MainActivity)Activity);
                   activity.interestReceiver.OnBroadcastReceive += interestReceiver_OnBroadcastReceive;
                   activity.RegisterReceiver(activity.interestReceiver, new IntentFilter(InterestReceiver.action));
                  return parentView;
       }

       void interestReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
       {
           adapter.Remove(currentSession);
           currentSession = null;
           adapter.NotifyDataSetChanged();
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


               lstInterest.AnimationCacheEnabled = false;
               lstInterest.ScrollingCacheEnabled = false;
           }
       }

       private void filterInterestUsingLinq(string dateTime)
        {
            DataManager.GetMyInterest(DBHelper.Instance.Connection).ContinueWith(t =>
              {
                 var allInterest = t.Result;
                  CurrentSelectedDate = Helper.convertToBuiltDate(dateTime);
                  var ids = allInterest.Select(p => p.session_time_id).ToList();
                  DataManager.GetSessionTimeFromSessionIds(DBHelper.Instance.Connection, ids).ContinueWith(s =>
                  {
                      var data = s.Result;
                      if (!string.IsNullOrEmpty(this.CurrentSelectedDate) && this.CurrentSelectedDate.ToLower().Contains("days"))
                      {
                          interestSource = data.OrderBy(p => p.time).ToList();
                          Activity.RunOnUiThread(() =>
                          {
                            adapter.Clear();
                            adapter.AddAll(interestSource);
                            lstInterest.Adapter = adapter;
                            adapter.NotifyDataSetChanged();
                          });
                          return;
                      }

                      if (!string.IsNullOrEmpty(this.CurrentSelectedDate))
                      {
                          var expectedResult = data.Where(q => q.date == CurrentSelectedDate);
                          interestSource = expectedResult.ToList().OrderBy(p=>p.time).ToList();
                          Activity.RunOnUiThread(() =>
                          {
                              adapter.Clear();
                              adapter.AddAll(interestSource);
                              lstInterest.Adapter = adapter;
                              adapter.NotifyDataSetChanged();
                          });
                          return;
                      }
                  });
              });
       }

       private void sessionGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
       {
           var current = View.FindViewById<RadioButton>(e.CheckedId);
           int i = sessionGroup.IndexOfChild(current);

           var date = lstDate[i];
           if (date.ToLower().Contains("days"))
           {
               CurrentSelectedDate = date;
               filterInterestUsingLinq(date);
           }
           else
           {
               var dt = lstDate.Where(p => p == date).FirstOrDefault();
               filterInterestUsingLinq(dt);
           }
       }

       private void getALLInterests(Action<List<BuiltSessionTime>> callback)
       {
           Func<string, string> convertToDate = (Date) =>
           {
               var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
               return date;
           };
           List<BuiltSessionTime> temp = new List<BuiltSessionTime>();
           Dictionary<string, BuiltSessionTime[]> filteredList = new Dictionary<string, BuiltSessionTime[]>();
           DataManager.GetMyInterest(DBHelper.Instance.Connection).ContinueWith(t =>
           {
               var allInterest = t.Result;
               if (allInterest != null && allInterest.Count > 0)
               {
                   var ids = allInterest.Select(p => p.session_time_id).ToList();
                   DataManager.GetSessionTimeFromSessionIds(DBHelper.Instance.Connection, ids).ContinueWith(s =>
                   {
                       var result = s.Result;

                       if (lstDate == null) lstDate = new List<string>();
                       lstDate = result.Select(q => q.date).Distinct().ToList();
                       if (lstDate.Count > 0)
                       {
                           lstDate.Insert(0, "All,Days");
                       }

                       if (callback != null)
                       {
                           callback(result);
                       }
                       
                   });
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

       public override void OnDestroy()
       {
           base.OnDestroy();
           if (activity.interestReceiver != null)
           {
               activity.UnregisterReceiver(activity.interestReceiver);
           }
       }
    }

   public class InterestAdapter : ArrayAdapter<BuiltSessionTime>
   {
       TextView sessionName;
       TextView room;
       TextView sessionDateTime;
       ImageView trackColor;
       List<BuiltSessionTime> items;
       BuiltTracks builtTracks = null;
       Android.App.Activity context;
       string[] keys;
       public int selectedTab = 0;
       private readonly int _resource;
       List<BuiltTracks> parentTracks;
       public InterestAdapter(Android.App.Activity context, int resource,  List<BuiltSessionTime> items,List<BuiltTracks> parentTracks)
           : base(context, resource, items)
       {
           this.context = context;
           this.items = items;
           this.parentTracks = parentTracks;
           _resource = resource;
       }

       public override View GetView(int position, View convertView, ViewGroup parent)
       {
           var builtSessionTime = items[position];

           View view = convertView;
           if (view == null) // no view to re-use, create new
               view = (RelativeLayout)this.context.LayoutInflater.Inflate(_resource, parent, false);

           sessionName = view.FindViewById<TextView>(Resource.Id.session_title_name_tv);
           room = view.FindViewById<TextView>(Resource.Id.session_address_tv);
           sessionDateTime = view.FindViewById<TextView>(Resource.Id.session_date_time_tv);
           trackColor = view.FindViewById<ImageView>(Resource.Id.session_track_color);
           UpdateCell(builtSessionTime, parentTracks);
           return view;
       }

       public void UpdateCell(BuiltSessionTime builtSessionTime,List<BuiltTracks> parentTracks)
       {
           if (builtSessionTime.BuiltSession != null)
           {
               builtTracks = parentTracks.FirstOrDefault(p => p.name == builtSessionTime.BuiltSession.track);
               sessionName.Text = builtSessionTime.BuiltSession.title;
           }
           if (builtTracks == null)
           {
               builtTracks = parentTracks.FirstOrDefault(p => p.name.ToLower() == "no track");
           }
           room.Text = builtSessionTime.room;
           var startDate=convertToStartDate(builtSessionTime.time);
           var endDate=convertToEndDate(builtSessionTime.time, builtSessionTime.length);
           sessionDateTime.Text = startDate + endDate;
           //if (builtTracks != null)
           //{
           //    trackColor.SetBackgroundColor(Android.Graphics.Color.ParseColor(builtTracks.color));
           //}
       }

       public string convertToStartDate(string time)
       {
           if (String.IsNullOrWhiteSpace(time))
               return String.Empty;

           try
           {
               var date = DateTime.Parse(time);
               return ToDateTimeString(date, "hh:mm tt").ToLower();
           }
           catch
           {
               return String.Empty;
           }
       }

       public string convertToEndDate(string time, string length)
       {
           if (String.IsNullOrWhiteSpace(time))
               return String.Empty;

           try
           {
               var endDate = DateTime.Parse(time).AddMinutes(Convert.ToInt32(length));
               return ToDateTimeString(endDate, "hh:mm tt").ToLower();
           }
           catch
           {
               return String.Empty;
           }
       }
       public static string ToDateTimeString(DateTime dt, string format)
       {
           if (String.IsNullOrWhiteSpace(format))
               return String.Empty;
           else
               return dt.ToString(format);
       }
   }
}