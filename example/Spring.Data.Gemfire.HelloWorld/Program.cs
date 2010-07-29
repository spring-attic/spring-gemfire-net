using System;
using GemStone.GemFire.Cache;
using Spring.Context;
using Spring.Context.Support;

namespace Spring.Data.Gemfire.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IApplicationContext ctx = new XmlApplicationContext("ApplicationContext.xml");

                HelloWorld helloWorld = (HelloWorld) ctx.GetObject("HelloWorld");
                helloWorld.GreetWorld(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("hit enter to exit");
                Console.ReadLine();
            }
            /*
            DistributedSystem system = DistributedSystem.Connect("dist", GemStone.GemFire.Cache.Properties.Create());
            Cache cache = CacheFactory.Create("cache", system);
            cache.Close();
            Console.WriteLine("calling disconnect");
            DistributedSystem.Disconnect();
            Console.WriteLine("Done. Hit enter to exit");
            Console.ReadLine();
            */
        }
    }
}
