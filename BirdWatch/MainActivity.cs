using Android.App;
using Android.Widget;
using Android.OS;

namespace BirdWatch
{
    [Activity(Label = "Bird Watching Ireland", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            Button newsButton = FindViewById<Button>(Resource.Id.newsButton);
            Button birdButton = FindViewById<Button>(Resource.Id.birdButton);
            Button seenButton = FindViewById<Button>(Resource.Id.seenButton);
            Button wishButton = FindViewById<Button>(Resource.Id.wishButton);
            Button mapButton = FindViewById<Button>(Resource.Id.mapButton);
            Button cameraButton = FindViewById<Button>(Resource.Id.cameraButton);

            newsButton.Click += delegate { StartActivity(typeof(NewsActivity)); };
            birdButton.Click += delegate { StartActivity(typeof(BirdActivity)); };
            seenButton.Click += delegate { StartActivity(typeof(SeenActivity)); };
            wishButton.Click += delegate { StartActivity(typeof(WishActivity)); };
            mapButton.Click += delegate { StartActivity(typeof(MapActivity)); };
            cameraButton.Click += delegate { StartActivity(typeof(CameraActivity)); };


        }

        

}
}

