# Arquitectura tecnica

## Mapa de carpetas

- `Content/Buffs` - debuffs de penalizacion y buffs propios.
- `Content/Globals` - hooks globales.
- `Content/Items/Souls` - items de Souls.
- `Content/Items/Weapons` - armas base, promocionales y de practica.
- `Content/NPCs` - NPCs y global NPC behavior.
- `Content/Passives` - registro de pasivas.
- `Content/Players` - la mayoria del estado runtime del jugador.
- `Content/Progression` - reglas puras de progresion, afinidad y gasto.
- `Content/Projectiles` - proyectiles de armas, whips y minions.
- `Content/Souls` - ids, inventario y reglas de compatibilidad de Souls.
- `Content/Systems` - keybinds y sistemas globales.
- `Content/UI` - UI custom.
- `Localization` y `en-US.hjson` - localizacion.
- `tests` - smoke tests PowerShell source-level.

## Flujo runtime principal

1. `EterniaPlayer.PreUpdate` resetea `ActiveSoul` a `None`.
2. Las Souls equipadas como accesorios llaman `ActivateSoul` en
   `UpdateAccessory`.
3. `EterniaPlayer.PostUpdateEquips` aplica no-Soul penalty o valida arma
   incorrecta mientras hay animacion/held item.
4. `SubclassPlayer.PostUpdateEquips` recalcula `CurrentSubclass` desde Soul,
   hardmode, afinidades y locked promotion.
5. Players de subclase (`SwordsmanPlayer`, `NecromancerPlayer`, etc.) exponen
   helpers `IsActive...` que cruzan:
   - `soul.HasClassSoul`;
   - `soul.ActiveSoul`;
   - `SubclassPlayer.CurrentSubclass`.
6. UI y efectos runtime deben usar esos helpers cuando existan.

## Reglas puras vs estado tModLoader

Reglas puras o casi puras:

- `SoulRules`
- `ClassPromotionRules`
- `ProgressionService`
- `PassiveRegistry`

Estado tModLoader:

- `EterniaPlayer`
- `EterniaStatsPlayer`
- `EterniaLevelPlayer`
- `SubclassPlayer`
- `PromotionRewardPlayer`
- players de subclase

Cuando agregues una regla nueva, intenta poner la decision en `Progression` o
`Souls`, y deja los `ModPlayer` como integracion con Terraria.

## Save data actual

`EterniaPlayer` guarda:

- posicion de Soul UI;
- flags de starter weapons entregadas.

`SubclassPlayer` guarda:

- `LockedWarriorPromotion`
- `LockedMagePromotion`
- `LockedRangerPromotion`
- `LockedSummonerPromotion`

`PromotionRewardPlayer` guarda:

- `AwardedPromotions` como lista de strings.

Riesgo: las strings guardadas no estan versionadas ni migradas. Si se eliminan
promociones, hay que sanitizar saves antiguos.

## UI architecture

`EterniaUI` es el helper visual compartido. Contiene:

- colores base;
- paneles centrados y bottom-left;
- clamp de rectangulos;
- helpers de texto;
- botones, close button, pills, tooltips y progress bars;
- `ShouldDrawPlayerUI`;
- `CloseMajorPanelsExcept`.

Paneles grandes:

- `SoulUISystem.Visible`
- `StatsUI.Visible`
- `PassiveUI.Visible`

Al abrir uno, debe llamar:

```csharp
EterniaUI.CloseMajorPanelsExcept(EterniaUI.MajorPanel.X)
```

para evitar solapes de paneles.

## Assets

Muchos items/proyectiles usan assets temporales o texturas existentes, por
ejemplo Souls como iconos. Los smoke tests verifican algunos paths, pero no hay
garantia visual de calidad final.

## CodeGraph

El repo tiene `.codegraph/`. Para Claude Code o Codex:

- Usar CodeGraph para simbolos, callers/callees y contexto estructural.
- Usar `rg` para texto literal, strings, localizacion y tests.

