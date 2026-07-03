$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$sharedPath = Join-Path $repoRoot "Content\UI\EterniaUI.cs"
$content = Get-Content -Raw $sharedPath

$headerMatch = [regex]::Match(
    $content,
    'public static void DrawHeader\([\s\S]+?\n\s*public static bool DrawButton\(',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if (-not $headerMatch.Success) {
    throw "Could not locate EterniaUI.DrawHeader."
}

$header = $headerMatch.Value

if ($header -notmatch "DrawTrimmedText") {
    throw "DrawHeader should trim title/subtitle text so narrow panels cannot overflow into the brand pill."
}

if ($header -notmatch "textMaxWidth") {
    throw "DrawHeader should calculate a textMaxWidth that reserves room for the brand pill."
}

if ($header -match "Math\.Max\(64,\s*panel\.Width - 164\)") {
    throw "DrawHeader should not force the old fixed 164px brand reservation on narrow panels."
}

if ($header -notmatch "showBrandPill" -or
    $header -notmatch "brandReservedWidth" -or
    $header -notmatch "brandWidth") {
    throw "DrawHeader should calculate compact brand visibility/width before laying out title text."
}

if ($header -notmatch "if\s*\(\s*showBrandPill\s*\)") {
    throw "DrawHeader should skip the brand pill when the panel is too narrow."
}

$centeredMatch = [regex]::Match(
    $content,
    'public static void DrawCenteredText\([^}]+?\n\s*}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline)

if (-not $centeredMatch.Success) {
    throw "Could not locate EterniaUI.DrawCenteredText."
}

$centered = $centeredMatch.Value

if ($centered -notmatch "FitText\(") {
    throw "DrawCenteredText should fit labels before measuring, so buttons and progress bars cannot draw outside their bounds."
}

if ($content -notmatch "private static string FitText\(") {
    throw "EterniaUI should provide a shared FitText helper for centered labels."
}

Write-Host "Eternia UI typography source smoke test passed."
