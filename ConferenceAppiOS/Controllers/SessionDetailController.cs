using System;
using CoreGraphics;
using CoreFoundation;
using UIKit;
using Foundation;
using CoreData;
using CommonLayer.Entities.Built;
using SQLiteNetExtensions.Extensions;
using System.Linq;
using CoreAnimation;
using System.Collections.Generic;
using ConferenceAppiOS.Helpers;
using CommonLayer;
using Newtonsoft.Json;
using BigTed;
using BuiltSDK;
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS
{
    public class SessionDetailController : BaseViewController
    {
		static nfloat leftMargin = 30;
		static nfloat headingLeftMargin = 40;
		static nfloat headingRightMargin = 180;
		static nfloat sessionNameLeftMargin = 30;
		static nfloat sessionNameTopMargin = 20;
		static nfloat sessionNameRightMargin = 60;
		static nfloat trackNameLeftMargin = 30;
		static nfloat trackNameTopMargin = 10;
		static nfloat trackNameRightMargin = 60;
		static nfloat trackNameHeight = 30;
		static nfloat topBarHeight = 60;
        string NAVIGATION_TEXT = AppTheme.SDsessionsTitleText;
		static nfloat locationLeftMargin = 30;
		static nfloat locationHeight = 20;
		static nfloat timingsLeftMargin = 30;
		static nfloat timingsHeight = 20;
		static nfloat btnAvailabilityTopMargin = 30;
		static nfloat btnAvailabilityHeight = 35;
		static nfloat btnAddScheduleTopMargin = 20;
		static nfloat lblDescriptionTopMargin = 20;
		static nfloat lblDescriptionBottomMargin = 40;
        BuiltSessionTime currentSession; BuiltTracks builtTrack;
        UITableView tableView;
        private string sessionId = string.Empty;
        public List<BuiltTracks> tracksList { get; set; }
        UIButton btnAddToSchedule;
        private UIButton _imgStar;
        public UIButton imgStar
        {
            get
            {
                if (_imgStar == null)
                {
                    _imgStar = UIButton.FromType(UIButtonType.Custom);
                    _imgStar.BackgroundColor = UIColor.White;
                    _imgStar.SetTitle(AppTheme.SDNormalStar, UIControlState.Normal);
                    _imgStar.SetTitle(AppTheme.SDSelectedStar, UIControlState.Selected);
                    _imgStar.SetTitleColor(AppTheme.Layer3BaseColor, UIControlState.Normal);
                    _imgStar.SetTitleColor(AppTheme.SDNormalStarSelectedColor, UIControlState.Highlighted);
                    _imgStar.Font = AppTheme.SDNormalStarNameFont;
                    _imgStar.Frame = new CGRect(0, 0, 30, 30);
                    View.Add(_imgStar);
                }
                return _imgStar;
            }
        }

        private UIButton _addSession;
        public UIButton addSession
        {
            get
            {
                if (_addSession == null)
                {
                    _addSession = UIButton.FromType(UIButtonType.Custom);
                    _addSession.BackgroundColor = UIColor.White;
                    _addSession.SetTitle(AppTheme.SDNormalAddSession, UIControlState.Normal);
                    _addSession.SetTitle(AppTheme.SDSelectedAddSession, UIControlState.Selected);
                    _addSession.SetTitleColor(AppTheme.SDNormalAddSessionPlusColor, UIControlState.Normal);
                    _addSession.SetTitleColor(AppTheme.SDSelectedAddSessionPlusColor, UIControlState.Highlighted);
                    _addSession.SetTitleColor(AppTheme.SDSelectedAddSessionPlusColor, UIControlState.Selected);
                    _addSession.Font = AppTheme.SDNormalStarNameFont;
                    _addSession.Frame = new CGRect(0, 0, 30, 30);
                    View.Add(_addSession);
                }
                return _addSession;
            }
        }
            
        public SessionDetailController(BuiltSessionTime session)
        {
            this.currentSession = session;
        }

        public SessionDetailController(string sessionId)
        {
            this.sessionId = sessionId;
            var session = DataManager.GetSessionTimeFromId(AppDelegate.Connection, sessionId).Result;
            currentSession = session;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        private void builtTrackAll(Action<List<BuiltTracks>> callback)
        {
            DataManager.GetListOfAllTrack(AppDelegate.Connection).ContinueWith(t =>
            {
                var result = t.Result;
                if (callback != null)
                    try
                    {
                        callback(result);
                    }
                    catch (Exception e)
                    {
                    }
            });
        }
        public override void ViewDidLoad()
        {
            View.BackgroundColor = AppTheme.SDsessionDetailsBackColor;

            base.ViewDidLoad();

            NavigationItem.TitleView = new UILabel(new CGRect(0, 0, 230, 30))
            {
                Text = NAVIGATION_TEXT,
                TextAlignment = UITextAlignment.Center,
                Font = AppTheme.SDsessionDetailsNavigationTitleFont,
                TextColor = AppTheme.SDsessionDetailsNavigationTitle
            };


            if (Convert.ToDateTime(currentSession.date) < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                NavigationItem.RightBarButtonItems = new[] 
            {
				new UIBarButtonItem(imgStar),
            };

            }
            else if (Convert.ToDateTime(currentSession.date) == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                if (Convert.ToDateTime(currentSession.date) == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(currentSession.time) < Helper.timeConverterForCurrentHourMinute())
                {
                    NavigationItem.RightBarButtonItems = new[] 
                    {
				        new UIBarButtonItem(imgStar),
                    };
                }
                else
                {
                    NavigationItem.RightBarButtonItems = new[] 
                    {
				        new UIBarButtonItem(addSession),new UIBarButtonItem(imgStar),
                    };
                }
            }
            else if (Convert.ToDateTime(currentSession.date) > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                NavigationItem.RightBarButtonItems = new[] 
            {
                new UIBarButtonItem(addSession),new UIBarButtonItem(imgStar),
            };
            }

            addSession.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (AppSettings.ApplicationUser != null)
                {
                    addSession.Selected = !addSession.Selected;
                    btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                    addSession.Enabled = false;
                    btnAddToSchedule.Enabled = false;

                    var setting = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    };
                    NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(currentSession, setting)), new NSString("session_time"));
                    if (btnAddToSchedule.Selected)
                    {
                        DataManager.AddSessionToSchedule(AppDelegate.Connection, currentSession, (session) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                addSession.Enabled = true;
                                btnAddToSchedule.Enabled = true;
                                ToastView.ShowToast(Helper.SessionAddedString, 2);
                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                            });
                        }, error =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                                addSession.Selected = !addSession.Selected;
                                btnAddToSchedule.Enabled = true;
                                addSession.Enabled = true;
                                new UIAlertView("", Helper.GetErrorMessage(error), null, AppTheme.DismissText).Show();
                            });
                        });
                    }
                    else
                    {
                        if (currentSession.BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                        currentSession.BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ToastView.ShowToast(Helper.CanntotUnschedule, 2);
                            addSession.Selected = !addSession.Selected;
                            btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                            btnAddToSchedule.Enabled = true;
                            addSession.Enabled = true;
                        }
                        else
                        {
                            var alert = new UIAlertView("", AppSettings.RemoveSessionAlertMessage, null, AppTheme.CancelTextTitle, AppTheme.ConfirmText);
                            alert.Clicked += (s, args) =>
                            {
                                if (args.ButtonIndex == 1)
                                {
                                    DataManager.RemoveSessionFromSchedule(AppDelegate.Connection, currentSession, (err, session) =>
                                    {
                                        if (err == null)
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                addSession.Enabled = true;
                                                btnAddToSchedule.Enabled = true;
                                                ToastView.ShowToast(Helper.SessionRemovedString, 2);
                                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionRemovedFromSchedule, null, dictionary);
                                            });
                                        }
                                    });
                                }
                                else
                                {
                                    addSession.Selected = !addSession.Selected;
                                    btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                                    btnAddToSchedule.Enabled = true;
                                    addSession.Enabled = true;
                                }
                            };
                            alert.Show();
                        }
                    }
                }
                else
                {
                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (s, arg) =>
                    {
                        if (arg.ButtonIndex.ToString() == "1")
                        {
                            AppDelegate.instance().ShowLogin();
                            return;
                        }
                        else
                        { }

                    };
                    alertView.Show();
                }

            };
            #region ---Star Click code
            imgStar.TouchUpInside += (s, e) =>
            {
                if (AppSettings.ApplicationUser != null)
                {
                    imgStar.Selected = !imgStar.Selected;
                    imgStar.Enabled = false;

                    DataManager.toggleExtension(this.currentSession.BuiltSession.session_id, AppDelegate.Connection, (call) =>
                    {
                        if (call.ToLower() == "added")
                        {
                            NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(call.ToLower()), new NSString("Star_Selected"));
                            InvokeOnMainThread(() =>
                            {
                                ToastView.ShowToast(Helper.InterestAddedString, 2);
                                imgStar.Enabled = true;
                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.StarAddedToInterest, null, dictionary);

                            });
                        }
                        else
                        {
                            NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString("unstar"), new NSString("Star_UnSelected"));
                            InvokeOnMainThread(() =>
                            {
                                ToastView.ShowToast(Helper.InterestRemovedString, 2);
                                imgStar.Enabled = true;
                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.StarRemovedFromInterest, null, dictionary);
                            });
                        }
                    }, (error) =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            imgStar.Selected = !imgStar.Selected;
                            new UIAlertView("", Helper.GetErrorMessage(error), null, AppTheme.DismissText).Show();
                        });
                    });
                }


                else
                {
                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (sender, arg) =>
                    {
                        if (arg.ButtonIndex.ToString() == "1")
                        {
                            AppDelegate.instance().ShowLogin();
                            return;
                        }
                        else
                        { }

                    };
                    alertView.Show();
                }
            };
            #endregion

            bool showStar = false;
            var result = DataManager.GetMyInterest(AppDelegate.Connection).Result;
            if (result != null && result.Count > 0)
            {
                showStar = result.Any(p => p.session_time_id == this.currentSession.BuiltSession.session_id);
            }

            if (showStar)
            {
                imgStar.Selected = true;
            }
            else
            {
                imgStar.Selected = false;
            }


            tableView = new UITableView();
            tableView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            var dictSection = GetSectionDetails();

            tableView.Source = new SessionDetailSource(this, currentSession.BuiltSession.speaker, dictSection);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.session_details, Helper.ToDateString(DateTime.Now));
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
        }

        private Dictionary<string, string> GetSectionDetails()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.session_id))
                result.Add("ID", currentSession.BuiltSession.abbreviation);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.session_id))
                result.Add("Twitter Hashtag", "#" + currentSession.BuiltSession.abbreviation);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.track))
            {
                if (currentSession.BuiltSession.track.ToLower() != "no track")
                {
                    result.Add("Track", currentSession.BuiltSession.track);
                }
            }
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.sub_track_separated))
                result.Add("Subtrack", currentSession.BuiltSession.sub_track_separated);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.market_segment_separated))
                result.Add("Market Segment", currentSession.BuiltSession.market_segment_separated);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.competency_separated))
                result.Add("Competency", currentSession.BuiltSession.competency_separated);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.technical_level))
                result.Add("Technical Level", currentSession.BuiltSession.technical_level);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.product_and_topic))
                result.Add("Product and Topic", currentSession.BuiltSession.product_and_topic);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.audience_separated))
                result.Add("Audience", currentSession.BuiltSession.audience_separated);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.type))
                result.Add("Session Type", currentSession.BuiltSession.type);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.role_separated))
                result.Add("Role", currentSession.BuiltSession.role_separated);
            if (!String.IsNullOrEmpty(currentSession.BuiltSession.skill_level_separated))
                result.Add("Skill Level", currentSession.BuiltSession.skill_level_separated);
            return result;
        }

        private void SetHeader()
        {
            nfloat height = 0;
            nfloat yPosition = 0;

            UIView header = new UIView();
            header.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            header.AutosizesSubviews = true;

            var headerDetail = new UIView();
            headerDetail.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            headerDetail.AutosizesSubviews = true;

            var sessionName = new UILabel(new CGRect(sessionNameLeftMargin, sessionNameTopMargin, View.Frame.Width - sessionNameRightMargin, 0))
            {
                TextColor = AppTheme.SDsessionDetailsSessionTitleTextColor,
                Font = AppTheme.SDsessionNameFont,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            sessionName.AttributedText = AppFonts.IncreaseLineHeight(currentSession.BuiltSession.title, sessionName.Font, sessionName.TextColor);
            sessionName.SizeToFit();
            sessionName.Frame = new CGRect(sessionNameLeftMargin, sessionNameTopMargin, View.Frame.Width - sessionNameRightMargin, sessionName.Frame.Height);
            yPosition = sessionName.Frame.Bottom;

            height += sessionNameTopMargin + sessionName.Frame.Height;

            if (currentSession.BuiltSession.track != null && currentSession.BuiltSession.track.Length > 0)
            {
                var trackName = new UILabel(new CGRect(trackNameLeftMargin, sessionName.Frame.Bottom + trackNameTopMargin, View.Frame.Width - trackNameRightMargin, 20))
                {
                    Text = string.Empty,
                    Font = AppFonts.ProximaNovaRegular(14),
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };
                if (currentSession.BuiltSession.track.ToLower() != "no track")
                {
                    trackName.Text = currentSession.BuiltSession.track;
                }
                if (builtTrack != null)
                {
                    trackName.TextColor = UIColor.Clear.FromHexString(builtTrack.color, 1.0f);
                }
                trackName.SizeToFit();
                trackName.Frame = new CGRect(trackNameLeftMargin, sessionName.Frame.Bottom + trackNameTopMargin, View.Frame.Width - trackNameRightMargin, trackName.Frame.Size.Height);
                headerDetail.AddSubview(trackName);

                if (currentSession.BuiltSession.track.ToLower() != "no track")
                {
                    height += trackNameTopMargin + trackName.Frame.Height;
                    yPosition = trackName.Frame.Bottom;
                }
            }

            if (currentSession.room != null && currentSession.room.Length > 0)
            {
                var location = new UILabel(new CGRect(locationLeftMargin, yPosition + trackNameTopMargin, View.Frame.Width - (locationLeftMargin), locationHeight))
                {
                    TextColor = AppTheme.SDlocationColor,
                    Font = AppTheme.SDlocationFont,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };
                location.Text = currentSession.room;
                headerDetail.AddSubview(location);
                location.SizeToFit();
                location.Frame = new CGRect(locationLeftMargin, yPosition + trackNameTopMargin, View.Frame.Width - (locationLeftMargin), location.Frame.Size.Height);
                height += locationHeight;
                yPosition = location.Frame.Bottom;
            }

            if (currentSession.date != null && currentSession.date.Length > 0)
            {
                var timings = new UILabel(new CGRect(timingsLeftMargin, yPosition + trackNameTopMargin / 2, View.Frame.Width - (timingsLeftMargin), timingsHeight))
                {
                    TextColor = AppTheme.SDTimingColor,
                    Font = AppTheme.SDtimingFont,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };
                timings.Text = Helper.convertToTodayTomorrowDate(currentSession.date) + ", " + Helper.convertToStartEndDate(currentSession.time, currentSession.length);
                timings.SizeToFit();
                timings.Frame = new CGRect(timingsLeftMargin, yPosition + trackNameTopMargin / 2, View.Frame.Width - (timingsLeftMargin), timings.Frame.Size.Height);
                height += timingsHeight;
                yPosition = timings.Frame.Bottom;
                headerDetail.AddSubview(timings);
            }


            btnAddToSchedule = UIButton.FromType(UIButtonType.Custom);

            btnAddToSchedule.Frame = new CGRect(leftMargin, yPosition + btnAvailabilityTopMargin, View.Frame.Width - (leftMargin * 2), btnAvailabilityHeight);
            btnAddToSchedule.SetTitleColor(AppTheme.LMButtontextColor, UIControlState.Normal);
            btnAddToSchedule.SetTitleColor(AppTheme.LMButtontextColor, UIControlState.Highlighted);
            btnAddToSchedule.SetTitleColor(AppTheme.LMButtontextColor, UIControlState.Selected);
            btnAddToSchedule.SetTitle(AppTheme.SDaddToMyScheduleTextTitle, UIControlState.Normal);
            btnAddToSchedule.SetTitle(AppTheme.SDremoveFromMyScheduleTextTitle, UIControlState.Selected);
            btnAddToSchedule.BackgroundColor = AppTheme.LILoginbuttonBackgroundColorInLeftmenu;
            btnAddToSchedule.Layer.CornerRadius = 5.0f;

            btnAddToSchedule.Frame = new CGRect(leftMargin, yPosition + btnAvailabilityTopMargin, View.Frame.Width - (leftMargin * 2), btnAvailabilityHeight);

            btnAddToSchedule.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            btnAddToSchedule.Font = AppTheme.SDsessionBtnFont;

            if (Convert.ToDateTime(currentSession.date) < TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                if (btnAddToSchedule.Hidden == false)
                { btnAddToSchedule.Hidden = true; }

            }
            else if (Convert.ToDateTime(currentSession.date) == TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date && Helper.timeConverterForBuiltTimeString(currentSession.time) < Helper.timeConverterForCurrentHourMinute())
            {
                if (btnAddToSchedule.Hidden == false)
                { btnAddToSchedule.Hidden = true; }
            }
            else if (Convert.ToDateTime(currentSession.date) > TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone).Date)
            {
                btnAddToSchedule.Hidden = false;

            }

            if (btnAddToSchedule.Enabled)
            {
            }

            if (AppSettings.MySessionIds != null)
            {
                if (AppSettings.MySessionIds.Contains(currentSession.session_time_id))
                {
                    btnAddToSchedule.Selected = true;
                    addSession.Selected = true;
                }
            }

            btnAddToSchedule.TouchUpInside += (s, e) =>
            {
                if (AppSettings.ApplicationUser != null)
                {
                    btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                    btnAddToSchedule.Enabled = false;
                    addSession.Selected = !addSession.Selected;
                    addSession.Enabled = false;

                    var setting = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    };
                    NSDictionary dictionary = NSDictionary.FromObjectAndKey(new NSString(JsonConvert.SerializeObject(currentSession, setting)), new NSString("session_time"));
                    if (btnAddToSchedule.Selected)
                    {
                        DataManager.AddSessionToSchedule(AppDelegate.Connection, currentSession, (session) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                btnAddToSchedule.Enabled = true;
                                addSession.Enabled = true;
                                ToastView.ShowToast(Helper.SessionAddedString, 2);
                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionAddedToSchedule, null, dictionary);
                            });
                        }, error =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                                addSession.Selected = !addSession.Selected;
                                btnAddToSchedule.Enabled = true;
                                addSession.Enabled = true;
                                new UIAlertView("", Helper.GetErrorMessage(error), null, AppTheme.DismissText).Show();
                            });
                        });
                    }
                    else
                    {
                        if (currentSession.BuiltSession.type.Equals(AppSettings.GeneralSession, StringComparison.InvariantCultureIgnoreCase) ||
                        currentSession.BuiltSession.type.Equals(AppSettings.ConferenceFunction, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ToastView.ShowToast(Helper.CanntotUnschedule, 2);
                            btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                            btnAddToSchedule.Enabled = true;
                            addSession.Selected = !addSession.Selected;
                            addSession.Enabled = true;
                        }
                        else
                        {
                            var alert = new UIAlertView("", AppSettings.RemoveSessionAlertMessage, null, AppTheme.CancelTextTitle, AppTheme.ConfirmText);
                            alert.Clicked += (sender, args) =>
                            {
                                if (args.ButtonIndex == 1)
                                {
                                    DataManager.RemoveSessionFromSchedule(AppDelegate.Connection, currentSession, (err, session) =>
                                    {
                                        if (err == null)
                                        {
                                            InvokeOnMainThread(() =>
                                            {
                                                btnAddToSchedule.Enabled = true;
                                                addSession.Enabled = true;
                                                ToastView.ShowToast(Helper.SessionRemovedString, 2);
                                                NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.SessionRemovedFromSchedule, null, dictionary);
                                            });
                                        }
                                    });
                                }
                                else
                                {
                                    btnAddToSchedule.Selected = !btnAddToSchedule.Selected;
                                    btnAddToSchedule.Enabled = true;
                                    addSession.Selected = !addSession.Selected;
                                    addSession.Enabled = true;
                                }
                            };
                            alert.Show();
                        }
                    }
                }
                else
                {
                    UIAlertView alertView = new UIAlertView(AppTheme.LoginRequiredText, AppTheme.LoginMessageText, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                    alertView.Clicked += (sender, arg) =>
                    {
                        if (arg.ButtonIndex.ToString() == "1")
                        {
                            AppDelegate.instance().ShowLogin();
                            return;
                        }
                        else
                        { }

                    };
                    alertView.Show();
                }
            };

            if (!btnAddToSchedule.Hidden)
            {
                yPosition = btnAddToSchedule.Frame.Bottom;
                height += (btnAvailabilityHeight * 2) + btnAddScheduleTopMargin;
            }
				
				var description = new UILabel(new CGRect(leftMargin, yPosition + lblDescriptionTopMargin, View.Frame.Width - (leftMargin * 2), 0))
            {
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextColor = AppTheme.SDdescriptionTextColor,
                Font = AppTheme.SDdescriptionFont,
            };
            description.AttributedText = AppFonts.IncreaseLineHeight(currentSession.BuiltSession._abstract, description.Font, description.TextColor);
            var h = Helper.getTextHeight(description.Text, description.Frame.Width, UILineBreakMode.WordWrap, UITextAlignment.Left, description.Font, description);
            var rect = description.Frame;

            rect.Height = h;
            description.Frame = rect;
            height += description.Frame.Height + lblDescriptionBottomMargin;
            header.Frame = new CGRect(0, 0, View.Frame.Width, height);
            tableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height);
            headerDetail.Frame = new CGRect(0, 0, View.Frame.Width, height - 10);
            headerDetail.AddSubviews(sessionName, btnAddToSchedule, description);
            header.AddSubviews(headerDetail);
            View.AddSubviews(tableView);
            tableView.TableHeaderView = header;
            SetFooter();
        }


        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (tableView.TableHeaderView == null)

                builtTrackAll((q) =>
                {
                    tracksList = q;
                    if (currentSession.BuiltSession.track == "" || currentSession.BuiltSession.track == null)
                    {
                        currentSession.BuiltSession.track = "No Track";
                    }
                    builtTrack = tracksList.FirstOrDefault(p => p.name.ToLower() == currentSession.BuiltSession.track.ToLower());
                    InvokeOnMainThread(() =>
                    {
                        SetHeader();
                    });
                });


        }

        private void SetFooter()
        {
            UIView footer = new UIView(CGRect.Empty);
            tableView.TableFooterView = footer;
        }

        public class SessionDetailSource : UITableViewSource
        {
            Dictionary<string, string> others;
            List<BuiltSessionSpeaker> speakers;
            string[] keys;
            SessionDetailController sessionDetailController;
            NSString cellIdentifierSpeaker = new NSString("SpeakerCell");
            NSString cellIdentifierOther = new NSString("OtherCell");
            public NSIndexPath selectedIndex;

            public SessionDetailSource(SessionDetailController sessionDetail, List<BuiltSessionSpeaker> speakers, Dictionary<string, string> others)
            {
                sessionDetailController = sessionDetail;
                this.speakers = speakers;
                this.others = others;
                keys = others.Keys.ToArray();
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                if (keys.Length == 0)
                {
                    if (speakers == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (speakers == null)
                    {
                        return keys.Length;
                    }
                    else
                    {
                        return keys.Length + 1;
                    }
                }
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {

                if (keys.Length == 0 || section >= keys.Length)
                    return speakers.Count;
                else
                {
                    if (others[keys[section]].Contains(","))
                    {
                        int count = others[keys[section]].Split(',').Length;
                        return count;
                    }
                    else
                    {
                        int count = others[keys[section]].Split('|').Length;
                        return count;
                    }
                }
            }
            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return AppTheme.SectionHeight;
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                if (keys.Length == 0 || indexPath.Section >= keys.Length)
                    return 60;
                else
                    return 40;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                string sectionTitle = null;
                if (keys.Length == 0 || section >= keys.Length)
                {
                    if (speakers.Count == 1)
                    {
                        sectionTitle = speakers.Count + AppTheme.SDspeakerTextTitleText;
                    }

                    else if (speakers.Count > 1)
                    {
                        sectionTitle = speakers.Count + AppTheme.SDspeakersTextTitleText;
                    }
                    else
                    {
                        return new UIView(new CGRect(0, 0, 0, 0));
                    }
                }

                else
                {
                    sectionTitle = keys[section];
                }


					UIView view = new UIView(new CGRect(0, 0, tableView.Frame.Width, AppTheme.SectionHeight))
				{
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                    AutosizesSubviews = true,
                    BackgroundColor = AppTheme.SDSectionBGColor
                };


					UILabel lblSectionHeader = new UILabel(new CGRect(15, 0, tableView.Frame.Width - 30, view.Frame.Height))
                {
                    BackgroundColor = UIColor.Clear,
                    TextColor = AppTheme.SDSectionTextColor,
                    Text = sectionTitle,
                    Font = AppTheme.SDSectionTextFont,
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth
                };

				var topLine = new UIView(new CGRect(0, 0, view.Frame.Width, 1)) { BackgroundColor = AppTheme.SDSeparatorBGColor };
				var bottomLine = new UIView(new CGRect(0, view.Frame.Bottom - 1, view.Frame.Width, 1)) { BackgroundColor = AppTheme.SDSeparatorBGColor };

                view.AddSubviews(lblSectionHeader, topLine, bottomLine);
                return view;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                if (keys.Length == 0 || indexPath.Section >= keys.Length)
                {
                    SessionSpeakerCell cell = tableView.DequeueReusableCell(cellIdentifierSpeaker) as SessionSpeakerCell;
                    if (cell == null) cell = new SessionSpeakerCell(cellIdentifierSpeaker);
                    var item = speakers[indexPath.Row];
                    cell.UpdateCell(item);
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    return cell;
                }
                else
                {
                    SingleRowCell cell = tableView.DequeueReusableCell(cellIdentifierOther) as SingleRowCell;
                    if (cell == null) cell = new SingleRowCell(cellIdentifierOther);
                    if (others[keys[indexPath.Section]].Contains(","))
                    {
                        var items = others[keys[indexPath.Section]].Split(',');
                        cell.textLabel.Text = items[indexPath.Row];
                    }
                    else
                    {
                        var items = others[keys[indexPath.Section]].Split('|');
                        cell.textLabel.Text = items[indexPath.Row];
                    }
                    return cell;
                }
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                selectedIndex = indexPath;

                if (keys.Length == 0 || indexPath.Section >= keys.Length)
                {
                    var uuid = speakers[indexPath.Row].user_ref;
                    var speaker = AppDelegate.Connection.GetAllWithChildren<BuiltSpeaker>(p => p.user_ref == uuid, true).FirstOrDefault();
                    if (speaker != null)
                    {
                        SpeakerDetailController vc = new SpeakerDetailController(speaker);
                        AppDelegate.instance().rootViewController.openDetail(vc, sessionDetailController, false);
                    }
                }
            }
        }

    }
}