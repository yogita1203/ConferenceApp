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
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Activities;
using Newtonsoft.Json;
using Android.OS;
using Android.Views.InputMethods;


namespace ConferenceAppDroid.Fragments
{
    public class Speaker : Android.Support.V4.App.Fragment
    {
        ListView lstSpeaker;
        const int speakerLimit = 50;
        private int speakerOffset = 0;
        List<BuiltSpeaker> SpeakerSource;
        TextView searchTextView, titleTextView, txtSearch;
        MainActivity activity;
        SpeakerAdapter adapter;
        RelativeLayout speakerSearchContainer;
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var actionbar = Activity.ActionBar.CustomView;
            searchTextView = actionbar.FindViewById<TextView>(Resource.Id.searchTextView);
            searchTextView.Visibility = ViewStates.Visible;
            var lmtUnicode = Helper.getUnicodeString(Activity, searchTextView, "&#xf002");
            searchTextView.Text = lmtUnicode;


            var view = inflater.Inflate(Resource.Layout.speakerLayout, null);
            lstSpeaker = view.FindViewById<ListView>(Resource.Id.speakerListView);
            speakerSearchContainer = view.FindViewById<RelativeLayout>(Resource.Id.speaker_search_container);
             txtSearch = view.FindViewById<EditText>(Resource.Id.speaker_search_edit_text);
            var btnCancel = view.FindViewById<Button>(Resource.Id.speaker_search_cancel_container_btn);
            var btnSerchCancel = view.FindViewById<Button>(Resource.Id.speaker_search_cancel_btn);
            var searchIcon = view.FindViewById<Button>(Resource.Id.speaker_search_btn);
            searchTextView.Click += (s, e) =>
            {
                if (speakerSearchContainer.Visibility == ViewStates.Gone)
                {
                    speakerSearchContainer.Visibility = ViewStates.Visible;
                    searchIcon.Visibility = ViewStates.Visible;
                    btnSerchCancel.Visibility = ViewStates.Visible;

                }
                else
                {
                    speakerSearchContainer.Visibility = ViewStates.Gone;
                    searchIcon.Visibility = ViewStates.Gone;
                    btnSerchCancel.Visibility = ViewStates.Gone;
                    txtSearch.Text = string.Empty;
                    hideSoftInputFromWindow();
                }
            };


            DataManager.GetSpeakersOnAndroid(DBHelper.Instance.Connection).ContinueWith(res =>
            {
                SpeakerSource = res.Result.OrderBy(p => p.first_name).ToList();
                AppSettings.Instance.speakerScreen = SpeakerSource;

                Activity.RunOnUiThread(() =>
                {
                    adapter = new SpeakerAdapter(Activity, Resource.Layout.speaker_row, SpeakerSource);
                    lstSpeaker.Adapter = adapter;
                });
            });


            btnCancel.Click += (s, e) =>
                {

                    hideSoftInputFromWindow();
                    speakerSearchContainer.Visibility = ViewStates.Gone;
                    txtSearch.Text = string.Empty;
                    searchSpeakers(string.Empty);
                };
            txtSearch.EditorAction += (sender, e) =>
            {
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Search)
                {
                    hideSoftInputFromWindow();
                    if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        searchSpeakers(txtSearch.Text);
                    }
                    else
                    {
                        Toast.MakeText(Activity, "Search cannot be blank.", ToastLength.Long);
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
                    txtSearch.Text = string.Empty;
                    searchIcon.Visibility = ViewStates.Visible;
                    btnSerchCancel.Visibility = ViewStates.Gone;
                    searchSpeakers(string.Empty);
                    hideSoftInputFromWindow();
                };
            lstSpeaker.ItemClick += (s, e) =>
                {
                    var currentSpeaker = SpeakerSource[e.Position];
                    Intent intent = new Intent(Activity, (typeof(SpeakerDetailActivity)));
                    intent.PutExtra("id", currentSpeaker.user_ref);
                    intent.PutExtra("search_speaker_text", AppConstants.SPEAKER_SEARCH_TEXT);
                    intent.PutExtra("position", e.Position);
                    intent.PutExtra("uid", currentSpeaker.user_ref);
                    //var test = JsonConvert.SerializeObject(SpeakerSource, new JsonSerializerSettings() { ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
                    //intent.PutExtra("speakerSource", test);
                    //Bundle bundle = new Bundle();
                    //if (SpeakerSource != null &&SpeakerSource.Count>0)
                    //{
                    //    var test = JsonConvert.SerializeObject(SpeakerSource, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    //    bundle.PutString("currentTwitterDataModel", test);
                    //    intent.PutExtras(bundle);
                       
                        
                    //}
                    StartActivity(intent);
                };

