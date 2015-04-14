
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Adapters;
using ConferenceAppDroid.Utilities;
using System;
using System.Linq;

namespace ConferenceAppDroid.Fragments
{
    public class Agenda : Android.Support.V4.App.Fragment
    {
        ListView agenda_list;
        Button btnPrevious, btnNext;
        TextView tvAgendaDate;
        BuiltAgendaItem[] allItems;
        string[] dateArray;
        short currentIndex = 0;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.list, null);
            agenda_list = view.FindViewById<ListView>(Resource.Id.list);
            agenda_list.Selector = new ColorDrawable(Android.Graphics.Color.Transparent);

            var header = inflater.Inflate(Resource.Layout.header_agenda, agenda_list, false);
            agenda_list.AddHeaderView(header);

            btnPrevious = header.FindViewById<Button>(Resource.Id.btnPrevious);
            btnPrevious.Click += btnPrevious_Click;
            btnNext = header.FindViewById<Button>(Resource.Id.btnNext);
            btnNext.Click += btnNext_Click;
            tvAgendaDate = header.FindViewById<TextView>(Resource.Id.tvDate);

            DataManager.GetAllAgendaItem(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                allItems = t.Result;
                dateArray = allItems.Select(p => p.BuiltAgenda.agenda_date).Distinct().OrderBy(p => Convert.ToDateTime(p)).ToArray();
                if (dateArray.Length > 0)
                {
                    setAdapter();
                }
            });

            return view;
        }

        private void setAdapter()
        {
            var items = allItems.Where(p => p.BuiltAgenda.agenda_date == dateArray[currentIndex]).ToArray();
            Activity.RunOnUiThread(() =>
            {
                var adapter = new AgendaAdapter(Activity, Resource.Layout.agenda_row, items);
                agenda_list.Adapter = adapter;
                setButtonState();
            });
        }

        private void setButtonState()
        {
            if (dateArray.Length > 0)
            {
                if (currentIndex == dateArray.Length - 1)
                    btnNext.Enabled = false;
                else
                    btnNext.Enabled = true;

                if (currentIndex == 0)
                    btnPrevious.Enabled = false;
                else
                    btnPrevious.Enabled = true;
            }
            else
            {
                btnNext.Enabled = false;
                btnPrevious.Enabled = false;
            }

            tvAgendaDate.Text = Convert.ToDateTime(dateArray[currentIndex]).ToString("ddd, MMM d");
        }

        void btnNext_Click(object sender, System.EventArgs e)
        {
            if (currentIndex < dateArray.Length - 1)
                currentIndex++;
            setAdapter();
        }

        void btnPrevious_Click(object sender, System.EventArgs e)
        {
            if (currentIndex > 0)
                currentIndex--;
            setAdapter();
        }
    }
    public class AgendaAdapter : ArrayAdapter<BuiltAgendaItem>
    {
        int _resource;
        public AgendaAdapter(Context c, int resourceId, BuiltAgendaItem[] objects)
            : base(c, resourceId, objects)
        {
            this._resource = resourceId;
        }

        /*private view holder class*/
        private class ViewHolder : Java.Lang.Object
        {
            public TextView txtTitle;
            public TextView txtTimings;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder = null;

            var rowItem = GetItem(position);

            LayoutInflater mInflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            if (convertView == null)
            {
                convertView = mInflater.Inflate(_resource, null);
                holder = new ViewHolder();
                holder.txtTitle = convertView.FindViewById<TextView>(Resource.Id.title);
                holder.txtTimings = convertView.FindViewById<TextView>(Resource.Id.description);
                convertView.Tag = holder;
            }
            else
                holder = (ViewHolder)convertView.Tag;

            holder.txtTitle.Text = rowItem.name;
            holder.txtTimings.Text = String.Concat(rowItem.start_time, " - ", rowItem.end_time);
            return convertView;
        }
    }
}