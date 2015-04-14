using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using CoreGraphics;
using CommonLayer;
using ConferenceAppiOS.Helpers;
using Alliance.Carousel;

namespace ConferenceAppiOS
{
	public class NextUpViewExplore : BaseViewController
	{
		public enum ViewType
		{
			NextUp,
			OnNow
		}

		CarouselView horizontalTableViewNextUp;
		UIView headerView, headerBorderBottom, headerTopBorder;
		UILabel lblSectionHeader; public NSIndexPath selectedIndex;
		public WhatsHappeningNewController _whatsHappeningNewController;
		public List<BuiltTracks> lstAllbuildTracks;
		public List<BuiltSessionTime> lstBuiltSessionTime;
		CGRect _rect;
		nfloat tableHeight = 80; static nfloat horizontalTableXPadding = 10;
		public LayoutEnum positionInParentView { get; set; }
		public nfloat RowWidth = 350;
		public nfloat RowHeight = 200;
		ViewType viewType;
        public Action OnNowLoaded;
        public Action NextUpLoaded;

		public NextUpViewExplore(CGRect rect, ViewType viewtype)
		{
			viewType = viewtype;
			_rect = rect;
			View.Frame = _rect;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = AppTheme.WHNextUpbackGround;
			headerView = new UIView()
			{
				BackgroundColor = AppTheme.SectionHeaderBackColor,
			};
			headerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			lblSectionHeader = new UILabel()
			{
				TextColor = AppTheme.SpeakerSessionTitleColor,
				Font = AppFonts.ProximaNovaRegular(18),
			};
			if (viewType.Equals(ViewType.NextUp))
			{
				lblSectionHeader.Text = AppTheme.nextUpheaderText;
			}
			else
			{
				lblSectionHeader.Text = AppTheme.onNowheaderText;
			}
			headerBorderBottom = new UIView();
			headerTopBorder = new UIView();
			headerBorderBottom.BackgroundColor = AppTheme.WHBorderColor;
			headerTopBorder.BackgroundColor = AppTheme.WHBorderColor;
			headerView.AddSubview(headerTopBorder);
			headerView.AddSubview(lblSectionHeader);
			headerView.AddSubview(headerBorderBottom);
			horizontalTableViewNextUp = new CarouselView(View.Bounds);
			horizontalTableViewNextUp.BackgroundColor = AppTheme.WHNextUpbackGround;
			horizontalTableViewNextUp.Bounces = true;
			lstAllbuildTracks = AppSettings.AllTracks;
			if (viewType == ViewType.NextUp)
			{
                DataManager.GetNextUpSessions(AppDelegate.Connection, 10, AppSettings.ApplicationUser).ContinueWith(t =>
					{
                        lstBuiltSessionTime = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();

						if (lstBuiltSessionTime == null || lstBuiltSessionTime.Count == 0)
						{

							_whatsHappeningNewController.showNextUp = false;
							InvokeOnMainThread(() =>
								{
									_whatsHappeningNewController.ViewWillLayoutSubviews();
                                    if (NextUpLoaded != null)
                                    {
                                        NextUpLoaded();
                                    }
								});
						}
						else
						{
							_whatsHappeningNewController.showNextUp = true;
							InvokeOnMainThread(() =>
								{
									horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);
									horizontalTableViewNextUp.Delegate = new NextUpTableDelgate(this);
									horizontalTableViewNextUp.CarouselType = CarouselType.Linear;
									horizontalTableViewNextUp.ConfigureView();
									horizontalTableViewNextUp.Vertical = false;
									View.AddSubviews(headerView, horizontalTableViewNextUp);
                                    if (NextUpLoaded != null)
                                    {
                                        NextUpLoaded();
                                    }
								});
						}
					});
			}
			else
			{
                DataManager.GetOnNowSessions(AppDelegate.Connection, 10).ContinueWith(t =>
					{
                        lstBuiltSessionTime = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();

						if (lstBuiltSessionTime == null || lstBuiltSessionTime.Count == 0)
						{
							_whatsHappeningNewController.showOnNow = false;
							InvokeOnMainThread(() =>
								{
									_whatsHappeningNewController.ViewWillLayoutSubviews();
                                    if (OnNowLoaded != null)
                                    {
                                        OnNowLoaded();
                                    }
								});
						}
						else
						{
							_whatsHappeningNewController.showOnNow = true;
							InvokeOnMainThread(() =>
								{
									horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);
									horizontalTableViewNextUp.Delegate = new NextUpTableDelgate(this);
									horizontalTableViewNextUp.CarouselType = CarouselType.Linear;
									horizontalTableViewNextUp.ConfigureView();
									horizontalTableViewNextUp.Vertical = false;
									View.AddSubviews(headerView, horizontalTableViewNextUp);
                                    if (OnNowLoaded != null)
                                    {
                                        OnNowLoaded();
                                    }
								});
						}
					});
			}
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			if (View.Frame.Height == 0)
			{
				horizontalTableViewNextUp.Hidden = true;
			}
			else
			{
				horizontalTableViewNextUp.Hidden = false;
			}

