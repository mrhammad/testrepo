# Windows Installer Guide (OrderWise ERP)

This guide packages the WPF app as a shareable Windows installer (`.exe`) you can copy to other machines.

## What gets created

- Published app files: `artifacts\publish\win-x64\`
- Installer output: `artifacts\installer\OrderWiseERP-Setup-<version>.exe`

## Prerequisites (Windows build machine)

1. **.NET 8 SDK** installed
2. **Inno Setup 6** installed (compiler `iscc.exe`)
   - Default path used by scripts:
     - `C:\Program Files (x86)\Inno Setup 6\iscc.exe`
  - If installed elsewhere, pass `-InnoSetupCompilerPath` in PowerShell script.

## One-command build

From repository root:

### Option A: PowerShell (recommended)

```powershell
.\scripts\build-installer.ps1
```

Optional version override:

```powershell
.\scripts\build-installer.ps1 -Version 1.0.1
```

### Option B: Batch wrapper

```bat
scripts\build-installer.bat
```

## Installer behavior

- Installs to: `Program Files\OrderWise ERP`
- Creates Start Menu shortcut
- Optional Desktop shortcut
- Launches app after install (unless silent mode)

## Notes

- The published build is **self-contained** (`win-x64`) so target machines do not need .NET runtime preinstalled.
- Current app data path stays user-local:
  - `%LOCALAPPDATA%\OrderWiseErp\phase1-data.json`
  - `%LOCALAPPDATA%\OrderWiseErp\settings.json`

