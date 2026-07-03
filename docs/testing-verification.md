# Testing y verificacion

## Naturaleza de los tests

Los tests en `tests/*.ps1` son smoke tests source-level. Verifican patrones,
archivos, reglas y cobertura estructural. Son utiles para evitar regresiones de
arquitectura, pero no prueban el mod dentro de Terraria.

Siempre complementar con pruebas manuales en tModLoader cuando cambie UI,
assets, reload o comportamiento runtime.

## Build

```powershell
dotnet build .\ETERNIA.csproj
```

Resultado esperado reciente:

- `Compilacion correcta.`
- `0 Advertencia(s)`
- `0 Errores`

## Suite completa

```powershell
$ErrorActionPreference = 'Stop'
$tests = Get-ChildItem -Path .\tests -Filter *.ps1 | Sort-Object Name
$passed = 0
foreach ($test in $tests) {
    Write-Host "RUN $($test.Name)"
    pwsh -NoProfile -ExecutionPolicy Bypass -File $test.FullName
    if ($LASTEXITCODE -ne 0) {
        throw "Smoke test failed: $($test.Name) with exit code $LASTEXITCODE"
    }
    $passed++
}
Write-Host "ALL $passed SMOKE TESTS PASSED"
```

Resultado esperado reciente:

- `ALL 49 SMOKE TESTS PASSED`

## Diff check

```powershell
git diff --check
```

Exit code 0 es suficiente. Git puede imprimir warnings de LF/CRLF; esos no son
errores de whitespace.

## Tests existentes

Lista actual de smoke tests:

- `BaseClassResourceUISourceSmokeTest.ps1`
- `BerserkerSkillCostSourceSmokeTest.ps1`
- `BerserkerSkillKnockbackSourceSmokeTest.ps1`
- `ClassProgressionUISourceSmokeTest.ps1`
- `CurseAccessoriesSourceSmokeTest.ps1`
- `CursedMageBootstrapSourceSmokeTest.ps1`
- `DamageClassCompatibilitySourceSmokeTest.ps1`
- `DefaultAssetCoverageSourceSmokeTest.ps1`
- `ElementalistPersistenceSourceSmokeTest.ps1`
- `EnemyRaritySourceSmokeTest.ps1`
- `EternalNPCSystemSourceSmokeTest.ps1`
- `EterniaUITypographySourceSmokeTest.ps1`
- `KeybindDefaultsSourceSmokeTest.ps1`
- `LocalizationContentSourceSmokeTest.ps1`
- `ModMetadataSourceSmokeTest.ps1`
- `NecromancerFlowSourceSmokeTest.ps1`
- `NecromancerMinionAISourceSmokeTest.ps1`
- `NPCLocalizationSourceSmokeTest.ps1`
- `OverlayPlayerStateSourceSmokeTest.ps1`
- `PassiveEffectScopeSourceSmokeTest.ps1`
- `PassiveEligibilitySourceSmokeTest.ps1`
- `PassiveRuntimeCoverageSourceSmokeTest.ps1`
- `PassiveRuntimeEffectsSourceSmokeTest.ps1`
- `PassiveUISourceSmokeTest.ps1`
- `PracticeYoyoSourceSmokeTest.ps1`
- `PromotionLocalizationSourceSmokeTest.ps1`
- `PromotionRewardsSourceSmokeTest.ps1`
- `PromotionRulesSmokeTest.ps1`
- `RemovedCartomancerResidueSmokeTest.ps1`
- `RemovedSoulSlotSystemSourceSmokeTest.ps1`
- `ResourceTexturePathsSourceSmokeTest.ps1`
- `ResponsiveUILayoutSourceSmokeTest.ps1`
- `SoulInventorySourceSmokeTest.ps1`
- `SoulRulesBehaviorSmokeTest.ps1`
- `SoulRulesOwnershipSourceSmokeTest.ps1`
- `SoulRulesSourceSmokeTest.ps1`
- `SoulTooltipSourceSmokeTest.ps1`
- `SoulUISystemUpdateSourceSmokeTest.ps1`
- `StarterLoadoutSourceSmokeTest.ps1`
- `StatsPersistenceSourceSmokeTest.ps1`
- `StatsPlayerSubclassOwnershipSourceSmokeTest.ps1`
- `StunnerMeleeSourceSmokeTest.ps1`
- `SubclassPromotionLockSourceSmokeTest.ps1`
- `SubclassRuntimeGatingSourceSmokeTest.ps1`
- `SummonerWhipSourceSmokeTest.ps1`
- `SwordsmanBleedSourceSmokeTest.ps1`
- `UIInteractionHardeningSourceSmokeTest.ps1`
- `UILifecycleSourceSmokeTest.ps1`
- `UIReworkSourceSmokeTest.ps1`

## Checklist manual en tModLoader

Flujo de onboarding:

1. Crear personaje/mundo o usar uno sin Souls.
2. Cargar mod sin excepciones.
3. Confirmar que sin Soul no hay no-Soul penalty.
4. Hablar con `EternalNPC`.
5. Recibir `EmptySoul`.
6. Craftear `EmptySoul` a una de:
   - Warrior Soul
   - Mage Soul
   - Ranger Soul
   - Summoner Soul
7. Confirmar que `EmptySoul` se consume.
8. Equipar la Soul como accesorio.
9. Confirmar starter weapon correcto.

Penalizaciones:

1. Con Soul activa, usar arma permitida.
2. Con Soul activa, intentar usar arma de otra clase.
3. Debe matar al jugador.
4. Tener Class Soul pero no equiparla.
5. Debe aplicar penalty fuerte/debuff.

UI:

1. Abrir `Soul UI`.
2. Abrir `Stats UI`; Soul UI debe cerrarse.
3. Abrir `Passive UI`; Stats UI debe cerrarse.
4. Cerrar con boton `X`.
5. Probar drag de Soul UI.
6. Probar resolucion baja y UI scale alto.

Promociones:

1. Pasar a hardmode o simular estado de hardmode.
2. Subir afinidades.
3. Confirmar promocion esperada.
4. Confirmar reward una sola vez.
5. Confirmar que reward no aparece si la Soul activa no corresponde.

