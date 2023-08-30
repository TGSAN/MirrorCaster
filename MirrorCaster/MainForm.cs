using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

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

        private Process stdoutProcess = null;
        private Process stdinProcess = null;
        private StreamPipe rePipe;
        private CastType castType = CastType.Internal;
        private bool isCastingValue = false;
        private bool isEnableVSync = false;
        private bool isEnableHWDec = true;
        private UserMPVArg profileArgs;

        private OrderedDictionary profileList = new OrderedDictionary() {
            {
                "超低延迟模式",
                new UserMPVArg("", false)
            },
            {
                "均衡（偏向低延迟）",
                new UserMPVArg("--speed=10", true)
            },
            {
                "均衡（偏向稳定）",
                new UserMPVArg( "--speed=1.01", true)
            },
            {
                "稳定模式",
                new UserMPVArg("", true)
            },
        };

        private bool isCasting
        {
            get => isCastingValue;
            set
            {
                isCastingValue = value;

                stopCastButton.Enabled = value;
                powerKeyButton.Enabled = value;
                backKeyButton.Enabled = value;
                homeKeyButton.Enabled = value;
                mutiKeyButton.Enabled = value;
                menuKeyButton.Enabled = value;
                volUpKeyButton.Enabled = value;
                volDownKeyButton.Enabled = value;

                profileComboBox.Enabled = !value;
                hwdecEnableCheckBox.Enabled = !value;
                vsyncEnableCheckBox.Enabled = !value;
            }
        }

        private enum CastType
        {
            Internal,
            Single
        }

        private readonly DeviceInfoData deviceInfoData = new DeviceInfoData(); // device info form adb
        private readonly DeviceInfoData instartDeviceInfoData = new DeviceInfoData(); // device info at start cast

        private double castMbitRate = 30; // 16M适中

        public static NamedPipeClientStream client;

        public MainForm()
        {
            InitializeComponent();

            profileComboBox.Items.Clear();
            foreach (string item in profileList.Keys)
            {
                profileComboBox.Items.Add(item);
            }
            profileComboBox.SelectedIndex = 0;
            profileArgs = profileList[profileComboBox.Text] as UserMPVArg;
#if DEBUG
            testButton.Visible = true;
#endif
        }

        private void startCastSingleButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("关于独立窗口模式\n\n可便于OBS等直播软件抓取，因为弹出外部播放器窗口相比内建可控性更差，画面旋转及分辨率自适应已被禁用。\n\n建议先将手机处于应用横屏状态再点击“确定”按钮开始投屏。\n\n若使用OBS进行抓取，请使用：\n游戏捕获->模式：捕获特定窗口->窗口：[mpv.exe] Mirror Caster Source", "提示");
            castType = CastType.Single;
            ShowConfigDialog();
        }

        private void StartCastButton_Click(object sender, EventArgs e)
        {
            castType = CastType.Internal;
            ShowConfigDialog();
        }

        private void ShowConfigDialog()
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
                {
                    MessageBox.Show("请输入正确的码率（Mbps）", "警告");
                }
            }
        }

        private void StartCastAction()
        {
            StopCast();
            if (UpdateScreenDeviceInfo())
            {
                StartCast();
            }
        }

        private void StartCast()
        {
            stdoutProcess = new Process();
            stdinProcess = new Process();
            StdOut();
            StdIn();
            rePipe = new StreamPipe(stdoutProcess.StandardOutput.BaseStream, stdinProcess.StandardInput.BaseStream);
            rePipe.Connect();
            instartDeviceInfoData.deviceVmode = deviceInfoData.deviceVmode; // 记录播放时的横竖屏状态（是否竖屏）
            isCasting = true;
            heartTimer.Enabled = true;
            if (castType == CastType.Single) nosigalLabel.Text = "投屏中 (｀・ω・´)";
        }

        private void StopCast()
        {
            try
            {
                try
                {
                    if (stdoutProcess != null)
                    {
                        stdoutProcess.Exited -= StdIOProcess_Exited;
                        // stdoutProcess.OutputDataReceived -= new DataReceivedEventHandler(StdOutProcessOutDataReceived);
                        stdoutProcess.Kill();
                        stdoutProcess = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("无法关闭StdOUT，" + ex.Message);
                }
                try
                {
                    if (stdinProcess != null) 
                    {
                        stdinProcess.Exited -= StdIOProcess_Exited;
                        // stdinProcess.OutputDataReceived -= new DataReceivedEventHandler(StdInProcessOutDataReceived);
                        stdinProcess.Kill();
                        stdoutProcess = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("无法关闭StdIN，" + ex.Message);
                }
                if (rePipe != null) { 
                    rePipe.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("无法断开管道重定向，" + ex.Message);
            }
            heartTimer.Enabled = false;
            isCasting = false;
            nosigalLabel.Text = "无信号_(:3」∠)_";
        }

        public void SetPenetrate(IntPtr useHandle, bool flag = true)
        {
            uint style = GetWindowLong(useHandle, GWL_EXSTYLE);
            if (flag)
            {
                SetWindowLong(useHandle, GWL_EXSTYLE, style | WS_EX_TRANSPARENT | WS_EX_LAYERED);
            }
            else
            {
                SetWindowLong(useHandle, GWL_EXSTYLE, style & ~(WS_EX_TRANSPARENT | WS_EX_LAYERED));
            }

            SetLayeredWindowAttributes(useHandle, 0, 100, LWA_ALPHA);
        }

        private void StdOut()
        {
            //stdoutProcess.OutputDataReceived -= new DataReceivedEventHandler(StdOutProcessOutDataReceived);
            // https://developer.android.com/studio/releases/platform-tools.html
            stdoutProcess.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\adb\adb.exe";
            stdoutProcess.StartInfo.Arguments = $"exec-out \"while true;do screenrecord --bit-rate={(int)(castMbitRate * 1000000)} --output-format=h264 --size {deviceInfoData.deviceWidth.ToString()}x{deviceInfoData.deviceHeight.ToString()} - ;done\""; // 
            stdoutProcess.StartInfo.UseShellExecute = false;
            stdoutProcess.StartInfo.RedirectStandardOutput = true;
            stdoutProcess.StartInfo.CreateNoWindow = true;
            stdoutProcess.EnableRaisingEvents = true;
            stdoutProcess.Exited += StdIOProcess_Exited;
            stdoutProcess.Start();
            if (stdinProcess.StartInfo.FileName.Length != 0)
            {
                stdinProcess.CancelOutputRead();
                stdinProcess.Close();
            }
            //stdoutProcess.OutputDataReceived += new DataReceivedEventHandler(StdOutProcessOutDataReceived);
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
            string widArg;
            switch (castType)
            {
                case CastType.Internal:
                    widArg = $"--wid={screenBox.Handle.ToInt64().ToString()}";
                    break;
                case CastType.Single:
                    widArg = default;
                    break;
                default:
                    widArg = default;
                    break;
            }
            string vsyncArgs = "--d3d11-sync-interval=" + (isEnableVSync ? "1" : "0");
            string releaseArgs = "--input-default-bindings=no --osd-level=0";
            string fpsControlArgs = profileArgs.useDeviceFPS ? $"--no-correct-pts --fps={deviceInfoData.deviceRefreshRate}" : "--untimed";
            string hwdecArgs = isEnableHWDec ? "--hwdec=yes" : "--hwdec=no";
#if DEBUG
            releaseArgs = default;
#endif
            string mpvFullArgs = $"--title=\"Mirror Caster Source\" --d3d11-output-csp=srgb --cache=no --no-cache --profile=low-latency --framedrop=decoder { vsyncArgs } --scale=spline36 --cscale=spline36 --dscale=mitchell --correct-downscaling=yes --linear-downscaling=yes --sigmoid-upscaling=yes { fpsControlArgs } --video-latency-hacks=yes { profileArgs.argsStr } --vo=gpu { hwdecArgs } --no-audio --no-config --no-border -no-osc --no-taskbar-progress { releaseArgs } { widArg } -";
            Console.WriteLine("MPV ARGS:\r\n" + mpvFullArgs);
            stdinProcess.StartInfo.FileName = System.AppDomain.CurrentDomain.BaseDirectory + @"lib\mpv\mpv.exe";
            stdinProcess.StartInfo.Arguments = mpvFullArgs;
            //stdinProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            stdinProcess.StartInfo.UseShellExecute = false;
            stdinProcess.StartInfo.RedirectStandardOutput = true;
            stdinProcess.StartInfo.RedirectStandardInput = true;
            stdinProcess.StartInfo.CreateNoWindow = true;
            stdinProcess.EnableRaisingEvents = true;
            stdinProcess.Exited += StdIOProcess_Exited;
            stdinProcess.Start();
            stdinProcess.BeginOutputReadLine();
            //stdinProcess.OutputDataReceived += new DataReceivedEventHandler(StdInProcessOutDataReceived);
        }

        private void StdIOProcess_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("StdIO管道被关闭，关闭投屏");
            Invoke(new Action(StopCast)); // 结束投屏需要修改UI，所以Invoke
        }

        private void StdInProcessOutDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    SetParent(stdinProcess.MainWindowHandle, screenBox.Handle);
                    SetPenetrate(stdinProcess.MainWindowHandle, true);
                    SetParent(stdinProcess.MainWindowHandle, Handle);
                    // window, x, y, width, height, repaint
                    MoveWindow(stdinProcess.MainWindowHandle, screenBox.Location.X, screenBox.Location.Y, screenBox.Width, screenBox.Height, false);
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
            //MoveWindow(stdinProcess.MainWindowHandle, screenBox.Location.X, screenBox.Location.Y, screenBox.Width, screenBox.Height, false);
        }

        private bool UpdateScreenDeviceInfo()
        {
            string str = ADBResult("shell \"dumpsys window displays && dumpsys SurfaceFlinger\"").ToLower();
            if (str.StartsWith("error: no devices/emulators found"))
            {
                MessageBox.Show("找不到任何设备或模拟器", "警告");
                return false;
            }
            else if (str.StartsWith("error: more than one device/emulator"))
            {
                MessageBox.Show("暂时只支持单个设备开启 ADB 调试，请关闭其它设备的调试或移除连接。", "警告");
                return false;
            }
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
                    int width = int.Parse(matchSize.Groups["width"].Value); //宽
                    int height = int.Parse(matchSize.Groups["height"].Value); //高
                    bool vmode = true; //垂直
                    if (width > height)
                    {
                        vmode = false; //水平
                    }

                    string strFormat = string.Format("{0}*{1},是否垂直:{2}", width, height, vmode.ToString());
                    Console.WriteLine(strFormat);
                    deviceInfoData.deviceWidth = width;
                    deviceInfoData.deviceHeight = height;
                    deviceInfoData.deviceVmode = vmode;
                }
                catch { }
            }
            if (matchRefreshRate.Success)
            {
                try
                {
                    Console.WriteLine("RefreshRate成功");
                    double refreshRate = double.Parse(matchRefreshRate.Groups["refreshRate"].Value);
                    string strFormat = string.Format("刷新率:{0}", refreshRate);
                    Console.WriteLine(strFormat);
                    deviceInfoData.deviceRefreshRate = refreshRate;
                }
                catch { }
            }
            return true;
        }

        private void HeartTimer_Tick(object sender, EventArgs e)
        {
            if (UpdateScreenDeviceInfo())
            {
                if (instartDeviceInfoData.deviceVmode != deviceInfoData.deviceVmode)
                {
                    if (castType == CastType.Internal) StartCastAction(); // 如果设备info切换则重新连接（为了转换分辨率）
                }
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

        private void vsyncEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            isEnableVSync = (sender as CheckBox).Checked;
        }

        private void hwdecEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            isEnableHWDec = (sender as CheckBox).Checked;
        }

        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            profileArgs = profileList[(sender as ComboBox).Text] as UserMPVArg;
        }
    }

    public class UserMPVArg
    {
        public string argsStr;
        public bool useDeviceFPS;

        public UserMPVArg(string argsStr, bool useDeviceFPS)
        {
            this.argsStr = argsStr;
            this.useDeviceFPS = useDeviceFPS;
        }
    }
}
