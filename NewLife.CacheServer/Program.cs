using System;
using NewLife.Agent;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Remoting;

namespace NewLife.CacheServer
{
    class Program
    {
        static void Main(String[] args)
        {
            MyService.ServiceMain();
        }

        class MyService : AgentServiceBase<MyService>
        {
            public MyService()
            {
                ServiceName = "CacheServer";
                DisplayName = "缓存服务器";
            }

            private ApiServer _Server;
            protected override void StartWork(String reason)
            {
                // 服务器
                var svr = new ApiServer(3344);
                svr.Log = XTrace.Log;

#if DEBUG
                svr.EncoderLog = XTrace.Log;

                var ns = svr.EnsureCreate() as Net.NetServer;
                ns.SessionLog = XTrace.Log;
                ns.SocketLog = XTrace.Log;
                ns.LogSend = true;
                ns.LogReceive = true;
#endif

                // 缓存提供者
                MemoryCache.Default.Expire = 24 * 3600;
                var svc = new CacheService
                {
                    Cache = MemoryCache.Default
                };
                svr.Register(svc, null);

                svr.Start();

                _Server = svr;

                base.StartWork(reason);
            }

            protected override void StopWork(String reason)
            {
                _Server.TryDispose();
                _Server = null;

                base.StopWork(reason);
            }

            public override Boolean Work(Int32 index) => false;
        }
    }
}