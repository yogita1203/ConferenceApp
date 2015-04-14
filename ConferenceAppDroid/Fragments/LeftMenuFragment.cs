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
using CommonLayer;
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Core;
using Android.Views.Animations;

namespace ConferenceAppDroid.Fragments
{
    public class LeftMenuFragment : Android.Support.V4.App.Fragment
    {
        BaseActivity baseActivity;
        View parentView;
        ListView leftMenuListView;
        LinearLayout addNoteLayout, openCameraLayout;
        TextView user_displayname, setting_icon, notification_icon, sync_icon;
        ImageView notification_receive_icon;
        public LeftMenuAdapter adapter;
        public List<BuiltSectionItems> source;
        private Animation animFadein;
        TextView add_note_icon, take_photo_icon;
        private TextView sync_info_textview;
        private View syncView;
        BaseActivity activity;

        public LeftMenuFragment(BaseActivity baseActivity)
        {
            this.baseActivity = baseActivity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.view_left_drawer, null);
            leftMenuListView = (ListView)parentView.FindViewById(Resource.Id.left_drawer_list);
            addNoteLayout = (LinearLayout)parentView.FindViewById(Resource.Id.add_note_container);
            openCameraLayout = (LinearLayout)parentView.FindViewById(Resource.Id.take_photo_container);
            user_displayname = (TextView)parentView.FindViewById(Resource.Id.user_displayname);

            add_note_icon = (TextView)parentView.FindViewById(Resource.Id.add_note_icon);
            var lmtUnicodeNote = Helper.getUnicodeString(Activity, add_note_icon, "&#xf044;");
            add_note_icon.Text = lmtUnicodeNote;
            add_note_icon.Click += (s, e) =>
                {
                    if (AppSettings.Instance.ApplicationUser != null)
                    {
                        Intent intent = new Intent(Activity, (typeof(NotesDetail)));
                        StartActivity(intent);
                    }
                    else
                    {
                        Helper.showAlertDialog(Activity, "Login Required", "You need to be logged-in to access this feature. Do you want to login?");
                    }

                };


            take_photo_icon = (TextView)parentView.FindViewById(Resource.Id.take_photo_icon);
            var lmtUnicodeCamera = Helper.getUnicodeString(Activity, take_photo_icon, "&#xf030;");
            take_photo_icon.Text = lmtUnicodeCamera;
            take_photo_icon.Click += (s, e) =>
                {
                    if (AppSettings.Instance.ApplicationUser != null)
                    {
                    }
                    else
                    {
                        Helper.showAlertDialog(Activity, "Login Required", "You need to be logged-in to access this feature. Do you want to login?");
                    }
                };


            setting_icon = (TextView)parentView.FindViewById(Resource.Id.setting_icon);
            var lmtUnicode = Helper.getUnicodeString(Activity, setting_icon, "&#xf013;");
            setting_icon.Text = lmtUnicode;

            setting_icon.Click += (s, e) =>
                {
                    Intent intent = new Intent(Activity, typeof(SettingsActivity));
                    Activity.StartActivity(intent);
                };


            notification_icon = (TextView)parentView.FindViewById(Resource.Id.notification_icon);
            notification_receive_icon = (ImageView)parentView.FindViewById(Resource.Id.notification_received_imageView);
            var lmtUnicodeNotification = Helper.getUnicodeString(Activity, notification_icon, "&#xf0a2;");
            notification_icon.Text = lmtUnicodeNotification;


            sync_icon = (TextView)parentView.FindViewById(Resource.Id.sync_icon);
            var lmtUnicodeSync = Helper.getUnicodeString(Activity, sync_icon, "&#xf021;");
            sync_icon.Text = lmtUnicodeSync;


            sync_icon.Click += (s, e) =>
                {
                    updateOnSync();
                };


            animFadein = AnimationUtils.LoadAnimation(Activity,
                         Resource.Animation.rotate);

            if (AppSettings.Instance.ApplicationUser != null)
            {
                user_displayname.Text = AppSettings.Instance.ApplicationUser.first_name;
            }

