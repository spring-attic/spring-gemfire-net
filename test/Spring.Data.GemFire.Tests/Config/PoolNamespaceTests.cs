#region License

/*
 * Copyright 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System.Collections;
using GemStone.GemFire.Cache;
using NUnit.Framework;
using Spring.Context;
using Spring.Context.Support;
using Spring.Data.GemFire.Config;
using Spring.Objects.Factory.Xml;

namespace Spring.Data.GemFire.Tests.Config
{
    [TestFixture]
    public class PoolNamespaceTests
    {
        private IApplicationContext ctx;

        [SetUp]
        public void Setup()
        {
            NamespaceParserRegistry.RegisterParser(typeof(GemfireNamespaceParser));
            ctx = new XmlApplicationContext(@"Config\PoolNamespaceTests.xml");
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        [Test]
        public void BasicPool()
        {
            Assert.IsTrue(ctx.ContainsObject("gemfire-pool"));
            GemStone.GemFire.Cache.Pool p1 = (GemStone.GemFire.Cache.Pool) ctx.GetObject("gemfire-pool");
            GemStone.GemFire.Cache.Pool p2 = PoolManager.Find("gemfire-pool");
            //TODO Seems to be a clone, can use equals in Java
            //Assert.AreEqual(p1, p2);
            PoolFactoryObject pfo = (PoolFactoryObject) ctx.GetObject("&gemfire-pool");
            IList locators = TestUtils.ReadField<IList>("locators", pfo);
            Assert.AreEqual(1, locators.Count);
            PoolConnection locator = (PoolConnection) locators[0];
            Assert.AreEqual("localhost", locator.Host);
            Assert.AreEqual(40403, locator.Port);
        }

        [Test]
        public void ComplexPool()
        {
            Assert.IsTrue(ctx.ContainsObject("complex"));
            PoolFactoryObject pfo = (PoolFactoryObject)ctx.GetObject("&complex");
            Assert.AreEqual(30, TestUtils.ReadField<int>("retryAttempts", pfo));
            Assert.AreEqual(6000, TestUtils.ReadField<int>("freeConnectionTimeout", pfo));
            Assert.AreEqual(5000, TestUtils.ReadField<int>("pingInterval", pfo));
            Assert.IsTrue(TestUtils.ReadField<bool>("subscriptionEnabled", pfo));


            IList servers = TestUtils.ReadField<IList>("servers", pfo);
            Assert.AreEqual(2, servers.Count);
            PoolConnection server = (PoolConnection)servers[0];
            Assert.AreEqual("localhost", server.Host);
            Assert.AreEqual(40404, server.Port);
            server = (PoolConnection)servers[1];
            Assert.AreEqual("localhost", server.Host);
            Assert.AreEqual(40405, server.Port);
        }
    }
}