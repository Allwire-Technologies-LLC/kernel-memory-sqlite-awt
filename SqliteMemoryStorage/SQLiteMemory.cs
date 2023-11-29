using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.MemoryStorage;
using Microsoft.Data.Sqlite;
using AWT.KernelMemory.SQLite;
using Microsoft.KernelMemory;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace AWT.KernelMemory.SQLite;

public class SQLiteMemory : IVectorDb
{
    private readonly ILogger<SQLiteMemory> _log;
    private readonly SQLiteConfig _config;
    private readonly Database _database;

    public SQLiteMemory(
        SQLiteConfig config,
        ILogger<SQLiteMemory>? log = null)
    {
        _config = config;
        _log = log ?? DefaultLogger<SQLiteMemory>.Instance;
        _database = new Database();
    }

    private SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_config.ConnString);
    }

    public async Task CreateIndexAsync(
        string index,
        int vectorSize,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await _database.CreateTableAsync(connection, cancellationToken).ConfigureAwait(false);
        await _database.CreateCollectionAsync(connection, index, cancellationToken).ConfigureAwait(false);
    }


    public async Task<IEnumerable<string>> GetIndexesAsync(
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        var collections = _database.GetCollectionsAsync(connection, cancellationToken);
        return await collections.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteIndexAsync(
        string index,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await _database.DeleteCollectionAsync(connection, index, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> UpsertAsync(
        string index,
        MemoryRecord record,
        CancellationToken cancellationToken = default)
    {
        // Convert Vector and Payload to JSON strings for storage
        string vectorString = JsonSerializer.Serialize(record.Vector);
        string payloadString = JsonSerializer.Serialize(record.Payload);

        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await _database.UpdateAsync(connection, index, record.Id, vectorString, payloadString, DateTime.UtcNow.ToString("O"), cancellationToken).ConfigureAwait(false);
        return record.Id;
    }


    public IAsyncEnumerable<(MemoryRecord, double)> GetSimilarListAsync(
        string index,
        Embedding embedding,
        ICollection<MemoryFilter>? filters = null,
        double minRelevance = 0,
        int limit = 1,
        bool withEmbeddings = false,
        CancellationToken cancellationToken = default)
    {
        // Implement the logic to retrieve similar records based on the embedding
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<MemoryRecord> GetListAsync(
       string index,
       ICollection<MemoryFilter>? filters = null,
       int limit = 1,
       bool withEmbeddings = false,
       CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        var entries = _database.ReadAllAsync(connection, index, cancellationToken);
        await foreach (var entry in entries.WithCancellation(cancellationToken))
        {
            yield return new MemoryRecord
            {
                Id = entry.Key,
                Vector = JsonSerializer.Deserialize<Embedding>(entry.EmbeddingString),
                Payload = JsonSerializer.Deserialize<Dictionary<string, object>>(entry.MetadataString)
            };
        }
    }

    public async Task DeleteAsync(
        string index,
        MemoryRecord record,
        CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await _database.DeleteAsync(connection, index, record.Id, cancellationToken).ConfigureAwait(false);
    }
}
