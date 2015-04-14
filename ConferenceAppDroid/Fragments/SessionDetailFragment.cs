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
using CommonLayer;
using ConferenceAppDroid.Utilities;
using SQLiteNetExtensions.Extensions;
using CommonLayer.Entities.Built;
using Android.Util;
using System.Threading.Tasks;
using Android.Graphics;
using ConferenceAppDroid.Activities;
using Android.Net;

namespace ConferenceAppDroid.Fragments
{
    public class SessionDetailFragment : Android.Support.V4.App.Fragment
    {
        TextView sessionDisplayNameTextView;
        TextView locationAndTimeTextView;
        TextView sessionTimeTextView;
        TextView session_detail_track_color;
        TextView sessionDescriptionTextView;
        ImageView scheduleImageView;
        TextView session_detail_add_to_schedule_tv_label;
        ImageView scheduleAddedImageView;
        TextView session_detail_added_to_schedule_tv_label;

        TextView sessionDetail_textview_id;
        TextView sessionDetail_textview_value;
        TextView sessionDetail_twitter_hash_tag_id;
        TextView sessionDetail_twitter_hash_tag_value;
        TextView sessionDetail_trackTextView_id;
        TextView sessionDetail_trackTextView_value;
        TextView sessionDetail_subTrackSectionTextView_id;
        LinearLayout sessionDetail_subtrack_container_value;
        TextView sessionDetail_technical_level_id;
        TextView sessionDetail_technical_level_value;


        LinearLayout sessionDetail_market_segment_container;
        TextView sessionDetail_market_segment_id;
        TextView sessionDetail_market_segment_value;
        TextView competency_textview_id;
        TextView competency_textview_value;
        LinearLayout sessionDetail_compatancy_container;
        TextView sessionDetail_product_n_topic_id;
        TextView sessionDetail_product_n_topic_value;
        TextView sessionDetail_audience_container_id;
        LinearLayout sessionDetail_audience_container_value;
        TextView sessionDetail_session_type_id;
        TextView sessionDetail_session_type_value;
        TextView sessionDetail_role_id;
        TextView sessionDetail_roleNameTextView_value;
        LinearLayout sessionDetail_role_container;
        TextView sessionDetail_industry_id;
        TextView sessionDetail_industryTextView_value;
        TextView sessionDetail_skilllevel_id;
        LinearLayout sessionDetail_skilllevel_container;
        TextView sessionDetails_speakerSectionTextView;
        RelativeLayout speakercontainer;
        BuiltSessionTime currentSession;
        RelativeLayout addToMyScheduleContainer;
        RelativeLayout addedToScheduleContainer;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.view_header_session_detail_screen, null);
            var rowSessionDetailPager = inflater.Inflate(Resource.Layout.view_session_detail_pager_adapter, null);
            var sessionDetailListView = (ListView)rowSessionDetailPager.FindViewById(Resource.Id.sessionDetailListView);
            sessionDisplayNameTextView = view.FindViewById<TextView>(Resource.Id.sessionDisplayNameTextView);

            locationAndTimeTextView = view.FindViewById<TextView>(Resource.Id.locationAndTimeTextView);

            sessionTimeTextView = view.FindViewById<TextView>(Resource.Id.sessionTimeTextView);

            session_detail_track_color = view.FindViewById<TextView>(Resource.Id.session_detail_track_color);

            sessionDescriptionTextView = view.FindViewById<TextView>(Resource.Id.sessionDescriptionTextView);
            scheduleImageView = view.FindViewById<ImageView>(Resource.Id.scheduleImageView);

            session_detail_add_to_schedule_tv_label = view.FindViewById<TextView>(Resource.Id.session_detail_add_to_schedule_tv_label);
            scheduleAddedImageView = view.FindViewById<ImageView>(Resource.Id.scheduleAddedImageView);
            session_detail_added_to_schedule_tv_label = view.FindViewById<TextView>(Resource.Id.session_detail_added_to_schedule_tv_label);

