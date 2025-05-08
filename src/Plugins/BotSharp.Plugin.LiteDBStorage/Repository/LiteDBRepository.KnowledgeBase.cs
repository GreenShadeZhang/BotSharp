using BotSharp.Abstraction.Knowledges.Models;
using BotSharp.Abstraction.VectorStorage.Models;

namespace BotSharp.Plugin.LiteDBStorage.Repository;

public partial class LiteDBRepository
{
    #region Configs
    public bool AddKnowledgeCollectionConfigs(List<VectorCollectionConfig> configs, bool reset = false)
    {
        var docs = configs?.Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new KnowledgeCollectionConfigDocument
            {
                Id = Guid.NewGuid().ToString(),
                Name = x.Name,
                Type = x.Type,
                VectorStore = KnowledgeVectorStoreConfigLiteDBModel.ToLiteDBModel(x.VectorStore),
                TextEmbedding = KnowledgeEmbeddingConfigLiteDBModel.ToLiteDBModel(x.TextEmbedding)
            })?.ToList() ?? new List<KnowledgeCollectionConfigDocument>();

        if (reset)
        {
            _dc.KnowledgeCollectionConfigs.DeleteAll();
            _dc.KnowledgeCollectionConfigs.InsertBulk(docs);
            return true;
        }

        // Update if collection already exists, otherwise insert.
        var insertDocs = new List<KnowledgeCollectionConfigDocument>();
        var updateDocs = new List<KnowledgeCollectionConfigDocument>();

        var names = docs.Select(x => x.Name).ToList();

        var savedConfigs = _dc.KnowledgeCollectionConfigs.Find(x => names.Contains(x.Name)).ToList();

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
            _dc.KnowledgeCollectionConfigs.InsertBulk(docs);
        }

        if (!updateDocs.IsNullOrEmpty())
        {
            foreach (var doc in updateDocs)
            {
                var congfig = _dc.KnowledgeCollectionConfigs.Find(x => x.Id == doc.Id).FirstOrDefault();

                if (congfig != null)
                {
                    _dc.KnowledgeCollectionConfigs.Update(doc);
                }
            }
        }

        return true;
    }

    public bool DeleteKnowledgeCollectionConfig(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName)) return false;

        var deleted = _dc.KnowledgeCollectionConfigs.DeleteMany(x => x.Name == collectionName);
        return deleted > 0;
    }

    public IEnumerable<VectorCollectionConfig> GetKnowledgeCollectionConfigs(VectorCollectionConfigFilter filter)
    {
        if (filter == null)
        {
            return Enumerable.Empty<VectorCollectionConfig>();
        }

        var query = _dc.KnowledgeCollectionConfigs.Query();

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
            VectorStore = KnowledgeVectorStoreConfigLiteDBModel.ToDomainModel(x.VectorStore),
            TextEmbedding = KnowledgeEmbeddingConfigLiteDBModel.ToDomainModel(x.TextEmbedding)
        });
    }
    #endregion

    #region Documents
    public bool SaveKnolwedgeBaseFileMeta(KnowledgeDocMetaData metaData)
    {
        if (metaData == null
            || string.IsNullOrWhiteSpace(metaData.Collection)
            || string.IsNullOrWhiteSpace(metaData.VectorStoreProvider))
        {
            return false;
        }

        var doc = new KnowledgeCollectionFileMetaDocument
        {
            Id = Guid.NewGuid().ToString(),
            Collection = metaData.Collection,
            FileId = metaData.FileId,
            FileName = metaData.FileName,
            FileSource = metaData.FileSource,
            ContentType = metaData.ContentType,
            VectorStoreProvider = metaData.VectorStoreProvider,
            VectorDataIds = metaData.VectorDataIds,
            RefData = KnowledgeFileMetaRefLiteDBModel.ToLiteDBModel(metaData.RefData),
            CreateDate = metaData.CreateDate,
            CreateUserId = metaData.CreateUserId
        };

        _dc.KnowledgeCollectionFileMeta.Insert(doc);
        return true;
    }

    public bool DeleteKnolwedgeBaseFileMeta(string collectionName, string vectorStoreProvider, Guid? fileId = null)
    {
        if (string.IsNullOrWhiteSpace(collectionName)
            || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return false;
        }

        // todo：fileId
        //if (fileId != null)
        //{
        //    filters.Add(builder.Eq(x => x.FileId, fileId));
        //}

        var res = _dc.KnowledgeCollectionFileMeta.DeleteMany(x => x.Collection == collectionName &&
        x.VectorStoreProvider == vectorStoreProvider);
        return res > 0;
    }

    public PagedItems<KnowledgeDocMetaData> GetKnowledgeBaseFileMeta(string collectionName, string vectorStoreProvider, KnowledgeFileFilter filter)
    {
        if (string.IsNullOrWhiteSpace(collectionName)
            || string.IsNullOrWhiteSpace(vectorStoreProvider))
        {
            return new PagedItems<KnowledgeDocMetaData>();
        }

        //todo：
        // Apply filters
        //if (filter != null)
        //{
        //    if (!filter.FileIds.IsNullOrEmpty())
        //    {
        //        docFilters.Add(builder.In(x => x.FileId, filter.FileIds));
        //    }

        //    if (!filter.FileNames.IsNullOrEmpty())
        //    {
        //        docFilters.Add(builder.In(x => x.FileName, filter.FileNames));
        //    }

        //    if (!filter.FileSources.IsNullOrEmpty())
        //    {
        //        docFilters.Add(builder.In(x => x.FileSource, filter.FileSources));
        //    }

        //    if (!filter.ContentTypes.IsNullOrEmpty())
        //    {
        //        docFilters.Add(builder.In(x => x.ContentType, filter.ContentTypes));
        //    }
        //}

        var docs = _dc.KnowledgeCollectionFileMeta.Find(x => x.Collection == collectionName &&
        x.VectorStoreProvider == vectorStoreProvider).OrderByDescending(x => x.CreateDate).Skip(filter.Offset).Take(filter.Size).ToList();
        var count = _dc.KnowledgeCollectionFileMeta.Count(x => x.Collection == collectionName &&
        x.VectorStoreProvider == vectorStoreProvider);

        var files = docs?.Select(x => new KnowledgeDocMetaData
        {
            Collection = x.Collection,
            FileId = x.FileId,
            FileName = x.FileName,
            FileSource = x.FileSource,
            ContentType = x.ContentType,
            VectorStoreProvider = x.VectorStoreProvider,
            VectorDataIds = x.VectorDataIds,
            RefData = KnowledgeFileMetaRefLiteDBModel.ToDomainModel(x.RefData),
            CreateDate = x.CreateDate,
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
