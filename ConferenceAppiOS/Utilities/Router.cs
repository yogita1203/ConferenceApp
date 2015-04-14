using System;
using Foundation;
using CoreGraphics;
using ConferenceAppiOS.Notes;

namespace ConferenceAppiOS
{

    public class Router
    {
        string scheme = "vmwareapp";
		string httpScheme = "http";
		string httpsScheme = "https";

        string leftmenuHost = "leftmenu";
        string afterLoginHost = "leftmenu-login";

        string InAppHost = "app";
        string InAppLoginHost = "app-login";

        string WhatsHappening = "/explore";
        string DailyHighlights = "/news";
        string Social = "/social";
        string Agenda = "/agenda";
        string Sessions = "/sessions";
        string ProgramsAndFAQ = "/others";
        string Surveys = "/survey";
        string SponsorsAndExhibitors = "/sponsors";
        string Location = "/locationmoscone";
        string LocationsInfo = "/locationsfo";
        string Schedule = "/schedule";
        string Activity = "/activity";
        string Notes = "/notes";
        string Game = "game";

        public Router()
        {
        }

        public void openUrl(NSUrl url)
        {
            if (url != null)
            {
				if (url.Scheme == scheme) {
					if (leftmenuHost == url.Host) {
						openControllerWithUrl (url);
					} else if (url.Host == "session") {
						openSessionDetail (url.Path.TrimStart ('/'));
					} else if (url.Host == "sponsor") {
						openSponsorDetail (url.Path.TrimStart ('/'));
					} else if (url.Host == "speaker") {
						openSpeakerDetail (url.Path.TrimStart ('/'));
					} else if (afterLoginHost == url.Host) {
						openControllerWithUrl (url);
					} else if (InAppHost == url.Host) {
						openControllerWithUrl (url);
					} else if (InAppLoginHost == url.Host) {
						openControllerWithUrl (url);
					} else {
						AppDelegate.instance ().ShowLogin ();
					}
				} else if (url.Scheme == httpScheme || url.Scheme == httpsScheme) {
					WebViewController homeScreen = new WebViewController(url.ToString());
					homeScreen.View.Frame = new CGRect(0,20,AppDelegate.instance ().rootViewController.rightSlideView.Frame.Width,AppDelegate.instance ().rootViewController.rightSlideView.Frame.Height-20);
					AppDelegate.instance().rootViewController.openFromMenuForFullscreenWeb(homeScreen);
				}
            }
        }

		void openSessionDetail(string sessionID){
			SessionController vc = new SessionController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
			AppDelegate.instance().rootViewController.openFromMenu(vc);
			vc.searchText = sessionID;
			vc.ShowSessionDetailControllerById (sessionID);
		}

		void openSponsorDetail(string sponsorID){
			SponsorsExhibitorsController sponsorsExhibitorsController = new SponsorsExhibitorsController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
			AppDelegate.instance().rootViewController.openFromMenu(sponsorsExhibitorsController);
            sponsorsExhibitorsController.ShowSponsorDetails(sponsorID);
		}

		void openSpeakerDetail(string speakerID){
			SessionController vc = new SessionController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
			AppDelegate.instance().rootViewController.openFromMenu(vc);
            vc.ShowSpeakerDetailControllerById(speakerID);
		}

        void openControllerWithUrl(NSUrl url)
        {
            if (url.Path == WhatsHappening)
            {
				WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
            }
            else if (url.Path == DailyHighlights)
            {
                AppDelegate.instance().rootViewController.openFromMenu(new DailyHighlightsController(AppDelegate.instance().rootViewController.rightSlideView.Frame));
            }
            else if (url.Path == Social)
            {
                SocialMediaController socialMediaController = new SocialMediaController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(socialMediaController);
            }
            else if (url.Path == Agenda)
            {
                AgendaProgramsHandsOnLabs aph = new AgendaProgramsHandsOnLabs(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(aph);
            }
            else if (url.Path == Sessions)
            {
                SessionController vc = new SessionController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(vc);
            }
            else if (url.Path == ProgramsAndFAQ)
            {
                ProgramFaqController vc = new ProgramFaqController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(vc);
            }
            else if (url.Path == Surveys)
            {
                SurveysController surveysController = new SurveysController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(surveysController);
            }
            else if (url.Path == SponsorsAndExhibitors)
            {
                SponsorsExhibitorsController sponsorsExhibitorsController = new SponsorsExhibitorsController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(sponsorsExhibitorsController);
            }
            else if (url.Path == Location)
            {
                VenueImageController mosconeImageControllerController = new VenueImageController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(mosconeImageControllerController);
            }
            else if (url.Path == LocationsInfo)
            {
                FoodAndDrinksViewController foodAndDrinksViewController = new FoodAndDrinksViewController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(foodAndDrinksViewController);
            }
            else if (url.Path == Schedule)
            {
                ScheduleAndInterestController scheduleAndInterestController = new ScheduleAndInterestController(AppDelegate.instance().rootViewController.rightSlideView.Frame);
                AppDelegate.instance().rootViewController.openFromMenu(scheduleAndInterestController);
            }
            else if (url.Path == Activity)
            {
            }
            else if (url.Path == Notes)
            {
                AppDelegate.instance().rootViewController.openFromMenu(new NotesTableController(AppDelegate.instance().rootViewController.rightSlideView.Frame));
            }
            else if (url.Path == Game)
            {
            }
            else
            {
            }
        }
    }
}