            activity = ((MainActivity)Activity);
            activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));
            return view;
        
            // Create your fragment here
        }

        public void hideSoftInputFromWindow()
        {
            InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(txtSearch.WindowToken, 0);
        }
        public void showSearchEditText(bool showSearch)
        {
            try
            {
                /* if (showSearch){
                     speakerSearchContainer.setVisibility(View.VISIBLE);
                     speakerSearchContainer.animate().y(0f);
                 }else{

                     speakerSearchContainer.animate().y(-0f);
                     speakerSearchContainer.setVisibility(View.GONE);
                 }*/

                if (speakerSearchContainer.Visibility==ViewStates.Visible)
                {
                    if (showSearch)
                    {
                        speakerSearchContainer.Animate().Y(-0f);
                        speakerSearchContainer.Visibility=ViewStates.Gone;


                    }
                    else
                    {
                        speakerSearchContainer.Animate().Y(-0f);
                        speakerSearchContainer.Visibility = ViewStates.Gone;

                    }

                }
                else if (speakerSearchContainer.Visibility == ViewStates.Gone)
                {
                    if (showSearch)
                    {

                        speakerSearchContainer.Visibility = ViewStates.Visible;
                        speakerSearchContainer.Animate().Y(0f);
                    }
                    else
                    {
                        speakerSearchContainer.Visibility = ViewStates.Visible;
                        speakerSearchContainer.Animate().Y(0f);

                    }
                }

            }
            catch (Exception e)
            {

            }

        }

        private void deltaCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            List<string> updatedUids = new List<string>();
            string temp = arg2.GetStringExtra("uids");
            if (temp != null)
            {
                updatedUids = temp.Split('|').ToList();
            }
            if (updatedUids != null && updatedUids.Contains(ApiCalls.speaker))
            {
                updateSpeaker();
            }
        }

        private void updateSpeaker()
        {
            getALLRefreshSpeakers(res =>
                {
                    SpeakerSource = res;
                    AppSettings.Instance.speakerScreen = SpeakerSource;
                    Activity.RunOnUiThread(() =>
                    {
                        if (adapter != null)
                        {
                            adapter.Clear();
                            adapter.AddAll(SpeakerSource);
                            lstSpeaker.Adapter = adapter;
                            adapter.NotifyDataSetChanged();
                        }
                        else
                        {
                            adapter = new SpeakerAdapter(Activity, Resource.Layout.speaker_row, SpeakerSource);
                            lstSpeaker.Adapter = adapter;
                        }
                        
                    });
                });
        }

        private void getALLRefreshSpeakers(Action<List<BuiltSpeaker>> callback)
        {
            DataManager.GetSpeakersOnAndroid(DBHelper.Instance.Connection).ContinueWith(res =>
            {
                SpeakerSource = res.Result.OrderBy(p => p.first_name).ToList();
                AppSettings.Instance.speakerScreen = SpeakerSource;
                if (callback != null)
                {
                    callback(SpeakerSource);
                }
                
            });
        }

        private void getALLSpeakers(Action<Dictionary<string, List<BuiltSpeaker>>> callback)
        {
            DataManager.GetSpeakers(DBHelper.Instance.Connection, speakerLimit, speakerOffset).ContinueWith(t =>
            {
                var res = t.Result;
                Dictionary<string, List<BuiltSpeaker>> result = res.GroupBy(p => p.first_name.First().ToString().ToUpper()).ToDictionary(p => p.Key, p => p.ToList());
                if (callback != null)
                {
                    callback(result);
                }

                if (res.Count == speakerLimit)
                {
                    speakerOffset += speakerLimit;
                    getALLSpeakers(callback);
                }
            });
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            int groupId = 0;
            int menuItemId = Menu.First;
            int menuItemOrder = Menu.None;

            var action_save = menu.Add(groupId, menuItemId, menuItemOrder, "Search");
            action_save.SetShowAsAction(ShowAsAction.IfRoom);
            action_save.SetIcon(Android.Resource.Drawable.IcMenuSearch);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (activity.deltaCompletedReceiver != null)
            {
                activity.UnregisterReceiver(activity.deltaCompletedReceiver);
            }
        }


        private void searchSpeakers(string p)
        {
            DataManager.GetSearchSpeakersOnAndroid(DBHelper.Instance.Connection, p).ContinueWith(t =>
            {
                SpeakerSource  =t.Result;
                AppSettings.Instance.speakerScreen = SpeakerSource;
                Activity.RunOnUiThread(() =>
                {
                    if (adapter != null)
                    {
                        adapter.Clear();
                        adapter.AddAll(SpeakerSource);
                        lstSpeaker.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    }
                    
                });
            });
        }
        //    DataManager.GetSearchSpeakersWithSection(DBHelper.Instance.Connection, p).ContinueWith(t =>
        //    {
        //        var data = new ListItemCollection<ListItemSpeakerValue>();
        //        foreach (var item in t.Result)
        //        {
        //            foreach (var menu in item.Value)
        //            {
        //                data.Add(new ListItemSpeakerValue(menu, item.Key));
        //            }
        //        }

        //        var sortedMenus = data.GetSortedData();
        //        var adapter = CreateAdapter(sortedMenus);
        //        Activity.RunOnUiThread(() =>
        //        {
        //            lstSpeaker.Adapter = adapter;
        //        });
        //    });
        //}

        //SeparatedListAdapter CreateAdapter<T>(Dictionary<string, List<T>> sortedObjects)
        //   where T : IHasLabel, IComparable<T>
        //{
        //    var adapter = new SeparatedListAdapter(Activity);
        //    foreach (var e in sortedObjects)
        //    {
        //        var label = e.Key;
        //        var section = e.Value;
        //        //adapter.AddSection(label, new ArrayAdapter<T>(Activity, Resource.Layout.menu_row, section));
        //        adapter.AddSection(label, new SpeakerAdapter<T>(Activity, section));
        //    }
        //    return adapter;
        //}


        private class SpeakerAdapter : ArrayAdapter<BuiltSpeaker>
        {
            Context context;
            List<BuiltSpeaker> items;
            TextView speakerName, companyLabel, sectionTitle;
            RelativeLayout section;
            int _resource;
            //List<T> data;
            public SpeakerAdapter(Android.App.Activity context, int resource, List<BuiltSpeaker> items)
                : base(context,resource,items)
            {
                this.context = context;
                this.items = items;
                this._resource=resource;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ViewHolder viewHolder;
                View view=convertView;
                if (null == convertView)
                {
                    view = LayoutInflater.From(context).Inflate(this._resource, null);
                    speakerName = view.FindViewById<TextView>(Resource.Id.speakerNameTextView);
                    companyLabel = view.FindViewById<TextView>(Resource.Id.speakerInfoTextView);
                    section = view.FindViewById<RelativeLayout>(Resource.Id.section);
                    sectionTitle = view.FindViewById<TextView>(Resource.Id.sectionTitle);
                    viewHolder = new ViewHolder(speakerName, companyLabel,section,sectionTitle);
                    view.Tag = viewHolder;
                }
                else
                {
                    viewHolder=(ViewHolder)view.Tag;
                    speakerName=viewHolder.speakerName;
                    companyLabel=viewHolder.companyLabel;
                    section = viewHolder.section;
                    sectionTitle = viewHolder.sectionTitle;
                }

                var speaker=GetItem(position);

                section.Visibility = ViewStates.Visible;
                sectionTitle.Text = speaker.first_name.First().ToString().ToUpper();
                if (position != 0)
                {
                    var previousItem = GetItem(position - 1);
                    if (!previousItem.first_name.First().ToString().ToUpper().Equals(speaker.first_name.First().ToString().ToUpper(), StringComparison.InvariantCultureIgnoreCase))
                    {

                        section.Visibility = ViewStates.Visible;
                        sectionTitle.Text = speaker.first_name.First().ToString().ToUpper();
                    }
                    else
                    {
                        section.Visibility = ViewStates.Gone;
                    }
                }

                var firstName = speaker.first_name;
                var companyName = speaker.company_name;
                var lastName = speaker.last_name;
                if (!String.IsNullOrWhiteSpace(firstName))
                {
                    speakerName.Text = firstName + " " + lastName;
                }

                if (!String.IsNullOrWhiteSpace(companyName))
                {
                    companyLabel.Text = companyName;
                }

                return view;
            //     
            }
            public class ViewHolder : Java.Lang.Object
            {
                public TextView speakerName;
                public TextView companyLabel;
                public RelativeLayout section;
                public TextView sectionTitle;
                public ViewHolder(TextView speakerName,TextView companyLabel,RelativeLayout section,TextView sectionTitle)
                {
                    this.speakerName = speakerName;
                    this.companyLabel = companyLabel;
                    this.section = section;
                    this.sectionTitle = sectionTitle;
                }
            }
            //public override View GetView(int position, View convertView, ViewGroup parent)
            //{
            //    if (null == convertView)
            //        convertView = LayoutInflater.From(context).Inflate(Resource.Layout.speaker_row, null);


            //    var item = GetItem(position) as ListItemSpeakerValue;

            //  

            //    var firstName=item.SectionItem.first_name;
            //    var companyName = item.SectionItem.company_name;
            //    var lastName = item.SectionItem.last_name;
            //    if (!String.IsNullOrWhiteSpace(firstName))
            //    {
            //        speakerName.Text = firstName + " " + lastName;
            //    }

            //    if (!String.IsNullOrWhiteSpace(companyName))
            //    {
            //        companyLabel.Text = companyName;
            //    }
            //    //icon.Text = "\uf0e7";

            //    //title.Text = "\uf001";

            //    return convertView;
            //}

            //public override T this[int position]
            //{
            //    get { return data[position]; }
            //}

            //public override int Count
            //{
            //    get { return data.Count; }
            //}

            //public override long GetItemId(int position)
            //{
            //    return position;
            //}
        }
    }
}