            user_displayname.Click += (s, e) =>
                {
                    if (AppSettings.Instance.ApplicationUser == null)
                    {
                        Intent intent = new Intent(Activity, typeof(LoginActivity));
                        Activity.StartActivity(intent);
                    }
                    else
                    {
                        showLogoutDialog();
                    }

                };


            DataManager.GetLeftMenuItems(DBHelper.Instance.Connection).ContinueWith(t =>
         {
             source = t.Result;

             Activity.RunOnUiThread(() =>
                 {
                     baseActivity.Title = source[0].menuname;
                     if (AppSettings.Instance.ApplicationUser == null)
                     {
                         source.RemoveAll(x => x.BuiltSection.sectionname == "");
                     }
                     adapter = new LeftMenuAdapter(Activity, Resource.Layout.row_left_drawer, source);
                     adapter.setSelection(0);
                     leftMenuListView.Adapter = adapter;
                 });
         });

            leftMenuListView.ItemClick += (s, e) =>
                {
                    Intent intent = new Intent(MenuChangeReceiver.action);
                    var item = adapter.GetItem(e.Position - 1);
                    if (item.menuname.Equals("survey", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (adapter.left_drawer_value_tv.Visibility == ViewStates.Visible)
                        {
                            adapter.left_drawer_value_tv.Text = "";
                            adapter.left_drawer_value_tv.Visibility = ViewStates.Gone;

                        }
                    }
                    adapter.setSelection(e.Position - 1);
                    intent.PutExtra("url", item.link);
                    Activity.SendBroadcast(intent);
                    baseActivity.Title = item.menuname;
                    baseActivity.Toggle();
                };

            setHeader();

            activity = ((BaseActivity)Activity);
            activity.deltaCompletedReceiver.OnBroadcastReceive += deltaCompletedReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.deltaCompletedReceiver, new IntentFilter(DeltaCompletedReceiver.action));
            return parentView;

        }

        void deltaCompletedReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            List<string> updatedUids = new List<string>();
            string temp = arg2.GetStringExtra("uids");
            if (temp != null)
            {
                updatedUids = temp.Split('|').ToList();
            }
            if (updatedUids != null && updatedUids.Contains(ApiCalls.left_menu))
            {
                updateLeftMenu();
            }
            if (updatedUids != null && updatedUids.Contains(ApiCalls.config))
            {
                DataManager.GetConfig(DBHelper.Instance.Connection).ContinueWith(t =>
                {
                    AppSettings.Instance.config = t.Result;
                    Intent intent = new Intent(ConfigCompletedReceiver.action);
                    Activity.SendBroadcast(intent);
                });
            }

