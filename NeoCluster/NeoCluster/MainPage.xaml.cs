using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Networking.Sockets;
using Windows.UI.Popups;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace NeoCluster
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int packet = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            lblDebug.Text = "Started";

            await Task.Run(async () => {
                using (var Client = new UdpClient(5678))
                {
                    int packet = 0;

                    while (true)
                    {
                        var ReceivedData = await Client.ReceiveAsync();
                        packet += 1;

                        byte[] data = ReceivedData.Buffer;

                        float maxRpm = BitConverter.ToSingle(data, 8);
                        float idleRpm = BitConverter.ToSingle(data, 12);
                        float currentRpm = BitConverter.ToSingle(data, 16);

                        float speed = BitConverter.ToSingle(data, 256) * 3.6f;
                        int gear = data[319];

                        float barRpm = 0;
                        if (maxRpm > 0)
                        {
                            barRpm = (currentRpm / maxRpm) * 100.0f;
                        }
                        int rSpeed = (int)speed;

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                            lblGear.Text = "" + gear;
                            lblSpeed.Text = "" + rSpeed;
                            prgRpm.Value = barRpm;

                            lblDebug.Text = "Packets received: " + packet;
                        });
                    }
                }
            });
        }
    }
}
