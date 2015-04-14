using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using CommonLayer;
using CommonLayer.Entities.Built;
using CoreGraphics;
using Foundation;
using UIKit;
using SDWebImage;
using ConferenceAppiOS;
using ConferenceAppiOS.Helpers;
using ConferenceAppiOS.Views;

namespace ConferenceAppiOS
{
    public class VenueImageController : BaseViewController
    {

        #region--Constants--
        string mapsTitle = AppTheme.VImosconeCenterText;
		static nfloat headerViewLeftMargin = 0;
		static nfloat headerHeight = 64;
		static nfloat mapsTableTopBorderWidth = 1;
		static nfloat mapsTableViewXpadding = 0;
		public static nfloat mapsTableViewWidthLeftClosed = 350;
		static nfloat mapsTableViewWidthLeftOpened = 350;
		static nfloat mapsTableViewYPadding = 0;
		static nfloat mapsTableViewFooterXPadding = 20;
		static nfloat mapsTableViewFooterYPadding = 0;
		static nfloat mapsTableViewFooterWidthPadding = 0;
		static nfloat mapsTableViewFooterHeightPadding = 1;
		static nfloat mapsTableVwRightBorderWidth = 1;
		static nfloat mapsTableViewHeaderHeight = headerHeight + 1;
		static nfloat imageBackgroundY = 15; static nfloat imageBackgroundHeight = 330;
		static nfloat imageXPadding = 15; static nfloat imageyPadding = 30; static nfloat imageWidth = 300; static nfloat imageHeight = 300;
		static nfloat lblNameXPadding = 20; static nfloat lblNameyPadding = 10; static nfloat lblNameRightPadding = 20; static nfloat lblNameHeight = 30;
		static nfloat lblDescriptionXPadding = 20; static nfloat lblDescriptionyPadding = 20; static nfloat lblDescriptionRightPadding = 20; static nfloat lblDescriptionHeight = 0; static nfloat bottomPaddingForloading = 100;
		static nfloat lblAddressXPadding = 20; static nfloat lblAddressyPadding = 20; static nfloat lblAddressRightPadding = 30; static nfloat lblAddressHeight = 0;
		static nfloat rightTopHeaderHeight = 54;
		static nfloat MaxLabelHeight = 300;
		static nfloat mapsheaderViewXPadding = 0;
		static nfloat mapsheaderViewYPadding = 0;

		static nfloat mapstitleheaderViewYPadding = 0;

		static nfloat borderTopXPadding = 0;

		static nfloat mapsTableViewXPadding = 0;

		static nfloat imgViewXPadding = 0;
		static nfloat imgViewYPadding = 0;
		static nfloat imgViewWidthPadding = 600;
		static nfloat imgViewHeightPadding = 600;

		static nfloat programBottomBorderWidthPadding = 10;
		static nfloat programBottomBorderHeightPadding = 1;
        #endregion

        #region--Views--
        UIScrollView scrollView;
		CGRect rectBase;
        LoadingOverlay loadingOverlay;
        UITableView mapsTableView;
        UIView mapsheaderView;
        TitleHeaderView mapstitleheaderView;
        UIView borderTop;
        UIActivityIndicatorView activityIndicator;
        UIView mapstableRightVerticalLine;
        UIView wholeRightPanelWithImageAndDdescription;

        UILabel _lblVenueinfo;
        UILabel lblVenueinfo
        {
            get
            {
                if (_lblVenueinfo == null)
                {
                    _lblVenueinfo = new UILabel();
                    _lblVenueinfo.Frame = new CGRect(20, 15, 200, 30);
                    _lblVenueinfo.Font = AppTheme.VIvenueInfoFont;
                    _lblVenueinfo.TextColor = AppTheme.VIvenueInfoTextColor;
                    _lblVenueinfo.Lines = 0;
                    _lblVenueinfo.LineBreakMode = UILineBreakMode.WordWrap;
                    _lblVenueinfo.Text = "Test";
                }
                return _lblVenueinfo;
            }
        }

