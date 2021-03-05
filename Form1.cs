using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        const int MOUSEEVENTF_LEFTDOWN = 2;
        const int MOUSEEVENTF_LEFTUP = 4;
        const int MOUSEEVENTF_RIGHTDOWN = 8;
        const int MOUSEEVENTF_RIGHTUP = 16;
        const int MOUSEEVENTF_MIDDLEUP = 32;
        const int MOUSEEVENTF_MIDDLEDOWN = 64;
        public const int START_HOTKEY = 1;
        public const int STOP_HOTKEY = 2;
        //input type constant
        const int INPUT_MOUSE = 0;
        private int Duration = 1000;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        public static MousePoint GetCursorPosition()
        {
            var gotPoint = GetCursorPos(out MousePoint currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        void ClickAtCurrentPosition(int duration)
        {
            if (duration < 0)
            {
                MessageBox.Show("Bad click duration");
                return;
            }
            else
            {
            }

            MousePoint mousePoint = GetCursorPosition();
            INPUT i = new INPUT
            {
                type = INPUT_MOUSE
            };
            i.mi.dx = mousePoint.X;
            i.mi.dy = mousePoint.Y;
            i.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            i.mi.dwExtraInfo = IntPtr.Zero;
            i.mi.mouseData = 0;
            i.mi.time = 0;

            SendInput(1, ref i, Marshal.SizeOf(i));

            if(duration != 0)
            {
                Thread.Sleep(duration);
            }

            mousePoint = GetCursorPosition();
            i.mi.dx = mousePoint.X;
            i.mi.dy = mousePoint.Y;
            i.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            SendInput(1, ref i, Marshal.SizeOf(i));
        }

        public Form1()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, START_HOTKEY, 0, 112);
            RegisterHotKey(this.Handle, STOP_HOTKEY, 0, 113);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == START_HOTKEY)
            {
                Start();
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == STOP_HOTKEY)
            {
                Stop();
            }
            base.WndProc(ref m);
        }

        private void Clicktimer_Tick(object sender, EventArgs e)
        {
            ClickAtCurrentPosition(Duration);
        }

        private void Start()
        {
            if (clicktimer.Enabled)
            {
                MessageBox.Show("Autoclicker already running", "Already running");
            }
            else
            {
                bool intervalError = false;
                bool durationError = false;
                int interval = 1000;
                try
                {
                    interval = 1000 * Convert.ToInt32(textBox3.Text) + Convert.ToInt32(textBox4.Text);
                }
                catch
                {
                    intervalError = true;
                }
                try
                {
                    Duration = 1000 * Convert.ToInt32(textBox2.Text) + Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    durationError = true;
                }
                if (!intervalError)
                {
                    if (!durationError)
                    {
                        groupBox1.Enabled = false;
                        groupBox3.Enabled = false;
                        clicktimer.Interval = interval;
                        clicktimer.Start();
                    }
                    else
                    {
                        MessageBox.Show("Invalid duration time enetered", "Invalid duration");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid time interval enetered", "Invalid interval");
                }
            }
        }

        private void Stop()
        {
            if (!clicktimer.Enabled)
            {
                MessageBox.Show("AutoClicker is not running", "Not running");
            }
            else 
            {

                groupBox1.Enabled = true;
                groupBox3.Enabled = true;
                clicktimer.Stop();
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (GetAsyncKeyState(0x70) != 0)
            {
                if(clicktimer.Enabled) Stop(); else Start();
            }    
        }
    }
}
