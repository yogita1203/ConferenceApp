using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace ConferenceAppDroid.CustomControls
{
    public class SegmentedControlButton : RadioButton
    {
        private float mX;
        public bool isFromSessionScreen = false;

        public SegmentedControlButton(Context context)
            : base(context)
        { }

        public SegmentedControlButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
        { }

        public SegmentedControlButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        { }

        //private static readonly float TEXT_SIZE = 16.0f;

        protected override void OnDraw(Android.Graphics.Canvas canvas)
        {
            string text = this.Text;
            Paint textPaint = new Paint();
            textPaint.AntiAlias = true;
            float currentWidth = textPaint.MeasureText(text);
            float currentHeight = textPaint.MeasureText("x");

            // final float scale =
            // getContext().getResources().getDisplayMetrics().density;
            // float textSize = (int) (TEXT_SIZE * scale + 0.5f);
            textPaint.TextSize = this.TextSize;
            textPaint.TextAlign = Android.Graphics.Paint.Align.Center;

            float canvasWidth = canvas.Width;
            float textWidth = textPaint.MeasureText(text);

            if (Checked)
            {
                //GradientDrawable grad = new GradientDrawable(GradientDrawable.Orientation.TopBottom, new int[] { Convert.ToInt32("0xffdcdcdc", 16), Convert.ToInt32("0xff111111", 16) });
                ColorDrawable grad = new ColorDrawable(Color.White);
                grad.SetBounds(0, 0, this.Width, this.Height);
                grad.Draw(canvas);
                //textPaint.Color = Color.White;
                textPaint.Color = Color.ParseColor("#000000");
                if(isFromSessionScreen)
                {
                    var paintTemp = new Paint() { Color = Color.DarkSlateGray };
                    paintTemp.StrokeWidth = 20;
                    canvas.DrawLine(0, this.Height, this.Width, this.Height, paintTemp);
                }
                
            }
            else
            {
                //GradientDrawable grad = new GradientDrawable(GradientDrawable.Orientation.TopBottom, new int[] { Convert.ToInt32("0xffa5a5a5", 16), Convert.ToInt32("0xff000000", 16) });
                ColorDrawable grad = new ColorDrawable(Color.LightGray);
                grad.SetBounds(0, 0, this.Width, this.Height);
                grad.Draw(canvas);
                //textPaint.Color = Color.ParseColor("#ffcccccc");
                textPaint.Color = Color.ParseColor("#000000");
            }

            float w = (this.Width / 2) - currentWidth;
            float h = (this.Height / 2) + currentHeight;
            if (!text.Contains(',') || text.Trim().StartsWith(",") || text.Trim().EndsWith(","))
                canvas.DrawText(text, mX, h, textPaint);
            else
            {
                var arr = text.Split(',');
                canvas.DrawText(arr[0], mX, h - (currentHeight+15), textPaint);
                canvas.DrawText(arr[1], mX, h + (currentHeight + 15), textPaint);
            }

            Paint paint = new Paint();
            paint.Color = Color.DarkGray;
            paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
            Rect rect = new Rect(0, 0, this.Width, this.Height);
            canvas.DrawRect(rect, paint);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            mX = w * 0.5f; // remember the center of the screen
        }
    }
}