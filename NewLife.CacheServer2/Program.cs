using System;
using System.Threading;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Remoting;

namespace NewLife.CacheServer
{
    class Program
    {
        private static ApiServer _Server;
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            // 配置
            var set = Setting.Current;

            // 服务器
            var svr = new ApiServer(set.Port)
            {
                Log = XTrace.Log
            };

            if (set.Debug) svr.EncoderLog = XTrace.Log;

            // 网络层日志
            var ns = svr.EnsureCreate() as Net.NetServer;
            ns.SessionLog = XTrace.Log;
            ns.SocketLog = XTrace.Log;
#if DEBUG
            ns.LogSend = true;
            ns.LogReceive = true;
#endif

            // 统计日志
            svr.StatPeriod = 10;

            // 缓存提供者
            var mc = new MemoryCache();
            if (set.Expire > 0) mc.Expire = set.Expire;

            // 注册RPC服务
            var svc = new CacheService { Cache = mc };
            svr.Register(svc, null);

            svr.Start();

            _Server = svr;

            Thread.Sleep(-1);
        }
    }
}