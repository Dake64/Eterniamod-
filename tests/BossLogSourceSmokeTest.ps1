$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Read-File($rel) {
    $p = Join-Path $repoRoot $rel
    if (-not (Test-Path $p)) { throw "Missing expected file: $rel" }
    return Get-Content -Raw $p
}

$codex = Read-File "Content\Progression\BossCodex.cs"
$player = Read-File "Content\Players\BossLogPlayer.cs"
$global = Read-File "Content\Globals\BossLogGlobalNPC.cs"
$ui = Read-File "Content\UI\BossLogUI.cs"
$keybinds = Read-File "Content\Systems\EterniaKeybinds.cs"

# --- The codex: a curated, ordered boss list, with completion detection ------

if ($codex -notmatch "WallofFlesh" -or $codex -notmatch "MoonLordCore") {
    throw "BossCodex should span the whole ladder (Wall of Flesh through Moon Lord)."
}

# Multi-part fights must list every piece so a kill only logs when the LAST one falls.
if ($codex -notmatch "EaterofWorldsHead" -or $codex -notmatch "EaterofWorldsBody") {
    throw "BossCodex should list worm segments so partial deaths don't count as a kill."
}

if ($codex -notmatch "Retinazer" -or $codex -notmatch "Spazmatism") {
    throw "BossCodex should group both Twins so the fight completes only when both are dead."
}

if ($codex -notmatch "TryResolveCompletion" -or
    $codex -notmatch "other\.active" -or
    $codex -notmatch "return false") {
    throw "BossCodex completion should require no other piece of the fight to still be active."
}

# The dangling Eternia final boss is present as a locked mystery.
if ($codex -notmatch "IsMystery" -or $codex -notmatch "not yet risen") {
    throw "BossCodex should tease the not-yet-existing Eternia final boss as a locked entry."
}

# --- The record: kills, best time, highest rarity, persisted -----------------

foreach ($field in @("Kills", "BestKillTicks", "HighestRarity")) {
    if ($player -notmatch $field) {
        throw "BossLogPlayer should track $field."
    }
}

# Best time keeps the FASTEST, rarity keeps the HIGHEST.
if ($player -notmatch "durationTicks\s*<\s*record\.BestKillTicks") {
    throw "BossLogPlayer should keep the fastest clear, not overwrite with the latest."
}

if ($player -notmatch "rarity\s*>\s*record\.HighestRarity") {
    throw "BossLogPlayer should keep the highest rarity ever faced."
}

if ($player -notmatch "SaveData" -or $player -notmatch "LoadData" -or $player -notmatch "BossLogIds") {
    throw "BossLogPlayer should persist the log with the character."
}

# --- The recorder: times the fight, reads the rarity, singleplayer-scoped ----

if ($global -notmatch "GameUpdateCount") {
    throw "BossLogGlobalNPC should time the fight from spawn to kill."
}

if ($global -notmatch "GetGlobalNPC<EterniaGlobalNPC>\(\)\.rarity") {
    throw "BossLogGlobalNPC should read the boss's rolled rarity from EterniaGlobalNPC."
}

if ($global -notmatch "TryResolveCompletion") {
    throw "BossLogGlobalNPC should only log a completed fight."
}

if ($global -notmatch "Main\.dedServ") {
    throw "BossLogGlobalNPC must never try to record on a dedicated server (no local player)."
}

# --- The UI: a scrollable panel, reached from the hub ------------------------
# The Codex no longer owns a keybind; it is a page of the Eternia menu, opened by its tab.

if ($keybinds -notmatch "ToggleEterniaMenu") {
    throw "The hub keybind should exist -- it is now the only way in to the Codex."
}

$hub = Read-File "Content\UI\EterniaHubUI.cs"

if ($hub -notmatch "MajorPanel\.Bosses" -or $hub -notmatch "BossLogUI\.Visible") {
    throw "The hub should expose the Boss Codex as one of its pages."
}

if ($ui -notmatch "ScrollWheel") {
    throw "BossLogUI should scroll (the list is longer than the panel)."
}

foreach ($shown in @("Kills", "best", "Not defeated", "RarityName")) {
    if ($ui -notmatch $shown) {
        throw "BossLogUI should surface '$shown'."
    }
}

# --- Interactive + visual master/detail codex --------------------------------

# Filter tabs (interactive).
if ($ui -notmatch "enum Filter" -or $ui -notmatch "DrawTabs") {
    throw "BossLogUI should offer filter tabs (All / Pre-HM / Hardmode / Defeated)."
}

# Clickable rows that drive a selection.
if ($ui -notmatch "selectedEntry" -or $ui -notmatch "mouseLeftRelease") {
    throw "BossLogUI rows should be clickable and select a boss."
}

# Boss portraits from the vanilla boss-head textures.
if ($ui -notmatch "BossHeadTextures" -or $ui -notmatch "DrawPortrait") {
    throw "BossLogUI should show boss portraits."
}

# A detail card and an overall progress bar.
if ($ui -notmatch "DrawDetail" -or $ui -notmatch "DrawStatTile") {
    throw "BossLogUI should show a detail card with stat tiles for the selected boss."
}

if ($ui -notmatch "DrawProgressBar") {
    throw "BossLogUI should show overall boss-completion progress."
}

# Real drops + drop rates, pulled from the game's drop database (works for modded bosses too).
if ($ui -notmatch "ItemDropsDB" -or $ui -notmatch "ReportDroprates") {
    throw "BossLogUI should read real drops from the drop database."
}

if ($ui -notmatch "RateText" -or $ui -notmatch "DrawItemIcon") {
    throw "BossLogUI should show each drop's icon and its drop rate."
}

# The Codex lost its own keybind when it became a page of the hub, so what has to be
# localised now is the single menu key. A leftover "Toggle Boss Codex" entry would show a
# dead binding in the Controls menu.
$loc = Read-File "en-US.hjson"

if ($loc -match '"Toggle Boss Codex\.DisplayName"') {
    throw "The retired Boss Codex keybind should no longer be localised."
}

if ($loc -notmatch '"Open Eternia Menu\.DisplayName"') {
    throw "en-US.hjson should localize the hub keybind that now opens the Codex."
}

Write-Host "Boss log source smoke test passed."
