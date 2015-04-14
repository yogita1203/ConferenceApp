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
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using Newtonsoft.Json;
using Java.Util;
using ConferenceAppDroid.async;
using ConferenceAppDroid.Activities;
using ConferenceAppDroid.Interfaces;
using Android.Content.PM;

namespace ConferenceAppDroid.Fragments
{
    public class SocialMoreActionDialogFragment : Android.Support.V4.App.DialogFragment, IGetTweetStatusObject
    {
        private Dialog dialog;
        ListView dropDownListview;
        Button cancelDropDown, removeFilter;
        LayoutInflater inflater;
        string[] twitterActions = { "Retweet", "Reply", "Favorite", "Email", "Message" };
        SocialMoreActionAdapter socialMoreActionAdapter;
        BuiltTwitter currentTwitterDataModel;
        Context mContext;
        private int count;
        private int position;
        private TextView tweetButton;
        long id;
        public override Android.App.Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            dialog = new Dialog(Activity);

            if (Arguments != null)
            {
                var test = Arguments.GetString("currentTwitterDataModel");
                Newtonsoft.Json.Linq.JObject dataTmp = Newtonsoft.Json.Linq.JObject.Parse(test.Replace("\r\n", ""));
                currentTwitterDataModel = JsonConvert.DeserializeObject<BuiltTwitter>(dataTmp.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                position = Arguments.GetInt("position", 0);
            }
            id = Convert.ToInt64(currentTwitterDataModel.social_object_dict["id_str"].ToString());
            mContext = Activity;
            this.inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);

            dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            dialog.SetCanceledOnTouchOutside(false);

            dialog.SetTitle("Social Post Options");
            dialog.SetContentView(Resource.Layout.view_tracks_filter_dialog);
            setDialogPosition(dialog);

            dropDownListview = (ListView)dialog.FindViewById(Resource.Id.drop_down_listview);
            cancelDropDown = (Button)dialog.FindViewById(Resource.Id.cancel_drop_down);
            removeFilter = (Button)dialog.FindViewById(Resource.Id.remove_filter);
            removeFilter.Visibility = ViewStates.Gone;

            dialog.Window.Attributes.WindowAnimations = Resource.Style.DialogAnimation;
            socialMoreActionAdapter = new SocialMoreActionAdapter(Activity, 0, twitterActions);
            dropDownListview.Adapter = socialMoreActionAdapter;

            dropDownListview.ItemClick += (s, e) =>
                {
                    var currentItem = twitterActions[e.Position];
                    if (currentItem.Equals("Email", StringComparison.InvariantCultureIgnoreCase))
                    {
                        ShareViaEmail();
                    }
                    if (currentItem.Equals("Retweet", StringComparison.InvariantCultureIgnoreCase))
                    {
                        retweetAction(e.Position);
                    }
                    if (currentItem.Equals("Reply", StringComparison.InvariantCultureIgnoreCase))
                    {
                        replyAction(e.Position);
                    }
                    if (currentItem.Equals("Favorite", StringComparison.InvariantCultureIgnoreCase))
                    {
                        favoriteAction(e.Position);
                    }


                    if (currentItem.Equals("Message", StringComparison.InvariantCultureIgnoreCase))
                    {
                        doMessage();
                    }
                };


            return dialog;
        }

