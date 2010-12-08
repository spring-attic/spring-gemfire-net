#region

using System;
using System.Collections;
using System.Collections.Specialized;
using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Aop.Framework;
using Spring.Aspects.Cache;
using Spring.Caching;
using Spring.Context.Support;
using Spring.Data.GemFire.Caching;

#endregion

namespace Spring.Data.GemFire.Tests.Caching
{
    [TestFixture]
    public class GemfireCacheTests
    {

        private GenericApplicationContext context;
        private CacheAspect cacheAspect;

        [SetUp]
        public void SetUp()
        {
            context = new GenericApplicationContext();

            cacheAspect = new CacheAspect();
            cacheAspect.ApplicationContext = context;
        }
        [Test]
        public void TestCaching()
        {
            Region region = CreateRegion();
            ICache cache = new GemFireCache(region);
            
            context.ObjectFactory.RegisterSingleton("inventors", cache);

            ProxyFactory pf = new ProxyFactory(new InventorStore());
            pf.AddAdvisors(cacheAspect);

            IInventorStore store = (IInventorStore)pf.GetProxy();

            Assert.AreEqual(0, cache.Count);

            IList inventors = store.GetAll();
            Assert.AreEqual(2, cache.Count);

            store.Delete((Inventor)inventors[0]);
            Assert.AreEqual(1, cache.Count);

            Inventor tesla = store.Load("Nikola Tesla");
            Assert.AreEqual(2, cache.Count);

            store.Save(tesla);
            Assert.AreEqual(2, cache.Count);
            Assert.AreEqual("Serbian", ((Inventor)cache.Get("Nikola Tesla")).Nationality);

            store.DeleteAll();
            Assert.AreEqual(0, cache.Count);
        }

        private Region CreateRegion()
        {

            DistributedSystem dsys = DistributedSystem.Connect("exampleregion");
            Cache cache = CacheFactory.Create("exampleregion", dsys);
            AttributesFactory attributesFactory = new AttributesFactory();
            attributesFactory.SetScope(ScopeType.Local);
            attributesFactory.SetCachingEnabled(true);
            RegionAttributes regionAttributes = attributesFactory.CreateRegionAttributes();

            return cache.CreateRegion("exampleregion", regionAttributes);                      
        }
    }

}