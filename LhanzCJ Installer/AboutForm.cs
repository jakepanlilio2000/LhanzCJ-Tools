using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LhanzCJ_Installer
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            linkLabelFacebook.Text = "Facebook";
            linkLabelGitHub.Text = "GitHub";
            linkLabelLinkedIn.Text = "LinkedIn";

            linkLabelFacebook.Links.Add(0, linkLabelFacebook.Text.Length, "https://www.facebook.com/jakepanlilio2000");
            linkLabelGitHub.Links.Add(0, linkLabelGitHub.Text.Length, "https://github.com/jakepanlilio2000");
            linkLabelLinkedIn.Links.Add(0, linkLabelLinkedIn.Text.Length, "https://www.linkedin.com/in/jakepanlilio2000/");
        }
        private void linkLabelFacebook_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    => LinkLabel_LinkClicked(sender, e);

        private void linkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => LinkLabel_LinkClicked(sender, e);

        private void linkLabelLinkedIn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            => LinkLabel_LinkClicked(sender, e);

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Link.LinkData.ToString(),
                UseShellExecute = true
            });
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
