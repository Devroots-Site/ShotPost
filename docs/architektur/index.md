# Architektur im Überblick

ShotPost folgt dem **MVVM-Muster**: Views zeigen an, ViewModels halten Zustand und Befehle,
darunter liegen Services und der Datenzugriff.

```text
┌──────────────────────────────────────────────────────────┐
│ Views (XAML)            MainWindow, HomeView, …          │
└──────────────▲───────────────────────────────────────────┘
               │ Bindings
┌──────────────┴───────────────────────────────────────────┐
│ ViewModels              MainViewModel, DatabaseViewModel,│
│                         SettingsViewModel, …             │
└───────┬──────────────────────────────┬───────────────────┘
        │                              │
┌───────▼──────────────┐   ┌───────────▼───────────────────┐
│ Services             │   │ Data                          │
│ SettingsService      │   │ SsmdbReader → SchemaDetector  │
│ ThemeService         │   │ → SsmdbQueries → Dapper       │
│ PayloadBuilder       │   │                               │
└───────┬──────────────┘   └───────────┬───────────────────┘
        │                              │
 settings.json                  Meyton-Datenbank (SSMDB2)
```

Models (`Scheibe`, `Serie`, `Treffer`, `AppSettings`) werden von allen Schichten genutzt.

## Programmstart

1. `Program.Main` konfiguriert Avalonia und startet die Desktop-Lebensdauer.
2. `App.OnFrameworkInitializationCompleted`:
   - erstellt den `SettingsService` und wendet die gespeicherte Hauptfarbe an,
   - erstellt `MainViewModel` und setzt es als `DataContext` des `MainWindow`,
   - ruft `InitializeAsync()` auf: kurzer Ladebildschirm, danach die Startseite.

::: info Kein DI-Container
Abhängigkeiten werden von Hand über Konstruktoren durchgereicht (`MainViewModel` bekommt den
`SettingsService` und gibt ihn an die Unter-ViewModels weiter). Die parameterlosen Konstruktoren
der ViewModels existieren nur für die Design-Vorschau in der IDE.
:::

## Grundsätze

- **Nur lesend:** Auf die Meyton-Datenbank laufen ausschließlich `SELECT`s.
- **Kein Nutzer-Input im SQL:** Tabellen- und Spaltennamen stammen aus festen Listen, Werte gehen
  als Parameter.
- **Feature-Erkennung statt Versionsnummer:** Die SSMDB2 kennt ihre Version nicht – geprüft wird,
  welche Spalten existieren.

## Die Bereiche im Detail

- [Oberfläche & Navigation](./oberflaeche)
- [Datenzugriff (SSMDB2)](./datenzugriff)
- [Modelle](./modelle)
- [Einstellungen & Theme](./einstellungen)
- [Payload](./payload)
