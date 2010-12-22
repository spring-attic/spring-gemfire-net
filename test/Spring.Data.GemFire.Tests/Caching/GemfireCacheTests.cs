#region

using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Aop.Framework;
using Spring.Aspects.Cache;
using Spring.Context.Support;
using Spring.Data.GemFire.Caching;

#endregion

namespace Spring.Data.GemFire.Tests.Caching
{
    /// <summary>
    /// A code driven test
    /// </summary>
    [TestFixture]
    public class GemfireCacheTests : AbstractGemfireCacheTests
    {
        private CacheAspect cacheAspect;

        [SetUp]
        public void SetUp()
        {
            context = new GenericApplicationContext();

            cacheAspect = new CacheAspect();
            cacheAspect.ApplicationContext = context;
        }

        protected override IInventorRepository CreateInventorStore()
        {

            Region region = CreateRegion();
            cache = new GemFireCache(region);

            context.ObjectFactory.RegisterSingleton("inventors", cache);


            ProxyFactory pf = new ProxyFactory(new InventorRepository());
            pf.AddAdvisors(cacheAspect);

            Repository = (IInventorRepository)pf.GetProxy();
            return Repository;
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