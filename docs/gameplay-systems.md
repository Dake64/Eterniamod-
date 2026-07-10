# Sistemas de gameplay

## Souls

Archivos principales:

- `Content/Souls/SoulId.cs`
- `Content/Souls/SoulRules.cs`
- `Content/Souls/SoulInventory.cs`
- `Content/Items/Souls/EmptySoul.cs`
- `Content/Items/Souls/ClassSoulItem.cs`
- `Content/Items/Souls/WarriorSoul.cs`
- `Content/Items/Souls/MageSoul.cs`
- `Content/Items/Souls/RangerSoul.cs`
- `Content/Items/Souls/SummonerSoul.cs`
- `Content/Players/EterniaPlayer.cs`

`EmptySoul` es un item accesorio gris. Puede equiparse, pero activa
`SoulId.Empty`, que no cuenta como Class Soul. Su proposito es ser el item que el
NPC entrega para que el jugador lo craftee en Warrior/Mage/Ranger/Summoner Soul.

`ClassSoulItem`:

- Es accesorio.
- En `UpdateAccessory` llama a `EterniaPlayer.ActivateSoul(Soul)`.
- Tiene receta con `EmptySoul`, consumiendo la Empty Soul.

`SoulRules` define:

- Que Souls son clase (`None` y `Empty` no lo son).
- Que items son combate.
- Que DamageClass permite cada Soul:
  - Warrior -> Melee / MeleeNoSpeed.
  - Mage -> Magic.
  - Ranger -> Ranged.
  - Summoner -> Summon / SummonMeleeSpeed.

## Penalizaciones

Archivo principal: `Content/Players/EterniaPlayer.cs`.

Reglas actuales:

- En `PreUpdate`, `ActiveSoul` se resetea a `None`. Las Souls equipadas vuelven
  a activarla por `UpdateAccessory`.
- Si el jugador NO tiene ninguna Soul equipada (`ActiveSoul == None`),
  `ApplyNoSoulPenalty` reduce dano, critico, velocidad, defensa y aplica
  `SoulLessDebuff` ("Alma Perdida"). Un cuerpo siempre ocupa una Soul.
- Equipar cualquier Soul (Empty o Class) como accesorio quita la penalizacion.
  La Empty Soul no activa clase, pero si cuenta como "cuerpo con alma".
- Si el jugador usa o intenta usar un arma de otra clase con Class Soul activa,
  `ApplyDeathPenalty` mata al jugador y aplica `SoulViolationDebuff`.

Cambio de diseno (2026-07-06): antes solo se penalizaba al poseer una Class Soul
sin equipar; ahora se penaliza a cualquier jugador sin Soul equipada, incluido el
recien creado. El debuff `NoSoulDebuff` (redundante) fue eliminado. Ver
`decision-log.md`.

## Arma inicial (ELIMINADA 2026-07-06)

Activar una Class Soul YA NO regala un arma inicial. Se elimino todo el sistema
(`GiveStarterWeaponIfNeeded`, los flags `*StarterGiven` y su save/load). La Soul
solo define la clase; el jugador consigue su propia arma. Ver `decision-log.md`.

Las armas de clase `TrainingBlade`/`ApprenticeWand`/`TrainingBow`/`TrainingWhip`
son CRAFTEABLES (10 madera en la mesa de trabajo, disponibles desde el minuto 1),
asi que cualquier clase puede armarse desde el inicio y evitar el death-trap de la
Copper Shortsword vanilla (que mataria a un Mage/Ranger/Summoner por clase
incorrecta).

## Subclases y promociones

Archivos principales:

- `Content/Progression/ClassPromotionRules.cs`
- `Content/Progression/ClassAffinitySnapshot.cs`
- `Content/Players/SubclassPlayer.cs`
- `Content/Players/PromotionRewardPlayer.cs`

`SubclassPlayer.PostUpdateEquips` recalcula `CurrentSubclass` desde:

- `EterniaPlayer.ActiveSoul`
- `Main.hardMode`
- afinidades del jugador
- promocion bloqueada por Soul, si existe

Antes de hardmode, o sin Soul de clase, la resolucion devuelve clase base o
`None`.

