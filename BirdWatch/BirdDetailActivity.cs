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
    [Activity(Label = "BirdDetail", Theme = "@style/NoActionBar")]
    public class BirdDetailActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BirdDetaillayout);
            var name = Intent.Extras.GetString("Name");
            var page = Intent.Extras.GetString("IncomingPage");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            switch (page)
            {
                case "Bird":
                    toolbar.SetBackgroundColor(Color.ParseColor("#b66916"));
                    break;
                default:
                    break;
            }
            //if (page == "Bird")
            //{
            //    toolbar.SetBackgroundColor(Color.ParseColor("#b66916"));
            //}
            //toolbar.SetBackgroundColor(Color.ParseColor("#d5a6bd"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Bird Detail";

            // Create your application here
            


        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}