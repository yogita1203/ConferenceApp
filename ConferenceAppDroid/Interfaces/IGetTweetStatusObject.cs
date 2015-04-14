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
using ConferenceAppDroid.async;
using Twitter4j;

namespace ConferenceAppDroid.Interfaces
{
    public interface IGetTweetStatusObject
    {
        void setTweetStatusObject(IStatus result, int position, TweetActionAsync.ActionType type);
    }
}