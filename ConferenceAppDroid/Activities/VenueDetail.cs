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

namespace ConferenceAppDroid
{
    [Activity(Label = "VenueDetail")]
    public class VenueDetail : Activity
    {
        View actionBarView;
        ImageView back_btn;
        TextView titleTextView;
        ImageButton rightMenuBtn;
        ImageView mosconeImageView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_moscone_center);

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;

            mosconeImageView = (ImageView)FindViewById(Resource.Id.mosconeImageView);

            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            rightMenuBtn.Visibility = ViewStates.Invisible;

            back_btn = (ImageView)actionBarView.FindViewById(Resource.Id.back_btn);
            back_btn.Visibility = ViewStates.Visible;
            back_btn.Click += (s, e) =>
            {
                Finish();
            };

            titleTextView = actionBarView.FindViewById<TextView>(Resource.Id.titleTextView);
            titleTextView.Text = "VENUE";

            var txtAddress= FindViewById<TextView>(Resource.Id.addressTextView);
            var txtDescription=FindViewById<TextView>(Resource.Id.descriptionTextView);


            var name= Intent.GetStringExtra("Name");
            var address = Intent.GetStringExtra("Address");
            var imageUrl = Intent.GetStringExtra("ImageUrl");

            if(!string.IsNullOrWhiteSpace(imageUrl))
            {
                UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(mosconeImageView, imageUrl, Resource.Drawable.ic_default_pic);
                mosconeImageView.Click += (s, e) =>
                {
                    Intent intent = new Intent(this, typeof(ImagePreviewActivity));
                    intent.PutExtra("url", imageUrl);
                    intent.PutExtra("mosconeCenterName", name);
                    StartActivity(intent);
                };
            }
            
          

            txtAddress.Text = name;
            txtDescription.Text = address;

            // Create your application here
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.hold, Resource.Animation.push_out_to_right);
        }

        public override void OnBackPressed()
        {
            Finish();
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