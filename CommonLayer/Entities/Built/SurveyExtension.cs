using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLayer.Entities.Built
{
    public class SurveyExtension
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string survey_id { get; set; }
        public string name { get; set; }
        [Ignore]
        public string id { get; set; }
        [Ignore]
        public string mobile_url { get; set; }
        [Ignore]
        public string url { get; set; }
        [Ignore]
        public string abbreviation { get; set; }
        [Ignore]
        public string session_id { get; set; }
        [Ignore]
        public string session_time_id { get; set; }
        [Ignore]
        public string session_title { get; set; }
    }

    public class Surveys
    {
        public List<SurveyExtension> survey { get; set; }
    }

    public class SurveyApi
    {
        public Surveys surveys { get; set; }
    }

    //---------------------------------------------------------------

    public class SurveySingle
    {
        public SurveyExtension survey { get; set; }
    }

    public class SurveyApiSingle
    {
        public SurveySingle surveys { get; set; }
    }

    //---------------------------------------------------------------

    public class SurveyApiNone
    {
        public string surveys { get; set; }
    }
}