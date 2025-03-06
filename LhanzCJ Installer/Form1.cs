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


namespace LhanzCJ_Installer
{


    public partial class LhanzCJ : Form
    {
        private const int WM_USER = 0x0400;
        private const int EM_GETSCROLLPOS = WM_USER + 221;
        private const int EM_SETSCROLLPOS = WM_USER + 222;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateProcessWithTokenW(
        IntPtr hToken,
        uint dwLogonFlags,
        string lpApplicationName,
        string lpCommandLine,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            int ImpersonationLevel,
            int TokenType,
            out IntPtr phNewToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_DUPLICATE = 0x0002;
        private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private const int SecurityImpersonation = 2;
        private const int TokenPrimary = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        private bool StartProcessAsUser(string filePath, string arguments)
        {
            IntPtr hToken = WindowsIdentity.GetCurrent().Token;
            IntPtr hDupedToken = IntPtr.Zero;

            try
            {
                if (!DuplicateTokenEx(
                    hToken,
                    TOKEN_QUERY | TOKEN_DUPLICATE | TOKEN_ASSIGN_PRIMARY,
                    IntPtr.Zero,
                    SecurityImpersonation,
                    TokenPrimary,
                    out hDupedToken))
                {
                    return false;
                }

                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(si);
                PROCESS_INFORMATION pi;

                string commandLine = $"\"{filePath}\" {arguments}";

                bool result = CreateProcessWithTokenW(
                    hDupedToken,
                    0,
                    null,
                    commandLine,
                    0,
                    IntPtr.Zero,
                    null,
                    ref si,
                    out pi);

                if (result)
                {
                    CloseHandle(pi.hProcess);
                    CloseHandle(pi.hThread);
                    return true;
                }
                return false;
            }
            finally
            {
                if (hDupedToken != IntPtr.Zero)
                    CloseHandle(hDupedToken);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        private const int SB_VERT = 1;

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
            button16.Enabled = isAdmin;
            button14.Enabled = isAdmin;
            wifiConnectBtn.Enabled = isAdmin;
            DrvUptBtn.Enabled = isAdmin;
            setclockBtn.Enabled = isAdmin;
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
                MessageBox.Show("Failed to open technical folder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                new InstallStep("VCRedistAIO", "VCRedistAIO.zip", "https://github.com/abbodi1406/vcredist/releases/download/v0.87.0/VisualCppRedist_AIO_x86_x64_87.zip", ""),
                new InstallStep("7zip", "7zip.exe", "https://www.7-zip.org/a/7z2409-x64.exe", "/S"),
                new InstallStep("Spotify", "Spotify.exe", "https://download.scdn.co/SpotifySetup.exe", "/S", runAsAdmin: false),
                new InstallStep("Zoom", "zoom.exe", "https://zoom.us/client/6.3.11.60501/ZoomInstallerFull.exe", "zoom.exe /silent"),
                new InstallStep("Google Chrome", "chrome.exe", "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7BF3CAB977-BF22-F629-B1C4-B9D9234DDFD5%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dprefers%26ap%3Dx64-stable/update2/installers/ChromeSetup.exe", "chrome.exe /silent /install"),
                new InstallStep("VLC", "vlc.exe", "https://get.videolan.org/vlc/3.0.21/win64/vlc-3.0.21-win64.exe", "vlc.exe /S"),
                new InstallStep("Acrobat Reader", "acrobat.exe", "https://ardownload2.adobe.com/pub/adobe/reader/win/AcrobatDC/2400520414/AcroRdrDC2400520414_en_US.exe", "/sAll /rs /l /msi /qb- /L*v"),
                new InstallStep("Winget CLI", "winget.msixbundle", "https://aka.ms/getwinget", "/q", runAsAdmin: true),

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
                        currentDownloadFile = step.FileName;

                        Invoke(new Action(() =>
                        {
                            progressBar2.Value = 0;
                            progressBar2.Style = ProgressBarStyle.Continuous;
                        }));

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
                            else
                            {
                                installationSuccess = RunInstaller(filePath, step.InstallArgs, step.RunAsAdmin);
                            }

                            if (step.Name == "Winget CLI" && installationSuccess)
                            {
                                AppendText("Verifying Winget installation...\n", Color.Blue);
                                if (!IsWingetInstalled())
                                {
                                    AppendText("Winget installation failed - trying manual registration...\n", Color.Red);
                                    installationSuccess = RegisterWingetManually(filePath);
                                }

                                if (installationSuccess)
                                {
                                    AppendText("Updating packages via Winget...\n", Color.Blue);
                                    UpdatePackagesWithWinget();
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
                            if (step.Name == "Winget CLI")
                            {
                                AppendText("Installing Winget CLI...\n", Color.Blue);
                                installationSuccess = InstallWinget(filePath);

                                if (installationSuccess)
                                {
                                    AppendText("Updating packages via Winget...\n", Color.Blue);
                                    UpdatePackagesWithWinget();
                                }
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
                catch (Exception ex)
                {
                    AppendText($"Error: {ex.Message}\n", Color.Red);
                }
                finally
                {
                    Invoke(new Action(() => button5.Enabled = true));
                }
            });

            thread.Start();
        }
        private bool InstallWinget(string msixPath)
        {
            string command = $"Add-AppxPackage -Path '{msixPath}'";
            return RunPowerShellCommand(command);
        }

        private void UpdatePackagesWithWinget()
        {
            string command = "winget upgrade --all --accept-source-agreements --accept-package-agreements";
            RunPowerShellCommand(command, true);
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
                    if (lastLine.Contains("%") || lastLine.Contains("/")) // Progress line pattern
                    {
                        richTextBox1.Select(lastLineStart + 1, richTextBox1.TextLength - (lastLineStart + 1));
                        richTextBox1.SelectedText = $"[{DateTime.Now:HH:mm:ss}] {progress}";
                        return;
                    }
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {progress}\n");
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
                        richTextBox1.SelectedText = $"[{DateTime.Now:HH:mm:ss}] {text}";
                        return;
                    }
                }

                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}\n");
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
                            AppendText($"Download error: {e.Error.Message}\n", Color.Red);
                            downloadCompleted.Set();
                        }
                        else if (e.Cancelled)
                        {
                            AppendText($"Download cancelled.\n", Color.DarkGray);
                            downloadCompleted.Set();
                        }
                        else
                        {
                            UpdateDownloadProgress(Path.GetFileName(destination), 100);
                            AppendText($"Downloading {Path.GetFileName(destination)}: Completed\n", Color.DarkBlue);
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
                AppendText($"Download error: {ex.Message}\n", Color.Red);
                return false;
            }
        }

