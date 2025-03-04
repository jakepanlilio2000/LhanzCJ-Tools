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
    - Install essential programs automatically using **Webclient** for freshly installed desktops and laptops.

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

The following essential programs are installed via **Webclient**:

- 7-Zip
- Google Chrome
- Zoom
- Adobe Acrobat Reader
- VLC Media Player
- Spotify
- .Net 4.8.1
- Visual C++ Redistributable AIO
- DirectX

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

Let me know if you want me to add **winget command examples** or a section for future updates.
