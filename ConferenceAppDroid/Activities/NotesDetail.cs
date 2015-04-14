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
using Android.Graphics;
using ConferenceAppDroid.Utilities;
using CommonLayer.Entities.Built;
using Android.Text;
using Java.IO;
using CommonLayer;
using ConferenceAppDroid.BroadcastReceivers;
using Android.Provider;
using Android.Content.PM;

namespace ConferenceAppDroid
{
    [Activity(Label = "NotesDetail")]
    public class NotesDetail : Android.App.Activity
    {
        private Context context;
        private View actionBarView;
        private ImageView leftArrowImageView;
        private ImageView rightArrowImageView;
        private ImageView starUnstarImageView;
        private ImageView showTagsImageView;
        private ImageButton leftMenuBtn;
        private ImageButton rightMenuBtn;
        private TextView titleTextView;
        private ImageView bottomImageView;
        private ImageView createNewNoteImageView;
        private ImageView deleteImageView;
        private ProgressDialog progressDialog;
        private TextView saveTextView;
        private TextView deleteTextView;
        private HorizontalScrollView imageHScrollView;
        private HorizontalScrollView tagsHScrollView;
        private HorizontalScrollView tagsSuggestionHScrollView;
        private LinearLayout tagContainer;
        private LinearLayout tagsSuggestionContainer;
        private EditText notesTitle_editText_title;
        private EditText notesDetail_editText;
        private EditText tag_editText;
        private TextView notestime_textView;
        List<string> uploadedUid;
        private String tagString = "";
        List<String> fetchTag = new List<String>();
        private List<String> selectedTagsList = new List<String>();
        private List<String> allTagsList = new List<String>();
        BuiltNotes notesModel;
        List<BuiltNotes> notesModelList = new List<BuiltNotes>();
        private List<String> deletetImagesList = new List<String>();
        private List<String> imagePath = new List<String>();
        string uid = string.Empty;
        bool isNewNotes;
        ImageView imageView;
        Android.Net.Uri uri;
        List<string> lstUploadUid = new List<string>();
        public static File _dir;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            this.context = this;
            OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = actionBarView;
            ActionBar.DisplayOptions = ActionBarDisplayOptions.ShowCustom;
            SetContentView(Resource.Layout.screen_note_detail);
            init();
            uid = Intent.GetStringExtra("uid");

            if (!string.IsNullOrWhiteSpace(uid))
            {
                DataManager.GetNotesByID(DBHelper.Instance.Connection, uid).ContinueWith(t =>
                {
                    notesModelList = t.Result;
                    allTagsList = getAllTags();
                    RunOnUiThread(() =>
                    {
                        showNotesDetail(false);
                        this.isNewNotes = false;

                        titleTextView.Text = (notesModelList[0].title.ToUpper());
                        deleteImageView.Visibility = ViewStates.Visible;
                        deleteTextView.Visibility = ViewStates.Gone;
                    });
                });
            }
            else
            {
                showNotesDetail(true);
                this.isNewNotes = true;
                titleTextView.Text = ("New Note".ToUpper());
                deleteImageView.Visibility = ViewStates.Gone;
                deleteTextView.Visibility = ViewStates.Gone; ;
                //createNewNoteImageView.setOnClickListener(onClickLIstener);
            }
            deleteImageView.Click += (s, e) =>
                {
                    showDeleteDialog();
                };



            leftMenuBtn.Visibility = ViewStates.Gone;
            showTagsImageView.Visibility = ViewStates.Gone;
            rightMenuBtn.Visibility = ViewStates.Invisible;
            bottomImageView.Visibility = ViewStates.Visible;
            bottomImageView.Click += bottomImageView_Click;
            //showTagsImageView.setOnClickListener(onClickLIstener);
            starUnstarImageView.Visibility = ViewStates.Gone;
            createNewNoteImageView.Visibility = ViewStates.Gone;
            rightArrowImageView.Visibility = ViewStates.Gone;
            leftArrowImageView.Visibility = ViewStates.Gone;
        }

