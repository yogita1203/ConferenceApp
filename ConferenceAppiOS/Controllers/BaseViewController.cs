using System;
using CoreGraphics;
using System.Linq;

using CoreFoundation;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace ConferenceAppiOS
{
    [Register("BaseViewController")]
    public class BaseViewController : UIViewController
    {
        public const string SessionAddedToSchedule = "SessionAddedToSchedule";
        public const string SessionRemovedFromSchedule = "SessionRemovedFromSchedule";
        public const string StarAddedToInterest = "StarAddedToInterest";
        public const string StarRemovedFromInterest = "StarRemovedFromInterest";
        public const string DELTA_STARTED = "DELTA_STARTED";
        public const string DELTA_COMPLETED = "DELTA_COMPLETED";
        public const string LATEST_DATA_AVAILABLE = "LATEST_DATA_AVAILABLE";
        public const string DATA_UP_TO_DATE = "DATA_UP_TO_DATE";
        public const string CHECKING_UPDATE = "CHECKING_UPDATE";

        public const string USER_LOGGED_IN = "USER_LOGGED_IN";
        public const string USER_LOGGED_OUT = "USER_LOGGED_OUT";
        public const string AFTER_LOGIN_DATA_FETCHED = "AFTER_LOGIN_DATA_FETCHED";

        public const string DETAIL_CLOSING = "DETAIL_CLOSING";
        public const string ReloadLeftMenu = "ReloadLeftMenu";
        public const string UpdateSessions = "UpdateSessions";
        public const string StartDelta = "StartDelta";

        NSObject observer;
        NSObject deltaStartedObserver;
        NSObject deltaCompletedObserver;

        NSObject loggedInObserver;
        NSObject loggedOutObserver;

        NSObject afterLoginObserver;

        NSObject detailClosingObserver;
        NSObject reloadLeftMenuObserver;
        NSObject updateSessionsObserver;

        public string observerName;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!String.IsNullOrEmpty(observerName))
            {
                if(observer == null)
					observer = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(observerName), (notification) =>
                {
                    OnObserverNotification(notification);
                });
            }

            if (deltaStartedObserver == null)
				deltaStartedObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DELTA_STARTED), (notification) =>
            {
                OnDeltaStarted(notification);
            });

            if (deltaCompletedObserver == null)
				deltaCompletedObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DELTA_COMPLETED), (notification) =>
            {
                List<string> updatedUids = null;
                if (notification.Object != null)
                {
                    var str = notification.Object.ToString();
                    if (!String.IsNullOrWhiteSpace(str))
                        updatedUids = str.Split('|').ToList();
                }
                OnDeltaCompleted(notification, updatedUids);
            });

            if (loggedInObserver == null)
				loggedInObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(USER_LOGGED_IN), (notification) =>
            {
                OnUserLoggedIn(notification);
            });

            if (loggedOutObserver == null)
				loggedOutObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(USER_LOGGED_OUT), (notification) =>
            {
                OnUserLoggedOut(notification);
			});

            if (afterLoginObserver == null)
				afterLoginObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(AFTER_LOGIN_DATA_FETCHED), (notification) =>
            {
                OnAfterLoginDataFetched(notification);
            });

            if (detailClosingObserver == null)
				detailClosingObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DETAIL_CLOSING), (notification) =>
                {
                    OnDetailClosing(notification);
                });

            if (reloadLeftMenuObserver == null)
				reloadLeftMenuObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(ReloadLeftMenu), (notification) =>
                {
                    OnReloadLeftMenu(notification);
                });

            if (updateSessionsObserver == null)
				updateSessionsObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(UpdateSessions), (notification) =>
                {
                    OnUpdateSessions(notification);
                });
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (observer != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
            NSNotificationCenter.DefaultCenter.RemoveObserver(deltaStartedObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(deltaCompletedObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(loggedInObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(loggedOutObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(afterLoginObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(afterLoginObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(detailClosingObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(reloadLeftMenuObserver);
            NSNotificationCenter.DefaultCenter.RemoveObserver(updateSessionsObserver);
        }

        public virtual void OnObserverNotification(NSNotification notification)
        {
        }

        public virtual void OnDeltaStarted(NSNotification notification)
        {
        }

        public virtual void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
        }

        public virtual void OnUserLoggedIn(NSNotification notification)
        {
        }

        public virtual void OnUserLoggedOut(NSNotification notification)
        {
        }

        public virtual void OnAfterLoginDataFetched(NSNotification notification)
        {
        }

        public virtual void OnDetailClosing(NSNotification notification)
        {
        }

        public virtual void OnReloadLeftMenu(NSNotification notification)
        {
        }

        public virtual void OnUpdateSessions(NSNotification notification)
        {
        }
    }

    [Register("BaseTableViewController")]
    public class BaseTableViewController : UITableViewController
    {
        public const string SessionAddedToSchedule = "SessionAddedToSchedule";
        public const string SessionRemovedFromSchedule = "SessionRemovedFromSchedule";
        public const string StarAddedToInterest = "StarAddedToInterest";
        public const string StarRemovedFromInterest = "StarRemovedFromInterest";
        public const string DELTA_STARTED = "DELTA_STARTED";
        public const string DELTA_COMPLETED = "DELTA_COMPLETED";
        public const string LATEST_DATA_AVAILABLE = "LATEST_DATA_AVAILABLE";

        public const string USER_LOGGED_IN = "USER_LOGGED_IN";
        public const string USER_LOGGED_OUT = "USER_LOGGED_OUT";
        public const string AFTER_LOGIN_DATA_FETCHED = "AFTER_LOGIN_DATA_FETCHED";

        public const string DETAIL_CLOSING = "DETAIL_CLOSING";


        NSObject observer;
        NSObject deltaStartedObserver;
        NSObject deltaCompletedObserver;

        NSObject loggedInObserver;
        NSObject loggedOutObserver;

        NSObject afterLoginObserver;

        NSObject detailClosingObserver;

        public string observerName;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (!String.IsNullOrEmpty(observerName))
            {
				observer = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(observerName), (notification) =>
                {
                    OnObserverNotification(notification);
                });
            }

			deltaStartedObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DELTA_STARTED), (notification) =>
            {
                OnDeltaObserverNotification(true, notification);
            });

			deltaCompletedObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DELTA_COMPLETED), (notification) =>
            {
                OnDeltaObserverNotification(false, notification);
            });


			loggedInObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(USER_LOGGED_IN), (notification) =>
            {
                OnUserLoggedIn(notification);
				});

			loggedOutObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(USER_LOGGED_OUT), (notification) =>
            {
                OnUserLoggedOut(notification);
            });

			afterLoginObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(AFTER_LOGIN_DATA_FETCHED), (notification) =>
            {
                OnAfterLoginDataFetched(notification);
            });


			detailClosingObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(DETAIL_CLOSING), (notification) =>
            {
                OnDetailClosing(notification);
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (observer != null)
			NSNotificationCenter.DefaultCenter.RemoveObserver(observer);
			NSNotificationCenter.DefaultCenter.RemoveObserver(deltaStartedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(deltaCompletedObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(loggedInObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(loggedOutObserver);
			NSNotificationCenter.DefaultCenter.RemoveObserver(afterLoginObserver);
        }

        public virtual void OnObserverNotification(NSNotification notification)
        {
        }

        public virtual void OnDeltaObserverNotification(bool isStarted, NSNotification notification)
        {
        }

        public virtual void OnUserLoggedIn(NSNotification notification)
        {
        }

        public virtual void OnUserLoggedOut(NSNotification notification)
        {
        }

        public virtual void OnAfterLoginDataFetched(NSNotification notification)
        {
        }

        public virtual void OnDetailClosing(NSNotification notification)
        {
        }
    }
}