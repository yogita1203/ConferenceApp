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
using CommonLayer.Entities.Built;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using Android.Text;
using ConferenceAppDroid.Interfaces;
using ConferenceAppDroid.Activities;

namespace ConferenceAppDroid.Fragments
{
    public class SpeakerDetailFragment : Android.Support.V4.App.Fragment
    {
        TextView speaker_detail_name_tv;
        TextView speaker_detail_company_name_tv;
        TextView speaker_detail_desgination_tv;
        TextView speaker_detail_description_tv;
        TextView speaker_detail_session_label;
        BuiltSpeaker currentSpeaker;
        ListView sessionDetailListView;
        List<BuiltSessionTime> SpeakerSessions;
        private Context context;
        public View rowSpeakerDetailPager;
        private int mShortAnimationDuration;
        View rowSpeakerDetailPagerHeader;
        SessionAdapter adapter;
        private ILoadFragment loadFragment;
        private String fromType;
        private int speakerPosition;
        private List<BuiltSpeaker> speakerList;

        private void setContext(Context c)
        {
            context = c;
        }

        public static SpeakerDetailFragment newInstance(Context context)
        {
            SpeakerDetailFragment f = new SpeakerDetailFragment();
            f.setContext(context);
            return f;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            if (Arguments != null)
            {
                Bundle bundle = Arguments;

                fromType = bundle.GetString("from");
                speakerPosition = bundle.GetInt("speakerPosition");
                //speakerList = (List<BuiltSpeaker>)bundle.GetSerializable("speakerList");
            }

            rowSpeakerDetailPagerHeader = inflater.Inflate(Resource.Layout.view_speaker_detail_header, null);
            //var uid = ((SpeakerDetail)Activity).uid;
            rowSpeakerDetailPager = inflater.Inflate(Resource.Layout.view_speakers_detail_pager_adapter, null);
            sessionDetailListView = (ListView)rowSpeakerDetailPager.FindViewById(Resource.Id.speaker_detail_listview);
            speaker_detail_name_tv = rowSpeakerDetailPagerHeader.FindViewById<TextView>(Resource.Id.speaker_detail_name_tv);

            speaker_detail_company_name_tv = rowSpeakerDetailPagerHeader.FindViewById<TextView>(Resource.Id.speaker_detail_company_name_tv);

            speaker_detail_desgination_tv = rowSpeakerDetailPagerHeader.FindViewById<TextView>(Resource.Id.speaker_detail_desgination_tv);
            speaker_detail_description_tv = rowSpeakerDetailPagerHeader.FindViewById<TextView>(Resource.Id.speaker_detail_description_tv);
            speaker_detail_session_label = rowSpeakerDetailPagerHeader.FindViewById<TextView>(Resource.Id.speaker_detail_session_label);
            DataManager.GetListOfAllTrack(DBHelper.Instance.Connection).ContinueWith(q =>
               {
                   var tracks = q.Result;
                   currentSpeaker = ((SpeakerDetailActivity)Activity).speakerModelList[speakerPosition];
                   DataManager.GetSpeakerSessions(DBHelper.Instance.Connection, currentSpeaker.session.Select(p => p.session_id).ToList()).ContinueWith(t =>
               {
                   SpeakerSessions = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ToList();
                   Activity.RunOnUiThread(() =>
                       {
                           setHeader(currentSpeaker, SpeakerSessions.Count);
                           loadFragment.loadFragment(true, "", currentSpeaker);
                           adapter = new SessionAdapter(Activity, Resource.Layout.row_all_session, SpeakerSessions, tracks, Screens.SpeakerSession);
                           sessionDetailListView.Adapter = adapter;
                       });
               });
               });

            sessionDetailListView.AddHeaderView(rowSpeakerDetailPagerHeader, null, false);
            sessionDetailListView.ItemClick += (s, e) =>
                {
                    var currentSession = SpeakerSessions[e.Position - 1];
                    Intent intent = new Intent(Activity, (typeof(SessionDetail)));
                    intent.PutExtra("uid", currentSession.BuiltSession.session_id);
                    StartActivity(intent);
                };

            mShortAnimationDuration = 1800;
            crossFade();
            return rowSpeakerDetailPager;

        }
        void setHeader(BuiltSpeaker currentSpeaker, int sessionCount)
        {
            if (currentSpeaker.full_name != null || currentSpeaker.first_name != null || currentSpeaker.last_name != null)
            {
                speaker_detail_name_tv.Text = string.IsNullOrEmpty(currentSpeaker.full_name) ? string.Format("{0} {1}", currentSpeaker.first_name, currentSpeaker.last_name) : string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(currentSpeaker.company_name))
            {
                speaker_detail_company_name_tv.Text = (Html.FromHtml(currentSpeaker.company_name)).ToString();
            }
            if (!string.IsNullOrWhiteSpace(currentSpeaker.job_title))
            {
                speaker_detail_desgination_tv.Text = (Html.FromHtml(currentSpeaker.job_title)).ToString();
            }
            if (!string.IsNullOrWhiteSpace(currentSpeaker.bio))
            {
                speaker_detail_description_tv.Text = (Html.FromHtml(currentSpeaker.bio)).ToString();
            }
            else
            {
                speaker_detail_description_tv.Visibility = ViewStates.Gone;
                rowSpeakerDetailPagerHeader.FindViewById(Resource.Id.speakerDetailLine).Visibility = ViewStates.Gone;
            }

            if (sessionCount != null && sessionCount > 0)
            {
                //var session = currentSpeaker.session;
                speaker_detail_session_label.Visibility = ViewStates.Visible;
                if (sessionCount == 1)
                {
                    speaker_detail_session_label.Text = SpeakerSessions.Count + "Session";
                }
                else
                {
                    speaker_detail_session_label.Text = SpeakerSessions.Count + " Sessions";
                }
            }
            else
            {
                speaker_detail_session_label.Visibility = ViewStates.Gone;
            }
            if (adapter != null)
            {
                adapter.NotifyDataSetChanged();
            }

        }

