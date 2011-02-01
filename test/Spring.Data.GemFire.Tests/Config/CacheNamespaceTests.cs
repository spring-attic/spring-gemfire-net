#region License

/*
 * Copyright 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Specialized;
using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using Spring.Data.GemFire.Config;
using Spring.Objects.Factory.Xml;

namespace Spring.Data.GemFire.Tests.Config
{
    [TestFixture]
    public class CacheNamespaceTests
    {

        private IApplicationContext ctx;

        [SetUp]
        public void Setup()
        {
            NamespaceParserRegistry.RegisterParser(typeof(GemfireNamespaceParser));
            ctx = new XmlApplicationContext(@"Config\CacheNamespaceTests.xml");
        }
        
        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();            
        }

        [Test]
        public void BasicCache()
        {
            //This is retrieving the object by the default name.
            Assert.IsTrue(ctx.ContainsObject("gemfire-cache"));
            CacheFactoryObject cacheFactoryObject = (CacheFactoryObject) ctx.GetObject("&gemfire-cache");
            AssertDefaultTestSettings(cacheFactoryObject, "MySystemName");
        }


        [Test]
        public void NamedCache()
        {            
            Assert.IsTrue(ctx.ContainsObject("cache-with-name"));
            CacheFactoryObject cacheFactoryObject = (CacheFactoryObject)ctx.GetObject("&cache-with-name");
            AssertDefaultTestSettings(cacheFactoryObject, "DistributedSystemDotNet");
        }

        [Test]
        public void CacheWithXml()
        {
            Assert.IsTrue(ctx.ContainsObject("cache-with-xml"));
            CacheFactoryObject cacheFactoryObject = (CacheFactoryObject)ctx.GetObject("&cache-with-xml");
            string cacheXmlFile = TestUtils.ReadField<string>("cacheXml", cacheFactoryObject);
            Assert.IsNotNull(cacheXmlFile);
            Assert.AreEqual("cache-config.xml", cacheXmlFile);

            NameValueCollection nvc = new NameValueCollection();
            nvc["foo"] = "bar";
        }

        [Test]
        public void RegionLookup()
        {
            Cache cache = (Cache) ctx["gemfire-cache"];
            AttributesFactory attributesFactory = new AttributesFactory();
            attributesFactory.SetScope(ScopeType.Local);
            attributesFactory.SetCachingEnabled(true);
            RegionAttributes regionAttributes = attributesFactory.CreateRegionAttributes();
            Region existing = cache.CreateRegion("existing", regionAttributes);
            Assert.IsTrue(ctx.ContainsObject("lookup"));
            RegionLookupFactoryObject regionLookupFactoryObject = (RegionLookupFactoryObject)ctx.GetObject("&lookup");
            Assert.AreEqual("existing", TestUtils.ReadField<string>("name", regionLookupFactoryObject));
            //TODO SGFNET-20: existing is not registered as an alias with lookup/.
            //Assert.AreEqual(ctx.GetObject("existing"), ctx.GetObject("lookup"));
        }

        private void AssertDefaultTestSettings(CacheFactoryObject cacheFactoryObject, string distributedSystemName)
        {
            Assert.IsNull(TestUtils.ReadField<string>("cacheXml", cacheFactoryObject));
            Assert.IsNull(TestUtils.ReadField<NameValueCollection>("properties", cacheFactoryObject));
            Assert.AreEqual(distributedSystemName, TestUtils.ReadField<string>("distributedSystemName", cacheFactoryObject));
            Assert.IsTrue(TestUtils.ReadField<bool>("disconnectOnClose", cacheFactoryObject));
        }
    }
}