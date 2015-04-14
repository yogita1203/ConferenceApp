using System;
using UIKit;
using Foundation;
using CoreGraphics;
using CommonLayer;
using ConferenceAppiOS.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;

namespace ConferenceAppiOS
{
    public class TitleHeaderView : UIView
    {

        const string searchPlaceHolder = "Search Sessions";


        //static string refreshTitle = "Checking for updates...";
        public static string refreshTitle = upToDate;
        public const string refreshTitleSelected = "Updating...";
        public const string refreshTitleHighlighted = "Update available. Hit refresh to sync";
        public const string timeStamp = "TimeStamp";
        public const string checkingUpdates = "Checking for updates...";
        public const string upToDate = "Your data is up to date";

		static nfloat leftMargin = 21.0f;

		static nfloat rightMargin = 21.0f;

		static nfloat padding = 21.0f;

		static nfloat addButtonWidth = 44;
		static nfloat searchButtonWidth = 44;
		static nfloat refreshButtonPadding = 10;
		static nfloat filterButtonWidth = 44;
		static nfloat shareButtonWidth = 44;
		static nfloat refreshButtonWidth = 44;

		static nfloat searchTextFieldWidth = 200;
		static nfloat searchTextFieldHeight = 35;

        //public static string previousTitle = String.Empty;

        nfloat gapMargin = 10;

        public Action RefreshButtonClicked;

        public Action<UIButton> FilterButtonClicked;

        public Action ShareButtonClicked;

        public Action AddButtonClicked;

        public Action cameraButtonClicked;

        public Action frontCameraButtonClicked;

        public Action postButtonClicked;

        public Action<UITextField> searchButtonClicked;

        public Action<UITextField> searchFieldClicked;

        //NSObject notification;
        static NSObject dataUpToDateNotification;
        static NSObject checkingUpdateNotification;
        static NSObject startDeltaNotification;
        //static NSObject deltaCompletedNotification;

        public bool showBaseLine;
        bool refreshButtonAnimating;