        private void crossFade()
        {

            rowSpeakerDetailPager.Alpha = (0f);
            rowSpeakerDetailPager.Visibility = ViewStates.Visible;

            rowSpeakerDetailPager.Animate()
                    .Alpha(1f)
                    .SetDuration(mShortAnimationDuration)
                    .SetListener(null);

        }

        private void setDetails(BuiltSpeaker speakerModel)
        {

            //speakerNameTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //speakerDetailSessionLabel.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //speakerDetailTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //speakerDetailCompanyTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //speakerDesignationTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

            if (speakerModel.full_name != null && speakerModel.full_name.Trim().Length > 0)
            {
                speaker_detail_name_tv.Text = (speakerModel.full_name);
            }
            else if (speakerModel.first_name != null && speakerModel.last_name != null && speakerModel.first_name.Trim().Length > 0 && speakerModel.last_name.Trim().Length > 0)
            {
                speaker_detail_name_tv.Text = (speakerModel.first_name + " " + speakerModel.last_name);
            }
            else if (speakerModel.first_name != null && speakerModel.first_name.Trim().Length > 0)
            {
                speaker_detail_name_tv.Text = (speakerModel.first_name);
            }

            if (!string.IsNullOrWhiteSpace((speakerModel.bio)))
            {
                speaker_detail_description_tv.Text = (Html.FromHtml(speakerModel.bio)).ToString();
            }
            else
            {
                speaker_detail_description_tv.Visibility = ViewStates.Gone;
                rowSpeakerDetailPagerHeader.FindViewById(Resource.Id.speakerDetailLine).Visibility = ViewStates.Gone;
            }

            if (speakerModel.company_name != null)
            {
                speaker_detail_company_name_tv.Text = (Html.FromHtml(speakerModel.company_name)).ToString();
            }

            if (speakerModel.job_title != null)
            {
                speaker_detail_desgination_tv.Text = (Html.FromHtml(speakerModel.job_title)).ToString();
            }

            if (adapter != null)
            {
                adapter.NotifyDataSetChanged();
            }
        }

        public override void OnAttach(Android.App.Activity activity)
        {
            base.OnAttach(activity);
            try
            {
                setContext(activity);
                loadFragment = (ILoadFragment)context;
            }
            catch (Exception e)
            {
            }
        }
    }
}