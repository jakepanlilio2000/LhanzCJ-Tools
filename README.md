---

# Technician Toolbox for Lhanz CJ Company

This project is a comprehensive **Technician Toolbox** designed specifically for **Lhanz CJ Company**. It provides technicians with a set of essential tools to test hardware components, access system settings, install necessary programs, and manage Windows configurations, all from a single application interface.

---

## Features

- **Built-in Hardware Testers**  
    - Keyboard Tester – Check if all keys are functional.  
    - Monitor Tester – Easily test display colors and patterns.  
    - Sound Tester – Play sound files to test audio output.  
    - Webcam Tester – Check the system's webcam feed.  
    - Mic Tester – (Credits to [**Sajeeb Chandan Saha**](https://github.com/sajeebchandan/MicTest)) – Capture and analyze microphone input.

- **System Shortcuts**  
    - Desktop Icon Settings shortcut  
    - Windows Update shortcut  
    - Windows Features shortcut  
    - Office Setup shortcut  
    - Windows License Key input shortcut

- **Network Access & License Management**  
    - Pre-configured **Network Path** for technicians to quickly access shared resources  
    - Record and manage Windows license keys for internal tracking and technical support purposes

- **Automated Software Installation**  
    - Install essential programs automatically using **winget** (PowerShell) for freshly installed desktops and laptops.

---

## Technologies Used

- C# (Windows Forms/WPF)
- PowerShell (winget)
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
    git clone https://github.com/yourusername/technician-toolbox-lhanz-cj.git
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

This project uses **winget** for software installation automation.  
Ensure **winget** is installed on your system:  
[Install winget](https://learn.microsoft.com/en-us/windows/package-manager/winget/)

### Programs Installed via Winget

The following essential programs are installed via **winget**:

- 7-Zip
- Google Chrome
- Zoom
- Adobe Acrobat Reader
- VLC Media Player
- Visual C++ Redistributable AIO
- DirectX

---

## License

This project is intended for internal use within **Lhanz CJ Company**.  
Credits to **Sajeeb Chandan Saha** for the Microphone Tester component.

---

## Author

**Lhanz CJ Company - Technician Tools Team**  
Contributions and suggestions are welcome for future improvements.

---

Let me know if you want me to add **winget command examples** or a section for future updates.
