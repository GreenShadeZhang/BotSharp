# BotSharp.Plugin.Pgvector

PostgreSQL with pgvector extension plugin for BotSharp vector storage and similarity search.

## Overview

This plugin provides vector storage capabilities using PostgreSQL with the pgvector extension, offering a robust and scalable alternative to dedicated vector databases like Qdrant. It integrates seamlessly with BotSharp's existing Entity Framework Core infrastructure.

## Features

- **Vector Storage**: Store high-dimensional vectors using PostgreSQL's pgvector extension
- **Similarity Search**: Perform cosine, L2, and inner product similarity searches
- **Index Support**: Automatic creation of HNSW and IVFFlat indexes for optimal performance
- **EF Core Integration**: Leverages existing Entity Framework Core infrastructure
- **Collection Management**: Full collection lifecycle management (create, delete, list)
- **Payload Support**: Store and query additional metadata with vectors
- **Pagination**: Support for paginated data retrieval
- **Filtering**: Advanced filtering capabilities using PostgreSQL's JSONB support

## Prerequisites

1. PostgreSQL 12+ with pgvector extension installed
2. Entity Framework Core
3. The following NuGet packages:
   - `Pgvector`
   - `Pgvector.EntityFrameworkCore`

## Installation

### 1. Install pgvector extension in PostgreSQL

```sql
-- Connect to your PostgreSQL database
CREATE EXTENSION IF NOT EXISTS vector;
```

### 2. Add the plugin to your BotSharp configuration

```json
{
  "KnowledgeBase": {
    "VectorDb": {
      "Provider": "Pgvector"
    }
  },
  "Database": {
    "BotSharpPostgreSql": "Host=localhost;Database=botsharp;Username=postgres;Password=your_password"
  },
  "Pgvector": {
    "DefaultIndexType": "hnsw",
    "DefaultDistanceFunction": "cosine",
    "AutoCreateIndex": true,
    "HnswIndex": {
      "M": 16,
      "EfConstruction": 200
    },
    "IvfFlatIndex": {
      "Lists": 100
    }
  }
}
```

### 3. Run database migrations

```bash
dotnet ef migrations add InitialPgvector --project BotSharp.Plugin.Pgvector
dotnet ef database update --project BotSharp.Plugin.Pgvector
```

## Configuration

### PgvectorSettings

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DefaultIndexType` | string | "hnsw" | Default vector index type (hnsw or ivfflat) |
| `DefaultDistanceFunction` | string | "cosine" | Default distance function (cosine, l2, inner_product) |
| `AutoCreateIndex` | bool | true | Enable automatic index creation |
| `HnswIndex.M` | int | 16 | Number of bidirectional links for HNSW |
| `HnswIndex.EfConstruction` | int | 200 | Size of dynamic candidate list for HNSW |
| `IvfFlatIndex.Lists` | int | 100 | Number of inverted lists for IVFFlat |

### Index Types

**HNSW (Hierarchical Navigable Small World)**
- Best for: High-dimensional vectors, fast queries
- Trade-off: Higher memory usage, longer build time
- Recommended for: Production workloads with frequent queries

**IVFFlat (Inverted File with Flat compression)**
- Best for: Large datasets, memory-constrained environments
- Trade-off: Slower queries, lower memory usage
- Recommended for: Batch processing, cost-sensitive deployments

### Distance Functions

- **Cosine**: Measures angle between vectors (range: 0-2)
- **L2**: Euclidean distance (range: 0+)
- **Inner Product**: Dot product similarity (range: varies)

## Usage Examples

### Creating a Collection

```csharp
var vectorDb = serviceProvider.GetRequiredService<IVectorDb>();
await vectorDb.CreateCollection("my_documents", dimension: 1536);
```

### Storing Vectors

```csharp
var vector = new float[1536]; // Your embedding vector
var payload = new Dictionary<string, object>
{
    ["title"] = "Document Title",
    ["category"] = "Technology",
    ["timestamp"] = DateTime.UtcNow
};

await vectorDb.Upsert("my_documents", Guid.NewGuid(), vector, "Document content", payload);
```

### Searching Vectors

```csharp
var queryVector = new float[1536]; // Query embedding
var results = await vectorDb.Search(
    collectionName: "my_documents",
    vector: queryVector,
    fields: new[] { "title", "category" },
    limit: 10,
    confidence: 0.7f,
    withVector: false
);
```

## Performance Considerations

### Index Parameters

**HNSW Parameters:**
- `M`: Higher values = better recall, more memory
- `EfConstruction`: Higher values = better index quality, longer build time

**IVFFlat Parameters:**
- `Lists`: More lists = better performance for large datasets, diminishing returns

### Query Optimization

1. **Use appropriate confidence thresholds** to limit result sets
2. **Create partial indexes** for frequently filtered collections
3. **Use JSONB indexes** for payload filtering
4. **Monitor query performance** with PostgreSQL's query analyzer

### Database Tuning

```sql
-- Recommended PostgreSQL settings for vector workloads
SET shared_preload_libraries = 'pg_stat_statements';
SET max_connections = 100;
SET shared_buffers = '256MB';
SET effective_cache_size = '1GB';
SET maintenance_work_mem = '64MB';
SET checkpoint_completion_target = 0.9;
SET wal_buffers = '16MB';
SET default_statistics_target = 100;
```

## Migration from Qdrant

To migrate from Qdrant to Pgvector:

1. **Export data** from Qdrant collections
2. **Update configuration** to use Pgvector provider
3. **Run migrations** to create Pgvector tables
4. **Import data** using the BotSharp knowledge management APIs
5. **Verify** search results and performance

## Troubleshooting

### Common Issues

**1. pgvector extension not found**
```sql
-- Install pgvector extension
CREATE EXTENSION IF NOT EXISTS vector;
```

**2. Index creation fails**
- Check PostgreSQL logs for detailed error messages
- Ensure sufficient memory for index creation
- Consider using IVFFlat for large datasets

**3. Slow queries**
- Verify indexes are created and being used
- Check query execution plans
- Consider adjusting index parameters

**4. High memory usage**
- Switch from HNSW to IVFFlat indexes
- Reduce HNSW M parameter
- Implement data archiving strategies

### Monitoring

```sql
-- Check index usage
SELECT schemaname, tablename, indexname, idx_scan, idx_tup_read, idx_tup_fetch
FROM pg_stat_user_indexes 
WHERE indexname LIKE 'idx_%_embedding_%';

-- Check vector collection sizes
SELECT collection_name, COUNT(*) as vector_count, 
       pg_size_pretty(pg_total_relation_size('vector_data')) as table_size
FROM vector_data 
GROUP BY collection_name;
```

## Contributing

When contributing to this plugin:

1. Follow the existing code patterns and Entity Framework conventions
2. Add appropriate unit and integration tests
3. Update documentation for any new features
4. Ensure migrations are reversible
5. Test with different PostgreSQL versions and pgvector configurations

## License

This plugin follows the same license as the main BotSharp project.
