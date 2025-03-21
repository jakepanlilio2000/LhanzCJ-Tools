using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class AddSoftwareForm : Form
    {
        public string SoftwareName { get; private set; }
        public string SoftwareUrl { get; private set; }

        public AddSoftwareForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SoftwareName = txtName.Text.Trim();
            SoftwareUrl = txtUrl.Text.Trim();

            if (string.IsNullOrEmpty(SoftwareName) || string.IsNullOrEmpty(SoftwareUrl))
            {
                MessageBox.Show("Please enter both name and URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidUrl(SoftwareUrl))
            {
                MessageBox.Show("Please enter a valid URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool IsValidUrl(string url)
        {
            string pattern = @"^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$";
            return Regex.IsMatch(url, pattern);
        }
    }
}
