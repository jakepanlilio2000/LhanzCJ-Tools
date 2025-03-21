namespace LhanzCJ_Installer
{
    partial class ChoiceDialogForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnAsIs;
        private System.Windows.Forms.Button btnAddSoftware;
        private System.Windows.Forms.Label lblMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnAsIs = new System.Windows.Forms.Button();
            this.btnAddSoftware = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblMessage
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(12, 9);
            this.lblMessage.Size = new System.Drawing.Size(260, 13);
            this.lblMessage.Text = "Would you like to add additional software?";

            // btnAsIs
            this.btnAsIs.Location = new System.Drawing.Point(15, 40);
            this.btnAsIs.Size = new System.Drawing.Size(100, 30);
            this.btnAsIs.Text = "As-Is";
            this.btnAsIs.UseVisualStyleBackColor = true;
            this.btnAsIs.Click += new System.EventHandler(this.btnAsIs_Click);

            // btnAddSoftware
            this.btnAddSoftware.Location = new System.Drawing.Point(130, 40);
            this.btnAddSoftware.Size = new System.Drawing.Size(100, 30);
            this.btnAddSoftware.Text = "Add Software";
            this.btnAddSoftware.UseVisualStyleBackColor = true;
            this.btnAddSoftware.Click += new System.EventHandler(this.btnAddSoftware_Click);

            // ChoiceDialogForm
            this.ClientSize = new System.Drawing.Size(250, 85);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnAsIs);
            this.Controls.Add(this.btnAddSoftware);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose an Option";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}