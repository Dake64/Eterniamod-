# Rediseño de katanas del Espadachín — ANÁLISIS (previo al código)

> Documento de revisión. **Todavía no se ha tocado código ni sprites.**

---

## 0. Nota obligatoria sobre la API

**`tmod_api_entities.json` NO existe** — ni en el repo ni en el sistema (lo busqué en todo
`C:\Users\USUARIO`). No puedo usarlo como fuente de verdad.

**Alternativa que sí confirma la API:** compilar contra los ensamblados reales de tModLoader
(`dotnet build .\ETERNIA.csproj -t:Compile`). Si un hook, propiedad o firma no existe, el
compilador falla. Es verificación más fuerte que un JSON, porque valida contra la versión
instalada (tModLoader 2026.5.3.0). Los hooks que pienso usar van listados en §6.

Si prefieres que trabaje contra ese JSON, tendrías que proporcionármelo.

---

## 1. Estructura medida de la Muramasa de Calamity

No la describí de memoria: medí `Murasama.png` píxel a píxel (referencia **estructural**; no se
copia su arte).

**Datos duros** (fotograma 0 de 13; hoja aislada por color, filtrando las piezas grises):

| Medida | Valor |
|---|---|
| Lienzo / fotograma | 90 × 134 px |
| Arte real | 50 × 120 px |
| **Escala de dibujo** | **2x** (cada color sale en pares → diseñada a media resolución) |
| Filas de hoja | 100 px = **50 px lógicos** |
| Ancho de hoja | 6 lógicos en la base → 5 medio → 4 arriba → 2 en la punta |
| **Proporción hoja** | **~10:1** (largo : ancho) |
| Guarda | ~11 lógicos de ancho (≈2× el ancho de hoja), ~4 de alto |
| Empuñadura | ~4 lógicos de ancho (más **estrecha** que la base de la hoja) |
| Colores | 23 |

### 1.1 LA CURVATURA (el punto crítico)

Medí, fila a fila, la desviación de la hoja respecto a la recta que une base y punta:

```
y     lomo   filo   desvLomo  desvFilo
 18     52     59       +1.9      +5.9
 30     50     59       +3.8      +9.8
 42     48     57       +5.7     +11.7
 54     44     53       +5.6     +11.6
 66     40     49       +5.5     +11.5
 78     34     45       +3.3     +11.3
```

**Todas las desviaciones son POSITIVAS** → la hoja bombea hacia +x.

Con la punta arriba-derecha, el **lomo** (mune, rojo oscuro `9A0000`) va a la **izquierda** y el
**filo** (ha, rojo intenso `EE0033`) a la **derecha**. La hoja bombea hacia la derecha, es decir:

> ### **EL FILO VA EN EL LADO CONVEXO (exterior) DE LA CURVA.** Ésa es la regla anatómica.

La flecha máxima es de ~4 lógicos sobre 50 de hoja ≈ **8% del largo**.

### 1.2 Por qué mis sprites están mal (medido, no supuesto)

Pasé mis sprites por la misma herramienta:

| | Murasama | Míos (Bloodletter) |
|---|---|---|
| Desviación | +5 a +12 px, constante | **−3 a +2 px** (ruido alrededor de 0) |

**Mis hojas no tienen la curva invertida: no tienen curva.** El fallo es del método — yo repartía
"roturas de escalera" de forma **uniforme** a lo largo de la hoja, y eso solo cambia la
**pendiente**; el resultado sigue siendo una recta, solo que más inclinada. Y el residuo que queda
apunta hacia el **lomo**, el lado equivocado.

**Corrección:** sustituir el reparto de roturas por un **arco explícito**:

```
x(fila) = baseX + floor(t * largo) + round(sori * sin(PI * t))
```

con `t` = 0 en la base y 1 en la punta, y `sori` **positivo hacia el filo**. Eso da flecha 0 en los
extremos y máxima en el centro: un arco real, con el filo convexo. `sori` ≈ 8% del largo.

