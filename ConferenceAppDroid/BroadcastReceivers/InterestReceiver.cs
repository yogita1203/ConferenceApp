using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.BroadcastReceivers
{
       [BroadcastReceiver]
    public class InterestReceiver:BroadcastReceiver
    {
        public const string action = "Interest";
        public event Action<Context, Intent> OnBroadcastReceive;
        public override void OnReceive(Context context, Intent intent)
        {
            if (OnBroadcastReceive != null)
                OnBroadcastReceive.Invoke(context, intent);
        }
    }
}
