using System;
using Alliance.Carousel;
using ConferenceAppiOS.Notes;
using UIKit;
using CoreGraphics;
using System.Linq;
using CommonLayer.Entities.Built;
using Foundation;

namespace ConferenceAppiOS
{
	public class ImageDetailScrollerDatasource : CarouselViewDataSource
	{
		static nfloat btnMargin = 10;
		static nfloat borderWidth = 4;
		static nfloat RowHeight = 175;

		NoteDetailsController notesTableController;

		public ImageDetailScrollerDatasource(NoteDetailsController vc)
		{
			notesTableController = vc;
		}

		public override nint NumberOfItems(CarouselView carousel)
		{
			if (notesTableController.currentNote == null) {
				int itemscount = 1;
				return (nint)itemscount;
			} else {
				return (nint)notesTableController.currentNote.photos.Count + 1;
			}		
		}

		public override UIView ViewForItem (CarouselView carouselView, nint index, UIView reusingView)
		{
			if (notesTableController.currentNote == null || index == notesTableController.currentNote.photos.Count || notesTableController.currentNote.photos.Count == 0)
			{
				UIView view = new UIView(new CGRect(0, 0, RowHeight, RowHeight))
				{
					AutoresizingMask = UIViewAutoresizing.FlexibleMargins,
				};
				var btnAddImage = UIButton.FromType(UIButtonType.Custom);
				btnAddImage.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
				btnAddImage.BackgroundColor = UIColor.White;
				btnAddImage.Frame = new CGRect(btnMargin, btnMargin, NotePhotoCell.ImageSize, NotePhotoCell.ImageSize);
				btnAddImage.Layer.BorderColor = UIColor.LightGray.CGColor;
				btnAddImage.Layer.BorderWidth = borderWidth;
				btnAddImage.Font = AppFonts.ProximaNovaSemiBold(24);
				btnAddImage.SetTitle("+", UIControlState.Normal);
				btnAddImage.SetTitleColor(UIColor.Black, UIControlState.Normal);
				btnAddImage.TouchUpInside += (s, e) =>
				{
					//if (AddImageHandler != null)
					//{
					//    AddImageHandler();
					//}
					notesTableController.addImageClicked();
				};

				view.AddSubview(btnAddImage);
				reusingView = view;
				return reusingView;
			}
			else
			{
				NotePhotoCell cell = new NotePhotoCell(new CGRect(5, 5, RowHeight, RowHeight));
				if (reusingView == null)
				{
					reusingView = cell;
				}

				NotePhotos note = notesTableController.currentNote.photos.ElementAt((int)index);
				NotePhotoCell celll = (NotePhotoCell)reusingView;
				celll.BtnRemove.TouchUpInside += (s, e) =>
				{
					notesTableController.removeImageClicked(note, null);
				};

				celll.UpdateCell(note, true);

				return reusingView;
			}
		}

	}
}
