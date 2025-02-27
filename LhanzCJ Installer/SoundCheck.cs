using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class SoundCheck : Form
    {
        private Player player;
        private List<string> audioFiles = new List<string>
        {
            @"audio\2left.m4a",
            @"audio\2right.m4a",
            @"audio\3left.m4a",
            @"audio\3center.m4a",
            @"audio\3right.m4a",
            @"audio\5left.m4a",
            @"audio\5center.m4a",
            @"audio\5right.m4a",
            @"audio\5backright.m4a",
            @"audio\5backleft.m4a"
        };

        public SoundCheck()
        {
            InitializeComponent();
            player = new Player(audioFiles);
            alternatingRadio.Checked = true;
            channels2Radio.Checked = true;
        }
        private void playStopButton_Click(object sender, EventArgs e)
        {
            if (player.IsPlaying)
            {
                player.Stop();
                playStopButton.Text = "Play";
            }
            else
            {
                string mode = alternatingRadio.Checked ? "Alternating" : "Simultaneous";
                string channels = GetSelectedChannels();

                player.SetMode(mode);
                player.SetChannels(channels);
                player.Play();
                playStopButton.Text = "Stop";
            }
        }

        private string GetSelectedChannels()
        {
            if (channels2Radio.Checked) return "2";
            if (channels3Radio.Checked) return "3";
            if (channels5Radio.Checked) return "5";
            return "7";
        }
    }

    public class Player
    {
        private List<string> audioFiles;
        private int currentIndex;
        private bool isPlaying;
        private string mode;
        private string channels;
        private Dictionary<string, int[]> channelMap;
        private List<WaveOutEvent> activePlayers;

        public bool IsPlaying => isPlaying;

        public Player(List<string> audios)
        {
            audioFiles = audios;
            channelMap = new Dictionary<string, int[]>
            {
                { "2", new[] { 0, 1 } },
                { "3", new[] { 2, 4 } },
                { "5", new[] { 5, 9 } },
                { "7", new[] { 10, 16 } }
            };
            activePlayers = new List<WaveOutEvent>();
            isPlaying = false;
            mode = "Alternating";
            channels = "2";
            currentIndex = channelMap[channels][0];
        }

        public void SetMode(string newMode) => mode = newMode;

        public void SetChannels(string newChannels)
        {
            channels = newChannels;
            currentIndex = channelMap[channels][0];
        }

        public void Play()
        {
            if (isPlaying) return;

            isPlaying = true;

            if (mode == "Simultaneous")
            {
                var start = channelMap[channels][0];
                var end = channelMap[channels][1];
                for (var i = start; i <= end; i++)
                    PlayFile(i, true);
            }
            else
            {
                PlayFile(currentIndex, false);
            }
        }

        private void PlayFile(int index, bool loop)
        {
            var waveOut = new WaveOutEvent();
            try
            {
                var audioFile = new MediaFoundationReader(audioFiles[index]);
                waveOut.Init(audioFile);
                waveOut.PlaybackStopped += (sender, args) =>
                {
                    waveOut.Dispose();
                    activePlayers.Remove(waveOut);

                    if (!isPlaying) return;

                    if (mode == "Simultaneous" && loop)
                        PlayFile(index, true);
                    else if (mode == "Alternating")
                    {
                        IncrementCounter();
                        PlayFile(currentIndex, false);
                    }
                };

                waveOut.Play();
                activePlayers.Add(waveOut);
            }
            catch
            {
                waveOut.Dispose();
            }
        }

        public void Stop()
        {
            if (!isPlaying) return;

            isPlaying = false;
            foreach (var player in activePlayers)
            {
                player.Stop();
                player.Dispose();
            }
            activePlayers.Clear();
            currentIndex = channelMap[channels][0];
        }

        private void IncrementCounter()
        {
            var start = channelMap[channels][0];
            var end = channelMap[channels][1];
            var delta = end - start;
            currentIndex = ((currentIndex - start + 1) % (delta + 1)) + start;
        }
    }
}
