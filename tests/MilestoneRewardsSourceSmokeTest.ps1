$ErrorActionPreference = "Stop"

# Every 5 unlocked passives is a MILESTONE that grants an escalating class-flavored
# bonus, celebrated with a banner + sound and shown in the passive panel sidebar.

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$milestone = Get-Content -Raw (Join-Path $c "Players\MilestonePlayer.cs")
$banner = Join-Path $c "UI\MilestoneBannerUI.cs"
$service = Get-Content -Raw (Join-Path $c "Progression\ProgressionService.cs")
$passive = Get-Content -Raw (Join-Path $c "UI\PassiveUI.cs")

# --- MilestonePlayer: 1 milestone per 5 unlocked, class-typed bonus -----------
if ($milestone -notmatch "NodesPerMilestone\s*=\s*5") {
    throw "MilestonePlayer should define a milestone every 5 nodes."
}

if ($milestone -notmatch "UnlockedPassives\.Count" -or
    $milestone -notmatch "/ NodesPerMilestone") {
    throw "MilestonePlayer should count milestones from unlocked passives."
}

if ($milestone -notmatch "PostUpdateEquips" -or
    $milestone -notmatch "GetDamage\(" -or
    $milestone -notmatch "GetCritChance\(") {
    throw "MilestonePlayer should apply an escalating class-typed damage/crit bonus."
}

# The bonus should be tuned to the SUBCLASS, not just the base class.
if ($milestone -notmatch "CurrentSubclass" -or
    $milestone -notmatch 'case "Swordsman"' -or
    $milestone -notmatch 'case "Necromancer"') {
    throw "Milestone bonuses should be tailored to the active subclass."
}

if ($milestone -notmatch "whoAmI != Main\.myPlayer" -or
    $milestone -notmatch "HasClassSoul") {
    throw "MilestonePlayer should guard the local player and require a class Soul."
}

# --- The special moment: banner + sound on reaching a milestone ---------------
if (!(Test-Path $banner)) {
    throw "A MilestoneBannerUI should announce reaching a milestone."
}

if ($service -notmatch "% MilestonePlayer\.NodesPerMilestone == 0" -or
    $service -notmatch "MilestoneBannerUI\.Show") {
    throw "ProgressionService should fire the milestone banner every 5 unlocks."
}

# --- Milestones also deepen the subclass MECHANIC (not just stats) -----------
foreach ($mech in @(
    "Players\NecromancerPlayer.cs",
    "Players\SwordsmanPlayer.cs",
    "Players\BerserkerPlayer.cs",
    "Players\CursedMagePlayer.cs")) {
    $src = Get-Content -Raw (Join-Path $c $mech)
    if ($src -notmatch "GetModPlayer<MilestonePlayer>\(\)\.Milestones") {
        throw "$mech should scale its subclass mechanic with milestones."
    }
}

# --- Shown in the passive panel ----------------------------------------------
if ($passive -notmatch "GetModPlayer<MilestonePlayer>" -or
    $passive -notmatch "Milestones") {
    throw "PassiveUI should show the milestone count/bonus in the sidebar."
}

Write-Host "Milestone rewards source smoke test passed."
