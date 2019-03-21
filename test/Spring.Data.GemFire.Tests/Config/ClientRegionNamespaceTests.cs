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
using Spring.Util;

namespace Spring.Data.GemFire.Tests.Config
{
    [TestFixture]
    public class ClientRegionNamespaceTests
    {
        private IApplicationContext ctx;

        [SetUp]
        public void Setup()
        {
            NamespaceParserRegistry.RegisterParser(typeof(GemfireNamespaceParser));
            ctx = new XmlApplicationContext(@"Config\ClientRegionNamespaceTests.xml");
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        [Test]
        public void BasicClient()
        {
            Assert.IsTrue(ctx.ContainsObject("simple"));
        }

        [Test]
        public void ComplexClient()
        {
            Assert.IsTrue(ctx.ContainsObject("complex"));           
            ClientRegionFactoryObject clientRegionFactoryObject = (ClientRegionFactoryObject) ctx.GetObject("&complex");
          


            ICacheListener[] listeners = TestUtils.ReadField<ICacheListener[]>("cacheListener", clientRegionFactoryObject);
            Assert.IsFalse(ObjectUtils.IsEmpty(listeners));
            Assert.AreEqual(2, listeners.Length);
            Assert.AreSame(listeners[0], ctx.GetObject("c-listener"));
            IInterest[] ints = TestUtils.ReadField<IInterest[]>("interests",clientRegionFactoryObject);
            Assert.AreEqual(3, ints.Length);

            // allkey interest
            IInterest allKeysInt = ints[0];
            Assert.IsTrue(TestUtils.ReadField<bool>("durable", allKeysInt));
            Assert.AreEqual(InterestResultPolicy.Keys, TestUtils.ReadField<InterestResultPolicy>("policy", allKeysInt));

            // key interest
            IInterest keyInt = ints[1];
            Assert.IsFalse(TestUtils.ReadField<bool>("durable", keyInt));
            Assert.AreEqual(InterestResultPolicy.KeysAndValues, TestUtils.ReadField<InterestResultPolicy>("policy", keyInt));

            // regex interest            
            RegexInterest regexInterest = (RegexInterest) ints[2];
            Assert.IsFalse(TestUtils.ReadField<bool>("durable", regexInterest));
            Assert.AreEqual(InterestResultPolicy.KeysAndValues, TestUtils.ReadField<InterestResultPolicy>("policy", regexInterest));












        }

    }
}