using BotSharp.Abstraction.Knowledges.Models;
using BotSharp.Abstraction.VectorStorage.Models;
using BotSharp.Plugin.EntityFrameworkCore.Entities;
using BotSharp.Plugin.EntityFrameworkCore.Mappers;

namespace BotSharp.Plugin.EntityFrameworkCore.Repository;

public partial class EfCoreRepository
{
    #region Knowledge Collection Configs
    public bool AddKnowledgeCollectionConfigs(List<VectorCollectionConfig> configs, bool reset = false)
    {
        var docs = configs?.Where(x => !string.IsNullOrWhiteSpace(x.Name))
           .Select(x => new KnowledgeCollectionConfig
           {
               Id = Guid.NewGuid().ToString(),
               Name = x.Name,
               Type = x.Type,
               VectorStore = x.VectorStore.ToEntity(),
               TextEmbedding = x.TextEmbedding.ToEntity(),
           })?.ToList() ?? [];

        if (reset)
        {
            _context.KnowledgeCollectionConfigs.RemoveRange(_context.KnowledgeCollectionConfigs);
            _context.SaveChanges();
            _context.KnowledgeCollectionConfigs.AddRange(docs);
            _context.SaveChanges();
            return true;
        }

        // Update if collection already exists, otherwise insert.
        var insertDocs = new List<KnowledgeCollectionConfig>();
        var updateDocs = new List<KnowledgeCollectionConfig>();

        var names = docs.Select(x => x.Name).ToList();
        var savedConfigs = _context.KnowledgeCollectionConfigs.Where(x => names.Contains(x.Name)).ToList();

        foreach (var doc in docs)
        {
            var found = savedConfigs.FirstOrDefault(x => x.Name == doc.Name);
            if (found != null)
            {
                found.Type = doc.Type;
                found.VectorStore = doc.VectorStore;
                found.TextEmbedding = doc.TextEmbedding;
                updateDocs.Add(found);
            }
            else
            {
                insertDocs.Add(doc);
            }
        }

        if (!insertDocs.IsNullOrEmpty())
        {
            _context.KnowledgeCollectionConfigs.AddRange(docs);
            _context.SaveChanges();
        }

        if (!updateDocs.IsNullOrEmpty())
        {
            foreach (var doc in updateDocs)
            {
                var config = _context.KnowledgeCollectionConfigs.FirstOrDefault(x => x.Id == doc.Id);
                if (config != null)
                {
                    config.Type = doc.Type;
                    config.VectorStore = doc.VectorStore;
                    config.TextEmbedding = doc.TextEmbedding;
                    _context.KnowledgeCollectionConfigs.Update(config);
                    _context.SaveChanges();
                }
            }
        }

        return true;
    }

