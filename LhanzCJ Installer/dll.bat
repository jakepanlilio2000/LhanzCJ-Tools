@echo off

:: this script needs https://www.nuget.org/packages/ilmerge

:: set your target executable name (typically [projectname].exe)
SET APP_NAME=LhanzCJInstaller.exe

:: Set build, used for directory. Typically Release or Debug
SET ILMERGE_BUILD=Release
:: Set platform, typically x64
SET ILMERGE_PLATFORM=x64

:: set your NuGet ILMerge Version, this is the number from the package manager install, for example:
:: PM> Install-Package ilmerge -Version 3.0.29
:: to confirm it is installed for a given project, see the packages.config file
SET ILMERGE_VERSION=3.0.29

:: the full ILMerge should be found here:
SET ILMERGE_PATH=C:\Users\Admin\Source\Repos\LhanzCJ-Tools\packages\ILMerge.3.0.41\tools\net452
:: dir "%ILMERGE_PATH%"\ILMerge.exe

echo Merging %APP_NAME% ...

:: add project DLL's starting with replacing the FirstLib with this project's DLL
"%ILMERGE_PATH%"\ILMerge.exe Bin\%ILMERGE_PLATFORM%\%ILMERGE_BUILD%\%APP_NAME%  ^
  /lib:Bin\%ILMERGE_PLATFORM%\%ILMERGE_BUILD%\ ^
  /out:%APP_NAME% ^
  AxInterop.WMPLib.dll ^
  CSCore.dll ^
  Interop.WMPLib.dll ^
  Microsoft.Win32.Registry.dll ^
  NAudio.Asio.dll ^
  NAudio.Core.dll ^
  NAudio.dll ^
  NAudio.Midi.dll ^
  NAudio.Wasapi.dll ^
  NAudio.WinForms.dll ^
  NAudio.WinMM.dll ^
  Newtonsoft.Json.dll ^
  OpenCvSharp.dll ^
  OpenCvSharp.Extensions.dll ^
  System.Buffers.dll ^
  System.Drawing.Common.dll ^
  System.Memory.dll ^
  System.Numerics.Vectors.dll ^
  System.Runtime.CompilerServices.Unsafe.dll ^
  System.Security.AccessControl.dll ^
  System.Security.Principal.Windows.dll ^
  System.ValueTuple.dll 


:Done
dir %APP_NAME%