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

namespace ConferenceAppDroid.Interfaces
{
    public interface ILoadFragment
    {
            void loadFragment(bool load, String fragmentName, Object @object);
    }
}