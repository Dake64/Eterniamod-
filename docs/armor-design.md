# Armaduras de Eternia — diseño visual

Referencia para construir los archivos de armadura. Los PNG están en `docs/armor-reference/`
(75 piezas: `<Set>_Head.png`, `_Body.png`, `_Legs.png`, 32×32, 1×, sin anti-aliasing).

---

## 0. Lo que hace falta saber antes de empezar

Una armadura de Terraria son **dos** conjuntos de sprites, no uno:

| | Qué es | Formato |
|---|---|---|
| **Icono de inventario** | Lo que se ve en la mochila y el crafteo | ~32×32, uno por pieza |
| **Textura equipada** | Lo que se dibuja **sobre el jugador** | Hoja **40×1120 = 20 fotogramas** |

Por set completo: 3 iconos + hojas de `_Head`, `_Body`, **`_Arms`** y `_Legs` → unos **80 fotogramas
de animación**. Por los 25 sets, ~2.000 fotogramas.

**Lo que hay en este documento son los ICONOS y el diseño**, que es lo que sirve de referencia
para dibujar después las hojas equipadas. Las hojas equipadas siguen pendientes; ver §5.

Estado actual del mod: los 25 sets existen en código, **ninguno tiene arte propio**. Cada pieza
apunta a un slot de armadura vanilla (`headSlot = ArmorIDs.Head.TurtleHelmet`, etc.), lo que hace
que el jugador se vea con esa armadura vanilla. Está documentado así en `EterniaArmor.cs`, y ese
es exactamente el punto que hay que sustituir.

---

## 1. Reglas de la familia

Lo que comparten TODAS las armaduras de Eternia:

- **32×32**, pixel art 1×, bordes duros, **sin** anti-aliasing ni degradados suaves.
- **Cinco tonos por set** y nada más: principal, secundario, sombra, brillo, acento.
  (Los sets con energía añaden un blanco de brillo puntual, nunca como relleno.)
- **La silueta identifica, la paleta ambienta.** A 32 px el color se pierde antes que la forma,
  así que dos sets nunca comparten el mismo tipo de casco *y* de peto.
- **Contorno oscuro de 1 px** propio del set (no negro puro: una versión muy oscura del principal).
- El brillo va **solo donde la ficción lo justifica**, y como acento, no como baño.

## 2. Lenguaje por clase

| Clase | Casco | Torso | Piernas | Lectura |
|---|---|---|---|---|
| **Guerrero** | Cerrado: `fullhelm`, `horned` | `plate`, `carapace` | `greaves`, `plated` | Macizo, hombreras, ranura de visión estrecha |
| **Mago** | `hood`, `crown` | `robe`, `cloak` | `skirt` | Tela que cae, silueta ensanchada abajo |
| **Ranger** | `visor`, `hood` | `coat`, `harness` | `trousers`, `wrapped` | Ligero, correas, bandolera, botas altas |
| **Invocador** | `skull`, `mask` | `cloak`, `carapace` | `wrapped`, `skirt` | Orgánico, ritual, hueso y marcas |
| **Soul-metal** | Varía por tier | Varía por tier | Varía por tier | Sin clase: el metal responde al Alma que lleves |

## 3. Progresión

El nivel se lee por **cantidad de piezas en la silueta**, no por saturación:

| Etapa | Casco | Torso | Piernas | Energía |
|---|---|---|---|---|
| Temprana | Liso, sin apéndices | Peto simple, sin hombreras marcadas | Grebas planas | Ninguna |
| Media | Cresta o cuernos cortos | Hombreras, cinturón, emblema | Placas segmentadas | Un acento |
| Avanzada | Corona / cuernos largos / máscara | Capa, núcleo, runas | Placas + botas marcadas | Brillo puntual visible |

## 4. Los 25 sets

Paletas en HEX: **principal · secundario · sombra · brillo · acento**.

### Soul-metal (sin clase — progresión mineral)

| Set | Piezas | Paleta | Idea |
|---|---|---|---|
| **Soulstone** | fullhelm · plate · greaves | `9AA0AA 6E7480 3A3E48 C8D0DA B8A24A` | Piedra y hierro basto. El punto de partida: nada brilla todavía |
| **Animite** | crested · plate · plated | `C0902E 8A6418 4A3408 F0C86A E8E0C0` | Latón con cresta. Primer adorno del mod |
| **Revenite** | fullhelm · carapace · plated | `2E6A44 1A4028 0C1C12 6ABE84 C43838` | Mineral que se cierra sobre el cuerpo como caparazón; vetas rojas |
| **Wraithite** | hood · cloak · wrapped | `7A2030 4A0E1A 240408 C4505E F0A0AC` | Capucha y capa: el metal ya no protege, envuelve |
| **Nullsteel** | crested · plate · plated | `3A2A58 241A3A 0E0818 7A5EB0 B060FF` | Acero del vacío, grieta violeta |
| **Aetherium** | crown · carapace · plated | `4A8AC8 2A5A94 12283F 9AD0F8 F0F4FF` | Corona y caparazón celeste. Cima del mineral |

