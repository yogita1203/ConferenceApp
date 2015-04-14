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

namespace ConferenceAppDroid.Fragments
{
    public class ScheduleInterest : Android.Support.V4.App.Fragment
    {
        RadioGroup radioGroup;
        MyScheduleFragment schedule;
        MyInterestSessionFragment interest;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.scheduleAndInterest, null);
            radioGroup = view.FindViewById<RadioGroup>(Resource.Id.notesGroup);
            radioGroup.CheckedChange += radioGroup_CheckedChange;

            schedule = new MyScheduleFragment();
            interest= new MyInterestSessionFragment();

            var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.my_schedule_fragment_container, schedule).Show(schedule);
            fragmentTransaction.Add(Resource.Id.my_schedule_fragment_container, interest).Hide(interest);
            fragmentTransaction.Commit();

            return view;
        }


        void radioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            var current = View.FindViewById<RadioButton>(radioGroup.CheckedRadioButtonId);
            var currentText=current.Text;
            if(currentText.Equals("Schedule",StringComparison.InvariantCultureIgnoreCase))
            {
                ChildFragmentManager.BeginTransaction().Show(schedule).Commit();
                ChildFragmentManager.BeginTransaction().Hide(interest).Commit();
            }
            else
            {
                ChildFragmentManager.BeginTransaction().Show(interest).Commit();
                ChildFragmentManager.BeginTransaction().Hide(schedule).Commit();
            }
        }
    }
}