        private void ShareViaEmail()
        {
            BuiltAllFeeds allFeeds = AppSettings.Instance.config.social.all_feeds;
            string link = string.Empty;
            if (currentTwitterDataModel.social_type.ToLower() == "instagram")
            {
                link = string.Format("https://instagram.com");
                string socialPayload;
                socialPayload = string.Format("{0} {1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.getALLSocialModel().content_text, link);
                Activity.RunOnUiThread(() =>
                {
                    Dictionary<string, object> dataTmp = new Dictionary<string, object>();
                    dataTmp.Add("subject", allFeeds.email_subject);
                    dataTmp.Add("recipients", "");
                    dataTmp.Add("payload", socialPayload);
                    sendEmail(dataTmp);
                    return;
                });


            }
            else
            {
                if (!string.IsNullOrEmpty(this.currentTwitterDataModel.user.screen_name) && this.currentTwitterDataModel.social_object_dict.ContainsKey("id_str"))
                {
                    link = string.Format("https://twitter.com/{0}/status/{1}", this.currentTwitterDataModel.user.screen_name, this.currentTwitterDataModel.social_object_dict["id_str"]);
                }


                string payload;
                payload = string.Format("{0} {1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.content_text, link);

                Activity.RunOnUiThread(() =>
                {
                    Dictionary<string, object> dataTmp = new Dictionary<string, object>();
                    dataTmp.Add("subject", allFeeds.email_subject);
                    dataTmp.Add("recipients", "");
                    dataTmp.Add("payload", payload);
                    sendEmail(dataTmp);
                });
            }
        }

        void doMessage()
        {
            BuiltAllFeeds allFeeds = AppSettings.Instance.config.social.all_feeds;
            string payload;
            if ((this.currentTwitterDataModel.entities.media.Count > 0 || this.currentTwitterDataModel.entities.urls.Count > 0) && (this.currentTwitterDataModel.entities.media[0].url != null && this.currentTwitterDataModel.entities.media[0].url.Length > 0))
            {
                payload = string.Format("{1} {2} \n{3}", allFeeds.hashtags, this.currentTwitterDataModel.content_text, this.currentTwitterDataModel.entities.media[0].url);
            }
            else
            {
                payload = string.Format("{1} {2}", allFeeds.hashtags, this.currentTwitterDataModel.content_text);
            }
            Activity.RunOnUiThread(() =>
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("recipients", "");
                data.Add("payload", payload);
                sendMessage(data);
            });
        }

        private void sendMessage(Dictionary<string, object> data)
        {
            Intent sendIntent = new Intent(Intent.ActionView);
            sendIntent.PutExtra("sms_body", data["payload"].ToString());
            sendIntent.SetType("vnd.android-dir/mms-sms");
            IList<ResolveInfo> availableActivities = Activity.PackageManager.QueryIntentActivities(sendIntent, PackageInfoFlags.MatchDefaultOnly);
            if (availableActivities != null && availableActivities.Count > 0)
            {
                StartActivity(sendIntent);
            }
        }

        private void sendEmail(Dictionary<string, object> dataTmp)
        {
            var email = new Intent(Android.Content.Intent.ActionSend);
            email.PutExtra(Android.Content.Intent.ExtraSubject, dataTmp["subject"].ToString());
            email.PutExtra(Android.Content.Intent.ExtraText, dataTmp["payload"].ToString());
            email.SetType("message/rfc822");
             IList<ResolveInfo> availableActivities=Activity.PackageManager.QueryIntentActivities(email, PackageInfoFlags.MatchDefaultOnly);
             if (availableActivities != null && availableActivities.Count>0)
             {
                 StartActivity(email);
             }
        }
        private void setDialogPosition(Dialog dialog)
        {
            Window window = dialog.Window;
            WindowManagerLayoutParams wlp = window.Attributes;

            wlp.Gravity = GravityFlags.Bottom;
            //		wlp.flags &= ~WindowManager.LayoutParams.FLAG_DIM_BEHIND;

            wlp.Width = RelativeLayout.LayoutParams.MatchParent;
            window.Attributes = wlp;

        }

        void dofavourite()
        {
            string requestURL = string.Empty;
            if (this.currentTwitterDataModel.favourite)
            {
                requestURL = "https://api.twitter.com/1.1/favorites/destroy.json";
            }
            else
            {
                requestURL = "https://api.twitter.com/1.1/favorites/create.json";
            }
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("id", currentTwitterDataModel.social_object_dict["id_str"]);

        }

        private void retweetAction(int position)
        {


            if (AppSettings.Instance.isTwitterAuthenticationDone(mContext))
            {

                cancelDropDown.PerformClick();

                Dialog dialog = new Dialog(mContext);
                View promptsView = inflater.Inflate(Resource.Layout.view_twitter_reply_alert_box, null);
                EditText replyText = (EditText)promptsView.FindViewById(Resource.Id.socialTwitterReply_reply);
                //replyText.setTypeface(Typeface.createFromAsset(mContext.getAssets(), AppConstants.FONT));
                TextView tweetCharCount = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_char_count);
                TextView twitterText = ((TextView)promptsView.FindViewById(Resource.Id.twitterText));
                twitterText.Text = (mContext.Resources.GetString(Resource.String.retweet_this_to_your_followers_text));
                promptsView.FindViewById(Resource.Id.horizontalLine).Visibility = ViewStates.Visible;
                TextView cancelButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_cancel_button);
                cancelButton.Visibility = ViewStates.Visible;
                TextView tweetButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_tweet_button);
                tweetButton.Visibility = ViewStates.Visible;
                tweetButton.Text = (mContext.Resources.GetString(Resource.String.retweet_text));

