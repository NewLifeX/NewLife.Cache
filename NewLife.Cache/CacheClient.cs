using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewLife.Remoting;

namespace NewLife.Caching
{
    /// <summary>缓存客户端。对接缓存服务端CacheServer</summary>
    public class CacheClient : Cache
    {
        #region 属性
        /// <summary>客户端</summary>
        public ApiClient Client { get; set; }
        #endregion

        #region 远程操作
        /// <summary>设置服务端地址。支持多地址负载均衡</summary>
        /// <param name="servers"></param>
        /// <returns></returns>
        public ApiClient SetServer(params String[] servers)
        {
            var ac = Client ?? new ApiClient();
            ac.Servers = servers;

            if (ac.Encoder == null) ac.Encoder = new BinaryEncoder();

            Client = ac;

            return ac;
        }

        /// <summary>调用</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual T Invoke<T>(String action, Object args = null) => Task.Run(() => Client.InvokeAsync<T>(action, args)).Result;
        #endregion

        #region 基础操作
        /// <summary>初始化配置</summary>
        /// <param name="config"></param>
        public override void Init(String config) { }

        /// <summary>缓存个数</summary>
        public override Int32 Count => Invoke<Int32>(nameof(Count));

        /// <summary>所有键</summary>
        public override ICollection<String> Keys => Invoke<String[]>(nameof(Keys));

        /// <summary>是否包含缓存项</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override Boolean ContainsKey(String key) => Invoke<Boolean>(nameof(ContainsKey), new { key });

        /// <summary>设置缓存项</summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
        /// <returns></returns>
        public override Boolean Set<T>(String key, T value, Int32 expire = -1) => Invoke<Boolean>(nameof(Set), new { key, value, expire });

        /// <summary>获取缓存项</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override T Get<T>(String key) => Invoke<T>(nameof(Get), new { key });

        /// <summary>批量移除缓存项</summary>
        /// <param name="keys">键集合</param>
        /// <returns></returns>
        public override Int32 Remove(params String[] keys) => Invoke<Int32>(nameof(Remove), new { keys });

        /// <summary>设置缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <param name="expire">过期时间，秒</param>
        public override Boolean SetExpire(String key, TimeSpan expire) => Invoke<Boolean>(nameof(SetExpire), new { key, expire = (Int64)expire.TotalSeconds });

        /// <summary>获取缓存项有效期</summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public override TimeSpan GetExpire(String key) => TimeSpan.FromSeconds(Invoke<Int64>(nameof(GetExpire), new { key }));
        #endregion

        #region 集合操作
        /// <summary>批量获取缓存项</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override IDictionary<String, T> GetAll<T>(IEnumerable<String> keys)
        {
            return Invoke<IDictionary<String, T>>(nameof(GetAll), new { keys = keys.ToArray() });
        }

        /// <summary>批量设置缓存项</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间Expire</param>
        public override void SetAll<T>(IDictionary<String, T> values, Int32 expire = -1)
        {
            Invoke<IDictionary<String, T>>(nameof(SetAll), new { values, expire });
        }
        #endregion

        #region 高级操作
        /// <summary>添加，已存在时不更新</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间，秒。小于0时采用默认缓存时间<seealso cref="Cache.Expire"/></param>
        /// <returns></returns>
        public override Boolean Add<T>(String key, T value, Int32 expire = -1) => Invoke<Boolean>(nameof(Add), new { key, value, expire });

        /// <summary>设置新值并获取旧值，原子操作</summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public override T Replace<T>(String key, T value) => Invoke<T>(nameof(Replace), new { key, value });

        /// <summary>累加，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        public override Int64 Increment(String key, Int64 value) => Invoke<Int64>(nameof(Increment), new { key, value });

        /// <summary>累加，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        public override Double Increment(String key, Double value) => Invoke<Double>(nameof(Increment) + "2", new { key, value });

        /// <summary>递减，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        public override Int64 Decrement(String key, Int64 value) => Invoke<Int64>(nameof(Decrement), new { key, value });

        /// <summary>递减，原子操作</summary>
        /// <param name="key">键</param>
        /// <param name="value">变化量</param>
        /// <returns></returns>
        public override Double Decrement(String key, Double value) => Invoke<Double>(nameof(Decrement) + "2", new { key, value });
        #endregion
    }
}