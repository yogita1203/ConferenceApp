using BuiltSDK;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltNotes
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }

        public string content { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<NotePhotos> photos { get; set; }

        public string title { get; set; }

        [Ignore]
        public string[] tags { get; set; }
        public string tags_separarated { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [Ignore]
        public BuiltACL ACL { get; set; }
    }

    public class NotePhotos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltNotes))]
        public int BuiltNotesId { get; set; }

        [ManyToOne]
        public BuiltNotes BuiltNotes { get; set; }

        //public string app_user_object_uid { get; set; }
        //public string content_type { get; set; }
        //public string file_size { get; set; }
        //public string filename { get; set; }

        //[Ignore]
        //public string[] tags { get; set; }
        //public string sqlitetags { get; set; }
        public string uid { get; set; }
        public string url { get; set; }
    }
}
