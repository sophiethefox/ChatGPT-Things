// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe OsuKeyOverlay.cs

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO; // Add a reference to the System.IO namespace

namespace HelloWorldOverlay
{
    public class OverlayForm : Form
    {
        private static int XClicks = 0; // Change this field to a static field
        private static int ZClicks = 0;
		private static int clicksLastSecond = 0;
        private static int clicksPerSecond = 0; // Declare a clicksPerSecond variable
		private static int previousClicksPerSecond = 0;
		
        private static Timer timer; // Declare a Timer control

        private static Label label;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public OverlayForm()
        {
			if (File.Exists("keypresses.txt"))
            {
				string[] lines = File.ReadAllLines("keypresses.txt");
				XClicks = int.Parse(lines[0]);
				ZClicks = int.Parse(lines[1]);
			}
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_HIDE);

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Red;
            this.TransparencyKey = Color.Red;
            this.TopMost = true;
            this.AllowTransparency = true;

            label = new Label();
            label.Text = "Z: " + ZClicks + " | X: " + XClicks + " | Clicks/s: " + clicksPerSecond;
            label.ForeColor = Color.White;
            label.Font = new Font("Arial", 24);
            label.AutoSize = true;

            Size dimensions = TextRenderer.MeasureText(
                label.Text + "0000000000000000000000000",
                label.Font
            );
            this.Width = dimensions.Width;
            this.Height = dimensions.Height;

            this.Controls.Add(label);

            timer = new Timer(); // Initialize the Timer control
            timer.Interval = 1000; // Set the interval to 1 second
            timer.Tick += new EventHandler(OnTimerTick); // Set the tick event handler
            timer.Start(); // Start the timer

            _hookID = SetHook(_proc);
			
			this.FormClosing += new FormClosingEventHandler(OnFormClosing);
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            label.Text = "Z: " + ZClicks + " | X: " + XClicks + " | Clicks/s: " + clicksLastSecond;
			previousClicksPerSecond = clicksLastSecond;
			clicksLastSecond = 0;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                createParams.ExStyle |= 0x8; // WS_EX_TOPMOST
                return createParams;
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    proc,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == (int)Keys.X)
                {
                    XClicks++;
					clicksLastSecond++;
                    label.Text = "Z: " + ZClicks + " | X: " + XClicks + " | Clicks/s: " + previousClicksPerSecond;
                }
                else if (vkCode == (int)Keys.Z)
                {
                    ZClicks++;
					clicksLastSecond++;
                    label.Text = "Z: " + ZClicks + " | X: " + XClicks + " | Clicks/s: " + previousClicksPerSecond;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

        private static Process GetActiveProcess()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);
            return p;
        }
		
		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			// Save the data to the text file
			string[] lines = {XClicks.ToString(), ZClicks.ToString()};
			File.WriteAllLines("keypresses.txt", lines);
		}

        public static void Main()
        {
            OverlayForm overlay = new OverlayForm();
            Application.Run(overlay);
            UnhookWindowsHookEx(_hookID);
        }
    }
}
