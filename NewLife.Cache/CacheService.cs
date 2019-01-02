﻿using System;
using System.Collections.Generic;
using NewLife.Data;
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
        /// <param name="key">键</param>
        /// <returns></returns>
        [Api(nameof(ContainsKey))]
        public Packet ContainsKey(Packet key)
        {
            var rs = Cache.ContainsKey(key.ToStr());
            return new[] { (Byte)(rs ? 1 : 0) };
        }

        /// <summary>设置缓存项</summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [Api(nameof(Set))]
        public Packet Set(Packet data)
        {
            var ms = data.GetStream();
            var key = ms.ReadArray().ToStr();
            var expire = ms.ReadBytes(4).ToInt();
            var value = ms.ReadBytes();

            var rs = Cache.Set(key, value, expire);
            return new[] { (Byte)(rs ? 1 : 0) };
        }

        /// <summary>获取缓存项</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [Api(nameof(Get))]
        public Packet Get(Packet key) => Cache.Get<Byte[]>(key.ToStr());

        /// <summary>批量移除缓存项</summary>
        /// <param name="keys">键集合</param>
        /// <returns></returns>
        [Api(nameof(Remove))]
        public Int32 Remove(Packet keys)
        {
            var ks = new List<String>();
            var ms = keys.GetStream();
            while (ms.Position < ms.Length)
            {
                ks.Add(ms.ReadArray().ToStr());
            }

            return Cache.Remove(ks.ToArray());
        }

        /// <summary>设置缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <param name="expire">过期时间，秒</param>
        [Api(nameof(SetExpire))]
        public Boolean SetExpire(String key, Int64 expire) => Cache.SetExpire(key, TimeSpan.FromSeconds(expire));

        /// <summary>获取缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [Api(nameof(GetExpire))]
        public Int64 GetExpire(String key) => (Int64)Cache.GetExpire(key).TotalSeconds;
        #endregion

        #region 集合操作
        /// <summary>批量获取缓存项</summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        [Api(nameof(GetAll))]
        public IDictionary<String, Object> GetAll(String[] keys) => Cache.GetAll<Object>(keys);

        /// <summary>批量设置缓存项</summary>
        /// <param name="values"></param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
        [Api(nameof(SetAll))]
        public void SetAll(IDictionary<String, Object> values, Int32 expire = -1) => Cache.SetAll(values, expire);
        #endregion

        #region 高级操作
        /// <summary>添加，已存在时不更新</summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间<seealso cref="Cache.Expire"/></param>
        /// <returns></returns>
        [Api(nameof(Add))]
        public Boolean Add(String key, Object value, Int32 expire = -1) => Cache.Add(key, value, expire);

        /// <summary>设置新值并获取旧值，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [Api(nameof(Replace))]
        public Object Replace(String key, Object value) => Cache.Replace(key, value);

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
        [Api(nameof(Increment2))]
        public Double Increment2(String key, Double value) => Cache.Increment(key, value);

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
        [Api(nameof(Decrement2))]
        public Double Decrement2(String key, Double value) => Cache.Decrement(key, value);
        #endregion
    }
}