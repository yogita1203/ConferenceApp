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
using CommonLayer.Entities.Built;
using Android.Text;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Util;
using ConferenceAppDroid.BroadcastReceivers;

namespace ConferenceAppDroid.Utilities
{
    public class ExploreHeaderComponent
    {
        private Context context;
        public View view;
        private TextView sponsorslabel_TextView;
        private TextView trackslabel_TextView;
        private TextView expolre_intro_TextView, explore_now_TextView;
        private float dpWidth;
        private HorizontalScrollView tracksHorizontalScrollView;
        public static String NEXTUP_SESSION_LIST = "NEXTUP_SESSION_LIST";
        public static String CURRENT_SESSION_LIST = "CURRENT_SESSION_LIST";
        private Handler handler = new Handler();
        public TextView popularSessionlabelTextView;
        public View getView()
        {
            return view;
        }
        public ExploreHeaderComponent(Context context)
        {
            this.context = context;
            view = View.Inflate(this.context, Resource.Layout.view_header_explore_screen, null);
            sponsorslabel_TextView = (TextView)view.FindViewById(Resource.Id.sponsorslabelTextView);
            expolre_intro_TextView = (TextView)view.FindViewById(Resource.Id.exploreIntro_textview_intro);
            trackslabel_TextView = (TextView)view.FindViewById(Resource.Id.trackslabelTextView);
            explore_now_TextView = (TextView)view.FindViewById(Resource.Id.explore_now_label);
            popularSessionlabelTextView = (TextView)view.FindViewById(Resource.Id.popularSessionlabelTextView);

        }

        public void hideSponsorsHeaderScrollView()
        {
            view.FindViewById(Resource.Id.sponsorsHorizontalScrollView).Visibility = ViewStates.Gone;
            sponsorslabel_TextView.Visibility = ViewStates.Gone;
        }

        public void showSponsorsHeaderScrollView(List<String> sponsorLevels)
        {

            HorizontalScrollView sponsorsHorizontalScrollView = (HorizontalScrollView)view.FindViewById(Resource.Id.sponsorsHorizontalScrollView);
            sponsorsHorizontalScrollView.Visibility = ViewStates.Visible;
            sponsorslabel_TextView.Visibility = ViewStates.Visible;

            if (sponsorsHorizontalScrollView.ChildCount > 0)
            {
                sponsorsHorizontalScrollView.RemoveAllViews();
            }
            LinearLayout linearView = new LinearLayout(context);
            linearView.Orientation = Orientation.Horizontal;
            if (sponsorLevels != null)
            {
                int count = sponsorLevels.Count;
                for (int i = 0; i < count; i++)
                {
                    int position = i;
                    View view1 = LayoutInflater.From(context).Inflate(Resource.Layout.view_horizontal_scroll_view, null);

                    ShapeDrawable footerBackground = new ShapeDrawable(new RectShape());

                    float[] radii = new float[8];
                    radii[0] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                    radii[1] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                    radii[2] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                    radii[3] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                    radii[4] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                    radii[5] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                    radii[6] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                    radii[7] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                    footerBackground.Shape = new RoundRectShape(radii, null, null);
                    footerBackground.Paint.Color = Android.Graphics.Color.ParseColor("#14417D");

                    TextView nameTextView = (TextView)view1.FindViewById(Resource.Id.nameTextView);
                    nameTextView.SetBackgroundDrawable(footerBackground);

                    nameTextView.Text = sponsorLevels[position].ToString();
                    view1.Click += (s, e) =>
                        {
                            Console.WriteLine();
                        };


                    if ((i == (sponsorLevels.Count - 1)))
                    {
                        view1.SetPadding(0, 0, Helper.dpToPx(context, 10), 0);
                    }
                    linearView.AddView(view1);
                }

                if (linearView.ChildCount > 0)
                {
                    sponsorsHorizontalScrollView.AddView(linearView);
                }
            }
            //


        }

