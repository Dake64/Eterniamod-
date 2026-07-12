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

## 2026-07-10 - Los Escudos son una categoria de arma con Aura Defensiva (daño Generic)

- Estado: Aceptada (spec del usuario).
- Contexto: el usuario definio los "Escudos" como una categoria de arma completamente
  distinta. No hay golpe cuerpo a cuerpo tradicional: se mantiene clic izquierdo,
  el escudo se levanta y tras ~0.5s proyecta un Aura Defensiva que hace daño continuo
  a los enemigos cercanos mientras se sostenga. El "Escudero" (subclase Guardian de
  afinidad Defensa) es quien saca el maximo, pero CUALQUIER clase puede usar escudos.
- Decision:
  - Mecanica base compartida en `IShieldWeapon` + `DefensiveAuraProjectile`
    (canalizado con `Item.channel`, spin-up de 30 ticks, pulsos de daño manuales por
    radio pequeño ~2.5-3 tiles, se mata al soltar). Cada escudo define daño, intervalo
    de pulso, radio, color y un efecto de personalidad (OnAuraHit / OnAuraPulse).
  - Daño del aura = **DamageClass.Generic** (no Melee): para que "cualquier clase"
    lo use de verdad y se beneficie por igual. NO subclass-locked. Se decidio Generic
    sobre Melee precisamente por el requisito "cualquier clase puede utilizar escudos".
  - Payoff del Escudero (Guardian): `GuardianPlayer.AuraDamageMultiplier()` escala el
    aura con la Defensa (+1%/5 def) y `AuraRadiusMultiplier()` da +15% de radio, ambos
    solo si IsActiveGuardian. "Escalado del daño con la Defensa" del spec.
  - El TrainingShield (premio de promocion Guardian) se dejo INTACTO por ahora (sigue
    siendo melee LightRed, no-crafteable, subclass-locked) para no romper los tests de
    balance/promocion; convertirlo al aura es un paso posterior.
- Consecuencias:
  - La rama de pasivas de Defensa AUN es de stats planos (def/DR/vida). El spec quiere
    que las pasivas del Escudero moldeen el aura (daño, radio, efectos, duracion de
    guardia, boosts temporales) -> pendiente: rediseñar la rama como se hizo con Combo.
  - Sin recurso de "Resistencia" todavia (el spec lo menciona como futuro: mantener el
    aura podria consumirlo).
  - Balance por tunear in-game; nada probado en juego.
- Archivos relacionados: Content/Items/IShieldWeapon.cs,
  Content/Projectiles/Guardian/DefensiveAuraProjectile.cs,
  Content/Items/Weapons/Guardian/{Wooden,Iron,Corrupt,Glacial,Ember,Holy}Shield.cs,
  Content/Players/GuardianPlayer.cs.

## 2026-07-10 - El Combo del Peleador es pasivo-driven (no da bonos por si solo)

- Estado: Aceptada (spec del usuario).
- Contexto: el Fighter daba bonos directos por combo (move/atk speed, dano hasta 2x)
  y sus nodos de pasiva daban stats melee planos. El usuario definio una identidad
  distinta para el "Peleador".
- Decision:
  - El Combo por si mismo NO otorga nada. Es un contador (20 base, 2.5s) que el
    ARBOL de pasivas convierte en efectos. Esto permite builds distintas sin cambiar
    la mecanica base.
  - Los nodos de la rama Combo dejan de dar stats planos (se quitaron de
    EterniaStatsPlayer) y ahora modifican el Combo (efectos leidos en FighterPlayer
    con `HasActivePassive("<nodo>")`, que ademas satisface el test de coverage).
  - El keystone de Combo es un buff CONDICIONAL (Frenesi al maximo), manejado por
    FighterPlayer, no un bono plano en KeystonePlayer.
  - Remate = tecnica de la tecla Skill que gasta el combo (paralelo a la Ejecucion
    del Espadachin). La distancia sigue premiando el cuerpo a cuerpo (100% pegado).
