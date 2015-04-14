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
    public class ImportantLinksFragment : Android.Support.V4.App.Fragment
    {
        View parentView;
        ListView importantLinkListView;
        List<BuiltImportantLinks> impLinksSource;
        ImportantLinksAdapter adapter;
        MainActivity activity;
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.fragment_important_link, null);
            importantLinkListView = (ListView)parentView.FindViewById(Resource.Id.importantLinksList);

            DataManager.GetImpLinks(DBHelper.Instance.Connection).ContinueWith(p =>
               {
                   var config=AppSettings.Instance.config;
                   var sequence = config.important_links.imp_link_ordering.OrderBy(u => u.link_sequence).ToArray();

                       Func<string, int> getIndex = (s) =>
                         {
                             var temp = sequence.FirstOrDefault(res => res.link_category == s);
                             if (temp == null)
                             {
                                 return 0;
                             }
                             else
                             {
                                 return temp.link_sequence;
                             }
                         };
                       var result = p.Result;
                       impLinksSource = result.OrderBy(u => u.sequence).ToList();
                       impLinksSource = impLinksSource.OrderBy(q => getIndex(q.category)).ToList();
                       Activity.RunOnUiThread(() =>
                           {
                               adapter = new ImportantLinksAdapter(Activity, Resource.Layout.row_important_link, impLinksSource);
                               importantLinkListView.Adapter = adapter;
                           });

               });

            activity = ((MainActivity)Activity);
            activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));

            return parentView;
        }

        private void deltaCompletedReceiver_OnBroadcastReceive(Android.Content.Context arg1, Android.Content.Intent arg2)
        {
            updateImpLinks();
    
        }

        private void updateImpLinks()
        {

            DataManager.GetImpLinks(DBHelper.Instance.Connection).ContinueWith(p =>
            {
                var config = AppSettings.Instance.config;
                var sequence = config.important_links.imp_link_ordering.OrderBy(u => u.link_sequence).ToArray();

                Func<string, int> getIndex = (s) =>
                {
                    var temp = sequence.FirstOrDefault(res => res.link_category == s);
                    if (temp == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return temp.link_sequence;
                    }
                };
                var result = p.Result;
                impLinksSource = result.OrderBy(u => u.sequence).ToList();
                impLinksSource = impLinksSource.OrderBy(q => getIndex(q.category)).ToList();
                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.Clear();
                        adapter.AddAll(impLinksSource);
                        importantLinkListView.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter = new ImportantLinksAdapter(Activity, Resource.Layout.row_important_link, impLinksSource);
                        importantLinkListView.Adapter = adapter;
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

    public class ImportantLinksAdapter : ArrayAdapter<BuiltImportantLinks>
    {
        Android.App.Activity context;
        int _resource;
        List<BuiltImportantLinks> items;
        public TextView row_important_link_title_TextView, row_important_link_description_TextView, important_link_sectionTitle;
        public LinearLayout important_section;
        public ImportantLinksAdapter(Android.App.Activity context, int resource, List<BuiltImportantLinks> items)
            : base(context, resource, items)
        {
            this.context = context;
            this.items = items;
            _resource = resource;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView;
            if (null == convertView)
            {
                view = LayoutInflater.From(context).Inflate(_resource, null);
                row_important_link_title_TextView = view.FindViewById<TextView>(Resource.Id.row_important_link_title_TextView);
                row_important_link_description_TextView = view.FindViewById<TextView>(Resource.Id.row_important_link_description_TextView);
                important_link_sectionTitle = view.FindViewById<TextView>(Resource.Id.important_link_sectionTitle);
                important_section = view.FindViewById<LinearLayout>(Resource.Id.important_section);
                viewHolder = new ViewHolder(row_important_link_title_TextView, row_important_link_description_TextView, important_link_sectionTitle, important_section);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                row_important_link_title_TextView = viewHolder.row_important_link_title_TextView;
                row_important_link_description_TextView = viewHolder.row_important_link_description_TextView;
                important_link_sectionTitle = viewHolder.important_link_sectionTitle;
                important_section = viewHolder.important_section;
            }

            var impLinks = GetItem(position);

            important_section.Visibility = ViewStates.Visible;
            important_link_sectionTitle.Text = impLinks.category;
            if (position != 0)
            {
                var previousItem = GetItem(position - 1);
                if (!previousItem.category.Equals(impLinks.category, StringComparison.InvariantCultureIgnoreCase))
                {

                    important_section.Visibility = ViewStates.Visible;
                    important_link_sectionTitle.Text = (impLinks.category);
                }
                else
                {
                    important_section.Visibility = ViewStates.Gone;
                }
            }

            if (!string.IsNullOrWhiteSpace(impLinks.title))
            {
                row_important_link_title_TextView.Text = impLinks.title;
            }

            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView row_important_link_title_TextView, row_important_link_description_TextView, important_link_sectionTitle;
            public LinearLayout important_section;
            public ViewHolder(TextView row_important_link_title_TextView, TextView row_important_link_description_TextView, TextView important_link_sectionTitle, LinearLayout important_section)
            {
                this.row_important_link_title_TextView = row_important_link_title_TextView;
                this.row_important_link_description_TextView = row_important_link_description_TextView;
                this.important_link_sectionTitle = important_link_sectionTitle;
                this.important_section = important_section;
            }

        }
    }
}
