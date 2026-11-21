# Avalonia in Kürze

[Avalonia UI](https://docs.avaloniaui.net/) ist ein plattformübergreifendes UI-Framework für .NET –
vom Konzept her nah an WPF. Diese Seite zeigt nur, **wie der Arbeitsablauf in ShotPost aussieht**.
Alles Weitere steht in der offiziellen Dokumentation.

## Der Ablauf in vier Schritten

```text
1. View (XAML)        beschreibt, WAS angezeigt wird
2. Binding            verbindet ein Steuerelement mit einer Property/einem Befehl
3. ViewModel (C#)     hält den Zustand und die Logik
4. Benachrichtigung   ändert sich eine Property, aktualisiert sich die Oberfläche
```

### 1. View

Eine `.axaml`-Datei plus eine fast leere `.axaml.cs`. `x:DataType` nennt das zugehörige ViewModel
– dadurch werden Bindings schon **beim Kompilieren** geprüft.

```xml
<UserControl
  x:Class="ShotPost.Views.DatabaseView"
  x:DataType="vm:DatabaseViewModel">
  <Button Content="Aktualisieren" Command="{Binding RefreshCommand}" />
</UserControl>
```

### 2. Binding

| Schreibweise | Bedeutung |
| --- | --- |
| `{Binding PageText}` | Wert aus dem ViewModel anzeigen |
| `{Binding SelectedTable}` an `SelectedItem` | Auswahl in beide Richtungen |
| `Command="{Binding RefreshCommand}"` | Klick ruft den Befehl auf |
| `IsVisible="{Binding ShowData}"` | Ein-/Ausblenden über eine `bool`-Property |

### 3. und 4. ViewModel

Siehe [MVVM mit dem Toolkit](./mvvm).

## ViewLocator

`ViewLocator.cs` ordnet einem ViewModel seine View **über den Namen** zu:

```text
ShotPost.ViewModels.SettingsViewModel  →  ShotPost.Views.SettingsView
```

Er ist in `App.axaml` als `DataTemplate` registriert. Überall, wo ein ViewModel als Inhalt gesetzt
wird (z.B. `ContentControl Content="{Binding CurrentView}"`), entsteht so automatisch die View.

::: warning Namenskonvention einhalten
`XyzViewModel` braucht eine `XyzView` im Namespace `ShotPost.Views`. Passt der Name nicht,
erscheint zur Laufzeit nur der Text „Not Found: …".
:::

## Styles und Ressourcen

- **Styles** wirken über Selektoren, ähnlich CSS: `Button.primary`, `Window.compact Border#Sidebar`.
- Klassen setzt man im XAML (`Classes="primary"`) oder gebunden (`Classes.active="{Binding …}"`).
- **Ressourcen** (Farben, Pinsel) liegen in `App.axaml`.
  In ShotPost immer `{DynamicResource …}` verwenden – Grund siehe
  [Theme](/architektur/einstellungen#theme).

## Entwicklerwerkzeuge

Im Debug-Build ist die Anbindung an die Avalonia-Entwicklerwerkzeuge aktiv
(`WithDeveloperTools()` in `Program.cs`, Paket `AvaloniaUI.DiagnosticsSupport`). Damit lassen sich
visueller Baum, Styles und Bindings der laufenden App untersuchen. Im Release-Build ist das
Paket nicht enthalten.

## Offizielle Dokumentation

| Thema | Link |
| --- | --- |
| Avalonia – Einstieg und Konzepte | [docs.avaloniaui.net](https://docs.avaloniaui.net/) |
| ViewLocator | [docs.avaloniaui.net/docs/concepts/view-locator](https://docs.avaloniaui.net/docs/concepts/view-locator) |
| MVVM-Toolkit | [learn.microsoft.com – CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) |
