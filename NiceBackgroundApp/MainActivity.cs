using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using System;

namespace NiceBackgroundApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Toast.MakeText(Android.App.Application.Context, "OnCreate", ToastLength.Long).Show();

            try
            {
                Intent downloadIntent = new Intent(Android.App.Application.Context, typeof(MyService));
                StartService(downloadIntent);
                //Toast.MakeText(Android.App.Application.Context, "MyService running", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                //Toast.MakeText(Android.App.Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}