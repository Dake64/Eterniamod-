$ErrorActionPreference = "Stop"

# Promoting into a subclass is a big moment; it should show an on-screen banner
# instead of a chat message. The banner is now raised by the Awakening ceremony
# (which plays the build-up first), but the promise is the same: a banner, no chat spam.

$repoRoot = Split-Path -Parent $PSScriptRoot
$bannerPath = Join-Path $repoRoot "Content\UI\PromotionBannerUI.cs"
$ceremonyPath = Join-Path $repoRoot "Content\Systems\AwakeningCeremony.cs"
$reward = Get-Content -Raw (Join-Path $repoRoot "Content\Players\PromotionRewardPlayer.cs")

if (!(Test-Path $bannerPath)) {
    throw "Promotions should show an on-screen banner (PromotionBannerUI)."
}

$ceremony = Get-Content -Raw $ceremonyPath

# The promotion triggers the ceremony, which raises the banner.
if ($reward -notmatch "AwakeningCeremony\.Begin") {
    throw "A promotion should trigger the Awakening (which raises the banner)."
}

if ($ceremony -notmatch "PromotionBannerUI\.Fire") {
    throw "The Awakening ceremony should raise the promotion banner."
}

if ($reward -match "Main\.NewText") {
    throw "A promotion should not spam the chat; use the banner."
}

Write-Host "Promotion banner source smoke test passed."
