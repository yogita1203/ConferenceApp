using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class ISSession
    {
        public string abbreviation { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }

    public class Sessions
    {
        public List<ISSession> session { get; set; }
    }

    public class Interests
    {
        public Sessions sessions { get; set; }
    }

    public class ISApi
    {
        public Interests interests { get; set; }
    }

    public class MyInterestSessionExtensionList
    {
        public ISApi api { get; set; }
    }

    /*-----------------------------------------------------------------------*/

    public class SessionsSingle
    {
        public ISSession session { get; set; }
    }

    public class InterestsSingle
    {
        public SessionsSingle sessions { get; set; }
    }

    public class ISApiSingle
    {
        public InterestsSingle interests { get; set; }
    }

    public class MyInterestSessionExtensionSingle
    {
        public ISApiSingle api { get; set; }
    }

    ///*-----------------------------------------------------------------------*/

    public class InterestsNone
    {
        public string sessions { get; set; }
    }

    public class ISApiNone
    {
        public InterestsNone interests { get; set; }
    }

    public class MyInterestSessionExtensionNone
    {
        public ISApiNone api { get; set; }
    }
}
