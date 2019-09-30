using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualBasic;

namespace MirrorCaster
{
    public partial class MainForm : Form
    {
        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(
        IntPtr hwnd,
        int nIndex,
        uint dwNewLong
        );

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(
        IntPtr hwnd,
        int nIndex
        );

        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(
        IntPtr hwnd,
        int crKey,
        int bAlpha,
        int dwFlags
        );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        Process stdout_process = new Process();
        Process stdin_process = new Process();
        StreamPipe RePipe;

        bool is_casting_value = false;
        bool is_casting
        {
            get
            {
                return is_casting_value;
            }
            set
            {
                is_casting_value = value;
                stopCastButton.Enabled = value;
                powerKeyButton.Enabled = value;
                backKeyButton.Enabled = value;
                homeKeyButton.Enabled = value;
                mutiKeyButton.Enabled = value;
                menuKeyButton.Enabled = value;
                volUpKeyButton.Enabled = value;
                volDownKeyButton.Enabled = value;
            }
        }

        DeviceInfoData deviceInfoData = new DeviceInfoData(); // device info form adb
        DeviceInfoData instart_deviceInfoData = new DeviceInfoData(); // device info at start cast

        double castMbitRate = 30; // 16M适中

        public static NamedPipeClientStream client;

        public MainForm()
        {
            InitializeComponent();
#if DEBUG
            testButton.Visible = true;
#endif
        }

        private void StartCastButton_Click(object sender, EventArgs e)
        {
            string inputText = string.Empty;
            try
            {
                inputText = Interaction.InputBox("请输入投屏码率（Mbps）：\r\n\r\n<10：适合互联网传输\r\n<30：适合一般手机通过USB传输\r\n<100：适合编码能力强的手机通过家庭局域网（百兆）内传输\r\n<=200：适合编码能力超强的手机通过USB传输\r\n\r\n建议值：30", "准备投屏", $"{castMbitRate}", -1, -1);
                castMbitRate = double.Parse(inputText);
                if (castMbitRate <= 0 || castMbitRate > 200)
                {
                    MessageBox.Show("码率（Mbps）必须大于0且不超过200", "警告");
                }
                else
                {
                    StartCastAction();
                }
            }
            catch
            {
                if (inputText.Length > 0)
                    MessageBox.Show("请输入正确的码率（Mbps）", "警告");
            }
        }

        private void StartCastAction()
        {
            StopCast();
            if (UpdateScreenDeviceInfo())
            {
                StartCast();
            }
            else
            {
                MessageBox.Show("找不到任何设备或模拟器", "警告");
            }
        }

        private void StartCast()
        {
            StdOut();
            StdIn();
            RePipe = new StreamPipe(stdout_process.StandardOutput.BaseStream, stdin_process.StandardInput.BaseStream);
            RePipe.Connect();
            instart_deviceInfoData.device_vmode = deviceInfoData.device_vmode; // 记录播放时的横竖屏状态（是否竖屏）
            is_casting = true;
            heartTimer.Enabled = true;
        }

        private void StopCast()
        {
            try
            {
                try
                {
                    // stdout_process.OutputDataReceived -= new DataReceivedEventHandler(StdOutProcessOutDataReceived);
                    stdout_process.Kill();
                }
                catch { }
                try
                {
                    // stdin_process.OutputDataReceived -= new DataReceivedEventHandler(StdInProcessOutDataReceived);
                    stdin_process.Kill();
                }
                catch
                { }
                RePipe.Disconnect();
            }
            catch { }
            is_casting = false;
        }

