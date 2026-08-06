# Guía del Espadachín (Swordsman)

Guía completa para jugar al Espadachín de cero, como si vieras un vídeo de cómo se juega.
Todos los números salen del código a fecha de esta guía; si el balance se ajusta, revísalos.

---

## 1. Qué eres

El Espadachín es el **maestro del sangrado**. Tu identidad no es un golpe fuerte — es:
**hacer sangrar → acumular esa sangre en una barra (el Rastro Carmesí) → gastarla en una
Ejecución** que revienta a todo lo que sangra a tu alrededor.

## 2. Cómo te conviertes en Espadachín

**Antes de hardmode NO eres Espadachín** — eres un Guerrero base. Te conviertes al **matar el
Muro de Carne**, y la subclase la decide tu **afinidad más alta**.

- Para salir Espadachín, tu **afinidad de Sangrado** debe ser la mayor.
- La subes gastando puntos de pasiva en la **rama de Sangrado**.
- Antes del Muro, habla con el **Eternal → "Read my soul"**: te dice hacia qué subclase vas.

> **Durante todo pre-hardmode: sube la rama de Sangrado.** Es lo que garantiza salir Espadachín.

## 3. ⚙️ Configuración obligatoria (no te la saltes)

tModLoader **no asigna la tecla de skill automáticamente**. Ve a
`Esc → Controles → "Class Skill"`. Si está en blanco, asígnale una tecla. Sin esto, tu
habilidad no hace nada y parece rota. *(Es el error #1 de todo el mundo.)*

La tecla **M** abre tu menú (Soul / Stats / Passives / Codex en pestañas).

## 4. La barra — cómo se carga el Rastro Carmesí

Barra de **0 a 100**. Regla de oro:

> **Solo cobras de sangre que YA está corriendo.** El golpe que *abre* la herida no da nada.

| Fuente | Cuánto |
|---|---|
| Golpear a un enemigo **que ya sangra** | **+3** (máx. 2 cobros cada medio segundo) |
| Cada segundo, por enemigo sangrando por tu herida | **+1** (tope 3 enemigos) |

Más **+1 por Milestone**, y luego multiplicadores (accesorios, armadura, nivel de mecánica).

Enemigos que matas de un golpe **no dan nada** (nunca llegaron a sangrar). Es un recurso que
**construyes durante la pelea**, no algo que esté siempre lleno.

## 5. La Q — Ejecución Carmesí

- **Cuesta 50.** La barra tiene una **línea grabada a la mitad**: cuando la sangre la pasa y
  pulsa, ya puedes.
- **Cooldown: 1,5 segundos.**
- Golpea a **TODOS los enemigos sangrando** cerca de ti, a la vez.
- **Daño por enemigo = 50 + (tu Afinidad de Sangrado × 5).** Con 80 de afinidad son **450 por
  enemigo**.

Siempre te avisa qué pasa:
- `CRIMSON 34/50` → te falta cargar
- `NOTHING BLEEDING` → no hay nada sangrando cerca (no gasta nada)
- `¡CRIMSON EXECUTION!` → ¡dispara!

## 6. Los 3 niveles de tu mecánica (crecen con el mundo)

| Nivel | Se desbloquea | Qué cambia |
|---|---|---|
| **Finisher** | Muro de Carne | Radio 20 bloques. Remata solo lo que TÚ hiciste sangrar |
| **Hemorragia** | Plantera | Radio 28. La ejecución **hace sangrar la zona ella sola** |
| **Aniquilación** | Moon Lord | Radio 40. Lo que quede bajo 25% de vida **muere al instante** (jefes exentos) |

## 7. El árbol de pasivas (rama de Sangrado)

Súbela en orden. Los más rentables:
- **Execution** (+15% vs sangrando) y **Exsanguinate** (+25% vs sangrando) — como todo lo que
  golpeas ya sangra, están activos siempre.
- **Blood Tithe** (+2 Rastro por golpe), **Open Veins** (ingreso pasivo cuenta 2 enemigos más),
  **Merciless** (la ejecución cuesta 10 menos) — alimentan tu recurso.
- **Rupture** y **Hemoplague** — sangrado más fuerte y más largo.

**Keystone (Hemorrhagic Frenzy):** +20% daño melee, pero la ejecución cuesta **+25**. Cógelo
solo si quieres pegar fuerte y ejecutar poco. Para "ejecutar mucho", **no lo cojas**.

## 8. Stats — qué subir

**Afinidad de Sangrado, sin dudar.** Es doblemente valiosa: define que seas Espadachín **Y**
escala el daño de tu sangrado y de tu ejecución (`50 + afinidad×5`). Cada punto ahí vale más
que en cualquier otra cosa.

## 9. Armas por etapa

Cualquier **arma de filo de sangrado**. Como Espadachín tus golpes **siempre** aplican
sangrado (te saltas la tirada que hacen los otros Guerreros), así que **elige por DAÑO puro**.

| Etapa | Arma | Daño |
|---|---|---|
| Pre-HM | Bloodletter Blade | 42 |
| Post-Muro | Sanguine Cleaver (14 barras Adamantita/Titanio) | 56 |
| Jefes mecánicos | Hallowed Bloodletter | 62 |
| Post-Plantera | Crimson Requiem | 92 |
| Endgame | Exsanguinator (barras lunares) | 112 |

**Bonus:** tus espadas lanzan un **tajo de sangrado a distancia** (~51 bloques de alcance) que
también sangra y carga la barra. Úsalo contra jefes voladores.

## 10. Armadura y accesorios

- **Pre-HM:** no hay set de Guerrero orientado a sangrado (Steelbound potencia Combo, otra
  rama). Usa mineral genérico (Revenite) o armadura vanilla mientras subes afinidad.
- **Hardmode:** **Hemocarnage** — *"Crimson Trail builds 60% faster"*. Es LA armadura del
  Espadachín.
- **Accesorios:** **Hemophage Sigil** (×1.60 al Rastro) es el mejor; combínalo con el
  **Crimson Chalice** (×1.35) → juntos ×2.16.

## 11. 🔁 El bucle de combate

**Contra grupos:** pega para hacer sangrar a varios → la barra sube → al pasar la línea de la
mitad, **Q** → revientas a todos los que sangran.

**Contra un jefe:** pégale (ya sangra desde el primer golpe, cobras +1/s) → usa el tajo a
distancia cuando se aleje → guarda la Q para cuando tengas 50 y él siga sangrando cerca.

**Desde Plantera** todo cambia: una sola Q hace sangrar todo en 28 bloques, así que en jefes
multi-parte (Moon Lord) una pulsación los pone a sangrar todos a la vez.

## 12. Errores comunes

- ❌ **La Q no hace nada** → la tecla está sin asignar (§3).
- ❌ **La barra no sube** → golpeas enemigos que aún no sangran, o los matas de un golpe.
- ❌ **Sale `NOTHING BLEEDING`** → el sangrado se acabó; pega y ejecuta más seguido.
- ❌ **Subes stats que no sean Afinidad de Sangrado** → dejas daño en la mesa.

---

## 13. Progresión por jefe — PRE-HARDMODE

> ⚠️ **Antes del Muro NO eres Espadachín todavía** — eres un Guerrero base. No tienes Rastro
> Carmesí ni la Q; tu sangrado es solo un daño-por-tiempo. Por eso los accesorios de Rastro
> (Bloodletter Band, etc.) **no hacen nada aquí** — usa accesorios melee normales de Terraria.
> Tu único objetivo pre-HM: **subir afinidad de Sangrado** en el árbol para salir Espadachín.

| Jefe | Arma de sangrado | Armadura | Accesorios (vanilla) |
|---|---|---|---|
| **King Slime** | Serrated Iron Blade (13) | Soulstone (mineral del mod) o vanilla | Hermes Boots, Cloud in a Bottle, Shackle |
| **Eye of Cthulhu** | Serrated Iron Blade → dropea **Dread Reaver (22)** al matarlo | Soulstone / Animite | + Band of Regeneration |
| **Eater / Brain** | Corruptor's Ripper (22, barras del mal) | Animite | + Feral Claws (velocidad melee) |
| **Queen Bee** | **Thornrender (24)** (lo dropea, o crafteas con cera+aguijón) | Animite / Revenite | + Shark Tooth Necklace (penetración) |
| **Skeletron** | Thornrender / **Bonewarden Sabre (24)** (drop raro de esqueletos de mazmorra) | Revenite | + Obsidian Shield |
| **Deerclops** | Bonewarden Sabre (24) | Revenite | + Fledgling Wings si las tienes |
| **Prototype-01** (jefe del mod) | Revenite Cleaver (25) o **Molten Gutripper (27)** | **Revenite** (tope pre-HM del mod) | Feral Claws, Cross Necklace, botas + globo |
| **Wall of Flesh** | Molten Gutripper (27) — lo mejor pre-HM | Revenite o Molten vanilla | Wings, Feral Claws, Lifeforce/Rage potions |

> 🩸 **Al matar el Muro te conviertes en Espadachín** y recibes de recompensa la
> **Bloodletter Blade (42)** — tu arma insignia. Aquí empieza el Rastro Carmesí y la Q.

---

## 14. Progresión por jefe — HARDMODE

> Ahora SÍ eres Espadachín: tienes barra, Q, y los accesorios de Rastro por fin funcionan.
> Recuerda los desbloqueos de tu mecánica: **Hemorragia al matar Plantera**, **Aniquilación al
> matar Moon Lord**.

| Jefe | Arma de sangrado | Armadura | Accesorios clave |
|---|---|---|---|
| **Queen Slime** | Bloodletter Blade (42) → **Quicksilver Fang (44)** (Mithril/Oricalco) | Transición a **Hemocarnage** en cuanto tengas Adamantita/Titanio | **Hemophage Sigil** (×1.60 Rastro), Warrior Emblem, botas, wings |
| **Prototype-02** (jefe del mod) | **Sanguine Cleaver (56)** (14 barras Adamantita/Titanio) | Hemocarnage (+60% Rastro) | Hemophage Sigil, Feral/Power Glove |
| **Los Gemelos** | Sanguine Cleaver (56) — usa el **tajo a distancia** (vuelan) | Hemocarnage | Hemophage Sigil, Mechanical Glove, wings |
| **The Destroyer** | Sanguine Cleaver (56) — tu sangrado prende en **todos los segmentos** | Hemocarnage | Hemophage Sigil, Ankh Shield en progreso |
| **Skeletron Prime** | **Hallowed Bloodletter (62)** (Hallowed + almas mecánicas) | Hemocarnage | + su alma da acceso al **Crimson Chalice** (×1.35) |
| **Plantera** 🔓 | Hallowed Bloodletter (62) / **Nullsteel Reaver (62)** / Chlorophyte Hemoblade (64) | Hemocarnage | Hemophage Sigil + **Crimson Chalice** (×2.16 combinados) |
| **Golem** | Chlorophyte Hemoblade (64) → dropea Beetle Husk para la siguiente | Hemocarnage | Fire Gauntlet, Celestial Stone |
| **Duke Fishron** | **Titan's Gutcleaver (84)** (Beetle Husk + Chlorophyte) | Hemocarnage | Wings imprescindibles, Celestial Shell |
| **Empress of Light** | Titan's Gutcleaver (84) | Hemocarnage | Ankh Shield, Master Ninja Gear |
| **Lunatic Cultist** | **Crimson Requiem (92)** (Broken Hero Sword + Hallowed + Chlorophyte; también dropea de Mothron) | Hemocarnage | full kit |
| **Moon Lord** 🔓 | mejor disponible → tras los Pilares, **Exsanguinator (112)** (barras lunares) | Hemocarnage | **Eternal Bulwark** (×1.30 Rastro + 12% melee) |
| **The Eternal** (jefe final del mod) | Exsanguinator (112) full equipado | Hemocarnage | todo lo anterior + pociones |

> 🩸 **Contra jefes multi-parte (Gemelos, Destructor, Moon Lord):** desde Plantera, una sola Q
> hace sangrar TODAS las partes a la vez y cada una te paga ingreso pasivo. Es el mejor
> escenario de tu mecánica.

> **Nota de armadura:** no hay un set de Guerrero pre-HM orientado a sangrado (el que existe,
> Steelbound, potencia Combo). Hasta Hemocarnage, usa mineral genérico o armadura vanilla —
> lo que importa pre-HM es subir afinidad, no el set.
