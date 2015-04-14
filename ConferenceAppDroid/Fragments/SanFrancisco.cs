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
using Android.Support.V4.View;
using Java.Lang;
using Android.Support.V4.App;

namespace ConferenceAppDroid.Fragments
{
    public class SanFrancisco : Android.Support.V4.App.Fragment
    {
        TabHost tabHost;
        ListView lstTransport;
        Transportation transportation;
        FoodNDrink foodndrink;
        ImageView Transportation_tab_icon;
        ImageView FoodNDrinks_tab_icon;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.sanfrancisco, null);
            Transportation_tab_icon = view.FindViewById<ImageView>(Resource.Id.Transportation_tab_icon);
            FoodNDrinks_tab_icon = view.FindViewById<ImageView>(Resource.Id.FoodNDrinks_tab_icon);
            transportation = new Transportation();
            foodndrink = new FoodNDrink();

            var fragmentTransaction = Activity.SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, transportation).Show(transportation);
            fragmentTransaction.Add(Resource.Id.sessionFragmentContainer, foodndrink).Hide(foodndrink);
            fragmentTransaction.Commit();

            Transportation_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_transportation_selected);

            Transportation_tab_icon.Click += Transportation_tab_icon_Click;
            FoodNDrinks_tab_icon.Click += FoodNDrinks_tab_icon_Click;

            return view;
        }

        void FoodNDrinks_tab_icon_Click(object sender, EventArgs e)
        {
            Transportation_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_transportation_normal);

            FoodNDrinks_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_foodndrink_selected);
            ChildFragmentManager.BeginTransaction().Show(foodndrink).Commit();
            ChildFragmentManager.BeginTransaction().Hide(transportation).Commit();
        }

        void Transportation_tab_icon_Click(object sender, EventArgs e)
        {
            Transportation_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_transportation_selected);

            FoodNDrinks_tab_icon.SetBackgroundResource(Resource.Drawable.ic_tab_foodndrink_normal);

            ChildFragmentManager.BeginTransaction().Hide(foodndrink).Commit();
            ChildFragmentManager.BeginTransaction().Show(transportation).Commit();
        }

        //protected class TabsAdapter : FragmentPagerAdapter, TabHost.IOnTabChangeListener, ViewPager.IOnPageChangeListener
        //{
        //    private Context _context;
        //    private TabHost _tabHost;
        //    private ViewPager _viewPager;
        //    private List<TabInfo> _tabs = new List<TabInfo>();

        //    public class TabInfo
        //    {
        //        public string tag;
        //        public Class clss;
        //        public Bundle args;
        //        public Android.Support.V4.App.Fragment fragment { get; set; }

        //        public TabInfo(string _tag, Class _class, Bundle _args)
        //        {
        //            tag = _tag;
        //            clss = _class;
        //            args = _args;
        //        }
        //    }

        //    public class DummyTabFactory : Java.Lang.Object, TabHost.ITabContentFactory
        //    {
        //        private Context _context;

        //        public DummyTabFactory(Context context)
        //        {
        //            _context = context;
        //        }

        //        public View CreateTabContent(string tag)
        //        {
        //            var v = new View(_context);
        //            v.SetMinimumHeight(0);
        //            v.SetMinimumWidth(0);
        //            return v;
        //        }
        //    }

        //    public TabsAdapter(FragmentActivity activity, TabHost tabHost, ViewPager pager)
        //        : base(activity.SupportFragmentManager)
        //    {
        //        _context = activity;
        //        _tabHost = tabHost;
        //        _viewPager = pager;
        //        _tabHost.SetOnTabChangedListener(this);
        //        _viewPager.Adapter = this;
        //        _viewPager.SetOnPageChangeListener(this);
        //    }

        //    public void AddTab(TabHost.TabSpec tabSpec, Class clss, Bundle args)
        //    {
        //        tabSpec.SetContent(new DummyTabFactory(_context));
        //        var tag = tabSpec.Tag;

        //        var info = new TabInfo(tag, clss, args);

        //        _tabs.Add(info);
        //        _tabHost.AddTab(tabSpec);
        //        NotifyDataSetChanged();
        //    }

        //    public override int Count
        //    {
        //        get
        //        {
        //            return _tabs.Count;
        //        }
        //    }

        //    public override Android.Support.V4.App.Fragment GetItem(int position)
        //    {
        //        var info = _tabs[position];
        //        return Android.Support.V4.App.Fragment.Instantiate(_context, info.clss.Name, info.args);
        //    }

        //    public void OnTabChanged(string tabId)
        //    {
        //        int position = _tabHost.CurrentTab;
        //        _viewPager.CurrentItem = position;
        //    }

        //    public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        //    {

        //    }

        //    public void OnPageSelected(int position)
        //    {
        //        // Unfortunately when TabHost changes the current tab, it kindly
        //        // also takes care of putting focus on it when not in touch mode.
        //        // The jerk.
        //        // This hack tries to prevent this from pulling focus out of our
        //        // ViewPager.
        //        var widget = _tabHost.TabWidget;
        //        var oldFocusability = widget.DescendantFocusability;
        //        widget.DescendantFocusability = DescendantFocusability.BlockDescendants;
        //        _tabHost.CurrentTab = position;
        //        widget.DescendantFocusability = oldFocusability;
        //    }

        //    public void OnPageScrollStateChanged(int state)
        //    {

        //    }
        //}
      
    }
}