using SensorTag;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace BigRedButton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MuteButtonService service;
        bool muted;
        bool nomic;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += OnWindowLoaded;

            service = MuteButtonService.Instance;
            service.ButtonStateChanged += OnMuteButtonStateChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            this.service.ButtonStateChanged -= OnMuteButtonStateChanged;
        }

        private void OnMuteButtonStateChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(OnUpdateButtons);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            bool? muted = AudioManager.IsMicrophoneMute();

            if (muted == true)
            {                
                this.muted = true;
            }
            else if (muted == null)
            {
                this.nomic = true;
            }

            UpdateMicIcon();
        }



        public static bool GetRemotePressed(DependencyObject obj)
        {
            return (bool)obj.GetValue(RemotePressedProperty);
        }

        public static void SetRemotePressed(DependencyObject obj, bool value)
        {
            obj.SetValue(RemotePressedProperty, value);
        }

        // Using a DependencyProperty as the backing store for RemotePressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemotePressedProperty =
            DependencyProperty.RegisterAttached("RemotePressed", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        private void OnUpdateButtons()
        {
            if (this.service.LeftButtonDown)
            {
                SetRemotePressed(BigButton, true);
            }
            else if (GetRemotePressed(BigButton))
            {
                SetRemotePressed(BigButton, false);
                ToggleMute();
            }

            // right button could control speaker output mute ?
            //if (this.service.RightButtonDown)
            //{
            //    RightButton.Background = Brushes.Green;
            //}
            //else
            //{
            //    RightButton.SetValue(Button.BackgroundProperty, DependencyProperty.UnsetValue);
            //}
        }

        void ToggleMute()
        {
            if (AudioManager.SetMicrophoneMute(!muted) == 0)
            {
                muted = !muted;
                UpdateMicIcon();
            }
        }

        void UpdateMicIcon()
        {
            if (muted)
            {
                MicOn.Visibility = Visibility.Collapsed;
                MicOff.Visibility = Visibility.Visible;
            }
            else
            {
                MicOn.Visibility = Visibility.Visible;
                MicOff.Visibility = Visibility.Collapsed;
            }
            if (nomic)
            {
                LeftButton.Background = Brushes.LightGray;
            }
        }

        private void OnBigButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleMute();
        }
    }
}
