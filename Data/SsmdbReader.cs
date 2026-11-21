using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;
using ShotPost.Models;

namespace ShotPost.Data;

/// <summary>Eine Seite Zeilen einer SSMDB2-Tabelle. <see cref="Rows"/> ist eine List&lt;Scheibe|Serie|Treffer&gt;.</summary>
public sealed record SsmdbPage(IList Rows, long TotalCount, string VersionLabel);

/// <summary>Lesender Zugriff auf die Meyton-Datenbank. Fuehrt ausschliesslich SELECTs aus.</summary>
public static class SsmdbReader
{
    public static MySqlConnectionStringBuilder BuildConnectionString(AppSettings s) =>
        BuildConnectionString(s.DbHost, s.DbPort, s.DbUser, s.DbPassword, s.DbName);

    public static MySqlConnectionStringBuilder BuildConnectionString(
        string host,
        int port,
        string user,
        string password,
        string database
    ) =>
        new()
        {
            Server = host.Trim(),
            Port = (uint)port,
            UserID = user.Trim(),
            Password = password,
            Database = database.Trim(),
            ConnectionTimeout = 5,
        };

    /// <param name="settings">Zugangsdaten.</param>
    /// <param name="table">SsmdbSchema.TabelleScheiben, TabelleSerien oder TabelleTreffer.</param>
    /// <param name="limit">Zeilen pro Seite; <c>null</c> = alle.</param>
    /// <param name="offset">Anzahl zu ueberspringender Zeilen.</param>
    /// <param name="ct">Abbruch.</param>
    public static async Task<SsmdbPage> ReadPageAsync(
        AppSettings settings,
        string table,
        int? limit,
        long offset,
        CancellationToken ct = default
    )
    {
        var databaseName = settings.DbName.Trim();
        await using var connection = new MySqlConnection(
            BuildConnectionString(settings).ConnectionString
        );
        await connection.OpenAsync(ct);
        var schema = await SsmdbSchemaDetector.DetectAsync(connection, databaseName, ct);

        // Tabellenname stammt aus der festen Liste unten, nie aus Nutzereingaben.
        IList rows = table switch
        {
            SsmdbSchema.TabelleScheiben => new List<Scheibe>(),
            SsmdbSchema.TabelleSerien => new List<Serie>(),
            SsmdbSchema.TabelleTreffer => new List<Treffer>(),
            _ => throw new ArgumentException($"Unbekannte Tabelle '{table}'.", nameof(table)),
        };
        var sql = table switch
        {
            SsmdbSchema.TabelleScheiben => SsmdbQueries.Scheiben(schema),
            SsmdbSchema.TabelleSerien => SsmdbQueries.Serien(schema),
            _ => SsmdbQueries.Treffer(schema),
        };
        Func<DbDataReader, object> map = table switch
        {
            SsmdbSchema.TabelleScheiben => SsmdbMapper.ReadScheibe,
            SsmdbSchema.TabelleSerien => SsmdbMapper.ReadSerie,
            _ => SsmdbMapper.ReadTreffer,
        };

        long total;
        await using (var countCmd = new MySqlCommand($"SELECT COUNT(*) FROM `{table}`", connection))
            total = Convert.ToInt64(
                await countCmd.ExecuteScalarAsync(ct),
                CultureInfo.InvariantCulture
            );

        await using var cmd = new MySqlCommand(sql, connection);
        if (limit is { } pageSize)
        {
            cmd.CommandText += " LIMIT @limit OFFSET @offset";
            cmd.Parameters.AddWithValue("@limit", pageSize);
            cmd.Parameters.AddWithValue("@offset", offset);
        }

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            rows.Add(map(reader));

        return new SsmdbPage(rows, total, schema.VersionLabel);
    }
}