**Verificación obligatoria:** tras regenerar, vuelvo a pasar la herramienta de medición sobre mis
PNG y compruebo que la desviación es positiva y con forma de arco. No doy un sprite por bueno
mirándolo: lo mido.

---

## 2. ANATOMÍA BASE — la familia Eternia

Toda katana del mod parte de **esta misma estructura**. Lo que cambia por arma es material,
ornamentación, guarda, empuñadura y efectos; **nunca la anatomía**.

| Elemento | Regla |
|---|---|
| Escala | Diseño en píxeles lógicos, salida **×2** |
| Ángulo | Punta arriba-derecha, ~3 columnas por cada 4 filas |
| Curvatura | Arco `sin`, flecha 8% del largo, **convexa hacia el filo** |
| Proporción | Hoja ~10:1 largo/ancho |
| Ancho | Base 6 → medio 5 → alto 4 → punta 2 (lógicos), afinado suave |
| Punta (kissaki) | Corta: último ~8%. Cierra **desde el lomo**; el filo llega recto |
| Yokote | Línea clara de 1 px que separa el kissaki |
| Sección | 5-6 tonos: lomo oscuro → cuerpo → claro → núcleo blanco → **filo saturado** |
| Habaki | Collar claro de 2 filas entre guarda y hoja |
| Guarda | ~2× el ancho de hoja, 3-4 filas, elíptica |
| Empuñadura | Más **estrecha** que la base de la hoja, corta (~25% del arma), envuelta en rombos |
| Ensanche | La hoja se abre 1-2 px al salir del habaki |

### 2.1 Hallazgo del prototipo: el ancho mínimo de hoja es 5, no 4

Al probar el arco descubrí un fallo colateral: con hoja de **4** lógicos la rampa queda
`lomo → cuerpo → núcleo → filo`, o sea el **blanco toca directamente al rojo medio**. En diagonal
y a escala ×2 ese salto de contraste hace que el núcleo se lea como **guiones sueltos**, no como
una línea encendida.

Murasama lo evita metiendo el tono **claro** entre cuerpo y núcleo. Por tanto:

- **Ancho por defecto de la familia: 5 lógicos** (rampa `lomo → cuerpo → claro → núcleo → filo`).
- Las armas listadas como `W4` en §4 pasan a `W5`; las `W3` (Silverlight) a `W4`; las `W6`/`W8`
  se mantienen y rellenan el tramo intermedio repitiendo cuerpo.
- Regla general: **el núcleo nunca es adyacente al cuerpo**; siempre va precedido del tono claro.

---

## 3. LAS 19 KATANAS — estado actual y problemas

### 3.1 Repetición real (medida sobre el código)

| Comportamiento | Cuántas lo comparten | Armas |
|---|---|---|
| **`Straight`** | **8** | Training, Serrated, Corruptor, Hunter, Molten, Bonewarden, Bloodletter, Requiem |
| `Wide` | 3 | Dread, Sanguine, Titan |
| `Pierce` | 2 | Silverlight, Revenite |
| `HomingAggressive` | 2 | Chlorophyte, Exsanguinator |
| `Split` | 2 | Hallowed, Thornrender |
| `Homing` | 1 | Quicksilver |
| `Return` | 1 | Nullsteel |

Además: **todas** lanzan el mismo `CrimsonSlash` (solo 4 formas de textura), **todas** usan
`DustID.Blood` como partícula principal, y **todas** tienen el mismo ritmo de animación
(aparecer → volar recto → desvanecer).

Ése es el motivo real de que "se sientan la misma arma".

### 3.2 Tabla comparativa — nueva identidad

