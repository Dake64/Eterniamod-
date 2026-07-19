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

## 2026-07-16 - El arbol de Sangrado por fin alimenta al Rastro Carmesi

- Pedido: "crees que puedas mejorarlo? o variarlo mas?" sobre el arbol del Espadachin.
- CORRECCION DE UN DIAGNOSTICO MIO ERRONEO: primero dije que los 132 nodos del mod eran
  cadenas lineales sin bifurcaciones. Falso. El campo `RequiredPassive` de PassiveNode esta
  MUERTO (nadie lo lee); los prerequisitos se generan proceduralmente en `BuildTiers` /
  `TierSize`, y la rama Bleed sale con forma 1-2-1-1-3-1, o sea CON rombos. Yo grepee el
  campo equivocado.
- Lo que si era cierto: los rombos no son una decision, porque `GetPrerequisites` exige TODOS
  los nodos del nivel anterior. Se compran los 9 igual.
- PROBLEMA REAL Y MAS GRAVE: ninguno de los 9 nodos tocaba el Rastro Carmesi. El arbol se
  escribio antes que la mecanica y nunca se conectaron: todo era dano melee / critico / sangrado
  generico. La mecanica insignia de la subclase no tenia soporte en su propio arbol.
- Nodos nuevos al final de la rama (Bleed pasa de 9 a 12 nodos):
  - Blood Tithe   (coste 4, +12 afinidad): +2 Rastro por golpe sangrante.
  - Open Veins    (coste 4, +13 afinidad): el ingreso pasivo cuenta 2 enemigos mas.
  - Merciless     (coste 5, +14 afinidad): la Ejecucion Carmesi cuesta 10 menos.
- KEYSTONE "Hemorrhagic Frenzy": estaba MUERTO -- solo existia el texto que prometia
  "+20% dano melee, -10% velocidad de ataque", sin una sola linea que lo implementara.
  Ahora existe de verdad, y su precio cambio: -10% de velocidad de ataque significaba MENOS
  golpes y por tanto MENOS Rastro, o sea que el keystone saboteaba el recurso sobre el que se
  monta. El precio ahora lo paga el rematador: +25 al coste de la ejecucion.
- `SwordsmanSkillPlayer.EffectiveCost()` (nuevo): el coste deja de ser una constante. Merciless
  lo baja, el keystone lo sube, y la BARRA lo lee tambien, asi que la linea de disparo grabada
  en el medidor siempre marca el coste real en vez de un 50 fijo que ya no aplica.
- Verificacion: compila 0/0; suite 117/117.
- PENDIENTE (no hecho, decision del owner): que los rombos sean una ELECCION real exigiria
  cambiar `GetPrerequisites` a "cualquiera del nivel anterior", y eso reequilibra los 18
  arboles del mod a la vez, no solo el del Espadachin.

## 2026-07-16 - El Rastro Carmesi se carga con la SANGRE, no con los golpes

- Dos cambios pedidos en playtest, en este orden:
  1. "que solo gane cuando ya este sangrando el enemigo"
  2. "quiero q cada tick del sangrado cargue la barra"
- ANTES: 12 por primera sangre / 6 por mantener. Contradecia el credo del Espadachin
  ("open the wound, then bank the blood") y ademas castigaba la pelea para la que existe la
  tecnica: un jefe solo, siempre ya sangrando, siempre en la tarifa BAJA. Lo mas rentable era
  barrer enemigos nuevos y seguir de largo.
- AHORA hay dos fuentes:
  - Golpe a enemigo QUE YA SANGRA: +6 (+Milestones, x multiplicadores). El golpe que abre la
    herida no banca nada. Aplica igual al golpe directo y al proyectil.
  - INGRESO PASIVO: cada segundo, +1 por cada enemigo que siga sangrando POR TU herida
    (se usa el `BleedOwner` que ya existia, asi que en multijugador la sangre de otro Guerrero
    es su ingreso). La barra por fin se mueve mientras esquivas sin poder pegar.
- TOPE de 8 enemigos contados en el ingreso pasivo. No es paranoia: en el escalon HEMORRAGIA
  una sola pulsacion hace sangrar 28 bloques, asi que en un evento con 30+ enemigos serian
  +30/s -- la barra se llenaria mas rapido que el propio cooldown de la tecnica y quedaria
  reducida a un boton mantenido.
- Efecto neto: contra grupos carga MAS LENTO que antes (era ~5 golpes, ahora ~9); contra jefe
  carga alrededor del doble de rapido gracias al ingreso pasivo. Esa inversion era el objetivo.
- Tests: fijan que ambos hooks salgan antes de conceder si el objetivo no sangraba, que el
  bonus de primera sangre siga eliminado, que solo paguen tus propias heridas, y que el
  ingreso pasivo tenga tope.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - Las 17 subclases tienen MEJORAS REALES por hito, no solo numeros

- Pedido: "hazlo con todos". La escalera anterior solo movia numeros; el Espadachin era el
  unico con una transformacion de verdad. Ahora todas cambian de COMPORTAMIENTO.
- GUERRERO
  - Peleador: la cadena deja de romperse entera. Plantera -> un fallo solo la parte a la mitad;
    Moon Lord -> pierde un tercio y al maximo la ventana deja de correr.
  - Guardian: Plantera -> devuelve el golpe COMPLETO (antes la mitad) y aturde; Moon Lord ->
    el contragolpe supera la herida (x1.5) y alcanza 60% mas lejos.
  - Berserker: el drenaje de vida del Overrage se reduce a la mitad (Plantera) y DESAPARECE
    (Moon Lord). Deja de ser una cuenta atras hacia tu propia muerte.
  - Stunner: el aturdimiento se CONTAGIA a los enemigos que rodean al objetivo, y dura mas.
- MAGO
  - Elementalista: cambiar de elemento cuesta la mitad (Plantera) y luego NADA (Moon Lord).
  - Cursed Mage: la explosion del Burst x1.5 y luego x2, con radio mucho mayor.
  - Nigromante: los muertos reclaman un tercio menos de vida, y luego casi la mitad.
  - Infinity Mage: la mitad del Overflow sobrevive al Overload y el bloqueo se parte por dos;
    en Moon Lord el deposito queda INTACTO y el Overload dura 50% mas.
  - Arcane Bard: la cancion se desvanece a la mitad de velocidad, y en el pico ya no baja.
- RANGER
  - Energy Gunner: el sobrecalentamiento libera al 60% (antes 30%) y en Moon Lord ya no
    bloquea: ventila solo y sigues disparando.
  - Arquero: un Perfect Shot deja media barra en pie (antes ~10), asi que los perfectos
    ENCADENAN; en Moon Lord un Legendary no cuesta nada.
  - Gunner: el doble de gracia antes de perder Momentum, y en Moon Lord la mitad superior
    nunca baja.
  - Virtuoso: las melodias duran 15s y luego 20s, y en Moon Lord una sola melodia da AMBOS
    refranes a la vez.
- SUMMONER
  - Domador: la Ferocidad se enfria a la mitad, y en Moon Lord nunca baja del 50%.
  - Advanced Summoner: la mitad del Command sobrevive a la orden y luego no cuesta nada.
  - Tech Summoner: los drones atacan mas rapido en Overdrive, y el pico de dano casi se dobla.
- El tier 1 queda INTACTO en todos los casos: el balance que se juega hoy no cambia hasta
  Plantera. Se verifico en los tres tests que fijaban el comportamiento base.
- Tres tests quedaron obsoletos porque fijaban el codigo literal (Focus = 10f, el 0.3f del
  overheat, el `ticksSinceHit > 30`). Se actualizaron para exigir el MISMO comportamiento base
  dentro de la expresion por tier, no para relajarlos.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - TODAS las subclases escalan con los hitos, no solo el Espadachin

- Pedido: "puedes hacer lo mismo para las demas subclases que su mecanica igual mejore".
- Se evito el camino obvio (tocar 16 archivos de subclase, uno por uno, sin poder probarlos).
  En su lugar se aprovecho un canal que YA existia: cada subclase expone campos publicos `Acc*`
  que accesorios, armaduras y tonicos alimentan. Un solo sistema central puede alimentarlos
  igual, sin tocar la logica de ninguna subclase.
- `Content/Progression/MechanicTier.cs` (NUEVO): unica fuente de verdad de la escalera.
  Awakened (Muro) -> Deepened (Plantera) -> Perfected (Moon Lord). `Steps()` devuelve 0/1/2.
- `Content/Players/MechanicTierPlayer.cs` (NUEVO): en PostUpdateEquips (antes de que las
  subclases lean sus hooks en PostUpdate, igual que MechanicTonicPlayer) aplica curvas
  COMPARTIDAS para que ninguna subclase escale mas rapido que otra por accidente:
  recursos +20%/+40%, decaimiento a 85%/70%, dano de pago +10%/+20%, ventanas +1s/+2s.
  Con steps=0 hace `return` inmediato: el balance del tier 1 queda EXACTAMENTE igual que antes.
- Cubre 11 subclases (Espadachin, Peleador, Guardian, Elementalista, Nigromante, Energy Gunner,
  Arquero, Gunner, Domador, Advanced Summoner, Tech Summoner).
- PENDIENTE, y hay que decirlo claro: Berserker, Stunner, Yoyo Master, Cursed Mage,
  Infinity Mage, Arcane Bard y Virtuoso NO tienen campos `Acc*`, asi que todavia no escalan.
  Necesitan que se les anada el gancho primero.
- `SwordsmanSkillPlayer.CurrentTier()` ahora delega en `MechanicTier.Current()`: se elimina la
  segunda definicion de "nivel" que podia desincronizarse.
- El Eternal ahora da consejo de crecimiento a TODAS las subclases (antes solo al Espadachin),
  diciendo en que escalon estas y que trae el siguiente hito.
- Tests actualizados: dos aserciones quedaron obsoletas POR DISENO (ya no se exige
  `Array.Empty`, porque ahora todas crecen de verdad). La nueva verificacion es mas fuerte:
  exige que la promesa del Eternal este respaldada por al menos 20 boosts `Acc*` reales.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - El Eternal ahora explica que tu mecanica SIGUE CRECIENDO

- Pedido: "me gustaria que el eternal el npc te diga como mejorar tu mecanica de subclase para
  que tengas la idea que tambien sube de nivel y mejora".
- Estado previo: "Read my soul" con el jugador ya promovido decia que subclase eres, que
  afinidad la sello, y acto seguido "A Soul Reforge would undo it". Un callejon sin salida: se
  lee como "esto ya esta terminado". Nada en el juego dice que la mecanica siga escalando.
- Ahora, al leer un alma promovida, el Eternal anade:
  1. "But {MECANICA} is not finished. Every point you feed {afinidad} deepens it, and what you
     wear can feed it further." -- usa la afinidad REAL que sello tu subclase y su valor actual.
  2. Pistas de los hitos del mundo que mejoran la mecanica, segun lo ya derrotado.
- `AwakeningCeremony.MechanicOf(subclase)` expuesto para nombrar la mecanica SIN duplicar la
  tabla de la ceremonia (una sola fuente de verdad).
- HONESTIDAD: `MechanicGrowthHints` solo devuelve pistas para el Espadachin, que es la unica
  subclase con mejoras por hito realmente implementadas (Plantera -> Hemorragia,
  Moon Lord -> Aniquilacion). Las demas devuelven vacio: prometer crecimiento no implementado
  seria peor que callar. El test verifica justo eso (exige `Array.Empty`).
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - La ceremonia del Muro ahora ENSENA la mecanica, no solo la nombra

- Pedido: "como el jugador se dara cuenta de la mecanica de la subclase... algo te diga su
  mecanica" al recibirla tras el Muro de Carne.
- Estado previo: la ceremonia YA nombraba la mecanica y daba una frase de sabor (Espadachin:
  "CRIMSON TRAIL" / "Open the wound, then bank the blood."). Evocador, pero no ensenaba NADA
  practico: ni que se llena con armas de filo, ni que se gasta con la tecla de skill.
- Ahora `Identity` devuelve una 4a pieza, `how`: una instruccion concreta por subclase (que la
  llena y que la gasta) para las 17. Ej. Espadachin: "Bleed foes with edge weapons to bank
  Crimson Trail, then press Q to execute everything bleeding nearby."
- `{KEY}` se sustituye por la tecla REALMENTE asignada en ese momento. Si esta sin bindear la
  linea lo dice: "the Class Skill key (UNBOUND - set it in Controls)" -- justo cuando importa,
  porque una tecla sin asignar hace que toda la mecanica parezca rota.
- El banner crecio de 152 a 200px de alto y de 260 a 420 ticks (7s) para que de tiempo a LEER,
  con la instruccion envuelta y centrada bajo el credo.
- Ademas se repite en el CHAT (`Main.NewText`): el banner se desvanece y el Muro muere en pleno
  caos, asi que es facilisimo perderse la unica explicacion del juego. En el chat se puede
  volver a leer cuando sea.
- Test: verifica que las 17 subclases tengan las 3 cadenas (mecanica + credo + instruccion),
  que se use la tecla real y no una letra fija, y que la leccion llegue al chat.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - Los avisos de la tecnica pasan a INGLES

- Pedido: "el texto que sale de nadie sangra cambialo al ingles".
- Yo habia metido esos CombatText en espanol por inercia de como conversamos, pero el mod
  entero va en ingles (CRIMSON, EXECUTE!, SET KEY, MAX, PATH LOCKED). Se tradujeron TODOS, no
  solo el mencionado:
  EN ENFRIAMIENTO -> ON COOLDOWN; RASTRO x/50 -> CRIMSON x/50; NADIE SANGRA -> NOTHING BLEEDING;
  NADIE CERCA -> NOTHING IN RANGE; !EJECUCION CARMESI! -> CRIMSON EXECUTION!;
  !ANIQUILACION! xN -> ANNIHILATION xN; ANIQUILADO -> ANNIHILATED.
- Beneficio extra: desaparecen los acentos, que dependian de que la fuente los soportara.
- Test: el smoke test del Crimson Trail ahora rechaza cualquier caracter no-ASCII en
  SwordsmanSkillPlayer, para que no vuelva a colarse texto en espanol.
  NOTA: la primera version listaba las vocales acentuadas literalmente y el propio .ps1 se
  corrompio (se guarda UTF-8 pero Windows PowerShell 5.1 lo lee como ANSI) rompiendo el parser.
  Por eso la regla se expresa como rango ASCII y los tests se mantienen en ASCII puro.

## 2026-07-16 - Las barras de recurso ya no imprimen la tecla

- Pedido: "quitale la q porque alguien mas le puede cambiar la tecla a otra y ya no ser la
  misma".
- La barra del Espadachin ya leia la tecla REAL (`GetAssignedKeys()`), asi que no mentia. Pero
  el pedido destapo algo peor: `SubclassResourceUI` SI tenia la Q hardcodeada para las otras
  subclases ("Q: ROAR", "Q: OVERDRIVE", "Q: OVERCLOCK", "Q: OVERLOAD"). Esas si se quedaban
  desactualizadas al rebindear.
- Decision: NINGUNA barra imprime teclas. El lado derecho muestra siempre TU numero de recurso,
  que ademas es mas util. El estado "listo" ya se comunica de sobra con la linea de disparo
  pulsando, el glow y el color caliente.
- `readyPrompt` (string) se reemplazo por `warning` (string, normalmente null) en
  `DrawFloatingResourceBar`; en `SubclassResourceUI` el out-param paso de `string readyPrompt`
  a `bool hasTechnique` (solo servia como bandera de "tiene tecnica activable"; el nombre de la
  habilidad NUNCA se llegaba a mostrar porque el codigo hacia Split(':')[0]).
- Se conserva un unico caso: si la tecla de skill esta SIN ASIGNAR, la barra dice "SET KEY",
  porque eso es un estado roto de verdad y si no el jugador no tiene forma de enterarse.
- Test nuevo en `SubclassResourceUISourceSmokeTest`: barre Content/UI y prohibe teclas
  hardcodeadas. Cazo una tercera que se me habia pasado. Nota: la primera version de la regex
  daba falso positivo con `"RAGE: {valor}"`; se apreto a 1-2 caracteres, que es lo que
  distingue un nombre de tecla de una etiqueta.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - El sangrado ahora prende en TODO, gusanos mecanicos incluidos

- Pedido: "quiero que el sangrado del Espadachin se le pueda aplicar a todos y tambien al
  gusano mecanico" (The Destroyer).
- Eran DOS causas distintas, y los jefes-gusano caian en las dos a la vez:
  1. INMUNIDAD A BUFFS: vanilla marca bastantes NPCs como inmunes, y `NPC.AddBuff` respeta
     `buffImmune`, asi que el AddBuff se descartaba en silencio.
  2. VIDA COMPARTIDA (realLife): los segmentos de un gusano comparten una sola reserva de vida
     a traves de `npc.realLife`. Al sangrar solo el segmento que golpeabas, el DoT corria contra
     la vida de bookkeeping del segmento y la reserva REAL no se tocaba -> The Destroyer parecia
     totalmente inmune aunque el debuff si estuviera puesto.
- `WarriorBleedPlayer.ApplyBleed` ahora: limpia `buffImmune` del debuff antes de aplicarlo
  (nada resiste la herida de un Guerrero) y ademas hiere al segmento duenno de la vida
  (`Main.npc[target.realLife]`), asi que los gusanos sangran de verdad.
- Tests: fijan las dos causas por separado (realLife + buffImmune) y ademas vigilan que nadie
  reintroduzca una exclusion de jefes dentro del DoT.
- OJO en playtest: contra gusanos ahora pueden sangrar varios segmentos A LA VEZ mas la cabeza;
  si el DoT se siente excesivo contra The Destroyer, ahi esta la razon.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - Radios de la Ejecucion Carmesi subidos (playtest, 3 pasadas, todas al alza)

- Playtest: 8 bloques se sentia apretado, 12 y 14 seguian quedandose cortos. Tres peticiones
  seguidas de mas radio.
- LECCION DE DISENO: mi premisa inicial estaba mal. Baje el radio de 16 a 8 buscando "identidad
  melee" (obligar a meterse), pero el owner NO juega esta tecnica asi: la usa como herramienta
  de control de grupos. El escalon base quedo POR ENCIMA del 16 original.
- Valores finales: FINISHER 20 bloques (320px), HEMORRAGIA 28 (448px), ANIQUILACION 40 (640px).
  Paso parejo de ~x1.4.
- 40 bloques es el techo practico: mas alla los objetivos ya estan fuera de pantalla.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - La Ejecucion Carmesi ahora ESCALA hasta el poder de late game

- Pedido: "quiero que en un futuro ya en late game aplique sangrado a todos y con el ejecutar
  mate a todos dentro de una zona, quiero que se sienta ese poder".
- Correccion del owner: el Espadachin solo existe DESPUES del Muro de Carne, asi que la escalera
  va entera dentro de hardmode (ver decision-log).
- `SwordsmanSkillPlayer` reescrito con 3 escalones:
  1. FINISHER (Muro)     radio 8 bloques. Solo remata lo que tu hiciste sangrar.
  2. HEMORRAGIA (Plantera) radio 12. La ejecucion aplica sangrado a TODA la zona ella misma.
  3. ANIQUILACION (Moon Lord) radio 20. Lo que quede bajo 25%+3%/SoulTier de vida MUERE al
     instante (hasta 40% ascendido al maximo).
- Jefes EXENTOS del instant-kill (`IsBossLike`): reciben la rafaga pero la pelea sigue siendo
  pelea. Sin esto la tecla borraba a Prototype-01 y a todos los jefes vanilla.
- El instant-kill pasa por `SimpleStrikeNPC` con dano suficiente en vez de poner life=0, para
  que loot, credito de kill y sincronizacion multijugador se comporten.
- Feedback por escalon: el golpe de camara crece con el tier, la rafaga de sangre pasa de 32 a
  48 dust al aniquilar, texto "ANIQUILADO" por enemigo y "!ANIQUILACION! xN" sobre el jugador.
  El aviso de fallo tambien cambia: "NADIE SANGRA" en tier 1, "NADIE CERCA" de tier 2 en
  adelante (porque ya no necesitas haberlos hecho sangrar).
- Tests: la escalera, que el radio CREZCA por escalon, que la hemorragia aplique sangrado, y
  sobre todo que el instant-kill exente jefes.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - BUG GRAVE: la tecla de skill NUNCA funciono en NINGUNA subclase

- Sintoma: "le pico a la q y no hace nada" con la barra al 100. Ni sonido ni texto: NADA.
- Se descartaron una por una: keybind (confirmado "Eternia/Class Skill": ["Q"] en el perfil
  Custom activo), build stale (se extrajo el Eternia.dll de dentro del .tmod y SI tenia el
  codigo nuevo), mod duplicado (Eternia2 esta activo pero no contiene nada del Espadachin),
  volumen y ShowItemText (ok).
- CAUSA RAIZ: `EterniaPlayer.PreUpdate()` pone `ActiveSoul = SoulId.None` cada frame, y el
  accesorio de Soul lo re-activa en `UpdateEquips`. Terraria corre `ProcessTriggers` (el input
  de teclas) ENTRE esos dos. Asi que al picar la tecla, `ActiveSoul` valia None, todos los
  `IsActive<Subclase>()` devolvian false y el skill hacia `return` en su PRIMERA linea, antes
  de poder emitir ningun aviso. La barra en cambio se dibuja en la fase de draw, ya con el Soul
  activo -> se veia perfecta al 100. Es el MISMO bug de fase que el de ResetEffects (2026-07-16),
  pero en otro hook, y afectaba a las 12 subclases, no solo al Espadachin.
- Arreglo (central, no parche por subclase): `EterniaPlayer` toma una INSTANTANEA del Soul ya
  confirmado en `PostUpdateEquips` (`InputSoul`, capturada antes de cualquier early return y
  para todos los jugadores) y expone versiones a prueba de fase:
  `EffectiveSoul` (valor vivo si existe, si no el ultimo confirmado), `HasClassSoulNow` y
  `HasAnySoulNow`. Los 42 sitios de gating pasaron a usarlas. Solo difieren del valor crudo
  dentro de la ventana pre-equips donde el crudo esta espuriamente en None, asi que son
  estrictamente mas correctas en todas partes.
- Test nuevo `SoulInputTimingSourceSmokeTest`: exige la instantanea, que se tome en
  PostUpdateEquips antes de cualquier return, y BARRE todo Content/ para que ningun archivo
  vuelva a gatear con el `soul.HasClassSoul` / `soul.ActiveSoul ==` crudo.
- Verificacion: compila 0/0; suite 117/117.

## 2026-07-16 - La Q del Espadachin ya nunca se siente muerta (playtest)

- Sintoma: "le pico a la q y no hace nadaaa". La captura mostraba la tecla SI bindeada y la
  barra con relleno -> el keybind no era el problema.
- Causa real: la tecnica tenia requisitos INVISIBLES (>=50 Rastro Y enemigos ya sangrando en
  rango de 260) y el unico aviso al fallar era un CombatText diminuto, invisible en combate.
  El jugador leia "la tecla no hace nada".
- Decision del owner: MANTENER la tecnica como finisher (solo remata enemigos sangrando), pero
  con aviso grande + sonido. No se convierte en AoE ni en dash.
- `SwordsmanSkillPlayer` reescrito:
  - Cada rama fallida ANUNCIA el motivo con CombatText dramatic (grande) + sonido:
    "EN ENFRIAMIENTO" (MenuTick), "RASTRO x/50" (MenuClose), "NADIE SANGRA" (MenuClose).
  - BUGFIX: antes hacia `TrySpend(50)` ANTES de saber si habia objetivo, asi que un press
    a destiempo QUEMABA 50 de Rastro sin efecto. Ahora `CountBleedingInRange()` valida primero
    y solo gasta si de verdad va a ejecutar.
  - Al ejecutar: "!EJECUCION CARMESI!" + PunchCameraModifier (golpe de camara, 14 frames) +
    estallido de sangre subido de 18 a 32 dust con velocidad y escala mayores, y el "EXECUTE!"
    por enemigo ahora es dramatic.
- `EterniaUI.DrawFloatingResourceBar` acepta `thresholdPercent`: dibuja la LINEA DE DISPARO
  grabada en el recipiente (el 50% = el costo). Tenue mientras no llegas, pulso brillante en
  cuanto la sangre la pasa -> el costo es legible de un vistazo en vez de un numero oculto.
- Tests: `CrimsonTrailSourceSmokeTest` fija las 3 ramas de aviso, el sonido, el texto dramatic
  y (por indice) que la validacion de objetivo ocurra ANTES del TrySpend.
- Verificacion: compila 0/0; suite 116/116. Falta confirmacion en juego.

## 2026-07-16 - Barra del Espadachin -> tema de SANGRE + tecla real en el prompt

- Pedido: darle "mas interfaz como de sangre haciendo alusion" al fantasy de sangrado del
  Espadachin (el medidor segmentado generico no evocaba nada).
- `EterniaUI.DrawFloatingResourceBar` ahora acepta `bloodTheme` (opt-in). Con el, dibuja
  `DrawBloodGauge`: recipiente coagulado, sangre arterial con gradiente vertical (coagulo abajo,
  cuerpo, banda brillante bajo la superficie), superficie humeda que ONDEA (meniscus por columnas),
  borde de nivel que TIEMBLA, y GOTAS que gotean del nivel (cada una con su fase, mas cuando esta
  ready). Todo animado por `Main.GlobalTimeWrappedHourly` (sin RNG por frame -> suave y net-safe).
  Las otras 11 subclases siguen con el medidor segmentado (no pasan `bloodTheme`).
- `CrimsonTrailUI` pasa `bloodTheme: true` y ademas muestra la TECLA REAL asignada
  (`SkillKey.GetAssignedKeys()`) en vez de un "Q" hardcodeado; si esta sin bindear muestra
  "SET KEY". Esto ataca la queja de "pico Q y no hace nada": tModLoader NO fuerza el default de
  una tecla de mod sobre un perfil de controles existente, asi que suele quedar sin asignar, y
  ahora la barra lo dice en pantalla.
- Verificacion: compila 0/0; suite 116/116. Falta feedback visual en juego (captura).

## 2026-07-16 - Barra de recurso -> MEDIDOR segmentado compacto (una sola fila)

- Pedido: que la barra no se vea como "una simple barra que se llena" y que ocupe poco para no
  molestar visualmente.
- `EterniaUI.DrawFloatingResourceBar` rediseñado: de barra plana 118x12 con label arriba + pill
  abajo (3 filas) -> un MEDIDOR SEGMENTADO (13 celdas con separacion) de 78x6 en UNA sola fila:
  `LABEL  [gauge]  valor/Q`. Lee como medidor de energia de un juego de accion, no como progreso.
  Relleno parcial suave entre celdas, borde de fuga brillante, gloss superior, y glow + pulso
  cuando esta ready/full. El label va a la izquierda en gris tenue (no grita); a la derecha el
  valor, o la tecla de accion (ej "Q") pulsando cuando esta listo.
- Mucho mas compacto verticalmente (una fila ~10px vs tres filas ~44px). Sirve a TODAS las
  subclases (comparten el helper).
- Verificacion: compila 0/0; suite 116/116.

## 2026-07-16 - BUG GRAVE (playtest): el recurso de subclase se borraba cada frame

- Reporte (Espadachin): "el Crimson Trail se mantiene en 0, sube poquito al pegar y se resetea".
- CAUSA RAIZ (afectaba a 12 subclases): cada player de recurso hacia `Resource = 0` dentro de
  `ResetEffects` si `!IsActive<Subclass>()`. Pero `ResetEffects` corre AL PRINCIPIO del frame,
  ANTES de que el accesorio de la Soul de clase re-active la Soul (eso pasa en UpdateEquips). Asi
  que `IsActive...()` leia false un instante CADA frame -> borraba el recurso justo despues de
  ganarlo. El recurso ganado a mitad de frame (on-hit) se perdia al frame siguiente.
- Por que no se habia visto: el usuario es el PRIMER playtester que usa a fondo un recurso de
  subclase. El Crimson Trail es el mas obvio porque es puramente acumulativo (no regenera ni
  decae), asi que el borrado era flagrante. Los demas (Momentum, Heat, Focus, Ferocidad, Combo,
  PowerCore, Command, Rage, Charge, Crescendo, Overflow, precisionStacks) tenian el mismo bug pero
  mas disimulado.
- FIX (12 players): mover el borrado "si no es la subclase" de ResetEffects a PostUpdate (hook
  tardio, cuando la Soul ya esta activa y IsActive es fiable). Los resets de campos Acc* se quedan
  en ResetEffects (esos SI deben correr siempre). Players: Crimson(Swordsman), Gunner,
  EnergyShooter, Archer, TechSummoner, AdvancedSummoner, Fighter, BeastTamer, ArcaneBard, Berserker,
  InfinityMage, Stunner, YoyoMaster. Guardian no aplica (no tiene recurso acumulable).
- Tests actualizados (anclaban la estructura buggy): CrimsonTrail (permite PostUpdate que limpia,
  no que regenere) y SubclassRuntimeGating (el gate del recurso se comprueba en PostUpdate, no en
  ResetEffects).
- Verificacion: compila 0/0; suite 116/116.

## 2026-07-16 - PLAYTEST: la barra de recurso de subclase no se veia + Q sin feedback

- Reporte (Espadachin, ya promovido): "no veo ninguna barra ni que la Q haga algo".
- Diagnostico: (1) `DrawFloatingResourceBar` se DESVANECIA a 0 recurso -> un Espadachin nuevo con 0
  Trail no veia NADA, asi que ni sabia que la mecanica existia. (2) La Q (SwordsmanSkillPlayer)
  hacia `return` silencioso si Trail<50 o si no habia enemigos sangrando -> "no hace nada".
  El sangrado y la ganancia de Trail SI funcionan (verificado); era puro problema de UX/descubrimiento.
- Fix 1 (universal): `DrawFloatingResourceBar` tiene un `alwaysShow`. CrimsonTrailUI y
  SubclassResourceUI lo pasan `true` -> la barra se ve SIEMPRE que esa subclase esta activa, aunque
  el recurso este a 0. Arregla la descubribilidad de TODAS las barras (Crimson Trail, Momentum,
  Temperatura, Concentracion, Ferocidad, Power Core...).
- Fix 2: la Q ahora da FEEDBACK: "Crimson Trail 12/50" si te falta recurso, y "No bleeding enemies"
  si disparas sin nada sangrando cerca. Ya no es una tecla muerta.
