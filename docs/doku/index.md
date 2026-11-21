# Doku pflegen

Diese Dokumentation liegt im Ordner `docs/` und wird mit [VitePress](https://vitepress.dev/)
gebaut. Sie soll **knapp** bleiben: zum schnellen Nachschlagen, nicht als vollständige Referenz.

## Lokal starten

```bash
cd docs
npm install
npm run dev       # Vorschau mit Live-Aktualisierung
npm run build     # Produktions-Build, meldet tote Links
npm run preview   # gebaute Seite ansehen
```

## Aufbau

```text
docs/
├─ .vitepress/
│  ├─ config.mts          Navigation, Sidebar, Suche, Texte
│  └─ theme/              Farben (custom.css) und das Aktualisiert-Badge
├─ einstieg/              Überblick, Schnellstart, Struktur, Workflow
├─ architektur/           Aufbau des Codes
├─ framework/             Avalonia- und MVVM-Workflow
└─ doku/                  diese Seite
```

Eine neue Seite ist eine `.md`-Datei im passenden Ordner **plus** ein Eintrag in der `sidebar`
in `config.mts`.

## Suche

Die Suche in der Kopfleiste (Tastenkürzel <kbd>Strg</kbd> + <kbd>K</kbd>) ist die eingebaute
lokale Suche von VitePress (`search.provider: 'local'`). Der Index entsteht beim Build – kein externer Dienst nötig.

## „Zuletzt aktualisiert"

Das Badge über jeder Seite und die Zeile am Seitenende zeigen den Zeitpunkt des
**letzten Git-Commits der jeweiligen Datei**.

- Noch nicht committete Seiten zeigen „Noch nicht committet".
- In einer Build-Pipeline braucht der Checkout die volle Historie
  (`fetch-depth: 0` bei `actions/checkout`), sonst stimmen die Daten nicht.

## Automatische Aktualisierung mit Claude Code

Wer mit [Claude Code](https://claude.com/claude-code) am Projekt arbeitet, muss die Doku nicht von
Hand nachziehen. Zwei Bausteine sorgen dafür:

| Baustein | Aufgabe |
| --- | --- |
| `CLAUDE.md` | Regel „Code-Änderung ⇒ Doku-Seite mit aktualisieren" und die Zuordnung Code → Seite |
| `.claude/hooks/docs-sync.mjs` | Hook, der die Regel absichert |

So arbeitet der Hook (eingebunden in `.claude/settings.json`):

1. **Nach jeder Dateiänderung** durch Claude merkt er sich, ob Code (`.cs`, `.axaml`, `.csproj`,
   `.sql`, CI- und Konfigurationsdateien) oder eine Datei unter `docs/` bearbeitet wurde.
2. **Bevor Claude die Arbeit beendet**, prüft er: Wurde Code geändert, aber danach nichts unter
   `docs/`? Dann bekommt Claude den Auftrag, die betroffenen Seiten abzugleichen und anzupassen.
3. Ist keine Doku-Änderung nötig (z.B. reine Umformatierung), sagt Claude das kurz und hört auf –
   der Hook erinnert je Änderungsrunde nur einmal.

::: info Grenzen
Der Hook sieht nur Änderungen, die Claude Code selbst über seine Datei-Werkzeuge macht. Wer von
Hand in der IDE ändert, pflegt die Doku selbst – oder bittet Claude darum.
Voraussetzung ist Node.js im `PATH`. Ansehen oder abschalten lässt sich der Hook in Claude Code
über `/hooks`.
:::

## Schreibregeln

- Kurz und konkret; lieber eine Tabelle als drei Absätze.
- Nur beschreiben, was im Code wirklich so ist. Veraltetes löschen.
- Für Details auf die offiziellen Dokumentationen verlinken, nicht abschreiben.
