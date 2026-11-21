# MVVM mit dem Toolkit

ShotPost nutzt
[CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/).
Dessen Source-Generatoren erzeugen den Code, den man sonst von Hand schreiben müsste.
Alle ViewModels erben von `ViewModelBase` (= `ObservableObject`) und sind `partial`.

## Properties: `[ObservableProperty]`

```csharp
[ObservableProperty]
public partial bool IsLoading { get; set; }
```

Der Generator ergänzt die Benachrichtigung: Wird `IsLoading` gesetzt, aktualisiert sich jedes
daran gebundene Steuerelement.

### Abhängige Properties

Berechnete Properties melden sich nicht von selbst. Deshalb wird angegeben, wer mitbenachrichtigt
werden soll:

```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(HasError), nameof(ShowData))]
public partial string ErrorMessage { get; set; } = "";

public bool HasError => ErrorMessage.Length > 0;
```

### Auf Änderungen reagieren

Der Generator stellt `partial`-Methoden bereit:

```csharp
partial void OnSelectedTableChanged(string value) => ReloadFromFirstPage();
```

## Befehle: `[RelayCommand]`

```csharp
[RelayCommand]
private void ShowHome() => CurrentView = _home;
```

Aus der Methode `ShowHome` wird die Property **`ShowHomeCommand`** – an sie bindet die View.

| Variante | Beispiel im Projekt |
| --- | --- |
| asynchron (`Task`) | `RefreshAsync` → `RefreshCommand` (Suffix `Async` entfällt) |
| mit Bedingung | `[RelayCommand(CanExecute = nameof(CanGoToNextPage))]` |
| Bedingung neu prüfen | `[NotifyCanExecuteChangedFor(nameof(NextPageCommand))]` an der Property |
| mit Parameter | `SelectPresetColor(string hex)` + `CommandParameter` im XAML |

## Typische Stolpersteine

- **Klasse nicht `partial`** → der Generator kann nichts ergänzen, Build-Fehler.
- **Berechnete Property ändert sich nicht in der Oberfläche** → `NotifyPropertyChangedFor` fehlt.
- **Button bleibt deaktiviert** → `NotifyCanExecuteChangedFor` fehlt oder
  `Command.NotifyCanExecuteChanged()` wurde nicht aufgerufen.
- **Binding auf `ShowHome` statt `ShowHomeCommand`** → Kompilierfehler dank `x:DataType`.

## Offizielle Dokumentation

- [ObservableProperty](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/generators/observableproperty)
- [RelayCommand](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/generators/relaycommand)
- [Übersicht MVVM-Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
