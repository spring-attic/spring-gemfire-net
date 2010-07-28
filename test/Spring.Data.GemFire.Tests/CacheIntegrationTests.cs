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

using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using Spring.Objects.Factory.Xml;

namespace Spring.Data.GemFire.Tests
{
    [TestFixture]
    public class CacheIntegrationTests
    {

        private IApplicationContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new XmlApplicationContext(@"CacheIntegrationTests.xml");
        }
        
        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();            
        }

        [Test]
        public void BasicCache()
        {
            Cache cache = (Cache) ctx.GetObject("default-cache");
            Assert.AreEqual("NativeCache", cache.Name);
        }

        [Test]
        public void CacheWithName()
        {
            Cache cache = (Cache)ctx.GetObject("cache-with-name");
            Assert.AreEqual("cache-with-name", cache.Name);
            //This is the default name of the distributed system.
            Assert.AreEqual("DistributedSystemDotNet", cache.DistributedSystem.Name);
        }

        [Test]
        public void CacheWithProps()
        {
            Cache cache = (Cache) ctx.GetObject("cache-with-props");
            Assert.AreEqual("cache-with-props", cache.Name);  
            Assert.AreEqual("MySpringDistributedSystem", cache.DistributedSystem.Name);
        }

        [Test]
        public void CacheWithXml()
        {
            Cache cache = (Cache) ctx.GetObject("cache-with-xml");
        }
    }
}