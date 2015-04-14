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

using Com.Tonicartos.Widget.Stickygridheaders;
using CommonLayer.Entities.Built;
using ConferenceAppDroid.Utilities;
using Android.Views.InputMethods;
using CommonLayer;
using ConferenceAppDroid.Activities;

namespace ConferenceAppDroid.Fragments
{
    public class SponsorByContribution : Android.Support.V4.App.Fragment
    {
        private Context context;
        private Com.Tonicartos.Widget.Stickygridheaders.StickyGridHeadersGridView gridView;
        private SponsorsByContributionSectionGridAdapter sponsorAdapter;
        private List<BuiltExhibitor> sponsorsList;
        private List<String> sponsorsHeader;
        private List<int> sponsorsHeaderTypeCount;
        private int hiddenSponsorCount = 0;

        private View parentView;
        private EditText sponsor_search_edit_text;
        private Button sponsor_search_cancel_btn;
        private Button sponsor_search_btn;
        private String sponsorSearchText;
        private RelativeLayout sponsorSearchContainer;
        private int headerSize = 0;
        private String fromExploreString;
        private int clickedLevelTypeCount = 0;

    //    public Android.Support.V4.App.Fragment newInstance(String str) 
    //{
    //    SponsorByContribution f = new SponsorByContribution();
    //    Bundle b = new Bundle();
    //    b.PutString("name", strings);

    //    f.Arguments=(b);

    //    return f;
    //}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.fragment_sponsors_by_contributors, container, false);
            sponsor_search_edit_text = (EditText)parentView.FindViewById(Resource.Id.sponsor_search_edit_text);
            sponsor_search_cancel_btn = (Button)parentView.FindViewById(Resource.Id.sponsor_search_cancel_btn);
            sponsor_search_btn = (Button)parentView.FindViewById(Resource.Id.sponsor_search_btn);
            sponsorSearchContainer = (RelativeLayout)parentView.FindViewById(Resource.Id.sponsor_search_container);
            Button sponsorSearchCancelContainerBtn = (Button)parentView.FindViewById(Resource.Id.sponsor_search_cancel_container_btn);
            this.context = Activity;
            sponsorSearchCancelContainerBtn.Click += (s, e) =>
                {
                    InputMethodManager imm = (InputMethodManager)context.GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(sponsor_search_edit_text.WindowToken, 0);

                    sponsorSearchContainer.Animate().Y(-0f);
                    sponsorSearchContainer.Visibility=ViewStates.Gone;
                    AppConstants.SEARCH_SPONSOR_SHOW_HIDE = false;

                    sponsor_search_edit_text.Text=(null);
                    sponsorSearchText = "";
                    sponsor_search_edit_text.ClearFocus();

                    sponsor_search_btn.Visibility=ViewStates.Visible;
                    sponsor_search_cancel_btn.Visibility = ViewStates.Gone;

                   // getDataAndShowGrid(null);
                };
            //sponsor_search_edit_text.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //sponsorSearchCancelContainerBtn.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            init();
            if (Arguments != null)
            {
                if (Arguments.ContainsKey("name"))
                {
                    if (Arguments.GetString("name").Equals("contribution", StringComparison.InvariantCultureIgnoreCase))
                    {

                    }
                    else
                    {
                        fromExploreString = Arguments.GetString("name");

                    }

                    //gridView.setOnItemClickListener(onItemClickLIstener);
                }
            }
            DataManager.GetExhibitorTypesByOrder(DBHelper.Instance.Connection).ContinueWith(t=>
                {
                     sponsorsHeader = t.Result;
                     headerSize = sponsorsHeader.Count;
                     sponsorsHeaderTypeCount = new List<int>();
                     for (int i = 0; i < headerSize; i++)
                     {
                         int count = i;
                         sponsorsHeaderTypeCount.Add(count);
                     }
            getAllExhibitorsByOrder(res => 
            {
                sponsorsList = res;
                Activity.RunOnUiThread(() => 
                {
                    sponsorAdapter = new SponsorsByContributionSectionGridAdapter(Activity, sponsorsList, sponsorsHeader, sponsorsHeaderTypeCount, 0, false,AppSettings.Instance.config);
                    gridView.Adapter = sponsorAdapter;
                });
            });
                });