            if (updatedUids != null && updatedUids.Contains(ApiCalls.track))
            {
                DataManager.GetTracks(DBHelper.Instance.Connection).ContinueWith((t) =>
                {
                    AppSettings.Instance.AllTracks = t.Result;

                    if (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended))
                    {
                        DataManager.GetbuiltSessionTimeListOfTrack(DBHelper.Instance.Connection).ContinueWith(fs =>
                        {
                            AppSettings.Instance.FeaturedSessions = fs.Result;
                            Activity.RunOnUiThread(() =>
                            {
                                Intent intent1 = new Intent(UpdateSessionsReceiver.action);
                                Activity.SendBroadcast(intent1);
                            });
                        });
                    }
                    else
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            Intent intent1 = new Intent(UpdateSessionsReceiver.action);
                            Activity.SendBroadcast(intent1);
                        });
                    }
                });
            }
            else if (updatedUids != null && (updatedUids.Contains(ApiCalls.session) || updatedUids.Contains(ApiCalls.recommended)))
            {
                DataManager.GetbuiltSessionTimeListOfTrack(DBHelper.Instance.Connection).ContinueWith(fs =>
                {
                    AppSettings.Instance.FeaturedSessions = fs.Result;
                    Activity.RunOnUiThread(() =>
                    {
                        Intent intent1 = new Intent(UpdateSessionsReceiver.action);
                        Activity.SendBroadcast(intent1);
                    });
                });
            }
        }

        private void updateLeftMenu()
        {
            DataManager.GetLeftMenuItems(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                source = t.Result;

                if (adapter != null)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        baseActivity.Title = source[0].menuname;
                        if (AppSettings.Instance.ApplicationUser == null)
                        {
                            source.RemoveAll(x => x.BuiltSection.sectionname == "");
                        }
                        adapter.Clear();
                        adapter.AddAll(source);
                        adapter.setSelection(0);
                        leftMenuListView.Adapter = adapter;
                        adapter.NotifyDataSetChanged();
                    });
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        baseActivity.Title = source[0].menuname;
                        if (AppSettings.Instance.ApplicationUser == null)
                        {
                            source.RemoveAll(x => x.BuiltSection.sectionname == "");
                        }
                        adapter = new LeftMenuAdapter(Activity, Resource.Layout.row_left_drawer, source);
                        adapter.setSelection(0);
                        leftMenuListView.Adapter = adapter;
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

        public void updateOnSync()
        {
            if (AppUtilities.IsNetworkAvailable(Activity))
            {
                changeSyncInfoText(Resources.GetString(Resource.String.checking_for_updates_sync_text), null);
                startRefreshAnimation();
                Intent intent = new Intent(DeltaStartedReceiver.action);
                Activity.SendBroadcast(intent);

                DataManager.deltaAllAsync(DBHelper.Instance.Connection, (res, updatedUids) =>
                {
                    try
                    {
                        if (res == null)
                        {
                            string uids = null;
                            if (updatedUids != null)
                                uids = String.Join("|", updatedUids);
                            Activity.RunOnUiThread(() =>
                            {
                                stopAnimation();
                                changeSyncInfoText(Helper.ConvertToTimeAgo(DateTime.Now));
                                Intent intent1 = new Intent(DeltaCompletedReceiver.action);
                                intent1.PutExtra("uids", uids);
                                Activity.SendBroadcast(intent1);
                            });

                        }
                        else
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                stopAnimation();
                            });
                        }
                    }
                    catch
                    { }
                }, AppSettings.Instance.ApplicationUser);
            }
            else
            {
                Toast.MakeText(Activity, Activity.GetString(Resource.String.no_internet_connection_text), ToastLength.Short).Show();
            }
        }

        public void changeSyncInfoText(String secondText)
        {
            sync_info_textview.Text = (" " + Resources.GetString(Resource.String.your_data_is_up_to_date_sync_text));
            syncView.SetBackgroundColor(Resources.GetColor(Resource.Color.sync_text_background));
            sync_info_textview.SetTextColor(Resources.GetColor(Resource.Color.left_menu_list_background));
            sync_info_textview.Invalidate();

            if (!string.IsNullOrWhiteSpace(secondText))
            {
                var CountDownTimer = new CountDown(2000, 1000);

                CountDownTimer.Tick += (s) =>
                    { };

                CountDownTimer.Finish += () =>
                    {
                        try
                        {
                            if (IsAdded)
                            {
                                sync_info_textview.Text = (secondText);
                                syncView.SetBackgroundColor(Resources.GetColor(Resource.Color.white));
                                sync_info_textview.SetTextColor(Resources.GetColor(Resource.Color.sync_info_text));
                                sync_info_textview.Invalidate();
                            }
                        }
                        catch (Exception ed)
                        {
                        }

                    };
            }
        }


        private void setHeader()
        {
            LayoutInflater layoutInflater = Activity.LayoutInflater;
            syncView = layoutInflater.Inflate(Resource.Layout.view_left_menu_sync_info, null);
            sync_info_textview = (TextView)syncView.FindViewById(Resource.Id.sync_info_textview);
            //sync_info_textview.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            syncView.SetOnClickListener(null);
            changeSyncInfoText((Helper.ConvertToTimeAgo(DateTime.Now)), null);
            leftMenuListView.AddHeaderView(syncView);

        }


        public void changeSyncInfoText(String firstText, String secondText)
        {
            try
            {
                sync_info_textview.Text = (firstText);
                sync_info_textview.Invalidate();


                if (!string.IsNullOrWhiteSpace(secondText))
                {
                    var CountDownTimer = new CountDown(2000, 1000);
                    CountDownTimer.Tick += CountDownTimer_Tick;
                    CountDownTimer.Finish += () =>
                    {
                        sync_info_textview.Text = (secondText);
                        sync_info_textview.Invalidate();
                    };
                    CountDownTimer.Start();
                }
            }
            catch (Exception e)
            {
            }
        }

        void CountDownTimer_Finish()
        {

        }

        void CountDownTimer_Tick(long millisUntilFinished)
        {

        }

        public void stopAnimation()
        {
            // animFadein.reset();
            sync_icon.ClearAnimation();
        }

        public void startRefreshAnimation()
        {
            try
            {
                sync_icon.StartAnimation(animFadein);
            }
            catch (Exception e)
            {
            }
        }


        private void showLogoutDialog()
        {

            new Android.App.AlertDialog.Builder(Activity).SetTitle("Delete selected item").SetMessage("Are you sure you want to permanently delete this item?")
                                  .SetPositiveButton("Yes", (sender, args) =>
                                  {
                                      logout();
                                  })
                                  .SetNegativeButton("No", (sender, args) => { }).Show();
        }

        private void logout()
        {
            DataManager.UpdateDataOnLogout(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                AppSettings.Instance.ApplicationUser = null;
                DataManager.SetCurrentUser(null);
                //AppSettings.MySessionIds = null;
                //AppSettings.NewSurveyCount = 0;

                Activity.RunOnUiThread(() =>
                {
                    loginReceiver_OnBroadcastReceive();
                    var u = Activity.SupportFragmentManager.BeginTransaction();
                    var contentFrag = Activity.SupportFragmentManager.FindFragmentById(Resource.Id.content_frame);
                    u.Replace(Resource.Id.content_frame, new whatsHappenning());
                    u.Commit();
                    this.baseActivity.Toggle();
                });
            });
        }

        //public override void OnResume()
        //{
        //    loginReceiver = new LoginReceivers();
        //    loginReceiver.OnBroadcastReceive += loginReceiver_OnBroadcastReceive;
        //    Activity.RegisterReceiver(loginReceiver, new IntentFilter(LoginReceivers.action));
        //}

        public void loginReceiver_OnBroadcastReceive()
        {
            DataManager.GetLeftMenuItems(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                source = t.Result;

                Activity.RunOnUiThread(() =>
                {
                    if (AppSettings.Instance.ApplicationUser == null)
                    {
                        source.RemoveAll(x => x.BuiltSection.sectionname == "");
                        user_displayname.Text = "Login";
                    }
                    else
                    {
                        user_displayname.Text = AppSettings.Instance.ApplicationUser.first_name;
                    }
                    adapter = new LeftMenuAdapter(Activity, Resource.Layout.row_left_drawer, source);
                    leftMenuListView.Adapter = adapter;
                });
            });
        }
    }



    public class LeftMenuAdapter : ArrayAdapter<BuiltSectionItems>
    {
        Android.App.Activity context;
        List<BuiltSectionItems> items;
        int _resource;
        public TextView listSeperatorTextView;
        public TextView left_menu_icon;
        public TextView left_drawer_row_tv;
        public TextView left_drawer_value_tv;
        public TextView left_drawer_info_tv;
        public static int NOT_SELECTED = -1;
        private int selectedPos = -1;

        public LeftMenuAdapter(Android.App.Activity context, int resource, List<BuiltSectionItems> items)
            : base(context, resource, items)
        {
            this.context = context;
            this.items = items;
            this._resource = resource;
        }

        public void setSelection(int i)
        {
            if (selectedPos == i)
            {
                //selectedPos = NOT_SELECTED;
                selectedPos = i;
            }
            else
            {
                selectedPos = i;
            }
            this.NotifyDataSetChanged();
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView;
            if (null == convertView)
            {
                view = LayoutInflater.From(context).Inflate(_resource, null);
                listSeperatorTextView = view.FindViewById<TextView>(Resource.Id.listSeperatorTextView);
                left_menu_icon = view.FindViewById<TextView>(Resource.Id.left_menu_icon);
                left_drawer_row_tv = view.FindViewById<TextView>(Resource.Id.left_drawer_row_tv);
                left_drawer_value_tv = view.FindViewById<TextView>(Resource.Id.left_drawer_value_tv);
                left_drawer_info_tv = view.FindViewById<TextView>(Resource.Id.left_drawer_info_tv);
                viewHolder = new ViewHolder(listSeperatorTextView, left_menu_icon, left_drawer_row_tv, left_drawer_value_tv, left_drawer_info_tv);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                listSeperatorTextView = viewHolder.listSeperatorTextView;
                left_menu_icon = viewHolder.left_menu_icon;
                left_drawer_row_tv = viewHolder.left_drawer_row_tv;
                left_drawer_value_tv = viewHolder.left_drawer_value_tv;
                left_drawer_info_tv = viewHolder.left_drawer_info_tv;
            }

            var item = GetItem(position);
            if (!string.IsNullOrWhiteSpace(item.BuiltSection.sectionname))
            {
                listSeperatorTextView.Visibility = ViewStates.Visible;
                listSeperatorTextView.Text = item.BuiltSection.sectionname;
            }

            if (position == 9)
            {
                //your color for selected item
                view.SetBackgroundColor(Android.Graphics.Color.ParseColor("#14417D"));
                left_drawer_row_tv.SetTextColor(Android.Graphics.Color.White);
                left_menu_icon.SetTextColor(Android.Graphics.Color.White);
            }
            else
            {
                view.SetBackgroundColor(Android.Graphics.Color.ParseColor("#f2f2f2"));
                left_drawer_row_tv.SetTextColor(Android.Graphics.Color.ParseColor("#656565"));
                left_menu_icon.SetTextColor(Android.Graphics.Color.ParseColor("#656565"));
            }

            if (position != 0)
            {
                var previousItem = GetItem(position - 1);
                if (!previousItem.BuiltSection.sectionname.Equals(item.BuiltSection.sectionname, StringComparison.InvariantCultureIgnoreCase))
                {

                    listSeperatorTextView.Visibility = ViewStates.Visible;
                    listSeperatorTextView.Text = (item.BuiltSection.sectionname);
                }
                else
                {
                    listSeperatorTextView.Visibility = ViewStates.Gone;
                }
            }

            if (!String.IsNullOrWhiteSpace(item.icon_code))
            {
                var lmtUnicode = Helper.getUnicodeString(context, left_menu_icon, item.icon_code);
                left_menu_icon.Text = lmtUnicode;
            }

            if (!string.IsNullOrWhiteSpace(item.menuname) && item.menuname.Equals(context.Resources.GetString(Resource.String.my_survey_single), StringComparison.InvariantCultureIgnoreCase))
            {
                left_drawer_value_tv.Visibility = ViewStates.Visible;
                left_drawer_value_tv.Text = AppSettings.Instance.NewSurveyCount.ToString();
            }
            else
            {
                left_drawer_value_tv.Text = string.Empty;
                left_drawer_value_tv.Visibility = ViewStates.Gone;

            }
            if (!string.IsNullOrWhiteSpace(item.menuname))
            {
                left_drawer_row_tv.Text = item.menuname;
            }

            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView listSeperatorTextView;
            public TextView left_menu_icon;
            public TextView left_drawer_row_tv;
            public TextView left_drawer_value_tv;
            public TextView left_drawer_info_tv;

            public ViewHolder(TextView listSeperatorTextView, TextView left_menu_icon, TextView left_drawer_row_tv, TextView left_drawer_value_tv, TextView left_drawer_info_tv)
            {
                this.listSeperatorTextView = listSeperatorTextView;
                this.left_menu_icon = left_menu_icon;
                this.left_drawer_row_tv = left_drawer_row_tv;
                this.left_drawer_value_tv = left_drawer_value_tv;
                this.left_drawer_info_tv = left_drawer_info_tv;
            }
        }

    }
}