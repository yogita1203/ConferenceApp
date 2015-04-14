using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Entities.Built;

namespace ConferenceAppDroid.Adapters
{
    public class ListItemSessionValue : Java.Lang.Object, IHasLabel, IComparable<ListItemSessionValue>
    {
        public ListItemSessionValue(BuiltSessionTime sectionItem, string section)
        {
            SectionItem = sectionItem;
            Section = section;
        }

        //public string Name {get; private set;}
        public BuiltSessionTime SectionItem { get; private set; }
        public string Section { get; private set; }

        int IComparable<ListItemSessionValue>.CompareTo(ListItemSessionValue value)
        {
            //return Name.CompareTo (value.Name);
            return SectionItem.BuiltSession.title.CompareTo(value.SectionItem.BuiltSession.title);
        }

        public override string ToString()
        {
            //return Name;
            return SectionItem.BuiltSession.title;
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
