using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BotSharp.Plugin.EntityFrameworkCore.EntityConfigurations;

public class KnowledgeCollectionFileMetaEntityTypeConfiguration : IEntityTypeConfiguration<Entities.KnowledgeCollectionFileMeta>
{
    private readonly string _tablePrefix;

    public KnowledgeCollectionFileMetaEntityTypeConfiguration(string tablePrefix)
    {
        _tablePrefix = tablePrefix;
    }

    public void Configure(EntityTypeBuilder<Entities.KnowledgeCollectionFileMeta> builder)
    {
        builder
           .ToTable($"{_tablePrefix}_KnowledgeCollectionFileMetas");

        builder
          .HasKey(a => a.Id);

        builder
          .Property(a => a.Id)
          .HasMaxLength(36);
    }
}
