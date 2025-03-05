using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Size = OpenCvSharp.Size;


namespace LhanzCJ_Installer
{
    public partial class CamTest : Form
    {
        private VideoCapture videoCapture;

        private Label lblLoading;  

        public CamTest()
        {
            InitializeComponent();

            lblLoading = new Label
            {
                Text = "Starting camera...",
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Visible = false,
                BackColor = Color.Transparent
            };
            Controls.Add(lblLoading);
            lblLoading.Location = new Point((ClientSize.Width - lblLoading.Width) / 2, 10);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                ShowLoadingIndicator(true); 

                videoCapture = new VideoCapture(0);
                if (!videoCapture.IsOpened())
                {
                    ShowLoadingIndicator(false);
                    MessageBox.Show("Failed to open webcam!");
                    return;
                }

                btnStart.Enabled = false;
                btnStop.Enabled = true;
                timer1.Start();
            }
            catch (Exception ex)
            {
                ShowLoadingIndicator(false);
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
                        ShowLoadingIndicator(false); 

                        var aspectRatio = (double)frame.Width / frame.Height;
                        var newHeight = (int)(pictureBox1.Height);
                        var newWidth = (int)(newHeight * aspectRatio);

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
            ShowLoadingIndicator(false); 

            if (videoCapture != null)
            {
                if (videoCapture.IsOpened())
                {
                    videoCapture.Release();
                }
                videoCapture.Dispose();
                videoCapture = null;
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

        private void ShowLoadingIndicator(bool show)
        {
            lblLoading.Visible = show;
            if (show)
            {
                lblLoading.Location = new Point((ClientSize.Width - lblLoading.Width) / 2, 10);
            }
        }
    }
}
