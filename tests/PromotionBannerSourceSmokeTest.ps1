$ErrorActionPreference = "Stop"

# Promoting into a subclass is a big moment; it should show an on-screen banner
# instead of a chat message.

$repoRoot = Split-Path -Parent $PSScriptRoot
$bannerPath = Join-Path $repoRoot "Content\UI\PromotionBannerUI.cs"
$reward = Get-Content -Raw (Join-Path $repoRoot "Content\Players\PromotionRewardPlayer.cs")

if (!(Test-Path $bannerPath)) {
    throw "Promotions should show an on-screen banner (PromotionBannerUI)."
}

if ($reward -notmatch "PromotionBannerUI\.Show") {
    throw "A promotion should trigger the on-screen banner."
}

if ($reward -match "Main\.NewText") {
    throw "A promotion should not spam the chat; use the banner."
}

Write-Host "Promotion banner source smoke test passed."
