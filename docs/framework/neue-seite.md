# Rezept: neue Seite

Am Beispiel einer Seite „Protokoll".

## 1. ViewModel anlegen

`ViewModels/ProtokollViewModel.cs`

```csharp
namespace ShotPost.ViewModels;

public partial class ProtokollViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Text { get; set; } = "";
}
```

## 2. View anlegen

`Views/ProtokollView.axaml` (+ `ProtokollView.axaml.cs` mit `InitializeComponent()`,
wie bei `HomeView`)

```xml
<UserControl
  xmlns="https://github.com/avaloniaui"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:vm="using:ShotPost.ViewModels"
  x:Class="ShotPost.Views.ProtokollView"
  x:DataType="vm:ProtokollViewModel">
  <Grid RowDefinitions="Auto,*">
    <Panel Classes="pageHeader">
      <TextBlock Classes="pageTitle" Text="Protokoll" />
    </Panel>
    <TextBlock Grid.Row="1" Classes="pageSide" Text="{Binding Text}" />
  </Grid>
</UserControl>
```

Der Name muss zur Konvention des [ViewLocators](./#viewlocator) passen.

## 3. In die Navigation einhängen

`ViewModels/MainViewModel.cs`

```csharp
private readonly ProtokollViewModel _protokoll = new();

public bool IsProtokollActive => CurrentView is ProtokollViewModel;

[RelayCommand]
private void ShowProtokoll() => CurrentView = _protokoll;
```

`IsProtokollActive` zusätzlich bei `CurrentView` in `NotifyPropertyChangedFor` eintragen –
sonst wird der Eintrag nicht als aktiv markiert.

## 4. Button im MainWindow

`Views/MainWindow.axaml`

```xml
<Button
  Classes="nav"
  Classes.active="{Binding IsProtokollActive}"
  Content="Protokoll"
  Command="{Binding ShowProtokollCommand}"
/>
```

## 5. Prüfen

```bash
dotnet csharpier format .
dotnet build
dotnet run
```

Und die Seite in der Tabelle unter
[Oberfläche & Navigation](/architektur/oberflaeche#seiten) nachtragen.
