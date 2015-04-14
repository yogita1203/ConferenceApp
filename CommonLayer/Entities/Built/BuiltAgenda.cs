using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltAgenda
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string agenda_day_id { get; set; }
        public string agenda_date { get; set; }
        public string date_string { get; set; }
        public string name { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public BuiltAgendaItem[] item { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
