$ErrorActionPreference = "Stop"

# Playtest 2026-07-16: "le pico a la Q y no hace nada", with the resource bar visibly full.
#
# Root cause: EterniaPlayer.PreUpdate wipes ActiveSoul every frame, and the Soul accessory
# re-applies it during UpdateEquips. Terraria runs ProcessTriggers (key input) BETWEEN those
# two, so every subclass skill that gated on ActiveSoul read SoulId.None at input time and
# returned before producing any feedback -- while the HUD, drawn after equips, looked fully
# active. This silently broke the skill key for ALL subclasses.
#
# The fix: EterniaPlayer snapshots the confirmed soul after equips (InputSoul) and exposes
# phase-safe EffectiveSoul / HasClassSoulNow / HasAnySoulNow. Gating code must use those.

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$eternia = Get-Content -Raw (Join-Path $contentRoot "Players\EterniaPlayer.cs")

# --- The snapshot must exist and be taken AFTER equips have applied ------------
if ($eternia -notmatch "public SoulId InputSoul") {
    throw "EterniaPlayer should expose an InputSoul snapshot for input-time checks."
}

if ($eternia -notmatch "public SoulId EffectiveSoul") {
    throw "EterniaPlayer should expose EffectiveSoul (live value, else last confirmed)."
}

if ($eternia -notmatch "public bool HasClassSoulNow") {
    throw "EterniaPlayer should expose a phase-safe HasClassSoulNow."
}

# The snapshot has to live in PostUpdateEquips (after accessories ran). Taking it in
# PreUpdate or ResetEffects would just re-capture the wiped value.
$postEquips = [regex]::Match(
    $eternia,
    'public override void PostUpdateEquips\(\)[\s\S]+?\n\s{8}\}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($postEquips -notmatch "InputSoul = ActiveSoul") {
    throw "InputSoul must be snapshotted in PostUpdateEquips, once equips have applied."
}

# It must be captured before any early return, or non-local players / penalty paths
# would leave the snapshot stale.
$snapshotAt = $postEquips.IndexOf("InputSoul = ActiveSoul")
$firstReturn = $postEquips.IndexOf("return;")

if ($firstReturn -ge 0 -and $snapshotAt -gt $firstReturn) {
    throw "InputSoul should be snapshotted before any early return in PostUpdateEquips."
}

# --- Nothing that gates subclass behaviour may read the mid-frame raw value ----
$offenders = @()

Get-ChildItem -Recurse -File $contentRoot -Filter "*.cs" | ForEach-Object {
    if ($_.Name -eq "EterniaPlayer.cs") {
        return
    }

    $src = Get-Content -Raw $_.FullName

    if ($src -match "soul\.HasClassSoul\b" -or $src -match "soul\.ActiveSoul ==") {
        $offenders += $_.Name
    }
}

if ($offenders.Count -gt 0) {
    throw ("These files gate on the raw mid-frame soul instead of the phase-safe " +
        "EffectiveSoul/HasClassSoulNow, so they break at input time: " +
        ($offenders -join ", "))
}

Write-Host "Soul input timing source smoke test passed."
