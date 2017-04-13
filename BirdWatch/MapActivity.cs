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

namespace BirdWatch
{
    [Activity(Label = "Map", Theme = "@style/NoActionBar")]
    public class MapActivity : Activity, IOnMapReadyCallback
    {
        readonly string[] PermissionsLocation =
    {
      Manifest.Permission.AccessCoarseLocation,
        Manifest.Permission.AccessFineLocation
    };
        const int RequestLocationId = 0;

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
            }

            //map = (SupportMapFragment)FragmentManager.FindFragmentById(R.id.map);
            //map.getMapAsync(this);

            //MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.my_mapfragment_container);
            //GoogleMap map = mapFrag.Map;
            //if (map != null)
            //{
            //    MapFragment _myMapFragment = MapFragment.NewInstance();
            //FragmentTransaction tx = FragmentManager.BeginTransaction();
            //tx.Add(getFragmentManager().findFragmentById(Resource.Layout.Maplayout), _myMapFragment);
            //tx.Commit();
            //}

            SetUpMap();

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
        }
        //public void onMapReady(GoogleMap map)
        //{
        //    map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(
        //             new LatLng(47.17, 27.5699), 16));
        //    map.AddMarker(new MarkerOptions().Icon(BitmapDescriptorFactory.FromResource(R.drawable.ic_launcher)).anchor(0.0f, 1.0f) // Anchors the marker on the bottom left
        //            .position(new LatLng(47.17, 27.5699))); //Iasi, Romania
        //    map.setMyLocationEnabled(true);
        //}



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