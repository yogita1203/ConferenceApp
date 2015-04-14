using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using FontAwesomeXamarin;
using Shared;

namespace ConferenceAppiOS
{
    public class AppTheme
    {

		public static bool StatusBar = true;
        public static bool ShowNextUpOnHome = true;
        public static bool ShowOnNowOnHome = true;

		public static readonly string Layer1Color = SharedAppTheme.AppBackgroundColor;//"#FFFFFF"; // App Background Color

		public static string Layer1SubviewsColor = SharedAppTheme.SubviewsBackgroundColor; // Subviews background color

		static string Layer2Color = SharedAppTheme.CellSelectedAndViewsBackground; // Cell selected and views background

		public static readonly string Layer3Color = SharedAppTheme.BaseColor; // Base Color

		public static readonly string SecondaryColor = SharedAppTheme.HeadingTitleTextColor;//Heading Title Text Color

		public static string TextColor = SharedAppTheme.TextColor; // Text Color

		public static string SectionBackColor = "#BADEE8";

		public static string IconColor = "#4B4B4B";

		public static string SectionHeadingbackground = SharedAppTheme.SectionHeadingbackground; //section heading background

		public static string NavigationTextAndIcons = SharedAppTheme.SubheadingTextColor; // subheading text color

		public static string LineColor = SharedAppTheme.LineColor;

		public static string Graybackground = SharedAppTheme.ScrollBackgroundColor; // scroll background color

		public static string UpdatingTextColor = SharedAppTheme.UpdatingTextColor;
		//buttons
		static string ButtonBackgroundColor = SecondaryColor;

		public static readonly string LoaderColor = SharedAppTheme.LoaderColor;


		// Heading Font  
		static UIFont HeadingFont = AppFonts.ProximaNovaRegular(14);
		static UIColor HeadingColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		static UIFont ParagraphFont = AppFonts.ProximaNovaRegular(14);
		static UIColor ParagraphColor = UIColor.Clear.FromHexString(TextColor,1.0f);

        // main header view
        static string SelectedIconAndtitleColor = SecondaryColor;

		static nfloat RightBorderForMiddleTableInWhatshapp = 4;

        public static nfloat SectionheaderHeight = 45;
        public static readonly nfloat borderForSectionHeaderTopWidhtVar = borderForSectionHeaderTopWidht;
        public static nfloat announceMentCellHeight = announceMentCellHeightVariable;

