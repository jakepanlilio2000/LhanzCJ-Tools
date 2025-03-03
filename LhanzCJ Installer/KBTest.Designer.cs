namespace LhanzCJ_Installer
{
    partial class KBTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblKeystrokes = new System.Windows.Forms.Label();
            this.EscBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.RShiftBtn = new System.Windows.Forms.Button();
            this.LShiftBtn = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.LCtrlBtn = new System.Windows.Forms.Button();
            this.CapsBtn = new System.Windows.Forms.Button();
            this.TabBtn = new System.Windows.Forms.Button();
            this.AposBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblKeystrokes
            // 
            this.lblKeystrokes.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblKeystrokes.Location = new System.Drawing.Point(0, 0);
            this.lblKeystrokes.Name = "lblKeystrokes";
            this.lblKeystrokes.Size = new System.Drawing.Size(1133, 82);
            this.lblKeystrokes.TabIndex = 0;
            this.lblKeystrokes.Text = "Keystrokes will be displayed here...";
            // 
            // EscBtn
            // 
            this.EscBtn.Location = new System.Drawing.Point(0, 0);
            this.EscBtn.Name = "EscBtn";
            this.EscBtn.Size = new System.Drawing.Size(50, 50);
            this.EscBtn.TabIndex = 1;
            this.EscBtn.TabStop = false;
            this.EscBtn.Text = "ESC";
            this.EscBtn.UseVisualStyleBackColor = true;
            
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RShiftBtn);
            this.panel1.Controls.Add(this.LShiftBtn);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.LCtrlBtn);
            this.panel1.Controls.Add(this.CapsBtn);
            this.panel1.Controls.Add(this.TabBtn);
            this.panel1.Controls.Add(this.AposBtn);
            this.panel1.Controls.Add(this.EscBtn);
            this.panel1.Location = new System.Drawing.Point(0, 85);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1130, 331);
            this.panel1.TabIndex = 3;
            // 
            // RShiftBtn
            // 
            this.RShiftBtn.Location = new System.Drawing.Point(673, 224);
            this.RShiftBtn.Name = "RShiftBtn";
            this.RShiftBtn.Size = new System.Drawing.Size(100, 50);
            this.RShiftBtn.TabIndex = 8;
            this.RShiftBtn.TabStop = false;
            this.RShiftBtn.Text = "Shift";
            this.RShiftBtn.UseVisualStyleBackColor = true;
            // 
            // LShiftBtn
            // 
            this.LShiftBtn.Location = new System.Drawing.Point(0, 224);
            this.LShiftBtn.Name = "LShiftBtn";
            this.LShiftBtn.Size = new System.Drawing.Size(100, 50);
            this.LShiftBtn.TabIndex = 7;
            this.LShiftBtn.TabStop = false;
            this.LShiftBtn.Text = "Shift";
            this.LShiftBtn.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(50, 56);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(50, 50);
            this.button4.TabIndex = 6;
            this.button4.TabStop = false;
            this.button4.Text = "1";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // LCtrlBtn
            // 
            this.LCtrlBtn.Location = new System.Drawing.Point(0, 280);
            this.LCtrlBtn.Name = "LCtrlBtn";
            this.LCtrlBtn.Size = new System.Drawing.Size(50, 50);
            this.LCtrlBtn.TabIndex = 5;
            this.LCtrlBtn.TabStop = false;
            this.LCtrlBtn.Text = "Ctrl";
            this.LCtrlBtn.UseVisualStyleBackColor = true;
            // 
            // CapsBtn
            // 
            this.CapsBtn.Location = new System.Drawing.Point(0, 168);
            this.CapsBtn.Name = "CapsBtn";
            this.CapsBtn.Size = new System.Drawing.Size(150, 50);
            this.CapsBtn.TabIndex = 4;
            this.CapsBtn.TabStop = false;
            this.CapsBtn.Text = "Caps Lock";
            this.CapsBtn.UseVisualStyleBackColor = true;
            // 
            // TabBtn
            // 
            this.TabBtn.Location = new System.Drawing.Point(0, 112);
            this.TabBtn.Name = "TabBtn";
            this.TabBtn.Size = new System.Drawing.Size(100, 50);
            this.TabBtn.TabIndex = 3;
            this.TabBtn.TabStop = false;
            this.TabBtn.Text = "Tab";
            this.TabBtn.UseVisualStyleBackColor = true;
            // 
            // AposBtn
            // 
            this.AposBtn.Location = new System.Drawing.Point(0, 56);
            this.AposBtn.Name = "AposBtn";
            this.AposBtn.Size = new System.Drawing.Size(50, 50);
            this.AposBtn.TabIndex = 2;
            this.AposBtn.TabStop = false;
            this.AposBtn.Text = "`\r\n~";
            this.AposBtn.UseVisualStyleBackColor = true;
            // 
            // KBTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 415);
            this.Controls.Add(this.lblKeystrokes);
            this.Controls.Add(this.panel1);
            this.Name = "KBTest";
            this.Text = "KBTest";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblKeystrokes;
        private System.Windows.Forms.Button EscBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AposBtn;
        private System.Windows.Forms.Button LShiftBtn;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button LCtrlBtn;
        private System.Windows.Forms.Button CapsBtn;
        private System.Windows.Forms.Button TabBtn;
        private System.Windows.Forms.Button RShiftBtn;
    }
}