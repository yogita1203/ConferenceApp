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
using System.Drawing;

namespace ConferenceAppDroid.Utilities
{
    public static class ColorExtension
    {
        public static Android.Graphics.Color FromHexString(string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace("#", "");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            int red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f);
                        green = Convert.ToInt32(Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f);
                        blue = Convert.ToInt32(Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f);
                        return Android.Graphics.Color.Argb(Convert.ToInt32(alpha), red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f);
                        green = Convert.ToInt32(Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f);
                        blue = Convert.ToInt32(Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f);
                        return Android.Graphics.Color.Argb(Convert.ToInt32(alpha), red, green, blue);
                    }

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));
            }
        }
    }
}