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

namespace ConferenceAppDroid.Utilities
{
   
    public class CountDown : CountDownTimer
    {
        public delegate void TickEvent(long millisUntilFinished);
        public delegate void FinishEvent();

        public string tem;
        public event TickEvent Tick;
        public event FinishEvent Finish;

        public CountDown(long totaltime, long interval)
            : base(totaltime, interval)
        {
        }

        public override void OnTick(long millisUntilFinished)
        {
            if (Tick != null)
                Tick(millisUntilFinished);
        }

        public override void OnFinish()
        {
            if (Finish != null)
                Finish();
        }
    }
}