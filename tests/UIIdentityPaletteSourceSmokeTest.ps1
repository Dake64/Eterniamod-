$ErrorActionPreference = "Stop"

# Eternia's UI wears ONE violet chrome. The rule that makes that survive contact with future
# features is the split:
#
#   CHROME  (panels, borders, muted labels, navigation) -> the shared palette. This is identity.
#   MEANING (enemy rarity, Soul colours, affinity branches) -> untouched. Gold reads as
#           Legendary and red reads as Bleed; tinting those violet would trade information the
#           player reads at a glance for decoration.

$repoRoot = Split-Path -Parent $PSScriptRoot
$uiRoot = Join-Path $repoRoot "Content\UI"

$toolkit = Get-Content -Raw (Join-Path $uiRoot "EterniaUI.cs")

# --- The palette is violet, and stays a ladder ---------------------------------
$palette = @{}

foreach ($name in @("PanelBackground", "PanelSurface", "PanelSurfaceAlt", "Border", "MutedText", "Brand")) {
    $m = [regex]::Match($toolkit, "$name\s*=\s*\r?\n?\s*new Color\((\d+),\s*(\d+),\s*(\d+)\)")

    if (-not $m.Success) {
        throw "The shared palette is missing '$name'."
    }

    $palette[$name] = @{
        R = [int]$m.Groups[1].Value
        G = [int]$m.Groups[2].Value
        B = [int]$m.Groups[3].Value
    }
}

# Violet means blue clearly ahead of green in every chrome colour. Without this the palette
# can drift back to neutral grey one tweak at a time and the identity quietly dissolves.
foreach ($name in @("PanelBackground", "PanelSurface", "PanelSurfaceAlt", "Border", "MutedText", "Brand")) {
    $c = $palette[$name]

    if ($c.B -le $c.G) {
        throw "'$name' is not violet any more (B=$($c.B) should exceed G=$($c.G))."
    }
}

# Surfaces must keep getting lighter in order, or panels stop reading as layered.
$ladder = @("PanelBackground", "PanelSurface", "PanelSurfaceAlt", "Border", "MutedText")

for ($i = 1; $i -lt $ladder.Count; $i++) {
    $prev = $palette[$ladder[$i - 1]]
    $cur = $palette[$ladder[$i]]

    if (($cur.R + $cur.G + $cur.B) -le ($prev.R + $prev.G + $prev.B)) {
        throw "The palette ladder broke: '$($ladder[$i])' is not lighter than '$($ladder[$i - 1])'."
    }
}

# --- Chrome uses the palette, it does not reinvent it --------------------------
# A panel-wide accent hardcoded in a UI file is how the mod ended up looking like four
# different programs in the first place.
Get-ChildItem -File $uiRoot -Filter "*.cs" | ForEach-Object {
    $src = Get-Content -Raw $_.FullName

    if ($src -match 'Accent\s*=\s*new Color\(') {
        throw ("$($_.Name) hardcodes a panel accent. Chrome should use EterniaUI.Brand so the " +
            "whole UI shares one identity.")
    }
}

# The navigation strip is on every page, so it is the surface that says "this is Eternia".
$hub = Get-Content -Raw (Join-Path $uiRoot "EterniaHubUI.cs")

if ($hub -notmatch "EterniaUI\.Brand") {
    throw "The hub tabs should wear the brand colour, not a per-page hue."
}

# --- Meaning is NOT unified ----------------------------------------------------
# These must keep their own colours; if they ever resolve to the palette, information the
# player reads at a glance has been thrown away for decoration.
$globalNpc = Get-Content -Raw (Join-Path $repoRoot "Content\Globals\EterniaGlobalNPC.cs")

if ($globalNpc -notmatch "public static Color GetRarityColor") {
    throw "Enemy rarity must keep its own colour table -- rarity is information, not chrome."
}

if ($globalNpc -match "GetRarityColor[\s\S]{0,400}EterniaUI\.Brand") {
    throw "Rarity colours must not collapse into the brand colour."
}

$bossLog = Get-Content -Raw (Join-Path $uiRoot "BossLogUI.cs")

if ($bossLog -notmatch "RarityColor\(record\.HighestRarity\)") {
    throw "The Codex should still colour each boss gem by the rarity you beat it at."
}

Write-Host "UI identity palette source smoke test passed."
