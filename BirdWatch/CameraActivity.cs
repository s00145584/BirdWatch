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
using Java.IO;
using Android.Provider;
using Android.Content.PM;
using Android.Graphics;
using Android;
using Android.Support.Design.Widget;

namespace BirdWatch
{
    [Activity(Label = "Camera", Theme = "@style/NoActionBar")]
    public class CameraActivity : Activity
    {

        ImageView _imageView;
        readonly string[] PermissionsLocation =
    {
      Manifest.Permission.Camera,
      Manifest.Permission.ReadExternalStorage
    };

        const int RequestLocationId = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Cameralayout);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetBackgroundColor(Color.ParseColor("#d5a6bd"));
            //Toolbar will now take on default Action Bar characteristics
            SetActionBar(toolbar);
            //You can now use and reference the ActionBar
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.Title = "Camera";

            // Create your application here

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = this.FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                button.Click += TakeAPicture;
            }

            const string permission = Manifest.Permission.Camera;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                RequestPermissions(PermissionsLocation, RequestLocationId);
            }
            else
            {
                if (ShouldShowRequestPermissionRationale(permission))
                {
                    //Explain to the user why we need to read the contacts
                    Snackbar.Make(FindViewById(Resource.Layout.Cameralayout), "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                            .SetAction("OK", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                            .Show();
                    return;
                }
                //Finally request permissions with the list of permissions and Id
                RequestPermissions(PermissionsLocation, RequestLocationId);
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

        private void CreateDirectoryForPictures()
        {
            Camera._dir = new File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!Camera._dir.Exists())
            {
                Camera._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {

            Intent intent = new Intent(MediaStore.ActionImageCapture);
            Camera._file = new File(Camera._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(Camera._file));
            StartActivityForResult(intent, 0);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(Camera._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;

            Camera.bitmap = Camera._file.Path.LoadAndResizeBitmap(width, height);
            if (Camera.bitmap != null)
            {
                _imageView.SetImageBitmap(Camera.bitmap);
                Camera.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }
    }
}