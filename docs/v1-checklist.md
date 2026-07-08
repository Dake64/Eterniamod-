# Camino a v1 (pulido)

Meta acordada: NO publicar hasta tener v1 pulido (arte propio, balance afinado,
contenido mas completo, hardening). Este doc traduce esa meta en tareas concretas.

## 1. Arte (lo hace el owner)

Estado real: 22 .png en `Content`, pero ~20 items reutilizan el sprite de otro.
Muchos items se ven IDENTICOS en juego. Sprites propios pendientes:

### Sprites compartidos (cada grupo se ve igual hoy)

- **Ícono del Ranger Soul** lo usan TODOS los items de Ranger:
  `TrainingBow`, `EnergySidearm`, `Longbow`, `TrainingPistol`, `ResonantCrossbow`.
- **Ícono del Summoner Soul** lo usan TODOS los látigos:
  `TrainingWhip`, `BeastWhip`, `FusionWhip`, `TechWhip` (y sus proyectiles via
  `BaseEterniaWhipProjectile`).
- **TrainingGauntlet** lo usan varios de Warrior:
  `TrainingBlade`, `BloodletterBlade`, `RageCleaver`, `PracticeYoyo`
  (y `PracticeYoyoProjectile`).
- **ElementalApprenticeStaff** lo usan: `ApprenticeWand`, `ArcaneFocus`.
- **CursedApprenticeTome** lo usa: `InfinityTome`.
- **TrainingShield** lo usa: `ImpactMace`.

### Otros placeholders

- `EternalNPC_Head.png` es una COPIA del cuerpo (placeholder) -> head propio.
- `SoulViolationDebuff` comparte el ícono de `SoulLessDebuff` (decidir si propio).
- Revisar si los sprites base (Souls, gauntlet, tome, staff, shield, proyectiles)
  son de calidad final o tambien placeholder.

Cuando exista el sprite propio de un item, quitar su `Texture =>` / `TexturePath`
override y poner el .png junto a su .cs (tModLoader lo auto-carga por co-ubicacion).

## 2. Tecnico (lo puede cerrar la IA)

- [ ] **Hardening / caza de bugs** en sistemas sin cobertura de test:
  `EterniaLevelPlayer` (XP/level), `WeakpointGlobalNPC`, `StunnedNPC`,
  `EterniaGlobalItem`, `EterniaAmmoGlobalItem`, `SkillPlayer` base.
- [ ] **Lint de assets** exhaustivo: garantizar que TODO item/proyectil/NPC tiene
  textura resoluble (evita `MissingResourceException` al recargar/publicar).
- [~] **Multijugador (base MP-safe hecha; falta prueba real)**. Decidido: MP real.
  Hecho:
  - Rareza de enemigos: se rolea SOLO en el servidor y se sincroniza via
    `SendExtraAI`/`ReceiveExtraAI` (`EterniaGlobalNPC`). Ya no desync.
  - Penalizacion de Soul: `EterniaPlayer.PostUpdateEquips` solo actua sobre el
    jugador local (`whoAmI == Main.myPlayer`).
  - XP: `OnKill` (server) envia `ModPacket` `AddExperience` al cliente que mato;
    `ETERNIA.HandlePacket` lo aplica en el cliente. Ya no hay broadcast global.
  Pendiente:
  - PRUEBA REAL con servidor + 2 clientes (los tests source solo aseguran que los
    mecanismos existen).
  - `EterniaAmmoGlobalItem` usa `Main.rand` por-cliente (ahorro de ammo) -> desync
    menor. Verificar proyectiles/minions en MP. Bonos de stat en jugadores remotos
    son cosmeticos e inofensivos.
- [ ] **Migracion de save** (P1 histórico): `SubclassPlayer.LoadData` /
  `PromotionRewardPlayer.LoadData` cargan strings crudos. Degrada con gracia hoy,
  pero conviene versionar el save antes de v1 por si cambia contenido.
- [ ] **Retirar de verdad** los archivos muertos (dead `Localization/…ETERNIA.hjson`)
  moviendo el repo fuera de OneDrive.

## 3. Contenido (diseño del owner + IA)

- Profundidad delgada: `Necromancer` solo tiene 1 minion (`SkeletonMinion`);
  Beast/Advanced/Tech Summoner no tienen player de habilidad activa (solo pasiva).
- Decidir si cada subclase necesita mas identidad (skill activa, arma unica, etc.).

## 4. Balance (solo jugando)

XP, stats, afinidades, pasivas, penalties, rewards, costos. No se cierra desde
codigo; se afina jugando. Idealmente con la beta ya jugable.

## 5. Metadata de release

- `build.txt`: subir version cuando toque; considerar `homepage`, `buildIgnore`.
- `description_workshop.txt`: DESACTUALIZADA. Dice "owning a class Soul without
  keeping one active weakens the player" (regla vieja). La regla nueva es:
  no tener NINGUNA Soul equipada debilita (un cuerpo siempre ocupa una Soul).
- Confirmar `icon.png` / `icon_small.png` finales.
