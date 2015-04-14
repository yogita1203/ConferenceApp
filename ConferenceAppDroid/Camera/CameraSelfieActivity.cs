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
using Android.Support.V4.App;

namespace ConferenceAppDroid.Camera
{
    [Activity(Label = "CameraSelfieActivity")]
    public class CameraSelfieActivity : FragmentActivity, CameraSelfieFragment
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
        }
    }
}