- Verificacion: compila 0/0; suite 116/116.
- Nota: si tras esto el usuario SIGUE sin ver la barra siendo Espadachin, entonces IsActiveSwordsman
  es false (Soul de Guerrero no equipada) -> siguiente cosa a revisar.

## 2026-07-16 - PLAYTEST: nerf a los stats (daban demasiado poder muy pronto)

- Reporte del usuario (Espadachin nivel 19, solo jefes): 38 Power = +11.4% daño, y se compone con
  armadura + substats -> demasiado poder para lo poco jugado. "Que las stats sean menores".
- Nerf a los valores POR PUNTO (enfocado al dano, dejando supervivencia casi igual):
  - Power: 0.003f -> 0.0015f  (+0.3% -> +0.15% dano por punto). 38 pts: +11.4% -> +5.7%.
  - Precision: 0.15f -> 0.1f  (+0.15% -> +0.1% crit por punto).
  - Vitality DR: 0.001f -> 0.0005f  (+0.1% -> +0.05% reduccion por punto). El +3 HP se mantiene.
  - Agility y Focus sin tocar.
- Se cambio en los TRES sitios que deben cuadrar: `EterniaStatsPlayer` (aplicacion), `StatsUI`
  (descripcion por-punto + `CurrentEffect` totales) y el test `StatsPanelClarity` (que ancla que el
  panel no mienta respecto al codigo). No se toco StatPointsPerLevel (3) ni la curva de XP -- el
  usuario pidio stats menores, no menos puntos; queda como opcion si sigue sintiendo que sube rapido.
- Verificacion: compila 0/0; suite 116/116.

## 2026-07-16 - Iconos de DEBUFF sobre los enemigos

- Pedido: mostrar visualmente cuando un enemigo tiene un debuff, "como el cuadrito de tus buffs".
- `EnemyDebuffDisplayGlobalNPC` (GlobalNPC.PostDraw): dibuja los DEBUFFS activos del enemigo como
  iconos pequeños en fila sobre su cabeza -- el icono real del buff (`TextureAssets.Buff[type]`),
  sobre un cuadrado oscuro con subrayado rojo (= "daño, sobre el enemigo"). Funciona con cualquier
  debuff, vanilla o del mod (bleed, veneno, on-fire...).
- Solo se muestran DEBUFFS (`Main.debuff[type]`), asi los buffs propios de un enemigo no ensucian.
  Se salta critters/friendly/townNPC/muertos. Capado a 6 iconos. Colocado por encima de la insignia
  de rareza que ya dibuja EterniaGlobalNPC, para que no se pisen.
- Verificacion: compila 0/0; suite 116/116 (test nuevo EnemyDebuffDisplay).
- Pendientes/riesgos: la posicion vertical (-30-cell sobre el sprite) es a ojo; en enemigos muy
  altos o con badge de rareza grande podria acercarse. Sin ModConfig para desactivarlo (el usuario
  declino ModConfig).

## 2026-07-16 - Minerales de HARDMODE (3) + su equipo completo

- Pedido: minerales para hardmode, y armas/armaduras/accesorios de subclase pre-HM y HM.
  IMPORTANTE (verificado antes de construir): los accesorios (45), sets de armadura (19 -> ahora 25)
  y armas (209) de subclase YA EXISTEN. No se duplico nada. Lo NUEVO son los minerales HM + su gear.
- 3 minerales de hardmode, continuando la escalera de alma:
  - **Wraithite** (pico Molten 100, look Palladium) -> caverna.
  - **Aetherium** (pico Mythril/Oricalco 150, look Orichalcum) -> caverna profunda.
  - **Nullsteel** (pico Pickaxe Axe 200, look Titanium) -> cerca del inframundo. Tope de la escalera.
  - Barras: 4-5 mena, se funden en `TileID.AdamantiteForge`.
- WORLDGEN CLAVE: los HM NO se generan al crear el mundo -> se siembran cuando cae el MURO DE CARNE
  (`HardmodeOreTriggerNPC.OnKill` -> `EterniaOreGeneration.SeedHardmodeOres`), igual que
  Cobalt/Mythril/Adamantite vanilla. Server/SP only. Guardado con un flag de mundo persistido
  (`HardmodeOresSeeded` en SaveWorldData) para que re-matar el Muro no re-siembre. En MP hace
  `SendData(WorldData)` para reenganchar clientes.
- EQUIPO (mismo lote):
  - 3 sets de armadura class-agnostic (SoulMetalArmor, bonus por tu Soul): Wraithite (39 def, +16%),
    Aetherium (46 def, +22%/+6crit), Nullsteel (58 def, +30%/+10crit). Recetas en Yunque Mithril.
  - 4 armas de Nullsteel (tope), una por clase: Reaver (melee 62, IBleedWeapon con bleed 30 -- el
    mas alto del mod), Repeater (ranged 46), Scepter (magic 52), Lash (summon 40). Ungated.
- Verificacion: compila 0/0; suite 115/115 (test EterniaOres ampliado con toda la parte HM).
  RECORDATORIO hjson: la forma inline `Key: { A: x, B: y }` es MALFORMADA en Hjson (rompio el parse
  una vez); las entradas deben ser multilinea.
- Pendientes/riesgos: balance a ojo; los HM solo aparecen tras romper el Muro en el mundo (los ya
  existentes en hardmode NO los tendran hasta... realmente no apareceran nunca si ya se rompio el
  Muro antes de este update -> son para mundos que entren a HM de ahora en adelante).

## 2026-07-16 - Armas de Revenite: el mineral top pasa a ser un peldaño COMPLETO

- Pedido: "haz una modificacion en la escalera de progresion y agrega armas de eso tambien".
- DECISION IMPORTANTE (y su porque): NO se cambiaron las recetas existentes para exigir minerales
  propios. Seria la "modificacion de escalera" mas obvia, pero ROMPERIA la partida en curso del
  usuario: su mundo NO tiene los minerales (el worldgen solo corre en mundos nuevos), asi que se
  quedaria sin poder craftear nada del mod. Se añaden como VIA PARALELA.
- La modificacion real de la escalera: el tier Revenite (el mineral mas profundo, pico de Demonita)
  pasa de dar solo armadura a ser un PELDAÑO COMPLETO -> armadura + 1 arma POR CLASE, en el rung
  pre-Molten. Asi el mineral top merece la pena para cualquier clase, no solo para los que usan
  armadura.
- 4 armas nuevas (ungated a proposito: son equipo PRE-Muro de Carne, tienen que funcionar antes de
  que exista una subclase):
  - **Revenite Cleaver** (Warrior, melee 25) -- `IBleedWeapon` con BleedChance 22, LA MAS ALTA de
    pre-hardmode (vanilla top es 18 en Death Sickle). Su identidad: pega algo menos que el Molten
    Gutripper (27) pero abre heridas como nada. Recibe el CrimsonSlash automaticamente.
  - **Revenite Longbow** (Ranger, ranged 19).
  - **Revenite Scepter** (Mage, magic 21).
  - **Revenite Lash** (Summoner, summon 16) -- reusa el whip projectile base (ungated).
- Se descarto hacer 12 armas (3 tiers x 4 clases): duplicaria la escalera que ya existe y el mod ya
  sufre de mucho contenido sin balancear. Soulstone/Animite ya tienen la armadura como sink.
- Verificacion: compila 0/0; suite 115/115 (test EterniaOres ampliado: 1 arma por clase, forjada en
  Revenite, y que la del Warrior lleve bleed).
- Pendientes/riesgos: daños a ojo (25/19/21/16) -- se colocaron por debajo del techo de hellstone,
  pero SIN probar. Si el usuario quiere, se pueden añadir tambien armas de Soulstone/Animite.

## 2026-07-16 - SISTEMA DE SUBSTATS (afijos) en armas -- RPG loot

- Pedido: "que las substats que salen al crear un arma haya mas y tenga un sistema". Se confirmo que
  NO habia nada propio (lo que se veia eran los PREFIJOS VANILLA). Se ofrecio A (mas prefijos
  vanilla) vs B (sistema de afijos RPG propio); el usuario eligio B.
- Diseño: cada arma que crafteas/compras/encuentras ROLEA UNA RAREZA, y la rareza decide CUANTAS
  substats trae (no cuan grandes) -> una Legendary es categoricamente mejor, no una Common con
  suerte. Misma escala Common->Nightmare que las rarezas de enemigos, con los MISMOS colores, para
  que la escala se lea igual en todo el mod.
  - Odds: Common 55% | Uncommon 22% | Rare 12% | SuperRare 6% | Legendary 3% | Mythic 1.4% |
    Ancient 0.5% | Nightmare 0.1%.
  - Nº de substats: Common 0 (queda como un arma vanilla normal, sin ruido en el tooltip),
    Uncommon 1, Rare/SuperRare 2, Legendary 3, Mythic 4, Ancient 5, Nightmare 6.
  - 6 afijos: dano %, crit %, penetracion de armadura, velocidad de ataque %, retroceso %,
    velocidad de movimiento % (al empuñar). Sin repetidos en la misma arma.
- Se aplica tambien a armas VANILLA (es un overhaul: una Terra Blade puede salir Legendary).
- Archivos: `Content/Affixes/AffixTable.cs` (enums, odds, rangos, roll, nombres/colores) y
  `Content/Globals/EterniaAffixGlobalItem.cs`.
- Detalles tecnicos que importan:
  - `AppliesToEntity` limita la instancia a ARMAS de verdad (no herramientas/accesorios/apilables)
    -> no paga memoria por cada item del mundo.
  - Rolea en `OnCreated` (craft/compra/bolsas) y `OnSpawn` (drops), con guarda de una sola vez.
  - `Clone` override OBLIGATORIO: la lista de afijos es un tipo referencia; sin el, las copias
    comparten o pierden su tirada.
  - `SaveData`/`LoadData` (persiste con el item) y `NetSend`/`NetReceive` (o un cliente MP veria
    un arma distinta).
  - Efectos por hooks propios del arma: ModifyWeaponDamage/Crit/Knockback, UseSpeedMultiplier, y
    HoldItem para penetracion + movimiento (solo con el arma en mano).
- Verificacion: compila 0/0; suite 115/115 (test nuevo WeaponAffixes).
- Pendientes/riesgos: SIN probar en juego. Rangos y odds a ojo. Los prefijos VANILLA siguen
  existiendo encima (se apilan) -- si se siente excesivo, es lo primero a mirar.

## 2026-07-16 - Primeros MINERALES propios (3, tier temprano) + worldgen

- Objetivo: el mod NO tenia ningun tile/mineral/bioma (cero worldgen; solo sembraba loot en cofres
  vanilla). Y el playtest destapo el hueco: la unica armadura de Guerrero pre-hardmode pide 14-20
  barras de HELLSTONE -> entre el inicio y el Infierno, Eternia no daba NADA propio.
- 3 minerales de alma (lore: la civilizacion antigua que construyo los Prototype), escalonados por
  PODER DE PICO para formar una escalera real:
  - **Soulstone** - subterraneo, comun. Pico 40 (Hierro+). 3 mena -> 1 barra.
  - **Animite** - caverna, medio. Pico 55 (Oro+). 4 mena -> 1 barra.
  - **Revenite** - caverna profunda, raro. Pico 65 (Demonita+). 5 mena -> 1 barra. Ultimo paso
    antes de Hellstone.
- Archivos: Content/Tiles/Ores/ (base `EterniaOreTile` + 3), Content/Items/Placeable/ (base
  `EterniaOreItem` + 3 menas), Content/Items/Materials/ (3 barras), Content/Systems/
  `EterniaOreGeneration.cs` (PassLegacy insertado tras "Shinies", TileRunner por capas, escalado
  por tamaño de mundo). Localizados items + MapEntry de los tiles.
- Arte PLACEHOLDER: cada tile/mena/barra toma prestado el sprite de un mineral HARDMODE vanilla
  (Cobalt/Mythril/Adamantite) A PROPOSITO -> el jugador no los confunde con nada que vea en
  pre-hardmode. Cuando haya arte, quitar el `Texture =>` y poner el .png al lado.
- EQUIPO (mismo lote): 3 sets de armadura, uno por mineral -> ESTO es lo que tapa el hueco.
  - Diseño clave: son CLASS-AGNOSTIC (`SoulMetalArmor`). El metal "responde a tu Soul": el set bonus
    potencia la DamageClass de tu alma activa (reusa `SoulAscensionPlayer.ClassOf`). Asi 3 sets
    cubren las 4 clases en vez de necesitar 12.
  - Soulstone (10 def, look Plata): +6% dano de tu clase.
  - Animite (16 def, look Oro): +10% dano de tu clase, +2 def.
  - Revenite (22 def, look Shadow): +14% dano y +5% crit de tu clase, +4 def. Ultimo antes de Molten.
  - Piezas sueltas dan crit / dano / velocidad genericos. Recetas en Yunque.
- Verificacion: compila 0/0; suite 114/114 (test EterniaOres cubre menas, barras, worldgen Y que
  cada barra tenga equipo -- para no repetir la trampa del SoulAlloy).
- PENDIENTES/RIESGOS:
  - **Solo aparecen en MUNDOS NUEVOS** (el worldgen corre al crear el mundo).
  - El override `Texture => "Terraria/Images/Tiles_X"` es el punto fragil: si el mod fallara al
    cargar, es lo primero a revisar.
  - Balance sin probar (defensas, % de los set bonus, densidad de vetas).

## 2026-07-16 - PRIMER FIX DE PLAYTEST: jefes tempranos imposibles + beam inutil

- Reporte del usuario (jugando de Guerrero contra el Rey Slime): "en rareza legendaria solo le
  quitaba 1 con el proyectil y no podia acercarme". Ademas: invoco 4 slimes -> 2 Legendary, 1
  SuperRare, 1 Mythic; NINGUNO comun ni raro.
- Diagnostico (reproducido con numeros): un Rey Slime Legendary salia nivel ~60 ->
  `defensa = 10*1.5 + 60/3 = 35` y `dano = 40*1.5 + 60/2 = 90`. Terraria resta defensa/2 a CADA
  golpe: el beam (9) -> clampeado a 1; la espada (20) -> 3. Y 90 de dano mata en 2 golpes a un
  personaje temprano. Era matematicamente imposible, no fallo del jugador.
- CAUSA RAIZ: el nivel del enemigo salia SOLO de la rareza (20-100 para jefes), sin ninguna
  relacion con el progreso del mundo. El PRIMER jefe del juego roleaba nivel 50-70.
- FIX 1 - `EterniaGlobalItem.BeamDamageFactor` 0.45f -> 1f: el beam de las espadas de sangrado hace
  el dano COMPLETO. Un beam a la mitad se clampea a 1 en cuanto el objetivo tiene defensa real, o
  sea, la opcion a distancia era inservible. Se mantiene la constante como unico knob de tuning.
- FIX 2 - `BossLevelProgressionScale()`: el nivel roleado de un JEFE se escala por progreso del
  mundo (x0.2 pre-Hardmode, x0.6 Hardmode, x1.0 post-Moon Lord). Un Legendary KS pasa de nivel ~60
  a ~12 -> defensa 19 en vez de 35. Sigue siendo elite (los multiplicadores intactos) pero jugable.
  Solo afecta a jefes; los enemigos normales quedan igual (sus niveles ya eran bajos).
- FIX 3 - distribucion de rareza de JEFES recalibrada. Estaba MAS generosa que la de enemigos
  normales (solo 32% Common; 45% de los jefes salian SuperRare+). Ahora:
  Common 58% | Rare 20% | SuperRare 11% | Legendary 6% | Mythic 3% | Ancient 1.5% | Nightmare 0.5%.
- NO tocado: los multiplicadores de rareza (vida x1-3.2, dano, defensa) siguen -- decision del
  usuario del 2026-07-15, que se mantiene. Ver enmienda en decision-log.
- Verificacion: compila 0/0; suite 113/113 (EnemyRarity test ampliado: escala por progreso +
  distribucion; SwordBeam test actualizado).

## 2026-07-15 - Auditoria de correctness + fixes de multijugador

- Objetivo: revisar el codigo de gameplay (que crecio rapido, validado solo por smoke tests de
  presencia) buscando bugs REALES, no solo presencia.
- Resultado (limpio): netcode del XP correcto; los 30+ campos Acc* TODOS se resetean cada frame
  (sin bug de acumulacion); los tonicos de mecanica se aplican en PostUpdateEquips antes de que las
  subclases lean sus Acc en PostUpdate (correcto y a prueba de orden de carga); IA del jefe sin
  crashes/softlocks; save/load sin perdida de datos (los 2 flags eran falsos positivos: un comentario
  y estado transitorio del Virtuoso); assets placeholder existen; grupos de receta registrados.
- FIX 1 (MP, correctness): la doma (`BeastTamingPlayer`) hacia `target.active = false` en el CLIENTE,
  que no es autoritativo -> la criatura se re-sincronizaba y se podia re-domar (exploit). Ahora en SP
  despawnea directo; en MP envia un paquete nuevo `TameDespawn` y el SERVIDOR la elimina + sincroniza
  (`ETERNIA.cs` HandlePacket). Sin loot (la bestia se une, no dropea).
- FIX 2 (MP, pulido): los jefes Prototype no marcaban `NPC.netUpdate` en cambios bruscos de velocidad
  -> tirones en clientes MP. Añadido en dash, embestida y cambio de fase (no cambia el single-player).
- NO tocado (necesita datos de juego / decision de diseño): la escala de vida por rareza en jefes
  (x1-3.2 aleatoria) y el nº de drones de Prototype-02 (hasta 7). Documentado para el playtest.
- Verificacion: compila 0/0; suite 113/113 (BeastTaming test ampliado con la ruta MP-safe).

## 2026-07-15 - Cuatro frentes: ascension visible + tonicos de mecanica + Prototype-02 + release

Cuatro cosas en un lote (pedido: "haz todo eso de los 4 puntos").

1) ASCENSION VISIBLE: el "Read my soul" del Eternal ahora reporta tu Soul tier (ademas del tooltip
   del Soul de clase que ya lo mostraba).

2) TONICOS DE MECANICA (6): Coolant (Energy Gunner), Adrenaline (Gunner), Focus (Archer), Warcry
   (Fighter), Bloodlust (Beast Tamer), Overdrive (Tech Summoner). Cada uno alimenta los hooks Acc*
   de su subclase. Se aplican en `MechanicTonicPlayer.PostUpdateEquips` (NO en el buff) porque los
   mecanismos leen sus Acc en PostUpdate, que corre despues -> a prueba de orden de carga. Verifique
   la DIRECCION de cada hook (mults de decay/heat/loss < 1, gains > 1) para que sean buff real.
   Buffs = marcadores (MechanicTonicBuff), sin logica. Craft en botella.

3) PROTOTYPE-02 (Hardmode): REFACTOR -> la logica del jefe se movio a `PrototypeBoss` (base abstracta
   con hooks virtuales: BaseLife/damage/defense, ProjectileDamageScale, SpeedScale, ExtraDrones,
   VentsInPhase2, tints, ConfigureLoot). Prototype01 y Prototype02 son subclases finas. 02: HM (24000
   vida base, x1.7 dano de proyectil, x1.15 velocidad, +2 drones, ventea el nucleo ya en fase 2,
   tinte rojizo/nucleo violeta). Nuevos items: RefinedPrototypeCore (mat HM), AwakenedSoulCore
   (invocacion HM, tambien en la tienda del Eternal en HM), SoulforgedGreatsaber (arma HM, mejora del
   Sabre). Añadido al Boss Codex (seccion Hardmode). El test BossPrototype se actualizo para leer la
   base + cubrir ambos jefes.

4) RELEASE POLISH: description.txt y description_workshop.txt reescritas al estado actual (12
   subclases, jefes, codex, pociones, ascension), manteniendo los terminos que exige el test de
   metadata. build.txt intacto (el test fija version = 0.1). Guia nueva: docs/progression-guide.md.

- Verificacion: compila 0/0; suite 113/113 (tests nuevos: MechanicTonics; BossPrototype ampliado).
- Pendientes/riesgos: SIN probar en juego -- todo el balance (tonicos, Prototype-02, ascension) es a
  ojo. Prototype-02 hereda la escala de vida por rareza (mismo tema que 01). Sin arte.

## 2026-07-15 - Prototype-01 fight-ready: el Eternal vende el Corrupted Soul Core

- Objetivo: poder invocar/probar a Prototype-01 sin farmear la chatarra tech de la receta.
- El Eternal (guardian de almas) ahora VENDE el Corrupted Soul Core a cualquiera con Soul de clase,
  por 3 oro (ModifyActiveShop). Tematico (el core es un vaso de alma corrupta) y habilita el
  playtest inmediato. La receta con chatarra tech sigue existiendo como via alternativa.
- Revision de robustez del AI de Prototype01: sin crashes ni softlocks (cada accion termina y
  vuelve a Reposition; las fases no pueden saltarse; TargetClosest + despawn seguros; los spawns de
  proyectil estan guardados por netMode). No se toco el sistema de rareza (regla: no cambiar
  sistemas existentes sin preguntar).
- RIESGO A VIGILAR EN LA PELEA (pendiente de decision del usuario): EterniaGlobalNPC escala la vida
  de TODOS los bosses por rareza (x1 a x3.2) -> Prototype-01 (6000 base) puede salir con ~6000 o
  ~19000 segun la tirada, y cambia en cada intento. Para un boss puede sentirse injusto/inconsistente.
  Opcion propuesta (si el playtest lo confirma): que la rareza en bosses sea SOLO cosmetica + codex,
  con vida/dano deterministas.
- Verificacion: compila 0/0; suite 112/112.

## 2026-07-15 - Sistema de Ascension de Soul (cierra el cabo del Soul Alloy)

- Objetivo: darle uso al Soul Alloy que dropeaba Prototype-01 y estaba "reservado". Cierra el cabo
  del sistema de mejora de Souls.
- Diseno: ASCENSION DE SOUL, perfectamente on-theme (tu poder VIENE de tu Soul, asi que fortalecer
  la Soul es el camino de poder permanente mas alla de niveles y pasivas).
  - `SoulAscensionPlayer`: `SoulTier` (0..MaxTier=5), persistido. Cada tier da a la CLASE ACTIVA
    (segun ActiveSoul -> DamageClass): +4% dano, +2% crit, +8 vida max, +2 defensa. Solo con
    class Soul equipada. SEPARADO del arbol de pasivas -> un Soul Reforge (respec) NO lo toca (el
    test lo verifica: ProgressionService no menciona SoulTier/SoulAscension).
  - `SoulAscension` (consumible): sube 1 tier por uso hasta el cap. CanUseItem exige class Soul y
    headroom. Feedback (CombatText + dust + sonido). Receta ritual en Altar del Demonio (como el
    Reforge): 3 Soul Alloy + 2 Prototype Core + 8 oro. El grind natural es farmear Prototype-01.
  - Visible: linea de tier en el tooltip del Soul de clase (ClassSoulItem) y en el propio item.
- Tooltip de Soul Alloy actualizado ("uses still to come" -> "Used to ascend your own Soul").
- Verificacion: compila 0/0; suite 112/112 (test nuevo SoulAscensionSourceSmokeTest).
- Pendientes/riesgos: SIN probar en juego -- numeros (per-tier, cap 5, coste) a ojo. No hay UI
  dedicada para ver el tier salvo los tooltips; se podria surfacear en el Stats/Soul UI o en el
  "Read my soul" del Eternal mas adelante.

## 2026-07-15 - Boss Codex: tratamiento visual "premium" + fuentes +1 (visto en juego)

- Feedback: subir un poco mas las fuentes pequenas y que el codex se vea "mucho mas visual, no una
  simple caja".
- Fondo/marco: se deja de usar EterniaUI.DrawPanel plano -> fondo con DEGRADADO vertical, BANDA DE
  TITULO con acento (gradiente dorado + linea/glow), ESQUINAS tipo HUD (corner brackets), borde con
  glow suave. Lista y detalle ahora son SUB-PANELES enmarcados (DrawInsetPanel: fondo inset + linea
  de acento + esquinas).
- Detalle: retrato con GLOW de rareza + corner brackets, banda de rareza tras la cabecera, fila
  seleccionada de la lista con DEGRADADO horizontal + barra de acento PULSANTE.
- Fuentes subidas otra vez: tier de lista (0.56->0.6), nombre detalle (1.05->1.12), estado, rate de
  slot (0.52->0.58 con badge de cantidad sobre fondo), header DROPS (0.6->0.66) con divisor.
- Helpers nuevos (locales, sin tocar EterniaUI): DrawVGradient/DrawHGradient/DrawCornerBrackets/
  DrawTitleBand/DrawCodexBackground/DrawInsetPanel. Panel 896x628.
- OJO paleta: el test UIRework prohibe `Color.Black *` crudo en UI -> usar EterniaUI.PanelBackground.
- Verificacion: compila 0/0; suite 111/111.

## 2026-07-15 - Boss Codex: fuentes mas grandes + drops en CUADRICULA de slots (visto en juego)

- Feedback con captura: fuentes pequenas se distinguen mal; los drops se ven "como lista".
- Fuentes subidas: filas de la lista (nombre 0.66->0.78, tier 0.46->0.56), nombre del detalle
  (0.92->1.05), pill de tier (0.46->0.56, mas grande), estado (0.56->0.64), tiles de stats
  (label 0.44->0.54, valor 0.66->0.84, tile 54->62 alto), pestanas (0.56->0.64), header DROPS
  (0.52->0.60).
- Drops REDISENADOS de lista -> CUADRICULA de slots estilo inventario: cada drop es un slot 54px
  con fondo + borde tenido por rareza (brilla al hover), icono del objeto centrado, badge de
  cantidad en la esquina (ej. "8-15") y el RATE debajo del slot coloreado. Rejilla responsive
  (columnas segun ancho); "+N more" si no cabe. Tooltip al hover con nombre + rate + cantidad.
- Verificacion: compila 0/0; suite 111/111.

## 2026-07-15 - Boss Codex: drops reales con rate + fix legibilidad de pestanas (visto en juego)

- Feedback con captura (ya renderiza y se ve bien): (1) el texto de la pestana ACTIVA era negro
  sobre dorado, ilegible; (2) que muestre los drops del boss y el rate de cada cosa.
- Fix pestanas: la pestana activa pasa a fondo oscuro (dorado tenue) + texto BLANCO + barras de
  acento arriba/abajo. Se lee claro y marca la seleccion.
- Drops REALES: en vez del texto curado, el detalle lee la BASE DE DATOS de drops del juego
  (`Main.ItemDropsDB.GetRulesForNPCID` + `ReportDroprates`) -> icono real de cada objeto + su
  probabilidad exacta, deduplicado por item (mejor rate) y ordenado por probabilidad. Funciona para
  TODOS los bosses vanilla Y para Prototype-01 (sus propios drops). Rate formateado (100% / 45% /
  1/9), coloreado por probabilidad; tooltip al hover con rate y cantidad. Cacheado por seleccion.
  Si un boss no reporta drops, cae al texto curado de la entrada.
- Verificacion: compila 0/0; suite 111/111 (test ampliado: ItemDropsDB/ReportDroprates + iconos+rate).
- Riesgos: sin ver la version con drops renderizada; los iconos animados usan GetFrame; la lista se
  corta con "+N more" si no cabe (sin scroll interno en el detalle).

## 2026-07-15 - Boss Codex: rediseno a maestro/detalle interactivo y visual

- Pedido: "haz la interfaz del codex mas interactiva y visual".
- De lista plana -> CODICE MAESTRO/DETALLE (`Content/UI/BossLogUI.cs` reescrito):
  - Barra de PROGRESO general arriba (X / N vencidos).
  - PESTANAS DE FILTRO clicables: All / Pre-HM / Hardmode / Defeated (resetean scroll).
  - Lista izquierda con RETRATOS (boss-head vanilla via NPCID.Sets.BossHeadTextures ->
    TextureAssets.NpcHeadBoss; fallback a icono de alma si no hay slot/no cargado), nombre, tier,
    barra de acento por rareza/tier, y una gema de color = rareza mas alta si vencido. Filas
    CLICABLES que seleccionan; resaltado al hover; scroll por filas.
  - Panel de DETALLE derecho: retrato grande enmarcado, nombre, pill de tier, estado (DEFEATED/
    Not defeated), 3 TILES de stats (Kills / Best Time / Top Rarity coloreada) y los drops.
  - Interaccion via el patron mouseLeftRelease (como EterniaUI.DrawButton); mouseInterface para no
    filtrar clics al mundo.
- Verificacion: compila 0/0; suite 111/111 (BossLogSourceSmokeTest ampliado: filtros, seleccion,
  retratos, detalle, progreso).
- Pendientes/riesgos: SIN ver renderizado (sin captura nueva). Los retratos dependen de que las
  texturas boss-head vanilla esten cargadas -- hay fallback a icono de alma si no. El glyph de
  "vencido" es una gema dibujada a mano (no depende de la fuente).

## 2026-07-15 - Fix: el Boss Codex se salia del panel + pulido (visto en juego)

- Sintoma (reportado con captura, el mod YA carga y corre): las filas del Boss Codex se dibujaban
  POR DEBAJO del panel, fuera de sus limites.
- Causa: el enfoque de "mascaras" (tapar el desborde con rectangulos opacos) no puede ocultar filas
  dibujadas FUERA del panel. Ademas dibujaba filas hasta viewBottom+RowHeight.
- Fix: `Content/UI/BossLogUI.cs` reescrito con scroll POR FILAS ENTERAS -- solo dibuja las filas que
  caben completas dentro del viewport (`VisibleRows`), asi NUNCA hay desborde. Sin scissor (fragil con
  la escala de UI) ni mascaras. La rueda mueve `scrollRow` de a 1.
- Pulido ("mejoralo"): resaltado de fila al pasar el raton, barra de acento por fila (color = rareza si
  vencido / tier si no / oro para el misterio), stats alineadas a la derecha, borde al hover, etiqueta
  "Pre-Hardmode"/"Hardmode" mas clara.
- HALLAZGO IMPORTANTE: tModLoader es DUENO de `en-US.hjson` y lo REESCRIBE al cargar el mod --
  aplano mi bloque `Prototype01: { DisplayName }` a la forma plana `Prototype01.DisplayName: ...`. El
  contenido queda intacto y valido. Los tests de localizacion deben aceptar AMBAS formas (bloque y
  plana); ajustado BossPrototypeSourceSmokeTest. Al editar el hjson a mano, asumir que tML puede
  reformatearlo (las entradas de un solo campo -> plano; multi-campo -> bloque).
