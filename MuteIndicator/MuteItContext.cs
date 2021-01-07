using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace MuteIndicator
{
    class MuteItContext : ApplicationContext
    {
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int MUTE_TOGGLE_CODE = 123;
        private const double REFRESH_INTERVAL = 1000;

        private readonly NotifyIcon _tbIcon;
        private readonly System.Timers.Timer _refreshTimer;

        public MuteItContext()
        {
            _tbIcon = CreateIcon();
            UpdateMicStatus();
            _refreshTimer = StartTimer();
            RegisterHotkeys();
        }

        private HotkeyManager RegisterHotkeys()
        {
            var manager = new HotkeyManager(this);

            RegisterHotKey(manager.Handle, MUTE_TOGGLE_CODE, Constants.ALT, (int)Keys.OemBackslash);

            return manager;
        }

        private NotifyIcon CreateIcon()
        {
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += new EventHandler(HandleExit);

            var icon = new NotifyIcon();
            icon.Icon = Properties.Resources.mic_off;
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.ContextMenuStrip.Items.Add(exitMenuItem);
            icon.DoubleClick += (source, e) => ToggleMicStatus();

            icon.Visible = true;

            return icon;
        }

        private System.Timers.Timer StartTimer()
        {
            var timer = new System.Timers.Timer(REFRESH_INTERVAL);
            timer.Elapsed += (source, e) => UpdateMicStatus();
            timer.Start();
            return timer;
        }

        public void SetMicMuteStatus(bool doMute)
        {
            var device = GetPrimaryMicDevice();

            if (device != null)
            {
                device.AudioEndpointVolume.Mute = doMute;
                UpdateMicStatus(device);
            }
            else
            {
                UpdateMicStatus(null);
            }
        }

        public void MuteMic()
        {
            SetMicMuteStatus(true);
        }

        public void UnmuteMic()
        {
            SetMicMuteStatus(false);
        }

        public void ToggleMicStatus()
        {
            var device = GetPrimaryMicDevice();

            if (device != null)
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                UpdateMicStatus(device);
            }
            else
            {
                UpdateMicStatus(null);
            }
        }

        private void UpdateMicStatus()
        {
            var device = GetPrimaryMicDevice();
            UpdateMicStatus(device);
            //System.GC.Collect();
        }

        private void UpdateMicStatus(MMDevice device)
        {
            if (device == null || device.AudioEndpointVolume.Mute == true)
                _tbIcon.Icon = Properties.Resources.mic_off;
            else
                _tbIcon.Icon = Properties.Resources.mic_on;

            DisposeDevice(device);
        }

        private MMDevice GetPrimaryMicDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            var result = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
            enumerator.Dispose();

            _tbIcon.Text = result.DeviceFriendlyName;

            return result;
        }

        private void DisposeDevice(MMDevice device)
        {
            if (device != null)
            {
                device.AudioEndpointVolume.Dispose();
                device.Dispose();
            }
        }

        private void HandleExit(object sender, EventArgs e)
        {
            _tbIcon.Visible = false;
            _refreshTimer.Stop();
            Application.Exit();
        }
    }
}
