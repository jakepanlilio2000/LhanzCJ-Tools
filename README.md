---

# Technician Toolbox for Lhanz CJ

This project is a comprehensive **Technician Toolbox** designed specifically for **Lhanz CJ Trading and Computer Center**. It provides technicians with a set of essential tools to test hardware components, access system settings, install necessary programs, and manage Windows configurations, all from a single application interface.

---

![{5C38E8D9-455A-4EBA-9C2F-21FDBFFDA3F2}](https://github.com/user-attachments/assets/94fe6a46-fa5f-488f-ad07-e2f3261ddf34)
![{B899FE25-EA23-49B7-840F-3E731AD48B07}](https://github.com/user-attachments/assets/f5bee822-1244-4c99-b5c0-47ee6885e9a4)



## Features

- **Built-in Hardware Testers**  
    - Keyboard Tester – Check if all keys are functional.  
    - Monitor Tester – Easily test display colors and patterns.  
    - Sound Tester – Play sound files to test audio output.  
    - Webcam Tester – Check the system's webcam feed.  
    - **Memory Checker** by [lordmulder](https://github.com/lordmulder/MemoryChecker) – Test and validate system memory (RAM).  
    - **Hard Disk Validator** by [TalAloni](https://github.com/TalAloni/HardDiskValidator) – Validate and test hard disk health and performance.  
    - **Mic Tester** by [Sajeeb Chandan Saha](https://github.com/sajeebchandan/MicTest) – Capture and analyze microphone input.

- **System Shortcuts**  
    - Desktop Icon Settings shortcut  
    - Windows Update shortcut  
    - Windows Features shortcut  
    - Office Setup shortcut  
    - Windows License Key input shortcut
    - Setup [OOBE](https://learn.microsoft.com/en-us/windows-hardware/customize/desktop/oobexml-in-windows-11)
    - Update Driver
    - [Official Office Downloader](https://support.microsoft.com/en-us/topic/office-deployment-tool-9fbd53e3-18a3-1aef-8cfe-e2eaeeeaaa4c)
    - dxdiag Shortcut
    - Device Manager shortcut
    - Control Panel shortcut
    - Syncing Clock (+8 GMT)

- **Network Access & License Management**  
    - Pre-configured **Network Path** for technicians to quickly access shared resources  
    - Record and manage Windows license keys for internal tracking and technical support purposes

- **Automated Software Installation**  
    - Install essential programs automatically using **Webclient** for freshly installed desktops and laptops.  
    - **Winget Integration** – Update and install programs using the Windows Package Manager (`winget`).

---

## Technologies Used

- C# (Windows Forms/WPF)
- NAudio (for audio input/output testing)
- OpenCvSharp4 (for webcam functionality)
- OpenCvSharp4.Extensions
- OpenCvSharp4.runtime.win

---

## Prerequisites

Before running the project, ensure the following **NuGet packages** are installed:

| Package | Description |
|---|---|
| **NAudio** | For handling audio (mic and speaker testing) |
| **OpenCvSharp4** | Core library for webcam capture |
| **OpenCvSharp4.Extensions** | Extension methods for easier OpenCV integration |
| **OpenCvSharp4.runtime.win** | Windows-specific runtime for OpenCV |

---

## Getting Started

1. Clone the Repository
    ```bash
    git clone https://github.com/jakepanlilio2000/LhanzCJ-Tools.git
    ```

2. Install Dependencies  
   Open **NuGet Package Manager** and install:

    - NAudio
    - OpenCvSharp4
    - OpenCvSharp4.Extensions
    - OpenCvSharp4.runtime.win

3. Run the Application  
   Build and run the project through **Visual Studio**.

---

## Winget Integration

The toolbox now uses **Windows Package Manager (winget)** to update and install programs. This ensures that all installed software is up-to-date and compatible with the latest system requirements.

### Winget Commands
The following commands are used to update and install programs:

```bash
# Update all installed programs
winget upgrade --all --accept-source-agreements --accept-package-agreements

# Install Visual C++ Redistributables
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2005.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2005.x64"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2008.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2008.x64"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2010.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2010.x64"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2012.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2012.x64"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2013.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2013.x64"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2015+.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2015+.x64"
```

---

## Included Tools and Credits to:

### Memory Checker
- **Description**: A tool to test and validate system memory (RAM).  
- **Source**: [lordmulder/MemoryChecker](https://github.com/lordmulder/MemoryChecker)  
- **Usage**: Run `MemoryChecker.exe` from the `apps` folder.

### Hard Disk Validator
- **Description**: A tool to validate and test hard disk health and performance.  
- **Source**: [TalAloni/HardDiskValidator](https://github.com/TalAloni/HardDiskValidator)  
- **Usage**: Run `HardDiskValidator.exe` from the `apps` folder.

### Mic Tester
- **Description**: A tool to capture and analyze microphone input.
- **Source**: [Sajeeb Chandan Saha/Mictest](https://github.com/sajeebchandan/MicTest)  
- **Usage**: Run `Mic Test` integrated to the system.
---

## License

Technician Toolbox for Lhanz CJ Company - Hardware Diagnostics and System Management Suite  
Copyright (C) 2025 Jake Panlilio

This program is free software: you can redistribute it and/or modify  
it under the terms of the GNU General Public License as published by  
the Free Software Foundation, either version 3 of the License, or  
(at your option) any later version.

This program is distributed in the hope that it will be useful,  
but WITHOUT ANY WARRANTY; without even the implied warranty of  
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the  
GNU General Public License for more details.

You should have received a copy of the GNU General Public License  
along with this program.  If not, see <https://www.gnu.org/licenses/>.

---

Contributions and suggestions are welcome for future improvements.

---

Let me know if you need further updates or additional details!