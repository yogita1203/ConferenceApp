using System;
using Alliance.Carousel;
using ConferenceAppiOS.Notes;

namespace ConferenceAppiOS
{
	public class ImageDetailScrollerDelegate: CarouselViewDelegate
	{
		NoteDetailsController noteDetailsController;

		public ImageDetailScrollerDelegate(NoteDetailsController vc)
		{
			noteDetailsController = vc;
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
//			base.DidSelectItem (carouselView, index);
		}

	}
}