        public static readonly UIColor Layer3BaseColor = UIColor.Clear.FromHexString(Layer3Color, 1.0f);
		public static readonly UIColor CellSelectedbackgroundColor = UIColor.Clear.FromHexString(Layer2Color, 1.0f);
        public static readonly UIColor SegmentBackgroundColor = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);
        public static readonly UIColor SegmentSelectedtabColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SegmentSelectedtabBottomColor = UIColor.Clear;
        public static readonly UIColor SegmentInactivetabColor = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);
		public static readonly UIColor SegmentTitletextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor SegmentSubTitletextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor SegmentSeparatorColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIFont SegmentFont = AppFonts.ProximaNovaSemiBold(13);
        public static readonly UIFont MultiLineFont = AppFonts.ProximaNovaRegular(12);

		public static nfloat LoginWidth = 440;
		public static nfloat LoginHeight = 440;
		public static nfloat SectionHeight = 30;

        #region --awesoem fonts
        static string badgeImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-circle");
        static string settingsImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-gear");
        static string notificationImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-bell-o");
        static string twitterImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-twitter");
        static string scheduleImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-calendar");
        static string notesImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-pencil-square-o");
        static string refreshImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-refresh");
        static string cameraImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-camera");
        static string frontCameraImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-camera-retro");
        static string filterImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-filter");
        static string shareImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-share-square-o");
        static string roundPlus = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-plus-circle");
        static string locationImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-map-marker");
        static string searchImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-search");
        static string plusImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-plus");
        static string rightAngle = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-angle-right");
        static string leftAngle = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-angle-left");
		static string trashIcon = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-trash-o");
        static string retweetImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-retweet");
        static string starImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-star");
        static string twitter = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-tumblr-square");
        static string instagramimg = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-instagram");
        static string instagramLike = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-heart-o");
        static string instagramComment = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-comment-o");
        static string dottedImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-ellipsis-h");
        static string timeImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-clock-o");
        static string groupImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-group");
        static string ScheduleIcon = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-clock-o");
        static string IntrestIcon = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-group");
        static string UnSelectedStar = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-star-o");
        static string NormalCheckBox = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-square-o");
        static string SelectedCheckBox = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-check-square-o");
        static string forwardImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-angle-double-right");
        static string exhibitorImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-external-link");
		static string crossImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-times");
        static string pwdImage = FontAwesome.FontAwesomeIconStringForIconIdentifier("fa-key");
        #endregion
	
        #region--Images string
        const string foodAndDrink = "Food-and-Drink_normal.png";
        const string placeholderImage = "image.png";
        const string favouriteImage = "favorite.png";
        const string replyImage = "reply.png";

        const string surveysImage = "surveys.png";
        const string qrcodeImage = "qrCode.png";
        const string qrcodeSelectedImage = "qrCode.png";
        const string qrcodeHighLightedImage = "qrCode.png";
        const string powerImage = "power.png";
        const string notesHighLightedImage = "notes.png";
        const string noteImage = "note.png";  
        const string mosconeCeonterImage = "mosconeCenter.png";


        const string ScheduleSelectedIcon = "Icon_2x_0032_Sessions-big-white.png";

        const string IntrestSelectedIcon = "Icon_2x_0029_Speakers-white.png";
        const string selectedstarImage = "star-32.png";
        const string forwardArrowImage = "fwdarrow.png";
        const string smallCrossImage = "SmallCross.png";
        const string smallCrossWhiteImage = "SmallCrossWhite.png";
        const string backArrowImage = "backarrow.png";
        const string agenda_calendarImage = "agenda_calendar.png";
        const string agendaImage = "agenda.png";
        const string checkedBox = "check-box-checked.png";
        const string UncheckedBox = "check-box-blank.png";
        const string agendaIconImage = "agenda.png";
        const string agendaStarImage = "star.png";
        const string agendaIcon = "agenda_calendar.png";
        const string programIcon = "program-48.png";
        const string handsOnLabs = "HandsOnLabs.png";
		const string loginbackgroundImage = "loginbackground.jpg";
		const string logo = "PEXlogo.png";
		public static string backArrow = "backarrow.png";
		const string userIcon = "ic_user.png";
		const string passwordIcon = "password.png";

		const string imagePlaceholder = "image_icon.png";

        #endregion

		#region--Static Text For Whole App

		#region--Alert Box Messages Static Text

		const string loginRequiredText = "Login Required";
		const string loginMessageText = "You need to be logged-in to access this feature. Do you want to login?";
		const string confirmText = "Confirm";
		const string noTextTitle = "No";
		const string yesTextTitle = "Yes";
		const string dismissTxt = "Dismiss";
		const string cancelTextTitle = "Cancel";

		#endregion

		#region--Whats Happening Static Text

		const string noNextUpTextForNoSession = "No Next Up Sessions";
		const string nextUpTextForNoSession = "NextUp";
		const string whatsHappeningText = "What's Happening";
		const string announcementTextVar = "ANNOUNCEMENTS";
		const string featuredSessionTextVar = "RECOMMENDED SESSIONS";
		const string myScheduleTxtVariable = "MY SCHEDULE";
		const string nextUpHeaderTextvar = "NEXT UP";
		const string onNowHeaderTextvar = "ON NOW";
		const string noSessionCellTextForMyScheduleVariable = "No scheduled session";
		const string noSessionCellTextForFeaturedSessVariable = "No featured sessions are present";
		const string noInterestCellText = "There are no interested session at this time.";
		const string noScheduleCellText = "There are no scheduled session at this time.";

		const string noIntroText = "NoIntro";
		const string noIntroTextForNotAnyIntro = "No Announcements";
		const string featuredSessionTxt= "FeaturedSessions";

		#endregion        

		#region-- Social Media Static Text

		const string socialMediaText = "Social Media";
		const string twitterAllfeeds = "All Feeds";
		const string instagram = "instagram";
		const string facebook = "facebook";
		const string youtube = "youtube";
		const string actionSheetTitle = "Social Post Options";
		const string emailtext = "Email";
		const string messagetext = "Message";
		const string retweettext = "Retweet";
		const string replytext = "Reply";
		const string favouritetext = "Favourite";
		const string notConfiguredmailText = "Looks like you haven't configured your mail!";
		const string cantSendMsgtext = "Cant send message!";
		const string okText = "OK";
		const string noTwitterAccntsText = "No Twitter Accounts";
		const string noTwitterAccntsTextDescription = "There are no twitter accounts configured. You can add or create a twitter account in Settings.";
		const string twitterNotConfiguredinSettingsText = "Looks like you haven't configured twitter account in Settings!";
		const string cantRetweetText = "Cannot retweet this status!";
		const string divicesCantsendmessText = "Device can't send messages!";
		const string cantFavouriteText = "Cannot favourite this status!";
		const string postCharacterExceedText = "Post cannot exceed 140 characters!";
		const string replyTextCantEmpty = "Reply text cannot be empty!";
		const string replyFailedText = "Reply Failed!";
		const string replyShouldContainUsrname = "Reply should contain mentions (@username) to reply to!";
		const string cantRetweetOwnStatusText = "Cannot retweet your own status!";
		const string retweetThistoUrFollowerText = "Retweet this to your followers?";
		const string youCantRetweetOwnStatusText = "You cannot retweet your own status!!";
		const string doYouWantToPostText = "Do you want to post";
		const string cameraText = "Camera";
		const string doesntSupportFrontCameraText = "Device doesn't have front camera!";
		const string doesntSupportCameraText = "Device doesn't have a camera!";

		#endregion

		#region --Login View Controller Static Text

		const string loginTitle = "Login";
		const string logoutText = "Logout";
		const string userNameTextFieldPlaceHolder = "Username";
		const string passwordTextFieldPlaceHolder = "Password";
		const string fullScreenText = "Welcome to Partner Exchange 2015";
		const string forgetUsername = "Forgot Username ?";
		const string forgetPassowrd = "Forgot Password ?";
		const string forgetSomething = "Forgot Something?";
		const string loginButtonText = "Login";
		const string licensetext = "By logging in, you are accepting the terms of the";
		const string ERROR_MESSAGE = "Error occured";
		const string usernamecannotbeblankText = "Username cannot be blank.";
		const string passwordcannotbeblank = "Password cannot be blank.";
		const string sureToLogoutText = "Are you sure you want to logout?";
		const string licenceTextLink = "License Agreement";

		#endregion

		#region--Notes Static Text        
		const string titleForNotesText = "Title";
		const string descriptionForNotesText = "Description";
		const string noteTitle = "Note";
		const string notedeleteAlertTitle = "Are you sure you want to delete this note?";
		const string topBarTitleEdit = "Edit Note";
		const string topBarTitleNew = "New Note"; 
		const string btnSaveText = "Save";
		const string titleCantBeEmptyText = "Title cannot be empty.";
		const string failedText = "Failed!"; 
		const string noData = "No notes";
		#endregion

		#region--Session Detail Static Text
		const string addToMyScheduleTextTitle = "+ Add to My Schedule";
		const string removeFromMyScheduleTextTitle = "Remove from My Schedule";
		#endregion       

		#region--Transportation Static Text
		const string transportationText = "Transportation";
		#endregion

		#region-- Left Menu Static Text

		const string twitterNotConfiguredText = "Twitter may not be configured in Settings";
		public static readonly string LoginDetailText = "Login to your VMworld account to access your personal schedule, notes and take surveys.";

		#endregion

		#region--Food and Drinks View controller Static Text

		const string sanFransicoTitle = "San Fransico";

		#endregion

		#region--Venue Image Controller Static Text

		const string mosconeCenter = "Moscone Center";

		#endregion

		#region--Sessions Static Text

		const string SessionsTitle = "Session";
		const string speakerSearchText = "Search Speakers";
		const string speakerText = "Speaker";
        const string speakersText = "Speakers";
		const string allDaysText = "All,Days";
		const string singleDayText = "days";



		#endregion

		#region--Schedule And Interest Static Text

		const string scheduletext = "Schedule";
		const string interestTitle = "Interests";


		#endregion

		#region--Filter Static Text

		const string filterText = "Filter";
		const string subTracksText = "Sub-Tracks";

		#endregion

		#region-- Appsettings Static Text

		const string appSettingsText = "App Settings";

		#endregion


		#region--Agenda Static Text

		const string agendaNavigationText = "Agenda Event";
		const string agendaTitle = "Agenda";

		#endregion

		#region -- Programs and Faq Static Text

		const string programTitle = "Programs";       
		const string holTextTitle = "Hands-on Labs";

		#endregion

		#region--Internet Error Text

		const string internetError = "Check your connection and try again.";
		const string noInternetConnectionTxt = "No Internet Connection and try again.";
		public static readonly string NoInternetConnectionTxt = noInternetConnectionTxt;

		#endregion

		#region--Exhibitor Detail Static Text

		const string exhibitorNavigationTitle = "Exhibitor";
		const string exhibitorSearchText = "Search Exhibitors";
		const string exhibitorsTitleString = "Exhibitors";
		const string boothHashText = "Booth #";
        const string ExhibitorText = "Visit Sponsor Website";

		#endregion

		#region--Daily Highlights Static Text

		const string newsTxt = "News";

		#endregion

		#region--Food and Drink Controller

		const string foodAndDrinkTitle = "Food and Drink";

		#endregion

		#region--Speakers Static Text

		const string speakersTextTitle = " Speakers";
		const string noSpeakersTextTitle = "No Speakers";
		const string simpleSpeakersTextTitle = " Speaker";

		#endregion

		#region--Notes Static Text

		const string notestopBarTitleText = "Note";
		const string notesDeleteAlert = "Are you sure you want to delete this note?";
		const string byDateText = "by Date";
		const string byTitleText = "by Title";
		const string byTagText = "by Tag";

		#endregion

		const string notificationtitleText = "My Notification";       

		#endregion



        #region--Colors
		static readonly UIColor tetLightGrayColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		static readonly UIColor textBlackColor = UIColor.Clear.FromHexString (TextColor,10.0f);
        static readonly UIColor cellTextColorSelected = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		static readonly UIColor pageControlOnColorVariable = UIColor.Clear.FromHexString (SecondaryColor, 1.0f);
		static readonly UIColor pageControlOffColorVariable = UIColor.Clear.FromHexString (SectionBackColor, 1.0f);

		static readonly UIColor myScduleSectionHeaderBgColorVariable = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        static readonly UIColor whatsHappeningBackGroundColorVariable = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

		static readonly UIColor wholeViewColor = UIColor.Clear.FromHexString(Layer1Color,1.0f);

        static readonly UIColor agendaBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

		static readonly UIColor LightGrayTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        #endregion

        #region--Constant floats

		static nfloat borderForSideOfTableWidht = separatorBorderWidth;
		static nfloat borderForSectionHeaderTopWidht = 1;
		static nfloat announceMentCellHeightVariable = 200;
		static nfloat trackButtonRowHeightVar = 70;
		static nfloat featuredSessionCellRowHeightVar = 80;
		static nfloat titleHeaderViewHeightVar = 64;
		static nfloat sectionBottomBorderHeightVariable = 1;
		static nfloat sectionheaderTextLeftPaddingVariable = 21;
		static nfloat myScheduleSectionHeaderHeightVar = 40;
		static int noOfsectionsForWhatsHappenigMiddleScreenVar = 2;
		static nfloat middleborderVar = 2;
		static nfloat separatorBorderWidth = 1;

        #endregion

        #region--Theming for Controllers        

        #region --AgendaDetailController--
        public static readonly UIColor ADBackgroundColor = agendaBackgroundColor;
		public static readonly UIColor ADNavigationTitle = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor ADNameTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor ADLocationColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor ADTimingColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor ADLineviewColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly string ADIconImage = agendaIconImage;
        public static readonly string ADStarImage = agendaStarImage;
        public static readonly UIFont ADNavigationTitleFont = AppFonts.ProximaNovaRegular(20);
        public static readonly string ADNavigationText = agendaNavigationText;
        #endregion

        #region--Agenda at Glance
        public static readonly string AGagendaTitle = agendaTitle;
        public static readonly string APNewImage = agendaIcon;
        public static readonly string programImage = programIcon;
        public static readonly string handOnlabsImage = handsOnLabs;
		public static readonly UIColor ADcellBottomLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);
        
        public static readonly UIColor ADcellTextColorselcted = cellTextColorSelected;
        public static readonly string AGalldaysText = allDaysText;
        public static string AGsingleDayText = singleDayText;
        public static readonly UIColor AGsegmentbackColor = SegmentBackgroundColor;
		public static readonly UIColor AGsegmentTitletextColor = SegmentTitletextColor;
		public static readonly UIColor AGsegmentSubTitletextColor = SegmentSubTitletextColor;
		public static readonly UIFont AGsegmentFont = AppFonts.ProximaNovaSemiBold(15);
        public static readonly UIColor AGsegmentSelectedtabColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont AGmultiLineFont = AppFonts.ProximaNovaRegular(12);
		public static readonly UIColor AGsegmentSeparatorColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor AGsegmentSelectedtabBottomColor = UIColor.Clear;
		public static readonly UIColor AGmenuSectionSeparator = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor AGheaderViewBackcolor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor AGheaderLabelBackcolor = UIColor.Clear;
		public static readonly UIColor AGheaderTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont AGheaderLabelFont = AppFonts.ProximaNovaRegular(16);

        #endregion

        #region--Program and Faq Controller
        public static readonly string PFagendaTitle = agendaTitle;
        public static readonly string PFprogramTitle = programTitle;
		public static readonly string PFHOlTitle = holTextTitle;
        public static readonly UIFont PFprogramHeaderfont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor PFprogramTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIColor PFtopHeaderBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor PFblankViewBackColor = Layer3BaseColor;
		public static readonly UIColor PFverticalLineBackColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        public static readonly UIColor PFtitleHeaderViewBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor PFprogramHeaderViewBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor PFtopBorderLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);
        #endregion

        #region--AppSetting Controller
        public static readonly string InternetErrorText = internetError;
        public static readonly string AppSettingsText = appSettingsText;
        public static readonly string DismissText = dismissTxt;
        public static readonly string APSInternetErrorText = internetError;
        public static readonly nfloat APSseparatorBorderWidth = separatorBorderWidth;
        public static readonly UIFont APSTitleLabelFont = AppFonts.ProximaNovaRegular(22);
        public static readonly UIColor APSBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor APSCloseBtnBackgroundColor = UIColor.Clear.FromHexString(Layer1Color,1.0f);
		public static readonly UIColor APSmenuSectionBackgroundColor = UIColor.Clear.FromHexString (SectionHeadingbackground, 1.0f);
        public static readonly UIColor APStitleLabelBackColor = AppTheme.ClearColor;
        public static readonly UIFont APSSectionTitleFont = AppFonts.ProximaNovaRegular(14);
        #endregion

        #region--DailyHighlights

		public static readonly string DHNewsText = newsTxt;     //strings

		public static readonly nfloat DHSeparatorBorderWidth = separatorBorderWidth;    //floats
		public static readonly nfloat DHcellRowHeight = 60;

        public static readonly string DHplaceholderImage = placeholderImage;
		public static readonly UIColor DHPageBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);              //Colors
		public static readonly UIColor DHNewsLinkTextColor = DarkGrayText;
		public static readonly UIFont DHNewsLinkFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIFont DHtitleLabelFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor DHImageCoverPhotoBackColor = UIColor.Black.ColorWithAlpha(0.1f);        
		public static readonly UIColor DHCellTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor DHHighLightedTextColor = UIColor.Clear.FromHexString(Layer1Color,1.0f);
		public static readonly UIColor DHdescriptionHighLightedTextColor = UIColor.Clear.FromHexString(Layer1Color,1.0f);
		public static readonly UIColor DHTitleBackColor = UIColor.Clear;
		public static readonly UIColor DHDescBackColor = UIColor.Clear;
		public static readonly UIColor DHcellDescriptionColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);        
		public static readonly UIColor DHnotesCellSelectedBackColor = CellSelectedbackgroundColor;
		public static readonly UIColor DHnewsdetailsbackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

		public static readonly UIFont DHdescLabelFont = AppFonts.ProximaNovaRegular(14);  //Fonts

		public static readonly UIFont DHnewsTitleFont = AppFonts.ProximaNovaRegular(20);

		public static readonly UIFont DHdescriptionFont = AppFonts.ProximaNovaRegular(15);


        #endregion

		#region --HeaderView

		#endregion


        #region --Sponsors and Exhibitors Controller
        public static readonly string EXNavigationTitleText = exhibitorNavigationTitle;
        public static readonly string SEexhibitorSearchText = exhibitorSearchText;
        public static readonly string SEexhibitorsTitleString = exhibitorsTitleString;
		public static readonly UIFont SESectionTitleFont = AppFonts.ProximaNovaRegular(14);

        public static readonly string BoothHashText = boothHashText;
        public static readonly string SPCExhibitorText = ExhibitorText;
        public static readonly nfloat EXseparatorBorderWidth = separatorBorderWidth;
        public static readonly UIColor EXpageBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor EXnameTextColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont EXnameFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont EXboothNameFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor EXcompanyNameColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor EXlineColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor EXdescriptionColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly string SECsponsorImage = exhibitorImage;

        public static readonly UIColor SECsponsorImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SECsponsorImageFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SECsponsorImageFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont SECsponsorImageFont = FontAwesome.Font(20);

		public static readonly string EXLogoPlaceholder = imagePlaceholder;

		//		Detail screen
        #endregion

		#region--Food and Drink controller

		public static readonly string FDtitleText = foodAndDrinkTitle;     //strings
		public static readonly string FDTabImage = foodAndDrink;

		public static readonly nfloat FDseparatorBorderWidth = separatorBorderWidth;   //floats

		public static readonly UIColor FDsegmentTitletextColor = SegmentTitletextColor;     //Colors
		public static readonly UIColor FDsegmentSubTitletextColor = SegmentSubTitletextColor;     //Colors
		public static readonly UIColor FDsegmentViewBackColor = SegmentBackgroundColor;
		public static readonly UIColor FDsegmentSelectedtabColor = SegmentSelectedtabColor;
		public static readonly UIColor FDsegmentSelectedtabBottomColor = SegmentSelectedtabBottomColor;
		public static readonly UIColor FDsegmentSeparatorColor = SegmentSeparatorColor;

		public static readonly UIFont FDsegmentFont = SegmentFont;      //Fonts
		public static readonly UIFont FDmultiLineFont = MultiLineFont;

		public static readonly UIFont VNDetailTextFont = AppFonts.ProximaNovaRegular(12); 

		public static readonly UIFont FDDetailTextFont = AppFonts.ProximaNovaRegular(14); 
		public static readonly UIColor FDDetailTextFontColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIFont FDTitleTextFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor FDTitleTextFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);

		#endregion

        #region--Food and Drinks View controller
        public static readonly string FDtitleTextSanFrancisco = sanFransicoTitle;
        #endregion

        #region -- HOL detail controller
        public static readonly string HOLspeakersTextTitleText = speakersTextTitle;
        public static readonly string HOLnoSpeakersTextTitleText = noSpeakersTextTitle;
        public static readonly string HOLsimpleSpeakersTitleText = simpleSpeakersTextTitle;
        public static readonly string HOLTextTitle = holTextTitle;
		public static readonly UIColor HOLDescTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont HOLDescTextFont = AppFonts.ProximaNovaRegular (14);
        public static readonly UIColor HOLpageBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor HOLnameTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont HOLnameFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor HOLheaderBackColor = UIColor.Clear.FromHexString (SectionHeadingbackground, 1.0f);
		public static readonly UIColor HOLheaderLabelTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont HOLheaderLabelFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIColor HOLheaderBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        #endregion

        #region--Login View Conntroller

        #endregion

        #region--Unknown region

        public static readonly string placeholderIcon = "placeholder.png";
        public static readonly string anonymousUser = "anonymousUser.png";

        public static readonly UIColor NavigationTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor NavigationTintColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor NavigationBarTintColor = UIColor.Black;

        public static readonly UIColor ClearColor = UIColor.Clear;

        public static readonly UIColor PageBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

		public static readonly UIColor DarkGrayBorder = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor SessionButtonText = UIColor.Clear.FromHexString (TextColor,1.0f);

        public static readonly UIColor MenuNormalCell = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);

		public static readonly UIColor MenuSectionBackgroundColor = UIColor.Clear.FromHexString (SectionHeadingbackground, 1.0f);

        public static readonly UIColor MenuHighlightedCell = CellSelectedbackgroundColor;
		public static readonly UIColor MenuBadgeColor = UIColor.Clear.FromHexString(SecondaryColor,1.0f);

		public static readonly UIColor MenuHeaderSeparator = UIColor.Clear.FromHexString (LineColor, 1.0f);
		public static readonly UIColor MenuSectionSeparator = UIColor.Clear.FromHexString (LineColor, 1.0f);

        public static readonly UIColor Section = UIColor.Black;
        public static readonly UIColor TableSeparator = UIColor.LightGray;
        public static readonly UIColor SectionBackground = UIColor.LightGray;

		static readonly UIColor DarkGrayText = UIColor.Clear.FromHexString (TextColor, 1.0f);//UIColor.DarkGray;
		public static readonly UIColor GrayText =  UIColor.Clear.FromHexString (NavigationTextAndIcons, 1.0f);
        
		public static readonly UIColor cellBottomLineColorVar = UIColor.Clear.FromHexString(LineColor, 1.0f);
        public static readonly string ScgroupImage = groupImage;
        public static readonly string SctimeImage = timeImage;


        public static readonly string ScheduleBigIcon = ScheduleIcon;//..
        public static readonly string ScheduleSelectedBigIcon = ScheduleSelectedIcon;

        public static readonly string IntrestBigIcon = IntrestIcon;
        public static readonly string IntrestSelectedBigIcon = IntrestSelectedIcon;

        #endregion

        #region -- AgendaProgramHandsonLabs

        #endregion

        #region-- Venue Image controller
        public static readonly string VImosconeCenterText = mosconeCenter;
        public static readonly UIFont VIvenueInfoFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor VIvenueInfoTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont VIvenueNameFont = AppFonts.ProximaNovaRegular(18);
        public static readonly UIColor VIvenuenameTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont VIvenueDescFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor VIvenueDescBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor VIvenueDescTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont VIvenueAddressFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor VIvenueAddressTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor VIvenueLocationBtnBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor VImiddleBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        public static readonly UIColor VIvenueImageViewBackColor = UIColor.Clear;
		public static readonly UIColor VIvenueImageViewBackGroundView = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor VIvenueTitleBottomBorderColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly string VIimgLocation = locationImage;
		public static readonly UIColor VIimgLocationNormalColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor VIimgLocationSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor VIimgLocationHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont VIimgLocationImageFont = FontAwesome.Font(26);
        public static readonly nfloat VIMiddleLineWidht = borderForSideOfTableWidht;
        #endregion

        #region-- Moscone center
        public static readonly nfloat LineWidht = borderForSideOfTableWidht;
        public static readonly UIFont venueTitleFont = AppFonts.ProximaNovaRegular(16);
        static readonly UIFont venueinfoFontVar = AppFonts.ProximaNovaRegular(16);
        public static UIFont venueinfofont = venueinfoFontVar;
        public static readonly UIColor MCVenueImageBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly string imgLocation = locationImage;
       
        
		public static readonly UIColor VenuecellBottomLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);

        #endregion

        #region--Session Controller
        public static readonly UIColor SCsidepanelBackColor = Layer3BaseColor;
        public static readonly UIColor SCblankViewBackColor = UIColor.Clear.FromHexString(AppTheme.Layer3Color, 1.0f);
        public static readonly string SCScheduleIcon = timeImage;
		public static readonly UIColor SCScheduleNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SCScheduleSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SCScheduleHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont SCScheduleIconFont = FontAwesome.Font(20);
        public static readonly string SCallDaysText = allDaysText;
        public static string SCsingleDayText = singleDayText;
		public static readonly UIColor SCsegmentTitletextColor = SegmentTitletextColor;
		public static readonly UIColor SCsegmentSubTitletextColor = SegmentSubTitletextColor;
		public static readonly UIFont SCsegmentFont = AppFonts.ProximaNovaSemiBold(13);
        public static readonly UIColor SCsegmentSelectedtabColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SCsegmentSelectedtabBottomColor = UIColor.Clear;
        public static readonly UIFont SCmultiLineFont = AppFonts.ProximaNovaRegular(12);
		public static readonly UIColor SCsegmentSeparatorColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        public static readonly string SCPlusIcon = plusImage;
		public static readonly UIColor SCPlusIconNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SCPlusIconSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SCPlusIconHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont SCPlusIconNameFont = FontAwesome.Font(26);
		public static readonly UIColor SCtableHeaderbackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor SCheaderlabelbackColor = UIColor.Clear;
		public static readonly UIColor SCheaderlabelTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor SCmenuSectionSeparator = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIFont SCSectionTextFont = AppFonts.ProximaNovaSemiBold(14);

        public static readonly string SCsessionsTitleText = SessionsTitle;
        public static readonly string SCspeakerSearchText = speakerSearchText;
        public static readonly string SCspeakesText = speakersText;
        public static readonly string AllDaysText = allDaysText;
        public static string SingleDayText = singleDayText;
        public static readonly string ConfirmText = confirmText;
        public static readonly string LoginRequiredText = loginRequiredText;
        public static readonly string LoginMessageText = loginMessageText;
        public static readonly string NoTextTitle = noTextTitle;
        public static readonly string YesTextTitle = yesTextTitle;
        public static readonly UIColor SCsegmentbackColor = SegmentBackgroundColor;
        #endregion

        #region--Notification View Controller
        public static readonly nfloat NVSeparatorBorderWidth = separatorBorderWidth;
        #endregion

        #region--Schedule and Interest
        public static readonly string Scheduletext = scheduletext;
        public static readonly string SIinterestTitle = interestTitle;
        public static readonly UIColor SIsegmentbackColor = SegmentBackgroundColor;
		public static readonly UIColor SIsegmentTitletextColor = SegmentTitletextColor;
		public static readonly UIColor SIsegmentSubTitletextColor = SegmentSubTitletextColor;
		public static readonly UIFont SIsegmentFont = AppFonts.ProximaNovaSemiBold(13);
        public static readonly UIColor SIsegmentSelectedtabColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SIsegmentSelectedtabBottomColor = UIColor.Clear;
        public static readonly UIFont SImultiLineFont = AppFonts.ProximaNovaRegular(12);
		public static readonly UIColor SIsegmentSeparatorColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        public static readonly UIColor SItmpSelectedScheuleBackColor = UIColor.Red;
        public static readonly UIColor SIcellSelectedbackgroundColor = UIColor.Clear.FromHexString(Layer2Color, 1.0f);
        public static readonly string SIalldaysText = allDaysText;
        public static string SIsingleDayText = singleDayText;
		public static readonly UIColor SIinterestHeaderViewbackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SIinterestHeaderLabelbackColor = UIColor.Clear;
		public static readonly UIColor SIinterestHeaderLabelTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont SIintrerestHeaderLabelFont = AppFonts.ProximaNovaSemiBold(14);
		public static readonly UIColor SImenuSectionSeparator = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor SIscheduleHeaderViewbackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SIscheduleHeaderLabelbackColor = UIColor.Clear;
		public static readonly UIColor SIscheduleHeaderLabelTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont SIscheduleHeaderLabelFont = AppFonts.ProximaNovaSemiBold(14);
        public static string SInoScheduleCellText = noScheduleCellText;
        #endregion

        #region--Interest

        #endregion

        #region--Filter
        public static readonly string FilterText = filterText;
        public static readonly string SubTracksText = subTracksText;

        // Normal Check Box
        public static readonly string FTNormalCheckBox = NormalCheckBox;
		public static readonly UIColor FTNormalCheckBoxNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor FTNormalCheckBoxSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor FTNormalCheckBoxFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont FTNormalCheckBoxFont = FontAwesome.Font(20);

        public static readonly string FTSelectedCheckBox = SelectedCheckBox;
		public static readonly UIColor FTSelectedCheckBoxNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor FTSelectedCheckBoxSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor FTSelectedCheckBoxFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont FTSelectedCheckBoxFont = FontAwesome.Font(15);

        #endregion

		#region--SocialMedia--

		public static readonly string SMRetweetImage = retweetImage;       //strings
		public static readonly string SMFavouriteImage = starImage;
		public static readonly string SMTwitterImage = twitter;
		public static readonly string SMInstagramImage = instagramimg;
		public static readonly string SMInstagramLike = instagramLike;
		public static readonly string SMInstagramComment = instagramComment;
		public static readonly string SMDottedImage = dottedImage;
		public static readonly string SMplaceholderImage = placeholderImage;
		public static readonly string SMfavouriteImage = favouriteImage;
		public static readonly string SMreplyImage = replyImage;
		public static readonly string SMdottedImage = dottedImage;
		public static readonly string SMinternetErrorText = internetError;
		public static readonly string SMsocialMediaText = socialMediaText;
		public static readonly string SMtwitterAllFeedsText = twitterAllfeeds;
		public static readonly string SMinstagramText = instagram;
		public static readonly string SMfacebookText = facebook;
		public static readonly string SMyoutubeText = youtube;
		public static readonly string SMYActionSheetTitleText = actionSheetTitle;
		public static readonly string EmailText = emailtext;
		public static readonly string MessageText = messagetext;
		public static readonly string Retweettext = retweettext;
		public static readonly string Replytext = replytext;
		public static readonly string Favouritetext = favouritetext;
		public static readonly string NotConfiguredmailText = notConfiguredmailText;
		public static readonly string CantSendMsgtext = cantSendMsgtext;
		public static readonly string OkText = okText;
		public static readonly string NoTwitterAccntsText = noTwitterAccntsText;
		public static readonly string NoTwitterAccntsTextDescription = noTwitterAccntsTextDescription;
		public static readonly string TwitterNotConfiguredInSettingsText = twitterNotConfiguredinSettingsText;
		public static readonly string CantRetweetText = cantRetweetText;
		public static readonly string DivicesCantsendmessText = divicesCantsendmessText;
		public static readonly string CantFavouriteText = cantFavouriteText;
		public static readonly string PostCharacterExceedText = postCharacterExceedText;
		public static readonly string ReplyTextCantEmpty = replyTextCantEmpty;
		public static readonly string ReplyFailedText = replyFailedText;
		public static readonly string ReplyShouldContainUsrname = replyShouldContainUsrname;
		public static readonly string CantRetweetOwnStatusText = cantRetweetOwnStatusText;
		public static readonly string RetweetThistoUrFollowerText = retweetThistoUrFollowerText;
		public static readonly string YouCantRetweetOwnStatusText = youCantRetweetOwnStatusText;
		public static readonly string DoYouWantToPostText = doYouWantToPostText;
		public static readonly string CameraText = cameraText;
		public static readonly string DoesntSupportFrontCameraText = doesntSupportFrontCameraText;
		public static readonly string DoesntSupportCameraText = doesntSupportCameraText;
		public static readonly string SMtwitterNotConfiguredText = twitterNotConfiguredText;
		public static readonly string SMInstagramLogo = "instagram_icon.png";
		public static readonly string SMInstagramLove = "instagram_love.png";

		public static readonly nfloat SMborderForSectionHeaderTopWidhtVar = borderForSectionHeaderTopWidht;
		public static readonly nfloat SMseparatorBorderWidth = separatorBorderWidth;

		public static readonly UIColor SMsectionHeaderBackColor = UIColor.Clear.FromHexString(SectionHeadingbackground, 1.0f);
		public static readonly UIColor SMmenuSectionSeparator = UIColor.Clear.FromHexString(LineColor, 1.0f);
		public static readonly UIColor SMsectionTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);

		public static readonly UIFont SMsectionLabelSectionFont = AppFonts.ProximaNovaRegular (14);

		//Retweet Button
		public static readonly UIColor SMRetweetNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMRetweetFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMRetweetFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMRetweetFont = FontAwesome.Font(15);

		//Favourite Button
		public static readonly UIColor SMFavouriteNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMFavouriteFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMFavouriteFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMFavouriteFont = FontAwesome.Font(15);

		//Twitter Image
		public static readonly UIColor SMTwitterNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMTwitterFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMTwitterFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMTwitterFont = FontAwesome.Font(20);

		//Instagram Image
		public static readonly UIColor SMInstagramNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMInstagramFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMInstagramFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMInstagramFont = FontAwesome.Font(20);

		//Instagram Like
		public static readonly UIColor SMInstagramLikeNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMInstagramLikeFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMInstagramLikeFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMInstagramLikeFont = FontAwesome.Font(15);

		//Instagram Comment
		public static readonly UIColor SMInstagramCommentNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMInstagramCommentFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMInstagramCommentFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMInstagramCommentFont = FontAwesome.Font(15);

		//Dotted Image
		public static readonly UIColor SMDottedImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor SMDottedImageFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor SMDottedImageFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont SMDottedImageFont = FontAwesome.Font(20);

		public static readonly UIColor SMfeedDescTextColor = DarkGrayText;
		public static readonly UIColor SMtwitterSocialCellTextColor = DarkGrayText;

		#endregion

		#region--Notes

		public static readonly string NTtitleForNotesText = titleForNotesText;        //strings
		public static readonly string NTdescriptionForNotesText = descriptionForNotesText;        
		public static readonly string NtnoteTitle = noteTitle;        
		public static readonly string NTdeleteAlertTitle = notedeleteAlertTitle;        
		public static readonly string NTtopBarTitleEdit = topBarTitleEdit;        
		public static readonly string NTtopBarTitleNew = topBarTitleNew;        
		public static readonly string NTbtnSaveText = btnSaveText;        
		public static readonly string FailedText = failedText;        
		public static readonly string NoNotesData = noData;
		public static readonly string TitleCantBeEmptyText = titleCantBeEmptyText;
		public static readonly string NTcrossImage = crossImage;
		public static readonly UIColor NTcrossImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor NTcrossImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor NTcrossImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont NTcrossImageFont = FontAwesome.Font(19);

		public static readonly UIColor NTLineColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly string NTDeleteAlert = notesDeleteAlert;
		public static readonly string NTbydateText = byDateText;
		public static readonly string NTbytitleText = byTitleText;
		public static readonly string NTbytagText = byTagText;
		public static readonly string NTnotesTitle = notestopBarTitleText;

		public static readonly string NTDeleteIconName = trashIcon;
		public static readonly UIColor NTDeleteIconNameNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor NTDeleteIconNameSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor NTDeleteIconNameHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIFont NTDeleteIconNameFont = FontAwesome.Font(20);

		public static readonly string NotesEditIconName = notesImage;
		public static readonly UIColor NotesEditIconNameNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor NotesEditIconNameSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor NotesEditIconNameHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIFont NotesEditIconNameFont = FontAwesome.Font(20);

		public static string NTbackArrow = backArrow;

		public static readonly nfloat NTSeparatorBorderWidth = separatorBorderWidth;     //floats

		public static readonly UIColor NotesCellSelectedColor = CellSelectedbackgroundColor;
		public static readonly UIColor NTnotesCellTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);          //colors
		public static readonly UIColor NotesCellDescriptionColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor NotesCellTimeColor = UIColor.Clear.FromHexString(NavigationTextAndIcons, 1.0f);
		public static readonly UIColor NTCellSelectedColor = CellSelectedbackgroundColor;
		public static readonly UIColor NTsegmentbackColor = SegmentBackgroundColor;
		public static readonly UIColor NTtopBarColor = UIColor.Clear.FromHexString(Layer1SubviewsColor,1.0f);
		public static readonly UIColor NDButtonBGGray = UIColor.Clear.FromHexString(Layer1SubviewsColor,1.0f);
		public static readonly UIColor NTButtonTextColor = UIColor.Clear.FromHexString (Layer1Color, 1.0f);
		public static readonly UIColor NTCancelButtonTextColor = UIColor.Clear.FromHexString (TextColor, 1.0f);

		public static readonly UIColor NDBtnSaveBGColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor,1.0f);
		public static readonly UIColor NTPageBGColor =UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor NTsegmentTitletextColor = SegmentTitletextColor;
		public static readonly UIColor NTsegmentSubTitletextColor = SegmentSubTitletextColor;
		public static readonly UIColor NTsegmentSelectedtabColor = SegmentSelectedtabColor;
		public static readonly UIColor NTsegmentSelectedtabBottomColor = SegmentSelectedtabBottomColor;
		public static readonly UIColor NTsegmentSeparatorColor = SegmentSeparatorColor;
		public static readonly UIColor NTTagNavigatorColor = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);


		public static readonly UIColor NTtopBarTitleBackColor = UIColor.Clear;
		public static readonly UIColor NTcellHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor NTcellTitleBackColor = UIColor.Clear;
		public static readonly UIColor NTlblDescPlaceHolderTextColor = LightGrayTextColor;
		public static readonly UIColor NTimageScrollerBackColor= UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor NTLineViewColor= UIColor.Clear.FromHexString(LineColor, 1.0f);

		public static readonly UIFont NTsegmentFont =SegmentFont;
		public static readonly UIFont NTmultiLineFont = MultiLineFont;
		public static readonly UIFont NTtopbarTitleFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor NTtopbarTitleFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);

		public static readonly UIColor NTtopbarTitleColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor,1.0f);
		public static readonly UIFont NTtitleFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor NTtitleFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont NTdescriptionFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor NTdescriptionFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont NTtagFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIFont NTtimeLabelFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIFont NTtextfieldTitleFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor NTtextfieldTitleFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIFont NTtextfieldDescFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor NTtextfieldDescFontColor = UIColor.Clear.FromHexString(TextColor,1.0f);

		public static readonly UIFont NTlblDescPlaceHolderFont = AppFonts.ProximaNovaRegular(12);
		public static readonly UIFont NTbtnCancelFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIFont NTbtnSaveFont = AppFonts.ProximaNovaRegular(16);


		#endregion       

        #region--ToastView--
        public static string checkMarkImg = "checkmark.png";
        #endregion

        #region--sidePanel--
        public static readonly UIFont SPIconFontSize = FontAwesome.Font(40);
        #endregion

        #region-- Speakers Controller

        public static readonly UIColor SPCcellSelectedbackgroundColor = CellSelectedbackgroundColor;
        public static readonly UIColor SPCspeakersTableBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

        #endregion

        #region --Session Details--

        public static readonly string SDsessionsTitleText = SessionsTitle;
        public static readonly string SDunSelectedSessionDetailStarImage = selectedstarImage;
        public static readonly string SDsessionDetailStarImage = starImage;
        public static readonly UIColor SDsessionDetailsBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont SDsessionDetailsNavigationTitleFont = AppFonts.ProximaNovaRegular(20);
		public static readonly UIColor SDsessionDetailsNavigationTitle = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor SDsessionDetailsSessionTitleTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont SDsessionNameFont = AppFonts.ProximaNovaRegular(18);
        public static readonly UIFont SDlocationFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont SDtimingFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont SDsessionBtnFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont SDdescriptionFont = AppFonts.ProximaNovaRegular(16);

		public static readonly UIColor SDlocationColor = UIColor.Clear.FromHexString (NavigationTextAndIcons, 1.0f);
		public static readonly UIColor SDTimingColor = UIColor.Clear.FromHexString (NavigationTextAndIcons, 1.0f);

        public static readonly string UnSelectedSessionDetailStarImage = selectedstarImage;        
        public static readonly string SDaddToMyScheduleTextTitle = addToMyScheduleTextTitle;
       
        public static readonly string SDremoveFromMyScheduleTextTitle = removeFromMyScheduleTextTitle;
		public static readonly UIColor SDdescriptionTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly string SDspeakersTextTitleText = speakersTextTitle;
        public static readonly string SDspeakerTextTitleText = simpleSpeakersTextTitle;

        public static readonly string SDNormalStar = UnSelectedStar;
        public static readonly string SDSelectedStar = starImage;

		public static readonly string SDNormalAddSession = plusImage;
		public static readonly string SDSelectedAddSession = ScheduleIcon;//..
		public static readonly UIColor SDNormalAddSessionPlusColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor SDSelectedAddSessionPlusColor = UIColor.Clear.FromHexString(Layer3Color,1.0f);

		public static readonly UIColor SDSectionBGColor = UIColor.Clear.FromHexString (SectionHeadingbackground, 1.0f);
		public static readonly UIColor SDSectionTextColor = UIColor.Clear.FromHexString (TextColor, 1.0f);
		public static readonly UIColor SDSeparatorBGColor = UIColor.Clear.FromHexString (LineColor, 1.0f);
		public static readonly UIFont SDSectionTextFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIColor SDNormalStarNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SDNormalStarSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SDNormalStarHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont SDNormalStarNameFont = FontAwesome.Font(26);

        //selected star
		public static readonly UIColor SDSelectedStarNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SDSelectedStarSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SDSelectedStarHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont SDSelectedStarNameFont = FontAwesome.Font(26);

        public static readonly UIFont SessionDetailsNavigationTitleFont = AppFonts.ProximaNovaRegular(20);

        #endregion

        #region--Custom Table View

        public static readonly string CTsessionsTitleText = "Sessions";
        public static readonly string CTspeakesText = speakersText;

        #endregion

        #region --Speaker Details--

        const string speakerStarImage = "star.png";
        public const string fwdArrowImage = "fwdarrow.png";
		public static readonly UIColor SPspeakerDetailsNavigationTitle = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly string SPspeakerstarImage = speakerStarImage;
		public static readonly UIColor SPspeakerNameColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont SPspeakerNameFont = AppFonts.ProximaNovaRegular(18);
        public static readonly UIFont SPcompanyNameFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIColor SPcompanyNameColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor SPjobTitleColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly UIFont SPjobTitleFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIColor SPdescriptionColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont SPdescriptionFont = AppFonts.ProximaNovaRegular(16);
        public static readonly UIColor SPheaderViewbackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor SPSectionBGColor = UIColor.Clear.FromHexString(SectionHeadingbackground, 1.0f);
        public static readonly UIColor SPheaderlabelbackColor = UIColor.Clear;
		public static readonly UIColor SPSectionTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIFont SPlabelSectionFont = AppFonts.ProximaNovaRegular(14);
		public static readonly UIColor SPspeakerDetailSeparatorColor = UIColor.Clear.FromHexString(LineColor,1.0f);
        public static readonly string SPspeakerNavigationText = speakerText;
        public static readonly UIColor SPspeakerDeatailBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont SPnavigationTitleFont = AppFonts.ProximaNovaRegular(18);
		public static readonly UIColor SpeakerSessionTitleColor = UIColor.Clear.FromHexString(TextColor,1.0f);        
		public static readonly UIColor SpeakerSessionTimeColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);

        #endregion

        #region --Login View Controller--

        public static readonly string LVCPwdImage= pwdImage;
        public static readonly UIColor LVCPwdImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LVCPwdImageFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LVCPwdImageFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont LVCPwdImageFont = FontAwesome.Font(20);



        public static readonly string LIuserNameTextFieldPlaceHolder = userNameTextFieldPlaceHolder;        
        public static readonly string LIpasswordTextFieldPlaceHolder = passwordTextFieldPlaceHolder;
        public static readonly string LIusernamecannotbeblankText = usernamecannotbeblankText;
        public static readonly string LIpasswordcannotbeblankText = passwordcannotbeblank;
        public static readonly UIColor LIinstanceRootViewbackColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.5f);

        public static readonly string LIfullScreenText = fullScreenText;
        public static readonly UIFont LItitleLabelFont = AppFonts.ProximaNovaRegular(22);
        public static readonly UIColor LIsubTitleTextColor = DarkGrayText;
        public static readonly UIColor LIforgotPwdbackcolor = UIColor.Clear;
        public static readonly UIColor LItitleLabelbackcolor = UIColor.Clear;
        public static readonly UIColor LIsubTitlebackcolor = UIColor.Clear;
		public static readonly UIColor LIuserNameRightViewcolor = UIColor.Clear;
		public static readonly UIColor LIuserNameLeftViewcolor = UIColor.Clear;
        public static readonly UIColor LIuserNameRightView1color = UIColor.Clear;
        public static readonly UIColor LIuserNameLeftView1color = UIColor.Clear;
        public static readonly string LIForgetUsername = forgetUsername;
        public static readonly string LIForgetPassowrd = forgetPassowrd;
        public static readonly string LIforgetSomethingText = forgetSomething;
        public static readonly string LIloginButtonText = loginButtonText;
        public static readonly string LIlicensetext = licensetext;
		public static readonly string LIlicenseLinkText = licenceTextLink;
        public static readonly string LIerrorMessgeText = ERROR_MESSAGE;
       
		public static readonly string LIBackgroundImage = loginbackgroundImage;
		public static readonly string LILogo = logo;
		public static readonly string LIUserIcon = userIcon;
		public static readonly string LIPasswordIcon = passwordIcon;
		public static readonly UIColor LIBackgroundColor = UIColor.Clear.FromHexString(Layer2Color, 1.0f);
		public static readonly UIColor LILoginbuttonBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor LILoginbuttonBackgroundColorInLeftmenu = UIColor.Clear.FromHexString(ButtonBackgroundColor, 1.0f);
		public static readonly UIColor LITextViewBackgroundColor = UIColor.FromRGBA(0.0f,0.0f,0.0f,0.4f);
        public static readonly UIColor LITitleFontColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor LITextViewFontColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor LITextViewPlaceHolderFontColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor LILoginbuttonTitleFontColor = UIColor.Clear.FromHexString(ButtonBackgroundColor, 1.0f);
		public static readonly UIColor LILoginbuttonTitleFontColorInLeftColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor LIOtherLabeltextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont LILicenceAgreementFont = AppFonts.ProximaNovaRegular(10);
        public static readonly UIFont LIForgotSomethingFont = AppFonts.ProximaNovaRegular(10);
        public static readonly UIFont LITextFieldFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont LILoginbuttonFont = AppFonts.ProximaNovaRegular(18);
        public static readonly UIFont LIOtherLabelsFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont LILoginTextLabelFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIFont LITitleFont = AppFonts.ProximaNovaRegular(16);
        public static readonly string LICrossIcon = smallCrossImage;
        public static readonly string LICrossWhiteIcon = smallCrossWhiteImage;
        public static readonly nfloat LIseparatorBorderWidth = separatorBorderWidth;

        #endregion

        #region -- Settings --

        public static readonly UIColor ASBlueColortext = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor ASCellSelectedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor ASCellTextColor = textBlackColor;
        public static readonly UIColor ASversionNumber = textBlackColor;
        public static readonly string ASstaticVersion = "Version ";
        public static readonly UIColor ASCellSelectedBackgroundColor = CellSelectedbackgroundColor;
        public static readonly UIColor ASBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor ASSectionTitleColor = UIColor.Clear.FromHexString (TextColor, 1.0f);
        public static readonly UIFont ASSectionTitleFont = AppFonts.ProximaNovaRegular(13);
        public static readonly string ASCrossIcon = smallCrossImage;
        public static readonly string SureToLogoutText = sureToLogoutText;
        public static readonly string LogoutText = logoutText;

        #endregion

        #region -- Notification --

        public static readonly UIFont NVcellTextFont = AppFonts.ProximaNovaRegular(16);
        public static readonly UIColor NVcellTextColor = textBlackColor;
        public static readonly UIColor NVbackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor NVCloseBtnBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont NVdescriptionFont = AppFonts.ProximaNovaRegular(14);
        public static readonly UIColor NVdescriptionTextColor = tetLightGrayColor;
        public static readonly string NVCrossIcon = smallCrossImage;
        public static readonly UIColor NVtitleColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont NVtitleLabelFont = AppFonts.ProximaNovaRegular(18);
        public static readonly string NVnotificationtitleText = notificationtitleText;
        public static readonly UIColor NVtitleBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

        #endregion


        #region -- Transportation --

        public static readonly UIFont TSTitleTextFont = AppFonts.ProximaNovaRegular(16);
        public static readonly UIFont TSDetailTextFont = AppFonts.ProximaNovaRegular(12);        
        public static readonly string TStransportationText = transportationText;
        public static readonly UIColor TSsegmentbackColor = SegmentBackgroundColor;
        public static readonly UIColor TSsegmentTitletextColor = SegmentTitletextColor;
		public static readonly UIColor TSsegmentSubTitletextColor = SegmentSubTitletextColor;

        public static readonly UIFont TSsegmentFont = SegmentFont;
        public static readonly UIColor TSsegmentSelectedtabColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor TSsegmentSelectedtabBottomColor = SegmentSelectedtabBottomColor;
        public static readonly UIFont TSmultiLineFont = MultiLineFont;
        public static readonly UIColor TSsegmentSeparatorColor = SegmentSeparatorColor;
        #endregion

        #region --TitleHeaderView-- //HeaderIconAndTitleColor

		public static readonly string THVBrandingImage = "vmware-band.png";
		public static readonly UIColor THVBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor THVTitleColor = UIColor.Clear.FromHexString(SecondaryColor, 1.0f);
		public static readonly UIColor THVUpdatedLabelColor = UIColor.Clear.FromHexString(TextColor, 1.0f);
		public static readonly UIColor THVUpdatedLabelSelectedColor = UIColor.Clear.FromHexString (UpdatingTextColor,1.0f); // 
		public static readonly UIColor THVSearchPlaceholderColor = UIColor.Clear.FromHexString(Layer1SubviewsColor,1.0f);
		public static readonly UIColor THVSearchBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor THVBottomLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);
        public static readonly UIFont THVUpdatedLabelFont = AppFonts.systemItalicFont14;
        public static readonly UIFont THVTitleLabelFont = AppFonts.ProximaNovaRegular(22);

        public static readonly string THVFilterImage = filterImage;
		public static readonly UIColor THVFilterImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVFilterImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVFilterImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont THVFilterImageFont = FontAwesome.Font(26);

        public static readonly string THVCameraImage = cameraImage;
		public static readonly UIColor THVCameraImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVCameraImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVCameraImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVCameraImageFont = FontAwesome.Font(26);

        public static readonly string THVPostImage = twitterImage;
		public static readonly UIColor THVPostImageFontNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVPostImageFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVPostImageFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont THVPostImageFont = FontAwesome.Font(26);

        public static readonly string THVRefreshIcon = refreshImage;
		public static readonly UIColor THVRefreshIconNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVRefreshIconSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVRefreshIconHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVRefreshButtonFont = FontAwesome.Font(26);

        public static readonly string THVShareImage = shareImage;
		public static readonly UIColor THVShareImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVShareImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVShareImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVShareImageButtonFont = FontAwesome.Font(26);

        public static readonly string THVAddImageName = roundPlus;
		public static readonly UIColor THVAddImageNameNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVAddImageNameSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVAddImageNameHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVAddImageNameFont = FontAwesome.Font(26);

        public static readonly string THVSearchImageName = searchImage;
		public static readonly UIColor THVSearchImageNameNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVSearchImageNameSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVSearchImageNameHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVSearchImageNameFont = FontAwesome.Font(26);
		public static readonly UIFont THVSearchText = AppFonts.ProximaNovaRegular (14);

		public static readonly UIColor THVSearchBackgroundColor = UIColor.Clear.FromHexString(Layer1SubviewsColor,1.0f);

        public static readonly string THVSelfieImage = frontCameraImage;
		public static readonly UIColor THVSelfieImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor THVSelfieImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor THVSelfieImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont THVSelfieImageFont = FontAwesome.Font(26);

        #endregion

        #region --Speakers--

        public static readonly UIColor speakerSearchBarColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly string imgFilter = "filter.png";

        #endregion

        #region --LineView--

		public static readonly UIColor LVBackgroundViewColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);

        #endregion

        #region--WebViewController--

        public static string WVCBackImage = "back.png";
        public static string WVCForwardImage = "fwd.png";

        #endregion
        
        #region-- What's Happening Controller
        
        public static readonly UIColor HeaderTextColor = UIColor.Black;
		public static readonly UIColor WHNextUpbackGround = UIColor.Clear.FromHexString(Graybackground, 1.0f);
		public static readonly UIColor WHBackGroundColor = UIColor.Clear.FromHexString(Layer3Color, 1.0f);
		public static readonly UIColor WHScrollViewbackGround = UIColor.Clear.FromHexString (Layer1Color, 1.0f);
        public static readonly UIColor HeaderBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont HeaderFontSize = AppFonts.ProximaNovaRegular(24);
        public static readonly UIFont SectionFontSize = AppFonts.ProximaNovaRegular(22);
        public static readonly UIColor SectionHeaderBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIFont TrackButtonFontSize = AppFonts.ProximaNovaRegular(22);
		public static readonly UIColor WHBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor WHcellBottomLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);
		public static readonly UIColor WHCellHeaderBorderColor = UIColor.Clear.FromHexString(LineColor, 1.0f);
        
        public static nfloat WHtitleHeaderViewHeight = titleHeaderViewHeightVar;
        public static readonly nfloat WHborderForSectionHeaderTopWidhtVar = borderForSectionHeaderTopWidht;
        public static readonly nfloat WHmiddleborderWidth = middleborderVar;
        public static nfloat WHannounceMentCellHeight = announceMentCellHeightVariable;
		public static nfloat WHsectionheaderHeight = SectionheaderHeight;
        public static readonly UIColor pageControlOnColor = pageControlOnColorVariable;
        public static readonly UIColor pageControlOffColor = pageControlOffColorVariable;        
        public static readonly UIColor myScduleSectionHeaderBgColor = myScduleSectionHeaderBgColorVariable;
		public static readonly UIColor MyScduleBgColor = UIColor.Clear.FromHexString(Layer2Color,1.0f);
        public static readonly UIColor whatsHappeningBackGroundColor = whatsHappeningBackGroundColorVariable;
		public static readonly UIColor introBackGroundColor = UIColor.Clear.FromHexString (SectionBackColor, 1.0f);

        public static nfloat trackButtonRowHeight = trackButtonRowHeightVar;
        public static nfloat featuredSessionCellRowHeight = featuredSessionCellRowHeightVar;       
        public static nfloat sectionBottomBorderHeight = sectionBottomBorderHeightVariable;
        public static nfloat sectionheaderTextLeftPadding = sectionheaderTextLeftPaddingVariable;
        public static nfloat myScheduleSectionHeaderHeight = myScheduleSectionHeaderHeightVar;
        public static nfloat rightBorderForMiddleTableWH = RightBorderForMiddleTableInWhatshapp;
        public static int noOfsectionsForWhatsHappenigMiddleScreen = noOfsectionsForWhatsHappenigMiddleScreenVar;
        public static string noSessionCellTextForMySchedule = noSessionCellTextForMyScheduleVariable;
        public static string noSessionCellTextForFeaturedSess = noSessionCellTextForFeaturedSessVariable;
        public static string featuredSessionText = featuredSessionTextVar;
        public static string announcementText = announcementTextVar;
        public static string myScheduleText = myScheduleTxtVariable;
        public static string noInterestCell = noInterestCellText;
        public static string noScheduleCell = noScheduleCellText;
        public static string nextUpheaderText = nextUpHeaderTextvar;
        public static string onNowheaderText = onNowHeaderTextvar;
        public static readonly UIFont introTextFont = AppFonts.ProximaNovaRegular(18);
        public static readonly UIFont FiraDetailTextFont = AppFonts.ProximaNovaRegular(12);       
        const int noOfsectionsForIntroTableVar = 1;
        public static int noOfsectionsForIntroTable = noOfsectionsForIntroTableVar;        
        public static readonly string NextUpTextForNoSession = nextUpTextForNoSession;        
        public static readonly string NoNextUpTextForNoSession = noNextUpTextForNoSession;       
        public static readonly string WhatsHappeningText = whatsHappeningText;
        public static readonly string WHPlusIcon = plusImage;
        public static readonly string WHScheduleIcon = timeImage;//..
		public static readonly UIColor WHPlusIconNormalColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor WHScheduleIconSelectedColor = UIColor.Clear.FromHexString(Layer3Color,1.0f);

        public static readonly UIFont WHPlusIconNameFont = FontAwesome.Font(26);

        #endregion

        #region--Left menu Controller
        public static readonly string FLcheckedBox = checkedBox;
        public static readonly string FLUncheckedBox = UncheckedBox;
		public static readonly UIColor FLSectionBGColor = UIColor.Clear.FromHexString(SectionHeadingbackground,1.0f);
		public static readonly UIColor FLSectiontextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static readonly UIColor FLSectionBorderColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIFont FLSectionTextFont = AppFonts.ProximaNovaRegular(14);

        public static readonly string LMSurveysIcon = surveysImage;
        public static readonly string LMWhatshappeningIcon = powerImage;
        public static readonly string LMSessionIcon = timeImage;
        public static readonly string LMSettingsIcon = settingsImage;
        public static readonly string LMBadgeIcon = badgeImage;
        public static readonly string LMNotificationIcon = notificationImage;
        public static readonly string LMAgendaIcon = agendaImage;
        public static readonly string LMMosconeCenterIcon = mosconeCeonterImage;

        public static readonly string LMTwitterIcon = twitterImage;
		public static readonly UIColor LMTwitterFontNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMTwitterFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMTwitterFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont LMTwitterFont = FontAwesome.Font(30);


        public static readonly string LMNotesIcon = notesImage;
		public static readonly UIColor LMNotesIconFontNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMNotesIconFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMNotesIconFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont LMNotesIconFont = FontAwesome.Font(32);

        public static readonly string LMQRCodeIcon = qrcodeImage;
        public static readonly string LMQRCodeSelectedIcon = qrcodeSelectedImage;
        public static readonly string LMQRCodeHighLightedIcon = qrcodeHighLightedImage;

        public static readonly string LMScheduleIcon = scheduleImage;
		public static readonly UIColor LMScheduleNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMScheduleSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMScheduleHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont LMScheduleIconFont = FontAwesome.Font(26);

        public static readonly string LMPlusIcon = plusImage;
		public static readonly UIColor LMPlusIconNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMPlusIconSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMPlusIconHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont LMPlusIconNameFont = FontAwesome.Font(26);
	
        public static readonly string LMLeftArrowIcon = leftAngle;
		public static readonly UIColor LMLeftArrowIconNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMLeftArrowIconSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMLeftArrowIconHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont LMLeftArrowIconFont = FontAwesome.Font(35);

        public static readonly string LMRightArrowIcon = rightAngle;
		public static readonly UIColor LMRightArrowIconNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMRightArrowIconSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMRightArrowIconHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIFont LMRightArrowIconFont = FontAwesome.Font(35);

        public static readonly string LMRightArrowImage = forwardArrowImage;

        // fonts
        public static readonly UIFont LMBadgeFontSize = FontAwesome.Font(16);
        public static readonly UIFont LMIconFontSize = FontAwesome.Font(21);

        public static readonly UIFont LMZoomIconFontSize = FontAwesome.Font(24);
        public static readonly UIFont LMHeaderFont = AppFonts.ProximaNovaRegular(22);
		public static readonly UIFont LMSectionTitleFont = AppFonts.ProximaNovaSemiBold(14);
        public static readonly UIFont LMSettingFont = FontAwesome.Font(24);
        public static readonly UIFont LMNotificationFont = FontAwesome.Font(20);

        /////// sessions
		public static readonly UIColor segmentSelectedLineColor = UIColor.Clear.FromHexString(LineColor, 1.0f);

        public static readonly UIFont SCSegmentTitleFont = AppFonts.ProximaNovaRegular(11);

        public const string sessionAddImage = "add.png";
        public const string sessionScheduledImage = "schedule.png";        
        
        public static readonly string LMloginTextTitle = loginTitle;
        public static readonly UIColor LMbottomTabViewShadowColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.5f);
        public static readonly UIColor LMbottomTabViewBorderColor = UIColor.FromRGBA(0.0f, 0.0f, 0.0f, 0.5f);
        public static readonly UIColor LMbottomTabViewBackColor = UIColor.Clear.FromHexString(AppTheme.Layer1Color, 1.0f);
        public static readonly string LMTwitterNotConfiguredText = twitterNotConfiguredText;
		public static readonly UIColor LMmenuSectionBackgroundColor = UIColor.Clear.FromHexString (SectionHeadingbackground, 1.0f);
		public static readonly UIColor LMMenuSectionFontColor = UIColor.Clear.FromHexString (TextColor, 1.0f);
		public static readonly UIColor LMButtontextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor LMmenuSectionLabelBackColor = UIColor.Clear;
		public static readonly UIColor LMmenuNormalText = UIColor.Clear.FromHexString(TextColor,0.6f);
        public static readonly UIColor LMmenuNormalCellBackColor = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);
		public static readonly UIColor LMmenuHeaderSeparator = UIColor.Clear.FromHexString (LineColor,1.0f);
        public static readonly string LMloginDetailText = LoginDetailText;
        public static readonly string CancelTextTitle = cancelTextTitle;

        // colors
		public static readonly UIColor LMNotificationNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor LMNotificationSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor LMNotificationHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);

		public static readonly UIColor LMSettingFontNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor LMSettingFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor LMSettingFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);

        public static readonly UIColor LMHeaderBackgroudColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor LMBackgroundColor = UIColor.Clear.FromHexString(Layer1SubviewsColor, 1.0f);

        #endregion

        #endregion

        #region--Theming Views--

        #region---Exhibitor cell
        public static readonly UIColor SEcellBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SEcellSelectedBackColor = CellSelectedbackgroundColor;
        public static readonly UIColor SEcellNameHighlghtdColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SEcellNameBackColor = UIColor.Clear;
        public static readonly UIFont SEcellnameFont = AppFonts.ProximaNovaRegular(16);
        public static readonly UIColor SEcellInitialNameBackColor = UIColor.Clear;
        public static readonly UIFont SEcellInitialnameFont = AppFonts.ProximaNovaRegular(16);
        public static readonly UIColor SEcellBoothBackColor = UIColor.Clear;
        public static readonly UIFont SEcellBoothFont = AppFonts.ProximaNovaRegular(14);
        public static readonly string SEcellBoothHashText = boothHashText;

        #endregion

        #region--Speaker cell for Speakers Controllers
        public static readonly UIColor SPCcellBackColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor SPCcellNameTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        public static readonly UIColor SPCcellNameHighlightedColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SPCcellNameBackColor = UIColor.Clear;
        public static readonly UIFont SPCnameFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor SPCcellInitialNameTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor SPCcellInitialNameHighlghtedColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly UIColor SPCcellInitialNameBackColor = UIColor.Clear;
        public static readonly UIFont SPCinitialNameFont = AppFonts.ProximaNovaRegular(16);
		public static readonly UIColor SPCcomapnyNameTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly UIColor SPCcellComapnyNameHighlghtedColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor SPCcellCompanyNameBackColor = UIColor.Clear;
		public static readonly UIColor SPCcellLineviewColor = UIColor.Clear.FromHexString(LineColor,1.0f);
		public static readonly UIColor SPCcellSectionBGColor = UIColor.Clear.FromHexString(SectionHeadingbackground,1.0f);
        public static readonly UIFont SPCcompanyNameFont = AppFonts.ProximaNovaRegular(14);
        #endregion

        #region--SessionData Cell---
		public static UIColor lblLeftTime = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor lblBottomTime = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor lblSessionName = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor lblRoom = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor SDClblTopTime = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor SDClblTopTimeHightlighted = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static UIColor SDCbtnAddRemove = cellTextColorSelected;
		public static UIColor SDCbtnAddRemoveBGColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor SDCSelectedbtnAddRemoveBGColor = UIColor.Clear.FromHexString(Layer3Color,1.0f);
        public static UIColor SDClblBottomTimeBackground = SegmentSelectedtabBottomColor;
        public static UIColor SDClblSessionNameHighlighted = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor SDClblRoomHighlighted = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        #endregion

        #region--SessionSpeakerCell--
        public static UIColor viewBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static UIColor nameLabel = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor roleLabel = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly UIColor MenuHighlightedText = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor SSCnameLabelBackground = SegmentSelectedtabBottomColor;
        public static UIColor SSCroleLabelBackground = SegmentSelectedtabBottomColor;

        public static readonly string SSCforward = forwardImage;
		public static readonly UIColor SSCForwardNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
        public static readonly UIColor SSCForwardFontSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static readonly UIColor SSCForwardFontHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
        public static UIFont SSCForwardFont = FontAwesome.Font(20);
        #endregion

        #region --SessionTableCell--
        public static UIColor STCbtnAddRemoveBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor STClblSessionNameBackgroundColor = SegmentSelectedtabBottomColor;
        public static UIColor STClblSessionNameHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor STClblSessionTextColor = UIColor.Clear.FromHexString(TextColor, 1.0f);
		public static readonly UIColor STClbldateTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor STClblRoomTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor STClblRoomTimeTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static readonly UIColor STClbldateBackgroundColor = SegmentSelectedtabBottomColor;
        public static readonly UIColor STClbldateHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor STClblRoomHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor STClblRoomBackgroundColor = UIColor.Clear.FromHexString(Layer1Color,1.0f);
        public static readonly UIColor STClblRoomTimeHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static readonly UIColor STClblRoomTimeBackgroundColor = SegmentSelectedtabBottomColor;
        public static readonly UIColor STCCellSelecteBackColor = CellSelectedbackgroundColor;

        #endregion

        #region--SingleRowCell--
		public static UIColor SRCTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        #endregion

        #region--SocialMediaCell--
		public static UIColor LinkTitle = UIColor.Clear.FromHexString(TextColor,1.0f);
        #endregion

        #region--SpeakerDataCell--
        #endregion

        #region--SpeakerSessionsCell--
        #endregion

        #region--TwitterSocialCell--
		public static UIColor titlelabel = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor retweetCountLabel = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor favouriteCount = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor timeLabel = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        #endregion

        #region--VenueCenterCell--
        #endregion

        #region--AgendaCell--
		public static UIColor AClblTopTime = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor AClblLeftTime = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor AClblBottomTime = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor ACTrackColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor AClblSessionName = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor AClblRoom = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        #endregion

        #region--MyScheduleCell--

		public static readonly UIColor MSCellNormalTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static readonly UIColor MSCellSessionNameTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);

        #endregion

        #region--AllSocialFeedCell--
		public static UIColor SFCtitleLabel = UIColor.Clear.FromHexString(TextColor,1.0f);
		public static UIColor SFCcommentCountLabel = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor SFClikeCountLabel = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        #endregion

        #region--AnnouncementsCell--
        public static UIColor AClblSyncIndicatorTextColor = LightGrayTextColor;
        #endregion

        #region--SidePanelCell--
        #endregion

        #region--FeaturedSessionViewExplore--
        #endregion

        #region--FilterViewController--
		public static UIColor btnToggle = UIColor.Clear.FromHexString(SecondaryColor,1.0f);
		public static UIColor FVClblSectionHeader = UIColor.Clear.FromHexString (TextColor,1.0f);
        #endregion

		#region--ImageViewer
		public static UIColor IVHeadingBackground = UIColor.Clear.FromHexString(Layer1Color,1.0f); 
		public static UIColor IVLineViewBackground = UIColor.Clear.FromHexString(LineColor,1.0f); 
		public static readonly string IVcrossImage = crossImage;
		public static readonly UIColor IVcrossImageNormalColor = UIColor.Clear.FromHexString(IconColor, 1.0f);
		public static readonly UIColor IVcrossImageSelectedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static readonly UIColor IVcrossImageHighlightedColor = UIColor.Clear.FromHexString(SelectedIconAndtitleColor, 1.0f);
		public static UIFont IVcrossImageFont = FontAwesome.Font(30);

		#endregion

        #region--FoodAndDrinkCell
        public static UIColor iconImageView = SegmentSeparatorColor;
        #endregion

        #region--HandsOnLabsCell--
		public static UIColor HOLlblTopTime = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
		public static UIColor HOLlblTopTimeHighlighted = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static UIColor lblHandsOnLbasNameHighlighted = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static readonly UIColor ADcellTextColor = UIColor.Clear.FromHexString(TextColor,1.0f);
        #endregion

        #region--HandsOnLabsDetailSpeakerCell--
		public static UIColor HOLDSCBGColor = UIColor.Clear.FromHexString (Layer1Color, 1.0f);
		public static UIColor HOLDSCnameLabelColor = UIColor.Clear.FromHexString(TextColor,1.0f);

        #endregion

        #region--HorizontalCell--
        public static UIColor wholeView = wholeViewColor;

        public static UIColor HClblSessionNameHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor HClblSessionNameBackgroundColor = SegmentSelectedtabBottomColor;
        public static UIColor HClbldateTextColor = LightGrayTextColor;
        public static UIColor HClbldateBackgroundColor = SegmentSelectedtabBottomColor;
        public static UIColor HClbldateHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor HClblRoomTextColor = LightGrayTextColor;
        public static UIColor HClblRoomBackgroundColor = SegmentSelectedtabBottomColor;
        public static UIColor HClblRoomHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor HClblRoomTimeTextColor = LightGrayTextColor;
        public static UIColor HClblRoomTimeBackgroundColor = SegmentSelectedtabBottomColor;
        public static UIColor HClblRoomTimeHighlightedTextColor =UIColor.Clear.FromHexString(Layer1Color, 1.0f) ;

        #endregion

        #region--IntroViewExplore--

        #endregion

        #region--LeftMenuCell--

        #endregion

        #region--MyscheduleCell--
		public static UIColor MSClblTopTime = UIColor.Clear.FromHexString(TextColor,1.0f);
        #endregion

        #region--MyScheduleViewExplorer--
        #endregion

        #region--NextUpViewExplore--
		public static readonly UIColor NUCellBackground = UIColor.Clear.FromHexString(Layer1Color,1.0f);
        #endregion

        #region--NoSessionsCell--
		public static UIColor NSCnameLabel = UIColor.Clear.FromHexString(TextColor, 1.0f);
        public static UIColor NSCviewBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor NSCnameLabelBackgroundColor = SegmentSelectedtabBottomColor;
        #endregion

        #region--NotificationCell--

        #endregion

        #region--PageControl--
        #endregion

        #region--ProgramsCell--
        public static UIColor PCCellBackgroundColor = CellSelectedbackgroundColor;
        #endregion

        #region--SessionCell--

        #endregion

        #region--Horizontal TableView--
        public static readonly UIColor HTTableViewBackground = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        #endregion

        #region--VenueCenterCell--
        public static UIColor VCCContentviewBackgroundColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
        public static UIColor VCCCellBackgroundColor = CellSelectedbackgroundColor;
		public static UIColor VCCnameLabelTextColor = UIColor.Clear.FromHexString(TextColor, 1.0f);
        public static UIColor VCCnameLabelHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);
		public static UIColor VCCinfoLabelTextColor = UIColor.Clear.FromHexString(NavigationTextAndIcons,1.0f);
        public static UIColor VCCinfoLabelHighlightedTextColor = UIColor.Clear.FromHexString(Layer1Color, 1.0f);

        #endregion

        #endregion

        #region--Loader--
        public static readonly UIColor LoaderBackgroundColor = UIColor.Clear.FromHexString(LoaderColor, 1.0f);
        #endregion
    }

    public class AppFonts
    {
        public static readonly UIFont systemItalicFont14 = UIFont.ItalicSystemFontOfSize(14);

		public static NSMutableAttributedString IncreaseLineHeight(string str, UIFont font, UIColor textColor){
			if (str != null && font != null && textColor != null) {
				UIStringAttributes stringAttributes = new UIStringAttributes {
					Font = font,
					ForegroundColor = textColor,
					ParagraphStyle = new NSMutableParagraphStyle () { LineSpacing = font.LineHeight / 4 }
				};
				var AttributedText = new NSMutableAttributedString (str);
				AttributedText.AddAttributes (stringAttributes, new NSRange (0, str.Length));
				return AttributedText;
			} else {
				var AttributedText = new NSMutableAttributedString ("");
				return AttributedText;
			}
		}

        public static UIFont ProximaNovaRegular(nfloat size)
        {
			return UIFont.FromName("ProximaNova-Regular", size);
        }

        public static UIFont ProximaNovaSemiBold(nfloat size)
        {
            return UIFont.FromName("ProximaNova-Semibold", size);
        }

        public static UIFont ProximaNovaRegularItalic(nfloat size)
        {
            return UIFont.FromName("ProximaNova-RegularItalic", size);
        }


    }
}