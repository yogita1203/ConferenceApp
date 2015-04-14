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
using CommonLayer.Entities.Built;

namespace ConferenceAppDroid.Fragments
{
    public class SurveysFragment : Android.Support.V4.App.Fragment
    {
        View parentView;
        ListView mySurveyListView;
        TextView noDataTextView;
        RelativeLayout surveyLoadingContainer;
        SurveysAdapter adapter;
        List<SurveyExtension> lstSurveys;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            parentView = inflater.Inflate(Resource.Layout.view_list, null);
            init();

            showProgress();
            DataManager.getMySurveyExtension(DBHelper.Instance.Connection, (t, count) =>
            {
                lstSurveys = t;
                if (lstSurveys.Count > 0)
                {
                    noDataTextView.Visibility = ViewStates.Gone;
                    noDataTextView.Text = "";
                }
                else
                {
                    noDataTextView.Visibility = ViewStates.Visible;
                    noDataTextView.Text = "There are no surveys to fill out at this time. Please check again later.";
                }
                AppSettings.Instance.NewSurveyCount = 0;
                Activity.RunOnUiThread(() =>
                {
                    hideProgress();
                    adapter = new SurveysAdapter(Activity, Resource.Layout.survey_row, lstSurveys);
                    mySurveyListView.Adapter = adapter;
                });
            }, AppSettings.Instance.NewSurveyCount);


            mySurveyListView.ItemClick += (s, e) =>
                {
                    var currentSurvey = lstSurveys[e.Position];
                    Intent i = new Intent(Activity, typeof(UIWebViewForSurvey));
                    if (!currentSurvey.mobile_url.Contains("http") && !currentSurvey.mobile_url.Contains("https"))
                    {
                        i.PutExtra("url", "http://" + currentSurvey.mobile_url);
                    }
                    else
                    {
                        i.PutExtra("url", currentSurvey.mobile_url);
                    }
                    Activity.StartActivity(i);
                };
            return parentView;
        }
        private void init()
        {
            noDataTextView = (TextView)parentView.FindViewById(Resource.Id.survey_NoData_TextView);
            mySurveyListView = (ListView)parentView.FindViewById(Android.Resource.Id.List);
            surveyLoadingContainer = (RelativeLayout)parentView.FindViewById(Resource.Id.surveyLoadingContainer);
            surveyLoadingContainer.Clickable = true;
        }

        public void showProgress()
        {
            if (surveyLoadingContainer != null)
            {
                surveyLoadingContainer.Visibility = ViewStates.Visible;
            }
        }

        public void hideProgress()
        {
            if (surveyLoadingContainer != null)
            {
                surveyLoadingContainer.Visibility = ViewStates.Gone;
            }
        }
    }


    public class SurveysAdapter : ArrayAdapter<SurveyExtension>
    {
        Android.App.Activity context;
        List<SurveyExtension> surveys;
        TextView surveys_textview_title, surveys_session_title;
        ImageView row_surveys_arrow;
        int _resource;
        public SurveysAdapter(Android.App.Activity context, int resource, List<SurveyExtension> surveys)
            : base(context, resource, surveys)
        {
            this.context = context;
            this._resource = resource;
            this.surveys = surveys;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View parentView;
            parentView = convertView;
            ViewHolder viewholder;
            if (null == convertView)
            {
                parentView = LayoutInflater.From(context).Inflate(Resource.Layout.survey_row, null);
                surveys_textview_title = parentView.FindViewById<TextView>(Resource.Id.surveys_textview_title);
                surveys_session_title = parentView.FindViewById<TextView>(Resource.Id.surveys_session_title);
                row_surveys_arrow = parentView.FindViewById<ImageView>(Resource.Id.row_handsonlab_arrow);
                viewholder = new ViewHolder(surveys_textview_title, surveys_session_title, row_surveys_arrow);
                parentView.Tag = viewholder;
            }
            else
            {
                viewholder = (ViewHolder)parentView.Tag;
                surveys_textview_title = viewholder.surveys_textview_title;
                surveys_session_title = viewholder.surveys_session_title;
                row_surveys_arrow = viewholder.row_surveys_arrow;
            }

            var item = GetItem(position);

            if (!string.IsNullOrWhiteSpace(item.name))
            {
                surveys_textview_title.Text = item.name;
            }
            if (!string.IsNullOrWhiteSpace(item.session_title))
            {
                surveys_session_title.Visibility = ViewStates.Visible;
                surveys_session_title.Text = item.session_title;
            }
            else
            {
                surveys_session_title.Visibility = ViewStates.Gone;
                surveys_session_title.Text = "";
            }


            return parentView;
        }

        public class ViewHolder : Java.Lang.Object
        {
            public TextView surveys_textview_title, surveys_session_title;
            public ImageView row_surveys_arrow;
            public ViewHolder(TextView surveys_textview_title, TextView surveys_session_title, ImageView row_surveys_arrow)
            {

                this.surveys_textview_title = surveys_textview_title;
                this.surveys_session_title = surveys_session_title;
                this.row_surveys_arrow = row_surveys_arrow;
            }
        }

    }
}