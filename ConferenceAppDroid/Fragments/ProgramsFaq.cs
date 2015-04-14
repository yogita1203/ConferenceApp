using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Fragments
{
    public class ProgramsFaq : Fragment
    {
        ListView faq_list;
        ProgramsFaqAdapter adapter;
        List<BuiltOthers> lstOthers;
        MainActivity activity;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.prog_faq, null);
            faq_list = view.FindViewById<ListView>(Resource.Id.lstFaq);

            DataManager.GetListOfProgramsFromOthers(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                Activity.RunOnUiThread(() =>
                {
                    lstOthers = t.Result;
                    lstOthers = lstOthers.OrderBy(p => p.sequence).ToList();
                    adapter = new ProgramsFaqAdapter(Activity, Resource.Layout.faq_row, lstOthers);
                    faq_list.Adapter = adapter;
                });
            });

            faq_list.ItemClick += (s, e) =>
                {
                    var Url = lstOthers[e.Position].url;
                    if (Url.StartsWith("vmwareapp"))
                    {
                        var title = Helper.getStringToShowFragment(lstOthers[e.Position].url);
                        if (!string.IsNullOrWhiteSpace(title) && title.Equals("handsonlabs", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Intent intent = new Intent(Activity, (typeof(HandsOnLabsActivity)));
                            intent.PutExtra("title", lstOthers[e.Position].title);
                            StartActivity(intent);
                        }

                    }
                    else
                    {
                        Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(Url));
                        StartActivity(browserIntent);
                    }

                };

            activity = ((MainActivity)Activity);
            activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));

            return view;
        }

        private void deltaCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            updateFaq();
        }

        private void updateFaq()
        {
            DataManager.GetListOfProgramsFromOthers(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                lstOthers = t.Result;
                lstOthers = lstOthers.OrderBy(p => p.sequence).ToList();
                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.Clear();
                        adapter.AddAll(lstOthers);
                        faq_list.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter = new ProgramsFaqAdapter(Activity, Resource.Layout.faq_row, lstOthers);
                        faq_list.Adapter = adapter;
                    });
                }
            });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (activity.deltaCompletedReceiver != null)
            {
                activity.UnregisterReceiver(activity.deltaCompletedReceiver);
            }
        }

        public class ProgramsFaqAdapter : ArrayAdapter<BuiltOthers>
        {
            Android.App.Activity context;
            List<BuiltOthers> others;
            TextView faq_textview_title;
            int _resource;
            public ProgramsFaqAdapter(Android.App.Activity context, int resource, List<BuiltOthers> others)
                : base(context, resource, others)
            {
                this.context = context;
                this._resource = resource;
                this.others = others;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ViewHolder viewHolder;
                View view = convertView; // re-use an existing view, if one is available
                if (convertView == null)
                {
                    view = context.LayoutInflater.Inflate(_resource, null);
                    faq_textview_title = view.FindViewById<TextView>(Resource.Id.faq_textview_title);
                    viewHolder = new ViewHolder(faq_textview_title);
                    view.Tag = viewHolder;
                }
                else
                {
                    viewHolder = (ViewHolder)view.Tag;
                    faq_textview_title = viewHolder.faq_textview_title;
                }

                var others = GetItem(position);
                if (!string.IsNullOrWhiteSpace(others.title))
                {
                    faq_textview_title.Text = others.title;
                }
                return view;

            }
            public class ViewHolder : Java.Lang.Object
            {
                public TextView faq_textview_title;
                public ViewHolder(TextView faq_textview_title)
                {
                    this.faq_textview_title = faq_textview_title;
                }
            }

        }
    }
}