        UILabel _lblVenuename;
        UILabel lblVenuename
        {
            get
            {
                if (_lblVenuename == null)
                {
                    _lblVenuename = new UILabel();
                    _lblVenuename.Frame = new CGRect(20, 15, 200, 30);
                    _lblVenuename.Font = AppTheme.VIvenueNameFont;
                    _lblVenuename.TextColor = AppTheme.VIvenuenameTextColor;
                }
                return _lblVenuename;
            }
        }
        UILabel _lblDescription;
        UILabel lblDescription
        {
            get
            {
                if (_lblDescription == null)
                {
                    _lblDescription = new UILabel();
                    _lblDescription.Frame = new CGRect(20, 15, 200, 30);
                    _lblDescription.Font = AppTheme.VIvenueDescFont;
                    _lblDescription.Lines = 0;
                    _lblDescription.BackgroundColor = AppTheme.VIvenueDescBackColor;
                    _lblDescription.TextColor = AppTheme.VIvenueDescTextColor;
                }
                return _lblDescription;
            }
        }
        UILabel _lblAddress;
        UILabel lblAddress
        {
            get
            {
                if (_lblAddress == null)
                {
                    _lblAddress = new UILabel();
                    _lblAddress.Frame = new CGRect(20, 15, 200, 30);
                    _lblAddress.Font = AppTheme.VIvenueAddressFont;
                    _lblAddress.Lines = 0;
                    _lblAddress.TextColor = AppTheme.VIvenueAddressTextColor;
                }
                return _lblAddress;
            }
        }
        UIImageView venueImageView; UIView venueImageViewBackGroundView;

        UIButton _locationButton;
        UIButton locationButton
        {
            get
            {
                if (_locationButton == null)
                {
                    _locationButton = new UIButton();
                    _locationButton.Layer.CornerRadius = 10.0f;
                    _locationButton.Layer.BorderColor = AppTheme.VIvenueLocationBtnBorderColor.CGColor;
                    _locationButton.Layer.BorderWidth = 1;
                }

                return _locationButton;
            }
        }

        UIButton _locationImage;

        UIButton locationImage
        {
            get
            {
                if (_locationImage == null)
                {
                    _locationImage = new UIButton();
                }
                return _locationImage;
            }
        }


        UILabel _locationName;
        UILabel locationName
        {
            get
            {
                if (_locationName == null)
                {
                    _locationName = new UILabel();
					_locationName.TextColor = UIColor.Clear.FromHexString(AppTheme.TextColor,1.0f);
                    _locationName.Lines = 0;
                    _locationName.LineBreakMode = UILineBreakMode.WordWrap;
                }
                return _locationName;

            }

        }

        #endregion

        BuiltVenue _builtVenue;
		List<BuiltVenue> lstVenues; static nfloat progrmLeftInset = 20;

        public VenueImageController(CGRect rect)
        {
            rectBase = rect;
            View.Frame = rectBase;
        }

        void ShowOverlay(UIView view)
        {
            var bounds = view.Bounds;
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                bounds.Size = new CGSize(bounds.Size.Width, bounds.Size.Height - (imageHeight + bottomPaddingForloading));
            }
            this.loadingOverlay = new LoadingOverlay(bounds);
            view.Add(this.loadingOverlay);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.Frame = rectBase;

            scrollView = new UIScrollView();
            scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
            scrollView.BackgroundColor = AppTheme.DHnewsdetailsbackColor;

            setMapsHeaderView();
            mapsTableView = new UITableView();

            getVenues(res =>
            {
                lstVenues = res.OrderBy(p => p.name.ToUpper()).ToList();

                InvokeOnMainThread(() =>
                {
                    mapsTableView.Source = new MosconeDataSource(this, lstVenues);
                    setRightViewData(lstVenues.ElementAt(0));

                    mapsTableView.ReloadData();

                    mapsTableView.SelectRow(NSIndexPath.FromRowSection(0, 0), true, UITableViewScrollPosition.None);

                });
            });


            mapsTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            UIView programBottomBorder = new UIView(new CGRect(mapsTableViewFooterXPadding, mapsTableViewFooterYPadding, View.Frame.Width - programBottomBorderWidthPadding, programBottomBorderHeightPadding));
            programBottomBorder.BackgroundColor = mapsTableView.SeparatorColor;
            mapsTableView.TableFooterView = programBottomBorder;
            mapstableRightVerticalLine = new UIView();
            mapstableRightVerticalLine.BackgroundColor = AppTheme.VImiddleBorderColor;
            mapsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
            mapsTableView.SeparatorInset = new UIEdgeInsets(0, progrmLeftInset, 0, 0);
            wholeRightPanelWithImageAndDdescription = new UIView();
            venueImageView = new UIImageView();
            venueImageView.BackgroundColor = AppTheme.DHImageCoverPhotoBackColor;
            venueImageView.Image = UIImage.FromFile(AppTheme.EXLogoPlaceholder);
            venueImageView.ContentMode = UIViewContentMode.Center;
            venueImageView.Layer.MasksToBounds = true;
            venueImageViewBackGroundView = new UIView();
           
