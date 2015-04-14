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

namespace ConferenceAppDroid.Fragments
{
    public class MyNotes : Android.Support.V4.App.Fragment
    {
        View parentView;
        RadioGroup radioGroup;
        Button btnPrevious, btnNext;
        TextView tvTags;
        ListView myNotesListView;
        List<BuiltNotes> lstNotes;
        MyNotesAdapter adapter;
        short currentIndex = 0;
        List<string> tags;
        View header;
        MainActivity activity;
        int currentCheckedId;
        Button newNotesButton;
        List<BuiltNotes> mainSource;
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.fragment_my_notes, null);

            myNotesListView = parentView.FindViewById<ListView>(Resource.Id.myNotesListView);
            newNotesButton = parentView.FindViewById<Button>(Resource.Id.newNotesButton);

            header = inflater.Inflate(Resource.Layout.header_agenda, myNotesListView, false); btnPrevious = header.FindViewById<Button>(Resource.Id.btnPrevious);
            btnPrevious.Click += btnPrevious_Click;
            btnNext = header.FindViewById<Button>(Resource.Id.btnNext);
            btnNext.Click += btnNext_Click;
            tvTags = header.FindViewById<TextView>(Resource.Id.tvDate);
            //myNotesListView.AddHeaderView(header);

            radioGroup = parentView.FindViewById<RadioGroup>(Resource.Id.notesGroup);
            radioGroup.CheckedChange += radioGroup_CheckedChange;
            DataManager.GetNotes(DBHelper.Instance.Connection).ContinueWith(t =>
           {
               mainSource = t.Result;

               if (mainSource != null)
               {
                   lstNotes = mainSource.OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();
                   Activity.RunOnUiThread(() =>
                       {
                           adapter = new MyNotesAdapter(Activity,Resource.Layout.list_row_notes,lstNotes);
                           myNotesListView.Adapter = adapter;
                       });

               }
           });

            myNotesListView.ItemClick += (s, e) =>
                {
                    Intent intent = new Intent(Activity, (typeof(NotesDetail)));
                    intent.PutExtra("uid", lstNotes[e.Position].uid);
                    StartActivity(intent);
                };

            newNotesButton.Click += (s, e) =>
                {
                    Intent intent = new Intent(Activity, (typeof(NotesDetail)));
                    StartActivity(intent);
                };

            activity = ((MainActivity)Activity);
            activity.notesReceiver.OnBroadcastReceive += notesReceiver_OnBroadcastReceive;
            activity.RegisterReceiver(activity.notesReceiver, new IntentFilter(NotesReceiver.action));
            return parentView;
        }

        void notesReceiver_OnBroadcastReceive(Context arg1, Intent arg2)
        {
            FilterSource(currentCheckedId);
        }

        private void radioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            var current = View.FindViewById<RadioButton>(e.CheckedId);
            currentCheckedId = radioGroup.IndexOfChild(current);
            FilterSource(currentCheckedId);
        }

        public void FilterSource( int previousSegmentIndex)
        {
            DataManager.GetNotes(DBHelper.Instance.Connection).ContinueWith(t =>
                      {
                          mainSource = t.Result;

                          if (mainSource != null)
                          {
                              if (previousSegmentIndex == 0)
                                  lstNotes = mainSource.OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();
                              else if (previousSegmentIndex == 1)
                                  lstNotes = mainSource.OrderBy(p => p.title).ToList();

                              else
                              {
                                  tags = mainSource.SelectMany(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
                                  if (tags != null && tags.Count > 0)
                                  {
                                      lstNotes = mainSource.Where(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(tags[currentIndex])).ToList();
                                      Activity.RunOnUiThread(() =>
                                      {
                                          if (myNotesListView.Adapter != null)
                                          {
                                              myNotesListView.Adapter = null;
                                              myNotesListView.AddHeaderView(header);
                                              adapter.Clear();
                                              adapter.AddAll(lstNotes);
                                              myNotesListView.Adapter = adapter;
                                              adapter.NotifyDataSetChanged();
                                              setButtonState();
                                          }
                                      });
                                  }
                                  return;
                              }
                              Activity.RunOnUiThread(() =>
                                  {
                                      if (myNotesListView.HeaderViewsCount > 0)
                                      {
                                          myNotesListView.RemoveHeaderView(header);
                                      }
                                      adapter.Clear();
                                      adapter.AddAll(lstNotes);
                                      myNotesListView.Adapter = adapter;
                                      adapter.NotifyDataSetChanged();
                                      
                                  });
                          }
                          
                      });
        }

        private void setButtonState()
        {
            if (tags.Count > 0)
            {
                if (currentIndex == tags.Count - 1)
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

            tvTags.Text = (tags[currentIndex]);
        }

        private void setAdapter()
        {
            lstNotes = mainSource.Where(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(tags[currentIndex])).ToList();
            Activity.RunOnUiThread(() =>
            {
                var adapter = new MyNotesAdapter(Activity, Resource.Layout.list_row_notes, lstNotes);
                myNotesListView.Adapter = adapter;
                setButtonState();
            });
        }

        void btnNext_Click(object sender, System.EventArgs e)
        {
            if (currentIndex < tags.Count - 1)
                currentIndex++;
            setAdapter();
        }

        void btnPrevious_Click(object sender, System.EventArgs e)
        {
            if (currentIndex > 0)
                currentIndex--;
            setAdapter();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (activity.notesReceiver != null)
            {
                activity.UnregisterReceiver(activity.notesReceiver);
            }
        }
    }

    public class MyNotesAdapter : ArrayAdapter<BuiltNotes>
    {
        Android.App.Activity activity;
        List<BuiltNotes> notes;
        int _resource;
        public TextView dateTextView;
        public TextView notesTitleTextView;
        public TextView notesContentTextView;
        public MyNotesAdapter(Android.App.Activity context, int resource, List<BuiltNotes> notes)
            : base(context, resource, notes)
        {
            activity = context ;
            this._resource = resource;
            this.notes = notes;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView;
            if (convertView == null)
            {
                view = activity.LayoutInflater.Inflate(_resource, null);
                dateTextView = view.FindViewById<TextView>(Resource.Id.dateTextView);
                notesTitleTextView = view.FindViewById<TextView>(Resource.Id.notesTitleTextView);
                notesContentTextView = view.FindViewById<TextView>(Resource.Id.notesContentTextView);
                viewHolder = new ViewHolder(dateTextView, notesTitleTextView, notesContentTextView);
                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                dateTextView = viewHolder.dateTextView;
                notesTitleTextView = viewHolder.notesTitleTextView;
                notesContentTextView = viewHolder.notesContentTextView;
            }

            var notes = GetItem(position);
            if(!string.IsNullOrWhiteSpace(notes.title))
            {
                notesTitleTextView.Text = notes.title;
            }

            if (!string.IsNullOrWhiteSpace(notes.content))
            {
                notesContentTextView.Text = notes.content;
            }
            if (!string.IsNullOrWhiteSpace(notes.updated_at))
            {
                dateTextView.Text = Convert.ToDateTime(notes.updated_at).ToString("MMM d, h:mm tt");
            }
            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView dateTextView;
            public TextView notesTitleTextView;
            public TextView notesContentTextView;

            public ViewHolder(TextView dateTextView, TextView notesTitleTextView, TextView notesContentTextView)
            {
                this.dateTextView = dateTextView;
                this.notesTitleTextView = notesTitleTextView;
                this.notesContentTextView = notesContentTextView;
            }
        }



    }
}