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

namespace BirdWatch
{
    [Activity(Label = "Bird List", Theme = "@style/NoActionBar")]
    public class BirdActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.Birdlayout);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            toolbar.SetBackgroundColor(Color.ParseColor("#b66916"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Bird List";

            List<Bird> birdList = new List<Bird>();
            string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
            SqlDataReader rdr = null;

            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.allbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = model.Email;

                connection.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    birdList.Add(new Bird() { Name = rdr["Name"].ToString()});
                }
                connection.Close();
            }

            //ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, birdList.Select(n => n.Name).ToList());
            //ListAdapter = new CustomListAdapter(this, Resource.Layout.custom_list, birdList.Select(n => n.Name).ToList());
            //mListView.setAdapter(listAdapter);
            ListView ListView = FindViewById<ListView>(Resource.Id.Listview);
            var ListAdapter = new CustomListAdapter(this, Resource.Layout.custom_list, birdList.Select(n => n.Name).ToList());
            //SetListAdapter(ListAdapter);
            
            ListView.Adapter = ListAdapter;
            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs position)
            {
                var selectedFromList = (String)(ListView.GetItemAtPosition(position.Position));
                var intent = new Intent(this, typeof(BirdDetailActivity));
                intent.PutExtra("Name", selectedFromList);
                intent.PutExtra("IncomingPage", "Bird");
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