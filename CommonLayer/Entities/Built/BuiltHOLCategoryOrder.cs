﻿using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltHOLCategoryOrder
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(BuiltConfig))]
        public int BuiltConfigId { get; set; }
        [OneToOne]
        public BuiltConfig BuiltConfig { get; set; }
        public string uid { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltHOLCategoryOrderElement> hol_category_order { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