### Guerrero

| Set | Piezas | Paleta | Idea |
|---|---|---|---|
| **Steelbound** | fullhelm · plate · greaves | `8A5030 5A3018 2A1408 C08050 D8A030` | Hierro remachado, correas de cuero |
| **Ironknuckle** | horned · plate · greaves | `6E7480 474C58 24272E A8B0BC C43838` | Cuernos cortos, puños reforzados |
| **Wardplate** | fullhelm · plate · plated | `4A6E9A 2E4668 141F30 8AB0D8 E8D070` | Guardia: escudo hecho armadura |
| **Ironchain** | horned · plate · plated | `5A5E68 3A3E48 1A1C22 9098A4 B84040` | Cadena y placa, carcelario |
| **Hemocarnage** | horned · carapace · greaves | `8A1420 5A0810 260406 D0303E FF6070` | Armadura que sangra: placas como costillar |
| **Aegis Bulwark** | fullhelm · plate · plated | `C8A24A 8A6A20 40300A F8E0A0 6ED0F0` | Oro ceremonial con núcleo azul. Cima de guerrero |

### Mago

| Set | Piezas | Paleta | Idea |
|---|---|---|---|
| **Emberweave** | hood · robe · skirt | `A03412 6A1E08 280A04 E87A20 FFD060` | Túnica que arde por dentro |
| **Everflow** | hood · robe · skirt | `2E7A8A 185060 0A2028 6EC0D0 A8F0FF` | Corriente perpetua; tela que fluye |
| **Prismatic** | crown · robe · skirt | `C8C0E8 8A80B0 3E3860 FFFFFF E060C0` | Corona y luz refractada |
| **Blightweave** | hood · cloak · skirt | `4A6A28 2E4416 121C08 8AB050 C8FF60` | Tela enferma, esporas |
| **Lich Regalia** | crown · robe · skirt | `2A2440 181428 0A0814 5A5080 60E8D0` | Corona de hueso y fuego frío. Cima de mago |

### Ranger

| Set | Piezas | Paleta | Idea |
|---|---|---|---|
| **Hunter's Garb** | hood · coat · trousers | `6A5432 46351C 20180C A0885C 8A9A48` | Abrigo de caza, sin metal |
| **Hawkeye Garb** | visor · harness · trousers | `3A6A4A 24462E 0E1E14 72B088 E8C040` | Visor y bandolera; ojo dorado |
| **Gunslinger Rig** | visor · harness · wrapped | `4A4038 302820 161210 847264 FF9030` | Arnés de pólvora, correas y cartuchos |
| **Reactor Suit** | visor · harness · plated | `2A4A5A 183038 0A1618 5A98B0 50FFC8` | Traje presurizado, energía verde. Cima de ranger |

### Invocador

| Set | Piezas | Paleta | Idea |
|---|---|---|---|
| **Packmaster** | skull · cloak · wrapped | `7A6A4A 50442E 262014 B0A078 C05030` | Cráneo de bestia y pieles |
| **Alphahide** | skull · carapace · wrapped | `6A4030 44281C 1E120C A0704E E8B040` | Piel del alfa, costillar |
| **Legion Regalia** | mask · cloak · skirt | `4A2A6A 2E1844 140A1E 8A5EB0 F0C040` | Máscara ritual y capa de mando |
| **Exoframe** | mask · carapace · plated | `2A5A54 183A36 0A1A18 60A89E A0FF60` | Armazón vivo. Cima de invocador |

---

## 5. Qué falta (honesto)

Lo entregado son **iconos y diseño**, es decir la referencia. Lo que NO está hecho:

1. **Las hojas equipadas** (`_Head`, `_Body`, `_Arms`, `_Legs`, 20 fotogramas cada una). Son el
   grueso del trabajo y tienen que encajar fotograma a fotograma con la animación del jugador.
2. Mientras no existan, cada set debe **seguir apuntando a su slot vanilla**, que es lo que hoy
   evita que el mod se rompa al cargar.

Cuando existan las hojas de un set, se cambian sus tres líneas de `headSlot`/`bodySlot`/`legSlot`
y nada más — la clase base ya está preparada para eso.

**Sobre Calamity:** se estudió su filosofía de tiers y diferenciación de clase. No se copió ni un
sprite, silueta, patrón ni color característico suyo; las formas de aquí se construyen desde
primitivas propias según la tabla de §2 y §3.
