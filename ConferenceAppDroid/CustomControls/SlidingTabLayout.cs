using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.CustomControls
{
    public class SlidingTabLayout : HorizontalScrollView, ViewPager.IOnPageChangeListener, TabHost.IOnTabChangeListener, View.IOnClickListener
    {
        private int mScrollState;
        public interface TabColorizer
        {

            /**
             * @return return the color of the indicator used when {@code position} is selected.
             */
            int getIndicatorColor(int position);

            /**
             * @return return the color of the divider drawn to the right of {@code position}.
             */
            int getDividerColor(int position);

        }

        private const int TITLE_OFFSET_DIPS = 24;
        private  const int TAB_VIEW_PADDING_DIPS = 16;
        private const int TAB_VIEW_TEXT_SIZE_SP = 12;

        private int mTitleOffset;

        private int mTabViewLayoutId;
        private int mTabViewTextViewDayId;
        private int mTabViewTextViewDateId;

        private ViewPager mViewPager;
        private ViewPager.IOnPageChangeListener mViewPagerPageChangeListener;

        private SlidingTabStrip mTabStrip;

        private Context context;


        public SlidingTabLayout(Context context)
            : base(context, null)
        {

            this.context = context;
        }

        public SlidingTabLayout(Context context, IAttributeSet attrs)
            : base(context, attrs, 0)
        {
            this.context = context;
        }
        public SlidingTabLayout(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            this.context = context;

            // Disable the Scroll Bar
            HorizontalScrollBarEnabled = false;
            // Make sure that the Tab Strips fills this View
            FillViewport = true;

            mTitleOffset = (int)(TITLE_OFFSET_DIPS * context.Resources.DisplayMetrics.Density);

            mTabStrip = new SlidingTabStrip(context);
            AddView(mTabStrip, LayoutParams.MatchParent, LayoutParams.WrapContent);
        }
        public void setCustomTabColorizer(SlidingTabLayout.TabColorizer tabColorizer)
        {
            mTabStrip.setCustomTabColorizer(tabColorizer);
        }
        public void setSelectedIndicatorColors(int colors)
        {
            mTabStrip.setSelectedIndicatorColors(colors);
        }

        /**
         * Sets the colors to be used for tab dividers. These colors are treated as a circular array.
         * Providing one color will mean that all tabs are indicated with the same color.
         */
        public void setDividerColors(int colors)
        {
            mTabStrip.setDividerColors(colors);
        }

        /**
         * Set the {@link ViewPager.OnPageChangeListener}. When using {@link SlidingTabLayout} you are
         * required to set any {@link ViewPager.OnPageChangeListener} through this method. This is so
         * that the layout can update it's scroll position correctly.
         *
         * @see ViewPager#setOnPageChangeListener(ViewPager.OnPageChangeListener)
         */
        //public void setOnPageChangeListener(ViewPager.OnPageChangeListener listener)
        //{
        //    mViewPagerPageChangeListener = listener;
        //}

        /**
         * Set the custom layout to be inflated for the tab views.
         *
         * @param layoutResId Layout id to be inflated
         * @param textDayId   id of the {@link TextView} in the inflated view
         */
        public void setCustomTabView(int layoutResId, int textDayId, int textDateId)
        {
            mTabViewLayoutId = layoutResId;
            mTabViewTextViewDayId = textDayId;
            mTabViewTextViewDateId = textDateId;
        }

        /**
         * Sets the associated view pager. Note that the assumption here is that the pager content
         * (number of tabs and tab titles) does not change after this call has been made.
         */
        public void setViewPager(ViewPager viewPager)
        {
            if (mTabStrip!=null && mTabStrip.ChildCount > 0)
            {
                mTabStrip.RemoveAllViews();
            }

            mViewPager = viewPager;
            if (viewPager != null)
            {
                populateTabStrip();
            }
        }

        protected TextView createDefaultTabView(Context context)
        {
            TextView textView = new TextView(context);
            textView.Gravity = Android.Views.GravityFlags.Center;
            //textView.TextSize=TypedValue.complect.COMPLEX_UNIT_SP, TAB_VIEW_TEXT_SIZE_SP);
            textView.SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.IceCreamSandwich)
            {
                // If we're running on Honeycomb or newer, then we can use the Theme's
                // selectableItemBackground to ensure that the View has a pressed state
                TypedValue outValue = new TypedValue();
                context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground,
                        outValue, true);
                textView.SetBackgroundResource(outValue.ResourceId);
            }

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.IceCreamSandwich)
            {
                // If we're running on ICS or newer, enable all-caps to match the Action Bar tab style
                textView.SetAllCaps(true);
            }

            int padding = (int)(TAB_VIEW_PADDING_DIPS * context.Resources.DisplayMetrics.Density);
            textView.SetPadding(padding, padding, padding, padding);

            return textView;
        }
        private void populateTabStrip()
        {
            PagerAdapter adapter = mViewPager.Adapter;
            //const View.OnClickListener tabClickListener = new TabClickListener();

            for (int i = 0; i < adapter.Count; i++)
            {
                View tabView = null;
                TextView tabTitleDayView = null;
                TextView tabTitleDateView = null;

                if (mTabViewLayoutId != 0)
                {
                    // If there is a custom tab view layout id set, try and inflate it
                    tabView = LayoutInflater.From(context).Inflate(mTabViewLayoutId, mTabStrip,
                            false);
                    tabTitleDayView = (TextView)tabView.FindViewById(mTabViewTextViewDayId);
                    tabTitleDateView = (TextView)tabView.FindViewById(mTabViewTextViewDateId);


                    String[] parts = adapter.GetPageTitle(i).ToString().Split(':');
                    String splitDay = null;
                    String splitDate = null;
                    String splitMonth = null;

                    if (i == 0)
                    {
                        tabTitleDateView.Visibility = ViewStates.Gone;
                        tabTitleDayView.Text = adapter.GetPageTitle(i).ToString().ToUpper();

                    }
                    else
                    {
                        tabTitleDateView.Visibility = ViewStates.Visible;
                        splitDay = parts[0];
                        splitDate = parts[1];
                        splitMonth = parts[2];

                        //tabTitleDateView.setText(AppUtilities.upperFirst(splitMonth) + " " + splitDate);
                        tabTitleDayView.Text = splitDay.ToUpper();

                    }
                }

                if (tabView == null)
                {
                    tabView = createDefaultTabView(Context);
                }

                if (tabTitleDayView == null && tabView.GetType() == typeof(TextView))
                {
                    tabTitleDayView = (TextView)tabView;
                    tabTitleDayView.Text = adapter.GetPageTitle(i);
                }
                
                mTabStrip.AddView(tabView);
            }

        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (mViewPager != null)
            {
                scrollToTab(mViewPager.CurrentItem, 0);
            }
        }

        private void scrollToTab(int tabIndex, int positionOffset)
        {
            int tabStripChildCount = mTabStrip.ChildCount;
            if (tabStripChildCount == 0 || tabIndex < 0 || tabIndex >= tabStripChildCount)
            {
                return;
            }

            View selectedChild = mTabStrip.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int targetScrollX = selectedChild.Left + positionOffset;

                if (tabIndex > 0 || positionOffset > 0)
                {
                    // If we're not at the first child and are mid-scroll, make sure we obey the offset
                    targetScrollX -= mTitleOffset;
                }

                ScrollTo(targetScrollX, 0);
            }
        }

        public void OnPageScrollStateChanged(int state)
        {
            mScrollState = state;
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            int tabStripChildCount = mTabStrip.ChildCount;
            if ((tabStripChildCount == 0) || (position < 0) || (position >= tabStripChildCount))
            {
                return;
            }


            View selectedTitle = mTabStrip.GetChildAt(position);
            int extraOffset = (selectedTitle != null)
                    ? (int)(positionOffset * selectedTitle.Width)
                    : 0;
            scrollToTab(position, extraOffset);

        }

        public void OnPageSelected(int position)
        {
            if (mScrollState == ViewPager.ScrollStateIdle)
            {
                scrollToTab(position, 0);
            }

        }

        public void OnTabChanged(string tabId)
        {
            throw new NotImplementedException();
        }

        public void OnClick(View v)
        {
            for (int i = 0; i < mTabStrip.ChildCount; i++)
            {
                if (v == mTabStrip.GetChildAt(i))
                {
                    mViewPager.CurrentItem = i;
                    return;
                }
            }
        }
    }

}
