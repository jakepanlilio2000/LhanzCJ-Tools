using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO.Compression;


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

        private string downloadPath = "path/dls/";

        private void button5_Click(object sender, EventArgs e)
        {
            InstallPrograms();
        }

        private void InstallPrograms()
        {
            richTextBox1.Clear();

            List<InstallStep> steps = new List<InstallStep>
            {
                new InstallStep("7zip", "7zip.exe", "https://www.7-zip.org/a/7z2409-x64.exe", "7zip.exe /S"),
                new InstallStep("Google Chrome", "chrome.exe", "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7B0A0703C5-744E-12EE-BA2C-574FA3D84FE6%7D%26lang%3Den%26browser%3D3%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-statsdef_1%26installdataindex%3Dempty/chrome/install/ChromeStandaloneSetup64.exe", "chrome.exe /silent /install"),
                new InstallStep("Zoom", "zoom.exe", "https://zoom.us/client/6.3.11.60501/ZoomInstallerFull.exe", "zoom.exe /silent"),
                new InstallStep("Acrobat Reader", "acrobat.exe", "https://download1532.mediafire.com/q9cgujl7cptg_M-F2lIH8MPr19TWj8YwPRZo9RACH3uXnzqr5DWDuOGdrG-_-WHfe-ZFjwF5gMAjyT-tAW573ee8hzVmaUtCtZWHGrJPeaXGRTA3aHPPLNas8nwAE2lX8IDsWVY5inl9V4eINa4Hw5ghhfryCqh08mMXCWw6fgn8/a06rljgf25fri7r/Reader_en_install.exe", "acrobat.exe /sAll /Quiet /sAll /rs /msi EULA_ACCEPT=YES"),
                new InstallStep("VLC", "vlc.exe", "https://get.videolan.org/vlc/3.0.21/win64/vlc-3.0.21-win64.exe", "vlc.exe /S"),
                new InstallStep("DirectX", "directx.exe", "https://download.microsoft.com/download/8/4/a/84a35bf1-dafe-4ae8-82af-ad2ae20b6b14/directx_Jun2010_redist.exe", "directx.exe /Q /T:C:\\DirectX"),
                new InstallStep("VCRedistAIO", "VCRedistAIO.zip", "https://sg1-dl.techpowerup.com/files/mcNKq_1or8HB3MOpyO_GBQ/1741034261/Visual-C-Runtimes-All-in-One-Nov-2024.zip", ""),
            };

            progressBar1.Maximum = steps.Count * 2;  // Each step has download + install
            progressBar1.Value = 0;

            Thread thread = new Thread(() =>
            {
                foreach (var step in steps)
                {
                    string filePath = Path.Combine(downloadPath, step.FileName);

                    Directory.CreateDirectory(downloadPath);

                    if (!File.Exists(filePath))
                    {
                        AppendText($"{step.Name}: Downloading...\n");
                        if (DownloadFile(step.DownloadUrl, filePath))
                        {
                            AppendText($"{step.Name}: Downloaded successfully.\n");
                        }
                        else
                        {
                            AppendText($"{step.Name}: Download failed.\n");
                            continue;
                        }
                    }
                    else
                    {
                        AppendText($"{step.Name}: Already downloaded. Skipping download.\n");
                    }
                    UpdateProgressBar();

                    if (!IsProgramInstalled(step.Name))
                    {
                        AppendText($"{step.Name}: Installing silently...\n");
                        bool installed = false;

                        if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            installed = InstallFromZip(filePath);
                        }
                        else
                        {
                            installed = RunInstaller(filePath, step.InstallArgs);
                        }

                        if (installed)
                        {
                            AppendText($"{step.Name}: Installed successfully.\n");
                        }
                        else
                        {
                            AppendText($"{step.Name}: Installation failed.\n");
                        }
                    }
                    else
                    {
                        AppendText($"{step.Name}: Already installed. Skipping installation.\n");
                    }
                    UpdateProgressBar();
                }

                AppendText("All installations completed.\n");
            });

            thread.Start();
        }

        private bool DownloadFile(string url, string destination)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        int percent = e.ProgressPercentage;
                        AppendOrUpdateText($"Downloading {Path.GetFileName(destination)}: {percent}%\n", destination);
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        AppendOrUpdateText($"Downloading {Path.GetFileName(destination)}: Completed\n", destination);
                    };

                    client.DownloadFileAsync(new Uri(url), destination);

                    while (client.IsBusy)
                    {
                        Thread.Sleep(100); 
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Download error: {ex.Message}\n");
                return false;
            }
        }

        private Dictionary<string, int> fileProgressLines = new Dictionary<string, int>();

        private void AppendOrUpdateText(string text, string filePath)
        {
            Invoke(new Action(() =>
            {
                string fileName = Path.GetFileName(filePath);

                if (!fileProgressLines.ContainsKey(fileName))
                {
                    // Add new line and remember its index
                    richTextBox1.AppendText(text);
                    fileProgressLines[fileName] = richTextBox1.Lines.Length - 1;
                }
                else
                {
                    // Update existing line
                    int lineIndex = fileProgressLines[fileName];
                    string[] lines = richTextBox1.Lines;
                    lines[lineIndex] = text.Trim();

                    richTextBox1.Lines = lines;
                }

                // Scroll to bottom
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }));
        }

        private bool RunInstaller(string filePath, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Installer error: {ex.Message}\n");
                return false;
            }
        }
        private bool InstallFromZip(string zipFilePath)
        {
            try
            {
                string extractPath = Path.Combine(downloadPath, Path.GetFileNameWithoutExtension(zipFilePath));

                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }

                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                AppendText($"Extracted to {extractPath}\n");

                string installBatPath = Path.Combine(extractPath, "install_all.bat");

                if (File.Exists(installBatPath))
                {
                    string output = RunBatchFileWithOutput(installBatPath);

                    if (output.IndexOf("Installation completed successfully", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        AppendText($"install.bat reports successful installation.\n");
                        return true;
                    }
                    else
                    {
                        AppendText($"install.bat did not indicate successful installation. Output:\n{output}\n");
                        return false;
                    }
                }
                else
                {
                    AppendText($"No install.bat found in {extractPath}\n");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Unzip or install.bat execution error: {ex.Message}\n");
                return false;
            }
        }

        private string RunBatchFileWithOutput(string batchFilePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{batchFilePath}\"",
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

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        AppendText($"install.bat error: {error}\n");
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error running install.bat: {ex.Message}\n");
                return string.Empty;
            }
        }

        private void AppendText(string text)
        {
            Invoke(new Action(() =>
            {
                richTextBox1.AppendText(text);
            }));
        }

        private void UpdateProgressBar()
        {
            Invoke(new Action(() =>
            {
                progressBar1.Value += 1;
            }));
        }

        private bool IsProgramInstalled(string appName)
        {
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                if (key != null)
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            string displayName = subkey.GetValue("DisplayName") as string;
                            if (!string.IsNullOrEmpty(displayName) && displayName.ToLower().Contains(appName.ToLower()))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private class InstallStep
        {
            public string Name { get; }
            public string FileName { get; }
            public string DownloadUrl { get; }
            public string InstallArgs { get; }

            public InstallStep(string name, string fileName, string downloadUrl, string installArgs)
            {
                Name = name;
                FileName = fileName;
                DownloadUrl = downloadUrl;
                InstallArgs = installArgs;
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

            try
            {

                string cameraPath = @"C:\Windows\System32\WindowsCamera.exe";

                if (System.IO.File.Exists(cameraPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = cameraPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    DialogResult result = MessageBox.Show(
                        "Camera App isn't installed. Open Camera Tester instead?",
                        "Camera App Missing",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.Yes)
                    {

                        CamTest camTest = new CamTest();
                        camTest.Show();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while trying to open the Camera app.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                CamTest camTest = new CamTest();
                camTest.Show();
            }

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
