# Change log de desarrollo

Este archivo es para continuidad entre IAs. Registrar aqui cambios no triviales
que quedan en el worktree.

Formato sugerido por entrada:

```md
## YYYY-MM-DD - Titulo corto

- Objetivo:
- Archivos principales:
- Cambios:
- Verificacion:
- Pendientes/riesgos:
```

## 2026-07-06 - Banner de promocion (fuera del chat) + sonido

- Objetivo: la promocion a subclase salia como mensaje de chat; darle fanfarria.
- Archivos: `Content/UI/PromotionBannerUI.cs` (nuevo),
  `Content/Players/PromotionRewardPlayer.cs`,
  `tests/PromotionBannerSourceSmokeTest.ps1` (nuevo).
- Cambios: nuevo `PromotionBannerUI` -> banner dramatico ("PROMOTION!" + nombre de
  la subclase, con glow y borde pulsante, ~3.7s). Al promocionar dispara el banner
  + sonido (`SoundID.Item37`) en vez de `Main.NewText`. Se limpio
  `GivePromotionReward` (ya no manda chat).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 63/63. Falta reload in-game.

## 2026-07-06 - Armas de clase crafteables desde el minuto 1

- Objetivo: tras quitar el arma inicial, asegurar que ninguna clase arranque rota.
- Contexto: las 4 armas de clase YA tenian receta (mi nota previa de "sin fuente"
  estaba equivocada). Se simplificaron las 2 con gate temprano (ApprenticeWand
  pedia Fallen Star = noche; TrainingWhip pedia Rope) a solo 10 madera, como las
  otras dos, para que cualquier clase se arme desde el minuto 1.
- Archivos: `Content/Items/Weapons/Magic/ApprenticeWand.cs`,
  `Content/Items/Weapons/Summoner/TrainingWhip.cs`,
  `tests/StarterWeaponRecipeSourceSmokeTest.ps1` (nuevo).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 62/62.

## 2026-07-06 - Sin arma inicial al activar Class Soul

- Objetivo: activar una Class Soul ya NO regala un arma inicial (pedido del usuario).
- Archivos: `Content/Players/EterniaPlayer.cs`, `tests/StarterLoadoutSourceSmokeTest.ps1`,
  docs (`gameplay-systems`, `ai-handoff`, `decision-log`).
- Cambios: eliminado todo el sistema de arma inicial (`GiveStarterWeaponIfNeeded`,
  `GiveStarterWeapon`, `GiveStarterStack`, `HasItem`, flags `*StarterGiven` y su
  save/load). El test se repuso para verificar la ELIMINACION.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 61/61.
- Pendientes/riesgos: las armas starter quedan sin fuente; un Mage/Ranger/Summoner
  nuevo no tiene arma de clase y el Copper Shortsword vanilla (melee) lo mataria.
  Decidir mitigacion (craftear armas de clase, o algo).

## 2026-07-06 - Aviso de puntos sin gastar en la barra de EXP

- Objetivo: recordar al jugador que tiene Stat/Passive points por gastar.
- Archivos: `Content/UI/ExpBarUI.cs`,
  `tests/UnspentPointsHintSourceSmokeTest.ps1` (nuevo).
- Cambios: cuando hay puntos sin gastar, la barra de EXP crece y muestra una linea
  PULSANTE ("N Stat & M Passive points to spend"); sin puntos, queda compacta.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 61/61. Falta reload in-game.

## 2026-07-06 - Banner de level-up (fuera del chat) + sonido

- Objetivo: quitar el spam de 3 mensajes de chat al subir de nivel.
- Archivos: `Content/UI/LevelUpBannerUI.cs` (nuevo),
  `Content/Players/EterniaLevelPlayer.cs`,
  `tests/LevelUpBannerSourceSmokeTest.ps1` (nuevo).
- Cambios:
  - Nuevo `LevelUpBannerUI`: banner centrado que aparece, sube un poco y se
    desvanece (~2.5s) mostrando "LEVEL UP!", el nivel y las recompensas
    (+Stats / +Passives, acumuladas si subes varios niveles de golpe).
  - `LevelUp` ahora dispara el banner + un sonido (`SoundID.Item4`) en vez de 3
    `Main.NewText`. Se conserva el CombatText "LEVEL X" sobre el jugador.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 60/60. Falta reload in-game.

