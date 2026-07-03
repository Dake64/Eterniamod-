$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$elementalistPath = Join-Path $repoRoot "Content\Players\ElementalistPlayer.cs"
$content = Get-Content -Raw $elementalistPath

foreach ($field in @("FireAffinity", "IceAffinity", "LightningAffinity", "CurrentElement")) {
    if ($content -notmatch "tag\[`"$field`"\]\s*=\s*$field") {
        throw "ElementalistPlayer.SaveData should persist $field."
    }

    if ($content -notmatch "$field\s*=\s*tag\.ContainsKey\(`"$field`"\)") {
        throw "ElementalistPlayer.LoadData should restore $field with a backwards-compatible default."
    }
}

if ($content -notmatch "using Terraria\.ModLoader\.IO;") {
    throw "ElementalistPlayer should import Terraria.ModLoader.IO for TagCompound persistence."
}

if ($content -notmatch "CurrentElement\s*>=\s*Elements\.Length") {
    throw "ElementalistPlayer.LoadData should clamp CurrentElement against the available element array."
}

Write-Host "Elementalist persistence source smoke test passed."
