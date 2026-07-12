$ErrorActionPreference = "Stop"

# v1 scope: the passive tree shows only 3 subclass branches per base class. The rest
# are HIDDEN (not deleted) via a single allowlist in PassiveUI, so re-enabling a
# subclass later is just editing that set. This pins the v1 allowlist to source.

$repoRoot = Split-Path -Parent $PSScriptRoot
$ui = Get-Content -Raw (Join-Path $repoRoot "Content\UI\PassiveUI.cs")

# --- The allowlist exists and both draw paths honour it ----------------------
if ($ui -notmatch "V1VisibleAffinities") {
    throw "PassiveUI should gate visible branches through a V1VisibleAffinities allowlist."
}
if ($ui -notmatch "IsAffinityVisible") {
    throw "PassiveUI should expose an IsAffinityVisible helper."
}

# GroupPassivesByAffinity (the radial branches) must filter by visibility.
$group = [regex]::Match(
    $ui,
    "private static List<AffinityGroup> GroupPassivesByAffinity\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($group -notmatch "IsAffinityVisible") {
    throw "GroupPassivesByAffinity should hide non-v1 branches (filter by IsAffinityVisible)."
}

# GetAffinities (the sidebar meters) must filter by visibility too.
$affinities = [regex]::Match(
    $ui,
    "private static IEnumerable<AffinityInfo> GetAffinities\([\s\S]+?\n {8}\}",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value
if ($affinities -notmatch "IsAffinityVisible") {
    throw "GetAffinities should hide non-v1 affinity meters (filter by IsAffinityVisible)."
}

# --- The allowlist contents: 3 per base class --------------------------------
$set = [regex]::Match(
    $ui,
    "V1VisibleAffinities\s*=\s*new HashSet<string>\s*\{[\s\S]+?\};",
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

$visible = @(
    "Bleed", "Combo", "Defense",       # Warrior: Swordsman / Fighter / Guardian
    "Elemental",                       # Mage: Elementalist affinity meter...
    "Fire", "Ice", "Lightning", "Wind", "Earth",  # ...and its 5 element sub-branches
    "Curse", "Infinity",               # Mage: Cursed Mage / Infinity Mage
    "Energy", "Bow", "Gun",            # Ranger
    "Beast", "Tech", "Shadow"          # Summoner
)
$hidden = @(
    "Precision", "Rage", "Control",    # Warrior: Yoyo / Berserker / Stunner
    "Arcane",                          # Mage: Arcane Bard
    "Music",                           # Ranger: Virtuoso
    "Fusion"                           # Summoner: Advanced Summoner
)

foreach ($a in $visible) {
    if ($set -notmatch ('"' + $a + '"')) {
        throw "v1 allowlist should include the shipped branch '$a'."
    }
}
foreach ($a in $hidden) {
    if ($set -match ('"' + $a + '"')) {
        throw "v1 allowlist must NOT include the hidden branch '$a'."
    }
}

Write-Host "Passive tree v1 visibility source smoke test passed."
