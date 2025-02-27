using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;
using Size = OpenCvSharp.Size;

namespace LhanzCJ_Installer
{
    public partial class CamTest : Form
    {
        private VideoCapture videoCapture;
        private Mat frame;

        public CamTest()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                videoCapture = new VideoCapture(0);
                if (!videoCapture.IsOpened())
                {
                    MessageBox.Show("Failed to open webcam!");
                    return;
                }

                btnStart.Enabled = false;
                btnStop.Enabled = true;
                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopCamera();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (videoCapture == null || videoCapture.IsDisposed) return;

                using (var frame = new Mat())
                {
                    if (!videoCapture.Read(frame)) return;

                    if (!frame.Empty())
                    {
                        // Preserve original aspect ratio
                        var aspectRatio = (double)frame.Width / frame.Height;
                        var newHeight = (int)(pictureBox1.Height);
                        var newWidth = (int)(newHeight * aspectRatio);

                        // Resize while maintaining aspect ratio
                        using (var resizedFrame = new Mat())
                        {
                            Cv2.Resize(frame, resizedFrame, new Size(newWidth, newHeight));
                            using (var bitmap = BitmapConverter.ToBitmap(resizedFrame))
                            {
                                pictureBox1.Image?.Dispose();
                                pictureBox1.Image = (Bitmap)bitmap.Clone();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing frame: {ex.Message}");
                StopCamera();
            }
        }

        private void StopCamera()
{
    timer1.Stop();
    btnStart.Enabled = true;
    btnStop.Enabled = false;

    if (videoCapture != null)
    {
        if (videoCapture.IsOpened())
        {
            videoCapture.Release();
        }
        videoCapture.Dispose();
        videoCapture = null; // This is the crucial fix
    }

    if (pictureBox1.Image != null)
    {
        pictureBox1.Image.Dispose();
        pictureBox1.Image = null;
    }
}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }
    }
}