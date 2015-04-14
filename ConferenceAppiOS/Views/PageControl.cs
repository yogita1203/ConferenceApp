using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreGraphics;

namespace ConferenceAppiOS.Views
{
    public enum PageControlType
    {
        OnFullOffFull,
        OnFullOffEmpty,
        OnEmptyOffFull,
        OnEmptyOffEmpty
    }

    public class PageControl : UIControl
    {
        private int numberOfPages;
        public int Pages
        {
            get
            {
                return numberOfPages;
            }

            set
            {
                // make sure the number of pages is positive
                numberOfPages = Math.Max(0, value);

                // we then need to update the current page
                currentPage = Math.Min(Math.Max(0, currentPage), numberOfPages - 1);

                // correct the bounds accordingly
                this.Bounds = this.Bounds;

                // we need to redraw
                this.SetNeedsDisplay();

                // depending on the user preferences, we hide the page control with a single element
                if (HidesForSinglePage && (numberOfPages < 2))
                    this.Hidden = true;
                else
                    this.Hidden = false;
            }
        }

        private int currentPage;
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }

            set
            {
                // no need to update in that case
                if (currentPage == value)
                    return;

                // determine if the page number is in the available range
                currentPage = Math.Min(Math.Max(0, value), numberOfPages - 1);

                this.SetNeedsDisplay();
            }
        }

        private bool hidesForSinglePage;
        public bool HidesForSinglePage
        {
            get
            {
                return hidesForSinglePage;
            }

            set
            {
                hidesForSinglePage = value;

                // depending on the user preferences, we hide the page control with a single element
                if (hidesForSinglePage && (numberOfPages < 2))
                    this.Hidden = true;
            }
        }


        public bool DefersCurrentPageDisplay { get; set; }
        public PageControlType ControlType { get; set; }
        public UIColor OnColor { get; set; }
        public UIColor OffColor { get; set; }

        public nfloat IndicatorDiameter { get; set; }
        public nfloat IndicatorSpace { get; set; }

		private static nfloat kDotDiameter = 10.0f;
		private static nfloat kDotSpace = 12.0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ieMobile.PageControl"/> class.
        /// </summary>
        /// <param name='ct'>
        /// Ct.
        /// </param>
        /// <param name='frame'>
        /// Frame.
        /// </param>
        public PageControl(PageControlType ct, CGRect rect)
            : base(rect)
        {
            ControlType = ct;
            DefersCurrentPageDisplay = false;

            this.BackgroundColor = UIColor.Clear;
        }

        /// <summary>
        /// Sizes for number of pages.
        /// </summary>
        /// <returns>
        /// The for number of pages.
        /// </returns>
        /// <param name='pageCount'>
        /// Page count.
        /// </param>
        public CGSize SizeForNumberOfPages(int pageCount)
        {
            nfloat diameter = (IndicatorDiameter > 0) ? IndicatorDiameter : kDotDiameter;
            nfloat space = (IndicatorSpace > 0) ? IndicatorSpace : kDotSpace;

			return new CGSize(pageCount * diameter + (pageCount - 1) * space + 44.0f, (nfloat)Math.Max(44.0f, diameter + 4.0f));
        }

        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name='area'>
        /// Area.
        /// </param>
        /// <param name='formatter'>
        /// Formatter.
        /// </param>
        public override void Draw(CGRect area)
        {
            base.Draw(area);

            // get the current context
            CGContext context = UIGraphics.GetCurrentContext();

            // save the context
            context.SaveState();

            // allow antialiasing
            context.SetAllowsAntialiasing(true);

            // get the caller's diameter if it has been set or use the default one 
            nfloat diameter = (IndicatorDiameter > 0) ? IndicatorDiameter : kDotDiameter;
            nfloat space = (IndicatorSpace > 0) ? IndicatorSpace : kDotSpace;

            // geometry
            CGRect currentBounds = this.Bounds;
            nfloat dotsWidth = this.Pages * diameter + Math.Max(0, this.Pages - 1) * space;
            nfloat x = currentBounds.GetMidX() - dotsWidth / 2;
            nfloat y = currentBounds.GetMidY() - diameter / 2;

            // get the caller's colors it they have been set or use the defaults
            CGColor onColorCG = OnColor != null ? OnColor.CGColor : UIColor.FromWhiteAlpha(1.0f, 1.0f).CGColor;
            CGColor offColorCG = OffColor != null ? OffColor.CGColor : UIColor.FromWhiteAlpha(0.7f, 0.5f).CGColor;

            // actually draw the dots
            for (int i = 0; i < Pages; i++)
            {
                CGRect dotRect = new CGRect(x, y, diameter, diameter);

                if (i == CurrentPage)
                {
                    if (ControlType == PageControlType.OnFullOffFull || ControlType == PageControlType.OnFullOffEmpty)
                    {
						context.SetStrokeColor(onColorCG);
                        context.FillEllipseInRect(dotRect.Inset(-1.0f, -1.0f));
                    }
                    else
                    {
						context.SetStrokeColor(onColorCG);
                        context.StrokeEllipseInRect(dotRect);
                    }
                }
                else
                {
                    if (ControlType == PageControlType.OnEmptyOffEmpty || ControlType == PageControlType.OnFullOffEmpty)
                    {
						context.SetStrokeColor(offColorCG);
                        context.StrokeEllipseInRect(dotRect);
                    }
                    else
                    {
						context.SetStrokeColor(offColorCG);
                        context.FillEllipseInRect(dotRect.Inset(-1.0f, -1.0f));
                    }
                }

                x += diameter + space;
            }

            // restore the context
            context.RestoreState();
        }

        /// <summary>
        /// Toucheses the ended.
        /// </summary>
        /// <param name='touches'>
        /// Touches.
        /// </param>
        /// <param name='evt'>
        /// Evt.
        /// </param>
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            // get the touch location
            UITouch theTouch = touches.AnyObject as UITouch;
            CGPoint touchLocation = theTouch.LocationInView(this);

            // check whether the touch is in the right or left hand-side of the control
            if (touchLocation.X < (this.Bounds.Size.Width / 2))
                this.CurrentPage = Math.Max(this.CurrentPage - 1, 0);
            else
                this.CurrentPage = Math.Min(this.CurrentPage + 1, Pages - 1);

            // send the value changed action to the target
            this.SendActionForControlEvents(UIControlEvent.ValueChanged);
        }
    }
}