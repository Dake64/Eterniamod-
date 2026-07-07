# Estado actual

Snapshot: 2026-07-03.

## Resumen

ETERNIA es un overhaul RPG para Terraria usando tModLoader. La vertical slice
actual gira alrededor de Souls de clase, progresion por afinidades, penalidades
duras por romper la identidad de clase, UI propia y primeras promociones
post-Wall of Flesh.

El objetivo de producto acordado hasta ahora:

- Clases base iniciales: `Warrior`, `Mage`, `Ranger`, `Summoner`.
- Flujo inicial:
  - Jugador nuevo no recibe penalizacion por no tener Soul.
  - NPC `EternalNPC` entrega `EmptySoul`.
  - `EmptySoul` se craftea en una de las 4 Class Souls y se consume.
  - La Class Soul debe equiparse como accesorio para activar clase.
- Penalizaciones no negociables:
  - Usar arma incorrecta con Class Soul activa mata al jugador.
  - No tener ninguna Soul equipada (`ActiveSoul == None`) aplica una penalizacion
    fuerte + `SoulLessDebuff` ("Alma Perdida"), incluido el jugador recien creado
    (cambio 2026-07-06). Equipar cualquier Soul, incluida Empty, la quita.
- `Cartomancer` esta descartado y sus residuos deben seguir eliminados.
- `Summoner` debe tener una promocion nueva orientada a `Necromancer`; el codigo
  ya contiene `Necromancer`, pero aun conserva otras promociones Summoner.

## Verificacion reciente

Ultima verificacion hecha antes de crear estos docs:

- `dotnet build .\ETERNIA.csproj`: correcto, 0 warnings, 0 errores.
- Suite completa de smoke tests: `ALL 49 SMOKE TESTS PASSED`.
- `git diff --check`: exit code 0. Solo hubo warnings de Git sobre normalizacion
  futura LF/CRLF.

No se hizo prueba visual dentro de tModLoader despues de los ultimos cambios.
Eso sigue siendo necesario antes de considerar listo el rework de UI.

## Cambios grandes ya presentes

- Sistema de Souls centralizado:
  - `Content/Souls/SoulId.cs`
  - `Content/Souls/SoulRules.cs`
  - `Content/Souls/SoulInventory.cs`
  - `Content/Items/Souls/ClassSoulItem.cs`
  - `Content/Items/Souls/EmptySoul.cs`
- Penalizaciones y activacion de clase:
  - `Content/Players/EterniaPlayer.cs`
- Progresion y promociones:
  - `Content/Progression/ClassPromotionRules.cs`
  - `Content/Players/SubclassPlayer.cs`
  - `Content/Players/PromotionRewardPlayer.cs`
- Rework fuerte de UI:
  - `Content/UI/EterniaUI.cs`
  - `Content/UI/PassiveUI.cs`
  - `Content/UI/StatsUI.cs`
  - `Content/UI/SoulUI.cs`
  - `Content/UI/ClassProgressionUI.cs`
  - overlays y barras de clase en `Content/UI/*UI.cs`
- Necromancer:
  - `Content/Players/NecromancerPlayer.cs`
  - `Content/Projectiles/Necromancer/*`
  - `Content/Items/Weapons/Summoner/BeginnerNecromancyBook.cs`
- Tests source-level:
  - `tests/*.ps1`

## Estado del worktree

El worktree esta intencionalmente muy modificado por la vertical slice. No
revertir cambios sin pedirlo explicitamente. Hay archivos nuevos, eliminaciones
de Cartomancer y cambios de UI/progresion. Tambien aparecen artefactos `obj/`
eliminados y warnings LF/CRLF en Git.

## Riesgos importantes aun abiertos

- Summoner conserva rutas `Beast Tamer`, `Advanced Summoner` y `Tech Summoner`
  en `ClassPromotionRules`, rewards y efectos. Esto contradice parcialmente la
  direccion de producto si la decision final es que Summoner promocione solo a
  `Necromancer`.
- Saves antiguos pueden tener `LockedSummonerPromotion` o `AwardedPromotions`
  con promociones Summoner obsoletas. Aun no hay migracion de save para eso.
- La UI ya es bastante mas robusta, pero varios overlays anclados al jugador
  todavia usan offsets fijos y no tienen clamp/flip contra bordes de pantalla.
- Falta prueba manual in-game de reload, UI scale y flujo real con NPC/crafting.