| Katana | Problema actual | Nueva identidad | Tipo de ataque | Animación | Partículas |
|---|---|---|---|---|---|
| **Training Blade** | `Straight` genérico | La referencia honesta | Un corte limpio, sin adorno | Neutra, corta | Chispas metálicas apagadas |
| **Serrated Iron Blade** | `Straight` genérico | Sierra que desgarra | **3 tajos cortos** en ráfaga | Nerviosa, entrecortada | Virutas de metal + gotas |
| **Silverlight Rapier** | `Pierce` compartido | Estocada de luz | **Línea recta** que atraviesa todo | Instantánea, seca | Líneas de velocidad finas |
| **Hunter's Warblade** | `Straight` genérico | Cazador de heridos | Corte que **se lanza** hacia el que sangra | Depredadora, con tirón | Motas verdes que convergen |
| **Corruptor's Ripper** | `Straight` genérico | Corrupción errática | Tajo que **serpentea** al avanzar | Caótica, temblorosa | Esporas moradas + chispa lima |
| **Dread Reaver** | `Wide` compartido | Pavor que se expande | Media luna que **crece** al viajar | Lenta, pesada | Niebla roja oscura |
| **Thornrender** | `Split` compartido | Zarza que arraiga | **Siembra espinas** en su camino | Deliberada | Fragmentos de hoja/espina |
| **Bonewarden Sabre** | `Straight` genérico | Guardián de huesos | Púas óseas brotan al golpear | Rápida, seca | Esquirlas de hueso |
| **Revenite Cleaver** | `Pierce` compartido | Lanza pesada | Perfora y **frena** al avanzar | Pesada, contundente | Esquirlas de metal verde |
| **Molten Gutripper** | `Straight` genérico | Acero fundido | **Deja rastro de brasas** | Densa, humeante | Ascuas que suben |
| **Bloodletter Blade** ⭐ | `Straight` genérico | La firma del Espadachín | Corte + **eco fantasma** detrás | Elegante, con eco | Gotas que caen |
| **Quicksilver Fang** | `Homing` suave | Colmillo de mercurio | **Dos tajos en X**, muy rápidos | Vertiginosa | Hilos de metal líquido |
| **Sanguine Cleaver** | `Wide` compartido | Guillotina | **Carga y suelta** un corte brutal | Muy lenta → estallido | Chorro espeso |
| **Hallowed Bloodletter** | `Split` compartido | Sangre bendita | Estalla en **esquirlas radiantes** | Luminosa, breve | Motas doradas |
| **Nullsteel Reaver** | `Return` propio (ok) | Vacío que devuelve | Bumerán que **vuelve más grande** | Sobrenatural, flotante | Motas que se hunden |
| **Chlorophyte Hemoblade** | `HomingAggr.` compartido | Cristal vivo | Persigue con **giros cerrados** | Insistente, viva | Esporas + vetas rojas |
| **Titan's Gutcleaver** | `Wide` compartido | Coloso | Corte + **onda por el suelo** | Sísmica | Polvo y escombros |
| **Crimson Requiem** | `Straight` genérico | Canto fúnebre | **Marca** y detona al acumular | Contenida → explosiva | Sigilos carmesí |
| **Exsanguinator** | `HomingAggr.` compartido | Drenaje total | **Cortes fantasma** que drenan | Espectral, continua | Hilos de sangre hacia ti |

**Ninguna comparte tipo de ataque con otra.** 19 armas, 19 comportamientos.

---

## 4. SPRITES — especificación por arma

Todas comparten la anatomía de §2. Aquí solo va **lo que las diferencia**.
`W` = ancho de hoja en píxeles lógicos. Rampa = lomo → cuerpo → claro → núcleo → filo.

### Pre-hardmode

**Training Blade** · W4 · sori suave
Acero liso sin ornamento. Guarda de hierro simple, empuñadura de cuero marrón.
`#3A3F4A #6A7280 #9AA4B2 #CFD8E2 #FFFFFF` · filo `#B6C0CE` · guarda `#8A6A3A` · mango `#4A3320`