- Consecuencias:
  - Un Peleador sin pasivas de Combo no obtiene beneficio del combo -> se espera que
    invierta la rama (coherente con "el arbol define la build").
  - Balance por tunear in-game.
- Archivos: `Content/Players/FighterPlayer.cs`, `FighterSkillPlayer.cs`,
  `Content/Projectiles/FighterPunchProjectile.cs`, `Content/Passives/PassiveRegistry.cs`,
  `Content/Players/EterniaStatsPlayer.cs`, `Content/Players/KeystonePlayer.cs`.

## 2026-07-09 - Un recurso es una mecanica de SUBCLASE, no de clase base

- Estado: Aceptada.
- Contexto: la clase base (sin promover) tenia su propio recurso de combate
  (Momentum / Charge / Focus / Bond): subia al pegar, decaia, y escalaba dano y
  velocidad. Genero varias iteraciones fallidas (buff, nodo de desbloqueo, barra).
  El usuario decidio quitarlo.
- Decision:
  - Las 4 clases base NO tienen recurso ni medidor. Su identidad pre-promocion son
    los `+stat` fijos de `BaseClassPlayer.PostUpdateEquips`.
  - Un "recurso" (barra/medidor + tecnica) es exclusivamente una mecanica de
    SUBCLASE. Esto refuerza que la promocion es el momento en que la clase "cobra
    vida" mecanicamente.
  - `BaseClassPlayer` se conserva solo por los `+stat` y por `IsActiveBaseClass`
    (que el test de gating exige como punto unico de chequeo).
- Consecuencias:
  - La pre-promocion es mas simple pero mas sosa; validar in-game si hace falta
    compensar los `+stat` fijos.
  - Se elimino `BaseClassResourceUI` y su test; `SoulUI` muestra "None - promote to
    gain one" en la fila Resource de una clase base.
- Archivos: `Content/Players/BaseClassPlayer.cs`, `Content/UI/SoulUI.cs`.

## 2026-07-09 - [DESCARTADA] Recursos como buff (no barras flotantes) + nodo "Core" de desbloqueo

- Estado: DESCARTADA (revertida el mismo dia). El usuario decidio que el buff+nodo
  del recurso base fue mala idea y pidio volver a la barra flotante pero mejorada.
  En su lugar: la barra flotante base ahora solo aparece cuando hay recurso (fade
  in/out) y tiene un dibujo mas pulido. La idea de "recursos como buff" para las
  subclases NO se aplico. Ver change-log 2026-07-09 (REVERT + mejora visual).
- Contexto: las barras de recurso flotando sobre el personaje se ven mal. El usuario
  pidio mostrarlas como buff y desbloquear el recurso de clase base con un nodo.
- Decision:
  - Los recursos se muestran como BUFF en la barra de buffs, no como barra flotante.
    Se usa UN solo buff dinamico (`ClassResourceBuff`) cuyo nombre/tooltip se leen en
    vivo del recurso activo -> 1 clase + 1 icono sirven para cualquier recurso (no 22
    clases/iconos). El icono se genera por codigo (orbe) por falta de arte a mano.
  - "Desbloquear con un nodo": se introduce un pseudo-branch de afinidad **"Core"**
    con 1 nodo por clase. Encaja en el sistema existente porque los grupos del arbol
    se arman dinamicamente por afinidad y `AddAffinity`/sidebar ignoran afinidades
    desconocidas; solo hubo que excluir "Core" del padding. Es la via de menor riesgo
    para meter un nodo "de clase" fuera de las ramas de afinidad.
  - Las barras viejas NO se borran: se apagan tras un flag `readonly` (mantiene verdes
    los tests source que verifican esas UIs y permite revertir). Se limpiaran cuando
    la etapa 2 este validada in-game.
  - Etapa por etapa (no todo de golpe) porque tocar las 18 barras implica 13 UIs
    (algunas interactivas) + 6 tests y no hay validacion in-game.
