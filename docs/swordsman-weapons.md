# Diseño de armas del Espadachín — 18 hojas, 18 identidades

Documento de diseño (§13 del brief). Cada arma tiene su **fantasía**, su **mecánica**, su
**sprite PNG con paleta hex**, partículas, sonido y balance. Estética **pixel art de Terraria**
(no realista, no 3D, no anime). Todas comparten identidad de subclase: espadas, sangre, cortes.

**Leyenda de estado del código:**
- ✅ = implementado (tandas 1–2)
- 🔜 = diseñado, código pendiente (tanda 3: proyectil/mecánica a medida)

El proyectil compartido es `CrimsonSlash`, que ahora se comporta distinto según el `SlashStyle`
del arma. Las mecánicas on-hit viven en `SwordIdentityGlobalItem`.

---

## PRE-HARDMODE

### Training Blade  ·  daño 11  ·  `Straight` ✅
- **Concepto:** espada de práctica. La referencia de la que todo diverge.
- **Nombre:** "training" → deliberadamente sin truco; enseña el tajo base.
- **Mecánica:** tajo recto simple. Sin secundaria (a propósito, §12: temprana = simple).
- **Partículas:** niebla de sangre tenue en la estela.
- **Sonido:** `SoundID.Item1` (espadazo básico).
- **Sprite (≈30px):** hoja recta gris de hierro, guardia simple, empuñadura de cuero.
  - Paleta: `#9AA0A8` hoja · `#C7CCD2` filo/highlight · `#4A4E55` sombra · `#6B4A2E` mango.
- **Balance:** el más débil, sin ventaja/desventaja. Es el suelo.

### Serrated Iron Blade  ·  daño 13  ·  `Straight` + on-hit ✅
- **Concepto:** sierra de carnicero. La herida no cierra.
- **Nombre:** "serrated" (dentada) → **refresca el sangrado a 480t en cada mordida**.
- **Mecánica principal:** sangrado sostenido larguísimo. **Secundaria:** presión constante.
- **Partículas:** pequeñas virutas rojas al golpear.
- **Sonido:** `SoundID.Item18` (corte áspero).
- **Sprite (≈32px):** hoja con **dientes de sierra** en un borde, hierro oscuro.
  - Paleta: `#7E5B3A` hoja oxidada · `#B08050` dientes · `#3A2A1A` sombra · `#D24A3A` filo con sangre.
- **Balance:** DPS de sangrado alto sostenido, corto alcance. **+** uptime de bleed **−** poco por golpe.

### Silverlight Rapier  ·  daño 16  ·  `Pierce` ✅
- **Concepto:** estoque veloz y luminoso.
- **Nombre:** "rapier" (estoque) + "silverlight" → **estocada que atraviesa la línea entera**.
- **Mecánica:** el tajo **perfora a TODOS** en recto. **Secundaria:** rápido (useTime 14).
- **Partículas:** destellos plateados finos en el trazo.
- **Sonido:** `SoundID.Item1` con toque agudo.
- **Sprite (≈30px, fino):** hoja **larga y estrecha**, guardia en copa plateada, punta afilada.
  - Paleta: `#DDE6F0` hoja plata · `#FFFFFF` brillo · `#8A93A6` sombra · `#B9C7DA` guardia.
- **Balance:** perfora filas, daño bajo por golpe. **+** anti-línea **−** frágil per-hit.

### Hunter's Warblade  ·  daño 20  ·  `Straight` + on-hit ✅
- **Concepto:** hoja de cazador que remata presa herida.
- **Nombre:** "hunter" → **+20% daño a enemigos que ya sangran**.
- **Mecánica:** finisher de presa herida. **Secundaria:** nada extra contra sanos.
- **Partículas:** salpicadura roja concentrada al golpear sangrando.
- **Sonido:** `SoundID.Item1`.
- **Sprite (≈32px):** machete de caza con **muesca de gancho** cerca de la punta, cuero y hueso.
  - Paleta: `#5A6E3A` hoja verdosa · `#8FA65A` filo · `#2E3A1E` sombra · `#C9A06A` mango de hueso.
- **Balance:** brilla contra sangrando, mediocre contra sanos. **+** ejecución **−** base normal.

### Corruptor's Ripper  ·  daño 22  ·  `Straight` + on-hit ✅
- **Concepto:** filo corrupto que quema la herida.
- **Nombre:** "corruptor" (Corrupción) + "ripper" → **aplica Cursed Inferno** junto al sangrado.
- **Mecánica:** doble DoT (fuego maldito + sangre). **Secundaria:** temática de corrupción.
- **Partículas:** chispas moradas de fuego maldito al golpear.
- **Sonido:** `SoundID.Item1`.
- **Sprite (≈34px):** hoja **retorcida y con púas**, veteado morado/negro corrupto.
  - Paleta: `#6A3A8A` veteado · `#9A5ABF` brillo · `#2A102A` sombra · `#B0FF3A` chispa maldita.