**Serrated Iron Blade** · W4 · sori suave
**Dientes de sierra** de 1 px en el filo, cada 2 filas. Hierro oxidado, guarda tosca.
`#3A2A16 #7A5A32 #A87A46 #D8A868 #FFE0B0` · filo `#E04A38` · guarda `#6A5028`

**Silverlight Rapier** · W3 · **sori = 0** (chokutō, recta)
La única recta de la familia — es su identidad. Hoja finísima, guarda dorada, mango azul noche.
`#5A6478 #8A94A8 #B4C0D2 #DCE6F2 #FFFFFF` · guarda `#C8A24A` · mango `#38405A`

**Hunter's Warblade** · W5 · sori marcado
Hoja ancha de caza, **muesca de gancho** cerca de la punta. Mango de hueso claro.
`#233016 #40561F #5E7438 #8CA858 #DCF0A0` · guarda `#8A6A3A` · mango `#C9A06A`

**Corruptor's Ripper** · W4 · sori marcado
**Púas** irregulares en el filo. Hoja morada con **filo lima** (contraste corrupto).
`#240C2C #45206A #6E3C92 #A06CC8 #E8C8FF` · filo `#B8FF44` · guarda `#4A2060`

**Dread Reaver** · W6 · sori suave
Hoja pesada y ancha, guarda de hierro negro, mango envuelto en tela oscura.
`#2A0608 #5A1014 #8E1C24 #C43038 #FFB0B6` · filo `#E04048` · guarda `#4A4A50`

**Thornrender** · W4 · sori marcado
**Espinas** que salen del filo cada 3 filas. Hoja vegetal, savia roja en el canal.
`#142609 #2A5218 #3E7A2E #68B848 #C8F0A0` · acento `#D24242` · guarda `#7A5A2A`

**Bonewarden Sabre** · W4 · **sori muy marcado**
Hoja de **hueso** (marfil), guarda de costillas, grabado granate.
`#6A5E44 #A89A78 #DCD2B4 #F2ECD4 #FFFEF4` · acento `#A02028` · guarda `#B09A6A`

**Revenite Cleaver** · W6 · sori suave
Hoja gruesa de mineral verde con **vetas rojas**. Guarda cuadrada maciza.
`#0C1C12 #1E4A2C #2E6A44 #4EA86A #B0F0C0` · acento `#C43838` · guarda `#1A4028`

**Molten Gutripper** · W4 · sori suave
Filo **astillado** (irregular, +1 px cada 3 filas). Grietas de lava en la hoja.
`#260A04 #6A1E08 #A03412 #E06A16 #FFE08A` · filo `#FF8A22` · guarda `#4A2008`

### Promoción

**Bloodletter Blade** ⭐ · W4 · sori medio
La insignia. **Canal de sangre** (bo-hi) recorriendo la hoja, **guarda plateada** grande, mango
azul noche. Paleta tomada de la medición:
`#4F0B1C #9A0000 #CB0000 #FF5E67 #FFE2E6` · filo `#EE0033` · guarda `#8A94A6` · mango `#231B30`

### Hardmode

**Quicksilver Fang** · W4 · **sori extremo** (wakizashi corta)
La más corta y curva. Metal líquido, punta con tinte rojo.
`#4A5464 #7A8698 #A8B4C6 #D4DEEC #FFFFFF` · acento `#C23A3A`

**Sanguine Cleaver** · W8 · sori suave (ōdachi)
Colosal. Guarda ancha de acero, mango largo a dos manos.
`#260406 #5E0C14 #9A1622 #D0303E #FFB4BC` · filo `#F04452` · guarda `#5A5A60`

**Hallowed Bloodletter** · W4 · sori medio
Bloodletter refinada: **runas** en el lomo, guarda dorada con alas, canal rojo.
`#8A6018 #C09A38 #E8D28A #F8ECC0 #FFFFFF` · acento `#D83A48` · guarda `#C8A24A`

