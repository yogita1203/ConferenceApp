using System;
using Alliance.Carousel;
using ConferenceAppiOS.Notes;
using UIKit;
using CoreGraphics;
using System.Linq;
using CommonLayer.Entities.Built;

namespace ConferenceAppiOS
{
		public class ImageScrollerDatasource : CarouselViewDataSource
		{
		static nfloat RowHeight = 175;
			
			NotesTableController notesTableController;

			public ImageScrollerDatasource(NotesTableController vc)
			{
				notesTableController = vc;
			}

			public override nint NumberOfItems(CarouselView carousel)
			{
				if (notesTableController.currentNote != null && notesTableController.currentNote.photos.Count > 0)
					return (nint)notesTableController.currentNote.photos.Count;
				else
					return 0;
			}

		public override UIView ViewForItem (CarouselView carouselView, nint index, UIView reusingView)
		{	
				if (reusingView == null) {
					NotePhotoCell cell = new NotePhotoCell (new CGRect(5, 5, RowHeight, RowHeight));
					reusingView = cell;
				} 
				NotePhotos note = notesTableController.currentNote.photos.ElementAt ((int)index);
				NotePhotoCell celll = (NotePhotoCell)reusingView;	
				celll.UpdateCell (note, false);

				return reusingView;
			}
		}
}