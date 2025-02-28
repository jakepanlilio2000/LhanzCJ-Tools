using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace LhanzCJ_Installer
{

    public partial class LhanzCJ : Form
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetFileAttributes(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        [DllImport("user32.dll")]
        private static extern int SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        public LhanzCJ()
        {
            InitializeComponent();
            CheckAdminPrivileges();
        }
        private void CheckAdminPrivileges()
        {
            bool isAdmin = IsAdministrator();

            button5.Enabled = isAdmin; 
            label1.Visible = !isAdmin;
            button7.Enabled = isAdmin;

        }

        private bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL desk.cpl,,0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Desktop Icon Settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IntPtr oldValue = IntPtr.Zero;
            bool redirectionDisabled = false;

            try
            {

                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    redirectionDisabled = Wow64DisableWow64FsRedirection(ref oldValue);
                }


                string exePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "OptionalFeatures.exe"
                );
                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Windows Features: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (redirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(oldValue);
                }
            }
        }





        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", @"\\technical");
            }
            catch (Exception ex)
            {
                richTextBox1.Clear();
                richTextBox1.Text = "Error: The network path was not found.\n\n" +
                                    "Possible Fix:\n" +
                                    "1. Enable SMB 1.0/CIFS File Sharing Support.\n" +
                                    "2. Open Windows Features\n" +
                                    "3. Check 'SMB 1.0/CIFS File Sharing Support' and click OK.\n" +
                                    "4. Restart your computer.\n\n" +
                                    $"Error: {ex.Message}";
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("ms-settings:windowsupdate");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Windows Update: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            InstallPrograms();
        }

        private void InstallPrograms()
        {
            richTextBox1.Clear();
            string[] commands = new string[]
                {
                    "echo Checking if App Installer is installed...",

                    "where winget >nul 2>nul && echo App Installer is already installed. || (echo Installing App Installer... && powershell -Command \"$ProgressPreference = 'SilentlyContinue'; irm https://github.com/asheroto/winget-install/releases/latest/download/winget-install.ps1 | iex\")",

                    "echo Installing 7zip...",
                    "winget install -e --id 7zip.7zip --silent --accept-source-agreements",

                    "echo Installing Google Chrome...",
                    "winget install -e --id Google.Chrome --silent --accept-source-agreements",

                    "echo Installing Zoom...",
                    "winget install -e --id Zoom.Zoom --silent --accept-source-agreements",

                    "echo Installing Acrobat Reader...",
                    "winget install -e --id Adobe.Acrobat.Reader.64-bit --silent --accept-source-agreements",

                    "echo Installing VLC...",
                    "winget install -e --id VideoLAN.VLC --silent --accept-source-agreements",

                    "echo Installing Visual C++ Redistributables...",
                    "winget install -e --id Microsoft.VCRedist.2005.x86 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2005.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2008.x86 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2008.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2010.x86 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2010.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2012.x86 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2012.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2013.x86 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2013.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2015+.x64 --silent --accept-source-agreements",
                    "winget install -e --id Microsoft.VCRedist.2015+.x86 --silent --accept-source-agreements",

                    "echo Installing DirectX...",
                    "if exist directx.exe (",
                    "    echo DirectX installer already exists, skipping download.",
                    ") else (",
                    "    echo Downloading DirectX installer...",
                    "    curl -o directx.exe https://download.microsoft.com/download/8/4/a/84a35bf1-dafe-4ae8-82af-ad2ae20b6b14/directx_Jun2010_redist.exe",
                    ")",

                    "if exist C:\\DirectX (",
                    "    rmdir /s /q C:\\DirectX",
                    ")",
                    "mkdir C:\\DirectX",

                    "directx.exe /Q /T:C:\\DirectX",
                    "C:\\DirectX\\DXSETUP.exe /silent && echo DirectX installation successful! || echo DirectX installation failed.",

                    "echo.",
                    "echo Installation completed successfully."
                };



            progressBar1.Maximum = commands.Length;
            progressBar1.Value = 0;

            Thread thread = new Thread(() =>
            {
                foreach (string command in commands)
                {
                    string output = RunCommand(command);

                    Invoke(new Action(() =>
                    {
                        UpdateRichTextBox(command, output);
                        progressBar1.Value += 1;
                    }));
                }
            });

            thread.Start();
        }

        private string RunCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    return !string.IsNullOrWhiteSpace(error) ? error : output;
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        private void UpdateRichTextBox(string command, string output)
        {
            if (command.Contains("winget install"))
            {
                string appName = command.Replace("winget install -e --id ", "").Replace("--silent", "").Replace("--accept-source-agreements", "").Trim();

                if (output.Contains("Successfully installed"))
                {
                    richTextBox1.AppendText($"{appName} installed successfully.\n\n");
                }
                else if (output.Contains("already installed"))
                {
                    richTextBox1.AppendText($"{appName} is already installed.\n\n");
                }
                else
                {
                    richTextBox1.AppendText($"Failed to install {appName}.\n\n");
                }
            }
            else if (command.Contains("echo"))
            {
                richTextBox1.AppendText(output + "\n"); 
            }
        }



        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.office.com/setup/", 
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the website: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            IntPtr oldValue = IntPtr.Zero;
            bool redirectionDisabled = false;

            try
            {

                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    redirectionDisabled = Wow64DisableWow64FsRedirection(ref oldValue);
                }


                string exePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "slui.exe"
                );
                Process.Start(exePath, "3");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open activation program: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
          
                if (redirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(oldValue);
                }
            }
        }

        private void dxdiag_Click(object sender, EventArgs e)
        {
            IntPtr oldValue = IntPtr.Zero;
            bool redirectionDisabled = false;

            try
            {
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    redirectionDisabled = Wow64DisableWow64FsRedirection(ref oldValue);
                }

                string exePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "dxdiag.exe"
                );
                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open program: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (redirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(oldValue);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CamTest camTest = new CamTest();
            camTest.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MicTest micTest = new MicTest(); 
            micTest.Show(); 
        }

        private void button10_Click(object sender, EventArgs e)
        {
            IntPtr oldValue = IntPtr.Zero;
            bool redirectionDisabled = false;

            try
            {
                if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                {
                    redirectionDisabled = Wow64DisableWow64FsRedirection(ref oldValue);
                }

                string exePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.System),
                    "devmgmt.msc"
                );
                Process.Start(exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open program: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (redirectionDisabled)
                {
                    Wow64RevertWow64FsRedirection(oldValue);
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            KeyboardTester KBTest = new KeyboardTester();
            KBTest.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MonitorTester LCDTest = new MonitorTester();
            LCDTest.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://accounts.google.com/AccountChooser/signinchooser?service=mail&continue=https://mail.google.com/mail/&flowName=GlifWebSignIn&flowEntry=AccountChooser&ec=asw-gmail-globalnav-signin",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open the website: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SoundCheck sCheck = new SoundCheck();
            sCheck.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            RecordSN recordSN = new RecordSN();
            recordSN.Show();
        }
    }
}
