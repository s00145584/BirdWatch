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

namespace BirdWatch
{
    [Activity(Label = "Wish List")]
    public class WishActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            List<Bird> wishList = new List<Bird>();
            string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
            SqlDataReader rdr = null;

            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.allwishbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = model.Email;

                connection.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    wishList.Add(new Bird() { Name = rdr["Name"].ToString(), Picture = imageConvert(rdr["Picture"].ToString()), Description = rdr["Description"].ToString() });
                }
                connection.Close();
            }

            ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, wishList.Select(n => n.Name).ToList());
        }

        protected byte[] imageConvert(string varbin)
        {
            if (varbin != "")
            {
                List<byte> byteList = new List<byte>();

                string hexPart = varbin.Substring(2);
                for (int i = 0; i < hexPart.Length / 2; i++)
                {
                    string hexNumber = hexPart.Substring(i * 2, 2);
                    byteList.Add((byte)Convert.ToInt32(hexNumber, 16));
                }

                return byteList.ToArray();
            }
            else
            {
                return Enumerable.Empty<byte>().ToArray();
            }

        }
    }
}