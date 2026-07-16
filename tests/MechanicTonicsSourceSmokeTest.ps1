$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$applier = Read-File "Content\Players\MechanicTonicPlayer.cs"
$loc = Read-File "en-US.hjson"

# --- Applied in the load-order-safe phase ------------------------------------
# The subclass mechanics read their Acc hooks in PostUpdate, so the tonics MUST land in
# PostUpdateEquips (which always runs first) to be guaranteed the same frame.

if ($applier -notmatch "public override void PostUpdateEquips") {
    throw "Mechanic tonics must apply in PostUpdateEquips so they land before the subclass PostUpdate reads them."
}

if ($applier -notmatch "HasBuff") {
    throw "MechanicTonicPlayer should apply effects based on the active tonic buffs."
}

# --- Each tonic exists, extends the marker base, feeds the RIGHT hook the RIGHT way ---

$tonics = @{
    "CoolantDraught"    = @("AccCoolRateMult\s*\+=", "AccHeatPerShotMult\s*\*=")
    "AdrenalineDraught" = @("AccMomentumGainMult\s*\+=", "AccMomentumDecayMult\s*\*=")
    "FocusDraught"      = @("AccFocusRegenMult\s*\+=", "AccFocusLossMult\s*\*=")
    "WarcryDraught"     = @("AccBonusMaxCombo\s*\+=", "AccBonusComboDuration\s*\+=")
    "BloodlustDraught"  = @("AccFerocityGainMult\s*\+=", "AccFerocityDecayMult\s*\*=")
    "OverdriveDraught"  = @("AccCoreRateMult\s*\+=", "AccOverdriveBonusTicks\s*\+=")
}

foreach ($name in $tonics.Keys) {
    $buff = Read-File "Content\Buffs\${name}Buff.cs"
    if ($buff -notmatch "MechanicTonicBuff") {
        throw "${name}Buff should extend MechanicTonicBuff."
    }

    $item = Read-File "Content\Items\Consumables\$name.cs"
    if ($item -notmatch "BuffType<${name}Buff>") {
        throw "$name should grant its ${name}Buff."
    }
    if ($item -notmatch "CreateRecipe") {
        throw "$name should be craftable."
    }

    foreach ($pattern in $tonics[$name]) {
        if ($applier -notmatch $pattern) {
            throw "$name should feed its subclass hook ($pattern) in the beneficial direction."
        }
    }

    # Localization.
    if ($loc -notmatch "(?ms)\b${name}:\s*\{[^}]*DisplayName:" -and $loc -notmatch "${name}\.DisplayName:") {
        throw "en-US.hjson should localize $name."
    }
}

# Decay/heat/loss multipliers must be < 1 (a benefit, not a nerf).
foreach ($nerfHook in @("AccHeatPerShotMult", "AccMomentumDecayMult", "AccFocusLossMult", "AccFerocityDecayMult")) {
    if ($applier -notmatch "$nerfHook\s*\*=\s*0\.") {
        throw "$nerfHook should be multiplied by a fraction (< 1) so the tonic helps."
    }
}

Write-Host "Mechanic tonics source smoke test passed."
