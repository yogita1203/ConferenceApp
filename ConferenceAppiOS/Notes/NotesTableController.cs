using BigTed;
using CommonLayer;
using CommonLayer.Entities.Built;
using ConferenceAppiOS.Helpers;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using System.Net;
using Alliance.Carousel;
using CoreAnimation;
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS.Notes
{
    [Register("NotesTableController")]
    public class NotesTableController : BaseViewController
    {
        const string TitleString = "";
        string deleteAlert = AppTheme.NTDeleteAlert;
		static nfloat HeaderHeight = 64;
		static nfloat FilterSectionHeight = 53;
		static nfloat tagNavigatorHeight = 60;
		static nfloat NotesTableWidth = 362;
        nfloat lineSize = AppTheme.NTSeparatorBorderWidth;
		static nfloat detailPageWidth = 560;
		static nfloat detailPageHeight = 660;
        nfloat ScrollViewHeight = 180;
		static nfloat TextFieldHeight = 40;
		static nfloat TextViewHeight = 0;
		static nfloat TopBarHeight = 50;
		static nfloat Margin = 20;
		nfloat ContentViewHeight = 0;
        UITableView notesTable;
        internal Action<BuiltNotes> NoteChangedHandler;
        readonly UIColor pageBackground = AppTheme.NTPageBGColor;

        LineView horizontalLine;
        LineView verticalLine;

        UILabel lblTitle;
        UILabel lblDescription;
		UIScrollView noteDetailsView;
		public BuiltNotes currentNote;
        CarouselView imageScroller;
		UIView lineView,lineview;

        TitleHeaderView titleheaderView;
		public List<BuiltNotes> result;
        public NotesTableController(CGRect rect)
        {
            View.Frame = rect;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view
            View.BackgroundColor = pageBackground;

            NoteChangedHandler = noteChanged;

            horizontalLine = new LineView(new CGRect(0, HeaderHeight, View.Frame.Width, lineSize))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            verticalLine = new LineView(new CGRect(NotesTableWidth, HeaderHeight, lineSize, View.Frame.Height - HeaderHeight));

            notesTable = new UITableView()
            {
                Frame = new CGRect(0, HeaderHeight, NotesTableWidth, View.Frame.Height),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleRightMargin,
            };

            setNotesHeader();
            setNotesHeaderForTag();
            notesTable.TableFooterView = new UIView(new CGRect(0, 0, 0, 1));

            setTableSource();

            View.AddSubviews(notesTable, verticalLine);

            DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.notes, Helper.ToDateString(DateTime.Now));
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            titleheaderView.Frame = new CGRect(0, 0, View.Frame.Width, HeaderHeight);
            verticalLine.Frame = new CGRect(NotesTableWidth, HeaderHeight, lineSize, View.Frame.Size.Height - HeaderHeight);
            notesTable.Frame = new CGRect(0, HeaderHeight, NotesTableWidth, View.Frame.Height - HeaderHeight);
			if (noteDetailsView != null) {
				noteDetailsView.Frame = new CGRect (verticalLine.Frame.Right, HeaderHeight + lineSize, View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));
				noteDetailsView.ContentSize = new CGSize (View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));
			}
        }

        private void setTableSource()
        {
            setHeaderHeight(FilterSectionHeight);
            DataManager.GetNotes(AppDelegate.Connection).ContinueWith(t =>
            {
                result = t.Result;

                if (result != null)
                {
                    result = result.OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();

                    InvokeOnMainThread(() =>
                    {
						notesTable.Source = new NotesTableSource(this, result);;
                        notesTable.ReloadData();

                        if (result.Count > 0)
                            notesTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                        currentNote = result.FirstOrDefault();
                        setNoteDetailsView();

                        if (AppSettings.OpenNewNote)
                        {
                            NoteDetailsController vc = new NoteDetailsController(new CGRect(0, 0, detailPageWidth, detailPageHeight), null);
                            vc.NoteSavedHandler = noteSaved;
                            vc.NoteDeletedHandler = noteDeleted;
                            AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                            AppSettings.OpenNewNote = false;
                        }
                    });
                }
            });
        }

        int previousSegmentIndex;
        private void setTableSource(int index)
        {
            previousSegmentIndex = index;
            if (index == 0)
                setHeaderHeight(FilterSectionHeight);
            else if (index == 1)
                setHeaderHeight(FilterSectionHeight);
            else
            {
                //setHeaderHeight(FilterSectionHeight + tagNavigatorHeight);
            }

            DataManager.GetNotes(AppDelegate.Connection).ContinueWith(t =>
            {
                result = t.Result;

                if (result != null)
                {
                    if (index == 0)
                        result = result.OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();
                    else if (index == 1)
                        result = result.OrderBy(p => p.title).ToList();
                    else
                    {
                        currentTagIndex = 0;
                        tags = result.SelectMany(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
                        if (tags != null && tags.Count > 0)
                        {
                            result = result.Where(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(tags[currentTagIndex])).ToList();
                            InvokeOnMainThread(() =>
                            {
                                lblTag.Text = tags[currentTagIndex];
                                setHeaderHeight(FilterSectionHeight + tagNavigatorHeight);
                            });
                        }

                        InvokeOnMainThread(() =>
                        {
                            SetTagButtonsState();
                        });
                    }

                    if (result != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            notesTable.Source = new NotesTableSource(this, result);
                            notesTable.ReloadData();

                            if (result.Count > 0)
                                notesTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                            currentNote = result.FirstOrDefault();
                            updateNoteDetailsView();

                            if (AppSettings.OpenNewNote)
                            {
                                NoteDetailsController vc = new NoteDetailsController(new CGRect(0, 0, detailPageWidth, detailPageHeight), null);
                                vc.NoteSavedHandler = noteSaved;
                                vc.NoteDeletedHandler = noteDeleted;
                                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
                                AppSettings.OpenNewNote = false;
                            }
                        });
                    }
                }
            });
        }

        private void SetTagButtonsState()
        {
            if (tags.Count <= 1)
            {
                btnPrevious.Enabled = false;
                btnNext.Enabled = false;
            }
            else
            {
                if (currentTagIndex == 0)
                    btnPrevious.Enabled = false;
                if (currentTagIndex == tags.Count - 1)
                    btnNext.Enabled = false;
                else
                    btnNext.Enabled = true;
            }
        }

        List<string> tags;
        private void setTableSourceForCurrentTag()
        {
            DataManager.GetNotes(AppDelegate.Connection).ContinueWith(t =>
            {
                result = t.Result;

                if (result != null)
                {
                    tags = result.SelectMany(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
                    if (tags != null && tags.Count > 0)
                    {
                        result = result.Where(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(tags[currentTagIndex])).ToList();
                    }
                }

                InvokeOnMainThread(() =>
                {
                    lblTag.Text = tags[currentTagIndex];
                    notesTable.Source = new NotesTableSource(this, result);
                    notesTable.ReloadData();

                    if (result.Count > 0)
                        notesTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                    currentNote = result.FirstOrDefault();
                    updateNoteDetailsView();

                    SetTagButtonsState();
                });
            });
        }

        void setHeaderHeight(nfloat height)
        {
            CGRect newFrame = headerWithTagFilter.Frame;

            if (newFrame.Height != height)
            {
                if (height == FilterSectionHeight)
                    tagNavigator.RemoveFromSuperview();
                else
                {
                    headerWithTagFilter.AddSubview(tagNavigator);
                }

                newFrame.Height = height;
                headerWithTagFilter.Frame = newFrame;
                notesTable.TableHeaderView = headerWithTagFilter;
            }
        }

        private void updateTableSource()
        {
            DataManager.GetNotes(AppDelegate.Connection).ContinueWith(t =>
            {
                int index = previousSegmentIndex;
                var result = t.Result;

                if (result != null)
                {
                    if (index == 0)
                        result = result.OrderByDescending(p => Convert.ToDateTime(p.updated_at)).ToList();
                    else if (index == 1)
                        result = result.OrderBy(p => p.title).ToList();
                    else
                    {
                        currentTagIndex = 0;
                        tags = result.SelectMany(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().ToList();
                        if (tags != null && tags.Count > 0)
                        {
                            result = result.Where(p => p.tags_separarated.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(tags[currentTagIndex])).ToList();
                            InvokeOnMainThread(() =>
                            {
                                lblTag.Text = tags[currentTagIndex];
                                setHeaderHeight(FilterSectionHeight + tagNavigatorHeight);
                            });
                        }
                        else
                        {
                            InvokeOnMainThread(() =>
                            {
                                setHeaderHeight(FilterSectionHeight);
                            });
                        }

                        InvokeOnMainThread(() =>
                        {
                            SetTagButtonsState();
                        });
                    }

                    if (result != null)
                    {
                        InvokeOnMainThread(() =>
                        {
                            notesTable.Source = new NotesTableSource(this, result);
                            notesTable.ReloadData();

                            if (result.Count > 0)
                                notesTable.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.None);

                            currentNote = result.FirstOrDefault();
                            updateNoteDetailsView();
                        });
                    }
                }
            });
        }

        private void noteChanged(BuiltNotes note)
        {
            currentNote = note;
            updateNoteDetailsView();

        }

        void setNotesHeader()
        {
            titleheaderView = new TitleHeaderView(TitleString, false, false, false, false, true, false, false, false)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            titleheaderView.AddButtonClicked = () =>
            {
                NoteDetailsController vc = new NoteDetailsController(new CGRect(0, 0, detailPageWidth, detailPageHeight), null);
                vc.NoteSavedHandler = noteSaved;
                vc.NoteDeletedHandler = noteDeleted;
                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
            };


            titleheaderView.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
            View.AddSubview(titleheaderView);
        }

        UIView headerWithTagFilter, tagNavigator;
        int currentTagIndex;
        UILabel lblTag;
        UIButton btnPrevious, btnNext;
        void setNotesHeaderForTag()
        {
            headerWithTagFilter = new UIView(new CGRect(0, 0, View.Frame.Width, FilterSectionHeight + tagNavigatorHeight))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };

            SegmentView notesFilter = new SegmentView(new List<string>
            {
                AppTheme.NTbydateText,
                AppTheme.NTbytitleText,
				AppTheme.NTbytagText
            })
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            notesFilter.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleWidth;
            notesFilter.Frame = new CGRect(-2, 0, headerWithTagFilter.Frame.Width + 5, FilterSectionHeight);
            notesFilter.SegmentChangedHandler = (index) =>
            {
                setTableSource(index);
            };

            notesFilter.BackgroundColor = AppTheme.NTsegmentbackColor;
            notesFilter.selectedSegmentIndex = 0;
            notesFilter.textColor = AppTheme.NTsegmentTitletextColor;
			notesFilter.subHeadingTextColor = AppTheme.NTsegmentSubTitletextColor;
            notesFilter.font = AppTheme.NTsegmentFont;
            notesFilter.selectedBoxColor = AppTheme.NTsegmentSelectedtabColor;
            notesFilter.selectedBoxColorOpacity = 1.0f;
            notesFilter.selectedBoxBorderWidth = 0.0f;
            notesFilter.selectionIndicatorHeight = 4.0f;
            notesFilter.selectedTextColor = AppTheme.NTsegmentTitletextColor;
            notesFilter.selectionIndicatorColor = AppTheme.NTsegmentSelectedtabBottomColor;
            notesFilter.selectionStyle = SegmentView.SGSegmentedControlSelectionStyle.Box;
            notesFilter.selectionIndicatorLocation = SegmentView.SGSegmentedControlSelectionIndicatorLocation.Down;
            notesFilter.multiLineSupport = false;
            notesFilter.multiLineFont = AppTheme.NTmultiLineFont;
            notesFilter.separatorColor = AppTheme.NTsegmentSeparatorColor;
			notesFilter.Layer.CornerRadius = 0.0f;
			notesFilter.Layer.BorderColor = AppTheme.SIsegmentSeparatorColor.CGColor;
			notesFilter.Layer.BorderWidth = 1.0f;
			notesFilter.AddSubview (horizontalLine);
            var frame = notesFilter.Frame;
            frame.Y = frame.Bottom;
            frame.Height = tagNavigatorHeight;

			horizontalLine.Frame = new CGRect (0,notesFilter.Frame.Height - lineSize,notesFilter.Frame.Width,lineSize);

            tagNavigator = new UIView(frame)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };
			tagNavigator.BackgroundColor = AppTheme.NTTagNavigatorColor;

            btnPrevious = UIButton.FromType(UIButtonType.Custom);
            btnPrevious.Frame = new CGRect(10, 10, 40, 40);
            btnPrevious.SetImage(UIImage.FromBundle(AppTheme.NTbackArrow), UIControlState.Normal);
            btnPrevious.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnPrevious.Enabled = false;
            btnPrevious.TouchUpInside += (s, e) =>
            {
                if (currentTagIndex > 0)
                {
                    currentTagIndex--;
                    setTableSourceForCurrentTag();

                    if (currentTagIndex == 0)
                        btnPrevious.Enabled = false;
                    btnNext.Enabled = true;
                }
            };
            btnNext = UIButton.FromType(UIButtonType.Custom);
            btnNext.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
            btnNext.Frame = new CGRect(tagNavigator.Frame.Width - 50, 10, 40, 40);
            btnNext.SetImage(UIImage.FromBundle("fwdarrow.png"), UIControlState.Normal);
            btnNext.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnNext.TouchUpInside += (s, e) =>
            {
                if (currentTagIndex < tags.Count - 1)
                {
                    currentTagIndex++;
                    setTableSourceForCurrentTag();

                    if (currentTagIndex == tags.Count - 1)
                        btnNext.Enabled = false;
                    btnPrevious.Enabled = true;
                }
            };

            lblTag = new UILabel(new CGRect(btnPrevious.Frame.Right + 10, 0, tagNavigator.Frame.Width - (btnPrevious.Frame.Width + btnNext.Frame.Width + 40), 60))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };
            lblTag.TextAlignment = UITextAlignment.Center;
            lblTag.TextColor = AppTheme.NTnotesCellTitleColor;
            lblTag.Font = AppTheme.NTtagFont;
            currentTagIndex = 0;
            if (tags != null && tags.Count > currentTagIndex)
                lblTag.Text = tags[currentTagIndex];

            tagNavigator.AddSubviews(btnPrevious, btnNext, lblTag);

            headerWithTagFilter.AddSubviews(notesFilter, tagNavigator);

            notesTable.TableHeaderView = headerWithTagFilter;
        }

		static nfloat topBarTitleLeftMargin = 20;
		static nfloat topBarTitleWidth = 100;
        string topBarTitleText = AppTheme.NTnotesTitle;

		static nfloat btnDeleteRightMargin = 90;
		static nfloat btnTopMargin = 10;
		static nfloat btnSize = 30;
		static nfloat btnImageEdgeInset = 5;

		static nfloat btnEditLeftMargin = 5;

        void setNoteDetailsView()
        {
			noteDetailsView = new UIScrollView();
			noteDetailsView.ClipsToBounds = true;
            noteDetailsView.Frame = new CGRect(verticalLine.Frame.Right, HeaderHeight + lineSize, View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));
			noteDetailsView.ContentSize = new CGSize (View.Frame.Width - verticalLine.Frame.Right, View.Frame.Height - (HeaderHeight - lineSize));
			noteDetailsView.BackgroundColor = AppTheme.NTPageBGColor;
			ContentViewHeight = 0;
            UIView topBar = new UIView(new CGRect(0, 0, noteDetailsView.Frame.Width, TopBarHeight))
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin,
                BackgroundColor = AppTheme.NTtopBarColor,
            };

			ContentViewHeight = topBar.Frame.Bottom;

            UILabel topBarTitle = new UILabel(new CGRect(topBarTitleLeftMargin, 0, topBarTitleWidth, TopBarHeight))
            {
                BackgroundColor =AppTheme.NTtopBarTitleBackColor,
                Text = topBarTitleText,
                Font = AppTheme.NTtopbarTitleFont,
				TextColor = AppTheme.NTtopbarTitleFontColor

            };

            UIButton btnDelete = UIButton.FromType(UIButtonType.Custom);
            btnDelete.Frame = new CGRect(topBar.Frame.Right - btnDeleteRightMargin, btnTopMargin, btnSize, btnSize);
			btnDelete.Font = AppTheme.NTDeleteIconNameFont;
			btnDelete.SetTitleColor (AppTheme.NTDeleteIconNameNormalColor, UIControlState.Normal);
			btnDelete.SetTitleColor (AppTheme.NTDeleteIconNameSelectedColor, UIControlState.Selected);
			btnDelete.SetTitleColor (AppTheme.NTDeleteIconNameHighlightedColor, UIControlState.Highlighted);
			btnDelete.SetTitle (AppTheme.NTDeleteIconName, UIControlState.Normal);
			btnDelete.SetTitle (AppTheme.NTDeleteIconName, UIControlState.Selected);
			btnDelete.SetTitle (AppTheme.NTDeleteIconName, UIControlState.Highlighted);
			btnDelete.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnDelete.ImageEdgeInsets = new UIEdgeInsets(btnImageEdgeInset, btnImageEdgeInset, btnImageEdgeInset, btnImageEdgeInset);
            btnDelete.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            btnDelete.TouchUpInside += (s, e) =>
            {
                var alert = new UIAlertView("", AppTheme.NTdeleteAlertTitle, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
                alert.Clicked += (sender, args) =>
                {
                    if (args.ButtonIndex == 1)
                    {
						LoadingView.Show(string.Empty);
                        DataManager.deleteNote(AppDelegate.Connection, currentNote.uid, (err) =>
                        {
                            if (err == null)
                            {
                                InvokeOnMainThread(() =>
                                {
									LoadingView.Dismiss();
                                    updateTableSource();
                                });
                            }
                            else
                            {
                                InvokeOnMainThread(() =>
                                {
									LoadingView.Dismiss();
                                    new UIAlertView("", AppTheme.FailedText, null, AppTheme.DismissText).Show();
                                });
                            }
                        });
                    }
                };
                alert.Show();
            };

            UIButton btnEdit = UIButton.FromType(UIButtonType.Custom);
            btnEdit.Frame = new CGRect(btnDelete.Frame.Right + btnEditLeftMargin, btnTopMargin+2, btnSize, btnSize);
			btnEdit.Font = AppTheme.NotesEditIconNameFont;
			btnEdit.SetTitleColor (AppTheme.NotesEditIconNameNormalColor, UIControlState.Normal);
			btnEdit.SetTitleColor (AppTheme.NotesEditIconNameSelectedColor, UIControlState.Selected);
			btnEdit.SetTitleColor (AppTheme.NotesEditIconNameHighlightedColor, UIControlState.Highlighted);
			btnEdit.SetTitle (AppTheme.NotesEditIconName, UIControlState.Normal);
			btnEdit.SetTitle (AppTheme.NotesEditIconName, UIControlState.Selected);
			btnEdit.SetTitle (AppTheme.NotesEditIconName, UIControlState.Highlighted);
			btnEdit.ContentMode = UIViewContentMode.ScaleAspectFit;
            btnEdit.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
            btnEdit.TouchUpInside += (s, e) =>
            {
                NoteDetailsController vc = new NoteDetailsController(new CGRect(0, 0, detailPageWidth, detailPageHeight), currentNote);
                vc.NoteSavedHandler = noteSaved;
                vc.NoteDeletedHandler = noteDeleted;
                vc.CancelClickedHandler = cancelClicked;
                AppDelegate.instance().rootViewController.openInDialougueView(vc, DialogAlign.center);
            };

            topBar.AddSubviews(topBarTitle, btnDelete, btnEdit);

			imageScroller = new CarouselView (new CGRect (0, topBar.Frame.Bottom+1, noteDetailsView.Frame.Width, ScrollViewHeight));

			lineView = new UIView (new CGRect (0, imageScroller.Frame.Bottom, imageScroller.Frame.Width, lineSize));
			lineView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			lineView.BackgroundColor = AppTheme.NTLineViewColor;

			lineview = new UIView (new CGRect (0, 0, 0, 0));
			lineview.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			lineview.BackgroundColor = AppTheme.NTLineViewColor;

            if (currentNote != null)
            {
                if (currentNote.photos == null || currentNote.photos.Count == 0)
                {
                    ScrollViewHeight = 0f;
					lineView.Frame = new CGRect (0, imageScroller.Frame.Bottom, imageScroller.Frame.Width, 0);
                }
            }
            else
            {
				lineView.Frame = new CGRect (0, imageScroller.Frame.Bottom, imageScroller.Frame.Width, 0);
                ScrollViewHeight = 0f;
            }

			imageScroller.DataSource = new ImageScrollerDatasource(this);
			imageScroller.Delegate = new ImageScrollerDelegate(this);
			imageScroller.CarouselType = CarouselType.Linear;
			imageScroller.Vertical = false;
			imageScroller.ConfigureView ();
            imageScroller.Frame = new CGRect(0, topBar.Frame.Bottom+1, noteDetailsView.Frame.Width, ScrollViewHeight);

			ContentViewHeight = imageScroller.Frame.Bottom;

            lblTitle = new UILabel(new CGRect(Margin, imageScroller.Frame.Bottom, noteDetailsView.Frame.Width - (Margin * 2), TextFieldHeight))
            {
				Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Font = AppTheme.NTtitleFont,
				TextColor = AppTheme.NTtitleFontColor
            };

            lblDescription = new UILabel(new CGRect(Margin, lblTitle.Frame.Bottom, noteDetailsView.Frame.Width - (Margin * 2), TextViewHeight))
            {
                Lines = 0,
				LineBreakMode = UILineBreakMode.WordWrap,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
                Font = AppTheme.NTdescriptionFont,
				TextColor = AppTheme.NTdescriptionFontColor
            };

            if (currentNote != null)
            {
				lblTitle.AttributedText = AppFonts.IncreaseLineHeight(currentNote.title,lblTitle.Font,lblTitle.TextColor);
				//currentNote.title;
				lblTitle.SizeToFit ();
				lblTitle.Frame = new CGRect (Margin, imageScroller.Frame.Bottom+10, noteDetailsView.Frame.Width - (Margin * 2), lblTitle.Frame.Height);

				lblDescription.AttributedText = AppFonts.IncreaseLineHeight(currentNote.content,lblDescription.Font,lblDescription.TextColor);;
                lblDescription.SizeToFit();
				lblDescription.Frame = new CGRect (Margin, lblTitle.Frame.Bottom+10, noteDetailsView.Frame.Width - (Margin * 2), lblDescription.Frame.Height);
				ContentViewHeight = lblDescription.Frame.Bottom;

			}
            else
            {
                noteDetailsView.Hidden = true;
            }


			noteDetailsView.AddSubviews(imageScroller,lineview, lineView, lblTitle, lblDescription, topBar);
            View.AddSubview(noteDetailsView);
			if (ContentViewHeight > (View.Frame.Height - (HeaderHeight - lineSize))) {
				noteDetailsView.ContentSize = new CGSize (View.Frame.Width - verticalLine.Frame.Right, ContentViewHeight +10);
			}
        }

        private void cancelClicked(bool shouldRefresh)
        {
            if (shouldRefresh)
            {
                updateTableSource();
            }
        }

        void updateNoteDetailsView()
        {
            if (currentNote != null)
            {
				ContentViewHeight = 0;
                if (currentNote.photos == null || currentNote.photos.Count == 0)
                {
                    ScrollViewHeight = 0f;
                }
                else
                {
                    ScrollViewHeight = 180f;
                }

				lblTitle.AttributedText = AppFonts.IncreaseLineHeight (currentNote.title,lblTitle.Font,lblTitle.TextColor);

				lblDescription.AttributedText = AppFonts.IncreaseLineHeight (currentNote.content,lblDescription.Font,lblDescription.TextColor);

                imageScroller.Frame = new CGRect(0, TopBarHeight+1, noteDetailsView.Frame.Width, ScrollViewHeight);
				lineView.Frame = new CGRect (0, imageScroller.Frame.Bottom, imageScroller.Frame.Width, lineSize);
				lineview.Frame = new CGRect (0, 0,0,0);
				if (lineView.Frame.Y != 0) {
					lineview.Frame = new CGRect (0, TopBarHeight+1, imageScroller.Frame.Width, lineSize);
				}
                lblTitle.Frame = new CGRect(Margin, imageScroller.Frame.Bottom+10, noteDetailsView.Frame.Width - (Margin * 2), TextFieldHeight);
				lblTitle.SizeToFit ();
				lblTitle.Frame = new CGRect (Margin, imageScroller.Frame.Bottom+10, noteDetailsView.Frame.Width - (Margin * 2), lblTitle.Frame.Height);

				lblDescription.Frame = new CGRect(Margin, lblTitle.Frame.Bottom+10, noteDetailsView.Frame.Width - (Margin * 2), TextViewHeight);

				imageScroller.ConfigureView();
				imageScroller.ReloadData ();
               
                var frame = lblDescription.Frame;
                lblDescription.SizeToFit();
                frame.Height = lblDescription.Frame.Height;
                lblDescription.Frame = frame;
				ContentViewHeight = lblDescription.Frame.Bottom;
                noteDetailsView.Hidden = false;
            }
            else
            {
                noteDetailsView.Hidden = true;
            }
			if (ContentViewHeight > (View.Frame.Height - (HeaderHeight - lineSize))) {
				noteDetailsView.ContentSize = new CGSize (View.Frame.Width - verticalLine.Frame.Right, ContentViewHeight + 10);
			}
        }

        void noteSaved()
        {
            updateTableSource();
        }

        void noteDeleted()
        {
            updateTableSource();
        }

        public override void OnAfterLoginDataFetched(NSNotification notification)
        {
            updateTableSource();
        }

        public override void OnUserLoggedOut(NSNotification notification)
        {
            WhatsHappeningNewController homeScreen = new WhatsHappeningNewController(new CGRect(0, 0, 954, this.View.Frame.Size.Height));
            AppDelegate.instance().rootViewController.openFromMenu(homeScreen);
        }
    }
}