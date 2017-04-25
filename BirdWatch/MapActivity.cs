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
using Android.Gms.Maps;
using Android.Graphics;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Gms.Maps.Model;
using System.Data.SqlClient;
using Android.Preferences;
using System.Data;
using Android.Locations;

namespace BirdWatch
{
    [Activity(Label = "Map", Theme = "@style/NoActionBar", ScreenOrientation = ScreenOrientation.SensorPortrait)]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        readonly string[] PermissionsLocation =
    {
      Manifest.Permission.AccessCoarseLocation,
        Manifest.Permission.AccessFineLocation
    };
        const int RequestLocationId = 0;

        string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
        public string androidID = PreferenceManager.GetDefaultSharedPreferences(Application.Context).GetString("androidID", "");
        SqlDataReader rdr = null;

        LocationManager _locationManager;

        public GoogleMap GMap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.Maplayout);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            toolbar.SetBackgroundColor(Color.ParseColor("#cf2a27"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Map";

            const string permission = Manifest.Permission.AccessCoarseLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
                SetUpMap();
            }
            else
            {
                if (ShouldShowRequestPermissionRationale(permission))
                {
                    //Explain to the user why we need to read the contacts
                    Snackbar.Make(FindViewById(Resource.Layout.Maplayout), "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                            .Show();
                    return;
                }
                //Finally request permissions with the list of permissions and Id
                RequestPermissions(PermissionsLocation, RequestLocationId);
                SetUpMap();
            }

            //SetUpMap();

        }

        private void SetUpMap()
        {
            if (GMap == null)
            {
                FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map).GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.GMap = googleMap;

            //GMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(47.17, 27.5699), 16));

            _locationManager = (LocationManager)GetSystemService(LocationService);

            // Get Current Location
            Location myLocation = getLastKnownLocation();

            if (myLocation == null)
            {
                Toast.MakeText(this, "Unable to get Location.\nPlease ensure Location is turned on.", ToastLength.Long).Show();
                Finish();
            }
            else
            {



                GMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), 16));


                using (SqlConnection connection = new SqlConnection(constring))
                {
                    SqlCommand cmd = new SqlCommand("dbo.returnseenbirds", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;

                    connection.Open();
                    rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        //birdList.Add(new Bird() { Name = rdr["Name"].ToString() });
                        double Lat = (double)rdr["Latitude"];
                        double Long = (double)rdr["Longitude"];
                        string Name = (string)rdr["Name"];
                        DateTime DateTime = (DateTime)rdr["Time"];

                        GMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(Lat, Long)).SetTitle(Name).SetSnippet(DateTime.ToString()));
                    }
                    connection.Close();
                }

                //GMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(47.16, 27.57)).SetTitle("Hello world"));
                //GMap.AddMarker(new MarkerOptions().Icon(BitmapDescriptorFactory.FromResource(R.drawable.ic_launcher)).anchor(0.0f, 1.0f) // Anchors the marker on the bottom left
                //        .position(new LatLng(47.17, 27.5699))); //Iasi, Romania
                GMap.MyLocationEnabled = true; //.setMyLocationEnabled(true);

                GMap.InfoWindowClick += MapOnInfoWindowClick;
            }
        }

        private void MapOnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            Marker myMarker = e.Marker; //e.P0;
            // Do something with marker.
            var Name = myMarker.Title;
            var intent = new Intent(this, typeof(BirdDetailActivity));
            intent.PutExtra("Name", Name);
            intent.PutExtra("IncomingPage", "Seen");
            StartActivityForResult(intent,0);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Recreate();
        }



        private Location getLastKnownLocation()
        {
            IList<String> providers = _locationManager.GetProviders(true);
            Location bestLocation = null;
            foreach (String provider in providers)
            {
                Location l = _locationManager.GetLastKnownLocation(provider);
                if (l == null)
                {
                    continue;
                }
                if (bestLocation == null || l.Accuracy < bestLocation.Accuracy)
                {
                    bestLocation = l;
                }
            }
            return bestLocation;
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