- **Balance:** DoT doble fuerte. **+** daño sostenido **−** sin control ni alcance extra.

### Dread Reaver  ·  daño 22  ·  `Wide` ✅
- **Concepto:** segador pesado que infunde pavor.
- **Nombre:** "reaver" (saqueador/segador) + "dread" → **media luna enorme, lenta, que barre**.
- **Mecánica:** tajo **ancho** que casi no viaja pero corta todo en el arco. **Secundaria:** golpe pesado.
- **Partículas:** spray grueso de sangre en el arco (estilo `Wide`).
- **Sonido:** `SoundID.Item71` (corte pesado).
- **Sprite (≈40px, grande):** guadaña-espada ancha, hoja curva oscura, casi una hoz.
  - Paleta: `#7A1A20` hoja carmesí oscuro · `#B0303A` filo · `#2A0808` sombra · `#3A3A3A` mango de hierro.
- **Balance:** AoE cercano fuerte, lento (useTime 28). **+** limpia grupos pegados **−** nada a distancia.

### Thornrender  ·  daño 24  ·  `Split` ✅
- **Concepto:** filo espinoso de la jungla.
- **Nombre:** "thorn" (espina) + "render" → **el tajo estalla en 3 espinas** al terminar.
- **Mecánica:** control de grupos por fragmentación. **Secundaria:** abanico de cobertura.
- **Partículas:** hojas/espinas verdes al fragmentarse.
- **Sonido:** `SoundID.Item1`.
- **Sprite (≈32px):** hoja de madera-hueso con **espinas laterales**, savia roja.
  - Paleta: `#3E6B2A` hoja vegetal · `#6FA83A` filo · `#1E2E12` sombra · `#C23A3A` savia sangrienta.
- **Balance:** cobertura de área por fragmentos. **+** grupos dispersos **−** menos foco single-target.

### Bonewarden Sabre  ·  daño 24  ·  `Straight` + proyectil ✅
- **Concepto:** sable guardián de los muertos de la mazmorra.
- **Nombre:** "bone" + "warden" → **al golpear, brota una púa de hueso del enemigo** (`BoneSpike`) que
  aguanta ~42t mordiendo un par de veces. ✅
- **Mecánica principal:** zona de negación clavada a la herida. **Secundaria:** temática ósea.
- **Partículas:** fragmentos de hueso blancos al golpear.
- **Sonido:** `SoundID.NPCHit2` (hueso).
- **Sprite (≈32px):** sable curvo hecho de **hueso**, guardia de costillas, filo amarillento.
  - Paleta: `#E8E0C8` hueso · `#FFFDF0` highlight · `#A89878` sombra · `#8A1A20` grabado de sangre.
- **Balance:** daño persistente localizado. **+** zona de negación **−** lento de arrancar.

### Revenite Cleaver  ·  daño 25  ·  `Pierce` ✅
- **Concepto:** hachazo del revenant, tope de mineral pre-HM.
- **Nombre:** "cleaver" (hacha) → **el tajo atraviesa a todos** en línea, pesado.
- **Mecánica:** pierce-all frontal. **Secundaria:** hoja densa (mineral Revenite).
- **Partículas:** niebla de sangre al perforar.
- **Sonido:** `SoundID.Item71`.
- **Sprite (≈36px, ancho):** cuchilla de carnicero maciza, metal verde-oscuro con vetas rojas.
  - Paleta: `#2E5A3A` metal revenite · `#4A8A5A` brillo · `#12241A` sombra · `#B03030` vetas.
- **Balance:** perfora filas con peso. **+** anti-fila con daño **−** medio-lento.

### Molten Gutripper  ·  daño 27  ·  `Straight` + on-hit ✅ (🔜 rastro de lava)
- **Concepto:** acero al rojo del infierno.
- **Nombre:** "molten" + "gutripper" → **aplica En Llamas** (✅) y 🔜 **deja un rastro de lava** en el suelo.
- **Mecánica principal:** fuego + sangre. **Secundaria (pendiente):** zona de lava persistente.
- **Partículas:** brasas y chispas naranjas al golpear.
- **Sonido:** `SoundID.Item45` (fuego).
- **Sprite (≈34px):** hoja de hellstone **incandescente**, grietas de lava, guardia ennegrecida.
  - Paleta: `#8A2A10` hoja · `#FF7A20` grietas de lava · `#2A0A05` sombra · `#FFD060` brillo caliente.
- **Balance:** DoT de fuego + (futuro) control de suelo. **+** daño sostenido/zona **−** lento (25).

---

## PROMOCIÓN (te vuelves Espadachín)