## 2026-07-06 - HUD movido a arriba-centro (fuera del chat)

- Objetivo: los paneles en abajo-izquierda los tapaba el chat.
- Archivos: `Content/UI/EterniaUI.cs` (helper `GetTopCenterPanel`), `ExpBarUI.cs`,
  `ClassProgressionUI.cs`, `CursedMageUI.cs`, `ElementalistUI.cs`,
  `NecromancerUI.cs`, `tests/ExpBarPlacementSourceSmokeTest.ps1` (nuevo).
- Cambios: nuevos helpers `GetTopCenterPanel` y `GetTopRowPanel`. Se movieron
  TODOS los paneles de abajo-izquierda a un HUD arriba-centro (despejado del chat
  abajo-izq, minimapa arriba-der, y barra de vida de boss abajo-centro): la barra
  de EXP y la progresion de clase van LADO A LADO en una fila arriba (via
  `GetTopRowPanel`), y los medidores de subclase Cursed/Elementalist/Necromancer
  (mutuamente exclusivos) van centrados justo debajo de esa fila.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 59/59. Falta reload in-game
  para confirmar el stack (y ajustar posiciones si algo se encima).

## 2026-07-06 - Badge de nivel en todos los enemigos + flair por tier

- Objetivo: el usuario reporto que los mobs (mayormente Common) no mostraban
  nivel/rareza, y pidio algo mas caracteristico por rareza.
- Archivos: `Content/Globals/EterniaGlobalNPC.cs`,
  `tests/BossRarityBadgeSourceSmokeTest.ps1`.
- Cambios:
  - `PostDraw` ahora muestra badge en TODO enemigo real (gate por `ShouldIgnore`,
    que excluye critters / town / friendly). Common no-boss recibe una etiqueta
    minima "Lv.X" (gris, pequena, sin placa) para no saturar; no-Common y bosses
    reciben el badge completo con placa/glow.
  - Flair por tier: Mythic/Ancient/Nightmare tienen un segundo anillo exterior
    contra-rotatorio; el aura de Nightmare TIEMBLA (se siente inestable/aterrador).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 58/58. Falta reload in-game.

## 2026-07-06 - Escalera de rareza ampliada (Mythic/Ancient/Nightmare)

- Objetivo: mas emocion cuando cae un enemigo muy raro.
- Archivos: `Content/Globals/EterniaGlobalNPC.cs`, `tests/RarityVisualsSourceSmokeTest.ps1`.
- Cambios:
  - 3 tiers nuevos sobre Legendary: **Mythic** (~0.8%), **Ancient** (~0.3%),
    **Nightmare** (~0.1%) en enemigos normales; los bosses sesgan mucho mas raro.
    Ladder normal: Common 68 / Uncommon 17 / Rare 8 / SuperRare 4 / Legendary 1.8 /
    Mythic 0.8 / Ancient 0.3 / Nightmare 0.1 (%).
  - Cada tier con color propio (Mythic violeta, Ancient teal, Nightmare rojo
    sangre), particula (PurpleTorch / IceTorch / Shadowflame), intensidad de aura
    y stats. Tanques con daño moderado: se respeto el guard de no-8x vida / 4x
    daño-defensa en enemigos NORMALES; los bosses escalan un poco mas.
  - XP escala fuerte con la rareza (Mythic x9, Ancient x15, Nightmare x25).
  - Aura limitada a 18 copias por perf.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 58/58. Falta reload in-game.

## 2026-07-06 - Juice visual de rareza (aura + badge dramatico)

- Objetivo: que las rarezas altas (SuperRare/Legendary) se sientan amenazantes.
- Archivos: `Content/Globals/EterniaGlobalNPC.cs`,
  `tests/RarityVisualsSourceSmokeTest.ps1` (nuevo).
- Cambios (todo escala con `GetRarityIntensity`):
  - `PreDraw`: aura de ecos tintados del color de rareza alrededor del enemigo,
    pulsante y giratoria. Legendary = halo grande y ominoso; Uncommon = sutil.
  - `DrawEffects`: luz que late + particulas mas densas/grandes segun rareza
    (`RedTorch` para Legendary, `GoldFlame` para SuperRare, `Enchanted_Gold` resto).
  - Badge (`DrawEnemyBadge`): placa de fondo semitransparente + subrayado de color
    + glow-halo detras del texto, con tamano y pulso segun rareza.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 58/58. Falta reload in-game;
  los numeros (intensidad, radios, alfas) son faciles de ajustar al gusto.