- Consecuencias:
  - Un jugador nuevo debe gastar 1 punto en el nodo Core para activar su recurso base
    (antes era automatico). Es intencional ("desbloqueo").
  - Etapa 2: extender el mapeo del buff a las 18 subclases y apagar sus UIs.
- Archivos: `Content/Buffs/ClassResourceBuff.cs(.png)`, `Content/Players/BaseClassPlayer.cs`,
  `Content/Passives/PassiveRegistry.cs`, `Content/UI/PassiveUI.cs` (color "Core"),
  `Content/UI/BaseClassResourceUI.cs`.

## 2026-07-09 - Balance del arbol: las descripciones deben ser honestas y el cap de afinidad no debe matar la mitad profunda de la rama

- Estado: Aceptada.
- Contexto: auditoria pedida por el usuario ("cada nodo balanceado + se ve
  crecimiento"). Dos hallazgos: (1) varias descripciones de nodo no coincidian con
  el efecto real (p. ej. Music prometia "ally buffs" pero daba self-buffs porque
  las auras multiplayer no estan implementadas); (2) `ApplyAffinityMastery` capeaba
  la afinidad en 40, pero los nodos notables de una rama ya suman ~63 -> los ~11
  nodos Minor de relleno no daban ningun stat directo (capeados).
- Decision:
  - La descripcion de cada nodo DEBE reflejar el efecto runtime real. Cuando el
    efecto "ideal" (auras a aliados) no esta implementado, la descripcion se ajusta
    al self-buff que si se aplica (no se promete lo que no se da).
  - El cap de afinidad se sube a 75 para que llenar una rama completa (notables +
    Minor) siga aportando crecimiento en vez de aplanarse a mitad de camino.
  - Los efectos "por nombre" siguen en `EterniaStatsPlayer`/subclase; los Minor
    (nombre interpolado) siguen aportando solo via afinidad -> por eso el cap
    importa para que el relleno no sea inutil.
- Consecuencias:
  - Subir el cap sube ~+5% el bonus de mastery de una rama full-invertida; queda
    por validar in-game (no trivializa por si solo, es inversion de ~20 puntos).
  - Si a futuro se implementan auras reales de soporte (Music/Arcane), habra que
    volver a alinear descripcion<->efecto.
- Archivos: `Content/Passives/PassiveRegistry.cs` (descripciones),
  `Content/Players/EterniaStatsPlayer.cs` (`AffinityCap`).

## 2026-07-09 - Cada subclase debe tener una mecanica signature (no solo stats)

- Estado: Aceptada.
- Contexto: el usuario pidio mejorar la mecanica de cada subclase al estilo del
  Espadachin. Al auditar, 5 subclases (Infinity Mage, Arcane Bard, Beast Tamer,
  Advanced Summoner, Tech Summoner) eran solo `+stat` plano, sin identidad de
  juego; las otras 13 ya tenian recurso/tecnica propios.
- Decision:
  - Cada subclase tiene su propio `ModPlayer` con una mecanica signature =
    recurso construido en combate + payoff (pasivo continuo y/o tecnica activa).
    Patron calcado del Espadachin: `IsActiveX()` para gating, `SkillKey` +
    `SkillPlayer` para la tecnica con cooldown compartido, feedback CombatText/Dust.
  - Cada una debe SENTIRSE distinta (verbo distinto): build-and-spend (Infinity),
    momento-que-decae sin gasto (Bard), frenesi de manada (Beast), sinergia por
    tropa llena + exceder tope (Advanced), bateria + escudo (Tech).
  - Los `+stat` base preexistentes en `SubclassEffectsPlayer` se conservan como
    baseline; la mecanica stackea encima (no se revierte nada existente).
- Consecuencias:
  - `SkillKey` (Q) queda multiplexado por subclase: no colisiona porque solo una
    subclase esta activa a la vez y cada handler filtra con su `IsActiveX()`.
  - El balance del baseline+mecanica queda por tunear con pruebas in-game.
- Archivos relacionados: `Content/Players/{InfinityMage,ArcaneBard,BeastTamer,
  AdvancedSummoner,TechSummoner}Player.cs`, `Content/Players/SubclassEffectsPlayer.cs`,
  `Content/Players/SkillPlayer.cs`, `Content/Systems/EterniaKeybinds.cs`.

## 2026-07-08 - Sangrado: solo espadas y chance visible (reversion de spec)

- Estado: Aceptada (revierte dos puntos del spec original del Espadachin, a
  peticion explicita del usuario).
- Contexto: el spec original decia "armas de filo compatibles" con "probabilidad
  completamente oculta". El usuario pidio cambiarlo: solo **espadas** y **mostrar
  el porcentaje**.
- Decision:
  - El sangrado se ata al **tipo de arma** via `EterniaGlobalItem.IsSword`
    (Swing/Rapier + melee, sin pick/axe/hammer). Cualquier espada, vanilla o del
    mod, lo aplica para un Guerrero activo.
  - `IBleedWeapon` deja de ser "el unico que sangra" y pasa a ser un **override de
    chance** para espadas insignia; el resto usa `DefaultSwordBleedChance`.
  - El chance es **visible** en el tooltip (solo para Guerreros activos), mostrando
    el efectivo (base + afinidad Bleed).
- Consecuencias:
  - Mecanica mas tangible: funciona con la espada que el jugador tenga, y es
    transparente. Si se quisiera limitar a espadas del mod, bastaria con exigir
    `IBleedWeapon` en `IsSword`.
- Archivos relacionados:
  - `Content/Globals/EterniaGlobalItem.cs`, `Content/Players/WarriorBleedPlayer.cs`,
    `Content/Players/SwordsmanPlayer.cs`, `Content/Items/IBleedWeapon.cs`

## 2026-07-08 - Rastro Carmesi: recurso exclusivo del Espadachin (Fase 2)

- Estado: Aceptada.
- Contexto: Fase 2 del rediseno del Espadachin. El diseno pide un recurso propio,
  ganado en combate, sin auto-regen, gastado solo por tecnicas, con barra propia y
  logica separada del sangrado.
- Decision:
  - `CrimsonTrailPlayer` guarda el recurso; solo existe/acumula con
    `IsActiveSwordsman` (reusa el helper Soul-gated); `ResetEffects` lo pone en 0
    fuera de Espadachin. Sin `PostUpdate` (nada de regeneracion automatica). Gain
    en `SwordsmanPlayer.OnHitNPCWithItem` (12 primera sangre / 6 sostener). Persiste.
  - `SwordsmanSkillPlayer` = unico sumidero: tecla `SkillKey` (Q) + cooldown
    compartido `SkillPlayer`; consume `TechniqueCost` (50) y ejecuta un burst AoE
    sobre sangrantes cercanos (`SimpleStrikeNPC`), recuperando el "EXECUTE!".
  - `CrimsonTrailUI` = barra world-anchored solo-Espadachin (clon de `BerserkerUI`).
- Consecuencias:
  - Se recupera el burst que la Fase 1 habia retirado, ahora como gasto de recurso
    (decision del usuario), cerrando el bucle: golpear -> sangrar -> cargar -> Q.
  - El recurso es local del jugador (no necesita packet MP); el burst usa el
    netcode propio de `SimpleStrikeNPC`.
- Archivos relacionados:
  - `Content/Players/CrimsonTrailPlayer.cs`, `Content/Players/SwordsmanSkillPlayer.cs`,
    `Content/UI/CrimsonTrailUI.cs`, `Content/Players/SwordsmanPlayer.cs`

## 2026-07-08 - Sangrado = mecanica del Guerrero (no del Espadachin)

- Estado: Aceptada.
- Contexto: el usuario entrego un diseno del Espadachin. Pide que el Sangrado sea
  un debuff propio del mod, de las armas de filo con probabilidad oculta, de un
  solo nivel y dano fijo, controlado por el arbol del Guerrero y reutilizable por
  futuras subclases; y que el Espadachin lo explote via un recurso aparte (Rastro
  Carmesi). Se acordo hacerlo en fases: Fase 1 = el Sangrado del Guerrero.
- Decision:
  - `BleedDebuff : ModBuff` = debuff real y visible; DoT y atribucion de owner en
    `BleedGlobalNPC` (un nivel, `BaseBleedDamage` fijo + afinidad Bleed).
  - `IBleedWeapon` marca armas de filo con `BleedChance` **oculto**; la aplicacion
    se centraliza en `WarriorBleedPlayer` (class-wide, cualquier Warrior Soul).
  - El arbol Bleed modifica atributos via afinidad (chance/duracion/dano) + pasivas
    con nombre (`Blood Flow` duracion, `Execution` dano vs sangrante).
  - El Espadachin conserva su `OnHitNPCWithItem` gateado como **maestria**
    (garantiza el sangrado); ahi enganchara el Rastro Carmesi en Fase 2.
- Consecuencias:
  - Se **reemplaza** el sistema previo de 5 stacks + "EXECUTE!" (decision explicita
    del usuario: el EXECUTE pasa a ser una tecnica del Rastro Carmesi en Fase 2).
    En la Fase 1 el Espadachin queda sin ese burst hasta que llegue la Fase 2.
  - Se reescribio `tests/SwordsmanBleedSourceSmokeTest.ps1` al nuevo diseno.
  - Los tests de gating exigen que `SwordsmanPlayer` mantenga `IsActiveSwordsman` +
    `OnHitNPCWithItem`, y que `SubclassEffectsPlayer` mantenga los 18
    `IsActiveSubclass`; ambos se respetaron.
  - Sync MP del debuff es best-effort (cliente-driven), igual que el sistema previo.
- Archivos relacionados:
  - `Content/Buffs/BleedDebuff.cs`, `Content/Items/IBleedWeapon.cs`,
    `Content/Players/WarriorBleedPlayer.cs`, `Content/NPCs/BleedGlobalNPC.cs`,
    `Content/Players/SwordsmanPlayer.cs`, `Content/Players/SubclassEffectsPlayer.cs`

## 2026-07-08 - Profundidad de los arboles de pasivas: 5 nodos por rama

- Estado: Aceptada.
- Contexto: cada rama de afinidad tenia 3 nodos; el usuario pidio "mejorar el
  arbol de pasivas de cada clase porque ahorita solo hay 3 mejoras por subclase".
- Decision: profundizar cada una de las 18 ramas a 5 nodos con escalado uniforme
  (tier 4 = coste 2 / afinidad 6; tier 5 = coste 3 / afinidad 7) y un efecto real
  codificado por nombre en `EterniaStatsPlayer`, siguiendo el patron existente.
- Consecuencias:
  - La afinidad maxima por rama sube de 12 a 25, pero la promocion sigue siendo
    RELATIVA (gana la afinidad dominante en `ClassPromotionRules`, sin umbral
    absoluto), asi que subir simetricamente todas las ramas no rompe la seleccion
    de subclase; solo abarata llegar a la rama en la que mas inviertes.
  - Los efectos nuevos son bonos planos siempre-activos (via el gran metodo de
    `EterniaStatsPlayer`); los condicionales siguen en los ModPlayer de subclase
    (Swordsman/YoyoMaster/Stunner/etc.).
- Archivos relacionados:
  - `Content/Passives/PassiveRegistry.cs`
  - `Content/Players/EterniaStatsPlayer.cs`
  - `tests/PassiveTreeDepthSourceSmokeTest.ps1`

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

