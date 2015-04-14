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
using Android.Content.PM;

namespace ConferenceAppDroid
{
    [Activity(Label = "SettingsScreen")]
    public class SettingsActivity : Android.App.Activity
    {
        private SettingsMenuAdapter settingsMenuAdapter;
        private ListView settingsMenuListView;
        private View actionBarView;
        private TextView titleTextView;
        private Context context;
        private String VERSION_NAME;
        private int VERSION_CODE;
        private ImageView bottomImageView;
        List<Submenus> subMenus;
        ImageButton rightMenuBtn;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_settings);
               this.context = this;

        OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);
        actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);

        ActionBar.CustomView = actionBarView;
        ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;


        settingsMenuListView = (ListView) FindViewById(Resource.Id.setting_menu_list);
        titleTextView = (TextView) actionBarView.FindViewById(Resource.Id.titleTextView);
        bottomImageView = (ImageView) actionBarView.FindViewById(Resource.Id.bottomImageView);
        titleTextView.Text="Settings";

        rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
        rightMenuBtn.Visibility = ViewStates.Invisible;

        bottomImageView.Visibility=ViewStates.Visible;
        bottomImageView.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.close_icon_selector));
        bottomImageView.Click += bottomImageView_Click;
        DataManager.GetSettingsMenu(DBHelper.Instance.Connection).ContinueWith(t =>
    {
         var menus = t.Result;
         subMenus = menus.SelectMany(p => p.menu).OrderBy(p => p.order).SelectMany(q => q.sub_menu).ToList();
         settingsMenuAdapter = new SettingsMenuAdapter(this, Resource.Layout.row_settings_list, subMenus);
         settingsMenuListView.Adapter = settingsMenuAdapter;
    });

        settingsMenuListView.ItemClick += settingsMenuListView_ItemClick;

        setFooter();
            // Create your application here
        }

        void settingsMenuListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var currentMenu=subMenus[e.Position];
                Intent intent = new Intent(this, typeof(UIWebView));
                if ((currentMenu != null) && (!string.IsNullOrWhiteSpace(currentMenu.link)))
                {
                    if (currentMenu.link.Contains("http"))
                    {
                        intent.PutExtra("url", currentMenu.link);
                        intent.PutExtra("title", currentMenu.name);
                        StartActivity(intent);
                    } 
                    else 
                    {
                        if (currentMenu.link.Contains("logout"))
                        {
                            showLogoutDialog();
                        }
                    }
                }
        }

        private void showLogoutDialog()
        {
            throw new NotImplementedException();
        }

        private void setFooter()
        {
            PackageInfo pi = null;
            try
            {
                pi = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            }
            catch (PackageManager.NameNotFoundException e1)
            {
                e1.PrintStackTrace();
            }
            if (pi != null)
            {
                this.VERSION_NAME = pi.VersionName;
                this.VERSION_CODE = pi.VersionCode;
            }
            TextView buildInfoTextView = new TextView(context);
            //buildInfoTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            buildInfoTextView.Text=" Version " + VERSION_NAME + "  (" + VERSION_CODE + ")";
            buildInfoTextView.Gravity=GravityFlags.CenterHorizontal;
            buildInfoTextView.SetPadding(Helper.dpToPx(context, 5), Helper.dpToPx(context, 8), Helper.dpToPx(context, 5), Helper.dpToPx(context, 8));

            settingsMenuListView.AddFooterView(buildInfoTextView);
        }

        void bottomImageView_Click(object sender, EventArgs e)
        {
            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();
            new AppUtilities().isFromPauseResume(this, false);
        }

        protected override void OnPause()
        {
            base.OnPause();
            new AppUtilities().isFromPauseResume(this, true);
        }
    }

    public class SettingsMenuAdapter : ArrayAdapter<Submenus>
    {
        Context context;
        int _resource;
        List<Submenus> items;
        public RelativeLayout settings_row_parent;
        public TextView listSeperatorTextView;
        public TextView settings_list_menu_textview;

        public ToggleButton togglePushNotification;

        public SettingsMenuAdapter(Android.App.Activity context, int resource, List<Submenus> items):base
            (context,resource,items)
        {
            this.context = context;
            this._resource = resource;
            this.items = items;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView;
            if (null == convertView)
            {
                view = LayoutInflater.From(context).Inflate(this._resource, null);
                settings_row_parent = view.FindViewById<RelativeLayout>(Resource.Id.settings_row_parent);
                listSeperatorTextView = view.FindViewById<TextView>(Resource.Id.listSeperatorTextView);
                settings_list_menu_textview = view.FindViewById<TextView>(Resource.Id.settings_list_menu_textview);
                togglePushNotification = view.FindViewById<ToggleButton>(Resource.Id.togglePushNotification);
                viewHolder = new ViewHolder(settings_row_parent, listSeperatorTextView, settings_list_menu_textview, togglePushNotification);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder=(ViewHolder)view.Tag;
                settings_row_parent = viewHolder.settings_row_parent;
                listSeperatorTextView = viewHolder.listSeperatorTextView;
                settings_list_menu_textview = viewHolder.settings_list_menu_textview;
                togglePushNotification = viewHolder.togglePushNotification;
            }

            var menu=GetItem(position);
            listSeperatorTextView.Visibility = ViewStates.Visible;
            listSeperatorTextView.Text = menu.Menus.section_name.ToString().ToUpper();
            if (position != 0)
            {
                var previousItem = GetItem(position - 1);
                if (!previousItem.Menus.section_name.Equals(menu.Menus.section_name, StringComparison.InvariantCultureIgnoreCase))
                {

                    listSeperatorTextView.Visibility = ViewStates.Visible;
                    listSeperatorTextView.Text = menu.Menus.section_name;
                }
                else
                {
                    listSeperatorTextView.Visibility = ViewStates.Gone;
                }
            }

            if (!string.IsNullOrWhiteSpace(menu.name))
            {
                settings_list_menu_textview.Text = menu.name;
            }

            if (!string.IsNullOrWhiteSpace(menu.link)&&  menu.link != "#{switch}" && !menu.link.Contains("{"))
            {
                togglePushNotification.Visibility = ViewStates.Gone;
            }
            else
            {
                if (menu.name == "Push Notification")
                {
                    togglePushNotification.Visibility = ViewStates.Visible;
                }
            }

            
            
            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public RelativeLayout settings_row_parent;
            public TextView listSeperatorTextView;
            public TextView settings_list_menu_textview;

            public ToggleButton togglePushNotification;
            public ViewHolder(RelativeLayout settings_row_parent, TextView listSeperatorTextView, TextView settings_list_menu_textview, ToggleButton togglePushNotification)
            {
                this.settings_row_parent = settings_row_parent;
                this.listSeperatorTextView = listSeperatorTextView;
                this.settings_list_menu_textview = settings_list_menu_textview;
                this.togglePushNotification = togglePushNotification;
            }
        }
    }
}