using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;

namespace XamBLEServer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private View ledView;
        private BLEPeripheral server;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            server = new BLEPeripheral(onMessageReceive);
            initView();
        }

        private void initView()
        {
            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SwitchCompat sw = FindViewById<SwitchCompat>(Resource.Id.switchServer);
            sw.CheckedChange += (sender, e) =>
            {
                if (!isPassPermissionAndSetting())
                {
                    sw.Checked = false;
                    return;
                }
                if (e.IsChecked)
                {
                    server.start("BLE");
                }
                else
                {
                    server.stop();
                }
            };

            ledView = FindViewById<View>(Resource.Id.ledView);
            ledView.SetBackgroundColor(Color.Gray);
        }

        private void onMessageReceive(string value)
        {
            Log.Debug("TEST", "onMessageReceive " + value);
            switch (value)
            {
                case "RED":
                    ledView.SetBackgroundColor(Color.Red);
                    break;
                case "GREEN":
                    ledView.SetBackgroundColor(Color.Green);
                    break;
                default:
                    ledView.SetBackgroundColor(Color.Gray);
                    break;
            }
        }


        private bool isPassPermissionAndSetting()
        {
            switch (PermissionsUtil.checkPermission(this))
            {
                case "Location":
                    PermissionsUtil.reqLocationPermission(this);
                    return false;

                case "GPS":
                    PermissionsUtil.reqGPSEnable(this);
                    return false;

                case "Bluetooth":
                    PermissionsUtil.reqBluetoothEnable(this);
                    return false;

                default: return true;

            }

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
