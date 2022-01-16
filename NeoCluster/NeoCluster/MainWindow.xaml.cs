using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Devices.Sensors;

namespace NeoCluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBlock[] gears;
        SolidColorBrush disabledBrush = new SolidColorBrush(Color.FromRgb(130, 130, 130));
        SolidColorBrush enabledBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public MainWindow()
        {
            InitializeComponent();
            gears = new TextBlock[] { lblGR, lblG1, lblG2, lblG3, lblG4, lblG5, lblG6, lblG7 };
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => {
                using (var Client = new UdpClient(5678))
                {
                    while (true)
                    {
                        var ReceivedData = await Client.ReceiveAsync();

                        byte[] data = ReceivedData.Buffer;

                        bool raceOn = (data[0] > 0);

                        float maxRpm = BitConverter.ToSingle(data, 8);
                        float idleRpm = BitConverter.ToSingle(data, 12);
                        float currentRpm = BitConverter.ToSingle(data, 16);

                        float x = BitConverter.ToSingle(data, 244);
                        float y = BitConverter.ToSingle(data, 248);
                        float z = BitConverter.ToSingle(data, 252);

                        float speed = BitConverter.ToSingle(data, 256) * 3.6f;
                        float barSpeed = (speed / 400.0f) * 100.0f;
                        int gear = data[319];
                        int clampedGear = Math.Clamp(gear, 0, gears.Length-1);

                        float bestLapTime = BitConverter.ToSingle(data, 296);
                        float lastLapTime = BitConverter.ToSingle(data, 300);
                        float currentLapTime = BitConverter.ToSingle(data, 304);
                        float currentRaceTime = BitConverter.ToSingle(data, 308);

                        byte lap = data[312];
                        byte position = data[314];

                        float barRpm = 0;
                        if (maxRpm > 0)
                        {
                            barRpm = (currentRpm / maxRpm) * 100.0f;
                        }
                        int rSpeed = (int)speed;

                        TimeSpan currentLapTimespan = TimeSpan.FromSeconds((double)(new decimal(currentLapTime)));
                        TimeSpan bestLapTimespan = TimeSpan.FromSeconds((double)(new decimal(currentLapTime)));
                        TimeSpan currentRaceTimespan = TimeSpan.FromSeconds((double)(new decimal(currentRaceTime)));

                        Dispatcher.Invoke(() => {
                            for(int i=0; i<gears.Length; i++)
                            {
                                gears[i].Foreground = disabledBrush;
                            }
                            gears[clampedGear].Foreground = enabledBrush;

                            lblSpeed.Text = "" + rSpeed;
                            lblDebug.Text = "" + gear;
                            prgRpm.Value = barRpm;
                            prgSpeed.Value = barSpeed;

                            lblBestLap.Text = bestLapTimespan.ToString(@"mm\:ss\:fff");
                            lblCurrentLap.Text = currentLapTimespan.ToString(@"mm\:ss\:fff");
                            lblRaceTime.Text = currentRaceTimespan.ToString(@"mm\:ss\:fff");

                            lblPosition.Text = position + "/12";
                            lblLaps.Text = (int)lap + 1 + "/3";
                            lblClock.Text = DateTime.Now.ToString("t");

                            lblConnected.Text = "Online";
                            lblConnected.Foreground = enabledBrush;
                        });
                    }
                }
            });
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowStyle != WindowStyle.SingleBorderWindow)
            {
                this.ResizeMode = ResizeMode.CanResize;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.Topmost = false;
            }
            else
            {
                this.ResizeMode = ResizeMode.NoResize;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
            }
        }
    }
}
