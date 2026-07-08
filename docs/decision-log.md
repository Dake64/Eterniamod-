# Decision log

Este archivo registra decisiones de producto/arquitectura para que una IA nueva
no las redescubra ni las contradiga.

Estados:

- Propuesta
- Aceptada
- Reemplazada
- Descartada

Formato sugerido:

```md
## YYYY-MM-DD - Decision

- Estado:
- Contexto:
- Decision:
- Consecuencias:
- Archivos relacionados:
```

## 2026-07-06 - Activar una Class Soul ya no regala arma inicial

- Estado: Aceptada.
- Contexto: el usuario pidio que al activar una Class Soul ya no se entregue un
  arma inicial automatica.
- Decision: eliminar el sistema de arma inicial (`GiveStarterWeaponIfNeeded` y los
  flags `*StarterGiven`). La Soul solo define la clase.
- Consecuencias:
  - Las armas de clase (TrainingBlade/ApprenticeWand/TrainingBow/TrainingWhip) YA
    eran crafteables; se simplificaron a 10 madera en la mesa de trabajo para que
    cualquier clase se arme desde el minuto 1 (evita el death-trap de la Copper
    Shortsword). Mitigacion resuelta.
- Archivos relacionados:
  - `Content/Players/EterniaPlayer.cs`
  - `tests/StarterLoadoutSourceSmokeTest.ps1`

## 2026-07-06 - Penalizacion por no tener Soul equipada (invierte fresh-player)

- Estado: Aceptada.
- Contexto: el usuario probo con una Empty Soul en inventario y esperaba castigo.
  El diseno anterior solo penalizaba al poseer una Class Soul sin equipar, y dejaba
  sin castigo al jugador sin Souls. El usuario decidio: "un cuerpo siempre ocupa
  una soul" -> castigar SIEMPRE que no haya Soul equipada.
- Decision:
  - `ApplyNoSoulPenalty` se dispara cuando `ActiveSoul == None` (ninguna Soul
    equipada), incluido el jugador recien creado. REEMPLAZA el invariante previo
    "fresh player sin Soul no recibe penalizacion".
  - Equipar cualquier Soul (Empty o Class) quita la penalizacion. Empty no activa
    clase pero cuenta como "cuerpo con alma".
  - El debuff mostrado es `SoulLessDebuff` ("Alma Perdida" / "Un cuerpo sin alma
    no sirve..."). Se elimino el debuff redundante `NoSoulDebuff` (era huerfano:
    el texto vivo se mostraba en ingles y `SoulLessDebuff` no se aplicaba nunca).
  - La penalizacion ya NO depende de escanear el inventario
    (`HasClassSoulAvailable`/`HasAnyClassSoulItem`), por lo que la "fuga" de
    guardar la Soul en el banco/Void Vault queda cerrada de raiz.
- Consecuencias:
  - Quedan 2 debuffs: `SoulLessDebuff` (sin Soul) y `SoulViolationDebuff` (arma).
  - `SoulInventory.HasAnyClassSoulItem` queda sin uso en la penalizacion (se
    conserva como API; el onboarding del NPC sigue usando `HasAnySoulItem`).
  - Actualizados `ai-handoff.md`, `current-state.md`, `gameplay-systems.md`.
- Archivos relacionados:
  - `Content/Players/EterniaPlayer.cs`
  - `Content/Buffs/SoulLessDebuff.cs` (aplicado), `Content/Buffs/NoSoulDebuff.cs` (ELIMINADO)
  - `Localization/en-US_Mods.ETERNIA.hjson`
  - `tests/NoSoulPenaltySourceSmokeTest.ps1` (nuevo), `tests/SoulInventorySourceSmokeTest.ps1`, `tests/ResourceTexturePathsSourceSmokeTest.ps1`

## 2026-07-06 - Summoner conserva sus 4 rutas (Opcion B)

- Estado: Aceptada.
- Contexto: la entrada del 2026-07-03 dejo abierta la duda A vs B. Un assessment
  del codigo confirmo que las 4 subclases de Summoner (Beast Tamer, Advanced
  Summoner, Tech Summoner, Necromancer) estan implementadas (pasiva + arma de
  recompensa), compilan y estan gateadas por Soul/subclase. No hay nada roto ni
  bloqueo tecnico; Necromancer ya reemplazo a Cartomancer en el slot Shadow.
- Decision: Opcion B. Summoner mantiene sus 4 rutas, simetrico con Mage/Ranger.
  NO se recorta a solo Necromancer.
- Consecuencias:
  - No borrar `BeastWhip`/`FusionWhip`/`TechWhip` ni sus proyectiles/rewards.
  - No hace falta migracion de save por promociones eliminadas.
  - El "P0 - Decidir y limpiar Summoner" del roadmap queda cerrado como decision,
    no como bloqueo.
