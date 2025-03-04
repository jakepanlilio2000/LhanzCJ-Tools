using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;

namespace LhanzCJ_Installer
{
    public partial class KBTest : Form
    {
        [DllImport("user32.dll")]

        public static extern short GetAsyncKeyState(Keys vKey);
        private Timer keyPollTimer;

        private Dictionary<Keys, Button> keyButtonMap = new Dictionary<Keys, Button>();
        private HashSet<Keys> firstPressKeys = new HashSet<Keys>();
        public KBTest()
        {
            KeyPreview = true;
            KeyDown += KBTest_KeyDown;
            InitializeComponent();
            ApplyNoFocusToAllButtons(this);

            InitializeKeyButtonMap();

            keyPollTimer = new Timer();
            keyPollTimer.Interval = 100;
            keyPollTimer.Tick += KeyPollTimer_Tick;
            keyPollTimer.Start();
        }
        private void KeyPollTimer_Tick(object sender, EventArgs e)
        {
            bool isLeftShiftDown = (GetAsyncKeyState(Keys.LShiftKey) & 0x8000) != 0;
            bool isRightShiftDown = (GetAsyncKeyState(Keys.RShiftKey) & 0x8000) != 0;
            bool isLeftCtrlDown = (GetAsyncKeyState(Keys.LControlKey) & 0x8000) != 0;
            bool isRightCtrlDown = (GetAsyncKeyState(Keys.RControlKey) & 0x8000) != 0;
            bool isLeftWinDown = (GetAsyncKeyState(Keys.LWin) & 0x8000) != 0;
            bool isRightWinDown = (GetAsyncKeyState(Keys.RWin) & 0x8000) != 0;


            if (isLeftShiftDown)
            {
                FlashButton(LShiftBtn); 
                RecordKeystroke("Left Shift", Keys.LShiftKey);
            }
            else if (isRightShiftDown) 
            {
                FlashButton(RShiftBtn);
                RecordKeystroke("Right Shift", Keys.RShiftKey);
            }
            else if (isLeftCtrlDown)
            {
                FlashButton(LCtrlBtn);
                RecordKeystroke("Left Ctrl", Keys.LControlKey);
            }
            //else if (isRightCtrlDown)
            //{
            //    FlashButton(RCtrlBtn);
            //    RecordKeystroke("Right Ctrl", Keys.RControlKey);
            //}
            //else if (isLeftWinDown)
            //{
            //    FlashButton(LWinBtn);
            //    RecordKeystroke("Left Win", Keys.LWin);
            //}
            //else if (isRightWinDown)
            //{
            //    FlashButton(RWinBtn);
            //    RecordKeystroke("Right Win", Keys.RWin);
            //}

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
        private readonly Dictionary<Keys, string> keyNameMap = new Dictionary<Keys, string>
        {
            { Keys.Escape, "ESC" },
            { Keys.Oem3, "Backtick" },
            { Keys.LShiftKey, "Left Shift" }
        };

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

            //keyButtonMap[Keys.C] = CBtn;
            // ... Add all keys y
            foreach (var key in keyButtonMap.Keys)
            {
                firstPressKeys.Add(key);
            }
        }
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
            firstPressKeys.Remove(key);
        }

        private void KBTest_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void button49_Click(object sender, EventArgs e)
        {

        }

        private void button50_Click(object sender, EventArgs e)
        {

        }

        private void button51_Click(object sender, EventArgs e)
        {

        }
    }
}