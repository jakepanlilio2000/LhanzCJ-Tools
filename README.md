# Technician Toolbox for Lhanz CJ

This project is a comprehensive **Technician Toolbox** designed specifically for **Lhanz CJ Trading and Computer Center**. It provides technicians with a set of essential tools to test hardware components, access system settings, install necessary programs, and manage Windows configurations, all from a single application interface.

---
![Toolbox Interface](https://github.com/user-attachments/assets/a4dce38f-d43e-4ac4-be17-637081068987)
![Software Installation](https://github.com/user-attachments/assets/2c7aa053-093d-457d-95e1-8ef1b5c8e0a9)

## Features

### Core Functionalities
- **System Configuration Tools**
  - **Initial Setup Wizard**  
    1. Enable desktop icons (Computer, Recycle Bin, Control Panel)  
    2. Disable AutoPlay  
    3. Clear taskbar cache  
    4. Remove taskbar shortcuts (except File Explorer)  
    5. Disable Windows Widgets  
    6. Restart Windows Explorer  
    7. Enable SMB1.0/CIFS file sharing

  - **Network Configuration**  
    1. Set registry keys for insecure guest authentication  
    2. Configure network signature requirements  
    3. Quick access to technical network shares  
    4. Custom network path input

  - **Windows Features**  
    1. Launch OptionalFeatures.exe  
    2. Manage Windows components/features  
    3. Toggle 32-bit redirection for legacy systems

### Hardware Diagnostics
- **Built-in Testers**  
  - **Keyboard Tester** - Full keyboard matrix verification  
  - **Monitor Tester** - Display color patterns and gradient tests  
  - **Sound Tester** - Audio output verification with waveform visualization  
  - **Webcam Tester** - Real-time camera feed with OpenCV  
  - **Mic Tester** - Audio input analysis with dB monitoring  

### System Management
- **Automated Maintenance**  
  1. SFC/DISM system scans  
  2. Network configuration reset  
  3. Windows Update component repair  
  4. System image cleanup  
  5. Explorer process management

- **Software Deployment**  
  - **Bulk Installation**  
    1. .NET Frameworks (5-9)  
    2. Essential utilities (7-Zip, VLC, Chrome)  
    3. Video conferencing tools (Zoom)  
    4. Document readers (Acrobat DC)  

  - **Office Deployment**  
    1. Multiple edition support (2019-2024, M365)  
    2. LibreOffice integration  
    3. Silent installation options  

### Technician Shortcuts
- **Quick Access Tools**  
  - One-click access to:  
    - Device Manager  
    - DirectX Diagnostics  
    - Control Panel  
    - Windows Activation  
    - System Clock Sync (UTC+8)  
    - OOBE Configuration  

- **Driver Management**  
  1. Driver Booster integration  
  2. Driver update automation  
  3. Legacy driver support  

## Technologies Used

- C# (Windows Forms/WPF)
- NAudio (for audio input/output testing)
- OpenCvSharp4 (for webcam functionality)
- Windows Package Manager (winget integration)
- Advanced registry manipulation
- System service management

## Enhanced Winget Integration

The toolbox leverages **Windows Package Manager** for comprehensive software management:

```bash
# Automated Package Updates
winget upgrade --all --accept-source-agreements --accept-package-agreements

# Visual C++ Redistributables Installation
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2005.x86"
winget install --exact --locale "en-US" --id="Microsoft.VCRedist.2005.x64"
... (additional VC++ versions)
```

## License Management

- Windows License tracking system
- Volume license configuration
- OEM key preservation tools
- License transfer utilities

## Prerequisites

| Package | Purpose |
|---|---|
| **NAudio** | Audio input/output analysis |
| **OpenCvSharp4** | Webcam feed processing |
| **OpenCvSharp4.runtime.win** | Native OpenCV bindings |

## Getting Started

1. Clone Repository
   ```bash
   git clone https://github.com/jakepanlilio2000/LhanzCJ-Tools.git
   ```

2. Install Dependencies via NuGet:
   ```powershell
   Install-Package NAudio
   Install-Package OpenCvSharp4
   ```

3. Build & Run in Visual Studio

---

## Credits & Acknowledgments

- **Mic Tester Core** by [Sajeeb Chandan Saha](https://github.com/sajeebchandan/MicTest)  
- Microsoft Winget Team  
- OpenCV Community  

---

**GNU GPLv3 License** | Copyright © 2025 Jake Panlilio  
