using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class KeyboardTester : Form
    {
        private const int baseWidth = 45;
        private const int baseHeight = 45;
        private const int keyGap = 3;
        private readonly Color pressedColor = Color.FromArgb(100, 181, 246);
        private readonly Color toggledColor = Color.FromArgb(129, 199, 132);
        private readonly Color keyColor = Color.FromArgb(64, 64, 64);
        private readonly Color textColor = Color.White;

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        private const int VK_LSHIFT = 0xA0;
        private const int VK_RSHIFT = 0xA1;
        private const int VK_LCONTROL = 0xA2;
        private const int VK_RCONTROL = 0xA3;
        private const int VK_LMENU = 0xA4;    
        private const int VK_RMENU = 0xA5;    

        public KeyboardTester()
        {
            InitializeComponent();
            KeyPreview = true;
            GenerateMainKeyboard();
            int mainKeyboardWidth = GetMainKeyboardWidth();
            GenerateNavCluster(mainKeyboardWidth);
            GenerateNumpad(mainKeyboardWidth);
        }
        private void GenerateMainKeyboard()
        {
            var mainKeys = new List<List<KeyInfo>>
            {
                new List<KeyInfo>
                {
                    new KeyInfo("Esc", Keys.Escape, 1),
                    new KeyInfo("F1", Keys.F1, 1),
                    new KeyInfo("F2", Keys.F2, 1),
                    new KeyInfo("F3", Keys.F3, 1),
                    new KeyInfo("F4", Keys.F4, 1),
                    new KeyInfo("F5", Keys.F5, 1),
                    new KeyInfo("F6", Keys.F6, 1),
                    new KeyInfo("F7", Keys.F7, 1),
                    new KeyInfo("F8", Keys.F8, 1),
                    new KeyInfo("F9", Keys.F9, 1),
                    new KeyInfo("F10", Keys.F10, 1),
                    new KeyInfo("F11", Keys.F11, 1),
                    new KeyInfo("F12", Keys.F12, 1)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("~\n`", Keys.Oemtilde, 1),
                    new KeyInfo("!\n1", Keys.D1, 1),
                    new KeyInfo("@\n2", Keys.D2, 1),
                    new KeyInfo("#\n3", Keys.D3, 1),
                    new KeyInfo("$\n4", Keys.D4, 1),
                    new KeyInfo("%\n5", Keys.D5, 1),
                    new KeyInfo("^\n6", Keys.D6, 1),
                    new KeyInfo("&\n7", Keys.D7, 1),
                    new KeyInfo("*\n8", Keys.D8, 1),
                    new KeyInfo("(\n9", Keys.D9, 1),
                    new KeyInfo(")\n0", Keys.D0, 1),
                    new KeyInfo("_\n-", Keys.OemMinus, 1),
                    new KeyInfo("+\n=", Keys.Oemplus, 1),
                    new KeyInfo("Backspace", Keys.Back, 2)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Tab", Keys.Tab, 1.5f),
                    new KeyInfo("Q", Keys.Q, 1),
                    new KeyInfo("W", Keys.W, 1),
                    new KeyInfo("E", Keys.E, 1),
                    new KeyInfo("R", Keys.R, 1),
                    new KeyInfo("T", Keys.T, 1),
                    new KeyInfo("Y", Keys.Y, 1),
                    new KeyInfo("U", Keys.U, 1),
                    new KeyInfo("I", Keys.I, 1),
                    new KeyInfo("O", Keys.O, 1),
                    new KeyInfo("P", Keys.P, 1),
                    new KeyInfo("{\n[", Keys.OemOpenBrackets, 1),
                    new KeyInfo("}\n]", Keys.OemCloseBrackets, 1),
                    new KeyInfo("|\n\\", Keys.OemPipe, 1.5f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Caps", Keys.CapsLock, 1.8f),
                    new KeyInfo("A", Keys.A, 1),
                    new KeyInfo("S", Keys.S, 1),
                    new KeyInfo("D", Keys.D, 1),
                    new KeyInfo("F", Keys.F, 1),
                    new KeyInfo("G", Keys.G, 1),
                    new KeyInfo("H", Keys.H, 1),
                    new KeyInfo("J", Keys.J, 1),
                    new KeyInfo("K", Keys.K, 1),
                    new KeyInfo("L", Keys.L, 1),
                    new KeyInfo(":\n;", Keys.OemSemicolon, 1),
                    new KeyInfo("\"\n'", Keys.OemQuotes, 1),
                    new KeyInfo("Enter", Keys.Enter, 2.2f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Shift", Keys.LShiftKey, 2.3f),
                    new KeyInfo("Z", Keys.Z, 1),
                    new KeyInfo("X", Keys.X, 1),
                    new KeyInfo("C", Keys.C, 1),
                    new KeyInfo("V", Keys.V, 1),
                    new KeyInfo("B", Keys.B, 1),
                    new KeyInfo("N", Keys.N, 1),
                    new KeyInfo("M", Keys.M, 1),
                    new KeyInfo("<\n,", Keys.Oemcomma, 1),
                    new KeyInfo(">\n.", Keys.OemPeriod, 1),
                    new KeyInfo("?\n/", Keys.OemQuestion, 1),
                    new KeyInfo("Shift", Keys.RShiftKey, 2.3f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Ctrl", Keys.LControlKey, 1.5f),
                    new KeyInfo("Win", Keys.LWin, 1.2f),
                    new KeyInfo("Alt", Keys.LMenu, 1.2f),
                    new KeyInfo("Space", Keys.Space, 6),
                    new KeyInfo("Alt", Keys.RMenu, 1.2f),
                    new KeyInfo("Win", Keys.RWin, 1.2f),
                    new KeyInfo("Menu", Keys.Apps, 1.2f),
                    new KeyInfo("Ctrl", Keys.RControlKey, 1.5f)
                }
            };

            int startX = 10;
            int startY = 10;
            int verticalSpacing = 10;

            foreach (var row in mainKeys)
            {
                int currentX = startX;
                foreach (var key in row)
                {
                    CreateKey(key, currentX, startY);
                    currentX += (int)(baseWidth * key.WidthMultiplier) + keyGap;
                }
                startY += baseHeight + verticalSpacing;
            }
        }
        private void GenerateNavCluster(int mainKeyboardWidth)
        {
            int startX = mainKeyboardWidth + 20;
            int startY = 10;
            int verticalSpacing = 10;

            var navRow1 = new List<KeyInfo>
    {
        new KeyInfo("PrtScr", Keys.PrintScreen, 1),
        new KeyInfo("ScrLk", Keys.Scroll, 1),
        new KeyInfo("Pause", Keys.Pause, 1)
    };

            var navRow2 = new List<KeyInfo>
    {
        new KeyInfo("Insert", Keys.Insert, 1),
        new KeyInfo("Home", Keys.Home, 1),
        new KeyInfo("PgUp", Keys.PageUp, 1)
    };
            var navRow3 = new List<KeyInfo>
    {
        new KeyInfo("Delete", Keys.Delete, 1),
        new KeyInfo("End", Keys.End, 1),
        new KeyInfo("PgDn", Keys.PageDown, 1)
    };

            List<List<KeyInfo>> navRows = new List<List<KeyInfo>> { navRow1, navRow2, navRow3 };

            for (int i = 0; i < navRows.Count; i++)
            {
                int currentX = startX;
                foreach (var key in navRows[i])
                {
                    CreateKey(key, currentX, startY);
                    currentX += (int)(baseWidth * key.WidthMultiplier) + keyGap;
                }
                startY += baseHeight + verticalSpacing;
                if (i == 0)
                {
                    startY += 20;
                }
            }
            int arrowStartY = startY + 30;
            int navClusterWidth = 3 * baseWidth + 2 * keyGap;
            int upX = startX + (navClusterWidth - baseWidth) / 2;
            CreateKey(new KeyInfo("Up", Keys.Up, 1), upX, arrowStartY);

            arrowStartY += baseHeight + verticalSpacing;
            var arrowRow = new List<KeyInfo>
    {
        new KeyInfo("Left", Keys.Left, 1),
        new KeyInfo("Down", Keys.Down, 1),
        new KeyInfo("Right", Keys.Right, 1)
    };
            int currentArrowX = startX;
            foreach (var key in arrowRow)
            {
                CreateKey(key, currentArrowX, arrowStartY);
                currentArrowX += (int)(baseWidth * key.WidthMultiplier) + keyGap;
            }
        }
        private void GenerateNumpad(int mainKeyboardWidth)
        {
            int navClusterWidth = 3 * baseWidth + 2 * keyGap;
            int startX = mainKeyboardWidth + 20 + navClusterWidth + 20;
            int startY = 60;
            int verticalSpacing = 10;

            var numpadKeys = new List<List<KeyInfo>>
            {
                new List<KeyInfo>
                {
                    new KeyInfo("Num Lock", Keys.NumLock, 1) { IsNumpad = true },
                    new KeyInfo("/", Keys.Divide, 1) { IsNumpad = true },
                    new KeyInfo("*", Keys.Multiply, 1) { IsNumpad = true },
                    new KeyInfo("-", Keys.Subtract, 1) { IsNumpad = true }
                },
                new List<KeyInfo>
                {
                    new KeyInfo("7", Keys.NumPad7, 1) { IsNumpad = true },
                    new KeyInfo("8", Keys.NumPad8, 1) { IsNumpad = true },
                    new KeyInfo("9", Keys.NumPad9, 1) { IsNumpad = true },
                    new KeyInfo("+", Keys.Add, 1) { IsNumpad = true }
                },
                new List<KeyInfo>
                {
                    new KeyInfo("4", Keys.NumPad4, 1) { IsNumpad = true },
                    new KeyInfo("5", Keys.NumPad5, 1) { IsNumpad = true },
                    new KeyInfo("6", Keys.NumPad6, 1) { IsNumpad = true }
                },
                new List<KeyInfo>
                {
                    new KeyInfo("1", Keys.NumPad1, 1) { IsNumpad = true },
                    new KeyInfo("2", Keys.NumPad2, 1) { IsNumpad = true },
                    new KeyInfo("3", Keys.NumPad3, 1) { IsNumpad = true },
                    new KeyInfo("Enter", Keys.Enter, 1) { IsNumpad = true }
                },
                new List<KeyInfo>
                {
                    new KeyInfo("0", Keys.NumPad0, 2) { IsNumpad = true },
                    new KeyInfo(".", Keys.Decimal, 1) { IsNumpad = true }
                }
            };

            foreach (var row in numpadKeys)
            {
                int currentX = startX;
                foreach (var key in row)
                {
                    CreateKey(key, currentX, startY);
                    currentX += (int)(baseWidth * key.WidthMultiplier) + keyGap;
                }
                startY += baseHeight + verticalSpacing;
            }
        }
        private int GetMainKeyboardWidth()
        {
            var mainKeys = new List<List<KeyInfo>>
            {
                new List<KeyInfo>
                {
                    new KeyInfo("Esc", Keys.Escape, 1),
                    new KeyInfo("F1", Keys.F1, 1),
                    new KeyInfo("F2", Keys.F2, 1),
                    new KeyInfo("F3", Keys.F3, 1),
                    new KeyInfo("F4", Keys.F4, 1),
                    new KeyInfo("F5", Keys.F5, 1),
                    new KeyInfo("F6", Keys.F6, 1),
                    new KeyInfo("F7", Keys.F7, 1),
                    new KeyInfo("F8", Keys.F8, 1),
                    new KeyInfo("F9", Keys.F9, 1),
                    new KeyInfo("F10", Keys.F10, 1),
                    new KeyInfo("F11", Keys.F11, 1),
                    new KeyInfo("F12", Keys.F12, 1)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("~\n`", Keys.Oemtilde, 1),
                    new KeyInfo("!\n1", Keys.D1, 1),
                    new KeyInfo("@\n2", Keys.D2, 1),
                    new KeyInfo("#\n3", Keys.D3, 1),
                    new KeyInfo("$\n4", Keys.D4, 1),
                    new KeyInfo("%\n5", Keys.D5, 1),
                    new KeyInfo("^\n6", Keys.D6, 1),
                    new KeyInfo("&\n7", Keys.D7, 1),
                    new KeyInfo("*\n8", Keys.D8, 1),
                    new KeyInfo("(\n9", Keys.D9, 1),
                    new KeyInfo(")\n0", Keys.D0, 1),
                    new KeyInfo("_\n-", Keys.OemMinus, 1),
                    new KeyInfo("+\n=", Keys.Oemplus, 1),
                    new KeyInfo("Backspace", Keys.Back, 2)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Tab", Keys.Tab, 1.5f),
                    new KeyInfo("Q", Keys.Q, 1),
                    new KeyInfo("W", Keys.W, 1),
                    new KeyInfo("E", Keys.E, 1),
                    new KeyInfo("R", Keys.R, 1),
                    new KeyInfo("T", Keys.T, 1),
                    new KeyInfo("Y", Keys.Y, 1),
                    new KeyInfo("U", Keys.U, 1),
                    new KeyInfo("I", Keys.I, 1),
                    new KeyInfo("O", Keys.O, 1),
                    new KeyInfo("P", Keys.P, 1),
                    new KeyInfo("{\n[", Keys.OemOpenBrackets, 1),
                    new KeyInfo("}\n]", Keys.OemCloseBrackets, 1),
                    new KeyInfo("|\n\\", Keys.OemPipe, 1.5f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Caps", Keys.CapsLock, 1.8f),
                    new KeyInfo("A", Keys.A, 1),
                    new KeyInfo("S", Keys.S, 1),
                    new KeyInfo("D", Keys.D, 1),
                    new KeyInfo("F", Keys.F, 1),
                    new KeyInfo("G", Keys.G, 1),
                    new KeyInfo("H", Keys.H, 1),
                    new KeyInfo("J", Keys.J, 1),
                    new KeyInfo("K", Keys.K, 1),
                    new KeyInfo("L", Keys.L, 1),
                    new KeyInfo(":\n;", Keys.OemSemicolon, 1),
                    new KeyInfo("\"\n'", Keys.OemQuotes, 1),
                    new KeyInfo("Enter", Keys.Enter, 2.2f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Shift", Keys.LShiftKey, 2.3f),
                    new KeyInfo("Z", Keys.Z, 1),
                    new KeyInfo("X", Keys.X, 1),
                    new KeyInfo("C", Keys.C, 1),
                    new KeyInfo("V", Keys.V, 1),
                    new KeyInfo("B", Keys.B, 1),
                    new KeyInfo("N", Keys.N, 1),
                    new KeyInfo("M", Keys.M, 1),
                    new KeyInfo("<\n,", Keys.Oemcomma, 1),
                    new KeyInfo(">\n.", Keys.OemPeriod, 1),
                    new KeyInfo("?\n/", Keys.OemQuestion, 1),
                    new KeyInfo("Shift", Keys.RShiftKey, 2.3f)
                },
                new List<KeyInfo>
                {
                    new KeyInfo("Ctrl", Keys.LControlKey, 1.5f),
                    new KeyInfo("Win", Keys.LWin, 1.2f),
                    new KeyInfo("Alt", Keys.LMenu, 1.2f),
                    new KeyInfo("Space", Keys.Space, 6),
                    new KeyInfo("Alt", Keys.RMenu, 1.2f),
                    new KeyInfo("Win", Keys.RWin, 1.2f),
                    new KeyInfo("Menu", Keys.Apps, 1.2f),
                    new KeyInfo("Ctrl", Keys.RControlKey, 1.5f)
                }
            };
            int maxWidth = 0;
            int gap = keyGap;
            foreach (var row in mainKeys)
            {
                int rowWidth = 10;
                foreach (var key in row)
                {
                    rowWidth += (int)(baseWidth * key.WidthMultiplier) + gap;
                }
                if (rowWidth > maxWidth)
                    maxWidth = rowWidth;
            }
            return maxWidth;
        }
        private void CreateKey(KeyInfo key, int x, int y)
        {
            Button btn = new Button
            {
                Text = key.DisplayText,
                Tag = key,  // store the full KeyInfo object
                Size = new Size((int)(baseWidth * key.WidthMultiplier), baseHeight),
                Location = new Point(x, y),
                Font = new Font("Arial", 8),
                Margin = new Padding(1),
                TabStop = false
            };
            panelKeyboard.Controls.Add(btn);
        }

        private void RecordKeystroke(string keyText, Keys keyCode)
        {
            lblKeystrokes.Text = $"{DateTime.Now:HH:mm:ss} - {keyText} ({keyCode})\n" + lblKeystrokes.Text;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.ShiftKey)
            {
                if ((GetKeyState(VK_LSHIFT) & 0x8000) != 0)
                    HighlightKey(Keys.LShiftKey, pressedColor);
                if ((GetKeyState(VK_RSHIFT) & 0x8000) != 0)
                    HighlightKey(Keys.RShiftKey, pressedColor);
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                if ((GetKeyState(VK_LCONTROL) & 0x8000) != 0)
                    HighlightKey(Keys.LControlKey, pressedColor);
                if ((GetKeyState(VK_RCONTROL) & 0x8000) != 0)
                    HighlightKey(Keys.RControlKey, pressedColor);
            }
            else if (e.KeyCode == Keys.Menu)
            {
                if ((GetKeyState(VK_LMENU) & 0x8000) != 0)
                    HighlightKey(Keys.LMenu, pressedColor);
                if ((GetKeyState(VK_RMENU) & 0x8000) != 0)
                    HighlightKey(Keys.RMenu, pressedColor);
            }
            else
            {
                HighlightKey(e.KeyCode, pressedColor);
            }
            RecordKeystroke(e.KeyCode.ToString(), e.KeyCode);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey)
            {
                HighlightKey(Keys.LShiftKey, SystemColors.Control);
                HighlightKey(Keys.RShiftKey, SystemColors.Control);
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                HighlightKey(Keys.LControlKey, SystemColors.Control);
                HighlightKey(Keys.RControlKey, SystemColors.Control);
            }
            else if (e.KeyCode == Keys.Menu)
            {
                HighlightKey(Keys.LMenu, SystemColors.Control);
                HighlightKey(Keys.RMenu, SystemColors.Control);
            }
            else
            {
                HighlightKey(e.KeyCode, SystemColors.Control);
            }
        }
        private void HighlightKey(Keys keyCode, Color color)
        {
            foreach (Control ctrl in panelKeyboard.Controls)
            {
                if (ctrl is Button btn && btn.Tag is KeyInfo keyInfo)
                {
                    if (keyInfo.KeyCode == keyCode)
                    {
                        if (keyCode == Keys.Enter && keyInfo.IsNumpad)
                            continue;
                        if (btn.BackColor != toggledColor)
                            btn.BackColor = color;
                    }
                }
            }
        }
    }
    public class KeyInfo
    {
        public string DisplayText { get; }
        public Keys KeyCode { get; }
        public float WidthMultiplier { get; }
        public bool IsNumpad { get; set; }

        public KeyInfo(string text, Keys code, float width)
        {
            DisplayText = text;
            KeyCode = code;
            WidthMultiplier = width;
            IsNumpad = false;
        }
    }
}
