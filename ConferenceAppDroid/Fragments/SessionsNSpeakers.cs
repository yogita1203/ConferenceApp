using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.Fragments
{
    public class SessionsNSpeakers: Android.Support.V4.App.Fragment
    {
        Session session;
        Speaker speaker;
        ImageView schedule_tab_icon;
        ImageView speakers_tab_icon;
        View actionbar;
        bool isFromExplore;
        public SessionsNSpeakers(bool isFromExplore)
        {
            this.isFromExplore = isFromExplore;
        }
        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            actionbar = Activity.ActionBar.CustomView;
            if (this.isFromExplore)
            {
                var titleTextView = actionbar.FindViewById<TextView>(Resource.Id.titleTextView);
                titleTextView.Text = "SESSIONS";
            }
            var view = inflater.Inflate(Resource.Layout.view_sessions, null);
            schedule_tab_icon=view.FindViewById<ImageView>(Resource.Id.schedule_tab_icon);
            speakers_tab_icon = view.FindViewById<ImageView>(Resource.Id.speakers_tab_icon);
            session = new Session(Activity as MainActivity);
            speaker = new Speaker();

            var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, session).Show(session);
            fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, speaker).Hide(speaker);
            fragmentTransaction.Commit();

            schedule_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_schedule_selected);

            schedule_tab_icon.Click += schedule_tab_icon_Click;
            speakers_tab_icon.Click += speakers_tab_icon_Click;
            return view;
        }

        void speakers_tab_icon_Click(object sender, EventArgs e)
        {
            schedule_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_schedule_normal);

            var titleTextView = actionbar.FindViewById<TextView>(Resource.Id.titleTextView);
            titleTextView.Text = "SPEAKERS";
            speakers_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_speakers_selected);
            ChildFragmentManager.BeginTransaction().Show(speaker).Commit();
            ChildFragmentManager.BeginTransaction().Hide(session).Commit();
        }

        void schedule_tab_icon_Click(object sender, EventArgs e)
        {
            schedule_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_schedule_selected);

            speakers_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_speakers_normal);

            var titleTextView = actionbar.FindViewById<TextView>(Resource.Id.titleTextView);
            titleTextView.Text = "SESSIONS";
            ChildFragmentManager.BeginTransaction().Hide(speaker).Commit();
            ChildFragmentManager.BeginTransaction().Show(session).Commit();
        }
    }
}
