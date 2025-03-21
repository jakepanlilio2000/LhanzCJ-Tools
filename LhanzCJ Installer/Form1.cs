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
using System.Text;
using System.Drawing;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Management;
using System.Threading.Tasks;


namespace LhanzCJ_Installer
{
    public partial class LhanzCJ : Form
    {
        #region Variables
        private readonly HttpClient httpClient = new HttpClient();
        private CancellationTokenSource installcts;
        private CancellationTokenSource officeCts;
        private Thread installThread;
        private string tempDownloadPath;
        private readonly string downloadPath = "packages\\";
        private HttpClient officeDownloadClient;
        private const int SB_VERT = 1;
        private readonly Dictionary<string, string> lastProgressMessages = new Dictionary<string, string>();
        private string currentDownloadFile;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern uint GetFileAttributes(string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        [DllImport("user32.dll")]
        private static extern int SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        #endregion
        public LhanzCJ()
        {
            ForceRunAsAdmin();
            InitializeComponent();
            FormClosing += LhanzCJ_FormClosing;
            InitializeSoftwareJson();
        }
        private void InitializeSoftwareJson()
        {
            string jsonPath = "additional_software.json";
            if (!File.Exists(jsonPath))
            {
                File.WriteAllText(jsonPath, "[]");
            }
        }
        private void ForceRunAsAdmin()
        {
            if (!IsAdministrator())
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        Verb = "runas",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Admin privileges are required. Please restart as administrator.\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
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
            richTextBox1.Clear();
            button1.Enabled = false;
            progressBar1.Value = 0;
            progressBar1.Maximum = 9;

            Thread workerThread = new Thread(() =>
            {
                try
                {
                    Invoke(new Action(() => AppendText("Setting up Windows Desktop Icons\n", Color.Blue)));
                    ToggleDesktopIcon("{20D04FE0-3AEA-1069-A2D8-08002B30309D}", true, "Computer");
                    Invoke((Action)(() => progressBar1.Value++));

                    ToggleDesktopIcon("{645FF040-5081-101B-9F08-00AA002F954E}", true, "Recycle Bin");
                    Invoke((Action)(() => progressBar1.Value++));

                    ToggleDesktopIcon("{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}", true, "Control Panel");
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Disabling AutoPlay...\n", Color.Blue)));
                    DisableAutoPlay(true);
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Clearing taskbar cache...\n", Color.Blue)));
                    ClearTaskbarCache();
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Removing taskbar shortcuts...\n", Color.Blue)));
                    RemoveTaskbarShortcuts();
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Disabling Windows Widgets...\n", Color.Blue)));
                    DisableWindowsWidgets();
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Restarting Explorer...\n", Color.Blue)));
                    RestartExplorer();
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("Enabling SMB1.0/CIFS File Sharing Support...\n", Color.Blue)));
                    EnableSMB1();
                    Invoke((Action)(() => progressBar1.Value++));

                    Invoke(new Action(() => AppendText("All steps completed successfully!\n", Color.Green)));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => AppendText($"Error: {ex.Message}\n", Color.Red)));
                }
                finally
                {
                    Invoke(new Action(() => button1.Enabled = true));
                }
            })
            { IsBackground = true };
            workerThread.Start();
        }
        private void EnableSMB1()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-Command \"Enable-WindowsOptionalFeature -Online -FeatureName 'SMB1Protocol' -NoRestart -Verbose\"",
                    Verb = "runas",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            if (e.Data.Contains("WARNING"))
                            {
                                Invoke(new Action(() => AppendText(e.Data + "\n", Color.Red)));
                            }
                            else
                            {
                                Invoke(new Action(() => AppendText(e.Data + "\n", Color.Green)));
                            }
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                            Invoke(new Action(() => AppendText(e.Data + "\n", Color.Red)));
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit(300000);

                    if (process.ExitCode == 0)
                    {
                        Invoke(new Action(() => AppendText("SMB1.0/CIFS enabled successfully.\n", Color.Green)));
                        Invoke(new Action(() => AppendText("Please restart your computer to apply the changes.\n", Color.Orange)));
                        ShowRestartPrompt();
                    }
                    else
                        Invoke(new Action(() => AppendText($"Failed to enable SMB1. Exit code: {process.ExitCode}\n", Color.Red)));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => AppendText($"Error enabling SMB1: {ex.Message}\n", Color.Red)));
            }
        }

        private void ShowRestartPrompt()
        {
            DialogResult result = MessageBox.Show("Changes have been made that require a restart. Do you want to restart now?", "Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Process.Start("shutdown", "/r /t 0");
            }
        }

        private void ClearTaskbarCache()
        {
            try
            {
                string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband";

                using (RegistryKey taskbandKey = Registry.CurrentUser.OpenSubKey(registryKeyPath, true))
                {
                    if (taskbandKey != null)
                    {
                        taskbandKey.DeleteValue("Favorites", false);
                        taskbandKey.DeleteValue("FavoritesResolve", false);
                    }
                    AppendText("Taskbar cache cleared.\n", Color.Green);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error clearing taskbar cache: {ex.Message}", Color.Red);
            }
        }
        private void RemoveTaskbarShortcuts()
        {
            try
            {
                string taskbarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                  @"Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar");

                if (Directory.Exists(taskbarPath))
                {
                    foreach (string file in Directory.GetFiles(taskbarPath, "*.lnk"))
                    {
                        if (!file.Contains("File Explorer.lnk"))
                        {
                            File.Delete(file);
                        }
                    }
                    AppendText("Taskbar shortcuts removed.\n", Color.Green);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error removing taskbar shortcuts: {ex.Message}", Color.Red);
            }
        }

        private void DisableWindowsWidgets()
        {
            try
            {
                using (RegistryKey feedsKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Feeds", true))
                {
                    feedsKey?.SetValue("ShellFeedsTaskbarViewMode", 2, RegistryValueKind.DWord);
                    AppendText("Windows News and Interests disabled.\n", Color.Green);
                }

                using (RegistryKey advancedKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true))
                {
                    advancedKey?.SetValue("TaskbarDa", 0, RegistryValueKind.DWord);
                    AppendText("Windows Widgets disabled.\n", Color.Green);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error disabling widgets: {ex.Message}", Color.Red);
            }
        }
        private void DisableAutoPlay(bool disable)
        {
            try
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    key?.SetValue("DisableAutoplay", disable ? 1 : 0, RegistryValueKind.DWord);
                    AppendText($"AutoPlay {(disable ? "disabled" : "enabled")}.\n", Color.Green);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error modifying AutoPlay: {ex.Message}", Color.Red);
            }
        }
        private void ToggleDesktopIcon(string guid, bool show, string iconName)
        {
            try
            {
                string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
                {
                    key?.SetValue(guid, show ? 0 : 1, RegistryValueKind.DWord);
                }
                AppendText($"{iconName} icon visibility set to {show}.\n", Color.Green);
            }
            catch (Exception ex)
            {
                AppendText($"Error modifying registry: {ex.Message}", Color.Red);
            }
        }

        private void RestartExplorer()
        {
            try
            {
                Process[] explorer = Process.GetProcessesByName("explorer");
                foreach (Process p in explorer)
                {
                    p.Kill();
                }
                Process.Start("explorer.exe");
            }
            catch (Exception ex)
            {
                AppendText($"Error restarting Explorer: {ex.Message}", Color.Red);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
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
            catch (Exception)
            {
                AppendText("Failed to open Windows Features: ", Color.Red);
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
            richTextBox1.Clear();
            try
            {
                SetRegistryKeys();
                DialogResult result = MessageBox.Show(
                    "Do you want to visit \\technical network? Click 'No' to enter a custom destination.",
                    "Network Access",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", @"\\technical");
                }
                else if (result == DialogResult.No)
                {
                    string inputPath = ShowInputDialog("Enter the network destination (e.g., \\\\server\\share):");

                    if (!string.IsNullOrWhiteSpace(inputPath))
                    {
                        Process.Start("explorer.exe", inputPath);
                    }
                }
            }
            catch (Exception)
            {
                AppendText("Failed to open network location: ", Color.Red);
            }
        }
        private string ShowInputDialog(string prompt)
        {
            Form promptForm = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Custom Network Access",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt };
            TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 350 };
            Button confirmation = new Button() { Text = "OK", Left = 150, Width = 100, Top = 90, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => { promptForm.Close(); };
            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(confirmation);
            promptForm.AcceptButton = confirmation;

            return promptForm.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
        private void SetRegistryKeys()
        {
            try
            {
                string registryPath = @"SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AllowInsecureGuestAuth", 1, RegistryValueKind.DWord);
                        key.SetValue("RequireSecuritySignature", 0, RegistryValueKind.DWord);
                        AppendText(Text = "Network registry keys updated successfully.\n", Color.Green);
                    }
                    else
                    {
                        AppendText("Failed to access registry path: ", Color.Red);
                    }
                }
            }
            catch (Exception)
            {
                AppendText("Failed to update registry: ", Color.Red);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                Process.Start("ms-settings:windowsupdate");
                AppendText("Windows Update opened.\n", Color.Blue);
            }
            catch (Exception)
            {
                AppendText("Failed to open Windows Update: ", Color.Red);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            using (var choiceDialog = new ChoiceDialogForm())
            {
                if (choiceDialog.ShowDialog() != DialogResult.OK) return;

                List<CustomSoftware> additionalSoftware = new List<CustomSoftware>();
                if (choiceDialog.AddAdditionalSoftware)
                {
                    using (var softwareForm = new SoftwareSelectionForm())
                    {
                        if (softwareForm.ShowDialog() == DialogResult.OK)
                        {
                            additionalSoftware = softwareForm.GetSelectedSoftware();
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                if (installcts != null)
                {
                    installcts.Dispose();
                    installcts = null;
                }
                installcts = new CancellationTokenSource();

                button5.Enabled = false;
                button18.Enabled = false;
                tempDownloadPath = Path.Combine(Path.GetTempPath(), "LhanzCJ_Downloads");
                Directory.CreateDirectory(tempDownloadPath);

                richTextBox1.Clear();
                progressBar1.Value = 0;
                progressBar2.Value = 0;
                button21.Visible = true;

                InstallPrograms(false, tempDownloadPath, additionalSoftware);

                // Disable other buttons as needed
                button16.Enabled = false;
                button20.Enabled = false;
                wifiConnectBtn.Enabled = false;
                setclockBtn.Enabled = false;
                officeEdition.Enabled = false;
            }
        }
        private void button18_Click(object sender, EventArgs e)
        {
            if (installcts != null)
            {
                installcts.Dispose();
                installcts = null;
            }
            installcts = new CancellationTokenSource();

            button18.Enabled = false;
            button5.Enabled = false;
            InstallPrograms(true, downloadPath, new List<CustomSoftware>());
            button21.Visible = true;
            button16.Enabled = false;
            button20.Enabled = false;
            wifiConnectBtn.Enabled = false;
            setclockBtn.Enabled = false;
            officeEdition.Enabled = false;
        }

        private void InstallPrograms(bool excludeComponents, string downloadDirectory, List<CustomSoftware> additionalSoftware)
        {
            richTextBox1.Clear();
            tempDownloadPath = downloadDirectory;

            List<InstallStep> steps = new List<InstallStep>();

            if (!excludeComponents)
            {
                steps.Add(new InstallStep("DirectX", "directx.exe", "https://download.microsoft.com/download/8/4/a/84a35bf1-dafe-4ae8-82af-ad2ae20b6b14/directx_Jun2010_redist.exe", "/Q /T:C:\\DirectX"));
            }
            steps.Add(new InstallStep(".NET 5", "net5.exe", "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/5.0.17/windowsdesktop-runtime-5.0.17-win-x64.exe", "/quiet"));
            steps.Add(new InstallStep(".NET 6", "net6.exe", "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/6.0.36/windowsdesktop-runtime-6.0.36-win-x64.exe", "/quiet"));
            steps.Add(new InstallStep(".NET 7", "net7.exe", "https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/7.0.20/windowsdesktop-runtime-7.0.20-win-x64.exe", "/quiet"));
            steps.Add(new InstallStep(".NET 8", "net8.exe", "https://download.visualstudio.microsoft.com/download/pr/64760cc4-228f-48e4-b57d-55f882dedc69/b181f927cb937ef06fbb6eb41e81fbd0/windowsdesktop-runtime-8.0.14-win-x64.exe", "/quiet"));
            steps.Add(new InstallStep(".NET 9", "net9.exe", "https://download.visualstudio.microsoft.com/download/pr/63f0335a-6012-4017-845f-5d655d56a44f/f8d5150469889387a1de578d45415201/windowsdesktop-runtime-9.0.3-win-x64.exe", "/quiet"));
            steps.Add(new InstallStep("7zip", "7zip.exe", "https://www.7-zip.org/a/7z2409-x64.exe", "/S"));
            steps.Add(new InstallStep("Zoom", "zoom.exe", "https://zoom.us/client/6.3.11.60501/ZoomInstallerFull.exe", "zoom.exe /silent"));
            steps.Add(new InstallStep("Google Chrome", "chrome.exe", "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BF3CAB977-BF22-F629-B1C4-B9D9234DDFD5%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-stable/update2/installers/ChromeSetup.exe", "chrome.exe /silent /install"));
            steps.Add(new InstallStep("VLC", "vlc.msi", "https://artifacts.videolan.org/vlc-3.0/nightly-win64/20250321-0216/vlc-3.0.21-win64-a381bf5b.msi", "/qn"));
            steps.Add(new InstallStep("Acrobat Reader", "acrobat.exe", "https://ardownload2.adobe.com/pub/adobe/reader/win/AcrobatDC/2400520414/AcroRdrDC2400520414_en_US.exe", "/sAll /rs /l /msi /qb- /L*v"));
            foreach (var software in additionalSoftware)
            {
                string fileName;
                try
                {
                    Uri uri = new Uri(software.Url);
                    fileName = Path.GetFileName(uri.LocalPath);
                    if (string.IsNullOrEmpty(fileName))
                        fileName = $"{software.Name.Replace(" ", "_")}.exe";
                }
                catch (UriFormatException)
                {
                    AppendText($"Invalid URL for {software.Name}: {software.Url}\n", Color.Red);
                    continue;
                }

                string installArgs = DetermineInstallArgs(fileName);
                steps.Add(new InstallStep(software.Name, fileName, software.Url, installArgs));
            }
            if (!excludeComponents)
            {
                steps.Add(new InstallStep("Winget CLI", "winget.msixbundle", "https://aka.ms/getwinget", "/q"));
            }
            int totalSteps = steps.Count * 2;
            progressBar1.Maximum = totalSteps;
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            installThread = new Thread(() =>
            {
                try
                {
                    if (installcts.IsCancellationRequested)
                    {
                        AppendText("Installation was cancelled before starting.\n", Color.Orange);
                        return;
                    }
                    int currentProgress = 0;

                    foreach (var step in steps)
                    {

                        currentDownloadFile = step.FileName;
                        if (installcts.IsCancellationRequested)
                        {
                            AppendText("Installation cancelled.\n", Color.Orange);
                            return;
                        }
                        Invoke(new Action(() =>
                        {
                            progressBar2.Value = 0;
                            progressBar2.Style = ProgressBarStyle.Continuous;
                        }));
                        string filePath = Path.Combine(downloadDirectory, step.FileName);
                        Directory.CreateDirectory(downloadDirectory);
                        if (IsProgramInstalled(step.Name))
                        {
                            AppendText($"{step.Name}: Already installed. Skipping download and installation.\n", Color.DarkCyan);
                            currentProgress += 2;
                            UpdateProgressBar(currentProgress, totalSteps);
                            continue;
                        }
                        bool downloadSuccess = false;
                        int downloadAttempts = 0;
                        const int maxDownloadAttempts = 3;

                        while (!downloadSuccess && downloadAttempts < maxDownloadAttempts)
                        {
                            downloadAttempts++;
                            if (!File.Exists(filePath) || (step.FileName.EndsWith(".zip") && downloadAttempts > 1))
                            {
                                AppendText($"{step.Name}: Downloading... (Attempt {downloadAttempts})\n", Color.Blue);
                                downloadSuccess = DownloadFile(step.DownloadUrl, filePath);

                                if (downloadSuccess)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        progressBar2.Style = ProgressBarStyle.Marquee;
                                        progressBar2.MarqueeAnimationSpeed = 30;
                                    }));

                                    AppendText($"{step.Name}: Downloaded successfully.\n", Color.Green);

                                    Invoke(new Action(() =>
                                    {
                                        progressBar2.Style = ProgressBarStyle.Continuous;
                                        progressBar2.Value = 100;
                                    }));
                                }
                                else
                                {
                                    if (downloadAttempts < maxDownloadAttempts)
                                    {
                                        AppendText($"{step.Name}: Download failed. Retrying...\n", Color.Red);
                                    }
                                    else
                                    {
                                        AppendText($"{step.Name}: Download failed after {maxDownloadAttempts} attempts. Skipping installation.\n", Color.Red);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                AppendText($"{step.Name}: Already downloaded. Skipping download.\n", Color.Blue);
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
                            AppendText($"{step.Name}: Installing...\n", Color.DarkCyan);

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
                                            AppendText($"{step.Name}: Extraction failed. Redownloading and retrying...\n", Color.Red);

                                            if (File.Exists(filePath))
                                            {
                                                File.Delete(filePath);
                                            }

                                            if (DownloadFile(step.DownloadUrl, filePath))
                                            {
                                                AppendText($"{step.Name}: Redownloaded successfully.\n", Color.Green);
                                            }
                                            else
                                            {
                                                AppendText($"{step.Name}: Redownload failed. Skipping installation.\n", Color.Red);
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        AppendText($"{step.Name}: Extraction error: {ex.Message}\n", Color.Red);

                                        if (extractAttempts < maxExtractAttempts)
                                        {
                                            AppendText($"{step.Name}: Retrying extraction...\n", Color.Red);
                                        }
                                        else
                                        {
                                            AppendText($"{step.Name}: Failed to extract after {maxExtractAttempts} attempts.\n", Color.Red);
                                        }
                                    }
                                }
                            }
                            else if (step.Name == "Winget CLI")
                            {
                                installationSuccess = InstallWinget(filePath);
                            }
                            else
                            {
                                installationSuccess = RunInstaller(filePath, step.InstallArgs);
                            }
                            if (step.Name == "DirectX" && installationSuccess)
                            {
                                string dxsetupPath = Path.Combine("C:\\DirectX", "DXSETUP.exe");
                                if (File.Exists(dxsetupPath))
                                {
                                    AppendText($"{step.Name}: Running DXSETUP installation...\n", Color.DarkCyan);
                                    bool dxsetupSuccess = RunInstaller(dxsetupPath, "/silent");

                                    if (dxsetupSuccess)
                                    {
                                        AppendText($"{step.Name}: Installation completed successfully.\n", Color.Green);
                                        installationSuccess = true;
                                    }
                                    else
                                    {
                                        AppendText($"{step.Name}: Failed to install via DXSETUP.exe.\n", Color.Red);
                                        installationSuccess = false;
                                    }
                                }
                                else
                                {
                                    AppendText($"{step.Name}: DXSETUP.exe not found in extraction directory.\n", Color.Red);
                                    installationSuccess = false;
                                }
                            }
                            if (step.Name == "Winget CLI")
                            {
                                AppendText("Installing Winget CLI...\n", Color.Blue);
                                installationSuccess = InstallWinget(filePath);

                                if (installationSuccess)
                                {
                                    AppendText("Updating packages via Winget...\n", Color.Blue);
                                    InstallSpotify();
                                    InstallVCRedistsWithWinget();
                                    UpdatePackagesWithWinget();
                                }
                                else
                                {
                                    AppendText("Winget CLI: Installation failed. Skipping Winget operations.\n", Color.Red);
                                }
                            }
                            if (installationSuccess)
                            {
                                AppendText($"{step.Name}: Installed successfully.\n", Color.Green);
                            }
                            else
                            {
                                AppendText($"{step.Name}: Installation failed.\n", Color.Red);
                            }
                        }
                        else
                        {
                            AppendText($"{step.Name}: Already installed. Skipping installation.\n", Color.DarkCyan);
                            installationSuccess = true;
                        }
                        currentProgress++;
                        UpdateProgressBar(currentProgress, totalSteps);
                    }
                    AppendText("All installations completed.\n", Color.Blue);
                }
                catch (OperationCanceledException)
                {
                    AppendText("Installation cancelled during setup.\n", Color.Orange);
                }
                finally
                {
                    Invoke(new Action(() =>
                    {
                        button5.Enabled = true;
                        button18.Enabled = true;
                        button21.Visible = false;
                        button16.Enabled = true;
                        button20.Enabled = true;
                        wifiConnectBtn.Enabled = true;
                        setclockBtn.Enabled = true;
                        officeEdition.Enabled = true;
                        progressBar1.Value = 0;
                        progressBar2.Value = 0;
                    }));
                    if (downloadDirectory.StartsWith(Path.GetTempPath()))
                    {
                        try
                        {
                            Directory.Delete(downloadDirectory, true);
                            AppendText("Temporary files cleaned up successfully.\n", Color.Green);
                        }
                        catch (Exception ex)
                        {
                            AppendText($"Error cleaning temp files: {ex.Message}\n", Color.Red);
                        }
                    }
                }
            });
            installThread.Start();
        }

        private string DetermineInstallArgs(string fileName)
        {
            if (fileName.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                return "/qn";
            else if (fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                return "/S";
            else if (fileName.EndsWith(".msixbundle", StringComparison.OrdinalIgnoreCase))
                return "/q";
            else
                return "";
        }
        private void KillProcesses()
        {
            string[] processNames = { "cmd", "powershell", "winget", "msiexec", "dism" };

            foreach (string name in processNames)
            {
                foreach (Process process in Process.GetProcessesByName(name))
                {
                    try
                    {
                        KillProcessTree(process.Id);
                        AppendText($"Terminated process and children: {name}\n", Color.Orange);
                    }
                    catch (Exception ex)
                    {
                        AppendText($"Error terminating {name}: {ex.Message}\n", Color.Red);
                    }
                }
            }
        }
        private void KillProcessTree(int parentId)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    $"SELECT ProcessId FROM Win32_Process WHERE ParentProcessId={parentId}"))
                {
                    foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                    {
                        int childProcessId = Convert.ToInt32(obj["ProcessId"]);
                        KillProcessTree(childProcessId);
                    }
                }

                Process parentProcess = Process.GetProcessById(parentId);
                parentProcess.Kill();
                parentProcess.WaitForExit(5000);
            }
            catch (Exception)
            {
            }
        }
        private void InstallSpotify()
        {
            string command = "winget install --id Spotify.Spotify --accept-package-agreements --accept-source-agreements";
            bool success = RunPowerShellCommand(command, true);

            if (success)
            {
                AppendText("Spotify installed successfully\n", Color.Green);
            }
            else
            {
                AppendText("Failed to install Spotify\n", Color.Red);
            }
        }
        private bool InstallWinget(string msixPath)
        {
            try
            {
                if (!File.Exists(msixPath))
                {
                    AppendText($"Winget CLI: File not found at {msixPath}\n", Color.Red);
                    return false;
                }

                string command = $@"Add-AppxPackage -Path '{msixPath}' -ErrorAction Stop;
                     if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {{
                         throw 'Winget installation failed'
                     }}";

                bool success = RunPowerShellCommand(command, true);

                if (success)
                {
                    AppendText("Winget CLI: Installed successfully\n", Color.Green);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AppendText($"Winget CLI: Installation error - {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private void UpdatePackagesWithWinget()
        {
            string command = "winget upgrade --all --accept-source-agreements --accept-package-agreements";
            RunPowerShellCommand(command, true);
        }
        private void InstallVCRedistsWithWinget()
        {
            string[] vcRedistCommands = new string[]
            {
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2005.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2005.x64\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2008.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2008.x64\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2010.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2010.x64\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2012.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2012.x64\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2013.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2013.x64\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2015+.x86\" --accept-package-agreements --accept-source-agreements",
                "winget install --exact --locale \"en-US\" --id=\"Microsoft.VCRedist.2015+.x64\" --accept-package-agreements --accept-source-agreements"
            };

            foreach (var command in vcRedistCommands)
            {
                bool success = RunPowerShellCommand(command, true);
                string packageId = Regex.Match(command, @"--id=""([^""]+)""").Groups[1].Value.Replace(".", " ");

                if (!success)
                {
                    AppendText($"{packageId} failed to install\n", Color.Red);
                }
                else
                {
                    AppendText($"{packageId} installed successfully\n", Color.Green);
                }
            }
        }
        private bool RunPowerShellCommand(string command, bool captureOutput = false)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    var outputBuffer = new StringBuilder();
                    var errorBuffer = new StringBuilder();

                    Thread outputThread = new Thread(() => ReadStream(process.StandardOutput, outputBuffer, Color.Green));
                    Thread errorThread = new Thread(() => ReadStream(process.StandardError, errorBuffer, Color.Red, true));

                    outputThread.Start();
                    errorThread.Start();

                    process.WaitForExit();
                    outputThread.Join();
                    errorThread.Join();
                    if (captureOutput)
                    {
                        AppendText(outputBuffer.ToString(), Color.Green);
                        AppendText(errorBuffer.ToString(), Color.Red);
                    }

                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                AppendText($"PowerShell error: {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private void ReadStream(StreamReader reader, StringBuilder buffer, Color color, bool isError = false)
        {
            char[] charBuffer = new char[1024];
            int bytesRead;

            while ((bytesRead = reader.Read(charBuffer, 0, charBuffer.Length)) > 0)
            {
                string chunk = new string(charBuffer, 0, bytesRead);
                if (TryHandleProgressBar(chunk, color))
                {
                    continue;
                }

                ProcessChunk(chunk, buffer, color, isError);
            }

            if (buffer.Length > 0)
            {
                FlushBuffer(buffer, color, isError);
            }
        }
        private bool TryHandleProgressBar(string chunk, Color color)
        {
            var match = Regex.Match(chunk, @"[█▒]+(\s+\d+%|\s+\d+\.\d+\s\w+/\d+\s\w+)");
            if (match.Success)
            {
                UpdateProgressLine(match.Value, color);
                return true;
            }
            return false;
        }
        private void UpdateProgressLine(string progress, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgressLine(progress, color)));
                return;
            }

            bool wasAtBottom = IsScrollAtBottom();
            SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)0, IntPtr.Zero);

            try
            {
                int lastLineStart = richTextBox1.Text.LastIndexOf('\n');
                if (lastLineStart >= 0)
                {
                    string lastLine = richTextBox1.Text.Substring(lastLineStart).Trim();
                    if (lastLine.Contains("%") || lastLine.Contains("/"))
                    {
                        richTextBox1.Select(lastLineStart + 1, richTextBox1.TextLength - (lastLineStart + 1));
                        richTextBox1.SelectedText = $"{progress}";
                        return;
                    }
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText($"{progress}\n");
            }
            finally
            {
                SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)1, IntPtr.Zero);
                richTextBox1.Invalidate();
            }
        }
        private void ProcessChunk(string chunk, StringBuilder buffer, Color color, bool isError)
        {
            foreach (char c in chunk)
            {
                if (c == '\r')
                {
                    FlushBuffer(buffer, color, isError, true);
                }
                else if (c == '\n')
                {
                    FlushBuffer(buffer, color, isError);
                }
                else if (char.IsControl(c))
                {
                }
                else
                {
                    buffer.Append(c);
                }
            }
        }
        private void FlushBuffer(StringBuilder buffer, Color color, bool isError, bool isCarriageReturn = false)
        {
            if (buffer.Length == 0)
                return;

            string text = buffer.ToString();
            buffer.Clear();

            text = CleanProgressArtifacts(text);

            if (!string.IsNullOrWhiteSpace(text))
            {
                AppendFormattedText(text, color, isError, isCarriageReturn);
            }
        }
        private void AppendFormattedText(string text, Color color, bool isError, bool isCarriageReturn)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendFormattedText(text, color, isError, isCarriageReturn)));
                return;
            }

            bool wasAtBottom = IsScrollAtBottom();
            SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)0, IntPtr.Zero);

            try
            {
                if (isCarriageReturn)
                {
                    int lastNewLine = richTextBox1.Text.LastIndexOf('\n');
                    if (lastNewLine >= 0)
                    {
                        richTextBox1.Select(lastNewLine + 1, richTextBox1.TextLength - (lastNewLine + 1));
                        richTextBox1.SelectedText = $"{text}";
                        return;
                    }
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText($"{text}\n");
                richTextBox1.SelectionColor = richTextBox1.ForeColor;

                if (wasAtBottom)
                {
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.ScrollToCaret();
                }
            }
            finally
            {
                SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)1, IntPtr.Zero);
                richTextBox1.Invalidate();
            }
        }
        private string CleanProgressArtifacts(string input)
        {
            var cleaned = Regex.Replace(input, @"\[\d{2}:\d{2}:\d{2}\]", "");
            cleaned = Regex.Replace(cleaned, @"\s[-\\|/]+\s", " ");

            return cleaned.Trim();
        }
        private bool DownloadFile(string url, string destination)
        {
            try
            {
                using (var response = httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
                using (var stream = response.Content.ReadAsStreamAsync().Result)
                using (var fileStream = new FileStream(destination, FileMode.Create))
                {
                    if (installcts.IsCancellationRequested)
                    {
                        AppendText("Download cancelled before starting.\n", Color.Orange);
                        return false;
                    }

                    var buffer = new byte[8192];
                    int bytesRead;
                    long totalRead = 0;
                    var contentLength = response.Content.Headers.ContentLength;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (installcts.IsCancellationRequested)
                        {
                            AppendText("Download cancelled during transfer.\n", Color.Orange);
                            File.Delete(destination);
                            return false;
                        }

                        fileStream.Write(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        if (contentLength.HasValue)
                        {
                            var progress = (int)((double)totalRead / contentLength.Value * 100);
                            UpdateDownloadProgress(Path.GetFileName(destination), progress);
                        }
                    }
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                AppendText("Download cancelled.\n", Color.Orange);
                return false;
            }
            catch (Exception ex)
            {
                AppendText($"Download error: {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private void UpdateDownloadProgress(string fileName, int percent)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateDownloadProgress(fileName, percent)));
                return;
            }

            string progressMessage = $"Downloading {fileName}: {percent}%";
            bool wasAtBottom = IsScrollAtBottom();

            SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)0, IntPtr.Zero);

            try
            {
                if (lastProgressMessages.TryGetValue(fileName, out string previousMessage))
                {
                    int startIndex = richTextBox1.Text.LastIndexOf(previousMessage);
                    if (startIndex >= 0)
                    {
                        richTextBox1.Select(startIndex, previousMessage.Length);
                        richTextBox1.SelectedText = progressMessage;
                    }
                    else
                    {
                        richTextBox1.AppendText(progressMessage + "\n");
                    }
                }
                else
                {
                    richTextBox1.AppendText(progressMessage + "\n");
                }

                lastProgressMessages[fileName] = progressMessage;

                if (wasAtBottom)
                {
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
            }
            finally
            {
                SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)1, IntPtr.Zero);
                richTextBox1.Invalidate();
            }
        }
        private bool RunInstaller(string filePath, string arguments)
        {
            try
            {
                string fileName = filePath;
                string args = arguments;

                if (filePath.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = "msiexec.exe";
                    args = $"/i \"{filePath}\" {arguments} /qn";
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    Verb = "runas",
                    UseShellExecute = true
                };
                Process.Start(psi)?.WaitForExit(300000);
                return true;
            }
            catch (Exception ex)
            {
                AppendText($"Installation Error: {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private bool InstallFromZip(string zipFilePath)
        {
            try
            {
                string extractPath = Path.Combine(Path.GetDirectoryName(zipFilePath),
                                     Path.GetFileNameWithoutExtension(zipFilePath));

                if (Directory.Exists(extractPath))
                {
                    RetryAction(() => Directory.Delete(extractPath, true));
                }

                RetryAction(() => Directory.CreateDirectory(extractPath));

                using (var zip = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (var entry in zip.Entries)
                    {
                        var destPath = Path.Combine(extractPath, entry.FullName);
                        RetryAction(() => entry.ExtractToFile(destPath, true));
                    }
                }

                AppendText($"Extracting zip file to {extractPath}...\n", Color.Blue);
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                AppendText($"Extraction completed\n", Color.Green);

                string installBatPath = Directory.GetFiles(extractPath, "install_all.bat", SearchOption.AllDirectories)
                                                 .FirstOrDefault();

                if (!string.IsNullOrEmpty(installBatPath))
                {
                    AppendText($"Found install_all.bat at {installBatPath}\n", Color.Green);
                    string output = RunBatchFileWithOutput(installBatPath);

                    if (output.IndexOf("Installation completed successfully", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        output.Contains("Finished"))
                    {
                        AppendText("VCRedistAIO installation completed successfully.\n", Color.Blue);
                        return true;
                    }
                    else
                    {
                        AppendText($"install_all.bat did not indicate successful installation. Output:\n{output}\n", Color.Red);
                        return false;
                    }
                }
                else
                {
                    AppendText($"No install_all.bat found in {extractPath}\n", Color.Red);
                    return false;
                }
            }
            catch (InvalidDataException ex)
            {
                AppendText($"Zip file is corrupted: {ex.Message}\n", Color.Red);
                return false;
            }
            catch (Exception ex)
            {
                AppendText($"Unzip or install_all.bat execution error: {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private void RetryAction(Action action, int retries = 3, int delay = 500)
        {
            do
            {
                try
                {
                    action();
                    return;
                }
                catch
                {
                    if (retries == 0) throw;
                    Thread.Sleep(delay);
                }
            } while (retries-- > 0);
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
                        AppendText($"install_all.bat error: {error}\n", Color.Red);
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error running install_all.bat: {ex.Message}\n", Color.Red);
                return string.Empty;
            }
        }
        private void AppendText(string text, Color color, bool replaceLastLine = false)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text, color, replaceLastLine)));
                return;
            }

            bool wasAtBottom = IsScrollAtBottom();
            SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)0, IntPtr.Zero);

            try
            {
                if (replaceLastLine)
                {
                    int lastNewLine = richTextBox1.Text.LastIndexOf('\n');
                    if (lastNewLine == -1)
                    {
                        richTextBox1.Clear();
                    }
                    else
                    {
                        richTextBox1.Select(lastNewLine + 1, richTextBox1.TextLength - (lastNewLine + 1));
                        richTextBox1.SelectedText = "";
                    }
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText($"{text}");
                richTextBox1.SelectionColor = richTextBox1.ForeColor;

                if (wasAtBottom)
                {
                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.ScrollToCaret();
                }
            }
            finally
            {
                SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)1, IntPtr.Zero);
                richTextBox1.Invalidate();
            }
        }
        private bool IsScrollAtBottom()
        {
            int scrollPos = GetScrollPos(richTextBox1.Handle, SB_VERT);
            GetScrollRange(richTextBox1.Handle, SB_VERT, out _, out int maxScroll);
            return scrollPos >= maxScroll;
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
            if (appName.Equals(".NET 5", StringComparison.OrdinalIgnoreCase))
            {
                return IsDotNetVersionInstalled(5);
            }
            if (appName.Equals(".NET 6", StringComparison.OrdinalIgnoreCase))
            {
                return IsDotNetVersionInstalled(6);
            }
            if (appName.Equals(".NET 7", StringComparison.OrdinalIgnoreCase))
            {
                return IsDotNetVersionInstalled(7);
            }
            if (appName.Equals(".NET 8", StringComparison.OrdinalIgnoreCase))
            {
                return IsDotNetVersionInstalled(8);
            }
            if (appName.Equals(".NET 9", StringComparison.OrdinalIgnoreCase))
            {
                return IsDotNetVersionInstalled(9);
            }


            if (appName.Equals("7zip", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                    string[] possiblePaths = {
            Path.Combine(programFiles, "7-Zip", "7z.exe"),
            !string.IsNullOrEmpty(programFilesX86) ? Path.Combine(programFilesX86, "7-Zip", "7z.exe") : ""
        };

                    foreach (string path in possiblePaths)
                    {
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                            return true;
                    }
                    return CheckRegistryForPartialName(new[] { "7-Zip" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking 7-Zip: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking Zoom: {ex.Message}\n", Color.Red);
                    return false;
                }
            }

            if (appName.Equals("Acrobat Reader", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
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
                    AppendText($"Error checking Adobe Reader: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking Google Chrome: {ex.Message}\n", Color.Red);
                    return false;
                }
            }
            if (appName.Equals("VLC", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                    string[] possiblePaths = {
                Path.Combine(programFiles, "VideoLAN", "VLC", "vlc.exe"),
                !string.IsNullOrEmpty(programFilesX86) ? Path.Combine(programFilesX86, "VideoLAN", "VLC", "vlc.exe") : ""
            };

                    foreach (string path in possiblePaths)
                    {
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                            return true;
                    }
                    return CheckRegistryForPartialName(new[] { "VLC", "VLC media player" });
                }
                catch (Exception ex)
                {
                    AppendText($"Error checking VLC: {ex.Message}\nStackTrace: {ex.StackTrace}\n", Color.Red);
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
        private bool IsDotNetVersionInstalled(int majorVersion)
        {
            try
            {
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                string dotnetSharedPath = Path.Combine(programFiles, "dotnet", "shared");
                string dotnetSharedPathX86 = Path.Combine(programFilesX86, "dotnet", "shared");

                string[] runtimeFolders = { "Microsoft.WindowsDesktop.App", "Microsoft.NETCore.App" };

                foreach (string runtimeFolder in runtimeFolders)
                {
                    string fullPath = Path.Combine(dotnetSharedPath, runtimeFolder);
                    if (Directory.Exists(fullPath))
                    {
                        var directories = Directory.EnumerateDirectories(fullPath, $"{majorVersion}.*");
                        if (directories.Any())
                            return true;
                    }

                    string fullPathX86 = Path.Combine(dotnetSharedPathX86, runtimeFolder);
                    if (Directory.Exists(fullPathX86))
                    {
                        var directories = Directory.EnumerateDirectories(fullPathX86, $"{majorVersion}.*");
                        if (directories.Any())
                            return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                AppendText($"Error checking .NET {majorVersion}: {ex.Message}\n", Color.Red);
                return false;
            }
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
            richTextBox1.Clear();
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.office.com/setup/",
                    UseShellExecute = true
                });
                AppendText("Opening Office setup website...\n", Color.Blue);
            }
            catch (Exception)
            {
                AppendText("Failed to open the website: ", Color.Red);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
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
                AppendText("Opening activation program...\n", Color.Blue);
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
            richTextBox1.Clear();
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
                AppendText("Opening dxdiag...\n", Color.Blue);
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
            richTextBox1.Clear();
            DialogResult choice = MessageBox.Show(
                "Choose a camera option:\n\nYes - Use Windows Camera\nNo - Use Software Camera",
                "Select Camera",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (choice == DialogResult.Yes)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "microsoft.windows.camera:",
                        UseShellExecute = true
                    });
                    AppendText("Opening Windows Camera app...\n", Color.Blue);
                }
                catch (Exception)
                {
                    AppendText("An error occurred while opening the Windows Camera app.", Color.Red);
                }
            }
            else if (choice == DialogResult.No)
            {
                CamTest camTest = new CamTest();
                camTest.Show();
                AppendText("Opening Software Camera...\n", Color.Blue);
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            MicTest micTest = new MicTest();
            micTest.Show();
            AppendText("Opening Microphone Tester...\n", Color.Blue);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
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
                AppendText("Opening Device Manager...\n", Color.Blue);
            }
            catch (Exception)
            {
                AppendText("Failed to open program: ", Color.Red);
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
            richTextBox1.Clear();
            KBTest KBTest1 = new KBTest();
            KBTest1.Show();
            AppendText("Opening Keyboard Tester...\n", Color.Blue);
        }
        private void button13_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                string imgFolder = Path.Combine(Application.StartupPath, "img");

                if (!Directory.Exists(imgFolder))
                {
                    AppendText("Image folder not found! Cannot open Monitor Tester.", Color.Red);
                    return;
                }

                string[] files = Directory.GetFiles(imgFolder, "*.png");
                if (files.Length == 0)
                {
                    AppendText("No images found in the folder! Cannot open Monitor Tester.", Color.Red);
                    return;
                }

                MonitorTester LCDTest = new MonitorTester();
                LCDTest.Show();
                AppendText("Opening Monitor Tester...\n", Color.Blue);
            }
            catch (Exception)
            {
                AppendText("Failed to open Monitor Tester: ", Color.Red);
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            SoundCheck sCheck = new SoundCheck();
            sCheck.Show();
            AppendText("Opening Sound Tester...\n", Color.Blue);
        }
        private void button15_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            RecordSN recordSN = new RecordSN();
            recordSN.Show();
            AppendText("Opening Serial Number Recorder...\n", Color.Blue);
        }
        private void wifiConnectBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            DialogResult result = MessageBox.Show(
                "Do you want to connect to the current configured WiFi?\n\n" +
                "Click 'Yes' to use the existing connection.\n" +
                "Click 'No' to enter a custom SSID and Password.",
                "WiFi Connection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                new Thread(() => ConnectToCurrentWiFi()).Start();
            }
            else if (result == DialogResult.No)
            {
                new Thread(() => ConnectToCustomWiFi()).Start();
            }
        }
        private void ConnectToCurrentWiFi()
        {
            try
            {
                string ssid = GetCurrentWiFiSSID();
                if (string.IsNullOrEmpty(ssid))
                {
                    AppendText("No active WiFi connection found!\n", Color.Red);
                    return;
                }

                AppendText($"Reconnecting to {ssid}...\n", Color.Blue);
                RunCommand("netsh", $"wlan connect name=\"{ssid}\"");
            }
            catch (Exception ex)
            {
                AppendText($"WiFi Connection Error: {ex.Message}\n", Color.Red);
            }
        }
        private void ConnectToCustomWiFi()
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "settings.ini");
                string ssid = "", password = "";

                if (File.Exists(settingsPath))
                {
                    string[] settings = File.ReadAllLines(settingsPath);
                    if (settings.Length >= 2)
                    {
                        ssid = settings[0];
                        password = settings[1];
                    }
                }

                if (string.IsNullOrEmpty(ssid) || string.IsNullOrEmpty(password))
                {
                    using (var inputForm = new WiFiInputForm())
                    {
                        if (inputForm.ShowDialog() == DialogResult.OK)
                        {
                            ssid = inputForm.SSID;
                            password = inputForm.Password;

                            File.WriteAllText(settingsPath, $"{ssid}\n{password}");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                string profileName = $"{ssid}_Profile";
                string xml = $@"<?xml version=""1.0""?>
        <WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
            <name>{profileName}</name>
            <SSIDConfig>
                <SSID>
                    <name>{ssid}</name>
                </SSID>
            </SSIDConfig>
            <connectionType>ESS</connectionType>
            <connectionMode>auto</connectionMode>
            <MSM>
                <security>
                    <authEncryption>
                        <authentication>WPA2PSK</authentication>
                        <encryption>AES</encryption>
                        <useOneX>false</useOneX>
                    </authEncryption>
                    <sharedKey>
                        <keyType>passPhrase</keyType>
                        <protected>false</protected>
                        <keyMaterial>{password}</keyMaterial>
                    </sharedKey>
                </security>
            </MSM>
        </WLANProfile>";

                string tempPath = Path.GetTempPath();
                string xmlPath = Path.Combine(tempPath, $"{profileName}.xml");
                File.WriteAllText(xmlPath, xml);

                AppendText($"Creating WiFi profile for {ssid}...\n", Color.Blue);
                RunCommand("netsh", $"wlan add profile filename=\"{xmlPath}\"");

                AppendText($"Connecting to {ssid}...\n", Color.Green);
                RunCommand("netsh", $"wlan connect name=\"{profileName}\"");

                File.Delete(xmlPath);
            }
            catch (Exception ex)
            {
                AppendText($"WiFi Connection Error: {ex.Message}\n", Color.Red);
            }
        }
        private string GetCurrentWiFiSSID()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show interfaces",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    foreach (string line in output.Split('\n'))
                    {
                        if (line.Trim().StartsWith("SSID") && !line.Contains("BSSID"))
                        {
                            return line.Split(':')[1].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error retrieving current WiFi SSID: {ex.Message}\n", Color.Red);
            }
            return string.Empty;
        }
        private void RunCommand(string fileName, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                AppendText(result, Color.Black);
            }
        }
        private void DrvUptBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string driverBoosterPath = Path.Combine(programPath, "Driverupdt", "App", "DriverBooster", "DriverBooster.exe");

            if (File.Exists(driverBoosterPath))
            {
                Process.Start(driverBoosterPath);
                AppendText("Opening Driver Booster...\n", Color.Blue);
            }
            else
            {
                AppendText("DriverBooster.exe not found at: " + driverBoosterPath, Color.Red);

            }
        }
        private void button17_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            try
            {
                Process.Start("control");
                AppendText("Opening Control Panel...\n", Color.Blue);
            }
            catch (Exception ex)
            {
                AppendText($"Failed to open Control Panel: {ex.Message}", Color.Red);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string selectedEdition = officeEdition.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedEdition))
            {
                AppendText("Please select an Office edition.", Color.Red);
                return;
            }

            button16.Enabled = false;
            string url = GetOfficeUrl(selectedEdition);
            if (url == null)
            {
                AppendText("Invalid Office edition selected.", Color.Red);
                button16.Enabled = true;
                return;
            }

            string fileName = selectedEdition == "Libre Office" ? "LibreOffice.msi" : "OfficeSetup.exe";
            string downloadFilePath = Path.Combine(downloadPath, fileName);

            officeDownloadClient = new HttpClient();
            button16.Enabled = false;
            button21.Visible = true;

            officeCts = new CancellationTokenSource();

            button21.Click += OfficeInstallCancelHandler;
            Thread officeThread = new Thread(async () =>
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        richTextBox1.Clear();
                        progressBar2.Value = 0;
                        progressBar2.Style = ProgressBarStyle.Continuous;
                        AppendText($"Starting Office {selectedEdition} installation...\n", Color.Blue);
                    }));

                    Directory.CreateDirectory(downloadPath);

                    if (File.Exists(downloadFilePath))
                    {
                        try
                        {
                            File.Delete(downloadFilePath);
                            Invoke(new Action(() => AppendText($"Existing {fileName} deleted.\n", Color.Orange)));
                        }
                        catch (Exception deleteEx)
                        {
                            Invoke(new Action(() => AppendText($"Warning: Failed to delete existing file: {deleteEx.Message}\n", Color.Red)));
                        }
                    }

                    bool downloadSuccess = await DownloadOfficeInstallerAsync(url, downloadFilePath, officeCts.Token);
                    if (officeCts.IsCancellationRequested)
                    {
                        Invoke(new Action(() => AppendText("Office installation cancelled.\n", Color.Orange)));
                        return;
                    }

                    if (!downloadSuccess)
                    {
                        Invoke(new Action(() => AppendText("Download failed. Installation aborted.\n", Color.Red)));
                        return;
                    }

                    Invoke(new Action(() =>
                    {
                        progressBar2.Style = ProgressBarStyle.Marquee;
                        progressBar2.MarqueeAnimationSpeed = 30;
                        AppendText("Installing Office... Please wait.\n", Color.DarkCyan);
                    }));

                    bool installSuccess = RunOfficeInstaller(downloadFilePath);

                    Invoke(new Action(() =>
                    {
                        progressBar2.Style = ProgressBarStyle.Continuous;
                        progressBar2.Value = 100;
                        AppendText(installSuccess
                            ? "Office installation completed successfully.\n"
                            : "Office installation failed.\n",
                            installSuccess ? Color.Green : Color.Red);
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => AppendText($"Error: {ex.Message}\n", Color.Red)));
                }
                finally
                {
                    Invoke(new Action(() =>
                    {
                        button16.Enabled = true;
                        button21.Visible = false;
                    }));
                    officeCts?.Dispose();
                    officeCts = null;
                }
            })
            {
                IsBackground = true
            };
            officeThread.Start();
        }
        private void OfficeInstallCancelHandler(object sender, EventArgs e)
        {
            if (officeCts != null && !officeCts.IsCancellationRequested)
            {
                officeCts.Cancel();
                Invoke(new Action(() =>
                {
                    AppendText("Cancelling Office installation...\n", Color.Orange);
                    button21.Visible = false;
                }));
                button21.Click -= OfficeInstallCancelHandler;
            }
        }
        private string GetOfficeUrl(string edition)
        {
            switch (edition)
            {
                case "Microsoft 365 Home Premium":
                    return "https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=O365EduCloudRetail&platform=x64&language=en-us&version=O16GA";
                case "Microsoft 365 Education Cloud":
                    return "https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=O365HomePremRetail&platform=x64&language=en-us&version=O16GA";
                case "Office Home 2024":
                    return "https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=Home2024Retail&platform=x64&language=en-us&version=O16GA";
                case "Office Home and Student 2021":
                    return "https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=HomeStudent2021Retail&platform=x64&language=en-us&version=O16GA";
                case "Office Home and Student 2019":
                    return "https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=HomeStudent2019Retail&platform=x64&language=en-us&version=O16GA";
                case "Libre Office":
                    return "https://download.documentfoundation.org/libreoffice/stable/25.2.1/win/x86_64/LibreOffice_25.2.1_Win_x86-64.msi";
                default:
                    return null;
            }
        }
        private async Task<bool> DownloadOfficeInstallerAsync(string url, string destination, CancellationToken cancellationToken)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        Invoke(new Action(() => AppendText($"Download error: {response.ReasonPhrase}\n", Color.Red)));
                        return false;
                    }

                    long? totalBytes = response.Content.Headers.ContentLength;
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        byte[] buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                AppendText("Download cancelled by user.\n", Color.Orange);
                                return false;
                            }
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            int percent = totalBytes.HasValue ? (int)((totalRead * 100) / totalBytes.Value) : 0;
                            Invoke(new Action(() =>
                            {
                                progressBar2.Value = percent;
                                UpdateDownloadProgress(Path.GetFileName(destination), percent);
                            }));
                        }
                    }

                    Invoke(new Action(() =>
                    {
                        progressBar2.Value = 100;
                        AppendText("Download completed successfully.\n", Color.Green);
                    }));
                    return File.Exists(destination);
                }
            }
            catch (OperationCanceledException)
            {
                AppendText("Download was cancelled.\n", Color.Orange);
                return false;
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => AppendText($"Download error: {ex.Message}\n", Color.Red)));
                return false;
            }
        }
        private bool RunOfficeInstaller(string installerPath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                if (installerPath.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                {
                    psi.FileName = "msiexec.exe";
                    psi.Arguments = $"/i \"{installerPath}\" /qn";
                    AppendText($"Executing: {psi.FileName} {psi.Arguments}\n", Color.Blue);
                }
                else
                {
                    psi.FileName = installerPath;
                    AppendText($"Executing: {psi.FileName}\n", Color.Blue);
                }

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        AppendText("Installation completed successfully.\n", Color.Green);
                        return true;
                    }
                    else
                    {
                        AppendText($"Installation failed with exit code: {process.ExitCode}\n", Color.Red);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => AppendText($"Installation error: {ex.Message}\n", Color.Red)));
                return false;
            }
        }

        private void setclockBtn_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            SetTimeZoneToManila();
            SyncTimeWithInternet();
        }

        private void SetTimeZoneToManila()
        {
            try
            {
                AppendText("Setting time zone to Manila...\n", Color.Blue);

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command Set-TimeZone -Id 'Singapore Standard Time'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    AppendText(output, Color.Gray);
                    if (!string.IsNullOrEmpty(error))
                        AppendText(error + "\n", Color.Red);
                    else
                        AppendText("Time zone set to Manila (Singapore Standard Time)\n", Color.Green);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error setting time zone: {ex.Message}\n", Color.Red);
            }
        }
        private void SyncTimeWithInternet()
        {
            try
            {
                AppendText("\nSynchronizing time with NTP servers...\n", Color.Blue);
                if (!ManageTimeService(start: true))
                {
                    AppendText("Failed to start Windows Time service\n", Color.Red);
                    return;
                }
                using (Process configProcess = new Process())
                {
                    configProcess.StartInfo = new ProcessStartInfo
                    {
                        FileName = "w32tm",
                        Arguments = "/config /manualpeerlist:\"time.windows.com,time.nist.gov,pool.ntp.org\" /syncfromflags:manual /update",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    AppendText("Configuring time servers...\n", Color.Blue);
                    configProcess.Start();
                    string configOutput = configProcess.StandardOutput.ReadToEnd();
                    string configError = configProcess.StandardError.ReadToEnd();
                    configProcess.WaitForExit();

                    AppendText(configOutput, Color.Gray);
                    if (!string.IsNullOrEmpty(configError))
                        AppendText($"Configuration error: {configError}\n", Color.Red);
                }
                AppendText("\nForcing time synchronization...\n", Color.Blue);
                using (Process resyncProcess = new Process())
                {
                    resyncProcess.StartInfo = new ProcessStartInfo
                    {
                        FileName = "w32tm",
                        Arguments = "/resync",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };

                    resyncProcess.Start();
                    string resyncOutput = resyncProcess.StandardOutput.ReadToEnd();
                    string resyncError = resyncProcess.StandardError.ReadToEnd();
                    resyncProcess.WaitForExit();

                    if (resyncProcess.ExitCode == 0)
                    {
                        AppendText("Time synchronization successful\n", Color.Green);
                    }
                    else
                    {
                        AppendText($"Synchronization failed: {resyncError}\n", Color.Red);
                        AppendText("Attempting service recovery...\n", Color.Orange);
                        if (ManageTimeService(stop: true) && ManageTimeService(start: true))
                        {
                            AppendText("Service recovered, retrying sync...\n", Color.Blue);
                            SyncTimeWithInternet();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendText($"Error syncing time: {ex.Message}\n", Color.Red);
            }
        }
        private bool ManageTimeService(bool start = false, bool stop = false)
        {
            try
            {
                if (stop)
                {
                    AppendText("Stopping Windows Time service...\n", Color.Blue);
                    using (Process stopProcess = new Process())
                    {
                        stopProcess.StartInfo = new ProcessStartInfo
                        {
                            FileName = "net",
                            Arguments = "stop w32time",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        stopProcess.Start();
                        stopProcess.WaitForExit();
                    }
                }

                if (start)
                {
                    using (Process configProcess = new Process())
                    {
                        configProcess.StartInfo = new ProcessStartInfo
                        {
                            FileName = "sc",
                            Arguments = "config w32time start= auto",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        configProcess.Start();
                        configProcess.WaitForExit();
                    }

                    AppendText("Starting Windows Time service...\n", Color.Blue);
                    using (Process startProcess = new Process())
                    {
                        startProcess.StartInfo = new ProcessStartInfo
                        {
                            FileName = "net",
                            Arguments = "start w32time",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            Verb = "runas"
                        };

                        startProcess.Start();
                        string output = startProcess.StandardOutput.ReadToEnd();
                        string error = startProcess.StandardError.ReadToEnd();
                        startProcess.WaitForExit();

                        if (startProcess.ExitCode != 0)
                        {
                            AppendText($"Failed to start service: {error}\n", Color.Red);
                            return false;
                        }

                        AppendText(output, Color.Gray);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                AppendText($"Service management error: {ex.Message}\n", Color.Red);
                return false;
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            string zipPath = Path.Combine(Application.StartupPath, "apps", "performancetest.zip");
            if (!File.Exists(zipPath))
            {
                AppendText("PerformanceTest files not found!\n", Color.Red);
                return;
            }
            string tempDir = Path.Combine(Path.GetTempPath(), "PerformanceTest");
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }

                ZipFile.ExtractToDirectory(zipPath, tempDir);

                string exePath = Path.Combine(tempDir, "PerformanceTest64.exe");

                if (!File.Exists(exePath))
                {
                    AppendText("PerformanceTest executable not found after extraction!\n", Color.Red);
                    return;
                }

                AppendText("PerformanceTest successfully extracted and launching...\n", Color.Green);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true
                };
                Process process = Process.Start(psi);

                if (process != null)
                {
                    AppendText("PerformanceTest is now running...\n", Color.Blue);
                    process.WaitForExit();
                    AppendText("PerformanceTest has exited.\n", Color.DarkCyan);
                }

                Directory.Delete(tempDir, true);
                AppendText("Temporary files deleted successfully.\n", Color.Green);
            }
            catch (Exception ex)
            {
                AppendText($"Error launching PerformanceTest: {ex.Message}\n", Color.Red);
            }
        }
        private void oobe_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            DialogResult firstConfirm = MessageBox.Show(
                "Are you sure you want to run the OOBE setup? This will restart or shut down your computer.",
                "Confirm OOBE Setup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (firstConfirm == DialogResult.Yes)
            {
                DialogResult secondConfirm = MessageBox.Show(
                    "This action is irreversible and will restart or shut down your system immediately.\n\nDo you want to proceed?",
                    "Final Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation);
                if (secondConfirm == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c netsh wlan delete profile name=*",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });

                        DialogResult rebootOrShutdown = MessageBox.Show(
                            "Do you want to reboot or shut down?",
                            "Choose Action",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1);

                        string sysprepArgument = "";

                        if (rebootOrShutdown == DialogResult.Yes)
                        {
                            sysprepArgument = "/oobe /reboot";
                        }
                        else if (rebootOrShutdown == DialogResult.No)
                        {
                            sysprepArgument = "/oobe /shutdown";
                        }
                        else
                        {
                            return;
                        }

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Environment.ExpandEnvironmentVariables(@"%windir%\system32\sysprep\sysprep.exe"),
                            Arguments = sysprepArgument,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                    }
                    catch (Exception ex)
                    {
                        AppendText("Error executing commands: " + ex.Message, Color.Red);
                    }
                }
            }
        }
        private void button20_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            AppendText("Starting system maintenance...\n", Color.Blue);
            new Thread(() =>
            {
                AppendText("Starting system file check...\n", Color.Blue);
                RunPowerShellCommand("sfc /scannow", true);

                AppendText("Checking Windows image health...\n", Color.Blue);
                RunPowerShellCommand("DISM /Online /Cleanup-Image /RestoreHealth", true);

                AppendText("Flushing DNS and resetting network settings...\n", Color.Blue);
                RunPowerShellCommand("ipconfig /flushdns", true);
                RunPowerShellCommand("netsh winsock reset", true);
                RunPowerShellCommand("netsh int ip reset", true);

                AppendText("Resetting Windows Update components...\n", Color.Blue);
                RunPowerShellCommand("net stop wuauserv", true);
                RunPowerShellCommand("net stop cryptSvc", true);
                RunPowerShellCommand("net stop bits", true);
                RunPowerShellCommand("net stop msiserver", true);
                RunPowerShellCommand("ren C:\\Windows\\SoftwareDistribution SoftwareDistribution.old", true);
                RunPowerShellCommand("ren C:\\Windows\\System32\\catroot2 catroot2.old", true);
                RunPowerShellCommand("net start wuauserv", true);
                RunPowerShellCommand("net start cryptSvc", true);
                RunPowerShellCommand("net start bits", true);
                RunPowerShellCommand("net start msiserver", true);

                AppendText("Repairing Windows components...\n", Color.Blue);
                RunPowerShellCommand("DISM /Online /Cleanup-Image /RestoreHealth", true);
                RunPowerShellCommand("DISM /Online /Cleanup-Image /StartComponentCleanup", true);

                AppendText("Restarting Windows Explorer...\n", Color.Blue);
                RunPowerShellCommand("taskkill /f /im explorer.exe", true);
                RunPowerShellCommand("start explorer.exe", true);

                AppendText("System maintenance completed successfully!\n", Color.Green);
            }).Start();
        }
        private void LhanzCJ_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillProcesses();
        }
        private void button21_Click(object sender, EventArgs e)
        {

            try
            {
                installcts.Cancel();
                KillProcesses();

                if (officeCts != null && !officeCts.IsCancellationRequested)
                {
                    officeCts.Cancel();
                    button21.Click -= OfficeInstallCancelHandler;
                }
                Invoke(new Action(() =>
                {
                    AppendText("Installation cancelled by user.\n", Color.Orange);
                    button21.Visible = false;
                    progressBar1.Value = 0;
                    progressBar2.Value = 0;
                }));
            }
            catch (Exception ex)
            {
                AppendText($"Error cancelling installation: {ex.Message}\n", Color.Red);
            }

        }
        private void button22_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
    }
}