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
using SQLite.Net;
using System.IO;
using SQLite.Net.Platform.XamarinAndroid;
using SQLite.Net.Async;

namespace ConferenceAppDroid.Utilities
{
    public class DBHelper
    {
        private static DBHelper db;
        public Context context;
        private SQLiteConnectionPool pool;
        private SQLiteConnectionString cs;
        SQLiteAsyncConnection asyncConnection;

        private DBHelper()
        { }

        public static DBHelper Instance
        {
            get
            {
                if (db == null)
                    db = new DBHelper();
                return db;
            }
        }

        public void CopyDatabaseToLibraryFolder(Context c, string filename)
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

            pool = new SQLiteConnectionPool(new SQLitePlatformAndroid());
            cs = new SQLiteConnectionString(destinationPath, false);

            //---path of source file---
            //var sourcePath =
            //    System.IO.Path.Combine(NSBundle.MainBundle.BundlePath,
            //    filename);

            try
            {
                if (!File.Exists(destinationPath))
                {
                    var dbStream = context.Assets.Open(filename);
                    using (FileStream fs = new FileStream(destinationPath, FileMode.OpenOrCreate))
                    {
                        dbStream.CopyTo(fs);
                    }
                }
                else
                {
                    Console.WriteLine("File already exists");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public SQLiteConnection Connection
        {
            get
            {
                return pool.GetConnection(cs);
            }
        }

        public SQLiteAsyncConnection AsyncConnection
        {
            get
            {
                asyncConnection = new SQLiteAsyncConnection(() => { return pool.GetConnection(cs); });
                return asyncConnection;
            }
        }
    }
}