        public void SetPenetrate(IntPtr useHandle, bool flag = true)
        {
            uint style = GetWindowLong(useHandle, GWL_EXSTYLE);
            if (flag)
                SetWindowLong(useHandle, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            else
                SetWindowLong(useHandle, GWL_EXSTYLE, style & ~(WS_EX_TRANSPARENT | WS_EX_LAYERED));
            SetLayeredWindowAttributes(useHandle, 0, 100, LWA_ALPHA);
        }

        private void StdOut()
        {
            //stdout_process.OutputDataReceived -= new DataReceivedEventHandler(StdOutProcessOutDataReceived);
            // https://developer.android.com/studio/releases/platform-tools.html
            stdout_process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\adb\adb.exe";
            stdout_process.StartInfo.Arguments = $"exec-out \"while true;do screenrecord --bit-rate={(int)(castMbitRate * 1000000)} --output-format=h264 --size {deviceInfoData.device_width.ToString()}x{deviceInfoData.device_height.ToString()} - ;done\""; // 
            stdout_process.StartInfo.UseShellExecute = false;
            stdout_process.StartInfo.RedirectStandardOutput = true;
            stdout_process.StartInfo.CreateNoWindow = true;
            stdout_process.Start();
            if (stdin_process.StartInfo.FileName.Length != 0)
            {
                stdin_process.CancelOutputRead();
                stdin_process.Close();
            }
            //stdout_process.OutputDataReceived += new DataReceivedEventHandler(StdOutProcessOutDataReceived);
        }

        private void StdOutProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {

        }

        private void ADBSentKey(string keycode)
        {
            new Thread(delegate ()
            {
                Process process = new Process();
                process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\adb\adb.exe";
                process.StartInfo.Arguments = "shell input keyevent " + keycode;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.Start();
                Console.WriteLine(process.StandardOutput.ReadToEnd());
            }).Start();
        }

        private string ADBResult(string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\adb\adb.exe";
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.Start();
            //while (!process.StandardOutput.EndOfStream)
            //{
            //    Console.WriteLine(process.StandardOutput.ReadLine());
            //}
            //process.WaitForExit();
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadLine();
            process.Close();
            return error + result;
        }

        private void StdIn()
        {
            stdin_process.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\mpv\mpv.exe";
            stdin_process.StartInfo.Arguments = $"--hwdec=auto --opengl-glfinish=yes --opengl-swapinterval=0 --d3d11-sync-interval=0 --fps={deviceInfoData.device_refreshRate} --no-audio --framedrop=decoder --no-correct-pts --speed=2 --profile=low-latency --no-border --no-config --input-default-bindings=no --osd-level=0 -no-osc --wid={screenBox.Handle.ToInt64().ToString()} -";
            stdin_process.StartInfo.UseShellExecute = false;
            stdin_process.StartInfo.RedirectStandardOutput = true;
            stdin_process.StartInfo.RedirectStandardInput = true;
            stdin_process.StartInfo.CreateNoWindow = true;
            stdin_process.Start();
            stdin_process.BeginOutputReadLine();
            //stdin_process.OutputDataReceived += new DataReceivedEventHandler(StdInProcessOutDataReceived);
        }

        private void StdInProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    SetParent(stdin_process.MainWindowHandle, screenBox.Handle);
                    SetPenetrate(stdin_process.MainWindowHandle, true);
                    SetParent(stdin_process.MainWindowHandle, this.Handle);
                    // window, x, y, width, height, repaint
                    MoveWindow(stdin_process.MainWindowHandle, screenBox.Location.X, screenBox.Location.Y, screenBox.Width, screenBox.Height, false);
                }));
            }
            catch { }
        }

        private void StopCastButton_Click(object sender, EventArgs e)
        {
            StopCast();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopCast();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //MoveWindow(stdin_process.MainWindowHandle, screenBox.Location.X, screenBox.Location.Y, screenBox.Width, screenBox.Height, false);
        }

        private bool UpdateScreenDeviceInfo()
        {
            string str = ADBResult("shell \"dumpsys window displays && dumpsys SurfaceFlinger\"").ToLower();
            if (str.StartsWith("error: no devices/emulators found"))
                return false; //MessageBox.Show("找不到任何设备或模拟器", "警告");
            // Console.WriteLine(str);
            Regex regexSize = new Regex(@"\s+cur=(?<width>[0-9]*)x(?<height>[0-9]*?)\s+", RegexOptions.Multiline);
            Match matchSize = regexSize.Match(str);
            Regex regexRefreshRate = new Regex(@"\s+refresh-rate.+?(?<refreshRate>[0-9]*\.{0,1}[0-9]*?)\s*fps\s+", RegexOptions.Multiline);
            Match matchRefreshRate = regexRefreshRate.Match(str);
            if (matchSize.Success)
            {
                Console.WriteLine("Size成功");
                try
                {
                    int width = Int32.Parse(matchSize.Groups["width"].Value); //宽
                    int height = Int32.Parse(matchSize.Groups["height"].Value); //高
                    bool vmode = true; //垂直
                    if (width > height)
                        vmode = false; //水平
                    string strFormat = string.Format("{0}*{1},是否垂直:{2}", width, height, vmode.ToString());
                    Console.WriteLine(strFormat);
                    deviceInfoData.device_width = width;
                    deviceInfoData.device_height = height;
                    deviceInfoData.device_vmode = vmode;
                }
                catch { }
            }
            if (matchRefreshRate.Success)
            {
                try
                {
                    Console.WriteLine("RefreshRate成功");
                    int refreshRate = (int)double.Parse(matchRefreshRate.Groups["refreshRate"].Value);
                    string strFormat = string.Format("刷新率:{0}", refreshRate);
                    Console.WriteLine(strFormat);
                    deviceInfoData.device_refreshRate = refreshRate;
                }
                catch { }
            }
            return true;
        }

        private void HeartTimer_Tick(object sender, EventArgs e)
        {
            if (UpdateScreenDeviceInfo())
            {
                if (instart_deviceInfoData.device_vmode != deviceInfoData.device_vmode)
                    StartCastAction(); // 如果设备info切换则重新连接（为了转换分辨率）
            }
            else
            {
                heartTimer.Enabled = false;
                MessageBox.Show("设备已断开连接", "警告");
                StopCast();
            }
        }

        private void PowerKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_POWER");
        }

        private void BackKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_BACK");
        }

        private void HomeKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_HOME");
        }

        private void MutiKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_APP_SWITCH");
        }

        private void MenuKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_MENU");
        }

        private void VolUpKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_VOLUME_UP");
        }

        private void VolDownKeyButton_Click(object sender, EventArgs e)
        {
            ADBSentKey("KEYCODE_VOLUME_DOWN");
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            UpdateScreenDeviceInfo();
        }
    }
}
