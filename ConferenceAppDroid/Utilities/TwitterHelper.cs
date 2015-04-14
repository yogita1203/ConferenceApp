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
using Twitter4j;
using Twitter4j.Conf;

namespace ConferenceAppDroid.Utilities
{
    public class TwitterHelper
    {
        public static ITwitter getTwitterInstance(Context _Context)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetOAuthConsumerKey(AppConstants.twitter_consumer_key);
            configurationBuilder.SetOAuthConsumerSecret(AppConstants.twitter_consumer_secret);
            configurationBuilder.SetOAuthAccessToken(AppSettings.Instance.getTwitterAccessToken(_Context));
            configurationBuilder.SetOAuthAccessTokenSecret(AppSettings.Instance.getTwitterAccessTokenSecret(_Context));
            IConfiguration configuration = configurationBuilder.Build();
            return new TwitterFactory(configuration).Instance;
        }
    }
}