- Verificacion: compila 0/0; suite 111/111.

## 2026-07-15 - Prototype-01: el PRIMER boss propio de Eternia (Pre-Hardmode)

- Objetivo: el usuario diseño el boss "Prototype-01" (recipiente fallido para una Soul artificial;
  inspiracion ULTRAKILL / NieR:Automata / MGR): rapido, agresivo, con CAMBIO DE MODULOS DE ARMA y
  3 fases que escalan al exponerse el nucleo. Se pidio una serie (01 pre-HM, 02 HM, Omega post-ML);
  aqui se construye 01 y se deja la arquitectura para los siguientes.

- Boss (`Content/NPCs/Bosses/Prototype01.cs`): ModNPC con maquina de estados server-authoritative.
  - Fases por fraccion de vida: 1 (>70%), 2 (35-70%), 3 (<35%). Al subir de fase: invuln breve,
    rugido, sacudida de camara, burst de dust; la fase 3 ademas ventea el nucleo. Menos defensa y
    mas dano de contacto conforme se rompe.
  - Modulos de arma (elegidos por peso segun fase, sin repetir): SwordDash (embestidas con crescents
    de energia), PlasmaVolley (abanicos de plasma), LanceCharge (telegrafo + embestida larga soltando
    plasma), DroneSpawn (misiles teledirigidos de energia, fase 2+), CoreVent (ondas expansivas +
    estallido radial de plasma, fase 3). Reposition entre ataques.
  - Draw PLACEHOLDER: cuerpo = sprite del Golem vanilla tintado (se enrojece al romperse) + nucleo de
    Soul brillante dibujado sobre el pecho que florece al bajar la vida. Sin crash (no depende de art
    propia). BossHeadTexture = icono de alma placeholder.
  - Integra GRATIS con lo que ya existe: EterniaGlobalNPC le da rareza + escala la vida y OnKill le da
    EXP de boss; BossLogGlobalNPC lo cronometra y lo registra; llena su entrada REAL en el Boss Codex.

- Proyectiles (`Content/Projectiles/Bosses/`): PrototypePlasmaBolt, PrototypeEnergySlash,
  PrototypeDrone (misil homing), PrototypeShockwave (anillo expansivo, solo el borde hace dano) --
  hostiles; + SoulSlash (amistoso, para el arma que dropea). Todos reusan PNGs existentes.

- Items:
  - `CorruptedSoulCore` (invocacion): craftea en yunque con la chatarra tech que YA existe
    (AncientBattery/EnergyCrystal/DamagedCircuit); no invoca dos a la vez.
  - `PrototypeCore` (material, dropea 8-15) y `SoulAlloy` (material de Soul, 2-4; reservado para
    recetas de mejora de Souls futuras).
  - `SoulforgedSabre` (arma unica, dropea 1): espada que lanza un crescent de energia (SoulSlash);
    tambien crafteable desde 10 PrototypeCore.

- BossCodex: refactor a init PEREZOSO (`EnsureBuilt`) para poder referenciar el tipo de NPC modded
  sin romper el static ctor; Prototype-01 insertado en orden pre-HM (antes del Muro de Carne).

- Verificacion: compila 0/0; suite 111/111 (test nuevo BossPrototypeSourceSmokeTest).
- Pendientes/riesgos: SIN probar en juego -- IA, patrones, tiempos, vida (6000 base, luego escalada
  por EterniaGlobalNPC), dano y drops son TODO a ojo. Sin arte (placeholder Golem + nucleo). Sin
  musica propia (usa ambiente). Sin treasure bag de expert (drops normales). Multijugador sin auditar
  (IA server-authoritative + proyectiles server-side, deberia funcionar, pero sin probar). Prototype-02
  (HM) y Omega (post-ML) quedan como extension futura.

## 2026-07-15 - Pociones de clase / consumibles (8) + Boss Codex (registro de bosses)

- Objetivo (pedido del usuario): "mete pociones de clase y consumibles y aparte una lista de
  bosses con toda la info: que dropea, la rareza mas alta que te ha salido, cuanto has tardado".

