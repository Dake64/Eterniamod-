$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$player = Read-File "Content\Players\SoulAscensionPlayer.cs"
$item = Read-File "Content\Items\Souls\SoulAscension.cs"
$classSoul = Read-File "Content\Items\Souls\ClassSoulItem.cs"
$reforge = Read-File "Content\Progression\ProgressionService.cs"
$loc = Read-File "en-US.hjson"

# --- A permanent, capped, per-class tier -------------------------------------

if ($player -notmatch "const int MaxTier" -or $player -notmatch "int SoulTier") {
    throw "SoulAscensionPlayer should have a capped SoulTier."
}

# Ascension must be gated by the cap.
if ($player -notmatch "SoulTier\s*>=\s*MaxTier") {
    throw "Ascend() should refuse past the cap."
}

# The bonus applies to the ACTIVE class's damage type, and only with a class Soul.
if ($player -notmatch "ClassOf\(" -or $player -notmatch "HasClassSoul") {
    throw "Ascension bonus should apply to the active class Soul only."
}

if ($player -notmatch "GetDamage\(dc\)" -or $player -notmatch "statLifeMax2" -or $player -notmatch "statDefense") {
    throw "Each tier should grant class damage, max life and defense."
}

# Persisted with the character.
if ($player -notmatch "SaveData" -or $player -notmatch "LoadData" -or $player -notmatch "SoulTier") {
    throw "SoulTier should be saved with the character."
}

# --- Independent from the respec ---------------------------------------------
# A Soul Reforge wipes passives/affinities but must NOT touch ascension.

if ($reforge -match "SoulTier" -or $reforge -match "SoulAscension") {
    throw "ProgressionService (respec) must not reset Soul Ascension -- it is a separate, permanent path."
}

# --- The consumable: the sink for Soul Alloy ---------------------------------

if ($item -notmatch "consumable\s*=\s*true") {
    throw "SoulAscension should be a consumable."
}

if ($item -notmatch "CanAscend") {
    throw "SoulAscension should refuse when maxed or with no class Soul."
}

if ($item -notmatch "\.Ascend\(\)") {
    throw "SoulAscension should raise the Soul tier on use."
}

if ($item -notmatch "AddIngredient<SoulAlloy>" -or $item -notmatch "TileID\.DemonAltar") {
    throw "SoulAscension should be a Soul Alloy ritual at the Demon Altar."
}

# --- Surfaced to the player --------------------------------------------------

if ($classSoul -notmatch "SoulAscension" -and $classSoul -notmatch "Soul Ascension") {
    throw "The class Soul tooltip should show the current Ascension tier."
}

# --- Soul Alloy now has a purpose (no longer 'reserved') ---------------------

if ($loc -match "uses still to come") {
    throw "Soul Alloy should no longer be described as having no use."
}

if ($loc -notmatch "(?ms)SoulAscension:\s*\{[^}]*DisplayName:" -and $loc -notmatch "SoulAscension\.DisplayName:") {
    throw "en-US.hjson should localize Soul Ascension."
}

Write-Host "Soul Ascension source smoke test passed."
