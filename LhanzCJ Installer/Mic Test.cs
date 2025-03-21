﻿using CSCore.CoreAudioAPI;
using CSCore.Win32;
using CSCore;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;

namespace LhanzCJ_Installer
{
    public partial class MicTest : Form
    {
        private CaptureMode captureMode = CaptureMode.Capture;
        private readonly GraphVisualization _graphVisualization = new GraphVisualization();
        private float l = 0;
        private float r = 0;
        private MMDevice _selectedDevice;
        private WasapiCapture _soundIn;
        private IWaveSource _finalSource;
        private ISoundOut _soundOut;
        public MicTest()
        {
            InitializeComponent();
            RefreshDevices();
        }

        public MMDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                if (value != null)
                    buttonCheck.Enabled = true;
            }
        }
        private void RefreshDevices()
        {
            listViewSources.Items.Clear();

            using (var deviceEnumerator = new MMDeviceEnumerator())
            using (var deviceCollection = deviceEnumerator.EnumAudioEndpoints(
                captureMode == CaptureMode.Capture ? DataFlow.Capture : DataFlow.Render, DeviceState.Active))
            {
                foreach (var device in deviceCollection)
                {
                    if (captureMode != CaptureMode.LoopbackCapture || device.DeviceFormat.Channels > 1)
                    {
                        var deviceFormat = WaveFormatFromBlob(device.PropertyStore[
            new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);

                        var item = new ListViewItem(device.FriendlyName) { Tag = device };
                        item.SubItems.Add(deviceFormat.Channels.ToString(CultureInfo.InvariantCulture));

                        listViewSources.Items.Add(item);
                    }

                }
            }
        }
        private static WaveFormat WaveFormatFromBlob(Blob blob)
        {
            if (blob.Length == 40)
                return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormatExtensible));
            return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormat));
        }
        private void buttonRefreshMicro0phone_Click(object sender, EventArgs e)
        {
            RefreshDevices();
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (listViewSources.SelectedItems.Count > 0)
            {
                if (_soundIn == null)
                {
                    if (SelectedDevice == null)
                        return;

                    buttonCheck.Enabled = false;
                    if (captureMode == CaptureMode.Capture)
                        _soundIn = new WasapiCapture();
                    else
                        _soundIn = new WasapiLoopbackCapture(100, new WaveFormat(48000, 24, 2));

                    _soundIn.Device = SelectedDevice;
                    _soundIn.Initialize();

                    var soundInSource = new SoundInSource(_soundIn)
                    { FillWithZeros = SelectedDevice.DeviceFormat.Channels <= 1 };

                    var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToStereo().ToSampleSource());
                    _finalSource = singleBlockNotificationStream.ToWaveSource();

                    singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStreamOnSingleBlockRead;

                    _soundIn.Start();
                    if (captureMode == CaptureMode.Capture)
                    {
                        if (SelectedDevice.DeviceFormat.Channels <= 1)
                        {
                            _soundOut = new WasapiOut()
                            {
                                Device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications)
                            };
                        }
                        else
                        {
                            _soundOut = new WasapiOut();
                        }
                        _soundOut.Initialize(_finalSource);
                        _soundOut.Play();
                    }
                    else
                    {
                        byte[] buffer = new byte[_finalSource.WaveFormat.BytesPerSecond / 2];
                        soundInSource.DataAvailable += (s, ex) =>
                        {
                            int read;
                            while ((read = _finalSource.Read(buffer, 0, buffer.Length)) > 0)
                            {

                            }
                        };
                    }
                    buttonCheck.Enabled = true;
                    buttonRefreshMicro0phone.Enabled = false;
                    listViewSources.Enabled = false;
                    checkBoxLoopback.Enabled = false;
                    buttonCheck.Text = "Stop";
                }
                else
                {
                    buttonCheck.Enabled = false;
                    if (_soundOut != null)
                    {
                        _soundOut.Stop();
                        _soundOut.Dispose();
                        _soundOut = null;
                    }
                    if (_soundIn != null)
                    {
                        _soundIn.Stop();
                        _soundIn.Dispose();
                        _soundIn = null;
                    }
                    buttonCheck.Enabled = true;
                    listViewSources.Enabled = true;
                    buttonRefreshMicro0phone.Enabled = true;
                    checkBoxLoopback.Enabled = true;
                    buttonCheck.Text = "Start";
                }
            }
            else
            {
                MessageBox.Show("Reload & Select a Device");
            }
        }
        private void SingleBlockNotificationStreamOnSingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            l = e.Left;
            r = e.Right;
            _graphVisualization.AddSamples(e.Left, e.Right);
        }

        private void listViewSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewSources.SelectedItems.Count > 0)
            {
                SelectedDevice = (MMDevice)listViewSources.SelectedItems[0].Tag;
                buttonCheck.Enabled = true;
            }
            else
            {
                SelectedDevice = null;
            }
        }

        private void timerGraph_Tick(object sender, EventArgs e)
        {
            if (_soundIn == null)
            {
                progressBarL.Value = 0;
                progressBarR.Value = 0;
            }
            progressBarL.Value = Math.Abs(Convert.ToInt32(l * 100));
            progressBarR.Value = Math.Abs(Convert.ToInt32(r * 100));
            var image = pictureBoxGraphVisualizer.Image;
            pictureBoxGraphVisualizer.Image = _graphVisualization.Draw(pictureBoxGraphVisualizer.Width, pictureBoxGraphVisualizer.Height);
            image?.Dispose();
        }

        private void MicTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_soundIn != null)
            {
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
            }
        }

        private void checkBoxLoopback_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLoopback.Checked)
            {
                captureMode = CaptureMode.LoopbackCapture;
                RefreshDevices();
                TopMost = true;
            }
            else
            {
                captureMode = CaptureMode.Capture;
                RefreshDevices();
                TopMost = false;
            }
        }

    }

    public enum CaptureMode
    {
        Capture,
        LoopbackCapture
    }
}
