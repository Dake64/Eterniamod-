# UI rework

## Objetivo

La UI anterior tenia paneles simples, coordenadas fijas, malos layouts y riesgo
de overflow. El rework actual busca:

- paneles consistentes;
- texto recortado o envuelto;
- estados vacios;
- botones con feedback;
- close buttons;
- tooltips;
- layouts responsive segun viewport;
- paneles grandes mutuamente exclusivos.

## Helper central

Archivo:

- `Content/UI/EterniaUI.cs`

Funciones principales:

- `GetCenteredPanel`
- `GetBottomLeftPanel`
- `ClampToScreen`
- `ShouldDrawPlayerUI`
- `CloseMajorPanelsExcept`
- `DrawPanel`
- `DrawHeader`
- `DrawButton`
- `DrawCloseButton`
- `DrawPill`
- `DrawProgressBar`
- `DrawTooltip`
- `DrawTrimmedText`
- `WrapText`
- `DrawConnector`

Al crear UI nueva, usar estos helpers antes de dibujar a mano.

## Paneles grandes

Paneles grandes actuales:

- `SoulUI`
- `StatsUI`
- `PassiveUI`

Regla:

- Solo uno debe estar abierto a la vez.
- Al abrir uno, llamar `EterniaUI.CloseMajorPanelsExcept`.
- `SoulUI` debe cerrarse via `SoulUISystem.CloseSoulPanel()` para limpiar el
  `UserInterface` state.

## Passive UI

Archivo:

- `Content/UI/PassiveUI.cs`

Estado:

- Rehecha con panel centrado, sidebar, tree area, nodos, estado sin Soul,
  estado sin pasivas, tooltips y botones.
- Usa `PassiveRegistry.GetPassivesForSoul`.
- Usa `ProgressionService.TryUnlockPassive`.
- Requiere Class Soul activa para gastar pasivas.

Riesgo pendiente:

- Algunos textos de estado vacio pueden necesitar wrapping mas estricto en
  viewports extremadamente estrechos.

## Stats UI

Archivo:

- `Content/UI/StatsUI.cs`

Estado:

- Panel centrado.
- Summary pills para level/stat/passive points.
- Rows de stats con boton `+`.
- Usa `ProgressionService.TrySpendStatPoint`.
- Tiene close button y exclusividad con otros paneles.

## Soul UI

Archivos:

- `Content/UI/SoulUI.cs`
- `Content/UI/SoulUISystem.cs`

Estado:

- Panel draggable.
- Close button.
- Usa `ShouldDrawPlayerUI`.
- Mantiene `mouseInterface` mientras se hoverea o arrastra.
- Se cierra limpiando `UserInterface` state.

## Overlays y barras

Archivos relevantes:

- `BaseClassResourceUI.cs`
- `ArcherFocusUI.cs`
- `BerserkerUI.cs`
- `CursedMageUI.cs`
- `ElementalistUI.cs`
- `EnergyHeatUI.cs`
- `FighterComboUI.cs`
- `GunnerUI.cs`
- `NecromancerUI.cs`
- `StunnerChargeUI.cs`
- `VirtuosoUI.cs`

Estado:

- Usan `EterniaUI.ShouldDrawPlayerUI`.
- La mayoria usa helpers `IsActive...` para evitar mostrar estado obsoleto de
  subclase si la Soul activa no corresponde.

Riesgo pendiente:

- Varios overlays anclados al jugador usan offsets fijos y no tienen helper
  comun de clamp/flip. En resoluciones chicas, UI scale alto o cerca de bordes
  de pantalla pueden quedar cortados.

Correccion recomendada:

- Crear en `EterniaUI` un helper de overlay anclado al jugador:
  - input: posicion world/screen, tamano, offset preferido;
  - output: rectangulo clampado a pantalla;
  - opcion de flip arriba/abajo si no cabe.
- Migrar primero:
  - `BaseClassResourceUI`
  - `ArcherFocusUI`
  - `GunnerUI`
  - `EnergyHeatUI`
  - `FighterComboUI`
  - `StunnerChargeUI`
  - `VirtuosoUI`

## Stack bottom-left

Paneles como XP, class progression y algunos recursos usan margenes bottom-left
manuales. Hoy pasan smoke tests, pero no hay stack manager real.

Correccion recomendada:

- Crear helper para reservar slots bottom-left con altura + gap.
- Evitar que cada panel tenga numeros magicos independientes.

