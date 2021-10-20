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
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        private static IPhotonReceiver receiver;
        public static Poplovok Poplovok = new Poplovok();
        public static int NeedOwnerID;
        public static IntPtr handle = IntPtr.Zero;
        public static int LastOwnerID;
        public static Poplovok MyPos = new Poplovok();
        public static Mutex mutexObj = new Mutex();
        public static bool fishingStatus = false;
        public static Point mousePoint;
        public static List<Point> ListMousPoints = new List<Point>();
        public static Point TriggerPoint = new Point(1025, 553);
        public static Thread ClickThread;
        public static int randomIndex = 0;

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
            HotKeyManager.RegisterHotKey(Keys.D2, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(Keys.D1, KeyModifiers.Alt);
            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Alt);
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
                case Keys.NumPad2: // set red zone
                    if (handle == IntPtr.Zero)
                        break;
                    TriggerPoint = CusoreConvertFromWinPosToGame(InputSender.GetCursorPosition());
                    //TriggerPoint = new Point(658, 401); // 1280*720
                    TriggerPoint = new Point(561, 426); // 1024*768
                    Console.WriteLine($"green zone: {TriggerPoint}");
                    break;
                case Keys.D2: // set mouse pos + start/stop fishing
                    if (handle == IntPtr.Zero)
                        break;
                    if (fishingStatus)
                    {
                        fishingStatus = false;
                        Program.ClickThread.Abort();
                        Console.WriteLine("fishingStop");
                    }
                    else
                    {
                        fishingStatus = true;
                        Program.ClickThread = new Thread(new ThreadStart(Program.TriggerClicker));
                        Program.ClickThread.Start();
                        Console.WriteLine($"fishingStart mousePos: {ListMousPoints}");
                    }
                    break;
                case Keys.D1:
                    ListMousPoints.Add(CusoreConvertFromWinPosToGame(InputSender.GetCursorPosition()));
                    Console.WriteLine("NewPoint");
                    break;
                case Keys.Oemtilde:
                    ListMousPoints.Clear();
                    Console.WriteLine("ClearPoint");
                    break;
            }
        }

        public static void TriggerClicker()
        {
            Point convertPoplovokZone = CusoreConvertFromGamePosToWin(TriggerPoint.X, TriggerPoint.Y);
            List<Point> convertMousePoints = new List<Point>();
            foreach (var VARIABLE in ListMousPoints)
            {
                convertMousePoints.Add(CusoreConvertFromGamePosToWin(VARIABLE.X, VARIABLE.Y));
            }
            //Point convertMousePoint = CusoreConvertFromGamePosToWin(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
            Bitmap screen = new Bitmap(1, 1);
            var screenGfx = Graphics.FromImage(screen);
            bool mouseIsPerss = false;
            InputCloseWindowSender.handle = handle;

            if (Poplovok.status == 4)
            {
                randomIndex = (++randomIndex)%convertMousePoints.Count;
                SetForegroundWindow(handle);
                InputSender.SetCursorPosition(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                Thread.Sleep(50);
                Console.WriteLine($"{Poplovok.status}");
                mouseIsPerss = false;
                InputSender.ClickMouseUp();
                Thread.Sleep(200);
                InputSender.ClickKey(0x1f);
                Thread.Sleep(200);
                InputSender.ClickMouseDown();
                Thread.Sleep(2050);
                InputSender.ClickMouseUp();
            }
            while (fishingStatus && (Poplovok.status == 2 || Poplovok.status == 3 || Poplovok.status == 4 || Poplovok.status == 5))
            {
                switch (Poplovok.status)
                {
                    case 2: // поклёвка
                        //SetForegroundWindow(handle);
                        InputSender.SetCursorPosition(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                        Thread.Sleep(20);
                        InputCloseWindowSender.ClickMouse(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                        Poplovok.status = 3;
                        SetForegroundWindow(handle);
                        Thread.Sleep(200);
                        break;
                    case 3: // тянуть
                        SetForegroundWindow(handle);
                        screenGfx.CopyFromScreen(convertPoplovokZone.X, convertPoplovokZone.Y, 0, 0, screen.Size);
                        byte B = screen.GetPixel(0, 0).B;
                        Console.WriteLine($"{Poplovok.status}/{screen.GetPixel(0, 0).R};{screen.GetPixel(0, 0).G};{screen.GetPixel(0, 0).B}");
                        InputSender.SetCursorPosition(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                        if (B < 75) // надо жать
                        {
                            if (!mouseIsPerss)
                            {
                                Console.WriteLine("ТЯНУ");
                                InputCloseWindowSender.ClickMouseDown(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                                mouseIsPerss = true;
                            }
                        }
                        else // не надо жать
                        {
                            if (mouseIsPerss)
                            {
                                Console.WriteLine("НЕ ТЯНУ ЧУТЬ ЧУТЬ");
                                InputCloseWindowSender.ClickMouseUp(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                                Thread.Sleep(15);
                                mouseIsPerss = false;
                                //InputCloseWindowSender.ClickMouseDown(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                            }
                        }
                        Thread.Sleep(5);
                        break;
                    default: // улов
                        randomIndex = (++randomIndex) % convertMousePoints.Count;
                        InputCloseWindowSender.ClickMouseUp(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                        SetForegroundWindow(handle);
                        InputSender.SetCursorPosition(convertMousePoints[randomIndex].X, convertMousePoints[randomIndex].Y);
                        Thread.Sleep(50);
                        Console.WriteLine($"{Poplovok.status}");
                        mouseIsPerss = false;
                        InputSender.ClickMouseUp();
                        Thread.Sleep(200);
                        InputSender.ClickKey(0x1f);
                        Thread.Sleep(200);
                        InputSender.ClickMouseDown();
                        Thread.Sleep(2050);
                        InputSender.ClickMouseUp();
                        return;
                }
            }
        }

        public static Point CusoreConvertFromWinPosToGame(InputSender.POINT winPoint)
        {
            Rect gameRectangle = new Rect();
            var game = GetWindowRect(handle, ref gameRectangle);
            return new Point(winPoint.X - gameRectangle.left, winPoint.Y - gameRectangle.top);
        }

        public static Point CusoreConvertFromGamePosToWin(int X, int Y)
        {
            Rect gameRectangle = new Rect();
            var game = GetWindowRect(handle, ref gameRectangle);
            return new Point(X + gameRectangle.left, Y + gameRectangle.top);
        }

    }
}