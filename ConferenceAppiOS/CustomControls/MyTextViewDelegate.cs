using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace ConferenceAppiOS
{
    public class MyTextViewDelegate : UITextViewDelegate
    {
        REComposeSheetView _controller = null;

        public MyTextViewDelegate(REComposeSheetView controller)
        {
            _controller = controller;
        }
       

         
        
    }
}