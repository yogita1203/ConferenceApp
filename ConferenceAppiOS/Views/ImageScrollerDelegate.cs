using System;
using Alliance.Carousel;
using ConferenceAppiOS.Notes;

namespace ConferenceAppiOS
{
	public class ImageScrollerDelegate: CarouselViewDelegate
	{
		NotesTableController vc;

		public ImageScrollerDelegate(NotesTableController vc)
		{
			this.vc = vc;
		}

		public override nfloat ValueForOption(CarouselView carousel, CarouselOption option, nfloat aValue)
		{
			if (option == CarouselOption.Spacing)
			{
				return aValue * 1.05f;
			}
			return aValue;
		}


		public override void DidSelectItem(CarouselView carousel, nint index)
		{
//			Console.WriteLine("Selected: " + ++index);

		}
	}
}

