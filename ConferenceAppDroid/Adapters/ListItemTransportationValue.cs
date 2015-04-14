using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommonLayer.Entities.Built;

namespace ConferenceAppDroid.Adapters
{
    class ListItemTransportationValue : Java.Lang.Object, IHasLabel, IComparable<ListItemTransportationValue>
    {
        public ListItemTransportationValue(BuiltTransportation sectionItem, string section)
        {
            SectionItem = sectionItem;
            Section = section;
        }
        public BuiltTransportation SectionItem { get; private set; }
        public string Section { get; private set; }

        int IComparable<ListItemTransportationValue>.CompareTo(ListItemTransportationValue value)
        {
            //return Name.CompareTo (value.Name);
            return SectionItem.name.CompareTo(value.SectionItem.name);
        }

        public override string ToString()
        {
            //return Name;
            return SectionItem.name;
        }

        public string Label
        {
            get
            {
                //return Name [0].ToString ();
                return Section;
            }
        }
    }
}