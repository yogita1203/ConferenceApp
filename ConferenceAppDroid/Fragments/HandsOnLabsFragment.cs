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
using CommonLayer;
using ConferenceAppDroid.Utilities;

namespace ConferenceAppDroid.Fragments
{

    public class HandsOnLabsFragment : Android.Support.V4.App.Fragment
    {
         View parentView;
         ListView handsOnLabsListView;
         HandsOnLabsAdapter adapter;
         List<BuiltHandsonLabs> lstHandsonLabs;
         public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
         {
             parentView=inflater.Inflate(Resource.Layout.view_hands_on_labs_fragment, null);
             handsOnLabsListView = parentView.FindViewById<ListView>(Resource.Id.handsOnLabsListView);
             DataManager.GetHandsOnLabs(DBHelper.Instance.Connection).ContinueWith(t =>
             {
                 lstHandsonLabs = t.Result;
                 lstHandsonLabs=lstHandsonLabs.OrderBy(p => p.hol_category).ToList();
                 Activity.RunOnUiThread(() => {
                     adapter = new HandsOnLabsAdapter(Activity, Resource.Layout.list_row_hands_on_labs, lstHandsonLabs);
                     handsOnLabsListView.Adapter = adapter;
                 });
                 
             });

             handsOnLabsListView.ItemClick += (s, e) =>
                 {
                     var currentHol = lstHandsonLabs[e.Position];
                     Intent intent = new Intent(Activity, (typeof(HandsOnLabsDetailActivity)));
                     intent.PutExtra("SessionID", currentHol.session_id);
                     StartActivity(intent);
                 };
             return parentView;
         }
    }

     public class HandsOnLabsAdapter : ArrayAdapter<BuiltHandsonLabs>
     {
         Android.App.Activity context;
         int _resource;
         List<BuiltHandsonLabs> items;
         LinearLayout section;
         TextView sectionTitle, handsOnTitleTextView, handsOnAbbreviationTextView;
         ImageView row_handsonlab_arrow;


         public HandsOnLabsAdapter(Android.App.Activity context, int resource, List<BuiltHandsonLabs> items):base(context,resource,items)
         {
             this.context = context;
             this._resource = resource;
             this.items = items;
         }
         public override View GetView(int position, View convertView, ViewGroup parent)
         {
             View parentView;
              parentView = convertView;
              ViewHolder viewholder;
              if (null == convertView)
              {
                  parentView = LayoutInflater.From(context).Inflate(Resource.Layout.list_row_hands_on_labs, null);
                  section = parentView.FindViewById<LinearLayout>(Resource.Id.section);
                  sectionTitle = parentView.FindViewById<TextView>(Resource.Id.sectionTitle);
                  row_handsonlab_arrow = parentView.FindViewById<ImageView>(Resource.Id.row_handsonlab_arrow);
                  handsOnTitleTextView = parentView.FindViewById<TextView>(Resource.Id.handsOnTitleTextView);
                  handsOnAbbreviationTextView = parentView.FindViewById<TextView>(Resource.Id.handsOnAbbreviationTextView);
                  viewholder = new ViewHolder(section, sectionTitle, row_handsonlab_arrow, handsOnTitleTextView, handsOnAbbreviationTextView);
                  parentView.Tag = viewholder;
              }
              else

              {
                  viewholder = (ViewHolder)parentView.Tag;
                  section=viewholder.section; 
                  sectionTitle=viewholder.sectionTitle; 
                  row_handsonlab_arrow=viewholder.row_handsonlab_arrow;
                  handsOnTitleTextView = viewholder.handsOnTitleTextView;
                  handsOnAbbreviationTextView = viewholder.handsOnAbbreviationTextView;
              }
              var item = GetItem(position);
              section.Visibility = ViewStates.Visible;
              sectionTitle.Text = item.hol_category;
              if (position != 0)
              {
                  var previousItem = GetItem(position - 1);
                  if (!previousItem.hol_category.Equals(item.hol_category, StringComparison.InvariantCultureIgnoreCase))
                  {

                      section.Visibility = ViewStates.Visible;
                      sectionTitle.Text = item.hol_category;
                  }
                  else
                  {
                      section.Visibility = ViewStates.Gone;
                  }
                

              }
              if (!string.IsNullOrWhiteSpace(item.session_id))
              {
                  handsOnAbbreviationTextView.Text = item.session_id;
              }
              if (!string.IsNullOrWhiteSpace(item.title))
              {
                  handsOnTitleTextView.Text = item.title;
              }
            
              return parentView;
         }

         public class ViewHolder : Java.Lang.Object
         {
             public LinearLayout section;
             public TextView sectionTitle;
             public ImageView row_handsonlab_arrow;
             public TextView handsOnTitleTextView;
             public TextView handsOnAbbreviationTextView;

             public ViewHolder(LinearLayout section, TextView sectionTitle, ImageView row_handsonlab_arrow, TextView handsOnTitleTextView, TextView handsOnAbbreviationTextView)
             {
                 this.section = section;
                 this.sectionTitle = sectionTitle;
                 this.row_handsonlab_arrow = row_handsonlab_arrow;
                 this.handsOnTitleTextView = handsOnTitleTextView;
                 this.handsOnAbbreviationTextView = handsOnAbbreviationTextView;
             }
         }
     }
       
}