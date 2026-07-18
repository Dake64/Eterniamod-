$ErrorActionPreference = "Stop"

# Bleed is a WARRIOR-WIDE mechanic restricted to a CURATED set of swords:
#  - a real mod debuff (BleedDebuff : ModBuff) with a single level and fixed damage,
#  - only specific vanilla swords (EterniaGlobalItem's ItemID->chance table) plus
#    the mod's signature swords (IBleedWeapon) can inflict it -- NOT every sword,
#  - each bleed sword has its own base chance, VISIBLE in the tooltip,
#  - the Warrior passive tree (Bleed affinity + named passives) tunes attributes.
# The Swordsman keeps an on-hit "mastery" hook (guaranteed bleed + Crimson Trail),
# and the old 5-stack EXECUTE is gone (it is now a Crimson Trail technique).

$repoRoot = Split-Path -Parent $PSScriptRoot
$contentRoot = Join-Path $repoRoot "Content"

$buff = Get-Content -Raw (Join-Path $contentRoot "Buffs\BleedDebuff.cs")
$iface = Get-Content -Raw (Join-Path $contentRoot "Items\IBleedWeapon.cs")
$globalItem = Get-Content -Raw (Join-Path $contentRoot "Globals\EterniaGlobalItem.cs")
$trainingBlade = Get-Content -Raw (Join-Path $contentRoot "Items\Weapons\Warrior\TrainingBlade.cs")
$blade = Get-Content -Raw (Join-Path $contentRoot "Items\Weapons\Promotion\BloodletterBlade.cs")
$warriorBleed = Get-Content -Raw (Join-Path $contentRoot "Players\WarriorBleedPlayer.cs")
$bleedNPC = Get-Content -Raw (Join-Path $contentRoot "NPCs\BleedGlobalNPC.cs")
$swordsman = Get-Content -Raw (Join-Path $contentRoot "Players\SwordsmanPlayer.cs")
$subclassEffects = Get-Content -Raw (Join-Path $contentRoot "Players\SubclassEffectsPlayer.cs")

# --- Real mod debuff -------------------------------------------------------
if ($buff -notmatch "class BleedDebuff\s*:\s*ModBuff" -or
    $buff -notmatch "Main\.debuff\[Type\]\s*=\s*true") {
    throw "BleedDebuff should be a real mod debuff (ModBuff with Main.debuff[Type] = true)."
}

if (!(Test-Path (Join-Path $contentRoot "Buffs\BleedDebuff.png"))) {
    throw "BleedDebuff needs a texture (BleedDebuff.png)."
}

# --- Curated bleed swords + VISIBLE chance ---------------------------------
if ($globalItem -notmatch "public static bool CanInflictBleed\(") {
    throw "EterniaGlobalItem should expose CanInflictBleed to gate the curated bleed swords."
}

# Only a CURATED set of vanilla swords bleed (an ItemID -> chance table), not all.
if ($globalItem -notmatch "ItemID\.BloodButcherer" -or
    $globalItem -notmatch "ItemID\.DeathSickle" -or
    $globalItem -notmatch "ContainsKey\(") {
    throw "EterniaGlobalItem should hold a curated ItemID->chance table of bleed swords."
}

if ($globalItem -notmatch "IBleedWeapon") {
    throw "Signature mod swords (IBleedWeapon) should also count as bleed swords."
}

if ($globalItem -notmatch "GetBaseBleedChance\(") {
    throw "EterniaGlobalItem should resolve a bleed sword's base chance."
}

if ($globalItem -notmatch "ModifyTooltips" -or
    $globalItem -notmatch "Bleed" -or
    $globalItem -notmatch "%") {
    throw "EterniaGlobalItem should show the bleed chance percentage in the tooltip."
}

# --- Signature swords override the chance ----------------------------------
if ($iface -notmatch "interface IBleedWeapon" -or
    $iface -notmatch "int BleedChance") {
    throw "IBleedWeapon should expose a signature-sword BleedChance override."
}

foreach ($weapon in @(
    @{ Name = "TrainingBlade"; Content = $trainingBlade },
    @{ Name = "BloodletterBlade"; Content = $blade })) {
    if ($weapon.Content -notmatch "IBleedWeapon" -or
        $weapon.Content -notmatch "public int BleedChance") {
        throw "$($weapon.Name) should be a signature bleed sword (implement IBleedWeapon)."
    }
}

