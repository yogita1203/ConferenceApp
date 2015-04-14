using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltAgendaItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltAgenda))]
        public int BuiltAgendaId { get; set; }

        [ManyToOne]
        public BuiltAgenda BuiltAgenda { get; set; }

        public string uid { get; set; }
        public string agenda_day_id { get; set; }
        public string agenda_item_id { get; set; }
        public string description { get; set; }
        public string end_time { get; set; }
        public string name { get; set; }
        public string start_time { get; set; }
        public string location { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
