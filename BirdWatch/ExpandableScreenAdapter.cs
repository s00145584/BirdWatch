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
using Android.Graphics;

namespace BirdWatch
{
    public class ExpandableScreenAdapter : BaseExpandableListAdapter
    {
        // Context, usually set to the activity:
        private readonly Context _context;

        // List of produce objects ("vegetables", "fruits", "herbs"):
        private readonly List<News> _news;

        public ExpandableScreenAdapter(Context context, List<News> news)
        {
            _context = context;
            _news = news;
        }

        public override bool HasStableIds
        {
            // Indexes are used for IDs:
            get { return true; }
        }

        //---------------------------------------------------------------------------------------
        // Group methods:

        public override long GetGroupId(int groupPosition)
        {
            // The index of the group is used as its ID:
            return groupPosition;
        }

        public override int GroupCount
        {
            // Return the number of produce ("vegetables", "fruits", "herbs") objects:
            get { return _news.Count; }
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            // Recycle a previous view if provided:
            var view = convertView;

            // If no recycled view, inflate a new view as a simple expandable list item 1:
            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Android.Resource.Layout.SimpleExpandableListItem1, null);
            }

            // Grab the produce object ("vegetables", "fruits", etc.) at the group position:
            var news = _news[groupPosition];

            // Get the built-in first text view and insert the group name ("Vegetables", "Fruits", etc.):
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = news.Date.ToString();

            if (groupPosition % 2 == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#d7a50f"));//row 2
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#f0be28"));//row 1
            }
            view.SetMinimumHeight(200);
            return view;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return null;
        }

        //---------------------------------------------------------------------------------------
        // Child methods:

        public override long GetChildId(int groupPosition, int childPosition)
        {
            // The index of the child is used as its ID:
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            // Return the number of children (produce item objects) in the group (produce object):
            var news = _news[groupPosition];
            return 1; //produce.ProduceItems.Length;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            // Recycle a previous view if provided:
            var view = convertView;

            // If no recycled view, inflate a new view as a simple expandable list item 2:
            if (view == null)
            {
                var inflater = _context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                view = inflater.Inflate(Android.Resource.Layout.SimpleExpandableListItem2, null);
            }

            // Grab the produce object ("vegetables", "fruits", etc.) at the group position:
            var news = _news[groupPosition];

            // Extract the produce item object ("bananas", "apricots", etc.) at the child position:
            var newsItem = news.NewsItem;//[childPosition];

            // Get the built-in first text view and insert the child name ("Bananas", "Apricots", etc.):
            TextView textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = newsItem;

            // Reuse the textView to insert the number of produce units into the child's second text field:
            textView = view.FindViewById<TextView>(Android.Resource.Id.Text2);
            textView.Text = "1";// produceItem.Count.ToString() + " units";

            if (groupPosition % 2 == 1)
            {
                view.SetBackgroundColor(Color.ParseColor("#d7a50f"));//row 2
            }
            else
            {
                view.SetBackgroundColor(Color.ParseColor("#f0be28"));//row1
            }

            return view;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return null;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}