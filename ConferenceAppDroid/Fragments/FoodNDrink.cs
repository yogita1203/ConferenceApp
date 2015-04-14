using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.CustomControls;
using ConferenceAppDroid.Utilities;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.Fragments
{
    public class FoodNDrink : Android.Support.V4.App.Fragment
    {
        RadioGroup radioGroup;
        ListView FoodNDrinkListView;
        Dictionary<string, BuiltSFFoodNDrink[]> foodNdrinks;
        HomeScreenAdapter adapter;
        MainActivity activity;
        SegmentedControlButton segmentedControlButton;
        List<string> segmentTitle;
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.FoodNDrinkLayout, null);
            FoodNDrinkListView = view.FindViewById<ListView>(Resource.Id.FoodNDrinkListView);
            radioGroup = view.FindViewById<RadioGroup>(Resource.Id.foodNDrinkGroup);
           
            DataManager.GetFoodNDrink(DBHelper.Instance.Connection).ContinueWith(t =>
          {
              foodNdrinks=t.Result;
              segmentTitle = foodNdrinks.Keys.ToList();
              adapter= new HomeScreenAdapter(Activity,Resource.Layout.row_list_sf, foodNdrinks);
              Activity.RunOnUiThread(() =>
                  {
                      if (radioGroup.ChildCount > 0)
                      {
                          radioGroup.RemoveAllViews();
                      }
                      for (int i = 0; i < segmentTitle.Count; i++)
                      {
                          if (i == 0)
                          {
                              //LinearLayout linearView = new LinearLayout(Activity);
                              LinearLayout.LayoutParams paramss = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 68, 1f);
                              //segmentedControlButton = (SegmentedControlButton)LayoutInflater.From(Activity).Inflate(Resource.Layout.temp, null);
                              segmentedControlButton = new SegmentedControlButton(Activity);
                              segmentedControlButton.SetMinimumHeight(68);
                              segmentedControlButton.LayoutParameters = paramss;
                              segmentedControlButton.Text = segmentTitle[i];
                              segmentedControlButton.Id = i;
                              segmentedControlButton.Checked = true;
                              radioGroup.AddView(segmentedControlButton);
                          }
                          else
                          {
                              LinearLayout.LayoutParams paramss = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 68, 1f);
                              segmentedControlButton = new SegmentedControlButton(Activity);
                              segmentedControlButton.LayoutParameters = paramss;
                              //segmentedControlButton = (SegmentedControlButton)LayoutInflater.From(Activity).Inflate(Resource.Layout.temp, null);
                              segmentedControlButton.SetMinimumHeight(68);
                              segmentedControlButton.Id = i;
                              segmentedControlButton.Text = segmentTitle[i];
                              segmentedControlButton.Checked = false;
                              radioGroup.AddView(segmentedControlButton);
                          }

                      }
                      radioGroup.CheckedChange += radioGroup_CheckedChange;
                      FoodNDrinkListView.Adapter = adapter;
                  });
          });

            FoodNDrinkListView.ItemClick += (s, e) =>
                {
                    var item = foodNdrinks[adapter.keys[adapter.selectedTab]][e.Position];
                    if (item.link_group != null)
                    {
                        var link = Helper.getFoodDrinkLink(item.link_group);
                        Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(link));
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
            DataManager.GetFoodNDrink(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                foodNdrinks = t.Result;
                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.Clear();
                        adapter.AddAll(foodNdrinks);
                        FoodNDrinkListView.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                
                Activity.RunOnUiThread(() =>
                {
                    adapter = new HomeScreenAdapter(Activity, Resource.Layout.row_list_sf, foodNdrinks);
                    FoodNDrinkListView.Adapter = adapter;
                });
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

        

        void radioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            var current =View.FindViewById<RadioButton>(radioGroup.CheckedRadioButtonId);
            int i=radioGroup.IndexOfChild(current);
            adapter.selectedTab = i;
            adapter.NotifyDataSetChanged();
        }
    }

    public class HomeScreenAdapter : ArrayAdapter<BuiltSFFoodNDrink>
    {
        Dictionary<string,BuiltSFFoodNDrink[]> items;
        Android.App.Activity context;
        public string[] keys;
        public int selectedTab = 0;
        private readonly int _resource;
        TextView sf_text_title, sf_text_desc;
        ImageView sf_image_dp;
        public HomeScreenAdapter(Android.App.Activity context,int resource, Dictionary<string,BuiltSFFoodNDrink[]> items):base(context,resource,items.Values.ToList()[0])
        {
            this.context = context;
            this.items = items;
            _resource = resource;
            keys = items.Keys.ToArray();
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            var item = items[keys [selectedTab]][position];
           
            View view = convertView;
            if (convertView == null)
            {
                view = (RelativeLayout)this.context.LayoutInflater.Inflate(_resource, parent, false);
                sf_text_title = view.FindViewById<TextView>(Resource.Id.sf_text_title);
                sf_text_desc = view.FindViewById<TextView>(Resource.Id.sf_text_desc);
                sf_image_dp = view.FindViewById<ImageView>(Resource.Id.sf_image_dp);
                viewHolder = new ViewHolder(sf_text_title, sf_text_desc, sf_image_dp);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder=(ViewHolder)view.Tag;
                sf_text_title=viewHolder.sf_text_title;
                sf_text_desc=viewHolder.sf_text_desc;
                sf_image_dp = viewHolder.sf_image_dp;
            }

            sf_text_title.Text = item.venue_name;
            sf_text_desc.Text = item.address;
            if (item.icon != null)
            {
                if (!System.String.IsNullOrWhiteSpace(item.icon.url))
                {
                    UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(sf_image_dp, item.icon.url, Resource.Drawable.ic_default_pic);
                }

            }
            else
            {
                sf_image_dp.SetImageResource( Resource.Drawable.ic_default_pic);
            }
            return view;
        }


    }

    public class ViewHolder : Java.Lang.Object
    {
        public TextView sf_text_title;
        public TextView sf_text_desc;
        public ImageView sf_image_dp;
        public ViewHolder(TextView sf_text_title, TextView sf_text_desc, ImageView sf_image_dp)
        {
            this.sf_text_title = sf_text_title;
            this.sf_text_desc = sf_text_desc;
            this.sf_image_dp = sf_image_dp;
        }
    }
}
