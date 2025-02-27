using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
        }
        public void initVars()
        {
            SNDate.MinDate = new DateTime(2025, 1, 1);
            SNDate.MaxDate = DateTime.Today;
        }
        private void KeyType_ItemCheck(object sender, ItemCheckEventArgs e)
        { 
            this.BeginInvoke((MethodInvoker)delegate
            {
                UpdateComboboxStates();
            });
        }
        private void UpdateComboboxStates()
        {
            OfficeVer.Enabled = KeyType.GetItemChecked(0);
            WinVer.Enabled = KeyType.GetItemChecked(1);
            if (KeyType.GetItemChecked(0))
            {
                OfficeVer.Enabled = true;
                OLK.Enabled = true;
                email.Enabled = true;
                pw.Enabled = true;
            }
            else
            {
                OfficeVer.Enabled = false;
                OLK.Enabled = false;
                email.Enabled = false;
                pw.Enabled = false;
            }
            if (KeyType.GetItemChecked(1))
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
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(UModel.Text) ||
                string.IsNullOrWhiteSpace(SNumber.Text))
            {
                return false; 
            }

            bool officeChecked = KeyType.GetItemChecked(0);
            bool windowsChecked = KeyType.GetItemChecked(1);

            if (!officeChecked && !windowsChecked)
            {
                
            }

            if (officeChecked)
            {
                if (OfficeVer.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(OLK.Text) ||
                    string.IsNullOrWhiteSpace(email.Text) ||
                    string.IsNullOrWhiteSpace(pw.Text))
                {
                    return false; 
                }
            }

            if (windowsChecked)
            {
                if (WinVer.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(WLK.Text))
                {
                    return false; 
                }
            }

 
            return true;
        }

        private void InputFields_Changed(object sender, EventArgs e)
        {
            RecordSN_Load();
            doneBtn.Enabled = ValidateInputs();
        }
        private void doneBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Please fill in all required fields before proceeding.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            RecordSN_Load();
            string filePath = System.IO.Path.Combine(Application.StartupPath, "Records.txt");

            try
            {
                System.IO.File.AppendAllText(filePath, SNOutput.Text + Environment.NewLine);

                MessageBox.Show("Record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save the record.\n\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RecordSN_Load()
        {
            String umodel_t = UModel.Text;
            String snum_t = SNumber.Text;
            String date_t = SNDate.Value.ToString("dd/MM/yyyy");

            String OfficeEdition = "N/A";
            String WindowsEdition = "N/A";
            String OffKey = "N/A";
            String WinKey = "N/A";
            String Email = "N/A";
            String Password = "N/A";

            bool officeChecked = KeyType.GetItemChecked(0);
            bool windowsChecked = KeyType.GetItemChecked(1);

            int checkedCount = (officeChecked ? 1 : 0) + (windowsChecked ? 1 : 0);

            if (officeChecked)
            {
                OfficeEdition = OfficeVer.SelectedItem != null ? OfficeVer.SelectedItem.ToString() : "N/A";
                OffKey = OLK.Text;
                Email = email.Text;
                Password = pw.Text;

                if (checkedCount == 1)
                {
                    WindowsEdition = "N/A";
                    WinKey = "N/A";
                }
            }

            if (windowsChecked)
            {
                WindowsEdition = WinVer.SelectedItem != null ? WinVer.SelectedItem.ToString() : "N/A";
                WinKey = WLK.Text;

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

            SNOutput.Text = "-----------------------------------\n" +
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
