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
using UK.CO.Senab.Photoview;
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid
{
    [Activity(Label = "ImagePreviewActivity")]
    public class ImagePreviewActivity : Activity
    {
        private Context context;
        private PhotoView previewImageView;
        private ProgressBar progress;
        private View actionBarView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private ImageButton back_btn;
        private String mosconeCenterName;
        private String fromLocal;
        private String url;
        private String title;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            OverridePendingTransition(Resource.Animation.pull_in_from_right, Resource.Animation.hold);
            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);

            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            SetContentView(Resource.Layout.activity_image_preview);

            if (Intent.Extras != null)
            {
                url = Intent.GetStringExtra("url");
                mosconeCenterName = Intent.GetStringExtra("mosconeCenterName");
                fromLocal = Intent.GetStringExtra("Local");
                title = Intent.GetStringExtra("title");
            }

            init();

            leftMenuBtn.Visibility=ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Gone;
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += back_btn_Click;
        }

        void back_btn_Click(object sender, EventArgs e)
        {
            Finish();
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }

        private void init()
        {
            var previewImageView = FindViewById<UK.CO.Senab.Photoview.PhotoView>(Resource.Id.previewImageView);
            progress = (ProgressBar)FindViewById(Resource.Id.progressBar);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            back_btn = (ImageButton)actionBarView.FindViewById(Resource.Id.back_btn);
            if (mosconeCenterName != null)
                titleTextView.Text=(mosconeCenterName.ToUpper());
            //titleTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

            if (title != null)
            {
                titleTextView.Text=(title.ToUpper());
            }


            if (fromLocal != null && fromLocal.Equals("FromLocal",StringComparison.InvariantCultureIgnoreCase))
            {
                // previewImageView.setImageUrl(url);
                UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(previewImageView, url);
            }
            else
            {
                try
                {
                    UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(previewImageView, url);
                }
                catch (Exception e)
                {
                  
                }
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);

        }

        protected override void OnResume()
        {
            base.OnResume();
            new AppUtilities().isFromPauseResume(this, false);
        }

        protected override void OnPause()
        {
            base.OnPause();
            new AppUtilities().isFromPauseResume(this, true);
        }
    }
}