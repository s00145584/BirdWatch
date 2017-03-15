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
    public class CustomWishListAdapter : BaseAdapter<String> {

        private Context mContext;
        private int id;
        private List<String> items;
        
        public CustomWishListAdapter(Context context, int textViewResourceId, List<string> list)
        {
            mContext = context;
            id = textViewResourceId;
            items = list;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override string this[int position]
        {
            get { return items[position]; }
        }

        public override View GetView(int position, View v, ViewGroup parent)
        {
            View mView = v;
            if (mView == null)
            {
                LayoutInflater vi = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);
                mView = vi.Inflate(id, null);
            }

            TextView text = (TextView)mView.FindViewById(Resource.Id.textView);

            if (items[position] != null)
            {
                text.SetTextColor(Color.White);
                text.SetText(items[position], TextView.BufferType.Normal);
                //text.SetBackgroundColor(Color.Red);
                //int colour = Color.Argb(200, 255, 64, 64);
                //text.SetBackgroundColor(colour);
            }

            if (position % 2 == 1)
            {
                mView.SetBackgroundColor(Color.ParseColor("#56418b"));//row 2
            }
            else
            {
                mView.SetBackgroundColor(Color.ParseColor("#6c52ad"));//row 1
            }
            mView.SetMinimumHeight(200);
            return mView;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

    }
}