### Bloodletter Blade  ·  daño 42  ·  ⭐ SIGNATURE  ·  `Straight` + on-hit ✅
- **Concepto:** LA espada del Espadachín. Recompensa de convertirte en uno.
- **Nombre:** "bloodletter" (sangrador) → **cada mordida banca +4 Rastro Carmesí**.
- **Mecánica principal:** alimenta tu recurso más rápido que ninguna. **Secundaria:** identidad de clase.
- **Partículas:** gotas de sangre que caen y se desvanecen rápido.
- **Sonido:** `SoundID.Item1` con eco.
- **Sprite (≈36px):** espada roja elegante, **canal central que "sangra"**, guardia en forma de gota.
  - Paleta: `#B91E2D` hoja · `#FF3A44` canal brillante · `#3A0808` sombra · `#E8C0C4` filo.
- **Balance:** la que mejor construye Rastro. **+** habilidad siempre lista **−** stats normales para su tier.

---

## HARDMODE

### Quicksilver Fang  ·  daño 44  ·  `Homing` ✅
- **Concepto:** colmillo velocísimo que busca sangre.
- **Nombre:** "quicksilver" (mercurio/veloz) + "fang" → **colmillos que se curvan hacia el que sangra**.
- **Mecánica:** homing suave + ataque rapidísimo (useTime 14). **Secundaria:** máxima cadencia.
- **Partículas:** rastro plateado-líquido fino.
- **Sonido:** `SoundID.Item1` agudo.
- **Sprite (≈30px):** daga-colmillo curva y estrecha, metal líquido plateado con punta roja.
  - Paleta: `#C8D0DA` mercurio · `#FFFFFF` brillo · `#7A8494` sombra · `#C23A3A` punta.
- **Balance:** DPS altísimo, poco por golpe, corto. **+** cadencia + auto-apunta **−** frágil per-hit.

### Sanguine Cleaver  ·  daño 56  ·  `Wide` + carga ✅
- **Concepto:** guillotina de sangre.
- **Nombre:** "sanguine" (sangre) + "cleaver" → click izq. = **media luna pesada** (✅); click der. =
  **mantener para cargar** (`SanguineCharge`) y soltar una **guillotina** (`SanguineGuillotine`)
  escalada por la carga: ~1.3x un espadazo al mínimo, ~3.4x a full. ✅
- **Mecánica principal:** golpe pesado AoE. **Secundaria:** carga liberable (alt-fire).
- **Partículas:** spray grueso carmesí; sangre que se agolpa en la hoja al cargar.
- **Sonido:** `SoundID.Item71`.
- **Sprite (≈40px, macizo):** cuchilla-guillotina enorme, rojo profundo, filo pulido.
  - Paleta: `#8A1420` hoja · `#C83040` filo · `#2A0808` sombra · `#5A5A5A` dorso metálico.
- **Balance:** mucho por golpe, lentísimo (useTime 30). **+** golpe demoledor y pico de carga enorme
  **−** te quedas quieto y vulnerable mientras cargas la guillotina.

### Hallowed Bloodletter  ·  daño 62  ·  `Split` ✅
- **Concepto:** sangre bendita.
- **Nombre:** "hallowed" (sagrado) + Bloodletter → **el tajo se fragmenta en esquirlas de luz**.
- **Mecánica:** fragmentación santa. **Secundaria:** cobertura sagrada.
- **Partículas:** destellos dorados al fragmentar.
- **Sonido:** `SoundID.Item1` cristalino.
- **Sprite (≈36px):** Bloodletter refinada en oro y blanco, **runas sagradas** en la hoja, canal rojo.
  - Paleta: `#F0E4B0` oro claro · `#FFFFFF` brillo · `#B08030` sombra dorada · `#C23A3A` canal de sangre.
- **Balance:** cobertura + daño sólido. **+** grupos + foco **−** fragmentos hacen menos que el tajo.

### Nullsteel Reaver  ·  daño 62  ·  `Return` + on-hit ✅
- **Concepto:** segador de vacío que niega.
- **Nombre:** "null" (vacío) + "reaver" → **bumerán que atraviesa y vuelve** (✅) + **Ichor** (baja defensa) ✅.
- **Mecánica principal:** proyectil de ida y vuelta (doble pasada). **Secundaria:** corroe armadura.
- **Partículas:** motes oscuros de vacío que se hunden.
- **Sonido:** `SoundID.Item71` con eco grave.
- **Sprite (≈36px):** hoja de metal **negro-vacío** con borde violeta, grieta luminosa en el centro.
  - Paleta: `#1A1420` metal null · `#5A2A8A` borde vacío · `#0A0810` sombra · `#B060FF` grieta.
- **Balance:** doble golpe (ida+vuelta) + defense-shred. **+** dos pasadas **−** requiere buen posicionamiento.

