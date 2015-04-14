using System;
using System.Drawing;
using UIKit;
using Foundation;
using CommonLayer.Entities.Built;
using CommonLayer;
using BigTed;
using System.Collections.Generic;
using ConferenceAppiOS.Helpers;
using NSTokenView;
using System.Linq;
using Alliance.Carousel;
using CoreGraphics;
using ConferenceAppiOS.CustomControls;

namespace ConferenceAppiOS.Notes
{
	[Register("NoteDetailsController")]
	public class NoteDetailsController : BaseViewController
	{
		static nfloat ScrollViewHeight = 175;
		static nfloat TextFieldHeight = 40;
		static nfloat TextViewHeight = 125;
		static nfloat BottomBarHeight = 60;
		static nfloat Margin = 20;
		static nfloat TokenMargin = 5;
		static nfloat TopBarHeight = 60;
		static nfloat ImageBarMargin = 5;
		nfloat tvDescLevelMargin = 3;
		string titlePlaceholderString = AppTheme.NTtitleForNotesText;
		string descPlaceholderString = AppTheme.NTdescriptionForNotesText;
		string defaultBarTitle = AppTheme.NtnoteTitle;
		string deleteAlert = AppTheme.NTDeleteAlert;
		static nfloat descPlaceholderMargin = 5;
		static nfloat descPlaceholderHeight = 20;
		nfloat ContentHeight = 0;
		UITextField tfTitle;
		UITextView tvDescription;

		public BuiltNotes currentNote;
		CGRect frame;

		internal Action NoteSavedHandler;
		internal Action NoteDeletedHandler;
		internal Action<bool> CancelClickedHandler;
		internal bool isDescriptionFrameChange = false;

		CarouselView imageScroller;
		UIScrollView scrollView;

		UIImagePickerController imagePicker;
		private bool shouldRefresh;

		#region --TokenView--
		[Outlet]
		public TokenView tokenView { get; set; }
		private List<string> _tokenSource = new List<string>();
		private NameSelectorDelegate _nameSelectorDelegate;
		private NameSelectorSource _nameSelectorSource;
		#endregion

		public NoteDetailsController()
		{
		}

		public NoteDetailsController(CGRect frame, BuiltNotes note)
		{
			this.frame = frame;
			currentNote = note;
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		private void InitTokenView()
		{
			_nameSelectorSource = new NameSelectorSource(this);
			_nameSelectorDelegate = new NameSelectorDelegate(this);
			tokenView.TokenDataSource = _nameSelectorSource;
			tokenView.TokenDelegate = _nameSelectorDelegate;

			tokenView.SetupInit();
			tokenView.Layer.CornerRadius = 5;
			tokenView.Layer.BorderColor = UIColor.LightTextColor.CGColor;
			tokenView.PlaceholderText = "Tag...";
			tokenView.ColorScheme = new UIColor(62 / 255.0f, 149 / 255.0f, 206 / 255.0f, 1.0f);
			tokenView._scrollView.ShowsVerticalScrollIndicator = false;
			tokenView._scrollView.ShowsHorizontalScrollIndicator = false;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
			scrollView.Frame = new CGRect(0, TopBarHeight, View.Frame.Size.Width, View.Frame.Size.Height - TopBarHeight);

			tokenView.Frame = new CGRect(TokenMargin, imageScroller.Frame.Bottom, View.Frame.Width - (TokenMargin * 2), tokenView.Frame.Size.Height);

			if (isDescriptionFrameChange)
			{
				tvDescription.Frame = new CGRect(Margin - tvDescLevelMargin, tfTitle.Frame.Bottom, View.Frame.Width - (Margin * 2), View.Frame.Height - (tfTitle.Frame.Bottom));
				ContentHeight = tvDescription.Frame.Bottom;
			}
			else
			{
				tvDescription.Frame = new CGRect(Margin - tvDescLevelMargin, tfTitle.Frame.Bottom, View.Frame.Width - (Margin * 2), TextViewHeight);
				ContentHeight = tvDescription.Frame.Bottom;
			}
			if (ContentHeight > (View.Frame.Height - TopBarHeight))
				scrollView.ContentSize = new SizeF((float)scrollView.Frame.Width, (float)ContentHeight);
			else
				scrollView.ContentSize = new SizeF((float)scrollView.Frame.Width, (float)((View.Frame.Height - TopBarHeight) + 10));

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			AddObserver();
			// Perform any additional setup after loading the view
			View.Frame = frame;
			View.BackgroundColor = AppTheme.NTPageBGColor;


			SetTopBar();
			SetupImageScroller();

			tokenView = new TokenView(new CGRect(TokenMargin, imageScroller.Frame.Bottom, View.Frame.Width - (TokenMargin * 2), TextFieldHeight));
			InitTokenView();

			ContentHeight = tokenView.Frame.Bottom;
			scrollView = new UIScrollView();
			scrollView.Frame = new CGRect(View.Frame.X, TopBarHeight, View.Frame.Size.Width, View.Frame.Size.Height - TopBarHeight);
			scrollView.BackgroundColor = UIColor.Clear;


			tfTitle = new UITextField(new CGRect(Margin, imageScroller.Frame.Bottom + TextFieldHeight, View.Frame.Width - (Margin * 2), TextFieldHeight))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Placeholder = titlePlaceholderString,
				Font = AppTheme.NTtextfieldTitleFont,
				TextColor = AppTheme.NTtextfieldTitleFontColor
			};
			tfTitle.SetValueForKeyPath(UIColor.Gray, new NSString("_placeholderLabel.textColor"));
			tfTitle.BecomeFirstResponder();
			tfTitle.Started += delegate
			{
				Console.WriteLine("Started");
			};

			tfTitle.ShouldBeginEditing += delegate
			{
				Console.WriteLine("ShouldBeginEditing");
				return true;
			};

			tvDescription = new UITextView(new CGRect(Margin - tvDescLevelMargin, tfTitle.Frame.Bottom, View.Frame.Width - (Margin * 2), TextViewHeight))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Font = AppTheme.NTtextfieldDescFont,
				TextColor = AppTheme.NTtextfieldDescFontColor
			};

