namespace BotSharp.Plugin.LiteDBStorage.Collections;

public class KnowledgeCollectionFileMetaDocument : LiteDBBase
{
    public string Collection { get; set; }
    public Guid FileId { get; set; }
    public string FileName { get; set; }
    public string FileSource { get; set; }
    public string ContentType { get; set; }
    public string VectorStoreProvider { get; set; }
    public IEnumerable<string> VectorDataIds { get; set; } = new List<string>();
    public KnowledgeFileMetaRefLiteDBModel? RefData { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUserId { get; set; }
}
