using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Albion.Network.Example
{
    internal class Program
    {
        private static IPhotonReceiver receiver;

        private static void Main(string[] args)
        {
            Dictionary<string, ChelInfo> chelDictionary = new Dictionary<string, ChelInfo>();
            ChelInfo MyInfo = new ChelInfo();
            ReceiverBuilder builder = ReceiverBuilder.Create();
            long testTime = DateTime.Now.Ticks;

            //builder.AddRequestHandler(new MoveRequestHandler(MyInfo)); // мое перемещение
            //builder.AddEventHandler(new MoveEventHandler(chelDictionary)); // Движения типов 
            //builder.AddEventHandler(new NewCharacterEventHandler(chelDictionary)); //новые типы
            //builder.AddEventHandler(new NewMobEventHandler()); // новые мобы
            builder.AddEventHandler(new TESTEventHandler());

            receiver = builder.Build();

            Console.WriteLine("Start");

            CaptureDeviceList devices = CaptureDeviceList.Instance;
            foreach (var device in devices)
            {
                new Thread(() =>
                {
                    Console.WriteLine($"Open... {device.Description}");

                    device.OnPacketArrival += new PacketArrivalEventHandler(PacketHandler);
                    device.Open(DeviceMode.Promiscuous, 1000);
                    device.StartCapture();
                })
                .Start();
            }
            //while (true)
            //{
            //    Console.SetCursorPosition(0, 0);
            //    Console.Clear();
            //    var timeNow = DateTime.Now.AddMinutes(-1);
            //    Console.WriteLine($"NAME \t\t DISTANCE \t\t ANGLE");
            //    try
            //    {
            //        foreach (var var in chelDictionary)
            //        {
            //            if (var.Value.time < timeNow) // если надо убрать старых челов
            //                chelDictionary.Remove(var.Key);
            //            else
            //                Console.WriteLine($"{var.Value.name} \t\t {Math.Sqrt(Math.Pow(var.Value.X - MyInfo.X, 2) + Math.Pow(var.Value.Y - MyInfo.Y, 2))} \t\t {Angle(var.Value.X - MyInfo.X, var.Value.Y - MyInfo.Y)}°");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //    }
            //    Console.WriteLine("===================================");
            //    Thread.Sleep(200);
            //}//←↑→↓
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

        public static double Angle(double X, double Y)
        {
            double grad = (Math.Atan2(X, Y) * (180 / Math.PI))+45;
            if (grad < 0)
            {
                return grad + 360;
            }
            else
            {
                return grad;
            }
        }
    }
}
