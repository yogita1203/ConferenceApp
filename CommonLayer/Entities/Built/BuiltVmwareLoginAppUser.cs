using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using CommonLayer.Entities;

namespace CommonLayer.Entities.Built
{
    public class BuiltVmwareLoginAppUser
    {
        public string uid { get; set; }
        public BuiltAppUser application_user;
        public string userName;
        public string password;
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltLoginOutput
    {
        public string uid { get; set; }
        public BuiltVmwareLoginAppUser output;
        public Exception error;
        public int errorNumber;
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}