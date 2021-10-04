using Albion.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using PacketDotNet;
using SendInputsDemo;
using SharpPcap;

namespace Albion.FishBot
{
    public class Program
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private static IPhotonReceiver receiver;
        public static Poplovok Poplovok = new Poplovok();
        public static int NeedOwnerID;
        public static IntPtr handle;
        public static int LastOwnerID;
        public static Poplovok MyPos = new Poplovok();
        public static Mutex mutexObj = new Mutex();
        public static bool fishingStatus = false;
        public static Point mousePoint;
        public static Thread ClickThread;
        public static ScreenCapture screenCapture = new ScreenCapture();

        private static void Main(string[] args)
        {
            MyPos.X = 0;
            MyPos.Y = 0;
            NeedOwnerID = 0;
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
            HotKeyManager.RegisterHotKey(Keys.NumPad1, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(Keys.NumPad2, KeyModifiers.Alt);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            
        }

        private static void PacketHandler(object sender, CaptureEventArgs e)
        {
            UdpPacket packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data).Extract<UdpPacket>();
            if (packet != null && (packet.SourcePort == 5056 || packet.DestinationPort == 5056))
            {
                receiver.ReceivePacket(packet.PayloadData);
            }
        }
        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.NumPad1: // set poplovok
                    NeedOwnerID = LastOwnerID;
                    handle = GetForegroundWindow();
                    Console.WriteLine($"set poplovok ID: {NeedOwnerID} handle: {handle}");
                    break;
                case Keys.NumPad2: // set mouse pos
                    if (fishingStatus)
                    {
                        fishingStatus = false;
                        Program.ClickThread.Abort();
                        Console.WriteLine("fishingStop");
                    }
                    else
                    {
                        fishingStatus = true;
                        mousePoint.X = InputSender.GetCursorPosition().X;
                        mousePoint.Y = InputSender.GetCursorPosition().Y;
                        Program.ClickThread = new Thread(new ThreadStart(Program.TriggerClicker));
                        Program.ClickThread.Start();
                        Console.WriteLine($"fishingStart mousePos: {mousePoint}");
                    }
                    break;
            }
        }

        public static void TriggerClicker()
        {
            Bitmap screen = new Bitmap(1, 1);
            var screenGfx = Graphics.FromImage(screen);
            bool mouseIsPerss = false;
            while (fishingStatus && (Poplovok.status == 2 || Poplovok.status == 3 || Poplovok.status == 4))
            {
                switch (Poplovok.status)
                {
                    case 2: // поклёвка
                        InputSender.SetCursorPosition(mousePoint.X, mousePoint.Y);
                        InputSender.ClickMouse();
                        Poplovok.status = 3;
                        Thread.Sleep(200);
                        break;
                    case 3: // тянуть
                        screenGfx.CopyFromScreen(1025, 553, 0, 0, screen.Size);
                        //byte R = screen.GetPixel(1025, 553).R;
                        //byte G = screen.GetPixel(1025, 553).G;
                        byte B = screen.GetPixel(0, 0).B;
                        Console.WriteLine($"{Poplovok.status}/{B}");
                        InputSender.SetCursorPosition(mousePoint.X, mousePoint.Y);
                        if (!(B > 100)) // надо жать
                        {
                            if (!mouseIsPerss)
                            {
                                Console.WriteLine("ТЯНУ");
                                InputSender.ClickMouseDown();
                                mouseIsPerss = true;
                            }
                        }
                        else // не надо жать
                        {
                            if (mouseIsPerss)
                            {
                                Console.WriteLine("НЕ ТЯНУ ЧУТЬ ЧУТЬ");
                                InputSender.ClickMouseUp();
                                Thread.Sleep(30);
                                InputSender.ClickMouseDown();
                            }
                        }
                        Thread.Sleep(30);
                        break;
                    default: // улов
                        InputSender.SetCursorPosition(mousePoint.X, mousePoint.Y);
                        Thread.Sleep(50);
                        Console.WriteLine($"{Poplovok.status}");
                        mouseIsPerss = false;
                        InputSender.ClickMouseUp();
                        Thread.Sleep(200);
                        InputSender.ClickKey(0x1f);
                        InputSender.ClickKey(0x1f);
                        Thread.Sleep(200);
                        InputSender.ClickMouseDown();
                        Thread.Sleep(2000);
                        InputSender.ClickMouseUp();
                        return;
                }
            }
        }

    }
}