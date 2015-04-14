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
using ConferenceAppDroid.Adapters;
using ConferenceAppDroid.Utilities;
using CommonLayer;
using Android.Graphics.Drawables;
using Android.Graphics;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Core;

namespace ConferenceAppDroid
{
    public class MenuFragment : ListFragment
    {
        BaseActivity baseActivity;
        Dictionary<string, List<ListItemValue>> sortedMenus;
        public MenuFragment(BaseActivity baseActivity)
        {
            this.baseActivity = baseActivity;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup p1, Bundle p2)
        {
            return inflater.Inflate(Resource.Layout.menu_list, null);
        }

        public override void OnActivityCreated(Bundle p0)
        {
            base.OnActivityCreated(p0);

            var header = Activity.LayoutInflater.Inflate(Resource.Layout.header_menu, ListView, false);
            ListView.AddHeaderView(header, null, false);

            DBHelper.Instance.CopyDatabaseToLibraryFolder(Activity.ApplicationContext, "vmworld_pex_uat.db");
            DataManager.GetLeftMenu(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var data = new ListItemCollection<ListItemValue>();
                foreach (var item in t.Result)
                {
                    foreach (var menu in item.Value)
                    {
                        data.Add(new ListItemValue(menu, item.Key));
                    }
                }

                sortedMenus = data.GetSortedData();
                var adapter = CreateAdapter(sortedMenus);
                Activity.RunOnUiThread(() =>
                {
                    ListAdapter = adapter;
                });
            });
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //v.Selected = true;

            var adapter = ListAdapter as SeparatedListAdapter;
            if (adapter != null)
            {
                var tags = v.Tag.ToString().Split('|');
                var section = Convert.ToInt32(tags[0]);
                var index = Convert.ToInt32(tags[1]);
                adapter.SetSelectedIndex(section, index);
            }

            Intent intent = new Intent(MenuChangeReceiver.action);
            var t = position;
            Console.WriteLine(t);
            var item = adapter.GetItem(position - 1) as ListItemValue;
            intent.PutExtra("position", position);
            intent.PutExtra("url", item.SectionItem.link);
            Activity.SendBroadcast(intent);
            baseActivity.Title = item.SectionItem.menuname;
            baseActivity.Toggle();
        }

        SeparatedListAdapter CreateAdapter<T>(Dictionary<string, List<T>> sortedObjects)
            where T : IHasLabel, IComparable<T>
        {
            var adapter = new SeparatedListAdapter(Activity);
            foreach (var e in sortedObjects)
            {
                var label = e.Key;
                var section = e.Value;
                //adapter.AddSection(label, new ArrayAdapter<T>(Activity, Resource.Layout.menu_row, section));
                adapter.AddSection(label, new MenuAdapter<T>(Activity, section));
            }
            return adapter;
        }

        private class MenuAdapter<T> : BaseAdapter<T>
        {
            Context context;
            List<T> data;
            public MenuAdapter(Context context, List<T> data)
                : base()
            {
                this.context = context;
                this.data = data;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (null == convertView)
                    convertView = LayoutInflater.From(context).Inflate(Resource.Layout.menu_row, null);


                var item = GetItem(position) as ListItemValue;

                var icon = convertView.FindViewById<TextView>(Resource.Id.row_icon);
                Typeface custom_font = Typeface.CreateFromAsset(context.Assets, "Fonts/FontAwesome.ttf");
                icon.Typeface = custom_font;

                if (String.IsNullOrWhiteSpace(item.SectionItem.icon_code))
                    icon.Text = "\uf0e7";
                else
                {
                    var val = item.SectionItem.icon_code.Substring(3).Replace(";", "");
                    int code = int.Parse(val, System.Globalization.NumberStyles.HexNumber);
                    string unicodeString = char.ConvertFromUtf32(code).ToString();
                    icon.Text = unicodeString;
                }
                //icon.Text = "\uf0e7";

                var title = convertView.FindViewById<TextView>(Resource.Id.row_title);
                title.Text = item.ToString();
                //title.Text = "\uf001";

                return convertView;
            }

            public override T this[int position]
            {
                get { return data[position]; }
            }

            public override int Count
            {
                get { return data.Count; }
            }

            public override long GetItemId(int position)
            {
                return position;
            }
        }
    }
}