$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$subclassPath = Join-Path $repoRoot "Content\Players\SubclassPlayer.cs"
$rulesPath = Join-Path $repoRoot "Content\Progression\ClassPromotionRules.cs"

$subclass = Get-Content -Raw $subclassPath
$rules = Get-Content -Raw $rulesPath

foreach ($field in @(
    "lockedWarriorPromotion",
    "lockedMagePromotion",
    "lockedRangerPromotion",
    "lockedSummonerPromotion")) {
    if ($subclass -notmatch $field) {
        throw "SubclassPlayer should store $field so each base Soul keeps its promotion."
    }
}

foreach ($method in @(
    "GetLockedPromotion",
    "SetLockedPromotion",
    "SaveData",
    "LoadData")) {
    if ($subclass -notmatch $method) {
        throw "SubclassPlayer should implement $method for promotion locking."
    }
}

if ($subclass -notmatch "ClassPromotionRules\.ResolveSubclass\([^;]*GetLockedPromotion\(soulPlayer\.ActiveSoul\)") {
    throw "SubclassPlayer should resolve subclass using the locked promotion for the active Soul."
}

if ($subclass -notmatch "public string GetLockedPromotion\(SoulId soul\)") {
    throw "SubclassPlayer should expose GetLockedPromotion(SoulId soul) for UI and reward feedback."
}

if ($subclass -notmatch "ClassPromotionRules\.IsPromotionForSoul") {
    throw "SubclassPlayer should validate a resolved promotion before locking it."
}

foreach ($key in @(
    "LockedWarriorPromotion",
    "LockedMagePromotion",
    "LockedRangerPromotion",
    "LockedSummonerPromotion")) {
    if ($subclass -notmatch $key) {
        throw "SubclassPlayer should persist '$key'."
    }
}

if ($rules -notmatch "ResolveSubclass\([^)]*string lockedPromotion" -or
    $rules -notmatch "IsPromotionForSoul") {
    throw "ClassPromotionRules should expose locked-promotion aware resolution and validation."
}

Write-Host "Subclass promotion lock source smoke test passed."
