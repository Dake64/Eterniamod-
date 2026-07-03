$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$keybindPath = Join-Path $repoRoot "Content\Systems\EterniaKeybinds.cs"
$content = Get-Content -Raw $keybindPath

$matches = [regex]::Matches(
    $content,
    'KeybindLoader\.RegisterKeybind\(\s*Mod,\s*"([^"]+)",\s*"([^"]+)"\s*\)',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if ($matches.Count -lt 7) {
    throw "Expected all Eternia keybind registrations to use explicit default keys."
}

$defaults = @{}

foreach ($match in $matches) {
    $name = $match.Groups[1].Value
    $key = $match.Groups[2].Value

    if ($defaults.ContainsKey($key)) {
        throw "Duplicate default key '$key' for '$($defaults[$key])' and '$name'."
    }

    $defaults[$key] = $name
}

if ($content -notmatch '"Elemental Ultimate",\s*"Z"') {
    throw "Elemental Ultimate should default to Z so it does not collide with Class Skill on Q."
}

foreach ($localizationPath in @(
    (Join-Path $repoRoot "Localization\en-US_Mods.ETERNIA.hjson"),
    (Join-Path $repoRoot "en-US.hjson"))) {
    $localization = Get-Content -Raw $localizationPath

    if ($localization -match "Class  Skill|Change  Note|Elemental  Ultimate|Cursed  Burst|Toggle  Passive|Toggle  Stats|Toggle  Soul|U I") {
        throw "Keybind localization in $localizationPath should not contain generated double-spaced labels."
    }

    $localizedKeybinds = [regex]::Matches(
        $localization,
        '"([^"]+)\.DisplayName"\s*:',
        [System.Text.RegularExpressions.RegexOptions]::Singleline) |
        ForEach-Object { $_.Groups[1].Value } |
        Sort-Object -Unique

    foreach ($name in $defaults.Values) {
        if ($localizedKeybinds -notcontains $name) {
            throw "Keybind localization in $localizationPath is missing '$name.DisplayName'."
        }
    }

    foreach ($name in $localizedKeybinds) {
        if ($defaults.Values -notcontains $name) {
            throw "Keybind localization in $localizationPath contains stale keybind '$name.DisplayName'."
        }
    }
}

Write-Host "Keybind defaults source smoke test passed."