## 2026-07-06 - Los bosses siempre muestran su badge de rareza/nivel

- Objetivo: los bosses casi nunca mostraban el badge "Rareza Lv.X" porque rolean
  Common ~65% (BossProfiles) y el badge se saltaba para Common (reportado).
- Archivos: `Content/Globals/EterniaGlobalNPC.cs`,
  `tests/BossRarityBadgeSourceSmokeTest.ps1` (nuevo).
- Cambios: `PostDraw` ahora solo salta el badge para enemigos Common NO-boss; los
  bosses siempre muestran su rareza + nivel (incluido Common). TDD.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 57/57. Falta reload in-game.

## 2026-07-06 - Fix z-order de tooltips (Stats + Passive)

- Objetivo: los tooltips de hover salian DETRAS de las filas/paneles siguientes
  porque se dibujaban inline a mitad del loop (reportado por el usuario).
- Archivos principales:
  - `Content/UI/EterniaUI.cs` (`QueueTooltip` + `DrawQueuedTooltip`)
  - `Content/UI/StatsUI.cs`, `Content/UI/PassiveUI.cs`
  - `tests/TooltipZOrderSourceSmokeTest.ps1` (nuevo)
- Cambios:
  - Tooltip diferido en `EterniaUI`: `QueueTooltip` guarda el tooltip durante el
    pase y `DrawQueuedTooltip` lo pinta al FINAL (encima de todo).
  - `StatsUI` y `PassiveUI` encolan el tooltip de hover y lo pintan al final del
    panel -> ya no queda detras. Ninguna otra UI dibuja tooltips inline.
  - TDD: `TooltipZOrderSourceSmokeTest` (fallando primero). Ajustado
    `UIReworkSourceSmokeTest` al nuevo patron.
- Verificacion:
  - `dotnet build -t:Compile`: 0/0. Suite: 56/56. Falta reload in-game.

## 2026-07-06 - Multijugador: rareza, penalizacion y XP MP-safe

- Objetivo: hacer el mod jugable en multijugador real (pedido del usuario).
- Archivos principales:
  - `Content/Globals/EterniaGlobalNPC.cs` (rareza server-side + sync; XP en red)
  - `Content/Players/EterniaPlayer.cs` (guard de penalizacion al jugador local)
  - `ETERNIA.cs` (HandlePacket + enum `EterniaMessageType`)
  - `tests/{EnemyRarityMultiplayer,SoulPenaltyMultiplayer,ExperienceMultiplayer}SourceSmokeTest.ps1` (nuevos)
- Cambios:
  - Rareza: se rolea SOLO en el servidor (guard `MultiplayerClient`) y se
    sincroniza via `SendExtraAI`/`ReceiveExtraAI`; el cliente reaplica scale/lifeMax
    una vez (guard idempotente `applied`). Ya no desync entre clientes.
  - Penalizacion de Soul: `PostUpdateEquips` solo actua sobre el jugador local
    (`whoAmI == Main.myPlayer`); ya no intenta matar/penalizar remotos.
  - XP: `OnKill` es server-authoritative; en servidor envia `ModPacket`
    `AddExperience` al cliente que mato; ese cliente aplica el XP y muestra el
    level-up. En SP se aplica directo. Se quito el broadcast global de `Main.NewText`.
  - TDD: 3 tests MP nuevos, escritos fallando primero.
- Verificacion:
  - `dotnet build -t:Compile`: 0 warnings, 0 errores. Suite: 55/55.
  - PENDIENTE: prueba real MP (servidor + 2 clientes).
- Pendientes/riesgos (MP menor): `EterniaAmmoGlobalItem` rand por-cliente;
  verificar proyectiles/minions en MP; bonos de stat en remotos son cosmeticos.

## 2026-07-06 - Hardening: pasada de bugs + ultimo espanol + plan v1

- Objetivo: revisar sistemas sin cobertura de test buscando bugs, completar la
  traduccion a ingles, y aterrizar el plan de v1.
- Archivos principales:
  - `Content/Players/EterniaLevelPlayer.cs`
  - `Content/Globals/EterniaGlobalItem.cs`
  - `docs/v1-checklist.md` (nuevo)