**Nullsteel Reaver** · W5 · sori marcado
Hoja **negro-vacío** con una **grieta violeta luminosa** en zigzag. Punta astillada.
`#120A20 #2A1C44 #4A3070 #7A46B4 #D8B0FF` · acento `#B060FF` · guarda `#3A2A54`

**Chlorophyte Hemoblade** · W4 · sori medio
Cristal verde translúcido con **vetas rojas latiendo**.
`#0C3018 #1E6E38 #34AA5A #5ED47C #D8FFE0` · acento `#D02C2C` · guarda `#1A4A2A`

**Titan's Gutcleaver** · W8 · sori suave (ōdachi)
Titánica, **remaches** visibles en el lomo, placas ámbar.
`#3A2206 #7A4E14 #C08430 #E8AE4E #FFEEC0` · acento `#C43030` · guarda `#5A3A10`

**Crimson Requiem** · W5 · sori medio
**Partitura grabada** en la hoja, guarda dorada, gema en la guarda.
`#38060E #8A101C #D2202E #FF5460 #FFD0D6` · filo `#FF3A4A` · guarda `#C8A24A`

**Exsanguinator** · W4 · sori suave
Luminita espectral cian con **canal de sangre** que fluye hacia el mango. Punta en gancho.
`#14484A #2E8A84 #5EE8D2 #A8FCEE #FFFFFF` · acento `#D02C40` · guarda `#2A6A64`

---

## 5. PROYECTILES — especificación por arma

Marco común: todos son `ModProjectile`, `DamageClass.Melee`, propiedad del jugador (heredan
sangrado + Rastro Carmesí por el pipeline existente), y **solo el dueño los genera**
(`Main.myPlayer == owner`) por seguridad en multijugador.

| Arma | Forma | Trayectoria | Vida | Al aparecer | Al golpear | Al morir | Sonido |
|---|---|---|---|---|---|---|---|
| Training | Arco corto | Recta, sin giro | 30t | — | 4 chispas | Se desvanece | `Item1` |
| Serrated | 3 arcos mini | Recta, desfasados 4t | 22t c/u | Chirrido | Virutas | — | `Item18` |
| Silverlight | Línea fina | Recta, muy rápida, atraviesa | 24t | Destello | Línea de luz | Se apaga | `Item1` agudo |
| Hunter | Arco + tirón | Acelera si hay sangrando cerca | 40t | — | Motas verdes | — | `Item1` |
| Corruptor | Arco irregular | **Serpentea** (seno lateral) | 45t | — | Esporas | Estallido morado | `Item1` |
| Dread | Media luna | Lenta, **escala x1 → x1.8** | 34t | — | Niebla | Se disipa | `Item71` |
| Thornrender | Arco + siembra | Recta, **deja espinas fijas** | 40t | — | Hojas | Espina final | `Item1` |
| Bonewarden | Arco corto | Recta rápida | 26t | — | **Púa ósea** | — | `NPCHit2` |
| Revenite | Lanza | Recta, **frena** (x0.94/t) | 40t | — | Esquirlas | — | `Item71` |
| Molten | Arco | Recta, **suelta brasas** | 40t | — | Ascuas | Rastro de fuego | `Item45` |
| Bloodletter ⭐ | Arco limpio | Recta + **eco a 6t** | 40t | — | Gotas | — | `Item1` |
| Quicksilver | 2 arcos en X | Cruzados, homing suave | 30t | — | Hilos plata | — | `Item1` agudo |
| Sanguine | Guillotina | **Carga** → cae, lenta | 46t | Sangre se agolpa | Chorro | — | `Item71` |
| Hallowed | Arco → esquirlas | Recta, **estalla al apogeo** | 36t | — | Motas doradas | 5 esquirlas | `Item1` cristal |
| Nullsteel | Bumerán | Va y **vuelve más grande** | 120t | — | Motas vacío | Al volver | `Item71` grave |
| Chlorophyte | Arco vivo | **Homing cerrado** | 50t | — | Esporas | — | `Item1` |
| Titan | Media luna | Lenta + **onda al suelo** | 30t | Temblor | Polvo | Onda | `Item70` |
| Requiem | Arco + sigilo | Recta, **marca** | 40t | — | Sigilo | Detonación a 5 | `Item122` |
| Exsanguinator | Arco fantasma | Homing + **estelas** | 50t | — | Hilos hacia ti | — | `Item122` |