                tweetCharCount.Visibility = ViewStates.Gone;
                replyText.Enabled = (false);
                //replyText.Text=(socialModel.message);
                tweetButton.Click += (s, e) =>
                    {
                        TweetActionAsync actionAsync = new TweetActionAsync(mContext, TweetActionAsync.ActionType.retweet, this, id, position);
                        actionAsync.Execute();
                        dialog.Dismiss();
                    };

                dialog.Window.RequestFeature(WindowFeatures.NoTitle);
                dialog.Window.SetBackgroundDrawableResource(Resource.Drawable.twitter_reply_dialog_drawable);
                dialog.Window.SetContentView(promptsView);

                cancelButton.Click += (s, e) =>
                    {
                        dialog.Cancel();
                    };

                dialog.Show();


            }
            else
            {
                Intent twitterLoginIntent = new Intent(mContext, typeof(TwitterLoginActivity));
                Bundle bundle = new Bundle();
                bundle.PutString("ActionType", ConferenceAppDroid.async.TweetActionAsync.ActionType.retweet.ToString());
                bundle.PutInt("position", position);
                bundle.PutBoolean("isFromDialog", true);
                twitterLoginIntent.PutExtras(bundle);
                mContext.StartActivity(twitterLoginIntent);
            }
        }

        private void favoriteAction(int position) 
        {

        if (AppSettings.Instance.isTwitterAuthenticationDone(mContext)) 
        {
            cancelDropDown.PerformClick();
            try 
            {
                  if (this.currentTwitterDataModel.favourite)
            {
                    TweetActionAsync actionAsync = new TweetActionAsync(mContext, TweetActionAsync.ActionType.unfavorite, this, id, position);
                    actionAsync.Execute();
                }
                  else
                  {
                    TweetActionAsync actionAsync = new TweetActionAsync(mContext, TweetActionAsync.ActionType.favorite, this, id, position);
                    actionAsync.Execute();
                }

            }
            catch (Exception e) 
            {
            }
        } 
        else 
        {
            Intent twitterLoginIntent = new Intent(mContext, typeof(TwitterLoginActivity));
            Bundle bundle = new Bundle();
            bundle.PutString("ActionType", ConferenceAppDroid.async.TweetActionAsync.ActionType.favorite.ToString());
            bundle.PutInt("position", position);
            bundle.PutBoolean("isFromDialog", true);
            twitterLoginIntent.PutExtras(bundle);
            mContext.StartActivity(twitterLoginIntent);
        }

    }


        private void replyAction(int position)
        {

            if (AppSettings.Instance.isTwitterAuthenticationDone(mContext))
            {

                cancelDropDown.PerformClick();

                Dialog dialog = new Dialog(mContext);
                View promptsView = inflater.Inflate(Resource.Layout.view_twitter_reply_alert_box, null);

                EditText replyText = (EditText)promptsView.FindViewById(Resource.Id.socialTwitterReply_reply);
                //replyText.setTypeface(Typeface.createFromAsset(mContext.getAssets(), AppConstants.FONT));
                TextView tweetCharCount = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_char_count);

                TextView twitterText = ((TextView)promptsView.FindViewById(Resource.Id.twitterText));
                twitterText.Text = (mContext.Resources.GetString(Resource.String.reply_text));


                promptsView.FindViewById(Resource.Id.horizontalLine).Visibility = ViewStates.Visible;
                TextView cancelButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_cancel_button);
                cancelButton.Visibility = ViewStates.Visible;
                tweetButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_tweet_button);
                tweetButton.Visibility = ViewStates.Visible;
                tweetButton.Text = (mContext.Resources.GetString(Resource.String.reply_text));

                count = (140 - (currentTwitterDataModel.username.ToString().Length));
                tweetCharCount.Text = ("" + count);
                replyText.Text = ("@" + (currentTwitterDataModel).username + " ");

                replyText.SetSelection(replyText.Text.ToString().Length, replyText.Text.ToString().Length);
                tweetButton.Click += (s, e) =>
                    {
                        try
                        {
                            if (replyText.Text.ToString().Contains("@" + (currentTwitterDataModel).username))
                            {
                                if (replyText.Text.ToString().Trim().Length > ((currentTwitterDataModel).username.Length + 1))
                                {
                                    TweetActionAsync actionAsync = new TweetActionAsync(mContext, TweetActionAsync.ActionType.reply, this, id, position);
                                    actionAsync.setreply("" + replyText.Text.ToString());
                                    actionAsync.Execute();
                                    dialog.Dismiss();
                                }
                                else
                                {
                                    Toast.MakeText(mContext, mContext.GetString(Resource.String.reply_text_can_not_be_blank_text), ToastLength.Short).Show();
                                }
                            }
                            else
                            {
                                Toast.MakeText(mContext, mContext.GetString(Resource.String.reply_mention_text), ToastLength.Short).Show();
                            }
                        }
                        catch {}
                    };


                dialog.Window.RequestFeature(WindowFeatures.NoTitle);
                dialog.Window.SetBackgroundDrawableResource(Resource.Drawable.twitter_reply_dialog_drawable);
                dialog.Window.SetContentView(promptsView);
                cancelButton.Click += (s, e) =>
                    {
                        dialog.Cancel();
                    };

                dialog.Show();

                replyText.TextChanged += (s, e) =>
                    {
                        if (e.BeforeCount > 0)
                        {
                            if (e.Start == 1)
                            {
                                replyText.Focusable = (false);
                                Toast.MakeText(mContext, mContext.GetString(Resource.String.reply_mention_text), ToastLength.Short).Show();
                            }
                            else
                            {
                                replyText.FocusableInTouchMode = (true);
                            }
                        }

                        if (e.AfterCount > 0)
                        {
                            if (e.Text.Count() <= 140)
                            {
                                tweetButton.Enabled = (true);
                                tweetButton.Clickable = (true);
                            }
                            else
                            {
                                tweetButton.Enabled = (false);
                                tweetButton.Clickable = (false);
                            }

                            tweetCharCount.Text = ("" + (140 - e.Text.Count()));
                        }
                    };


            }
            else
            {
                Intent twitterLoginIntent = new Intent(mContext, typeof(TwitterLoginActivity));
                Bundle bundle = new Bundle();
                bundle.PutString("ActionType", ConferenceAppDroid.async.TweetActionAsync.ActionType.reply.ToString());
                bundle.PutInt("position", position);
                bundle.PutBoolean("isFromDialog", true);
                twitterLoginIntent.PutExtras(bundle);
            }

        }

        public void setTweetStatusObject(Twitter4j.IStatus result, int position, TweetActionAsync.ActionType type)
        {
            Console.WriteLine();
        }



    }
    public class SocialMoreActionAdapter : ArrayAdapter<string>
    {
        string[] items;
        Context context;
        private LayoutInflater mInflater;
        TextView actionName;

        public SocialMoreActionAdapter(Context context, int resource, string[] items)
            : base(context, resource, items)
        {
            this.context = context;
            this.items = items;
            mInflater = LayoutInflater.From(context);
        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override int Count
        {
            get { return items.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder viewHolder;
            View view = convertView; // re-use an existing view, if one is available
            if (convertView == null)
            {
                view = mInflater.Inflate(Resource.Layout.row_tracks_session, null);
                actionName = (TextView)view.FindViewById(Resource.Id.row_tracks_name_tv);
                viewHolder = new ViewHolder(actionName);

                view.Tag = viewHolder;
            }
            else
            {
                viewHolder = (ViewHolder)view.Tag;
                actionName = viewHolder.actionName;
            }


            actionName.Text = GetItem(position);
            return view;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView actionName;
            public ViewHolder(TextView actionName)
            {
                this.actionName = actionName;
            }
        }

    }
}