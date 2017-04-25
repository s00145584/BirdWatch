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
using System.Data.SqlClient;
using Android.Graphics;
using Android.Preferences;
using System.Data;
using Android.Content.PM;

namespace BirdWatch
{
    [Activity(Label = "Seen List", Theme = "@style/NoActionBar", ScreenOrientation = ScreenOrientation.SensorPortrait)]
    public class SeenActivity : Activity
    {
        Context mContext = Application.Context;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.Seenlayout);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            var androidID = prefs.GetString("androidID", "");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            toolbar.SetBackgroundColor(Color.ParseColor("#4c7939"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Seen List";

            List<Bird> seenList = new List<Bird>();
            string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
            SqlDataReader rdr = null;

            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.allseenbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;

                connection.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    seenList.Add(new Bird() { Name = rdr["Name"].ToString()});
                }
                connection.Close();
            }

            //ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, seenList.Select(n => n.Name).ToList());
            ListView ListView = FindViewById<ListView>(Resource.Id.Listview);
            var ListAdapter = new CustomSeenListAdapter(this, Resource.Layout.custom_list, seenList.Select(n => n.Name).ToList());
            //SetListAdapter(ListAdapter);
            ListView.Adapter = ListAdapter;
            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs position)
            {
                var selectedFromList = (String)(ListView.GetItemAtPosition(position.Position));
                var intent = new Intent(this, typeof(BirdDetailActivity));
                intent.PutExtra("Name", selectedFromList);
                intent.PutExtra("IncomingPage", "Seen");
                StartActivity(intent);
            };
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

        //protected byte[] imageConvert(string varbin)
        //{
        //    if (varbin != "")
        //    {
        //        List<byte> byteList = new List<byte>();

        //        string hexPart = varbin.Substring(2);
        //        for (int i = 0; i < hexPart.Length / 2; i++)
        //        {
        //            string hexNumber = hexPart.Substring(i * 2, 2);
        //            byteList.Add((byte)Convert.ToInt32(hexNumber, 16));
        //        }

        //        return byteList.ToArray();
        //    }
        //    else
        //    {
        //        return Enumerable.Empty<byte>().ToArray();
        //    }

        //}
    }
}