using System;
using System.Collections.Generic;
using NewLife.Remoting;

namespace NewLife.Caching
{
    /// <summary>缓存服务</summary>
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
        public Int32 Count() => Cache.Count;

        /// <summary>所有键</summary>
        [Api(nameof(Keys))]
        public ICollection<String> Keys() => Cache.Keys;

        /// <summary>是否包含缓存项</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Api(nameof(ContainsKey))]
        public Boolean ContainsKey(String key) => Cache.ContainsKey(key);

        /// <summary>设置缓存项</summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
        /// <returns></returns>
        [Api(nameof(Set))]
        public Boolean Set<T>(String key, T value, Int32 expire = -1) => Cache.Set(key, value, expire);

        /// <summary>获取缓存项</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [Api(nameof(Get))]
        public T Get<T>(String key) => Cache.Get<T>(key);

        /// <summary>批量移除缓存项</summary>
        /// <param name="keys">键集合</param>
        /// <returns></returns>
        [Api(nameof(Remove))]
        public Int32 Remove(params String[] keys) => Cache.Remove(keys);

        /// <summary>设置缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <param name="expire">过期时间，秒</param>
        [Api(nameof(SetExpire))]
        public Boolean SetExpire(String key, TimeSpan expire) => Cache.SetExpire(key, expire);

        /// <summary>获取缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [Api(nameof(GetExpire))]
        public TimeSpan GetExpire(String key) => Cache.GetExpire(key);
        #endregion

        #region 集合操作
        /// <summary>批量获取缓存项</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        [Api(nameof(GetAll))]
        public IDictionary<String, T> GetAll<T>(IEnumerable<String> keys) => Cache.GetAll<T>(keys);

        /// <summary>批量设置缓存项</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
        [Api(nameof(SetAll))]
        public void SetAll<T>(IDictionary<String, T> values, Int32 expire = -1) => Cache.SetAll(values, expire);
        #endregion

        #region 高级操作
        /// <summary>添加，已存在时不更新</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间<seealso cref="Cache.Expire"/></param>
        /// <returns></returns>
        [Api(nameof(Add))]
        public Boolean Add<T>(String key, T value, Int32 expire = -1) => Cache.Add(key, value, expire);

        /// <summary>设置新值并获取旧值，原子操作</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [Api(nameof(Replace))]
        public T Replace<T>(String key, T value) => Cache.Replace(key, value);

        /// <summary>累加，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        [Api(nameof(Increment))]
        public Int64 Increment(String key, Int64 value) => Cache.Increment(key, value);

        /// <summary>累加，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        [Api(nameof(IncrementDouble))]
        public Double IncrementDouble(String key, Double value) => Cache.Increment(key, value);

        /// <summary>递减，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        [Api(nameof(Decrement))]
        public Int64 Decrement(String key, Int64 value) => Cache.Decrement(key, value);

        /// <summary>递减，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        [Api(nameof(DecrementDouble))]
        public Double DecrementDouble(String key, Double value) => Cache.Decrement(key, value);
        #endregion
    }
}