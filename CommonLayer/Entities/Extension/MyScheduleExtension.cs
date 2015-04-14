using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class Item
    {
        public string sessiontime_id { get; set; }
        public string session_id { get; set; }
    }

    public class _Schedule
    {
        public List<Item> item { get; set; }
    }

    public class Api
    {
        public _Schedule schedule { get; set; }
    }

    public class MyScheduleExtensionList
    {
        public Api api { get; set; }
    }

    /*-----------------------------------------------------------------------*/

    public class _ScheduleSingle
    {
        public Item item { get; set; }
    }

    public class ApiSingle
    {
        public _ScheduleSingle schedule { get; set; }
    }

    public class MyScheduleExtensionSingle
    {
        public ApiSingle api { get; set; }
    }

    /*-----------------------------------------------------------------------*/

    //public class _ScheduleNone
    //{
    //    public Item item { get; set; }
    //}

    public class ApiNone
    {
        public string schedule { get; set; }
    }

    public class MyScheduleExtensionNone
    {
        public ApiNone api { get; set; }
    }
}
