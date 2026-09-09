$ErrorActionPreference = "Stop"

# The armour helmets now use Eternia's OWN equip textures instead of borrowing a vanilla head
# slot. That means each helm declares a texture path, and tModLoader autoloads the equip sheet
# from "<that path>_Head.png".
#
# A DECLARED PATH WITH NO FILE MAKES THE MOD FAIL TO LOAD -- not render oddly, fail outright. So
# this test is the one that matters most in the armour area: every declared texture must exist,
# and every equip sheet must be exactly 40x1120 (20 frames of 40x56), which is the format
# measured off real armour rather than assumed.

Add-Type -AssemblyName System.Drawing

$repoRoot = Split-Path -Parent $PSScriptRoot
$armorDir = Join-Path $repoRoot "Content\Items\Armor"

$declared = 0
$checked = @()

Get-ChildItem -Path $armorDir -Filter "*Set.cs" | ForEach-Object {
    $src = Get-Content -Raw $_.FullName

    foreach ($m in [regex]::Matches($src, 'override string Texture => "ETERNIA/([^"]+)"')) {
        $declared++
        $rel = $m.Groups[1].Value -replace '/', '\'
        $icon = Join-Path $repoRoot ($rel + ".png")

        if (-not (Test-Path $icon)) {
            throw "Declared item texture has no file: $rel.png -- the mod would fail to load."
        }

        # A helm that takes its own equip slot must ship the sheet that slot loads.
        if ($src -match [regex]::Escape($m.Groups[1].Value) -and $rel -match '(Helm|Hood|Crown)$') {
            $sheet = Join-Path $repoRoot ($rel + "_Head.png")
            if (-not (Test-Path $sheet)) {
                throw "Helm $rel declares its own equip slot but ships no ${rel}_Head.png."
            }

            $bmp = [System.Drawing.Image]::FromFile($sheet)
            $w = $bmp.Width; $h = $bmp.Height
            $bmp.Dispose()

            if ($w -ne 40 -or $h -ne 1120) {
                throw "Equip sheet ${rel}_Head.png is ${w}x${h}; Terraria needs 40x1120 (20 frames of 40x56)."
            }

            $checked += $rel
        }
    }
}

if ($declared -lt 25) {
    throw "Expected at least 25 helms declaring their own texture; found $declared."
}

# No helm should still be borrowing a vanilla head slot now that it has its own art.
Get-ChildItem -Path $armorDir -Filter "*Set.cs" | ForEach-Object {
    $src = Get-Content -Raw $_.FullName
    if ($src -match 'Item\.headSlot = ArmorIDs\.Head\.') {
        throw "$($_.BaseName) still borrows a vanilla head slot instead of using its own equip texture."
    }
}

Write-Host "Armor equip texture source smoke test passed ($($checked.Count) helms, sheets 40x1120)."
