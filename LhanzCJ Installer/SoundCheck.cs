using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;

namespace LhanzCJ_Installer
{
    public partial class SoundCheck : Form
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string selectedFilePath;
        private List<AudioFile> audioFiles = new List<AudioFile>();
        private Timer progressTimer;

        public SoundCheck()
        {
            InitializeComponent();
            outputDevice = new WaveOutEvent();
            outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
            progressTimer = new Timer { Interval = 500 };
            progressTimer.Tick += ProgressTimer_Tick;
            this.Load += SoundCheck_Load;
        }

        private class AudioFile
        {
            public string DisplayName { get; set; }
            public string FullPath { get; set; }
            public TimeSpan Duration { get; set; }
        }


        private void SoundCheck_Load(object sender, EventArgs e)
        {
            string assetsPath = Path.Combine(Application.StartupPath, "assets");

            if (!Directory.Exists(assetsPath))
            {
                try
                {
                    Directory.CreateDirectory(assetsPath);
                    lblStatus.Text = "Assets folder created! Add audio files to: " + assetsPath;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error creating assets folder: " + ex.Message;
                    return;
                }
            }

            LoadAudioFiles(assetsPath);
        }
        private void LoadAudioFiles(string folderPath)
        {
            try
            {
                audioFiles.Clear();
                var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    .Select(f => {
                        try
                        {
                            using (var reader = new AudioFileReader(f))
                            {
                                return new AudioFile
                                {
                                    DisplayName = Path.GetFileNameWithoutExtension(f),
                                    FullPath = f,
                                    Duration = reader.TotalTime
                                };
                            }
                        }
                        catch
                        {
                            return new AudioFile
                            {
                                DisplayName = Path.GetFileNameWithoutExtension(f),
                                FullPath = f,
                                Duration = TimeSpan.Zero
                            };
                        }
                    })
                    .ToList();

                audioFiles = files;
                lstFiles.DataSource = null;
                lstFiles.DisplayMember = "DisplayName";
                lstFiles.ValueMember = "FullPath";
                lstFiles.DataSource = audioFiles;

                lblStatus.Text = $"Loaded {files.Count} files from: {Path.GetFileName(folderPath)}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error loading files: {ex.Message}";
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadAudioFiles(folderDialog.SelectedPath);
                }
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (audioFile != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                var currentTime = audioFile.CurrentTime;
                var totalTime = audioFile.TotalTime;

                progressBar.Value = (int)Math.Min(100, (currentTime.TotalMilliseconds / totalTime.TotalMilliseconds) * 100);

                lblCurrentTime.Text = FormatTime(currentTime);
                lblTotalTime.Text = FormatTime(totalTime);
            }
        }
        private string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OutputDevice_PlaybackStopped(sender, e)));
                return;
            }

            // Only update if it's the current device
            if (sender as WaveOutEvent == outputDevice)
            {
                progressBar.Value = 100;
                lblCurrentTime.Text = FormatTime(audioFile?.TotalTime ?? TimeSpan.Zero);
                progressTimer.Stop();
            }
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItem != null)
            {
                var selectedFile = (AudioFile)lstFiles.SelectedItem;
                selectedFilePath = selectedFile.FullPath;
                lblStatus.Text = $"Selected: {selectedFile.DisplayName}";

                PlaySelectedFile();
            }
        }

        private void PlaySelectedFile()
        {
            StopPlayback();

            if (string.IsNullOrEmpty(selectedFilePath)) return;

            try
            {
                // Create new instances
                outputDevice = new WaveOutEvent();
                audioFile = new AudioFileReader(selectedFilePath);

                // Reset UI before starting
                progressBar.Value = 0;
                lblCurrentTime.Text = "00:00";
                lblTotalTime.Text = FormatTime(audioFile.TotalTime);

                // Wire up events
                outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
                outputDevice.Init(audioFile);
                outputDevice.Play();

                // Start fresh progress updates
                progressTimer.Stop();
                progressTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing file: {ex.Message}", "Playback Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void StopPlayback()
        {
            if (outputDevice != null)
            {
                // Unsubscribe first to prevent lingering events
                outputDevice.PlaybackStopped -= OutputDevice_PlaybackStopped;
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            if (audioFile != null)
            {
                audioFile.Dispose();
                audioFile = null;
            }

            progressTimer.Stop();
            progressBar.Value = 0;
            lblCurrentTime.Text = "00:00";
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (outputDevice == null) return;

            try
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    outputDevice.Pause();
                    progressTimer.Stop();
                }
                else
                {
                    outputDevice.Play();
                    progressTimer.Start();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error playing audio file: {ex.Message}";
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                progressTimer.Stop();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPlayback();
            lblStatus.Text = "Playback stopped";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
            base.OnFormClosing(e);
        }
    }
}