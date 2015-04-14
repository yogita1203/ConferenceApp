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
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Fragments
{
    public class DailyHighlights : Android.Support.V4.App.Fragment
    {
       ListView lstNews;
       List<BuiltNews> news;
       MainActivity activity;
       NewsAdapter adapter;
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.DailyHighlightsLayout, null);
            lstNews = view.FindViewById<ListView>(Resource.Id.lstNews);

            DataManager.GetNews(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                Activity.RunOnUiThread(() =>
                {
                    news=t.Result;
                    var adapter = new NewsAdapter(Activity, Resource.Layout.DailyHighlightsLayout, news);
                    lstNews.Adapter = adapter;
                });
            });

            lstNews.ItemClick += (s, e) =>
                {
                    var currentNews = news[e.Position];
                    var activity2 = new Intent(Activity, typeof(DailyHighlightsDetailsActivity));
                    if (!string.IsNullOrWhiteSpace(currentNews.title))
                    {
                        activity2.PutExtra("Title", currentNews.title);
                    }
                    if (!string.IsNullOrWhiteSpace(currentNews.published_date))
                    {
                        activity2.PutExtra("Date",currentNews.published_date);
                    }
                    if (!string.IsNullOrWhiteSpace(currentNews.desc))
                    {
                        activity2.PutExtra("Description", currentNews.desc.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(currentNews.link.href))
                    {
                        activity2.PutExtra("Link", currentNews.link.href);
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
            updateNews();
        }

        private void updateNews()
        {
            DataManager.GetNews(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                news = t.Result;
                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.Clear();
                        adapter.AddAll(news);
                        lstNews.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter = new NewsAdapter(Activity, Resource.Layout.DailyHighlightsLayout, news);
                        lstNews.Adapter = adapter;
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

    public class NewsAdapter : ArrayAdapter<BuiltNews>
    {
        Android.App.Activity dailyHighlightsActivity;
        List<BuiltNews> news;
        TextView txtTitle;
        TextView txtDate;
        TextView txtDescription;
        public NewsAdapter(Android.App.Activity context, int resource, List<BuiltNews> news)
            : base(context, resource, news )
        {
            dailyHighlightsActivity = context ;
            this.news = news;
        }

        public override int Count
        {
            get { return news.Count; }
        }


        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView; // re-use an existing view, if one is available
            if (convertView == null)
            {
                view = dailyHighlightsActivity.LayoutInflater.Inflate(Resource.Layout.NewsCells, null);
                txtTitle = view.FindViewById<TextView>(Resource.Id.news_textview_title);
                txtDate = view.FindViewById<TextView>(Resource.Id.news_textview_publishedDate);
                txtDescription = view.FindViewById<TextView>(Resource.Id.news_textview_descr);
                viewHolder = new ViewHolder(txtTitle, txtDate, txtDescription);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder=(ViewHolder)view.Tag;
                txtTitle=viewHolder.txtTitle;
                txtDate=viewHolder.txtDate;
                txtDescription=viewHolder.txtDescription;
            }

          
            txtTitle.SetTypeface(txtTitle.Typeface, Android.Graphics.TypefaceStyle.Bold);
            
            var title = GetItem(position).title;
            if(!string.IsNullOrWhiteSpace(title))
            {
                txtTitle.Text =title ;
            }

            var date = GetItem(position).published_date;
         
            if (!string.IsNullOrWhiteSpace(date))
            {
                txtDate.Visibility = ViewStates.Visible;
                txtDate.Text = Helper.convertToNewsDate(date);
            }

            else
            {
                txtDate.Visibility = ViewStates.Gone;
            }

           
            txtDescription.SetTypeface(txtDescription.Typeface, Android.Graphics.TypefaceStyle.Normal);

            var desc=GetItem(position).desc.Trim();
            if(!string.IsNullOrWhiteSpace(desc))
            {
                txtDescription.Text = desc;
            }
            return view;
        }
        public class ViewHolder : Java.Lang.Object
        {
            public TextView txtTitle;
            public TextView txtDate;
            public TextView txtDescription;

            public ViewHolder(TextView txtTitle, TextView txtDate, TextView txtDescription)
            {
                this.txtTitle = txtTitle;
                this.txtDate = txtDate;
                this.txtDescription = txtDescription;
            }
        }
    }

}
