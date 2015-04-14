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
using Java.Util;

namespace ConferenceAppDroid
{
    [Application()]
    public class MainApplication : Android.App.Application
    {
        private Timer mActivityTransitionTimer;
        private TimerTask mActivityTransitionTimerTask;
        public bool wasInBackground;
        private long MAX_ACTIVITY_TRANSITION_TIME_MS = 2000;
        public long lastOpenedWebFragmentId = 0;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        { }
        public override void OnCreate()
        {
            base.OnCreate();

        }

        public void startActivityTransitionTimer()
        {
            this.mActivityTransitionTimer = new Timer();
            this.mActivityTransitionTimerTask = new MyTask(this);


            this.mActivityTransitionTimer.Schedule(mActivityTransitionTimerTask,
                    MAX_ACTIVITY_TRANSITION_TIME_MS);
        }
        public void stopActivityTransitionTimer()
        {
            if (this.mActivityTransitionTimerTask != null)
            {
                this.mActivityTransitionTimerTask.Cancel();
            }

            if (this.mActivityTransitionTimer != null)
            {
                this.mActivityTransitionTimer.Cancel();
            }

            
            this.wasInBackground = false;
            Console.WriteLine(this.wasInBackground);
        }
    }

    public class MyTask : TimerTask
    {
        MainApplication application;
        public MyTask(MainApplication application)
        {
            this.application = application;
        }
        public override void Run()
        {
            application.wasInBackground = true;
            Console.WriteLine(application.wasInBackground);
        }
    }


}