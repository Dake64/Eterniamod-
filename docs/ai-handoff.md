# Handoff para IA

Este archivo esta escrito para una IA que continue el desarrollo, especialmente
Claude Code.

## Antes de editar

1. Lee en orden:
   - `docs/current-state.md`
   - `docs/claude-code-workflow.md`
   - `docs/gameplay-systems.md`
   - `docs/technical-architecture.md`
   - `docs/roadmap-known-issues.md`
2. Revisa el estado real con:

```powershell
git status --short
dotnet build .\ETERNIA.csproj
```

3. Usa CodeGraph para preguntas estructurales:
   - Donde se define un simbolo.
   - Quien llama a un metodo.
   - Que archivos se verian afectados.

4. Usa `rg` para busquedas literales.

## Reglas de trabajo

- No revertir cambios existentes salvo que el usuario lo pida explicitamente.
- No eliminar penalizaciones de Soul. Son parte central del diseno.
- No reintroducir `Cartomancer`.
- No asumir que los smoke tests prueban comportamiento in-game completo; son
  source smoke tests.
- Para cambios de comportamiento, escribir primero una prueba que falle y luego
  implementar.
- Mantener cambios acotados. El repo ya tiene mucho diff acumulado.
- Registrar cambios no triviales en `docs/change-log.md`.
- Registrar decisiones de producto/arquitectura en `docs/decision-log.md`.
- Antes de reorganizar archivos, seguir `docs/claude-code-workflow.md`.

## Invariantes de producto

- No tener NINGUNA Soul equipada aplica penalizacion fuerte + `SoulLessDebuff`
  ("Alma Perdida"), incluido el jugador recien creado. Un cuerpo siempre ocupa
  una Soul (cambio 2026-07-06; antes el fresh player no se castigaba).
- `EmptySoul` no cuenta como clase activa, pero equipada SI quita la penalizacion
  de "sin Soul" (cuenta como cuerpo con alma).
- Class Souls activan clase al equiparse como accesorios.
- Usar arma de otra clase con Class Soul activa mata al jugador.
- Las subclases y recompensas deben estar respaldadas por la Soul activa
  correcta.
- Cartomancer queda fuera por ahora.
- Summoner/Necromancer necesita una decision limpia: actualmente hay varias
  promociones Summoner en codigo, pero la direccion del usuario favorece
  Necromancer como reemplazo de Cartomancer.

## Lo mas importante si continuas

La siguiente decision tecnica grande es si `Summoner` debe tener solo
`Necromancer` como promocion o si se conservan temporalmente `Beast Tamer`,
`Advanced Summoner` y `Tech Summoner`.

Si la decision es "solo Necromancer":

- Cambiar `ClassPromotionRules.IsPromotionForSoul`.
- Cambiar `ResolveSummonerPromotion`.
- Migrar/sanitizar `SubclassPlayer.LoadData`.
- Podar `PromotionRewardPlayer` y `AwardedPromotions`.
- Podar pasivas/rewards/items/efectos de promociones Summoner obsoletas.
- Actualizar tests:
  - `PromotionRulesSmokeTest.ps1`
  - `PromotionRewardsSourceSmokeTest.ps1`
  - `SubclassRuntimeGatingSourceSmokeTest.ps1`
  - `SummonerWhipSourceSmokeTest.ps1`

Si la decision es "varias promociones Summoner por ahora":

- Documentar que `Necromancer` no es la unica promocion.
- Mantener gating por Soul activa y subclase para todos los proyectiles/items.
- Asegurar que la UI no venda la idea de una sola ruta.

## Comandos de verificacion obligatorios

Despues de cambios de codigo:

```powershell
dotnet build .\ETERNIA.csproj
```

Despues de cambios de comportamiento o tests:

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

Antes de entregar:

```powershell
git diff --check
```

## Pruebas manuales recomendadas

En tModLoader:

1. Reload del mod sin excepciones de assets.
2. Nuevo personaje sin Soul: no debe recibir no-Soul penalty.
3. Hablar con EternalNPC: debe recibir `EmptySoul`.
4. Craftear `EmptySoul` a una de las 4 Class Souls.
5. Equipar la Soul: debe activar UI/progresion y entregar starter weapon.
6. Usar arma correcta: permitido.
7. Usar arma incorrecta: muerte inmediata.
8. Tener Class Soul en inventario pero no equipada: penalizacion fuerte.
9. Abrir/cerrar `Soul UI`, `Stats UI`, `Passive UI`: no deben solaparse.
10. Probar UI scale alto y resolucion baja.
