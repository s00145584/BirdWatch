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
using Android.Graphics;

namespace BirdWatch
{
    public static class Camera
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }
}