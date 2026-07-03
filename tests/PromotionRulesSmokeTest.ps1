$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$servicePath = Join-Path $repoRoot "Content\Progression\ClassPromotionRules.cs"
$soulIdPath = Join-Path $repoRoot "Content\Souls\SoulId.cs"

if (!(Test-Path $servicePath)) {
    throw "Missing required source file: $servicePath"
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("eternia-promotion-test-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot | Out-Null

try {
    $programPath = Join-Path $tempRoot "Program.cs"
    $projectPath = Join-Path $tempRoot "PromotionRulesSmokeTest.csproj"

    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="$soulIdPath" Link="SoulId.cs" />
    <Compile Include="$servicePath" Link="ClassPromotionRules.cs" />
  </ItemGroup>
</Project>
"@ | Set-Content -Path $projectPath -Encoding UTF8

    @'
using Eternia.Content.Progression;
using Eternia.Content.Souls;

static void AssertEqual(string expected, string actual, string scenario)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
    {
        throw new Exception($"{scenario}: expected '{expected}', got '{actual}'");
    }
}

var none = ClassAffinitySnapshot.Empty;

AssertEqual("None", ClassPromotionRules.ResolveSubclass(SoulId.None, hardMode: false, none), "no soul");
AssertEqual("None", ClassPromotionRules.ResolveSubclass(SoulId.Empty, hardMode: false, none), "empty soul");

AssertEqual("Warrior", ClassPromotionRules.ResolveSubclass(SoulId.Warrior, hardMode: false, none), "warrior base");
AssertEqual("Mage", ClassPromotionRules.ResolveSubclass(SoulId.Mage, hardMode: false, none), "mage base");
AssertEqual("Ranger", ClassPromotionRules.ResolveSubclass(SoulId.Ranger, hardMode: false, none), "ranger base");
AssertEqual("Summoner", ClassPromotionRules.ResolveSubclass(SoulId.Summoner, hardMode: false, none), "summoner base");

AssertEqual("Warrior", ClassPromotionRules.ResolveSubclass(SoulId.Warrior, hardMode: true, none), "warrior hardmode with no affinity");
AssertEqual("Mage", ClassPromotionRules.ResolveSubclass(SoulId.Mage, hardMode: true, none), "mage hardmode with no affinity");
AssertEqual("Ranger", ClassPromotionRules.ResolveSubclass(SoulId.Ranger, hardMode: true, none), "ranger hardmode with no affinity");
AssertEqual("Summoner", ClassPromotionRules.ResolveSubclass(SoulId.Summoner, hardMode: true, none), "summoner hardmode with no affinity");

AssertEqual(
    "Fighter",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Warrior,
        hardMode: true,
        none with { ComboAffinity = 3 }),
    "warrior combo promotion");

AssertEqual(
    "Cursed Mage",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Mage,
        hardMode: true,
        none with { CurseAffinity = 4 }),
    "mage curse promotion");

AssertEqual(
    "Archer",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Ranger,
        hardMode: true,
        none with { BowAffinity = 2 }),
    "ranger bow promotion");

AssertEqual(
    "Necromancer",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Summoner,
        hardMode: true,
        none with { ShadowAffinity = 5 }),
    "summoner shadow promotion");

AssertEqual(
    "Necromancer",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Summoner,
        hardMode: true,
        none with { BeastAffinity = 5, ShadowAffinity = 5 }),
    "summoner shadow tie promotion");

AssertEqual(
    "Shadow",
    ClassPromotionRules.GetDominantAffinityName(
        SoulId.Summoner,
        none with { BeastAffinity = 5, ShadowAffinity = 5 }),
    "summoner dominant affinity name");

if (ClassPromotionRules.GetDominantAffinityValue(
    SoulId.Summoner,
    none with { ShadowAffinity = 5 }) != 5)
{
    throw new Exception("summoner dominant affinity value: expected 5");
}

AssertEqual(
    "None",
    ClassPromotionRules.GetDominantAffinityName(
        SoulId.Mage,
        none),
    "empty dominant affinity name");

if (ClassPromotionRules.GetDominantAffinityValue(SoulId.Mage, none) != 0)
{
    throw new Exception("empty dominant affinity value: expected 0");
}

AssertEqual(
    "Warrior",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Warrior,
        hardMode: false,
        none with { BleedAffinity = 5 },
        lockedPromotion: "Swordsman"),
    "locked warrior promotion ignored before hardmode");

AssertEqual(
    "Swordsman",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Warrior,
        hardMode: true,
        none with { ComboAffinity = 10 },
        lockedPromotion: "Swordsman"),
    "locked warrior promotion overrides later dominant affinity");

AssertEqual(
    "Fighter",
    ClassPromotionRules.ResolveSubclass(
        SoulId.Warrior,
        hardMode: true,
        none with { ComboAffinity = 10 },
        lockedPromotion: "Necromancer"),
    "invalid locked promotion ignored for active soul");

Console.WriteLine("Promotion rules smoke test passed.");
'@ | Set-Content -Path $programPath -Encoding UTF8

    dotnet run --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "Promotion rules smoke test executable failed with exit code $LASTEXITCODE."
    }
}
finally {
    if (Test-Path $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
