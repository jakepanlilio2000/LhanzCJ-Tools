using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class RecordSN : Form
    {

        public RecordSN()
        {
            InitializeComponent();
            initVars();
            UModel.TextChanged += InputFields_Changed;
            SNumber.TextChanged += InputFields_Changed;
            SNDate.ValueChanged += InputFields_Changed;
            KeyType.ItemCheck += KeyType_ItemCheck;
            OLK.TextChanged += InputFields_Changed;
            WLK.TextChanged += InputFields_Changed;
            OfficeVer.SelectedIndexChanged += InputFields_Changed;
            WinVer.SelectedIndexChanged += InputFields_Changed;
            email.TextChanged += InputFields_Changed;
            pw.TextChanged += InputFields_Changed;


            if (OfficeVer.Items.Count > 0)
            {
                OfficeVer.SelectedIndex = 0;
            }

            if (WinVer.Items.Count > 0)
            {
                WinVer.SelectedIndex = 0;
            }
        }
        public void initVars()
        {
            SNDate.MinDate = new DateTime(2025, 1, 1);
            SNDate.MaxDate = DateTime.Today;
        }
        private void KeyType_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                UpdateComboboxStates();
                RecordSN_Load();
                UpdateDoneButtonState();
            });
        }
        private void UpdateComboboxStates()
        {
            OfficeVer.Enabled = KeyType.GetItemChecked(1);
            WinVer.Enabled = KeyType.GetItemChecked(0);
            if (KeyType.GetItemChecked(1))
            {
                OfficeVer.Enabled = true;
                OLK.Enabled = true;
            }
            else
            {
                OfficeVer.Enabled = false;
                OLK.Enabled = false;
            }
            if (KeyType.GetItemChecked(0))
            {
                WinVer.Enabled = true;
                WLK.Enabled = true;
            }
            else
            {
                WinVer.Enabled = false;
                WLK.Enabled = false;
            }
        }



        private void InputFields_Changed(object sender, EventArgs e)
        {
            RecordSN_Load();
            UpdateDoneButtonState();
        }
        private bool IsKeyValid(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            string cleanKey = key.Replace("-", "");
            return cleanKey.Length == 25;
        }
        private void UpdateDoneButtonState()
        {
            doneBtn.Enabled = ValidateInputs();
        }
        private bool ValidateInputs()
        {
            bool isValid = true;
            errorProvider1.Clear();

            bool windowsChecked = KeyType.GetItemChecked(0);
            bool officeChecked = KeyType.GetItemChecked(1);

            int checkedCount = (windowsChecked ? 1 : 0) + (officeChecked ? 1 : 0);

            if (checkedCount == 0)
            {
                if (string.IsNullOrEmpty(UModel.Text))
                {
                    errorProvider1.SetError(UModel, "Unit model must not be empty.");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(SNumber.Text))
                {
                    errorProvider1.SetError(SNumber, "Serial number must not be empty.");
                    isValid = false;
                }

                return isValid;
            }

            if (windowsChecked)
            {
                if (WinVer.SelectedIndex == -1)
                {
                    errorProvider1.SetError(WinVer, "Windows version is required.");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(WLK.Text))
                {
                    errorProvider1.SetError(WLK, "Windows license key is required.");
                    isValid = false;
                }
                else if (!IsKeyValid(WLK.Text))
                {
                    errorProvider1.SetError(WLK, "Key must be exactly 25 characters.");
                    isValid = false;
                }
            }

            if (officeChecked)
            {

                if (OfficeVer.SelectedIndex == -1)
                {
                    errorProvider1.SetError(OfficeVer, "Office version is required.");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(OLK.Text))
                {
                    errorProvider1.SetError(OLK, "Office license key is required.");
                    isValid = false;
                }
                else if (!IsKeyValid(OLK.Text))
                {
                    errorProvider1.SetError(OLK, "Key must be exactly 25 characters.");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(email.Text))
                {
                    errorProvider1.SetError(email, "Email is required.");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(pw.Text))
                {
                    errorProvider1.SetError(pw, "Password is required.");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void doneBtn_Click(object sender, EventArgs e)
        {
            RecordSN_Load();

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string msAccountFilePath = Path.Combine(desktopPath, "MS Account.txt");
            string recordsFilePath = Path.Combine(Application.StartupPath, "Records.txt");

            try
            {
                if (File.Exists(msAccountFilePath))
                {
                    FileAttributes attributes = File.GetAttributes(msAccountFilePath);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(msAccountFilePath, attributes & ~FileAttributes.ReadOnly);
                    }
                }
                string msAccountContent = "Customer's Copy" + Environment.NewLine + SNOutput.Text;
                File.WriteAllText(msAccountFilePath, msAccountContent);
                File.SetAttributes(msAccountFilePath, File.GetAttributes(msAccountFilePath) | FileAttributes.ReadOnly);
                File.AppendAllText(recordsFilePath, SNOutput.Text + Environment.NewLine);

                Process.Start(new ProcessStartInfo()
                {
                    FileName = "notepad.exe",
                    Arguments = msAccountFilePath,
                    UseShellExecute = true
                });

                Process.Start(new ProcessStartInfo()
                {
                    FileName = "notepad.exe",
                    Arguments = recordsFilePath,
                    UseShellExecute = true
                });

                MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save the record.\n\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string FormatKey(string inputKey)
        {
            string cleanKey = inputKey.Replace("-", "").ToUpper();

            StringBuilder formattedKey = new StringBuilder();
            for (int i = 0; i < cleanKey.Length; i++)
            {
                formattedKey.Append(cleanKey[i]);
                if ((i + 1) % 5 == 0 && i != cleanKey.Length - 1)
                {
                    formattedKey.Append("-");
                }
            }

            return formattedKey.ToString();
        }



        private void RecordSN_Load()
        {
            const String DefaultKey = "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX";

            String iniEmail = "";
            String umodel_t = UModel.Text;
            String snum_t = SNumber.Text;
            String date_t = SNDate.Value.ToString("dd/MM/yyyy");

            String OfficeEdition = "N/A";
            String WindowsEdition = "N/A";
            String OffKey = DefaultKey;
            String WinKey = DefaultKey;
            String Email = "N/A";
            String Password = "N/A";
            String OffEmailEnabled = "Admin1122";
            String pSerial;


            if (snum_t.Length <= 7)
            {
                pSerial = snum_t;
            }
            else
            {
                pSerial = snum_t.Substring(snum_t.Length - 6);
            }

            bool officeChecked = KeyType.GetItemChecked(1);
            bool windowsChecked = KeyType.GetItemChecked(0);

            int checkedCount = (officeChecked ? 1 : 0) + (windowsChecked ? 1 : 0);

            if (officeChecked)
            {
                OfficeEdition = OfficeVer.SelectedItem != null ? OfficeVer.SelectedItem.ToString() : "N/A";
                OffKey = OLK.Text.Length > 0 ? FormatKey(OLK.Text) : DefaultKey;




                if (OfficeVer.SelectedItem != null && OfficeVer.SelectedItem.ToString() == "Office Home 2024")
                {
                    iniEmail = "office2024";
                }
                else if (OfficeVer.SelectedItem != null && OfficeVer.SelectedItem.ToString() == "Office Home and Student 2021")
                {
                    iniEmail = "office2021";
                }
                else
                {
                    iniEmail = "microsof365";
                }


                if (checkedCount == 1)
                {
                    WindowsEdition = "N/A";
                    WinKey = "N/A";
                }

                Email = iniEmail + pSerial + "@gmail.com";
                Password = OffEmailEnabled;
                pw.Text = OffEmailEnabled;
                email.Text = Email;



            }

            if (windowsChecked)
            {
                WindowsEdition = WinVer.SelectedItem != null ? WinVer.SelectedItem.ToString() : "N/A";
                WinKey = WLK.Text.Length > 0 ? FormatKey(WLK.Text) : DefaultKey;

                if (checkedCount == 1)
                {
                    OfficeEdition = "N/A";
                    OffKey = "N/A";
                    Email = "N/A";
                    Password = "N/A";
                }

            }

            if (checkedCount == 0)
            {
                OfficeEdition = "N/A";
                WindowsEdition = "N/A";
                OffKey = "N/A";
                WinKey = "N/A";
                Email = "N/A";
                Password = "N/A";
            }


            SNOutput.Text = "-------------0000000000------------\n" +
                            "Date: " + date_t + "\n" +
                            "Unit Number: " + umodel_t + "\n" +
                            "Serial Number: " + snum_t + "\n" +
                            "\n" +
                            "Office Edition: " + OfficeEdition + "\n" +
                            "Office Key: " + OffKey + "\n" +
                            "Email: " + Email + "\n" +
                            "Password: " + Password + "\n" +
                            "\n" +
                            "Windows Edition: " + WindowsEdition + "\n" +
                            "Windows Key: " + WinKey + "\n" +
                            "-----------------------------------\n" +
                            "@";
        }

    }
}
