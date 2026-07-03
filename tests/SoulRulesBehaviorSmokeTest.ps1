$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$soulRulesPath = Join-Path $repoRoot "Content\Souls\SoulRules.cs"
$soulIdPath = Join-Path $repoRoot "Content\Souls\SoulId.cs"
$tModLoaderPath = "C:\Program Files (x86)\Steam\steamapps\common\tModLoader\tModLoader.dll"
$fnaPath = "C:\Program Files (x86)\Steam\steamapps\common\tModLoader\Libraries\FNA\1.0.0\FNA.dll"
$reLogicPath = "C:\Program Files (x86)\Steam\steamapps\common\tModLoader\Libraries\ReLogic\1.0.0\ReLogic.dll"

if (!(Test-Path $tModLoaderPath)) {
    throw "Missing tModLoader assembly: $tModLoaderPath"
}

if (!(Test-Path $fnaPath)) {
    throw "Missing FNA assembly: $fnaPath"
}

if (!(Test-Path $reLogicPath)) {
    throw "Missing ReLogic assembly: $reLogicPath"
}

$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("eternia-soulrules-test-" + [System.Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempRoot | Out-Null

try {
    $programPath = Join-Path $tempRoot "Program.cs"
    $projectPath = Join-Path $tempRoot "SoulRulesBehaviorSmokeTest.csproj"

    @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="tModLoader">
      <HintPath>$tModLoaderPath</HintPath>
    </Reference>
    <Reference Include="FNA">
      <HintPath>$fnaPath</HintPath>
      <Private>true</Private>
    </Reference>
    <Reference Include="ReLogic">
      <HintPath>$reLogicPath</HintPath>
      <Private>true</Private>
    </Reference>
    <Compile Include="$soulIdPath" Link="SoulId.cs" />
    <Compile Include="$soulRulesPath" Link="SoulRules.cs" />
  </ItemGroup>
</Project>
"@ | Set-Content -Path $projectPath -Encoding UTF8

    @'
using Eternia.Content.Souls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

static Item MakeItem(int damage, DamageClass damageClass)
{
    return new Item
    {
        type = ItemID.WoodenSword,
        stack = 1,
        damage = damage,
        DamageType = damageClass,
        createTile = -1,
        createWall = 0
    };
}

static void AssertTrue(bool condition, string scenario)
{
    if (!condition)
    {
        throw new Exception($"{scenario}: expected true.");
    }
}

static void AssertFalse(bool condition, string scenario)
{
    if (condition)
    {
        throw new Exception($"{scenario}: expected false.");
    }
}

Item meleeWeapon = MakeItem(10, DamageClass.Melee);
Item rangedWeapon = MakeItem(10, DamageClass.Ranged);
Item magicWeapon = MakeItem(10, DamageClass.Magic);
Item summonWeapon = MakeItem(10, DamageClass.Summon);
Item whipWeapon = MakeItem(10, DamageClass.SummonMeleeSpeed);
Item yoyoWeapon = MakeItem(10, DamageClass.MeleeNoSpeed);

AssertTrue(SoulRules.IsCombatItem(meleeWeapon), "damaging melee item is combat");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Warrior, meleeWeapon), "warrior allows melee");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Warrior, yoyoWeapon), "warrior allows yoyo melee");
AssertFalse(SoulRules.IsWeaponAllowed(SoulId.Warrior, rangedWeapon), "warrior rejects ranged");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Ranger, rangedWeapon), "ranger allows ranged");
AssertFalse(SoulRules.IsWeaponAllowed(SoulId.Ranger, magicWeapon), "ranger rejects magic");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Mage, magicWeapon), "mage allows magic");
AssertFalse(SoulRules.IsWeaponAllowed(SoulId.Mage, summonWeapon), "mage rejects summon");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Summoner, summonWeapon), "summoner allows summon");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Summoner, whipWeapon), "summoner allows whips");
AssertFalse(SoulRules.IsWeaponAllowed(SoulId.Summoner, meleeWeapon), "summoner rejects melee");

Item tool = MakeItem(4, DamageClass.Melee);
tool.pick = 35;
AssertFalse(SoulRules.IsCombatItem(tool), "pickaxe is not combat even with damage");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Mage, tool), "pickaxe never triggers wrong-weapon penalty");

Item tile = MakeItem(0, DamageClass.Default);
tile.type = ItemID.DirtBlock;
tile.createTile = TileID.Dirt;
AssertFalse(SoulRules.IsCombatItem(tile), "placeable tile is not combat");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Warrior, tile), "placeable tile never triggers wrong-weapon penalty");

Item harmless = MakeItem(0, DamageClass.Melee);
AssertFalse(SoulRules.IsCombatItem(harmless), "zero-damage item is not combat");
AssertTrue(SoulRules.IsWeaponAllowed(SoulId.Ranger, harmless), "zero-damage item never triggers wrong-weapon penalty");

Console.WriteLine("SoulRules behavior smoke test passed.");
'@ | Set-Content -Path $programPath -Encoding UTF8

    dotnet run --project $projectPath

    if ($LASTEXITCODE -ne 0) {
        throw "SoulRules behavior smoke test executable failed with exit code $LASTEXITCODE."
    }
}
finally {
    if (Test-Path $tempRoot) {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force
    }
}
