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
using System.Threading;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using System.Net;
using Android.Media;
using System.Threading.Tasks;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Core;
using System.IO;
using Newtonsoft.Json;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Text.Method;
using Android.Text.Util;

namespace ConferenceAppDroid.Fragments
{
    public class SocialAllFeedsFragment : Android.Support.V4.App.Fragment
    {
        View parentView;
        private View footerView;
        private Context context;
        private ListView mTweetListView;
        bool isLastRecord;
        int skip = 0;
        int limit = 10;
        public List<BuiltTwitter> twitterSource;
        AllFeedsDataAdapter adapter;
        private RelativeLayout relativeLayout;
        ImageButton right_menu_btn;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var actionbar = Activity.ActionBar.CustomView;
            right_menu_btn=actionbar.FindViewById<ImageButton>(Resource.Id.right_menu_btn);
            right_menu_btn.Visibility = ViewStates.Visible;
            right_menu_btn.SetBackgroundColor(Color.Red);
            relativeLayout = new RelativeLayout(Activity);
            RelativeLayout.LayoutParams mainLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent,RelativeLayout.LayoutParams.MatchParent);
            relativeLayout.LayoutParameters=(mainLayoutParams);
            relativeLayout.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.white));

            parentView = inflater.Inflate(Resource.Layout.allFeedsLayout, null);
            mTweetListView = parentView.FindViewById<ConferenceAppDroid.Core.PullToRefreshListView>(Resource.Id.lvTweets);
            mTweetListView.EmptyView = inflater.Inflate(Resource.Layout.view_loading, null);
            mTweetListView.AddFooterView(setFooter());
            mTweetListView.DividerHeight = (1);
            mTweetListView.Scroll += mTweetListView_Scroll;


            PullToRefreshListView plv = mTweetListView as PullToRefreshListView;
            plv.SetOnRefreshListener(new RefreshListener(this));
            getTweets().ContinueWith(tweets =>
                {
                    twitterSource = tweets.Result;
                    Activity.RunOnUiThread(() =>
                        {
                            try
                            {
                                adapter = new AllFeedsDataAdapter(Activity, Resource.Layout.view_all_feeds, twitterSource);
                                mTweetListView.Adapter = adapter;
                            }
                            catch (Exception e)
                            { }
                        });
                });
            //setBottomActionBar();
            //relativeLayout.AddView(parentView);
            return parentView;
        }

        void mTweetListView_Scroll(object sender, AbsListView.ScrollEventArgs e)
        {
            int lastItem = e.FirstVisibleItem + e.VisibleItemCount;
            if (lastItem == e.TotalItemCount)
            {
                LoadMoreData();
            }
        }

        bool isLoading;
        private void LoadMoreData()
        {
            if (isLastRecord || adapter.Count < limit || isLoading)
                return;
            isLoading = true;
            getTweets().ContinueWith(tweets =>
                {
                    twitterSource = tweets.Result;
                    isLastRecord = twitterSource.Count() == 0;
                    isLoading = false;
                    Activity.RunOnUiThread(() =>
                    {
                        adapter.AddAll(twitterSource);
                        adapter.NotifyDataSetChanged();

                    });
                });
        }

        private void setBottomActionBar() 
        {

            View postActionView = Activity.LayoutInflater.Inflate(Resource.Layout.view_bottom_twitter_post_action, null);
        RelativeLayout.LayoutParams paramss = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
        paramss.AddRule(LayoutRules.Below, mTweetListView.Id);
        paramss.AddRule(LayoutRules.AlignBottom);
        postActionView.LayoutParameters=(paramss);


        RelativeLayout.LayoutParams paramslist = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
        paramslist.AddRule(LayoutRules.Above, postActionView.Id);
        paramslist.BottomMargin = Helper.dpToPx(Activity, 65);

        TextView takeSelfieTextView = (TextView) postActionView.FindViewById(Resource.Id.takeSelfieTextView);
        TextView cameraTextView = (TextView) postActionView.FindViewById(Resource.Id.cameraTextView);
        TextView newTweetTextView = (TextView) postActionView.FindViewById(Resource.Id.newTweetTextView);

        //takeSelfieTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
        //cameraTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
        //newTweetTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));

        takeSelfieTextView.SetTextColor(Resources.GetColor(Resource.Color.blue));
        cameraTextView.SetTextColor(Resources.GetColor(Resource.Color.blue));
        newTweetTextView.SetTextColor(Resources.GetColor(Resource.Color.blue));

        //customView.setLayoutParams(paramslist);
        parentView.LayoutParameters = paramslist;
        relativeLayout.AddView(postActionView);
        relativeLayout.AddView(parentView);


        //postActionView.findViewById(R.id.takeSelfieTextView).setOnClickListener(new OnClickListener() {

        //    @Override
        //    public void onClick(View arg0) {
        //        startActivity(new Intent(context, CameraSelfieActivity.class));
        //    }
        //});

        //postActionView.findViewById(R.id.newTweetTextView).setOnClickListener(new OnClickListener() {

        //    @Override
        //    public void onClick(View arg0) {
        //        AppUtilities.postOnTwitter(context, QueryOrm.getInstance(context).getAllFeedsHashTags() + " " + QueryOrm.getInstance(context).getAllFeedsTweetsText(), null, SocialAllFeedsFragment.this);

        //    }
        //});

        //postActionView.findViewById(R.id.cameraTextView).setOnClickListener(new OnClickListener() {
        //    @Override
        //    public void onClick(View view) {
        //        Intent intent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
        //        fileUri = getOutputMediaFile(0);
        //        intent.putExtra(MediaStore.EXTRA_OUTPUT, fileUri);
        //        getActivity().startActivityForResult(intent, CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE);

        //    }
        //});

    }


        View setFooter()
        {
            if (footerView == null)
            {
                footerView = Activity.LayoutInflater.Inflate(Resource.Layout.view_loading_list, null);

                // ((TextView) footerView.findViewById(R.id.loading_list_tv)).setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
                ((TextView)footerView.FindViewById(Resource.Id.loading_list_tv)).Text = (Activity.Resources.GetString(Resource.String.please_wait_title));

            }
            return footerView;
        }
        Task<List<BuiltTwitter>> getTweets()
        {
            string response_output = string.Empty;
            return Task.Run<List<BuiltTwitter>>(async () =>
            {
                var urltemp = AppSettings.Instance.config.social.all_feeds.url;
                var constraint = string.Format("&skip={0}&limit={1}", skip, limit);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(urltemp + constraint));
                request.ContentType = "application/json";
                request.Method = "GET";

                // Send the request to the server and wait for the response:
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // Get a stream representation of the HTTP web response:
                    using (System.IO.Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        response_output = reader.ReadToEnd();
                        Newtonsoft.Json.Linq.JObject dataTmp = Newtonsoft.Json.Linq.JObject.Parse(response_output.Replace("\r\n", ""));
                        twitterSource = JsonConvert.DeserializeObject<List<BuiltTwitter>>(dataTmp.Property("objects").Value.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        // Use this stream to build a JSON document object:
                        //JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                        //Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                        // Return the JSON document:

                    }
                }
                return twitterSource;
            });

        }

        class RefreshListener : PullToRefreshListView.OnRefreshListener
        {
            private SocialAllFeedsFragment fragment;

            public RefreshListener(SocialAllFeedsFragment _fragment)
            {
                this.fragment = _fragment;
            }

            public void onRefresh()
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    PullToRefreshListView plv = fragment.mTweetListView as PullToRefreshListView;
                    fragment.RefreshTweets();

                    fragment.Activity.RunOnUiThread(delegate
                    {
                        plv.onRefreshComplete();
                    });
                });
            }
        }

        internal void RefreshTweets()
        {
            if (adapter.Count == 0)
                return;
            getTweets().ContinueWith(tweet =>
            {
                twitterSource = tweet.Result;
                Activity.RunOnUiThread(() =>
                {
                    for (int i = 0; i < twitterSource.Count; i++)
                    {
                        adapter.Insert(twitterSource[i], i);
                    }
                    adapter.NotifyDataSetChanged();
                });
            });
        }
    }

    public class AllFeedsDataAdapter : ArrayAdapter<BuiltTwitter>
    {
        Android.App.Activity activity;
        List<BuiltTwitter> builtTwitter;
        public AllFeedsDataAdapter(Android.App.Activity activity, int resource, List<BuiltTwitter> builtTwitter)
            : base(activity, resource, builtTwitter)
        {
            this.activity = activity;
            this.builtTwitter = builtTwitter;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SocialFeedsViewHolder socialFeedsViewHolder;
            View view = convertView;
            TextView message, socialHandleTextView, userNameTextView, socialAllFeedsTimeTextView;
            LinearLayout actionContainer, imageFeedContainer;
            ImageView socialIconImageView, userImageView, feedImageView, playVideoImageView, moreAction;
            if (convertView == null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.view_all_feeds, null);


                message = (TextView)view.FindViewById(Resource.Id.socialAllFeeds_description_textview);
                socialHandleTextView = (TextView)view.FindViewById(Resource.Id.socialAllFeeds_handle_textview);
                userNameTextView = (TextView)view.FindViewById(Resource.Id.socialAllFeeds_user_name_textview);
                socialIconImageView = (ImageView)view.FindViewById(Resource.Id.socialAllFeeds_social_icon_imageview);
                userImageView = (ImageView)view.FindViewById(Resource.Id.socialAllFeeds_user_imageview);
                actionContainer = (LinearLayout)view.FindViewById(Resource.Id.socialAllFeeds_action_container);
                imageFeedContainer = (LinearLayout)view.FindViewById(Resource.Id.socialAllFeeds_feed_image_container);
                feedImageView = (ImageView)view.FindViewById(Resource.Id.socialAllFeeds_feed_image);
                playVideoImageView = (ImageView)view.FindViewById(Resource.Id.socialAllFeeds_play_video_image);
                moreAction = (ImageView)view.FindViewById(Resource.Id.socialAllFeeds_action_more);
                socialAllFeedsTimeTextView = (TextView)view.FindViewById(Resource.Id.socialAllFeeds_time_textview);
                socialFeedsViewHolder = new SocialFeedsViewHolder(socialAllFeedsTimeTextView, message, socialIconImageView, socialHandleTextView, userNameTextView, userImageView, actionContainer, imageFeedContainer, feedImageView, moreAction, playVideoImageView, activity);
                view.Tag = socialFeedsViewHolder;
            }
            else
            {
                socialFeedsViewHolder = (SocialFeedsViewHolder)convertView.Tag;
                message = socialFeedsViewHolder.message;
                socialHandleTextView = socialFeedsViewHolder.socialHandleTextView;
                userNameTextView = socialFeedsViewHolder.userNameTextView;
                socialIconImageView = socialFeedsViewHolder.socialIconImageView;
                userImageView = socialFeedsViewHolder.userImageView;
                actionContainer = socialFeedsViewHolder.actionContainer;
                imageFeedContainer = socialFeedsViewHolder.imageFeedContainer;
                feedImageView = socialFeedsViewHolder.feedImageView;
                playVideoImageView = socialFeedsViewHolder.playVideoImageView;
                moreAction = socialFeedsViewHolder.moreAction;
                socialAllFeedsTimeTextView = socialFeedsViewHolder.socialAllFeedsTimeTextView;
            }
            try
            {
                socialFeedsViewHolder.Populate(builtTwitter[position], position);
            }
            catch { }
            return view;
        }
    }

    public class SocialFeedsViewHolder : Java.Lang.Object
    {
        public TextView socialAllFeedsTimeTextView;
        public TextView message;
        public ImageView socialIconImageView;
        public TextView socialHandleTextView;
        public TextView userNameTextView;
        public ImageView userImageView;
        public LinearLayout actionContainer;
        public LinearLayout imageFeedContainer;
        public ImageView feedImageView;
        public ImageView moreAction;
        static string INSTAGRAM = "instagram";
        static string FACEBOOK = "facebook";
        static string YOUTUBE = "youtube";

        public String postToMessage;
        Android.App.Activity mContext;
        public ImageView playVideoImageView;

        public SocialFeedsViewHolder(TextView socialAllFeedsTimeTextView, TextView message, ImageView socialIconImageView, TextView socialHandleTextView, TextView userNameTextView, ImageView userImageView, LinearLayout actionContainer, LinearLayout imageFeedContainer, ImageView feedImageView, ImageView moreAction, ImageView playVideoImageView, Android.App.Activity context)
        {
            this.socialAllFeedsTimeTextView = socialAllFeedsTimeTextView;
            this.message = message;
            this.socialIconImageView = socialIconImageView;
            this.socialHandleTextView = socialHandleTextView;
            this.userNameTextView = userNameTextView;
            this.userImageView = userImageView;
            this.actionContainer = actionContainer;
            this.imageFeedContainer = imageFeedContainer;
            this.feedImageView = feedImageView;
            this.moreAction = moreAction;
            this.playVideoImageView = playVideoImageView;
            mContext = context;

        }

        public SocialFeedsViewHolder(Context context)
        {

        }

        public void Populate(BuiltTwitter item, int position)
        {
            if (playVideoImageView.Visibility == ViewStates.Visible)
            {
                playVideoImageView.Visibility = ViewStates.Invisible;
            }

            if (item.social_type.ToLower() == INSTAGRAM || item.social_type.ToLower() == FACEBOOK || item.social_type.ToLower() == YOUTUBE)
            {
            }
            else
            {
                var twitterUser = item.user;
                var result = item.social_object_dict;
                if (item.created_at != null)
                {
                    String twitter = Helper.SocialToTimeAgo(Convert.ToDateTime(item.created_at));
                    if (twitter == null)
                    {
                        socialAllFeedsTimeTextView.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        socialAllFeedsTimeTextView.Text = (twitter);
                    }
                }
                else
                {
                    socialAllFeedsTimeTextView.Visibility = ViewStates.Gone;
                }

                setMessage(mContext, message, item.content_text);
                socialIconImageView.SetImageResource(Resource.Drawable.ic_twitter);
                if (twitterUser != null)
                {
                    if (twitterUser.screen_name != null)
                    {
                        socialHandleTextView.Text = string.Format("@{0}", twitterUser.screen_name);
                    }
                }
                userNameTextView.Text = twitterUser.name;
                if (twitterUser != null)
                {
                    if (twitterUser.profile_image_url != null)
                    {
                        UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(userImageView, twitterUser.profile_image_url, Resource.Drawable.ic_unknown);
                    }
                }

                LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
                View view = inflater.Inflate(Resource.Layout.view_bottom_twitter_actions, null);
                actionContainer.RemoveAllViews();
                actionContainer.AddView(view);



                if (result.ContainsKey("retweet_count") && result["retweet_count"].ToString() != null)
                {
                    var text = result["retweet_count"].ToString();
                    var temp = Convert.ToInt32(text);
                    temp++;
                    ((TextView)view.FindViewById(Resource.Id.socialAllFeeds_twitter_action_retweet)).Text = ("" + text);
                }


                if (result.ContainsKey("favorite_count") && result["favorite_count"].ToString() != null)
                {
                    var text = result["favorite_count"].ToString();
                    var temp = Convert.ToInt32(text);
                    temp++;
                    ((TextView)view.FindViewById(Resource.Id.socialAllFeeds_twitter_action_favorite)).Text = ("" + Convert.ToString(temp));
                }
                if (item.entities != null)
                {
                    if (item.entities.media != null && item.entities.media.Count > 0)
                    {
                        imageFeedContainer.Visibility = ViewStates.Visible;
                        UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(feedImageView, item.entities.media[0].media_url, Resource.Drawable.ic_unknown);
                    }
                    else
                    {
                        imageFeedContainer.Visibility = ViewStates.Gone;
                    }

                }
            }

            Linkify.AddLinks(message, MatchOptions.All);
            moreAction.Click += (s, e) =>
                {
                    showMoreAction(item, position);
                };
        }

        private void setMessage(Context context, TextView textView, String message)
        {
            textView.SetTextColor(Color.Black);
            if (message != null)
            {
                String[] str_array = message.Split(' ');
                SpannableStringBuilder ss = new SpannableStringBuilder(message);
                for (int i = 0; i < str_array.Length; i++)
                {
                    if (str_array[i].StartsWith("@"))
                    {
                        ss.SetSpan(new CustomSpan(context, ""), message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length), 0);
                        ss.SetSpan(new ForegroundColorSpan(Color.Blue), message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length),
                            SpanTypes.ExclusiveExclusive);

                        NoUnderlineSpan noUnderline = new NoUnderlineSpan();
                        ss.SetSpan(noUnderline, message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length), 0);
                    }

                    if (str_array[i].Contains("#"))
                    {
                        ss.SetSpan(new ForegroundColorSpan(context.Resources.GetColor(Resource.Color.blue)), str_array[i].IndexOf("#"), message.IndexOf(str_array[i]) + (str_array[i].Length),
                                SpanTypes.ExclusiveExclusive);

                        int j;
                        j = i;
                        ss.SetSpan(new CustomSpan(context, ""), message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length), SpanTypes.ExclusiveExclusive);

                    }


                    if (Patterns.WebUrl.Matcher(str_array[i]).Matches())
                    {
                        int j;
                        j = i;
                        ss.SetSpan(new CustomSpan(context, str_array[j]), message.IndexOf(str_array[i], i), message.IndexOf(str_array[i], i) + (str_array[i].Length), 0);
                    }
                }
                textView.Text = (ss.ToString());
                textView.MovementMethod = (LinkMovementMethod.Instance);

            }
        }

        private void showMoreAction(BuiltTwitter model, int position)
        {

            Android.Support.V4.App.FragmentManager fm = ((MainActivity)mContext).SupportFragmentManager;
            SocialMoreActionDialogFragment testDialog = new SocialMoreActionDialogFragment();

            Bundle bundle = new Bundle();
            if (model != null)
            {
                var test = JsonConvert.SerializeObject(model, new JsonSerializerSettings() { ReferenceLoopHandling=ReferenceLoopHandling.Ignore});
                bundle.PutString("currentTwitterDataModel", test);
                //bundle.PutSerializable("model", model);
            }
            
            bundle.PutString("postMessage", postToMessage);
            bundle.PutString("emailSubject", "");
            bundle.PutInt("position", position);
            testDialog.Arguments=(bundle);
            testDialog.RetainInstance=(true);
            testDialog.Show(fm, "fragment_name");
        }

        public class CustomSpan : Android.Text.Style.ClickableSpan
        {
            string str;
            Context context;
            public CustomSpan(Context context, string str)
            {
                this.str = str;
            }
            public override void OnClick(View widget)
            {
                String url = null;
                if (!str.ToLower().Contains("http") && !str.ToLower().Contains("https"))
                {
                    url = "http://" + str;
                }
                else
                {
                    url = str;
                }
                Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                (context).StartActivity(browserIntent);
            }
        }

        class NoUnderlineSpan : UnderlineSpan
        {
            public NoUnderlineSpan()
            {

            }

            public NoUnderlineSpan(Parcel src)
            {

            }

            public override void UpdateDrawState(TextPaint ds)
            {
                ds.UnderlineText=(false);
            }
        }

    }
}