            gridView.ItemClick += (s, e) =>
                {
                     BuiltExhibitor builtExhibitor = (BuiltExhibitor) sponsorsList[e.Position];
//            Intent intent = new Intent(getActivity(), UISponsorDetailScreen.class);
            Intent intent = new Intent(Activity,typeof(SponsorsDetail));
            intent.PutExtra("id", builtExhibitor.exhibitor_id);
            intent.PutExtra("from", AppConstants.SPONSORS_BY_CONTRIBUTION);
            intent.PutExtra("position", e.Position);
            intent.PutExtra("sponsor_search_text", sponsorSearchText);
//            hashMap.clear();
//            hashMap.put(sessionTimeModel._id, sponsorsList);
            StartActivity(intent);
                };
            return parentView; 

        }

        private void init()
        {
            gridView = parentView.FindViewById <Com.Tonicartos.Widget.Stickygridheaders.StickyGridHeadersGridView> (Resource.Id.gridView);
        }

        public void showSearchEditText(bool showSearch)
        {
            try
            {

                if (sponsorSearchContainer.Visibility== ViewStates.Visible)
                {
                    if (showSearch)
                    {
                        sponsorSearchContainer.Visibility = (ViewStates.Visible);
                        sponsorSearchContainer.Animate().Y(0f);
                    }
                    else
                    {

                        sponsorSearchContainer.Animate().Y(-0f);
                        sponsorSearchContainer.Visibility=(ViewStates.Gone);
                    }

                }
                else if (sponsorSearchContainer.Visibility == ViewStates.Gone)
                {
                    if (showSearch)
                    {
                        sponsorSearchContainer.Visibility=(ViewStates.Visible);
                        sponsorSearchContainer.Animate().Y(0f);
                    }
                    else
                    {

                        sponsorSearchContainer.Animate().Y(-0f);
                        sponsorSearchContainer.Visibility=ViewStates.Gone;
                    }
                }
            }
            catch (Exception e)
            {
            }

        }

