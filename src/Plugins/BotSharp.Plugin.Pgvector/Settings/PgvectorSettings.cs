namespace BotSharp.Plugin.Pgvector.Settings;

public class PgvectorSettings
{
    /// <summary>
    /// Connection string for PostgreSQL database
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Default vector index type (ivfflat or hnsw)
    /// </summary>
    public string DefaultIndexType { get; set; } = "hnsw";

    /// <summary>
    /// Default distance function (cosine, l2, inner_product)
    /// </summary>
    public string DefaultDistanceFunction { get; set; } = "cosine";

    /// <summary>
    /// Enable automatic index creation
    /// </summary>
    public bool AutoCreateIndex { get; set; } = true;

    /// <summary>
    /// HNSW index parameters
    /// </summary>
    public HnswIndexSettings HnswIndex { get; set; } = new();

    /// <summary>
    /// IVFFlat index parameters
    /// </summary>
    public IvfFlatIndexSettings IvfFlatIndex { get; set; } = new();
}

public class HnswIndexSettings
{
    /// <summary>
    /// Number of bidirectional links for each new element (default: 16)
    /// </summary>
    public int M { get; set; } = 16;

    /// <summary>
    /// Size of the dynamic candidate list (default: 200)
    /// </summary>
    public int EfConstruction { get; set; } = 200;
}

public class IvfFlatIndexSettings
{
    /// <summary>
    /// Number of inverted lists (default: 100)
    /// </summary>
    public int Lists { get; set; } = 100;
}