- Archivos relacionados:
  - `Content/Progression/ClassPromotionRules.cs`
  - `Content/Players/SubclassEffectsPlayer.cs`
  - `Content/Players/PromotionRewardPlayer.cs`
  - `docs/roadmap-known-issues.md`

## 2026-07-06 - Eternal NPC es Town NPC permanente

- Estado: Aceptada.
- Contexto: el NPC lo spawneaba un sistema solo para jugadores sin Soul y NO
  persistia al recargar el mundo (no era townNPC), asi que desaparecia. El usuario
  lo quiere permanente como la Guia.
- Decision: `EternalNPC` ahora es Town NPC (`townNPC=true`, aiStyle Passive,
  `CanTownNPCSpawn=true`, `SetNPCNameList`). Se muda a casas vacantes y se guarda
  con el mundo -> persiste. Se conserva `EternalNPCSystem` como fallback de
  onboarding (spawnea uno homeless para jugadores sin Soul y sin casa; no duplica
  porque revisa `NPC.AnyNPCs`).
- Consecuencias:
  - Requiere textura `_Head`. Se creo `Content/NPCs/EternalNPC_Head.png` como
    PLACEHOLDER (copia del sprite del cuerpo) -> reemplazar por un head propio.
  - El jugador necesita una casa vacante para que se mude (comportamiento townNPC).
  - Tests de chat/system sin cambios (se preservaron los patrones esperados).
- Archivos relacionados:
  - `Content/NPCs/EternalNPC.cs`
  - `Content/NPCs/EternalNPC_Head.png` (placeholder)
  - `Content/Systems/EternalNPCSystem.cs`

## 2026-07-06 - CORRECCION: en-US.hjson raiz es el archivo VIVO

- Estado: Aceptada. Reemplaza a "Localizacion: fuente canonica unica" (abajo),
  que tenia los archivos AL REVES.
- Contexto: la evidencia in-game (el debuff mostraba la clave cruda
  `Mods.Eternia.Buffs.SoulLessDebuff.Description`) demostro que el mod resuelve
  `Mods.Eternia.*`, es decir el `en-US.hjson` RAIZ, que tModLoader regenera y
  gestiona. El `Localization/en-US_Mods.ETERNIA.hjson` (`Mods.ETERNIA.*`) es el
  MUERTO. El workflow asumio el nombre interno `ETERNIA` desde la clase `Mod` y se
  equivoco; el efectivo para localizacion es `Eternia`.
- Decision: `en-US.hjson` (raiz, `Mods.Eternia.*`) es la fuente VIVA y UNICA. Los
  textos reales (ej. `SoulLessDebuff` = "Lost Soul") van AHI.
- Estado de ejecucion: COMPLETADO (2026-07-06). Se porto el contenido al archivo
  vivo, se re-apuntaron los 7 tests de localizacion a `en-US.hjson`, y se vacio el
  `Localization/en-US_Mods.ETERNIA.hjson` (muerto). Sigue en disco vaciado por
  OneDrive/tModLoader; para borrarlo, sacar el repo de OneDrive y `git rm`.
- Archivos relacionados:
  - `en-US.hjson` (vivo, gestionado por tModLoader)
  - `Localization/en-US_Mods.ETERNIA.hjson` (muerto)

## 2026-07-06 - Localizacion: fuente canonica unica [REEMPLAZADA - AL REVES]

- Estado: Reemplazada (tenia los archivos invertidos; ver correccion arriba).
- Contexto: existian dos archivos de localizacion. El `en-US.hjson` raiz estaba
  100% muerto (namespace `Mods.Eternia.*`; el nombre interno del mod es
  `ETERNIA`). El archivo `Localization/en-US_Mods.ETERNIA.hjson` es el que resuelve
  in-game via auto-prefijo `Mods.ETERNIA.*`.
- Decision: `Localization/en-US_Mods.ETERNIA.hjson` es la unica fuente de verdad.
  Se elimino `en-US.hjson`. Toda localizacion nueva va SOLO al archivo canonico
  como bloques top-level (Buffs, Items, NPCs, Projectiles, Keybinds), sin envolver
  en `Mods: {...}` (eso lo hace el auto-prefijo).
- Consecuencias:
  - Los tests de localizacion apuntan solo al archivo canonico.
  - `LocalizationIntegritySourceSmokeTest.ps1` protege contra self-refs, bloques
    `Mods:` muertos, placeholders y tooltips vacios.
- Archivos relacionados:
  - `Localization/en-US_Mods.ETERNIA.hjson`
  - `tests/LocalizationIntegritySourceSmokeTest.ps1`

## 2026-07-03 - Cartomancer queda descartado

- Estado: Aceptada.
- Contexto: el usuario indico que `Cartomancer` queda descartado de momento.
- Decision: no reintroducir `Cartomancer`, `CartomancerSoul`, armas/cartas o
  proyectiles de cartas.
