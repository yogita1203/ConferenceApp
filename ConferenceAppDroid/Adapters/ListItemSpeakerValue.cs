using CommonLayer.Entities.Built;
using System;
using System.Collections.Generic;

namespace ConferenceAppDroid
{
    class ListItemSpeakerValue : Java.Lang.Object, IHasLabel, IComparable<ListItemSpeakerValue>
    {
        //public ListItemValue (string name, string section)
        //{
        //    Name = name;
        //    Section = section;
        //}

        public ListItemSpeakerValue(BuiltSpeaker sectionItem, string section)
        {
            SectionItem = sectionItem;
            Section = section;
        }

        //public string Name {get; private set;}
        public BuiltSpeaker SectionItem { get; private set; }
        public string Section { get; private set; }

        int IComparable<ListItemSpeakerValue>.CompareTo(ListItemSpeakerValue value)
        {
            //return Name.CompareTo (value.Name);
            return SectionItem.first_name.CompareTo(value.SectionItem.first_name);
        }

        public override string ToString()
        {
            //return Name;
            return SectionItem.first_name;
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