Promociones validas actuales por codigo:

- Warrior:
  - Swordsman
  - Fighter
  - Guardian
  - Yoyo Master
  - Berserker
  - Stunner
- Ranger:
  - Energy Gunner
  - Archer
  - Gunner
  - Virtuoso
- Mage:
  - Elementalist
  - Cursed Mage
  - Infinity Mage
  - Arcane Bard
- Summoner:
  - Beast Tamer
  - Advanced Summoner
  - Tech Summoner
  - Necromancer

Importante: el usuario pidio descartar Cartomancer y crear una promocion nueva
para Summoner. `Necromancer` cumple esa funcion, pero el codigo todavia conserva
otras promociones Summoner. Esto es deuda de diseno y debe resolverse.

## Rewards de promocion

Archivo principal: `Content/Players/PromotionRewardPlayer.cs`.

Reglas:

- Solo corre para `Main.myPlayer`.
- Requiere `soul.HasClassSoul`.
- Ignora clase base.
- Verifica `ClassPromotionRules.IsPromotionForSoul(soul.ActiveSoul, subclass)`.
- Guarda promociones ya recompensadas en `AwardedPromotions`.

Rewards destacadas:

- Necromancer -> `BeginnerNecromancyBook`.
- Beast Tamer -> `BeastWhip`.
- Advanced Summoner -> `FusionWhip`.
- Tech Summoner -> `TechWhip`.

Si se decide que Summoner debe ser solo Necromancer, hay que podar las otras
recompensas o migrarlas.

## Necromancer

Archivos principales:

- `Content/Players/NecromancerPlayer.cs`
- `Content/Projectiles/Necromancer/BaseNecroMinion.cs`
- `Content/Projectiles/Necromancer/SkeletonMinion.cs`
- `Content/Items/Weapons/Summoner/BeginnerNecromancyBook.cs`

`NecromancerPlayer.IsActiveNecromancer` exige:

- Class Soul activa.
- `ActiveSoul == SoulId.Summoner`.
- `CurrentSubclass == "Necromancer"`.

`BaseNecroMinion` ya usa ese helper. Si el owner deja de ser Necromancer con
Summoner Soul activa, el minion se mata.

## Warrior bleed (mecanica de clase)

Archivos principales:

- `Content/Buffs/BleedDebuff.cs` (debuff propio del mod, visible en el enemigo)
- `Content/Globals/EterniaGlobalItem.cs` (`CanInflictBleed`, tabla de espadas + tooltip)
- `Content/Items/IBleedWeapon.cs` (override de chance para espadas insignia)
- `Content/Players/WarriorBleedPlayer.cs` (aplicacion class-wide + dano vs sangrante)
- `Content/NPCs/BleedGlobalNPC.cs` (DoT de 1 nivel, dano fijo + afinidad)
- `Content/Players/SwordsmanPlayer.cs` (maestria del Espadachin)

El Sangrado es una mecanica del **Guerrero** que **solo aplican espadas concretas**,
no cualquier espada. `EterniaGlobalItem` tiene una **tabla curada** `VanillaBleedSwords`
(ItemID -> chance base) con espadas tematicas (sangrientas/serradas/filosas: p.ej.
BloodButcherer 30, Bladetongue 30, DeathSickle 35, NightsEdge 22, Katana 18, ...) mas
las espadas insignia del mod via `IBleedWeapon` (TrainingBlade 25, BloodletterBlade 45).
`CanInflictBleed` decide la elegibilidad; el resto de espadas **no** sangran.

Cualquier Warrior Soul activa que golpee con una espada elegible tiene una
probabilidad de aplicar `BleedDebuff` (rodada en `WarriorBleedPlayer`). El chance es
**visible**: `ModifyTooltips` muestra el porcentaje efectivo (base + afinidad Bleed)
para un Guerrero activo. El DoT es de **un solo nivel** con **dano fijo**
(`BaseBleedDamage` + afinidad) y solo tickea mientras el aplicador siga siendo Warrior
activo. La pasiva `Execution` da dano extra vs sangrantes (Warrior-wide).

El **Espadachin** es el maestro del sangrado: `SwordsmanPlayer.OnHitNPCWithItem`
(gateado por `IsActiveSwordsman`, tambien solo espadas elegibles) **garantiza** el
sangrado en cada golpe, saltandose la probabilidad. El viejo sistema de 5 stacks +
"EXECUTE!" se retiro y renacio como la tecnica del Rastro Carmesi (abajo).

Arsenal pre-hardmode del Guerrero (espadas insignia `IBleedWeapon`, craftables, no
subclass-locked, en `Content/Items/Weapons/Warrior/`): TrainingBlade (25),
SerratedIronBlade (rapida, 32), HuntersWarblade (equilibrada, 28), Thornrender
(jungla, especialista 36), MoltenGutripper (infierno, pesada, 35). Cubren el arco
pre-hardmode con nichos distintos; la promocion a Espadachin entrega BloodletterBlade
(45, subclass-locked).

## Rastro Carmesi (recurso del Espadachin)

Archivos principales:

- `Content/Players/CrimsonTrailPlayer.cs` (recurso + save/load)
- `Content/Players/SwordsmanSkillPlayer.cs` (tecnica con la tecla de habilidad)
- `Content/UI/CrimsonTrailUI.cs` (barra propia)

El Rastro Carmesi es **exclusivo del Espadachin** y su logica esta **separada** del
sangrado (para que otras subclases del Guerrero reutilicen el sangrado sin
acoplarse). `CrimsonTrailPlayer` solo acumula recurso mientras `IsActiveSwordsman`
(si no, `ResetEffects` lo pone en 0) y **no tiene regeneracion automatica**: solo se
gana en `SwordsmanPlayer.OnHitNPCWithItem` al golpear con arma de filo (mas al sacar
primera sangre que al mantener el sangrado). `Add`/`TrySpend` respetan el cap
`MaxCrimsonTrail` y persiste via `SaveData`/`LoadData`.

La unica forma de gastarlo es la tecnica **Crimson Execution** (`SwordsmanSkillPlayer`,
tecla `SkillKey`/Q + cooldown compartido `SkillPlayer`): consume `TechniqueCost` y
golpea a todos los enemigos sangrantes cercanos con un burst (el "EXECUTE!" renacido,
escalado por afinidad Bleed). La barra `CrimsonTrailUI` solo se dibuja para un
Espadachin activo y se enciende (`Q: EXECUTE`) cuando hay recurso suficiente.

## Mecanicas signature de subclase

Cada subclase tiene su propio `ModPlayer` con una mecanica de identidad (recurso
construido en combate + payoff), no solo `+stat`. Patron comun: `IsActiveX()`
para gating, `ResetEffects` limpia el recurso cuando la subclase no esta activa,
la tecnica usa `SkillKey` (`EterniaKeybinds`) + `SkillPlayer` (cooldown global),
feedback via CombatText/Dust/`SoundEngine`. Los `+stat` base viven en
`SubclassEffectsPlayer` y sirven de baseline; la mecanica STACKEA encima.

Estado por clase:

- **Warrior**: Swordsman (bleed + Rastro Carmesi), Fighter (`FighterPlayer` Combo),
  Guardian (`GuardianPlayer` aura de reflejo), Yoyo Master (`YoyoMasterPlayer`
  Precision -> True Strike), Berserker (`BerserkerPlayer`/`BerserkerSkillPlayer`
  Rage), Stunner (`StunnerPlayer` Charge -> stun).
- **Ranger**: Archer (`ArcherPlayer` Focus -> Perfect Shot), Gunner
  (`GunnerPlayer` Sweet Spot -> Dead Eye), Energy Gunner (`EnergyShooterPlayer`
  Heat/Overdrive), Virtuoso (`VirtuosoPlayer` Notas/Melodias).
- **Mage**: Elementalist (`ElementalistPlayer` elementos + ultimates), Cursed Mage
  (`CursedMagePlayer` energia), **Infinity Mage** (`InfinityMagePlayer`), **Arcane
  Bard** (`ArcaneBardPlayer`).
