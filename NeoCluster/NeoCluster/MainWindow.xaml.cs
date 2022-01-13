﻿using System;
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

namespace NeoCluster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => {
                using (var Client = new UdpClient(5678))
                {
                    while (true)
                    {
                        var ReceivedData = await Client.ReceiveAsync();

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

                        Dispatcher.Invoke(() => {
                            lblGear.Text = "" + gear;
                            lblSpeed.Text = "" + rSpeed;
                            prgRpm.Value = barRpm;
                        });
                    }
                }
            });
        }
    }
}