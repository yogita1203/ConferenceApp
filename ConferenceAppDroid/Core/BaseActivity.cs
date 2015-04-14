using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ConferenceAppDroid.BroadcastReceivers;
using ConferenceAppDroid.Fragments;
using SlidingMenuSharp;
using SlidingMenuSharp.App;
using ListFragment = Android.Support.V4.App.ListFragment;

namespace ConferenceAppDroid.Core
{
    public class BaseActivity : SlidingFragmentActivity
    {
        
        private readonly int _titleRes;
        protected Android.Support.V4.App.Fragment Frag;
        public Bundle save;
        public DeltaCompletedReceiver deltaCompletedReceiver;
        public ConfigCompletedReceiver configCompletedReceiver;
        public string title;
        public static bool isAppWentToBg = false;

        public static bool isWindowFocused = false;

        public BaseActivity(int titleRes, string title)
        {
            _titleRes = titleRes;
            this.title = title;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            save = savedInstanceState;
            //SetTitle(_titleRes);

            SetBehindContentView(Resource.Layout.menu_frame);

            if (null == savedInstanceState)
            {
                var t = SupportFragmentManager.BeginTransaction();
                Frag = new LeftMenuFragment(this);
                t.Replace(Resource.Id.menu_frame, Frag,"left_fragment");
                t.Commit();
            }
            else
                Frag =
                    (ListFragment)
                    SupportFragmentManager.FindFragmentById(Resource.Id.menu_frame);

            SlidingMenu.BehindWidth = SlidingMenu.Width;
            SlidingMenu.ShadowWidthRes = Resource.Dimension.shadow_width;
            SlidingMenu.BehindOffsetRes = Resource.Dimension.slidingmenu_offset;
            SlidingMenu.ShadowDrawableRes = Resource.Drawable.shadow;
            SlidingMenu.FadeDegree = 0.25f;
            SlidingMenu.TouchModeAbove = TouchMode.Margin;
            Title = Title;

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            deltaCompletedReceiver = new DeltaCompletedReceiver();
            configCompletedReceiver = new ConfigCompletedReceiver();
        }

        protected override void OnStart()
        {
            applicationWillEnterForeground();
                base.OnStart();
        }

        private void applicationWillEnterForeground()
        {
            if (isAppWentToBg)
            {
                isAppWentToBg = false;
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            applicationdidenterbackground();
        }

        private void applicationdidenterbackground()
        {
            if (!isWindowFocused)
            {
                isAppWentToBg = true;
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Toggle();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}