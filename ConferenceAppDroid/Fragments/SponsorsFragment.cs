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
using Android.Views.Animations;
using ConferenceAppDroid.Utilities;


namespace ConferenceAppDroid.Fragments
{
    public class SponsorsFragment : Android.Support.V4.App.Fragment
    {
        private View parentView;
        private Handler handler = new Handler();
        private SponsorByContribution byContribution;
        private LoadFragmentTimer loadFragmentTimer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.view_sponsors, container, false);

            loadFragmentTimer = new LoadFragmentTimer(AppConstants.TOTALTIME, AppConstants.INTERVAL,this);
            loadFragmentTimer.Start();

            return parentView;
        }

        private class LoadFragmentTimer: CountDownTimer 
        {
            private SponsorsFragment byContribution;
            public LoadFragmentTimer(long millisInFuture, long countDownInterval, SponsorsFragment SponsorsFragment)
                : base(millisInFuture, countDownInterval)
        {
            byContribution = SponsorsFragment;
        }

        public override void OnFinish()
        {
            byContribution.loadData();
        }

        public override void OnTick(long millisUntilFinished)
        {
        }
   
    }

        
    public void loadData()
    {
        try
        {
            byContribution = new SponsorByContribution();
            ChildFragmentManager.BeginTransaction().Add(Resource.Id.sponsor_fragment_container, byContribution, "demo").Show(byContribution).Commit();
            if (AppConstants.SPONSOR_SELECTED_FROM_EXPLORE.ToString().Trim().Length > 0)
            {
                Bundle bundle = new Bundle();
                bundle.PutString("name", AppConstants.SPONSOR_SELECTED_FROM_EXPLORE.ToString().Trim());
                byContribution.Arguments=(bundle);
            }
            else
            {
                Bundle bundle = new Bundle();
                bundle.PutString("name", "contribution");
                byContribution.Arguments=(bundle);
            }

            Animation animationFadeIn = AnimationUtils.LoadAnimation(Activity, Resource.Animation.fadein);
            parentView.StartAnimation(animationFadeIn);
        }
        catch (Exception e)
        {}
    }

         public void loadViews(String param) 
    {
        handler.PostDelayed(()=>
        {

                    byContribution = new SponsorByContribution();

                    ChildFragmentManager.BeginTransaction().Add(Resource.Id.sponsor_fragment_container, byContribution).Show(byContribution).Commit();
                    Bundle bundle = new Bundle();
                    bundle.PutString("name", param);
                    byContribution.Arguments=(bundle);
        }, 500);
    }

    public void showSponsorSearch(bool showSearch)
    {
        byContribution.showSearchEditText(showSearch);
    }


    }
}