        public void showIntro(BuiltIntro introModel)
        {
            RelativeLayout bgContainer = (RelativeLayout)this.view.FindViewById(Resource.Id.exploreIntro_container);
            ImageView coverImage = (ImageView)this.view.FindViewById(Resource.Id.exploreIntro_imageview_coverImage);
            //        TextView introContent = (TextView) this.view.findViewById(R.id.exploreIntro_textview_intro);

            bgContainer.Visibility = ViewStates.Visible; ;


            if (introModel != null)
            {
                UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(coverImage, introModel.bg_image.url, Resource.Drawable.ic_default_pic);
                if (!string.IsNullOrWhiteSpace(introModel.bg_color))
                {
                    bgContainer.SetBackgroundColor(Color.ParseColor("#" + introModel.bg_color));
                }
                else
                {
                    bgContainer.SetBackgroundColor(Color.Transparent);
                }
                /*RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WRAP_CONTENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
                params.addRule(RelativeLayout.TEXT_ALIGNMENT_GRAVITY , );*/
                //            introContent.setGravity(Gravity.CENTER_VERTICAL);
                //            introContent.setGravity(Gravity.CENTER_HORIZONTAL);
                //            introContent.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_BOLD));

                if (introModel.text_color != null && introModel.text_color.Trim().Length > 0)
                {
                    if (introModel.text_color.Contains("#"))
                    {
                        expolre_intro_TextView.SetTextColor(Color.ParseColor(introModel.text_color));
                    }
                    else
                    {
                        expolre_intro_TextView.SetTextColor(Color.ParseColor("#" + introModel.text_color));
                    }
                }

                String intro = Html.FromHtml(introModel.desc.ToString().Trim()).ToString();

                expolre_intro_TextView.Text = Html.FromHtml(intro.Trim()).ToString();

                //CustomLinkMovementMethod customLinkMovementMethod = (CustomLinkMovementMethod)CustomLinkMovementMethod.Instance;
                //customLinkMovementMethod.movementContext = context;
                //expolre_intro_TextView.MovementMethod=customLinkMovementMethod;

                //            introContent.setMovementMethod(CustomLinkMovementMethod.getInstance(context));
            }
        }

        public void hideIntro()
        {
            this.view.FindViewById(Resource.Id.exploreIntro_container).Visibility = ViewStates.Gone;
        }

