using BuiltSDK;
using CommonLayer.Entities.Built;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLiteNetExtensions.Extensions;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Reflection;
using Newtonsoft.Json.Linq;
using CommonLayer.Entities;
using System.Linq.Expressions;

namespace CommonLayer
{
    public partial class DataManager
    {
        static int skip = 0, limit = 800;
        const short totalFetchCalls = 21;
        static short completedFetchCalls;
        public const string internet_required_error = "No internet connection";

        public async static void fetchAllAsync(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            try
            {
                var callsCount = totalFetchCalls;
                completedFetchCalls = 0;

                fetchAchievements(connection, (res) => { completedFetchCalls++; });
                fetchAgenda(connection, (res) => { completedFetchCalls++; });
                fetchExhibitor(connection, (res) => { completedFetchCalls++; });
                fetchFoodAndDrink(connection, (res) => { completedFetchCalls++; });
                fetchHandsOnLabs(connection, (res) => { completedFetchCalls++; });
                fetchImportantLinks(connection, (res) => { completedFetchCalls++; });
                fetchIntro(connection, (res) => { completedFetchCalls++; });
                fetchLegal(connection, (res) => { completedFetchCalls++; });
                fetchMenu(connection, (res) => { completedFetchCalls++; });
                fetchSettingsMenu(connection, (res) => { completedFetchCalls++; });

                fetchNews(connection, (res) => { completedFetchCalls++; });
                fetchOthers(connection, (res) => { completedFetchCalls++; });
                fetchPopularSessions(connection, (res) => { completedFetchCalls++; });
                fetchQrcodes(connection, (res) => { completedFetchCalls++; });
                fetchSpeaker(connection, (res) => { completedFetchCalls++; });
                fetchTransportation(connection, (res) => { completedFetchCalls++; });
                fetchVenue(connection, (res) => { completedFetchCalls++; });
                fetchEventNotification(connection, (res) => { completedFetchCalls++; });
                //fetchNotes(connection, (res) => { completedFetchCalls++; }); //skip
                //fetchMyActivity(connection, (res) => { completedFetchCalls++; });

                while (completedFetchCalls != callsCount - 3)
                {
                    await Task.Delay(1000);
                }

                fetchConfig(connection, (res) =>
                {
                    completedFetchCalls++;

                    fetchSessions(connection, (sessionRes) =>
                    {
                        completedFetchCalls++;

                        fetchTrack(connection, (resTrack) =>
                        {
                            completedFetchCalls++;

                            createLastActivityTable(connection, result =>
                            {
                                if (callbackHandler != null)
                                    callbackHandler(true);
                            });
                        });
                    });
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (callbackHandler != null)
                    callbackHandler(false);
            }
        }

        private static void createLastActivityTable(SQLiteConnection connection, Action<bool> callback)
        {
            try
            {
                ApiCalls.getLastActivities<BuiltLastActivity>(async success =>
                {
                    var config = connection.Get<BuiltConfig>(p => p.uid == p.uid);

                    connection.CreateTable<BuiltLastActivity>();
                    var uids = GetUidTypesDictionary().Keys.ToArray();
                    BuiltLastActivity builtLastActivity = null;
                    for (int i = 0; i < uids.Length; i++)
                    {
                        builtLastActivity = new BuiltLastActivity
                        {
                            uid = uids[i],
                            last_activity = config.last_synced_delta_time
                        };
                        connection.Insert(builtLastActivity);
                    }

                    if (callback != null)
                        callback(true);

                }, (error) =>
                {
                    if (callback != null)
                        callback(false);
                });
            }
            catch
            {
                if (callback != null)
                    callback(false);
            }
        }

        public static void fetchSessions(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchSession<BuiltSession>(skip, limit, (success, totalCount) =>
            {
                try
                {
                    int insertedCount = 0;

                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltSession>();
                        connection.CreateTable<BuiltSessionFile>();
                        connection.CreateTable<BuiltSessionTime>();
                        connection.CreateTable<BuiltSessionSpeaker>();
                        connection.CreateTable<BuiltTracks>();

                        var result = (BuiltSession[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {


                                    var session = result[i];


                                    session.audience_separated = getSeparatedString(session.audience);
                                    session.program_location_separated = getSeparatedString(session.program_location);
                                    session.role_separated = getSeparatedString(session.role);
                                    session.skill_level_separated = getSeparatedString(session.skill_level);
                                    session.solutions_separated = getSeparatedString(session.solutions);
                                    session.sub_track_separated = getSeparatedString(session.sub_track);

                                    for (int x = 0; x < result[i].session_time.Count; x++)
                                    {
                                        session.session_time[x].start_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(session.session_time[x].date, session.session_time[x].time));
                                        session.session_time[x].end_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(session.session_time[x].date, session.session_time[x].time).AddMinutes(Convert.ToInt32(session.session_time[x].length)));
                                    }


                                    if (!String.IsNullOrEmpty(session.track))
                                    {
                                        var trackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ?", session.track);
                                        if (trackCount == 0)
                                        {
                                            BuiltTracks track = new BuiltTracks();
                                            track.name = session.track;
                                            track.color = "242424";
                                            connection.InsertWithChildren(track);
                                        }


                                        foreach (var item in session.sub_track_separated.Split('|'))
                                        {
                                            if (!String.IsNullOrEmpty(item))
                                            {
                                                var subtrackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ? and parentTrackName != ''", item);
                                                if (subtrackCount == 0)
                                                {
                                                    BuiltTracks subtrack = new BuiltTracks();
                                                    subtrack.name = item;
                                                    subtrack.parentTrackName = session.track;
                                                    connection.InsertWithChildren(subtrack);
                                                }
                                            }
                                        }
                                    }

                                    connection.InsertWithChildren(session, true);
                                }
                                catch (Exception ex)
                                {
                                    totalCount--;
                                    Console.WriteLine(ex.StackTrace);
                                }
                            }
                        }

                        insertedCount = connection.ExecuteScalar<int>("select count(*) from BuiltSession");
                    }

                    if (insertedCount < totalCount)
                    {
                        skip += limit;
                        fetchSessions(connection, callbackHandler);
                    }
                    else
                    {
                        skip = 0;
                        if (callbackHandler != null)
                            callbackHandler(true);
                    }
                }
                catch (Exception ex)
                {
                    skip = 0;
                    Console.WriteLine(ex.Message);

                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                skip = 0;
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchAchievements(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchAchievements<BuiltAchievements>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltAchievements>();
                        connection.CreateTable<BuiltAchievementsImage>();

                        var result = (BuiltAchievements[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltAchievements where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchAgenda(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchAgenda<BuiltAgenda>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltAgenda>();
                        connection.CreateTable<BuiltAgendaItem>();

                        var result = (BuiltAgenda[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltAgenda where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }


                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchConfig(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchConfig<BuiltConfig>(success =>
            {

                try
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltConfig>();
                        connection.CreateTable<BuiltExplore>();
                        connection.CreateTable<BuiltConfigMenu>();
                        connection.CreateTable<BuiltConfigSocial>();
                        connection.CreateTable<BuiltConfigTwitter>();
                        connection.CreateTable<BuiltAllFeeds>();
                        connection.CreateTable<BuiltSponsors>();
                        connection.CreateTable<BuiltOrderingInfo>();
                        connection.CreateTable<BuiltConfigImportantLinks>();
                        connection.CreateTable<BuiltImpLinkOrdering>();
                        connection.CreateTable<BuiltHOLCategoryOrder>();
                        connection.CreateTable<BuiltHOLCategoryOrderElement>();
                        connection.CreateTable<BuiltGame>();

                        var result = (BuiltConfig[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    if (destinationTimeZone == null)
                                        destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(result[i].timezone ?? "US/Pacific");


                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltConfig where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;

                                    result[i].menu.left_menu_separated = getSeparatedString(result[i].menu.left_menu);
                                    result[i].menu.right_menu_separated = getSeparatedString(result[i].menu.right_menu);
                                    result[i].explore.banner_details_separated = getSeparatedString(result[i].explore.banner_details);

                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }

                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    if (callbackHandler != null)
                        callbackHandler(false);
                }

            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchEventNotification(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchEventNotification<BuiltEventNotifications>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltEventNotifications>();
                        var result = (BuiltEventNotifications[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltEventNotifications where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }

                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchExhibitor(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchExhibitor<BuiltExhibitor>(skip, limit, (success, totalCount) =>
            {
                try
                {
                    int insertedCount = 0;

                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltExhibitor>();
                        connection.CreateTable<BuiltExhibitorFile>();
                        connection.CreateTable<BuiltParticipant>();

                        var result = (BuiltExhibitor[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltExhibitor where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                        insertedCount = connection.ExecuteScalar<int>("select count(*) from BuiltExhibitor");
                    }

                    if (insertedCount < totalCount)
                    {
                        skip += limit;
                        fetchExhibitor(connection, callbackHandler);
                    }
                    else
                    {
                        skip = 0;
                        if (callbackHandler != null)
                            callbackHandler(true);
                    }
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchFoodAndDrink(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchFoodAndDrink<BuiltSFFoodNDrink>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        connection.DropTable<BuiltSFFoodNDrink>();
                        connection.DropTable<BuiltSFFoodNDrinkImage>();

                        connection.CreateTable<BuiltSFFoodNDrink>();
                        connection.CreateTable<BuiltSFFoodNDrinkImage>();
                        connection.CreateTable<FDLinkGroup>();

                        var result = (BuiltSFFoodNDrink[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltSFFoodNDrink where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchHandsOnLabs(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchHandsOnLabs<BuiltHandsonLabs>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltHandsonLabs>();

                        var result = (BuiltHandsonLabs[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltHandsonLabs where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;

                                    result[i].speaker_separated = getSeparatedString(result[i].speaker);

                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchImportantLinks(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchImportantLinks<BuiltImportantLinks>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltImportantLinks>();
                        connection.CreateTable<BuiltImpLink>();

                        var result = (BuiltImportantLinks[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltImportantLinks where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchIntro(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchIntro<BuiltIntro>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltIntro>();
                        connection.CreateTable<BuiltIntroBgImage>();

                        var result = (BuiltIntro[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltIntro where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchLegal(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchLegal<BuiltLegal>(success =>
            {

                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltLegal>();
                        connection.CreateTable<GamingRules>();
                        connection.CreateTable<LicenseAgreement>();
                        connection.CreateTable<OpenSourceLicenseAndroid>();
                        connection.CreateTable<OpenSourceLicenseIos>();
                        connection.CreateTable<Policy>();
                        connection.CreateTable<Tnc>();
                        connection.CreateTable<TrademarkCopyright>();

                        var result = (BuiltLegal[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltLegal where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchMenu(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchMenu<BuiltMenu>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltMenu>();
                        connection.CreateTable<BuiltSection>();
                        connection.CreateTable<BuiltSectionItems>();
                        connection.CreateTable<MenuIconFile>();
                        connection.CreateTable<MenuIconSelectedFile>();

                        var result = (BuiltMenu[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltMenu where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchMyActivity(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchMyActivity<BuiltMyActivity>(currentUser, success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltMyActivity>();

                        var result = (BuiltMyActivity[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltMyActivity where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchNews(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchNews<BuiltNews>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltNews>();
                        connection.CreateTable<BuiltNewsLink>();
                        connection.CreateTable<CoverImageFile>();

                        var result = (BuiltNews[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    //result[i].published_date = ToDateStringWithAppTimezone(Convert.ToDateTime(result[i].published_date));

                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltNews where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;

                                    if (result[i].link != null)
                                    {
                                        if (String.IsNullOrWhiteSpace(result[i].link.title))
                                            result[i].link.title = result[i].link.href;
                                    }

                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchNotes(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchNotes<BuiltNotes>(currentUser, success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltNotes>();
                        connection.CreateTable<NotePhotos>();

                        var result = (BuiltNotes[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltNotes where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;

                                    result[i].tags_separarated = getSeparatedString(result[i].tags);

                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchOthers(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchOthers<BuiltOthers>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltOthers>();

                        var result = (BuiltOthers[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltOthers where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchPopularSessions(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchPopularSessions<BuiltPopularSessions>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltPopularSessions>();

                        var result = (BuiltPopularSessions[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltPopularSessions where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;

                                    result[i].session_separated = getSeparatedString(result[i].session);

                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }

                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchQrcodes(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchQrcodes<BuiltQRCodes>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltQRCodes>();

                        var result = (BuiltQRCodes[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltQRCodes where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i]);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchSpeaker(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchSpeaker<BuiltSpeaker>(skip, limit, (success, totalCount) =>
            {
                try
                {
                    int insertedCount = 0;

                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltSpeaker>();
                        connection.CreateTable<BuiltSpeakerSession>();

                        var result = (BuiltSpeaker[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltSpeaker where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }

                        insertedCount = connection.ExecuteScalar<int>("select count(*) from BuiltSpeaker");
                    }

                    if (insertedCount < totalCount)
                    {
                        skip += limit;
                        fetchSpeaker(connection, callbackHandler);
                    }
                    else
                    {
                        skip = 0;
                        if (callbackHandler != null)
                            callbackHandler(true);
                    }
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchTrack(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchTrack<BuiltTracks>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltTracks>();

                        var result = (BuiltTracks[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    var item = result[i];
                                    if (!String.IsNullOrEmpty(item.name))
                                    {
                                        var name = item.name;
                                        bool faulted = false;
                                        try
                                        {
                                            var track = connection.Get<BuiltTracks>(p => p.name == name);
                                            track.color = item.color;
                                            connection.Update(track);
                                        }
                                        catch
                                        {
                                            faulted = true;
                                        }

                                        if (faulted)
                                        {
                                            if (name.Equals("no track", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                var track = new BuiltTracks();
                                                track.color = item.color;
                                                track.name = name;
                                                connection.InsertWithChildren(track);
                                            }
                                        }
                                    }

                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchVenue(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchVenue<BuiltVenue>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        connection.CreateTable<BuiltVenue>();
                        connection.CreateTable<VenueImage>();

                        var result = (BuiltVenue[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltVenue where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchTransportation(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchTransportation<BuiltTransportation>(success =>
            {

                try
                {
                    lock (_SyncLock)
                    {

                        connection.DropTable<BuiltTransportation>();
                        connection.DropTable<TransportationIcon>();

                        connection.CreateTable<BuiltTransportation>();
                        connection.CreateTable<TransportationIcon>();
                        connection.CreateTable<TransLinkGroup>();

                        var result = (BuiltTransportation[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltTransportation where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void fetchSettingsMenu(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            ApiCalls.fetchSettingsMenu<BuiltSettingsMenu>(success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltSettingsMenu>();
                        connection.CreateTable<Menus>();
                        connection.CreateTable<Submenus>();
                        connection.CreateTable<LinkGroup>();

                        var result = (BuiltSettingsMenu[])success.result;

                        if (result.Length > 0)
                        {
                            int count = 0;
                            for (int i = 0; i < result.Length; i++)
                            {
                                try
                                {
                                    count = connection.ExecuteScalar<int>("select count(*) from BuiltSettingsMenu where uid = ?", result[i].uid);
                                    if (count > 0)
                                        continue;
                                    connection.InsertWithChildren(result[i], true);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                            }
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void LoginExtension(string username, string password, SQLiteConnection connection, Action<BuiltError> callbackHandler)
        {
            //if (!IsNetworkAvailable())
            //{
            //    callbackHandler(new BuiltError(internet_required_error));
            //    return;
            //}

            var dict = new Dictionary<string, object> { { "user_name", username }, { "password", password } };

            ApiCalls.getLoginExtension<BuiltLoginExtension>("vmwareLogin", dict, success =>
            {
                var lstCreatedAt = success.result;

                try
                {
                    lock (_SyncLock)
                    {
                        //connection.DropTable<BuiltLoginExtensionJson>();
                        //connection.CreateTable<BuiltLoginExtensionJson>();
                        var data = new BuiltLoginExtensionJson
                        {
                            login_json = Newtonsoft.Json.JsonConvert.SerializeObject(lstCreatedAt)
                        };
                        connection.Insert(data);
                    }

                    if (callbackHandler != null)
                        callbackHandler(null);
                }
                catch (Exception ex)
                {
                    if (callbackHandler != null)
                        callbackHandler(new BuiltError(ex));
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(error);
            });
        }

        public static void getMyInterestExtension(SQLiteConnection connection, Action<List<BuiltSession>> callbackHandler)
        {
            var dict = new Dictionary<string, object> { { "clientID", currentUser.external_api_identifier } };
            ApiCalls.getMyInterestExtension<Dictionary<string, object>>("getMyInterests", dict, s =>
            {
                try
                {
                    List<string> abbreviations = null;
                    var jobj = s.result["api"] as JObject;
                    try
                    {
                        var a = jobj.ToObject<ISApi>();
                        abbreviations = a.interests.sessions.session.Select(p => p.abbreviation).ToList();
                    }
                    catch
                    {
                        try
                        {
                            var a = jobj.ToObject<ISApiSingle>();

                            Console.WriteLine(a);
                        }
                        catch
                        { }
                    }

                    if (abbreviations != null && abbreviations.Count > 0)
                    {
                        lock (_SyncLock)
                        {
                            var sess = connection.GetAllWithChildren<BuiltSession>(p => abbreviations.Contains(p.abbreviation));

                            connection.DropTable<UserInterestSession>();
                            connection.CreateTable<UserInterestSession>();
                            var uis = sess.Select(p => new UserInterestSession { session_time_id = p.session_id }).ToList();
                            connection.InsertAllWithChildren(uis, true);

                            if (callbackHandler != null)
                            {
                                callbackHandler(sess);
                            }
                        }
                    }
                    else
                    {
                        if (callbackHandler != null)
                            callbackHandler(null);
                    }

                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(null);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(null);
            });
        }

        public static void fetchAfterLoginAsync(SQLiteConnection connection, ExtensionApplicationUser user, Action<bool, int> callbackHandler)
        {
            Task.Run(() =>
            {
                try
                {
                    int completedCount = 0;
                    int newSurveyCount = 0;

                    var config = connection.Get<BuiltConfig>(p => p.uid == p.uid);

                    destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.timezone ?? "US/Pacific");

                    //fetchMyActivity(connection, (res) =>
                    //{
                    //    completedCount++;
                    //});

                    fetchNotes(connection, (res) =>
                    {
                        completedCount++;
                    });

                    //fetchEventNotification(connection, (res) =>
                    //{
                    //    completedCount++;
                    //});

                    //getMyInterestExtension(connection, (res) =>
                    //{
                    //    completedCount++;
                    //});

                    //getMySessionsExtension(connection, (res) =>
                    //{
                    //    completedCount++;
                    //});

                    //getMySurveyExtension(connection, (res, count) =>
                    //{
                    //    newSurveyCount = count;
                    //    completedCount++;
                    //}, 0);

                    while (completedCount != 1)
                    {
                        //await Task.Delay(1000);
                    }

                    if (callbackHandler != null)
                        callbackHandler(true, newSurveyCount);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false, 0);
                }
            });
        }

        public static void fetchExtensionsData(SQLiteConnection connection, ExtensionApplicationUser user, Action<bool> callbackHandler)
        {
            Task.Run(async () =>
            {
                try
                {
                    int completedCount = 0;

                    var config = connection.Get<BuiltConfig>(p => p.uid == p.uid);

                    destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.timezone ?? "US/Pacific");

                    getMyInterestExtension(connection, (res) =>
                    {
                        completedCount++;
                    });

                    getMySessionsExtension(connection, (res) =>
                    {
                        completedCount++;
                    });

                    getMySurveyExtension(connection, user, (res) =>
                    {
                        completedCount++;
                    });

                    getDeltaEventNotification(connection, getLastActivity(connection, typeof(BuiltEventNotifications)), res =>
                    {
                        completedCount++;
                    });

                    while (completedCount != 4)
                    {
                        //await Task.Delay(1000);
                    }

                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            });
        }

        public static Task UpdateDataOnLogout(SQLiteConnection connection)
        {
            return Task.Run(() =>
            {
                lock (_SyncLock)
                {
                    try
                    {
                        connection.DeleteAll<UserInterestSession>();
                        connection.DeleteAll<BuiltMySession>();
                        connection.DeleteAll<BuiltLoginExtensionJson>();
                        connection.DeleteAll<BuiltMyActivity>();
                        connection.DeleteAll<BuiltNotes>();
                        connection.DeleteAll<NotePhotos>();
                        connection.DeleteAll<SurveyExtension>();
                    }
                    catch { }
                }
            });
        }

        public static void AddSessionToSchedule(SQLiteConnection connection, BuiltSessionTime session_time, Action<BuiltSessionTime> success, Action<BuiltError> error)
        {
            Task.Run(() =>
            {
                if (!IsNetworkAvailable())
                {
                    error(new BuiltError(internet_required_error));
                    return;
                }

                lock (_SyncLock)
                {
                    int count = connection.ExecuteScalar<int>("select count(*) from BuiltMySession where session_time_id = ?", session_time.session_time_id);
                    if (count > 0)
                    {
                        success(session_time);
                        return;
                    }
                }

                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("clientID", currentUser.external_api_identifier);
                dict.Add("session_time_id", session_time.session_time_id);
                ApiCalls.executeFunction<Dictionary<string, object>>("addSession", dict, (succ) =>
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltMySession>();
                        var session = new BuiltMySession
                        {
                            session_time_id = session_time.session_time_id
                        };
                        connection.Insert(session);
                    }

                    AddEventInfo(connection, AnalyticsEventIds.session_add_to_schedule, ToDateString(DateTime.Now));

                    if (success != null)
                        success(session_time);

                }, (err) =>
                {
                    if (error != null)
                        error(err);
                });
            });
        }

        public static void RemoveSessionFromSchedule(SQLiteConnection connection, BuiltSessionTime session_time, Action<BuiltError, BuiltSessionTime> callback)
        {
            Task.Run(() =>
            {

                if (!IsNetworkAvailable())
                {
                    callback(new BuiltError(internet_required_error), null);
                    return;
                }

                lock (_SyncLock)
                {
                    int count = connection.ExecuteScalar<int>("select count(*) from BuiltMySession where session_time_id = ?", session_time.session_time_id);
                    if (count == 0)
                    {
                        callback(null, session_time);
                        return;
                    }
                }

                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("clientID", currentUser.external_api_identifier);
                dict.Add("session_time_id", session_time.session_time_id);
                ApiCalls.executeFunction<Dictionary<string, object>>("removeSession", dict, (success) =>
                {
                    lock (_SyncLock)
                    {
                        connection.CreateTable<BuiltMySession>();
                        var sid = session_time.session_time_id;
                        var session = connection.Get<BuiltMySession>(p => p.session_time_id == sid);
                        connection.Delete(session);
                    }
                    AddEventInfo(connection, AnalyticsEventIds.session_remove_from_schedule, ToDateString(DateTime.Now));
                    callback(null, session_time);
                }, (error) =>
                {
                    if (callback != null)
                        callback(error, null);
                });
            });
        }

        public static void toggleExtension(string sessionId, SQLiteConnection connection, Action<string> callbackHandler, Action<BuiltError> error)
        {

            //if (!IsNetworkAvailable())
            //{
            //    error(new BuiltError(internet_required_error));
            //    return;
            //}

            var dict = new Dictionary<string, object> { { "sessionID", sessionId }, { "clientID", currentUser.external_api_identifier } };
            ApiCalls.ToggleInterestExtension<ToggleResult>("toggleInterest", dict, success =>
            {
                var result = success.result;

                try
                {
                    if (result.api.operation.Equals("added", StringComparison.InvariantCultureIgnoreCase))
                    {
                        lock (_SyncLock)
                        {
                            connection.CreateTable<UserInterestSession>();
                            int count = connection.ExecuteScalar<int>("select count(*) from UserInterestSession where session_time_id = ?", result.api.session_id);
                            if (count == 0)
                            {
                                var session = new UserInterestSession
                                {
                                    session_time_id = result.api.session_id
                                };

                                connection.Insert(session);
                            }
                        }
                    }
                    else
                    {
                        lock (_SyncLock)
                        {
                            connection.CreateTable<UserInterestSession>();
                            int count = connection.ExecuteScalar<int>("select count(*) from UserInterestSession where session_time_id = ?", result.api.session_id);
                            if (count > 0)
                            {
                                var id = result.api.session_id;
                                try
                                {
                                    var session = connection.Get<UserInterestSession>(p => p.session_time_id == id);
                                    connection.Delete(session);
                                }
                                catch { }
                            }
                        }
                    }

                    callbackHandler(result.api.operation);
                }
                catch { }
            }, err =>
            {
                if (callbackHandler != null)
                {
                    callbackHandler(string.Empty);
                    error(err);
                }

            });
        }

        public static void CheckIfLatestDataAvailable(SQLiteConnection connection, Action<bool> handler)
        {
            if (IsActiveNetworkAvailable != null)
            {
                if (!IsActiveNetworkAvailable())
                {
                    handler(false);
                    return;
                }
            }

            ApiCalls.getLastActivities<BuiltLastActivity>(success =>
            {
                bool result = false;
                var config = Get<BuiltConfig>(connection, p => p.uid == p.uid);

                destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.timezone);

                createLastActivityTable(connection, config.last_synced_delta_time);
                var lastActivities = success.result;

                var dict = GetUidTypesDictionary();
                for (int i = 0; i < lastActivities.Length; i++)
                {
                    if (lastActivities[i].uid == ApiCalls.notes || lastActivities[i].uid == ApiCalls.my_activity)
                        continue;

                    if (!dict.ContainsKey(lastActivities[i].uid))
                        continue;

                    var lastActivityBuilt = Convert.ToDateTime(lastActivities[i].last_activity);
                    lastActivityBuilt = lastActivityBuilt.AddMilliseconds(-lastActivityBuilt.Millisecond);

                    try
                    {
                        var lastActivityDb = getLastActivity(connection, dict[lastActivities[i].uid]);
                        lastActivityDb = lastActivityDb.AddMilliseconds(-lastActivityDb.Millisecond);
                        if (lastActivityBuilt > lastActivityDb)
                        {
                            result = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (handler != null)
                    handler(result);
            }, error =>
            {
                if (handler != null)
                    handler(false);
            });
        }

        public static void getMySessionsExtension(SQLiteConnection connection, Action<bool> callbackHandler)
        {
            var dict = new Dictionary<string, object> { { "clientID", currentUser.external_api_identifier } };
            ApiCalls.executeFunction<Dictionary<string, object>>("getMySchedule", dict, s =>
            {
                try
                {
                    List<string> ids = null;
                    var jobj = s.result["api"] as JObject;
                    try
                    {
                        var a = jobj.ToObject<Api>();
                        ids = a.schedule.item.Select(p => p.sessiontime_id).ToList();
                    }
                    catch
                    {
                        try
                        {
                            var a = jobj.ToObject<ApiSingle>();
                            ids = new List<string> { a.schedule.item.sessiontime_id };
                        }
                        catch
                        { }
                    }

                    if (ids != null && ids.Count > 0)
                    {
                        lock (_SyncLock)
                        {
                            connection.DropTable<BuiltMySession>();
                            connection.CreateTable<BuiltMySession>();
                            var sessions = ids.Select(p => new BuiltMySession { session_time_id = p }).ToList();
                            connection.InsertAll(sessions);
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void getMySurveyExtension(SQLiteConnection connection, ExtensionApplicationUser user, Action<bool> callbackHandler)
        {
            var dict = new Dictionary<string, object> { { "clientID", user.external_api_identifier } };
            ApiCalls.executeFunction<Dictionary<string, object>>("getUserSurvey", dict, s =>
            {
                try
                {

                    Console.WriteLine(s.result);

                    lock (_SyncLock)
                    {
                    }
                    if (callbackHandler != null)
                        callbackHandler(true);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(false);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(false);
            });
        }

        public static void getMySurveyExtension(SQLiteConnection connection, Action<List<SurveyExtension>, int> callbackHandler, int previousCount)
        {
            if (IsActiveNetworkAvailable != null)
            {
                if (!IsActiveNetworkAvailable())
                {
                    callbackHandler(new List<SurveyExtension>(), 0);
                    return;
                }
            }

            if (currentUser == null)
            {
                callbackHandler(new List<SurveyExtension>(), 0);
                return;
            }
            var dict = new Dictionary<string, object> { { "clientID", currentUser.external_api_identifier } };
            ApiCalls.executeFunction<Dictionary<string, object>>("getUserSurvey", dict, s =>
            {
                List<SurveyExtension> result = new List<SurveyExtension>();
                try
                {
                    var jobj = s.result["api"] as JObject;
                    try
                    {
                        var a = jobj.ToObject<SurveyApi>();
                        result.AddRange(a.surveys.survey);
                    }
                    catch
                    {
                        try
                        {
                            var a = jobj.ToObject<SurveyApiSingle>();
                            result.Add(a.surveys.survey);
                        }
                        catch
                        { }
                    }

                    result = result.Where(p => p != null).ToList();
                    int newSurveyCount = previousCount;
                    foreach (var item in result)
                    {
                        var survey = item;

                        if (!String.IsNullOrWhiteSpace(survey.session_id))
                            survey.survey_id = survey.id + survey.session_id;
                        else if (!String.IsNullOrWhiteSpace(survey.id))
                            survey.survey_id = survey.id;
                        else
                            survey.survey_id = survey.name;

                        var count = 0;
                        lock (_SyncLock)
                        {
                            count = connection.ExecuteScalar<int>("select count(*) from SurveyExtension where survey_id = ?", survey.survey_id);
                        }
                        if (count == 0)
                        {
                            newSurveyCount++;
                            InsertWithChildren(connection, survey);
                        }
                        else
                            Delete(connection, survey);
                    }

                    if (callbackHandler != null)
                        callbackHandler(result, newSurveyCount);
                }
                catch
                {
                    if (callbackHandler != null)
                        callbackHandler(result, 0);
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(new List<SurveyExtension>(), 0);
            });
        }

        public static void saveNote(SQLiteConnection connection, BuiltNotes note, Action<BuiltError> callbackHandler)
        {
            //if (!IsNetworkAvailable())
            //{
            //    callbackHandler(new BuiltError(internet_required_error));
            //    return;
            //}

            if (currentUser == null)
            {
                callbackHandler(new BuiltError());
                return;
            }

            BuiltACL aclForNote = new BuiltACL();
            aclForNote.setPublicReadAccess(false);
            aclForNote.setPublicWriteAccess(false);
            aclForNote.setPublicDeleteAccess(false);
            aclForNote.setUserReadAccess(currentUser.uid, true);
            aclForNote.setUserWriteAccess(currentUser.uid, true);
            aclForNote.setUserDeleteAccess(currentUser.uid, true);
            note.ACL = aclForNote;

            ApiCalls.SaveWithDictionary<BuiltNotes>("notes", note, note.uid, currentUser, (result) =>
            {
                try
                {
                    var success = result;
                    success.tags_separarated = getSeparatedString(success.tags);

                    lock (_SyncLock)
                    {
                        var count = connection.ExecuteScalar<int>("select count(*) from BuiltNotes where uid = ?", success.uid);
                        if (count > 0)
                        {
                            var uid = success.uid;
                            var tmpObj = connection.GetAllWithChildren<BuiltNotes>(p => p.uid == uid).FirstOrDefault();
                            connection.Delete(tmpObj, true);
                            tmpObj = success;
                            connection.InsertWithChildren(tmpObj, true);
                        }
                        else
                        {
                            connection.InsertWithChildren(success, true);
                        }
                    }
                    if (callbackHandler != null)
                        callbackHandler(null);
                }
                catch (Exception ex)
                {
                    if (callbackHandler != null)
                        callbackHandler(new BuiltError(ex));
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(error);
            });
        }

        public static void deleteNote(SQLiteConnection connection, string noteUid, Action<BuiltError> callbackHandler)
        {
            //if (!IsNetworkAvailable())
            //{
            //    callbackHandler(new BuiltError(internet_required_error));
            //    return;
            //}

            ApiCalls.Delete<BuiltNotes>("notes", noteUid, currentUser, success =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        try
                        {
                            var tmpObj = connection.Get<BuiltNotes>(p => p.uid == noteUid);
                            connection.Delete(tmpObj, true);
                        }
                        catch { }
                    }
                    if (callbackHandler != null)
                        callbackHandler(null);
                }
                catch (Exception ex)
                {
                    if (callbackHandler != null)
                        callbackHandler(new BuiltError(ex));
                }
            }, error =>
            {
                if (callbackHandler != null)
                    callbackHandler(error);
            });
        }

        public static void getALLFeeds(SQLiteConnection connection, Action<List<BuiltTwitter>> callbackHandler)
        {
            List<BuiltTwitter> result = new List<BuiltTwitter>();

        }

        public static void UploadFile(string path, byte[] data, ExtensionApplicationUser user, Action<BuiltFile> callbackHandler)
        {
            try
            {
                ApiCalls.UploadFile(path, data, user, (res) =>
                {
                    if (callbackHandler != null)
                        callbackHandler(res);
                }, (err) =>
                {
                    if (callbackHandler != null)
                        callbackHandler(null);
                });
            }
            catch
            {
                if (callbackHandler != null)
                    callbackHandler(null);
            }
        }

        public static void RefreshUserInfo(SQLiteConnection connection, Action<BuiltError> error)
        {
            Task.Run(() =>
            {
                if (currentUser != null)
                {
                    ApiCalls.RefreshUserInfo(currentUser.uid, (s) =>
                    {
                        lock (_SyncLock)
                        {
                            BuiltLoginExtension bla = new BuiltLoginExtension();
                            bla.application_user = currentUser;
                            bla.application_user.public_view_private_schedule = s.public_view_private_schedule;
                            bla.application_user.private_view_private_schedule = s.private_view_private_schedule;
                            bla.application_user.cannot_view_or_schedule = s.cannot_view_or_schedule;

                            connection.DeleteAll<BuiltLoginExtensionJson>();
                            //connection.CreateTable<BuiltLoginExtensionJson>();

                            var data = new BuiltLoginExtensionJson
                            {
                                login_json = Newtonsoft.Json.JsonConvert.SerializeObject(bla)
                            };
                            connection.Insert(data);
                        }
                        error(null);
                    }, (e) =>
                    {
                        error(e);
                    });
                }
                else
                {
                    error(null);
                }
            });
        }

        public static void CreateInstallation(string deviceToken, Action<BuiltError> error, bool disableNotifications = false)
        {
            if (!IsNetworkAvailable())
            {
                error(new BuiltError(internet_required_error));
                return;
            }

            ApiCalls.createInstallation(deviceToken, err =>
            {
                error(err);
            }, disableNotifications);
        }

        public static void TriggerEvent(string token, Dictionary<string, object> properties, Action<BuiltError> error)
        {
            if (!IsNetworkAvailable())
            {
                error(new BuiltError(internet_required_error));
                return;
            }

            ApiCalls.TriggerEvent(token, properties, err =>
            {
                error(err);
            });
        }

        public static void TriggerMultipleEvents(SQLiteConnection connection, List<EventUid> data, Dictionary<string, object> superProperties, Action<BuiltError> error)
        {
            if (!IsNetworkAvailable())
            {
                error(new BuiltError(internet_required_error));
                return;
            }

            ApiCalls.TriggerMultipleEvents(data, superProperties, err =>
            {
                if (err == null)
                {
                    //lock (_SyncLock)
                    //{
                    //    connection.DropTable<EventInformation>();
                    //    connection.CreateTable<EventInformation>();
                    //}
                    lstEventInformation = new List<EventInformation>();
                }

                error(err);
            });
        }

        public static void RefreshNotifiactions(SQLiteConnection connection, Action<bool> callback)
        {
            if (!IsNetworkAvailable())
            {
                callback(false);
                return;
            }

            getDeltaEventNotification(connection, getLastActivity(connection, typeof(BuiltEventNotifications)), (res) =>
            {
                if (callback != null)
                    callback(res);
            });
        }

        public static void Delete(SQLiteConnection connection, object obj, bool recursive = false)
        {
            if (obj != null)
            {
                lock (_SyncLock)
                {
                    connection.Delete(obj, recursive);
                }
            }
        }
        public static void InsertWithChildren(SQLiteConnection connection, object obj, bool recursive = false)
        {
            lock (_SyncLock)
            {
                connection.InsertWithChildren(obj, recursive);
            }
        }

        public static List<T> GetAllWithChildren<T>(SQLiteConnection connection, Expression<Func<T, bool>> filter = null, bool recursive = false) where T : new()
        {
            lock (_SyncLock)
            {
                if (typeof(T) == typeof(BuiltSession) || typeof(T) == typeof(BuiltSessionTime))
                {
                    lock (_SessionLock)
                    {
                        return connection.GetAllWithChildren<T>(filter, recursive);
                    }
                }
                else
                    return connection.GetAllWithChildren<T>(filter, recursive);
            }
        }

        public static T Get<T>(SQLiteConnection connection, Expression<Func<T, bool>> filter) where T : new()
        {
            lock (_SyncLock)
            {
                //return connection.Get<T>(filter);
                try
                {
                    return connection.GetAllWithChildren<T>(filter).FirstOrDefault();
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }

    public static class ObjectExtensions
    {
        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }
    }
}