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
using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;

namespace Spring.Data.GemFire.Tests
{
    [TestFixture]
    public class RegionIntegrationTests
    {
        private IApplicationContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new XmlApplicationContext(@"RegionIntegrationTests.xml");
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        [Test]
        [Ignore]
        public void Foo()
        {
            // 1. Connect to system
            Console.WriteLine("{0}Connecting to GemFire", Environment.NewLine);
            DistributedSystem dsys = DistributedSystem.Connect("exampleregion");

            // 2. Create a cache
            Cache cache = CacheFactory.Create("exampleregion", dsys);

            // 3. Create default region attributes
            AttributesFactory af = new AttributesFactory();
            RegionAttributes rAttrib = af.CreateRegionAttributes();

            // 4. Create region
            Region region = cache.CreateRegion("exampleregion", rAttrib);

        }


        [Test]
        public void BasicRegion()
        {
            Region region = (Region) ctx.GetObject("basic");
            Assert.AreEqual("basic", region.Name);
        }

        [Test]
        public void ExistingRegion()
        {
            Region region = (Region)ctx.GetObject("root");
            Assert.AreEqual("root", region.Name);
        }

        [Test]
        public void RegionWithListeners()
        {
            Region region = (Region) ctx.GetObject("listeners");
            Assert.AreEqual("listeners", region.Name);
            Assert.IsInstanceOf(typeof(SimpleCacheListener), region.Attributes.CacheListener);
            Assert.IsInstanceOf(typeof(SimpleCacheLoader), region.Attributes.CacheLoader);
            Assert.IsInstanceOf(typeof(SimpleCacheWriter), region.Attributes.CacheWriter);
        }

        [Test]
        public void RegionWithAllKeyInterest()
        {
            Region region = (Region)ctx.GetObject("basic-allkey-interest");
            Assert.AreEqual("basic-allkey-interest", region.Name);
        }

        [Test]
        public void Hello()
        {
            DistributedSystem dsys = DistributedSystem.Connect("exampledstest");
            Cache cache = CacheFactory.Create("exampledscache", dsys);            
            
            AttributesFactory af = new AttributesFactory();
            af.SetClientNotificationEnabled(true);
            af.SetEndpoints("localhost:40404");
            RegionAttributes rAttrib = af.CreateRegionAttributes();
            Region region = cache.CreateRegion("exampledsregion", rAttrib);
            

            region.RegisterAllKeys();

            cache.Close();
        }
    }
}