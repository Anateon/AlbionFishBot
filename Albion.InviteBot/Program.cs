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

namespace Albion.InviteBot
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
        public static IntPtr handle = IntPtr.Zero;
        public static Mutex mutexObj = new Mutex();
        public static Thread InviteBoThread;
        public static Point PlusPoint = new Point(45, 607);
        public static Point NickNameStringPoint = new Point(515, 395);
        public static bool inviteBotIsWork = false;
        public static Queue<string> chelList = new Queue<string>();
        public static HashSet<string> invitedList = new HashSet<string>();

        public static int allPlayers = 0;
        public static int invitedPlayers = 0;
        private static void Main(string[] args)
        {
            ReceiverBuilder builder = ReceiverBuilder.Create();
            builder.AddEventHandler(new NewChatacterEventHandler());
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
            HotKeyManager.RegisterHotKey(Keys.D1, KeyModifiers.Alt);
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
                case Keys.NumPad1: 
                    handle = GetForegroundWindow();
                    InputCloseWindowSender.handle = handle;
                    Console.WriteLine($"Handle: {handle}");
                    break;
                case Keys.D1: // set mouse pos + start/stop fishing
                    if (handle == IntPtr.Zero)
                        break;
                    if (inviteBotIsWork)
                    {
                        inviteBotIsWork = false;
                        InviteBoThread.Abort();
                        Console.WriteLine("InvitingStop");
                    }
                    else
                    {
                        inviteBotIsWork = true;
                        InviteBoThread = new Thread(new ThreadStart(Program.TriggerClicker));
                        InviteBoThread.Start();
                        Console.WriteLine($"InvitingStrat");
                    }
                    break;

            }

        }

        public static void TriggerClicker()
        {
            Point convertPlusButtonZone;
            Point convertNickStrPoint;
            while (inviteBotIsWork)
            {
                if (chelList.Count != 0)
                {
                    string name = chelList.Dequeue();
                    try
                    {
                        invitedList.Add(name);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR");
                    }
                    convertPlusButtonZone = CusoreConvertFromGamePosToWin(PlusPoint.X, PlusPoint.Y);
                    convertNickStrPoint = CusoreConvertFromGamePosToWin(NickNameStringPoint.X, NickNameStringPoint.Y);
                    Console.ForegroundColor = ConsoleColor.Green;
                    SetForegroundWindow(handle);
                    Console.WriteLine($"{invitedPlayers++}\tIVITE:\t{name}");
                    var scanCodes = GetScancodes(name);
                    Console.ResetColor();
                    Thread.Sleep(200);
                    InputSender.ClickKey(0x22); // G
                    Thread.Sleep(200);
                    InputSender.SetCursorPosition(convertPlusButtonZone.X, convertPlusButtonZone.Y); // MOVE TO +
                    Thread.Sleep(200);
                    InputSender.ClickMouse(); // CLICK TO +
                    InputSender.SetCursorPosition(convertPlusButtonZone.X+1, convertPlusButtonZone.Y+1); // MOVE TO +
                    Thread.Sleep(200);
                    InputSender.ClickMouse(); // CLICK TO +
                    InputCloseWindowSender.ClickMouse(convertPlusButtonZone.X, convertPlusButtonZone.Y);
                    Thread.Sleep(200);
                    InputSender.SetCursorPosition(convertNickStrPoint.X, convertNickStrPoint.Y); // MOVE TO STR
                    Thread.Sleep(200);
                    InputCloseWindowSender.ClickMouse(NickNameStringPoint.X, NickNameStringPoint.Y);
                    Thread.Sleep(200);
                    foreach (var code in scanCodes)
                    {
                        InputSender.ClickKey(code);
                        Thread.Sleep(50);
                    }
                    InputSender.ClickKey(0x1C); // CLICK ENTER
                    Thread.Sleep(150);
                    InputSender.ClickKey(0x01); // CLICK ESC
                    Thread.Sleep(150);
                    InputSender.ClickKey(0x01); // CLICK ESC
                    Thread.Sleep(150);
                    InputSender.ClickKey(0x01); // CLICK ESC
                    Thread.Sleep(150);
                    InputSender.ClickKey(0x01); // CLICK ESC
                    //InputCloseWindowSender.ClickMouse(convertPlusButtonZone.X, convertPlusButtonZone.Y);
                }
                else
                {
                    Thread.Sleep(250);
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

        public static List<byte> GetScancodes(string str)
        {
            var tmp = new List<byte>();
            var Upper = str.ToUpper();
            foreach (var ch in Upper)
            {
                switch (ch)
                {
                    case 'A':	tmp.Add(0x1E); break;
                    case 'B':	tmp.Add(0x30); break;
                    case 'C':	tmp.Add(0x2E); break;
                    case 'D':	tmp.Add(0x20); break;
                    case 'E':	tmp.Add(0x12); break;
                    case 'F':	tmp.Add(0x21); break;
                    case 'G':	tmp.Add(0x22); break;
                    case 'H':	tmp.Add(0x23); break;
                    case 'I':	tmp.Add(0x17); break;
                    case 'J':	tmp.Add(0x24); break;
                    case 'K':	tmp.Add(0x25); break;
                    case 'L':	tmp.Add(0x26); break;
                    case 'M':	tmp.Add(0x32); break;
                    case 'N':	tmp.Add(0x31); break;
                    case 'O':	tmp.Add(0x18); break;
                    case 'P':	tmp.Add(0x19); break;
                    case 'Q':	tmp.Add(0x10); break;
                    case 'R':	tmp.Add(0x13); break;
                    case 'S':	tmp.Add(0x1F); break;
                    case 'T':	tmp.Add(0x14); break;
                    case 'U':	tmp.Add(0x16); break;
                    case 'V':	tmp.Add(0x2F); break;
                    case 'W':	tmp.Add(0x11); break;
                    case 'X':	tmp.Add(0x2D); break;
                    case 'Y':	tmp.Add(0x15); break;
                    case 'Z':	tmp.Add(0x2C); break;
                    case '0':	tmp.Add(0x0B); break;
                    case '1':	tmp.Add(0x2); break;
                    case '2':	tmp.Add(0x3); break;
                    case '3':	tmp.Add(0x4); break;
                    case '4':	tmp.Add(0x5); break;
                    case '5':	tmp.Add(0x6); break;
                    case '6':	tmp.Add(0x7); break;
                    case '7':	tmp.Add(0x8); break;
                    case '8':	tmp.Add(0x9); break;
                    case '9':	tmp.Add(0x0A); break;
                    case '~':	tmp.Add(0x29); break;
                    case '-':	tmp.Add(0x0C); break;
                    case '=':	tmp.Add(0x0D); break;
                    case '\\':	tmp.Add(0x2B); break;
                    case '[':	tmp.Add(0x1A); break;
                    case ']':	tmp.Add(0x1B); break;
                    case ';':	tmp.Add(0x27); break;
                    case '\'':	tmp.Add(0x28); break;
                    case ',':	tmp.Add(0x33); break;
                    case '.':	tmp.Add(0x34); break;
                    case '/':	tmp.Add(0x35); break;
                }
            }
            return tmp;
        }
    }
}