using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltLegal
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        //public object gaming_rules { get; set; }
        //public object license_agreement { get; set; }
        //public object open_source_license_android { get; set; }
        //public object open_source_license_ios { get; set; }
        //public object policy { get; set; }
        //public object tnc { get; set; }
        //public object trademark_copyright { get; set; }
        public string platform { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public GamingRules gaming_rules { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public LicenseAgreement license_agreement { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public OpenSourceLicenseAndroid open_source_license_android { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public OpenSourceLicenseIos open_source_license_ios { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Policy policy { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Tnc tnc { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public TrademarkCopyright trademark_copyright { get; set; }
    }

    //public class BuiltLegalLink
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int Id { get; set; }

    //    [ForeignKey(typeof(BuiltLegal))]
    //    public int BuiltLegalId { get; set; }

    //    public string href { get; set; }
    //    public string title { get; set; }
    //}

    public class GamingRules
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }

        public string href { get; set; }
        public string title { get; set; }
    }

    public class LicenseAgreement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class OpenSourceLicenseAndroid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class OpenSourceLicenseIos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class Policy
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class Tnc
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }

    public class TrademarkCopyright
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLegal))]
        public int BuiltLegalId { get; set; }

        [OneToOne]
        public BuiltLegal BuiltLegal { get; set; }
        public string href { get; set; }
        public string title { get; set; }
    }
}
