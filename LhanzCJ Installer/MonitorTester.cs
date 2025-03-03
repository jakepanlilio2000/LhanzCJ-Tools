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
            KeyPreview = true;
            KeyDown += MonitorTester_KeyDown;
        }

        private void LoadImages()
        {
            string imgFolder = Path.Combine(Application.StartupPath, "img");


            foreach (string file in Directory.GetFiles(imgFolder, "*.png"))
            {
                imagePaths.Add(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                string displayName = fileName.Replace("FHD-1920x1080-", "")
                                            .Replace("-", " ");
                imageNames.Add(displayName);
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
                Text = imageNames[currentIndex];
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
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;

            comboBox1.Left = (ClientSize.Width - comboBox1.Width) / 2;
            int buttonTop = ClientSize.Height - btnPrev.Height - 20;
            btnPrev.Left = (ClientSize.Width / 2) - btnPrev.Width - 10;
            btnNext.Left = (ClientSize.Width / 2) + 10;
            btnPrev.Top = btnNext.Top = buttonTop;
        }

        private void MonitorTester_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
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