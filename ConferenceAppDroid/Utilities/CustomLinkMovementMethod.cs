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
using Android.Text.Method;
using Android.Text;
using Android.Text.Style;

namespace ConferenceAppDroid.Utilities
{
    public class CustomLinkMovementMethod : LinkMovementMethod
    {
        public Context movementContext;
        private static CustomLinkMovementMethod linkMovementMethod = new CustomLinkMovementMethod();

        public override bool OnTouchEvent(TextView widget, Android.Text.ISpannable buffer, MotionEvent e)
        {
            int action = e.ActionIndex;
            if (action == e.ACTION_UP) 
            {
            int x = (int) e.GetX();
            int y = (int) e.GetY();

            x -= widget.TotalPaddingLeft;
            y -= widget.TotalPaddingTop;

            x += widget.ScrollX;
            y += widget.ScrollY;

            Layout layout = widget.Layout;
            int line = layout.GetLineForVertical(y);
            int off = layout.GetOffsetForHorizontal(line, x);

            URLSpan[] link = ((URLSpan[])buffer.GetSpans(off, off, Java.Lang.Class.FromType(typeof(StyleSpan))));
            if (link.Length != 0) {
                String url = link[0].URL;
                if (url.StartsWith("https") || url.StartsWith("http") )
                {

                    Intent intent = new Intent(movementContext, Java.Lang.Class.FromType(typeof(UIWebView)));
                    intent.PutExtra("url",url);
                    movementContext.StartActivity(intent);
                } 
                else if (url.StartsWith("vmwareapp:"))
                {
                    Console.WriteLine();
                    //AppUtilities.callStartActivity(movementContext, false, url, url, true);
                }
                return true;
            }
        }
            base.OnTouchEvent(widget, buffer, e);
        }

    }

}