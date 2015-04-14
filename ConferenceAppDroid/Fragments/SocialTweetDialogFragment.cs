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
using Java.IO;
using Android.Graphics;
using ConferenceAppDroid.Utilities;
using Android.Provider;
using Java.Text;
using Java.Util;
using ConferenceAppDroid.async;
using ConferenceAppDroid.Interfaces;
using ConferenceAppDroid.Camera;

namespace ConferenceAppDroid.Fragments
{
    public class SocialTweetDialogFragment : Android.Support.V4.App. DialogFragment, IGetTweetStatusObject
    {
        private View promptsView;
        TextView cancelButton;
        TextView tweetButton;
        EditText replyEditText;
        ImageView replyImageView;
        LinearLayout replyImageContainer;
        TextView charCountTextView;

        TextView userNameTextView;
        TextView userHandleTextView;
        ImageView userImageView;
        private File photoDirectory;
        private int tweetCount = 140;

        int count;
        String imageUrl;
        private Context context;

        private int CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE = 1221;
     

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            context = (Context)activity;
            System.Console.WriteLine(  );
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            promptsView = inflater.Inflate(Resource.Layout.view_twitter_reply_alert_box, null);
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            Dialog.Window.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.twitter_reply_dialog_drawable));

            cancelButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_cancel_button);
            tweetButton = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_tweet_button);
            replyEditText = (EditText)promptsView.FindViewById(Resource.Id.socialTwitterReply_reply);
            //replyEditText.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            replyImageView = (ImageView)promptsView.FindViewById(Resource.Id.socialTwitterReply_image);
            replyImageContainer = (LinearLayout)promptsView.FindViewById(Resource.Id.socialTwitterReply_image_container);
            charCountTextView = (TextView)promptsView.FindViewById(Resource.Id.socialTwitterReply_char_count);

            if (Arguments != null)
            {
                if (Arguments.GetString("imageUrl") != null)
                {
                    imageUrl = Arguments.GetString("imageUrl");
                    File file = new File(imageUrl);
                    if (file.Exists())
                    {
                        replyImageView.SetImageBitmap(BitmapFactory.DecodeFile(imageUrl));
                        replyImageContainer.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        replyImageContainer.Visibility=ViewStates.Gone;
                    }
                    tweetCount = 117;
                }

                if (Arguments.GetString("postMessage") != null)
                {
                    replyEditText.Text = (Arguments.GetString("postMessage").ToString());
                    replyEditText.SetSelection(replyEditText.Text.ToString().Length, replyEditText.Text.ToString().Length);
                    count = ( tweetCount - (replyEditText.Text.ToString().Length));
                }

                replyImageContainer.Visibility = ViewStates.Gone;
             
            }

        

            //AQuery aQuery = new AQuery(getActivity());
            //aQuery.id(userImageView).image(AppSettings.getTwitterUserImage(getActivity()), true, true, userImageView.getWidth(), 0, null, 0);

            //var socialTwitterReply_selfie_button = promptsView.FindViewById(Resource.Id.socialTwitterReply_selfie_button);
            //socialTwitterReply_selfie_button.Click += (s, e) =>
            //    {
            //        //StartActivity(new Intent(context, typeof(CameraSelfieActivity)));
            //    };
            //var socialTwitterReply_camera_button = promptsView.FindViewById(Resource.Id.socialTwitterReply_camera_button);
            //socialTwitterReply_camera_button.Click += (s, e) =>
            //    {
            //        Intent intent = new Intent(MediaStore.ActionImageCapture);
            //        File fileUri = getOutputMediaFile(0);
            //        intent.PutExtra(MediaStore.ExtraOutput, fileUri);
            //        Activity.StartActivityForResult(intent, CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE);

            //    };

            cancelButton.Visibility = ViewStates.Visible;
            tweetButton.Visibility = ViewStates.Visible;


            charCountTextView.Visibility = ViewStates.Visible;
            charCountTextView.Text = ("" + count);

            cancelButton.Click += (s, e) =>
                {
                    Dismiss();
                };
            tweetButton.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(replyEditText.Text.ToString().Trim()))
                    {
                        Toast.MakeText(context, context.GetString(Resource.String.tweet_can_not_be_blank_text), ToastLength.Short).Show();
                    }
                    else
                    {
                        try
                        {
                            if (replyEditText.Text.Length <= 140)
                            {

                                TweetActionAsync actionAsync = new TweetActionAsync(context, TweetActionAsync.ActionType.tweet, this, 0, 0);
                                actionAsync.setreply(replyEditText.Text.ToString());
                                if (imageUrl != null)
                                {
                                    actionAsync.setImageFileToUpload(new File(imageUrl));
                                }
                                actionAsync.Execute();
                                Dismiss();
                            }

                        }
                        catch 
                        {
                        }
                    }
                };

            replyEditText.TextChanged += (s, e) =>
                {
                    if (e.AfterCount > 0)
                    {
                        if (e.Text.Count() <= 140)
                        {
                            tweetButton.Enabled = (true);
                            tweetButton.Clickable = (true);
                        }
                        else
                        {
                            tweetButton.Enabled = (false);
                            tweetButton.Clickable = (false);
                        }


                        charCountTextView.Text = ("" + (140 - e.Text.Count()));

                    }
                };
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.DialogAnimation;
            return promptsView;
        }

        private File getOutputMediaFile(int type)
        {
            // To be safe, you should check that the SDCard is mounted
            // using Environment.getExternalStorageState() before doing this.

            File mediaStorageDir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), GetString(Resource.String.app_name));
            // This location works best if you want the created images to be shared
            // between applications and persist after your app has been uninstalled.

            // Create the storage directory if it does not exist
            if (!mediaStorageDir.Exists())
            {
                if (!mediaStorageDir.Mkdirs())
                {
                    return null;
                }
            }

            // Create a media file name
            String timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            File mediaFile;
            //        if (type == MEDIA_TYPE_IMAGE){
            mediaFile = new File(mediaStorageDir.Path + File.Separator + "IMG_" + timeStamp + ".jpg");

            return mediaFile;

        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == CAPTURE_IMAGE_ACTIVITY_REQUEST_CODE)
            {
                //if (resultCode ==Result)
                //{

                    byte[] cameraData = data.Extras.GetByteArray("data");
                    DisplayActivity.imageToShow = cameraData;
                    Intent intent = new Intent(Activity, typeof(DisplayActivity));
                    intent.PutExtra("imageUrl", saveImage(cameraData));
                    intent.PutExtra("isBackCamera", false);
                    intent.PutExtra("isFlashOn", false);
                    Activity.StartActivity(intent);
                    Activity.Finish();
                //}
            }
        }

      

        public String saveImage(byte[] image)
        {
            File photo = getPhotoPath();

            if (photo.Exists())
            {
                photo.Delete();
            }

            try
            {
                //System.IO.Stream fos = new FileOutputStream(photo.Path);
                //BufferedOutputStream bos = new BufferedOutputStream(fos);

                //bos.Write(image);
                //bos.Flush();
                ////fos.getFD().sync();
                //bos.Close();
            }
            catch (Java.IO.IOException e)
            {
            }
            return photo.AbsolutePath;
        }
        protected File getPhotoPath()
        {
            File dir = getPhotoDirectory();

            dir.Mkdirs();

            return (new File(dir, getPhotoFilename()));
        }

        protected File getPhotoDirectory()
        {
            if (photoDirectory == null)
            {
                initPhotoDirectory();
            }

            return (photoDirectory);
        }

        private void initPhotoDirectory()
        {
            photoDirectory =
                    Android.OS.Environment.GetExternalStoragePublicDirectory(GetString(Resource.String.app_name));
        }
        protected String getPhotoFilename()
        {
            String ts = new SimpleDateFormat("yyyyMMdd_HHmmss", Locale.Us).Format(new Date());

            return ("Photo_" + ts + ".jpg");
        }


        public void setTweetStatusObject(Twitter4j.IStatus result, int position, TweetActionAsync.ActionType type)
        {
            if (result != null)
            {

                Toast.MakeText(context, context.Resources.GetString(Resource.String.success_text), ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(context, context.Resources.GetString(Resource.String.fail_text), ToastLength.Short).Show();
            }

        }
    }
}