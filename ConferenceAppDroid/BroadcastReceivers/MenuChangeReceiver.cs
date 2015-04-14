using Android.Content;
using System;

namespace ConferenceAppDroid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class MenuChangeReceiver : BroadcastReceiver
    {
        public const string action = "menu_changed";
        public event Action<Context, Intent> OnBroadcastReceive;
        public override void OnReceive(Context context, Intent intent)
        {
            if (OnBroadcastReceive != null)
                OnBroadcastReceive.Invoke(context, intent);
        }
    }
}