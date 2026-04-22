#ifndef AppName
#define AppName "OrderWise ERP"
#endif

#ifndef AppVersion
#define AppVersion "1.0.0"
#endif

#ifndef AppPublisher
#define AppPublisher "OrderWise ERP"
#endif

#ifndef AppExeName
#define AppExeName "OrderWiseErp.App.exe"
#endif

#ifndef PublishDir
#error "PublishDir define is required. Example: /DPublishDir=C:\path\to\publish"
#endif

#ifndef InstallerOutputDir
#define InstallerOutputDir "artifacts\installer"
#endif

[Setup]
AppId={{8A7A81D2-5367-4D2F-9221-09E39D88DA16}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir={#InstallerOutputDir}
OutputBaseFilename=OrderWiseERP-Setup-{#AppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#AppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop icon"; GroupDescription: "Additional icons:"

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent
