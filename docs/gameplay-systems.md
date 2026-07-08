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

Nota: las armas `TrainingBlade`/`ApprenticeWand`/`TrainingBow`/`TrainingWhip`
existen aun como items pero quedan sin fuente (ni regaladas ni crafteables);
decidir si craftearlas o quitarlas. Riesgo: un Mage/Ranger/Summoner nuevo no tiene
arma de su clase y su arma vanilla inicial (Copper Shortsword = melee) lo mataria
por clase incorrecta.

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

## Swordsman bleed

Archivos principales:

- `Content/Players/SwordsmanPlayer.cs`
- `Content/NPCs/BleedGlobalNPC.cs`

`SwordsmanPlayer.IsActiveSwordsman` exige Warrior Soul activa y subclase
Swordsman. `BleedGlobalNPC` usa ese helper para que el dano de bleed no siga
activo si el jugador cambia de Soul o subclase.

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

