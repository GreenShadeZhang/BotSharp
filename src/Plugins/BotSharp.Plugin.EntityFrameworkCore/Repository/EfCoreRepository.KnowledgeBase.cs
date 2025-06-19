using BotSharp.Abstraction.Knowledges.Models;
using BotSharp.Abstraction.VectorStorage.Models;
using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Knowledge Collection Configs
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
            },
            TextEmbedding = new KnowledgeEmbeddingConfig
            {
                Provider = x.TextEmbedding?.Provider,
                Model = x.TextEmbedding?.Model,
                Dimension = x.TextEmbedding?.Dimension ?? 0
            }
        });
    }
    #endregion

    #region Knowledge Documents
    public bool SaveKnowledgeBaseFileMeta(KnowledgeDocMetaData metaData)
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
                existing.VectorDataIds = metaData.VectorDataIds?.ToList() ?? new List<string>();
                existing.CreateDate = metaData.CreateDate;
                existing.CreateUserId = metaData.CreateUserId;
                // 处理 RefData 如果需要
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
                    VectorDataIds = metaData.VectorDataIds?.ToList() ?? new List<string>(),
                    CreateDate = metaData.CreateDate,
                    CreateUserId = metaData.CreateUserId
                    // 处理 RefData 如果需要
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

    public bool DeleteKnowledgeBaseFileMeta(string collectionName, string vectorStoreProvider, Guid? fileId = null)
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
            if (!docsToDelete.Any())
            {
                return false;
            }
            
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

        try
        {
            var query = _context.KnowledgeDocuments.Where(x => 
                x.Collection == collectionName && 
                x.VectorStoreProvider == vectorStoreProvider);

            if (filter != null)
            {
                if (filter.FileIds?.Any() == true)
                {
                    query = query.Where(x => filter.FileIds.Contains(x.FileId));
                }
                
                if (filter.FileNames?.Any() == true)
                {
                    query = query.Where(x => filter.FileNames.Contains(x.FileName));
                }
                
                if (filter.FileSources?.Any() == true)
                {
                    query = query.Where(x => filter.FileSources.Contains(x.FileSource));
                }
                
                if (filter.ContentTypes?.Any() == true)
                {
                    query = query.Where(x => filter.ContentTypes.Contains(x.ContentType));
                }
            }

            var totalCount = query.Count();
            
            // 应用排序，默认按创建日期降序
            var orderedQuery = !string.IsNullOrEmpty(filter?.Sort) 
                ? (filter.Order?.ToLower() == "asc"
                    ? ApplySortAscending(query, filter.Sort)
                    : ApplySortDescending(query, filter.Sort))
                : query.OrderByDescending(x => x.CreateDate);
            
            var docs = orderedQuery
                .Skip(filter?.Offset ?? 0)
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
                // RefData = ..., // 如果需要映射 RefData
                CreateDate = x.CreateDate,
                CreateUserId = x.CreateUserId
            }).ToList();

            return new PagedItems<KnowledgeDocMetaData>
            {
                Items = items,
                Count = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving knowledge base file meta");
            return new PagedItems<KnowledgeDocMetaData> { Items = new List<KnowledgeDocMetaData>(), Count = 0 };
        }
    }
    
    private IQueryable<KnowledgeDocument> ApplySortAscending(IQueryable<KnowledgeDocument> query, string sortField)
    {
        return sortField.ToLower() switch
        {
            "filename" => query.OrderBy(x => x.FileName),
            "filesource" => query.OrderBy(x => x.FileSource),
            "contenttype" => query.OrderBy(x => x.ContentType),
            "createdate" => query.OrderBy(x => x.CreateDate),
            "createuserid" => query.OrderBy(x => x.CreateUserId),
            _ => query.OrderBy(x => x.CreateDate)
        };
    }
    
    private IQueryable<KnowledgeDocument> ApplySortDescending(IQueryable<KnowledgeDocument> query, string sortField)
    {
        return sortField.ToLower() switch
        {
            "filename" => query.OrderByDescending(x => x.FileName),
            "filesource" => query.OrderByDescending(x => x.FileSource),
            "contenttype" => query.OrderByDescending(x => x.ContentType),
            "createdate" => query.OrderByDescending(x => x.CreateDate),
            "createuserid" => query.OrderByDescending(x => x.CreateUserId),
            _ => query.OrderByDescending(x => x.CreateDate)
        };
    }
    #endregion
}
