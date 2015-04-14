using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Fragments
{
    public class Venue : Android.Support.V4.App.Fragment
    {
        ListView lstVenue;
        List<BuiltVenue> venue;
        MainActivity activity;
        VenueAdapter adapter;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.VenueLayout, null);
            lstVenue = view.FindViewById<ListView>(Resource.Id.lstVenue);
            DataManager.GetListOfVenues(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                Activity.RunOnUiThread(() =>
                {
                    venue=t.Result;
                    var result=venue.OrderBy(p => p.name.ToUpper()).ToList();
                    var adapter = new VenueAdapter(Activity, Resource.Layout.VenueLayout, result);
                    lstVenue.Adapter = adapter;
                });
            });

             lstVenue.ItemClick += (s, e) =>
                {
                    var currentVenue = venue[e.Position];
                    var activity2 = new Intent(Activity, typeof(VenueDetail));
                    if (!string.IsNullOrWhiteSpace(currentVenue.name))
                    {
                        activity2.PutExtra("Name", currentVenue.name);
                    }
                    if (!string.IsNullOrWhiteSpace(currentVenue.info))
                    {
                        activity2.PutExtra("Address", currentVenue.info);
                    }

                    if (currentVenue.image!=null && !string.IsNullOrWhiteSpace(currentVenue.image.url))
                    {
                        activity2.PutExtra("ImageUrl", currentVenue.image.url);
                    }

                    StartActivity(activity2);
                    Activity.OverridePendingTransition(Resource.Animation.none, Resource.Animation.ltr);
                };

             activity = ((MainActivity)Activity);
             activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive;
             activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));

            return view;
        }

        private void deltaCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            updateVenue();
        }

        private void updateVenue()
        {
            DataManager.GetListOfVenues(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                venue = t.Result;
                var result = venue.OrderBy(p => p.name.ToUpper()).ToList();
                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.Clear();
                        adapter.AddAll(result);
                        lstVenue.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter = new VenueAdapter(Activity, Resource.Layout.VenueLayout, result);
                        lstVenue.Adapter = adapter;
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
    }

    public class VenueAdapter : ArrayAdapter<BuiltVenue>
    {
        TextView txtTitle, txtDescription;
        Android.App.Activity dailyHighlightsActivity;
        List<BuiltVenue> venue;
        public VenueAdapter(Android.App.Activity context, int resource, List<BuiltVenue> venue)
            : base(context, resource, venue)
        {
            dailyHighlightsActivity = context;
            this.venue = venue;
        }

        public override int Count
        {
            get { return venue.Count; }
        }


        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView; // re-use an existing view, if one is available
            if (convertView == null)
            {
                view = dailyHighlightsActivity.LayoutInflater.Inflate(Resource.Layout.VenueCells, null);
                txtTitle = view.FindViewById<TextView>(Resource.Id.moscone_row_title);
                txtDescription = view.FindViewById<TextView>(Resource.Id.moscone_row_desc);
                viewHolder = new ViewHolder(txtTitle, txtDescription);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder=(ViewHolder)view.Tag;
                txtTitle=viewHolder.txtTitle;
                txtDescription=viewHolder.txtDescription;
            }

            
            txtTitle.SetTypeface(txtTitle.Typeface, Android.Graphics.TypefaceStyle.Bold);

            var name = GetItem(position).name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                txtTitle.Text = name;
            }

           
            txtDescription.SetTypeface(txtDescription.Typeface, Android.Graphics.TypefaceStyle.Normal);

            var address = GetItem(position).address.Trim();
            if (!string.IsNullOrWhiteSpace(address))
            {
                txtDescription.Text = address;
            }
            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView txtTitle;
            public TextView txtDescription;

            public ViewHolder(TextView txtTitle,TextView txtDescription)
            {
                this.txtTitle = txtTitle;
                this.txtDescription = txtDescription;
            }
        }
    }


}