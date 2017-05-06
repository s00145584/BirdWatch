using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Telephony;
using Android.Content;
using Java.Util;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Preferences;
using static Android.Resource;
using Android.Gms.Common;

namespace BirdWatch
{
    [Activity(Label = "Bird Watching Ireland", MainLauncher = true, Icon = "@drawable/app_icon5", Theme = "@style/NoActionBar",ScreenOrientation = ScreenOrientation.SensorPortrait)]


    public class MainActivity : Activity
    {
        readonly string[] PermissionsLocation =
            {
            Manifest.Permission.ReadPhoneState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.Camera,
            Manifest.Permission.ReadExternalStorage
        };

        const int RequestLocationId = 0;
        Context mContext = Application.Context;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            const string permission = Manifest.Permission.ReadPhoneState;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
            else
            {
                var myIntent = new Intent(this, typeof(PermissionsActivity));
                StartActivityForResult(myIntent, 0);
            }


            #region commented code

            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager tManager = (TelephonyManager)GetSystemService(Context.TelephonyService);

            //if (tManager != null)
            //{
            //    if (!string.IsNullOrEmpty(tManager.DeviceId))
            //        telephonyDeviceID = tManager.DeviceId;
            //    if (!string.IsNullOrEmpty(tManager.SimSerialNumber))
            //        telephonySIMSerialNumber = tManager.SimSerialNumber;
            //}
            //var androidID = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            //var deviceUuid = new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            //var deviceID = deviceUuid.ToString();

            //var iid = InstanceID.GetInstance(this).getId();
            #endregion

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            toolbar.SetBackgroundColor(Android.Graphics.Color.ParseColor("#599cf3"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.Title = "Bird Watching Ireland";

            Button newsButton = FindViewById<Button>(Resource.Id.newsButton);
            Button birdButton = FindViewById<Button>(Resource.Id.birdButton);
            Button seenButton = FindViewById<Button>(Resource.Id.seenButton);
            Button wishButton = FindViewById<Button>(Resource.Id.wishButton);
            Button mapButton = FindViewById<Button>(Resource.Id.mapButton);
            Button cameraButton = FindViewById<Button>(Resource.Id.cameraButton);

            //Android.Graphics.Drawables.Drawable drawable = mContext.GetDrawable(Resource.Drawable.news2);
            //drawable.SetBounds(0, 0, 385, 385);
            //newsButton.SetCompoundDrawables(null, drawable, null, null);

            //Android.Graphics.Drawables.Drawable drawable = mContext.GetDrawable(Resource.Drawable.bird2);
            //drawable.SetBounds(0, 0, 385, 385);
            //birdButton.SetCompoundDrawables(null, drawable, null, null);



            newsButton.Click += delegate { StartActivity(typeof(NewsActivity)); };
            birdButton.Click += delegate { StartActivity(typeof(BirdActivity)); };
            seenButton.Click += delegate { StartActivity(typeof(SeenActivity)); };
            wishButton.Click += delegate { StartActivity(typeof(WishActivity)); };
            mapButton.Click += delegate { StartActivity(typeof(MapActivity)); };
            cameraButton.Click += delegate { StartActivity(typeof(CameraActivity)); };


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Menu_About, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Toast.MakeText(this, "Action selected: " + item.TitleFormatted +" " + item.ItemId,ToastLength.Short).Show();
            //return base.OnOptionsItemSelected(item);

            switch (item.ItemId)
            {
                case Resource.Id.menu_about:
                    var intent = new Intent(this, typeof(BirdDetailActivity));
                    //intent.PutExtra("IncomingPage", "Bird");
                    StartActivity(intent);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                var helloLabel = data.GetStringExtra("greeting");
                var telephonyDeviceID = string.Empty;
                var telephonySIMSerialNumber = string.Empty;
                TelephonyManager tManager = (TelephonyManager)GetSystemService(Context.TelephonyService);

                if (tManager != null)
                {
                    if (!string.IsNullOrEmpty(tManager.DeviceId))
                        telephonyDeviceID = tManager.DeviceId;
                    if (!string.IsNullOrEmpty(tManager.SimSerialNumber))
                        telephonySIMSerialNumber = tManager.SimSerialNumber;
                }
                var androidID = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                var deviceUuid = new UUID(androidID.GetHashCode(), ((int)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
                var deviceID = deviceUuid.ToString();

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("androidID", androidID);
                // editor.Commit();    // applies changes synchronously on older APIs
                editor.Apply();        // applies changes asynchronously on newer APIs
            }
        }

        public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults)
        {
            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager tManager = (TelephonyManager)GetSystemService(Context.TelephonyService);

            //if (tManager != null)
            //{
            //    if (!string.IsNullOrEmpty(tManager.DeviceId))
            //        telephonyDeviceID = tManager.DeviceId;
            //    if (!string.IsNullOrEmpty(tManager.SimSerialNumber))
            //        telephonySIMSerialNumber = tManager.SimSerialNumber;
            //}
            //var androidID = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            //var deviceUuid = new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            //var deviceID = deviceUuid.ToString();

            //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            //ISharedPreferencesEditor editor = prefs.Edit();
            //editor.PutString("androidID", androidID);
            //// editor.Commit();    // applies changes synchronously on older APIs
            //editor.Apply();        // applies changes asynchronously on newer APIs
        }

    }
}

