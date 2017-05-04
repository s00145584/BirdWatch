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
using Android.Content.Res;
using System.Data;
using Android.Graphics;
using System.IO;
using Android.Content.PM;

namespace BirdWatch
{
    [Activity(Label = "News and Updates", Theme = "@style/NoActionBar", ScreenOrientation = ScreenOrientation.SensorPortrait)]
    public class NewsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.Newslayout);

                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

                toolbar.SetBackgroundColor(Color.ParseColor("#bf920d"));
                //Toolbar will now take on default Action Bar characteristics
                SetActionBar(toolbar);
                //You can now use and reference the ActionBar
                ActionBar.SetHomeButtonEnabled(true);
                ActionBar.SetDisplayHomeAsUpEnabled(true);
                ActionBar.Title = "News";

                //var view = FindViewById<LinearLayout>(Resource.Id.view);
                //view.SetBackgroundColor(Color.ParseColor("#f3cd58"));

                List<News> newsList = new List<News>();
                string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
                //string constring2 = "Data Source=tcp:noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;MultipleActiveResultSets=False;Connection Timeout=30;";
                SqlDataReader rdr = null;

                using (SqlConnection connection = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand("dbo.allnews", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = model.Email;

                    connection.Open();
                    rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        newsList.Add(new News() { NewsItem = rdr["NewsItem"].ToString(), Date = (DateTime)rdr["Date"], sDate = "News Update: " + ((DateTime)rdr["Date"]).ToString("dd/MM/yyyy") });
                        //LoginRole = rdr["RoleId"].ToString();
                        //OwnerId = rdr["UserId"].ToString();
                    }
                    connection.Close();
                }
                newsList = newsList.OrderByDescending(x => x.Date).ToList();
                ExpandableListView expListView = FindViewById<ExpandableListView>(Resource.Id.Listview);
                var ListAdapter = new ExpandableScreenAdapter(this, newsList);
                //SetListAdapter(ListAdapter);
                expListView.SetAdapter(ListAdapter);


                
            }

            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                //Console.WriteLine("{0} Exception caught.", e);
            }
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