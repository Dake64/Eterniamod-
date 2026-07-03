$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$removedTerms = @(
    "EterniaSoulSystem",
    "SoulSlotUnlocked"
)

foreach ($term in $removedTerms) {
    $matches = Get-ChildItem -Path $contentRoot -Recurse -File -Include *.cs |
        Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" } |
        Select-String -Pattern $term -SimpleMatch

    if ($matches) {
        throw "Removed Soul slot residue '$term' found in $($matches[0].Path)."
    }
}

Write-Host "Removed Soul slot system source smoke test passed."
