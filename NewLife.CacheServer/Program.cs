﻿using NewLife.Agent;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Remoting;

namespace NewLife.CacheServer;

internal class Program
{
    private static void Main(String[] args) => new MyService().Main(args);

    private class MyService : ServiceBase
    {
        public MyService()
        {
            ServiceName = "CacheServer";
            DisplayName = "缓存服务器";
        }

        private ApiServer _Server;
        public override void StartWork(String reason)
        {
            // 配置
            var set = Setting.Current;

            // 服务器
            var svr = new ApiServer(set.Port)
            {
                //Encoder = new BinaryEncoder(),
                Log = XTrace.Log,
                ShowError = true,
            };

            if (set.Debug) svr.EncoderLog = XTrace.Log;

            // 网络层日志
            var ns = svr.EnsureCreate() as Net.NetServer;
            ns.SessionLog = XTrace.Log;
            ns.SocketLog = XTrace.Log;
#if DEBUG
            //ns.LogSend = true;
            //ns.LogReceive = true;
            svr.EncoderLog = XTrace.Log;
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

            base.StartWork(reason);
        }

        public override void StopWork(String reason)
        {
            _Server.TryDispose();
            _Server = null;

            base.StopWork(reason);
        }
    }
}