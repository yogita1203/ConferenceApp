using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using ConferenceAppDroid.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.Fragments
{
    public class SocialFragment : Android.Support.V4.App.Fragment
    {
        private View parentView;
        private Context context;
        private Button allFeedsButton;
        private Button impLinkButton;
        private ImportantLinksFragment importantLinksFragment;
        private SocialAllFeedsFragment socialAllFeedsFragment;
        private FragmentTransaction fragmentTransaction;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.view_social, null, false);
            context = Activity;
            try
            {
                allFeedsButton = (Button)parentView.FindViewById(Resource.Id.social_tabs_all_feed_button);
                impLinkButton = (Button)parentView.FindViewById(Resource.Id.social_tabs_imp_link_button);

                //allFeedsButton.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
                //impLinkButton.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));

                allFeedsButton.Click+=allFeedsButton_Click;     
                impLinkButton.Click+=impLinkButton_Click; 


                socialAllFeedsFragment = new SocialAllFeedsFragment();
                importantLinksFragment = new ImportantLinksFragment();

                var fragmentTransaction =ChildFragmentManager.BeginTransaction();
                fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, socialAllFeedsFragment).Show(socialAllFeedsFragment);
                fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, importantLinksFragment).Hide(importantLinksFragment);

                fragmentTransaction.Commit();


            }
            catch (Exception e)
            {
            }
            return parentView;
        }

        void impLinkButton_Click(object sender, EventArgs e)
        {
            impLinkButton.SetBackgroundColor(Resources.GetColor(Resource.Color.white));

                impLinkButton.SetTextColor(Resources.GetColor(Resource.Color.text_description));
                allFeedsButton.SetBackgroundColor(Resources.GetColor(Resource.Color.grey));
            //else
            //{
            //    impLinkButton.setTextColor(getResources().getColor(R.color.theme_bottom_color));
            //    allFeedsButton.setBackgroundColor(getResources().getColor(R.color.theme_bottom_color));
            //}
            allFeedsButton.SetTextColor(Resources.GetColor(Resource.Color.text_description));

            if (!importantLinksFragment.IsVisible)
            {
                ChildFragmentManager.BeginTransaction().Hide(socialAllFeedsFragment).Show(importantLinksFragment).Commit();
                //                    getChildFragmentManager().beginTransaction().show(importantLinksFragment).commit();
            }

        }

        void allFeedsButton_Click(object sender, EventArgs e)
        {
            allFeedsButton.SetBackgroundColor(Resources.GetColor(Resource.Color.white));

                allFeedsButton.SetTextColor(Resources.GetColor(Resource.Color.text_description));
                impLinkButton.SetBackgroundColor(Resources.GetColor(Resource.Color.grey));
            //else
            //{
            //    allFeedsButton.setTextColor(getResources().getColor(R.color.theme_bottom_color));
            //    impLinkButton.setBackgroundColor(getResources().getColor(R.color.theme_bottom_color));
            //}

            impLinkButton.SetTextColor(Resources.GetColor(Resource.Color.text_description));

            if (!socialAllFeedsFragment.IsVisible)
            {
                ChildFragmentManager.BeginTransaction().Show(socialAllFeedsFragment).Hide(importantLinksFragment).Commit();
                //                    getChildFragmentManager().beginTransaction().hide(importantLinksFragment).commit();
            }
        }
    }
}