    public bool DeleteKnowledgeCollectionConfig(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName)) return false;
        var config = _context.KnowledgeCollectionConfigs.FirstOrDefault(x => x.Name == collectionName);
        if (config == null) return false;
        var deleted = _context.KnowledgeCollectionConfigs.Remove(config);
        _context.SaveChanges();
        return true;
    }

    public IEnumerable<VectorCollectionConfig> GetKnowledgeCollectionConfigs(VectorCollectionConfigFilter filter)
    {
        if (filter == null)
        {
            return Enumerable.Empty<VectorCollectionConfig>();
        }

        var query = _context.KnowledgeCollectionConfigs.AsQueryable();

        // Apply filters
        if (!filter.CollectionNames.IsNullOrEmpty())
        {
            query = query.Where(x => filter.CollectionNames.Contains(x.Name));
        }

        if (!filter.CollectionTypes.IsNullOrEmpty())
        {
            query = query.Where(x => filter.CollectionTypes.Contains(x.Type));
        }

        if (!filter.VectorStroageProviders.IsNullOrEmpty())
        {
            query = query.Where(x => filter.VectorStroageProviders.Contains(x.VectorStore.Provider));
        }

        // Get data
        var configs = query.ToList();

        return configs.Select(x => new VectorCollectionConfig
        {
            Name = x.Name,
            Type = x.Type,
            VectorStore = x.VectorStore.ToModel(),
            TextEmbedding = x.TextEmbedding.ToModel(),
        });
    }
    #endregion

    #region Knowledge Documents
    public bool SaveKnowledgeBaseFileMeta(KnowledgeDocMetaData metaData)
    {
        if (metaData == null
          || string.IsNullOrWhiteSpace(metaData.Collection)
          || string.IsNullOrWhiteSpace(metaData.VectorStoreProvider))
        {
            return false;
        }

        var doc = new KnowledgeCollectionFileMeta
        {
            Id = Guid.NewGuid().ToString(),
            Collection = metaData.Collection,
            FileId = metaData.FileId,
            FileName = metaData.FileName,
            FileSource = metaData.FileSource,
            ContentType = metaData.ContentType,
            VectorStoreProvider = metaData.VectorStoreProvider,
            VectorDataIds = metaData.VectorDataIds,
            RefData = metaData.RefData.ToEntity(),
            CreatedTime = metaData.CreateDate,
            CreateUserId = metaData.CreateUserId
        };

        _context.KnowledgeCollectionFileMetas.Add(doc);
        _context.SaveChanges();
        return true;
    }

    public bool DeleteKnowledgeBaseFileMeta(string collectionName, string vectorStoreProvider, Guid? fileId = null)
    {
        if (string.IsNullOrWhiteSpace(collectionName)
             || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return false;
        }

        var query = _context.KnowledgeCollectionFileMetas.Where(x => x.Collection == collectionName && x.VectorStoreProvider == vectorStoreProvider);

        if (fileId != null)
        {
            query = query.Where(x => x.FileId == fileId);
        }

        _context.KnowledgeCollectionFileMetas.RemoveRange(query.ToList());
        _context.SaveChanges();
        return true;
    }

    public PagedItems<KnowledgeDocMetaData> GetKnowledgeBaseFileMeta(string collectionName, string vectorStoreProvider, KnowledgeFileFilter filter)
    {
        if (string.IsNullOrWhiteSpace(collectionName)
            || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return new PagedItems<KnowledgeDocMetaData>();
        }

        var query = _context.KnowledgeCollectionFileMetas.Where(x => x.Collection == collectionName && x.VectorStoreProvider == vectorStoreProvider);

        // Apply filters
        if (filter != null)
        {
            if (!filter.FileIds.IsNullOrEmpty())
            {
                query = query.Where(x => filter.FileIds.Contains(x.FileId));
            }

            if (!filter.FileNames.IsNullOrEmpty())
            {
                query = query.Where(x => filter.FileNames.Contains(x.FileName));
            }

            if (!filter.FileSources.IsNullOrEmpty())
            {
                query = query.Where(x => filter.FileSources.Contains(x.FileSource));
            }

            if (!filter.ContentTypes.IsNullOrEmpty())
            {
                query = query.Where(x => filter.ContentTypes.Contains(x.ContentType));
            }
        }
        var count = query.Count();
        var docs = query.OrderByDescending(x => x.CreatedTime).Skip(filter.Offset).Take(filter.Size).ToList();

        var files = docs?.Select(x => new KnowledgeDocMetaData
        {
            Collection = x.Collection,
            FileId = x.FileId,
            FileName = x.FileName,
            FileSource = x.FileSource,
            ContentType = x.ContentType,
            VectorStoreProvider = x.VectorStoreProvider,
            VectorDataIds = x.VectorDataIds,
            RefData = x.RefData.ToModel(),
            CreateUserId = x.CreateUserId
        })?.ToList() ?? new();

        return new PagedItems<KnowledgeDocMetaData>
        {
            Items = files,
            Count = (int)count
        };
    }
    #endregion
}
