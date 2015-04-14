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
using Android.Support.V4.App;
using ConferenceAppDroid.Interfaces;
using UK.CO.Senab.Photoview;
using ConferenceAppDroid.BroadcastReceivers;
using Android.Graphics;

namespace ConferenceAppDroid.Camera
{
    [Activity(Label = "DisplayActivity")]
    public class DisplayActivity : FragmentActivity, IGetTweetStatusObject
    {
        public static byte[] imageToShow = null;
        PhotoView imageView;
        Button saveButton;
        Button cancelButton;
        String imageURL;

        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private RelativeLayout actionBarTitleContainer;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            View actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView=(actionBarView);
            ActionBar.DisplayOptions=ActionBarDisplayOptions.ShowCustom;
            // Create your application here

            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            actionBarTitleContainer = (RelativeLayout)FindViewById(Resource.Id.actionBarTitleContainer);

            leftMenuBtn.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility=ViewStates.Invisible;
            bottomImageView.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.close_icon_selector));
            bottomImageView.Visibility=ViewStates.Visible;

            bottomImageView.Click += (s, e) =>
                {
                    Finish();
                };

            titleTextView.Text=("");
            //        actionBarTitleContainer.setBackgroundResource(R.drawable.bg_repeat_session);
            SetContentView(Resource.Layout.activity_selfie_image_preview);
            Title=("");
             if (imageToShow == null) 
             {
            Toast.MakeText(this, Resource.String.no_image, ToastLength.Long).Show();
            Finish();
        }

            else
             {

            if (Intent != null) 
            {
                imageURL = Intent.GetStringExtra("imageUrl");
            }

            imageView = (PhotoView) FindViewById(Resource.Id.selfiePreview_previewImageView);
            saveButton = (Button) FindViewById(Resource.Id.selfiePreview_save_button);
            cancelButton = (Button) FindViewById(Resource.Id.selfiePreview_cancel_button);
                 saveButton.Click+=(s,e)=>
                     {
                         Intent selfieIntent = new Intent(SelfieImageDoneBroadCast.action);
                    selfieIntent.PutExtra ("imagePath", imageURL);
                         SendBroadcast (selfieIntent);
                    Finish ();
                     };

     

            BitmapFactory.Options opts = new BitmapFactory.Options();

            opts.InPurgeable = true;
            opts.InInputShareable = true;
            opts.InMutable = false;
            opts.InSampleSize = 2;

            imageView.SetImageBitmap(BitmapFactory.DecodeByteArray(imageToShow, 0, imageToShow.Length, opts));
            imageToShow = null;

            imageView.SetScaleType(ImageView.ScaleType.CenterInside);


        }
        }

        public void setTweetStatusObject(Twitter4j.IStatus result, int position, async.TweetActionAsync.ActionType type)
        {
        }
    }
}