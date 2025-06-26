# BotSharp PostgreSQL File Storage Plugin

## 概述

BotSharp.Plugin.PostgreSqlFileStorage 是一个基于PostgreSQL数据库的文件存储插件，设计用于测试环境以简化部署架构。该插件将文件以二进制数据的形式存储在PostgreSQL数据库中，避免了对外部对象存储服务的依赖。

## 特性

- ✅ 实现完整的 `IFileStorageService` 接口
- ✅ 支持对话文件存储与管理
- ✅ 支持用户头像存储
- ✅ 支持语音文件存储
- ✅ 支持知识库文档存储
- ✅ 基于PostgreSQL的事务一致性
- ✅ 自动文件元数据管理
- ✅ 支持文件分类和索引

## 配置

### 1. appsettings.json 配置

```json
{
  "Database": {
    "Default": "PostgreSqlRepository",
    "BotSharpPostgreSql": "Host=localhost;Database=botsharp;Username=postgres;Password=your_password"
  },
  "FileCore": {
    "Storage": "PostgreSqlFileStorage"
  }
}
```

### 2. 数据库迁移

插件会自动创建所需的数据表：

```sql
CREATE TABLE file_storages (
    id UUID PRIMARY KEY,
    file_path VARCHAR(1000) UNIQUE NOT NULL,
    file_name VARCHAR(255) NOT NULL,
    content_type VARCHAR(100) NOT NULL,
    file_size BIGINT NOT NULL,
    file_data BYTEA NOT NULL,
    category VARCHAR(50) NOT NULL,
    entity_id VARCHAR(50),
    metadata JSONB,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NOT NULL,
    directory VARCHAR(500) NOT NULL
);

-- 创建索引以优化查询性能
CREATE INDEX idx_file_storages_file_path ON file_storages(file_path);
CREATE INDEX idx_file_storages_category ON file_storages(category);
CREATE INDEX idx_file_storages_entity_id ON file_storages(entity_id);
CREATE INDEX idx_file_storages_directory ON file_storages(directory);
CREATE INDEX idx_file_storages_created_at ON file_storages(created_at);
```

## 使用方法

### 1. 启用插件

在你的BotSharp应用程序中，确保PostgreSQL文件存储插件被正确加载：

```csharp
// 插件会根据配置自动注册
services.AddBotSharp(Configuration);
```

### 2. 依赖注入

```csharp
public class YourService
{
    private readonly IFileStorageService _fileStorage;
    
    public YourService(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }
    
    public async Task SaveFile(string conversationId, string messageId, FileDataModel file)
    {
        var files = new List<FileDataModel> { file };
        var result = _fileStorage.SaveMessageFiles(conversationId, messageId, "user", files);
        // ...
    }
}
```

## 文件存储结构

插件模拟了与TencentCos插件相同的目录结构：

```
conversations/
├── {conversationId}/
│   ├── files/
│   │   └── {messageId}/
│   │       ├── user/
│   │       │   └── {index}/
│   │       │       └── filename.ext
│   │       └── bot/
│   │           └── {index}/
│   │               └── filename.ext
│   └── speeches/
│       └── speech_file.mp3
├── users/
│   └── {userId}/
│       └── avatar/
│           └── avatar.jpg
└── knowledgebase/
    └── document/
        └── {vectorStoreProvider}/
            └── {collectionName}/
                └── {fileId}/
                    └── document.pdf
```

## 性能考虑

### 优点
- 事务一致性保证
- 简化的部署架构
- 统一的数据管理
- 自动备份（随数据库备份）

### 限制
- 不适合大文件存储（建议单文件不超过100MB）
- 数据库体积会增长较快
- 缺少CDN加速支持
- 不支持多地域复制

### 性能优化建议

1. **文件大小限制**：建议对上传文件设置大小限制
2. **定期清理**：实现定期清理过期文件的机制
3. **连接池配置**：适当配置数据库连接池大小
4. **索引优化**：根据查询模式添加合适的索引

## 与TencentCos插件的对比

| 特性 | PostgreSqlFileStorage | TencentCos |
|------|----------------------|------------|
| 部署复杂度 | 低 | 中 |
| 文件大小限制 | 建议<100MB | 几乎无限制 |
| 并发性能 | 中 | 高 |
| CDN支持 | 无 | 有 |
| 成本 | 低（测试环境） | 按用量计费 |
| 可扩展性 | 有限 | 高 |

## 适用场景

✅ **推荐使用场景**：
- 开发和测试环境
- 小型应用（文件总量<10GB）
- 简化部署需求
- 对数据一致性要求高的场景

❌ **不推荐使用场景**：
- 生产环境大规模应用
- 大文件存储需求
- 高并发文件访问
- 需要CDN加速的场景

## 迁移工具

可以通过以下方式从其他存储方式迁移到PostgreSQL文件存储：

```csharp
// TODO: 实现迁移工具
public class FileStorageMigrator
{
    public async Task MigrateFromLocalStorage(string localPath)
    {
        // 实现从本地文件存储迁移
    }
    
    public async Task MigrateFromTencentCos(TencentCosSettings settings)
    {
        // 实现从腾讯云COS迁移
    }
}
```

## 故障排除

### 常见问题

1. **连接数据库失败**
   - 检查连接字符串配置
   - 确认PostgreSQL服务运行状态
   - 验证用户权限

2. **文件上传失败**
   - 检查文件大小是否超限
   - 确认数据库磁盘空间充足
   - 查看错误日志

3. **性能问题**
   - 检查数据库索引
   - 考虑文件大小和数量
   - 优化查询条件

## 开发者信息

- 插件ID: `f8e9d7c6-b5a4-3210-9876-543210fedcba`
- 插件名称: `PostgreSQL File Storage`
- 版本: 1.0.0
- 兼容性: BotSharp 5.0+