			UILabel lblDescPlaceholder = new UILabel(new CGRect(descPlaceholderMargin, descPlaceholderMargin, tvDescription.Frame.Width, descPlaceholderHeight))
			{
				TextColor = AppTheme.NTlblDescPlaceHolderTextColor,
				Font = AppTheme.NTlblDescPlaceHolderFont,
				Text = descPlaceholderString,
			};

			tvDescription.AddSubview(lblDescPlaceholder);

			tvDescription.Changed += (s, e) =>
			{
				if (!String.IsNullOrEmpty(tvDescription.Text))
					lblDescPlaceholder.Hidden = true;
				else
					lblDescPlaceholder.Hidden = false;
			};

			if (currentNote != null)
			{
				tfTitle.Text = currentNote.title;
				tvDescription.Text = currentNote.content;
				Title = currentNote.title ?? defaultBarTitle;
				if (!String.IsNullOrWhiteSpace(currentNote.tags_separarated))
				{
					try
					{
						_tokenSource.AddRange(currentNote.tags_separarated.Split('|').ToList());
						tokenView.ReloadData();
					}
					catch { }
				}
			}
			else
			{
				DataManager.AddEventInfo(AppDelegate.Connection, AnalyticsEventIds.new_note_opened, Helper.ToDateString(DateTime.Now));
			}

			if (!String.IsNullOrEmpty(tvDescription.Text))
				lblDescPlaceholder.Hidden = true;
			else
				lblDescPlaceholder.Hidden = false;

			View.ClipsToBounds = true;
			scrollView.AddSubviews(imageScroller, tfTitle, tvDescription, tokenView);
			View.AddSubviews(scrollView);

			ContentHeight = tvDescription.Frame.Bottom;
			scrollView.ContentSize = new SizeF((float)scrollView.Frame.Width, (float)ContentHeight);
			ViewWillLayoutSubviews();
		}

