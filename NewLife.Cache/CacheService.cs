using NewLife.Collections;
using NewLife.Data;
using NewLife.Remoting;
using NewLife.Serialization;

namespace NewLife.Caching;

/// <summary>缓存服务</summary>
/// <remarks>所有接口以IPacket作为出入参，避免Json序列化，最大程度提升性能</remarks>
[Api(null)]
public class CacheService
{
    #region 属性
    /// <summary>适配对象</summary>
    public ICache Cache { get; set; } = MemoryCache.Default;
    #endregion

    #region 基础操作
    /// <summary>缓存个数</summary>
    [Api(nameof(Count))]
    public IPacket Count() => (ArrayPacket)Cache.Count.GetBytes();

    /// <summary>所有键</summary>
    [Api(nameof(Keys))]
    public IPacket Keys()
    {
        var ks = Cache.Keys;

        var ms = Pool.MemoryStream.Get();
        foreach (var item in ks)
        {
            ms.WriteArray(item.GetBytes());
        }

        return (ArrayPacket)ms.Return(true);
    }

    /// <summary>是否包含缓存项</summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [Api(nameof(ContainsKey))]
    public IPacket ContainsKey(IPacket key)
    {
        var rs = Cache.ContainsKey(key.ToStr());
        return (ArrayPacket)new[] { (Byte)(rs ? 1 : 0) };
    }

    /// <summary>设置缓存项</summary>
    /// <param name="data">参数</param>
    /// <returns></returns>
    [Api(nameof(Set))]
    public IPacket Set(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var expire = (Int32)ms.ReadBytes(4).ToUInt32(0, false);
        var value = ms.ReadBytes(-1);

        var rs = Cache.Set(key, value, expire);
        return (ArrayPacket)new[] { (Byte)(rs ? 1 : 0) };
    }

    /// <summary>获取缓存项</summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [Api(nameof(Get))]
    public IPacket Get(IPacket key) => (ArrayPacket)Cache.Get<Byte[]>(key.ToStr());

    /// <summary>批量移除缓存项</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Remove))]
    public IPacket Remove(IPacket data)
    {
        var keys = new List<String>();
        var ms = data.GetStream();
        while (ms.Position < ms.Length)
        {
            keys.Add(ms.ReadArray().ToStr());
        }

        return (ArrayPacket)Cache.Remove(keys.ToArray()).GetBytes();
    }

    /// <summary>设置缓存项有效期</summary>
    /// <param name="data">数据</param>
    [Api(nameof(SetExpire))]
    public IPacket SetExpire(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var expire = ms.ReadBytes(4).ToInt();

        var rs = Cache.SetExpire(key, TimeSpan.FromSeconds(expire));
        return (ArrayPacket)new[] { (Byte)(rs ? 1 : 0) };
    }

    /// <summary>获取缓存项有效期</summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    [Api(nameof(GetExpire))]
    public IPacket GetExpire(IPacket key)
    {
        var rs = (Int64)Cache.GetExpire(key.ToStr()).TotalSeconds;
        return (ArrayPacket)rs.GetBytes();
    }
    #endregion

    #region 集合操作
    /// <summary>批量获取缓存项</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(GetAll))]
    public IPacket GetAll(IPacket data)
    {
        var keys = new List<String>();
        var ms = data.GetStream();
        while (ms.Position < ms.Length)
        {
            keys.Add(ms.ReadArray().ToStr());
        }

        var dic = Cache.GetAll<Object>(keys);
        var ms2 = Pool.MemoryStream.Get();
        var bn = new Binary { Stream = ms2 };
        foreach (var item in dic)
        {
            bn.Write(item.Key);

            // 统一使用二进制序列化返回数据，否则客户端无法解码
            if (item.Value is IPacket pk)
                bn.Write(pk);
            else if (item.Value is Byte[] buf)
                bn.Write((ReadOnlySpan<Byte>)buf);
            else
                bn.Write(Binary.FastWrite(item.Value));
        }

        return (ArrayPacket)ms2.Return(true);
    }

    /// <summary>批量设置缓存项</summary>
    /// <param name="values"></param>
    /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
    [Api(nameof(SetAll))]
    public void SetAll(IDictionary<String, Object> values, Int32 expire = -1) => Cache.SetAll(values, expire);
    #endregion

    #region 高级操作
    /// <summary>添加，已存在时不更新</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Add))]
    public IPacket Add(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var expire = ms.ReadBytes(4).ToInt();
        var value = ms.ReadBytes(-1);

        var rs = Cache.Add(key, value, expire);
        return (ArrayPacket)new[] { (Byte)(rs ? 1 : 0) };
    }

    /// <summary>设置新值并获取旧值，原子操作</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Replace))]
    public IPacket Replace(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var value = ms.ReadBytes(-1);

        return (ArrayPacket)Cache.Replace(key, value);
    }

    /// <summary>累加，原子操作</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Increment))]
    public IPacket Increment(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var value = ms.ReadBytes(8).ToLong();

        var rs = Cache.Increment(key, value);
        return (ArrayPacket)rs.GetBytes();
    }

    /// <summary>累加，原子操作</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Increment2))]
    public IPacket Increment2(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var value = ms.ReadBytes(8).ToDouble();

        var rs = Cache.Increment(key, value);
        return (ArrayPacket)BitConverter.GetBytes(rs);
    }

    /// <summary>递减，原子操作</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Decrement))]
    public IPacket Decrement(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var value = ms.ReadBytes(8).ToLong();

        var rs = Cache.Decrement(key, value);
        return (ArrayPacket)rs.GetBytes();
    }

    /// <summary>递减，原子操作</summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    [Api(nameof(Decrement2))]
    public IPacket Decrement2(IPacket data)
    {
        var ms = data.GetStream();
        var key = ms.ReadArray().ToStr();
        var value = ms.ReadBytes(8).ToDouble();

        var rs = Cache.Decrement(key, value);
        return (ArrayPacket)BitConverter.GetBytes(rs);
    }
    #endregion
}