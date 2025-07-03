using BotSharp.Abstraction.Knowledges.Enums;
using BotSharp.Plugin.Pgvector.DbContexts;
using BotSharp.Plugin.Pgvector.Entities;
using BotSharp.Plugin.Pgvector.Settings;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace BotSharp.Plugin.Pgvector.Services;

public class PgvectorDb : IVectorDb
{
    private readonly PgvectorDbContext _context;
    private readonly PgvectorSettings _settings;
    private readonly ILogger<PgvectorDb> _logger;
    private readonly IServiceProvider _services;

    public PgvectorDb(
        PgvectorDbContext context,
        PgvectorSettings settings,
        ILogger<PgvectorDb> logger,
        IServiceProvider services)
    {
        _context = context;
        _settings = settings;
        _logger = logger;
        _services = services;
    }

    public string Provider => "Pgvector";

    #region Collection Management

    public async Task<bool> DoesCollectionExist(string collectionName)
    {
        return await _context.VectorCollections
            .AnyAsync(x => x.Name == collectionName);
    }

    public async Task<bool> CreateCollection(string collectionName, int dimension)
    {
        var exists = await DoesCollectionExist(collectionName);
        if (exists) return false;

        try
        {
            var collection = new VectorCollection
            {
                Name = collectionName,
                Dimension = dimension,
                Type = "document", // Default type
                IndexType = _settings.DefaultIndexType,
                DistanceFunction = _settings.DefaultDistanceFunction,
                IsIndexed = false
            };

            _context.VectorCollections.Add(collection);
            await _context.SaveChangesAsync();

            // Create vector index if auto-create is enabled
            if (_settings.AutoCreateIndex)
            {
                await CreateVectorIndex(collectionName, dimension);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating collection {CollectionName} with dimension {Dimension}",
                collectionName, dimension);
            return false;
        }
    }

