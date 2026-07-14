$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- Ferocity mechanic + passive shaping -------------------------------------

$player = Read-File "Content\Players\BeastTamerPlayer.cs"

foreach ($member in @("public float Ferocity", "const float MaxFerocity", "int FrenzyTimer", "IsActiveBeastTamer")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "BeastTamerPlayer should expose '$member'."
    }
}

# Only the pack builds Ferocity; a full bar unleashes Primal Roar.
if ($player -notmatch "proj\.minion" -or $player -notmatch "Ferocity\s*\+=") {
    throw "Only minions/summon projectiles should build Ferocity."
}

if ($player -notmatch "PRIMAL ROAR" -or $player -notmatch "FrenzyTimer\s*=") {
    throw "A full Ferocity bar should unleash Primal Roar."
}

# Beast branch shapes the mechanic.
foreach ($node in @("Wild Bond", "Feral Roar", "Primal Instinct", "Savage Alpha", "Alpha Beast", "Bloodhound")) {
    if ($player -notmatch "HasBeast\(`"$node`"\)") {
        throw "Beast node '$node' should shape the Ferocity mechanic in BeastTamerPlayer."
    }
}

if ($player -notmatch "HasKeystone\(`"Apex Alpha`"\)") {
    throw "The Apex Alpha keystone should empower the frenzy."
}

# --- Minion infrastructure (shared buff pattern) -----------------------------

$buff = Read-File "Content\Buffs\BeastMinionBuff.cs"
if ($buff -notmatch "class BeastMinionBuff\s*:\s*ModBuff" -or $buff -notmatch "BeastMinionActive") {
    throw "BeastMinionBuff should keep the pack alive via BeastMinionPlayer.BeastMinionActive."
}

$mplayer = Read-File "Content\Players\BeastMinionPlayer.cs"
if ($mplayer -notmatch "BeastMinionActive" -or $mplayer -notmatch "ResetEffects") {
    throw "BeastMinionPlayer should reset BeastMinionActive each frame."
}

$minionBase = Read-File "Content\Projectiles\Summoner\BeastMinion.cs"
if ($minionBase -notmatch "abstract class BeastMinion\s*:\s*ModProjectile") {
    throw "BeastMinion should be the base minion projectile."
}
if ($minionBase -notmatch "Projectile\.minion\s*=\s*true" -or $minionBase -notmatch "minionSlots" -or
    $minionBase -notmatch "DamageClass\.Summon") {
    throw "BeastMinion should be a summon-damage slot minion."
}
if ($minionBase -notmatch "MinionContactDamage" -or $minionBase -notmatch "BeastMinionBuff") {
    throw "BeastMinion should deal contact damage and stay alive via the shared buff."
}

# --- Beasts + staves ---------------------------------------------------------

$beasts = @("WolfMinion", "BoarMinion", "RaptorMinion", "BearMinion", "SabertoothMinion", "WyvernMinion")
foreach ($b in $beasts) {
    $src = Read-File "Content\Projectiles\Summoner\$b.cs"
    if ($src -notmatch "class $b\s*:\s*BeastMinion") {
        throw "$b should extend BeastMinion."
    }
}

$staffBase = Read-File "Content\Items\Weapons\Summoner\BeastStaff.cs"
if ($staffBase -notmatch "abstract class BeastStaff\s*:\s*ModItem" -or
    $staffBase -notmatch "AddBuff\(ModContent\.BuffType<BeastMinionBuff>" -or
    $staffBase -notmatch "NewProjectileDirect") {
    throw "BeastStaff should summon its minion and apply the shared buff."
}

$staves = @{
    "WolfFangTotem" = "WolfMinion"; "BoarhideTotem" = "BoarMinion"; "RaptorTalon" = "RaptorMinion";
    "UrsineTotem" = "BearMinion"; "SabertoothFang" = "SabertoothMinion"; "Wyrmcaller" = "WyvernMinion"
}
foreach ($staff in $staves.Keys) {
    $src = Read-File "Content\Items\Weapons\Summoner\$staff.cs"
    if ($src -notmatch "class $staff\s*:\s*BeastStaff") {
        throw "$staff should extend BeastStaff."
    }
    if ($src -notmatch $staves[$staff]) {
        throw "$staff should summon a $($staves[$staff])."
    }
}

# --- Whip arsenal: the Summoner's own weapon, pre-HM through endgame ---------

$whipBase = Read-File "Content\Items\Weapons\Summoner\SummonerWhip.cs"
if ($whipBase -notmatch "abstract class SummonerWhip\s*:\s*ModItem" -or
    $whipBase -notmatch "SummonMeleeSpeed" -or $whipBase -notmatch "WhipProjectile") {
    throw "SummonerWhip should be the base for every whip."
}

# Pre-Hardmode whips are open to ANY Summoner (the Beast Tamer does not exist yet).
$preHardmodeWhips = @{
    "LeatherLash" = "LeatherWhipProjectile"; "ThornLash" = "ThornWhipProjectile";
    "BloodfangWhip" = "BloodfangWhipProjectile"; "MoltenLash" = "MoltenWhipProjectile"
}
foreach ($whip in $preHardmodeWhips.Keys) {
    $item = Read-File "Content\Items\Weapons\Summoner\$whip.cs"
    if ($item -notmatch "class $whip\s*:\s*SummonerWhip" -or $item -notmatch $preHardmodeWhips[$whip]) {
        throw "$whip should be a SummonerWhip shooting $($preHardmodeWhips[$whip])."
    }

    $proj = Read-File "Content\Projectiles\Summoner\$($preHardmodeWhips[$whip]).cs"
    if ($proj -notmatch "BaseEterniaWhipProjectile") {
        throw "$($preHardmodeWhips[$whip]) should extend BaseEterniaWhipProjectile."
    }
    if ($proj -match "RequiredSubclass") {
        throw "$($preHardmodeWhips[$whip]) is pre-Hardmode and must NOT be locked to a subclass."
    }
}

# Hardmode whips are Beast Tamer gear.
$hardmodeWhips = @{
    "BeastcallerWhip" = "BeastWhipProjectile"; "FeralLash" = "FeralWhipProjectile";
    "SavageLash" = "SavageWhipProjectile"; "AlphasLash" = "AlphaWhipProjectile"
}
foreach ($whip in $hardmodeWhips.Keys) {
    $item = Read-File "Content\Items\Weapons\Summoner\$whip.cs"
    if ($item -notmatch "class $whip\s*:\s*SummonerWhip" -or $item -notmatch $hardmodeWhips[$whip]) {
        throw "$whip should be a SummonerWhip shooting $($hardmodeWhips[$whip])."
    }

    $proj = Read-File "Content\Projectiles\Summoner\$($hardmodeWhips[$whip]).cs"
    if ($proj -notmatch "RequiredSubclass" -or $proj -notmatch "Beast Tamer") {
        throw "$($hardmodeWhips[$whip]) should be Beast Tamer gear."
    }
}

Write-Host "Beast Tamer source smoke test passed."
