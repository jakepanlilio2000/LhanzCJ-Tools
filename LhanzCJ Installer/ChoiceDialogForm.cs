using System;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class ChoiceDialogForm : Form
    {
        public bool AddAdditionalSoftware { get; private set; }

        public ChoiceDialogForm()
        {
            InitializeComponent();
        }

        private void btnAsIs_Click(object sender, EventArgs e)
        {
            AddAdditionalSoftware = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAddSoftware_Click(object sender, EventArgs e)
        {
            AddAdditionalSoftware = true;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