		private void SetupImageScroller()
		{
			imageScroller = new CarouselView(new CGRect(ImageBarMargin, TopBarHeight + 5, View.Frame.Width - ImageBarMargin * 2, ScrollViewHeight))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
			};
			//
			imageScroller.DataSource = new ImageDetailScrollerDatasource(this);
			imageScroller.Delegate = new ImageDetailScrollerDelegate(this);
			imageScroller.CarouselType = CarouselType.Linear;
			imageScroller.ConfigureView();
			imageScroller.Vertical = false;
			imageScroller.Frame = new CGRect(ImageBarMargin, TopBarHeight + 5, View.Frame.Width - ImageBarMargin * 2, ScrollViewHeight);
			ContentHeight = imageScroller.Frame.Bottom;
		}

		public void addImageClicked()
		{
			tfTitle.ResignFirstResponder();
			tvDescription.ResignFirstResponder();
			showImagePicker();
		}

		public void removeImageClicked(NotePhotos photo, NSIndexPath path)
		{
			currentNote.photos.Remove(photo);
			//            notePhotosSource.UpdateSource(currentNote.photos);
			imageScroller.ReloadData();
			shouldRefresh = true;
		}

		void HideKeyboard()
		{
			tfTitle.ResignFirstResponder();
			tvDescription.ResignFirstResponder();
		}

		public void AddObserver()
		{
			NSObject keyboardUpNotify;
			keyboardUpNotify = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillHideNotification"), (KeyboardDownNotification) =>
				{
					isDescriptionFrameChange = true;
					ViewWillLayoutSubviews();

				});
			keyboardUpNotify = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillShowNotification"), (KeyboardUpNotification) =>
				{
					isDescriptionFrameChange = false;
					ViewWillLayoutSubviews();

				});
		}

		private void SetTopBar()
		{
			UIView topBar = new UIView(new CGRect(0, 0, View.Frame.Width, TopBarHeight))
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin,
			};


			nfloat btnMargin = 10;
			nfloat btnCancelHeight = 40;
			nfloat btnSize = 40;
			string btnCancelString = AppTheme.CancelTextTitle;
			string topBarTitleEdit = AppTheme.NTtopBarTitleEdit;
			string topBarTitleNew = AppTheme.NTtopBarTitleNew;
			string btnSaveText = AppTheme.NTbtnSaveText;

			UIButton btnCancel = UIButton.FromType(UIButtonType.Custom);
			btnCancel.SetTitle(btnCancelString, UIControlState.Normal);
			btnCancel.Frame = new CGRect(btnMargin, btnMargin, btnCancelHeight * 2, btnCancelHeight);
			btnCancel.BackgroundColor = AppTheme.NDButtonBGGray;
			btnCancel.Font = AppTheme.NTbtnCancelFont;
			btnCancel.SetTitleColor(AppTheme.NTCancelButtonTextColor, UIControlState.Normal);

			btnCancel.TouchUpInside += (s, e) =>
			{
				HideKeyboard();
				AppDelegate.instance().rootViewController.closeDialogue();
				if (CancelClickedHandler != null)
				{
					CancelClickedHandler(shouldRefresh);
				}
			};

			UILabel topBarTitle = new UILabel(new CGRect(btnCancel.Frame.Right + btnMargin, 0, topBar.Frame.Width - (btnCancel.Frame.Right * 2 + (btnMargin * 2)), TopBarHeight))
			{
				BackgroundColor = UIColor.Clear,
				TextColor = AppTheme.NTtopbarTitleColor,
				Text = currentNote != null ? topBarTitleEdit : topBarTitleNew,
				Font = AppTheme.NTtopbarTitleFont,
				TextAlignment = UITextAlignment.Center,
			};

			UIButton btnSave = UIButton.FromType(UIButtonType.Custom);
			btnSave.SetTitle(btnSaveText, UIControlState.Normal);
			btnSave.BackgroundColor = AppTheme.NDBtnSaveBGColor;
			btnSave.Frame = new CGRect(View.Frame.Right - ((btnCancelHeight * 2) + btnMargin), btnMargin, btnCancelHeight * 2, btnCancelHeight);
			btnSave.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			btnSave.Font = AppTheme.NTbtnSaveFont;
			btnSave.SetTitleColor(UIColor.White, UIControlState.Normal);
			btnSave.TouchUpInside += (s, e) =>
			{
				if (String.IsNullOrWhiteSpace(tfTitle.Text))
				{
					new UIAlertView("", AppTheme.TitleCantBeEmptyText, null, AppTheme.DismissText).Show();
					return;
				}

				HideKeyboard();
				LoadingView.Show(string.Empty);

				var tags = _tokenSource.ToArray();
				if (currentNote == null)
				{
					currentNote = new BuiltNotes
					{
						title = tfTitle.Text,
						content = tvDescription.Text,
						tags = tags
					};
				}
				else
				{
					currentNote.title = tfTitle.Text;
					currentNote.content = tvDescription.Text;
					currentNote.tags = tags;
				}

				DataManager.saveNote(AppDelegate.Connection, currentNote, (err) =>
					{
						if (err == null)
						{
							InvokeOnMainThread(() =>
								{
									LoadingView.Dismiss();
									AppDelegate.instance().rootViewController.closeDialogue();

									if (NoteSavedHandler != null)
									{
										NoteSavedHandler();
									}
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
			};

			UIButton btnDelete = UIButton.FromType(UIButtonType.Custom);
			btnDelete.Frame = new CGRect(btnSave.Frame.Left - (btnSize), btnMargin, btnSize, btnSize);
			btnDelete.BackgroundColor = UIColor.Clear;
			btnDelete.Font = AppTheme.NTDeleteIconNameFont;
			btnDelete.SetTitleColor(AppTheme.NTDeleteIconNameNormalColor, UIControlState.Normal);
			btnDelete.SetTitleColor(AppTheme.NTDeleteIconNameSelectedColor, UIControlState.Selected);
			btnDelete.SetTitleColor(AppTheme.NTDeleteIconNameHighlightedColor, UIControlState.Highlighted);
			btnDelete.SetTitle(AppTheme.NTDeleteIconName, UIControlState.Normal);
			btnDelete.SetTitle(AppTheme.NTDeleteIconName, UIControlState.Selected);
			btnDelete.SetTitle(AppTheme.NTDeleteIconName, UIControlState.Highlighted);
			btnDelete.ContentMode = UIViewContentMode.ScaleAspectFit;
			btnDelete.ImageEdgeInsets = new UIEdgeInsets(btnMargin, btnMargin, btnMargin, btnMargin);

			btnDelete.TouchUpInside += (s, e) =>
			{
				var alert = new UIAlertView("", AppTheme.NTDeleteAlert, null, AppTheme.NoTextTitle, AppTheme.YesTextTitle);
				alert.Clicked += (sender, args) =>
				{
					if (args.ButtonIndex == 1)
					{
						HideKeyboard();

						LoadingView.Show(string.Empty);
						DataManager.deleteNote(AppDelegate.Connection, currentNote.uid, (err) =>
							{
								if (err == null)
								{
									InvokeOnMainThread(() =>
										{
											LoadingView.Dismiss();
											AppDelegate.instance().rootViewController.closeDialogue();

											if (NoteDeletedHandler != null)
											{
												NoteDeletedHandler();
											}
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


			btnDelete.Hidden = currentNote == null;

			LineView lv = new LineView(new CGRect(0, topBar.Frame.Bottom - 1, topBar.Frame.Width, AppTheme.NTSeparatorBorderWidth))
			{
				BackgroundColor = AppTheme.NTtopBarColor
			};

			topBar.AddSubviews(btnCancel, topBarTitle, btnDelete, btnSave, lv);
			View.AddSubview(topBar);
			ContentHeight = topBar.Frame.Bottom;
		}

		private void showImagePicker()
		{
			imagePicker = new UIImagePickerController();
//			imagePicker.SupportedInterfaceOrientations = (a) =>
//			{
//				return UIInterfaceOrientationMask.LandscapeLeft | UIInterfaceOrientationMask.LandscapeRight;
//			};
			// set our source to the photo library
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = new[] { "public.image" };
			imagePicker.FinishedPickingMedia += imagePicker_FinishedPickingMedia;
			imagePicker.Canceled += imagePicker_Canceled;

			AppDelegate.instance().rootViewController.popOverController = new UIPopoverController(imagePicker);

			AppDelegate.instance().rootViewController.popOverController.PresentFromRect(imageScroller.Bounds, imageScroller, UIPopoverArrowDirection.Any, false);

		}

		void imagePicker_Canceled(object sender, EventArgs e)
		{

		}

		void imagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			AppDelegate.instance ().rootViewController.popOverController.Dismiss (false);
			LoadingView.Show(string.Empty);
			if (e.OriginalImage == null)
				return;

			var filePath = e.Info.ObjectForKey(UIImagePickerController.ReferenceUrl);

			var str = filePath.ToString().Replace("ext=", "ext=.");

			var data = Helper.ImageToByteArray(e.OriginalImage);
			DataManager.UploadFile(str, data, AppSettings.ApplicationUser, res =>
				{
					InvokeOnMainThread(() =>
						{
							LoadingView.Dismiss();
						});

					if (res == null)
						return;

					if (currentNote == null)
						currentNote = new BuiltNotes();

					if (currentNote.photos == null)
						currentNote.photos = new List<NotePhotos>();

					currentNote.photos.Add(new NotePhotos
						{
							uid = res.result[0].uid,
							url = res.result[0].url,
						});

					shouldRefresh = true;

					InvokeOnMainThread(() =>
						{
							//                        notePhotosSource.UpdateSource(currentNote.photos);
							imageScroller.ReloadData();
						});
				});
		}

		private sealed class NameSelectorDelegate : TokenViewDelegate
		{
			private List<string> _tokenSource;
			private NoteDetailsController _controller;

			public NameSelectorDelegate(NoteDetailsController controller)
			{
				_tokenSource = controller._tokenSource;
				_controller = controller;
			}

			public override void FilterToken(TokenView tokenView, string text)
			{
				if (!String.IsNullOrWhiteSpace(text) && text.EndsWith(" "))
				{
					_tokenSource.Add(text.Trim());
					_controller.tokenView.ReloadData();
				}
			}

			public override void DidEnterToken(TokenView tokenView, string text)
			{
				_tokenSource.Add(text);
				_controller.tokenView.ReloadData();
			}

			public override void DidDeleteTokenAtIndex(TokenView tokenView, int index)
			{
				_tokenSource.RemoveAt(index);
				_controller.tokenView.ReloadData();
			}
		}

		private sealed class NameSelectorSource : TokenViewSource
		{

			private List<string> _tokenSource;

			public NameSelectorSource(NoteDetailsController controller)
			{
				_tokenSource = controller._tokenSource;
			}

			public override string GetToken(TokenView tokenView, int index)
			{
				return _tokenSource[index];
			}

			public override int NumberOfTokens(TokenView tokenView)
			{
				return _tokenSource.Count;
			}
		}
	}
}
