# Entwicklungs-Workflow

## Vor jedem Commit

```bash
dotnet csharpier format .   # Code formatieren
dotnet build                # Build inkl. Analyzer und Stilregeln
```

Die CI führt dieselben Prüfungen aus – wer sie lokal laufen lässt, erlebt keine Überraschungen.

## Was die CI prüft

`.github/workflows/ci.yml` läuft bei jedem Pull Request und bei Pushes auf `main`:

1. `dotnet tool restore`
2. `dotnet csharpier check .` – schlägt fehl, wenn etwas nicht formatiert ist
3. `dotnet build --configuration Release`

::: warning Warnungen brechen den Build in der CI
Auf GitHub ist `CI=true` gesetzt. Dadurch gilt `TreatWarningsAsErrors` (siehe `ShotPost.csproj`):
Compiler-, Nullable-, Analyzer- und `.editorconfig`-Warnungen sind dort Fehler.
Lokal nachstellen: `CI=true dotnet build` (PowerShell: `$env:CI = 'true'; dotnet build`).
:::

## Code-Stil in Kürze

- Formatierung macht **CSharpier** – nicht von Hand diskutieren.
- Regeln stehen in `.editorconfig` (u.a. file-scoped Namespaces, max. 100 Zeichen je Zeile).
- `Nullable` ist aktiviert: `null` muss im Typ sichtbar sein (`string?`).
- Kommentare im Code sind deutsch und ohne Umlaute geschrieben (`ae`, `oe`, `ue`).

## Dokumentation mitpflegen

Ändert sich das Verhalten des Codes, gehört die passende Doku-Seite in denselben Commit.
Mit Claude Code passiert das automatisch – siehe [Doku pflegen](/doku/).
