using Albion.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using PacketDotNet;
using SharpPcap;

namespace AlbionFishBot
{
    class Program
    {
        private static IPhotonReceiver receiver;
        public static Poplovok Poplovok;
        public static string NeedPoplovokID;
        public static Poplovok MyPos;
        public static Mutex mutexObj = new Mutex();

        private static void Main(string[] args)
        {
            ReceiverBuilder builder = ReceiverBuilder.Create();
            builder.AddEventHandler(new FishingMiniGameEventHandler());
            builder.AddEventHandler(new LeaveEventHandler());
            builder.AddRequestHandler(new MoveRequestHandler());
            receiver = builder.Build();
            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (var device in devices)
            {
                new Thread(() =>
                    {
                        device.OnPacketArrival += new PacketArrivalEventHandler(PacketHandler);
                        device.Open(DeviceMode.Promiscuous, 1000);
                        device.StartCapture();
                    })
                    .Start();
            }
            Console.Read();
        }

        private static void PacketHandler(object sender, CaptureEventArgs e)
        {
            UdpPacket packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract<UdpPacket>();
            if (packet != null && (packet.SourcePort == 5056 || packet.DestinationPort == 5056))
            {
                receiver.ReceivePacket(packet.PayloadData);
            }
        }
    }
}