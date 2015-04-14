using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Utilities;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views;
using Android.App;
using ConferenceAppDroid.Adapters;
using System.Threading.Tasks;
using UrlImageViewHelper;
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Fragments
{
    public class Transportation : Android.Support.V4.App.Fragment
    {
        ListView lstTransport;
        TransportationCustomAdapter adapter;
        List<BuiltTransportation> source;
        MainActivity activity;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.transportationLayout, null);
            lstTransport = view.FindViewById<ListView>(Resource.Id.transportationListView);

            DataManager.GetTransportationItem(DBHelper.Instance.Connection).ContinueWith(t =>
                {
                    Activity.RunOnUiThread(() =>
                    {

                        source=t.Result.ToList();
                        adapter = new TransportationCustomAdapter(Activity, Resource.Layout.row_list_sf, source);
                        lstTransport.Adapter = adapter;
                    });
                });

            lstTransport.ItemClick += (s, e) =>
                {
                    var currentTransportation = source[e.Position];
                    if (!string.IsNullOrWhiteSpace(currentTransportation.icon.url))
                    {
                        Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(currentTransportation.icon.url));
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
            updateTransportation();
        }

        private void updateTransportation()
        {
            DataManager.GetTransportationItem(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                source = t.Result.ToList();

                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                        {
                            adapter.Clear();
                            adapter.AddAll(source);
                            lstTransport.Adapter = adapter;
                            adapter.NotifyDataSetChanged();
                        });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter = new TransportationCustomAdapter(Activity, Resource.Layout.row_list_sf, source);
                        lstTransport.Adapter = adapter;
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

        //SeparatedListAdapter CreateAdapter<T>(Dictionary<string, List<T>> sortedObjects)
        //where T : IHasLabel, IComparable<T>
        //{
        //    var adapter = new SeparatedListAdapter(Activity);
        //    foreach (var e in sortedObjects)
        //    {
        //        var label = e.Key;
        //        var section = e.Value;
        //        //adapter.AddSection(label, new ArrayAdapter<T>(Activity, Resource.Layout.menu_row, section));
        //        adapter.AddSection(label, new TransportationAdapter<T>(Activity, section));
        //    }
        //    return adapter;
        //}
        //private class TransportationAdapter<T> : BaseAdapter<T>
        //{
        //    TextView title, description;
        //    ImageView sf_image_dp;
        //    Context context;
        //    List<T> data;
        //    public TransportationAdapter(Context context, List<T> data)
        //        : base()
        //    {
        //        this.context = context;
        //        this.data = data;
        //    }

        //    public override View GetView(int position, View convertView, ViewGroup parent)
        //    {
        //        ViewHolder viewHolder;
        //        View view = convertView;
        //        if (null == convertView)
        //        {
        //            view = LayoutInflater.From(context).Inflate(Resource.Layout.row_list_sf, null);
        //            title = view.FindViewById<TextView>(Resource.Id.sf_text_title);
        //            description = view.FindViewById<TextView>(Resource.Id.sf_text_desc);
        //            sf_image_dp = view.FindViewById<ImageView>(Resource.Id.sf_image_dp);
        //            viewHolder = new ViewHolder(title, description, sf_image_dp);
        //            view.Tag = viewHolder;
        //        }
        //        else
        //        {
        //            viewHolder = (ViewHolder)view.Tag;
        //            title = viewHolder.title;
        //            description = viewHolder.description;
        //            sf_image_dp = viewHolder.sf_image_dp;
        //        }




        //        var item = GetItem(position) as ListItemTransportationValue;




        //        var name = item.SectionItem.name;
        //        var desc = item.SectionItem.short_desc;
        //        if (!String.IsNullOrWhiteSpace(name))
        //        {
        //            title.Text = name;
        //        }

        //        if (!String.IsNullOrWhiteSpace(desc))
        //        {
        //            description.Text = desc;
        //        }
        //        if (!String.IsNullOrWhiteSpace(url))
        //        {
        //            UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(sf_image_dp, item.SectionItem.icon.url, Resource.Drawable.ic_default_pic);
        //        }
        //        return convertView;
        //    }

        //    public override T this[int position]
        //    {
        //        get { return data[position]; }
        //    }

        //    public override int Count
        //    {
        //        get { return data.Count; }
        //    }

        //    public override long GetItemId(int position)
        //    {
        //        return position;
        //    }

        //}
        public class ViewHolder : Java.Lang.Object
        {
            public TextView title;
            public TextView description;
            public ImageView sf_image_dp;
            public LinearLayout sf_section;
            public TextView sf_sectionTitle;

            public ViewHolder(TextView title, TextView description, ImageView sf_image_dp, LinearLayout sf_section, TextView sf_sectionTitle)
            {
                this.title = title;
                this.description = description;
                this.sf_image_dp = sf_image_dp;
                this.sf_section = sf_section;
                this.sf_sectionTitle = sf_sectionTitle;
            }
        }

        public class TransportationCustomAdapter : ArrayAdapter<BuiltTransportation>
        {
            Android.App.Activity context;
            List<BuiltTransportation> items;
            int _resource;
            TextView title, description;
            ImageView sf_image_dp;
            public LinearLayout sf_section;
            public TextView sf_sectionTitle;
            public TransportationCustomAdapter(Android.App.Activity context, int resource, List<BuiltTransportation> items)
                : base(context, resource, items)
            {
                this.context = context;
                this.items = items;
                this._resource = resource;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ViewHolder viewHolder;
                View view = convertView;
                if (null == view)
                {
                    view = LayoutInflater.From(context).Inflate(Resource.Layout.row_list_sf, null);
                    title = view.FindViewById<TextView>(Resource.Id.sf_text_title);
                    description = view.FindViewById<TextView>(Resource.Id.sf_text_desc);
                    sf_image_dp = view.FindViewById<ImageView>(Resource.Id.sf_image_dp);
                    sf_sectionTitle = view.FindViewById<TextView>(Resource.Id.sf_sectionTitle);
                    sf_section = view.FindViewById<LinearLayout>(Resource.Id.sf_section);
                    viewHolder = new ViewHolder(title, description, sf_image_dp,sf_section,sf_sectionTitle);
                    view.Tag = viewHolder;
                }
                else
                {
                    viewHolder = (ViewHolder)view.Tag;
                    title = viewHolder.title;
                    description = viewHolder.description;
                    sf_image_dp = viewHolder.sf_image_dp;
                    sf_section = viewHolder.sf_section;
                    sf_sectionTitle = viewHolder.sf_sectionTitle;
                }

                var transportation = GetItem(position);
                sf_section.Visibility = ViewStates.Visible;
                sf_sectionTitle.Text = transportation.category;
                if (position != 0)
                {
                    var previousItem = GetItem(position - 1);
                    if (!previousItem.category.Equals(transportation.category, StringComparison.InvariantCultureIgnoreCase))
                    {

                        sf_section.Visibility = ViewStates.Visible;
                        sf_sectionTitle.Text = transportation.category;
                    }
                    else
                    {
                        sf_section.Visibility = ViewStates.Gone;
                    }
                }

                if (!String.IsNullOrWhiteSpace(transportation.name))
                {
                    title.Text = transportation.name;
                }

                if (!String.IsNullOrWhiteSpace(transportation.short_desc))
                {
                    description.Text = transportation.short_desc;
                }
                if (!String.IsNullOrWhiteSpace(transportation.icon.url))
                {
                    UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(sf_image_dp, transportation.icon.url, Resource.Drawable.ic_default_pic);
                }

                return view;
            }
        }

    }
}
