using System;
using Java.Util;

namespace XamBLEServer
{
    public class Const
    {
        public static UUID UUID_SERVICE = UUID.FromString("cdb7950d-73f1-4d4d-8e47-c090502dbd63"); //128bit
        public static UUID UUID_DATA = UUID.FromString("0000950d-0000-1000-8000-00805f9b34fb"); //16bit

        public static readonly string PERMISSION_GRANTED = "PERMISSION_GRANTED";

        public static readonly int REQ_LOCATION_CODE = 111;
        public static readonly int REQ_BLUETOOTH_CODE = 222;
        public static readonly int REQ_GPS_CODE = 333;

    }
}
