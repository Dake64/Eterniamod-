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

