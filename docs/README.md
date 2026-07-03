# ETERNIA docs

Esta carpeta existe para que una IA nueva, por ejemplo Claude Code, pueda
continuar el mod sin reconstruir todo el contexto desde cero.

## Lectura recomendada

1. `current-state.md` - snapshot actual, que esta implementado y que esta verificado.
2. `ai-handoff.md` - instrucciones operativas para una IA que continue el trabajo.
3. `claude-code-workflow.md` - reglas para registrar cambios, decisiones y reorganizar estructura.
4. `change-log.md` - historial vivo de cambios no triviales.
5. `decision-log.md` - decisiones aceptadas, propuestas y descartadas.
6. `gameplay-systems.md` - reglas de Souls, clases, progresion, penalizaciones y combate.
7. `technical-architecture.md` - mapa de archivos y flujo tecnico principal.
8. `ui-rework.md` - estado del rework de UI, helpers y riesgos pendientes.
9. `testing-verification.md` - comandos de build, smoke tests y checklist manual.
10. `roadmap-known-issues.md` - prioridades siguientes y decisiones abiertas.

## Datos rapidos

- Proyecto: mod de Terraria/tModLoader llamado `ETERNIA`.
- Ruta local esperada:
  `C:\Users\jalom\Documents\My Games\Terraria\tModLoader\ModSources\ETERNIA`
- Branch observado al documentar: `master`.
- Target de build: `net8.0` via `ETERNIA.csproj`.
- El repo tiene indice `.codegraph/`; usar CodeGraph para preguntas estructurales.
- Toda sesion no trivial debe actualizar `change-log.md`; decisiones de diseno
  o arquitectura deben actualizar `decision-log.md`.

## Comandos basicos

```powershell
dotnet build .\ETERNIA.csproj
```

```powershell
$ErrorActionPreference = 'Stop'
$tests = Get-ChildItem -Path .\tests -Filter *.ps1 | Sort-Object Name
$passed = 0
foreach ($test in $tests) {
    Write-Host "RUN $($test.Name)"
    pwsh -NoProfile -ExecutionPolicy Bypass -File $test.FullName
    if ($LASTEXITCODE -ne 0) {
        throw "Smoke test failed: $($test.Name) with exit code $LASTEXITCODE"
    }
    $passed++
}
Write-Host "ALL $passed SMOKE TESTS PASSED"
```

```powershell
git diff --check
```

PowerShell puede imprimir un warning de `CursorPosition` por el entorno de
Codex. Si el comando termina con exit code 0, ese warning no invalida la
verificacion.