if ($blade -notmatch "ModifyTooltips" -or
    $blade -notmatch "bleed" -or
    $blade -notmatch "Swordsman") {
    throw "BloodletterBlade should explain its bleed identity and Swordsman requirement."
}

# --- Warrior-wide application via the curated check ------------------------
if ($warriorBleed -notmatch "public bool IsActiveWarrior\(") {
    throw "WarriorBleedPlayer should centralize the active-Warrior check."
}

$activeWarrior = [regex]::Match(
    $warriorBleed,
    'public bool IsActiveWarrior\([\s\S]+?\n\s*}',
    [System.Text.RegularExpressions.RegexOptions]::Singleline).Value

if ($activeWarrior -notmatch "GetModPlayer<EterniaPlayer>\(\)" -or
    $activeWarrior -notmatch "HasClassSoul" -or
    $activeWarrior -notmatch "(Active|Effective)Soul == SoulId\.Warrior") {
    throw "IsActiveWarrior should require an equipped Warrior class Soul (not a subclass name)."
}

if ($warriorBleed -notmatch "if \(!IsActiveWarrior\(\)\)") {
    throw "WarriorBleedPlayer application should be gated by an active Warrior Soul."
}

if ($warriorBleed -notmatch "CanInflictBleed\(" -or
    $warriorBleed -notmatch "Main\.rand" -or
    $warriorBleed -notmatch "BleedDebuff") {
    throw "WarriorBleedPlayer should roll the bleed chance for curated swords and apply the BleedDebuff."
}

if ($warriorBleed -notmatch 'HasActivePassive\([^)]*"Execution"') {
    throw "WarriorBleedPlayer should apply the Execution passive (extra damage vs bleeding enemies) Warrior-wide."
}

# Bleed affinity contributes only a scaled-down bonus to the chance, so a maxed
# Bleed tree cannot push proc rates near-guaranteed.
if ($warriorBleed -notmatch "affinity / 2") {
    throw "WarriorBleedPlayer should scale down the Bleed affinity contribution to the chance (affinity / 2)."
}

# --- Single-level, fixed-damage DoT gated on an active Warrior owner --------
if ($bleedNPC -match "BleedStacks") {
    throw "Bleed is now single-level; BleedGlobalNPC should not track BleedStacks anymore."
}

if ($bleedNPC -notmatch "WarriorBleedPlayer" -or
    $bleedNPC -notmatch "IsActiveWarrior\(\)") {
    throw "BleedGlobalNPC should require the owner to be an active Warrior (WarriorBleedPlayer.IsActiveWarrior)."
}

if ($bleedNPC -match "GetModPlayer<SubclassPlayer>") {
    throw "BleedGlobalNPC should not read SubclassPlayer directly; that bypasses active Soul gating."
}

if ($bleedNPC -notmatch "lastInteraction\s*<\s*0" -or
    $bleedNPC -notmatch "lastInteraction\s*>=\s*Main\.maxPlayers") {
    throw "BleedGlobalNPC should validate npc.lastInteraction before indexing Main.player."
}

if ($bleedNPC -notmatch "const int BaseBleedDamage") {
    throw "BleedGlobalNPC should use a named constant for the fixed bleed damage."
}

# --- Swordsman keeps a gated on-hit mastery hook; EXECUTE stacks are gone ---
if ($swordsman -notmatch "public bool IsActiveSwordsman\(" -or
    $swordsman -notmatch "OnHitNPCWithItem") {
    throw "SwordsmanPlayer should keep IsActiveSwordsman and a gated OnHitNPCWithItem hook."
}

if ($swordsman -notmatch "CanInflictBleed\(") {
    throw "SwordsmanPlayer mastery should also be restricted to bleed swords."
}

if ($swordsman -match "BleedStacks") {
    throw "SwordsmanPlayer should use the new single-level bleed model, not BleedStacks."
}

if ($subclassEffects -match "BleedStacks") {
    throw "The 5-stack EXECUTE should be removed from SubclassEffectsPlayer (it becomes a Crimson Trail technique)."
}

Write-Host "Swordsman bleed source smoke test passed."
