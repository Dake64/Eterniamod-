$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- The reset itself --------------------------------------------------------

$service = Read-File "Content\Progression\ProgressionService.cs"

if ($service -notmatch "public static int ResetPassives\(Player player\)") {
    throw "ProgressionService should expose ResetPassives -- the only way out of a build."
}

$reset = [regex]::Match(
    $service,
    "public static int ResetPassives\(Player player\)[\s\S]*?\n        \}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

# It must WIPE the passives...
if ($reset -notmatch "UnlockedPassives\.Clear\(\)") {
    throw "ResetPassives must clear the unlocked passives."
}

# ...zero the affinities (they are what decide your subclass)...
if ($reset -notmatch "ClearAffinities\(stats\)") {
    throw "ResetPassives must zero the affinities -- they decide the subclass."
}

# ...and refund EXACTLY what the passives cost, not a flat guess.
if ($reset -notmatch "refunded\s*\+=\s*node\.Cost") {
    throw "ResetPassives must refund each passive's real Cost."
}

if ($reset -notmatch "level\.passivePoints\s*\+=\s*refunded") {
    throw "ResetPassives must hand the refunded points back to the passive-point pool."
}

# Every affinity that AddAffinity can raise must be cleared, or a stale affinity
# would keep forcing the old subclass after a reforge.
$affinities = @(
    "Bleed", "Combo", "Defense", "Precision", "Rage", "Control",
    "Energy", "Bow", "Gun", "Music",
    "Elemental", "Curse", "Infinity", "Arcane",
    "Beast", "Fusion", "Tech", "Shadow")

$clear = [regex]::Match(
    $service,
    "private static void ClearAffinities\(EterniaStatsPlayer stats\)[\s\S]*?\n        \}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

foreach ($a in $affinities) {
    if ($clear -notmatch "stats\.$($a)Affinity\s*=\s*0") {
        throw "ClearAffinities must zero $($a)Affinity, or a reforge would leave you stuck in the old subclass."
    }
}

# --- The item ----------------------------------------------------------------

$item = Read-File "Content\Items\Souls\SoulReforge.cs"

if ($item -notmatch "class SoulReforge\s*:\s*ModItem") {
    throw "SoulReforge should be the respec item."
}

if ($item -notmatch "Item\.consumable\s*=\s*true") {
    throw "SoulReforge should be consumed, so every change of heart costs the grind again."
}

if ($item -notmatch "ProgressionService\.ResetPassives\(player\)") {
    throw "SoulReforge should perform the reset."
}

# It must refuse to be wasted.
if ($item -notmatch "CanUseItem" -or $item -notmatch "UnlockedPassives\.Count > 0") {
    throw "SoulReforge should refuse to be used when there is nothing to refund."
}

# Deliberately expensive.
if ($item -notmatch "AddRecipes" -or $item -notmatch "SoulofLight" -or
    $item -notmatch "SoulofNight" -or $item -notmatch "EmptySoul") {
    throw "SoulReforge should be an expensive ritual craft."
}

Write-Host "Soul Reforge source smoke test passed."
