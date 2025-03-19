using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class KBTest : Form
    {
        [DllImport("user32.dll")]

        public static extern short GetAsyncKeyState(Keys vKey);
        private Timer keyPollTimer;

        private Dictionary<Keys, Button> keyButtonMap = new Dictionary<Keys, Button>();
        public KBTest()
        {
            KeyPreview = true;
            KeyDown += KBTest_KeyDown;
            InitializeComponent();
            ApplyNoFocusToAllButtons(this);
            InitializeKeyButtonMap();

            keyPollTimer = new Timer
            {
                Interval = 100
            };
            keyPollTimer.Tick += KeyPollTimer_Tick;
            keyPollTimer.Start();

            UpdateLockIndicators();
        }

        private void KeyPollTimer_Tick(object sender, EventArgs e)
        {
            var keyMappings = new (Keys Key, string Name, Button Btn)[]
            {
                (Keys.LShiftKey, "Left Shift", LShiftBtn),
                (Keys.RShiftKey, "Right Shift", RShiftBtn),
                (Keys.LControlKey, "Left Ctrl", LCtrlBtn),
                (Keys.RControlKey, "Right Ctrl", RCtrlBtn),
                (Keys.LWin, "Left Win", LWinBtn),
                (Keys.RWin, "Right Win", RWinBtn),
                (Keys.LMenu, "Left Alt", LAltBtn),
                (Keys.RMenu, "Right Alt", RAltBtn),
            };

            foreach (var (key, name, button) in keyMappings)
            {
                if ((GetAsyncKeyState(key) & 0x8000) != 0)
                {
                    FlashButton(button);
                    RecordKeystroke(name, key);
                    break;
                }
            }

            UpdateLockIndicators();
        }

        private void UpdateLockIndicators()
        {
            UpdateLockIndicator(Keys.CapsLock, capsRad, capsBtn);
            UpdateLockIndicator(Keys.NumLock, numRad, numBtn);
            UpdateLockIndicator(Keys.Scroll, scrollRad, scrollBtn);
        }

        private void UpdateLockIndicator(Keys key, CheckBox checkBox, Button button)
        {
            if (Control.IsKeyLocked(key))
            {
                checkBox.Checked = true;
                button.BackColor = Color.Gray;
            }
            else
            {
                checkBox.Checked = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;
            const int VK_RETURN = 0x0D;
            const int KF_EXTENDED = 0x01000000;

            if (m.Msg == WM_KEYDOWN || m.Msg == WM_KEYUP)
            {
                int vkCode = (int)((long)m.WParam & 0xFF);
                int flags = (int)((long)m.LParam);
                bool isExtended = (flags & KF_EXTENDED) != 0;

                if (vkCode == VK_RETURN)
                {
                    bool isKeyDown = m.Msg == WM_KEYDOWN;
                    bool isNumpadEnter = isExtended;

                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        Button targetButton = isNumpadEnter ? REntBtn : LEntBtn;
                        if (isKeyDown)
                        {
                            FlashButton(targetButton);
                            RecordKeystroke(isNumpadEnter ? "Enter (Numpad)" : "Enter (Main)", Keys.Enter);
                        }
                    });
                    return;
                }
            }

            base.WndProc(ref m);
        }
        private void ApplyNoFocusToAllButtons(Control parent)
        {
            foreach (Control panel1 in parent.Controls)
            {
                if (panel1 is Button)
                {
                    panel1.GotFocus += (s, e) => ActiveControl = null;
                }

                if (panel1.HasChildren)
                {
                    ApplyNoFocusToAllButtons(panel1);
                }
            }
        }
        #region Keymaps
        private readonly Dictionary<Keys, string> keyNameMap = new Dictionary<Keys, string>
        {
            { Keys.Escape, "ESC" },
            { Keys.Oem3, "`" },
            { Keys.LShiftKey, "Left Shift" },
            { Keys.RShiftKey, "Right Shift" },
            { Keys.D1, "1" },
            { Keys.D2, "2" },
            { Keys.D3, "3" },
            { Keys.D4, "4" },
            { Keys.D5, "5" },
            { Keys.D6, "6" },
            { Keys.D7, "7" },
            { Keys.D8, "8" },
            { Keys.D9, "9" },
            { Keys.D0, "0" },
            { Keys.OemMinus, "-" },
            { Keys.Oemplus, "=" },
            { Keys.Back, "Backspace" },
            { Keys.Tab, "Tab" },
            { Keys.Q, "Q" },
            { Keys.W, "W" },
            { Keys.E, "E" },
            { Keys.R, "R" },
            { Keys.T, "T" },
            { Keys.Y, "Y" },
            { Keys.U, "U" },
            { Keys.I, "I" },
            { Keys.O, "O" },
            { Keys.P, "P" },
            { Keys.OemOpenBrackets, "[" },
            { Keys.Oem6, "]" },
            { Keys.OemQuestion, "/" },
            { Keys.A, "A" },
            { Keys.S, "S" },
            { Keys.D, "D" },
            { Keys.F, "F" },
            { Keys.G, "G" },
            { Keys.H, "H" },
            { Keys.J, "J" },
            { Keys.K, "K" },
            { Keys.L, "L" },
            { Keys.Oem1, ";" },
            { Keys.Oem7, "'" },
            { Keys.Oem5, "\\" },
            { Keys.Z, "Z" },
            { Keys.X, "X" },
            { Keys.C, "C" },
            { Keys.V, "V" },
            { Keys.B, "B" },
            { Keys.N, "N" },
            { Keys.M, "M" },
            { Keys.Oemcomma, "," },
            { Keys.OemPeriod, "." },
            { Keys.LControlKey, "Left Ctrl" },
            { Keys.RControlKey, "Right Ctrl" },
            { Keys.LWin, "Left Win" },
            { Keys.RWin, "Right Win" },
            { Keys.Space, "Space" },
            { Keys.LMenu, "Left Alt" },
            { Keys.RMenu, "Right Alt" },
            { Keys.Up, "Up" },
            { Keys.Down, "Down" },
            { Keys.Left, "Left" },
            { Keys.Right, "Right" },
            { Keys.Insert, "Insert" },
            { Keys.Delete, "Delete" },
            { Keys.Home, "Home" },
            { Keys.End, "End" },
            { Keys.PageUp, "Page Up" },
            { Keys.PageDown, "Page Down" },
            { Keys.NumLock, "Num Lock" },
            { Keys.CapsLock, "Caps Lock" },
            { Keys.Scroll, "Scroll Lock" },
            { Keys.Divide, "Num /" },
            { Keys.Multiply, "Num *" },
            { Keys.Subtract, "Num -" },
            { Keys.Add, "Num +" },
            { Keys.NumPad1, "Num 1" },
            { Keys.NumPad2, "Num 2" },
            { Keys.NumPad3, "Num 3" },
            { Keys.NumPad4, "Num 4" },
            { Keys.NumPad5, "Num 5" },
            { Keys.NumPad6, "Num 6" },
            { Keys.NumPad7, "Num 7" },
            { Keys.NumPad8, "Num 8" },
            { Keys.NumPad9, "Num 9" },
            { Keys.NumPad0, "Num 0" },
            { Keys.Decimal, "Num ." },
            { Keys.F1, "F1" },
            { Keys.F2, "F2" },
            { Keys.F3, "F3" },
            { Keys.F4, "F4" },
            { Keys.F5, "F5" },
            { Keys.F6, "F6" },
            { Keys.F7, "F7" },
            { Keys.F8, "F8" },
            { Keys.F9, "F9" },
            { Keys.F10, "F10" },
            { Keys.F11, "F11" },
            { Keys.F12, "F12" },
            { Keys.Apps, "Menu" },
            { Keys.PrintScreen, "Print Screen" },
            { Keys.Pause, "Pause" },
        };

        #endregion

        #region ButtonMaps
        private void InitializeKeyButtonMap()
        {
            keyButtonMap[Keys.Escape] = EscBtn;
            keyButtonMap[Keys.Oem3] = AposBtn;
            keyButtonMap[Keys.LShiftKey] = LShiftBtn;
            keyButtonMap[Keys.RShiftKey] = RShiftBtn;
            keyButtonMap[Keys.D1] = oneBtn;
            keyButtonMap[Keys.D2] = twoBtn;
            keyButtonMap[Keys.D3] = thrBtn;
            keyButtonMap[Keys.D4] = fourBtn;
            keyButtonMap[Keys.D5] = fivBtn;
            keyButtonMap[Keys.D6] = sixBtn;
            keyButtonMap[Keys.D7] = sevBtn;
            keyButtonMap[Keys.D8] = eigBtn;
            keyButtonMap[Keys.D9] = ninBtn;
            keyButtonMap[Keys.D0] = zerBtn;
            keyButtonMap[Keys.OemMinus] = minBtn;
            keyButtonMap[Keys.Oemplus] = equBtn;
            keyButtonMap[Keys.Back] = bckspcBtn;
            keyButtonMap[Keys.Tab] = tabBtn;
            keyButtonMap[Keys.Q] = qBtn;
            keyButtonMap[Keys.W] = wBtn;
            keyButtonMap[Keys.E] = eBtn;
            keyButtonMap[Keys.R] = rBtn;
            keyButtonMap[Keys.T] = tBtn;
            keyButtonMap[Keys.Y] = yBtn;
            keyButtonMap[Keys.U] = uBtn;
            keyButtonMap[Keys.I] = iBtn;
            keyButtonMap[Keys.O] = oBtn;
            keyButtonMap[Keys.P] = pBtn;
            keyButtonMap[Keys.OemOpenBrackets] = lBracBtn;
            keyButtonMap[Keys.Oem6] = rBracBtn;
            keyButtonMap[Keys.OemQuestion] = fSlashBtn;
            keyButtonMap[Keys.A] = aBtn;
            keyButtonMap[Keys.S] = sBtn;
            keyButtonMap[Keys.D] = dBtn;
            keyButtonMap[Keys.F] = fBtn;
            keyButtonMap[Keys.G] = gBtn;
            keyButtonMap[Keys.H] = hBtn;
            keyButtonMap[Keys.J] = jBtn;
            keyButtonMap[Keys.K] = kBtn;
            keyButtonMap[Keys.L] = lBtn;
            keyButtonMap[Keys.Oem1] = semiBtn;
            keyButtonMap[Keys.Oem7] = apos1Btn;
            keyButtonMap[Keys.Oem5] = bSlashBtn;
            keyButtonMap[Keys.Z] = zBtn;
            keyButtonMap[Keys.X] = xBtn;
            keyButtonMap[Keys.C] = cBtn;
            keyButtonMap[Keys.V] = vBtn;
            keyButtonMap[Keys.B] = bBtn;
            keyButtonMap[Keys.N] = nBtn;
            keyButtonMap[Keys.M] = mBtn;
            keyButtonMap[Keys.Oemcomma] = comBtn;
            keyButtonMap[Keys.OemPeriod] = dotBtn;
            keyButtonMap[Keys.LControlKey] = LCtrlBtn;
            keyButtonMap[Keys.RControlKey] = RCtrlBtn;
            keyButtonMap[Keys.LWin] = LWinBtn;
            keyButtonMap[Keys.RWin] = RWinBtn;
            keyButtonMap[Keys.Space] = spaceBtn;
            keyButtonMap[Keys.LMenu] = LAltBtn;
            keyButtonMap[Keys.RMenu] = RAltBtn;
            keyButtonMap[Keys.Up] = upBtn;
            keyButtonMap[Keys.Down] = downBtn;
            keyButtonMap[Keys.Left] = leftBtn;
            keyButtonMap[Keys.Right] = rightBtn;
            keyButtonMap[Keys.Insert] = insBtn;
            keyButtonMap[Keys.Delete] = delBtn;
            keyButtonMap[Keys.Home] = homeBtn;
            keyButtonMap[Keys.End] = endBtn;
            keyButtonMap[Keys.PageUp] = pgUpBtn;
            keyButtonMap[Keys.PageDown] = pgDnBtn;
            keyButtonMap[Keys.NumLock] = numBtn;
            keyButtonMap[Keys.CapsLock] = capsBtn;
            keyButtonMap[Keys.Scroll] = scrollBtn;
            keyButtonMap[Keys.Divide] = divBtn;
            keyButtonMap[Keys.Multiply] = mulBtn;
            keyButtonMap[Keys.Subtract] = subBtn;
            keyButtonMap[Keys.Add] = addBtn;
            keyButtonMap[Keys.NumPad1] = oneNumBtn;
            keyButtonMap[Keys.NumPad2] = twoNumBtn;
            keyButtonMap[Keys.NumPad3] = thrNumBtn;
            keyButtonMap[Keys.NumPad4] = fourNumBtn;
            keyButtonMap[Keys.NumPad5] = fivNumBtn;
            keyButtonMap[Keys.NumPad6] = sixNumBtn;
            keyButtonMap[Keys.NumPad7] = sevNumBtn;
            keyButtonMap[Keys.NumPad8] = eigNumBtn;
            keyButtonMap[Keys.NumPad9] = ninNumBtn;
            keyButtonMap[Keys.NumPad0] = zerNumBtn;
            keyButtonMap[Keys.Decimal] = decBtn;
            keyButtonMap[Keys.F1] = f1Btn;
            keyButtonMap[Keys.F2] = f2Btn;
            keyButtonMap[Keys.F3] = f3Btn;
            keyButtonMap[Keys.F4] = f4Btn;
            keyButtonMap[Keys.F5] = f5Btn;
            keyButtonMap[Keys.F6] = f6Btn;
            keyButtonMap[Keys.F7] = f7Btn;
            keyButtonMap[Keys.F8] = f8Btn;
            keyButtonMap[Keys.F9] = f9Btn;
            keyButtonMap[Keys.F10] = f10Btn;
            keyButtonMap[Keys.F11] = f11Btn;
            keyButtonMap[Keys.F12] = f12Btn;
            keyButtonMap[Keys.Apps] = MenuBtn;
            keyButtonMap[Keys.PrintScreen] = prtScBtn;
            keyButtonMap[Keys.Pause] = pauseBtn;

        }
        #endregion
        private async void FlashButton(Button btn)
        {

            btn.BackColor = Color.LightGray;
            Color originalColor = btn.BackColor;
            btn.BackColor = Color.LightCyan;
            await Task.Delay(100);
            btn.BackColor = originalColor;
        }
        private void HandleKeyPress(Keys key)
        {
            string keyText = keyNameMap.ContainsKey(key) ? keyNameMap[key] : key.ToString();
            RecordKeystroke(keyText, key);
        }

        private void KBTest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                return;
            }

            if (keyButtonMap.TryGetValue(e.KeyCode, out Button targetButton))
            {
                FlashButton(targetButton);
                HandleKeyPress(e.KeyCode);
            }
        }
        private void KBTest_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyButtonMap.TryGetValue(e.KeyCode, out Button targetButton))
            {
                FlashButton(targetButton);
                HandleKeyPress(e.KeyCode);
            }
        }


        private void RecordKeystroke(string keyText, Keys keyCode)
        {

            lblKeystrokes.Text = $"{DateTime.Now:HH:mm:ss} - {keyText} ({keyCode})\n" + lblKeystrokes.Text;
        }

    }
}