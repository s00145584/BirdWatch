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
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;

namespace BirdWatch
{
    [Activity(Label = "PermissionsActivity", Theme = "@style/NoActionBar")]
    public class PermissionsActivity : Activity
    {
        readonly string[] PermissionsLocation =
    {
      Manifest.Permission.ReadPhoneState
    };

        const int RequestLocationId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.Permissionlayout);

            const string permission = Manifest.Permission.ReadPhoneState;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
            else
            {
                if (ShouldShowRequestPermissionRationale(permission))
                {
                    //Explain to the user why we need to read the contacts
                    Snackbar.Make(FindViewById(Resource.Layout.Permissionlayout), "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                            .Show();
                    return;
                }
                //Finally request permissions with the list of permissions and Id
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            //Snackbar.Make(getView(), "Location permission is available, getting lat/long.", Snackbar.LengthShort).Show();

                            Intent myIntent = new Intent(this, typeof(MainActivity));
                            myIntent.PutExtra("greeting", "Hello from the Second Activity!");
                            SetResult(Result.Ok, myIntent);
                            Finish();
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                            Snackbar.Make(FindViewById(Resource.Layout.Permissionlayout), "Location permission is denied.", Snackbar.LengthShort).Show();
                        }
                    }
                    break;
            }
        }
    }
}