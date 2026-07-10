$ErrorActionPreference = "Stop"

# The passive tree is non-linear: within a branch, tiers alternate 1 node then 2
# nodes, and a node's prerequisites are ALL nodes in the previous tier -- so the
# node after a 2-node tier requires BOTH of them (a merge/gate). Prerequisites are
# derived from this tier structure and used for both unlocking and the UI (which
# draws a line per prerequisite and fans split nodes out into a diamond).

$repoRoot = Split-Path -Parent $PSScriptRoot
$registry = Get-Content -Raw (Join-Path $repoRoot "Content\Passives\PassiveRegistry.cs")
$service = Get-Content -Raw (Join-Path $repoRoot "Content\Progression\ProgressionService.cs")
$passive = Get-Content -Raw (Join-Path $repoRoot "Content\UI\PassiveUI.cs")

if ($registry -notmatch "List<List<PassiveNode>>\s*BuildTiers" -or
    $registry -notmatch "public static List<string> GetPrerequisites\(") {
    throw "PassiveRegistry should expose BuildTiers and GetPrerequisites for the diamond lattice."
}

# Tiers alternate 1, then 2 nodes, which is what creates the two-into-one merges.
if ($registry -notmatch "tier % 2 == 0 \? 1 : 2") {
    throw "BuildTiers should alternate 1-node and 2-node tiers so branches form diamonds."
}

# Branches get different shapes so the tree doesn't look uniform.
if ($registry -notmatch "BranchStyle\(" -or
    $registry -notmatch "TierSize\(") {
    throw "BuildTiers should vary branch shape (BranchStyle/TierSize) so branches differ."
}

# Each branch is padded up to an extensive depth for v1.
if ($registry -notmatch "BranchTarget\s*=\s*20" -or
    $registry -notmatch "PadBranchesTo\(") {
    throw "PassiveRegistry should pad every branch up to 20 nodes (BranchTarget/PadBranchesTo)."
}

# Prerequisites are ALL nodes of the previous tier (so a merge requires two).
if ($registry -notmatch "tiers\[t - 1\]\.ConvertAll") {
    throw "GetPrerequisites should return every node of the previous tier."
}

# Unlocking must require ALL prerequisites, not just a single previous node.
if ($service -notmatch "PassiveRegistry\.GetPrerequisites\(soul\.ActiveSoul, passive\)" -or
    $service -match "passive\.RequiredPassive") {
    throw "ProgressionService should gate unlocks on all GetPrerequisites, not a single RequiredPassive."
}

# The UI state check and the connectors both use the derived prerequisites.
if ($passive -notmatch "PassiveRegistry\.GetPrerequisites\(") {
    throw "PassiveUI should use GetPrerequisites for node state and connectors."
}

if ($passive -notmatch "BuildTiers\(" -or
    $passive -notmatch "laneSpacing") {
    throw "PassiveUI should lay split nodes out into diamonds (BuildTiers + laneSpacing offset)."
}

# The tooltip must list ALL prerequisites (from GetPrerequisites), not a single
# stale RequiredPassive, so a merge/gate node shows every node it needs.
if ($passive -notmatch "Requires ALL of") {
    throw "The node tooltip should list every prerequisite for merge/gate nodes."
}

Write-Host "Passive diamond tree source smoke test passed."