### Ritmos (para que no todas se sientan igual)

- **Rápidas / agresivas:** Quicksilver, Silverlight, Serrated, Bonewarden
- **Pesadas / poderosas:** Titan, Sanguine, Dread, Revenite
- **Precisas:** Silverlight, Hunter
- **Elegantes:** Bloodletter, Hallowed
- **Caóticas:** Corruptor, Molten
- **Sobrenaturales:** Nullsteel, Exsanguinator, Requiem

---

## 6. Plan de implementación (para aprobar)

**Fase A — sprites. ✅ HECHA.** Arco corregido y las 19 regeneradas con la anatomía de §2 y los
detalles de §4.

La verificación **está dentro del generador**: tras construir la tabla de filas, mide su propia
flecha contra la cuerda base→punta y **lanza error** si la hoja sale recta, si la flecha es
negativa (filo cóncavo) o si el máximo no cae cerca del centro. Resultado:

```
TrainingBlade         +2.9 px  t=0.62   SanguineCleaver      +3.9 px  t=0.61
SerratedIronBlade     +2.9 px  t=0.62   HallowedBloodletter  +4.9 px  t=0.63
SilverlightRapier     RECTA (chokuto)   NullsteelReaver      +5.9 px  t=0.63
HuntersWarblade       +4.9 px  t=0.62   ChlorophyteHemoblade +4.9 px  t=0.63
CorruptorsRipper      +5.9 px  t=0.63   TitansGutcleaver     +3.9 px  t=0.61
DreadReaver           +3.9 px  t=0.63   CrimsonRequiem       +4.9 px  t=0.63
Thornrender           +5.9 px  t=0.63   Exsanguinator        +3.9 px  t=0.63
BonewardenSabre       +7.3 px  t=0.53   BloodletterBlade     +4.9 px  t=0.63
ReveniteCleaver       +3.9 px  t=0.63   MoltenGutripper      +3.9 px  t=0.63
QuicksilverFang       +6.3 px  t=0.62
```

Todas positivas (filo convexo) y con el máximo cerca del centro. La aserción de recorte en los
cuatro bordes cazó de paso que el **gancho de punta del Exsanguinator** se salía del lienzo: los
adornos de punta ahora se suman al ancho calculado.

**Fase B — proyectiles.** Ampliar el sistema actual: mantener `CrimsonSlash` como base para las
que son variaciones de tajo, y añadir `ModProjectile` propios solo donde la mecánica lo exige
(serpenteo, siembra, eco, X, esquirlas, drenaje). Evita duplicar 19 clases casi idénticas.

**Se conserva sin tocar:** daño, useTime, rareza, recetas, progresión, nombres, `BleedChance`, el
pipeline de sangrado / Rastro Carmesí y las mecánicas ya existentes (carga del Sanguine,
marca/detonación del Requiem, onda del Titan, púas del Bonewarden).

**Hooks a usar** — todos ya en uso en el mod, por tanto verificados por compilación:
`ModProjectile.SetDefaults / AI / OnHitNPC / OnKill / PreDraw`,
`GlobalItem.OnHitNPC / ModifyHitNPC`, `ModItem.Shoot / CanUseItem / AltFunctionUse`,
`Projectile.NewProjectile`, `Dust.NewDustPerfect`.
Ninguno inventado. Si en la fase B necesito algo nuevo, lo señalo y lo valido compilando antes.
