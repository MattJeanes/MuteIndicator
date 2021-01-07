using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MuteIndicator
{
    public partial class SettingsForm : Form
    {
        private bool _updating;

        public SettingsForm()
        {
            InitializeComponent();
            var hueLinked = !string.IsNullOrEmpty(Properties.Settings.Default.HueKey);
            linkHueButton.Enabled = !hueLinked;
            unlinkHueButton.Enabled = hueLinked;
            statusLabel.Text = $"Status: Hue bridge {(hueLinked ? "linked" : "not linked")}";
            Load += new EventHandler(Settings_Load);
            lightsCheckedListBox.DisplayMember = nameof(LightCheckBoxItem.Text);
        }

        private async void Settings_Load(object sender, System.EventArgs e)
        {
            await UpdateLights();
        }

        private async void linkHueButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                var locator = new HttpBridgeLocator();
                statusLabel.Text = "Status: Locating Hue bridge";
                var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
                var bridge = bridges.FirstOrDefault();
                if (bridge == null)
                {
                    statusLabel.Text = "Status: Unable to find Hue bridge";
                    return;
                }
                statusLabel.Text = "Status: Bridge found, please press button";
                var client = new LocalHueClient(bridge.IpAddress);
                var totalWait = 30;
                for (var i = 0; i < totalWait; i++)
                {
                    await Task.Delay(1000);
                    try
                    {
                        Properties.Settings.Default.HueKey = await client.RegisterAsync("MuteIndicator", "MuteIndicator");
                        Properties.Settings.Default.HueIPAddress = bridge.IpAddress;
                        break;
                    }
                    catch (LinkButtonNotPressedException)
                    {
                        linkProgressBar.Value = (int)((double)i / totalWait * 100);
                    }
                }
                linkProgressBar.Value = 100;
                var hueKey = Properties.Settings.Default.HueKey;
                if (string.IsNullOrEmpty(hueKey))
                {
                    statusLabel.Text = "Failed: Link button was not pressed";
                    return;
                }
                Properties.Settings.Default.Save();
                statusLabel.Text = "Status: Hue bridge successfully linked";
                linkHueButton.Enabled = false;
                unlinkHueButton.Enabled = true;
                await UpdateLights();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Failed: {ex.Message}";
            }
        }

        private async Task UpdateLights()
        {
            var hueLinked = !string.IsNullOrEmpty(Properties.Settings.Default.HueKey);
            if (!hueLinked)
            {
                lightsCheckedListBox.Items.Clear();
                return;
            }
            var hueKey = Properties.Settings.Default.HueKey;
            var hueIPAddress = Properties.Settings.Default.HueIPAddress;
            var client = new LocalHueClient(hueIPAddress, hueKey);
            var lights = await client.GetLightsAsync();
            foreach (var light in lights)
            {
                lightsCheckedListBox.Items.Insert(0, new LightCheckBoxItem { Text = light.Name, Value = light.Id });
            }
            var savedLights = Properties.Settings.Default.HueLights?.Cast<string>().ToList() ?? new List<string>();
            _updating = true;
            var checkIndexes = new List<int>();
            foreach (var item in lightsCheckedListBox.Items)
            {
                if (savedLights.Contains(((LightCheckBoxItem)item).Value))
                {
                    checkIndexes.Add(lightsCheckedListBox.Items.IndexOf(item));
                }
            }
            foreach (var index in checkIndexes)
            {
                lightsCheckedListBox.SetItemChecked(index, true);
            }
            _updating = false;
        }

        private async void unlinkHueButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.HueKey = null;
            Properties.Settings.Default.HueIPAddress = null;
            Properties.Settings.Default.HueLights = null;
            Properties.Settings.Default.Save();
            statusLabel.Text = "Status: Hue bridge unlinked";
            linkHueButton.Enabled = true;
            unlinkHueButton.Enabled = false;
            await UpdateLights();
        }

        private void lightsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_updating) { return; }
            BeginInvoke((MethodInvoker)(() =>
            {
                Properties.Settings.Default.HueLights = new StringCollection();
                foreach (var light in lightsCheckedListBox.CheckedItems)
                {
                    Properties.Settings.Default.HueLights.Add(((LightCheckBoxItem)light).Value);
                }
                Properties.Settings.Default.Save();
            }));
        }

        private class LightCheckBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}
