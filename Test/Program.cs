using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NewLife.Caching;
using NewLife.Log;
using NewLife.Serialization;

namespace Test
{
    class Program
    {
        static void Main(String[] args)
        {
            XTrace.UseConsole();

            try
            {
                Test1();
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            Console.WriteLine("OK");
            Console.Read();
        }

        static void Test1()
        {
            var client = new CacheClient();
            client.SetServer("tcp://127.0.0.1:1234");

            //client.Bench();

            client.Set("aa", 1234);
            client.Set("bb", false);
            client.Set("cc", 3.14);
            client.Set("dd", "NewLife", 5);
            client.Set("ee", new { Name = "新生命", Year = 2002 });

            Console.WriteLine(client.Get<Int32>("aa"));
            Console.WriteLine(client.Get<Boolean>("bb"));
            Console.WriteLine(client.Get<Double>("cc"));
            Console.WriteLine(client.Get<String>("dd"));
            Console.WriteLine(client.Get<Object>("ee").ToJson());

            Console.WriteLine("Count={0}", client.Count);
            Console.WriteLine("Keys={0}", client.Keys.Join());
            Thread.Sleep(2000);
            Console.WriteLine("Expire={0}", client.GetExpire("dd"));
        }
    }
}