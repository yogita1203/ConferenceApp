using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceAppDroid.CustomControls
{
    public class SlidingTabStrip : LinearLayout
    {
        private int[] mIndicatorColors;
        private int[] mDividerColors;
        private const int DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS = 2;
        private const byte DEFAULT_BOTTOM_BORDER_COLOR_ALPHA = 0x26;
        private const int SELECTED_INDICATOR_THICKNESS_DIPS = 8;
        private static int temp =Convert.ToInt32(0xFF33B5E5);
        private int DEFAULT_SELECTED_INDICATOR_COLOR = Convert.ToInt32(0xFF33B5E5);

        private const int DEFAULT_DIVIDER_THICKNESS_DIPS = 1;
        private const byte DEFAULT_DIVIDER_COLOR_ALPHA = 0x20;
        private const float DEFAULT_DIVIDER_HEIGHT = 0.5f;

        private int mBottomBorderThickness;
        private Paint mBottomBorderPaint;

        private int mSelectedIndicatorThickness;
        private Paint mSelectedIndicatorPaint;

        private int mDefaultBottomBorderColor;

        private Paint mDividerPaint;
        private float mDividerHeight;

        private int mSelectedPosition;
        private float mSelectionOffset;

        public SlidingTabLayout.TabColorizer mCustomTabColorizer;
        private SimpleTabColorizer mDefaultTabColorizer;

        public SlidingTabStrip(Context context)
            : base(context, null)
        {

        }
        SlidingTabStrip(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            SetWillNotDraw(false);

            float density = context.Resources.DisplayMetrics.Density;

            TypedValue outValue = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
            int themeForegroundColor = outValue.Data;

            mDefaultBottomBorderColor = setColorAlpha(themeForegroundColor,
                    DEFAULT_BOTTOM_BORDER_COLOR_ALPHA);

            mDefaultTabColorizer = new SimpleTabColorizer();
            mDefaultTabColorizer.setIndicatorColors(DEFAULT_SELECTED_INDICATOR_COLOR);
            mDefaultTabColorizer.setDividerColors(setColorAlpha(themeForegroundColor,
                    DEFAULT_DIVIDER_COLOR_ALPHA));

            mBottomBorderThickness = (int)(DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS * density);
            mBottomBorderPaint = new Paint();
            //mBottomBorderPaint.Color = mDefaultBottomBorderColor;
            mBottomBorderPaint.Color = Color.DarkGray;

            mSelectedIndicatorThickness = (int)(SELECTED_INDICATOR_THICKNESS_DIPS * density);
            mSelectedIndicatorPaint = new Paint();

            mDividerHeight = DEFAULT_DIVIDER_HEIGHT;
            mDividerPaint = new Paint();
            mDividerPaint.StrokeWidth = ((int)(DEFAULT_DIVIDER_THICKNESS_DIPS * density));
        }

        public void setCustomTabColorizer(SlidingTabLayout.TabColorizer customTabColorizer)
        {
            mCustomTabColorizer = customTabColorizer;
            Invalidate();
        }

        public void setSelectedIndicatorColors(int colors)
        {
            // Make sure that the custom colorizer is removed
            mCustomTabColorizer = null;
            mDefaultTabColorizer.setIndicatorColors(colors);
            Invalidate();
        }

        public void setDividerColors(int colors)
        {
            // Make sure that the custom colorizer is removed
            mCustomTabColorizer = null;
            mDefaultTabColorizer.setDividerColors(colors);
            Invalidate();
        }

        void onViewPagerPageChanged(int position, float positionOffset)
        {
            mSelectedPosition = position;
            mSelectionOffset = positionOffset;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int childCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(0f, mDividerHeight), 1f) * height);
            SlidingTabLayout.TabColorizer tabColorizer = mCustomTabColorizer != null
                    ? mCustomTabColorizer
                    : mDefaultTabColorizer;

            // Thick colored underline below the current selection
            if (childCount > 0)
            {
                View selectedTitle = GetChildAt(mSelectedPosition);
                int left = selectedTitle.Left;
                int right = selectedTitle.Right;
                int color = tabColorizer.getIndicatorColor(mSelectedPosition);

                if (mSelectionOffset > 0f && mSelectedPosition < (childCount - 1))
                {
                    int nextColor = tabColorizer.getIndicatorColor(mSelectedPosition + 1);
                    if (color != nextColor)
                    {
                        color = blendColors(nextColor, color, mSelectionOffset);
                    }

                    // Draw the selection partway between the tabs
                    View nextTitle = GetChildAt(mSelectedPosition + 1);
                    left = (int)(mSelectionOffset * nextTitle.Left +
                            (1.0f - mSelectionOffset) * left);
                    right = (int)(mSelectionOffset * nextTitle.Right +
                            (1.0f - mSelectionOffset) * right);
                }

                mSelectedIndicatorPaint.Color = Color.ParseColor((color).ToString());

                canvas.DrawRect(left, height - mSelectedIndicatorThickness, right,
                        height, mSelectedIndicatorPaint);
            }

            // Thin underline along the entire bottom edge
            canvas.DrawRect(0, height - mBottomBorderThickness, Width, height, mBottomBorderPaint);

            // Vertical separators between the titles
            int separatorTop = (height - dividerHeightPx) / 2;
            for (int i = 0; i < childCount - 1; i++)
            {
                View child = GetChildAt(i);
                mDividerPaint.Color = Color.ParseColor(tabColorizer.getDividerColor(i).ToString());
                canvas.DrawLine(child.Right, separatorTop, child.Right,
                        separatorTop + dividerHeightPx, mDividerPaint);
            }
        }
        private static int setColorAlpha(int color, byte alpha)
        {
            return Color.Argb(alpha, Color.Red, Color.Green, Color.Blue);
        }

        /**
         * Blend {@code color1} and {@code color2} using the given ratio.
         *
         * @param ratio of which to blend. 1.0 will return {@code color1}, 0.5 will give an even blend,
         *              0.0 will return {@code color2}.
         */
        private static int blendColors(int color1, int color2, float ratio)
        {
            float inverseRation = 1f - ratio;
            float r = (Color.Red * ratio) + (Color.Red * inverseRation);
            float g = (Color.Green * ratio) + (Color.Green * inverseRation);
            float b = (Color.Blue * ratio) + (Color.Blue * inverseRation);
            return Color.Rgb((int)r, (int)g, (int)b);
        }

        public class SimpleTabColorizer : SlidingTabLayout.TabColorizer
        {
            private int[] mIndicatorColors;
            private int[] mDividerColors;



            public int getIndicatorColor(int position)
            {
                return mIndicatorColors[position % mIndicatorColors.Length];
            }

            public int getDividerColor(int position)
            {
                return mDividerColors[position % mDividerColors.Length];
            }
            public void setIndicatorColors(params int[] colors)
            {
                mIndicatorColors = colors;
            }

            public void setDividerColors(params int[] colors)
            {
                mDividerColors = colors;
            }
        }


        //public int getIndicatorColor(int position)
        //{
        //    return mIndicatorColors[position % mIndicatorColors.Length];
        //}

        //public int getDividerColor(int position)
        //{
        //    return mDividerColors[position % mDividerColors.Length];
        //}
        //void setIndicatorColors(int colors)
        //{
        //    mIndicatorColors[0] = colors;
        //}

        //void setDividerColors(int colors)
        //{
        //    mDividerColors[0] = colors;
        //}
    }
}