### Chlorophyte Hemoblade  ·  daño 64  ·  `HomingAggressive` ✅
- **Concepto:** hoja viva que persigue la sangre.
- **Nombre:** "chlorophyte" (teledirigido clásico) + "hemo" → **tajos que persiguen agresivamente** a los que sangran.
- **Mecánica:** homing fuerte multi-objetivo. **Secundaria:** temática viva/vegetal.
- **Partículas:** esporas verdes + niebla de sangre.
- **Sonido:** `SoundID.Item1`.
- **Sprite (≈36px):** hoja de cristal de clorofita, **venas rojas latiendo** dentro del verde.
  - Paleta: `#3AA85A` cristal · `#7AE89A` brillo · `#1A4A2A` sombra · `#C23030` venas de sangre.
- **Balance:** casi imposible de fallar. **+** auto-apunta fuerte **−** daño medio para su tier.

### Titan's Gutcleaver  ·  daño 84  ·  `Wide` + proyectil ✅
- **Concepto:** hacha colosal que sacude el suelo.
- **Nombre:** "titan" (colosal) → **media luna gigante** (✅) + **onda de choque** (`TitanShockwave`)
  que recorre el suelo desde el jugador al golpear (par izq/der, una por espadazo). ✅
- **Mecánica principal:** el arma más pesada, mayor impacto. **Secundaria:** onda terrestre que barre.
- **Partículas:** polvo y escombros al impactar.
- **Sonido:** `SoundID.Item70` (impacto pesado).
- **Sprite (≈44px, el más grande):** cuchilla titánica de placas de beetle, naranja/bronce, remaches.
  - Paleta: `#B0762A` placas · `#E8A850` brillo · `#5A3A10` sombra · `#C23030` filo ensangrentado.
- **Balance:** daño y AoE brutales, el más lento (useTime 32). **+** devastador **−** cadencia mínima.

### Crimson Requiem  ·  daño 92  ·  `Straight` + marca/detona ✅
- **Concepto:** el canto fúnebre. Ejecución.
- **Nombre:** "requiem" (misa de muerte) → cada golpe **marca** al enemigo; a las **5 marcas** el réquiem
  **detona** (`RequiemDetonation`): un estallido carmesí que ejecuta al grupo alrededor. Las marcas
  se desvanecen si dejas de golpear (hay que "cantar" el réquiem). ✅
- **Mecánica principal:** marcas + detonación en umbral (ejecución de grupo). **Secundaria:** presión constante para no perder marcas.
- **Partículas:** notas/sigilos carmesí flotando sobre los marcados.
- **Sonido:** `SoundID.Item122` (energía) + un tono grave al detonar.
- **Sprite (≈38px):** espada Terra-Blade reforjada en carmesí, **partitura grabada** en la hoja, aura tenue.
  - Paleta: `#C41E30` hoja · `#FF4A58` aura · `#3A0810` sombra · `#F0D0D4` grabado de partitura.
- **Balance:** ejecución de grupo demoledora, daño base normal-alto. **+** limpia grupos marcados **−** necesita montar las marcas.

### Exsanguinator  ·  daño 112  ·  `HomingAggressive` + on-hit ✅
- **Concepto:** el drenador final de sangre.
- **Nombre:** "exsanguinate" (drenar toda la sangre) → **persigue** (✅) y **drena Rastro + vida** del que sangra ✅.
- **Mecánica principal:** homing fuerte + robo de vida/recurso. **Secundaria:** fantasía de poder endgame.
- **Partículas:** hilos de sangre espectral que fluyen del enemigo hacia ti.
- **Sonido:** `SoundID.Item122` etéreo.
- **Sprite (≈38px):** hoja de luminita espectral **cian-blanca** con **canal de sangre** que fluye hacia la empuñadura.
  - Paleta: `#78F0DC` luminita · `#C8FFF6` brillo · `#2A6A64` sombra · `#C23040` canal de sangre.
- **Balance:** endgame: sustain + auto-apunta + drena. **+** sostenibilidad total **−** brilla solo contra sangrando.

---

## Nota de arte

**Ya NO son placeholder.** Cada una de las 19 hojas tiene su propio sprite pixel-art real, generado
por su nombre + la paleta de este doc (con System.Drawing, 56–72px), en
`Content/Items/Weapons/Warrior/<Nombre>.png` (y `Promotion/BloodletterBlade.png`). El proyectil
también: 4 formas de tajo (`CrimsonSlash`, `_Heavy`, `_Pierce`, `_Return`) que `CrimsonSlash`
elige según el estilo del arma y tiñe con su color.

Son sprites **procedurales** — buenos como base de calidad, con la forma/tamaño/paleta ya fijadas.
Si más adelante un/a artista quiere pintarlos a mano, este doc sigue siendo la guía y basta
reemplazar el PNG en su sitio. Los efectos a medida (onda del Titan, púas, detonación del Requiem)
se dibujan con partículas a propósito y no necesitan PNG.
