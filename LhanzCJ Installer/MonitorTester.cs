using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class MonitorTester : Form
    {
        private List<string> imagePaths = new List<string>();
        private List<string> imageNames = new List<string>();
        private int currentIndex = 0;

        public MonitorTester()
        {
            InitializeComponent();
            LoadImages();
            this.KeyPreview = true;
            this.KeyDown += MonitorTester_KeyDown;
        }

        private void LoadImages()
        {
            string imgFolder = Path.Combine(Application.StartupPath, "img");

            if (!Directory.Exists(imgFolder))
            {
                MessageBox.Show("Image folder not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            foreach (string file in Directory.GetFiles(imgFolder, "*.png"))
            {
                imagePaths.Add(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                string displayName = fileName.Replace("FHD-1920x1080-", "")
                                            .Replace("-", " ");
                imageNames.Add(displayName);
            }

            if (imagePaths.Count == 0)
            {
                MessageBox.Show("No images found in the folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            comboBox1.DataSource = new List<string>(imageNames);
            UpdateImage();
        }

        private void UpdateImage()
        {
            try
            {
                if (pictureBox.Image != null)
                {
                    pictureBox.Image.Dispose();
                }

                using (Image tempImage = Image.FromFile(imagePaths[currentIndex]))
                {
                    pictureBox.Image = new Bitmap(tempImage);
                }

                comboBox1.SelectedIndex = currentIndex;
                this.Text = imageNames[currentIndex];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            currentIndex = (currentIndex - 1 + imagePaths.Count) % imagePaths.Count;
            UpdateImage();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            currentIndex = (currentIndex + 1) % imagePaths.Count;
            UpdateImage();
        }

        private void MonitorTester_Load(object sender, EventArgs e)
        {
            // True fullscreen setup
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            // Position controls
            comboBox1.Left = (this.ClientSize.Width - comboBox1.Width) / 2;
            int buttonTop = this.ClientSize.Height - btnPrev.Height - 20;
            btnPrev.Left = (this.ClientSize.Width / 2) - btnPrev.Width - 10;
            btnNext.Left = (this.ClientSize.Width / 2) + 10;
            btnPrev.Top = btnNext.Top = buttonTop;
        }

        private void MonitorTester_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != currentIndex)
            {
                currentIndex = comboBox1.SelectedIndex;
                UpdateImage();
            }
        }
    }
}