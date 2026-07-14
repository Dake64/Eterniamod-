$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

# --- LEGION / COMMAND / OVERCLOCK + Fusion passive shaping --------------------

$player = Read-File "Content\Players\AdvancedSummonerPlayer.cs"

foreach ($member in @("public float Command", "const float MaxCommand", "int OverclockTimer", "IsActiveAdvancedSummoner")) {
    if ($player -notmatch [regex]::Escape($member)) {
        throw "AdvancedSummonerPlayer should expose '$member'."
    }
}

# LEGION: damage scales with how full the roster is.
if ($player -notmatch "slotsMinions\s*/\s*cap") {
    throw "LEGION should scale summon damage with how full the minion roster is."
}

# OVERCLOCK raises the cap.
if ($player -notmatch "OVERCLOCK" -or $player -notmatch "Player\.maxMinions\s*\+=") {
    throw "Overclock should temporarily raise the minion cap."
}

foreach ($node in @("Perfect Fusion", "Ultimate Fusion", "Synchronized Assault", "Transcendent Fusion", "Overdrive", "Singularity")) {
    if ($player -notmatch "HasFusion\(`"$node`"\)") {
        throw "Fusion node '$node' should shape the Legion/Overclock mechanic."
    }
}

if ($player -notmatch "HasKeystone\(`"Living Swarm`"\)") {
    throw "The Living Swarm keystone should pay off past a full roster."
}

# --- Legion minions: half-slot swarm -----------------------------------------

$minionBase = Read-File "Content\Projectiles\Summoner\LegionMinion.cs"

if ($minionBase -notmatch "abstract class LegionMinion\s*:\s*ModProjectile") {
    throw "LegionMinion should be the base legion minion."
}

if ($minionBase -notmatch "minionSlots\s*=\s*0\.5f") {
    throw "Legion minions should cost half a minion slot."
}

if ($minionBase -notmatch "LegionCount" -or $minionBase -notmatch "SwarmBonusPer") {
    throw "Legion minions should get stronger the more of them are alive (swarm synergy)."
}

$legionPlayer = Read-File "Content\Players\LegionMinionPlayer.cs"
if ($legionPlayer -notmatch "ReportLegionMinion") {
    throw "LegionMinionPlayer should tally the live legion each frame."
}

$buff = Read-File "Content\Buffs\LegionMinionBuff.cs"
if ($buff -notmatch "class LegionMinionBuff\s*:\s*ModBuff") {
    throw "LegionMinionBuff should keep a mixed legion summoned."
}

# --- OBTENTION: the legion is FUSED from summons you already own --------------
# (Beast Tamer TAMES; Tech Summoner ASSEMBLES parts; Advanced Summoner FUSES summons.)

$staves = @{
    "WispLantern" = "WispMinion"; "SpiritBanner" = "SpiritSoldierMinion";
    "ConstructCore" = "ArcaneConstructMinion"; "FusionMatrix" = "FusionGolemMinion";
    "SentinelBeacon" = "ArcSentinelMinion"; "SingularityCore" = "SingularityWraithMinion"
}

foreach ($staff in $staves.Keys) {
    $minion = $staves[$staff]

    $msrc = Read-File "Content\Projectiles\Summoner\$minion.cs"
    if ($msrc -notmatch "class $minion\s*:\s*LegionMinion") {
        throw "$minion should extend LegionMinion."
    }

    $ssrc = Read-File "Content\Items\Weapons\Summoner\$staff.cs"
    if ($ssrc -notmatch "class $staff\s*:\s*LegionStaff" -or $ssrc -notmatch $minion) {
        throw "$staff should be a LegionStaff summoning a $minion."
    }
}

# The Wisp Lantern is the SEED: the only one crafted from raw materials.
$wisp = Read-File "Content\Items\Weapons\Summoner\WispLantern.cs"
if ($wisp -match "AddIngredient<(SpiritBanner|ConstructCore|FusionMatrix|SentinelBeacon|SingularityCore)>") {
    throw "WispLantern is the seed -- it must NOT be fused from other staves."
}
if ($wisp -notmatch "AddRecipes") {
    throw "WispLantern should be craftable from raw materials (the legion's seed)."
}

# Every stronger legionnaire is FUSED from staves you already own (which are consumed).
$fusions = @{
    "SpiritBanner"   = @("WispLantern")
    "ConstructCore"  = @("SpiritBanner", "WispLantern")
    "FusionMatrix"   = @("ConstructCore", "SpiritBanner")
    "SentinelBeacon" = @("FusionMatrix", "ConstructCore")
    "SingularityCore" = @("SentinelBeacon", "FusionMatrix")
}

foreach ($staff in $fusions.Keys) {
    $src = Read-File "Content\Items\Weapons\Summoner\$staff.cs"

    foreach ($input in $fusions[$staff]) {
        if ($src -notmatch "AddIngredient<$input>") {
            throw "$staff should be FUSED from a $input (the Advanced Summoner fuses its own summons)."
        }
    }
}

# --- Hardmode Fusion whips ---------------------------------------------------

$whips = @{ "FusionLash" = "FusionWhipProjectile"; "LegionLash" = "LegionWhipProjectile" }

foreach ($whip in $whips.Keys) {
    $item = Read-File "Content\Items\Weapons\Summoner\$whip.cs"
    if ($item -notmatch "class $whip\s*:\s*SummonerWhip" -or $item -notmatch $whips[$whip]) {
        throw "$whip should be a SummonerWhip shooting $($whips[$whip])."
    }

    $proj = Read-File "Content\Projectiles\Summoner\$($whips[$whip]).cs"
    if ($proj -notmatch "RequiredSubclass" -or $proj -notmatch "Advanced Summoner") {
        throw "$($whips[$whip]) should be Advanced Summoner gear."
    }
}

Write-Host "Advanced Summoner source smoke test passed."
