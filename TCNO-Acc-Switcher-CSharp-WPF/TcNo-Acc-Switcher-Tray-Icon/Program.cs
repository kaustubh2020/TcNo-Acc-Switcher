using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TcNo_Acc_Switcher_Tray_Icon.Properties;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TcNo_Acc_Switcher_Tray_Icon
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (SelfAlreadyRunning())
            {
                Console.WriteLine("TcNo Account Switcher Tray is already running");
                Environment.Exit(99);
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppCont());
        }
        private static bool SelfAlreadyRunning()
        {
            Process[] processes = Process.GetProcesses();
            Process currentProc = Process.GetCurrentProcess();
            foreach (Process process in processes)
            {
                if (currentProc.ProcessName == process.ProcessName && currentProc.Id != process.Id)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class AppCont : ApplicationContext
    {
        string mainProgram = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "TcNo Account Switcher.exe");

        private NotifyIcon trayIcon;

        public AppCont()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Renderer = new ContentRenderer();
            contextMenu.Items.Add(new ToolStripMenuItem()
            {
                Name = "START",
                Text = "Start TcNo Account Switcher",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(255, 34, 34, 34)
            });
            contextMenu.Items.Add(new ToolStripMenuItem()
            {
                Name = "EXIT",
                Text = "Exit",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(255, 34, 34, 34)
            });
            contextMenu.ItemClicked += new ToolStripItemClickedEventHandler(contextMenu_ItemClicked);


            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.icon,
                ContextMenuStrip = contextMenu,
                Visible = true
            };
            trayIcon.DoubleClick += new EventHandler(NotifyIcon_DoubleClick);
        }
        private void NotifyIcon_DoubleClick(object sender, System.EventArgs e)
        {
            startSwitcher();
        }

        // Function to check if the .exe is already running
        bool alreadyRunning()
        {
            return Process.GetProcessesByName("TcNo Account Switcher").Length > 0;
        }

        // Start with Windows login, using https://stackoverflow.com/questions/15191129/selectively-disabling-uac-for-specific-programs-on-windows-programatically for automatic administrator.
        // Adding to Start Menu shortcut also creats "Start in Tray", which is a shortcut to this program. 


        [DllImport("user32")]
        private static extern bool SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        void bringToFront()
        {
            var proc = Process.GetProcessesByName("TcNo Account Switcher").FirstOrDefault();
            if (proc != null && proc.MainWindowHandle != IntPtr.Zero)
            {
                int SW_RESTORE = 9;
                ShowWindow(proc.MainWindowHandle, SW_RESTORE);
                SetForegroundWindow(proc.MainWindowHandle);
            }
        }
        void closeMainProcess()
        {
            var proc = Process.GetProcessesByName("TcNo Account Switcher").FirstOrDefault();
            if (proc != null && proc.MainWindowHandle != IntPtr.Zero)
            {
                proc.CloseMainWindow();
                proc.WaitForExit();
            }
        }
        private void startSwitcher() {
            if (alreadyRunning())
            {
                // Already open
                bringToFront();
            }
            else
            {
                string processName = mainProgram;
                if (File.Exists(mainProgram))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = processName;
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    Process.Start(startInfo);
                }
                else
                {
                    MessageBox.Show("Could not open the main .exe. Make sure it exists.\n\nI attempted to open: " + mainProgram, "TcNo Account Switcher - Tray launch fail");
                }
            }
        }
        void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (item.Name == "START")
            {
                startSwitcher();
            } else if (item.Name == "EXIT")
            {
                trayIcon.Visible = false;
                closeMainProcess();
                Application.Exit();
            }
        }
        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        #region ContextMenuStrip Style Section
        private class ContentRenderer : ToolStripProfessionalRenderer
        {
            public ContentRenderer() : base(new MyColors()) { }
        }

        private class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(255, 24, 24, 24); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(255, 34, 34, 34); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(255, 34, 34, 34); }
            }
        }
        #endregion
    }
}
