# Claude Code workflow

Este archivo define como debe trabajar Claude Code en este repo. No reemplaza
las instrucciones del usuario; las hace operativas dentro del proyecto.

## Al iniciar una sesion

1. Leer:
   - `docs/README.md`
   - `docs/ai-handoff.md`
   - `docs/current-state.md`
   - `docs/roadmap-known-issues.md`
2. Revisar estado real:

```powershell
git status --short
dotnet build .\ETERNIA.csproj
```

3. Si la tarea toca gameplay, leer `docs/gameplay-systems.md`.
4. Si la tarea toca UI, leer `docs/ui-rework.md`.
5. Si la tarea toca estructura, leer este archivo completo.

## Registro obligatorio de cambios

Cada bloque de trabajo no trivial debe actualizar `docs/change-log.md` con:

- fecha;
- objetivo;
- archivos tocados;
- resumen de cambio;
- verificacion corrida;
- pendientes nuevos o riesgos detectados.

No registrar microcambios mecanicos que se deshacen en la misma sesion. Si el
cambio queda en el worktree, debe aparecer en el log.

## Registro obligatorio de decisiones

Actualizar `docs/decision-log.md` cuando se decida algo que afecte direccion del
mod, arquitectura o contenido. Ejemplos:

- quitar o conservar una clase/promocion;
- mover sistemas entre carpetas;
- cambiar reglas de penalizacion;
- renombrar clases, Souls, stats o afinidades;
- decidir que un sistema queda temporalmente como deuda.

Cada decision debe incluir:

- estado: propuesta, aceptada, reemplazada o descartada;
- contexto;
- decision;
- consecuencias;
- archivos afectados o por afectar.

## Reorganizacion de estructura

Antes de mover archivos:

1. Identificar referencias con CodeGraph o `rg`.
2. Revisar namespace y paths de assets.
3. Revisar tests que esperan rutas exactas.
4. Hacer el move en un cambio separado cuando sea posible.
5. Actualizar docs que mencionen rutas.
6. Correr build y smoke tests relevantes.

No reorganizar por estetica si no reduce deuda real. La estructura actual ya
esta en movimiento; evitar churn sin beneficio.

## Reglas para nuevos archivos

Usar esta organizacion:

- Souls y reglas puras de Souls: `Content/Souls`.
- Items de Soul: `Content/Items/Souls`.
- Reglas puras de progresion: `Content/Progression`.
- Estado por jugador: `Content/Players`.
- UI reusable o panels: `Content/UI`.
- Weapons por dominio:
  - base class weapons: `Content/Items/Weapons/Warrior`, `Magic`, `Ranger`,
    `Summoner`;
  - promotion weapons: `Content/Items/Weapons/Promotion`.
- Projectiles por dominio:
  - `Content/Projectiles/Summoner`;
  - `Content/Projectiles/Necromancer`;
  - otros bajo `Content/Projectiles` si no hay dominio aun.
- Tests source-level: `tests`.

Si una carpeta nueva parece necesaria, documentar la razon en
`docs/change-log.md`.

## Reglas para cambios de gameplay

- No eliminar penalizaciones de Soul.
- No reintroducir Cartomancer.
- No permitir efectos de subclase sin Soul activa correcta.
- Preferir helpers `IsActive...` en `ModPlayer` para checks runtime.
- Recompensas de promocion deben validar `ClassPromotionRules.IsPromotionForSoul`.
- Proyectiles persistentes deben revalidar owner/Soul/subclase antes de aplicar
  dano o efectos.

## Reglas para cambios de UI

- Usar `EterniaUI` para paneles, botones, tooltips, texto y colores.
- Paneles grandes deben ser mutuamente exclusivos via
  `EterniaUI.CloseMajorPanelsExcept`.
- UI player-bound debe revisar `EterniaUI.ShouldDrawPlayerUI(player)`.
- Evitar coordenadas absolutas sin clamp.
- Evitar texto largo sin trim/wrap.
- Si se agrega UI nueva, agregar o actualizar smoke test de UI.

## Reglas de tests

Para bugfix o comportamiento nuevo:

1. Crear o actualizar un test primero.
2. Verlo fallar por el motivo correcto.
3. Implementar el cambio.
4. Ver el test pasar.
5. Correr build.
6. Correr suite completa si el cambio toca sistemas compartidos.

## Entrega de una sesion

Antes de terminar:

1. Actualizar `docs/change-log.md`.
2. Actualizar `docs/decision-log.md` si hubo decisiones.
3. Correr verificacion adecuada.
4. Reportar claramente:
   - que cambio;
   - que se verifico;
   - que queda pendiente;
   - si hubo algo no probado en tModLoader.

