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
using System.Diagnostics;


namespace CommonLayer
{
    public partial class DataManager
    {
        static short completedDeltas;
        const short totalDeltas = 23;
        const string separator = "|";
        private static readonly object _SyncLock = new object();
        public static TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("US/Pacific");
        public static ExtensionApplicationUser currentUser;

        public static Func<bool> IsActiveNetworkAvailable;

        private static bool continueWithDelta;

        public static void deltaAllAsync(SQLiteConnection connection, Action<BuiltError, List<string>> callbackHandler, ExtensionApplicationUser user)
        {
            Task.Run(() =>
            {
                //if (!IsNetworkAvailable())
                //{
                //    callbackHandler(new BuiltError(internet_required_error), null);
                //    return;
                //}

                continueWithDelta = true;

                try
                {
                    ApiCalls.getLastActivities<BuiltLastActivity>(async success =>
                    {
                        var config = Get<BuiltConfig>(connection, p => p.uid == p.uid);

                        destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById(config.timezone ?? "US/Pacific");

                        createLastActivityTable(connection, config.last_synced_delta_time);

                        var lastActivities = success.result;

                        var updatable = new List<Type>();
                        var dict = GetUidTypesDictionary();
                        for (int i = 0; i < lastActivities.Length; i++)
                        {
                            if (!dict.ContainsKey(lastActivities[i].uid))
                                continue;

                            var lastActivityBuilt = Convert.ToDateTime(lastActivities[i].last_activity);
                            lastActivityBuilt = lastActivityBuilt.AddMilliseconds(-lastActivityBuilt.Millisecond);

                            var lastActivityDb = getLastActivity(connection, dict[lastActivities[i].uid]);
                            lastActivityDb = lastActivityDb.AddMilliseconds(-lastActivityDb.Millisecond);
                            if (lastActivityBuilt > lastActivityDb)
                            {
                                if (dict.ContainsKey(lastActivities[i].uid))
                                    if (!updatable.Contains(dict[lastActivities[i].uid]))
                                        updatable.Add(dict[lastActivities[i].uid]);
                            }
                        }

                        string maxDate = null;
                        if (updatable.Count > 0)
                        {
                            maxDate = ToDateString(lastActivities.Select(p => Convert.ToDateTime(p.last_activity)).Max());
                        }

                        var callsCount = updatable.Count;
                        completedDeltas = 0;

                        if (continueWithDelta && updatable.Contains(typeof(BuiltConfig)))
                        {
                            getDeltaConfig(connection, getLastActivity(connection, typeof(BuiltConfig)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltConfig), maxDate);
                                }
                            });

                            while (completedDeltas != 1)
                            {
                                await Task.Delay(1000);
                            }
                        }

                        if (!continueWithDelta)
                        {
                            fetchAllAsync(connection, res =>
                            {
                                if (callbackHandler != null)
                                    callbackHandler(null, null);
                            });
                            return;
                        }


                        if (continueWithDelta && updatable.Contains(typeof(BuiltSession)))
                            getDeltaSession(connection, getLastActivity(connection, typeof(BuiltSession)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltSession), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltAchievements)))
                            getDeltaAchievements(connection, getLastActivity(connection, typeof(BuiltAchievements)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltAchievements), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltAgenda)))
                            getDeltaAgenda(connection, getLastActivity(connection, typeof(BuiltAgenda)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltAgenda), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltEventNotifications)))
                            getDeltaEventNotification(connection, getLastActivity(connection, typeof(BuiltEventNotifications)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltEventNotifications), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltExhibitor)))
                            getDeltaExhibitor(connection, getLastActivity(connection, typeof(BuiltExhibitor)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltExhibitor), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltSFFoodNDrink)))
                            getDeltaFoodAndDrink(connection, getLastActivity(connection, typeof(BuiltSFFoodNDrink)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltSFFoodNDrink), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltHandsonLabs)))
                            getDeltaHandsOnLabs(connection, getLastActivity(connection, typeof(BuiltHandsonLabs)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltHandsonLabs), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltImportantLinks)))
                            getDeltaImportantLinks(connection, getLastActivity(connection, typeof(BuiltImportantLinks)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltImportantLinks), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltIntro)))
                            getDeltaIntro(connection, getLastActivity(connection, typeof(BuiltIntro)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltIntro), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltLegal)))
                            getDeltaLegal(connection, getLastActivity(connection, typeof(BuiltLegal)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltLegal), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltMenu)))
                            getDeltaMenu(connection, getLastActivity(connection, typeof(BuiltMenu)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltMenu), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltNews)))
                            getDeltaNews(connection, getLastActivity(connection, typeof(BuiltNews)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltNews), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltOthers)))
                            getDeltaOthers(connection, getLastActivity(connection, typeof(BuiltOthers)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltOthers), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltPopularSessions)))
                            getDeltaPopularSessions(connection, getLastActivity(connection, typeof(BuiltPopularSessions)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltPopularSessions), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltQRCodes)))
                            getDeltaQrcodes(connection, getLastActivity(connection, typeof(BuiltQRCodes)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltQRCodes), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltSpeaker)))
                            getDeltaSpeaker(connection, getLastActivity(connection, typeof(BuiltSpeaker)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltSpeaker), maxDate);
                                }
                            });


                        if (continueWithDelta && updatable.Contains(typeof(BuiltTracks)))
                            getDeltaTrack(connection, getLastActivity(connection, typeof(BuiltTracks)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltTracks), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltTransportation)))
                            getDeltaTransportation(connection, getLastActivity(connection, typeof(BuiltTransportation)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltTransportation), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltVenue)))
                            getDeltaVenue(connection, getLastActivity(connection, typeof(BuiltVenue)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltVenue), maxDate);
                                }
                            });

                        if (continueWithDelta && updatable.Contains(typeof(BuiltSettingsMenu)))
                            getDeltaSettingsMenu(connection, getLastActivity(connection, typeof(BuiltSettingsMenu)), (res) =>
                            {
                                completedDeltas++;
                                if (res)
                                {
                                    updateLastActivity(connection, typeof(BuiltSettingsMenu), maxDate);
                                }
                            });



                        if (user != null)
                        {
                            if (continueWithDelta && updatable.Contains(typeof(BuiltMyActivity)))
                                getDeltaMyActivity(connection, getLastActivity(connection, typeof(BuiltMyActivity)), (res) =>
                                {
                                    completedDeltas++;
                                    if (res)
                                    {
                                        updateLastActivity(connection, typeof(BuiltMyActivity), maxDate);
                                    }
                                });

                            if (continueWithDelta && updatable.Contains(typeof(BuiltNotes)))
                                getDeltaNotes(connection, getLastActivity(connection, typeof(BuiltNotes)), (res) =>
                                {
                                    completedDeltas++;
                                    if (res)
                                    {
                                        updateLastActivity(connection, typeof(BuiltNotes), maxDate);
                                    }
                                });

                        }
                        else
                        {
                            if (updatable.Contains(typeof(BuiltMyActivity)))
                                callsCount -= 1;
                            if (updatable.Contains(typeof(BuiltNotes)))
                                callsCount -= 1;

                        }
                        while (completedDeltas != callsCount)
                        {
                            await Task.Delay(1000);
                        }
                        var updatedUids = dict.Where(p => updatable.Contains(p.Value)).Select(p => p.Key).ToList();

                        if (callbackHandler != null)
                            callbackHandler(null, updatedUids);
                    }, error =>
                    {
                        if (callbackHandler != null)
                            callbackHandler(error, null);
                    });
                }
                catch (Exception ex)
                {
                    if (callbackHandler != null)
                        callbackHandler(new BuiltError(ex), null);
                }
            });
        }

        public static void getDeltaSession(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaSession<BuiltSession>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltSession builtSession = null;

                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtSession = Get<BuiltSession>(connection, p => p.uid == uid);
                                    Delete(connection, builtSession, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltSession builtSession = null;
                            int count = 0;
                            string uid = null;

                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lstUpdatedAt[i].audience_separated = getSeparatedString(lstUpdatedAt[i].audience);
                                    lstUpdatedAt[i].program_location_separated = getSeparatedString(lstUpdatedAt[i].program_location);
                                    lstUpdatedAt[i].role_separated = getSeparatedString(lstUpdatedAt[i].role);
                                    lstUpdatedAt[i].skill_level_separated = getSeparatedString(lstUpdatedAt[i].skill_level);
                                    lstUpdatedAt[i].solutions_separated = getSeparatedString(lstUpdatedAt[i].solutions);
                                    lstUpdatedAt[i].sub_track_separated = getSeparatedString(lstUpdatedAt[i].sub_track);
                                    lstUpdatedAt[i].market_segment_separated = getSeparatedString(lstUpdatedAt[i].market_segment, ", ");
                                    lstUpdatedAt[i].competency_separated = getSeparatedString(lstUpdatedAt[i].competency, ", ");

                                    for (int x = 0; x < lstUpdatedAt[i].session_time.Count; x++)
                                    {
                                        lstUpdatedAt[i].session_time[x].start_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(lstUpdatedAt[i].session_time[x].date, lstUpdatedAt[i].session_time[x].time));
                                        lstUpdatedAt[i].session_time[x].end_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(lstUpdatedAt[i].session_time[x].date, lstUpdatedAt[i].session_time[x].time).AddMinutes(Convert.ToInt32(lstUpdatedAt[i].session_time[x].length)));
                                    }

                                    #region --Track--
                                    if (!String.IsNullOrEmpty(lstUpdatedAt[i].track))
                                    {
                                        var trackCount = 0;
                                        lock (_SyncLock)
                                        {
                                            trackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ?", lstUpdatedAt[i].track);
                                        }
                                        if (trackCount == 0)
                                        {
                                            BuiltTracks track = new BuiltTracks();
                                            track.name = lstUpdatedAt[i].track;
                                            track.color = "242424";

                                            InsertWithChildren(connection, track);
                                        }

                                        foreach (var item in lstUpdatedAt[i].sub_track_separated.Split('|'))
                                        {
                                            if (!String.IsNullOrEmpty(item))
                                            {
                                                var subtrackCount = 0;
                                                lock (_SyncLock)
                                                {
                                                    subtrackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ? and parentTrackName != ''", item);
                                                }
                                                if (subtrackCount == 0)
                                                {
                                                    BuiltTracks subtrack = new BuiltTracks();
                                                    subtrack.name = item;
                                                    subtrack.parentTrackName = lstUpdatedAt[i].track;
                                                    InsertWithChildren(connection, subtrack);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSession where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtSession = GetAllWithChildren<BuiltSession>(connection, p => p.uid == uid, true).FirstOrDefault();
                                        Delete(connection, builtSession, true);
                                        builtSession = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtSession, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSession where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    lstCreatedAt[i].audience_separated = getSeparatedString(lstCreatedAt[i].audience);
                                    lstCreatedAt[i].program_location_separated = getSeparatedString(lstCreatedAt[i].program_location);
                                    lstCreatedAt[i].role_separated = getSeparatedString(lstCreatedAt[i].role);
                                    lstCreatedAt[i].skill_level_separated = getSeparatedString(lstCreatedAt[i].skill_level);
                                    lstCreatedAt[i].solutions_separated = getSeparatedString(lstCreatedAt[i].solutions);
                                    lstCreatedAt[i].sub_track_separated = getSeparatedString(lstCreatedAt[i].sub_track);
                                    lstCreatedAt[i].market_segment_separated = getSeparatedString(lstCreatedAt[i].market_segment, ", ");
                                    lstCreatedAt[i].competency_separated = getSeparatedString(lstCreatedAt[i].competency, ", ");

                                    for (int x = 0; x < lstCreatedAt[i].session_time.Count; x++)
                                    {
                                        lstCreatedAt[i].session_time[x].start_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(lstCreatedAt[i].session_time[x].date, lstCreatedAt[i].session_time[x].time));
                                        lstCreatedAt[i].session_time[x].end_date_time = ToDateStringWithAppTimezone(MergeDateAndTime(lstCreatedAt[i].session_time[x].date, lstCreatedAt[i].session_time[x].time).AddMinutes(Convert.ToInt32(lstCreatedAt[i].session_time[x].length)));
                                    }

                                    #region --Track--
                                    if (!String.IsNullOrEmpty(lstCreatedAt[i].track))
                                    {
                                        var trackCount = 0;
                                        lock (_SyncLock)
                                        {
                                            trackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ?", lstCreatedAt[i].track);
                                        }
                                        if (trackCount == 0)
                                        {
                                            BuiltTracks track = new BuiltTracks();
                                            track.name = lstCreatedAt[i].track;
                                            track.color = "242424";
                                            InsertWithChildren(connection, track);
                                        }

                                        foreach (var item in lstCreatedAt[i].sub_track_separated.Split('|'))
                                        {
                                            if (!String.IsNullOrEmpty(item))
                                            {
                                                var subtrackCount = 0;
                                                lock (_SyncLock)
                                                {
                                                    subtrackCount = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ? and parentTrackName != ''", item);
                                                }
                                                if (subtrackCount == 0)
                                                {
                                                    BuiltTracks subtrack = new BuiltTracks();
                                                    subtrack.name = item;
                                                    subtrack.parentTrackName = lstCreatedAt[i].track;
                                                    InsertWithChildren(connection, subtrack);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaAchievements(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaAchievements<BuiltAchievements>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltAchievements builtAchievements = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtAchievements = Get<BuiltAchievements>(connection, p => p.uid == uid);
                                    Delete(connection, builtAchievements, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltAchievements builtAchievements = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltAchievements where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtAchievements = GetAllWithChildren<BuiltAchievements>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtAchievements, true);
                                        builtAchievements = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtAchievements, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSession where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaAgenda(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaAgenda<BuiltAgenda>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltAgenda builtAgenda = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtAgenda = Get<BuiltAgenda>(connection, p => p.uid == uid);
                                    Delete(connection, builtAgenda, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltAgenda builtAgenda = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    //lstUpdatedAt[i].agenda_date = ToDateStringWithAppTimezone(Convert.ToDateTime(lstUpdatedAt[i].agenda_date));

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltAgenda where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtAgenda = GetAllWithChildren<BuiltAgenda>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtAgenda, true);
                                        builtAgenda = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtAgenda, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltAgenda where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion

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

        public static void getDeltaConfig(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaConfig<BuiltConfig>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltConfig builtConfig = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtConfig = Get<BuiltConfig>(connection, p => p.uid == uid);
                                    Delete(connection, builtConfig, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltConfig builtConfig = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lstUpdatedAt[i].menu.left_menu_separated = getSeparatedString(lstUpdatedAt[i].menu.left_menu);
                                    lstUpdatedAt[i].menu.right_menu_separated = getSeparatedString(lstUpdatedAt[i].menu.right_menu);
                                    lstUpdatedAt[i].explore.banner_details_separated = getSeparatedString(lstUpdatedAt[i].explore.banner_details);

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltConfig where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtConfig = GetAllWithChildren<BuiltConfig>(connection, p => p.uid == uid, true).FirstOrDefault();
                                        Delete(connection, builtConfig, true);
                                        builtConfig = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtConfig, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltConfig where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    lstCreatedAt[i].menu.left_menu_separated = getSeparatedString(lstCreatedAt[i].menu.left_menu);
                                    lstCreatedAt[i].menu.right_menu_separated = getSeparatedString(lstCreatedAt[i].menu.right_menu);
                                    lstCreatedAt[i].explore.banner_details_separated = getSeparatedString(lstCreatedAt[i].explore.banner_details);

                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaEventNotification(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaEventNotification<BuiltEventNotifications>(dateTime, currentUser, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltEventNotifications builtEventNotifications = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtEventNotifications = Get<BuiltEventNotifications>(connection, p => p.uid == uid);
                                    Delete(connection, builtEventNotifications, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltEventNotifications builtEventNotifications = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltEventNotifications where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtEventNotifications = Get<BuiltEventNotifications>(connection, p => p.uid == uid);
                                        Delete(connection, builtEventNotifications, true);
                                        builtEventNotifications = lstUpdatedAt[i];
                                        pk = builtEventNotifications.Id;
                                        builtEventNotifications.Id = pk;

                                        InsertWithChildren(connection, builtEventNotifications);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltEventNotifications where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaExhibitor(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaExhibitor<BuiltExhibitor>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltExhibitor builtExhibitor = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtExhibitor = Get<BuiltExhibitor>(connection, p => p.uid == uid);
                                    Delete(connection, builtExhibitor, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltExhibitor builtExhibitor = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltExhibitor where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtExhibitor = GetAllWithChildren<BuiltExhibitor>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtExhibitor, true);
                                        builtExhibitor = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtExhibitor, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltExhibitor where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaFoodAndDrink(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaFoodAndDrink<BuiltSFFoodNDrink>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltSFFoodNDrink builtSFFoodNDrink = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtSFFoodNDrink = Get<BuiltSFFoodNDrink>(connection, p => p.uid == uid);
                                    Delete(connection, builtSFFoodNDrink, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltSFFoodNDrink builtSFFoodNDrink = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSFFoodNDrink where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtSFFoodNDrink = GetAllWithChildren<BuiltSFFoodNDrink>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtSFFoodNDrink, true);
                                        builtSFFoodNDrink = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtSFFoodNDrink, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSFFoodNDrink where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaHandsOnLabs(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaHandsOnLabs<BuiltHandsonLabs>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltHandsonLabs builtHandsonLabs = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtHandsonLabs = Get<BuiltHandsonLabs>(connection, p => p.uid == uid);
                                    Delete(connection, builtHandsonLabs, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltHandsonLabs builtHandsonLabs = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lstUpdatedAt[i].speaker_separated = getSeparatedString(lstUpdatedAt[i].speaker);

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltHandsonLabs where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtHandsonLabs = Get<BuiltHandsonLabs>(connection, p => p.uid == uid);
                                        Delete(connection, builtHandsonLabs, true);
                                        builtHandsonLabs = lstUpdatedAt[i];
                                        pk = builtHandsonLabs.Id;
                                        builtHandsonLabs.Id = pk;

                                        InsertWithChildren(connection, builtHandsonLabs);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltHandsonLabs where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    lstCreatedAt[i].speaker_separated = getSeparatedString(lstCreatedAt[i].speaker);

                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion

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

        public static void getDeltaImportantLinks(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaImportantLinks<BuiltImportantLinks>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltImportantLinks builtImportantLinks = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtImportantLinks = Get<BuiltImportantLinks>(connection, p => p.uid == uid);
                                    Delete(connection, builtImportantLinks, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltImportantLinks builtImportantLinks = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltImportantLinks where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtImportantLinks = GetAllWithChildren<BuiltImportantLinks>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtImportantLinks, true);
                                        builtImportantLinks = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtImportantLinks, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltImportantLinks where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaIntro(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaIntro<BuiltIntro>(dateTime, success =>
            {


                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltIntro builtIntro = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    builtIntro = Get<BuiltIntro>(connection, p => p.uid == uid);
                                    Delete(connection, builtIntro, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltIntro builtIntro = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltIntro where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        builtIntro = GetAllWithChildren<BuiltIntro>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, builtIntro, true);
                                        builtIntro = lstUpdatedAt[i];
                                        InsertWithChildren(connection, builtIntro, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltIntro where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaLegal(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaLegal<BuiltLegal>(dateTime, success =>
            {


                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltLegal tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltLegal>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltLegal tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltLegal where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltLegal>(connection, p => p.uid == uid, true).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltLegal where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaMenu(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaMenu<BuiltMenu>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltMenu tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltMenu>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltMenu tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltMenu where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltMenu>(connection, p => p.uid == uid, true).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltMenu where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaMyActivity(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaMyActivity<BuiltMyActivity>(currentUser, dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltMyActivity tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltMyActivity>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltMyActivity tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltMyActivity where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = Get<BuiltMyActivity>(connection, p => p.uid == uid);
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        pk = tmpObj.Id;
                                        tmpObj.Id = pk;
                                        InsertWithChildren(connection, tmpObj);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltMyActivity where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaNews(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaNews<BuiltNews>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltNews tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltNews>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltNews tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltNews where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {

                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltNews>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        //if (tmpObj.link != null)
                                        //{
                                        //    if (String.IsNullOrWhiteSpace(tmpObj.link.title))
                                        //        tmpObj.link.title = tmpObj.link.href;
                                        //}
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    //lstCreatedAt[i].published_date = ToDateStringWithAppTimezone(Convert.ToDateTime(lstCreatedAt[i].published_date));

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltNews where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    //if (lstCreatedAt[i].link != null)
                                    //{
                                    //    if (String.IsNullOrWhiteSpace(lstCreatedAt[i].link.title))
                                    //        lstCreatedAt[i].link.title = lstCreatedAt[i].link.href;
                                    //}

                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaNotes(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaNotes<BuiltNotes>(currentUser, dateTime, success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltNotes tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltNotes>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltNotes tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lstUpdatedAt[i].tags_separarated = getSeparatedString(lstUpdatedAt[i].tags);

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltNotes where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltNotes>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltNotes where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    lstCreatedAt[i].tags_separarated = getSeparatedString(lstCreatedAt[i].tags);

                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaOthers(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaOthers<BuiltOthers>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltOthers tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltOthers>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltOthers tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltOthers where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = Get<BuiltOthers>(connection, p => p.uid == uid);
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        pk = tmpObj.Id;
                                        tmpObj.Id = pk;

                                        InsertWithChildren(connection, tmpObj);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltOthers where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaPopularSessions(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaPopularSessions<BuiltPopularSessions>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltPopularSessions tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltPopularSessions>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltPopularSessions tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lstUpdatedAt[i].session_separated = getSeparatedString(lstUpdatedAt[i].session);

                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltPopularSessions where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = Get<BuiltPopularSessions>(connection, p => p.uid == uid);
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        pk = tmpObj.Id;
                                        tmpObj.Id = pk;

                                        InsertWithChildren(connection, tmpObj);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltPopularSessions where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;

                                    lstCreatedAt[i].session_separated = getSeparatedString(lstCreatedAt[i].session);

                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaQrcodes(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaQrcodes<BuiltQRCodes>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltQRCodes tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltQRCodes>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltQRCodes tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltQRCodes where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = Get<BuiltQRCodes>(connection, p => p.uid == uid);
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        pk = tmpObj.Id;
                                        tmpObj.Id = pk;

                                        InsertWithChildren(connection, tmpObj);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltQRCodes where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaSpeaker(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaSpeaker<BuiltSpeaker>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltSpeaker tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltSpeaker>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltSpeaker tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSpeaker where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltSpeaker>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSpeaker where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaTrack(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaTrack<BuiltTracks>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltTracks tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltTracks>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltTracks tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string name = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    //count = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where uid = ?", lstUpdatedAt[i].uid);
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ?", lstUpdatedAt[i].name);
                                    }
                                    //if (count > 0)
                                    //{
                                    //    uid = lstUpdatedAt[i].uid;
                                    //    tmpObj = connection.Get<BuiltTracks>(p => p.uid == uid);
                                    //    connection.Delete(tmpObj, true);
                                    //    tmpObj = lstUpdatedAt[i];
                                    //    pk = tmpObj.Id;
                                    //    tmpObj.Id = pk;

                                    //    connection.InsertWithChildren(tmpObj);
                                    //}
                                    if (count > 0)
                                    {
                                        name = lstUpdatedAt[i].name;
                                        tmpObj = Get<BuiltTracks>(connection, p => p.name == name);
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];

                                        InsertWithChildren(connection, tmpObj);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i]);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    //count = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where uid = ?", lstCreatedAt[i].uid);
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltTracks where name = ?", lstCreatedAt[i].name);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i]);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaVenue(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaVenue<BuiltVenue>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltVenue tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltVenue>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltVenue tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltVenue where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltVenue>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltVenue where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        public static void getDeltaTransportation(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaTransportation<BuiltTransportation>(dateTime, success =>
            {
                try
                {
                    lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltTransportation tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltTransportation>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltTransportation tmpObj = null;
                            int pk = 0;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltTransportation where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltTransportation>(connection, p => p.uid == uid).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltTransportation where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion


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

        public static void getDeltaSettingsMenu(SQLiteConnection connection, DateTime dateTime, Action<bool> callbackHandler)
        {
            ApiCalls.getDeltaSettingsMenu<BuiltSettingsMenu>(dateTime, success =>
            {
                try
                {
                    //lock (_SyncLock)
                    {

                        #region --Deleted--
                        if (success.deletedAt().Count > 0)
                        {
                            var lstDeletedAt = success.deletedAt().Select(p => p.result).ToList();
                            BuiltSettingsMenu tmpObj = null;
                            string uid = null;
                            for (int i = 0; i < lstDeletedAt.Count; i++)
                            {
                                try
                                {
                                    uid = lstDeletedAt[i].uid;
                                    tmpObj = Get<BuiltSettingsMenu>(connection, p => p.uid == uid);
                                    Delete(connection, tmpObj, true);
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Updated--
                        if (success.updatedAt().Count > 0)
                        {
                            var lstUpdatedAt = success.updatedAt().Select(p => p.result).ToList();
                            BuiltSettingsMenu tmpObj = null;
                            int count = 0;
                            string uid = null;
                            for (int i = 0; i < lstUpdatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSettingsMenu where uid = ?", lstUpdatedAt[i].uid);
                                    }
                                    if (count > 0)
                                    {
                                        uid = lstUpdatedAt[i].uid;
                                        tmpObj = GetAllWithChildren<BuiltSettingsMenu>(connection, p => p.uid == uid, true).FirstOrDefault();
                                        Delete(connection, tmpObj, true);
                                        tmpObj = lstUpdatedAt[i];
                                        InsertWithChildren(connection, tmpObj, true);
                                    }
                                    else
                                    {
                                        InsertWithChildren(connection, lstUpdatedAt[i], true);
                                    }
                                }
                                catch { }
                            }
                        }
                        #endregion

                        #region --Created--
                        if (success.createdAt().Count > 0)
                        {
                            var lstCreatedAt = success.createdAt().Select(p => p.result).ToList();
                            int count = 0;
                            for (int i = 0; i < lstCreatedAt.Count; i++)
                            {
                                try
                                {
                                    lock (_SyncLock)
                                    {
                                        count = connection.ExecuteScalar<int>("select count(*) from BuiltSettingsMenu where uid = ?", lstCreatedAt[i].uid);
                                    }
                                    if (count > 0)
                                        continue;
                                    InsertWithChildren(connection, lstCreatedAt[i], true);
                                }
                                catch { }
                            }
                        }
                        #endregion
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

        private static void ClearDatabase(SQLiteConnection connection, int clear_db_id)
        {
            if (clear_db_id == 0)
                return;

            lock (_SyncLock)
            {
                var oldConfig = connection.GetAllWithChildren<BuiltConfig>(recursive: true).FirstOrDefault();

                if (oldConfig.clear_db_id != clear_db_id)
                {
                    DropAllTables(connection);
                    if (File.Exists(connection.DatabasePath))
                    {

                    }
                }
            }


            continueWithDelta = false;

        }

        private static string getSeparatedString(IEnumerable<string> collection, string customSeparator = null)
        {
            if (collection == null)
                return String.Empty;

            if (!String.IsNullOrWhiteSpace(customSeparator))
                return String.Join(customSeparator, collection);
            else
                return String.Join(separator, collection);
        }

        private static void createLastActivityTable(SQLiteConnection connection, string configSyncTime)
        {
            lock (_SyncLock)
            {
                try
                {
                    int count = connection.ExecuteScalar<int>("select count(*) from BuiltLastActivity");
                    if (count > 0)
                        return;
                }
                catch { }

                var uids = GetUidTypesDictionary().Keys.ToArray();
                BuiltLastActivity builtLastActivity = null;
                for (int i = 0; i < uids.Length; i++)
                {
                    builtLastActivity = new BuiltLastActivity
                    {
                        uid = uids[i],
                        last_activity = configSyncTime
                    };
                    connection.Insert(builtLastActivity);
                }
            }
        }

        private static DateTime getLastActivity(SQLiteConnection connection, Type type)
        {
            BuiltLastActivity builtLastActivity;
            try
            {
                var dict = GetUidTypesDictionary().Where(p => p.Value == type).ToDictionary(p => p.Key, p => p.Value);
                var uid = dict == null ? null : dict.Keys.First();
                if (String.IsNullOrEmpty(uid))
                    return DateTime.MinValue;
                lock (_SyncLock)
                {
                    builtLastActivity = connection.Get<BuiltLastActivity>(p => p.uid == uid);
                }
                return Convert.ToDateTime(builtLastActivity.last_activity);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static void updateLastActivity(SQLiteConnection connection, Type type, string syncTime)
        {
            try
            {
                var dict = GetUidTypesDictionary().Where(p => p.Value == type).ToDictionary(p => p.Key, p => p.Value);
                var uid = dict == null ? null : dict.Keys.First();
                if (String.IsNullOrEmpty(uid))
                    return;
                lock (_SyncLock)
                {
                    var builtLastActivity = connection.Get<BuiltLastActivity>(p => p.uid == uid);
                    builtLastActivity.last_activity = syncTime;
                    connection.Update(builtLastActivity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone, TimeZoneInfo sourceTimeZone = null)
        {
            if (destinationTimeZone == null)
                throw new ArgumentNullException("destinationTimeZone");

            if (sourceTimeZone == null)
                sourceTimeZone = TimeZoneInfo.Local;

            try
            {
                return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone);
            }
            catch
            {
                throw;
            }
        }

        private static string ToISODate(DateTime input)
        {
            return input.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }

        private static string ToDateStringWithAppTimezone(DateTime input)
        {
            var utcOffset = destinationTimeZone.BaseUtcOffset;
            return input.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'") + ((utcOffset < TimeSpan.Zero) ? "-" : "+") + utcOffset.ToString("hhmm") + "Z";
        }

        private static string ToDateString(DateTime input)
        {
            return input.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
        }

        private static DateTime ToLocalDateTime(string inputDateString)
        {
            try
            {
                return DateTime.Parse(inputDateString);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static DateTime MergeDateAndTime(string date, string time)
        {
            try
            {
                var dateParts = date.Trim().Split('-');
                var timeParts = time.Trim().Split(':');
                return new DateTime(
                    Convert.ToInt32(dateParts[0]),
                    Convert.ToInt32(dateParts[1]),
                    Convert.ToInt32(dateParts[2]),
                    Convert.ToInt32(timeParts[0]),
                    Convert.ToInt32(timeParts[1]), 0);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static Dictionary<string, Type> GetUidTypesDictionary()
        {
            return new Dictionary<string, Type>
            {
                {"session", typeof(BuiltSession)},
                {"achievements",typeof(BuiltAchievements)},
                {"agenda",typeof(BuiltAgenda)},
                {"event_notifications",typeof(BuiltEventNotifications)},
                {"exhibitor",typeof(BuiltExhibitor)},
                {"imp_links",typeof(BuiltImportantLinks)},
                {"intro",typeof(BuiltIntro)},
                {"legal",typeof(BuiltLegal)},
                {"left_menu",typeof(BuiltMenu)},
                {"my_activity",typeof(BuiltMyActivity)},
                {"news",typeof(BuiltNews)},
                {"notes",typeof(BuiltNotes)},
                {"others",typeof(BuiltOthers)},
                {"qrcodes",typeof(BuiltQRCodes)},
                {"food_n_drink",typeof(BuiltSFFoodNDrink)},
                {"transportation",typeof(BuiltTransportation)},
                {"speaker",typeof(BuiltSpeaker)},
                {"track",typeof(BuiltTracks)},
                {"config",typeof(BuiltConfig)},
                {"hol",typeof(BuiltHandsonLabs)},
                
                {"recommended",typeof(BuiltPopularSessions)},
                {"venue",typeof(BuiltVenue)},

                {"settings_menu",typeof(BuiltSettingsMenu)},
            };
        }

        public static void CreateAllTables(SQLiteConnection connection)
        {
            connection.CreateTable<BuiltSession>();
            connection.CreateTable<BuiltSessionFile>();
            connection.CreateTable<BuiltSessionTime>();
            connection.CreateTable<BuiltSessionSpeaker>();
            connection.CreateTable<BuiltTracks>();
            connection.CreateTable<BuiltAchievements>();
            connection.CreateTable<BuiltAchievementsImage>();
            connection.CreateTable<BuiltAgenda>();
            connection.CreateTable<BuiltAgendaItem>();
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
            connection.CreateTable<BuiltEventNotifications>();
            connection.CreateTable<BuiltExhibitor>();
            connection.CreateTable<BuiltExhibitorFile>();
            connection.CreateTable<BuiltParticipant>();
            connection.CreateTable<BuiltSFFoodNDrink>();
            connection.CreateTable<BuiltSFFoodNDrinkImage>();
            connection.CreateTable<FDLinkGroup>();
            connection.CreateTable<BuiltHandsonLabs>();
            connection.CreateTable<BuiltImportantLinks>();
            connection.CreateTable<BuiltImpLink>();
            connection.CreateTable<BuiltIntro>();
            connection.CreateTable<BuiltIntroBgImage>();
            connection.CreateTable<BuiltLegal>();
            connection.CreateTable<GamingRules>();
            connection.CreateTable<LicenseAgreement>();
            connection.CreateTable<OpenSourceLicenseAndroid>();
            connection.CreateTable<OpenSourceLicenseIos>();
            connection.CreateTable<Policy>();
            connection.CreateTable<Tnc>();
            connection.CreateTable<TrademarkCopyright>();
            connection.CreateTable<BuiltMenu>();
            connection.CreateTable<BuiltSection>();
            connection.CreateTable<BuiltSectionItems>();
            connection.CreateTable<MenuIconFile>();
            connection.CreateTable<MenuIconSelectedFile>();
            connection.CreateTable<BuiltMyActivity>();
            connection.CreateTable<BuiltNews>();
            connection.CreateTable<BuiltNewsLink>();
            connection.CreateTable<CoverImageFile>();
            connection.CreateTable<BuiltNotes>();
            connection.CreateTable<NotePhotos>();
            connection.CreateTable<BuiltOthers>();
            connection.CreateTable<BuiltPopularSessions>();
            connection.CreateTable<BuiltQRCodes>();
            connection.CreateTable<BuiltSpeaker>();
            connection.CreateTable<BuiltSpeakerSession>();
            connection.CreateTable<BuiltTracks>();
            connection.CreateTable<BuiltVenue>();
            connection.CreateTable<VenueImage>();
            connection.CreateTable<BuiltTransportation>();
            connection.CreateTable<TransportationIcon>();
            connection.CreateTable<TransLinkGroup>();
            connection.CreateTable<BuiltLastActivity>();
            connection.CreateTable<BuiltSettingsMenu>();
            connection.CreateTable<Menus>();
            connection.CreateTable<Submenus>();
            connection.CreateTable<LinkGroup>();
            connection.CreateTable<BuiltMySession>();
            connection.CreateTable<UserInterestSession>();
            connection.CreateTable<SurveyExtension>();
            connection.CreateTable<BuiltLoginExtensionJson>();
        }

        private static void DropAllTables(SQLiteConnection connection)
        {
            connection.DropTable<BuiltSession>();
            connection.DropTable<BuiltSessionFile>();
            connection.DropTable<BuiltSessionTime>();
            connection.DropTable<BuiltSessionSpeaker>();
            connection.DropTable<BuiltAchievements>();
            connection.DropTable<BuiltAchievementsImage>();
            connection.DropTable<BuiltActivity>();
            connection.DropTable<BuiltAgenda>();
            connection.DropTable<BuiltAgendaItem>();
            connection.DropTable<BuiltConfig>();
            connection.DropTable<BuiltExplore>();
            connection.DropTable<BuiltConfigMenu>();
            connection.DropTable<BuiltConfigSocial>();
            connection.DropTable<BuiltConfigTwitter>();
            connection.DropTable<BuiltAllFeeds>();
            connection.DropTable<BuiltSponsors>();
            connection.DropTable<BuiltOrderingInfo>();
            connection.DropTable<BuiltConfigImportantLinks>();
            connection.DropTable<BuiltImpLinkOrdering>();
            connection.DropTable<BuiltHOLCategoryOrder>();
            connection.DropTable<BuiltHOLCategoryOrderElement>();
            connection.DropTable<BuiltGame>();
            connection.DropTable<BuiltEventNotifications>();
            connection.DropTable<BuiltExhibitor>();
            connection.DropTable<BuiltExhibitorFile>();
            connection.DropTable<BuiltParticipant>();
            connection.DropTable<BuiltSFFoodNDrink>();
            connection.DropTable<BuiltSFFoodNDrinkImage>();
            connection.DropTable<BuiltHandsonLabs>();
            connection.DropTable<BuiltImportantLinks>();
            connection.DropTable<BuiltImpLink>();
            connection.DropTable<BuiltIntro>();
            connection.DropTable<BuiltIntroBgImage>();
            connection.DropTable<BuiltLegal>();
            connection.DropTable<GamingRules>();
            connection.DropTable<LicenseAgreement>();
            connection.DropTable<OpenSourceLicenseAndroid>();
            connection.DropTable<OpenSourceLicenseIos>();
            connection.DropTable<Policy>();
            connection.DropTable<Tnc>();
            connection.DropTable<TrademarkCopyright>();
            connection.DropTable<BuiltMenu>();
            connection.DropTable<BuiltSection>();
            connection.DropTable<BuiltSectionItems>();
            connection.DropTable<MenuIconFile>();
            connection.DropTable<BuiltMosconeCenter>();
            connection.DropTable<VenuePicFile>();
            connection.DropTable<BuiltMyActivity>();
            connection.DropTable<BuiltNews>();
            connection.DropTable<BuiltNewsLink>();
            connection.DropTable<CoverImageFile>();
            connection.DropTable<BuiltNotes>();
            connection.DropTable<NotePhotos>();
            connection.DropTable<BuiltOthers>();
            connection.DropTable<BuiltPendingActivity>();
            connection.DropTable<BuiltPopularSessions>();
            connection.DropTable<BuiltQRCodes>();
            connection.DropTable<BuiltSpeaker>();
            connection.DropTable<BuiltSpeakerSession>();
            connection.DropTable<BuiltSync>();
            connection.DropTable<BuiltTracks>();
            connection.DropTable<BuiltVenue>();
            connection.DropTable<VenueImage>();
            connection.DropTable<BuiltTransportation>();
            connection.DropTable<TransportationIcon>();
            connection.DropTable<BuiltLoginExtension>();
            connection.DropTable<ExtensionApplicationUser>();
            connection.DropTable<Schedule>();
            connection.DropTable<Activity>();
            connection.DropTable<Session>();
            connection.DropTable<Survey>();
            connection.DropTable<Qr>();
            connection.DropTable<ClaimedQr>();
            connection.DropTable<BuiltLastActivity>();
            connection.DropTable<BuiltSettingsMenu>();
            connection.DropTable<Menus>();
            connection.DropTable<Submenus>();
        }

        public static void SetCurrentUser(ExtensionApplicationUser user)
        {
            currentUser = user;
        }

        public static void Initialize(string api_key, string app_uid, string credentials_name = null, string host = null, string url_scheme = null)
        {
            if (!String.IsNullOrWhiteSpace(api_key))
                GlobalValues.ApplicationAPIKey = api_key;
            if (!String.IsNullOrWhiteSpace(app_uid))
                GlobalValues.ApplicationUID = app_uid;

            if (!String.IsNullOrWhiteSpace(credentials_name))
                GlobalValues.CredentialsName = credentials_name;

            BuiltSDK.Built.initializeWithApiKey(GlobalValues.ApplicationAPIKey, GlobalValues.ApplicationUID);
            BuiltAppConstants.BuiltBaseURL = GlobalValues.BuiltBaseURL;
        }

        private static bool IsNetworkAvailable()
        {
            if (IsActiveNetworkAvailable != null)
                return IsActiveNetworkAvailable();
            else
                return false;
        }

        #region--DbOperations--

        #endregion
    }

    public enum TableObserverNames
    {
        menu,
    }
}