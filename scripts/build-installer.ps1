param(
    [string]$Version = "1.0.0",
    [ValidateSet("win-x64", "win-arm64")]
    [string]$Runtime = "win-x64",
    [switch]$SelfContained = $true,
    [switch]$SingleFile = $true,
    [switch]$CleanOutput = $true,
    [string]$InnoSetupCompilerPath = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot "src/OrderWiseErp.App/OrderWiseErp.App.csproj"
$publishDir = Join-Path $repoRoot "artifacts/publish/$Runtime"
$installerScript = Join-Path $repoRoot "installer/OrderWiseErp.iss"

if (!(Test-Path $projectPath)) {
    throw "Project file not found at: $projectPath"
}

if (!(Test-Path $installerScript)) {
    throw "Installer script not found at: $installerScript"
}

if ($CleanOutput) {
    if (Test-Path $publishDir) {
        Remove-Item -Recurse -Force $publishDir
    }
}

Write-Host "Publishing OrderWise ERP ($Runtime)..." -ForegroundColor Cyan

$publishArgs = @(
    "publish", $projectPath,
    "-c", "Release",
    "-r", $Runtime,
    "-o", $publishDir,
    "/p:PublishSingleFile=$($SingleFile.IsPresent.ToString().ToLower())",
    "/p:IncludeNativeLibrariesForSelfExtract=true",
    "/p:PublishTrimmed=false",
    "/p:Version=$Version"
)

if ($SelfContained) {
    $publishArgs += "--self-contained"
    $publishArgs += "true"
} else {
    $publishArgs += "--self-contained"
    $publishArgs += "false"
}

& dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE"
}

Write-Host "Looking for Inno Setup compiler..." -ForegroundColor Cyan

$isccCandidates = @(
    "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe",
    "${env:ProgramFiles}\Inno Setup 6\ISCC.exe"
)

if (-not [string]::IsNullOrWhiteSpace($InnoSetupCompilerPath)) {
    if (!(Test-Path $InnoSetupCompilerPath)) {
        throw "Provided Inno Setup compiler path not found: $InnoSetupCompilerPath"
    }

    $isccPath = $InnoSetupCompilerPath
} else {
    $isccPath = $isccCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}

if (-not $isccPath) {
    $isccPath = (Get-Command "iscc.exe" -ErrorAction SilentlyContinue)?.Source
}

if (-not $isccPath) {
    throw "Inno Setup compiler (ISCC.exe) not found. Install Inno Setup 6 first."
}

Write-Host "Building installer via ISCC..." -ForegroundColor Cyan

& $isccPath `
    "/DAppVersion=$Version" `
    "/DAppRuntime=$Runtime" `
    "/DPublishDir=$publishDir" `
    $installerScript

if ($LASTEXITCODE -ne 0) {
    throw "ISCC failed with exit code $LASTEXITCODE"
}

$installerOutputDir = Join-Path $repoRoot "artifacts/installer"
Write-Host "Installer build complete." -ForegroundColor Green
Write-Host "Output folder: $installerOutputDir" -ForegroundColor Green
