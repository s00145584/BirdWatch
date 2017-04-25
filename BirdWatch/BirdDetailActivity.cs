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
using System.Data.SqlClient;
using System.Data;
using Android.Preferences;
using Android.Locations;
using Android.Content.PM;

namespace BirdWatch
{
    [Activity(Label = "BirdDetail", Theme = "@style/NoActionBar", ScreenOrientation = ScreenOrientation.SensorPortrait)]
    public class BirdDetailActivity : Activity
    {
        string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
        public string androidID = PreferenceManager.GetDefaultSharedPreferences(Application.Context).GetString("androidID", "");
        int result;
        string birdName;
        string page;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.BirdDetaillayout);
            birdName = Intent.Extras.GetString("Name");
            page = Intent.Extras.GetString("IncomingPage");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            var lLayout = FindViewById<LinearLayout>(Resource.Id.linearID);
            switch (page)
            {
                case "Bird":
                    toolbar.SetBackgroundColor(Color.ParseColor("#b66916"));
                    lLayout.SetBackgroundColor(Color.ParseColor("#f1c18e"));
                    break;
                case "Seen":
                    toolbar.SetBackgroundColor(Color.ParseColor("#4c7939"));
                    lLayout.SetBackgroundColor(Color.ParseColor("#b6d6a8"));
                    break;
                case "Wish":
                    toolbar.SetBackgroundColor(Color.ParseColor("#4b3979"));
                    lLayout.SetBackgroundColor(Color.ParseColor("#b5a8d6"));
                    break;
                default:
                    break;
            }
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = birdName;

            // Create your application here

            SqlDataReader rdr = null;

            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.returnBird", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = birdName;

                connection.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    FindViewById<TextView>(Resource.Id.tViewName).Text = rdr["Name"].ToString();
                    FindViewById<TextView>(Resource.Id.tViewLatinName).Text = rdr["LatinName"].ToString();
                    FindViewById<TextView>(Resource.Id.tViewLocation).Text = rdr["Location"].ToString();
                    FindViewById<TextView>(Resource.Id.tViewIdentification).Text = rdr["Identification"].ToString();
                    FindViewById<TextView>(Resource.Id.tViewDiet).Text = rdr["Diet"].ToString();
                    FindViewById<TextView>(Resource.Id.tViewConservation).Text = rdr["Conservation"].ToString();

                    byte[] test = (byte[])rdr["Picture"];
                    var test2 = test.GetType();

                    //byte[] bArray = Convert.ToString(rdr["Picture"]).ToCharArray().Select(c => (byte)c).ToArray();
                    //byte[] bArray = Convert.FromBase64String(Convert.ToString(rdr["Picture"]));

                    Bitmap bmp = BitmapFactory.DecodeByteArray(test, 0, test.Length);
                    FindViewById<ImageView>(Resource.Id.iViewPicture).SetImageBitmap(bmp);

                    //birdList.Add(new Bird() { Name = rdr["Name"].ToString(), Description = rdr["Description"].ToString() });
                }

                connection.Close();
            }



        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (page == "Seen" || page == "Wish")
            {
                MenuInflater.Inflate(Resource.Menu.delete_top_menus, menu);
                return base.OnCreateOptionsMenu(menu);
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.top_menus, menu);
                return base.OnCreateOptionsMenu(menu);
            }

            
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Toast.MakeText(this, "Action selected: " + item.TitleFormatted +" " + item.ItemId,ToastLength.Short).Show();
            //return base.OnOptionsItemSelected(item);

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                case Resource.Id.menu_preferences:
                    showMenu();
                    return true;
                case Resource.Id.delete_menu_preferences:
                    showMenu();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void showMenu()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            PopupMenu menu = new PopupMenu(this, toolbar, GravityFlags.Center);
            menu.Gravity = GravityFlags.Right;
            if (page == "Seen" || page == "Wish")
            {
                menu.Inflate(Resource.Menu.DeleteMenuList);
                IMenuItem menuItem = menu.Menu.FindItem(Resource.Id.delsel);
                menuItem.SetTitle(string.Format("Remove from {0} List",page));
            }
            else
            {
                menu.Inflate(Resource.Menu.menuList);
            }
                
            menu.Show();

            menu.MenuItemClick += (s1, arg1) => {
                carryOutMenu(s1,arg1);
                Console.WriteLine("{0} selected", arg1.Item.TitleFormatted);
            };
        }

        public void carryOutMenu(object s1, PopupMenu.MenuItemClickEventArgs arg1)
        {
            var test = arg1.Item.TitleFormatted.ToString();

            switch (arg1.Item.TitleFormatted.ToString())
            {
                case "Add to Seen List":
                    AddSeen();
                    return;
                case "Add to Wish List":
                    AddWish();
                    return;
                case "Remove from Seen List":
                    RemoveSeen();
                    return;
                case "Remove from Wish List":
                    RemoveWish();
                    return;
                default:
                    return;
            }
        }

        private void RemoveSeen()
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.deleteseenbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = birdName;

                IDbDataParameter rVal = cmd.CreateParameter();
                rVal.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rVal);

                connection.Open();

                cmd.ExecuteNonQuery();
                result = (int)rVal.Value;

                connection.Close();
            }

            if (result == 0)
            {
                Toast.MakeText(this, string.Format("{0} succesfully removed from Seen List.", birdName) , ToastLength.Long).Show();
                Finish();
            }
            else
            {
                Toast.MakeText(this, string.Format("{0} unsuccesfully removed from Seen List.", birdName), ToastLength.Long).Show();
            }
        }

        private void RemoveWish()
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.deletewishbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = birdName;

                IDbDataParameter rVal = cmd.CreateParameter();
                rVal.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rVal);

                connection.Open();

                cmd.ExecuteNonQuery();
                result = (int)rVal.Value;

                connection.Close();
            }

            if (result == 0)
            {
                Toast.MakeText(this, string.Format("{0} succesfully removed from Wish List.", birdName), ToastLength.Long).Show();
                Finish();
            }
            else
            {
                Toast.MakeText(this, string.Format("{0} unsuccesfully removed from Wish List.", birdName), ToastLength.Long).Show();
            }
        }

        public void AddSeen()
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.checkseenbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = birdName;

                IDbDataParameter rVal = cmd.CreateParameter();
                rVal.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rVal);

                connection.Open();

                cmd.ExecuteNonQuery();
                result = (int)rVal.Value;

                connection.Close();
            }

            if (result == 0)
            {
                var myIntent = new Intent(this, typeof(SeenLocationActivity));
                myIntent.PutExtra("Name", birdName);
                StartActivityForResult(myIntent, 0);
            }
            else
            {
                Toast.MakeText(this, "Bird is Already in Seen List.", ToastLength.Long).Show();
            }
            

        }


        public void AddWish()
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.addwishbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = birdName;

                IDbDataParameter rVal = cmd.CreateParameter();
                rVal.Direction = ParameterDirection.ReturnValue;

                cmd.Parameters.Add(rVal);

                connection.Open();

                cmd.ExecuteNonQuery();
                result = (int)rVal.Value;

                connection.Close();
            }

            switch (result)
            {
                case 0:
                    Toast.MakeText(this, FindViewById<TextView>(Resource.Id.tViewName).Text + " has been added to wish list", ToastLength.Long).Show();
                    return;
                case 99:
                    Toast.MakeText(this, FindViewById<TextView>(Resource.Id.tViewName).Text + " is already in wish list", ToastLength.Long).Show();
                    return;
                case 100:
                    Toast.MakeText(this, FindViewById<TextView>(Resource.Id.tViewName).Text + " is already in seen list, please remove first", ToastLength.Long).Show();
                    return;
                default:
                    return;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                Toast.MakeText(this, FindViewById<TextView>(Resource.Id.tViewName).Text + " has been added to seen list", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, FindViewById<TextView>(Resource.Id.tViewName).Text + " hasn't been added to seen list, error occured", ToastLength.Long).Show();
            }
        }
    }
}