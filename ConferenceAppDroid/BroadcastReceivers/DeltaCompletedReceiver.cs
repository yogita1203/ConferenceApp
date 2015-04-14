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

namespace ConferenceAppDroid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class DeltaCompletedReceiver : BroadcastReceiver
    {
        public const string action = "completed";
        public event Action<Context, Intent> OnBroadcastReceive;
        public override void OnReceive(Context context, Intent intent)
        {
            if (OnBroadcastReceive != null)
                OnBroadcastReceive.Invoke(context, intent);
        }
    }
}