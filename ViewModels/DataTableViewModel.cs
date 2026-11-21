using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShotPost.Services;

namespace ShotPost.ViewModels;

/// <summary>Ein einzelnes Feld (Property) einer Tabelle, das gesendet werden kann.</summary>
public partial class FieldOptionViewModel(string name, bool isRequired) : ViewModelBase
{
    public string Name { get; } = name;

    /// <summary>Schluesselfeld: wird immer gesendet und kann nicht abgewaehlt werden.</summary>
    public bool IsRequired { get; } = isRequired;

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = true;
}

/// <summary>Eine Tabelle (Scheiben, Serien, Treffer) mit ihrer Feldauswahl.</summary>
public partial class DataTableViewModel : ViewModelBase
{
    public DataTableViewModel(string title, string description, Type modelType)
    {
        Title = title;
        Description = description;
        ModelType = modelType;
        Fields = PayloadBuilder
            .GetFieldNames(modelType)
            .Select(n => new FieldOptionViewModel(n, PayloadBuilder.IsKeyField(modelType, n)))
            .ToList();

        foreach (var field in Fields)
            field.PropertyChanged += (_, _) => OnChanged();
    }

    public string Title { get; }
    public string Description { get; }
    public Type ModelType { get; }
    public IReadOnlyList<FieldOptionViewModel> Fields { get; }

    /// <summary>Wird ausgeloest, wenn sich die Auswahl aendert.</summary>
    public event Action? Changed;

    /// <summary>Ob die Tabelle ueberhaupt gesendet wird.</summary>
    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = true;

    public string Summary => $"{Fields.Count(f => f.IsSelected)} von {Fields.Count} Feldern";

    public List<string> SelectedFieldNames =>
        Fields.Where(f => f.IsSelected).Select(f => f.Name).ToList();

    /// <param name="isEnabled">Ob die Tabelle gesendet wird.</param>
    /// <param name="selectedFields">Ausgewaehlte Feldnamen; <c>null</c> = alle.</param>
    public void Load(bool isEnabled, IReadOnlyCollection<string>? selectedFields)
    {
        IsEnabled = isEnabled;
        foreach (var field in Fields)
            field.IsSelected =
                field.IsRequired || selectedFields is null || selectedFields.Contains(field.Name);
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var field in Fields)
            field.IsSelected = true;
    }

    [RelayCommand]
    private void SelectNone()
    {
        foreach (var field in Fields)
            field.IsSelected = field.IsRequired;
    }

    partial void OnIsEnabledChanged(bool value) => OnChanged();

    private void OnChanged()
    {
        OnPropertyChanged(nameof(Summary));
        Changed?.Invoke();
    }
}
