using System;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    partial class SoundCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SoundCheck));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.simultaneousRadio = new System.Windows.Forms.RadioButton();
            this.alternatingRadio = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.channels7Radio = new System.Windows.Forms.RadioButton();
            this.channels5Radio = new System.Windows.Forms.RadioButton();
            this.channels3Radio = new System.Windows.Forms.RadioButton();
            this.channels2Radio = new System.Windows.Forms.RadioButton();
            this.playStopButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.simultaneousRadio);
            this.groupBox1.Controls.Add(this.alternatingRadio);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 87);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mode";
            // 
            // simultaneousRadio
            // 
            this.simultaneousRadio.AutoSize = true;
            this.simultaneousRadio.Location = new System.Drawing.Point(5, 36);
            this.simultaneousRadio.Name = "simultaneousRadio";
            this.simultaneousRadio.Size = new System.Drawing.Size(88, 17);
            this.simultaneousRadio.TabIndex = 1;
            this.simultaneousRadio.TabStop = true;
            this.simultaneousRadio.Text = "Simultaneous";
            this.simultaneousRadio.UseVisualStyleBackColor = true;
            // 
            // alternatingRadio
            // 
            this.alternatingRadio.AutoSize = true;
            this.alternatingRadio.Location = new System.Drawing.Point(5, 15);
            this.alternatingRadio.Name = "alternatingRadio";
            this.alternatingRadio.Size = new System.Drawing.Size(75, 17);
            this.alternatingRadio.TabIndex = 0;
            this.alternatingRadio.TabStop = true;
            this.alternatingRadio.Text = "Alternating";
            this.alternatingRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.channels7Radio);
            this.groupBox2.Controls.Add(this.channels5Radio);
            this.groupBox2.Controls.Add(this.channels3Radio);
            this.groupBox2.Controls.Add(this.channels2Radio);
            this.groupBox2.Location = new System.Drawing.Point(187, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 87);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Channels";
            // 
            // channels7Radio
            // 
            this.channels7Radio.AutoSize = true;
            this.channels7Radio.Location = new System.Drawing.Point(88, 36);
            this.channels7Radio.Name = "channels7Radio";
            this.channels7Radio.Size = new System.Drawing.Size(31, 17);
            this.channels7Radio.TabIndex = 3;
            this.channels7Radio.Text = "7";
            this.channels7Radio.UseVisualStyleBackColor = true;
            // 
            // channels5Radio
            // 
            this.channels5Radio.AutoSize = true;
            this.channels5Radio.Location = new System.Drawing.Point(88, 15);
            this.channels5Radio.Name = "channels5Radio";
            this.channels5Radio.Size = new System.Drawing.Size(31, 17);
            this.channels5Radio.TabIndex = 2;
            this.channels5Radio.Text = "5";
            this.channels5Radio.UseVisualStyleBackColor = true;
            // 
            // channels3Radio
            // 
            this.channels3Radio.AutoSize = true;
            this.channels3Radio.Location = new System.Drawing.Point(5, 36);
            this.channels3Radio.Name = "channels3Radio";
            this.channels3Radio.Size = new System.Drawing.Size(31, 17);
            this.channels3Radio.TabIndex = 1;
            this.channels3Radio.Text = "3";
            this.channels3Radio.UseVisualStyleBackColor = true;
            // 
            // channels2Radio
            // 
            this.channels2Radio.AutoSize = true;
            this.channels2Radio.Location = new System.Drawing.Point(5, 15);
            this.channels2Radio.Name = "channels2Radio";
            this.channels2Radio.Size = new System.Drawing.Size(31, 17);
            this.channels2Radio.TabIndex = 0;
            this.channels2Radio.Text = "2";
            this.channels2Radio.UseVisualStyleBackColor = true;
            // 
            // playStopButton
            // 
            this.playStopButton.Location = new System.Drawing.Point(10, 102);
            this.playStopButton.Name = "playStopButton";
            this.playStopButton.Size = new System.Drawing.Size(348, 20);
            this.playStopButton.TabIndex = 2;
            this.playStopButton.Text = "Play";
            this.playStopButton.UseVisualStyleBackColor = true;
            this.playStopButton.Click += new System.EventHandler(this.playStopButton_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 131);
            this.Controls.Add(this.playStopButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form3";
            this.Text = "Audio Player";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion


        private GroupBox groupBox1;
        private RadioButton simultaneousRadio;
        private RadioButton alternatingRadio;
        private GroupBox groupBox2;
        private RadioButton channels7Radio;
        private RadioButton channels5Radio;
        private RadioButton channels3Radio;
        private RadioButton channels2Radio;
        private Button playStopButton;
    }
}