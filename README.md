## 缓存中间件
NewLife.Cache 是一个基于RPC的缓存中间件，客户端CacheClient的所有操作映射到服务端CacheService的相应接口。  
CacheService 采用适配器模式，默认由MemoryCache提供实现，也可以更换为DbCache、RocksDB等其它嵌入式缓存实现。  

这不仅是一个缓存中间件，还是一个标准的RPC通讯（ApiServer）示例！  
