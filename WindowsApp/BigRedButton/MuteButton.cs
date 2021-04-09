using SensorTag;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace BigRedButton
{
    public class MuteButtonService : IDisposable
    {
        BleButtonService buttons;
        BleDeviceWatcher watcher;
        static MuteButtonService instance;

        public void Start()
        {
            instance = this;
            buttons = new BleButtonService();
            watcher = new BleDeviceWatcher();
            watcher.StartWatching(Guid.Parse("f000aa00-0451-4000-b000-000000000000"));
            _ = FindPairedDevices();
        }

        public static MuteButtonService Instance => instance;

        public bool LeftButtonDown { get; set; }

        public bool RightButtonDown { get; set; }

        private async Task FindPairedDevices()
        {
            foreach (var info in await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.DeviceInformation)))
            {
                if (info.Name == "SensorTag 2.0")
                {
                    Debug.WriteLine("Found device: {0}: {1}", info.Name, info.Id);
                    string containerId = info.Properties[BleGenericGattService.CONTAINER_ID_PROPERTY_NAME]?.ToString();
                    await this.ConnectButtonsAsync(containerId);
                }
            }
        }
        private async Task ConnectButtonsAsync(string containerId)
        {
            Debug.WriteLine("connecting...");
            await buttons.ConnectAsync(containerId);
            buttons.ButtonValueChanged += OnButtonValueChanged;
        }

        private void OnButtonValueChanged(object sender, SensorButtonEventArgs e)
        {
            this.LeftButtonDown = e.LeftButtonDown;
            this.RightButtonDown = e.RightButtonDown;
            Debug.WriteLine("Button state: left={0}, right={1}", e.LeftButtonDown, e.RightButtonDown);
            ButtonStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            using (buttons) { }
            using (watcher) { }
        }

        public event EventHandler ButtonStateChanged;
    }
}
