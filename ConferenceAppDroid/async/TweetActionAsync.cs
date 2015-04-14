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
using ConferenceAppDroid.Interfaces;
using Java.IO;
using ConferenceAppDroid.Utilities;
using Twitter4j;

namespace ConferenceAppDroid.async
{
    public class TweetActionAsync:AsyncTask
    {
        public enum ActionType
        {
            favorite, retweet, reply, unfavorite, tweet
        }

        private ActionType mActionType;
        private Context mContext;
        private IGetTweetStatusObject mStatusObject;
        private long mTweetId;
        private int mPosition;
        private String mReplyText;
        private File imageFile;

        public TweetActionAsync(Context context, ActionType type, IGetTweetStatusObject statusObject, long tweetId, int position)
        {
            mContext = context;
            this.mActionType = type;
            this.mStatusObject = statusObject;
            this.mTweetId = tweetId;
            this.mPosition = position;
        }

        public void setreply(String reply)
        {
            mReplyText = reply;
        }

        public void setImageFileToUpload(File imageFile)
        {
            if (imageFile.Exists())
            {
                this.imageFile = imageFile;
            }
        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            try
            {
            if (mActionType == ActionType.favorite)
            {
                mStatusObject.setTweetStatusObject(TwitterHelper.getTwitterInstance(mContext).CreateFavorite(mTweetId), mPosition, mActionType);

            }
            else if (mActionType == ActionType.unfavorite)
            {
                mStatusObject.setTweetStatusObject(TwitterHelper.getTwitterInstance(mContext).DestroyFavorite(mTweetId), mPosition, mActionType);

            }
            else if (mActionType == ActionType.retweet) 
            {

                mStatusObject.setTweetStatusObject(TwitterHelper.getTwitterInstance(mContext).RetweetStatus(mTweetId), mPosition, mActionType);


            }
            else if (mActionType == ActionType.reply) 
            {
                ITwitter twitter = TwitterHelper.getTwitterInstance(mContext);
                StatusUpdate statusUpdate = new StatusUpdate(mReplyText);
                statusUpdate.InReplyToStatusId=(mTweetId);
                mStatusObject.setTweetStatusObject(twitter.UpdateStatus(statusUpdate), mPosition, mActionType);

            } else if (mActionType == ActionType.tweet)
            {
                //AppUtilities.sendAnalyticsCall(mContext, "send_tweet", "TweetActionAsync", null);

                ITwitter twitter = TwitterHelper.getTwitterInstance(mContext);
                StatusUpdate statusUpdate = new StatusUpdate(mReplyText);
                if (imageFile != null) {
                    statusUpdate.SetMedia(imageFile);
                }
                twitter.UpdateStatus(statusUpdate);
                //mStatusObject.setTweetStatusObject(twitter.UpdateStatus(statusUpdate), mPosition, mActionType);

            }

        }
            catch (Exception e)
            {
                mStatusObject.setTweetStatusObject(null, mPosition, mActionType);
            }
            return null;
        }
       
    }
}