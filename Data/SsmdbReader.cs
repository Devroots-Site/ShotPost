using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
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
        var sql = table switch
        {
            SsmdbSchema.TabelleScheiben => SsmdbQueries.Scheiben(schema),
            SsmdbSchema.TabelleSerien => SsmdbQueries.Serien(schema),
            SsmdbSchema.TabelleTreffer => SsmdbQueries.Treffer(schema),
            _ => throw new ArgumentException($"Unbekannte Tabelle '{table}'.", nameof(table)),
        };

        var total = await connection.ExecuteScalarAsync<long>(
            new CommandDefinition($"SELECT COUNT(*) FROM `{table}`", cancellationToken: ct)
        );

        object? param = null;
        if (limit is { } pageSize)
        {
            sql += " LIMIT @limit OFFSET @offset";
            param = new { limit = pageSize, offset };
        }

        var query = new CommandDefinition(sql, param, cancellationToken: ct);
        IList rows = table switch
        {
            SsmdbSchema.TabelleScheiben => await QueryAsync<Scheibe>(connection, query),
            SsmdbSchema.TabelleSerien => await QueryAsync<Serie>(connection, query),
            _ => await QueryAsync<Treffer>(connection, query),
        };

        return new SsmdbPage(rows, total, schema.VersionLabel);
    }

    // Dapper mappt Spalten ueber den Namen auf die Properties (Gross-/Kleinschreibung egal).
    private static async Task<List<T>> QueryAsync<T>(
        MySqlConnection connection,
        CommandDefinition query
    ) => (await connection.QueryAsync<T>(query)).AsList();
}