			headerView.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.SectionheaderHeight);
			headerTopBorder.Frame = new CGRect(0, 0, View.Frame.Width, AppTheme.borderForSectionHeaderTopWidhtVar);
			headerBorderBottom.Frame = new CGRect(0, headerView.Frame.Size.Height, View.Frame.Width, AppTheme.sectionBottomBorderHeight);
			lblSectionHeader.Frame = new CGRect(AppTheme.sectionheaderTextLeftPadding, 0, headerView.Frame.Width, headerView.Frame.Height);
			horizontalTableViewNextUp.Frame = new CGRect(0, headerBorderBottom.Frame.Bottom+horizontalTableXPadding, View.Frame.Width, tableHeight);
		}

        public void RefreshOnNow(Action completionHandler)
        {
            DataManager.GetOnNowSessions(AppDelegate.Connection, 10).ContinueWith(t =>
            {
					try{
						lstAllbuildTracks = AppSettings.AllTracks;
						lstBuiltSessionTime = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();

						if (lstBuiltSessionTime == null || lstBuiltSessionTime.Count == 0)
						{
							InvokeOnMainThread(() =>
								{
									_whatsHappeningNewController.showOnNow = false;
									_whatsHappeningNewController.ViewWillLayoutSubviews();
									if (completionHandler != null)
									{
										completionHandler();
									}
								});
						}
						else
						{
							_whatsHappeningNewController.showOnNow = true;
							InvokeOnMainThread(() =>
								{
									if (horizontalTableViewNextUp.DataSource == null)
									{
										horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);
										horizontalTableViewNextUp.ReloadData();
										horizontalTableViewNextUp.Delegate = new NextUpTableDelgate(this);
										horizontalTableViewNextUp.CarouselType = CarouselType.Linear;
										horizontalTableViewNextUp.ConfigureView();
										horizontalTableViewNextUp.Vertical = false;
										View.AddSubviews(headerView, horizontalTableViewNextUp);
									}
									else 
									{
										if(horizontalTableViewNextUp.DataSource == null)
											horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);

										(horizontalTableViewNextUp.DataSource as NextUpTableSource).UpdateSource(this);
										horizontalTableViewNextUp.ReloadData();
									}
									_whatsHappeningNewController.ViewWillLayoutSubviews();
									if (completionHandler != null)
									{
										completionHandler();
									}
								});
						}
					}catch{
						Console.WriteLine("sfdsf");
					}
                
            });
        }

        public void RefreshNextUp(Action completionHandler)
        {
            DataManager.GetNextUpSessions(AppDelegate.Connection, 10, AppSettings.ApplicationUser).ContinueWith(t =>
            {
                lstAllbuildTracks = AppSettings.AllTracks;
                lstBuiltSessionTime = t.Result.OrderBy(p => Convert.ToDateTime(p.date)).ThenBy(p => Helper.timeConverterForBuiltTimeString(p.time)).ToList();
//
//                if (lstBuiltSessionTime == null || lstBuiltSessionTime.Count == 0)
//                {
//
//                    _whatsHappeningNewController.showNextUp = false;
//                    InvokeOnMainThread(() =>
//                    {
//                        _whatsHappeningNewController.ViewWillLayoutSubviews();
//                    });
//                }
//                else
//                {
//                    _whatsHappeningNewController.showNextUp = true;
//                    InvokeOnMainThread(() =>
//                    {
//                        if (horizontalTableViewNextUp.DataSource == null)
//                        {
//                            horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);
//                            horizontalTableViewNextUp.ReloadData();
//                            horizontalTableViewNextUp.Delegate = new NextUpTableDelgate(this);
//                            horizontalTableViewNextUp.CarouselType = CarouselType.Linear;
//                            horizontalTableViewNextUp.ConfigureView();
//                            horizontalTableViewNextUp.Vertical = false;
//                            View.AddSubviews(headerView, horizontalTableViewNextUp);
//                        }
//                        else
//                        {
//							if(horizontalTableViewNextUp.DataSource == null)
//                            horizontalTableViewNextUp.DataSource = new NextUpTableSource(this);
//
//                            (horizontalTableViewNextUp.DataSource as NextUpTableSource).UpdateSource(this);
//                            horizontalTableViewNextUp.ReloadData();
//                        }
//                        _whatsHappeningNewController.ViewWillLayoutSubviews();
//                    });
//                }
//
                if (completionHandler != null)
                {
                    completionHandler();
                }
            });
        }
	}

	public class NextUpTableDelgate : CarouselViewDelegate
	{
		NextUpViewExplore nextUpViewExplore;

		public NextUpTableDelgate(NextUpViewExplore vc)
		{
			nextUpViewExplore = vc;
		}

		public override nfloat ValueForOption (CarouselView carouselView, CarouselOption option, nfloat aValue)
		{
			if (option == CarouselOption.Spacing)
			{
				return aValue * 1.05f;
			}
			return aValue;
		}

		public override void DidSelectItem (CarouselView carouselView, nint index)
		{
			SessionDetailController sessionDetailController = new SessionDetailController(nextUpViewExplore.lstBuiltSessionTime.ElementAt((int)index));
			sessionDetailController.View.Frame = new CGRect(-100, 0, 477, nextUpViewExplore._whatsHappeningNewController.View.Frame.Size.Height);
			AppDelegate.instance().rootViewController.openDetail(sessionDetailController, nextUpViewExplore._whatsHappeningNewController, false);
			DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.explore_ongoing_or_nextup_session, Helper.ToDateString(DateTime.Now));
		}
	}

	public class NextUpTableSource : CarouselViewDataSource
	{
		NextUpViewExplore nextUpViewExplore;

		public NextUpTableSource(NextUpViewExplore vc)
		{
			this.nextUpViewExplore = vc;
		}

		public override nint NumberOfItems(CarouselView carousel)
		{
			return (nint)nextUpViewExplore.lstBuiltSessionTime.Count;
		}

		public override UIView ViewForItem (CarouselView carouselView, nint index, UIView reusingView)
		{
			if (reusingView == null)
			{
				HorizontalCell cell = new HorizontalCell(new CGRect(0, 7, 340, nextUpViewExplore.View.Frame.Height - (AppTheme.SectionheaderHeight + 20)));
				reusingView = cell;
			}
			BuiltSessionTime sessiontime = nextUpViewExplore.lstBuiltSessionTime.ElementAt((int)index);

			HorizontalCell celll = (HorizontalCell)reusingView;
			celll.UpdateCell(sessiontime, nextUpViewExplore.lstAllbuildTracks);
			celll.BackgroundColor = UIColor.Clear;
			return reusingView;
		}

        public void UpdateSource(NextUpViewExplore vc)
        {
            this.nextUpViewExplore = vc;
        }
	}
}