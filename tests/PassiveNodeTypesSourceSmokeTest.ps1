$ErrorActionPreference = "Stop"

# Nodes come in three kinds so the big tree reads clearly:
#  - Minor: small "path" dots (the generated affinity nodes),
#  - Notable: the hand-authored nodes with their own effects (cards),
#  - Keystone: one build-defining node per branch (large, with a trade-off).

$repoRoot = Split-Path -Parent $PSScriptRoot
$c = Join-Path $repoRoot "Content"

$node = Get-Content -Raw (Join-Path $c "Passives\PassiveNode.cs")
$registry = Get-Content -Raw (Join-Path $c "Passives\PassiveRegistry.cs")
$keystone = Get-Content -Raw (Join-Path $c "Players\KeystonePlayer.cs")
$passive = Get-Content -Raw (Join-Path $c "UI\PassiveUI.cs")

# --- The kind concept exists on the node -------------------------------------
if ($node -notmatch "enum PassiveKind" -or
    $node -notmatch "Minor" -or
    $node -notmatch "Notable" -or
    $node -notmatch "Keystone" -or
    $node -notmatch "PassiveKind Kind") {
    throw "PassiveNode should carry a PassiveKind (Minor/Notable/Keystone)."
}

# --- Generated nodes are Minor; each branch ends in a Keystone ----------------
if ($registry -notmatch "\.Kind = PassiveKind\.Minor" -or
    $registry -notmatch "\.Kind = PassiveKind\.Keystone") {
    throw "PadBranchesTo should mark path nodes Minor and cap each branch with a Keystone."
}

if ($registry -notmatch "public static string KeystoneName\(" -or
    $registry -notmatch "public static string KeystoneDescription\(") {
    throw "PassiveRegistry should name/describe each branch keystone."
}

# --- Keystones have real (build-defining) effects, gated to the class ---------
if ($keystone -notmatch "PassiveRegistry\.KeystoneName\(" -or
    $keystone -notmatch "UnlockedPassives\.Contains" -or
    $keystone -notmatch 'case "Bleed"' -or
    $keystone -notmatch 'case "Shadow"') {
    throw "KeystonePlayer should apply each unlocked keystone's effect for the active class."
}

# --- The UI renders each kind differently ------------------------------------
if ($passive -notmatch "DrawMinorNode" -or
    $passive -notmatch "DrawKeystoneNode" -or
    $passive -notmatch "CardWidth\(" -or
    $passive -notmatch "PassiveKind\.Keystone") {
    throw "PassiveUI should size and draw Minor/Notable/Keystone nodes distinctly."
}

Write-Host "Passive node types source smoke test passed."