- POCIONES (8 consumibles):
  - 4 de clase base (pre-HM): Warrior's / Arcanist's / Hunter's / Packleader's Brew -> +10-12%
    dano + crit + velocidad de la clase correspondiente (melee/magia/ranged/invocacion). Sirven a
    las 3 subclases de cada alma porque tocan la DamageClass base.
  - 2 tonicos universales: Battle Tonic (pre-HM, +6% a las 4 clases) y Grand Battle Tonic (HM,
    +10% a las 4 + crit).
  - Eternal Feast (comida, 20 min: regen + un poco de todo) y Warding Tonic (+8 def, +5% DR, para
    peleas de boss).
  - Base `EterniaConsumable` (item) + `EterniaPotionBuff` (buff). Todo via hooks VANILLA seguros;
    `Item.potion` = false (sin Potion Sickness). Recetas en Botella/Cazuela con hierbas vanilla;
    el Grand Battle sube desde el Battle con almas de Might/Sight/Fright.
  - Archivos: Content/Items/Consumables/*.cs (9), Content/Buffs/*Buff.cs (9). Localizados los 8.

- BOSS CODEX (registro de bosses, tecla N):
  - Reutiliza el sistema de rareza que YA existe (`EterniaGlobalNPC.rarity`, Common->Nightmare):
    esa es "la rareza mas alta que te ha salido" por boss.
  - `BossCodex`: 18 bosses vanilla en orden de progresion + 1 teaser BLOQUEADO ("The Eternal, not
    yet risen") que ata el cabo suelto del jefe final que la gear endgame ya craftea. Cada entrada
    lista TODAS las piezas del combate; una kill solo cuenta cuando cae la ULTIMA pieza (gusanos,
    Gemelos, Moon Lord).
  - `BossLogPlayer`: por cada boss guarda kills, mejor tiempo (el mas rapido) y rareza mas alta.
    Persistido con el personaje.
  - `BossLogGlobalNPC`: cronometra spawn->muerte con `Main.GameUpdateCount`, lee la rareza del
    boss y lo registra en `Main.LocalPlayer`. Nunca en servidor dedicado.
  - `BossLogUI`: panel scrolleable (rueda del raton), fila por boss con tier, kills, mejor tiempo
    y etiqueta de rareza coloreada; tooltip al pasar el raton con los drops y el detalle. Integrado
    con `EterniaUI.MajorPanel.Bosses` (cierra los otros paneles al abrirse).
  - Keybind "Toggle Boss Codex" (N) en EterniaKeybinds + localizado.

- Verificacion: compila 0/0; suite 110/110 (2 tests nuevos: ClassPotions y BossLog).
- Pendientes/riesgos: SIN probar en juego -- todos los numeros (dano, recetas, duraciones) son a
  ojo. Multijugador es hueco conocido (el Boss Codex es por jugador y de alcance singleplayer;
  cada cliente registra lo que ve, sin netcode para repartir credito). Cronometraje puede quedar
  corto en bosses con piezas que aparecen a mitad de pelea (cabeza libre del Golem). Sin arte
  (items y buffs reusan iconos de alma placeholder).

## 2026-07-14 - La Ceremonia del Despertar (animacion al recibir la subclase)

- Objetivo (pedido del usuario): "me gustaria que hubiera una animacion de cuando derrotas al
  muro de carne y recibes la subclase".
- Correccion importante: la promocion NO era silenciosa como creia -- `PromotionRewardPlayer` ya
  mostraba un banner "PROMOTION!" + sonido + arma de regalo (via `PromotionBannerUI`). Lo que
  FALTABA era la animacion/build-up y, sobre todo, explicar la MECANICA que acabas de recibir
  (te entregan un sistema de juego entero y nada en el juego lo explica).
- Diseño: un unico momento ceremonial en 3 tiempos, sin duplicar banners:
  1. GATHER (~90 ticks): luz de alma (dust) converge hacia ti y la pantalla se oscurece.
  2. BURST (~12 ticks): flash blanco, rugido, onda de dust y sacudida de camara (PunchCameraModifier).
  3. BANNER: el burst levanta `PromotionBannerUI`, ahora enriquecido -- nombra tu subclase Y su
     mecanica insignia (COMBO, CONCENTRATION, MOMENTUM, FEROCITY, THE POWER CORE, ...) con una
     frase-lema, todo en el color de acento de esa subclase.
- Reconciliacion (evitar doble banner/sonido): antes `PromotionRewardPlayer` disparaba el banner
  y el sonido directamente. Ahora dispara `AwakeningCeremony.Begin(subclass)` (sigue regalando el
  arma). La ceremonia toca sus propios sonidos y levanta el banner en el burst -> un solo momento,
  disparado UNA vez por la lista guardada `awardedPromotions` (mismo frame, mismo ModPlayer -> sin
  carrera entre sistemas). `SubclassPlayer` queda puro (no conoce UI).
- `PromotionBannerUI`: `Show(sub)` -> `Prepare(sub, mecanica, lema, color)` + `Fire()`. La ceremonia
  hace Prepare en Begin y Fire en el burst. Banner mas alto, con linea de mecanica en blanco.
- Archivos: `Content/Systems/AwakeningCeremony.cs` (nuevo), `Content/UI/PromotionBannerUI.cs`,
  `Content/Players/PromotionRewardPlayer.cs`. La tabla Identity() cubre las 18 subclases (12 v1 +
  las 6 heredadas: Berserker, Stunner, Yoyo Master, Infinity Mage, Arcane Bard, Virtuoso).
- Verificacion: compila 0/0; suite 108/108 (test nuevo AwakeningCeremonySourceSmokeTest; actualizado
  PromotionBannerSourceSmokeTest para el nuevo flujo -- sigue garantizando "banner, no spam de chat").
- Pendientes/riesgos: SIN probar en juego (timing del build-up y la intensidad del flash/oscurecido
  son a ojo). Sin arte (dust vanilla + placeholder). En saves viejos ya promovidos sin
  `awardedPromotions`, la ceremonia se reproduce una vez al cargar (aceptable, no se repite).

## 2026-07-14 - El ETERNAL: tienda por alma + "leer tu alma" (revela tu promocion)

- Contexto: el Eternal era el UNICO NPC del mod, de las poquisimas cosas que YA tienen arte
  (EternalNPC.png)... y lo unico que hacia era regalarte un Alma Vacia. No vendia nada.
- LO MAS IMPORTANTE -- "Read my soul": la promocion se decide por "la afinidad mas alta gana",
  y NADA en el juego te decia hacia que subclase ibas. Era informacion OCULTA que solo
  descubrias al matar al Muro de Carne, con los puntos ya gastados y sin vuelta atras. Ahora
  el Eternal (que lee almas) te dice tu afinidad dominante, su valor, y en que subclase
  despertarias SI el Muro cayera hoy. Si ya estas promovido, te dice que afinidad lo sello.
- `SubclassPlayer` expone PredictedSubclass() / DominantAffinityName() / DominantAffinityValue(),
  reutilizando su PROPIA logica de promocion (el NPC no la re-deriva -> no puede desincronizarse).
  PredictedSubclass llama a ResolveSubclass con hardMode=true a proposito, para poder adivinar
  antes del Muro.
- TIENDA DINAMICA POR ALMA: el stock se reconstruye segun el Alma que llevas (AddShops +
  ModifyActiveShop). Warrior -> Training Gauntlet/Shield + Soul of Steel; Mage -> Apprentice
  Wand + Soul of Ember; Ranger -> Training Bow/Pistol + Soul of the Hunt; Summoner -> Training
  Whip + Soul of the Pack. En Hardmode ademas vende la SOUL REFORGE por 25 de oro (la salida
  cara de un build, ahora tambien comprable).
- Dialogo reactivo: cambia si no tienes alma / tienes Alma Vacia / no has gastado puntos / ya
  te promoviste.
- Verificacion: build 0/0; suite PASS=107 (nuevo `tests/EternalNPCSourceSmokeTest.ps1`).
  Ajustado `EternalNPCSystemSourceSmokeTest`: comprobaba la propiedad correcta (que el chat use
  la POSESION del objeto Alma, no el alma activa) pero por el NOMBRE EXACTO de una variable
  local; ahora lo comprueba por comportamiento dentro de GetChat/SetChatButtons, asi sobrevive
  a una reescritura sin perder la proteccion.
- Pendientes/riesgos: SIN probar en juego.

## 2026-07-14 - SOUL REFORGE: el respec (la unica salida de un build)

- Problema encontrado auditando el codigo: `UnlockedPassives` solo tenia UNA operacion en todo
  el mod -- `.Add()`. No habia Remove ni Clear. Una pasiva desbloqueada lo era PARA SIEMPRE.
  Y como la promocion es "la afinidad mas alta gana", un jugador que invertia en Bow y luego
  descubria que queria el Momentum del Gunner quedaba encerrado en Archer para toda la vida de
  ese personaje. En un mod cuya gracia son DOCE subclases, eso castiga justo lo que queremos
  premiar: experimentar.
- `ProgressionService.ResetPassives(player)` (nuevo, junto al TryUnlockPassive): borra las
  pasivas, pone a CERO las 18 afinidades, y devuelve EXACTAMENTE los puntos que costaron
  (lee node.Cost de cada una, incluidos los keystones -- que tambien son nodos del registro).
- Cero fontaneria extra para la re-promocion: `SubclassPlayer` ya recalcula CurrentSubclass
  cada frame desde el snapshot de afinidades, asi que al ponerlas a cero vuelves a tu clase
  base sola, y al re-gastar los puntos te vuelves a promover sola.
- `Content/Items/Souls/SoulReforge.cs`: consumible, y DELIBERADAMENTE CARO (decision del
  usuario: "que la decision pese"). Ritual en un ALTAR DEMONIACO: 1 Empty Soul + 30 Almas de
  Luz + 30 Almas de Noche + 20 barras de oro/platino. Se niega a usarse si no hay nada que
  devolver (no se desperdicia), y el tooltip avisa cuantas pasivas vas a borrar.
- Verificacion: build 0/0; suite PASS=106 (nuevo `tests/SoulReforgeSourceSmokeTest.ps1`, que
  fija que devuelva el coste REAL y que borre LAS 18 afinidades -- si quedase una sin limpiar,
  el reforge te dejaria atrapado en la subclase vieja).
- Pendientes/riesgos: SIN probar en juego.

## 2026-07-14 - ARMADURAS: 16 sets (48 piezas) cuyo BONUS DE SET dobla la mecanica

- Hueco detectado al auditar el mod: NO habia ni una sola armadura. El poder en Terraria es
  un triangulo (arma + armadura + accesorio) y el mod solo tenia dos lados: llegabas a
  Hardmode con tus armas y accesorios... y la armadura de vanilla.
- SOLUCION AL BLOQUEO DE ARTE (lo importante): una armadura en tModLoader necesita texturas
  de equipo que se dibujan SOBRE EL JUGADOR (<Name>_Head.png etc.) o el mod NI SIQUIERA
  CARGA. Como no hay arte, cada pieza TOMA PRESTADA la apariencia de una armadura vanilla
  apuntando head/body/legSlot a un `ArmorIDs.*` vanilla. El jugador se ve con ese set vanilla
  -- se lee como armadura de verdad, no como un sprite roto. Cuando haya arte, se cambian 3
  lineas por set y nada mas. (El test lo obliga: sin ArmorIDs, falla.)
- Estructura: UN ARCHIVO POR SET (3 piezas + el bonus juntos) en vez de 48 sueltos.
- 4 sets de CLASE BASE (pre-HM), utiles antes de la promocion:
  Steelbound (Warrior, look Molten) / Emberweave (Mage, Jungle) / Hunter's Garb (Ranger,
  Necro) / Packmaster (Summoner, Bee).
- 12 sets de SUBCLASE (HM). El BONUS DE SET reutiliza los MISMOS hooks Acc* que los
  accesorios, asi que armadura y accesorios COMPONEN en vez de pelearse:
  - Ironchain (Fighter, Titanium): +10 max Combo, ventana +2s
  - Hemocarnage (Swordsman, Adamantite): Rastro Carmesi +60%
  - Aegis Bulwark (Escudero, Turtle): aura +35% daño, +20% radio (y el aura escala con
    Defensa, asi que la defensa enorme del set se retroalimenta)
  - Prismatic (Elementalist, Hallowed): cambio de elemento -20 ticks Y Surge +3s (te da las
    DOS mitades que los accesorios te obligan a elegir)
  - Blightweave (Cursed Mage, Spectre): +55 Corrupcion base
  - Lich Regalia (Necromancer, Nebula): Vida Reservada -32% y drenaje de mana -32%
  - Reactor Suit (Energy Gunner, Vortex): -25% calor por disparo, +60% enfriamiento
  - Hawkeye Garb (Archer, Chlorophyte): Concentracion +55%, Disparo Perfecto +20%
  - Gunslinger Rig (Gunner, Shroomite): Momentum +50% gana / -50% decae
  - Alphahide (Beast Tamer, Spooky): Ferocidad +60%, Frenesi +20% daño
  - Exoframe (Tech Summoner, Stardust): Power Core +60%, escudo de Overdrive +20
  - Legion Regalia (Advanced Summoner, Tiki): +2 esbirros (y la legion cuesta MEDIO slot, o
    sea +4 cuerpos), roster lleno vale mas, Command +50%
- Detalle: el set pre-HM del Ranger ya toca la Concentracion, porque TODO Ranger la aprende
  antes del Muro de Carne (no hay que esperar a ser Archer).
- Verificacion: build 0/0; suite PASS=105 (nuevo `tests/ArmorSourceSmokeTest.ps1`, que fija
  las 3 piezas por set, el bonus, y que cada set doble su mecanica).
- Pendientes/riesgos: SIN probar en juego. Los sets se VEN como armadura vanilla hasta que
  haya arte propio (por diseño, ver arriba). Sin commit aun.

## 2026-07-14 - 44 ACCESORIOS que doblan la mecanica de cada subclase (pre-HM + HM)

- Peticion del usuario: accesorios pre-HM y HM para las subclases, con VARIEDAD, y mezclando
  accesorios vanilla existentes en las recetas.
- Principio de diseño: NO son "+10% daño". Cada accesorio mete la mano en la MECANICA FIRMA
  de su subclase (Temperatura, Concentracion, Momentum, Ferocidad, Power Core, Legion,
  Combo, Rastro Carmesi, Aura, Afinidad, Corrupcion, Vida Reservada) y la dobla -- igual que
  hacen los arboles de pasivas. Se craftean combinando un accesorio VANILLA con materiales
  de Eternia, asi que se leen como mejoras de gear que ya conoces.
- FONTANERIA (nuevo patron): cada ModPlayer de subclase expone campos `Acc*` publicos que se
  RESETEAN cada frame en ResetEffects; los accesorios los re-aplican en UpdateAccessory.
  (Si no se reseteasen, el efecto se acumularia infinito -- el test lo verifica.)
  El CursedMagePlayer ya tenia el patron (BaseCorruption "from equipped curse accessories").
- 44 accesorios (`Content/Items/Accessories/`, base `EterniaAccessory` + 4 bases de clase):
  - 4 de CLASE BASE (pre-HM): Soul of Steel / Ember / the Hunt / the Pack -- algo que ponerte
    antes de saber que subclase seras.
  - 36 de SUBCLASE (1 pre-HM + 2 HM por cada una de las 12). Las dos opciones de HM son
    DIVERGENTES a proposito, no un upgrade lineal. Ejemplos:
    - Energy Gunner: Heat Sink Array (vive en la zona critica) vs Refractory Plating (el
      sobrecalentamiento YA NO TE HACE DAÑO, pero enfrias mas lento).
    - Archer: Falcon Eye (mas Disparos Perfectos, mas fuertes) vs Steady Nerve (recibir un
      golpe casi no te quita Concentracion).
    - Gunner: Hot Streak Rig (llegas a Dead Eye volando) vs Dead Eye Regulator (te quedas
      dentro mucho mas tiempo).
    - Cursed Mage: Blighted Heart (+70 Corrupcion base... -40 de vida maxima).
  - 4 CAPSTONES post-Moon Lord (Eternal Bulwark/Grimoire/Sight/Crown): alimentan las TRES
    mecanicas de su clase a la vez, asi que sirven sea cual sea tu subclase.
- Recetas: usan accesorios vanilla (Shackle, Aglet, Feral Claws, Obsidian Shield, Cross
  Necklace, Magic Quiver, Sniper Scope, Power Glove, Papyrus Scarab, Necromantic Scroll,
  Mana Flower, Philosopher's Stone, los Emblemas de clase, Avenger Emblem) + materiales del
  mod. El accesorio pre-HM se CONSUME al craftear su version HM.
- Verificacion: build 0/0; suite PASS=104 (nuevo `tests/AccessorySourceSmokeTest.ps1`, que
  fija que cada accesorio doble su mecanica y que TODOS los hooks se reseteen cada frame).
- Pendientes/riesgos: SIN probar en juego (los numeros son mi criterio). Sin commit aun.

## 2026-07-14 - Advanced Summoner: rama Fusion + LEGION que se obtiene FUSIONANDO

- Cierra las 12 subclases v1. Obtencion elegida por el usuario: el Advanced Summoner no doma
  (Beast Tamer) ni ensambla piezas (Tech Summoner) -- FUSIONA SUS PROPIAS INVOCACIONES.
  (Nota: una version previa de esta subclase se construyo con recetas de materiales y luego
  con un sistema de componentes; ambas se revirtieron. Esta es la definitiva.)
- CADENA DE FUSION (la staff anterior SE CONSUME):
  - Wisp Lantern = la SEMILLA: la unica crafteada de materiales crudos (madera+antorchas),
    repetible. Todo lo demas sale de ella.
  - 2x Wisp            -> Spirit Banner (Spirit Soldier)
  - Soldier + Wisp     -> Construct Core (Arcane Construct)   [mejor pre-HM]
  - Construct + Soldier-> Fusion Matrix (Fusion Golem)        [primer HM, rompe armadura]
  - Golem + Construct  -> Sentinel Beacon (Arc Sentinel)      [post-mechs, electrifica]
  - Sentinel + Golem   -> Singularity Core (Singularity Wraith) [final, mitad de defensa]
  Tension de diseño: la mecanica LEGION quiere el roster LLENO, pero fusionar te come staves
  -> hay que volver a craftear semillas. El coste es real.
- Arsenal/mecanica de la legion: `LegionMinion` (base) -- cada legionario cuesta MEDIO slot
  (fieldas el doble) y tiene SINERGIA DE ENJAMBRE (+6% daño por cada otro legionario vivo,
  tope +60%). `LegionMinionPlayer` (conteo por frame) + `LegionMinionBuff` (buff compartido).
- Rama Fusion cableada en `AdvancedSummonerPlayer`: Perfect Fusion (Command +50% mas rapido),
  Ultimate Fusion (bonus de Legion 15%->24%), Synchronized Assault / Transcendent Fusion /
  Overdrive / Singularity (Overclock mas rapido, mas fuerte, mas largo, +4 al cap), keystone
  Living Swarm (el bonus de Legion paga hasta 130% de roster).
- 2 latigos HM (gated a Advanced Summoner): Fusion Lash (52) -> Legion Lash (100, endgame).
  Subido el tag del FusionWhipProjectile existente (3 -> 12).
- UI: `Content/UI/AdvancedSummonerUI.cs` (Command + OVERCLOCK + "LEGION n/cap").
- Verificacion: build 0/0; suite verde (nuevo `tests/AdvancedSummonerSourceSmokeTest.ps1`,
  que fija la cadena de fusion: la semilla NO se fusiona y cada escalon SI consume el anterior).
- Pendientes/riesgos: SIN probar en juego. Sin commit aun.

## 2026-07-14 - Tech Summoner (Summoner): rama Tech + flota de DRONES (se FABRICAN)

- Contexto: la mecanica POWER CORE/OVERDRIVE ya existia (bateria que se carga sola, mas
  rapido con drones desplegados; a tope, Overdrive = pico de daño + escudo). Faltaba moldear
  la rama Tech y darle arsenal. Decision del usuario: sus invocaciones se CRAFTEAN (esta era
  la instruccion que por error aplique al Advanced Summoner y luego revertí).
- Rama Tech cableada en `Content/Players/TechSummonerPlayer.cs`: Tech Protocol (el core carga
  +50% mas rapido), War Machine (un core cargado da 12%->20% de daño), Overclocked Core
  (Overdrive dura +3s), Nanoswarm (Overdrive da +40% en vez de +25%), keystone Combat
  Protocol (escudo de Overdrive 15->28 def). Combat AI y Targeting Array se leen en el DRON
  (cadencia -30%, alcance +40%). Descripciones de los nodos Tech reformadas.
- Identidad mecanica de los drones: son minions A DISTANCIA -- mantienen formacion alrededor
  del ingeniero y DISPARAN (MinionContactDamage=false), a diferencia de las bestias del Beast
  Tamer que embisten. `Content/Projectiles/Summoner/DroneMinion.cs` (base, con hook
  ConfigureShot para que cada dron moldee su disparo) + `DroneLaser.cs` (daño de invocacion,
  lleva el debuff de su dron) + `DroneMinionPlayer`/`DroneMinionBuff` (buff compartido).
- 6 drones: Scout, Repeater (cadencia alta), Tesla (electrifica), Plasma (quema), Rail
  (francotirador, perfora una linea), Omega (apex: cadencia brutal + perforacion).
- CRAFTEO EN 2 ETAPAS (la identidad de la subclase -- el ingeniero CONSTRUYE su flota):
  1) Forjas 3 componentes (`Content/Items/Materials/`, base `DroneComponent`): Drone Chassis
     (armazon), Servo Core (motor), Command Chip (cerebro) -- hechos de los materiales tech
     que YA dropean en el mundo (EnergeticFragment/DamagedCircuit/EnergyCrystal), reusando la
     economia de materiales del Energy Gunner.
  2) Ensamblas el dron: cada uno de los 6 kits exige Chassis + Servo + Chip (+ material de
     tier). Scout (1+1+1) -> ... -> Omega (6+5+5 + 15 Ancient Battery + Luminite).
- 2 latigos HM (gated a Tech Summoner): Circuit Lash (52) -> Omega Lash (100, endgame).
  Subido el tag del TechWhipProjectile existente (5 -> 12) + OmegaWhipProjectile nuevo.
- UI: `Content/UI/TechSummonerUI.cs` (barra de Power Core + OVERDRIVE + "FLEET n/cap").
- Verificacion: build 0/0; suite PASS=102 (nuevo `tests/TechSummonerSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego (daños/recetas/IA sin tunear). CON ESTO SE CIERRAN
  LAS 12 SUBCLASES v1 salvo el Advanced Summoner, que quedo revertido y pendiente. Sin commit.

## 2026-07-14 - Beast Tamer: arsenal de LATIGOS (pre-HM + HM) + re-escalonado de bestias

- Hueco detectado por el usuario: el Beast Tamer solo tenia 2 latigos, AMBOS Hardmode. El
  latigo es EL arma del invocador (las bestias vienen de la doma), asi que faltaba toda la
  progresion de armas pre-Hardmode.
- Base `Content/Items/Weapons/Summoner/SummonerWhip.cs` (nueva, quita boilerplate; los 2
  latigos existentes se refactorizaron a ella).
- 8 latigos en progresion:
  - PRE-HM (abiertos a CUALQUIER Summoner -- el Beast Tamer aun no existe): Training Whip
    (ya existia) -> Leather Lash (15) -> Thorn Lash (23, veneno) -> Bloodfang Whip (31,
    sangrado) -> Molten Lash (40, quema; el mejor pre-HM, para el Muro de Carne).
  - HM (gated a Beast Tamer): Beastcaller Whip (52) -> Feral Lash (68, sangrado) ->
    Savage Lash (84, sangrado+veneno) -> Alpha's Lash (110, endgame).
  - 6 proyectiles de latigo nuevos + subido el tag de los 2 existentes (BeastWhip 4->12,
    AlphaWhip 12->26; eran demasiado bajos para su tier de HM).
- Re-escalonado de las 6 bestias: con la doma, cada bestia se obtiene cuando aparece SU
  criatura, no cuando la crafteas. Granite Golem y Giant Flying Fox son pre-HM -> Boar (12)
  y Raptor (18) son las bestias tempranas. Wolf/Unicorn/Werewolf/Wyvern son HM -> Wolf (28,
  ya no es la "starter"), Bear (38), Sabertooth (48), Wyvern (62).
- Localizacion: 6 latigos + 6 proyectiles en en-US.hjson.
- Verificacion: build 0/0; suite PASS=101 (BeastTamerSourceSmokeTest ahora fija la
  progresion de latigos y que los pre-HM NO esten gated a subclase).
- Pendientes/riesgos: SIN probar en juego (daÃ±os/recetas sin tunear). Sin commit aun.

## 2026-07-14 - Beast Tamer: obtencion por DOMA (reemplaza el craft de las staves)

- Decision (usuario): el Beast Tamer NO craftea sus bestias; las DOMA. Debilitar una
  criatura a poca vida y golpearla con el latigo tiene probabilidad de domarla, lo que
  desbloquea esa bestia (y te entrega su staff). Ver decision-log.
- `Content/Taming/BeastTameRegistry.cs` (nuevo): mapa criatura->bestia (fuente unica de
  verdad, editable). Cada una de mis 6 bestias se doma de una criatura vanilla:
  Wolf<-Wolf, Boar<-Granite Golem/Flyer, Raptor<-Giant Flying Fox, Bear<-Unicorn,
  Sabertooth<-Werewolf, Wyvern<-Wyvern. (Vanilla no tiene raptor/sabertooth literales; son
  el fit tematico mas cercano -- editar SourceNPCs libremente.)
- `Content/Players/BeastTamingPlayer.cs` (nuevo): coleccion TamedBeasts (guardada) + la
  mecanica en OnHitNPCWithProj: cualquier Summoner, golpe de latigo (IsAWhip), criatura
  VIVA a <=15% de vida -> 40% de doma. Exito: desbloquea + te da la staff (si no la tienes)
  + la criatura se une (desaparece sin loot) + "tamed!". Funciona pre-promocion (con el
  TrainingWhip) para coleccionar antes de ser Beast Tamer.
- Quitadas las recetas de craft de las 6 staves (ahora solo por doma). Los latigos siguen
  crafteables (son el instrumento de doma).
- Mejoras de la doma (segunda pasada, "mejoralo si quieres"):
  - Probabilidad ESCALADA: en vez de 40% fijo, va de 30% (justo bajo el umbral 15%) a 90%
    (casi muerta) via MathHelper.Lerp -> debilitarla mas premia la precision y frustra menos.
  - El arbol Beast tambien moldea la doma: Wild Bond da +15% de probabilidad.
  - Descubribilidad: golpear con el latigo a una criatura domable pero SANA muestra la pista
    "weaken it to tame!" (con cooldown, sin spam).
  - Feedback en el mundo: `Content/Globals/TameableGlobalNPC.cs` (nuevo) hace BRILLAR con
    chispas doradas a una criatura domable en cuanto entra en la ventana de doma (solo para
    un Summoner cercano) -> se ve de un vistazo "azota esto ya".
  - Recompensa: domar una bestia NUEVA da +25 de Ferocidad si ya eres Beast Tamer.
  - UI: la barra del Beast Tamer muestra el progreso "PACK N/6".
  - Tooltips de las 6 staves ahora dicen de que criatura se doma cada bestia.
- Verificacion: build 0/0; suite PASS=101 (nuevo `tests/BeastTamingSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego. El mapa criatura->bestia es un placeholder
  tematico; el usuario puede reasignar SourceNPCs. La doma despawnea al bicho de forma
  simple (ok en un jugador; en multiplayer faltaria sync). Sin commit aun.

## 2026-07-14 - Beast Tamer (Summoner): pasivas de Ferocidad + arsenal de bestias

- Contexto: las 3 subclases del Summoner YA tenian mecanica (Beast Tamer=Ferocidad,
  Advanced=Legion, Tech=Power Core). El usuario eligio empezar por Beast Tamer y que yo
  diseÃ±ara pasivas + arsenal (mantiene la mecanica actual de Ferocidad).
- Ferocidad (ya existia, ahora moldeada por la rama Beast): los golpes de esbirros suben
  Ferocidad -> escala daÃ±o de invocacion; a tope, la tecla de habilidad lanza PRIMAL ROAR
  (frenesi ~6s). Cableado de la rama Beast en `Content/Players/BeastTamerPlayer.cs`:
  Wild Bond (sube mas rapido), Feral Roar (decae mas lento), Primal Instinct (mas daÃ±o por
  Ferocidad), Savage Alpha (frenesi mas largo), Alpha Beast (mas knockback en el roar),
  Bloodhound (+crit de invocacion en frenesi), keystone Apex Alpha (+daÃ±o en frenesi).
  Descripciones de los 8 nodos Beast reformadas; efecto base intacto en EterniaStatsPlayer.
- Arsenal de bestias (nuevo). Infra de minion estandar (slots), distinta del Nigromante
  (que usa Vida Reservada): `Content/Players/BeastMinionPlayer.cs` (flag) +
  `Content/Buffs/BeastMinionBuff.cs` (UN buff compartido para toda la manada mixta) +
  `Content/Projectiles/Summoner/BeastMinion.cs` (base: minion de invocacion, IA
  persecucion, daÃ±o por contacto, refresco mutuo con el buff).
  - 6 bestias: Wolf, Boar (slow), Raptor (bleed), Bear, Sabertooth (bleed+lifesteal),
    Wyvern (apex). 6 staves (`BeastStaff` base): WolfFangTotem -> BoarhideTotem ->
    RaptorTalon -> UrsineTotem -> SabertoothFang -> Wyrmcaller (pre-HM -> post-ML).
  - 2 latigos (tag): BeastcallerWhip (BeastWhipProjectile) + Alpha's Lash (nuevo
    `AlphaWhipProjectile`, tag 12, mas alcance). Usan el sistema de whips existente.
- UI: `Content/UI/BeastTamerUI.cs` (barra de Ferocidad + estado PRIMAL ROAR).
- Localizacion: 8 items + 6 minions + AlphaWhipProjectile + BeastMinionBuff en en-US.hjson.
- Verificacion: build 0/0; suite PASS=100 (nuevo `tests/BeastTamerSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego (balance/recetas/IA de minion sin tunear). Faltan
  las otras 2 subclases del Summoner (Advanced Summoner, Tech Summoner). Sin commit aun.

## 2026-07-14 - Gunner (Ranger): mecanica de MOMENTUM (pistolero de fuego rapido)

- Decision de diseÃ±o (usuario): el Gunner NO cambia de postura y NO es el francotirador
  (esa fantasia se reserva para una futura clase de francotirador). Es el pistolero de
  FUEGO RAPIDO. RediseÃ±o del viejo Sweet Spot/Dead Eye (minijuego de timing de una barra
  oscilante) a una mecanica de racha de fuego sostenido. Ver decision-log.
- Mecanica (solo armas de balas, AmmoID.Bullet):
  - `Content/Players/GunnerPlayer.cs` (reescrito): barra Momentum 0-100 que SUBE al acertar
    balazos (+4, Quick Trigger +6) y BAJA rapido si dejas de acertar (ticksSinceHit>30 ->
    -1.5/tick, Rapid Chamber -1.0). Escalones 40-69 (+8% daÃ±o/cadencia) y 70-99 (+15% +5%
    crit). Al llegar a 100 se enciende DEAD EYE (sobremarcha ~5s: +30% daÃ±o, +20-28% crit,
    +25% cadencia; Momentum congelado; al terminar vuelve a 0). Metralletas la llenan por
    volumen; rifles lentos apenas la mueven (por diseÃ±o -> francotirador = clase futura).
  - `Content/Globals/GunnerGlobalProjectile.cs` (nuevo): durante Dead Eye las balas perforan
    (+1, Full Auto +2) e ignoran armadura (+8, Armor Piercing +20).
  - `Content/UI/GunnerUI.cs` + `SoulUI.cs`: barra de Momentum coloreada por escalon, marcas
    40/70, y estado DEAD EYE (reemplaza el marcador oscilante del Sweet Spot).
  - `Content/Passives/PassiveRegistry.cs`: los 8 nodos de la rama Gun re-describen su rol de
    Momentum (Quick Trigger=gana mas rapido, Rapid Chamber=decae mas lento, Deadshot=Dead
    Eye dura mas, Hair Trigger=mas cadencia, Bullet Storm=mas daÃ±o, Full Auto=perfora en
    Dead Eye, Armor Piercing=ignora armadura en Dead Eye, Executioner=mas crit en Dead Eye).
    Efecto base intacto en EterniaStatsPlayer.
- Keystone Trigger Discipline: matar durante Dead Eye lo extiende (+60 ticks) -> mantener
  la matanza. Leido via HasKeystone.
- Distinto del Energy Gunner (gestiona un TECHO: no sobrecalentar) y del Archer (paciencia):
  aqui la tension es MANTENER la racha viva (no parar, no fallar).
- Verificacion: build 0/0; suite PASS=98 (nuevo `tests/GunnerMomentumSourceSmokeTest.ps1`;
  el gating estricto !IsActiveGunner() ya cumplia SubclassRuntimeGating, sin cambios).
- Pendientes/riesgos: SIN probar en juego. Sin commit aun.

## 2026-07-14 - Gunner Pass 2: arsenal de 15 armas de balas (fuego rapido)

- Objetivo (usuario pidio que yo diseÃ±ara la progresion): 15 armas de balas de fuego rapido
  (pistolas, SMGs, metralletas, autoguns, minigun endgame), SIN francotiradores.
- Arquitectura: `Content/Items/Weapons/Ranger/GunnerGun.cs` (base : ModItem, munition Bullet
  -> Momentum aplica) con BulletsPerShot/SpreadDegrees (base Shoot dispara N balas con
  dispersion y las etiqueta) + virtual OnBulletHit. `Content/Globals/
  GunnerGunGlobalProjectile.cs` despacha OnBulletHit al arma que disparo.
- Pre-HM (5): Scrap Pistol (inicio), Rattler SMG (spray rapido), Twin Derringer (2 balas),
  Chatterbox (bullet hose), Hellfire Repeater (quema, mejor para el Muro de Carne).
- HM (10): Mythril Ripper, Autoloader (cadencia extrema), Twin Vortex SMG (2 balas),
  Suppressor (mas Momentum al golpear -> Dead Eye mas rapido), Frost Sweeper (Chilled+
  Frostburn), Chlorophyte Autogun, Spectral Shredder, Vortex Ripper, Storm Minigun (drop
  Martian Saucer), Singularity Barrage (minigun endgame, useTime 3).
- Obtencion: recetas + `Content/Globals/GunnerObtentionGlobalNPC.cs` (Storm Minigun del
  Martian Saucer). `GunnerPlayer.AddMomentum` para la sinergia del Suppressor.
- Localizacion: 15 armas en en-US.hjson (parsea con Hjson.dll).
- Verificacion: build 0/0; suite PASS=99 (nuevo `tests/GunnerArsenalSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego (balance/recetas sin tunear). Sin commit aun.

## 2026-07-14 - Archer (Ranger): mecanica de CONCENTRACION + sniper

- Objetivo (spec): el Archer premia posicionamiento y paciencia. RediseÃ±o del viejo Focus
  (que se ganaba al golpear y decaia quieto -- al reves del spec) a una barra de
  Concentracion que se CARGA al no disparar (mas rapido quieto) y se GASTA al disparar.
- Archivos principales:
  - `Content/Players/ArcherPlayer.cs` (reescrito): Focus 0-100; regen por
    ticksSinceFire (quieto x2.5 vs moviendo; Eagle Vision +50%); un enemigo a <12 bloques
    bloquea la regen de un Archer promovido; recibir daÃ±o resta 30; tiers 31-60 (+5%
    daÃ±o/vel) y 61-100 (+10% +5% crit). Un Ranger base tambien "aprende" la mecanica
    (IsRangerLearning: solo tiers, sin Disparo Perfecto).
  - `Content/Globals/ArcherGlobalProjectile.cs` (nuevo): bono por distancia a la flecha
    (10 bloques 0% -> 50+ bloques +40%), Sniper (Marksman x1.5), Predator (Volley: +25% a
    enemigos con vida llena), y perforacion/ignora-defensa/FX dorado de Perfectos/Legendarios.
  - `Content/UI/ArcherFocusUI.cs` + `SoulUI.cs`: quitado "PRESS Q" (el Perfect es
    automatico al disparar con la barra llena); "PERFECT READY".
  - `Content/Passives/PassiveRegistry.cs`: los 8 nodos de la rama Bow re-describen su rol
    de Concentracion (Eagle Vision/Hunter Instinct/Piercing Arrow/Deadeye/Sniper/Weak
    Point/Predator/Strong Draw), leidos en ArcherPlayer/ArcherGlobalProjectile. Efecto base
    de cada nodo intacto en EterniaStatsPlayer (cobertura/tree-depth sin tocar).
- Disparo Perfecto (Archer promovido, barra llena): +35% daÃ±o (Deadeye -> +55%), +25% crit,
  +40% vel de proyectil, ignora defensa, mas knockback; la barra vuelve a ~10.
- Hawkeye (keystone): cada 8 Disparos Perfectos, el siguiente es Legendary (+80% daÃ±o,
  +50% crit, atraviesa todo, gran ignora-defensa, estela/explosion dorada).
- La mecanica potencia CUALQUIER arco (arrow ammo), asi que el Archer es jugable ya con
  arcos vanilla/mod -- no requiere arsenal propio como el Energy Gunner.
- Verificacion: build 0/0; suite PASS=96 (nuevo
  `tests/ArcherConcentrationSourceSmokeTest.ps1`; ajustada la lista de metodos del Archer
  en SubclassRuntimeGatingSourceSmokeTest a los hooks reales).
- Pendientes/riesgos: SIN probar en juego. Sin commit aun.

## 2026-07-14 - Archer Pass 2: arsenal de 16 arcos + obtencion

- Objetivo (spec): 16 arcos con identidad propia, muchos ligados al Disparo Perfecto, que
  acompaÃ±an la progresion. Potencian la precision, no la cadencia.
- Arquitectura: `Content/Items/Weapons/Ranger/ArcherBow.cs` (base : ModItem) dispara la
  flecha del jugador (munition Arrow, asi la Concentracion aplica) y la etiqueta con el arco
  + estado de Disparo Perfecto; virtuals OnArrowSpawn/ModifyArrowHit/OnArrowHit/UpdateArrow.
  `Content/Globals/ArcherBowGlobalProjectile.cs` (InstancePerEntity) despacha esos hooks al
  arco que disparo -> cada efecto vive en su propio archivo. Convive con
  ArcherGlobalProjectile (distancia/Perfecto) sin conflicto.
- Arcos pre-HM (8): Hunter's Branch (inicio), Woodland Longbow (Merchant tras EoC, +daÃ±o por
  distancia), Falcon Bow (cada 3er disparo), Wind Whisper (Sky Islands, perfora+acelera),
  Jungle Stinger (veneno), Crimson/Corrupt Hunter (lifesteal / Ichor), Molten Longbow
  (Perfecto explota + On Fire).
- Arcos HM (8): Mythril Precision (Perfecto ignora defensa), Frostpiercer (Ice Mimic,
  Perfecto congela), Chlorophyte Sniper (homing suave), Temple Judgement (Golem, Perfecto
  atraviesa todo + onda), Eclipse Recurve (Mothron, ramp por perforacion), Dragonbone (Duke
  Fishron, dragon espectral), Celestial Horizon (fragmentos, lluvia de estrellas), Eternal
  Horizon (ultimate: cada Perfecto -> Constellation Arrow).
- Proyectiles nuevos (`Content/Projectiles/Ranger/`): SpectralDragon, StarShard,
  ConstellationArrow (perfora infinito, ignora 50% defensa via DefenseEffectiveness, escala
  con distancia recorrida, explota). Reutilizan textura LightningBoltProjectile.
- Obtencion: `Content/Globals/ArcherObtentionGlobalNPC.cs` (drops Ice Mimic/Golem/Mothron/
  Duke Fishron + tienda del Merchant tras EoC) + `Content/Systems/ArcherChestLoot.cs`
  (Wind Whisper en cofres de Sky Island).
- Localizacion: 16 arcos + 3 proyectiles en en-US.hjson (parsea con Hjson.dll).
- Verificacion: build 0/0; suite PASS=97 (nuevo `tests/ArcherArsenalSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego (balance/recetas sin tunear). Eternal Horizon es
  crafteable con Luminite como PLACEHOLDER hasta que exista el boss final de Eternia (el
  spec lo pide como drop de ese boss). Sin commit aun.

## 2026-07-13 - Energy Gunner (Ranger): mecanica de TEMPERATURA por zonas

- Objetivo (spec): el Energy Gunner premia mantener sus armas en la zona 70-99% de
  temperatura. RediseÃ±o del viejo Heat lineal + Overdrive a un sistema de zonas 0-100%.
- Archivos principales:
  - `Content/Players/EnergyShooterPlayer.cs` (reescrito): `Heat`, `Overheated`, `MaxHeat`
    (100 + 30 con Reactor Core), `HeatPercent`, `Zone` (0 stable / 1 hot / 2 critical).
  - `Content/Globals/EnergyGunnerGlobalProjectile.cs` (nuevo): Conductores de Plasma.
  - `Content/UI/EnergyHeatUI.cs`: barra por zonas (cyan/naranja/rojo), marcas 40%/70%,
    etiqueta STABLE/HOT/CRITICAL/OVERHEAT + %.
  - `Content/UI/SoulUI.cs`: specialty y linea de estado del Energy Gunner en % + zona.
  - `Content/Passives/PassiveRegistry.cs`: 8 nodos Energy re-describen su rol termico.
- Mecanica:
  - Disparar sube calor (`CanUseItem`). Zonas: 0-40 sin bonus; 40-70 +10% daÃ±o/cadencia;
    70-99 +20% daÃ±o/cadencia +10% crit; 100 = OVERHEAT.
  - Overheat: bloquea el arma, true damage al jugador (30, o 15 con Fusion Cannon),
    Burning, y sigue bloqueada hasta enfriar a <=30% de MaxHeat.
  - Passive shaping: Reactor Core (+techo), Energy Core (-calor/disparo), Ion Surge
    (enfria mas rapido), Fusion Cannon (menos true damage), Overload (explosion de
    emergencia al fundirse), Overcharge (daÃ±o escala con calor), Particle Beam (+crit en
    critico), Plasma Reactor (Conductores de Plasma: perfora+quema+crece en critico).
- Verificacion: build 0/0; suite PASS=95 (nuevo
  `tests/EnergyGunnerTemperatureSourceSmokeTest.ps1`).
- Pendientes/riesgos: SIN probar en juego. Sin commit aun.

## 2026-07-13 - Energy Gunner Pass 2: arsenal completo (spec ampliado) + obtencion

- Objetivo (spec ampliado del usuario): 9 prototipos pre-HM (SIN temperatura) + 21 armas de
  energia HM (CON temperatura) en 5 tramos de progresion, y el requisito clave de que "cada
  arma genere una cantidad distinta de calor" y se SIENTA diferente (cadena, guiado,
  perforacion, plasma/AoE, precision, haz).
- Marcador `Content/Items/Weapons/Ranger/IEnergyWeapon.cs` (ahora declara `HeatPerShot`):
  SOLO las armas que lo implementan generan calor. `EnergyShooterPlayer` exige
  `IsEnergyWeapon(item)` en sus 4 hooks de item y lee el `HeatPerShot` del arma equipada ->
  cada arma sube el calor distinto; prototipos y armas vanilla quedan FRIAS (honra el spec).
- Base `EnergyWeapon.cs` (: ModItem, IEnergyWeapon): sin municion (reactor integrado) +
  knobs virtuales (HeatPerShot, EnergyProjectile, Pierce, ShotCount, SpreadDegrees, ProjScale)
  con un `Shoot` compartido, para que cada arma se configure con pocas lineas.
- Proyectiles (`Content/Projectiles/Ranger/`, todos ranged, reusan textura
  LightningBoltProjectile con tintes): `EnergyBolt` (recto), `EnergyPlasmaBolt` (explota/AoE),
  `EnergyChainBolt` (cadena a enemigos cercanos), `EnergyHomingBolt` (guiado).
- Materiales (5, `Content/Items/Materials/`): EnergeticFragment, DamagedCircuit,
  EnergyCrystal, PlasmaCore, AncientBattery (base `TechMaterial`).
- Prototipos pre-HM (9, balas, NO IEnergyWeapon): Early = CoilPistol, SparkCarbine,
  ElectromagneticRifle; Mid = ExperimentalPlasmaShotgun, TeslaRepeater, PulseLauncher;
  Late = UnstablePlasmaCannon, ExperimentalPhotonRifle, RailgunPrototype.
- Arsenal energia HM (21, IEnergyWeapon, calor y rareza escalan por tramo):
  - Inicio HM (calor 3-5): LaserRifle, PlasmaCarbine, BeamPistol, PulseSMG.
  - Post-mechs (5-8): RailgunMkII, TeslaCannon(cadena), IonRifle(guiado), PlasmaShotgun(AoE x5).
  - Post-Plantera (6-9): PhotonBlaster, PulseAccelerator, ArcCannon(cadena), QuantumRifle(guiado).
  - Post-Golem (11-14): NovaCannon(AoE), FusionRifle(perfora), NeutronLauncher(guiado), HyperBeam(haz).
  - Post-Moon Lord (16-22): AntimatterCannon(AoE), SingularityRifle, StellarRailgun(precision),
    CelestialBeam(haz), EterniaReactorCannon (firma, doble plasma).
- Obtencion: `Content/Globals/EnergyDropsGlobalNPC.cs` (materiales de Meteor Heads / Dungeon /
  Martian Madness; PhotonBlaster de Martian Saucer; EterniaReactorCannon de Moon Lord) +
  `Content/Systems/EnergyChestLoot.cs` (AncientBattery en cofres del Dungeon). El resto se
  craftea (Anvil pre-HM, MythrilAnvil HM, LunarCraftingStation en el tramo Moon Lord).
- Localizacion: 35 items + EnergyBolt en en-US.hjson (multi-linea; parsea con Hjson.dll).
- Verificacion: build 0/0; suite PASS=95; hjson parsea OK. Test ampliado cubre calor
  por-arma (HeatPerShot), los 9 prototipos y que ninguno implemente IEnergyWeapon.
- Desbloqueo: NO se usa un item "Energy Soul" (el usuario confirmo que fue error del spec).
  La subclase se activa como todas: tras el Muro de Carne, el Ranger se promueve a la
  subclase con la afinidad de pasivas mas alta. Ya cubierto por ClassPromotionRules.
- Pendientes/riesgos: SIN probar en juego (balance de daÃ±o/calor/recetas sin tunear en vivo;
  "haz continuo" es EMULADO con bolts rapidos muy perforantes, no un rayo sostenido real).
  Sin commit aun.

## 2026-07-10 - Fix: en-US.hjson malformado (el mod no cargaba) + test de parseo

- Sintoma (usuario): "The localization file en-US.hjson is malformed and failed to load /
  Name is not closed" -> los mods se desactivan al arrancar.
- Causa: los 10 Grimorios se aÃ±adieron en formato inline de una linea
  (`WarGrimoire: { DisplayName: X, Tooltip: "" }`). Hjson lee los strings SIN comillas
  hasta el fin de linea, asi que la coma y el resto se tragaban en el valor y rompian el
  parseo (las llaves dejaban de cuadrar).
- Fix: reescritos los 10 en formato multilinea (DisplayName/Tooltip en lineas propias),
  como el resto del archivo.
- Regresion: nuevo `tests/LocalizationParsesSourceSmokeTest.ps1` que PARSEA en-US.hjson
  con la libreria Hjson real de tModLoader (no solo grep de claves) -> este tipo de error
  ahora falla en la suite, no al cargar el mod. (Skippea si no encuentra la instalacion.)
- Verificacion: hjson parsea OK; build 0/0; suite PASS=94.

## 2026-07-10 - Nigromante Fase 4: Grimorios Especializados (el "arma principal")

- Objetivo (spec): las criaturas dominadas deciden QUE invocas; el Grimorio equipado
  decide COMO se comporta TODO el ejercito. Misma coleccion, estilos totalmente distintos.
- Sistema: `Content/Necromancy/ISpecializedGrimoire.cs` (modificadores: SummonDamageMult,
  ReserveMult, ManaMult, MoveSpeedMult, SizeMult, DefenseDelta, Lifesteal, OnHitDebuff,
  BossEchoMult, CommonMult) + base `Content/Items/Weapons/Summoner/SpecializedGrimoire.cs`
  (logica de invocar/ciclar movida aqui desde GrimoireOfDeath + virtuals neutrales).
- Cableo: `NecromancerPlayer` rastrea el Grimorio activo (el sostenido, o el ultimo) y
  aplica ReserveMult (vida reservada), ManaMult (drenaje) y DefenseDelta. `BaseNecroMinion`
  lo lee: ModifyHitNPC escala el daÃ±o (con split BossEcho/Common para el Rey Muerto),
  OnHitNPC aplica lifesteal + debuff, y en AI aplica SizeMult (escala) y MoveSpeedMult.
  Ecos de jefe marcados con IsBossEcho.
- 10 GRIMORIOS (crafteables, uno equipado a la vez):
  Pre-HM: Basic (neutral), War (+25% daÃ±o, -8 def), Sacrifice (-40% vida reservada, +50%
  mana), Arcane (-40% mana, -15% daÃ±o), Commander (+40% velocidad, -10% daÃ±o).
  HM: Lich (lifesteal, +30% reserva), Plague (Venom on-hit, -10% daÃ±o), Swarm (-50%
  reserva, mas pequeÃ±os/debiles), Titan (+60% reserva/tamaÃ±o/daÃ±o), Void (+20% daÃ±o +
  Shadowflame, +50% mana), Dead King (ecos de jefe +100%, comunes -50%).
- Tests: `NecromancerGrimoireVariantsSourceSmokeTest` (contrato, base, 10 grimorios con su
  modificador firma + crafteo, player aplica reserva/mana/def, minion aplica daÃ±o/lifesteal/
  debuff/boss). Ajustados NecromancerGrimoire/Rank (logica ahora en la base). Suite PASS=93.
- Verificacion: build 0/0; suite PASS=93. SIN probar in-game.
- Con esto el Nigromante esta completo al 100% del spec (nucleo + coleccion + rangos/paginas
  + jefes + grimorios especializados). Pendiente: bestiario ampliado; arte real; balance.

## 2026-07-10 - El Nigromante pasa a ser subclase de MAGO (no Invocador)

- Correccion (usuario): la clase base del Nigromante es Mago. Reemplaza al Mago del
  Infinito en la v1 (via la afinidad Infinity). Encaja mejor: el drenaje de mana es una
  mecanica muy de Mago.
- Cambios:
  - `NecromancerPlayer.IsActiveNecromancer` -> gate en SoulId.Mage.
  - `ClassPromotionRules`: Necromancer pasa de Summoner a Mage. Afinidad Infinity ->
    "Necromancer" (antes Infinity Mage, que queda sin construir/oculto). Se quita
    Necromancer de las promociones de Summoner; la afinidad Shadow del Summoner queda
    sin subclase (cae al fallback).
  - Clase de daÃ±o MAGIC (para que un Mago no dispare la penalizacion de Soul con el
    Grimorio, y para que los no-muertos escalen con la magia): GrimoireOfDeath,
    BeginnerNecromancyBook y BaseNecroMinion pasan de Summon a Magic.
  - `SubclassEffectsPlayer`: Necromancer bajo Mago (+10% magia, +20 mana max para
    sostener el ejercito que drena mana).
  - Presentacion: SoulUI (especialidad/bono) y MilestonePlayer (milestone -> daÃ±o magico)
    actualizados; se quitan los bonos de invocador (minion slots/summon) obsoletos.
  - Visibilidad v1: Mago mantiene Elemental/Curse/Infinity (Infinity = ruta Necromancer);
    Summoner cambia Shadow -> Fusion (Beast/Fusion/Tech).
- Tests actualizados: PromotionRules (Infinity->Necromancer; Shadow ya no promueve),
  SubclassRuntimeGating (Necromancer requiere alma de Mago), v1 visibility (Shadow<->
  Fusion). Suite PASS=92.
- Verificacion: build 0/0; suite PASS=92. SIN probar in-game.

## 2026-07-10 - Nigromante: visibilidad de la coleccion en el HUD

- Objetivo: cerrar el bucle de coleccion con feedback -> el jugador ve cuanto lleva del
  Grimorio y a que alma apuntar.
- `NecromancerCollectionPlayer`: DominatedCount/TotalCount + NextTarget() (la criatura
  no-dominada elegible por rango mas cercana a desbloquearse, por ratio de muertes).
- `NecromancerUI`: panel mas alto + linea de coleccion -> "Dominated X/Y - Next: <Zombie>
  70/100 kills" (o "All in reach dominated"). Complementa la linea de rango/paginas/
  drenaje ya existente.
- Tests: sin nuevos (helpers cubiertos indirectamente). Suite PASS=92.
- Verificacion: build 0/0; suite PASS=92. SIN probar in-game.
- Con esto el Nigromante queda con feedback completo del bucle. Pendiente (contenido/
  polish): mas criaturas + almas de jefe (aÃ±adir al registro); panel interactivo del
  Grimorio para gestionar el loadout manualmente; balance in-game.

## 2026-07-10 - Nigromante Fase 3: Rangos del Grimorio + Paginas Activas + almas de jefe

- Objetivo (spec): el Grimorio evoluciona con la progresion. Su RANGO gatea que
  categorias de criaturas puedes dominar/invocar, y un limite de PAGINAS ACTIVAS obliga
  a preparar un loadout.
- Sistemas nuevos:
  - `Content/Necromancy/GrimoireRank.cs`: rango I..IV segun progreso (Pre-HM / post-WoF /
    post-Plantera / post-Moon Lord) y paginas activas 3/5/7/10. Nombres:
    Apprentice/Adept/Master/Supreme Lich.
  - `GrimoireEntry.RequiredRank` (1..4): las criaturas de categorias altas exigen mas
    rango. `EligibleEntries()` filtra por rango: solo puedes ciclar/invocar lo que tu
    rango permite.
  - PAGINAS ACTIVAS (`NecromancerCollectionPlayer.ActivePages`, persistente): invocar una
    criatura la vuelve pagina activa; si el loadout esta lleno se expulsa la mas antigua
    y sus no-muertos crumblean (EnsureActive + DespawnCreature). Asi no puedes tener
    todas las invocaciones a la vez -> estrategia.
  - ALMAS DE JEFE (categoria "Boss"): matar al jefe N veces -> su Boss Soul -> registrar
    -> version menor. Fase 3: King Slime -> Guardian Slime, Eye of Cthulhu -> Eye Spirit
    (almas KingSlimeSoul/EyeOfCthulhuSoul + minions GuardianSlimeMinion/EyeSpiritMinion).
- UI: NecromancerUI muestra el rango del Grimorio + paginas activas (X/Max) + drenaje.
- Tests: `NecromancerRankSourceSmokeTest` (rango por progreso, paginas 3/5/7/10,
  RequiredRank, boss echoes, ActivePages con eviction + persistencia + elegibilidad por
  rango; el Grimorio usa elegibles y activa al invocar). Suite PASS=92. +hjson.
- Verificacion: build 0/0; suite PASS=92. SIN probar in-game.
- Con esto el Nigromante tiene su bucle completo (nucleo + coleccion + rangos/paginas +
  jefes). Pendiente: mas criaturas/almas de jefe (contenido que se aÃ±ade al registro);
  UI del Grimorio (coleccion visible / gestion de loadout); balance in-game.

## 2026-07-10 - Nigromante Fase 2: Grimorio de la Muerte + sistema de Almas

- Objetivo (spec): el Nigromante progresa coleccionando almas, no armas. Dominar una
  criatura = matar N enemigos -> obtener su <Enemy> Soul -> registrarla en el Grimorio.
- Sistema (data-driven):
  - `Content/Necromancy/GrimoireRegistry.cs`: lista de criaturas (Id, enemigos fuente,
    umbral de muertes, alma, minion, categoria). Fase 2 comunes: Skeleton (default
    desbloqueado), Zombie (100 muertes), Demon Eye (75).
  - `Content/Players/NecromancerCollectionPlayer.cs`: Kills (dict NPC->muertes) +
    Unlocked (criaturas dominadas) + SelectedIndex, persistente (Save/Load).
  - `Content/Globals/NecromancerKillGlobalNPC.cs`: OnKill cuenta muertes; ModifyNPCLoot
    aÃ±ade el drop del alma via `SoulDropCondition` (1/5 una vez alcanzado el umbral y si
    no la has dominado aun).
  - Almas: `Content/Items/Souls/{EnemySoul(base),ZombieSoul,DemonEyeSoul}.cs`. Usar el
    alma como Nigromante desbloquea la criatura (consumible).
  - `Content/Items/Weapons/Summoner/GrimoireOfDeath.cs`: clic izq invoca la criatura
    seleccionada (si ReservedLifeFraction<0.9), clic der cicla la seleccion. Crafteable
    (Book + 15 Bone).
- Refactor: la IA de los no-muertos (follow/chase + validez + fade de mana) se movio a
  BaseNecroMinion (subclases solo definen stats). Nuevos minions ZombieMinion (reserva
  20%, lento) y DemonEyeMinion (reserva 10%, rapido).
- Tests: `NecromancerGrimoire` (registry, coleccion persistente, condicion de drop,
  GlobalNPC, alma desbloquea+consume, grimorio invoca/cicla). NecromancerMinionAI
  actualizado al refactor. Suite PASS=91. +hjson.
- Verificacion: build 0/0; suite PASS=91. SIN probar in-game.
- PENDIENTE (Fase 3): mas criaturas (Skeleton Archer/Mage, Spectral Knight, Abomination,
  corruptas, inframundo) + almas de JEFE (versiones menores: King Slime->Guardian Slime,
  etc.); UI del Grimorio (coleccion visible); balance.

## 2026-07-10 - Nigromante Fase 1: Vida Reservada + drenaje de mana (rediseÃ±o del nucleo)

- Objetivo (spec del usuario, reemplaza al Mago del Infinito en la v1): el Nigromante NO
  usa minion slots. Cada no-muerto reserva vida maxima y drena mana; sin mana crumblean
  los mas debiles primero.
- RediseÃ±o del nucleo (Fase 1):
  - `NecromancerPlayer`: quita MaxNecroSlots/UsedNecroSlots. Cuenta no-muertos ->
    ReservedLifeFraction (suma de ReservePercent, tope 90%) -> baja statLifeMax2. Suma
    ManaDrain; cada segundo drena mana y si llega a 0 despawnea el mas debil
    (DespawnWeakest por menor ReservePercent). ShadowAffinity/Bone Conduit/milestones
    alivian la reserva.
  - `BaseNecroMinion`: SlotCost -> ReservePercent; ya no se auto-mata sin mana (solo
    fade); el player gestiona los despawns.
  - SkeletonMinion: ReservePercent 15. BeginnerNecromancyBook: invoca mientras
    ReservedLifeFraction < 0.9 (sin slots). UI (Necromancer/SoulUI) muestran "Reserved
    life %" + nÂº de no-muertos.
- Tests: `NecromancerReservedLife` (nuevo: sin slots, reserva vida, despawn del mas
  debil, base no se auto-mata); NecromancerFlow y PassiveRuntimeEffects (Bone Conduit)
  actualizados al nuevo diseÃ±o. Suite PASS=89.
- Verificacion: build 0/0; suite PASS=89. SIN probar in-game.
- PENDIENTE (fases del spec): Fase 2 = Grimorio de la Muerte + sistema de Almas (matar N
  -> drop de <Enemy> Soul -> registrar en el Grimorio -> desbloquea la criatura);
  Fase 3 = mas criaturas + almas de jefe (versiones menores). Balance in-game.

## 2026-07-10 - Mago de Maldiciones: la rama Curse moldea la mecanica de Corrupcion

- Objetivo: que la rama de pasivas Curse moldee la mecanica (energia/corrupcion/burst),
  como Defensa->aura y Elemental->afinidad. Se mantuvieron los 8 nombres de nodo y sus
  stats magicos planos en EterniaStatsPlayer (lo exigen los tests) y se AÃ‘ADIO el
  moldeado en CursedMagePlayer (leido via nuevo helper HasCurse).
- Mapeo (efecto plano + rol de mecanica, gated a Cursed Mage promovido):
  - Dark Ritual: +3% curse power - +1 regen de Energia Maldita.
  - Cursed Blood: +15% cursed dmg - la Corrupcion da mas daÃ±o magico (+0.125%/pt).
  - Doom Bringer: +12% magic dmg - la Corrupcion da mas vel. lanzamiento (+0.05%/pt).
  - Withering Curse: +5 pen - reduce a la MITAD la perdida de defensa por Corrupcion.
  - Soul Rot: +5 pen - menos perdida de vida max por Corrupcion (c/8 en vez de c/5).
  - Blight: +8% magic dmg - +50% a la explosion del Cursed Burst.
  - Malediction: +8% crit - el Burst refunde mas Energia (70) y el Colapso empieza mas
    tarde (190 en vez de 175).
  - Forbidden Hex: +10% duracion de debuff (rol de maldicion, sin cambios).
- Consecuencia de diseÃ±o: invertir en Curse te deja empujar mas la Corrupcion (menos
  castigo) y sacarle mas provecho (mas daÃ±o/regen/burst) -> profundiza el riesgo/
  recompensa. Sin tocar el conteo de nodos ni EterniaStatsPlayer.
- Tests: CursedMageMechanic ampliado (HasCurse + los 7 nodos que moldean). Suite PASS=89.
- Verificacion: build 0/0; suite PASS=89. SIN probar in-game.
- Con esto el Mago de Maldiciones esta COMPLETO (mecanica 2 fases + arsenal 12 + rama
  Curse que la moldea). Pendiente global: balance in-game; arte real.

## 2026-07-10 - Mago de Maldiciones: arsenal de 12 armas (Energia + Corrupcion)

- Objetivo (spec): armas que giran en torno a administrar recursos, no solo daÃ±o.
  Pre-HM enseÃ±an a manejar Energia Maldita; HM aÃ±aden Corrupcion (riesgo/recompensa).
- Arquitectura: interfaz `Content/Items/ICurseWeapon.cs` (EnergyCost + BurstMultiplier),
  clase base `Content/Items/Weapons/Magic/CurseWeapon.cs` (CanUseItem gatea por energia;
  Shoot consume energia y en HM suma CorruptionPerCast; ModifyWeaponDamage escala con
  corrupcion), y 2 proyectiles compartidos: `CurseBolt` (refund de energia + AoE
  opcional, ai[0]=refund/ai[1]=AoE) y `CurseOrb` (orbe perseguidor que sube corrupcion
  al golpear y escala con ella). No subclass-locked (el Soul + el gate de energia ya los
  mantienen solo-Mago).
- PRE-HARDMODE (5, solo Energia): Initiate's Grimoire (14, refund) -> Void Rod (20,
  perfora+refund) -> Forbidden Tome (24, AoE+refund por grupo) -> Eclipse Staff (10,
  rapido/drena) -> Abyssal Codex (40, alto coste).
- HARDMODE (7, +Corrupcion): Grimoire of Sin (60, poca corrupcion+refund) -> Cursed
  Staff (68, x1.3 si corrupcion>50) -> Tome of the Abyss (78, AoE) -> Doom Orb (50, orbe
  perseguidor que sube corrupcion y escala con ella) -> Book of Collapse (95, x1.5 si
  corrupcion>150) -> Scepter of the End (115, mucha corrupcion, +0.3%/pt) -> Necronomicon
  of Eternia (140, +0.4%/pt, BurstMultiplier x1.5; se craftea desde el Scepter).
- Wire: `CursedMagePlayer.ActivateBurst` lee el BurstMultiplier del arma sostenida (el
  Necronomicon amplifica la explosion). `CursedMageUI` ahora muestra un mini-panel de
  Energia pre-HM cuando sostienes un ICurseWeapon (feedback del recurso en la fase de
  entrenamiento).
- Tests: `CurseArsenalSourceSmokeTest` (12 armas extienden CurseWeapon, EnergyCost,
  crafteo, no-lock, daÃ±o exacto, HM genera corrupcion; base consume energia + corrupcion
  gated a Cursed Mage + escala; escalados de Cursed Staff/Book of Collapse/Necronomicon).
  Suite PASS=89. +hjson.
- Verificacion: build 0/0; suite PASS=89. SIN probar in-game.
- Con esto el Mago de Maldiciones tiene mecanica + arsenal. Pendiente: rama de pasivas
  Curse que moldee la mecanica (reducir castigos, mas regen, potenciar el Burst); balance
  in-game.

## 2026-07-10 - Mago de Maldiciones: rediseÃ±o de la mecanica en dos fases (spec usuario)

- Objetivo (spec del usuario): dos fases claras. PRE-HARDMODE (cualquier Mago): solo
  Energia Maldita con regen FIJA; sin Corrupcion ni Burst (entrenamiento del recurso).
  HARDMODE (Cursed Mage promovido): se desbloquea todo.
- Cambio de mecanica (autorizado por el spec) en `CursedMagePlayer`:
  - Gate nuevo `IsActiveMage()` (energia existe para cualquier Mago) vs
    `IsActiveCursedMage()` (corrupcion/burst). `ConsumeEnergy` ahora permite gastar a
    cualquier Mago (armas de maldicion funcionan pre-HM).
  - `TotalCorruption` = 0 salvo Cursed Mage promovido -> pre-HM sin corrupcion aunque
    lleves accesorios de maldicion.
  - Regen de energia: FIJA pre-HM (PreHardmodeRegen=3); en HM escala con Corrupcion
    (1..12) + milestones.
  - Corrupcion = riesgo/recompensa continuo (antes casi todo era via Burst):
    RECOMPENSA +0.25%/pt daÃ±o magico y +0.1%/pt vel. lanzamiento; RIESGO -def (/20),
    -vida max (/5), +daÃ±o recibido (endurance -0.15%/pt); COLAPSO: DoT fuerte >=175.
  - Cursed Burst REDISEÃ‘ADO: de buff sostenido (+40% 10s + backlash) a EXPLOSION
    instantanea que gasta TODA la corrupcion: dano = 40 + corrupcion*4 en area
    (radio 220+corrupcion), resetea la corrupcion temporal a 0 y refunde 40 de energia.
    Escala con la corrupcion gastada.
- UI: CursedMageUI ajustada (se quito el estado de burst sostenido; muestra "Press V for
  Cursed Burst" a partir de BurstMinCorruption).
- Tests: `CursedMageBootstrap` actualizado (regen fija pre-HM + baseline HM; ConsumeEnergy
  para cualquier Mago); nuevo `CursedMageMechanicSourceSmokeTest` (2 gates, corrupcion
  gated, fase pre-HM sale temprano, recompensa+riesgo+colapso, burst-explosion escalado
  + reset + refund). Suite PASS=88.
- Verificacion: build 0/0; suite PASS=88. SIN probar in-game.
- Pendiente: rama de pasivas Curse que moldee la mecanica (reducir castigos, mas regen,
  potenciar el Burst, debuffs al golpear); arsenal de maldicion pre-HM (armas que gastan
  Energia, no subclass-locked) + HM; UI de energia pre-HM (marcador de arma de maldicion).

## 2026-07-10 - Mago Elemental: arsenal de 15 armas magicas elementales

- Objetivo (spec): armas asociadas a un elemento desde que se obtienen. Pre-HM funcionan
  como armas elementales normales; en HM la Afinidad Elemental las modifica. El mismo
  arsenal juega distinto por afinidad.
- Arquitectura: UN proyectil `Content/Projectiles/ElementalArsenalBolt.cs` parametrizado
  por elemento en ai[0] (aplica el efecto base -quemar/ralentizar/electrificar/pierce/
  burst- a cualquier lanzador, asi que sirve pre-HM). Clase base
  `Content/Items/Weapons/Magic/ElementalStaff.cs` que dispara ese bolt; las armas de
  "afinidad/ciclo" (Element = -1) disparan el elemento ACTIVO del Elementalist (o uno
  aleatorio si no esta promovido). La capa de Afinidad Elemental (globales previos)
  superpone el efecto de la afinidad activa en HM sobre CUALQUIER arma magica -> el
  mismo arma se juega distinto por elemento.
- PRE-HARDMODE (7): Ember Spark (18, Fuego, craft) -> Boreal Staff (25, Hielo, cofres de
  hielo) -> Spark Rod (29, Rayo, Rey Slime) -> Gale Scepter (34, Viento, craft) ->
  Granite Core (40, Tierra, bioma granito) -> Magma Tome (47, Fuego, craft hellstone) ->
  Elemental Sage Scepter (56, ciclo, Skeletron; mejor arma magica pre-HM).
- HARDMODE (8): Phoenix Staff (72, Fuego, craft) -> Glacial Scepter (80, Hielo, craft) ->
  Storm Orb (88, Rayo, craft) -> Hurricane Staff (96, Viento, Piratas) -> Seismic
  Scepter (104, Tierra, Golem) -> Grimoire of the Five Elements (118, ciclo, craft desde
  los 5 bastones HM) -> Cataclysm Staff (135, ciclo, Pilar Nebulosa) -> Heart of Gaia
  (155, ciclo, post Moon Lord, arma definitiva).
- Obtencion: `Content/Globals/ElementalDropsGlobalNPC.cs` (King Slime->Spark Rod,
  Skeletron->Sage Scepter, granito->Granite Core, Piratas->Hurricane, Golem->Seismic,
  Pilar Nebulosa->Cataclysm) y `Content/Systems/ElementalChestLoot.cs` (Boreal Staff en
  cofres del bioma de hielo, detectado muestreando bloques de nieve/hielo). Drop-only:
  Boreal, Spark Rod, Granite Core, Sage Scepter, Hurricane, Seismic, Cataclysm.
- No subclass-locked (son armas magicas; cualquier Mago las usa). Placeholder de textura:
  todas reusan ElementalApprenticeStaff; el bolt reusa FireBoltProjectile (tintado por
  elemento via GetAlpha/dust).
- Tests: `ElementalArsenalSourceSmokeTest` (15 armas: extienden ElementalStaff, elemento
  correcto, dano monotono exacto, crafteo vs drop-only; el bolt Magic keyed por ai[0]
  con efecto base; drops + cofre de hielo). +hjson. Suite PASS=87.
- Verificacion: build 0/0; suite PASS=87. SIN probar in-game.
- Con esto el Mago Elemental esta completo (mecanica + arbol 5 sub-ramas + Maestria HM +
  arsenal). Pendiente global: balance in-game; arte real.

## 2026-07-10 - Mago Elemental: nodos HM de Maestria (cambio rapido + surge al cambiar)

- Objetivo (spec/usuario): nodos exclusivos de Hardmode para cambiar de afinidad mas
  rapido y ganar bonos temporales al cambiar.
- Mecanica (ElementalistPlayer): el cambio de elemento ahora tiene un cooldown base de
  45 ticks (0.75s) -- `SwitchTimer`, ticked en PostUpdate, gatea ChangeNote. Al cambiar,
  si tienes Momentum Shift se enciende un `SurgeTimer` (surge) que da +daÃ±o magico
  temporal en PostUpdateEquips.
- Sub-rama nueva "Elemental Mastery" (AffinityType "Elemental", 3 nodos, transversal a
  los elementos; rutea a ElementalAffinity como el resto):
  - Elemental Flux: -20 ticks al cooldown de cambio.
  - Momentum Shift: al cambiar, surge de +15% daÃ±o magico por 3s.
  - Grand Attunement: -15 ticks mas (min 8), y el surge sube a +25% por 5s.
  Leidos via `ElementalistPlayer.HasElementNode(...)` (solo importan promovido = HM).
- Tests: ElementalBranches ampliado (3 nodos Mastery + SwitchTimer/SurgeTimer). Ajustes:
  PassiveTreeDepth (Mage 39->42), PassiveRuntimeCoverage (acepta HasElementNode como
  wrapper de HasActivePassive). Suite PASS=86.
- Verificacion: build 0/0; suite PASS=86. SIN probar in-game.
- Nota: el cambio de elemento paso de instantaneo a 0.75s de cooldown base (evita spam y
  da sentido a los nodos de velocidad). Pendiente: arsenal de armas magicas del
  Elementalist; balance in-game.

## 2026-07-10 - Mago Elemental: el ramo de pasivas en 5 sub-ramas elementales

- Objetivo (spec): la rama "Elemental" se especializa por elemento. Cada sub-rama mejora
  un elemento; en HM tambien potencian la afinidad activa.
- Estructura: se reemplazo la cadena unica de 8 nodos genericos (Elemental Control...
  Storm Caller) por 5 sub-ramas (AffinityType Fire/Ice/Lightning/Wind/Earth), cada una
  una cadena de 3 nodos (15 en total). Cada elemento es su propia espiga en el arbol
  radial (5 sub-ramas visibles).
- Promocion intacta: `ProgressionService.AddAffinity` rutea las 5 afinidades
  elementales a `ElementalAffinity`, asi que invertir en CUALQUIER elemento sigue
  promoviendo a Elementalist (no se toco ClassPromotionRules ni el snapshot).
- Nodos (efecto pre-HM en EterniaStatsPlayer + sinergia HM en los globales):
  - Fuego: Kindling (+6% mag), Ember Fury (+8%; HM quemadura mas larga 240->360),
    Pyromancer (+10%; HM bono vs quemados 1.15->1.30).
  - Hielo: Frost Touch (+6%), Deep Freeze (+10% crit), Absolute Zero (+8%; HM congela
    mas seguido 1/8 -> 1/4).
  - Rayo: Static Charge (+6%), Chain Master (+8%; HM arco en cada golpe), Tempest
    (+8% dmg/+8% crit; HM un arco extra).
  - Viento: Zephyr (+5% dmg, -5% mana), Gale Force (-10% mana; HM +2 pierce), Tempest
    Winds (+12%; HM proyectiles aun mas rapidos 1.25->1.4).
  - Tierra: Stone Skin (+5 def), Tremor (+8%; HM burst mayor), Tectonic (+8 def, +10%;
    HM explosiones aun mayores).
- UI: PassiveUI colorea las 5 afinidades (Fire/Ice/Lightning/Wind/Earth); la lista
  blanca v1 las incluye (mas "Elemental" para el medidor de la barra). El medidor
  lateral sigue siendo un unico "Elemental" (afinidad total hacia la promocion).
- Sinergias HM leidas via `ElementalistPlayer.HasElementNode(nodo)` en los globales.
- Tests: `ElementalBranchesSourceSmokeTest` (5 sub-ramas, ruteo a ElementalAffinity,
  sinergias HM). Ajustados PassiveTreeDepth (Mage 32->39; sin los 2 nodos viejos),
  PassiveRuntimeEffects (15 nodos nuevos), v1 visibility (+5 elementos). Suite PASS=86.
- Verificacion: build 0/0; suite PASS=86. SIN probar in-game.
- Pendiente: nodos exclusivos de HM adicionales (cambio de afinidad mas rapido, bonos
  al cambiar), arsenal de armas magicas del Elementalist, balance in-game.

## 2026-07-10 - Mago Elemental: Afinidad Elemental (5 elementos que remodelan la magia)

- Objetivo (spec del usuario): el Elementalist domina 5 elementos. En hardmode (ya
  promovido) puede cambiar libremente de afinidad; la afinidad activa modifica el
  comportamiento de PRACTICAMENTE TODAS las armas magicas. Pre-hardmode: sin afinidad,
  solo se especializa via arbol (pendiente el rediseÃ±o del ramo elemental).
- Base existente extendida: `ElementalistPlayer` pasa de 3 a **5 elementos** (se aÃ±aden
  Viento y Tierra): WindAffinity/EarthAffinity + niveles + Save/Load + colores +
  ultimates (CYCLONE empuja enemigos, EARTHQUAKE golpea en area). Se conserva toda la
  API previa (carga, GainAffinity, ultimates con teclas R/Z).
- Efectos de stat de la afinidad activa (`ElementalistPlayer.PostUpdateEquips`, gated a
  Elementalist): Fuego +12% daÃ±o magico; Viento -15% mana + 15% vel. de lanzamiento;
  Tierra +12 defensa y sin retroceso mientras empuÃ±a magia. (Hielo/Rayo son on-hit.)
- NUEVO nucleo (el spec): la afinidad remodela TODAS las armas magicas via globales:
  - `Content/Globals/ElementalAffinityGlobalProjectile.cs` (solo magia de un Elementalist
    activo): OnHit -> Fuego Quemadura; Hielo Escarcha+Chilled+chance Congelar; Rayo
    Electrified + arco a un enemigo cercano (ChainLightning); Tierra explosion en area
    (EarthBurst). ModifyHitNPC -> Fuego +15% vs enemigos quemados. OnSpawn -> Viento +1
    penetracion.
  - `Content/Globals/ElementalAffinityGlobalItem.cs`: Rayo/Viento -> proyectiles +25%
    mas rapidos (ModifyShootStats).
- UI: ElementalistUI muestra los 5 pills (Viento/Tierra aÃ±adidos, panel mas alto); el
  SoulUI ya mostraba elemento+carga; especialidad actualizada.
- Tests: `ElementalAffinitySourceSmokeTest` (5 elementos, Save, stats por afinidad,
  globales de magia con on-hit por elemento + fuego/viento + velocidad). Suite PASS=85.
- Verificacion: build 0/0; suite PASS=85. SIN probar in-game.
- Pendiente (proximas pasadas): rediseÃ±ar el ramo de pasivas "Elemental" en 5
  sub-ramas (Fuego/Hielo/Rayo/Viento/Tierra) + nodos exclusivos de hardmode; arsenal de
  armas magicas del Elementalist; balance in-game.

## 2026-07-10 - Alcance v1: ocultar en el arbol de pasivas las subclases no-v1

- Objetivo (usuario): para la v1, mostrar solo 3 subclases por clase base en el arbol
  de pasivas. Guerrero: Sangrado/Combo/Defensa (Espadachin/Peleador/Escudero). Mago,
  Rango e Invocador: 3 de cada, ocultar el resto. NO borrar codigo: solo esconder.
- Implementacion (`Content/UI/PassiveUI.cs`): lista blanca unica `V1VisibleAffinities`
  (12 afinidades) + helper `IsAffinityVisible`. Se filtra en:
  - `GroupPassivesByAffinity` -> las ramas radiales del arbol.
  - `GetAffinities` -> los medidores de afinidad de la barra lateral.
  Como las ramas ocultas no se dibujan, no se pueden invertir puntos en ellas -> no se
  gana su afinidad -> no se puede promover a esas subclases (el gating de la v1 sale
  gratis). Nada del registro/mecanicas/armas de las subclases ocultas se toca.
- Visibles v1: Guerrero {Bleed, Combo, Defense}; Mago {Elemental, Curse, Infinity};
  Rango {Energy, Bow, Gun}; Invocador {Beast, Tech, Shadow}.
- Ocultas: Precision (Yoyo), Rage (Berserker), Control (Stunner), Arcane (Arcane Bard),
  Music (Virtuoso), Fusion (Advanced Summoner). Re-activar una = aÃ±adir su afinidad a
  la lista blanca.
- Tests: `PassiveTreeV1VisibilitySourceSmokeTest` fija la lista blanca (12 visibles, 6
  ocultas) y que ambos draws filtran por IsAffinityVisible. Suite PASS=84.
- Verificacion: build 0/0; suite PASS=84. SIN probar in-game.

## 2026-07-10 - Escudos: linea de hardmode + obtencion + escudo de promocion al aura

- Objetivo (usuario): meter "todo lo demas" del Escudero salvo el recurso de
  Resistencia. Hecho: linea de escudos de hardmode, 2Âª pasada de obtencion, y convertir
  el TrainingShield (premio de promocion) a la mecanica de aura.
- LINEA HARDMODE (6, crafteables salvo lo indicado, Generic, NO subclass-locked):
  Mythril Bulwark (60, HM ore, Frostburn) -> Hallowed Bulwark (74, Hallow, Cursed
  Inferno) -> Nightfall Barrier (88, Eclipse/Broken Hero Sword, Ichor) ->
  Plaguebringer Wall (104, Chloro+Beetle Husk, Venom+Cursed Inferno) -> Luminite
  Barrier (128, Luminite, Ichor + regen aliados) -> Eternal Aegis (156, capstone
  Eternia, Shadowflame+Ichor+regen; se craftea desde Luminite Barrier + 4 fragmentos).
  DPS de aura ~225 (Mythril) a ~670 (Eternal), x1.8 con el Escudero.
- OBTENCION 2Âª pasada (`Content/Globals/ShieldDropsGlobalNPC.cs`,
  `Content/Systems/ShieldChestLoot.cs`, `tests/ShieldObtentionSourceSmokeTest.ps1`):
  - Drop-only (sin receta): Corrupt Shield <- Brain of Cthulhu (1/2) y Eater of Worlds
    (OnKill 1/2, con guarda de ultimo segmento).
  - Crafteable + drop bonus: Hallowed Bulwark <- jefes mecanicos (1/4); Nightfall
    Barrier <- Mothron (1/4); Plaguebringer Wall <- Plantera (1/3).
  - Cofre bonus: Glacial Shield sembrado en cofres subterraneos (no-Dungeon), sigue
    crafteable.
- TrainingShield (premio de promocion Guardian): convertido a IShieldWeapon/aura
  (Generic, dmg 55, pulse 15, radio 90), sigue LightRed, no-crafteable y locked a
  Guardian (CanUseItem + tooltip). Es el escudo insignia del Escudero, entra en
  hardmode. Se beneficia del moldeado del arbol de Defensa. Compatible con los tests
  de balance/promocion (no verifican DamageType).
- Tests: ShieldArsenal reescrito -> valida ambas lineas (dano monotono + tope por tier,
  channel, aura Generic, no-swing, una sola aura, no-lock, efecto de personalidad,
  drop-only sin receta) + que HM supera a pre-HM; ShieldObtention valida el mapeo
  jefe->escudo, el cofre y el no-crafteo. Suite PASS=83.
- Verificacion: build 0/0; suite PASS=83. SIN probar in-game.
- Pendiente unico pedido fuera: recurso de Resistencia.

## 2026-07-10 - El arbol de Defensa moldea el aura del Escudero

- Objetivo (usuario): mejorar la rama de pasivas de Defensa para el Escudero. Se
  mantuvo la identidad tanque (todos los nodos siguen dando def/DR/vida plana en
  EterniaStatsPlayer -- lo exige PassiveTreeDepth y es el fantasy del Guardian) y
  ADEMAS cada nodo ahora moldea el aura del escudo (gated a Guardian activo).
- Mapeo (leido en GuardianPlayer via HasDefensePassive, solo si IsActiveGuardian):
  - DaÃ±o de aura: Iron Wall +10%, Fortress Body +15%, Aegis +20% (sobre el +25% base
    del Escudero + escalado con Defensa).
  - Radio de aura: Shield Training +10%, Bulwark +15% (sobre el +15% base).
  - Velocidad de pulso: Unbreakable y Stonewall, cada uno *0.85 (con ambos ~28% mas
    rapido). Nuevo AuraPulseMultiplier(), usado por el proyectil.
  - Efecto por pulso: Last Bastion -> el aura tambien te cura un poco cada pulso
    (ApplyGuardianAuraPulse, solo en el cliente del owner).
- No se toco el conteo de nodos (WarriorPassives sigue en 49) ni el keystone
  (Immovable Object queda como tanque puro); solo se enriquecieron descripciones +
  logica de aura. Los efectos planos permanecen para cualquier Guerrero; el moldeado
  del aura es exclusivo del Escudero.
- Tests: ShieldAura verifica que GuardianPlayer lee los 8 nodos de Defensa para el
  aura y que el proyectil usa AuraPulseMultiplier + ApplyGuardianAuraPulse. Suite
  PASS=82.
- Verificacion: build 0/0; suite PASS=82. SIN probar in-game.

## 2026-07-10 - Balance de escudos: viables para matar jefes (DPS + Escudero + guardia)

- Objetivo (usuario): que los jugadores tambien puedan derrotar JEFES con escudos, no
  solo hacer control de grupos. El cuello de botella era el DPS a un solo objetivo.
- Radio: subido en dos pasos a peticion del usuario. Ahora Wooden 72 ... Holy 104 px
  (~4.5 a ~6.5 tiles); el Escudero suma +15% encima.
- Knockback: los pulsos ahora aplican knockback (Item.knockBack 0->3 por escudo, el
  pulso usa Projectile.knockBack en vez de 0f). Con el radio grande el empujon no saca
  al enemigo del aura: queda orbitando en el borde, recibiendo daÃ±o sin tocarte. Los
  jefes son resistentes al knockback (comportamiento vanilla).
- DPS a un objetivo (daÃ±o de aura = daÃ±o / intervalo de pulso). Se subio daÃ±o y se
  acelero el pulso, y luego se subio el daÃ±o otra vez (~+60%) a peticion del usuario:
  - Wooden 6->16 (/20t, ~48 DPS); Iron 9->21 (~63); Corrupt 11->27 (/18, ~90,
    +Ichor); Glacial 12->31 (~103, +Slow); Ember 14->36 (/16, ~135, +fuego); Holy
    16->44 (~165, +regen). Con el Escudero (arbol completo ~1.8x) el Holy ronda ~300
    DPS de aura en area. Caps del test subidos a 20/26/32/36/42/50.
- Payoff del Escudero (GuardianPlayer.AuraDamageMultiplier): ahora +25% PLANO de daÃ±o
  de aura (es su arma) + escalado con Defensa +1%/4 def. Un Escudero a 40 def -> +35%;
  el Holy Shield en su mano ~= 90*1.35 ~ 121 DPS de aura.
- Bonus de GUARDIA (nuevo `Content/Players/ShieldPlayer.cs`, cualquier clase): mientras
  el aura este activa (posee el DefensiveAuraProjectile) -> +10% endurance. Permite
  tanquear el "abrazo" al jefe. Se prefirio sobre knockback (que empujaria enemigos
  fuera del radio pequeÃ±o).
- Tests: caps de daÃ±o del ShieldArsenal subidos (12/15/18/20/24/28); ShieldAura verifica
  el +25% plano del Escudero y el guard DR de ShieldPlayer. Suite PASS=82.
- Verificacion: build 0/0; suite PASS=82. SIN probar in-game (numeros a validar).

## 2026-07-10 - Fix: los escudos (Generic) ya no disparan la penalizacion de Soul

- Sintoma (reportado por el usuario): al usar un escudo (p. ej. Corrupt Shield) el
  sistema de Soul lo trataba como "arma de otra clase" y aplicaba la penalizacion de
  muerte (SoulViolation), matando al jugador.
- Causa: `SoulRules.IsWeaponAllowed` mapea DamageClass -> clase. Un escudo es
  DamageClass.Generic, que no es Melee/Ranged/Magic/Summon, asi que TODA clase lo
  rechazaba (isMelee/isMagic/... = false) -> penalizacion. Contradecia el diseÃ±o
  ("cualquier clase puede usar escudos").
- Arreglo (sin quitar la penalizacion): un arma class-neutral (DamageClass.Generic)
  no pertenece a ninguna clase, por lo que nunca traiciona a un Soul -> se permite
  para todos. Se uso igualdad EXACTA (`item.DamageType == DamageClass.Generic`), no
  CountsAsClass, porque toda clase real deriva de Generic y CountsAsClass(Generic)
  haria pasar tambien a una espada melee.
- Nota de compatibilidad: el test de comportamiento compila SoulRules.cs en
  aislamiento (solo con SoulId.cs), por eso el fix NO referencia IShieldWeapon; usa
  solo DamageClass.Generic (disponible en esa compilacion).
- Tests: `SoulRulesBehaviorSmokeTest` amplia con aserciones runtime (Generic permitido
  por Warrior/Ranger/Mage/Summoner y es combat item). Suite PASS=82; el test de
  comportamiento compila+ejecuta la logica real y pasa.
- Archivos: Content/Souls/SoulRules.cs, tests/SoulRulesBehaviorSmokeTest.ps1.

## 2026-07-10 - Subclase Escudero: categoria de arma ESCUDO con Aura Defensiva

- Objetivo (spec del usuario): los Escudos son una categoria de arma distinta. Clic
  izquierdo mantenido levanta el escudo; tras ~0.5s proyecta un Aura Defensiva que hace
  daÃ±o continuo a enemigos en un radio pequeÃ±o mientras se sostenga; al soltar
  desaparece. No hay golpe melee tradicional. Cualquier clase puede usarlos; el
  Escudero (Guardian) saca el maximo.
- Infra nueva: `Content/Items/IShieldWeapon.cs` (interfaz: AuraPulseInterval,
  AuraRadius, AuraColor, OnAuraHit, OnAuraPulse) y
  `Content/Projectiles/Guardian/DefensiveAuraProjectile.cs` (proyectil canalizado:
  spin-up 30 ticks, se pega al owner, pulsos de daÃ±o manuales Generic por radio, aplica
  efecto del escudo, se mata al soltar el canal; solo el owner hace daÃ±o para MP).
- Payoff del Escudero: `GuardianPlayer.AuraDamageMultiplier()` (aura escala con Defensa,
  +1%/5 def) y `AuraRadiusMultiplier()` (+15% radio), solo si IsActiveGuardian.
- LINEA DE ESCUDOS pre-hardmode (6, crafteables, NO subclass-locked, Generic):
  Wooden Shield (6, aura fisica basica) -> Iron Shield (9, mas daÃ±o) -> Corrupt Shield
  (11, Ichor = debilita defensa enemiga) -> Glacial Shield (12, Slow+Frostburn) ->
  Ember Shield (14, En llamas) -> Holy Shield (16, daÃ±a + regen ligera a aliados via
  OnAuraPulse). DaÃ±o de aura monotono creciente.
- Cada escudo: Item.channel=true, noMelee, useStyle Shoot, dispara UNA aura persistente
  (guarda ownedProjectileCounts[type]==0), UseSound=null (evita sonido repetido).
- Tests: `ShieldAuraSourceSmokeTest` (canal, spin-up 30, Kill al soltar, pulsos Generic,
  lee IShieldWeapon, aplica efectos, payoff Guardian con Defensa) y
  `ShieldArsenalSourceSmokeTest` (6 escudos: IShieldWeapon, channel, aura Generic,
  no-swing, una sola aura, crafteo, no-lock, efecto de personalidad, daÃ±o monotono).
- Verificacion: build 0/0; suite PASS=82. SIN probar in-game.
- Pendientes (proximas pasadas): rediseÃ±ar la rama de pasivas de Defensa para que
  moldee el aura (daÃ±o/radio/efectos/duracion de guardia); recurso de Resistencia;
  linea de escudos de hardmode; 2Âª pasada de obtencion (drops/cofres); convertir el
  TrainingShield (premio de promocion) al aura; UI/HUD del aura; arte real.

## 2026-07-10 - Obtencion 2Âª pasada del arsenal de puÃ±os (drops de jefe/evento/cofre)

- Objetivo (spec del usuario): igual que el Espadachin, atar las armas de puÃ±os al
  jefe/zona/evento que les corresponde en vez de un simple gate de materiales.
- Archivos nuevos: `Content/Globals/FighterDropsGlobalNPC.cs` (drops via ModifyNPCLoot
  + OnKill), `Content/Systems/FighterChestLoot.cs` (semilla en cofres subterraneos),
  `tests/FighterObtentionSourceSmokeTest.ps1`. Sin receta ahora: ProspectorsGauntlets
  y BloodfeastGauntlets (se les quito AddRecipes).
- DROP-ONLY (sin receta):
  - Bloodfeast Gauntlets (final pre-WoF) -> jefe del mal: Brain of Cthulhu (1/2 via
    ModifyNPCLoot) y Eater of Worlds (1/2 via OnKill, con guarda de ultimo segmento
    para que un gusano no lo suelte por cada cabeza). Espeja a DreadReaver.
  - Prospector's Gauntlets (oro) -> cofres subterraneos (no-Dungeon, ~1/4 de hasta 3
    cofres en PostWorldGen) + goteo de Undead Miner (1/8, tematico minero). Espeja a
    BonewardenSabre (cofre + goteo).
- CRAFTEABLE + drop bonus (doble via):
  - Thornfist Gauntlets -> Queen Bee (1/3).
  - Nightfall Gauntlets -> Mothron / Eclipse Solar (1/4).
  - Plaguebringer Fists -> Plantera (1/3).
- Reserva de espacio: FighterChestLoot excluye el Dungeon (ese espacio es del
  Bonewarden Sabre del Espadachin).
- Tests: FistArsenal ahora marca DropOnly (Prospector's/Bloodfeast) y verifica que NO
  son crafteables; FighterObtention valida el mapeo jefe->arma, el cofre y el
  no-crafteo. Suite PASS=80.
- Verificacion: build 0/0; suite PASS=80. SIN probar in-game.

## 2026-07-10 - Arsenal completo de puÃ±os del Peleador (pre + hardmode) con efectos

- Objetivo (spec del usuario): las armas del Peleador representan presion constante a
  corto alcance, NO golpe unico devastador. Todas comparten mecanicas base; solo
  cambian stats, tier y un efecto secundario. Regla central: **un arma NUNCA toca el
  Combo** -- el Combo lo potencia solo la subclase; las armas evolucionan en
  dano/velocidad/efecto.
- Mecanicas base compartidas (todas): lanzan `FighterPunchProjectile` (+1 Combo por
  golpe, mecanica de distancia 100%->50%), Melee, useTime bajo (rapidas), knockback
  muy bajo (<=3), crafteables, NO subclass-locked.
- Infra nueva: `Content/Items/IFistWeapon.cs` (interfaz marcadora con
  `OnPunchHit(owner, target)` de cuerpo vacio por defecto). El puÃ±etazo, al golpear,
  lee el arma equipada (`HeldItem.ModItem is IFistWeapon`) y ejecuta su efecto
  secundario -- que jamas modifica el Combo.
- LINEA PRE-HARDMODE (6): Padded Fists (8, inicio, sin efecto) -> Iron Knuckles (11,
  mineral) -> Prospector's Gauntlets (15, oro) -> Thornfist Gauntlets (17, jungla,
  Envenenado) -> Molten Knuckles (20, inframundo, En llamas) -> Bloodfeast Gauntlets
  (24, final pre-WoF, robo de vida ligero).
- LINEA HARDMODE (6): Mythril Brawlers (42, mineral HM, Quemadura Helada) -> Hallowed
  Knuckles (54, bioma/Hallow, Fuego Maldito) -> Nightfall Gauntlets (64,
  evento/Eclipse via Broken Hero Sword, Icor) -> Plaguebringer Fists (78, jefe/Chloro
  +Beetle Husk, Veneno+Fuego Maldito) -> Luminite Knuckles (98, Luminite, Icor + robo
  de vida) -> Eternal Fury (116, capstone Eternia, Shadowflame+Icor+robo de vida;
  se craftea desde Luminite Knuckles + 4 fragmentos).
- Cableo: `FighterPunchProjectile.OnHitNPC` invoca `IFistWeapon.OnPunchHit` tras
  sumar Combo. Efectos por AddBuff (OnFire/Poisoned/Frostburn/CursedInferno/Ichor/
  Venom/ShadowFlame) o lifesteal (statLife+HealEffect con probabilidad).
- Tests: `FistArsenalSourceSmokeTest` reescrito -> valida ambas lineas (dano
  monotono + tope por tier, useTime<=15, kb<=3, crafteo, no subclass-lock, efecto via
  IFistWeapon.OnPunchHit, y que NINGUN arma manipula el Combo en codigo) + que el
  puÃ±etazo cablea OnPunchHit.
- Verificacion: build 0/0; suite PASS=79. SIN probar in-game.
- Pendientes: numeros/efectos a tunear; arte real de cada arma (todas reusan el
  placeholder TrainingGauntlet); drops de jefe/evento y cofres (2Âª pasada de
  obtencion, como se hizo con el Espadachin) aun no hechos -- por ahora todo es
  crafteo.

## 2026-07-10 - Peleador pre-hardmode: Combo como CONTADOR + armas de puÃ±os

- Objetivo (spec del usuario): en pre-hardmode el jugador ya usa armas de puÃ±os y
  aprende el Combo, pero el Combo NO da nada (solo contador). Al promover a Peleador
  (post Muro de Carne) el MISMO combo empieza a interactuar con las pasivas -> la
  progresion se siente natural, no un sistema nuevo en hardmode.
- Arquitectura (paralela al sangrado): el Combo es una mecanica de clase WARRIOR (lo
  construye cualquier Guerrero con puÃ±os), y el payoff es de SUBCLASE (Peleador).
- Archivos: `Content/Players/FighterPlayer.cs` (contador Warrior-wide, efectos
  Fighter-gated), `Content/Projectiles/FighterPunchProjectile.cs` (gate ->
  IsActiveWarrior; la distancia funciona pre-hardmode), armas nuevas
  `Weapons/Fighter/{PaddedFists,IronKnuckles,ProspectorsGauntlets,MoltenKnuckles}.cs`;
  `tests/FistArsenalSourceSmokeTest.ps1` (nuevo), `FighterComboSourceSmokeTest` y
  `SubclassRuntimeGatingSourceSmokeTest` ajustados. +hjson.
- Cambios:
  - `FighterPlayer`: el contador (build/decay, cap 20, ventana 2.5s) corre para
    cualquier Guerrero (IsActiveWarrior). TODO el payoff (dano/vel/move por combo,
    generacion, cap+/duracion+ por pasivas, Frenesi, perdida al recibir dano) esta
    gated a IsActiveFighter -> pre-hardmode el combo no modifica ningun stat.
  - `FighterPunchProjectile`: gate a IsActiveWarrior. La mecanica de distancia
    (100% pegado -> 50% al final del alcance) aplica desde pre-hardmode y no cambia
    en hardmode. El multiplicador de combo es 1 hasta promover.
  - LINEA DE PUÃ‘OS pre-hardmode (crafteable, NO subclass-locked): Padded Fists (8,
    inicio) -> Iron Knuckles (11) -> Prospector's Gauntlets (15) -> Molten Knuckles
    (20, inframundo). Todas lanzan el puÃ±etazo. El Training Gauntlet sigue siendo el
    premio de promocion Fighter-locked (hardmode).
- Verificacion: build 0/0; suite PASS=79. SIN probar in-game.
- Pendientes: numeros a tunear; FighterComboUI aun con valores viejos (cosmetico);
  arte real de puÃ±os.

## 2026-07-10 - Rediseno del Peleador (Fighter): Combo pasivo-driven + Frenesi + Remate

- Objetivo (spec del usuario): el Peleador es un brawler agresivo cuyo Combo NO da
  bonos por si solo; TODO el arbol de pasivas define que hace el Combo. Base 20
  acumulaciones / 2.5s (antes 50 / 2s).
- Archivos: `Content/Players/FighterPlayer.cs` (reescrito),
  `Content/Players/FighterSkillPlayer.cs` (nuevo, Remate),
  `Content/Projectiles/FighterPunchProjectile.cs`, `Content/Passives/PassiveRegistry.cs`
  (descripciones rama Combo + keystone), `Content/Players/EterniaStatsPlayer.cs`
  (se quitaron los efectos planos de los nodos Combo), `Content/Players/KeystonePlayer.cs`
  (caso Combo -> lo maneja FighterPlayer), `Content/UI/SoulUI.cs` (Combo x/max dinamico);
  `tests/FighterComboSourceSmokeTest.ps1` (nuevo) + PassiveTreeDepth ajustado.
- Cambios:
  - Combo pasivo-driven: por si solo no hace nada. Cada nodo de la rama Combo cambia
    su comportamiento (leidos en FighterPlayer): Combo Instinct / Limit Breaker =
    +1% dano melee por acumulacion c/u; Flow State = +0.6% vel ataque por acum.;
    Perfect Rhythm = +0.5% move por acum.; Adrenaline Rush = +1.5s ventana; Unbroken
    Chain = +10 al cap; Rapid Blows = crit/pegado dan +1 combo; Thousand Cuts =
    conservas el combo al recibir dano (baseline lo parte a la mitad).
  - Keystone Perpetual Motion -> FRENESI: mientras mantengas el combo maximo,
    +15% dano / +10% vel ataque / +8% reduccion de dano.
  - REMATE (nuevo FighterSkillPlayer): tecla Skill gasta TODO el combo en un golpe
    AoE a bocajarro escalado por el combo (30 + combo*10), con cooldown compartido.
  - Distancia recalibrada: bocajarro 100% (los bonos >100% vienen de pasivas),
    lejos 50% -> obliga a pelear pegado.
- Verificacion: build 0/0; suite PASS=78. SIN probar in-game.
- Pendientes/riesgos: (a) numeros a tunear in-game (por-combo, frenesi, remate,
  penalizacion al recibir dano). (b) FighterComboUI muestra el combo pero sus
  umbrales de color y la barra de timer usan valores viejos (50/120) -> cosmetico.
  (c) el Peleador sigue con UNA sola arma (Training Gauntlet); linea de guanteletes
  pendiente. (d) un Guerrero no-Peleador que compre el keystone Combo ya no recibe
  su viejo bono plano.

## 2026-07-09 - Tajo custom (CrimsonSlash) unico por espada

- Objetivo (peticion del usuario): personalizar el proyectil (era el EnchantedBeam
  AZUL de vanilla) y hacer cada espada unica.
- Solucion (data-driven, 1 proyectil para las 17 espadas):
  - `Content/Projectiles/Warrior/CrimsonSlash.cs(.png)` (nuevo): un tajo en media
    luna (textura generada por codigo, gris para tintar limpio). En AI lee la espada
    que lo disparo (`owner.HeldItem is IBleedWeapon`) y toma su `SlashColor`/
    `SlashScale` cada frame (MP-safe) -> cada espada tiene su propio color y tamano.
  - `IBleedWeapon` extendida con `SlashColor`/`SlashScale` (miembros por defecto);
    las 17 espadas los sobreescriben con valores unicos (script de inyeccion):
    rojos/rosas para rapidas chicas, rojo oscuro grande para pesadas, verde para
    Chlorophyte/Requiem, cian luminita para Exsanguinator, etc.
  - El BALANCE (alcance + pierce 2) se movio DENTRO de CrimsonSlash; se borro
    `SwordBeamGlobalProjectile`. Dano del tajo sigue al 45%. Alcance: a peticion del
    usuario se alargo de ~13 a ~29 tiles (timeLeft 20->42) -> ahora es una extension
    a distancia de verdad, no solo un poke corto.
  - `EterniaGlobalItem.SetDefaults` ahora pone `item.shoot = CrimsonSlash`.
- Verificacion: build 0/0; suite PASS=77. +hjson (DisplayName del proyectil).
- Pendientes: el tajo usa una textura generada (media luna gris tintada); si quieres
  arte de tajo real, se cambia el .png. Confirmar in-game que cada espada muestra su
  color/tamano.

## 2026-07-09 - Las espadas del Espadachin lanzan un TAJO a distancia (que sangra)

- Objetivo (peticion del usuario): que las espadas peguen desde lejos, como la
  mayoria de armas melee de Terraria (Terra Blade, Enchanted Sword, etc.).
- Decision: TODAS las 16 espadas del mod (IBleedWeapon) lanzan un tajo, de forma
  uniforme (identidad del Espadachin), centralizado -> no se toco ningun archivo de
  espada.
- Archivos: `Content/Globals/EterniaGlobalItem.cs` (set beam + reduce dano),
  `Content/Players/WarriorBleedPlayer.cs` (+OnHitNPCWithProj, +ModifyHitNPCWithProj),
  `Content/Players/SwordsmanPlayer.cs` (+OnHitNPCWithProj);
  `tests/SwordBeamSourceSmokeTest.ps1` (nuevo).
- Cablo:
  - `EterniaGlobalItem.SetDefaults`: si `ModItem is IBleedWeapon`, set
    `item.shoot = ProjectileID.EnchantedBeam`, `shootSpeed = 11`. Central para las 16.
  - `ModifyShootStats`: el tajo hace 55% del dano de la espada (`BeamDamageFactor`);
    el golpe cuerpo a cuerpo sigue al 100%, asi que melee de cerca sigue siendo mejor.
  - El SANGRADO ahora se aplica tambien en el impacto del tajo (proyectil), espejo del
    golpe directo: `WarriorBleedPlayer.OnHitNPCWithProj` tira la probabilidad;
    `SwordsmanPlayer.OnHitNPCWithProj` da sangrado GARANTIZADO + Rastro Carmesi.
    Gated a proyectil melee + `CanInflictBleed(Player.HeldItem)`, asi que otros
    proyectiles melee (yoyos, etc.) no sangran.
  - `ModifyHitNPCWithProj`: el tajo tambien recibe Execution/Exsanguinate vs sangrantes.
- BALANCE (peticion del usuario): (1) dano del tajo 55%->45%; (2) nuevo
  `SwordBeamGlobalProjectile` acorta el tajo de NUESTRAS espadas a ~13 tiles de
  alcance (timeLeft 20) y pierce 2, para que sea una EXTENSION melee de alcance
  corto, no un arma a distancia con spam a pantalla completa. Solo afecta beams
  disparados teniendo una espada de sangrado (la Enchanted Sword vanilla no se toca).
  Efecto: a bocajarro melee(<=vanilla) + 45% queda cerca/por debajo de vanilla del
  tier; a distancia media es un poke debil de alcance limitado.
- Verificacion: build 0/0; suite PASS=77. SIN probar in-game.
- Pendientes/riesgos: (a) el beam es EnchantedBeam (AZUL) -> off-theme; tajo rojo
  custom cuando haya arte. (b) los % y el alcance son tuneables; el tuning fino pide
  pruebas in-game (sobre todo las espadas rapidas). (c) confirmar in-game que el tajo
  dispara/sangra/se acorta.

## 2026-07-09 - [REVERTIDO] Arsenal pre-hardmode del Ranger

- Se creo un arsenal Ranger (4 arcos/armas) pero el usuario decidio quitarlo: las
  clases no-Warrior no necesitan armas del mod (usan vanilla), asi que era contenido
  tematico de baja prioridad. Borrado (archivos, test, hjson). Se retomo el pulido
  de subclases en su lugar.

## 2026-07-09 - Espadas del Espadachin: 2da pasada de obtencion (drops/cofres/evento)

- Objetivo (peticion del usuario): atar las espadas al jefe/zona en vez de soft-gate
  por material.
- Archivos (nuevos): `Content/Globals/SwordsmanDropsGlobalNPC.cs`,
  `Content/Systems/SwordsmanChestLoot.cs`; tests
  `SwordsmanObtentionSourceSmokeTest.ps1` (nuevo) + arsenal test ajustado.
  Modificados: `DreadReaver.cs`, `BonewardenSabre.cs` (sin receta).
- Cambios:
  - Dread Reaver: DROP del Ojo de Cthulhu (1/2), sin receta. Era el soft-gate mas
    flagrante (Oro+Lentes sin matar al jefe). Ahora gateado al jefe.
  - Bonewarden Sabre: LOOT de cofres del Dungeon (`SwordsmanChestLoot.PostWorldGen`
    inyecta en ~1/3 de cofres con pared de dungeon, hasta 3) + trickle de no-muertos
    del Dungeon (Angry Bones/Dark Caster/Cursed Skull, 1/60) para mundos existentes.
    Sin receta.
  - Thornrender: + drop de Reina Abeja (1/3), MANTIENE receta (Bee Wax).
  - Crimson Requiem: + drop de Mothron en el Eclipse (1/4), MANTIENE receta
    (Broken Hero Sword). Ejemplo de "drop de evento".
  - Las demas (Corruptor's via escama del mal, etc.) ya estaban gateadas por
    material -> sin cambio.
- Verificacion: build 0/0; suite PASS=76. SIN probar in-game.
- Pendientes: (a) el chest loot solo afecta mundos NUEVOS (como todo chest loot
  vanilla); mundos viejos usan el drop de no-muertos. (b) Arte real sigue pendiente.
  (c) Lineas de las otras 3 clases.

## 2026-07-09 - Linea de espadas del Espadachin: lote HARDMODE

- Objetivo: continuar la linea del Espadachin por todo hardmode (una espada
  crafteable por tier mayor). La Bloodletter Blade (premio de promo) cubre Cobalto.
- Archivos (nuevos, `Weapons/Warrior/`): QuicksilverFang (Mythril), SanguineCleaver
  (Adamantita), HallowedBloodletter (jefes mecanicos), ChlorophyteHemoblade
  (Clorofita), TitansGutcleaver (Golem), CrimsonRequiem (Eclipse), Exsanguinator
  (Endgame/Luminita). +2 recipe groups (Mythril/Oricalco, Adamantita/Titanio).
  +hjson. `tests/SwordsmanHardmodeArsenalSourceSmokeTest.ps1` (nuevo).
- Linea hardmode (dano/vel/sangrado): Cobalto Bloodletter (42/-/22% promo) - Mythril
  Quicksilver Fang (44/14/24% rapida) - Adamantita Sanguine Cleaver (56/30/10%
  pesada) - Mecanicos Hallowed Bloodletter (62/20/16% equilibrada) - Clorofita
  Chlorophyte Hemoblade (64/15/22% rapida) - Golem Titan's Gutcleaver (84/32/12%
  pesada) - Eclipse Crimson Requiem (92/22/18%, Broken Hero Sword) - Endgame
  Exsanguinator (112/14/24% rapida, Luminita).
- Balance: dano sube monotonicamente y CADA espada queda <= su referencia vanilla
  del mismo tier (Excalibur 66, Terra Blade 95, Terrarian 190...); el test lo verifica
  con un cap por tier.
- Verificacion: build 0/0; suite PASS=75.
- Pendientes: la linea del Espadachin esta COMPLETA (inicio -> endgame, 16 espadas +
  Bloodletter). Falta: (a) arte real (todas usan placeholder), (b) 2da pasada de
  obtencion (drops/cofres/eventos en vez de soft-gate por material), (c) las lineas
  de las OTRAS clases (Ranger/Mage/Summoner) siguen con solo el arma inicial.

## 2026-07-09 - Linea de espadas del Espadachin: lote PRE-HARDMODE completo

- Objetivo (spec del usuario): una linea de espadas propia del Espadachin a lo largo
  de toda la progresion, con identidad rapida/equilibrada/pesada (rapida = mas
  sangrado/menos dano; pesada = al reves), sin superar a las vanilla del mismo tier.
- Decisiones del usuario: tooltip del % de sangrado se MANTIENE visible (no se toco
  `ModifyTooltips`); obtencion = armas + recetas primero (drops/cofres reales en una
  2da pasada).
- Arquitectura: las espadas son `IBleedWeapon` usables por CUALQUIER Guerrero (el
  sangrado es mecanica de clase Warrior; la promocion a Espadachin es hardmode-only,
  asi que no pueden ser subclass-locked). El Rastro Carmesi se alimenta solo cuando
  ya eres Espadachin, via la mecanica de sangrado existente (sin codigo extra).
- Archivos (nuevos): `Content/Systems/EterniaRecipeGroups.cs` (grupos Silver/Gold/
  EvilBar/EvilScale para ambos tipos de mundo); espadas
  `Weapons/Warrior/{SilverlightRapier,DreadReaver,CorruptorsRipper,BonewardenSabre}.cs`.
  Retuneadas: `HuntersWarblade` (Oro), `Thornrender` (Reina Abeja). +hjson.
- Linea pre-hardmode (9 tiers): Inicio Training Blade (11/10%) - Hierro Serrated
  (13/16%, rapida) - Plata Silverlight Rapier (16/24%, rapida) - Oro Hunter's
  Warblade (20/14%, equilibrada) - Ojo Cthulhu Dread Reaver (22/8%, pesada) -
  EoW/BoC Corruptor's Ripper (22/22%, rapida) - Reina Abeja Thornrender (24/20%,
  especialista) - Dungeon Bonewarden Sabre (24/20%, rapida) - Inframundo Molten
  Gutripper (27/16%, pesada). Todas <= vanilla del mismo tier.
- Verificacion: build 0/0; suite PASS=74 (arsenal test extendido a las 8 espadas).
- Pendientes: (a) HARDMODE: faltan ~8 espadas (Cobalto->Endgame); Bloodletter Blade
  (premio de promo) cubre el tier cobalto. (b) 2da pasada de obtencion: drops de jefe
  reales, loot de cofres del Dungeon (Bonewarden), drops de evento. (c) ARTE: todas
  reutilizan el sprite placeholder del TrainingGauntlet; falta arte real de espada.
  (d) Dread Reaver (Ojo Cthulhu) y Bonewarden (Dungeon) estan soft-gated por material
  (Lens / Bone), no hard-gated al jefe/zona; candidatos a drop real.

## 2026-07-09 - Armas de promocion subidas a tier HARDMODE (promoverse ya no es un downgrade)

- Objetivo (peticion del usuario tras auditar las armas): las 18 recompensas de
  promocion tenian stats pre-hardmode pero se entregan EN hardmode.
- Diagnostico: `ClassPromotionRules.ResolveSubclass` devuelve la clase base si
  `!Main.hardMode` -> la promocion SOLO ocurre en hardmode. Pero las 18 armas
  tenian 8-20 de dano y rareza Blue. La del Espadachin (Bloodletter Blade, 16) era
  MAS DEBIL que el Molten Gutripper (27) crafteable pre-hardmode. El Necromancer
  recibia un libro de 8 de dano. Ademas, 2 de ellas eran crafteables con materiales
  de inicio (Book + Fallen Star / Book + 15 Bone).
- Archivos: las 18 armas de promocion; `tests/WeaponBalanceSourceSmokeTest.ps1`.
- Cambios:
  - Dano reescalado a tier de entrada de hardmode (cobalt/palladium), escalado por
    `useTime` de cada arma: Melee 34-58 (RageCleaver 20->58 por ser la mas lenta,
    BloodletterBlade 16->42), Ranged 30-36, Magic 31-36, Latigos/invocacion 26-30
    (bajos a proposito: en vanilla el latigo de hardmode Firecracker hace 30).
  - Rareza Blue -> LightRed (tier cobalt) en las 18. Valor -> 2 oro.
  - `CursedApprenticeTome` y `BeginnerNecromancyBook` PIERDEN su receta: eran armas
    de promocion crafteables con materiales de inicio, lo que ahora daria un arma
    de hardmode en los primeros minutos.
  - Test reescrito: verifica LightRed + sin receta + que cada arma supere a la mejor
    crafteable pre-hardmode DE SU PROPIA CLASE DE DANO (comparar un latigo contra un
    espadon era incorrecto; el primer intento del test lo cazo) + piso de hardmode.
- Verificacion: build 0/0; suite PASS=74. SIN probar in-game.
- Pendientes: (a) Ranger/Mage/Summoner siguen con UNA sola arma crafteable (la de
  madera); falta su escalera pre-hardmode como la del Warrior. (b) Siguen sin existir
  armas POST-hardmode. (c) "Training Gauntlet"/"Training Shield" conservan nombre de
  arma de entrenamiento pese a ser premios de hardmode (renombrar es churn: hjson,
  refs, tests).

## 2026-07-09 - Balance de progresion de la rama Bleed (Espadachin)

- Objetivo (peticion del usuario): auditar si la rama del Espadachin esta balanceada
  "en cuestion de desarrollo". Diagnostico: NO. Tres problemas.
- Diagnostico (numeros del codigo, `PassivePointsPerLevel = 1`):
  - La rama Bleed costaba 41 puntos (= nivel 42 para completarla).
  - P1 GRAVE: los 3 escalados del bleed (chance, duracion, dano DoT) se capeaban en
    afinidad Bleed 20, alcanzada en el 5o nodo (9 de 41 puntos, 22% del costo). De
    los 76 de afinidad de la rama, 56 (74%) no hacian NADA por el sangrado.
  - P2 GRAVE: los 10 nodos Minor costaban 13 pts (32% de la rama) y su aporte
    marginal real era +1.6% melee (cap de mastery ya alcanzado) = ~0.12%/pt, contra
    5.0%/pt de Sword Mastery. ~40x peores. 13 niveles sin sentir nada.
  - P3: Exsanguinate (costo 4) daba el MISMO +15% vs sangrantes que Execution (costo 2).
  - P4: la rama otorgaba 76 de afinidad con cap de mastery 75 -> 1 punto desperdiciado.
    `Blood Flow` hacia 2 cosas (armor pen + duracion) y describia solo una.
- Archivos: `Content/NPCs/BleedGlobalNPC.cs`, `Content/Players/WarriorBleedPlayer.cs`,
  `Content/Passives/PassiveRegistry.cs`, `Content/Players/EterniaStatsPlayer.cs`;
  `tests/BleedProgressionSourceSmokeTest.ps1` (nuevo).
- Cambios:
  - P1: dano y duracion del bleed ahora escalan por tramos (valor completo hasta
    afinidad 20; luego +1/3 de dano y +2 ticks por afinidad hasta 90). El tope 90
    esta por ENCIMA de los ~86 que da la rama completa -> el sangrado crece durante
    TODA la rama. `BleedGlobalNPC.GetBleedDamage(affinity)` centraliza el dano.
    La PROBABILIDAD se dejo intacta (capeada en +10) porque el usuario ya la habia
    pedido nivelar; ahora invertir profundo te hace mas letal, no mas frecuente.
  - P2: los nodos Minor cuestan 1 (antes hasta 2) y dan 2 de afinidad (antes 1). La
    rama baja de 41 a 38 puntos y su afinidad sube a ~86, que ahora SI alimenta el
    sangrado.
  - P3: Exsanguinate +15% -> +25% vs sangrantes.
  - P4: `AffinityCap` 75 -> 100 (ningun punto invertido se desperdicia). Descripcion
    de `Blood Flow` completada.
- Efecto neto (rama Bleed full, 38 pts): dano DoT 26 -> ~48/tick (53 con Rupture);
  duracion ~9s -> ~11s; melee de mastery +13.5% -> +15.5%.
- Verificacion: build 0/0; suite PASS=74. SIN probar in-game.
- Pendientes/riesgos: los numeros de P1 son un buff notable al DoT; tunear jugando.
  P2 se aplico a las 18 ramas (los Minor son genericos), no solo a Bleed: revisar
  que no infle otras clases.

## 2026-07-09 - Panel de Stats: mostrar el TOTAL actual + preview del siguiente punto

- Objetivo (peticion del usuario): mejorar el panel de Character Stats.
- Diagnostico: el panel decia el ritmo POR PUNTO ("+0.3% all damage") pero nunca
  cuanto te daban ya tus puntos invertidos (Power 42 -> +12.6%). Tampoco habia
  progreso de EXP ni senal de que tenias puntos sin gastar.
- Archivos: `Content/UI/StatsUI.cs`; `tests/StatsPanelClaritySourceSmokeTest.ps1` (nuevo).
- Cambios:
  - Nuevo `CurrentEffect(name, value)` que espeja la matematica de
    `EterniaStatsPlayer.PostUpdateEquips` -> cada fila muestra su TOTAL actual como
    linea principal (ej. Power 42 -> "+12.6% all damage"), y el ritmo por punto como
    linea secundaria atenuada. Sin puntos: "Not invested yet".
  - Tooltip mejorado: "Now: ..." / "Per point: ..." / "Next point: ..." (preview del
    total DESPUES de gastar el punto).
  - Barra de progreso de EXP dentro del panel ("EXP 1234 / 2000").
  - Las pildoras Stats/Passives PULSAN cuando tienes puntos sin gastar, y el boton
    "+" tiene glow pulsante cuando puedes gastar (ya se atenuaba sin puntos).
  - Descripciones por punto corregidas (Agility "+0.5% move, +1% run", Focus
    "+3 max mana, +0.5 mana regen") para que coincidan con el codigo real.
  - Panel 486x520 -> 486x548 para acomodar la barra de EXP.
- Riesgo/mantenimiento: `CurrentEffect` DUPLICA la matematica de
  `EterniaStatsPlayer`. El test nuevo verifica que los multiplicadores del player no
  cambien sin actualizar la UI (falla ruidosamente si hay drift).
- Verificacion: build 0/0; suite PASS=73. SIN probar in-game.

## 2026-07-09 - ELIMINADO el recurso de clase base (Momentum/Charge/Focus/Bond)

- Objetivo (decision del usuario): quitar por completo el recurso de las clases
  base. Un recurso pasa a ser estrictamente una mecanica de SUBCLASE.
- Archivos: `Content/Players/BaseClassPlayer.cs` (reescrito), `Content/UI/SoulUI.cs`,
  `docs/gameplay-systems.md`. BORRADOS: `Content/UI/BaseClassResourceUI.cs`,
  `tests/BaseClassResourceUISourceSmokeTest.ps1`. Tests ajustados:
  `OverlayClampSourceSmokeTest`, `OverlayPlayerStateSourceSmokeTest`.
- Cambios:
  - `BaseClassPlayer` pierde los 4 campos de recurso y toda su logica (acumulacion
    en OnHit, decay, escalado de dano en `ModifyWeaponDamage`, velocidad en
    `UseSpeedMultiplier`). CONSERVA los `+stat` fijos de clase base en
    `PostUpdateEquips` y `IsActiveBaseClass` (que el test de gating exige).
  - Se borro la barra flotante del recurso base y su test.
  - `SoulUI`: ya no depende de `BaseClassPlayer`; la fila Resource de una clase sin
    promover ahora dice "None - promote to gain one".
- Consecuencia de balance: una clase base pre-promocion pierde el escalado que daba
  el recurso (hasta ~+15% dano / +10% velocidad a full). Ahora solo tiene sus
  `+stat` fijos. Queda por validar in-game si la pre-promocion se siente sosa.
- Verificacion: build 0/0; suite PASS=72 (73 -> 72 al borrar el test de la UI).

## 2026-07-09 - Barra de recurso flotante COMPARTIDA (mismo look para las demas clases)

- Objetivo (peticion del usuario): aplicar la mejora visual de la barra base a las
  demas clases/subclases.
- Archivos: `Content/UI/EterniaUI.cs` (nuevo helper), `BaseClassResourceUI.cs`,
  `CrimsonTrailUI.cs`, `SubclassResourceUI.cs`; tests `CrimsonTrailSourceSmokeTest`,
  `SubclassResourceUISourceSmokeTest`, `OverlayClampSourceSmokeTest`.
- Cambios:
  - Nuevo `EterniaUI.DrawFloatingResourceBar(spriteBatch, player, label, value, max,
    color, ready, readyPrompt)`: fade in/out con el valor (nunca se queda en 0),
    track oscuro + relleno con borde brillante y gloss, glow pulsante cuando esta
    casi lleno o "ready", etiqueta compacta arriba ("X 45" / "X MAX"), pill
    "Q: ..." opcional debajo. Ancla y CLAMPEA la posicion internamente. Un solo
    `resourceBarAlpha` estatico compartido (solo una subclase activa a la vez).
  - `BaseClassResourceUI`, `CrimsonTrailUI` y `SubclassResourceUI` ahora solo
    resuelven (label, value, max, color, ready) y delegan el dibujo al helper ->
    se elimino el dibujo duplicado. Cubre: 4 clases base + Espadachin (Crimson
    Trail) + las 6 subclases de `SubclassResourceUI` = 11 recursos con el look nuevo.
  - Tests actualizados: Crimson/SubclassResourceUI ahora exigen
    `DrawFloatingResourceBar`; `OverlayClamp` acepta el clamp delegado al helper.
- Verificacion: build 0/0; suite PASS=73. SIN probar in-game.
- Pendientes: quedan 10 medidores BESPOKE con visual propio (FighterComboUI contador
  de combo, StunnerChargeUI, BerserkerUI, ArcherFocusUI, GunnerUI, EnergyHeatUI,
  VirtuosoUI notas, CursedMageUI, ElementalistUI, NecromancerUI). Decidir si se
  unifican al helper (consistencia) o conservan su estilo propio.

## 2026-07-09 - REVERT del Momentum-buff + mejora visual de la barra flotante

- Objetivo: el usuario decidio que lo del buff+nodo del Momentum fue mala idea;
  pidio "regresalo a como estaba y mejora lo visual".
- REVERTIDO (la entrada de abajo "Momentum -> buff + nodo" queda anulada): se
  quitaron los 4 nodos "Core", el gate en `BaseClassPlayer`, `TryGetResourceDisplay`
  y el AddBuff; se borro `Content/Buffs/ClassResourceBuff.cs(.png)` y su test; se
  reactivo `BaseClassResourceUI`; se limpio la entrada huerfana del buff en
  `en-US.hjson`; se revirtieron los conteos de `PassiveTreeDepthSourceSmokeTest`.
  El recurso de clase base vuelve a ser automatico (sin nodo).
- MEJORA VISUAL de la barra flotante (`Content/UI/BaseClassResourceUI.cs`):
  - Solo aparece cuando hay recurso (fade in/out suave via `barAlpha`) -> se acabo
    el feo "MOMENTUM 0/100" permanente sobre la cabeza.
  - Dibujo pulido a mano (paleta EterniaUI): track oscuro + relleno con borde
    brillante y gloss superior, glow pulsante cuando esta casi lleno, sombra suave.
  - Etiqueta compacta arriba de la barra ("MOMENTUM 45" / "MOMENTUM MAX"),
    posicionada un poco mas alto. Barra 118x12.
- Verificacion: build 0/0; suite PASS=73. SIN probar in-game.

## 2026-07-09 - [ANULADO] Momentum -> buff + desbloqueo por nodo (revertido arriba)

- Objetivo (peticion del usuario): la barra flotante del Momentum (recurso de clase
  base) sobre el personaje se ve mal; convertirla a buff y desbloquearla con un nodo
  del arbol. El usuario tambien pidio "todas a buff", pero eso toca 13 UIs (incl.
  paneles interactivos) + 6 tests y no se puede validar in-game -> se hace por etapas.
  ESTA etapa: solo el recurso de clase base.
- Archivos (nuevos): `Content/Buffs/ClassResourceBuff.cs` + `.png` (orbe generado),
  `tests/ClassResourceBuffSourceSmokeTest.ps1`. (mod) `PassiveRegistry.cs`,
  `BaseClassPlayer.cs`, `UI/PassiveUI.cs`, `UI/BaseClassResourceUI.cs`,
  `tests/PassiveTreeDepthSourceSmokeTest.ps1`.
- Cambios:
  - NODO de desbloqueo: se agrego 1 nodo "Core" por clase al inicio de cada lista
    (Warrior "Momentum", Mage "Arcane Charge", Ranger "Steady Focus", Summoner
    "Spirit Bond"), afinidad "Core" (ignorada por afinidad/mastery), costo 1,
    comprable de inmediato. `PadBranchesTo` excluye "Core" (no lo rellena ni le
    pone keystone); se renderiza como su propio spoke corto (color Goldenrod).
  - GATE: `BaseClassPlayer` solo acumula/da el recurso si el nodo Core esta
    desbloqueado (`HasActivePassive` con literal por clase). Los +stat base de clase
    NO se tocan (siguen siempre).
  - BUFF: un solo `ClassResourceBuff` dinamico (nombre + tooltip vivos via
    `BaseClassPlayer.TryGetResourceDisplay`), aplicado cada frame cuando el recurso
    esta desbloqueado y > 0. Icono = orbe generado por codigo (System.Drawing).
    Sin tinte por ahora (BuffDrawParams no resolvio; es solo estetico).
  - La barra flotante base (`BaseClassResourceUI`) se apago via flag
    `DrawFloatingBar=false` (se conservan los strings que verifican los tests).
- Verificacion: build 0/0; suite PASS=74. SIN probar in-game.
- Pendientes: ETAPA 2 = convertir las 18 barras de subclase a este mismo buff
  (extender `ClassResourceBuff`/aplicador con el mapeo de las 18) y apagar sus UIs.
  Falta: localizacion del buff (hjson) y opcional tinte del icono por color.

## 2026-07-09 - Panel de Soul: seccion de SUBCLASE (especialidad + bonus + recurso)

- Objetivo (peticion del usuario): el panel de Soul mostraba solo la identidad de
  la clase base y una fila "Base resource: Inactive after promotion" (fea al estar
  promovido). Pedido: que muestre tambien lo de la SUBCLASE.
- Archivos: `Content/UI/SoulUI.cs`.
- Cambios:
  - Panel dividido en dos: **CLASE** (Survival/Damage/Combat/Speed, ya reflejan el
    total con bonus de subclase) y una seccion **SUBCLASE - <nombre>** con divisor.
  - La seccion subclase tiene 3 filas: **Specialty** (one-liner de su mecanica),
    **Subclass bonus** (los +stat que da, espejo de `SubclassEffectsPlayer`) y
    **Resource** (el recurso EN VIVO de esa subclase: Crimson Trail, Rage, Overflow,
    Ferocity, Necro slots, etc. â€” mapeo de las 18 subclases + las 4 base).
  - Se elimino la fila "Base resource / Inactive after promotion"; ahora sin
    promover se muestra el recurso base (Momentum/Charge/Focus/Bond) en la fila
    Resource, y las clases base dicen "Not promoted - build affinity to promote".
  - Panel agrandado 368x402 -> 384x500 para las filas nuevas.
- Verificacion: build 0/0; suite PASS=73. SIN probar in-game.
- Pendientes/riesgos: los textos de bonus son estaticos (si se cambian los stats en
  `SubclassEffectsPlayer` hay que actualizarlos aqui). Confirmar in-game que el
  panel mas alto no se sale en resoluciones chicas (usa ClampToScreen).

## 2026-07-09 - HUD: barras de recurso para las 6 subclases que no tenian

- Objetivo (eleccion del usuario, rumbo v1): completar el HUD. 6 subclases tenian
  mecanica pero NINGUNA barra (solo texto/particulas): Infinity Mage (Overflow),
  Arcane Bard (Crescendo), Beast Tamer (Ferocity), Advanced Summoner (Command),
  Tech Summoner (Power Core) y Yoyo Master (Precision). Ahora las 18 subclases
  muestran su recurso.
- Archivos: `Content/UI/SubclassResourceUI.cs` (nuevo),
  `tests/SubclassResourceUISourceSmokeTest.ps1` (nuevo).
- Diseno: UN solo ModSystem consolidado (como `BaseClassResourceUI`) via
  `ModifyInterfaceLayers` -> barra `DrawProgressBar` anclada sobre el jugador +
  pill "Q: ..." cuando la tecnica esta lista (Overload/Roar/Overclock/Overdrive).
  Crescendo y Precision no tienen pill (uno es pasivo, el otro auto-dispara a 5).
  Solo una subclase activa a la vez; no colisiona con la barra de Crimson Trail
  (Espadachin) ni con la de clase base.
- Verificacion: build 0/0; suite PASS=73. SIN probar in-game.
- Pendientes/riesgos: colores/posicion tuneables. Confirmar in-game que la barra
  no tapa otra UI. Netcode: los recursos no estan sincronizados en multiplayer.

## 2026-07-09 - Fuente mas grande via wrap de 2 lineas (sin agrandar cards)

- Objetivo (peticion del usuario): "haz que la fuente crezca sin agrandar las
  tarjetas". Hasta ahora, subir la fuente obligaba a ensanchar la card (los
  nombres largos se recortaban con "...").
- Solucion: nuevo helper `DrawNodeLabel` en `PassiveUI` que ENVUELVE el nombre en
  hasta 2 lineas (`EterniaUI.WrapText`) y lo centra vertical/horizontalmente,
  usando el espacio vertical libre de la card. Un nombre corto = 1 linea grande;
  uno de 2 palabras = 2 lineas ("Adrenaline"/"Rush") en vez de encogerse/recortarse.
- Fuente base 0.85 -> 0.95 para los 3 tipos (Minor/Notable/Keystone). Cards SIN
  cambios (Minor/Notable 198x40, Keystone 238x58); el keystone dibuja el nombre en
  su region superior y el tag "KEYSTONE" (centrado) abajo, sin solaparse.
- Verificacion: build 0/0; suite PASS=72. SIN probar in-game.
- Pendientes/riesgos: si un nombre necesitara 3+ lineas se recorta a 2 (ninguno
  actual lo necesita). Las cards quedaron algo mas anchas de lo necesario ahora
  que el texto envuelve; se podrian estrechar a futuro para compactar el arbol.

## 2026-07-09 - Auditoria de balance del arbol + fuente 0.85

- Objetivo (peticion del usuario): fuente un poco mas grande + "verifica si cada
  nodo esta balanceado y se ve crecimiento en el desarrollo del jugador".
- Fuente: 0.8->0.85; cards 188->198 (keystone 224->238); `TierStep`/`LaneSpacing`
  -> no-keystone 220/216, keystone 263/259. Anti-solape reverificado.
- Auditoria (hallazgos):
  - OK: todo nodo tiene efecto runtime (test de coverage lo garantiza; cero nodos
    muertos). Costo sube con profundidad (1->4). Efectos escalan. Ramas simetricas.
  - FIX 1 - 6 descripciones no coincidian con el efecto real (la tarjeta "mentia",
    el jugador no veia bien su progreso). Corregidas en `PassiveRegistry`:
    Blood Flow "+10% bleed duration"->"+3 melee armor penetration";
    Precision Flow "+10% yoyo range"->"Infinite yoyo string";
    Blood Rage "+5% low HP damage"->"+12% melee below 35% HP";
    Musical Soul "+3% support power"->"+3% movement speed";
    Resonance "+5% buff duration"->"+2% damage reduction";
    Symphony Master "+10% ally buffs"->"+5% damage".
  - FIX 2 - crecimiento se aplanaba a mitad de rama: `AffinityCap` estaba en 40,
    pero los notables de una rama ya suman ~63, asi que los ~11 nodos Minor
    profundos daban CERO stat directo (via `ApplyAffinityMastery`). Subido a 75
    para que llenar la rama (incluidos los Minor) siga dando crecimiento.
- Verificacion: build 0/0; suite PASS=72. SIN probar in-game.
- Pendientes/riesgos: el cap 40->75 sube ~+5% el bonus de una rama full-invertida
  (endgame). Tunear con pruebas in-game. La fuente sigue creciendo el arbol; si se
  quiere mas grande sin agrandar cards, evaluar nombres en 2 lineas (wrap).

## 2026-07-09 - Fix: nodos del arbol se enimaban (tarjetas Minor 92px)

- Objetivo: bug reportado por screenshot -> los nodos se montaban unos sobre otros
  (Expert/Apex/Ascendant amontonados, keystone tapando el nodo previo).
- Causa: al pasar los Minor de "dots" (30px) a tarjetas con nombre (92px), el
  `TierStep(Minor)` seguia en 50px, pensado para dots. Ademas el paso radial usaba
  solo el tamano del tier ANTERIOR, asi que un Keystone ancho se enimaba con el
  Minor previo.
- Archivos: `Content/UI/PassiveUI.cs`.
- Cambios:
  - `TierStep` ahora es el footprint real (diagonal de la tarjeta + gap):
    Minor 50->116, Notable 122->138, Keystone 168->180.
  - El paso entre tiers t y t+1 usa `0.5*(TierStep[t]+TierStep[t+1])`: la mitad
    del footprint de cada uno -> garantiza cero solape en cualquier angulo del
    spoke (la diagonal es la distancia maxima centro-esquina).
  - `LaneSpacing` subido a 112/136/176 para separar hermanos del mismo tier.
  - Como el arbol quedo mas grande, `MinZoom` 0.5->0.4.
  - Legibilidad (2do screenshot del usuario: "las letras se ven chicas, tengo que
    hacer zoom"): el texto escala con el zoom, asi que el zoom inicial 0.7 lo hacia
    chico. Se dejo el zoom inicial/reset en 1.0 (la vista que el usuario confirmo
    legible) y se subio la letra de los Minor 0.44->0.5. Para el overview se aleja
    con la rueda (hasta 0.4).
  - Letra mas grande (3er screenshot: "haz la letra mas grande"): como los nombres
    largos se recortan si la fuente crece sin ensanchar la tarjeta, se agrandaron
    tarjeta+fuente JUNTAS: Minor 92x30->108x32 fuente 0.5->0.56; Notable
    112x40->152x40 fuente 0.5->0.6; Keystone 152x54->184x58 fuente 0.6->0.64. El
    espaciado (`TierStep`/`LaneSpacing`, derivado del footprint) se recalculo:
    Minor 116/112->131/128, Notable 138/136->176/172, Keystone 180/176->211/207.
    Verificado que ningun par (incluido Minor->Keystone) puede solaparse.
  - Tamano uniforme + fuente 0.7 (4to screenshot: "haz la fuente mas grande y los
    nuevos nodos no tienen el mismo tamano"): los Minor eran mas chicos que los
    Notable -> ahora Minor y Notable comparten UNA sola card (168x40); solo el
    Keystone es mayor (200x58, es el capstone dorado). La distincion es por estilo,
    no por tamano. Fuente unificada a 0.7 (Minor/Notable/Keystone). `TierStep`/
    `LaneSpacing` recalculados: no-keystone 191/187, keystone 226/222. Anti-solape
    reverificado. Nota: el arbol crece; se recorre con pan y se aleja con la rueda.
  - Fuente 0.8 (5to screenshot: "hazla mas grande"): fuente 0.7->0.8, cards
    168x40->188x40 (keystone 200->224) para que los nombres largos no se recorten.
    `TierStep`/`LaneSpacing` -> no-keystone 210/206, keystone 250/246. `MinZoom`
    0.4->0.3 para que el overview siga cabiendo con el arbol mas grande.
- Verificacion: build 0/0; suite PASS=72. SIN probar in-game (esperar screenshot).

## 2026-07-09 - Mecanicas signature para las 5 subclases "solo-stats"

- Objetivo (peticion del usuario, trabajo autonomo): mejorar la mecanica de cada
  subclase "como el Espadachin". Diagnostico: 13 de 18 subclases ya tienen mecanica
  propia; 5 eran SOLO `+stat` plano en `SubclassEffectsPlayer` (sin player propio):
  Infinity Mage, Arcane Bard, Beast Tamer, Advanced Summoner, Tech Summoner. Se le
  dio a cada una una mecanica signature distinta (recurso construido + payoff).
- Archivos (nuevos): `Content/Players/InfinityMagePlayer.cs`,
  `ArcaneBardPlayer.cs`, `BeastTamerPlayer.cs`, `AdvancedSummonerPlayer.cs`,
  `TechSummonerPlayer.cs`; `tests/SubclassMechanicsSourceSmokeTest.ps1` (nuevo).
- Cambios (5 mecanicas, todas gated a la subclase activa via `IsActiveX`,
  reusan `SkillKey`+`SkillPlayer` cooldown como el Espadachin):
  - **Infinity Mage â€” OVERFLOW**: castear rebosa un pozo (0-100); a tope empuja
    +15% magia pasivo; `SkillKey` a full = ARCANE OVERLOAD (~5s de `manaCost=0` +
    +25% magia). Fantasia "nunca te quedas sin mana".
  - **Arcane Bard â€” CRESCENDO**: golpes magicos suben momento (0-100) que escala
    magia/cast speed/move de forma CONTINUA y decae si dejas de pegar; a tope,
    pulso de cura. Sin gasto ni boton (identidad = mantener el ritmo).
  - **Beast Tamer â€” FEROCITY**: los golpes de tus minions suben furia (+15% invoc
    pasivo); `SkillKey` a full = PRIMAL ROAR (~6s, +30% invoc + knockback).
  - **Advanced Summoner â€” LEGION + OVERCLOCK**: +daÃ±o segun que tan llena esta la
    tropa (`slotsMinions/maxMinions`); la tropa carga COMMAND; `SkillKey` a full =
    OVERCLOCK (~5s, +2 al tope de minions + velocidad de invocacion).
  - **Tech Summoner â€” POWER CORE**: bateria que carga sola (mas rapido con drones)
    y da +defensa pasiva; `SkillKey` a full = OVERDRIVE PROTOCOL (~5s, +25% invoc +
    15 defensa/escudo).
- Nota: los `+stat` base en `SubclassEffectsPlayer` NO se tocaron (sirven de
  baseline; la mecanica STACKEA encima). Feedback via CombatText/Dust/Sonido como
  las demas subclases; sin barras de UI dedicadas todavia (opcional a futuro).
- Verificacion: `dotnet build -t:Compile` 0/0; suite completa PASS=72 FAIL=0.
- Pendientes/riesgos: SIN probar in-game. Numeros tuneables. Posible barra de UI
  por recurso (como Crimson Trail) si se quiere. Revisar balance del stack
  baseline+mecanica. Esperar feedback del usuario antes de tocar mas.

## 2026-07-08 - Tipos de nodo (Minor/Notable/Keystone) + 18 Keystones

- Objetivo (eleccion del usuario): la mejora #1 recomendada -> distinguir tipos de
  nodo (arbol legible) y agregar keystones (decisiones que definen build).
- Archivos: `Content/Passives/PassiveNode.cs` (`PassiveKind` + `Kind`),
  `Content/Passives/PassiveRegistry.cs` (Minor/Keystone + KeystoneName/Description),
  `Content/Players/KeystonePlayer.cs` (nuevo, efectos), `Content/UI/PassiveUI.cs`
  (render + layout por tipo), `tests/PassiveNodeTypesSourceSmokeTest.ps1` (nuevo).
- Cambios:
  - `PassiveKind` = Minor / Notable / Keystone. Hand-authored = Notable (default);
    generados = Minor; capstone de rama = Keystone.
  - RENDER por tipo: Minor = tarjeta CHICA con el nombre (se quita el prefijo de
    afinidad: "Bleed Adept" -> "Adept"), Notable = tarjeta con nombre,
    Keystone = tarjeta GRANDE dorada ("KEYSTONE" + glow/borde pulsante).
    (Correccion: los Minor empezaron como dots sin texto; el usuario pidio ver el
    nombre, asi que ahora son tarjetas chicas legibles.)
  - LAYOUT por tipo: espaciado/tamano segun el kind del tier (`CardWidth`,
    `TierStep`, `LaneSpacing`). Los dots se empacan densos cerca de las puntas,
    las tarjetas notables tienen aire cerca del hub -> arbol mucho mas legible.
  - 18 KEYSTONES (uno por rama) con efecto que define build + TRADE-OFF, aplicados
    en `KeystonePlayer` solo para la clase activa. Ej: Bleed "Hemorrhagic Frenzy"
    (+20% melee, -10% vel), Rage "Death Wish" (+25% melee, -40 vida), Beast
    "Apex Alpha" (+30% invoc, -1 minion), etc. El keystone cierra cada rama y pide
    haberla recorrido.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 71/71.
- Pendientes/riesgos: SIN probar in-game. Numeros de keystone ajustables. Beast
  "-1 minion" puede quedar bajo si tienes pocos (keystone es profundo, post-promo).

## 2026-07-08 - Fix tooltip: mostrar TODOS los prerequisitos (no uno solo)

- Bug (reportado con screenshot): un nodo tras un gate de 3 mostraba "Requires: X"
  (uno solo) aunque el gating real pedia los 3. La linea del tooltip usaba el campo
  viejo `passive.RequiredPassive` en vez de `GetPrerequisites` (derivado de tiers).
- Fix: `GetTooltipLines` ahora usa `PassiveRegistry.GetPrerequisites` y lista TODOS
  los prerequisitos ("Requires ALL of:" + cada uno, marcando "(owned)"). El gating
  ya era correcto; era solo la visualizacion.
- Archivos: `Content/UI/PassiveUI.cs`, `tests/PassiveDiamondTreeSourceSmokeTest.ps1`.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 70/70.

## 2026-07-08 - Passive Tree: zoom + tooltip grande + efectos de nodo + gates de 3

- Objetivo (pedido del usuario): zoom para el arbol grande; efectos/descripciones
  tematicas en los nodos generados; tooltip mas visible; y que algunos nodos
  requieran 1 o mas nodos.
- Archivos: `Content/UI/PassiveUI.cs` (zoom), `Content/UI/EterniaUI.cs` (tooltip),
  `Content/Passives/PassiveRegistry.cs` (TierSize gate + descripciones),
  `Content/Players/EterniaStatsPlayer.cs` (efecto generico de afinidad).
- Cambios:
  - ZOOM: rueda del mouse sobre el area (0.5x-1.35x). Se agrego un `ToScreen` que
    escala posicion Y tamano por zoom; conectores, hub y texto de nodo escalan con
    el. Reset al abrir. Hint: "Drag to pan - wheel to zoom".
  - TOOLTIP: mas grande y legible (ancho 408, fuente 0.7, titulo 0.84, mas alto de
    linea) en vez del 0.58 chico.
  - EFECTOS TEMATICOS: cada punto de afinidad da un bonus pequeno y TEMATICO
    (Bleed->melee, Defense->toughness, Elemental->magic, Shadow->summon, etc.;
    `ApplyAffinityMastery` en EterniaStatsPlayer, capado a 40/afinidad). Asi los
    nodos generados (que solo dan afinidad) SI hacen algo. Sus descripciones ahora
    dicen el efecto (`AffinityEffectText`).
  - GATES: `TierSize` ahora incluye un tier ancho de 3 nodos periodico (aislado,
    1->3->1), asi algunos nodos requieren TRES prerequisitos. Ya habia de 1 y 2.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 70/70.
- Pendientes/riesgos: SIN probar in-game. El zoom escala texto de nodo (a zoom muy
  bajo el nombre queda chico, es overview). El efecto generico apila con milestones
  + nodos notables; capado, pero revisable en balance.

## 2026-07-08 - Arbol extenso (20 nodos/rama) + fix estilos + letra milestone

- Objetivo (feedback con screenshot): letra chica en Milestones; la rama Control
  salia recta; y llevar cada rama a 20 nodos para v1 mas extensa.
- Archivos: `Content/UI/PassiveUI.cs` (letra milestone),
  `Content/Passives/PassiveRegistry.cs` (TierSize + padding),
  `tests/PassiveDiamondTreeSourceSmokeTest.ps1`.
- Cambios:
  - Milestone: el subtexto del perk ahora es mas grande y legible (DrawTrimmedText
    0.62, color claro) en vez del 0.5 minusculo.
  - Estilos de rama: ninguno es ya una LINEA RECTA. Los 4 estilos ahora tienen
    diamantes en distintos "beats" (antes el estilo 1 era lineal -> Control salia
    derecho). Se mantiene la regla: un tier de 2 siempre lo sigue uno de 1.
  - Padding a 20 nodos/rama: cada arbol se autogenera hasta `BranchTarget = 20`
    nodos por rama con nodos "de camino" (`PadBranchesTo`, nombres unicos
    "{Afinidad} {Titulo}") que dan +1 afinidad. No se escriben a mano ~215 nodos;
    se generan en el ctor estatico. Los nodos menores alimentan la eleccion de
    subclase + los milestones (el sistema de milestones es su recompensa).
    Totales runtime: Warrior 120, Ranger/Mage/Summoner 80 c/u.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 70/70 (los nodos generados no
  rompen los tests de conteo/cobertura porque usan nombres interpolados/variables).
- Pendientes/riesgos: SIN probar in-game. Con 20/rama los brazos quedan MUY largos
  -> mucho pan; aqui el ZOOM ayudaria bastante (siguiente paso natural). Balance de
  afinidad acumulada (p.ej. slots de Necromancer) revisable.

## 2026-07-08 - Milestones potencian la MECANICA de la subclase

- Objetivo (feedback del usuario): que el milestone ayude a la mecanica de la
  subclase, no solo a stats.
- Archivos: `Content/Players/NecromancerPlayer.cs`, `SwordsmanPlayer.cs`,
  `BerserkerPlayer.cs`, `CursedMagePlayer.cs`, `MilestonePlayer.cs` (PerkLabel),
  `tests/MilestoneRewardsSourceSmokeTest.ps1`.
- Cambios: cada subclase con RECURSO/sistema propio ahora escala su mecanica con la
  cantidad de milestones (leyendo `MilestonePlayer.Milestones`), encima del stat:
  - Necromancer: +1 slot de minion cada 2 milestones (`MaxNecroSlots`).
  - Swordsman: +Rastro Carmesi ganado por golpe (= n de milestones).
  - Berserker: +tope de Rage (`MaxRage` 100 -> 100 + milestones*10; mas Overrage y
    mas dano escalado por rage).
  - Cursed Mage: +regen de energia maldita (solo cuando ya regenera, para no
    romper el acople con la corrupcion).
  - `PerkLabel` actualizado en la UI para reflejar la mecanica.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 70/70.
- Pendientes/riesgos: SIN probar in-game. Las subclases sin recurso propio siguen
  con el bonus tematico de stat. Se puede extender a Fighter (combo), Elementalist
  (carga) y las de Ranger si se quiere.

## 2026-07-08 - Milestones: cada 5 nodos, algo especial

- Objetivo (idea del usuario): cada 5 pasivas desbloqueadas, desbloquear algo
  especial.
- Archivos: `Content/Players/MilestonePlayer.cs` (nuevo),
  `Content/UI/MilestoneBannerUI.cs` (nuevo), `Content/Progression/ProgressionService.cs`,
  `Content/UI/PassiveUI.cs` (sidebar), `tests/MilestoneRewardsSourceSmokeTest.ps1`.
- Cambios:
  - Cada 5 pasivas desbloqueadas = 1 MILESTONE (`NodesPerMilestone = 5`).
  - Cada milestone da un bonus ESCALADO tematico segun la SUBCLASE activa (no la
    clase base): p.ej. Swordsman = melee + penetracion, Guardian = defensa,
    Berserker = dano melee, Elementalist = dano magico, Infinity Mage = mana +
    eficiencia, Necromancer = dano de invocacion, etc. (`MilestonePlayer`,
    switch por `SubclassPlayer.CurrentSubclass`). Antes de promocionar, cae a un
    bonus generico de la clase (por Soul). `PerkLabel` da la etiqueta para la UI.
  - Momento especial: al alcanzar un milestone, `ProgressionService` dispara un
    banner dorado (`MilestoneBannerUI`, "MILESTONE X") + sonido.
  - El sidebar del panel muestra "Milestones: X (n/5)" + el bonus acumulado.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 70/70.
- Pendientes/riesgos: SIN probar in-game. Numeros ajustables (4%/2 crit por
  milestone). Idea futura: milestones con perks DISTINTOS/elegibles (keystones) en
  vez de solo escalado.

## 2026-07-08 - Passive Tree: conectores rectos + formas de rama variadas

- Objetivo (feedback con screenshot): los conectores diagonales se entrelazaban y
  todas las ramas se veian iguales.
- Causa del enredo: `DrawConnector` rutea en angulo recto por el punto medio (Z);
  en ramas diagonales sus tramos horizontales cruzaban los verticales de otros.
- Archivos: `Content/UI/EterniaUI.cs` (nuevo `DrawLine`), `Content/UI/PassiveUI.cs`
  (conectores rectos), `Content/Passives/PassiveRegistry.cs` (BranchStyle/TierSize),
  `tests/PassiveDiamondTreeSourceSmokeTest.ps1`.
- Cambios:
  - `EterniaUI.DrawLine` = linea recta (rotada) entre dos puntos. Los conectores del
    arbol ahora son ARISTAS RECTAS tipo telarana PoE: un diamante se ve como una V/â—‡
    limpia, sin escaleras que se cruzan. (Se conserva el glow de camino tomado y el
    pulso de disponible.)
  - Formas de rama VARIADAS: `BranchStyle` (deterministico por afinidad) elige entre
    4 patrones de `TierSize` -> diamante en escalera, linea recta, diamantes
    espaciados, o split temprano + linea. Asi las ramas no se ven todas iguales
    (y varian su largo). Un tier de 2 nodos siempre lo sigue uno de 1 (merge limpio).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 69/69.
- BUGFIX (reportado con screenshot): la 1a version de `DrawLine` escalaba
  `MagicPixel` por `(length, thickness)`; como esa textura no es 1x1, dibujaba
  cuÃ±as gigantes que llenaban la pantalla. Corregido dividiendo la escala por las
  dimensiones reales de la textura (`length / pixel.Width`, `thickness / pixel.Height`).
- Pendientes/riesgos: SIN probar in-game. Ajustable: `laneSpacing`/`nodeSpacing`
  si un diamante queda muy junto/separado, grosor de linea, y el reparto de estilos.

## 2026-07-08 - Passive Tree: estructura de diamantes (requiere DOS para el siguiente)

- Objetivo (feedback del usuario): el arbol se veia lineal (cada nodo pedia solo al
  anterior); querer que a veces necesites desbloquear DOS juntos para el siguiente.
- Archivos: `Content/Passives/PassiveRegistry.cs` (BuildTiers/GetBranch/GetPrerequisites),
  `Content/Progression/ProgressionService.cs`, `Content/UI/PassiveUI.cs`,
  `tests/PassiveDiamondTreeSourceSmokeTest.ps1` (nuevo).
- Cambios:
  - Estructura de TIERS derivada del orden de la rama: tier par = 1 nodo, tier impar
    = 2 nodos, alternando. Los prerequisitos de un nodo son TODOS los nodos del tier
    anterior -> tras un tier de 2 nodos, el siguiente requiere AMBOS (merge/gate).
    Sin re-cablear a mano los 145 nodos: se deriva del orden (`BuildTiers`,
    `GetPrerequisites`).
  - `ProgressionService.TryUnlockPassive` y `GetPassiveState` (UI) ahora exigen TODOS
    los prerequisitos (ya no un solo `RequiredPassive`, que queda vestigial).
  - Layout radial: los 2 nodos de un tier se abren en perpendicular al brazo
    (`laneSpacing`) formando un DIAMANTE que el siguiente nodo vuelve a juntar.
  - Conectores: una linea por prerequisito -> un merge muestra DOS lineas
    convergiendo (con el mismo highlight de camino tomado / pulso de disponible).
  - Efecto secundario: las ramas quedan mas compactas (menos tiers que nodos).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 69/69.
- Pendientes/riesgos: SIN probar in-game. `GetPrerequisites` aloca listas por
  frame en el panel abierto (menu, no gameplay; optimizable si se nota). Las ramas
  de 8 nodos dan 2 diamantes + cola; las de 9 (Bleed) dan 3 diamantes.

## 2026-07-08 - Passive Tree: juice RPG (camino iluminado + halos + hub)

- Objetivo (feedback del usuario): la web radial se veia bien pero le faltaba algo
  para sentirse mas RPG.
- Archivos: `Content/UI/PassiveUI.cs` (`DrawTree` + `DrawPassiveNode`).
- Cambios (solo visual, sin tocar mecanica):
  - Conectores por estado de asignacion: el CAMINO tomado se ilumina (glow + nucleo
    brillante), el siguiente nodo disponible PULSA para invitarlo, el resto queda
    tenue. Da el feel de "tu build se enciende" de PoE.
  - Nodos: halo suave detras de los tomados (glow fijo) y de los disponibles (pulso
    animado), marco exterior por estado (disponible pulsa, tomado brillante, resto
    tenue) + bisel interior blanco. Todo con `GlobalTimeWrappedHourly`.
  - Nucleo "CORE" con doble glow pulsante y borde en color de clase (se pasa
    `accent` a `DrawTree`).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68.
- Pendientes/riesgos: SIN probar in-game. Siguiente nivel de RPG (opcional):
  iconos por nodo (requiere texturas), fondo arcano/vignette, o zoom.

## 2026-07-08 - Passive Tree estilo Path of Exile (web radial paneable)

- Objetivo (pedido del usuario): que el arbol se sienta mas RPG, tipo PoE.
  Se eligio "web radial paneable" (reusa la logica; sin zoom en v1).
- Archivos: `Content/UI/PassiveUI.cs` (`BuildLayouts` + `DrawTree` + estado de pan
  + `DrawPassiveNode` con `allowClicks`),
  `tests/ResponsiveUILayoutSourceSmokeTest.ps1` (asserts hubRadius/nodeSpacing/panX).
- Cambios (estructura):
  - `BuildLayouts` ahora es PROCEDURAL y radial: cada rama de afinidad es un BRAZO
    que irradia desde un nucleo central (`hubRadius` 170, `nodeSpacing` 122); los
    nodos avanzan hacia afuera por tier. Posiciones en espacio de lienzo (hub 0,0).
    Sin colocar nodos a mano; escala solo al agregar nodos (brazo mas largo).
  - `DrawTree` = lienzo PANEABLE: se arrastra para moverse (`panX/panY`, con clamp a
    `reach`). Conectores nodo->prerequisito (o al hub) tipo telarana. Nucleo "CORE"
    al centro. Culling: solo se dibujan/clickean las tarjetas completamente dentro
    del area (sin tocar el SpriteBatch -> sin riesgo de scissor).
  - Distincion arrastre vs click: un drag (>6px) NO cuenta como click de nodo
    (`allowClicks = !dragMoved` pasado a `DrawPassiveNode`).
  - Tarjetas de nodo compactas (112x40, estilo solo-texto: nombre + click).
  - La mecanica no cambia (afinidad, promocion por afinidad dominante, requiredPassive).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68.
- Pendientes/riesgos: SIN probar in-game. Knobs a ajustar segun se sienta:
  `hubRadius`/`nodeSpacing`/`cardWidth`/`cardHeight`, velocidad de pan, umbral de
  drag (6px), y el culling "completo" deja zona muerta ~1 tarjeta en los bordes.
  Sin ZOOM (siguiente paso si se quiere). Hub dice "CORE" (placeholder).

## 2026-07-08 - Passive Tree scrollable (nodos de altura fija + scroll)

- Objetivo (pedido del usuario): que la interfaz del arbol escale a MUCHOS nodos a
  futuro sin verse mal (dejar de encoger nodos).
- Archivos: `Content/UI/PassiveUI.cs` (`BuildLayouts` + `DrawTree`),
  `tests/ResponsiveUILayoutSourceSmokeTest.ps1` (asercion `treeScroll`).
- Cambios (estructura):
  - Los nodos ahora usan una altura FIJA legible (clamp `NodeMinHeight..NodeMaxHeight`);
    `maxFittingNodeHeight` decide si caben sin scroll, pero nunca se encogen por
    debajo del minimo.
  - Las filas se dimensionan al CONTENIDO y se apilan; si el total supera el area,
    el arbol hace SCROLL vertical (rueda del mouse sobre el area), con barra
    indicadora. Cuando todo cabe, scroll = 0 y se ve igual que antes.
  - Dibujo con culling manual: solo se dibujan/clickean los nodos completamente
    dentro del area (sin tocar el estado del SpriteBatch -> sin riesgo de scissor).
    Fondos de grupo recortados via `Rectangle.Intersect`; se quitaron los bordes de
    caja (eran los que se desbordaban).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68.
- Pendientes/riesgos: SIN probar in-game (no puedo ejecutar el juego). Posibles
  ajustes: (a) direccion de la rueda (`treeScroll -=`), invertir si va al reves;
  (b) el culling "completamente dentro" deja una pequena zona muerta (~1 nodo) en
  los bordes al hacer scroll (efecto lista que encaja); si molesta, se puede pasar
  a clipping por scissor. La velocidad de scroll es `delta / 4`.

## 2026-07-08 - Arbol de pasivas extenso para v1 (todas las ramas a 8+)

- Objetivo (pedido del usuario): para v1 el arbol debe sentirse extenso, no corto.
  Se profundizaron las 17 ramas que seguian en 5 nodos (igual que Bleed).
- Archivos: `Content/Passives/PassiveRegistry.cs` (+51 nodos),
  `Content/Players/EterniaStatsPlayer.cs` (+51 efectos),
  `tests/PassiveTreeDepthSourceSmokeTest.ps1` (conteos).
- Cambios:
  - +3 nodos por rama (tiers 6-8, afinidad 8/9/10) a Combo, Defense, Precision,
    Rage, Control (Warrior); Energy, Bow, Gun, Music (Ranger); Elemental, Curse,
    Infinity, Arcane (Mage); Beast, Fusion, Tech, Shadow (Summoner).
  - Cada nodo con efecto real en `EterniaStatsPlayer` (dano/crit/velocidad/defensa/
    penetracion/mana/+minion/vida) segun el tema de la rama.
  - Totales: Warrior 49 (Bleed 9 + 5x8), Ranger/Mage/Summoner 32 c/u (4x8).
    145 nodos de pasiva en total. La rama Bleed sigue mas profunda (9) como
    vitrina del Espadachin; la promocion sigue siendo relativa, no se rompe.
  - El layout de filas proporcionales absorbe las ramas mas largas sin desbordar.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68
  (cobertura confirma que los 51 nodos tienen efecto en runtime).

## 2026-07-08 - Passive Tree: filas proporcionales (arreglo de desborde)

- Objetivo (bug reportado con screenshot): la rama Bleed (9 nodos) se desbordaba y
  pisaba la rama de abajo (Precision). Debia escalar para futuras ramas mas grandes.
- Causa: `BuildLayouts` usaba grilla de altura de fila FIJA (treeHeight/rows) +
  `maxFittingNodeHeight = Math.Max(24, fittedNodeHeight)` que forzaba 24px por nodo
  aunque cupieran menos, empujando los nodos fuera de la caja del grupo.
- Archivos: `Content/UI/PassiveUI.cs` (`BuildLayouts`),
  `tests/ResponsiveUILayoutSourceSmokeTest.ps1` (asercion `rowMaxNodes`).
- Cambios (cambio de estructura):
  - Cada FILA se dimensiona proporcional a su rama mas larga (`rowMaxNodes` -> peso),
    asi una rama densa reclama mas altura en vez de desbordar la fila de abajo.
  - La altura de nodo se ajusta a la rama mas larga de la fila (columnas alineadas);
    `maxFittingNodeHeight` ya no fuerza un piso de 24 (nunca rebasa la caja).
  - Se quito el bloque que forzaba `NodeMinHeight`. Piso legible bajado a 22, pero
    siempre acotado por lo que realmente cabe.
  - Resultado: sin solape; una rama de 9 nodos entra exacta en su fila y escala si
    crecen mas nodos (los nodos se encogen gradualmente, sin pisarse).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68.
- Pendientes/riesgos: con MUCHISimos nodos por rama a futuro, los nodos se harian
  pequenos; si llega ese caso, agregar scroll vertical al area del arbol.

## 2026-07-08 - Rama Bleed ampliada (4 nodos de potencia + caps de afinidad)

- Objetivo (pedido del usuario): agregar mas al arbol de pasivas del Bleed. Se hizo
  sin re-inflar el proc (que se acababa de nivelar): los nodos suben POTENCIA del
  sangrado, no probabilidad, y se acota el aporte de afinidad.
- Archivos: `Content/Passives/PassiveRegistry.cs` (rama Bleed 5 -> 9 nodos),
  `Content/Players/WarriorBleedPlayer.cs`, `Content/NPCs/BleedGlobalNPC.cs`,
  `tests/BleedPassiveTreeSourceSmokeTest.ps1` (nuevo),
  `tests/PassiveTreeDepthSourceSmokeTest.ps1` (Warrior 30 -> 34).
- Cambios:
  - 4 nodos nuevos en la rama Bleed (encadenados tras Crimson Reaper):
    - `Rupture` (+5 dano de sangrado, en `BleedGlobalNPC`).
    - `Hemoplague` (+2s de duracion).
    - `Exsanguinate` (+15% dano vs sangrantes, apila con `Execution`).
    - `Bloodthirst` (curas al golpear enemigos sangrantes; ~1/5 hits, +2 HP).
  - Caps de afinidad para que un arbol profundo no dispare los numeros:
    - chance: `Math.Min(affinity / 2, 10)`,
    - duracion: `Math.Min(affinity, 20) * 6`,
    - DoT: `Math.Min(affinity, 20)`.
  - La rama Bleed queda mas profunda que las demas (9 vs 5), tematico para el
    camino del Espadachin; la promocion sigue siendo relativa, no se rompe.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 68/68.

## 2026-07-08 - Nivelacion del chance de sangrado (estaba muy alto)

- Objetivo (feedback del usuario): el % de aplicar sangrado se sentia muy alto,
  agravado porque la afinidad Bleed se sumaba 1:1 (una build full daba +25%).
- Archivos: `Content/Globals/EterniaGlobalItem.cs` (tabla vanilla),
  `Content/Players/WarriorBleedPlayer.cs` (aporte de afinidad),
  las 6 espadas insignia (`BleedChance`), `tests/SwordsmanBleedSourceSmokeTest.ps1`.
- Cambios:
  - Chances base reducidos ~a la mitad. Insignia: TrainingBlade 25->10,
    SerratedIronBlade 32->16, HuntersWarblade 28->12, Thornrender 36->20,
    MoltenGutripper 35->16, BloodletterBlade 45->22. Vanilla curadas tambien
    (p.ej. BloodButcherer 30->16, DeathSickle 35->18, Katana 18->10, ...).
  - La afinidad Bleed ahora aporta `affinity / 2` al chance (antes 1:1), asi una
    build maxeada suma +12 en vez de +25.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 67/67 (test afirma `affinity / 2`).
- Pendientes/riesgos: ajustable; si aun se siente alto/bajo, tunear base o el divisor.

## 2026-07-08 - Arsenal pre-hardmode del Espadachin (4 espadas de sangrado)

- Objetivo (pedido del usuario): dar variedad de espadas pre-hardmode para el
  camino del Guerrero/Espadachin, con distintos nichos.
- Archivos: `Content/Items/Weapons/Warrior/SerratedIronBlade.cs`,
  `HuntersWarblade.cs`, `Thornrender.cs`, `MoltenGutripper.cs` (nuevos),
  `en-US.hjson` (nombres/tooltips), `tests/SwordsmanArsenalSourceSmokeTest.ps1`.
- Cambios: 4 espadas craftables `IBleedWeapon` (no subclass-locked, usables pre-HM),
  cada una con nicho y chance de sangrado insignia:
  - SerratedIronBlade: rapida-sangradora. dmg 13 / useTime 17 / White / bleed 32.
    Receta: IronBar 8 + Wood 5 (Anvil).
  - HuntersWarblade: equilibrada. dmg 16 / 21 / Blue / bleed 28. IronBar 12 + Wood 10.
  - Thornrender: especialista en sangrado (jungla). dmg 19 / 23 / Blue / bleed 36.
    Stinger 8 + JungleSpores 6 + Vine 3.
  - MoltenGutripper: pesada (infierno). dmg 27 / 25 / Orange / bleed 35.
    HellstoneBar 12 + Wood 5.
  - Todas usan `CanInflictBleed` (via IBleedWeapon) y muestran su % en el tooltip.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 67/67.
- Pendientes/riesgos: texturas placeholder (override Texture al sprite del
  TrainingGauntlet, como TrainingBlade/BloodletterBlade). Recetas usan el grupo
  vanilla "IronBar" (hierro/plomo).

## 2026-07-08 - Pase de balance de armas del mod

- Objetivo: revisar y balancear las armas del mod (4 starters de clase + 18
  recompensas de promocion) buscando outliers e inconsistencias.
- Hallazgos: la distribucion de DPS estaba mayormente bien (gradiente ~22-46 con
  subclases de utilidad abajo y de dano arriba). Outliers/inconsistencias puntuales:
  - `InfinityTome` era el DPS mas alto del tier (15 dmg / 18 = 50), por encima del
    melee y con seguridad de rango.
  - `TrainingShield` con 9 de dano, muy bajo incluso para arma defensiva.
  - `CursedApprenticeTome` y `BeginnerNecromancyBook` eran rareza White pese a ser
    recompensas de promocion (el resto son Blue).
- Cambios:
  - `InfinityTome` dmg 15 -> 13 (50 -> ~43 DPS, en linea con Pistol/Bloodletter;
    mantiene su identidad rapida/eficiente por el mana bajo del Infinity Mage).
  - `TrainingShield` dmg 9 -> 11 (sube el piso; sigue siendo el melee mas bajo,
    coherente con el rol tanque del Guardian).
  - `CursedApprenticeTome` y `BeginnerNecromancyBook` White -> Blue (consistencia).
    El mana gratis del Cursed Tome es intencional (usa CursedEnergy), se dejo.
  - No se toco el resto: la variedad de DPS por rol/subclase es intencional.
- Archivos: `Content/Items/Weapons/Promotion/InfinityTome.cs`,
  `Content/Items/Weapons/Guardian/TrainingShield.cs`,
  `Content/Items/Weapons/Magic/CursedApprenticeTome.cs`,
  `Content/Items/Weapons/Summoner/BeginnerNecromancyBook.cs`,
  `tests/WeaponBalanceSourceSmokeTest.ps1` (nuevo, invariantes de rareza),
  `tests/PromotionRewardsSourceSmokeTest.ps1` (afloje del assert de dano fijo).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 66/66.

## 2026-07-08 - Sangrado: lista curada de espadas (no todas) con chance base

- Objetivo (pedido del usuario): que no todas las espadas sangren, sino una
  seleccion tematica de espadas vanilla, y que algunas ya traigan un chance base.
- Archivos: `Content/Globals/EterniaGlobalItem.cs`,
  `Content/Players/WarriorBleedPlayer.cs`, `Content/Players/SwordsmanPlayer.cs`,
  `tests/SwordsmanBleedSourceSmokeTest.ps1`, `docs/gameplay-systems.md`.
- Cambios:
  - `EterniaGlobalItem.IsSword` (aceptaba cualquier espada) se reemplazo por una
    **tabla curada** `VanillaBleedSwords` (ItemID -> chance base) + `CanInflictBleed`.
  - Espadas elegidas (tematicas): IronBroadsword 8, LeadBroadsword 8, BladeofGrass
    15, FalconBlade 16, Katana 18, Muramasa 20, Cutlass 20, LightsBane 20,
    BloodButcherer 30, Bladetongue 30, NightsEdge 22, TrueNightsEdge 30, Seedler 25,
    DeathSickle 35. Mas las insignia del mod (IBleedWeapon: TrainingBlade 25,
    BloodletterBlade 45). El resto de espadas ya **no** sangran.
  - `WarriorBleedPlayer`/`SwordsmanPlayer` usan `CanInflictBleed` en vez de `IsSword`.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 65/65 (test reescrito).
- Pendientes/riesgos: lista ajustable a gusto (agregar/quitar ItemID o tunear
  chances). El tooltip solo se muestra a Guerreros activos.

## 2026-07-08 - Sangrado: solo espadas + porcentaje visible

- Objetivo (pedido del usuario): el sangrado debe aplicarse **solo con espadas** y
  el **porcentaje** de aplicarlo debe ser **visible** (revierte el diseno previo de
  "arma de filo marcada" + "probabilidad oculta").
- Archivos: `Content/Globals/EterniaGlobalItem.cs` (antes vacio),
  `Content/Players/WarriorBleedPlayer.cs`, `Content/Players/SwordsmanPlayer.cs`,
  `Content/Items/IBleedWeapon.cs`, `tests/SwordsmanBleedSourceSmokeTest.ps1`,
  `docs/gameplay-systems.md`.
- Cambios:
  - `EterniaGlobalItem.IsSword` = deteccion de espada real (Swing/Rapier, melee,
    sin pick/axe/hammer; excluye lanzas/manguales/yoyos/latigos/herramientas).
    Ahora **cualquier espada** (vanilla o del mod) puede aplicar sangrado, no solo
    las 2 armas marcadas.
  - `IBleedWeapon` pasa de "chance oculto por arma" a **override de chance para
    espadas insignia**; `DefaultSwordBleedChance` (15) para el resto.
  - `EterniaGlobalItem.ModifyTooltips` muestra "`X% chance to inflict Bleed`" en el
    tooltip de las espadas cuando el jugador es Guerrero activo (chance efectivo =
    base + afinidad Bleed).
  - `WarriorBleedPlayer`/`SwordsmanPlayer` usan `IsSword` en vez del marcador.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 65/65 (test reescrito,
  rojo->verde).
- Pendientes/riesgos: si el usuario queria SOLO las espadas del mod (no vanilla),
  restringir `IsSword` a `IBleedWeapon`. Heuristica de espada podria incluir/excluir
  algun caso borde (shortswords incluidas via estilo Rapier).

## 2026-07-08 - Espadachin Fase 2: Rastro Carmesi (recurso + tecnica + barra)

- Objetivo: dar identidad al Espadachin con su recurso exclusivo, cerrando el
  rediseno. El Sangrado (Fase 1) ya es del Guerrero; el Espadachin lo explota.
- Archivos principales:
  - `Content/Players/CrimsonTrailPlayer.cs` (nuevo, recurso + save/load).
  - `Content/Players/SwordsmanSkillPlayer.cs` (nuevo, tecnica con SkillKey).
  - `Content/UI/CrimsonTrailUI.cs` (nueva barra, clon de BerserkerUI).
  - `Content/Players/SwordsmanPlayer.cs` (gana recurso en golpes de filo).
  - `tests/CrimsonTrailSourceSmokeTest.ps1` (nuevo), `docs/gameplay-systems.md`.
- Cambios:
  - Rastro Carmesi = recurso **exclusivo del Espadachin** (cap 100). Solo acumula
    con `IsActiveSwordsman`; si no, `ResetEffects` lo pone en 0. **Sin auto-regen**:
    solo se gana al golpear con arma de filo (12 primera sangre / 6 sostener).
    `Add`/`TrySpend` + `SaveData`/`LoadData`. Logica separada del sangrado.
  - Tecnica **Crimson Execution** (`SwordsmanSkillPlayer`, tecla Q + cooldown
    compartido): gasta 50 y golpea a todos los sangrantes cercanos con un burst
    (el "EXECUTE!" renacido, `SimpleStrikeNPC` escalado por afinidad Bleed).
  - Barra `CrimsonTrailUI` solo-Espadachin; se enciende (`Q: EXECUTE`) al tener
    recurso suficiente.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 65/65
  (nuevo `CrimsonTrailSourceSmokeTest`, rojo->verde por TDD).
- Pendientes/riesgos: el burst usa `SimpleStrikeNPC` (netcode propio de tML);
  la barra/tecnica no requieren sync extra (recurso local del jugador). Falta
  probar in-game el flujo completo (aplicar bleed -> cargar -> Q).

## 2026-07-08 - Espadachin Fase 1: Sangrado como mecanica del Guerrero

- Objetivo: rediseno del Espadachin (spec del usuario). Fase 1 = convertir el
  Sangrado en mecanica del Guerrero (class-wide), no exclusiva del Espadachin.
- Archivos principales:
  - `Content/Buffs/BleedDebuff.cs` (nuevo ModBuff) + `BleedDebuff.png` (placeholder).
  - `Content/Items/IBleedWeapon.cs` (nueva interfaz, `BleedChance` oculto).
  - `Content/Players/WarriorBleedPlayer.cs` (nuevo, aplicacion class-wide).
  - `Content/NPCs/BleedGlobalNPC.cs`, `Content/Players/SwordsmanPlayer.cs`,
    `Content/Players/SubclassEffectsPlayer.cs` (refactor).
  - `Content/Items/Weapons/Warrior/TrainingBlade.cs`,
    `Content/Items/Weapons/Promotion/BloodletterBlade.cs` (marcadas IBleedWeapon).
  - `en-US.hjson` (localizacion del debuff), `docs/gameplay-systems.md`,
    `tests/SwordsmanBleedSourceSmokeTest.ps1` (reescrito).
- Cambios:
  - El Sangrado ahora es un **debuff real** (`BleedDebuff`, visible en el enemigo),
    de **un solo nivel** y **dano fijo** (`BaseBleedDamage` 6 + afinidad Bleed).
  - Solo lo aplican **armas de filo compatibles** (`IBleedWeapon`) con una
    **probabilidad oculta por arma** (TrainingBlade 25, BloodletterBlade 45),
    rodada en `WarriorBleedPlayer` para cualquier Warrior Soul activa. La afinidad
    Bleed sube chance y duracion; `Blood Flow` alarga; `Execution` da dano vs
    sangrantes (todo Warrior-wide).
  - El **Espadachin** es la maestria: garantiza el sangrado en cada golpe de filo
    (`SwordsmanPlayer.OnHitNPCWithItem` gateado por `IsActiveSwordsman`).
  - Se retiro el sistema de 5 stacks + "EXECUTE!" de `SubclassEffectsPlayer`
    (se reconvertira en tecnica del Rastro Carmesi en la Fase 2).
- Verificacion: `dotnet build -t:Compile` 0/0; suite 64/64.
- Pendientes/riesgos: Fase 2 = recurso Rastro Carmesi + barra UI + tecnica(s).
  Sync MP del debuff/timer es best-effort (cliente-driven, como antes); revisar si
  se quiere autoridad de servidor. Textura del debuff es placeholder.

## 2026-07-08 - Arboles de pasivas ampliados (3 -> 5 por rama)

- Objetivo: cada rama de afinidad topaba en 3 nodos ("solo hay 3 mejoras por
  subclase"); dar profundidad y decisiones de build.
- Archivos principales: `Content/Passives/PassiveRegistry.cs`,
  `Content/Players/EterniaStatsPlayer.cs`,
  `tests/PassiveTreeDepthSourceSmokeTest.ps1` (nuevo).
- Cambios: se agregaron 2 nodos (tier 4 y 5) a las 18 ramas -> 36 pasivas nuevas,
  90 nodos en total. Escalado uniforme: tier 4 = coste 2 / afinidad 6, tier 5 =
  coste 3 / afinidad 7, encadenados por `requiredPassive`. Cada nodo nuevo aplica
  un efecto real en `EterniaStatsPlayer` (dano, crit, velocidad, defensa,
  penetracion, mana o +minion) que coincide con su descripcion.
- Verificacion: `dotnet build -t:Compile` 0/0; suite 64/64
  (nuevo `PassiveTreeDepthSourceSmokeTest`, rojo->verde por TDD).
- Pendientes/riesgos: balance sin probar in-game; varias ramas de Summoner dan
  +1 minion (stackeable). Revisar el scroll de `PassiveUI` con 5 filas por rama.

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
    y stats. Tanques con daÃ±o moderado: se respeto el guard de no-8x vida / 4x
    daÃ±o-defensa en enemigos NORMALES; los bosses escalan un poco mas.
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