- Consecuencias:
  - Mantener `RemovedCartomancerResidueSmokeTest.ps1`.
  - Evitar nombres, assets o localizacion de Cartomancer.
- Archivos relacionados:
  - `tests/RemovedCartomancerResidueSmokeTest.ps1`
  - `Content/Progression/ClassPromotionRules.cs`
  - `Content/Players/PromotionRewardPlayer.cs`

## 2026-07-03 - Penalizaciones de Soul son parte central del mod

- Estado: Aceptada.
- Contexto: el usuario pidio conservar la penalizacion de matar al jugador por
  arma incorrecta y una penalizacion fuerte por no tener Soul activa.
- Decision:
  - Arma incorrecta con Class Soul activa mata al jugador.
  - Tener Class Soul disponible pero no activa aplica no-Soul penalty fuerte.
  - Jugador nuevo sin Class Soul no debe ser castigado.
- Consecuencias:
  - No suavizar estas reglas sin aprobacion explicita.
  - Cualquier sistema nuevo de arma/proyectil debe respetar `SoulRules`.
- Archivos relacionados:
  - `Content/Players/EterniaPlayer.cs`
  - `Content/Souls/SoulRules.cs`
  - `Content/Souls/SoulInventory.cs`
  - `tests/SoulRulesBehaviorSmokeTest.ps1`
  - `tests/DamageClassCompatibilitySourceSmokeTest.ps1`

## 2026-07-03 - Souls se activan como accesorios

- Estado: Aceptada.
- Contexto: el flujo correcto definido por el usuario es recibir `EmptySoul` y
  craftearla a una de las 4 Class Souls.
- Decision:
  - `EmptySoul` no activa clase.
  - Class Souls activan clase al equiparse en slot de accesorio.
  - La receta de cada Class Soul consume `EmptySoul`.
- Consecuencias:
  - No volver a un sistema de Soul slot custom sin nueva decision.
  - UI debe mostrar estado activo desde `EterniaPlayer.ActiveSoul`.
- Archivos relacionados:
  - `Content/Items/Souls/ClassSoulItem.cs`
  - `Content/Items/Souls/EmptySoul.cs`
  - `Content/Players/EterniaPlayer.cs`
  - `tests/RemovedSoulSlotSystemSourceSmokeTest.ps1`
  - `tests/SoulInventorySourceSmokeTest.ps1`

## 2026-07-03 - Summoner/Necromancer aun requiere decision final

- Estado: Reemplazada (resuelta 2026-07-06 como Opcion B).
- Contexto: el usuario pidio implementar una nueva promocion para Summoner y se
  implemento `Necromancer`, pero el codigo aun conserva `Beast Tamer`,
  `Advanced Summoner` y `Tech Summoner`.
- Decision pendiente:
  - Opcion A: Summoner promociona solo a `Necromancer`.
  - Opcion B: `Necromancer` es una ruta mas dentro de varias promociones
    Summoner temporales.
- Consecuencias:
  - Si se elige opcion A, hay que limpiar reglas, rewards, pasivas, items,
    proyectiles, saves y tests.
  - Si se elige opcion B, hay que documentarlo como direccion oficial y ajustar
    UI/copy para no prometer una sola promocion.
- Archivos relacionados:
  - `Content/Progression/ClassPromotionRules.cs`
  - `Content/Players/SubclassPlayer.cs`
  - `Content/Players/PromotionRewardPlayer.cs`
  - `Content/Projectiles/Summoner/*`
  - `Content/Items/Weapons/Promotion/BeastWhip.cs`
  - `Content/Items/Weapons/Promotion/FusionWhip.cs`
  - `Content/Items/Weapons/Promotion/TechWhip.cs`
  - `docs/roadmap-known-issues.md`

## 2026-07-03 - UI debe centralizarse en EterniaUI

- Estado: Aceptada.
- Contexto: la UI anterior tenia paneles simples, malos layouts y overflows.
- Decision:
  - Nuevas UIs deben usar `EterniaUI` para paneles, texto, botones, tooltips,
    colores y close buttons.
  - Paneles grandes deben ser mutuamente exclusivos.
  - UI player-bound debe usar `ShouldDrawPlayerUI`.
- Consecuencias:
  - Evitar nuevos paneles con coordenadas fijas sin clamp.
  - Agregar tests source-level para patrones de UI.
- Archivos relacionados:
  - `Content/UI/EterniaUI.cs`
  - `Content/UI/PassiveUI.cs`
  - `Content/UI/StatsUI.cs`
  - `Content/UI/SoulUI.cs`
  - `Content/UI/SoulUISystem.cs`
  - `tests/UIReworkSourceSmokeTest.ps1`
  - `tests/ResponsiveUILayoutSourceSmokeTest.ps1`
  - `tests/UILifecycleSourceSmokeTest.ps1`

