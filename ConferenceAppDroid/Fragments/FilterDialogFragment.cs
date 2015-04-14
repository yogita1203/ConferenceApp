using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConferenceAppDroid.Fragments
{
    public class FilterDialogFragment : Android.Support.V4.App.DialogFragment
    {
        private Dialog dialog;
        ListView dropDownListview;
        Button cancelDropDown, removeFilter;
        RelativeLayout selectFilterContainer;
        TextView filterTracksTitleTv;
        public List<string> sessionTracks;
        public static Action<string> action;
        public static Action<string> subtrackHandler;
        public string selectedTrackName { get; set; }
        public string selectedSubtrackName { get; set; }
        public Tracks tracks;
        BuiltTracks[] subtrackArray;
        public FilterDialogFragment(string selectedTrackName, Tracks tracks)
        {
            this.selectedTrackName = selectedTrackName;
            this.tracks = tracks;
        }
        public override Android.App.Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            dialog = new Dialog(Activity);

            dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            dialog.SetCanceledOnTouchOutside(false);

            dialog.SetTitle("Tracks");
            dialog.SetContentView(Resource.Layout.view_tracks_filter_dialog);
            setDialogPosition(dialog);

            dropDownListview = (ListView)dialog.FindViewById(Resource.Id.drop_down_listview);
            cancelDropDown = (Button)dialog.FindViewById(Resource.Id.cancel_drop_down);
            removeFilter = (Button)dialog.FindViewById(Resource.Id.remove_filter);
            #region--ParentTracks--
            if (this.tracks == Tracks.parentTracks)
            {
                if (this.selectedTrackName.Equals("All Tracks", StringComparison.InvariantCultureIgnoreCase))
                {

                    removeFilter.Visibility = ViewStates.Gone;
                }
                else
                {
                    removeFilter.Visibility = ViewStates.Visible;
                }

                selectFilterContainer = (RelativeLayout)dialog.FindViewById(Resource.Id.select_filter_container);
                filterTracksTitleTv = (TextView)dialog.FindViewById(Resource.Id.filter_tracks_title_tv);
                filterTracksTitleTv.Text = "Tracks";
                filterTracksTitleTv.SetTextColor(Android.Graphics.Color.Black);
                sessionTracks = AppSettings.Instance.TrackDictionary.Keys.ToList();
                Activity.RunOnUiThread(() =>
                {
                    TrackArrayAdapter trackAdapter = new TrackArrayAdapter(Activity, 0, AppSettings.Instance.TrackDictionary.Keys.ToArray());
                    dropDownListview.Adapter = trackAdapter;
                });

                cancelDropDown.Click += (s, e) =>
                {
                    dialog.Dismiss();
                };

                dropDownListview.ItemClick += (s, e) =>
                {
                    var currentItem = sessionTracks[e.Position];
                    if (action != null)
                    {
                        this.selectedTrackName = currentItem;
                        dialog.Dismiss();
                        action(currentItem);


                    }
                };
                removeFilter.Click += (s, e) =>
                {
                    if (action != null)
                    {
                        dialog.Dismiss();
                        action(string.Empty);


                    }
                };
            }

            #endregion

            else
            {
                if (this.selectedSubtrackName.Equals("Select Sub-track", StringComparison.InvariantCultureIgnoreCase))
                {

                    removeFilter.Visibility = ViewStates.Gone;
                }
                else
                {
                    removeFilter.Visibility = ViewStates.Visible;
                }

                selectFilterContainer = (RelativeLayout)dialog.FindViewById(Resource.Id.select_filter_container);
                filterTracksTitleTv = (TextView)dialog.FindViewById(Resource.Id.filter_tracks_title_tv);
                filterTracksTitleTv.Text = "Sub-tracks";
                filterTracksTitleTv.SetTextColor(Android.Graphics.Color.Black);


                subtrackArray = AppSettings.Instance.TrackDictionary[this.selectedTrackName];
                var subTrackList = subtrackArray.SelectMany(p => new List<string> { p.name }).ToList();
                subTrackAdapter trackAdapter = new subTrackAdapter(Activity, 0, subTrackList.ToArray());
                dropDownListview.Adapter = trackAdapter;


                cancelDropDown.Click += (s, e) =>
                {
                    dialog.Dismiss();
                };

                dropDownListview.ItemClick += (s, e) =>
                {
                    var currentItem = subTrackList[e.Position];
                    if (subtrackHandler != null)
                    {
                        dialog.Dismiss();
                        subtrackHandler(currentItem);


                    }
                };
                removeFilter.Click += (s, e) =>
                {
                    if (subtrackHandler != null)
                    {
                        dialog.Dismiss();
                        subtrackHandler(string.Empty);


                    }
                };
            }


            dialog.Window.Attributes.WindowAnimations = Resource.Style.DialogAnimation;
            return dialog;
        }

        private void setDialogPosition(Dialog dialog)
        {
            Window window = dialog.Window;
            WindowManagerLayoutParams wlp = window.Attributes;

            wlp.Gravity = GravityFlags.Bottom;
            //		wlp.flags &= ~WindowManager.LayoutParams.FLAG_DIM_BEHIND;

            wlp.Width = RelativeLayout.LayoutParams.MatchParent;
            window.Attributes = wlp;

        }

        public enum Tracks
        {
            parentTracks,
            subTracks
        }
    }

    public class TrackArrayAdapter : ArrayAdapter<string>
    {
        string[] items;
        Context context;
        private LayoutInflater mInflater;
        TextView parentTrackName;

        public TrackArrayAdapter(Context context, int resource, string[] items)
            : base(context, resource, items)
        {
            this.context = context;
            this.items = items;
            mInflater = LayoutInflater.From(context);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView; // re-use an existing view, if one is available
            if (convertView == null)
            {
                view = mInflater.Inflate(Resource.Layout.row_tracks_session, null);
                parentTrackName = (TextView)view.FindViewById(Resource.Id.row_tracks_name_tv);
                viewHolder = new ViewHolder(parentTrackName);

                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                parentTrackName = viewHolder.parentTrackName;
            }


            parentTrackName.Text = GetItem(position);
            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView parentTrackName;
            public ViewHolder(TextView parentTrackName)
            {
                this.parentTrackName = parentTrackName;
            }
        }

    }

    public class subTrackAdapter : ArrayAdapter<string>
    {
        string[] items;
        Context context;
        private LayoutInflater mInflater;
        TextView subTrackName;

        public subTrackAdapter(Context context, int resource, string[] items)
            : base(context, resource, items)
        {
            this.context = context;
            this.items = items;
            mInflater = LayoutInflater.From(context);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView; // re-use an existing view, if one is available
            if (convertView == null)
            {
                view = mInflater.Inflate(Resource.Layout.row_tracks_session, null);
                subTrackName = (TextView)view.FindViewById(Resource.Id.row_tracks_name_tv);
                viewHolder = new ViewHolder(subTrackName);

                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                subTrackName = viewHolder.subTrackName;
            }


            subTrackName.Text = GetItem(position);
            return view;
        }
        public class ViewHolder : Java.Lang.Object
        {
            public TextView subTrackName;
            public ViewHolder(TextView subTrackName)
            {
                this.subTrackName = subTrackName;
            }
        }
    }
}
