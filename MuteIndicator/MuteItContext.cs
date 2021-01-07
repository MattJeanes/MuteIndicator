using NAudio.CoreAudioApi;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
        private readonly HotkeyManager _manager;
        internal static SettingsForm _settings;
        private bool _init;
        private bool _muted;

        public MuteItContext()
        {
            _tbIcon = CreateIcon();
            UpdateMicStatus();
            _refreshTimer = StartTimer();
            _manager = RegisterHotkeys();
        }

        private HotkeyManager RegisterHotkeys()
        {
            var manager = new HotkeyManager(this);

            RegisterHotKey(manager.Handle, MUTE_TOGGLE_CODE, Constants.CTRL, (int)Keys.OemPipe);

            return manager;
        }

        private NotifyIcon CreateIcon()
        {

            var icon = new NotifyIcon();
            icon.Icon = Properties.Resources.mic_off;
            icon.ContextMenuStrip = new ContextMenuStrip();
            icon.DoubleClick += (source, e) => ToggleMicStatus();
            icon.Visible = true;

            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += new EventHandler(HandleExit);
            icon.ContextMenuStrip.Items.Add(exitMenuItem);

            var settingsMenuItem = new ToolStripMenuItem("Settings");
            settingsMenuItem.Click += new EventHandler(HandleSettings);
            icon.ContextMenuStrip.Items.Add(settingsMenuItem);

            return icon;
        }

        private System.Timers.Timer StartTimer()
        {
            var timer = new System.Timers.Timer(REFRESH_INTERVAL);
            timer.Elapsed += (source, e) => UpdateMicStatus();
            timer.Start();
            return timer;
        }

        public void ToggleMicStatus()
        {
            var device = GetPrimaryMicDevice();

            if (device != null)
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                UpdateMicStatusAsync(device);
            }
            else
            {
                UpdateMicStatusAsync(null);
            }
        }

        private void UpdateMicStatus()
        {
            var device = GetPrimaryMicDevice();
            UpdateMicStatusAsync(device);
        }

        private async void UpdateMicStatusAsync(MMDevice device)
        {
            bool muted;
            if (device == null || device.AudioEndpointVolume.Mute == true)
            {
                muted = true;
            }
            else
            {
                muted = false;
            }

            var hueKey = Properties.Settings.Default.HueKey;
            var hueLights = Properties.Settings.Default.HueLights?.Cast<string>().ToList() ?? new List<string>();
            var hueLinked = !string.IsNullOrEmpty(hueKey) && hueLights.Count > 0;
            var changed = (muted != _muted) || !_init;
            ILocalHueClient hueClient = null;
            if (hueLinked)
            {
                hueClient = new LocalHueClient(Properties.Settings.Default.HueIPAddress, Properties.Settings.Default.HueKey);
            }

            try
            {
                _tbIcon.Icon = muted ? Properties.Resources.mic_off : Properties.Resources.mic_on;
                if (hueLinked && changed)
                {
                    var command = new LightCommand();
                    var color = muted ? new RGBColor(255, 0, 0) : new RGBColor(255, 255, 255);
                    command.SetColor(color);
                    await hueClient.SendCommandAsync(command, hueLights);
                }
            }
            catch (Exception e)
            {
                if (changed)
                {
                    MessageBox.Show($"Failed to update mic status: {e.Message}");
                }
            }


            DisposeDevice(device);

            if (!_init)
            {
                _init = true;
            }
            _muted = muted;
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

        private void HandleSettings(object sender, EventArgs e)
        {
            if (_settings == null || _settings.IsDisposed)
            {
                _settings = new SettingsForm();
            }
            _settings.Show();
            _settings.BringToFront();
        }
    }
}
