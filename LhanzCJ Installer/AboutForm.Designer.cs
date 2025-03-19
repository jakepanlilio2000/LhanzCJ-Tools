namespace LhanzCJ_Installer
{

    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.linkLabelFacebook = new System.Windows.Forms.LinkLabel();
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.linkLabelLinkedIn = new System.Windows.Forms.LinkLabel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkLabelFacebook
            // 
            this.linkLabelFacebook.AutoSize = true;
            this.linkLabelFacebook.Location = new System.Drawing.Point(13, 207);
            this.linkLabelFacebook.Name = "linkLabelFacebook";
            this.linkLabelFacebook.Size = new System.Drawing.Size(55, 13);
            this.linkLabelFacebook.TabIndex = 2;
            this.linkLabelFacebook.TabStop = true;
            this.linkLabelFacebook.Text = "Facebook";
            this.linkLabelFacebook.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFacebook_LinkClicked);
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.Location = new System.Drawing.Point(13, 225);
            this.linkLabelGitHub.Name = "linkLabelGitHub";
            this.linkLabelGitHub.Size = new System.Drawing.Size(40, 13);
            this.linkLabelGitHub.TabIndex = 2;
            this.linkLabelGitHub.TabStop = true;
            this.linkLabelGitHub.Text = "GitHub";
            this.linkLabelGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGitHub_LinkClicked);
            // 
            // linkLabelLinkedIn
            // 
            this.linkLabelLinkedIn.AutoSize = true;
            this.linkLabelLinkedIn.Location = new System.Drawing.Point(13, 242);
            this.linkLabelLinkedIn.Name = "linkLabelLinkedIn";
            this.linkLabelLinkedIn.Size = new System.Drawing.Size(48, 13);
            this.linkLabelLinkedIn.TabIndex = 3;
            this.linkLabelLinkedIn.TabStop = true;
            this.linkLabelLinkedIn.Text = "LinkedIn";
            this.linkLabelLinkedIn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLinkedIn_LinkClicked);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(276, 225);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(88, 33);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(346, 190);
            this.label1.TabIndex = 0;
            this.label1.Text = "Program: LhanzCJ Toolbox\n" +
              "Author: Jake Ashley Panlilio\n\n" +
              "This program was developed as a free tool with no \n" +
              "monetary transactions involved. It is designed to \n" +
              "assist technicians by streamlining their tasks,\n" +
              "enabling them to work more efficiently and \n" +
              "automate processes.\n\n" +
              "Follow me on:";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 273);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabelFacebook);
            this.Controls.Add(this.linkLabelGitHub);
            this.Controls.Add(this.linkLabelLinkedIn);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel linkLabelFacebook;
        private System.Windows.Forms.LinkLabel linkLabelGitHub;
        private System.Windows.Forms.LinkLabel linkLabelLinkedIn;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
    }
}