        private Dictionary<string, string> lastProgressMessages = new Dictionary<string, string>();
        private string currentDownloadFile;
        private void UpdateDownloadProgress(string fileName, int percent)
        {
            string progressMessage = $"Downloading {fileName}: {percent}%";
            if (fileName != currentDownloadFile)
                return;

            Invoke(new Action(() =>
            {
                progressBar2.Value = percent;
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
            }));

        }

        private bool RunInstaller(string filePath, string arguments, bool runAsAdmin)
        {
            try
            {
                if (runAsAdmin)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = arguments,
                        UseShellExecute = true,
                        Verb = "runas",
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(psi))
                    {
                        process.WaitForExit();
                        return process.ExitCode == 0;
                    }
                }
                else
                {
                    // Run non-elevated using CreateProcessWithTokenW
                    return StartProcessAsUser(filePath, arguments);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Installer error: {ex.Message}\n", Color.Red);
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
                        AppendText($"Error deleting existing directory: {ex.Message}\n", Color.Red);
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
                richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}");
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
        private bool IsWingetInstalled()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "winget",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output.ToLower().Contains("winget");
                }
            }
            catch
            {
                return false;
            }
        }

        private bool RegisterWingetManually(string msixPath)
        {
            string command = $@"Add-AppxPackage -Path '{msixPath}' -ErrorAction Stop;
                        if (-not (Get-Command winget -ErrorAction SilentlyContinue)) {{
                            Write-Error 'Winget installation failed' -ErrorAction Stop
                        }}";

            return RunPowerShellCommand(command);
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
                    AppendText($"Error checking .NET Framework version: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking DirectX: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking VC++ Redistributables: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking 7-Zip: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking Spotify: {ex.Message}\n", Color.Red);
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
                    AppendText($"Error checking VLC: {ex.Message}\n", Color.Red);
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
            public bool RunAsAdmin { get; }

            public InstallStep(string name, string fileName, string downloadUrl, string installArgs, bool runAsAdmin = true)
            {
                Name = name;
                FileName = fileName;
                DownloadUrl = downloadUrl;
                InstallArgs = installArgs;
                RunAsAdmin = runAsAdmin;
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
                Process.Start(new ProcessStartInfo
                {
                    FileName = "microsoft.windows.camera:",
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                DialogResult result = MessageBox.Show(
                    "An error occurred while trying to open the Camera app. Open Camera Tester instead?",
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

            //CamTest camTest = new CamTest();
            //camTest.Show();
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
                string autoplayKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers";
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(autoplayKeyPath))
                {
                    if (key != null)
                    {
                        key.SetValue("DisableAutoplay", 1, RegistryValueKind.DWord);
                    }
                }

                MessageBox.Show("AutoPlay has been disabled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to disable AutoPlay: {ex.Message}");
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:autoplay",
                UseShellExecute = true
            });
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

        private void wifiConnectBtn_Click(object sender, EventArgs e)
        {
            new Thread(() => ConnectToWiFi()).Start();
        }

        private void ConnectToWiFi()
        {
            try
            {
                string ssid = "Test_Wifi 2.4G";
                string password = "admin123";
                string profileName = "Test_Wifi_Profile";

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

                richTextBox1.Clear();
                AppendText($"Creating WiFi profile for {ssid}...\n", Color.Blue);
                RunCommand("netsh", $"wlan add profile filename=\"{xmlPath}\"");

                AppendText($"Connecting to {ssid}...\n", Color.Green);
                RunCommand("netsh", $"wlan connect name=\"{profileName}\"");

                File.Delete(xmlPath);
            }
            catch (Exception ex)
            {
                richTextBox1.Clear();
                AppendText($"WiFi Connection Error: {ex.Message}\n", Color.Red);

            }
        }

        private void RunCommand(string command, string args)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    AppendText(output + "\n", Color.Black);
                }
            }
            catch (Exception ex)
            {
                AppendText($"Command Error ({command}): {ex.Message}\n", Color.Red);
            }
        }

        private void DrvUptBtn_Click(object sender, EventArgs e)
        {
            string programPath = AppDomain.CurrentDomain.BaseDirectory;
            string driverBoosterPath = Path.Combine(programPath, "Driverupdt", "App", "DriverBooster", "DriverBooster.exe");

            if (File.Exists(driverBoosterPath))
            {
                Process.Start(driverBoosterPath);
            }
            else
            {
                MessageBox.Show("DriverBooster.exe not found at: " + driverBoosterPath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("control");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Control Panel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string selectedEdition = officeEdition.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedEdition))
            {
                MessageBox.Show("Please select an Office edition.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            button16.Enabled = false;
            string url = GetOfficeUrl(selectedEdition);
            if (url == null)
            {
                MessageBox.Show("Invalid Office edition selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button16.Enabled = true;
                return;
            }

            string fileName = "OfficeSetup.exe";
            string downloadFilePath = Path.Combine(downloadPath, fileName);

            Thread officeThread = new Thread(() =>
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
                            Invoke(new Action(() => AppendText("Existing OfficeSetup.exe deleted.\n", Color.Orange)));
                        }
                        catch (Exception deleteEx)
                        {
                            Invoke(new Action(() => AppendText($"Warning: Failed to delete existing file: {deleteEx.Message}\n", Color.Red)));
                        }
                    }

                    // Download the installer
                    bool downloadSuccess = DownloadOfficeInstaller(url, downloadFilePath);

                    if (!downloadSuccess)
                    {
                        Invoke(new Action(() => AppendText("Download failed. Installation aborted.\n", Color.Red)));
                        return;
                    }

                    // Run the installer
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
                    Invoke(new Action(() => button16.Enabled = true));
                }
            });

            officeThread.IsBackground = true;
            officeThread.Start();
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
                default:
                    return null;
            }
        }

        private bool DownloadOfficeInstaller(string url, string destination)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    ManualResetEvent downloadCompleted = new ManualResetEvent(false);
                    bool success = false;

                    client.DownloadProgressChanged += (s, e) =>
                    {
                        int percent = e.ProgressPercentage;
                        Invoke(new Action(() =>
                        {
                            progressBar2.Value = percent;
                            UpdateDownloadProgressMessage(percent, Path.GetFileName(destination));
                        }));
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            Invoke(new Action(() => AppendText($"Download error: {e.Error.Message}\n", Color.Red)));
                            success = false;
                        }
                        else if (e.Cancelled)
                        {
                            Invoke(new Action(() => AppendText("Download cancelled.\n", Color.DarkGray)));
                            success = false;
                        }
                        else
                        {
                            success = true;
                            Invoke(new Action(() =>
                            {
                                progressBar2.Value = 100;
                                AppendText("Download completed successfully.\n", Color.Green);
                            }));
                        }
                        downloadCompleted.Set();
                    };

                    Invoke(new Action(() => AppendText($"Downloading Office installer...\n", Color.Blue)));
                    client.DownloadFileAsync(new Uri(url), destination);
                    downloadCompleted.WaitOne();

                    return success && File.Exists(destination);
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => AppendText($"Download error: {ex.Message}\n", Color.Red)));
                return false;
            }
        }

        private void UpdateDownloadProgressMessage(int percent, string fileName)
        {
            string progressMessage = $"{fileName}: {percent}%";
            bool wasAtBottom = IsScrollAtBottom();

            Invoke(new Action(() =>
            {
                SendMessageW(richTextBox1.Handle, 0x000B, (IntPtr)0, IntPtr.Zero);

                try
                {
                    int lastLineIndex = richTextBox1.Text.LastIndexOf($"[{DateTime.Now:HH:mm:ss}]");
                    if (lastLineIndex != -1)
                    {
                        int startIndex = richTextBox1.Text.LastIndexOf('\n', lastLineIndex) + 1;
                        richTextBox1.Select(startIndex, richTextBox1.TextLength - startIndex);
                        richTextBox1.SelectedText = $"[{DateTime.Now:HH:mm:ss}] {progressMessage}";
                    }
                    else
                    {
                        richTextBox1.AppendText($"[{DateTime.Now:HH:mm:ss}] {progressMessage}\n");
                    }

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
            }));
        }

        private bool RunOfficeInstaller(string installerPath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true,
                    Verb = "runas", 
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                    return process.ExitCode == 0;
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
            SetTimeZoneToManila();
            SyncTimeWithInternet();
        }

        private void SetTimeZoneToManila()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "tzutil.exe";
                process.StartInfo.Arguments = "/s \"Singapore Standard Time\""; 
                process.StartInfo.Verb = "runas"; 
                process.StartInfo.UseShellExecute = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set time zone. Please run the application as Administrator.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SyncTimeWithInternet()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C w32tm /resync";
                process.StartInfo.Verb = "runas"; 
                process.StartInfo.UseShellExecute = true;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to sync time. Please run the application as Administrator.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}