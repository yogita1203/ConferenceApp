using BuiltSDK;
using CommonLayer.Entities.Built;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{
    public class ApiCalls
    {
        public const string session = "session";
        public const string achievements = "achievements";
        public const string agenda = "agenda";
        public const string event_notifications = "event_notifications";
        public const string exhibitor = "exhibitor";
        public const string imp_links = "imp_links";
        public const string intro = "intro";
        public const string legal = "legal";
        public const string left_menu = "left_menu";
        public const string moscone_center = "moscone_center";
        public const string my_activity = "my_activity";
        public const string news = "news";
        public const string notes = "notes";
        public const string others = "others";
        public const string qrcodes = "qrcodes";
        public const string food_n_drink = "food_n_drink";
        public const string transportation = "transportation";
        public const string speaker = "speaker";
        public const string track = "track";
        public const string config = "config";
        public const string hol = "hol";
        public const string recommended = "recommended";
        public const string venue = "venue";
        public const string settings_menu = "settings_menu";
        //const string sync = "sync";
        //const string pending_activity = "pending_activity";

        public static void fetchSession<T>(int skip, int limit, Action<QueryResult<T>, int> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(session);
                builtObject.where("status", "Approved");
                builtObject.notEqualTo("session_published", "");
                builtObject.skip(skip);
                builtObject.limit(limit);
                builtObject.includeCount();
                builtObject.exec(t =>
                {
                    success(t, t.totalCount);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaSession<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(session);

                builtDelta.where("status", "Approved");
                builtDelta.notEqualTo("session_published", "");

                builtDelta.allDeltaAt(dateTime);

                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchAchievements<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(achievements);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaAchievements<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(achievements);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchAgenda<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(agenda);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaAgenda<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(agenda);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchEventNotification<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>("event_notifications");
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaEventNotification<T>(DateTime dateTime, ExtensionApplicationUser user, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>("event_notifications");
                builtDelta.allDeltaAt(dateTime);
                if (user != null)
                {
                    builtDelta.setHeader("authtoken", user.authtoken);
                }
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchExhibitor<T>(int skip, int limit, Action<QueryResult<T>, int> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(exhibitor);

                builtObject.skip(skip);
                builtObject.limit(limit);
                builtObject.includeCount();

                builtObject.exec(t =>
                {
                    success(t, t.totalCount);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaExhibitor<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(exhibitor);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchImportantLinks<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(imp_links);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaImportantLinks<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(imp_links);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchIntro<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(intro);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaIntro<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(intro);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchLegal<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(legal);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaLegal<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(legal);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchMenu<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(left_menu);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaMenu<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(left_menu);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        //public static void fetchMosconeCentre<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        //{
        //    try
        //    {
        //        BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(moscone_center);
        //        builtObject.exec(t =>
        //        {
        //            success(t);
        //        }, t =>
        //        {
        //            error(t);
        //        });
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //public static void getDeltaMosconeCentre<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        //{
        //    try
        //    {
        //        BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(moscone_center);
        //        builtDelta.allDeltaAt(dateTime);
        //        builtDelta.exec(s =>
        //        {
        //            success(s);
        //        }, e =>
        //        {
        //            builtError(e);
        //        });
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        public static void fetchMyActivity<T>(ExtensionApplicationUser user, Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(my_activity);

                if (user != null)
                {
                    builtObject.where("client_id", user.external_api_identifier);
                    builtObject.where("user_uid", user.uid);
                }

                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaMyActivity<T>(ExtensionApplicationUser user, DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(my_activity);

                if (user != null)
                {
                    builtDelta.where("client_id", user.external_api_identifier);
                    builtDelta.where("user_uid", user.uid);
                }

                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchNews<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(news);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaNews<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(news);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchNotes<T>(ExtensionApplicationUser user, Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(notes);
                if (user != null)
                {
                    //builtObject.where("client_id", user.external_api_identifier);
                    //builtObject.where("user_uid", user.uid);
                    //builtObject.where("app_user_object_uid", user.uid);
                    builtObject.setHeader("authtoken", user.authtoken);
                }
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaNotes<T>(ExtensionApplicationUser user, DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(notes);
                if (user != null)
                {
                    //builtDelta.where("client_id", user.external_api_identifier);
                    //builtDelta.where("user_uid", user.uid);
                    builtDelta.setHeader("authtoken", user.authtoken);
                }
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchOthers<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(others);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaOthers<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(others);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchQrcodes<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(qrcodes);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaQrcodes<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(qrcodes);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchFoodAndDrink<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(food_n_drink);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaFoodAndDrink<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(food_n_drink);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchTransportation<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(transportation);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaTransportation<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(transportation);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchSpeaker<T>(int skip, int limit, Action<QueryResult<T>, int> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(speaker);
                builtObject.skip(skip);
                builtObject.limit(limit);
                builtObject.includeCount();

                builtObject.exec(t =>
                {
                    success(t, t.totalCount);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaSpeaker<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(speaker);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchTrack<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(track);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaTrack<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(track);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchConfig<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(config);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaConfig<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(config);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchHandsOnLabs<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(hol);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaHandsOnLabs<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(hol);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchPopularSessions<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(recommended);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaPopularSessions<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(recommended);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }


        //public static void fetchNotificationInWebView<T>( Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        //{
        //    try
        //    {
        //        //BuiltSDK.Built.initialize(GlobalValues.ApplicationAPIKey, GlobalValues.ApplicationUID);
        //        //BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
        //        BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(classUid);
        //        builtObject.exec(t =>
        //        {
        //            success(t);
        //        }, t =>
        //        {
        //            error(t);
        //        });
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        //public static void getDeltaNotificationInWebView<T>(string classUid, DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        //{
        //    try
        //    {
        //        //BuiltSDK.Built.initialize(GlobalValues.ApplicationAPIKey, GlobalValues.ApplicationUID);
        //        //BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
        //        BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(classUid);
        //        builtDelta.allDeltaAt(dateTime);
        //        builtDelta.exec(s =>
        //        {
        //            success(s);
        //        }, e =>
        //        {
        //            builtError(e);
        //        });
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        public static void fetchVenue<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(venue);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaVenue<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(venue);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void fetchSettingsMenu<T>(Action<QueryResult<T>> success, Action<BuiltError> error) where T : new()
        {
            try
            {
                BuiltSDK.BuiltQuery<T> builtObject = new BuiltQuery<T>(settings_menu);
                builtObject.exec(t =>
                {
                    success(t);
                }, t =>
                {
                    error(t);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void getDeltaSettingsMenu<T>(DateTime dateTime, Action<DeltaResult<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltSDK.BuiltDelta<T> builtDelta = new BuiltSDK.BuiltDelta<T>(settings_menu);
                builtDelta.allDeltaAt(dateTime);
                builtDelta.exec(s =>
                {
                    success(s);
                }, e =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex));
            }
        }

        public static void getLoginExtension<T>(string functionName, Dictionary<string, object> properties, Action<BuiltExtension<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltExtension<T> builtExtension = new BuiltExtension<T>();
                builtExtension.execute(functionName, properties, (s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void getLastActivities<T>(Action<BuiltApplication<T[]>> success, Action<BuiltError> builtError)
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltApplication<T[]> builtApp = new BuiltApplication<T[]>();
                builtApp.inlcudeLastActivities();
                builtApp.fetchApplicationClassSchema((s) =>
                    {
                        success(s);
                    },
                    (error) =>
                    {
                        builtError(error);
                    });
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }
        }

        public static void executeFunction<T>(string functionName, Dictionary<string, object> properties, Action<BuiltExtension<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltExtension<T> builtExtension = new BuiltExtension<T>();
                builtExtension.execute(functionName, properties, (s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void getMyInterestExtension<T>(string functionName, Dictionary<string, object> properties, Action<BuiltExtension<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltExtension<T> builtExtension = new BuiltExtension<T>();
                builtExtension.execute(functionName, properties, (s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void ToggleInterestExtension<T>(string functionName, Dictionary<string, object> properties, Action<BuiltExtension<T>> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltExtension<T> builtExtension = new BuiltExtension<T>();
                builtExtension.execute(functionName, properties, (s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void Save<T>(string classUid, T data, string objUid, ExtensionApplicationUser user, Action<T> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltObject<T> builtObject = new BuiltObject<T>(classUid);
                builtObject.data = data;

                if (user != null)
                    builtObject.setHeader("authtoken", user.authtoken);

                if (!String.IsNullOrWhiteSpace(objUid))
                    builtObject.ObjectUID = objUid;

                builtObject.save((s) =>
                {
                    success(s.result);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void SaveWithDictionary<T>(string classUid, T data, string objUid, ExtensionApplicationUser user, Action<T> success, Action<BuiltError> builtError)
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltObject<T> builtObject = new BuiltObject<T>(classUid);
                builtObject.data = data;

                if (user != null)
                    builtObject.setHeader("authtoken", user.authtoken);

                if (!String.IsNullOrWhiteSpace(objUid))
                    builtObject.ObjectUID = objUid;

                var dataDict = data.AsDictionary();
                if (data is BuiltNotes)
                {
                    try
                    {
                        dataDict["photos"] = (data as BuiltNotes).photos.Select(p => p.uid).ToList();
                    }
                    catch { }
                }

                builtObject.saveWithDictionary(dataDict, (s) =>
                {
                    success(s.result);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void Delete<T>(string classUid, string objectUid, ExtensionApplicationUser user, Action<string> success, Action<BuiltError> builtError) where T : new()
        {
            try
            {
                BuiltSDK.Built.initializeWithApiKey("blt1cda804432b357de", "vmworld_pex_uat");
                BuiltSDK.BuiltAppConstants.BuiltBaseURL = "vm-api.built.io";
                BuiltObject<T> builtObject = new BuiltObject<T>(classUid);
                builtObject.ObjectUID = objectUid;

                if (user != null)
                    builtObject.setHeader("authtoken", user.authtoken);

                builtObject.destroy((s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void UploadFile(string path, byte[] data, ExtensionApplicationUser user, Action<BuiltFile> success, Action<BuiltError> builtError)
        {
            try
            {
                BuiltFile builtFile = new BuiltFile();
                builtFile.setHeader("authtoken", user.authtoken);
                builtFile.setFile(path);

                BuiltACL aclForFile = new BuiltACL();
                aclForFile.setPublicReadAccess(false);
                aclForFile.setPublicWriteAccess(false);
                aclForFile.setPublicDeleteAccess(false);
                aclForFile.setUserReadAccess(user.uid, true);
                aclForFile.setUserWriteAccess(user.uid, true);
                aclForFile.setUserDeleteAccess(user.uid, true);
                builtFile.setACL(aclForFile);

                builtFile.save(data, (s) =>
                {
                    success(s);
                }, (e) =>
                {
                    builtError(e);
                });
            }
            catch (Exception ex)
            {
                builtError(new BuiltError(ex.Message));
            }
        }

        public static void RefreshUserInfo(string uid, Action<ExtensionApplicationUser> success, Action<BuiltError> error)
        {
            try
            {
                BuiltUser<ExtensionApplicationUser> user = new BuiltUser<ExtensionApplicationUser>();
                user.refreshUserInfo(uid, s =>
                    {
                        success(s.result);
                    }, e =>
                    {
                        error(e);
                    });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void createInstallation(string deviceToken, Action<BuiltError> error, bool disableNotifications = false)
        {
            try
            {
                BuiltInstallation<BuiltInstallationData> installation = new BuiltInstallation<BuiltInstallationData>();
                //BuiltSDK.BuiltAppConstants.InstallationFileName = Path.Combine(Path.GetDirectoryName(dbPath), "installation");
                if (DataManager.currentUser != null)
                {
                    installation.setHeader("authtoken", DataManager.currentUser.authtoken);
                }

                Dictionary<string, object> extras = new Dictionary<string, object> { { "badge", 0 }, { "disable", disableNotifications } };
                if (!String.IsNullOrWhiteSpace(GlobalValues.CredentialsName))
                {
                    extras.Add("credentials_name", GlobalValues.CredentialsName);
                }

                installation.createInstallation(deviceToken, new List<string> { "event_notifications.object.create" }, extras, r =>
                {
                    error(null);
                }, e =>
                {
                    error(e);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void TriggerEvent(string token, Dictionary<string, object> properties, Action<BuiltError> error)
        {
            try
            {
                BuiltEvent builtEvent = new BuiltEvent("test");
                builtEvent.setProperties(properties);

                BuiltAnalytics<object> analytics = new BuiltAnalytics<object>();
                analytics.trigger(builtEvent, r =>
                {
                    error(null);
                }, e =>
                {
                    error(e);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }

        public static void TriggerMultipleEvents(List<EventUid> data, Dictionary<string, object> superProperties, Action<BuiltError> error)
        {
            try
            {
                BuiltAnalytics<object> analytics = new BuiltAnalytics<object>();
                analytics.superProperties(superProperties);
                analytics.triggerMultiple(data, r =>
                {
                    error(null);
                }, e =>
                {
                    error(e);
                });
            }
            catch (Exception ex)
            {
                error(new BuiltError(ex));
            }
        }
    }
}
