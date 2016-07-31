using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;

namespace GPSDemoWindowsPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this is used to get coordinates about our location
            //System.Device.Location
            //this require network (WiFi, SIM network etc)
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            //this fires an event and it is fired every time when device moves
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            //in how many meters we want to fire this event so for every 20 meters this event will fire
            watcher.MovementThreshold = 20;
            watcher.Start();
        }
        public delegate void del();
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            //We cannot access things created by other thread from other thread
            //this is called thread affininty
            //so we have to make a delegate
            del obj = delegate
            {
                LatTextBox.Text = "" + e.Position.Location.Latitude.ToString();
                LongTextBox.Text = "" + e.Position.Location.Longitude.ToString();
            };

            //Dispatcher returns the reference of main thread and then BeginInvoke(obj)
            //will execute the code of obj in main thread
            this.Dispatcher.BeginInvoke(obj);
        }

        private void GetAddressButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference1.GeocodeServiceClient client = new ServiceReference1.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            ServiceReference1.ReverseGeocodeRequest request = new ServiceReference1.ReverseGeocodeRequest();
            request.Credentials = new ServiceReference1.Credentials();
            request.Credentials.ApplicationId = "AtaeM1towemiWqFlso-tLXG9DP9v2JFzOQQHxe9XBnhLoD31NkpovUP1aAGm1lso";
            ServiceReference1.Location loc = new ServiceReference1.Location();
            loc.Longitude = Convert.ToDouble(LongTextBox.Text);
            loc.Latitude = Convert.ToDouble(LatTextBox.Text);
            request.Location = loc;
            client.ReverseGeocodeCompleted += new EventHandler<ServiceReference1.ReverseGeocodeCompletedEventArgs>(client_ReverseGeocodeCompleted);
            client.ReverseGeocodeAsync(request);
        }

        void client_ReverseGeocodeCompleted(object sender, ServiceReference1.ReverseGeocodeCompletedEventArgs e)
        {
            del locDel = delegate
            {
                Address.Text += e.Result.Results[0].Address.FormattedAddress;
            };
            this.Dispatcher.BeginInvoke(locDel);
        }
    }
}