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
        private string downloadPath = "packages\\";
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
            button5.Enabled = false;
            InstallPrograms();
        }

        private void InstallPrograms()
        {
            richTextBox1.Clear();

            List<InstallStep> steps = new List<InstallStep>
            {
                new InstallStep(".Net 4.8.1", "Net4.8.1.exe", "https://go.microsoft.com/fwlink/?linkid=2203305", "/q /norestart"),
                new InstallStep("DirectX", "directx.exe", "https://download.microsoft.com/download/8/4/a/84a35bf1-dafe-4ae8-82af-ad2ae20b6b14/directx_Jun2010_redist.exe", "directx.exe /Q /T:C:\\DirectX"),
                new InstallStep("VCRedistAIO", "VCRedistAIO.zip", "https://sg1-dl.techpowerup.com/files/mcNKq_1or8HB3MOpyO_GBQ/1741034261/Visual-C-Runtimes-All-in-One-Nov-2024.zip", ""),
                new InstallStep("7zip", "7zip.exe", "https://www.7-zip.org/a/7z2409-x64.exe", "/S"),
                new InstallStep("Spotify", "Spotify.exe", "https://download.scdn.co/SpotifySetup.exe", "/S"),
                new InstallStep("Zoom", "zoom.exe", "https://zoom.us/client/6.3.11.60501/ZoomInstallerFull.exe", "zoom.exe /silent"),
                new InstallStep("Acrobat Reader", "acrobat.exe", "https://download1532.mediafire.com/q9cgujl7cptg_M-F2lIH8MPr19TWj8YwPRZo9RACH3uXnzqr5DWDuOGdrG-_-WHfe-ZFjwF5gMAjyT-tAW573ee8hzVmaUtCtZWHGrJPeaXGRTA3aHPPLNas8nwAE2lX8IDsWVY5inl9V4eINa4Hw5ghhfryCqh08mMXCWw6fgn8/a06rljgf25fri7r/Reader_en_install.exe", "/sAll /rs /l /msi /qb-"),
                new InstallStep("Google Chrome", "chrome.exe", "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7B0A0703C5-744E-12EE-BA2C-574FA3D84FE6%7D%26lang%3Den%26browser%3D3%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-statsdef_1%26installdataindex%3Dempty/chrome/install/ChromeStandaloneSetup64.exe", "chrome.exe /silent /install"),
                new InstallStep("VLC", "vlc.exe", "https://get.videolan.org/vlc/3.0.21/win64/vlc-3.0.21-win64.exe", "vlc.exe /S"),
            };

            int totalSteps = steps.Count * 2;
            progressBar1.Maximum = totalSteps;
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            Thread thread = new Thread(() =>
            {
                try
                {
                    int currentProgress = 0;

                    foreach (var step in steps)
                    {
                        string filePath = Path.Combine(downloadPath, step.FileName);

                        Directory.CreateDirectory(downloadPath);

                        bool downloadSuccess = false;
                        int downloadAttempts = 0;
                        const int maxDownloadAttempts = 3;

                        while (!downloadSuccess && downloadAttempts < maxDownloadAttempts)
                        {
                            downloadAttempts++;

                            if (!File.Exists(filePath) || (step.FileName.EndsWith(".zip") && downloadAttempts > 1))
                            {
                                AppendText($"{step.Name}: Downloading... (Attempt {downloadAttempts})\n");
                                downloadSuccess = DownloadFile(step.DownloadUrl, filePath);

                                if (downloadSuccess)
                                {
                                    AppendText($"{step.Name}: Downloaded successfully.\n");
                                }
                                else
                                {
                                    if (downloadAttempts < maxDownloadAttempts)
                                    {
                                        AppendText($"{step.Name}: Download failed. Retrying...\n");
                                    }
                                    else
                                    {
                                        AppendText($"{step.Name}: Download failed after {maxDownloadAttempts} attempts. Skipping installation.\n");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                AppendText($"{step.Name}: Already downloaded. Skipping download.\n");
                                downloadSuccess = true;
                            }
                        }

                        currentProgress++;
                        UpdateProgressBar(currentProgress, totalSteps);

                        if (!downloadSuccess)
                        {
                            continue;
                        }

                        bool installationSuccess = false;

                        if (!IsProgramInstalled(step.Name))
                        {
                            AppendText($"{step.Name}: Installing...\n");

                            if (filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                            {
                                int extractAttempts = 0;
                                const int maxExtractAttempts = 3;

                                while (!installationSuccess && extractAttempts < maxExtractAttempts)
                                {
                                    extractAttempts++;

                                    try
                                    {
                                        installationSuccess = InstallFromZip(filePath);

                                        if (!installationSuccess && extractAttempts < maxExtractAttempts)
                                        {
                                            AppendText($"{step.Name}: Extraction failed. Redownloading and retrying...\n");

                                            if (File.Exists(filePath))
                                            {
                                                File.Delete(filePath);
                                            }

                                            if (DownloadFile(step.DownloadUrl, filePath))
                                            {
                                                AppendText($"{step.Name}: Redownloaded successfully.\n");
                                            }
                                            else
                                            {
                                                AppendText($"{step.Name}: Redownload failed. Skipping installation.\n");
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        AppendText($"{step.Name}: Extraction error: {ex.Message}\n");

                                        if (extractAttempts < maxExtractAttempts)
                                        {
                                            AppendText($"{step.Name}: Retrying extraction...\n");
                                        }
                                        else
                                        {
                                            AppendText($"{step.Name}: Failed to extract after {maxExtractAttempts} attempts.\n");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                installationSuccess = RunInstaller(filePath, step.InstallArgs);
                            }

                            if (installationSuccess)
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
                            installationSuccess = true;
                        }

                        currentProgress++;
                        UpdateProgressBar(currentProgress, totalSteps);
                    }

                    AppendText("All installations completed.\n");
                }
                catch (Exception ex)
                {
                    AppendText($"Error: {ex.Message}\n");
                }
                finally
                {
                    Invoke(new Action(() => button5.Enabled = true));
                }
            });

            thread.Start();
        }

        private bool DownloadFile(string url, string destination)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ManualResetEvent downloadCompleted = new ManualResetEvent(false);

                    client.DownloadProgressChanged += (s, e) =>
                    {
                        int percent = e.ProgressPercentage;
                        UpdateDownloadProgress(Path.GetFileName(destination), percent);
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            AppendText($"Download error: {e.Error.Message}\n");
                            downloadCompleted.Set();
                        }
                        else if (e.Cancelled)
                        {
                            AppendText($"Download cancelled.\n");
                            downloadCompleted.Set();
                        }
                        else
                        {
                            UpdateDownloadProgress(Path.GetFileName(destination), 100);
                            AppendText($"Downloading {Path.GetFileName(destination)}: Completed\n");
                            downloadCompleted.Set();
                        }
                    };

                    client.DownloadFileAsync(new Uri(url), destination);
                    downloadCompleted.WaitOne();

                    return !client.IsBusy && File.Exists(destination);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Download error: {ex.Message}\n");
                return false;
            }
        }

        private Dictionary<string, string> lastProgressMessages = new Dictionary<string, string>();

        private void UpdateDownloadProgress(string fileName, int percent)
        {
            string progressMessage = $"Downloading {fileName}: {percent}%";

            Invoke(new Action(() =>
            {
                if (lastProgressMessages.ContainsKey(fileName))
                {
                    string lastMessage = lastProgressMessages[fileName];

                    int start = richTextBox1.Text.LastIndexOf(lastMessage);

                    if (start >= 0)
                    {
                        richTextBox1.SelectionStart = start;
                        richTextBox1.SelectionLength = lastMessage.Length;
                        richTextBox1.SelectedText = progressMessage;
                    }
                    else
                    {
                        richTextBox1.AppendText($"{progressMessage}\n");
                    }
                }
                else
                {
                    richTextBox1.AppendText($"{progressMessage}\n");
                }

                lastProgressMessages[fileName] = progressMessage;

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
                    try
                    {
                        Directory.Delete(extractPath, true);
                    }
                    catch (Exception ex)
                    {
                        AppendText($"Error deleting existing directory: {ex.Message}\n");
                    }
                }

                AppendText($"Extracting zip file to {extractPath}...\n");
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                AppendText($"Extraction completed\n");

                string installBatPath = Directory.GetFiles(extractPath, "install_all.bat", SearchOption.AllDirectories)
                                                 .FirstOrDefault();

                if (!string.IsNullOrEmpty(installBatPath))
                {
                    AppendText($"Found install_all.bat at {installBatPath}\n");
                    string output = RunBatchFileWithOutput(installBatPath);

                    if (output.IndexOf("Installation completed successfully", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        output.Contains("Finished"))
                    {
                        AppendText("VCRedistAIO installation completed successfully.\n");
                        return true;
                    }
                    else
                    {
                        AppendText($"install_all.bat did not indicate successful installation. Output:\n{output}\n");
                        return false;
                    }
                }
                else
                {
                    AppendText($"No install_all.bat found in {extractPath}\n");
                    return false;
                }
            }
            catch (InvalidDataException ex)
            {
                AppendText($"Zip file is corrupted: {ex.Message}\n");
                return false;
            }
            catch (Exception ex)
            {
                AppendText($"Unzip or install_all.bat execution error: {ex.Message}\n");
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
                        AppendText($"install_all.bat error: {error}\n");
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error running install_all.bat: {ex.Message}\n");
                return string.Empty;
            }
        }

        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text)));
                return;
            }

            richTextBox1.AppendText(text);
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void UpdateProgressBar(int currentValue, int maxValue)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgressBar(currentValue, maxValue)));
                return;
            }

            int percentage = (int)Math.Round((double)currentValue / maxValue * 100);

            progressBar1.Value = currentValue;
            Text = $"LhanzCJ Installer - {percentage}% Complete";
        }

        private bool IsProgramInstalled(string appName)
        {
            if (appName.Equals(".Net 4.8.1", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using (Microsoft.Win32.RegistryKey ndpKey =
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\"))
                    {
                        if (ndpKey != null)
                        {
                            object release = ndpKey.GetValue("Release");
                            if (release != null)
                            {
                                int releaseKey = Convert.ToInt32(release);
                                return releaseKey >= 533320;
                            }
                        }
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking .NET Framework version: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("DirectX", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string dxDiagPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "dxdiag.exe");
                    if (!File.Exists(dxDiagPath))
                        return false;
                    using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX"))
                    {
                        return key != null;
                    }
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking DirectX: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("VCRedistAIO", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] vcRedistKeys = new[] {
                @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64",
                @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x86",
                @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\x64",
                @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\x86"
            };

                    foreach (string keyPath in vcRedistKeys)
                    {
                        using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(keyPath))
                        {
                            if (key != null)
                            {
                                return true;
                            }
                        }
                    }

                    return CheckRegistryForPartialName(new[] { "Microsoft Visual C++", "VC++", "Visual C++ Redistributable" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking VC++ Redistributables: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("7zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "7-Zip", "7z.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "7-Zip" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking 7-Zip: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("Spotify", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "Spotify.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spotify", "Spotify.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Spotify", "Spotify.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Spotify", "Spotify.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "Spotify" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking Spotify: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("Zoom", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zoom", "bin", "Zoom.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Zoom", "bin", "Zoom.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Zoom", "bin", "Zoom.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Zoom", "bin", "Zoom.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "Zoom" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking Zoom: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("Acrobat Reader", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    // Check common Adobe Reader installation paths
                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Adobe", "Acrobat Reader DC", "Reader", "AcroRd32.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Adobe", "Acrobat Reader DC", "Reader", "AcroRd32.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Adobe", "Reader", "AcroRd32.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Adobe", "Reader", "AcroRd32.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "Adobe Reader", "Acrobat Reader", "Adobe Acrobat" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking Adobe Reader: {ex.Message}\n");
                    return false;
                }
            }

            if (appName.Equals("Google Chrome", StringComparison.OrdinalIgnoreCase))
            {
                try
                {

                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "Google Chrome" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking Google Chrome: {ex.Message}\n");
                    return false;
                }
            }
            if (appName.Equals("VLC", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string[] possiblePaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VideoLAN", "VLC", "vlc.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "VideoLAN", "VLC", "vlc.exe")
            };

                    foreach (string path in possiblePaths)
                    {
                        if (File.Exists(path))
                            return true;
                    }

                    return CheckRegistryForPartialName(new[] { "VLC", "VLC media player" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking VLC: {ex.Message}\n");
                    return false;
                }
            }

            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string uninstallKey32 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            if (CheckRegistryForProgram(uninstallKey, appName))
                return true;

            if (Environment.Is64BitOperatingSystem && CheckRegistryForProgram(uninstallKey32, appName))
                return true;

            return false;
        }
        private bool CheckRegistryForPartialName(string[] partialNames)
        {
            string[] registryKeys = {
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    };

            foreach (string registryKey in registryKeys)
            {
                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryKey))
                {
                    if (key == null) continue;

                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            if (subkey == null) continue;

                            string displayName = subkey.GetValue("DisplayName") as string;
                            if (string.IsNullOrEmpty(displayName)) continue;

                            foreach (string partialName in partialNames)
                            {
                                if (displayName.IndexOf(partialName, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool CheckRegistryForProgram(string registryKey, string appName)
        {
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            if (subkey != null)
                            {
                                string displayName = subkey.GetValue("DisplayName") as string;
                                if (!string.IsNullOrEmpty(displayName) && displayName.IndexOf(appName, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    return true;
                                }
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
            KBTest KBTest1 = new KBTest();
            KBTest1.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                string imgFolder = Path.Combine(Application.StartupPath, "img");

                if (!Directory.Exists(imgFolder))
                {
                    MessageBox.Show("Image folder not found! Cannot open Monitor Tester.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string[] files = Directory.GetFiles(imgFolder, "*.png");
                if (files.Length == 0)
                {
                    MessageBox.Show("No images found in the folder! Cannot open Monitor Tester.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MonitorTester LCDTest = new MonitorTester();
                LCDTest.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open Monitor Tester: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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