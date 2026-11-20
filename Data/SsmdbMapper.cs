using System;
using System.Data.Common;
using ShotPost.Models;

namespace ShotPost.Data;

/// <summary>
/// Mappt eine Reader-Zeile (Reihenfolge laut <see cref="SsmdbQueries"/>) auf die Modelle.
/// Konvertiert per Convert, damit abweichende Spaltentypen (int vs. bigint) nicht stoeren.
/// </summary>
public static class SsmdbMapper
{
    public static Scheibe ReadScheibe(DbDataReader r) => new()
    {
        ScheibenID = Int(r, 0) ?? 0,
        Starterliste = Str(r, 1),
        StarterlistenID = Int(r, 2),
        Startzeit = Dt(r, 3),
        StartNr = Int(r, 4),
        Nachname = Str(r, 5),
        Vorname = Str(r, 6),
        Geschlecht = Str(r, 7),
        Land = Str(r, 8),
        SportpassID = Lng(r, 9),
        StandNr = Shrt(r, 10),
        StandNrText = Str(r, 11),
        Disziplin = Str(r, 12),
        DisziplinID = Int(r, 13),
        Klasse = Str(r, 14),
        KlassenID = Int(r, 15),
        Verein = Str(r, 16),
        VereinsID = Int(r, 17),
        Mannschaft = Str(r, 18),
        MannschaftsID = Int(r, 19),
        Rangliste = Str(r, 20),
        RanglistenID = Int(r, 21),
        Gruppe = Str(r, 22),
        GruppenID = Int(r, 23),
        Status = Str(r, 24),
        Trefferzahl = Shrt(r, 25),
        TotalRing = Int(r, 26),
        TotalRing01 = Int(r, 27),
        BesterTeiler01 = Int(r, 28),
        BesterTeiler001 = Int(r, 29),
        Zeitstempel = Dt(r, 30),
    };

    public static Serie ReadSerie(DbDataReader r) => new()
    {
        ScheibenID = Int(r, 0) ?? 0,
        Stellung = Shrt(r, 1) ?? 0,
        SerieNr = Shrt(r, 2) ?? 0,
        Ring = Int(r, 3),
        Ring01 = Int(r, 4),
    };

    public static Treffer ReadTreffer(DbDataReader r) => new()
    {
        ScheibenID = Int(r, 0) ?? 0,
        Stellung = Shrt(r, 1) ?? 0,
        TrefferNr = Shrt(r, 2) ?? 0,
        Serie = Shrt(r, 3),
        X = Int(r, 4),
        Y = Int(r, 5),
        Innenzehner = Int(r, 6) is { } iz ? iz != 0 : null,
        Wertung = Int(r, 7),
        Text = Str(r, 8),
        Ring = Int(r, 9),
        Ring01 = Int(r, 10),
        Teiler01 = Int(r, 11),
        Teiler001 = Int(r, 12),
        Zeitstempel = Dt(r, 13),
        Millisekunden = Shrt(r, 14),
    };

    private static string? Str(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToString(r.GetValue(i));
    private static int? Int(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i));
    private static long? Lng(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt64(r.GetValue(i));
    private static short? Shrt(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToInt16(r.GetValue(i));
    private static DateTime? Dt(DbDataReader r, int i) => r.IsDBNull(i) ? null : Convert.ToDateTime(r.GetValue(i));
}
