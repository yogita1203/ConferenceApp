using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Utilities;
using Android.Text.Style;
using Android.Graphics;
using Android.Text;
using Android.Text.Method;
using ConferenceAppDroid.Interfaces;
using System.Threading.Tasks;
using CommonLayer;
using ConferenceAppDroid.Activities;

namespace ConferenceAppDroid.Fragments
{
    public class SponsorDetailFragment : Android.Support.V4.App.Fragment
    {
        private Context context;
        private String id;
        private String from;
        private int position;
        private long totalCount;
        private View parentView;
        //private AppUtilities appUtilities;
        private int mShortAnimationDuration;
        private String searchText;
        private ILoadFragment loadFragment;
        List<BuiltExhibitor> sponsorList = new List<BuiltExhibitor>();
        private void initializeValues(String id, int sponsorPosition, String fromScreen, long countTotal, List<BuiltExhibitor> sponsorList, String sponsorSearchText)
        {
            this.id = id;
            this.from = fromScreen;
            this.position = sponsorPosition;
            this.sponsorList = sponsorList;
            this.totalCount = countTotal;
            this.searchText = sponsorSearchText;

        }

        private void setContext(Context c)
        {
            context = c;
        }

        public static SponsorDetailFragment newInstance(Context context, int sponsorPosition, String fromScreen, String id, long countTotal, List<BuiltExhibitor> sponsorList, String sponsorSearchText)
        {
            SponsorDetailFragment fragment = new SponsorDetailFragment();
            fragment.initializeValues(id, sponsorPosition, fromScreen, countTotal, sponsorList, sponsorSearchText);
            fragment.setContext(context);
            return fragment;
        }
        public SponsorDetailFragment()
        {
            // Required empty public constructor
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = (View)LayoutInflater.From(context).Inflate(Resource.Layout.view_sponsors_detail_pager_adapter, null);
            //appUtilities = new AppUtilities(context);
            mShortAnimationDuration = 1800;
            crossFade();

            var currentItem=sponsorList[position];
            setData(currentItem);
            loadFragment.loadFragment(true, "", currentItem);

           //FetchSpecificSponsorAsync().ContinueWith(t =>
           //     {
           //         if (t.Result != null)
           //         {
           //             setData(t.Result);
           //             loadFragment.loadFragment(true, "", t.Result);
           //         }
                
           //     });
            //new FetchSpecificSponsorAsync().execute();

            return parentView;
        }

        private void crossFade()
        {

            parentView.Alpha = (0f);
            parentView.Visibility = ViewStates.Visible;

            parentView.Animate()
                    .Alpha(1f)
                    .SetDuration(mShortAnimationDuration)
                    .SetListener(null);

        }

        private void setData(BuiltExhibitor sponsorsTimeModel)
        {
            ImageView sponsorsDetailImageView = (ImageView)parentView.FindViewById(Resource.Id.sponsorsDetailImageView);
            TextView sponsorsNameTextView = (TextView)parentView.FindViewById(Resource.Id.sponsorsNameTextView);
            TextView sponsorsDetailTextView = (TextView)parentView.FindViewById(Resource.Id.sponsorsDetailTextView);
            TextView sponsorsTypeNameTextView = (TextView)parentView.FindViewById(Resource.Id.sponsorsTypeNameTextView);
            TextView sponsorsWebsiteTextView = (TextView)parentView.FindViewById(Resource.Id.sponsorsDetailWebSiteTextView);
            TextView sponsorsBoothTextView = (TextView)parentView.FindViewById(Resource.Id.sponsorsBoothTextView);

            //sponsorsNameTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //sponsorsDetailTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //sponsorsTypeNameTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //sponsorsBoothTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //sponsorsWebsiteTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));


            if (sponsorsTimeModel.company_description != null)
            {
                setMessage(context, sponsorsDetailTextView, sponsorsTimeModel.company_description);
            }
            if (!string.IsNullOrWhiteSpace(sponsorsTimeModel.booth))
            {
                sponsorsBoothTextView.Text = (context.Resources.GetString(Resource.String.booth_text) + ": " + sponsorsTimeModel.booth);
            }
            else
            {
                sponsorsBoothTextView.Visibility = ViewStates.Gone;
            }

            if (!string.IsNullOrWhiteSpace(sponsorsTimeModel.url))
            {
                sponsorsWebsiteTextView.Visibility = ViewStates.Visible;
                sponsorsWebsiteTextView.Click += (s, e) =>
                    {
                        try
                        {

                            String HTTPS = "https://";
                            String HTTP = "http://";

                            if (!sponsorsTimeModel.url.StartsWith(HTTP) && !sponsorsTimeModel.url.StartsWith(HTTPS))
                            {
                                sponsorsTimeModel.url = HTTP + sponsorsTimeModel.url;
                            }

                            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(sponsorsTimeModel.url));
                            context.StartActivity(Intent.CreateChooser(intent, "Choose Browser"));

                        }
                        catch (Exception t)
                        {
                        }
                    };

            }
            else
            {
                sponsorsWebsiteTextView.Visibility = ViewStates.Gone;
            }


