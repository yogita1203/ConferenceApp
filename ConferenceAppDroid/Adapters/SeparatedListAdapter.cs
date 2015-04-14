//or use this - http://javatechig.com/android/listview-with-section-header-in-android or http://codetheory.in/android-dividing-listview-sections-group-headers/

using System;
using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using ConferenceAppDroid.Utilities;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace ConferenceAppDroid.Adapters
{
    class SeparatedListAdapter : BaseAdapter
    {
        private int selectedIndex;
        private int selectedSection;

        public SeparatedListAdapter(Context context)
        {
            headers = new ArrayAdapter<string>(context, Resource.Layout.list_header);
            selectedIndex = -1;
            selectedSection = -1;
        }

        public void SetSelectedIndex(int section, int index)
        {
            selectedIndex = index;
            selectedSection = section;
            NotifyDataSetChanged();
        }

        Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter>();
        ArrayAdapter<string> headers;
        const int TypeSectionHeader = 0;

        public void AddSection(string section, IAdapter adapter)
        {
            headers.Add(section);
            sections.Add(section, adapter);
        }

        public override Java.Lang.Object GetItem(int position)
        {
            int op = position;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;
                if (position == 0)
                    return section;
                if (position < size)
                    return adapter.GetItem(position - 1);
                position -= size;
            }
            return null;
        }

        public override int Count
        {
            get
            {
                return sections.Values.Sum(adapter => adapter.Count + 1);
            }
        }

        public override int ViewTypeCount
        {
            get
            {
                return 1 + sections.Values.Sum(adapter => adapter.ViewTypeCount);
            }
        }

        public override int GetItemViewType(int position)
        {
            int type = 1;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                // check if position inside this section
                if (position == 0)
                    return TypeSectionHeader;
                if (position < size)
                    return type + adapter.GetItemViewType(position - 1);

                // otherwise jump into next section
                position -= size;
                type += adapter.ViewTypeCount;
            }
            return -1;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return (GetItemViewType(position) != TypeSectionHeader);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int sectionnum = 0;
            foreach (var section in sections.Keys)
            {
                var adapter = sections[section];
                int size = adapter.Count + 1;

                // check if position inside this section
                if (position == 0)
                {
                    if (String.IsNullOrWhiteSpace(section))
                        return new View(DBHelper.Instance.context);
                    else
                        return headers.GetView(sectionnum, convertView, parent);
                }
                if (position < size)
                {
                    var v = adapter.GetView(position - 1, convertView, parent);
                    v.Tag = sectionnum + "|" + position;
                    if (selectedIndex != -1 && position == selectedIndex && sectionnum == selectedSection)
                    {
                        v.SetBackgroundResource(Resource.Color.menu_selected_color);
                        //v.Selected = true;
                    }
                    else
                    {
                        v.SetBackgroundResource(Resource.Color.menu_default_color);
                        //v.Selected = false;
                    }
                    return v;
                    //return adapter.GetView(position - 1, convertView, parent);
                }

                // otherwise jump into next section
                position -= size;
                sectionnum++;
            }
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}
