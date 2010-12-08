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
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Objects.Factory;
using Spring.Util;

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Simple FactoryObject for retrieving generic GemFire Regions. If the region doesn't exist, an exception is thrown.
    /// For declaring and configuring new regions, see <see cref="ClientRegionFactoryObject"/>.
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class RegionLookupFactoryObject : IFactoryObject, IInitializingObject, IObjectNameAware
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RegionLookupFactoryObject));

        private string objectName;
        private Cache cache;
        private string name;
        protected internal Region region;

        public virtual void AfterPropertiesSet()
        {
            AssertUtils.ArgumentNotNull(cache, "Cache Property must be set");
            name = (!StringUtils.HasText(name) ? objectName : name);
            AssertUtils.ArgumentHasText(name, "Name (or ObjectName) property must be set");

            // first get cache
            region = cache.GetRegion(name);
            if (region != null)
            {
                log.Info("Retrieved region [" + name + "] from cache");
            }
            // fall back to cache creation if one is not found
            else
            {
                region = LookupFallback(cache, name);
            }
        }

        protected virtual Region LookupFallback(Cache cacheObject, string regionName)
        {
            throw new ObjectInitializationException("Cannot find region named " + regionName + " in cache " + cacheObject);
        }

        public object GetObject()
        {
            return region;
        }

        public Type ObjectType
        {
            get
            {
                //return (region != null ? region.GetType() : typeof (Region));
                return typeof(Region);
            }
        }

        public bool IsSingleton
        {
            get { return true; }
        }
        public string ObjectName
        {
            set { objectName = value; }
        }



        /// <summary>
        /// Sets the cache used for creating the region
        /// </summary>
        /// <value>The cache to set</value>
        public Cache Cache
        {
            set { cache = value; }
        }
   

        /// <summary>
        /// Sets the name of the cache region. If no cache is found under
        /// the given name, a new one will be created.
        /// If no name is given, the objectName will be used.
        /// </summary>
        /// <value>The region name.</value>
        public string Name
        {
            set { name = value; }
        }
    }
}