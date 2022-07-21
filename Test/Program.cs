using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NewLife;
using NewLife.Caching;
using NewLife.Data;
using NewLife.Log;
using NewLife.Reflection;
using NewLife.Serialization;

namespace Test;

internal class Program
{
    private static void Main(String[] args)
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

    private static void Test1()
    {
        var client = new CacheClient();
        client.SetServer("tcp://127.0.0.1:1234");

#if !DEBUG
        client.Bench();
#else
        client.Client.EncoderLog = XTrace.Log;

        Console.WriteLine("缓存个数：{0}", client.Count);

        Debug.Assert(client.Set("aa", 1234));
        Debug.Assert(client.ContainsKey("aa"));
        Debug.Assert(client.Get<Int32>("aa") == 1234);

        client.Set("bb", false);
        client.Set("cc", 3.14);
        client.Set("dd", "NewLife", 5);
        client.Set("ee", new Student { Name = "新生命", Age = 24 });

        Console.WriteLine(client.Get<Int32>("aa"));
        Console.WriteLine(client.Get<Boolean>("bb"));
        Console.WriteLine(client.Get<Double>("cc"));
        Console.WriteLine(client.Get<String>("dd"));
        Console.WriteLine(client.Get<Student>("ee").ToJson());

        Console.WriteLine();
        Console.WriteLine("Count={0}", client.Count);
        Console.WriteLine("Keys={0}", client.Keys.Join(","));
        Thread.Sleep(2000);
        Console.WriteLine("Expire={0}", client.GetExpire("dd"));

        Console.WriteLine();
        client.Decrement("aa", 30);
        client.Increment("cc", 0.3);

        Console.WriteLine();
        var dic = client.GetAll<Packet>(new[] { "aa", "bb", "cc", "dd", "ee" });
        foreach (var item in dic)
        {
            var val = item.Value.ToHex();
            //if (val != null && item.Value.GetType().GetTypeCode() == TypeCode.Object) val = val.ToHex();

            Console.WriteLine("{0}={1}", item.Key, val);
        }
#endif
    }

    class Student
    {
        public String Name { get; set; }

        public Int32 Age { get; set; }
    }
}