        public void showTracksHeaderScrollView(List<BuiltTracks> tracksList)
        {

            view.FindViewById(Resource.Id.tracksHorizontalScrollView).Visibility = ViewStates.Visible;
            trackslabel_TextView.Visibility = ViewStates.Visible;
            tracksHorizontalScrollView = (HorizontalScrollView)view.FindViewById(Resource.Id.tracksHorizontalScrollView);
            if (tracksHorizontalScrollView.ChildCount > 0)
            {
                tracksHorizontalScrollView.RemoveAllViews();
            }
            LinearLayout linearView = new LinearLayout(context);

            if (tracksList != null)
            {

                int count = tracksList.Count;
                for (int i = 0; i < count; i++)
                {
                    BuiltTracks trackModel = tracksList[i];

                    if (trackModel.name != null && !(trackModel.name.Trim().Equals("NoTrack", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        //                    View view = LayoutInflater.from(context).inflate(R.layout.view_sponsor_horizontal_scroll_view, null);
                        View view1 = LayoutInflater.From(context).Inflate(Resource.Layout.view_horizontal_scroll_view, null);

                        if ((i == (tracksList.Count - 1)))
                        {
                            view1.SetPadding(0, 0, Helper.dpToPx(context, 10), 0);
                        }

                        TextView nameTextView = (TextView)view1.FindViewById(Resource.Id.nameTextView);




                        ShapeDrawable footerBackground = new ShapeDrawable(new RectShape());

                        float[] radii = new float[8];
                        radii[0] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                        radii[1] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                        radii[2] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                        radii[3] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                        radii[4] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                        radii[5] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);

                        radii[6] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                        radii[7] = context.Resources.GetDimension(Resource.Dimension.explore_track_corners_for_pex);
                        //if (AppConstants.IS_PEX_EVENT) {

                        //    radii[0] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);
                        //    radii[1] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);

                        //    radii[2] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);
                        //    radii[3] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);

                        //    radii[4] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);
                        //    radii[5] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);

                        //    radii[6] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);
                        //    radii[7] = context.getResources().getDimension(R.dimen.explore_track_corners_for_pex);

                        //} else {

                        //    radii[0] = context.getResources().getDimension(R.dimen.explore_track_corners);
                        //    radii[1] = context.getResources().getDimension(R.dimen.explore_track_corners);

                        //    radii[2] = context.getResources().getDimension(R.dimen.explore_track_corners);
                        //    radii[3] = context.getResources().getDimension(R.dimen.explore_track_corners);

                        //    radii[4] = context.getResources().getDimension(R.dimen.explore_track_corners);
                        //    radii[5] = context.getResources().getDimension(R.dimen.explore_track_corners);

                        //    radii[6] = context.getResources().getDimension(R.dimen.explore_track_corners);
                        //    radii[7] = context.getResources().getDimension(R.dimen.explore_track_corners);
                        //}


                        footerBackground.Shape = new RoundRectShape(radii, null, null);


                        try
                        {
                            String name;
                            Color color = Color.ParseColor("#242424");
                            if (trackModel.color != null && trackModel.color.Length > 0)
                            {
                                if (trackModel.color.Contains("#"))
                                {
                                    color = Color.ParseColor(trackModel.color);
                                }
                                else
                                {
                                    color = Color.ParseColor("#" + trackModel.color);
                                }
                            }

                            footerBackground.Paint.Color = color;
                            nameTextView.SetBackgroundDrawable(footerBackground);

                            nameTextView.Text = trackModel.name;
                            view1.Click += (s, e) =>
                            {
                                Intent intent = new Intent(ExploreTrackReceiver.action);
                                intent.PutExtra("track_name", trackModel.name);
                                ((MainActivity)context).exploreTrack = trackModel.name;
                                ((MainActivity)context).showSessionFragment();
                            };
                        }
                        catch (Exception e)
                        {
                        }

                        linearView.AddView(view1);



                        /*RelativeLayout.LayoutParams params = (RelativeLayout.LayoutParams) view.getLayoutParams();
                        params.setMargins(10,10,10,10);
                        view.setLayoutParams(params);*/
                    }
                }
                if (linearView.ChildCount > 0)
                {
                    tracksHorizontalScrollView.AddView(linearView);
                }

            }
        }

        //     public void showOnNowAndNextUpPagerHeaderView(List<BuiltSessionTime> sessionOnNowTimeModelList,  List<BuiltSessionTime> sessionNextUpTimeModelList) 
        //     {

        //    HorizontalScrollView exploreNowHorizontalScrollView = (HorizontalScrollView) view.FindViewById(Resource.Id.exploreNowHorizontalScrollView);
        //    HorizontalScrollView exploreNextUpHorizontalScrollView = (HorizontalScrollView) view.FindViewById(Resource.Id.exploreNextUpHorizontalScrollView);

        //    ((TextView) view.FindViewById(Resource.Id.explore_next_up_label)).SetTypeface(Typeface.CreateFromAsset(context.Assets, AppConstants.FONT));
        //    explore_now_TextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
        //    RelativeLayout nowContainer = (RelativeLayout) view.FindViewById(Resource.Id.nowContainer);
        //    RelativeLayout nextContainer = (RelativeLayout) view.FindViewById(Resource.Id.nextContainer);

        //    nowContainer.Visibility=ViewStates.Gone;
        //    nextContainer.Visibility=ViewStates.Gone;


        //    Display display = ((Android.App.Activity) context).WindowManager.DefaultDisplay;
        //    DisplayMetrics outMetrics = new DisplayMetrics();
        //    display.GetMetrics(outMetrics);

        //    float density = context.Resources.DisplayMetrics.Density;
        //    dpWidth = outMetrics.WidthPixels / density;

        //    if (dpWidth != 0 && dpWidth > 400) {
        //        dpWidth = dpWidth - 80;
        //    }

        //    if (sessionOnNowTimeModelList != null && sessionOnNowTimeModelList.Count > 0) {

        //        showOnNowAndNextUp(exploreNowHorizontalScrollView, sessionOnNowTimeModelList, AppConstants.CURRENT_SESSION_LIST);

        //        nowContainer.Visibility=ViewStates.Visible;

        //    } else {
        //        nowContainer.Visibility=ViewStates.Gone;
        //    }

        //    if (sessionNextUpTimeModelList != null && sessionNextUpTimeModelList.Count> 0) {
        //        showOnNowAndNextUp(exploreNextUpHorizontalScrollView, sessionNextUpTimeModelList, AppConstants.NEXTUP_SESSION_LIST);
        //        nextContainer.Visibility=ViewStates.Visible;
        //    } else {
        //        nextContainer.Visibility=ViewStates.Gone;
        //    }
        //}

        public void showOnNowAndNextUpPagerHeaderView(List<BuiltSessionTime> sessionOnNowTimeModelList, List<BuiltSessionTime> sessionNextUpTimeModelList)
        {
            HorizontalScrollView exploreNowHorizontalScrollView = (HorizontalScrollView)view.FindViewById(Resource.Id.exploreNowHorizontalScrollView);
            HorizontalScrollView exploreNextUpHorizontalScrollView = (HorizontalScrollView)view.FindViewById(Resource.Id.exploreNextUpHorizontalScrollView);

            // ((TextView) view.findViewById(R.id.explore_next_up_label)).setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //explore_now_TextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            RelativeLayout nowContainer = (RelativeLayout)view.FindViewById(Resource.Id.nowContainer);
            RelativeLayout nextContainer = (RelativeLayout)view.FindViewById(Resource.Id.nextContainer);

            nowContainer.Visibility = ViewStates.Gone;
            nextContainer.Visibility = ViewStates.Gone;

            Display display = ((Android.App.Activity)context).WindowManager.DefaultDisplay;
            DisplayMetrics outMetrics = new DisplayMetrics();
            display.GetMetrics(outMetrics);

            float density = context.Resources.DisplayMetrics.Density;
            dpWidth = outMetrics.WidthPixels / density;

            if (dpWidth != 0 && dpWidth > 400)
            {
                dpWidth = dpWidth - 80;
            }

            if (sessionOnNowTimeModelList != null && sessionOnNowTimeModelList.Count > 0)
            {

                showOnNowAndNextUp(exploreNowHorizontalScrollView, sessionOnNowTimeModelList, CURRENT_SESSION_LIST);

                nowContainer.Visibility = ViewStates.Visible;

            }
            else
            {
                nowContainer.Visibility = ViewStates.Gone;
            }

            if (sessionNextUpTimeModelList != null && sessionNextUpTimeModelList.Count > 0)
            {
                showOnNowAndNextUp(exploreNextUpHorizontalScrollView, sessionNextUpTimeModelList, NEXTUP_SESSION_LIST);
                nextContainer.Visibility = ViewStates.Visible;
            }
            else
            {
                nextContainer.Visibility = ViewStates.Gone;
            }

        }

        private void showOnNowAndNextUp(HorizontalScrollView exploreNowHorizontalScrollView, List<BuiltSessionTime> sessionOnNowTimeModelList, string CURRENT_SESSION_LIST)
        {
            if (exploreNowHorizontalScrollView.ChildCount > 0)
            {
                exploreNowHorizontalScrollView.RemoveAllViews();
            }

            LinearLayout linearView = new LinearLayout(context);
            LinearLayout.LayoutParams paramss = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            paramss.RightMargin = 2;
            linearView.SetPadding(Helper.dpToPx(context, 2), Helper.dpToPx(context, 2), Helper.dpToPx(context, 2), Helper.dpToPx(context, 2));
            linearView.LayoutParameters = (paramss);


                int count = sessionOnNowTimeModelList.Count;
                for (int i = 0; i < count; i++)
                {
                    int position = i;
                    BuiltSessionTime sessionOnNowTimeModel = sessionOnNowTimeModelList[i];

                    View rowExploreTopNow = (View)LayoutInflater.From(context).Inflate(Resource.Layout.row_explore_top_now_design, null);
                    TextView nowSessionTitle = (TextView)rowExploreTopNow.FindViewById(Resource.Id.explore_top_now_session_title_tv);
                    TextView nowSessionTime = (TextView)rowExploreTopNow.FindViewById(Resource.Id.explore_top_now_session_time_tv);
                    TextView nowSessionRoom = (TextView)rowExploreTopNow.FindViewById(Resource.Id.explore_top_now_session_room_tv);
                    ImageView nowNextTrackColor = (ImageView)rowExploreTopNow.FindViewById(Resource.Id.now_next_track_color);



                    RelativeLayout.LayoutParams viewParams = new RelativeLayout.LayoutParams(Helper.dpToPx(context, (int)dpWidth), ViewGroup.LayoutParams.WrapContent);

                    if (sessionOnNowTimeModel.BuiltSession.track != null)
                    {
                        try
                        {
                            if (sessionOnNowTimeModel.BuiltSession.track.Contains("#"))
                            {
                                nowNextTrackColor.SetBackgroundColor(Color.ParseColor(sessionOnNowTimeModel.BuiltSession.track));
                            }
                            else
                            {
                                nowNextTrackColor.SetBackgroundColor(Color.ParseColor("#" + sessionOnNowTimeModel.BuiltSession.track));
                            }
                        }
                        catch (Exception e)
                        {
                            nowNextTrackColor.SetBackgroundColor(Color.ParseColor("#" + AppConstants.DEFAULT_TRACKS_COLOR));

                        }
                    }
                    else
                    {
                        nowNextTrackColor.SetBackgroundColor(Color.ParseColor("#" + AppConstants.DEFAULT_TRACKS_COLOR));
                    }
                    /* nowSessionTitle.setLayoutParams(viewParams);*/
                    //nowSessionTitle.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
                    //nowSessionTime.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
                    //nowSessionRoom.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

                    nowSessionTitle.Text = (sessionOnNowTimeModel.BuiltSession.title);
                    nowSessionRoom.Text = (sessionOnNowTimeModel.room);
                    nowSessionTime.Text = Helper.convertToDate(sessionOnNowTimeModel.date) + ", " + convertToStartDate(sessionOnNowTimeModel.time) + " - " + convertToEndDate(sessionOnNowTimeModel.time, sessionOnNowTimeModel.length);

                    //                    rowExploreTopNow.setOnClickListener(new OnClickListener() {
                    //                        @Override
                    //                        public void onClick(View arg0) {
                    ////                    Intent intent = new Intent(context, UISessionDetailScreen.class);
                    //                            Intent intent = new Intent(context, UISessionDayDetail.class);
                    //                            intent.putExtra("id", sessionTimeModel._id);
                    //                            intent.putExtra("from", controller);
                    //                            intent.putExtra("position", position);

                    //                            HashMap<String, Object> map = new HashMap<String, Object>();
                    //                            map.put("session_time_id", sessionTimeModel._id);
                    //                            AppUtilities.sendAnalyticsCall(context, "explore_ongoing_or_nextup_session", "ExploreHeaderComponent", map);


                    //                  /*  hashMapMyPagerAdapter.clear();
                    //                    hashMapMyPagerAdapter.put(sessionTimeModel._id, sessionTimeModels);*/
                    //                            context.startActivity(intent);
                    //                        }
                    //                    });


                    linearView.AddView(rowExploreTopNow);
                }

                if (linearView.ChildCount > 0)
                {
                    exploreNowHorizontalScrollView.AddView(linearView);
                }

        }


        public void hideOnNowAndNextUpPagerHeaderView()
        {
            this.view.FindViewById(Resource.Id.nowContainer).Visibility = ViewStates.Gone;
            this.view.FindViewById(Resource.Id.nextContainer).Visibility = ViewStates.Gone;
        }
        public void hideTracksHeaderScrollView()
        {
            view.FindViewById(Resource.Id.tracksHorizontalScrollView).Visibility = ViewStates.Gone;
            trackslabel_TextView.Visibility = ViewStates.Gone;
        }


        public string convertToStartDate(string time)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(time);
                return Helper.ToDateTimeString(date, "hh:mm tt").ToLower();
            }
            catch
            {
                return String.Empty;
            }
        }

        public string convertToEndDate(string time, string length)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                var endDate = DateTime.Parse(time).AddMinutes(Convert.ToInt32(length));
                return Helper.ToDateTimeString(endDate, "hh:mm tt").ToLower();
            }
            catch
            {
                return String.Empty;
            }
        }

    }
}