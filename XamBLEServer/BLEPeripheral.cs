using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using System;
using System.Threading.Tasks;
using Android.OS;
using System.Text;
using Java.Util;
using Android.Runtime;
using Android.Util;

namespace XamBLEServer
{
    public delegate void MessageDelegate(string msg);

    
    public class BLEPeripheral
    {

        private BluetoothManager bluetoothManager;
        private BluetoothGattServer gattServer;

        private ServerCallback serverCallback;
        private AdvCallback advCallback;

        private string advValue = "";

        public BLEPeripheral(MessageDelegate messageDelegate)
        {
            this.serverCallback = new ServerCallback(messageDelegate);
            this.advCallback = new AdvCallback();
        }

        public void start(string value)
        {
            this.advValue = value;
            this.startAdvertising();
            this.startServer();
        }

        public void stop()
        {
            this.stopAdvertising();
            this.stopServer();
        }


        private bool init()
        {
            try
            {
                if (bluetoothManager == null)
                {
                    bluetoothManager = (BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService);
                }

                Log.Debug("TEST","Initialized");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("TEST", "Initialized error " + ex.Message);
                return false;
            }
        }


        private void startAdvertising()
        {
            if (!init()) return;


            var settings = new AdvertiseSettings.Builder()
            .SetConnectable(true)
            .SetAdvertiseMode(AdvertiseMode.LowLatency) //100ms
            .SetTxPowerLevel(AdvertiseTx.PowerMedium)
            .SetTimeout(0)
            .Build();


            var bytes = Encoding.ASCII.GetBytes(advValue);
            Log.Debug("TEST", "advertising value: " + advValue + " toBytes: " + string.Join(",", bytes));

            var data = new AdvertiseData.Builder()
            .AddServiceUuid(new ParcelUuid(Const.UUID_SERVICE))
            .AddServiceData(new ParcelUuid(Const.UUID_DATA), bytes) // Convert a C# string to a byte array  
            .SetIncludeDeviceName(false)
            .SetIncludeTxPowerLevel(false)
            .Build();

            bluetoothManager.Adapter.BluetoothLeAdvertiser.StartAdvertising(settings, data, advCallback);
        }

        private void stopAdvertising()
        {
            bluetoothManager.Adapter.BluetoothLeAdvertiser.StopAdvertising(advCallback);
        }

        private void startServer()
        {
            gattServer = bluetoothManager.OpenGattServer(Android.App.Application.Context, serverCallback);
            gattServer.AddService(createService());
        }

        private void stopServer()
        {
            gattServer?.Close();
        }

        private BluetoothGattService createService()
        {
            var service = new BluetoothGattService(Const.UUID_SERVICE, GattServiceType.Primary);
            var write_ch = new BluetoothGattCharacteristic(Const.UUID_DATA, GattProperty.WriteNoResponse, GattPermission.Write);
            service.AddCharacteristic(write_ch);
            return service;
        }

    }

    public  class ServerCallback : BluetoothGattServerCallback
    {
        MessageDelegate _delegate;

        public ServerCallback(MessageDelegate messageDelegate)
        {
            this._delegate = messageDelegate;
        }

        public override void OnConnectionStateChange(BluetoothDevice device, [GeneratedEnum] ProfileState status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);

            if (newState == ProfileState.Connected)
            {
                Log.Debug("TEST", "BluetoothDevice CONNECTED: " + device);
            }
            else if (newState == ProfileState.Disconnected)
            {
                Log.Debug("TEST", "BluetoothDevice DISCONNECTED: " + device);
                _delegate?.Invoke("DISCONNECTED");
            }
        }


        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
            Log.Debug("TEST", "value " + value);

            if(value != null || value.Length != 0)
            {
               string result = Encoding.UTF8.GetString(value);
                _delegate?.Invoke(result);
            }
            else
            {
                _delegate?.Invoke("Error");
            }
        }

    }

    public class AdvCallback: AdvertiseCallback
    {
        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
            Log.Debug("TEST", "AdvertiseCallback -> OnStartSuccess");
        }


        public override void OnStartFailure([GeneratedEnum] AdvertiseFailure errorCode)
        {
            base.OnStartFailure(errorCode);
            Log.Debug("TEST", "AdvertiseCallback -> OnStartFailure " + errorCode);
        }
    }
}
