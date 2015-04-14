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
   public class Prefs
    {
       public static ISharedPreferences get(Context context)
       {
           return context.GetSharedPreferences("BUILTIO_VMWORLD_APP_PREF", 0);
       }
    }
}