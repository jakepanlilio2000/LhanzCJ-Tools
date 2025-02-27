namespace LhanzCJ_Installer
{
    partial class KeyboardTester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblKeystrokes;
        private System.Windows.Forms.Panel panelKeyboard;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Method required for Designer support – do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyboardTester));
            this.lblKeystrokes = new System.Windows.Forms.Label();
            this.panelKeyboard = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblKeystrokes
            // 
            this.lblKeystrokes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKeystrokes.Font = new System.Drawing.Font("Consolas", 9F);
            this.lblKeystrokes.Location = new System.Drawing.Point(0, 0);
            this.lblKeystrokes.Name = "lblKeystrokes";
            this.lblKeystrokes.Padding = new System.Windows.Forms.Padding(5);
            this.lblKeystrokes.Size = new System.Drawing.Size(578, 60);
            this.lblKeystrokes.TabIndex = 0;
            this.lblKeystrokes.Text = "Keystrokes will be displayed here...";
            // 
            // panelKeyboard
            // 
            this.panelKeyboard.AutoSize = true;
            this.panelKeyboard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelKeyboard.Location = new System.Drawing.Point(0, 60);
            this.panelKeyboard.Name = "panelKeyboard";
            this.panelKeyboard.Size = new System.Drawing.Size(578, 97);
            this.panelKeyboard.TabIndex = 1;
            // 
            // KeyboardTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(578, 157);
            this.Controls.Add(this.panelKeyboard);
            this.Controls.Add(this.lblKeystrokes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "KeyboardTester";
            this.Text = "Keyboard Tester";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
