using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace LhanzCJ_Installer
{
    public partial class MonitorTester: Form
    {
        private List<string> testPatterns = new List<string>
        {
            "Solid White",
            "Solid Black",
            "Solid Red",
            "Solid Green",
            "Solid Blue",
            "Horizontal Gradient",
            "Vertical Gradient",
            "RGB Gradient",
            "Grid Pattern",
            "Converging Lines",
            "Color Bars",
            "Pixel Walk",
            "Gray Scale"
        };

        private int currentTestIndex = 0;
        private bool isFullScreen = false;
        private FormWindowState previousWindowState;

        public MonitorTester()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void MonitorTesterForm_Load(object sender, EventArgs e)
        {
            cbTests.DataSource = testPatterns;
            GenerateTestPattern();
            UpdateResolutionLabel();
        }

        private void GenerateTestPattern()
        {
            var bmp = new Bitmap(pbDisplay.Width, pbDisplay.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                switch (testPatterns[currentTestIndex])
                {
                    case "Solid White":
                        g.Clear(Color.White);
                        break;

                    case "Solid Black":
                        g.Clear(Color.Black);
                        break;

                    case "Solid Red":
                        g.Clear(Color.Red);
                        break;

                    case "Solid Green":
                        g.Clear(Color.Green);
                        break;

                    case "Solid Blue":
                        g.Clear(Color.Blue);
                        break;

                    case "Horizontal Gradient":
                        using (var brush = new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(bmp.Width, 0),
                            Color.Black,
                            Color.White))
                        {
                            g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                        }
                        break;

                    case "Vertical Gradient":
                        using (var brush = new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(0, bmp.Height),
                            Color.Black,
                            Color.White))
                        {
                            g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                        }
                        break;

                    case "RGB Gradient":
                        using (var brush = new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(bmp.Width, bmp.Height),
                            Color.Red,
                            Color.Blue))
                        {
                            g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                        }
                        break;

                    case "Grid Pattern":
                        DrawGridPattern(g, bmp.Size, 20);
                        break;

                    case "Converging Lines":
                        DrawConvergingLines(g, bmp.Size);
                        break;

                    case "Color Bars":
                        DrawColorBars(g, bmp.Size);
                        break;

                    case "Pixel Walk":
                        DrawPixelWalk(bmp);
                        break;

                    case "Gray Scale":
                        DrawGrayScale(g, bmp.Size);
                        break;
                }

                // Add test information
                string info = $"{testPatterns[currentTestIndex]}\n{pbDisplay.Width}x{pbDisplay.Height}";
                g.DrawString(info, new Font("Arial", 12), Brushes.White, 10, 10);
            }

            pbDisplay.Image?.Dispose();
            pbDisplay.Image = bmp;
            lblTestName.Text = testPatterns[currentTestIndex];
            UpdateResolutionLabel();
        }

        private void DrawGridPattern(Graphics g, Size size, int spacing)
        {
            using (var pen = new Pen(Color.White, 1))
            {
                // Vertical lines
                for (int x = 0; x < size.Width; x += spacing)
                {
                    g.DrawLine(pen, x, 0, x, size.Height);
                }

                // Horizontal lines
                for (int y = 0; y < size.Height; y += spacing)
                {
                    g.DrawLine(pen, 0, y, size.Width, y);
                }
            }
        }
        private void DrawConvergingLines(Graphics g, Size size)
        {
            // Calculate the center point of the drawing area.
            int centerX = size.Width / 2;
            int centerY = size.Height / 2;

            // Use a pen with a contrasting color.
            using (Pen pen = new Pen(Color.Black, 1))
            {
                // Draw lines from points along the top and bottom edges to the center.
                for (int x = 0; x < size.Width; x += 10)
                {
                    g.DrawLine(pen, x, 0, centerX, centerY);
                    g.DrawLine(pen, x, size.Height - 1, centerX, centerY);
                }

                // Draw lines from points along the left and right edges to the center.
                for (int y = 0; y < size.Height; y += 10)
                {
                    g.DrawLine(pen, 0, y, centerX, centerY);
                    g.DrawLine(pen, size.Width - 1, y, centerX, centerY);
                }
            }
        }

        private void DrawColorBars(Graphics g, Size size)
        {
            // Define a set of colors for the vertical bars.
            Color[] colors = new Color[]
            {
        Color.White,
        Color.Yellow,
        Color.Cyan,
        Color.Green,
        Color.Magenta,
        Color.Red,
        Color.Blue
            };

            int barCount = colors.Length;
            int barWidth = size.Width / barCount;

            // Draw each color bar.
            for (int i = 0; i < barCount; i++)
            {
                using (SolidBrush brush = new SolidBrush(colors[i]))
                {
                    g.FillRectangle(brush, i * barWidth, 0, barWidth, size.Height);
                }
            }
        }

        private void DrawPixelWalk(Bitmap bmp)
        {
            Random rnd = new Random();
            // Start from the center of the bitmap.
            int x = bmp.Width / 2;
            int y = bmp.Height / 2;

            // Determine the number of steps (tweak this value as desired).
            int steps = (bmp.Width * bmp.Height) / 20;

            for (int i = 0; i < steps; i++)
            {
                // Choose a random step from -1, 0, or 1 for both x and y.
                int dx = rnd.Next(-1, 2);
                int dy = rnd.Next(-1, 2);

                // Update position and ensure it's within the bounds.
                x = Math.Max(0, Math.Min(bmp.Width - 1, x + dx));
                y = Math.Max(0, Math.Min(bmp.Height - 1, y + dy));

                // Set the pixel at (x,y) to a random color.
                bmp.SetPixel(x, y, Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
            }
        }

        private void DrawGrayScale(Graphics g, Size size)
        {
            // Draw a vertical series of lines, each with a slightly different gray value.
            for (int x = 0; x < size.Width; x++)
            {
                // Compute the gray intensity based on the x position.
                int gray = (int)(x * 255.0 / (size.Width - 1));
                using (Pen pen = new Pen(Color.FromArgb(gray, gray, gray)))
                {
                    g.DrawLine(pen, x, 0, x, size.Height);
                }
            }
        }



        private void UpdateResolutionLabel()
        {
            lblResolution.Text = $"{pbDisplay.Width}x{pbDisplay.Height}";
        }

        // Add similar drawing methods for other patterns (ColorBars, ConvergingLines, etc.)

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            currentTestIndex = (currentTestIndex - 1 + testPatterns.Count) % testPatterns.Count;
            GenerateTestPattern();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentTestIndex = (currentTestIndex + 1) % testPatterns.Count;
            GenerateTestPattern();
        }

        private void cbTests_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentTestIndex = cbTests.SelectedIndex;
            GenerateTestPattern();
        }

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }

        private void ToggleFullScreen()
        {
            if (!isFullScreen)
            {
                previousWindowState = WindowState;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                isFullScreen = true;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = previousWindowState;
                isFullScreen = false;
            }
        }

        private void MonitorTesterForm_Resize(object sender, EventArgs e)
        {
            GenerateTestPattern();
        }

        private void MonitorTesterForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (isFullScreen) ToggleFullScreen();
                    break;
                case Keys.Left:
                    btnPrevious.PerformClick();
                    break;
                case Keys.Right:
                    btnNext.PerformClick();
                    break;
                case Keys.F11:
                    ToggleFullScreen();
                    break;
            }
        }

    }
}
