using CommonLayer.Entities.Built;
using System;
using System.Collections.Generic;

namespace ConferenceAppDroid
{
	class ListItemValue : Java.Lang.Object, IHasLabel, IComparable<ListItemValue>
	{
        //public ListItemValue (string name, string section)
        //{
        //    Name = name;
        //    Section = section;
        //}

        public ListItemValue(BuiltSectionItems sectionItem, string section)
        {
            SectionItem = sectionItem;
            Section = section;
        }
		
		//public string Name {get; private set;}
        public BuiltSectionItems SectionItem {get; private set;}
        public string Section { get; private set; }
		
		int IComparable<ListItemValue>.CompareTo (ListItemValue value)
		{
			//return Name.CompareTo (value.Name);
            return SectionItem.menuname.CompareTo (value.SectionItem.menuname);
		}
		
		public override string ToString ()
		{
			 //return Name;
            return SectionItem.menuname;
		}
		
		public string Label {
			get 
            {
                //return Name [0].ToString ();
                return Section;
            }
		}
	}
}