        void bottomImageView_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void init()
        {
            leftArrowImageView = (ImageView)FindViewById(Resource.Id.leftArrowImageView);
            rightArrowImageView = (ImageView)FindViewById(Resource.Id.rightArrowImageView);
            starUnstarImageView = (ImageView)FindViewById(Resource.Id.starUnstarImageView);
            leftMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.left_menu_btn);
            rightMenuBtn = (ImageButton)actionBarView.FindViewById(Resource.Id.right_menu_btn);
            titleTextView = (TextView)actionBarView.FindViewById(Resource.Id.titleTextView);
            bottomImageView = (ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView);
            bottomImageView.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.close_icon_selector));
            createNewNoteImageView = (ImageView)FindViewById(Resource.Id.createNewNoteImageView);
            deleteImageView = (ImageView)FindViewById(Resource.Id.deleteImageView);
            showTagsImageView = (ImageView)FindViewById(Resource.Id.tagsImageView);

            progressDialog = new ProgressDialog(context);

            progressDialog.SetCancelable(false);
            //titleTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            titleTextView.SetMinWidth(5);

            LinearLayout actionBottomConatiner = (LinearLayout)FindViewById(Resource.Id.bottomActionContainer);
            actionBottomConatiner.Visibility = ViewStates.Visible;

            saveTextView = (TextView)actionBarView.FindViewById(Resource.Id.doneTextView);
            deleteTextView = (TextView)actionBarView.FindViewById(Resource.Id.deleteTextView);
            saveTextView.Text = "Save";
            //saveTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

            //if(AppConstants.IS_PEX_EVENT){
            saveTextView.SetTextColor(Resources.GetColor(Resource.Color.create_note_save_text_pex));
            saveTextView.Click += saveTextView_Click;
            /*}else{
                saveTextView.setTextColor(getResources().getColor(R.color.white));
            }*/

            saveTextView.Visibility = ViewStates.Visible;
            deleteTextView.Visibility = ViewStates.Gone;


            imageHScrollView = (HorizontalScrollView)FindViewById(Resource.Id.imageHorizontalScrollView);
            tagsHScrollView = (HorizontalScrollView)FindViewById(Resource.Id.tagsHorizontalScrollView);
            tagsSuggestionHScrollView = (HorizontalScrollView)FindViewById(Resource.Id.tagsSuggestionHorizontalScrollView);
            tagContainer = (LinearLayout)FindViewById(Resource.Id.tagContainers);
            tagsSuggestionContainer = (LinearLayout)FindViewById(Resource.Id.tagSuggestContainers);
            notesTitle_editText_title = (EditText)FindViewById(Resource.Id.notesTitle_editText_title);
            notesDetail_editText = (EditText)FindViewById(Resource.Id.notesDetail_editText);
            tag_editText = (EditText)FindViewById(Resource.Id.tag_edit_text);
            //tag_editText.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));

            notestime_textView = (TextView)FindViewById(Resource.Id.notestime_textView);

            //notesTitle_editText_title.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));
            //notesDetail_editText.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            //notestime_textView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));




        }

        void saveTextView_Click(object sender, EventArgs e)
        {
            SaveNote();
        }
        private void showDeleteDialog()
        {
            new Android.App.AlertDialog.Builder(this).SetTitle("Delete selected item").SetMessage("Are you sure you want to permanently delete this item?")
                               .SetPositiveButton("Yes", (sender, args) =>
                               {
                                   DeleteNote();
                               })
                               .SetNegativeButton("No", (sender, args) =>
                               {

                               }).Show();
        }

        void SaveNote()
        {
            if (notesModel == null)
            {
                notesModel = new BuiltNotes
                {
                    title = notesTitle_editText_title.Text,
                    content = notesDetail_editText.Text,
                    tags = allTagsList.ToArray()
                };
            }
            else
            {
                notesModel.title = notesTitle_editText_title.Text;
                notesModel.content = notesDetail_editText.Text;
                notesModel.tags = allTagsList.ToArray();
            }
            DataManager.saveNote(DBHelper.Instance.Connection, notesModel, (err) =>
            {
                if (err == null)
                {
                    RunOnUiThread(() =>
                    {
                        showSaveDialog("Saved!");
                        Intent intent = new Intent(NotesReceiver.action);
                        SendBroadcast(intent);
                        base.OnBackPressed();
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        showSaveDialog("Failed!");
                    });
                }
            });
        }

        void DeleteNote()
        {
            ProgressDialog progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage("Please wait…");
            progressDialog.Show();
            DataManager.deleteNote(DBHelper.Instance.Connection, notesModel.uid, (err) =>
            {
                if (err == null)
                {
                    RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();

                        showSaveDialog("Deleted!");
                        Intent intent = new Intent(NotesReceiver.action);
                        SendBroadcast(intent);
                        base.OnBackPressed();
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        progressDialog.Dismiss();
                        showSaveDialog("Failed!");

                    });
                }
            });
        }

        private List<string> getAllTags()
        {
            List<String> arrayList = new List<String>();
            notesModel = notesModelList[0];
            if (!string.IsNullOrWhiteSpace(notesModel.tags_separarated) && notesModel.tags_separarated.Contains("|"))
            {
                arrayList = notesModel.tags_separarated.Split('|').ToList();
            }
            else
            {
                arrayList.Add(notesModel.tags_separarated);
            }
            return arrayList;
        }

        public void showSaveDialog(String message)
        {
            Toast.MakeText(context, message, ToastLength.Short).Show();
        }

        public bool checkForExist(String fetchTag, String keyword)
        {
            bool makeTrue = false;
            if (selectedTagsList != null && selectedTagsList.Count > 0)
            {
                for (int i = 0; i < selectedTagsList.Count; i++)
                {
                    String temp = selectedTagsList[i];
                    if (temp.Trim().Equals(fetchTag.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        makeTrue = true;
                    }
                }
            }
            if (makeTrue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateBubble(String tagName, HorizontalScrollView horizantalScrollView, View searchView)
        {

            View attachmentButtonView = LayoutInflater.From(context).Inflate(Resource.Layout.view_selection_button, null);
            ImageView closeImageView = (ImageView)attachmentButtonView.FindViewById(Resource.Id.tag_close_imageview);
            TextView bubbleName = (TextView)attachmentButtonView.FindViewById(Resource.Id.res);
            //bubbleName.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
            bubbleName.Visibility = ViewStates.Visible;
            bubbleName.Text = ("  " + tagName + "  ");
            bubbleName.SetTextColor(Color.ParseColor("#448ac9"));
            attachmentButtonView.SetPadding(Helper.dpToPx(context, 3), Helper.dpToPx(context, 1), Helper.dpToPx(context, 2), Helper.dpToPx(context, 1));
            tagContainer.AddView(attachmentButtonView);
            selectedTagsList.Add(bubbleName.Text.ToString());

            bubbleName.Click += (s, e) =>
                {
                    if (closeImageView.Visibility == ViewStates.Gone)
                    {
                        closeImageView.SetBackgroundColor(context.Resources.GetColor(Android.Resource.Color.Transparent));
                        closeImageView.Visibility = ViewStates.Visible;
                        closeImageView.SetPadding(Helper.dpToPx(context, 3), Helper.dpToPx(context, 1), Helper.dpToPx(context, 2), Helper.dpToPx(context, 1));
                        bubbleName.SetBackgroundColor(Color.ParseColor("#eef3f8"));
                        bubbleName.SetTextColor(Color.ParseColor("#448ac9"));
                        tag_editText.ClearFocus();
                    }
                };


            closeImageView.Click += (u, k) =>
                {
                    String tag = bubbleName.Text.ToString().Trim();
                    for (int i = 0; i < allTagsList.Count; i++)
                    {
                        if (tag.Equals((allTagsList[i].ToString().Trim()), StringComparison.InvariantCultureIgnoreCase))
                        {
                            allTagsList.RemoveAt(i);
                            break;
                        }
                    }
                    int count = selectedTagsList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        //int id = selectedTagsList.indexOf(tag);
                        if (tag.Equals((selectedTagsList[i].ToString().Trim()), StringComparison.InvariantCultureIgnoreCase))
                        {
                            selectedTagsList.RemoveAt(i);
                            break;
                        }
                    }
                    tagContainer.RemoveView(attachmentButtonView);
                };


        }


        private void showNotesDetail(bool isNewNotes)
        {

            uploadedUid = new List<string>();

            notesTitle_editText_title.FocusChange += (s, e) =>
                {
                    if (e.HasFocus)
                    {
                        if (tagsSuggestionHScrollView.Visibility == ViewStates.Visible)
                        {
                            tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                        }
                    }
                };

            notesDetail_editText.FocusChange += (s, e) =>
            {
                if (e.HasFocus)
                {
                    if (tagsSuggestionHScrollView.Visibility == ViewStates.Visible)
                    {
                        tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                    }
                }
            };

            tag_editText.TextChanged += (s, e) =>
                {
                    tagString = e.Text.ToString();
                    tagsSuggestionContainer.RemoveAllViews();
                    //fetchTag = getAllTags(e.Text.ToString());
                    for (int i = 0; i < fetchTag.Count; i++)
                    {
                        if (fetchTag[i].Contains(e.Text.ToString()))
                        {
                            try
                            {
                                if (!checkForExist(fetchTag[i], e.Text.ToString()))
                                {

                                    if (tagsSuggestionHScrollView.Visibility != ViewStates.Visible)
                                    {
                                        tagsSuggestionHScrollView.Visibility = ViewStates.Visible;
                                    }

                                    TextView textView = new TextView(context);
                                    textView.Text = (fetchTag[i]);
                                    textView.SetTextColor(Color.ParseColor("#448ac9"));
                                    textView.SetPadding(Helper.dpToPx(context, 10), Helper.dpToPx(context, 5), Helper.dpToPx(context, 10), Helper.dpToPx(context, 5));
                                    //textView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT));
                                    tagsSuggestionContainer.AddView(textView);

                                    textView.Click += (t, u) =>
                                        {
                                            tag_editText.Text = ("");
                                            tag_editText.ClearFocus();
                                            CreateBubble(textView.Text.ToString(), tagsHScrollView, tag_editText);
                                        };

                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                };




            tag_editText.BeforeTextChanged += (s, e) =>
                {

                };

            tag_editText.AfterTextChanged += (s, e) =>
                {
                    if (tagString.Contains(" ") && !string.IsNullOrWhiteSpace(tagString.ToString().Trim()))
                    {
                        if (!allTagsList.Contains(tagString) && !fetchTag.Contains(tagString))
                        {
                            allTagsList.Add(tagString);
                        }

                        CreateBubble(tagString, tagsHScrollView, tag_editText);
                        tag_editText.Text = ("");
                        tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                    }

                    if (e.Editable.Length() == 0)
                    {
                        tagsSuggestionContainer.RemoveAllViews();
                        tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                    }
                };

            if (!isNewNotes)
            {
                notesModel = notesModelList[0];
                String[] strings = null;
                List<string> temp = new List<string>();
                if (notesModel.photos != null && notesModel.photos.Count > 0)
                {
                    for (int i = 0; i < notesModel.photos.Count; i++)
                    {
                        temp.Add(notesModel.photos[i].url);
                    }
                    strings = temp.ToArray();
                }
                else if (notesModel.photos != null && notesModel.photos.Count > 0)
                {
                    for (int i = 0; i < notesModel.photos.Count; i++)
                    {
                        strings = new String[1];
                        strings[0] = notesModel.photos[i].url;
                    }

                }


                String[] stringsPhotoUID = null;
                List<string> tempUID = new List<string>();
                if (notesModel.photos != null && notesModel.photos.Count > 0)
                {
                    for (int i = 0; i < notesModel.photos.Count; i++)
                    {
                        tempUID.Add(notesModel.photos[i].uid);
                    };
                    stringsPhotoUID = tempUID.ToArray();
                }
                else if (notesModel.photos != null && notesModel.photos.Count > 0)
                {
                    for (int i = 0; i < notesModel.photos.Count; i++)
                    {
                        stringsPhotoUID = new String[1];
                        stringsPhotoUID[0] = notesModel.photos[i].uid;
                    }
                }

                if (stringsPhotoUID != null)
                {
                    for (int i = 0; i < stringsPhotoUID.Length; i++)
                    {
                        uploadedUid.Add(stringsPhotoUID[i]);
                    }
                }

                List<String> arrayList = new List<String>();
                if (strings != null)
                {
                    for (int i = 0; i < strings.Length; i++)
                    {
                        arrayList.Add(strings[i]);
                    }
                }
                arrayList.Add("qwerty");
                lstUploadUid = arrayList;

                createImageScroller(imageHScrollView, arrayList, uploadedUid);


                if (notesModel.title != null)
                {
                    notesTitle_editText_title.Text = (notesModel.title);
                }

                if (notesModel.updated_at != null)
                {
                    //notestime_textView.setText(context.getResources().getString(R.string.last_edited_text) + " " + AppUtilities.extractDate(context, notesModel.updatedAt));
                }

                if (notesModel.content != null)
                {
                    notesDetail_editText.Text = (Html.FromHtml(notesModel.content)).ToString();
                }

                String[] tagStrings = null;

                if (!string.IsNullOrWhiteSpace(notesModel.tags_separarated) && notesModel.tags_separarated.Contains("|"))
                {
                    tagStrings = notesModel.tags_separarated.Split('|');

                    for (int i = 0; i < tagStrings.Length; i++)
                    {
                        CreateBubble(tagStrings[i], tagsHScrollView, tag_editText);
                    }

                }
                else if (!string.IsNullOrWhiteSpace(notesModel.tags_separarated))
                {
                    CreateBubble(notesModel.tags_separarated, tagsHScrollView, tag_editText);
                }

            }
            else
            {

                List<String> arrayList = new List<String>();
                arrayList.Add("qwerty");
                createImageScroller(imageHScrollView, arrayList, uploadedUid);
            }

        }

        private const int FILE_SELECT_CODE = 0;
        private void ShowFileChooser()
        {
            Intent intent = new Intent(Intent.ActionGetContent);

            AlertDialog.Builder builderSingle = new AlertDialog.Builder(this).SetTitle("Choose Options");
            builderSingle.SetIcon(Android.Resource.Drawable.ArrowDownFloat);
            ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SelectDialogItem);
            arrayAdapter.AddAll(new[] { "Choose from gallery", "Take a Photo" }.ToList());
            builderSingle.SetCancelable(true);

            builderSingle.SetAdapter(arrayAdapter, (sender, e) =>
            {
                if (e.Which == 0)
                {
                    intent.SetType("image/*");
                }
                else
                {
                    if (IsThereAnAppToTakePictures())
                    {
                        CreateDirectoryForPictures();
                    }
                }
                try
                {
                    StartActivityForResult(Intent.CreateChooser(intent, "Select a File to Upload"), FILE_SELECT_CODE);
                }
                catch (Android.Content.ActivityNotFoundException ex)
                {
                    Toast.MakeText(this, "Please install a File Manager.", ToastLength.Short).Show();
                }
            }).Show();
        }

        private void CreateDirectoryForPictures()
        {
            _dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            var file = new File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));

            StartActivityForResult(intent, 1);
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case FILE_SELECT_CODE:
                    if (resultCode == Result.Ok)
                    {
                        // Get the Uri of the selected file 
                        uri = data.Data;
                        // Get the path
                        var path=GetPath();
                        UploadFile(path);
                    }
                    break;
                case 1:
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    Android.Net.Uri contentUri = Android.Net.Uri.FromFile(_dir);
                    mediaScanIntent.SetData(contentUri);
                    SendBroadcast(mediaScanIntent);
                    var cameraPath=GetPath();
                    UploadFile(cameraPath);
                    break;
                default:
                    break;
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        string GetPath()
        {
            string path = getRealPathFromURI(uri);

            if (path == null)
            {
                Handler h = new Handler(MainLooper);
                h.Post(() =>
                {
                    Toast.MakeText(ApplicationContext, "Error while uploading file!", ToastLength.Short).Show();
                    
                });
                return "";
            }


            // Get the file instance
            var file = new Java.IO.File(path);
            // Get length of file in bytes
            double fileSizeInBytes = file.Length();
            // Convert the bytes to Kilobytes (1 KB = 1024 Bytes)
            var fileSizeInKb = fileSizeInBytes / 1024;
            // Convert the KB to MegaBytes (1 MB = 1024 KBytes)
            var fileSizeInMb = fileSizeInKb / 1024;

            //Initiate the upload
            string filename = System.IO.Path.GetFileName(path);
            return path;
        }

        private void UploadFile(string path)
        {
            var data = System.IO.File.ReadAllBytes(path);
            DataManager.UploadFile(path, data, AppSettings.Instance.ApplicationUser, res =>
                {
                    if (res == null)
                        return;

                    if (notesModel == null)
                        notesModel = new BuiltNotes();

                    if (notesModel.photos == null)
                        notesModel.photos = new List<NotePhotos>();

                    notesModel.photos.Add(new NotePhotos
                    {
                        uid = res.result[0].uid,
                        url = res.result[0].url,
                    });
                    RunOnUiThread(() =>
                        {
                            lstUploadUid.RemoveAt(lstUploadUid.Count - 1);
                            lstUploadUid.Add(res.result[0].url);
                            lstUploadUid.Add("qwerty");
                            uploadedUid.Add(res.result[0].uid);
                            createImageScroller(imageHScrollView, lstUploadUid, uploadedUid);
                        });

                });
        }

        public String getRealPathFromURI(Android.Net.Uri contentUri)
        {
            String[] projection = new String[] 
            { 
          Android.Provider. MediaStore.MediaColumns.Data
            };
            ContentResolver cr = this.ContentResolver;
            Android.Database.ICursor cursor = cr.Query(contentUri, projection, null, null, null);
            if (cursor != null && cursor.Count > 0)
            {
                cursor.MoveToFirst();
                var index = cursor.GetColumnIndex(Android.Provider.MediaStore.MediaColumns.Data);
                return cursor.GetString(index);
            }
            return null;
        }

        private void createImageScroller(HorizontalScrollView imageHScrollView, List<String> arrayList, List<String> uploadedUID)
        {
            try
            {

                if (imageHScrollView.ChildCount > 0)
                {
                    imageHScrollView.RemoveAllViews();
                }
                LinearLayout linearView = new LinearLayout(context);
                LinearLayout.LayoutParams paramss = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                paramss.RightMargin = 2;
                linearView.SetBackgroundColor(context.Resources.GetColor(Resource.Color.light_blue));
                linearView.SetPadding(Helper.dpToPx(context, 5), Helper.dpToPx(context, 5), Helper.dpToPx(context, 5), Helper.dpToPx(context, 5));
                linearView.LayoutParameters = (paramss);

                int count = arrayList.Count;
                for (int i = 0; i < count; i++)
                {
                    View rowImagePager = LayoutInflater.From(context).Inflate(Resource.Layout.view_image_adapter, null);

                    RelativeLayout.LayoutParams viewParams = new RelativeLayout.LayoutParams(Android.Widget.RelativeLayout.LayoutParams.WrapContent, Android.Widget.RelativeLayout.LayoutParams.WrapContent);
                    viewParams.RightMargin = 5;
                    rowImagePager.LayoutParameters = (viewParams);
                    imageView = (ImageView)rowImagePager.FindViewById(Resource.Id.imageView);
                    ImageView cancelImageView = (ImageView)rowImagePager.FindViewById(Resource.Id.cancelImageView);
                    TextView nameTextView = (TextView)rowImagePager.FindViewById(Resource.Id.nameTextView);

                    cancelImageView.Click += (m, n) =>
                        {
                            if (tagsSuggestionHScrollView.Visibility == ViewStates.Visible)
                            {
                                tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                            }
                            String op = (String)cancelImageView.Tag;
                            imagePath.Remove(op);
                            notesModel.photos.RemoveAll(p => p.uid == lstUploadUid[(Convert.ToInt32(op))]);
                            arrayList.Remove(op);
                            createImageScroller(imageHScrollView, arrayList, uploadedUID);
                        };

                    //nameTextView.setTypeface(Typeface.createFromAsset(context.getAssets(), AppConstants.FONT_MEDIUM));

                    if (arrayList[i] != null)
                    {

                        if (arrayList[i].Contains("qwerty"))
                        {
                            imageView.SetImageDrawable(context.Resources.GetDrawable(Resource.Drawable.ic_add_session));
                            imageView.Click += (s, e) =>
                                {
                                    if (tagsSuggestionHScrollView.Visibility == ViewStates.Visible)
                                    {
                                        tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                                    }
                                    try
                                    {
                                        ShowFileChooser();

                                        //builtUIPickerController.showPicker(false);
                                    }
                                    catch
                                    {
                                        //AppUtilities.showError("MyNotesDetailPagerAdapter", e);
                                    }
                                };


                            cancelImageView.Visibility = ViewStates.Gone;
                            nameTextView.Visibility = ViewStates.Visible;

                        }
                        else
                        {

                            File imgFile = new File(arrayList[i]);
                            if (imgFile.Exists())
                            {

                                (rowImagePager.FindViewById(Resource.Id.notes_progress_bar)).Visibility = ViewStates.Gone;

                                String path = imgFile.AbsolutePath;

                                if (imgFile.Exists())
                                {
                                    try
                                    {
                                        //aQuery.id(imageView).image(new File(path), 200);
                                    }
                                    catch
                                    {

                                    }
                                }
                                UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(imageView, path);

                                //imageView.setOnClickListener(new OnClickListener() {
                                //    @Override
                                //    public void onClick(View v) {
                                //        if (tagsSuggestionHScrollView.getVisibility() == View.VISIBLE) {
                                //            tagsSuggestionHScrollView.setVisibility(View.GONE);
                                //        }
                                //        Intent intent = new Intent(context, UIAQueryImagePreview.class);
                                //        intent.putExtra("url", path);
                                //        intent.putExtra("title", notesTitle_editText_title.getText().toString());
                                //        intent.putExtra("Local", "FromLocal");
                                //        context.startActivity(intent);
                                //    }
                                //});

                                cancelImageView.Tag = (arrayList[i]);
                                cancelImageView.Click += (u, s) =>
                                    {
                                        if (tagsSuggestionHScrollView.Visibility == ViewStates.Visible)
                                        {
                                            tagsSuggestionHScrollView.Visibility = ViewStates.Gone;
                                        }
                                        String op = (String)cancelImageView.Tag;
                                        imagePath.Remove(op);
                                        notesModel.photos.RemoveAll(p => p.uid == (uploadedUID[Convert.ToInt32(op)]));
                                        arrayList.Remove(op);
                                        createImageScroller(imageHScrollView, arrayList, uploadedUID);
                                    };

                            }
                            else
                            {
                                if (arrayList[i] != null)
                                {
                                    if (arrayList[i].Contains("http://") || arrayList[i].Contains("https://"))
                                    {
                                        try
                                        {

                                            (rowImagePager.FindViewById(Resource.Id.notes_progress_bar)).Visibility = ViewStates.Gone;
                                            if (AppSettings.Instance.ApplicationUser != null)
                                            {
                                                arrayList[i] += "?AUTHTOKEN=" + AppSettings.Instance.ApplicationUser.authtoken;
                                            }
                                            var imageUrl = arrayList[i];
                                            UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(imageView, arrayList[i], Resource.Drawable.ic_default_pic);
                                            imageView.Click += (s, e) =>
                                                {
                                                    Intent intent = new Intent(this, typeof(ImagePreviewActivity));
                                                    intent.PutExtra("url", imageUrl);
                                                    intent.PutExtra("title", notesTitle_editText_title.Text.ToString());
                                                    StartActivity(intent);
                                                };
                                            //UrlImageViewHelper.UrlImageViewHelper.SetUrlDrawable(imageView, path);
                                            //final BitmapAjaxCallback bitmapAjax = new BitmapAjaxCallback() {
                                            //    @Override
                                            //    protected void callback(String url, ImageView iv, Bitmap bm, AjaxStatus status) {
                                            //        super.callback(url, iv, bm, status);
                                            //        if (bm != null) {
                                            //            iv.setImageBitmap(bm);
                                            //        }
                                            //    }
                                            //};
                                            //bitmapAjax.header("authtoken", BuiltUser.getSession().getAuthToken());
                                            //bitmapAjax.url(arrayList.get(i)).memCache(false).fileCache(true).targetWidth(100);
                                            //aQuery.id(imageView).progress(R.id.notes_progress_bar).image(bitmapAjax);

                                            //imageView.setOnClickListener(new OnClickListener() {
                                            //    @Override
                                            //    public void onClick(View v) {
                                            //        Intent intent = new Intent(context, UIAQueryImagePreview.class);
                                            //        intent.putExtra("url", bitmapAjax.getUrl());
                                            //        intent.putExtra("title", notesTitle_editText_title.getText().toString());
                                            //        context.startActivity(intent);
                                            //    }
                                            //});

                                            cancelImageView.Tag = (i);
                                            cancelImageView.Click += (a, b) =>
                                                {
                                                    int op = (int)cancelImageView.Tag;
                                                    deletetImagesList.Add(uploadedUID[op]);
                                                    notesModel.photos.RemoveAll(p => p.uid == (uploadedUID[op]));
                                                    arrayList.RemoveAt(op);
                                                    createImageScroller(imageHScrollView, arrayList, uploadedUID);
                                                };


                                        }
                                        catch (Exception e)
                                        {
                                        }
                                    }
                                    cancelImageView.Visibility = ViewStates.Visible;

                                }
                                else
                                {

                                }
                            }

                        }
                        linearView.AddView(rowImagePager);
                    }
                }

                //setImageArrayList(arrayList);

                if (linearView.ChildCount > 0)
                {
                    imageHScrollView.AddView(linearView);
                }

            }
            catch { }
        }

        private object getAllTags(string tagString)
        {
            throw new NotImplementedException();
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

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.up_from_bottom, Resource.Animation.hold);
        }

    }
}