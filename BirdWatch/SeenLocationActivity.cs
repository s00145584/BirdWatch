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
using Android.Locations;
using Android.Util;
using Android.Graphics;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using System.Data.SqlClient;
using System.Data;
using Android.Preferences;

namespace BirdWatch
{
    [Activity(Label = "SeenLocationActivity", Theme = "@style/NoActionBar", ScreenOrientation = ScreenOrientation.SensorPortrait)]
    public class SeenLocationActivity : Activity, ILocationListener
    {
        string constring = "Data Source=noctis2.database.windows.net,1433;Initial Catalog=Birdwatching;Persist Security Info=True;User ID=snakesosa;Password=Freyasweetie1*;";
        public string androidID = PreferenceManager.GetDefaultSharedPreferences(Application.Context).GetString("androidID", "");
        int result;
        double lng,lat;

        readonly string[] PermissionsLocation =
            {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        const int RequestLocationId = 0;

        static readonly string TAG = "X:" + typeof(SeenLocationActivity).Name;
        TextView _addressText;
        Location _currentLocation;
        LocationManager _locationManager;

        string _locationProvider;
        TextView _latText;
        TextView _longText;

        Button btnSave;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SeenLocationlayout);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            toolbar.SetBackgroundColor(Color.ParseColor("#b66916"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Add Location";

            const string permission = Manifest.Permission.AccessCoarseLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
            else
            {
                if (ShouldShowRequestPermissionRationale(permission))
                {
                    //Explain to the user why we need to read the contacts
                    Snackbar.Make(FindViewById(Resource.Layout.SeenLocationlayout), "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                            .Show();
                    return;
                }
                //Finally request permissions with the list of permissions and Id
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }


            _addressText = FindViewById<TextView>(Resource.Id.tvaddress);
            _latText = FindViewById<TextView>(Resource.Id.tvlat);
            _longText = FindViewById<TextView>(Resource.Id.tvlong);
            FindViewById<TextView>(Resource.Id.btnGetLocation).Click += StartAddress;
            btnSave = FindViewById<Button>(Resource.Id.btnAddLocation);
            btnSave.Click += SaveAddress;
            InitializeLocationManager();
        }

        private void SaveAddress(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand cmd = new SqlCommand("dbo.addseenbirds", connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@userID", SqlDbType.VarChar).Value = androidID;
                cmd.Parameters.Add("@birdName", SqlDbType.VarChar).Value = Intent.Extras.GetString("Name");
                cmd.Parameters.Add("@lat", SqlDbType.Float).Value = lat;
                cmd.Parameters.Add("@lng", SqlDbType.Float).Value = lng;

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
                Intent myIntent = new Intent(this, typeof(BirdDetailActivity));
                SetResult(Result.Ok, myIntent);
                Finish();
            }
            else
            {
                Intent myIntent = new Intent(this, typeof(BirdDetailActivity));
                SetResult(Result.Canceled, myIntent);
                Finish();
            }

        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
                Toast.MakeText(this, "Unable to get Location.\nPlease ensure Location is turned on.", ToastLength.Long).Show();
                Finish();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            //_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            //_latText.Text = "Latitude: " + location.Latitude;
            //_longText.Text = "Longitude: " + location.Longitude;
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        void StartAddress(object sender, EventArgs eventArgs)
        {
            _locationManager.RequestSingleUpdate(_locationProvider, this, null);
            Location location = getLastKnownLocation(); //_locationManager.GetLastKnownLocation(_locationProvider);
            _currentLocation = location;
            _latText.Text = Math.Round(location.Latitude, 6).ToString();
            _longText.Text = Math.Round(location.Longitude, 6).ToString();
            lat = Math.Round(location.Latitude, 6);
            lng = Math.Round(location.Longitude, 6);

            getAddress(sender, eventArgs);
            btnSave.Enabled = true;
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

        async void getAddress(object sender, EventArgs eventArgs)
        {
            if (_currentLocation == null)
            {
                _addressText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
            //_locationManager.RemoveUpdates(this);
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressText.Text = deviceAddress.ToString();
            }
            else
            {
                _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
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