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
using Spring.Aop.Framework;
using Spring.Caching;
using Spring.Context;
using Spring.Data.GemFire.Caching;

namespace Spring.Data.GemFire.Tests.Caching
{
    [TestFixture]
    public abstract class AbstractGemfireCacheTests
    {

        protected IConfigurableApplicationContext context;

        protected IInventorRepository Repository;

        protected ICache cache;


        [Test]
        public void TestCaching()
        {
            CreateInventorStore();

            Assert.AreEqual(0, cache.Count);

            IList inventors = Repository.GetAll();
            Assert.AreEqual(2, cache.Count);

            Repository.Delete((Inventor)inventors[0]);
            Assert.AreEqual(1, cache.Count);

            Inventor tesla = Repository.Load("Nikola Tesla");
            Assert.AreEqual(2, cache.Count);

            Repository.Save(tesla);
            Assert.AreEqual(2, cache.Count);
            Assert.AreEqual("Serbian", ((Inventor)cache.Get("Nikola Tesla")).Nationality);

            Repository.DeleteAll();
            Assert.AreEqual(0, cache.Count);
        }

        protected abstract IInventorRepository CreateInventorStore();
    }
}