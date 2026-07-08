# Roadmap y puntos debiles

## P0 (RESUELTO 2026-07-06) - Summoner conserva 4 rutas (Opcion B)

RESUELTO: se eligio la Opcion B (ver `decision-log.md`). Summoner mantiene Beast
Tamer, Advanced Summoner, Tech Summoner y Necromancer. No era un bloqueo: las 4
rutas ya funcionaban y estaban gateadas. No se poda nada; no hace falta migracion
de save por promociones eliminadas. El texto de abajo queda como contexto
historico.

Problema (historico):

El usuario pidio descartar Cartomancer y crear una promocion nueva para
Summoner. `Necromancer` ya existe, pero el codigo actual conserva:

- `Beast Tamer`
- `Advanced Summoner`
- `Tech Summoner`
- `Necromancer`

como promociones validas de Summoner.

Riesgo:

- El diseno y el codigo pueden contradecirse.
- Saves pueden bloquearse en promociones Summoner obsoletas.
- Rewards y efectos pueden mantener contenido que ya no pertenece al mod.

Plan si se decide "Summoner solo Necromancer":

1. Cambiar `ClassPromotionRules.IsPromotionForSoul` para Summoner.
2. Cambiar `ResolveSummonerPromotion` para resolver solo Necromancer o Summoner
   base segun regla nueva.
3. Migrar `SubclassPlayer.LoadData`:
   - Si `LockedSummonerPromotion` no es `Necromancer`, convertir a `None` o
     `Necromancer` segun decision de diseno.
4. Migrar `PromotionRewardPlayer.LoadData`:
   - Quitar `Beast Tamer`, `Advanced Summoner`, `Tech Summoner` de
     `AwardedPromotions`.
5. Quitar o desactivar:
   - `BeastWhip`
   - `FusionWhip`
   - `TechWhip`
   - sus proyectiles
   - efectos/pasivas asociadas si ya no aplican.
6. Actualizar tests.
7. Correr build + suite completa.

Plan si se conservan varias promociones Summoner temporalmente:

1. Documentar explicitamente que Necromancer no es unica promocion.
2. Mantener gating runtime para cada promocion.
3. Ajustar UI para comunicar las rutas Summoner reales.
4. No borrar rewards/items.

## P1 - Migracion de saves

Problema:

`SubclassPlayer` y `PromotionRewardPlayer` guardan strings crudos. Si se renombran
o eliminan promociones, el save puede quedar con estados imposibles.

Correccion recomendada:

- Agregar version de save.
- Sanitizar locked promotions al cargar.
- Sanitizar awarded promotions al cargar.
- Agregar smoke test de migracion source-level.

## P1 - Prueba in-game de reload y assets

Los smoke tests no sustituyen reload real de tModLoader.

Checklist:

- Reload sin `MissingResourceException`.
- Confirmar que todos los items nuevos tienen textura o fallback valido.
- Confirmar que no queda referencia a `CartomancerSoul`.
- Confirmar que UI no explota con screen scale.

## P1 (RESUELTO 2026-07-06) - Overlay clamp/position helper

RESUELTO: se agrego `EterniaUI.ClampWorldAnchored` y los 8 overlays anclados al
jugador (BaseClassResource, ArcherFocus, Berserker, EnergyHeat, Gunner,
StunnerCharge, Virtuoso, FighterCombo) rutean su drawPos por ahi. Guardado por
`tests/OverlayClampSourceSmokeTest.ps1`. Falta confirmar visualmente en juego.

Problema (historico):

Varios overlays anclados al jugador usan offsets fijos. Pueden quedar fuera de
pantalla.

Correccion recomendada:

- Crear helper en `EterniaUI` para rectangulos anclados a entidades con clamp.
- Migrar overlays uno por uno con test source-level.

## P2 - Stack manager bottom-left

Problema:

Varios paneles bottom-left usan margenes magicos independientes.

Correccion recomendada:

- Helper de stack en `EterniaUI`.
- Cada panel pide un slot con altura y gap.
- Tests que prohiban margenes hardcodeados nuevos.

## P2 - Localizacion

Duplicacion/namespaces RESUELTO (2026-07-06): la fuente viva y UNICA es la RAIZ
`en-US.hjson` (`Mods.Eternia.*`, gestionada por tModLoader). El
`Localization/en-US_Mods.ETERNIA.hjson` (`Mods.ETERNIA.*`) era el muerto y se
vacio. Los 7 tests de localizacion apuntan al archivo vivo; guardado por
`LocalizationIntegritySourceSmokeTest.ps1`. (Correccion: una consolidacion previa
tomo los archivos al reves; ver decision-log.) Falta reload in-game para confirmar
los tooltips y nombres de keybind.

Hay mucho texto en UI y tooltips. Parte ya esta en hjson, parte sigue hardcoded.

Correccion recomendada:

- Mover textos user-facing a localization cuando el contenido deje de cambiar
  tanto.
- Mantener nombres de keys estables.

## P2 - Balance

Todavia falta balance real de:

- XP.
- stats.
- afinidades.
- pasivas.
- penalties.
- rewards.
- costos de habilidades.

No balancear solo desde codigo: probar en juego.

## P3 - Assets finales

Muchos contenidos usan iconos temporales o reutilizados. Antes de release:

- Crear sprites propios para Souls, armas, proyectiles y UI.
- Revisar Workshop metadata.
- Revisar `icon.png` y `icon_small.png`.
