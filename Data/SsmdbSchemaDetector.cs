using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;

namespace ShotPost.Data;

public sealed class SsmdbSchemaException(string message) : Exception(message);

public static class SsmdbSchemaDetector
{
    private static readonly string[] Tabellen =
    [
        SsmdbSchema.TabelleScheiben,
        SsmdbSchema.TabelleSerien,
        SsmdbSchema.TabelleTreffer,
    ];

    /// <summary>Liest die vorhandenen Spalten aus information_schema. Nur lesender Zugriff.</summary>
    public static async Task<SsmdbSchema> DetectAsync(
        MySqlConnection connection,
        string databaseName,
        CancellationToken ct = default
    )
    {
        const string sql = """
            SELECT TABLE_NAME, COLUMN_NAME
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = @db
              AND TABLE_NAME IN ('Scheiben', 'Serien', 'Treffer')
            """;

        var columns = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@db", databaseName);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var table = reader.GetString(0);
            var column = reader.GetString(1);
            if (!columns.TryGetValue(table, out var set))
                columns[table] = set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            set.Add(column);
        }

        foreach (var t in Tabellen)
        {
            if (!columns.ContainsKey(t))
                throw new SsmdbSchemaException(
                    $"Tabelle '{t}' fehlt in der Datenbank '{databaseName}'. Ist das eine SSMDB2?"
                );
        }

        return new SsmdbSchema(databaseName, columns);
    }
}
