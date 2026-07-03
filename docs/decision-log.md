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

- Estado: Propuesta.
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

