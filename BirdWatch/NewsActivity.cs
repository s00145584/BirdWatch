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

namespace BirdWatch
{
    [Activity(Label = "News and Updates")]
    public class NewsActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            List<News> newsList = new List<News>();
            string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
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
                    newsList.Add(new News() {NewsItem=rdr["NewsItem"].ToString(),Date= (DateTime)rdr["Date"] });
                    //LoginRole = rdr["RoleId"].ToString();
                    //OwnerId = rdr["UserId"].ToString();
                }
                connection.Close();
            }

            ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, newsList.Select(n=>n.NewsItem).ToList());
        }
    }
}