- Hallazgos y cambios:
  - Single-player: SIN crashes. XP/rareza/ammo solidos en SP.
  - Ultimo espanol user-facing: mensajes de level-up ("Ahora eres nivel...",
    "Stat Points obtenidos", "Passive Point obtenido") -> ingles ("You are now
    level...", "gained"). Tambien el comentario de `EterniaGlobalItem`. Barrido
    final: `Content` ya no tiene NADA de espanol.
  - MULTIJUGADOR (documentado, NO arreglado, seria enganoso medio-hacerlo): XP,
    rareza y ammo no son MP-safe (`OnKill` server-side + `Main.NewText` broadcast,
    `Main.rand` por-cliente en rareza/ammo, ModPlayer sin sync). Ver v1-checklist.
  - Nuevo `docs/v1-checklist.md`: inventario de sprites compartidos (art TODO),
    lista tecnica, y notas de metadata (la descripcion de Workshop esta
    desactualizada re: la regla de penalizacion).
- Verificacion:
  - `dotnet build -t:Compile`: 0 warnings, 0 errores. Suite: 52/52.

## 2026-07-06 - Clamp de overlays UI + null-guard de save

- Objetivo: que los overlays anclados al jugador no se salgan de pantalla en los
  bordes (roadmap P1) y endurecer el LoadData de rewards.
- Archivos principales:
  - `Content/UI/EterniaUI.cs` (helper `ClampWorldAnchored`)
  - `Content/UI/{BaseClassResource,ArcherFocus,Berserker,EnergyHeat,Gunner,StunnerCharge,Virtuoso,FighterCombo}UI.cs`
  - `Content/Players/PromotionRewardPlayer.cs`
  - `tests/OverlayClampSourceSmokeTest.ps1` (nuevo)
- Cambios:
  - Nuevo helper `EterniaUI.ClampWorldAnchored(anchor, offsetLeft, offsetTop,
    width, height)` que corre el drawPos para mantener el bounding box del overlay
    dentro de la pantalla (reusa `ClampToScreen`).
  - Los 8 overlays anclados al jugador rutean su drawPos por el helper, con su
    bounding box (incluye las pills que van por encima de la barra).
  - null-guard en `PromotionRewardPlayer.LoadData` (evita NRE si el tag
    `AwardedPromotions` deserializa a null en un save malformado).
  - TDD: `OverlayClampSourceSmokeTest.ps1` escrito fallando primero.
- Verificacion:
  - `dotnet build -t:Compile`: 0 warnings, 0 errores. Suite: 52/52 PASSED.
  - Falta reload in-game: pegarse a un borde de pantalla con una subclase activa y
    confirmar que la barra ya no se corta.
- Pendientes/riesgos:
  - Los bounding boxes son aproximados (barra + pills). Si algun elemento aun
    asoma en un borde, ajustar los offsets de ese overlay.

## 2026-07-06 - Consolidacion de localizacion COMPLETADA (fuente: en-US.hjson)

- Objetivo: cerrar la localizacion con UNA fuente viva (`en-US.hjson`), portar
  contenido que estaba atrapado en el archivo muerto y re-apuntar los tests.
- Archivos principales:
  - `en-US.hjson` (fuente viva)
  - `Localization/en-US_Mods.ETERNIA.hjson` (vaciado / retirado)
  - `tests/{LocalizationContent,NPCLocalization,KeybindDefaults,SummonerWhip,PromotionLocalization,RemovedCartomancerResidue,LocalizationIntegrity}SourceSmokeTest.ps1`
- Cambios:
  - Limpiados los nombres de keybinds ("Toggle  Soul  U I" -> "Toggle Soul UI",
    etc.) en el archivo vivo.
  - Portados al archivo vivo los tooltips reales que solo vivian en el muerto:
    WeakCurse, BloodCurse, TrainingBlade (Momentum), ApprenticeWand (Charge),
    TrainingBow (Focus), TrainingWhip (Bond).
  - Eliminada la clave huerfana `NoSoulDebuff` del archivo vivo (clase ya borrada).
  - Re-apuntados los 7 tests de localizacion a `en-US.hjson` (antes validaban el
    archivo muerto = confianza falsa). `LocalizationIntegrity` reescrito para el
    archivo vivo (self-ref, labels con doble espacio, nombre del debuff).
  - Vaciado el `Localization/en-US_Mods.ETERNIA.hjson` (comentario + objeto vacio).
- Verificacion:
  - Suite: 51/51 PASSED. `git diff --check`: exit 0.
- Pendientes/riesgos:
  - Falta reload in-game para ver los tooltips y nombres de keybind ya bien.
  - El archivo muerto sigue en disco (vaciado) por OneDrive/tModLoader; para
    borrarlo de verdad, mover el repo fuera de OneDrive y `git rm`.

## 2026-07-06 - Eternal NPC convertido a Town NPC permanente

- Objetivo: que el Eternal NPC persista y este siempre disponible (desaparecia al
  recargar el mundo porque no era townNPC).
- Archivos principales:
  - `Content/NPCs/EternalNPC.cs`
  - `Content/NPCs/EternalNPC_Head.png` (nuevo, placeholder)
- Cambios:
  - `EternalNPC` -> `townNPC=true`, defaults de town NPC, `CanTownNPCSpawn=true`,
    `SetNPCNameList` ["Eternal"]. Ahora se guarda con el mundo y se muda a casas.
  - Se creo una textura `_Head` placeholder (copia del sprite del cuerpo).
  - Dialogo/boton sin cambios (ya en ingles); `EternalNPCSystem` intacto (fallback).
- Verificacion:
  - `dotnet build -t:Compile`: 0 warnings, 0 errores.
  - Suite: 51/51 PASSED.
  - Falta reload in-game: darle una casa vacante y confirmar que se muda y persiste.
- Pendientes/riesgos:
  - Reemplazar `EternalNPC_Head.png` por un head propio (hoy es el sprite del cuerpo,
    se vera grande en el icono del mapa/casa).

## 2026-07-06 - Todo el texto visible pasa a ingles

- Objetivo: el usuario quiere el mod parejo en ingles. Traducir todo el texto
  hardcodeado en espanol que ve el jugador.
- Archivos principales:
  - `Content/NPCs/EternalNPC.cs` (dialogos + boton + mensajes NewText)
  - `Content/Souls/SoulRules.cs` (GetWrongWeaponMessage, 5 mensajes por clase)
  - `Content/Players/CursedMagePlayer.cs` (mensajes de muerte por corrupcion)
  - `Content/Players/PromotionRewardPlayer.cs` (mensaje de promocion)
  - `en-US.hjson` (Buffs SoulLessDebuff + SoulViolationDebuff a ingles)
- Cambios:
  - Todo el dialogo del Eternal NPC, los mensajes de arma incorrecta, muerte por
    corrupcion y promocion desbloqueada ahora en ingles.
  - Debuffs: `Lost Soul` / "A body without a soul is worthless..." y
    `Soul Violation` / "Your soul rejects the weapon you tried to use.".
  - Unico espanol restante: 2 comentarios internos en `EterniaGlobalItem.cs`
    (no visibles para el jugador; se dejaron como notas de dev).
- Verificacion:
  - `dotnet build -t:Compile`: 0 warnings, 0 errores.
  - Suite: 51/51 PASSED. `git diff --check`: exit 0.
  - Falta reload in-game para confirmar los textos del NPC.

## 2026-07-06 - CORRECCION localizacion: el archivo vivo es en-US.hjson raiz

- Objetivo: corregir la direccion de la consolidacion previa (estaba invertida) y
  arreglar el debuff que in-game mostraba la clave cruda.
- Contexto: la imagen in-game mostro `Mods.Eternia.Buffs.SoulLessDebuff.Description`,
  probando que el mod resuelve `Mods.Eternia.*` -> `en-US.hjson` RAIZ (gestionado
  por tModLoader), NO el `Localization/...ETERNIA.hjson` (`Mods.ETERNIA.*`, muerto).
- Cambios:
  - `en-US.hjson`: `Buffs.SoulLessDebuff` -> DisplayName "Alma Perdida",
    Description "Un cuerpo sin alma no sirve...". Arregla el debuff visible.
- Verificacion:
  - Suite: 51/51 (sin cambios; OJO: los tests de localizacion aun validan el
    archivo MUERTO, pendiente re-apuntarlos a en-US.hjson).
  - Falta reload in-game para confirmar el texto del debuff.
- Pendientes/riesgos:
  - Re-consolidar de verdad sobre `en-US.hjson`: portar los tooltips reales
    (curses, starter weapons) que hoy solo viven en el archivo muerto, re-apuntar
    los ~6 tests de localizacion a `en-US.hjson`, y retirar el `Localization/`
    muerto. La entrada previa "Consolidacion de localizacion" quedo invertida.

## 2026-07-06 - Penalizacion por no tener Soul equipada + limpieza de debuffs

- Objetivo: implementar la regla "un cuerpo siempre ocupa una soul" y consolidar
  los debuffs de Soul.
- Archivos principales:
  - `Content/Players/EterniaPlayer.cs`
  - `Content/Buffs/NoSoulDebuff.cs` (ELIMINADO)
  - `Localization/en-US_Mods.ETERNIA.hjson`
  - `tests/NoSoulPenaltySourceSmokeTest.ps1` (nuevo)
  - `tests/SoulInventorySourceSmokeTest.ps1`, `tests/ResourceTexturePathsSourceSmokeTest.ps1`
- Contexto: el usuario probo con Empty Soul en inventario y no habia castigo. Se
  decidio penalizar SIEMPRE que no haya Soul equipada (invierte el invariante
  "fresh player sin penalizacion"). Ver `decision-log.md`.
- Cambios:
  - `PostUpdateEquips` ahora aplica `ApplyNoSoulPenalty` cuando `!HasAnySoul`
    (ninguna Soul equipada), incluido el jugador recien creado. Se quito el gate
    `HasClassSoulAvailable` (ya no escanea inventario -> cierra la fuga de banco).
  - `ApplyNoSoulPenalty` ahora aplica `SoulLessDebuff` ("Alma Perdida"), no
    `NoSoulDebuff`. Se elimino `NoSoulDebuff` (huerfano/redundante).
  - TDD: `NoSoulPenaltySourceSmokeTest.ps1` escrito fallando primero.
  - Docs actualizados: `ai-handoff.md`, `current-state.md`, `gameplay-systems.md`.
- Verificacion:
  - Suite completa: 51/51 PASSED.
  - `dotnet build -t:Compile`: 0 warnings, 0 errores.
  - Falta reload in-game para confirmar visualmente el debuff "Alma Perdida" y que
    equipar la Empty Soul quita la penalizacion.
- Pendientes/riesgos:
  - Penaliza desde el spawn: confirmar en juego que se siente bien (es lo pedido).

## 2026-07-06 - Consolidacion de localizacion + suite portable

- Objetivo: dejar UNA sola fuente de verdad de localizacion que resuelva in-game,
  arreglar los textos rotos de Souls, y hacer la suite de tests portable.
- Archivos principales:
  - `Localization/en-US_Mods.ETERNIA.hjson` (canonico)
  - `en-US.hjson` (vaciado; OneDrive impide su borrado real)
  - `tests/LocalizationIntegritySourceSmokeTest.ps1` (nuevo)
  - `tests/SoulRulesBehaviorSmokeTest.ps1`
  - `tests/{LocalizationContent,NPCLocalization,KeybindDefaults,SummonerWhip,RemovedCartomancerResidue}SourceSmokeTest.ps1`
- Contexto: el `en-US.hjson` raiz estaba 100% muerto (envuelto en `Mods.Eternia.*`;
  el nombre interno del mod es `ETERNIA`, asi que sus ~150 claves nunca resolvian).
  El archivo `Localization/` es el que si resuelve (auto-prefijo `Mods.ETERNIA.*`),
  pero tenia un bloque `Mods: {...}` anidado muerto y bugs en las claves vivas.
- Cambios:
  - Bug in-game corregido: `Buffs.SoulLessDebuff.Description` era auto-referencial
    (mostraba la clave cruda). Ahora DisplayName `Alma Perdida` / Description real.
  - `Items.EmptySoul.Tooltip` estaba vacio; se restauro el texto autor `Esta SOUL
    esta vacia...`. Ambos textos estaban atrapados en el bloque muerto anidado.
  - Eliminado el bloque `Mods: {...}` anidado muerto del archivo canonico.
  - `en-US.hjson` era un duplicado 100% muerto. Migrados los 5 tests que lo leian
    para apuntar solo al archivo canonico. NOTA: OneDrive restaura los borrados de
    archivos, asi que en vez de borrarlo se vacio (objeto hjson vacio + comentario).
    Para eliminarlo de verdad: mover el repo fuera de OneDrive y `git rm en-US.hjson`.
  - `SoulRulesBehaviorSmokeTest.ps1` ahora resuelve el install de tModLoader por
    `TML_INSTALL_DIR` -> el import de `..\tModLoader.targets` -> ubicaciones Steam
    comunes, y hace SKIP elegante si no encuentra tModLoader/dotnet. Ya no clava
    `C:\Program Files (x86)\Steam\...`.
  - TDD: `LocalizationIntegritySourceSmokeTest.ps1` codifica los invariantes (sin
    self-ref, sin bloque `Mods:` muerto, sin placeholders, tooltip presente). Se
    escribio primero fallando, luego se implemento el fix.
- Verificacion:
  - Suite completa: 50/50 PASSED (49 previos + el nuevo; ya sin el fallo de path).
  - `git diff --check`: exit 0.
  - `dotnet build`: compila a DLL con 0 warnings; el empaquetado `.tmod` sigue
    fallando con TML003 solo porque tModLoader esta abierto (lock), no por codigo.
    Falta un reload real in-game para confirmar visualmente los textos.
- Pendientes/riesgos:
  - Verificar in-game (reload) que las descripciones de debuff y el tooltip de
    EmptySoul se muestran bien.
  - Deuda menor abierta (del assessment): fuga de penalizacion de Soul si la Class
    Soul se guarda en Void Vault/Piggy Bank (SoulInventory solo escanea
    inventario+armadura); 8 overlays de UI sin clamp de pantalla; null-guard en
    `PromotionRewardPlayer.LoadData`.

## 2026-07-03 - Handoff docs para Claude Code

- Objetivo: crear documentacion para que Claude Code pueda continuar el mod con
  contexto tecnico y reglas operativas.
- Archivos principales:
  - `docs/README.md`
  - `docs/ai-handoff.md`
  - `docs/current-state.md`
  - `docs/gameplay-systems.md`
  - `docs/technical-architecture.md`
  - `docs/ui-rework.md`
  - `docs/testing-verification.md`
  - `docs/roadmap-known-issues.md`
  - `docs/claude-code-workflow.md`
  - `docs/change-log.md`
  - `docs/decision-log.md`
- Cambios:
  - Documentado estado actual del mod.
  - Documentado flujo de Souls, penalizaciones, UI, tests y roadmap.
  - Agregado protocolo para registrar cambios, decisiones y reorganizaciones.
- Verificacion:
  - ASCII check sobre `docs/*.md`.
  - `git diff --check -- .\docs`.
- Pendientes/riesgos:
  - Falta prueba manual in-game de reload/UI.
  - Falta resolver si Summoner queda solo con Necromancer o conserva varias
    promociones temporalmente.

## 2026-07-03 - Rework de UI y hardening de Soul gating

- Objetivo: mejorar UI y reforzar que subclases/proyectiles no funcionen sin
  Soul activa correcta.
- Archivos principales:
  - `Content/UI/EterniaUI.cs`
  - `Content/UI/PassiveUI.cs`
  - `Content/UI/StatsUI.cs`
  - `Content/UI/SoulUI.cs`
  - `Content/UI/SoulUISystem.cs`
  - `Content/NPCs/BleedGlobalNPC.cs`
  - `Content/Projectiles/Necromancer/BaseNecroMinion.cs`
  - `Content/Projectiles/Summoner/BaseEterniaWhipProjectile.cs`
  - `tests/*.ps1`
- Cambios:
  - Rework visual y layout de paneles principales.
  - Paneles `Soul`, `Stats` y `Passive` ahora son mutuamente exclusivos.
  - Bleed de Swordsman valida `IsActiveSwordsman`.
  - Minions de Necromancer validan `IsActiveNecromancer`.
  - Whip projectiles validan owner, Summoner Soul y required subclass.
- Verificacion:
  - `dotnet build .\ETERNIA.csproj`: 0 warnings, 0 errores.
  - Suite completa: `ALL 49 SMOKE TESTS PASSED`.
  - `git diff --check`: exit code 0, solo warnings LF/CRLF.
- Pendientes/riesgos:
  - Overlays anclados al jugador todavia necesitan helper de clamp/flip.
  - Decision Summoner/Necromancer pendiente.