        UILabel _titleLabel;
        UILabel titleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new UILabel();
                    _titleLabel.TextColor = AppTheme.THVTitleColor;
                    _titleLabel.BackgroundColor = UIColor.Clear;//AppTheme.THVBackgroundColor;
                    _titleLabel.Font = AppTheme.THVTitleLabelFont;
                    AddSubview(_titleLabel);
                }
                return _titleLabel;
            }
        }


        UIButton _refreshTitleButton;
        UIButton refreshTitleButton
        {
            get
            {
                if (_refreshTitleButton == null)
                {
                    _refreshTitleButton = UIButton.FromType(UIButtonType.Custom);
                    _refreshTitleButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _refreshTitleButton.SetTitleColor(AppTheme.THVUpdatedLabelColor, UIControlState.Normal);
                    _refreshTitleButton.SetTitleColor(AppTheme.THVUpdatedLabelSelectedColor, UIControlState.Selected);
                    _refreshTitleButton.SetTitleColor(AppTheme.THVUpdatedLabelSelectedColor, UIControlState.Highlighted);
                    _refreshTitleButton.Font = AppTheme.THVUpdatedLabelFont;
                    refreshTitleButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        startRefresh();
                    };
                    _refreshTitleButton.BackgroundColor = UIColor.Clear;//AppTheme.THVBackgroundColor;

                    _refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                    _refreshTitleButton.SetTitle(refreshTitleSelected, UIControlState.Selected);
                    //_refreshTitleButton.SetTitle(refreshTitleHighlighted, UIControlState.Highlighted);

                    AddSubview(_refreshTitleButton);
                }

                //var dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(timeStamp);
                //if (!string.IsNullOrEmpty(dateTime))
                //    refreshTitle = Helper.ConvertToTimeAgo(Helper.FromDateString(dateTime));
                //else
                //{
                //    if (!String.IsNullOrWhiteSpace(previousTitle))
                //    {
                //        refreshTitle = previousTitle;
                //    }
                //}
                //_refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                //_refreshTitleButton.SetTitle(refreshTitleSelected, UIControlState.Selected);
                //_refreshTitleButton.SetTitle(refreshTitleHighlighted, UIControlState.Highlighted);

                return _refreshTitleButton;
            }
        }

        private void UpdateSyncState()
        {
            refreshTitleButton.Selected = false;
            refreshTitleButton.Highlighted = false;
            refreshButton.Selected = false;
            refreshButton.Highlighted = false;

            if (AppSettings.SyncState == refreshTitleSelected)
            {
                refreshTitleButton.Selected = true;
                refreshButton.Selected = true;
            }
            else if (AppSettings.SyncState == upToDate)
            {
                var dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(timeStamp);
                if (!string.IsNullOrEmpty(dateTime))
                {
                    AppSettings.SyncState = Helper.ConvertToTimeAgo(Helper.FromDateString(dateTime));
                    refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                }
                else
                {
                    refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                }
            }
            else
            {
                refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
            }
        }

        UIButton _refreshButton;
        UIButton refreshButton
        {
            get
            {
                if (_refreshButton == null)
                {
                    _refreshButton = UIButton.FromType(UIButtonType.Custom);
                    _refreshButton.SetTitle(AppTheme.THVRefreshIcon, UIControlState.Normal);
                    _refreshButton.SetTitle(AppTheme.THVRefreshIcon, UIControlState.Selected);
                    _refreshButton.SetTitle(AppTheme.THVRefreshIcon, UIControlState.Highlighted);
                    _refreshButton.SetTitleColor(AppTheme.THVRefreshIconNormalColor, UIControlState.Normal);
                    _refreshButton.SetTitleColor(AppTheme.THVRefreshIconSelectedColor, UIControlState.Selected);
                    _refreshButton.SetTitleColor(AppTheme.THVRefreshIconHighlightedColor, UIControlState.Highlighted);
                    _refreshButton.Font = AppTheme.THVRefreshButtonFont;
                    _refreshButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    _refreshButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    _refreshButton.TitleEdgeInsets = new UIEdgeInsets(2, 2, 0, 0);
                    _refreshButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        startRefresh();
                    };
                    _refreshButton.BackgroundColor = AppTheme.ClearColor;

                    AddSubview(_refreshButton);
                }
                return _refreshButton;
            }
        }
        bool refreshBool;


        UIButton _cameraButton;
        UIButton cameraButton
        {
            get
            {
                if (_cameraButton == null)
                {
                    _cameraButton = UIButton.FromType(UIButtonType.Custom);
                    _cameraButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _cameraButton.SetTitle(AppTheme.THVCameraImage, UIControlState.Normal);
                    _cameraButton.SetTitle(AppTheme.THVCameraImage, UIControlState.Selected);
                    _cameraButton.SetTitle(AppTheme.THVCameraImage, UIControlState.Highlighted);
                    _cameraButton.SetTitleColor(AppTheme.THVCameraImageNormalColor, UIControlState.Normal);
                    _cameraButton.SetTitleColor(AppTheme.THVCameraImageSelectedColor, UIControlState.Selected);
                    _cameraButton.SetTitleColor(AppTheme.THVCameraImageHighlightedColor, UIControlState.Highlighted);
                    _cameraButton.Font = AppTheme.THVCameraImageFont;
                    _cameraButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (cameraButtonClicked != null)
                        {
                            cameraButtonClicked();
                        }
                    };
                    _cameraButton.BackgroundColor = AppTheme.THVBackgroundColor;

                    AddSubview(_cameraButton);
                }
                return _cameraButton;
            }
        }
        bool cameraBool;


        UIButton _frontCamera;
        UIButton frontCamera
        {
            get
            {
                if (_frontCamera == null)
                {
                    _frontCamera = UIButton.FromType(UIButtonType.Custom);
                    _frontCamera.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _frontCamera.SetTitle(AppTheme.THVSelfieImage, UIControlState.Normal);
                    _frontCamera.SetTitle(AppTheme.THVSelfieImage, UIControlState.Selected);
                    _frontCamera.SetTitle(AppTheme.THVSelfieImage, UIControlState.Highlighted);
                    _frontCamera.SetTitleColor(AppTheme.THVSelfieImageNormalColor, UIControlState.Normal);
                    _frontCamera.SetTitleColor(AppTheme.THVSelfieImageSelectedColor, UIControlState.Selected);
                    _frontCamera.SetTitleColor(AppTheme.THVSelfieImageHighlightedColor, UIControlState.Highlighted);
                    _frontCamera.Font = AppTheme.THVSelfieImageFont;
                    _frontCamera.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (frontCameraButtonClicked != null)
                        {
                            frontCameraButtonClicked();
                        }
                    };
                    _frontCamera.BackgroundColor = AppTheme.THVBackgroundColor;

                    AddSubview(_frontCamera);
                }
                return _frontCamera;
            }
        }
        bool frontCameraBool;

        UIButton _postButton;
        UIButton postButton
        {
            get
            {
                if (_postButton == null)
                {
                    _postButton = UIButton.FromType(UIButtonType.Custom);
                    _postButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _postButton.SetTitle(AppTheme.THVPostImage, UIControlState.Normal);
                    _postButton.SetTitle(AppTheme.THVPostImage, UIControlState.Selected);
                    _postButton.SetTitle(AppTheme.THVPostImage, UIControlState.Highlighted);
                    _postButton.SetTitleColor(AppTheme.THVPostImageFontNormalColor, UIControlState.Normal);
                    _postButton.SetTitleColor(AppTheme.THVPostImageFontSelectedColor, UIControlState.Selected);
                    _postButton.SetTitleColor(AppTheme.THVPostImageFontHighlightedColor, UIControlState.Highlighted);
                    _postButton.Font = AppTheme.THVSelfieImageFont;
                    _postButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (postButtonClicked != null)
                        {
                            postButtonClicked();
                        }
                    };
                    _postButton.BackgroundColor = AppTheme.THVBackgroundColor;
                    AddSubview(_postButton);
                }
                return _postButton;
            }
        }
        bool postbuttonBool;

        UIButton _filterButton;
        UIButton filterButton
        {
            get
            {
                if (_filterButton == null)
                {
                    _filterButton = UIButton.FromType(UIButtonType.Custom);
                    _filterButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _filterButton.SetTitle(AppTheme.THVFilterImage, UIControlState.Normal);
                    _filterButton.SetTitle(AppTheme.THVFilterImage, UIControlState.Selected);
                    _filterButton.SetTitle(AppTheme.THVFilterImage, UIControlState.Highlighted);
                    _filterButton.SetTitleColor(AppTheme.THVFilterImageNormalColor, UIControlState.Normal);
                    _filterButton.SetTitleColor(AppTheme.THVFilterImageSelectedColor, UIControlState.Selected);
                    _filterButton.SetTitleColor(AppTheme.THVFilterImageHighlightedColor, UIControlState.Highlighted);
                    _filterButton.Font = AppTheme.THVFilterImageFont;
                    _filterButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (FilterButtonClicked != null)
                        {
                            FilterButtonClicked(_filterButton);
                        }
                    };
                    _filterButton.BackgroundColor = AppTheme.THVBackgroundColor;
                    AddSubview(_filterButton);
                }
                return _filterButton;
            }
        }
        bool filterBool;

        UIButton _shareButton;
        UIButton shareButton
        {
            get
            {
                if (_shareButton == null)
                {
                    _shareButton = UIButton.FromType(UIButtonType.Custom);
                    _shareButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _shareButton.SetTitle(AppTheme.THVShareImage, UIControlState.Normal);
                    _shareButton.SetTitle(AppTheme.THVShareImage, UIControlState.Selected);
                    _shareButton.SetTitle(AppTheme.THVShareImage, UIControlState.Highlighted);
                    _shareButton.SetTitleColor(AppTheme.THVShareImageNormalColor, UIControlState.Normal);
                    _shareButton.SetTitleColor(AppTheme.THVShareImageSelectedColor, UIControlState.Selected);
                    _shareButton.SetTitleColor(AppTheme.THVShareImageHighlightedColor, UIControlState.Highlighted);
                    _shareButton.Font = AppTheme.THVAddImageNameFont;
                    _shareButton.BackgroundColor = AppTheme.THVBackgroundColor;
                    _shareButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (ShareButtonClicked != null)
                        {
                            ShareButtonClicked();
                        }
                    };
                    AddSubview(_shareButton);
                }
                return _shareButton;
            }
        }
        bool shareBool;


        UIButton _addButton;
        UIButton addButton
        {
            get
            {
                if (_addButton == null)
                {
                    _addButton = UIButton.FromType(UIButtonType.Custom);
                    _addButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _addButton.SetTitle(AppTheme.THVAddImageName, UIControlState.Normal);
                    _addButton.SetTitle(AppTheme.THVAddImageName, UIControlState.Selected);
                    _addButton.SetTitle(AppTheme.THVAddImageName, UIControlState.Highlighted);
                    _addButton.SetTitleColor(AppTheme.THVAddImageNameNormalColor, UIControlState.Normal);
                    _addButton.SetTitleColor(AppTheme.THVAddImageNameSelectedColor, UIControlState.Selected);
                    _addButton.SetTitleColor(AppTheme.THVAddImageNameHighlightedColor, UIControlState.Highlighted);
                    _addButton.Font = AppTheme.THVAddImageNameFont;
                    _addButton.BackgroundColor = AppTheme.THVBackgroundColor;
                    _addButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        if (AddButtonClicked != null)
                        {
                            AddButtonClicked();
                        }
                    };

                    AddSubview(_addButton);
                }
                return _addButton;
            }
        }
        bool addBool;

        UITextField _searchField;
        public UITextField searchField
        {
            get
            {
                if (_searchField == null)
                {
                    _searchField = new UITextField();
                    _searchField.Placeholder = "Search";
                    _searchField.TextAlignment = UITextAlignment.Center;
                    _searchField.BackgroundColor = AppTheme.THVSearchBackgroundColor;
                    _searchField.Layer.CornerRadius = 5.0f;
                    _searchField.Layer.MasksToBounds = true;

                    _searchField.Font = AppTheme.THVSearchText;
                    UIView paddingViewLeft = new UIView(new CGRect(0, 0, 5, 20));
                    _searchField.LeftView = paddingViewLeft;
                    _searchField.LeftViewMode = UITextFieldViewMode.Always;
                    UIView paddingViewRight = new UIView(new CGRect(0, 0, 5, 20));
                    _searchField.RightView = paddingViewRight;
                    _searchField.RightViewMode = UITextFieldViewMode.Always;
                    _searchField.TextColor = UIColor.Clear.FromHexString(AppTheme.TextColor, 1.0f);
                    _searchField.ShouldReturn += (textField) =>
                    {
                        textField.ResignFirstResponder();
                        if (searchFieldClicked != null)
                        {
                            searchFieldClicked(_searchField);
                        }
                        return true;
                    };
                    AddSubview(_searchField);
                }
                return _searchField;
            }
        }

        UIView _lineView;
        UIView lineView
        {
            get
            {
                if (_lineView == null)
                {
                    _lineView = new UIView();
                    _lineView.BackgroundColor = AppTheme.THVSearchBorderColor;
                    AddSubview(_lineView);
                }
                return _lineView;
            }
        }

        UIImageView _cornerBrandingImage;
        UIImageView cornerBrandingImage
        {
            get
            {
                if (_cornerBrandingImage == null)
                {
                    _cornerBrandingImage = new UIImageView();
                    _cornerBrandingImage.BackgroundColor = AppTheme.ClearColor;
                    _cornerBrandingImage.Image = UIImage.FromFile(AppTheme.THVBrandingImage);
                    AddSubview(_cornerBrandingImage);
                }
                return _cornerBrandingImage;
            }
        }

        UIButton _searchButton;
        UIButton searchButton
        {
            get
            {
                if (_searchButton == null)
                {
                    _searchButton = UIButton.FromType(UIButtonType.Custom);
                    _searchButton.ContentMode = UIViewContentMode.ScaleAspectFit;
                    _searchButton.SetTitle(AppTheme.THVSearchImageName, UIControlState.Normal);
                    _searchButton.SetTitle(AppTheme.THVSearchImageName, UIControlState.Selected);
                    _searchButton.SetTitle(AppTheme.THVSearchImageName, UIControlState.Highlighted);
                    _searchButton.SetTitleColor(AppTheme.THVSearchImageNameNormalColor, UIControlState.Normal);
                    _searchButton.SetTitleColor(AppTheme.THVSearchImageNameSelectedColor, UIControlState.Selected);
                    _searchButton.SetTitleColor(AppTheme.THVSearchImageNameHighlightedColor, UIControlState.Highlighted);
                    _searchButton.Font = AppTheme.THVSearchImageNameFont;
                    _searchButton.BackgroundColor = AppTheme.THVBackgroundColor;
                    _searchButton.TouchUpInside += (object sender, EventArgs e) =>
                    {
                        searchField.BecomeFirstResponder();
                        if (searchButtonClicked != null)
                        {
                            searchButtonClicked(searchField);
                        }
                    };
                    AddSubview(_searchButton);
                }
                return _searchButton;
            }
        }

        bool searchBool;

        public TitleHeaderView(string title, bool refresh, bool filter, bool search, bool share, bool ad, bool camera, bool front, bool post)
        {
            showBaseLine = true;

            AddSubview(cornerBrandingImage);

            if (title.ToLower() == "interests" || title.ToLower() == "speakers")
            {
                titleLabel.Text = title.ToUpper();
            }
            else
            {
                if (!String.IsNullOrEmpty(LeftMenuController.SelectedMenuTitle))
                    title = LeftMenuController.SelectedMenuTitle.ToUpper();
            }

            titleLabel.Text = title.ToUpper();
            refreshBool = refresh;
            filterBool = filter;
            searchBool = search;
            shareBool = share;
            addBool = ad;

            BackgroundColor = AppTheme.THVBackgroundColor;

            if (UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Rear))
            {
                cameraBool = camera;
            }
            else
            {
                cameraBool = false;
            }

            if (UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Front))
            {
                frontCameraBool = front;
            }
            else
            {
                frontCameraBool = false;
            }
            postbuttonBool = post;
            //notification = NSNotificationCenter.DefaultCenter.AddObserver(BaseViewController.LATEST_DATA_AVAILABLE, (nsn) =>
            //{
            //    previousTitle = refreshTitleHighlighted;
            //    refreshTitle = String.Empty;
            //    refreshTitleButton.Selected = false;
            //    refreshButton.Selected = false;
            //    refreshButton.Highlighted = true;
            //    refreshTitleButton.Highlighted = true;
            //    LayoutSubviews();
            //});

            UpdateSyncState();

            if (dataUpToDateNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(dataUpToDateNotification);
            }
			dataUpToDateNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.DATA_UP_TO_DATE), (nsn) =>
            {
                refreshTitleButton.Selected = false;
                refreshButton.Selected = false;
                refreshButton.Highlighted = false;
                refreshTitleButton.Highlighted = false;


                var dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(timeStamp);
                if (!string.IsNullOrEmpty(dateTime))
                {
                    AppSettings.SyncState = Helper.ConvertToTimeAgo(Helper.FromDateString(dateTime));

                }
                else
                {
                    AppSettings.SyncState = upToDate;
                    refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                }


                refreshTitleButton.SetTitle(AppSettings.SyncState, UIControlState.Normal);
                refreshTitle = AppSettings.SyncState;
                //previousTitle = refreshTitle;
                LayoutSubviews();
            });

            if (checkingUpdateNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(checkingUpdateNotification);
            }
			checkingUpdateNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.CHECKING_UPDATE), (nsn) =>
            {
                refreshTitleButton.Selected = false;
                refreshButton.Selected = false;
                refreshButton.Highlighted = false;
                refreshTitleButton.Highlighted = false;
                refreshTitle = checkingUpdates;
                refreshTitleButton.SetTitle(checkingUpdates, UIControlState.Normal);
                LayoutSubviews();
            });

            if (startDeltaNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(startDeltaNotification);
            }
			startDeltaNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(BaseViewController.StartDelta), (nsn) =>
            {
                startRefresh();
            });
        }


        public void startRefresh()
        {

            var dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(timeStamp);

            if (RefreshButtonClicked != null)
            {
                RefreshButtonClicked();
            }
            refreshTitleButton.Highlighted = false;
            refreshButton.Highlighted = false;
            refreshTitleButton.Selected = true;
            refreshButton.Selected = true;
            startSpin();
            LayoutSubviews();
            NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.DELTA_STARTED, null);
            DataManager.deltaAllAsync(AppDelegate.Connection, (res, updatedUids) =>
                {
                    try
                    {
                        if (res == null)
                        {
                            refreshTitle = Helper.ConvertToTimeAgo(DateTime.Now);
                            AppSettings.SyncState = refreshTitle;
                            NSString uids = null;
                            if (updatedUids != null)
                                uids = new NSString(String.Join("|", updatedUids));

                            InvokeOnMainThread(() =>
                                {
                                    NSUserDefaults.StandardUserDefaults.SetString(Helper.ToDateString(DateTime.Now), timeStamp);
                                    refreshTitleButton.SetTitle(refreshTitle, UIControlState.Normal);
                                    refreshTitleButton.Selected = false;
                                    refreshButton.Selected = false;
                                    refreshTitleButton.Highlighted = false;
                                    refreshButton.Highlighted = false;
                                    stopSpin();
                                    LayoutSubviews();
                                    NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.DELTA_COMPLETED, uids);
                                    Task.Delay(3000).ContinueWith(t =>
                                        {
                                            InvokeOnMainThread(() =>
                                                {
                                                    dateTime = NSUserDefaults.StandardUserDefaults.StringForKey(timeStamp);
                                                    if (!string.IsNullOrEmpty(dateTime))
                                                    {
                                                        refreshTitle = Helper.ConvertToTimeAgo(Helper.FromDateString(dateTime));
                                                        AppSettings.SyncState = refreshTitle;
                                                        refreshTitleButton.SetTitle(refreshTitle, UIControlState.Normal);
                                                        NSNotificationCenter.DefaultCenter.PostNotificationName(BaseViewController.DATA_UP_TO_DATE, null);
                                                        LayoutSubviews();
                                                    }
                                                });
                                        });
                                });

                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                stopSpin();
                                AppSettings.SyncState = upToDate;
                                UpdateSyncState();
                            });
                        }
                    }
                    catch
                    { }
                }, AppSettings.ApplicationUser);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();


            cornerBrandingImage.SizeToFit();
            cornerBrandingImage.Frame = new CGRect(Frame.Width - 90.5f, 20, 90.5f, Frame.Height - 20);

            nfloat xPosition = Frame.Width - cornerBrandingImage.Frame.Width;
            nfloat yPosition = 20;
            nfloat height = Frame.Height - yPosition;
            titleLabel.SizeToFit();
            gapMargin = 0;

            if (postbuttonBool && (postButton != null))
            {
                xPosition = xPosition - addButtonWidth;
                postButton.Frame = new CGRect(xPosition, yPosition, addButtonWidth, height);
                gapMargin = 10;
            }

            if (cameraBool && (cameraButton != null))
            {
                xPosition = xPosition - addButtonWidth;
                cameraButton.Frame = new CGRect(xPosition, yPosition, addButtonWidth, height);
                gapMargin = 10;
            }


            if (frontCameraBool && (frontCamera != null))
            {
                xPosition = xPosition - addButtonWidth;
                frontCamera.Frame = new CGRect(xPosition, yPosition, addButtonWidth, height);
                gapMargin = 10;
            }

            if (addBool && (addButton != null))
            {
                xPosition = xPosition - addButtonWidth;
                addButton.Frame = new CGRect(xPosition, yPosition, addButtonWidth, height);
                gapMargin = 10;
            }

            if (searchBool && (searchButton != null))
            {
                xPosition = xPosition - (searchButtonWidth + gapMargin);
                searchButton.Frame = new CGRect(xPosition, yPosition, searchButtonWidth, height);

                xPosition = xPosition - (searchTextFieldWidth);
                yPosition = height / 2 - searchTextFieldHeight / 2;
                searchField.Frame = new CGRect(xPosition, yPosition + 20, searchTextFieldWidth, searchTextFieldHeight);
                yPosition = 20;
                gapMargin = 10;
            }

            if (filterBool && (filterButton != null))
            {
                xPosition = xPosition - (filterButtonWidth + gapMargin);
                filterButton.Frame = new CGRect(xPosition, yPosition, filterButtonWidth, height);
                gapMargin = 10;

            }

            if (shareBool && (shareButton != null))
            {
                xPosition = xPosition - (shareButtonWidth + gapMargin);
                shareButton.Frame = new CGRect(xPosition, yPosition, shareButtonWidth, height);
                gapMargin = 10;
            }

            if (refreshBool && (refreshButton != null))
            {

                gapMargin = 0;
                refreshTitleButton.SizeToFit();
                refreshButton.SizeToFit();
                xPosition = xPosition - (height + gapMargin);
				refreshButton.Frame = new CGRect(xPosition, yPosition, height, height);

                xPosition = xPosition - (refreshTitleButton.Frame.Width);


				refreshTitleButton.Frame = new CGRect(xPosition, yPosition, refreshTitleButton.Frame.Width, height);
                gapMargin = 10;

            }

            if (titleLabel.Text.Length > 0)
            {

				titleLabel.Frame = new CGRect(leftMargin, yPosition, xPosition - (leftMargin + gapMargin + 20), height);
            }


            if (showBaseLine)
				lineView.Frame = new CGRect(0, Frame.Height - 1, Frame.Width, 1);
            else
                lineView.Frame = new CGRect(0, 0, 0, 0);

        }



        void spinWithOptions(UIViewAnimationOptions options)
        {
            // this spin completes 360 degrees every 2 seconds
            UIView.Animate(0.25, 0.0, options, () =>
            {
                refreshButton.Transform = CGAffineTransform.Rotate(refreshButton.Transform, (nfloat)Math.PI/2.0f);
            }, () =>
            {
                if (refreshButtonAnimating)
                {
                    // if flag still set, keep spinning with constant speed
                    spinWithOptions(UIViewAnimationOptions.CurveLinear);
                }
                else if (options != UIViewAnimationOptions.CurveEaseOut)
                {
                    // one last spin, with deceleration
                    spinWithOptions(UIViewAnimationOptions.CurveEaseOut);
                }
            });
        }

        void startSpin()
        {
            if (!refreshButtonAnimating)
            {
                refreshButtonAnimating = true;
                spinWithOptions(UIViewAnimationOptions.CurveEaseIn);
            }
        }

        void stopSpin()
        {
            // set the flag to stop spinning after one last 90 degree increment
            refreshButtonAnimating = false;
            refreshButton.Selected = false;
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (notification != null)
            //{
            //    NSNotificationCenter.DefaultCenter.RemoveObserver(notification);
            //}

            //if (dataUpToDateNotification != null)
            //{
            //    NSNotificationCenter.DefaultCenter.RemoveObserver(dataUpToDateNotification);
            //    dataUpToDateNotification = null;
            //}

            //if (checkingUpdateNotification != null)
            //{
            //    NSNotificationCenter.DefaultCenter.RemoveObserver(checkingUpdateNotification);
            //    checkingUpdateNotification = null;
            //}

            //if (startDeltaNotification != null)
            //{
            //    NSNotificationCenter.DefaultCenter.RemoveObserver(startDeltaNotification);
            //    startDeltaNotification = null;
            //}
        }
    }
}