            venueImageViewBackGroundView.BackgroundColor = AppTheme.VIvenueImageViewBackGroundView.ColorWithAlpha(0.1f);
            venueImageViewBackGroundView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            lblVenuename.Frame = new CGRect(lblNameXPadding, lblNameyPadding, View.Frame.Width - lblNameRightPadding, lblNameHeight);
            lblVenuename.LineBreakMode = UILineBreakMode.WordWrap;


            activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			activityIndicator.BackgroundColor = UIColor.White;
            venueImageViewBackGroundView.AddSubview(activityIndicator);
            wholeRightPanelWithImageAndDdescription.AddSubviews(venueImageView, lblAddress, lblDescription);
            scrollView.AddSubviews(wholeRightPanelWithImageAndDdescription, lblVenueinfo);

            #region--locationButton--
            locationButton.AddSubviews(locationImage, locationName);
            scrollView.AddSubview(locationButton);
            View.AddSubviews(scrollView,mapsheaderView, mapsTableView,mapstableRightVerticalLine);
            #endregion

            ViewWillLayoutSubviews();

            var extras = new Dictionary<string, object> { { "venue_name", AppSettings.venue_name } };
            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.venue, Helper.ToDateString(DateTime.Now), extras);
        }

        void setMapsHeaderView()
        {
            mapsheaderView = new UIView();
            mapsheaderView.AutosizesSubviews = true;
            mapstitleheaderView = new TitleHeaderView(mapsTitle, true, false, false, false, false, false, false, false);
			mapsheaderView.BackgroundColor = mapstitleheaderView.BackgroundColor;
            mapstitleheaderView.Frame = new CGRect(headerViewLeftMargin, 0, View.Frame.Width, headerHeight);
            borderTop = new UIView();
            borderTop.Frame = new CGRect(0, mapstitleheaderView.Frame.Bottom, View.Frame.Width, mapsTableTopBorderWidth);
            borderTop.BackgroundColor = AppTheme.VIvenueTitleBottomBorderColor;
            mapsheaderView.AddSubviews(mapstitleheaderView, borderTop);
        }

        public void setRightViewData(BuiltVenue builtVenue)
        {
            _builtVenue = builtVenue;
            var viewArr=View.Subviews;
            foreach (var item in viewArr)
            {
                if (item is UIView)
                {
                    foreach (var item2 in item.Subviews)
                    {
                        if (item2 is UIImageView)
                        {
                            venueImageView.ContentMode = UIViewContentMode.Center;
                            break;
                        }
                    }
                }
            }
            //venueImageView.Image = null;

            venueImageView.Alpha = 1f;
            activityIndicator.StartAnimating();

            InvokeOnMainThread(() =>
            {
                if (_builtVenue.image != null)
                {
                    venueImageView.SetImage(NSUrl.FromString(_builtVenue.image.url), UIImage.FromBundle(AppTheme.EXLogoPlaceholder), (t2, t3, t4, t5) =>
                        {

                            venueImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                            UITapGestureRecognizer singleTap = new UITapGestureRecognizer(() =>
                            {
                                if (_builtVenue.image != null)
                                {
											ImageViewController vc = new ImageViewController(new CGRect(0, 0, 600, 600), _builtVenue.image.url);
                                    AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                                }
                            });
                            singleTap.NumberOfTapsRequired = 1;
                            venueImageView.AddGestureRecognizer(singleTap);
                        });
                }
                else
                {
                    venueImageView.Image = UIImage.FromBundle(AppTheme.EXLogoPlaceholder);
                }
                
                locationImage.SetTitle(AppTheme.VIimgLocation, UIControlState.Normal);
                locationImage.SetTitleColor(AppTheme.VIimgLocationNormalColor, UIControlState.Normal);
                locationImage.SetTitleColor(AppTheme.VIimgLocationSelectedColor, UIControlState.Selected);
				locationImage.SetTitleColor(AppTheme.VIimgLocationHighlightedColor, UIControlState.Selected);
                locationImage.Font = AppTheme.VIimgLocationImageFont;
                lblVenueinfo.Text = builtVenue.info;
                locationName.Text = builtVenue.address;
                activityIndicator.StopAnimating();

                venueImageView.UserInteractionEnabled = true;
                ViewWillLayoutSubviews();
            });

        }

        public void checkImageCallback(Action<bool> callback)
        {
            InvokeOnMainThread(() =>
                {
					venueImageView.SetImage(NSUrl.FromString(_builtVenue.image.url), UIImage.FromBundle(""));
                    if (venueImageView.Image != null)
                    {
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }

                });
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            mapsheaderView.Frame = new CGRect(mapsheaderViewXPadding, mapsheaderViewYPadding, View.Frame.Width, mapsTableViewHeaderHeight);
            mapstitleheaderView.Frame = new CGRect(headerViewLeftMargin, mapstitleheaderViewYPadding, View.Frame.Width, headerHeight);
            borderTop.Frame = new CGRect(borderTopXPadding, mapstitleheaderView.Frame.Bottom, View.Frame.Width, mapsTableTopBorderWidth);

			mapsTableView.Frame = new CGRect(mapsTableViewXPadding, mapsheaderView.Frame.Bottom, mapsTableViewWidthLeftClosed, View.Frame.Height - mapsheaderView.Frame.Bottom);
			mapstableRightVerticalLine.Frame = new CGRect(mapsTableView.Frame.Right, mapsheaderView.Frame.Bottom, AppTheme.VIMiddleLineWidht, View.Frame.Height);
			wholeRightPanelWithImageAndDdescription.Frame = new CGRect(20,5 , View.Frame.Width - (mapsTableView.Frame.Width+20), View.Frame.Height);
			lblVenuename.Frame = new CGRect(lblNameXPadding, lblNameyPadding, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblNameRightPadding + lblNameXPadding), lblNameHeight);
			venueImageView.Frame = new CGRect(0, imageyPadding, wholeRightPanelWithImageAndDdescription.Frame.Width - (imageXPadding * 2), imageHeight);
			venueImageViewBackGroundView.Frame = new CGRect(imageXPadding, imageBackgroundY, wholeRightPanelWithImageAndDdescription.Frame.Width - (imageXPadding * 2), imageBackgroundHeight);

            activityIndicator.Center = venueImageView.Center;

            lblAddress.Frame = new CGRect(lblAddressXPadding, venueImageView.Frame.Height + venueImageView.Frame.Y + lblAddressyPadding, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblAddressRightPadding + lblAddressXPadding), 0);
            lblAddress.SizeToFit();
            nfloat addresssHeight = 300;
            if (lblAddress.Frame.Height < 300)
            {
                addresssHeight = lblAddress.Frame.Height;
                lblAddress.Frame = new CGRect(lblAddressXPadding, venueImageView.Frame.Height + venueImageView.Frame.Y + lblAddressyPadding, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblAddressRightPadding + lblAddressXPadding), addresssHeight);
            }
            else
            {
                lblAddress.Frame = new CGRect(lblAddressXPadding, venueImageView.Frame.Height + venueImageView.Frame.Y + lblAddressyPadding, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblAddressRightPadding + lblAddressXPadding), addresssHeight);
            }

            lblDescription.Frame = new CGRect(lblDescriptionXPadding, venueImageView.Frame.Height + 100, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblDescriptionRightPadding + lblDescriptionXPadding + 30), 0);
            lblDescription.SizeToFit();
            nfloat descriptionHeight = 300;
            if (lblDescription.Frame.Height < 300)
            {
                descriptionHeight = lblDescription.Frame.Height;
                lblDescription.Frame = new CGRect(lblDescriptionXPadding, venueImageView.Frame.Height + 100, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblDescriptionRightPadding + lblDescriptionXPadding + 30), descriptionHeight);
            }
            else
            {
                lblDescription.Frame = new CGRect(lblDescriptionXPadding, venueImageView.Frame.Height + 100, wholeRightPanelWithImageAndDdescription.Frame.Width - (lblDescriptionRightPadding + lblDescriptionXPadding + 30), descriptionHeight);
            }

			locationButton.Frame = new CGRect(20, wholeRightPanelWithImageAndDdescription.Frame.Bottom, View.Frame.Width - (mapstableRightVerticalLine.Frame.Right + 25), 0);
			locationImage.Frame = new CGRect(15, 15, 30, 30);
			locationName.Frame = new CGRect(locationImage.Frame.GetMaxX() + 10, 15, this.locationButton.Frame.Width - (locationImage.Frame.GetMaxX()+ 10) , 0);
            locationName.SizeToFit();
            locationName.Frame = new CGRect(locationImage.Frame.GetMaxX() + 10, 15, this.locationButton.Frame.Width - (locationImage.Frame.GetMaxX() + 10), locationName.Frame.Size.Height);
            if ((locationName.Frame.Height + 30) > (locationImage.Frame.Size.Height+30))
            {

				locationButton.Frame = new CGRect(20, 380, View.Frame.Width - (mapstableRightVerticalLine.Frame.Right + 35), (locationName.Frame.Height + 30));
				locationName.Frame = new CGRect(locationImage.Frame.GetMaxX() + 10, 15, this.locationButton.Frame.Width - (locationImage.Frame.GetMaxX() + 10), locationName.Frame.Size.Height);
            }
            else
            {

				locationButton.Frame = new CGRect(20, 380, View.Frame.Width - (mapstableRightVerticalLine.Frame.Right + 35), (locationImage.Frame.Size.Height + 30));
				locationImage.Frame = new CGRect(15, locationButton.Frame.Height/2-15, 30, 30);
				locationName.Frame = new CGRect(locationImage.Frame.GetMaxX() + 10, 15, this.locationButton.Frame.Width - (locationImage.Frame.GetMaxX() + 10), locationName.Frame.Size.Height);
            }


			lblVenueinfo.Frame = new CGRect(20, 380, View.Frame.Width - (mapstableRightVerticalLine.Frame.Right + 40), 0);
            lblVenueinfo.SizeToFit();

			lblVenueinfo.Frame = new CGRect(20, locationButton.Frame.Bottom + 30, View.Frame.Width - (mapstableRightVerticalLine.Frame.Right + 40), lblVenueinfo.Frame.Height);
			scrollView.Frame = new CGRect(mapstableRightVerticalLine.Frame.Right, headerHeight + mapsTableTopBorderWidth, View.Frame.Width - mapstableRightVerticalLine.Frame.Right, View.Frame.Height -(headerHeight - mapsTableTopBorderWidth));
            scrollView.ContentSize = new CGSize(View.Frame.Width - mapstableRightVerticalLine.Frame.Right, lblVenueinfo.Frame.Bottom + 20);

        }

        private void getVenues(Action<List<BuiltVenue>> callback)
        {
            DataManager.GetListOfVenues(AppDelegate.Connection).ContinueWith(t =>
            {
                lstVenues = t.Result;

                if (callback != null)
                {
                    callback(lstVenues);
                }
            });
        }

        public override void OnDeltaCompleted(NSNotification notification, List<string> updatedUids)
        {
            if (updatedUids != null && updatedUids.Contains(ApiCalls.venue))
            {
                updateSource();
            }
        }

        private void updateSource()
        {
            DataManager.GetListOfVenues(AppDelegate.Connection).ContinueWith(t =>
            {
                lstVenues = t.Result;
                lstVenues = lstVenues.OrderBy(p => p.name.ToUpper()).ToList();
                InvokeOnMainThread(() =>
                {
                    (mapsTableView.Source as MosconeDataSource).UpdateSource(lstVenues);
                    setRightViewData(lstVenues.ElementAt(0));
                    mapsTableView.ReloadData();
                    mapsTableView.SelectRow(NSIndexPath.FromRowSection(0, 0), true, UITableViewScrollPosition.None);
                });
            });
        }
    }

    #region--MosconeDataSource--
    public class MosconeDataSource : UITableViewSource
    {
        VenueImageController mosconeImageController;
        List<BuiltVenue> lstbuiltVenue;
        NSString cellIdentifier = new NSString("TableCell");

        #region--Constants
		static nfloat defaultCellHeight = 60.0f;
        #endregion

        public NSIndexPath selectedIndex;

        public MosconeDataSource(VenueImageController mosconeImageController, List<BuiltVenue> lstbuiltVenue)
        {
            this.mosconeImageController = mosconeImageController;
            this.lstbuiltVenue = lstbuiltVenue;
        }
        public void UpdateSource(List<BuiltVenue> lstbuiltOthers)
        {
            this.lstbuiltVenue = lstbuiltOthers;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return lstbuiltVenue.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            VenueCenterCell cell = tableView.DequeueReusableCell(cellIdentifier) as VenueCenterCell;
            if (cell == null) cell = new VenueCenterCell(cellIdentifier);
            var item = lstbuiltVenue.ElementAt(indexPath.Row);
            cell.UpdateCell(item);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = lstbuiltVenue[indexPath.Row];
            NSString str1 = (NSString)item.name;
            NSString str = (NSString)item.info;


			CGSize size1 = str1.StringSize(AppFonts.ProximaNovaRegular(16), new CGSize(tableView.Frame.Width - 30, 999), UILineBreakMode.WordWrap);
			CGSize size = str.StringSize(AppFonts.ProximaNovaRegular(12), new CGSize(tableView.Frame.Width-30, 36), UILineBreakMode.WordWrap);
            var height=size1.Height + size.Height;
//            if (height < defaultCellHeight)
//            {
//                return defaultCellHeight;
//            }

            return height+ 30;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            selectedIndex = indexPath;
            this.mosconeImageController.setRightViewData(lstbuiltVenue.ElementAt(indexPath.Row));
        }

    }
    #endregion
}