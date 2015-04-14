using System;
using UIKit;
using CoreGraphics;
using CoreGraphics;

namespace ConferenceAppiOS
{
	public class UIViewWithShadow : UIImageView
	{
		public enum ShadowSide {
			left,
			right,
			top,
			bottom
		}

		public ShadowSide shadowSide;

		public UIViewWithShadow (ShadowSide side)
		{
			shadowSide = side;
			BackgroundColor = UIColor.Clear;
			Opaque = false;
			AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			CGRect fillRect = CGRect.Empty;
			CGSize offset = CGSize.Empty;
			if (shadowSide == ShadowSide.bottom) {
				offset = new CGSize (0, 2);
				fillRect =  new CGRect(0,Frame.Size.Height+1,Frame.Size.Width,1);
			} else if (shadowSide == ShadowSide.top) {
				offset = new CGSize (0, -2);
				fillRect =  new CGRect(0, Frame.Size.Height-1, Frame.Size.Width, 1);
			} else if (shadowSide == ShadowSide.left) {
				offset = new CGSize (-2,0);
				fillRect =  new CGRect(Frame.Size.Width-1, 0, 1, Frame.Size.Height);
			} else if (shadowSide == ShadowSide.right) {
				offset = new CGSize (2, 0);
				fillRect =  new CGRect(0, 0, 1, Frame.Size.Height);
			}

			CGSize bitmapSize = new CGSize (Frame.Size);
			using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero, (int)bitmapSize.Width, (int)bitmapSize.Height, 8, (int)(4 * bitmapSize.Width), CGColorSpace.CreateDeviceRGB (), CGImageAlphaInfo.PremultipliedFirst)) {
				//==== create a grayscale shadow
				// 1) save graphics state
				context.SaveState ();
				// 2) set shadow context for offset and blur
				// context.SetShadow (new SizeF (-5, 0), 15);
				context.SetShadow (offset,10.0f, UIColor.Black.CGColor);
				// 3) perform your drawing operation
				context.SetFillColor(UIColor.White.CGColor);
				//				context.FillRect (new CGRect (100, 600, 300, 250));
				context.FillRect (fillRect);
				// 4) restore the graphics state
				context.RestoreState ();
				// output the drawing to the view
				Image = UIImage.FromImage (context.ToImage ());
			}
		}


	}
}
