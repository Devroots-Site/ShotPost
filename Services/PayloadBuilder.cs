using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShotPost.Models;

namespace ShotPost.Services;

/// <summary>
/// Baut aus einem Modell (Scheibe, Serie, Treffer) das Objekt, das an den Server geht –
/// nur mit den in den Einstellungen ausgewaehlten Feldern.
/// </summary>
public static class PayloadBuilder
{
    /// <summary>Schluesselfelder werden immer gesendet, sonst lassen sich die Datensaetze nicht zuordnen.</summary>
    private static readonly Dictionary<Type, string[]> KeyFields = new()
    {
        [typeof(Scheibe)] = [nameof(Scheibe.ScheibenID)],
        [typeof(Serie)] = [nameof(Serie.ScheibenID), nameof(Serie.Stellung), nameof(Serie.SerieNr)],
        [typeof(Treffer)] =
        [
            nameof(Treffer.ScheibenID),
            nameof(Treffer.Stellung),
            nameof(Treffer.TrefferNr),
        ],
    };

    /// <summary>Alle sendbaren Felder eines Modells (oeffentliche Properties mit Setter), in Deklarationsreihenfolge.</summary>
    public static IReadOnlyList<string> GetFieldNames(Type modelType) =>
        GetProperties(modelType).Select(p => p.Name).ToList();

    public static bool IsKeyField(Type modelType, string fieldName) =>
        KeyFields.TryGetValue(modelType, out var keys) && keys.Contains(fieldName);

    /// <param name="item">Der zu sendende Datensatz.</param>
    /// <param name="selectedFields">Ausgewaehlte Feldnamen; <c>null</c> = alle Felder.</param>
    public static Dictionary<string, object?> Build<T>(
        T item,
        IReadOnlyCollection<string>? selectedFields
    )
        where T : class
    {
        var result = new Dictionary<string, object?>();
        foreach (var property in GetProperties(typeof(T)))
        {
            if (
                selectedFields is null
                || selectedFields.Contains(property.Name)
                || IsKeyField(typeof(T), property.Name)
            )
            {
                result[property.Name] = property.GetValue(item);
            }
        }

        return result;
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type modelType) =>
        modelType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.SetMethod is { IsPublic: true })
            .OrderBy(p => p.MetadataToken);
}