            if (!string.IsNullOrWhiteSpace(sponsorsTimeModel.type))
            {

                String[] splited = sponsorsTimeModel.type.Split(' ');
                String headerString = "";

                for (int i = 0; i < splited.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(splited[i]))
                    {
                        headerString = headerString + " " + AppUtilities.UppercaseFirst(splited[i]);
                    }
                }
                sponsorsTypeNameTextView.Text = ("Type: " + headerString.Trim());

            }
            string url = string.Empty;
            if (sponsorsTimeModel.exhibitor_file != null && sponsorsTimeModel.exhibitor_file.Count > 0)
            {
                url=sponsorsTimeModel.exhibitor_file[0].url;
                 if (!string.IsNullOrWhiteSpace(url))
                 {
                     UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(sponsorsDetailImageView,url, Resource.Drawable.ic_default_pic);
                 }
                
            }

            
            //Picasso.with(context)
            //        .load(appUtilities.getImageUrlForSponsorAndExhibitor(sponsorsTimeModel))
            //        .fit()
            //        .noFade()
            //        .centerInside()
            //        .placeholder(R.drawable.ic_default_pic)
            //        .error(R.drawable.ic_default_pic)
            //        .into(sponsorsDetailImageView);


        }

        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                setContext(activity);
                loadFragment = (ILoadFragment)activity;
            }
            catch (Exception e)
            {
            }
        }

        public class NoUnderlineSpan : UnderlineSpan
        {
            public NoUnderlineSpan()
            {
            }

            public NoUnderlineSpan(Parcel src)
            {
            }

            public override void UpdateDrawState(Android.Text.TextPaint ds)
            {
                ds.UnderlineText = (false);
            }

        }

        Task<BuiltExhibitor> FetchSpecificSponsorAsync()
        {
            return Task.Run<BuiltExhibitor>(() => 
            {
                BuiltExhibitor sponsorModel = null;
                if (from != null && from.Equals(AppConstants.SPONSORS_BY_CONTRIBUTION,StringComparison.InvariantCultureIgnoreCase))
                {
                    //                sponsorModelList = SponsorByContribution.hashMap.get(id);
                    sponsorModel = DataManager.GetExhibitorFromId(DBHelper.Instance.Connection, id).Result;
                }
                else if (from != null && from.Equals(AppConstants.APPUTILITIES,StringComparison.InvariantCultureIgnoreCase))
                {
                    //List<BuiltExhibitor> sponsorModelList = qrm.getExhibitorById(id);
                    //if (sponsorModelList != null && sponsorModelList.size() > 0)
                    //{
                    //    sponsorModel = sponsorModelList.get(0);
                    //}
                }
                return sponsorModel;
            });
        }

        public class CustomSpan : ClickableSpan
        {
            string url;
            int urlLength;
            Android.App.Activity context;
            public CustomSpan(string url, int urlLength, Android.App.Activity context)
            {
                this.url = url;
                this.urlLength = urlLength;
                this.context = context;
            }
            public override void OnClick(View widget)
            {
                try
                {
                    if (url.EndsWith("."))
                    {
                        String newUrl = url.Substring(0, (urlLength - 1));

                        if (newUrl.Contains("http") || newUrl.Contains("https"))
                        {
                            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(newUrl));
                            context.StartActivity(Intent.CreateChooser(intent, "Choose Browser"));
                        }
                        else
                        {
                            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://" + newUrl));
                            context.StartActivity(Intent.CreateChooser(intent, "Choose Browser"));
                        }
                    }
                    else
                    {
                        if (url.Contains("http") || url.Contains("https"))
                        {
                            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                            context.StartActivity(Intent.CreateChooser(intent, "Choose Browser"));
                        }
                        else
                        {
                            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://" + url));
                            context.StartActivity(Intent.CreateChooser(intent, "Choose Browser"));
                        }
                    }

                }
                catch (Exception e)
                {
                }
            }
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
                    if (str_array[i].StartsWith("www") || str_array[i].StartsWith("http") || str_array[i].StartsWith("https"))
                    {
                        String url = str_array[i];
                        int urlLength = str_array[i].Length;

                        ss.SetSpan(new CustomSpan(url, urlLength, Activity), message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length), 0);

                        ss.SetSpan(new ForegroundColorSpan(Color.Blue), message.IndexOf(str_array[i]), message.IndexOf(str_array[i]) + (str_array[i].Length),
                               SpanTypes.ExclusiveExclusive);

                    }

                }
                textView.Text = (ss.ToString());
                textView.MovementMethod = (LinkMovementMethod.Instance);
            }
            else
            {
                textView.Visibility = ViewStates.Gone;
            }
        }

    }

}