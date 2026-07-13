$ErrorActionPreference = "Stop"

# Necromancer Phase 4: specialized Grimoires. The dominated creatures decide WHAT you
# raise; the equipped Grimoire decides HOW the whole army behaves. Same collection, wildly
# different playstyles.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"
$g = Join-Path $c "Items\Weapons\Summoner"

$iface = Get-Content -Raw (Join-Path $c "Necromancy\ISpecializedGrimoire.cs")
$base = Get-Content -Raw (Join-Path $g "SpecializedGrimoire.cs")
$player = Get-Content -Raw (Join-Path $c "Players\NecromancerPlayer.cs")
$minion = Get-Content -Raw (Join-Path $c "Projectiles\Necromancer\BaseNecroMinion.cs")

# --- The modifier contract ---------------------------------------------------
foreach ($m in @("SummonDamageMult", "ReserveMult", "ManaMult", "MoveSpeedMult",
                 "SizeMult", "DefenseDelta", "Lifesteal", "OnHitDebuff",
                 "BossEchoMult", "CommonMult")) {
    if ($iface -notmatch $m) {
        throw "ISpecializedGrimoire should expose the '$m' modifier."
    }
}

# The base handles raising/cycling and provides neutral virtual modifiers.
if ($base -notmatch "abstract class SpecializedGrimoire\s*:\s*ModItem,\s*ISpecializedGrimoire" -or
    $base -notmatch "EnsureActive\(" -or
    $base -notmatch "virtual float SummonDamageMult") {
    throw "SpecializedGrimoire should be the raising base with neutral virtual modifiers."
}

# --- 10 grimoires, each overriding a modifier and craftable ------------------
$grimoires = @{
    "GrimoireOfDeath"   = $null
    "WarGrimoire"       = "SummonDamageMult => 1.25f"
    "SacrificeGrimoire" = "ReserveMult => 0.6f"
    "ArcaneGrimoire"    = "ManaMult => 0.6f"
    "CommanderGrimoire" = "MoveSpeedMult => 1.4f"
    "LichGrimoire"      = "Lifesteal => true"
    "PlagueGrimoire"    = "OnHitDebuff => BuffID.Venom"
    "SwarmGrimoire"     = "ReserveMult => 0.5f"
    "TitanGrimoire"     = "SizeMult => 1.6f"
    "VoidGrimoire"      = "OnHitDebuff => BuffID.ShadowFlame"
    "DeadKingGrimoire"  = "BossEchoMult => 2f"
}

foreach ($pair in $grimoires.GetEnumerator()) {
    $path = Join-Path $g "$($pair.Key).cs"
    if (!(Test-Path $path)) {
        throw "Missing grimoire '$($pair.Key)'."
    }
    $src = Get-Content -Raw $path

    if ($src -notmatch ":\s*SpecializedGrimoire") {
        throw "$($pair.Key) should extend SpecializedGrimoire."
    }
    if ($src -notmatch "CreateRecipe\(\)") {
        throw "$($pair.Key) should be craftable."
    }
    if ($null -ne $pair.Value -and $src -notmatch [regex]::Escape($pair.Value)) {
        throw "$($pair.Key) should override its signature modifier ($($pair.Value))."
    }
}

# --- Player applies the active Grimoire (reserve/mana/defense) ----------------
if ($player -notmatch "ISpecializedGrimoire ActiveGrimoire" -or
    $player -notmatch "ReserveMult" -or
    $player -notmatch "ManaMult" -or
    $player -notmatch "DefenseDelta") {
    throw "NecromancerPlayer should apply the equipped Grimoire's reserve/mana/defense mods."
}

# --- Undead apply the active Grimoire (damage/lifesteal/debuff/boss split) ----
if ($minion -notmatch "ModifyHitNPC" -or
    $minion -notmatch "SummonDamageMult" -or
    $minion -notmatch "IsBossEcho \? g\.BossEchoMult : g\.CommonMult") {
    throw "Undead should scale their damage with the Grimoire (boss echoes split out)."
}
if ($minion -notmatch "g\.Lifesteal" -or $minion -notmatch "g\.OnHitDebuff") {
    throw "Undead should apply the Grimoire's lifesteal and on-hit debuff."
}

# Boss echoes are flagged.
$slime = Get-Content -Raw (Join-Path $c "Projectiles\Necromancer\GuardianSlimeMinion.cs")
if ($slime -notmatch "IsBossEcho => true") {
    throw "Boss echoes (Guardian Slime) should set IsBossEcho."
}

Write-Host "Necromancer grimoire variants source smoke test passed."