    public async Task<bool> DeleteCollection(string collectionName)
    {
        var exists = await DoesCollectionExist(collectionName);
        if (!exists) return false;

        try
        {
            // Drop vector index first
            await DropVectorIndex(collectionName);

            // Delete collection and all related data (cascade delete)
            var collection = await _context.VectorCollections
                .FirstOrDefaultAsync(x => x.Name == collectionName);

            if (collection != null)
            {
                _context.VectorCollections.Remove(collection);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection {CollectionName}", collectionName);
            return false;
        }
    }

    public async Task<IEnumerable<string>> GetCollections()
    {
        return await _context.VectorCollections
            .Select(x => x.Name)
            .ToListAsync();
    }

    public async Task<VectorCollectionDetails?> GetCollectionDetails(string collectionName)
    {
        var collection = await _context.VectorCollections
            .Include(x => x.VectorData)
            .FirstOrDefaultAsync(x => x.Name == collectionName);

        if (collection == null) return null;

        var vectorCount = await _context.VectorData
            .Where(x => x.CollectionName == collectionName)
            .CountAsync();

        return new VectorCollectionDetails
        {
            Status = "Available",
            OptimizerStatus = collection.IsIndexed ? "Optimized" : "Not Optimized",
            SegmentsCount = 1, // PostgreSQL doesn't have segments like Qdrant
            VectorsCount = (ulong)vectorCount,
            IndexedVectorsCount = collection.IsIndexed ? (ulong)vectorCount : 0,
            PointsCount = (ulong)vectorCount,
            InnerConfig = new VectorCollectionDetailConfig
            {
                Param = new VectorCollectionDetailConfigParam
                {
                    ShardNumber = 1, // PostgreSQL single instance
                    ShardingMethod = "Single",
                    ReplicationFactor = 1,
                    WriteConsistencyFactor = 1,
                    ReadFanOutFactor = 1
                }
            }
        };
    }

    #endregion

    #region Data Operations

    public async Task<bool> Upsert(string collectionName, Guid id, float[] vector, string text, Dictionary<string, object>? payload = null)
    {
        try
        {
            var collection = await _context.VectorCollections
                .FirstOrDefaultAsync(x => x.Name == collectionName);

            if (collection == null)
            {
                _logger.LogWarning("Collection {CollectionName} does not exist", collectionName);
                return false;
            }

            // Validate vector dimension
            if (vector.Length != collection.Dimension)
            {
                _logger.LogWarning("Vector dimension {VectorDim} does not match collection dimension {CollectionDim}",
                    vector.Length, collection.Dimension);
                return false;
            }

            var existingData = await _context.VectorData
                .FirstOrDefaultAsync(x => x.Id == id && x.CollectionName == collectionName);

            var pgVector = new Vector(vector);

            // Process payload similar to QdrantDb approach
            var processedPayload = new Dictionary<string, object>();
            processedPayload[KnowledgePayloadName.Text] = text;

            if (payload != null)
            {
                foreach (var item in payload)
                {
                    var value = item.Value?.ToString();
                    if (value == null) continue;

                    // Type conversion similar to QdrantDb
                    if (bool.TryParse(value, out var b))
                    {
                        processedPayload[item.Key] = b;
                    }
                    else if (byte.TryParse(value, out var int8))
                    {
                        processedPayload[item.Key] = int8;
                    }
                    else if (short.TryParse(value, out var int16))
                    {
                        processedPayload[item.Key] = int16;
                    }
                    else if (int.TryParse(value, out var int32))
                    {
                        processedPayload[item.Key] = int32;
                    }
                    else if (long.TryParse(value, out var int64))
                    {
                        processedPayload[item.Key] = int64;
                    }
                    else if (float.TryParse(value, out var f32))
                    {
                        processedPayload[item.Key] = f32;
                    }
                    else if (double.TryParse(value, out var f64))
                    {
                        processedPayload[item.Key] = f64;
                    }
                    else if (DateTime.TryParse(value, out var dt))
                    {
                        processedPayload[item.Key] = dt.ToUniversalTime().ToString("o");
                    }
                    else
                    {
                        processedPayload[item.Key] = value;
                    }
                }
            }

            var payloadJson = JsonSerializer.Serialize(processedPayload);

            if (existingData != null)
            {
                // Update existing
                existingData.Embedding = pgVector;
                existingData.Text = text;
                existingData.PayloadJson = payloadJson;
                existingData.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Insert new
                var vectorData = new VectorData
                {
                    Id = id,
                    CollectionName = collectionName,
                    Embedding = pgVector,
                    Text = text,
                    PayloadJson = payloadJson,
                    DataSource = payload?.ContainsKey(KnowledgePayloadName.DataSource) == true
                        ? payload[KnowledgePayloadName.DataSource]?.ToString() ?? VectorDataSource.Api
                        : VectorDataSource.Api
                };

                _context.VectorData.Add(vectorData);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting vector data for collection {CollectionName}", collectionName);
            return false;
        }
    }

    public async Task<IEnumerable<VectorCollectionData>> Search(string collectionName, float[] vector,
        IEnumerable<string>? fields, int limit = 5, float confidence = 0.5f, bool withVector = false)
    {
        try
        {
            var exists = await DoesCollectionExist(collectionName);
            if (!exists)
            {
                return Enumerable.Empty<VectorCollectionData>();
            }

            var collection = await _context.VectorCollections
                .FirstOrDefaultAsync(x => x.Name == collectionName);

            if (collection == null)
            {
                return Enumerable.Empty<VectorCollectionData>();
            }

            var pgVector = new Vector(vector);
            var distanceOperator = GetDistanceOperator(collection.DistanceFunction);

            // Build query based on distance function
            var query = _context.VectorData
                .Where(x => x.CollectionName == collectionName)
                .Select(x => new
                {
                    Data = x,
                    Distance = collection.DistanceFunction == "cosine" ? x.Embedding.CosineDistance(pgVector) :
                              collection.DistanceFunction == "l2" ? x.Embedding.L2Distance(pgVector) :
                              collection.DistanceFunction == "inner_product" ? x.Embedding.MaxInnerProduct(pgVector) :
                              x.Embedding.CosineDistance(pgVector)
                })
                .OrderBy(x => x.Distance)
                .Take(limit);

            var results = await query.ToListAsync();

            return results
                .Where(x => ConvertDistanceToSimilarity(x.Distance, collection.DistanceFunction) >= confidence)
                .Select(x => new VectorCollectionData
                {
                    Id = x.Data.Id.ToString(),
                    Data = ParsePayload(x.Data.PayloadJson, x.Data.Text, fields),
                    Score = ConvertDistanceToSimilarity(x.Distance, collection.DistanceFunction),
                    Vector = withVector ? x.Data.Embedding.ToArray() : null
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vectors in collection {CollectionName}", collectionName);
            return Enumerable.Empty<VectorCollectionData>();
        }
    }

    public async Task<StringIdPagedItems<VectorCollectionData>> GetPagedCollectionData(string collectionName, VectorFilter filter)
    {
        try
        {
            var exists = await DoesCollectionExist(collectionName);
            if (!exists)
            {
                return new StringIdPagedItems<VectorCollectionData>();
            }

            var query = _context.VectorData
                .Where(x => x.CollectionName == collectionName);

            // Apply search filters
            if (!filter.SearchPairs.IsNullOrEmpty())
            {
                foreach (var pair in filter.SearchPairs)
                {
                    query = query.Where(x => x.PayloadJson.Contains($"\"{pair.Key}\":\"{pair.Value}\""));
                }
            }

            // Apply pagination
            if (!string.IsNullOrWhiteSpace(filter.StartId) && Guid.TryParse(filter.StartId, out var startGuid))
            {
                query = query.Where(x => x.Id.CompareTo(startGuid) > 0);
            }

            var totalCount = await query.CountAsync();
            var results = await query
                .OrderBy(x => x.Id)
                .Take(filter.Size)
                .ToListAsync();

            var items = results.Select(x => new VectorCollectionData
            {
                Id = x.Id.ToString(),
                Data = ParsePayload(x.PayloadJson, x.Text, filter.IncludedPayloads),
                Vector = filter.WithVector ? x.Embedding.ToArray() : null
            }).ToList();

            return new StringIdPagedItems<VectorCollectionData>
            {
                Count = (uint)totalCount,
                Items = items,
                NextId = results.LastOrDefault()?.Id.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged collection data for {CollectionName}", collectionName);
            return new StringIdPagedItems<VectorCollectionData>();
        }
    }

    public async Task<IEnumerable<VectorCollectionData>> GetCollectionData(string collectionName, IEnumerable<Guid> ids,
        bool withPayload = false, bool withVector = false)
    {
        try
        {
            var idList = ids.ToList();
            if (!idList.Any())
            {
                return Enumerable.Empty<VectorCollectionData>();
            }

            var exists = await DoesCollectionExist(collectionName);
            if (!exists)
            {
                return Enumerable.Empty<VectorCollectionData>();
            }

            var results = await _context.VectorData
                .Where(x => x.CollectionName == collectionName && idList.Contains(x.Id))
                .ToListAsync();

            return results.Select(x => new VectorCollectionData
            {
                Id = x.Id.ToString(),
                Data = withPayload ? ParsePayload(x.PayloadJson, x.Text, null) : new Dictionary<string, object>(),
                Vector = withVector ? x.Embedding.ToArray() : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collection data for {CollectionName}", collectionName);
            return Enumerable.Empty<VectorCollectionData>();
        }
    }

    public async Task<bool> DeleteCollectionData(string collectionName, List<Guid> ids)
    {
        try
        {
            if (!ids.Any()) return false;

            var exists = await DoesCollectionExist(collectionName);
            if (!exists) return false;

            var itemsToDelete = await _context.VectorData
                .Where(x => x.CollectionName == collectionName && ids.Contains(x.Id))
                .ToListAsync();

            if (itemsToDelete.Any())
            {
                _context.VectorData.RemoveRange(itemsToDelete);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection data for {CollectionName}", collectionName);
            return false;
        }
    }

    public async Task<bool> DeleteCollectionAllData(string collectionName)
    {
        try
        {
            var exists = await DoesCollectionExist(collectionName);
            if (!exists) return false;

            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM vector_data WHERE collection_name = {0}", collectionName);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all data for collection {CollectionName}", collectionName);
            return false;
        }
    }

    #endregion

    #region Snapshot Operations (PostgreSQL-specific implementation)

    public async Task<IEnumerable<VectorCollectionSnapshot>> GetCollectionSnapshots(string collectionName)
    {
        // For PostgreSQL, we can implement this using pg_dump or custom export logic
        // This is a placeholder implementation
        _logger.LogInformation("Snapshot listing not implemented for PostgreSQL backend");
        return Enumerable.Empty<VectorCollectionSnapshot>();
    }

    public async Task<VectorCollectionSnapshot?> CreateCollectionShapshot(string collectionName)
    {
        // Implement using pg_dump or custom export logic
        _logger.LogInformation("Snapshot creation not implemented for PostgreSQL backend");
        return null;
    }

    public async Task<BinaryData> DownloadCollectionSnapshot(string collectionName, string snapshotFileName)
    {
        _logger.LogInformation("Snapshot download not implemented for PostgreSQL backend");
        return BinaryData.Empty;
    }

    public async Task<bool> RecoverCollectionFromShapshot(string collectionName, string snapshotFileName, BinaryData snapshotData)
    {
        _logger.LogInformation("Snapshot recovery not implemented for PostgreSQL backend");
        return false;
    }

    public async Task<bool> DeleteCollectionShapshot(string collectionName, string snapshotName)
    {
        _logger.LogInformation("Snapshot deletion not implemented for PostgreSQL backend");
        return false;
    }

    #endregion

    #region Private Helper Methods

    private async Task CreateVectorIndex(string collectionName, int dimension)
    {
        try
        {
            var collection = await _context.VectorCollections
                .FirstOrDefaultAsync(x => x.Name == collectionName);

            if (collection == null) return;

            var indexName = $"idx_{collectionName}_embedding_{collection.IndexType}";
            var distanceOp = GetDistanceOperator(collection.DistanceFunction);

            string indexSql;
            if (collection.IndexType.ToLower() == "hnsw")
            {
                indexSql = $@"
                    CREATE INDEX CONCURRENTLY IF NOT EXISTS {indexName} 
                    ON vector_data USING hnsw (embedding {distanceOp}) 
                    WHERE collection_name = '{collectionName}'
                    WITH (m = {_settings.HnswIndex.M}, ef_construction = {_settings.HnswIndex.EfConstruction})";
            }
            else
            {
                indexSql = $@"
                    CREATE INDEX CONCURRENTLY IF NOT EXISTS {indexName} 
                    ON vector_data USING ivfflat (embedding {distanceOp}) 
                    WHERE collection_name = '{collectionName}'
                    WITH (lists = {_settings.IvfFlatIndex.Lists})";
            }

            await _context.Database.ExecuteSqlRawAsync(indexSql);

            // Update collection status
            collection.IsIndexed = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created vector index {IndexName} for collection {CollectionName}",
                indexName, collectionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vector index for collection {CollectionName}", collectionName);
        }
    }

    private async Task DropVectorIndex(string collectionName)
    {
        try
        {
            var collection = await _context.VectorCollections
                .FirstOrDefaultAsync(x => x.Name == collectionName);

            if (collection == null) return;

            var indexName = $"idx_{collectionName}_embedding_{collection.IndexType}";
            var dropSql = $"DROP INDEX CONCURRENTLY IF EXISTS {indexName}";

            await _context.Database.ExecuteSqlRawAsync(dropSql);
            _logger.LogInformation("Dropped vector index {IndexName} for collection {CollectionName}",
                indexName, collectionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dropping vector index for collection {CollectionName}", collectionName);
        }
    }

    private string GetDistanceOperator(string distanceFunction)
    {
        return distanceFunction.ToLower() switch
        {
            "cosine" => "vector_cosine_ops",
            "l2" => "vector_l2_ops",
            "inner_product" => "vector_ip_ops",
            _ => "vector_cosine_ops"
        };
    }

    private float ConvertDistanceToSimilarity(double distance, string distanceFunction)
    {
        return distanceFunction.ToLower() switch
        {
            "cosine" => (float)(1.0 - distance), // Cosine distance to similarity
            "inner_product" => (float)(-distance),   // Negative inner product to similarity
            "l2" => (float)(1.0 / (1.0 + distance)), // L2 distance to similarity
            _ => (float)(1.0 - distance)
        };
    }

    private Dictionary<string, object> ParsePayload(string payloadJson, string text, IEnumerable<string>? fields)
    {
        var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson)
                     ?? new Dictionary<string, object>();

        // Always include text
        payload[KnowledgePayloadName.Text] = text;

        // Convert JsonElement values to proper types (similar to QdrantDb approach)
        var convertedPayload = new Dictionary<string, object>();
        foreach (var kvp in payload)
        {
            if (kvp.Value is JsonElement jsonElement)
            {
                convertedPayload[kvp.Key] = jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                    JsonValueKind.Number => jsonElement.TryGetInt64(out var longVal) ? longVal : jsonElement.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => string.Empty,
                    _ => kvp.Value
                };
            }
            else
            {
                convertedPayload[kvp.Key] = kvp.Value;
            }
        }

        // Filter fields if specified
        if (fields != null)
        {
            var fieldList = fields.ToList();
            if (fieldList.Any())
            {
                var filteredPayload = new Dictionary<string, object>();
                foreach (var field in fieldList)
                {
                    if (convertedPayload.ContainsKey(field))
                    {
                        filteredPayload[field] = convertedPayload[field];
                    }
                }
                return filteredPayload;
            }
        }

        return convertedPayload;
    }

    #endregion
}