        private void getAllExhibitorsByOrder(Action<List<BuiltExhibitor>> callback)
        {
                  DataManager.GetAllExhibitorsByOrderInAndroid(DBHelper.Instance.Connection).ContinueWith(t =>
                  {
                      var res = t.Result;
                      if (callback != null)
                      {
                          callback(res);
                      }
                  });
        }
    }

    public class SponsorsByContributionSectionGridAdapter : BaseAdapter<BuiltExhibitor>, Com.Tonicartos.Widget.Stickygridheaders.IStickyGridHeadersBaseAdapter
    {
        private bool isSearch;
        private Context context;
        private LayoutInflater inflater;
        private List<BuiltExhibitor> sponsorModelList;
        private List<String> sponsorHeaderList;
        private List<int> sponsorsHeaderTypeCount;
        private AppUtilities appUtilities;
        private int hiddenHeaderCount;
        ImageView sponsorsImageView;
        TextView sponsorsTextView;
        BuiltConfig config;

        public SponsorsByContributionSectionGridAdapter(Context context, List<BuiltExhibitor> sponsorModelList, List<String> sponsorsHeader, List<int> sponsorsHeaderTypeCount, int hiddenHeaderCount, bool isSearch,BuiltConfig config)
        {
            this.context = context;
            inflater = ((Android.App.Activity)context).LayoutInflater;
            this.sponsorModelList = sponsorModelList;
            this.sponsorHeaderList = sponsorsHeader;
            this.sponsorsHeaderTypeCount = sponsorsHeaderTypeCount;
            //appUtilities = new AppUtilities(context);
            this.hiddenHeaderCount = hiddenHeaderCount;
            this.isSearch = true;
            this.config = config;
        }
        public override int Count
        {
            get
            {
                return sponsorModelList.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position < sponsorModelList.Count)
            {
                BuiltExhibitor sponsorModel = sponsorModelList[position];
                SponsorByContributionViewHolder viewHolder;
                View view = convertView;
                if (null == convertView)
                {
                    view = inflater.Inflate(Resource.Layout.grid_row_sponsors_by_contribution, null);

                    sponsorsImageView = (ImageView)view.FindViewById(Resource.Id.sponsorsImageView);
                    sponsorsTextView = (TextView)view.FindViewById(Resource.Id.sponsorsTextView);
                    //viewHolder.sponsorsTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
                    viewHolder = new SponsorByContributionViewHolder(sponsorsImageView,sponsorsTextView);
                    view.Tag=(viewHolder);
                }
                else

                {
                    viewHolder=(SponsorByContributionViewHolder)view.Tag;
                    sponsorsImageView=viewHolder.sponsorsImageView;
                    sponsorsTextView = viewHolder.sponsorsTextView;
                }

                if (!string.IsNullOrWhiteSpace(sponsorModel.name))
                {
                    viewHolder.sponsorsTextView.Text=(sponsorModel.name);
                }

                 if (sponsorModel.type!= null && sponsorModel.type.Equals(AppConstants.MOBILE_LOGO,StringComparison.InvariantCultureIgnoreCase)) 
                 {
                        if (sponsorModel.url!= null ) 
                        {
                                UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(sponsorsImageView, sponsorModel.url, Resource.Drawable.ic_default_pic);
                        }
                 }
                 return view;
        }
            else
            {
                return null;
            }
        }

        public int GetCountForHeader(int p0)
        {
            return sponsorsHeaderTypeCount[p0];
        }

        public View GetHeaderView(int p0, View p1, ViewGroup p2)
        {
            TextView headerTextView;
            if (p1 == null)
            {
                p1 = inflater.Inflate(Resource.Layout.view_sponsor_contribution_header, p2, false);
            }

            if (isSearch)
            {

                if (sponsorHeaderList != null)
                {

                    String headerString = sponsorHeaderList[p0].ToString();


                    if (headerString != null)
                    {

                        bool isHidden = config.sponsors.ordering_info.Any(t=> (t.type.Equals(headerString,StringComparison.InvariantCultureIgnoreCase)));
                        if (!isHidden)
                        {

                            String[] splited = headerString.Split(' ');
                            headerString = "";

                            for (int i = 0; i < splited.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(splited[i]))
                                {
                                    headerString = headerString + " " + AppUtilities.UppercaseFirst(splited[i]);
                                }
                            }
                            headerTextView = ((TextView)p1.FindViewById(Resource.Id.headerTextView));
                            headerTextView.Text = (headerString);
                            //headerTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));

                            ViewGroup.LayoutParams paramss = p1.LayoutParameters;
                            paramss.Height = Helper.dpToPx(context, 35);
                            p1.LayoutParameters = (paramss);
                        }
                        else
                        {
                            ViewGroup.LayoutParams paramss = p1.LayoutParameters;
                            paramss.Height = Helper.dpToPx(context, 1);
                            p1.LayoutParameters = (paramss);
                        }
                    }
                    else
                    {
                        ViewGroup.LayoutParams paramss = p1.LayoutParameters;
                        paramss.Height = Helper.dpToPx(context, 1);
                        p1.LayoutParameters = (paramss);
                    }
                }
            }
                else
                {

                    if (hiddenHeaderCount > p0)
                    {
                        ViewGroup.LayoutParams paramss = p1.LayoutParameters;
                        paramss.Height = Helper.dpToPx(context, 1);
                        p1.LayoutParameters = (paramss);
                    }
                    else
                    {
                        if (sponsorHeaderList != null)
                        {

                            String headerString = sponsorHeaderList[p0].ToString();

                            if (headerString != null)
                            {

                                String[] splited = headerString.Split(' ');
                                headerString = "";


                                for (int i = 0; i < splited.Length; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(splited[i]))
                                    {
                                        headerString = headerString + " " + AppUtilities.UppercaseFirst(splited[i]);
                                    }
                                }
                                headerTextView = ((TextView)p1.FindViewById(Resource.Id.headerTextView));
                                headerTextView.Text = (headerString);
                                //headerTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));

                                ViewGroup.LayoutParams paramss = p1.LayoutParameters;
                                paramss.Height = Helper.dpToPx(context, 35);
                                p1.LayoutParameters = (paramss);
                            }
                        }

                    }

                }
            return p1;
         
        }

        public int NumHeaders
        {
            get { return sponsorHeaderList.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override BuiltExhibitor this[int position]
        {
            get 
            { 
            return    sponsorModelList[position]; 
            }
        }

        public class SponsorByContributionViewHolder : Java.Lang.Object
        {

            public ImageView sponsorsImageView;
            public TextView sponsorsTextView;

            public SponsorByContributionViewHolder(ImageView sponsorsImageView, TextView sponsorsTextView)
            {
                this.sponsorsImageView = sponsorsImageView;
                this.sponsorsTextView = sponsorsTextView;
            }

        }
    }

}