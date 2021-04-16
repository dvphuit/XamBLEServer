using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Locations;
using AndroidX.Core.Content;
using static Android.Manifest;
using Android.Provider;

namespace XamBLEServer
{

    //https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/permissions?tabs=macos

    public class PermissionsUtil
    {
  
        public static string checkPermission(Context context)
        {
            string ret = Const.PERMISSION_GRANTED;

            if (PermissionChecker.CheckSelfPermission(context, Permission.AccessFineLocation) != (int)PermissionChecker.PermissionGranted)
            {
                return "Location";
            }

            if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                return "Bluetooth";
            }

            if (!((LocationManager)context.GetSystemService(Context.LocationService)).IsProviderEnabled(LocationManager.GpsProvider))
            {
                return "GPS";
            }

            return ret;
        }

        public static void reqLocationPermission(Activity activity)
        {
            new AlertDialog.Builder(activity)
                .SetTitle("Location Permission")
                .SetMessage("Must accept location permission")
                .SetPositiveButton("OK", (sender, args) =>
                {
                    var permission = new string[] { Permission.AccessFineLocation };
                    activity.RequestPermissions(permission, Const.REQ_LOCATION_CODE);
                })
                .SetNegativeButton("Close", (sender, args) => { })
                .Show();
        }


        public static void reqBluetoothEnable(Activity activity)
        {
            new AlertDialog.Builder(activity)
                .SetTitle("Bluetooth")
                .SetMessage("Must enable Bluetooth")
                .SetPositiveButton("Setting", (sender, args) =>
                {
                    activity.StartActivityForResult(new Intent(BluetoothAdapter.ActionRequestEnable), Const.REQ_BLUETOOTH_CODE);
                })
                .SetNegativeButton("Close", (sender, args) => { })
                .Show();
        }

        public static void reqGPSEnable(Activity activity)
        {
            new AlertDialog.Builder(activity)
                .SetTitle("GPS")
                .SetMessage("Must enable GPS")
                .SetPositiveButton("Setting", (sender, args) =>
                {
                    activity.StartActivityForResult(new Intent(Settings.ActionLocationSourceSettings), Const.REQ_GPS_CODE);
                })
                .SetNegativeButton("Close", (sender, args) => { })
                .Show();
        }

    }
}
