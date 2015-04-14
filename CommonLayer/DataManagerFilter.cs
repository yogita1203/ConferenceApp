﻿using BuiltSDK;
using CommonLayer.Entities;
using CommonLayer.Entities.Built;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace CommonLayer
{
    public partial class DataManager
    {
        public static readonly object _SessionLock = new object();
        public static Task<Dictionary<string, BuiltSectionItems[]>> GetLeftMenu(SQLiteConnection connection)
        {
            return Task.Run<Dictionary<string, BuiltSectionItems[]>>(() =>
            {
                List<BuiltMenu> builtMenu;
                lock (_SyncLock)
                {
                    builtMenu = connection.GetAllWithChildren<BuiltMenu>(recursive: true);
                }
                var menus = builtMenu.SelectMany(p => p.section).ToList().OrderBy(p => p.order).ToList();
                menus = menus.Where(p => p.menuitems != null && p.menuitems.Count > 0).ToList();
                var section_menus = menus.GroupBy(p => p.sectionname).ToDictionary(p => p.Key, p => p.SelectMany(q => q.menuitems.OrderBy(r => r.order)).ToArray());

                //return section_menus;
                Dictionary<string, List<BuiltSectionItems>> res = new Dictionary<string, List<BuiltSectionItems>>();
                foreach (var item in section_menus)
                {
                    foreach (var menu in item.Value)
                    {
                        if (!res.ContainsKey(item.Key))
                        {
                            res.Add(item.Key, new List<BuiltSectionItems> { menu });
                        }
                        else
                        {
                            var collection = res[item.Key];
                            var exists = collection.Any(p => p.menuname == menu.menuname);
                            if (!exists)
                            {
                                res[item.Key].Add(menu);
                            }
                        }
                    }
                }
                return res.ToDictionary(p => p.Key, p => p.Value.ToArray());
            });
        }

        public static Task<List<BuiltSectionItems>> GetLeftMenuItems(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltSectionItems>>(() =>
          {
              List<BuiltMenu> builtMenu;
              lock (_SyncLock)
              {
                  builtMenu = connection.GetAllWithChildren<BuiltMenu>(recursive: true);
              }
              var menus = builtMenu.SelectMany(p => p.section).ToList().OrderBy(p => p.order).ToList();
              menus = menus.Where(p => p.menuitems != null && p.menuitems.Count > 0).ToList();
              var sectionItems = menus.SelectMany(u => u.menuitems.OrderBy(r => r.order).ToList());
              return sectionItems.ToList();

          });
        }

        public static Task<List<BuiltSpeaker>> GetSpeakers(SQLiteConnection connection, int limit, int offset)
        {
            return Task.Run<List<BuiltSpeaker>>(() =>
            {
                string query = @"select Id as Value from BuiltSpeaker as d ORDER BY d.first_name LIMIT ? OFFSET ?";
                List<BuiltSpeaker> result = null;
                lock (_SyncLock)
                {
                    var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
                    result = connection.GetAllWithChildren<BuiltSpeaker>(p => ids.Contains(p.Id));
                }
                result = result.OrderBy(p => p.first_name).ToList();
                return result;
            });
        }

        public static Task<List<BuiltSpeaker>> GetSpeakersOnAndroid(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltSpeaker>>(() =>
            {
                List<BuiltSpeaker> result = null;
                lock (_SyncLock)
                {
                    result = connection.GetAllWithChildren<BuiltSpeaker>();
                }
                result = result.OrderBy(p => p.first_name).ToList();
                return result;
            });
        }

        public static Task<List<BuiltExhibitor>> GetExhibitors(SQLiteConnection connection, int limit, int offset)
        {
            string query = @"select Id as Value from BuiltExhibitor as d ORDER BY upper(d.name) LIMIT ? OFFSET ?";
            return Task.Run<List<BuiltExhibitor>>(() =>
            {
                lock (_SyncLock)
                {
                    var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
                    return connection.GetAllWithChildren<BuiltExhibitor>(p => ids.Contains(p.Id));
                }
            });
        }

        public static Task<List<BuiltExhibitor>> GetExhibitorsByType(SQLiteConnection connection, int limit, int offset)
        {
            string query = @"select Id as Value from BuiltExhibitor as d ORDER BY type LIMIT ? OFFSET ?";
            return Task.Run<List<BuiltExhibitor>>(() =>
            {
                lock (_SyncLock)
                {
                    var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
                    return connection.GetAllWithChildren<BuiltExhibitor>(p => ids.Contains(p.Id));
                }
            });
        }

        public static Task<List<BuiltExhibitor>> GetAllExhibitorsByOrderInAndroid(SQLiteConnection connection)
        {
            Func<string, BuiltConfig, int> getSponsorOrder = (type, config) =>
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(type))
                        return 0;

                    var orderInfo = config.sponsors.ordering_info.FirstOrDefault(p => p.type == type);
                    if (orderInfo == null)
                        return 999;
                    else
                    {
                        if (orderInfo.is_hidden)
                            return 0;
                        else
                            return orderInfo.order;
                    }
                }
                catch
                {
                    return 999;
                }
            };

            Func<string, BuiltConfig, string> getModifiedKey = (type, config) =>
            {
                try
                {
                    var orderInfo = config.sponsors.ordering_info.FirstOrDefault(p => p.type == type);
                    if (orderInfo.is_hidden)
                        return String.Empty;
                    else
                        return type;
                }
                catch
                {
                    return type;
                }
            };

            return Task.Run<List<BuiltExhibitor>>(() =>
            {
                try
                {
                    var config = DataManager.GetConfig(connection).Result;
                    if (config == null)
                        return null;
                    List<BuiltExhibitor> lstSponsors = null;
                    lock (_SyncLock)
                    {
                        lstSponsors = connection.GetAllWithChildren<BuiltExhibitor>(p => p.status != null);
                    }
                    var result = lstSponsors.Where(p => p.status.Equals("approved", StringComparison.InvariantCultureIgnoreCase)).OrderBy(p => getModifiedKey(p.type, config)).ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            });
        }

        public static Task<Dictionary<string, List<BuiltExhibitor>>> GetAllExhibitorsByOrder(SQLiteConnection connection)
        {
            Func<string, BuiltConfig, int> getSponsorOrder = (type, config) =>
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(type))
                        return 0;

                    var orderInfo = config.sponsors.ordering_info.FirstOrDefault(p => p.type == type);
                    if (orderInfo == null)
                        return 999;
                    else
                    {
                        if (orderInfo.is_hidden)
                            return 0;
                        else
                            return orderInfo.order;
                    }
                }
                catch
                {
                    return 999;
                }
            };

            Func<string, BuiltConfig, string> getModifiedKey = (type, config) =>
            {
                try
                {
                    var orderInfo = config.sponsors.ordering_info.FirstOrDefault(p => p.type == type);
                    if (orderInfo.is_hidden)
                        return String.Empty;
                    else
                        return type;
                }
                catch
                {
                    return type;
                }
            };

            return Task.Run<Dictionary<string, List<BuiltExhibitor>>>(() =>
            {
                try
                {
                    var config = DataManager.GetConfig(connection).Result;
                    if (config == null)
                        return null;
                    List<BuiltExhibitor> lstSponsors = null;
                    lock (_SyncLock)
                    {
                        lstSponsors = connection.GetAllWithChildren<BuiltExhibitor>(p => p.status != null);
                    }


                    var result = lstSponsors.Where(p => p.status.Equals("approved", StringComparison.InvariantCultureIgnoreCase)).GroupBy(p => getModifiedKey(p.type, config)).ToDictionary(p => p.Key, p => p.ToList()).ToDictionary(p => getModifiedKey(p.Key, config), p => p.Value).OrderBy(p => getSponsorOrder(p.Key, config)).ToDictionary(p => p.Key, p => p.Value);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            });
        }

        public static Task<List<string>> GetExhibitorTypesByOrder(SQLiteConnection connection)
        {
            return Task.Run<List<string>>(() =>
            {
                try
                {
                    var config = DataManager.GetConfig(connection).Result;
                    if (config == null)
                        return null;
                    var result = config.sponsors.ordering_info.Where(p => !p.is_hidden).OrderBy(p => p.order).Select(p => p.type).ToList();
                    return result;
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<Dictionary<string, List<BuiltSpeaker>>> GetSearchSpeakersWithSection(SQLiteConnection connection, string SearchName)
        {
            return Task.Run<Dictionary<string, List<BuiltSpeaker>>>(() =>
            {
                List<BuiltSpeaker> lstSpeakers = null;
                lock (_SyncLock)
                {
                    lstSpeakers = connection.GetAllWithChildren<BuiltSpeaker>();
                }
                Dictionary<string, List<BuiltSpeaker>> result = lstSpeakers.Where(p => p.first_name.ToLower().Contains(SearchName.ToLower()) || p.last_name.ToLower().Contains(SearchName.ToLower()) || p.company_name.ToLower().Contains(SearchName.ToLower())).OrderBy(p => p.first_name).GroupBy(p => p.first_name.First().ToString().ToUpper()).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
                return result;
            });
        }

        public static Task< List<BuiltSpeaker>> GetSearchSpeakersOnAndroid(SQLiteConnection connection, string SearchName)
        {
            return Task.Run<List<BuiltSpeaker>>(() =>
            {
                List<BuiltSpeaker> lstSpeakers = null;
                lock (_SyncLock)
                {
                    lstSpeakers = connection.GetAllWithChildren<BuiltSpeaker>();
                }
                List<BuiltSpeaker> result = lstSpeakers.Where(p => p.first_name.ToLower().Contains(SearchName.ToLower()) || p.last_name.ToLower().Contains(SearchName.ToLower()) || p.company_name.ToLower().Contains(SearchName.ToLower())).OrderBy(p => p.first_name).ToList();
                return result;
            });
        }

        public static Task<Dictionary<string, List<BuiltExhibitor>>> GetSearchExhibitorWithSection(SQLiteConnection connection, string SearchName)
        {
            return Task.Run<Dictionary<string, List<BuiltExhibitor>>>(() =>
            {
                List<BuiltExhibitor> lstSpeakers = null;
                lock (_SyncLock)
                {
                    lstSpeakers = connection.GetAllWithChildren<BuiltExhibitor>(p => p.status != null).Where(p => p.status.Equals("approved", StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
                //Dictionary<string, List<BuiltExhibitor>> result = lstSpeakers.Where(p => p.name.ToLower().Contains(SearchName.ToLower()) || p.booth.ToLower().Contains(SearchName.ToLower())).GroupBy(p => p.name.First().ToString().ToUpper()).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
                Dictionary<string, List<BuiltExhibitor>> result = lstSpeakers.Where(p => p.name.ToLower().Contains(SearchName.ToLower()) || p.booth.ToLower().Contains(SearchName.ToLower())).GroupBy(p => p.type).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
                return result;
            });
        }

        public static Task<List<BuiltSessionTime>> GetSessionTime(SQLiteConnection connection, string date = null)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                List<BuiltSessionTime> result = null;
                if (date != null)
                {
                    lock (_SyncLock)
                    {
                        lock (_SessionLock)
                        {
                            result = connection.GetAllWithChildren<BuiltSessionTime>(p => p.date == date);
                        }
                    }
                    result = result.Where(p => p.BuiltSession != null).ToList();
                    result = result.Where(q => q.BuiltSession.session_published != null).ToList();

                    filterSessionsForUser(currentUser, ref result);
                    return result;
                }
                else
                {
                    lock (_SyncLock)
                    {
                        lock (_SessionLock)
                        {
                            result = connection.GetAllWithChildren<BuiltSessionTime>();
                        }
                    }

                    result = result.Where(p => p.BuiltSession != null).ToList();

                    filterSessionsForUser(currentUser, ref result);
                    return result;
                }
            });
        }

        public static Task<List<BuiltSessionTime>> GetSessionTimeFromIds(SQLiteConnection connection, List<string> ids)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                List<BuiltSessionTime> result = null;
                lock (_SyncLock)
                {
                    lock (_SessionLock)
                    {
                        result = connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.session_time_id)).ToList();
                    }
                }
                result = result.Where(p => p.BuiltSession != null).ToList();
                filterSessionsForUser(currentUser, ref result);
                return result;
            });
        }

        public static Task<List<BuiltSessionTime>> GetSessionTimeFromSessionIds(SQLiteConnection connection, List<string> ids)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                List<BuiltSessionTime> lst = new List<BuiltSessionTime>();
                Func<BuiltSession, List<BuiltSessionTime>> firstSessionTime = (str) =>
                {
                    var first = str.session_time.FirstOrDefault();
                    lst.Add(first);
                    return lst;
                };

                List<BuiltSessionTime> result = null;
                var session = GetAllWithChildren<BuiltSession>(connection, p => ids.Contains(p.session_id)).ToList();
                result = session.SelectMany(s => s.session_time).ToList();
                result = result.Where(p => p.BuiltSession != null).ToList();
                filterSessionsForUser(currentUser, ref result);
                return result;
            });
        }

        public static Task<BuiltSessionTime> GetSessionTimeFromId(SQLiteConnection connection, string id)
        {
            return Task.Run<BuiltSessionTime>(() =>
            {
                List<BuiltSessionTime> result = null;
                lock (_SyncLock)
                {
                    lock (_SessionLock)
                    {
                        result = connection.GetAllWithChildren<BuiltSessionTime>(p => id == p.session_time_id).Where(p => p.BuiltSession != null).ToList();
                    }
                }
                filterSessionsForUser(currentUser, ref result);
                if (result != null)
                    return result.FirstOrDefault();
                else
                    return null;
            });
        }

        public static Task<BuiltSpeaker> GetSpeakerFromId(SQLiteConnection connection, string id)
        {
            return Task.Run<BuiltSpeaker>(() =>
            {
                List<BuiltSpeaker> result = null;
                lock (_SyncLock)
                {
                    result = connection.GetAllWithChildren<BuiltSpeaker>(p => id == p.user_ref);
                }
                if (result != null)
                    return result.FirstOrDefault();
                else
                    return null;
            });
        }

        public static Task<BuiltExhibitor> GetExhibitorFromId(SQLiteConnection connection, string id)
        {
            return Task.Run<BuiltExhibitor>(() =>
            {
                List<BuiltExhibitor> result = null;
                lock (_SyncLock)
                {
                    result = connection.GetAllWithChildren<BuiltExhibitor>(p => id == p.exhibitor_id);
                }
                if (result != null)
                    return result.FirstOrDefault();
                else
                    return null;
            });
        }

        public static string convertToBuiltDate(string p)
        {
            var date = DateTime.Parse(p).ToString("yyyy-MM-dd");
            return date;
        }

        public static string convertToStartDate(string time)
        {
            string date = DateTime.Parse(time).ToString("hh:mm tt").ToLower();
            return date;
        }

        public static Task<List<BuiltSessionTime>> GetbuiltSessionTimeListOfTrack(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                List<BuiltSessionTime> result = null;

                Func<string, int> timeConverter = (str) =>
                {
                    if (String.IsNullOrEmpty(str))
                        return 0;
                    str = str.Replace(":", String.Empty);
                    return Convert.ToInt32(str);
                };


                var PoularSessObj = GetAllWithChildren<BuiltPopularSessions>(connection);
                var PopularSession_ids = PoularSessObj.SelectMany(p => p.session_separated.Split('|')).ToList();
                lock (_SessionLock)
                {
                    result = GetAllWithChildren<BuiltSession>(connection, p => PopularSession_ids.Contains(p.session_id)).SelectMany(p => p.session_time).Where(p => p.BuiltSession != null).ToList();
                }

                //var resultTmp1 = result;
                //var resultTmp2 = result;
                //resultTmp1 = resultTmp1.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) == DateTime.Today).ToList();
                //resultTmp1 = resultTmp1.Where(x => timeConverterForBuiltTimeString(x.time) > timeConverterForCurrentHourMinute()).ToList();
                //resultTmp1 = resultTmp1.Where(p => p.BuiltSession != null).ToList();
                //resultTmp2 = resultTmp2.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) > DateTime.Today).ToList();
                //resultTmp2 = resultTmp2.Where(p => p.BuiltSession != null).ToList();
                //resultTmp1.AddRange(resultTmp2);
                //result = resultTmp1;

                result = result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => MergeDateAndTime(x.date, x.time) > TimeZoneInfo.ConvertTime(DateTime.Now, destinationTimeZone)).ToList();
                // result = result.Where(p => p.BuiltSession != null).ToList();
                filterSessionsForUser(currentUser, ref result);
                return result;

            });
        }

        public static int timeConverterForBuiltTimeString(string str)
        {
            DateTime.Parse(str).ToString("HH:MM");
            str = str.Replace(":", String.Empty);
            return Convert.ToInt32(str);
        }

        public static int timeConverterForCurrentHourMinute()
        {
            var dt = TimeZoneInfo.ConvertTime(DateTime.Now, destinationTimeZone);
            string strNow = dt.ToString("HH") + dt.ToString("MM");
            return Convert.ToInt32(strNow);
        }

        public static Task<Dictionary<string, List<BuiltSessionTime>>> GetMyScheduleToday(SQLiteConnection connection)
        {
            #region --Today Tomorrow Calculation
            Func<string, string> convertToDate = (Date) =>
            {
                var date = DateTime.Parse(Date).Date;
                if (date.Day == DateTime.Now.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Today";
                    return Date;
                }
                else if (date.Day == DateTime.Now.Day + 1 && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Tomorrow";
                    return Date;
                }
                else
                {
                    var date1 = DateTime.Parse(Date).ToString("ddd, MMM dd");
                    return date1;
                }
            };
            #endregion
            Func<string, int> timeConverter = (str) =>
            {
                if (String.IsNullOrEmpty(str))
                    return 0;
                str = str.Replace(":", String.Empty);
                return Convert.ToInt32(str);
            };
            return Task.Run<Dictionary<string, List<BuiltSessionTime>>>(() =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        //var data = connection.GetAllWithChildren<BuiltLoginExtensionJson>().FirstOrDefault();
                        //var loginData = Newtonsoft.Json.JsonConvert.DeserializeObject<BuiltLoginExtension>(data.login_json);
                        //var ids = loginData.application_user.schedule.session_time;

                        var data = GetAllWithChildren<BuiltMySession>(connection);
                        var ids = data.Select(p => p.session_time_id).ToList();


                        var result = GetAllWithChildren<BuiltSessionTime>(connection, p => ids.Contains(p.session_time_id));
                        result = result.Where(p => p.BuiltSession != null).GroupBy(p => p.BuiltSession.session_id).Select(q => q.First()).ToList();

                        filterSessionsForUser(currentUser, ref result);

                        ////  result = result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).ToList();
                        ////  result = result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) >= DateTime.Today && timeConverterForBuiltTimeString(x.time) > timeConverterForCurrentHourMinute()).ToList();

                        ////16-12-2014///  result = result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) >= DateTime.Today).ToList();
                        ////16-12-2014 ////result = result.Where(x => timeConverterForBuiltTimeString(x.time) > timeConverterForCurrentHourMinute()).ToList();
                        ////16-12-2014 ////result = result.Where(p => p.BuiltSession != null).ToList();
                        var resultTmp1 = result;
                        var resultTmp2 = result;
                        resultTmp1 = resultTmp1.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) == TimeZoneInfo.ConvertTime(DateTime.Today, destinationTimeZone).Date).ToList();
                        resultTmp1 = resultTmp1.Where(x => timeConverterForBuiltTimeString(x.time) > timeConverterForCurrentHourMinute()).ToList();
                        resultTmp1 = resultTmp1.Where(p => p.BuiltSession != null).ToList();
                        resultTmp2 = resultTmp2.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => timeConverter(p.time)).Where(x => Convert.ToDateTime(x.date) > TimeZoneInfo.ConvertTime(DateTime.Today, destinationTimeZone).Date).ToList();
                        resultTmp2 = resultTmp2.Where(p => p.BuiltSession != null).ToList();
                        resultTmp1.AddRange(resultTmp2);
                        var dictResult = resultTmp1.GroupBy(p => convertToDate(p.date)).ToDictionary(p => p.Key, p => p.ToList());
                        return dictResult;
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<List<BuiltTracks>> GetListOfTrack(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltTracks>>(() =>
            {
                List<BuiltTracks> trackObj = null;

                //lock (_SyncLock)
                {
                    trackObj = GetAllWithChildren<BuiltTracks>(connection, p => p.parentTrackName == null);

                    var PoularSessObj = GetAllWithChildren<BuiltPopularSessions>(connection);
                    var PopularSession_ids = PoularSessObj.SelectMany(p => p.session_separated.Split('|')).ToList();

                    var res = GetAllWithChildren<BuiltSession>(connection, p => PopularSession_ids.Contains(p.session_id)).SelectMany(p => p.session_time).ToList();
                    var result = res.Select(p => p.BuiltSession.track).Distinct().ToList();
                    result = result.Select<string, string>(s => s == null || s == "" || s == " " ? "No Track" : s).ToList();

                    trackObj = trackObj.Where(v => result.Contains(v.name)).ToList();
                    trackObj = trackObj.GroupBy(p => p.name).Select(e => e.First()).ToList();
                }
                // result = result.Where(p => p.BuiltSession != null).ToList();
                return trackObj;

            });
        }

        public static Task<List<BuiltTracks>> GetListOfAllTrack(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltTracks>>(() =>
            {
                List<BuiltTracks> trackObj = null;

                lock (_SyncLock)
                {
                    trackObj = connection.GetAllWithChildren<BuiltTracks>(p => p.parentTrackName == null);
                }

                return trackObj;

            });
        }

        public static Task<List<BuiltOthers>> GetListOfProgramsFromOthers(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltOthers>>(() =>
            {
                List<BuiltOthers> lstprograms = null;
                lock (_SyncLock)
                {
                    lstprograms = connection.GetAllWithChildren<BuiltOthers>(p => p.title != null);
                }
                return lstprograms;
            });
        }

        public static Task<List<BuiltVenue>> GetListOfVenues(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltVenue>>(() =>
            {
                List<BuiltVenue> lstVenues = null;
                lock (_SyncLock)
                {
                    lstVenues = connection.GetAllWithChildren<BuiltVenue>(p => p.name != null);
                }
                return lstVenues;
            });
        }

        public static Task<List<BuiltIntro>> GetListOfIntroData(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltIntro>>(() =>
            {
                List<BuiltIntro> introObj = null;

                lock (_SyncLock)
                {
                    introObj = connection.GetAllWithChildren<BuiltIntro>();
                }
                // result = result.Where(p => p.BuiltSession != null).ToList();
                return introObj;

            });
        }

        public static Task<List<string>> GetListOfExhibitorTypes(SQLiteConnection connection)
        {
            return Task.Run<List<string>>(() =>
             {
                 List<string> exhibitorObj = null;

                 lock (_SyncLock)
                 {
                     exhibitorObj = connection.GetAllWithChildren<BuiltExhibitor>().Where(p=> !string.IsNullOrWhiteSpace(p.type)).Select(p=>(p.type)).Distinct().ToList();
                 }
                 return exhibitorObj;
             });
        }

        public static Task<BuiltSessionTime[]> GetMyScheduleTodayArray(SQLiteConnection connection)
        {
            Func<string, string> convertToDate = (Date) =>
            {
                var date = DateTime.Parse(Date).Date;
                if (date.Day == DateTime.Now.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Today";
                    return Date;
                }
                else if (date.Day == DateTime.Now.Day + 1 && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Tomorrow";
                    return Date;
                }
                else
                {
                    var date1 = DateTime.Parse(Date).ToString("ddd, MMM dd");
                    return date1;
                }
            };

            return Task.Run<BuiltSessionTime[]>(() =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        var data = GetAllWithChildren<BuiltMySession>(connection);
                        var ids = data.Select(p => p.session_time_id).ToList();

                        var result = GetAllWithChildren<BuiltSessionTime>(connection, p => ids.Contains(p.session_time_id));
                        result = result.Where(p => p.BuiltSession != null).GroupBy(p => p.BuiltSession.session_id).Select(q => q.First()).ToList();
                        var Latestresult = result.Where(p => p.BuiltSession != null).ToArray();
                        filterSessionsForUser(currentUser, ref Latestresult);
                        return Latestresult;
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<List<UserInterestSession>> GetMyInterest(SQLiteConnection connection)
        {
            return Task.Run<List<UserInterestSession>>(() =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        return connection.GetAllWithChildren<UserInterestSession>(recursive: true);
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<BuiltLoginExtension> GetCurrentUser(SQLiteConnection connection)
        {
            return Task.Run<BuiltLoginExtension>(() =>
            {
                try
                {
                    //lock (_SyncLock)
                    {
                        var data = GetAllWithChildren<BuiltLoginExtensionJson>(connection).FirstOrDefault();
                        if (data != null)
                        {
                            var loginData = Newtonsoft.Json.JsonConvert.DeserializeObject<BuiltLoginExtension>(data.login_json);
                            return loginData;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<BuiltAgendaItem[]> GetAllAgendaItem(SQLiteConnection connection)
        {
            return Task.Run<BuiltAgendaItem[]>(() =>
            {
                List<BuiltAgenda> agenda = null;
                lock (_SyncLock)
                {
                    agenda = connection.GetAllWithChildren<BuiltAgenda>();
                }
                agenda = agenda.Where(p => p.item != null).ToList();
                var agendaItem = agenda.SelectMany(p => p.item).ToArray();
                return agendaItem;
            });
        }

        public static Task<List<BuiltHandsonLabs>> GetHandsOnLabs(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltHandsonLabs>>(() =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        var result = connection.GetAllWithChildren<BuiltHandsonLabs>(p => p.title != null, recursive: true);
                        return result;
                    }
                }
                catch
                {
                    return null;
                }
            });
        }
        public static Task<BuiltHandsonLabs> GetHandsOnLabsUsingSessionId(SQLiteConnection connection, string id)
        {
            return Task.Run<BuiltHandsonLabs>(() =>
            {
                try
                {
                    lock (_SyncLock)
                    {
                        var result = connection.GetAllWithChildren<BuiltHandsonLabs>(p => id == p.session_id, recursive: true).FirstOrDefault();
                        return result;
                    }
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<Dictionary<string, List<BuiltHandsonLabs>>> GetSectionedhandsOnLabs(SQLiteConnection connection)
        {
            return Task.Run<Dictionary<string, List<BuiltHandsonLabs>>>(() =>
            {
                List<BuiltHandsonLabs> holSections;
                lock (_SyncLock)
                {
                    holSections = connection.GetAllWithChildren<BuiltHandsonLabs>(recursive: true);
                }
                var dict = holSections.GroupBy(p => p.hol_category).ToDictionary(q => q.Key, q => q.ToList());
                return dict;



            });

        }

        public static Task<List<BuiltSessionTime>> GetSessions(SQLiteConnection connection, int limit, int offset)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                string query = @"select Id as Value from BuiltSessionTime as d ORDER BY d.DATE, d.TIME   LIMIT ? OFFSET ?";


                List<BuiltSessionTime> result = null;
                lock (_SyncLock)
                {
                    lock (_SessionLock)
                    {
                        var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
                        result = connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id), true);
                    }
                }

                result = result.Where(q => q.BuiltSession != null).ToList();
                result = result.Where(u => !string.IsNullOrEmpty(u.BuiltSession.session_published)).ToList();
                filterSessionsForUser(currentUser, ref result);
                return result;

            });
        }

        //public static Task<Dictionary<string, List<BuiltSessionTime>>> GetSessionsByTimeGroup(SQLiteConnection connection, string date, int limit, int offset)
        //{
        //    return Task.Run<Dictionary<string, List<BuiltSessionTime>>>(() =>
        //    {
        //        string query = @"select Id as Value from BuiltSessionTime as d where d.DATE = ? ORDER BY d.DATE, d.TIME   LIMIT ? OFFSET ?";


        //        List<BuiltSessionTime> sessions = null;
        //        lock (_SyncLock)
        //        {
        //            var ids = connection.Query<ID>(query, date, limit, offset).Select(p => p.Value).ToList();
        //            sessions = connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id), true);
        //        }

        //        sessions = sessions.Where(q => q.BuiltSession != null).ToList();
        //        sessions = sessions.Where(u => !string.IsNullOrEmpty(u.BuiltSession.session_published)).ToList();
        //        filterSessionsForUser(currentUser, ref sessions);
        //        return sessions.GroupBy(p => getTimeKey(p.time)).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
        //    });
        //}

        //public static Task<Dictionary<string, List<BuiltSessionTime>>> GetSessionsByDayGroup(SQLiteConnection connection, int limit, int offset)
        //{
        //    return Task.Run<Dictionary<string, List<BuiltSessionTime>>>(() =>
        //    {
        //        string query = @"select Id as Value from BuiltSessionTime as d ORDER BY d.DATE, d.TIME   LIMIT ? OFFSET ?";


        //        List<BuiltSessionTime> sessions = null;
        //        lock (_SyncLock)
        //        {
        //            var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
        //            sessions = connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id), true);
        //        }

        //        sessions = sessions.Where(q => q.BuiltSession != null).ToList();
        //        sessions = sessions.Where(u => !string.IsNullOrEmpty(u.BuiltSession.session_published)).ToList();
        //        filterSessionsForUser(currentUser, ref sessions);
        //        return sessions.GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList());
        //    });
        //}

        //public static Task<Dictionary<string, Dictionary<string, List<BuiltSessionTime>>>> GetSessionsByDayTimeGroup(SQLiteConnection connection, int limit, int offset)
        //{
        //    return Task.Run<Dictionary<string, Dictionary<string, List<BuiltSessionTime>>>>(() =>
        //    {
        //        string query = @"select Id as Value from BuiltSessionTime as d ORDER BY d.DATE, d.TIME   LIMIT ? OFFSET ?";


        //        List<BuiltSessionTime> sessions = null;
        //        lock (_SyncLock)
        //        {
        //            var ids = connection.Query<ID>(query, limit, offset).Select(p => p.Value).ToList();
        //            sessions = connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id), true);
        //        }

        //        sessions = sessions.Where(q => q.BuiltSession != null).ToList();
        //        sessions = sessions.Where(u => !string.IsNullOrEmpty(u.BuiltSession.session_published)).ToList();
        //        filterSessionsForUser(currentUser, ref sessions);
        //        return sessions.GroupBy(p => p.date).OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.ToList().GroupBy(q => getTimeKey(q.time)).OrderBy(q => p.Key).ToDictionary(q => p.Key, q => q.ToList()));
        //    });
        //}

        public static Task<List<BuiltSessionTime>> GetSpeakerSessions(SQLiteConnection connection, List<string> sessionIds)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                List<BuiltSession> sessions = null;
                lock (_SyncLock)
                {
                    lock (_SessionLock)
                    {
                        sessions = connection.GetAllWithChildren<BuiltSession>(p => sessionIds.Contains(p.session_id));
                    }
                }
                var result = sessions.SelectMany(p => p.session_time).ToList();
                result = result.OrderBy(p => p.date).ThenBy(p => p.time).ToList();
                filterSessionsForUser(currentUser, ref result);
                return result;
            });
        }

        public static Task<List<BuiltTracks>> GetTracks(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltTracks>>(() =>
            {
                lock (_SyncLock)
                {
                    //return connection.GetAllWithChildren<BuiltTracks>().Where(p => p.parentTrackName == null).ToList();
                    return connection.GetAllWithChildren<BuiltTracks>().ToList();
                }
            });
        }

        public static Task<List<string>> GetSessionTracks(SQLiteConnection connection)
        {
            string query = @"select DISTINCT Track as Value from BuiltSession where Track!= ''  and Track in (Select Name from BuiltTracks where parentTrackName is NULL)";
            return Task.Run<List<string>>(() =>
            {
                lock (_SyncLock)
                {
                    var tmplst = connection.Query<Date>(query).ToList();
                    return tmplst.Select(u => u.Value).ToList();
                }
            });
        }

        public static Task<List<BuiltSettingsMenu>> GetSettingsMenu(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltSettingsMenu>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltSettingsMenu>(recursive: true);
                }
            });
        }

        public static Task<List<BuiltEventNotifications>> GetEventNotifications(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltEventNotifications>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltEventNotifications>();
                }
            });
        }

        public static Task<DateTime> GetLastSyncedTime(SQLiteConnection connection)
        {
            return Task.Run<DateTime>(() =>
            {
                lock (_SyncLock)
                {
                    var str = connection.Get<BuiltConfig>(p => p.uid == p.uid).last_synced_delta_time;
                    return Convert.ToDateTime(str);
                }
            });
        }

        public static Task<List<BuiltLegal>> GetLegal(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltLegal>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltLegal>();
                }
            });
        }

        public static Task<List<BuiltOthers>> GetOthers(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltOthers>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltOthers>();
                }
            });
        }

        public static Task<List<BuiltNotes>> GetNotes(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltNotes>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltNotes>().OrderByDescending(p => Convert.ToDateTime(p.created_at)).ToList();
                }
            });
        }

        public static Task<List<BuiltNotes>> GetNotesByID(SQLiteConnection connection, string uid)
        {
            return Task.Run<List<BuiltNotes>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltNotes>(p => (p.uid == uid)).ToList();
                }
            });
        }

        public static Task<List<BuiltNews>> GetNews(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltNews>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltNews>().OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();
                }
            });
        }

        public static Task<List<BuiltImportantLinks>> GetImpLinks(SQLiteConnection connection)
        {
            return Task.Run<List<BuiltImportantLinks>>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltImportantLinks>();
                }
            });
        }

        public static Task<BuiltConfig> GetConfig(SQLiteConnection connection)
        {
            return Task.Run<BuiltConfig>(() =>
            {
                lock (_SyncLock)
                {
                    return connection.GetAllWithChildren<BuiltConfig>(recursive: true).FirstOrDefault();
                }
            });
        }

        public static Task<Dictionary<string, BuiltSFFoodNDrink[]>> GetFoodNDrink(SQLiteConnection connection)
        {
            return Task.Run<Dictionary<string, BuiltSFFoodNDrink[]>>(() =>
            {
                List<BuiltSFFoodNDrink> foodAndDrinks;
                lock (_SyncLock)
                {
                    foodAndDrinks = connection.GetAllWithChildren<BuiltSFFoodNDrink>(recursive: true);
                }
                var foodAndDrinksList = foodAndDrinks.OrderBy(p => p.venue_name).GroupBy(p => p.category ?? String.Empty).ToDictionary(p => p.Key, p => p.ToArray());
                return foodAndDrinksList;
            });
        }

        public static Task<Dictionary<string, BuiltTransportation[]>> GetTransportation(SQLiteConnection connection)
        {
            return Task.Run<Dictionary<string, BuiltTransportation[]>>(() =>
            {
                List<BuiltTransportation> transportation;
                lock (_SyncLock)
                {
                    transportation = connection.GetAllWithChildren<BuiltTransportation>(recursive: true);
                }
                var transportationList = transportation.OrderBy(p => p.name).GroupBy(p => p.category).ToDictionary(p => p.Key, p => p.ToArray());
                return transportationList;
            });
        }

        public static Task<BuiltTransportation[]> GetTransportationItem(SQLiteConnection connection)
        {
            return Task.Run<BuiltTransportation[]>(() =>
            {
                List<BuiltTransportation> transportation;
                lock (_SyncLock)
                {
                    transportation = connection.GetAllWithChildren<BuiltTransportation>(recursive: true);
                }
                var transportationList = transportation.OrderBy(p => p.name).OrderBy(p => p.category).ToArray();
                return transportationList;
            });
        }

        public static Task<List<string>> GetUniqueSessionDates(SQLiteConnection connection)
        {
            return Task.Run<List<string>>(() =>
            {
                lock (_SyncLock)
                {
                    var query = @"select DISTINCT DATE as Value from BuiltSessionTime where BuiltSessionId in (select Id from BuiltSession where session_published != '' and status = ? COLLATE NOCASE) order by DATE";
                    return connection.Query<Date>(query, "Approved").Select(p => p.Value).Where(p => !String.IsNullOrWhiteSpace(p)).ToList();
                }
            });
        }

        public static void filterSessionsForUser(ExtensionApplicationUser user, ref List<BuiltSessionTime> sessions)
        {
            try
            {
                sessions = sessions.Where(q => !string.IsNullOrWhiteSpace(q.BuiltSession.session_published) && !string.IsNullOrWhiteSpace(q.BuiltSession.status) && !String.IsNullOrWhiteSpace(q.date) && !String.IsNullOrWhiteSpace(q.time)).ToList();
            }
            catch { }

            if (user != null)
            {
                try
                {
                    var pvps = user.private_view_private_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.private_view_private_schedule) || pvps.Contains(p.BuiltSession.private_view_private_schedule)).ToList();
                }
                catch
                { }

                try
                {
                    var cvos = user.cannot_view_or_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.cannot_view_or_schedule) || !cvos.Contains(p.BuiltSession.cannot_view_or_schedule)).ToList();
                }
                catch
                { }
            }
            else
            {
                try
                {
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.private_view_private_schedule)).ToList();
                }
                catch
                { }

                try
                {
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.cannot_view_or_schedule)).ToList();
                }
                catch
                { }
            }
        }

        public static void filterSessionsForUser(ExtensionApplicationUser user, ref BuiltSessionTime[] sessions)
        {
            try
            {
                sessions = sessions.Where(q => !string.IsNullOrEmpty(q.BuiltSession.session_published) && !string.IsNullOrEmpty(q.BuiltSession.status) && !String.IsNullOrWhiteSpace(q.date) && !String.IsNullOrWhiteSpace(q.time)).ToArray();
            }
            catch { }

            if (user != null)
            {
                try
                {
                    var pvps = user.private_view_private_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.private_view_private_schedule) || pvps.Contains(p.BuiltSession.private_view_private_schedule)).ToArray();
                }
                catch
                { }

                try
                {
                    var cvos = user.cannot_view_or_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.cannot_view_or_schedule) || !cvos.Contains(p.BuiltSession.cannot_view_or_schedule)).ToArray();
                }
                catch
                { }
            }
            else
            {
                try
                {
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.private_view_private_schedule)).ToArray();
                }
                catch
                { }

                try
                {
                    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.cannot_view_or_schedule)).ToArray();
                }
                catch
                { }
            }
        }

        public static Task<List<BuiltSessionTime>> GetNextUpSessions(SQLiteConnection connection, int limit, ExtensionApplicationUser user)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                try
                {
                    List<BuiltSessionTime> result = new List<BuiltSessionTime>();
                    //int offset = 0;

                    string query = String.Empty;
                    var dt = TimeZoneInfo.ConvertTime(DateTime.Now, destinationTimeZone).ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                    List<int> ids = null;
                    if (user == null)
                    {
                        query = @"select Id as Value from BuiltSessionTime as d where datetime(d.DATE || ' ' || d.TIME) > datetime(?) and d.BuiltSessionId in (select Id from BuiltSession where status = 'Approved' and session_published != '' and private_view_private_schedule = '' and cannot_view_or_schedule = '') order by datetime(d.DATE || ' ' || d.TIME) limit ?";

                        lock (_SyncLock)
                        {
                            lock (_SessionLock)
                            {
                                ids = connection.Query<ID>(query, dt, limit).Select(p => p.Value).ToList();
                            }
                        }
                    }
                    else
                    {
                        var pvps = user.private_view_private_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => "'" + p + "'");
                        var pvps_string = string.Join(",", pvps);

                        //    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.private_view_private_schedule) || pvps.Contains(p.BuiltSession.private_view_private_schedule)).ToList();

                        var cvos = user.cannot_view_or_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => "'" + p + "'");
                        var cvos_string = string.Join(",", cvos);
                        //    sessions = sessions.Where(p => String.IsNullOrWhiteSpace(p.BuiltSession.cannot_view_or_schedule) || !cvos.Contains(p.BuiltSession.cannot_view_or_schedule)).ToList();

                        query = @"select Id as Value from BuiltSessionTime as d where datetime(d.DATE || ' ' || d.TIME) > datetime(?) and d.BuiltSessionId in (select Id from BuiltSession where status = 'Approved' and session_published != '' and (private_view_private_schedule = '' or private_view_private_schedule in (?)) and (cannot_view_or_schedule = '' or cannot_view_or_schedule not in (?))) order by datetime(d.DATE || ' ' || d.TIME) limit ?";
                        lock (_SyncLock)
                        {
                            lock (_SessionLock)
                            {
                                ids = connection.Query<ID>(query, dt, pvps_string, cvos_string, limit).Select(p => p.Value).ToList();
                            }
                        }
                    }

                    //while (result.Count < limit)
                    //{
                    //    lock (_SyncLock)
                    //    {
                    //ids = connection.Query<ID>(query, dt, limit).Select(p => p.Value).ToList();
                    //if (ids.Count == 0)
                    //    break;
                    lock (_SessionLock)
                    {
                        result.AddRange(connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id)));
                    }
                    filterSessionsForUser(user, ref result);
                    //        offset += limit;
                    //    }
                    //}

                    //return result.Take(limit).ToList();
                    return result;
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<List<BuiltSessionTime>> GetOnNowSessions(SQLiteConnection connection, int limit)
        {
            return Task.Run<List<BuiltSessionTime>>(() =>
            {
                try
                {
                    List<BuiltSessionTime> result = new List<BuiltSessionTime>();

                    string query = String.Empty;
                    var dt = TimeZoneInfo.ConvertTime(DateTime.Now, destinationTimeZone).ToString("yyyy'-'MM'-'dd HH':'mm':'ss");
                    List<int> ids = null;
                    if (currentUser == null)
                    {
                        query = @"select Id as Value from BuiltSessionTime as d where datetime(?) BETWEEN datetime(d.DATE || ' ' || d.TIME) and datetime(d.DATE || ' ' || d.TIME, '+' || d.length || ' minutes') and d.BuiltSessionId in (select Id from BuiltSession where status = 'Approved' and session_published != '' and private_view_private_schedule = '' and cannot_view_or_schedule = '') order by datetime(d.DATE || ' ' || d.TIME) limit ?";

                        lock (_SyncLock)
                        {
                            lock (_SessionLock)
                            {
                                ids = connection.Query<ID>(query, dt, limit).Select(p => p.Value).ToList();
                            }
                        }
                    }
                    else
                    {
                        var pvps = currentUser.private_view_private_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => "'" + p + "'");
                        var pvps_string = string.Join(",", pvps);

                        var cvos = currentUser.cannot_view_or_schedule.Replace(" ", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => "'" + p + "'");
                        var cvos_string = string.Join(",", cvos);

                        query = @"select Id as Value from BuiltSessionTime as d where datetime(?) BETWEEN datetime(d.DATE || ' ' || d.TIME) and datetime(d.DATE || ' ' || d.TIME, '+' || d.length || ' minutes') and d.BuiltSessionId in (select Id from BuiltSession where status = 'Approved' and session_published != '' and (private_view_private_schedule = '' or private_view_private_schedule in (?)) and (cannot_view_or_schedule = '' or cannot_view_or_schedule not in (?))) order by datetime(d.DATE || ' ' || d.TIME) limit ?";
                        lock (_SyncLock)
                        {
                            lock (_SessionLock)
                            {
                                ids = connection.Query<ID>(query, dt, pvps_string, cvos_string, limit).Select(p => p.Value).ToList();
                            }
                        }
                    }

                    lock (_SessionLock)
                    {
                        result.AddRange(connection.GetAllWithChildren<BuiltSessionTime>(p => ids.Contains(p.Id)));
                    }
                    filterSessionsForUser(currentUser, ref result);
                    //return result.Take(limit).ToList();
                    return result;
                }
                catch
                {
                    return null;
                }
            });
        }

        public static Task<List<EventInformation>> GetEvents(SQLiteConnection connection)
        {
            return Task.Run<List<EventInformation>>(() =>
            {
                return lstEventInformation;
            });
        }

        public static List<EventUid> GetEventProperties(List<EventInformation> info)
        {
            List<EventData> lstEventData = new List<EventData>();
            EventData obj;
            foreach (var item in info)
            {
                obj = new EventData
                {
                    uid = item.uid,
                    properties = new Dictionary<string, object> { { "timestamp", item.timestamp } },
                    previous_event_uid = item.previous_event_uid
                };

                if (item.Extras != null)
                {
                    foreach (var extra in item.Extras)
                    {
                        if (!obj.properties.ContainsKey(extra.Key))
                            obj.properties.Add(extra.Key, extra.Value);
                    }
                }

                lstEventData.Add(obj);
            }

            var result = lstEventData.GroupBy(p => p.uid).Select(p => new EventUid
            {
                uid = p.Key,
                properties = p.ToList().Select(q => q.properties).ToArray(),
                previous_event_uid = p.ToList().Select(q => q.previous_event_uid).FirstOrDefault()
            }).ToList();

            return result;
        }

        static string previous_event_uid;
        static List<EventInformation> lstEventInformation = new List<EventInformation>();
        public static void AddEventInfo(SQLiteConnection connection, string uid, string timestamp, Dictionary<string, object> extras = null)
        {
            Task.Run(() =>
            {
                EventInformation ei = new EventInformation
                {
                    uid = uid,
                    timestamp = timestamp,
                    previous_event_uid = previous_event_uid,
                    Extras = extras
                };
                lstEventInformation.Add(ei);
                previous_event_uid = uid;
            });
        }
        #region --Helpers--
        static Func<string, string> getTimeKey = (time) =>
        {
            try
            {
                var key = DateTime.Parse(time).ToString("hh:mm tt");
                return key;
            }
            catch
            {
                return string.Empty;
            }
        };
        #endregion

    }
}