            #region--ID--
            sessionDetail_textview_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_textview_id);
            sessionDetail_textview_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_textview_value);
            #endregion

            #region--hastag--
            sessionDetail_twitter_hash_tag_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_twitter_hash_tag_id);
            sessionDetail_twitter_hash_tag_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_twitter_hash_tag_value);
            #endregion

            #region--track--
            sessionDetail_trackTextView_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_trackTextView_id);
            sessionDetail_trackTextView_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_trackTextView_value);
            #endregion

            sessionDetail_subTrackSectionTextView_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_subTrackSectionTextView_id);
            sessionDetail_subtrack_container_value = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_subtrack_container_value);

            sessionDetail_technical_level_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_technical_level_id);
            sessionDetail_technical_level_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_technical_level_value);

            sessionDetail_market_segment_container = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_market_segment_container);
            sessionDetail_market_segment_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_market_segment_id);
            sessionDetail_market_segment_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_market_segment_value);

            competency_textview_id = view.FindViewById<TextView>(Resource.Id.competency_textview_id);
            competency_textview_value = view.FindViewById<TextView>(Resource.Id.competency_textview_value);
            sessionDetail_compatancy_container = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_compatancy_container);

            sessionDetail_product_n_topic_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_product_n_topic_id);
            sessionDetail_product_n_topic_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_product_n_topic_value);

            sessionDetail_audience_container_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_audience_container_id);
            sessionDetail_audience_container_value = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_audience_container_value);

            sessionDetail_session_type_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_session_type_id);
            sessionDetail_session_type_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_session_type_value);

            sessionDetail_role_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_role_id);
            sessionDetail_roleNameTextView_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_roleNameTextView_value);
            sessionDetail_role_container = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_role_container);

            sessionDetail_industry_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_industry_id);
            sessionDetail_industryTextView_value = view.FindViewById<TextView>(Resource.Id.sessionDetail_industryTextView_value);

            sessionDetail_skilllevel_id = view.FindViewById<TextView>(Resource.Id.sessionDetail_skilllevel_id);
            sessionDetail_skilllevel_container = view.FindViewById<LinearLayout>(Resource.Id.sessionDetail_skilllevel_container);

            sessionDetails_speakerSectionTextView = view.FindViewById<TextView>(Resource.Id.sessionDetails_speakerSectionTextView);
            speakercontainer = view.FindViewById<RelativeLayout>(Resource.Id.speakercontainer);

            addToMyScheduleContainer = view.FindViewById<RelativeLayout>(Resource.Id.addToMyScheduleContainer);
            addedToScheduleContainer = view.FindViewById<RelativeLayout>(Resource.Id.addedToScheduleContainer);

            var uid = ((SessionDetail)Activity).uid;
            DataManager.GetSessionTimeFromSessionIds(DBHelper.Instance.Connection, new List<string> { uid }).ContinueWith(t =>
            {
                currentSession = t.Result.FirstOrDefault();
                builtTrackAll((res) =>
               {
                   var tracksList = res;
                   if (currentSession.BuiltSession.track == "" || currentSession.BuiltSession.track == null)
                   {
                       currentSession.BuiltSession.track = "No Track";
                   }
                   var builtTrack = tracksList.FirstOrDefault(p => p.name.ToLower() == currentSession.BuiltSession.track.ToLower());
                   Activity.RunOnUiThread(() =>
                   {
                       setHeader(currentSession, builtTrack);
                       AppSettings.Instance.SessionDetailSpeaker = currentSession.BuiltSession.speaker;
                       sessionDetailListView.Adapter = new speakerDetailAdapter(Activity, Resource.Layout.speaker_row, currentSession.BuiltSession.speaker);
                   });
               });
            });

            sessionDetailListView.AddHeaderView(view, null, false);
            sessionDetailListView.ItemClick += (s, e) =>
                {
                    if (e.Position < e.Position - 1)
                        return;
                    var currentSpeaker = currentSession.BuiltSession.speaker[e.Position - 1];
                    Intent intent = new Intent(Activity, (typeof(SpeakerDetailActivity)));
                    intent.PutExtra("uid", currentSpeaker.user_ref);
                    StartActivity(intent);
                };
            return rowSessionDetailPager;
        }

        void setHeader(BuiltSessionTime currentSession, BuiltTracks builtTracks)
        {
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.title))
            {
                sessionDisplayNameTextView.Visibility = ViewStates.Visible;
                sessionDisplayNameTextView.Text = currentSession.BuiltSession.title;
            }

            if (!string.IsNullOrWhiteSpace(currentSession.room))
            {
                locationAndTimeTextView.Visibility = ViewStates.Visible;
                locationAndTimeTextView.Text = currentSession.room;
            }

            if (currentSession.date != null && currentSession.date.Length > 0)
            {
                sessionTimeTextView.Visibility = ViewStates.Visible;
                sessionTimeTextView.Text = currentSession.date;
            }



            try
            {
                if (Convert.ToDateTime(currentSession.date).Date < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    if (addToMyScheduleContainer.Visibility == ViewStates.Visible)
                    { addToMyScheduleContainer.Visibility = ViewStates.Gone; }

                }
                else if (Convert.ToDateTime(currentSession.date).Date == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(currentSession.time) < Helper.timeConverterForCurrentHourMinute())
                {
                    if (addToMyScheduleContainer.Visibility == ViewStates.Visible)
                    { addToMyScheduleContainer.Visibility = ViewStates.Gone; }
                }
                else if (Convert.ToDateTime(currentSession.date).Date > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
                {
                    addToMyScheduleContainer.Visibility = ViewStates.Visible;
                }
            }
            catch
            {
                addToMyScheduleContainer.Visibility = ViewStates.Gone;
            }


            #region--Add to Schedule--

            addToMyScheduleContainer.Click += (s, e) =>
                {
                    if (AppSettings.Instance.ApplicationUser != null)
                    {
                        ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);

                        if (connectivityManager.ActiveNetworkInfo == null)
                        {
                            Toast.MakeText(Activity, Activity.Resources.GetString(Resource.String.no_internet_connection_text), ToastLength.Long).Show();
                            return;
                        }
                        Helper.showProgress(Activity, true);
                        DataManager.AddSessionToSchedule(DBHelper.Instance.Connection, currentSession, (session) =>
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                     Helper.showProgress(Activity, false);
                                     Toast.MakeText(Activity, (Helper.SessionAddedString), ToastLength.Long).Show();
                                     addToMyScheduleContainer.Visibility = ViewStates.Gone;
                                     addedToScheduleContainer.Visibility = ViewStates.Visible;
                                });
                            }, error =>
                            {
                                Activity.RunOnUiThread(() =>
                                {
                                    Helper.showProgress(Activity, false);
                                    Toast.MakeText(Activity, (Helper.GetErrorMessage(error)), ToastLength.Long).Show();
                                });
                            });
                    }
                    else
                    {
                        new Android.App.AlertDialog.Builder(Activity).SetTitle("Login Required").SetMessage("You need to be logged-in to access this feature. Do you want to login?")
                                                       .SetPositiveButton("Yes", (sender, args) =>
                                                       {
                                                           Intent intent = new Intent(Activity, typeof(LoginActivity));
                                                           StartActivity(intent);
                                                       })
                                                       .SetNegativeButton("No", (sender, args) => { }).Show();
                    }
                };

            #endregion

            #region--Remove from MySchedule--
            addedToScheduleContainer.Click += (s, e) =>
               {
                   if (AppSettings.Instance.ApplicationUser != null)
                   {
                       ConnectivityManager connectivityManager = (ConnectivityManager)Activity.GetSystemService(Context.ConnectivityService);

                       if (connectivityManager.ActiveNetworkInfo == null)
                       {
                           Toast.MakeText(Activity, Activity.Resources.GetString(Resource.String.no_internet_connection_text), ToastLength.Long).Show();
                           return;
                       }
                       Helper.showProgress(Activity, true);
                       if (currentSession.BuiltSession.type.Equals(AppSettings.Instance.GeneralSession, StringComparison.InvariantCultureIgnoreCase) || currentSession.BuiltSession.type.Equals(AppSettings.Instance.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                       {
                           Toast.MakeText(Activity, (Helper.CanntotUnschedule), ToastLength.Long).Show();
                       }
                       else
                       {
                           DataManager.RemoveSessionFromSchedule(DBHelper.Instance.Connection, currentSession, (err, session) =>
                           {
                               if (err == null)
                               {
                                   Activity.RunOnUiThread(() =>
                                   {
                                       Helper.showProgress(Activity, false);
                                       Toast.MakeText(Activity, (Helper.SessionRemovedString), ToastLength.Long).Show();
                                       addedToScheduleContainer.Visibility = ViewStates.Gone;
                                       addToMyScheduleContainer.Visibility = ViewStates.Visible;
                                   });
                               }
                           });
                       }
                   }
                   else
                   {
                       new Android.App.AlertDialog.Builder(Activity).SetTitle("Login Required").SetMessage("You need to be logged-in to access this feature. Do you want to login?")
                                                      .SetPositiveButton("Yes", (sender, args) =>
                                                      {
                                                          Intent intent = new Intent(Activity, typeof(LoginActivity));
                                                          StartActivity(intent);
                                                      })
                                                      .SetNegativeButton("No", (sender, args) => { }).Show();
                   }
               };
            #endregion


            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession._abstract))
            {
                sessionDescriptionTextView.Visibility = ViewStates.Visible;
                sessionDescriptionTextView.Text = currentSession.BuiltSession._abstract;
            }
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.abbreviation))
            {
                sessionDetail_textview_id.Visibility = ViewStates.Visible;
                sessionDetail_textview_value.Visibility = ViewStates.Visible;
                sessionDetail_textview_value.Text = currentSession.BuiltSession.abbreviation;
            }
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.abbreviation))
            {
                sessionDetail_twitter_hash_tag_id.Visibility = ViewStates.Visible;
                sessionDetail_twitter_hash_tag_value.Visibility = ViewStates.Visible;
                sessionDetail_twitter_hash_tag_value.Text = string.Concat(new List<string> { "#", currentSession.BuiltSession.abbreviation });
            }

            if (!String.IsNullOrWhiteSpace(currentSession.BuiltSession.track))
            {
                sessionDetail_trackTextView_id.Visibility = ViewStates.Visible;
                sessionDetail_trackTextView_value.Visibility = ViewStates.Visible;
                sessionDetail_trackTextView_value.Text = currentSession.BuiltSession.track;
            }

            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.technical_level))
            {
                sessionDetail_technical_level_id.Visibility = ViewStates.Visible;
                sessionDetail_technical_level_value.Visibility = ViewStates.Visible;
                sessionDetail_technical_level_value.Text = currentSession.BuiltSession.technical_level;

            }
            foreach (var  i in currentSession.BuiltSession.speaker)
            {
                if (i.full_name != string.Empty && currentSession.BuiltSession.speaker.Count > 0)
                {
                    var speaker = currentSession.BuiltSession.speaker;
                    speakercontainer.Visibility = ViewStates.Visible;
                    sessionDetails_speakerSectionTextView.Visibility = ViewStates.Visible;
                    if (speaker.Count == 1)
                    {
                        sessionDetails_speakerSectionTextView.Text = speaker.Count + "Speaker";
                    }
                    else
                    {
                        sessionDetails_speakerSectionTextView.Text = speaker.Count + " Speakers";
                    }

                }
                else
                {
                    speakercontainer.Visibility = ViewStates.Gone;
                    sessionDetails_speakerSectionTextView.Visibility = ViewStates.Gone;
                }
            }
            if (builtTracks != null)
            {

                if (builtTracks.color != null && builtTracks.color.Length > 0)
                {
                    session_detail_track_color.Visibility = ViewStates.Visible;
                    if (builtTracks.color.Contains("#"))
                    {
                        session_detail_track_color.SetBackgroundColor(Color.ParseColor(builtTracks.color));
                    }
                    else
                    {
                        session_detail_track_color.SetBackgroundColor(Color.ParseColor("#" + builtTracks.color));
                    }
                }

            }
            #region--marketsegment--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.market_segment_separated))
            {
                var marketSegment = currentSession.BuiltSession.market_segment_separated;
                sessionDetail_market_segment_id.Visibility = ViewStates.Visible;
                sessionDetail_market_segment_container.Visibility = ViewStates.Visible;
                if (sessionDetail_market_segment_container.ChildCount > 0)
                {
                    sessionDetail_market_segment_container.RemoveAllViews();
                }

                if (marketSegment.Contains(","))
                {
                    string[] values = marketSegment.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_market_segment_container.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.market_segment_separated.Trim();
                    sessionDetail_market_segment_container.AddView(textView);
                }
            }
            #endregion

            #region--subtrack--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.sub_track_separated))
            {
                var subTrack = currentSession.BuiltSession.sub_track_separated;
                sessionDetail_subTrackSectionTextView_id.Visibility = ViewStates.Visible;
                sessionDetail_subtrack_container_value.Visibility = ViewStates.Visible;
                if (sessionDetail_subtrack_container_value.ChildCount > 0)
                {
                    sessionDetail_subtrack_container_value.RemoveAllViews();
                }

                if (subTrack.Contains(","))
                {
                    string[] values = subTrack.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_subtrack_container_value.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.sub_track_separated.Trim();
                    sessionDetail_subtrack_container_value.AddView(textView);
                }
            }
            #endregion

            #region--competency--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.competency_separated))
            {
                var competency = currentSession.BuiltSession.competency_separated;
                competency_textview_id.Visibility = ViewStates.Visible;
                sessionDetail_compatancy_container.Visibility = ViewStates.Visible;
                if (sessionDetail_compatancy_container.ChildCount > 0)
                {
                    sessionDetail_compatancy_container.RemoveAllViews();
                }

                if (competency.Contains(","))
                {
                    string[] values = competency.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_compatancy_container.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.competency_separated.Trim();
                    sessionDetail_compatancy_container.AddView(textView);
                }
            }
            #endregion

            #region--audience--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.audience_separated))
            {
                var audience = currentSession.BuiltSession.audience_separated;
                sessionDetail_audience_container_id.Visibility = ViewStates.Visible;
                sessionDetail_audience_container_value.Visibility = ViewStates.Visible;
                if (sessionDetail_audience_container_value.ChildCount > 0)
                {
                    sessionDetail_audience_container_value.RemoveAllViews();
                }

                if (audience.Contains(","))
                {
                    string[] values = audience.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_audience_container_value.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.audience_separated.Trim();
                    sessionDetail_audience_container_value.AddView(textView);
                }
            }
            #endregion

            #region--role--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.role_separated))
            {
                var role = currentSession.BuiltSession.role_separated;
                sessionDetail_role_id.Visibility = ViewStates.Visible;
                sessionDetail_role_container.Visibility = ViewStates.Visible;
                if (sessionDetail_role_container.ChildCount > 0)
                {
                    sessionDetail_role_container.RemoveAllViews();
                }

                if (role.Contains(","))
                {
                    string[] values = role.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_role_container.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.role_separated.Trim();
                    sessionDetail_role_container.AddView(textView);
                }
            }
            #endregion

            #region--skilllevel--
            if (!string.IsNullOrWhiteSpace(currentSession.BuiltSession.skill_level_separated))
            {
                var skill = currentSession.BuiltSession.skill_level_separated;
                sessionDetail_skilllevel_id.Visibility = ViewStates.Visible;
                sessionDetail_skilllevel_container.Visibility = ViewStates.Visible;
                if (sessionDetail_skilllevel_container.ChildCount > 0)
                {
                    sessionDetail_skilllevel_container.RemoveAllViews();
                }

                if (skill.Contains(","))
                {
                    string[] values = skill.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                        textView.Text = values[i].Trim();
                        if (i == values.Length - 1)
                        {
                            textView.SetBackgroundResource(Resource.Drawable.bg_white);
                        }
                        else
                        {
                            textView.SetBackgroundResource(Resource.Drawable.layerlist_bottom_grey_line);
                            textView.SetPadding(dpToPx(Activity, 15), dpToPx(Activity, 10), dpToPx(Activity, 15), dpToPx(Activity, 10));
                        }

                        sessionDetail_skilllevel_container.AddView(textView);
                    }
                }
                else
                {
                    TextView textView = (TextView)LayoutInflater.From(Activity).Inflate(Resource.Layout.tv_session_deails_value, null);
                    textView.Text = currentSession.BuiltSession.skill_level_separated.Trim();
                    sessionDetail_skilllevel_container.AddView(textView);
                }
            }
            #endregion
        }

        public static int dpToPx(Context context, int dp)
        {
            int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
            return px;
        }
        private void builtTrackAll(Action<List<BuiltTracks>> callback)
        {
            DataManager.GetListOfAllTrack(DBHelper.Instance.Connection).ContinueWith(t =>
            {
                var result = t.Result;
                if (callback != null)
                    try
                    {
                        callback(result);
                    }
                    catch (Exception e)
                    {
                    }
            });
        }

        public class speakerDetailAdapter : ArrayAdapter<BuiltSessionSpeaker>
        {
            TextView speakerName;
            TextView speakerInfo;
            List<BuiltSessionSpeaker> items;
            Android.App.Activity context;
            string[] keys;
            public int selectedTab = 0;
            private readonly int _resource;
            public speakerDetailAdapter(Android.App.Activity context, int resource, List<BuiltSessionSpeaker> items)
                : base(context, resource, items)
            {
                this.context = context;
                this.items = items;
                _resource = resource;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ViewHolder viewHolder;
                var builtSessionSpeaker = items[position];
                View view = convertView;
                if (convertView == null)
                {
                    view = (RelativeLayout)this.context.LayoutInflater.Inflate(_resource, parent, false);
                    speakerName = view.FindViewById<TextView>(Resource.Id.speakerNameTextView);
                    speakerInfo = view.FindViewById<TextView>(Resource.Id.speakerInfoTextView);
                    viewHolder = new ViewHolder(speakerName, speakerInfo);
                    view.Tag = viewHolder;
                }
                else
                {
                    viewHolder = (ViewHolder)view.Tag;
                    speakerName = viewHolder.speakerName;
                    speakerInfo = viewHolder.speakerInfo;
                }

                UpdateCell(builtSessionSpeaker);
                return view;
            }
            public class ViewHolder : Java.Lang.Object
            {
                public TextView speakerName;
                public TextView speakerInfo;
                public ViewHolder(TextView speakerName, TextView speakerInfo)
                {
                    this.speakerName = speakerName;
                    this.speakerInfo = speakerInfo;
                }
            }

            public void UpdateCell(BuiltSessionSpeaker builtSessionSpeaker)
            {
                var fullName = builtSessionSpeaker.full_name;
                var roles = builtSessionSpeaker.roles;
                if (!string.IsNullOrEmpty(fullName))
                {
                    speakerName.Text = fullName;
                }
                if (!string.IsNullOrEmpty(roles))
                {
                    speakerInfo.Text = roles;
                }
            }

        }
    }
}