- **Summoner**: Necromancer (`NecromancerPlayer` slots + drain), **Beast Tamer**
  (`BeastTamerPlayer`), **Advanced Summoner** (`AdvancedSummonerPlayer`), **Tech
  Summoner** (`TechSummonerPlayer`).

Las 5 en negrita se agregaron 2026-07-09 (antes eran solo `+stat`):

- **Infinity Mage — Overflow**: castear rebosa el pozo (0-100); a tope, +15% magia
  pasivo; `Q` a full = Arcane Overload (~5s `manaCost=0` + +25% magia).
- **Arcane Bard — Crescendo**: golpes magicos suben momento continuo (magia/cast/
  move) que decae al parar; a tope, pulso de cura. Sin gasto ni boton.
- **Beast Tamer — Ferocity**: golpes de minions suben furia (+15% invoc); `Q` a
  full = Primal Roar (~6s +30% invoc + knockback).
- **Advanced Summoner — Legion/Overclock**: +daño segun `slotsMinions/maxMinions`;
  carga Command; `Q` a full = Overclock (~5s +2 al tope de minions + vel invoc).
- **Tech Summoner — Power Core**: bateria auto-recarga (mas con drones) + defensa
  pasiva; `Q` a full = Overdrive Protocol (~5s +25% invoc + 15 defensa).

`tests/SubclassMechanicsSourceSmokeTest.ps1` fija estas 5 al source.

IMPORTANTE - las CLASES BASE NO tienen recurso. El viejo recurso de clase base
(Momentum / Charge / Focus / Bond) fue ELIMINADO: un recurso es estrictamente una
mecanica de SUBCLASE. Sin promover, la clase solo da los `+stat` fijos de
`BaseClassPlayer.PostUpdateEquips` (no hay medidor ni barra). El panel de Soul
muestra "None - promote to gain one" en la fila Resource.

Barras de recurso: cada subclase muestra su recurso en el HUD. Swordsman en
`CrimsonTrailUI`; las 6 nuevas (Infinity Mage, Arcane Bard, Beast Tamer, Advanced
Summoner, Tech Summoner, Yoyo Master) consolidadas en `SubclassResourceUI`; ambas
delegan el dibujo a `EterniaUI.DrawFloatingResourceBar` (barra flotante compartida:
fade in/out con el valor, relleno pulido, glow al estar casi llena, pill "Q: ..."
cuando la tecnica esta lista, clamp de posicion incluido). El resto conserva su UI
propia con estilo bespoke (`FighterComboUI`, `ArcherFocusUI`, `EnergyHeatUI`,
`GunnerUI`, `VirtuosoUI`, `StunnerChargeUI`, `BerserkerUI`, `CursedMageUI`,
`ElementalistUI`, `NecromancerUI`).

## Whips Summoner

Archivos principales:

- `Content/Projectiles/Summoner/BaseEterniaWhipProjectile.cs`
- `Content/Projectiles/Summoner/TrainingWhipProjectile.cs`
- `Content/Projectiles/Summoner/BeastWhipProjectile.cs`
- `Content/Projectiles/Summoner/FusionWhipProjectile.cs`
- `Content/Projectiles/Summoner/TechWhipProjectile.cs`

`BaseEterniaWhipProjectile` valida al owner en `CanHitNPC`, `OnHitNPC` y
`ModifyHitNPC`.

El owner debe:

- existir y estar activo;
- tener Class Soul;
- tener `ActiveSoul == SoulId.Summoner`;
- cumplir `RequiredSubclass` si el proyectil promocional lo declara.

Los whips promocionales declaran `RequiredSubclass`; `TrainingWhipProjectile`
no declara subclase requerida.

## Pasivas y stats

Archivos principales:

- `Content/Players/EterniaStatsPlayer.cs`
- `Content/Players/EterniaLevelPlayer.cs`
- `Content/Passives/PassiveRegistry.cs`
- `Content/Progression/ProgressionService.cs`

Las pasivas se compran desde `PassiveUI` via `ProgressionService`. Los stats
tambien se gastan desde UI via `ProgressionService.TrySpendStatPoint`.

Hay pruebas source-level que verifican elegibilidad, coverage y efectos runtime,
pero no reemplazan una prueba in-game.

