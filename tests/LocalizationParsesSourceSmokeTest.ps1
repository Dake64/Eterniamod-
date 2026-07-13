$ErrorActionPreference = "Stop"

# en-US.hjson must actually PARSE as Hjson, not just contain the right keys. This loads
# it with the same Hjson library tModLoader uses, so a malformed file (e.g. an inline
# `{ a: b, c: d }` object, which Hjson mis-reads because unquoted strings run to EOL)
# fails here instead of at mod load.

$repoRoot = Split-Path -Parent $PSScriptRoot
$hjsonPath = Join-Path $repoRoot "en-US.hjson"

# --- Resolve the tModLoader install (for Hjson.dll) --------------------------
$tmlDir = $null

if ($env:TML_INSTALL_DIR -and (Test-Path (Join-Path $env:TML_INSTALL_DIR "tModLoader.dll"))) {
    $tmlDir = $env:TML_INSTALL_DIR
}

if (-not $tmlDir) {
    $targetsPath = Join-Path (Split-Path -Parent $repoRoot) "tModLoader.targets"
    if (Test-Path $targetsPath) {
        $targetsText = Get-Content -Raw $targetsPath
        $importMatch = [regex]::Match($targetsText, 'Project="([^"]*tMLMod\.targets)"')
        if ($importMatch.Success) {
            $candidate = Split-Path -Parent $importMatch.Groups[1].Value
            if (Test-Path (Join-Path $candidate "tModLoader.dll")) {
                $tmlDir = $candidate
            }
        }
    }
}

if (-not $tmlDir) {
    foreach ($candidate in @(
        "C:\Program Files (x86)\Steam\steamapps\common\tModLoader",
        "D:\steam\steamapps\common\tModLoader",
        "D:\Steam\steamapps\common\tModLoader",
        "E:\Steam\steamapps\common\tModLoader")) {
        if (Test-Path (Join-Path $candidate "tModLoader.dll")) {
            $tmlDir = $candidate
            break
        }
    }
}

if (-not $tmlDir) {
    Write-Host "SKIP: tModLoader install not found (set TML_INSTALL_DIR). Skipping localization parse test."
    exit 0
}

$hjsonDll = (Get-ChildItem -Path (Join-Path $tmlDir "Libraries") -Filter "Hjson.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1).FullName

if (-not $hjsonDll) {
    Write-Host "SKIP: Hjson.dll not found under $tmlDir. Skipping localization parse test."
    exit 0
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "SKIP: dotnet SDK not found. Skipping localization parse test."
    exit 0
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("eternia-hjson-test-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot | Out-Null

try {
    $programPath = Join-Path $tempRoot "Program.cs"
    $projectPath = Join-Path $tempRoot "LocalizationParsesSmokeTest.csproj"

    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Hjson">
      <HintPath>$hjsonDll</HintPath>
      <Private>true</Private>
    </Reference>
  </ItemGroup>
</Project>
"@ | Set-Content -Path $projectPath -Encoding UTF8

    @"
using Hjson;

try
{
    HjsonValue.Load(@`"$hjsonPath`");
    Console.WriteLine(`"en-US.hjson parsed OK.`");
}
catch (Exception e)
{
    Console.Error.WriteLine(`"en-US.hjson is malformed: `" + e.Message);
    Environment.Exit(1);
}
"@ | Set-Content -Path $programPath -Encoding UTF8

    dotnet run --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "en-US.hjson failed to parse as Hjson."
    }
}
finally {
    if (Test-Path $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}

Write-Host "Localization parse source smoke test passed."
