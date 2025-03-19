using System;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class WiFiInputForm : Form
    {
        public string SSID { get; private set; }
        public string Password { get; private set; }

        public WiFiInputForm()
        {
            InitializeComponent(); // Ensure components are initialized
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSSID.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both SSID and Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SSID = txtSSID.Text;
            Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
