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
using Android.Text;
using CommonLayer;
using ConferenceAppDroid.Utilities;
using System.IO;
using Android.Content.Res;
using Java.IO;
using ConferenceAppDroid.Core;

namespace ConferenceAppDroid
{
    [Activity(Label = "SplashScreen", MainLauncher = true, Icon = "@drawable/icon")]
    public class SplashScreen : Activity
    {
        private Context context;
        private TextView copyRightTextView;
        private TextView splash_loadingText;

        //public SplashScreen()
        //    : base(Resource.String.ApplicationName, "")
        //{

        //}
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.activity_splash_screen);
            //SetupAsPerIniFile();
            DBHelper.Instance.CopyDatabaseToLibraryFolder(ApplicationContext, "vmworld_pex_uat.db");
            DataManager.GetConfig(DBHelper.Instance.Connection).ContinueWith(c =>
         {
             DataManager.CreateAllTables(DBHelper.Instance.Connection);
             AppSettings.Instance.config = c.Result;
             DataManager.GetCurrentUser(DBHelper.Instance.Connection).ContinueWith((t2) =>
                {
                    if (t2.Result != null)
                    {
                        AppSettings.Instance.ApplicationUser = t2.Result.application_user;
                        DataManager.SetCurrentUser(AppSettings.Instance.ApplicationUser);
                    }
                    DataManager.GetTracks(DBHelper.Instance.Connection).ContinueWith((t) =>
                      {
                          AppSettings.Instance.AllTracks = t.Result;
                          DataManager.GetbuiltSessionTimeListOfTrack(DBHelper.Instance.Connection).ContinueWith(fs =>
                             {
                                 AppSettings.Instance.FeaturedSessions = fs.Result;
                                 DataManager.GetSessionTracks(DBHelper.Instance.Connection).ContinueWith((t3) =>
                              {
                                  List<string> sessionTracks = t3.Result;
                                  foreach (var item in sessionTracks)
                                  {
                                      if (!AppSettings.Instance.TrackDictionary.ContainsKey(item))
                                          AppSettings.Instance.TrackDictionary.Add(item, t.Result.Where(p => p.parentTrackName == item).ToArray());
                                  }

                                  RunOnUiThread(() =>
                                  {
                                      callVideoIntent();
                                  });
                              });
                             });

                      });
                });
         });
            init();

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
            //for (int i = 0; i < 500; i++)
            //{
            //    if (i == 300)
            //    {
            //        callVideoIntent();
            //        return;
            //    }
            //}
        }

        private void init()
        {
            copyRightTextView = (TextView)FindViewById(Resource.Id.copy_rights_TextView);
            splash_loadingText = (TextView)FindViewById(Resource.Id.splash_loadingText);


            //String normalString = "Copyright &#169; 2014 VMware, Inc. All rights reserved. This product is protected by copyright and intellectual property laws in the United States and other countries as well as by international treaties. VMware products are covered by one or more patents listed at ";
            //String link = normalString + "<a href='com.builtio.vmworld.event://vmwareapp/webview/vmwarelink'>" + "http://www.vmware.com/go/patents." + "</a>";
            //copyRightTextView.Text = Html.FromHtml(link).ToString();
            ////copyRightTextView.MovementMethod=linkm.setMovementMethod(LinkMovementMethod.getInstance());
            //copyRightTextView.Visibility = ViewStates.Visible;
            //FindViewById(Resource.Id.loadingContainerForPex).Visibility = ViewStates.Gone;
            //FindViewById(Resource.Id.loadingContainer).Visibility = ViewStates.Visible;
            FindViewById(Resource.Id.loadingContainerForPex).Visibility = ViewStates.Visible;
            FindViewById(Resource.Id.loadingContainer).Visibility = ViewStates.Gone;
            copyRightTextView.Visibility = ViewStates.Gone;
        }

        private void callVideoIntent()
        {
            Intent intent1 = new Intent(this, typeof(MainActivity));
            StartActivity(intent1);
            Finish();
        }

        void SetupAsPerIniFile()
        {
            var path = CopyINIToLibraryFolder(this, "config.ini");
            INIFile readfile = new INIFile(path, false, true);
            string environment = readfile.GetValue("defaults", "environment", String.Empty);
            string apiKey = readfile.GetValue(environment, "api_key", String.Empty);
            string appUid = readfile.GetValue(environment, "app_uid", String.Empty);
            string credentials_name = readfile.GetValue(environment, "credentials_name", String.Empty);
            DataManager.Initialize(apiKey, appUid, credentials_name);

            var db_name = readfile.GetValue(environment, "db_name", String.Empty);
            if (!String.IsNullOrWhiteSpace(db_name))
                AppSettings.dbFileName = db_name;

            //string appId = readfile.GetValue(environment, "crittercism_key", String.Empty);
            //if (!String.IsNullOrWhiteSpace(appId))
            //    CrittercismIOS.Crittercism.Init(appId);
        }

        public string CopyINIToLibraryFolder(Context c, string filename)
        {
            context = c;
            //---path to Documents folder---
            var documentsPath =
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            //string libraryPath = System.IO.Path.Combine(documentsPath, "..", "Library", "Application Support"); // Library folder

            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            var destinationPath = System.IO.Path.Combine(documentsPath, filename);
            //dbPath = destinationPath;

          

            //---path of source file---
            //var sourcePath =
            //    System.IO.Path.Combine(NSBundle.MainBundle.BundlePath,
            //    filename);

            try
            {
                if (!System.IO.File.Exists(destinationPath))
                {
                    var dbStream = context.Assets.Open(filename);
                    using (FileStream fs = new FileStream(destinationPath, FileMode.OpenOrCreate))
                    {
                        dbStream.CopyTo(fs);
                    }
                }
                else
                {
                  System.Console.WriteLine("File already exists");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return destinationPath;
        }

        //private void copyFile(Stream inn, Stream outt)
        //{
        //    byte[] buffer = new byte[1024];
        //    int read;
        //    while ((read = inn.Read(buffer,0,buffer.Length)) != -1)
        //    {
        //        outt.Write(buffer, 0, read);
        //    }
        //}

    }
}