using BotSharp.Abstraction.Knowledges.Models;
using BotSharp.Abstraction.VectorStorage.Models;
using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Knowledge Base
    public bool AddKnowledgeCollectionConfigs(List<VectorCollectionConfig> configs, bool reset = false)
    {
        if (configs?.Any() != true) return false;

        var validConfigs = configs.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
        if (!validConfigs.Any()) return false;

        try
        {
            if (reset)
            {
                _context.KnowledgeCollectionConfigs.RemoveRange(_context.KnowledgeCollectionConfigs);
            }

            var existingConfigs = _context.KnowledgeCollectionConfigs.ToList();
            var insertDocs = new List<KnowledgeCollectionConfig>();
            var updateDocs = new List<KnowledgeCollectionConfig>();

            foreach (var config in validConfigs)
            {
                var existing = existingConfigs.FirstOrDefault(x => x.Name == config.Name);
                if (existing != null && !reset)
                {
                    existing.Type = config.Type;
                    existing.VectorStore = new KnowledgeVectorStoreConfigElement
                    {
                        Provider = config.VectorStore?.Provider,
                        Model = config.VectorStore?.Model,
                        Dimension = config.VectorStore?.Dimension ?? 0
                    };
                    existing.TextEmbedding = new KnowledgeEmbeddingConfigElement
                    {
                        Provider = config.TextEmbedding?.Provider,
                        Model = config.TextEmbedding?.Model,
                        Dimension = config.TextEmbedding?.Dimension ?? 0
                    };
                    updateDocs.Add(existing);
                }
                else
                {
                    insertDocs.Add(new KnowledgeCollectionConfig
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = config.Name,
                        Type = config.Type,
                        VectorStore = new KnowledgeVectorStoreConfigElement
                        {
                            Provider = config.VectorStore?.Provider,
                            Model = config.VectorStore?.Model,
                            Dimension = config.VectorStore?.Dimension ?? 0
                        },
                        TextEmbedding = new KnowledgeEmbeddingConfigElement
                        {
                            Provider = config.TextEmbedding?.Provider,
                            Model = config.TextEmbedding?.Model,
                            Dimension = config.TextEmbedding?.Dimension ?? 0
                        }
                    });
                }
            }

            if (insertDocs.Any())
            {
                _context.KnowledgeCollectionConfigs.AddRange(insertDocs);
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving knowledge collection configs");
            return false;
        }
    }

    public bool DeleteKnowledgeCollectionConfig(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName)) return false;

        try
        {
            var configs = _context.KnowledgeCollectionConfigs.Where(x => x.Name == collectionName);
            _context.KnowledgeCollectionConfigs.RemoveRange(configs);
            var deleted = _context.SaveChanges();
            return deleted > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting knowledge collection config {CollectionName}", collectionName);
            return false;
        }
    }

    public IEnumerable<VectorCollectionConfig> GetKnowledgeCollectionConfigs(VectorCollectionConfigFilter filter)
    {
        if (filter == null) return Enumerable.Empty<VectorCollectionConfig>();

        var query = _context.KnowledgeCollectionConfigs.AsQueryable();

        if (filter.CollectionNames?.Any() == true)
        {
            query = query.Where(x => filter.CollectionNames.Contains(x.Name));
        }

        if (filter.CollectionTypes?.Any() == true)
        {
            query = query.Where(x => filter.CollectionTypes.Contains(x.Type));
        }

        if (filter.VectorStroageProviders?.Any() == true)
        {
            query = query.Where(x => filter.VectorStroageProviders.Contains(x.VectorStore.Provider));
        }

        var configs = query.ToList();
        return configs.Select(x => new VectorCollectionConfig
        {
            Name = x.Name,
            Type = x.Type,
            VectorStore = new VectorStoreConfig
            {
                Provider = x.VectorStore?.Provider,
                Model = x.VectorStore?.Model,
                Dimension = x.VectorStore?.Dimension ?? 0
            },
            TextEmbedding = new EmbeddingConfig
            {
                Provider = x.TextEmbedding?.Provider,
                Model = x.TextEmbedding?.Model,
                Dimension = x.TextEmbedding?.Dimension ?? 0
            }
        });
    }

    public bool SaveKnolwedgeBaseFileMeta(KnowledgeDocMetaData metaData)
    {
        if (metaData == null || string.IsNullOrWhiteSpace(metaData.Collection) || 
            string.IsNullOrWhiteSpace(metaData.VectorStoreProvider)) return false;

        try
        {
            var existing = _context.KnowledgeDocuments.FirstOrDefault(x => 
                x.Collection == metaData.Collection && 
                x.VectorStoreProvider == metaData.VectorStoreProvider && 
                x.FileId == metaData.FileId);

            if (existing != null)
            {
                existing.FileName = metaData.FileName;
                existing.FileSource = metaData.FileSource;
                existing.ContentType = metaData.ContentType;
                existing.VectorStoreProvider = metaData.VectorStoreProvider;
                existing.VectorDataIds = metaData.VectorDataIds ?? new List<string>();
                existing.CreateDate = metaData.CreateDate;
                existing.CreateUserId = metaData.CreateUserId;
            }
            else
            {
                var newDoc = new KnowledgeDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    Collection = metaData.Collection,
                    FileName = metaData.FileName,
                    FileSource = metaData.FileSource,
                    ContentType = metaData.ContentType,
                    FileId = metaData.FileId,
                    VectorStoreProvider = metaData.VectorStoreProvider,
                    VectorDataIds = metaData.VectorDataIds ?? new List<string>(),
                    CreateDate = metaData.CreateDate,
                    CreateUserId = metaData.CreateUserId
                };
                _context.KnowledgeDocuments.Add(newDoc);
            }

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving knowledge base file meta");
            return false;
        }
    }

    public bool DeleteKnolwedgeBaseFileMeta(string collectionName, string vectorStoreProvider, Guid? fileId = null)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(vectorStoreProvider)) 
            return false;

        try
        {
            var query = _context.KnowledgeDocuments.Where(x => 
                x.Collection == collectionName && 
                x.VectorStoreProvider == vectorStoreProvider);

            if (fileId.HasValue)
            {
                query = query.Where(x => x.FileId == fileId.Value);
            }

            var docsToDelete = query.ToList();
            _context.KnowledgeDocuments.RemoveRange(docsToDelete);
            var deleted = _context.SaveChanges();
            return deleted > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting knowledge base file meta");
            return false;
        }
    }

    public PagedItems<KnowledgeDocMetaData> GetKnowledgeBaseFileMeta(string collectionName, string vectorStoreProvider, KnowledgeFileFilter filter)
    {
        if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(vectorStoreProvider))
            return new PagedItems<KnowledgeDocMetaData> { Items = new List<KnowledgeDocMetaData>(), Count = 0 };

        var query = _context.KnowledgeDocuments.Where(x => 
            x.Collection == collectionName && 
            x.VectorStoreProvider == vectorStoreProvider);

        if (filter != null)
        {
            if (filter.FileIds?.Any() == true)
            {
                query = query.Where(x => filter.FileIds.Contains(x.FileId));
            }

            if (!string.IsNullOrWhiteSpace(filter.FileSource))
            {
                query = query.Where(x => x.FileSource == filter.FileSource);
            }

            if (!string.IsNullOrWhiteSpace(filter.CreateUserId))
            {
                query = query.Where(x => x.CreateUserId == filter.CreateUserId);
            }
        }

        var totalCount = query.Count();
        var docs = query.Skip(filter?.Offset ?? 0)
                        .Take(filter?.Size ?? 10)
                        .ToList();

        var items = docs.Select(x => new KnowledgeDocMetaData
        {
            Collection = x.Collection,
            FileName = x.FileName,
            FileSource = x.FileSource,
            ContentType = x.ContentType,
            FileId = x.FileId,
            VectorStoreProvider = x.VectorStoreProvider,
            VectorDataIds = x.VectorDataIds,
            CreateDate = x.CreateDate,
            CreateUserId = x.CreateUserId
        }).ToList();

        return new PagedItems<KnowledgeDocMetaData>
        {
            Items = items,
            Count = totalCount
        };
    }
    #endregion
}
