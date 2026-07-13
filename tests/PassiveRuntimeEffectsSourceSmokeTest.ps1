$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$statsPath = Join-Path $repoRoot "Content\Players\EterniaStatsPlayer.cs"
$necroPath = Join-Path $repoRoot "Content\Players\NecromancerPlayer.cs"

$stats = Get-Content -Raw $statsPath
$necro = Get-Content -Raw $necroPath

$expectedPassives = @(
    "Energy Core",
    "Overcharge",
    "Plasma Reactor",
    "Bow Precision",
    "Eagle Eye",
    "Hunter Instinct",
    "Quick Trigger",
    "Rapid Chamber",
    "Deadshot",
    "Musical Soul",
    "Resonance",
    "Symphony Master",
    "Kindling",
    "Ember Fury",
    "Pyromancer",
    "Frost Touch",
    "Deep Freeze",
    "Absolute Zero",
    "Static Charge",
    "Chain Master",
    "Tempest",
    "Zephyr",
    "Gale Force",
    "Tempest Winds",
    "Stone Skin",
    "Tremor",
    "Tectonic",
    "Dark Ritual",
    "Forbidden Hex",
    "Cursed Blood",
    "Infinite Pages",
    "Endless Wisdom",
    "Limit Break",
    "Arcane Melody",
    "Mystic Chorus",
    "Grand Orchestra",
    "Wild Bond",
    "Alpha Beast",
    "Primal Instinct",
    "Fusion Mind",
    "Perfect Fusion",
    "Ultimate Fusion",
    "Tech Protocol",
    "Combat AI",
    "War Machine",
    "Necrotic Pact",
    "Bone Conduit",
    "Grave Legion"
)

foreach ($passive in $expectedPassives) {
    $pattern = 'HasActivePassive\(soulPlayer\.ActiveSoul,\s*"' +
        [regex]::Escape($passive) +
        '"\)'

    if ($stats -notmatch $pattern) {
        throw "EterniaStatsPlayer should apply runtime effects for passive '$passive'."
    }
}

if ($necro -notmatch 'HasActivePassive\(\s*soul\.ActiveSoul,\s*"Bone Conduit"\s*\)') {
    throw "NecromancerPlayer should make Bone Conduit affect the reserved-life mechanic."
}

if ($necro -notmatch 'ease \*= 0\.8f') {
    throw "Bone Conduit should ease the Necromancer's reserved-life toll."
}

Write-Host "Passive runtime effects source smoke test passed."
