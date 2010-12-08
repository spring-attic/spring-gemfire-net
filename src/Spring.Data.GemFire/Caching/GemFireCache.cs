#region License

/*
 * Copyright 2002-2009 the original author or authors.
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
using System.Collections;
using GemStone.GemFire.Cache;
using Spring.Caching;

namespace Spring.Data.GemFire.Caching {
    /// <summary>
    /// Cache implementation for Gemfire
    /// </summary>
    /// <author></author>
    public class GemFireCache : AbstractCache
    {
        private Region region;

        public GemFireCache(Region region)
        {
            this.region = region;
        }

        #region Overrides of AbstractCache

        public override object Get(object key)
        {
            string cachableKey = GetCachableKey(key);
            IGFSerializable cacheableValue = region.Get(cachableKey);
            if (cacheableValue == null) return null;

            CacheableObject cacheableObject = cacheableValue as CacheableObject;
            if (cacheableObject != null)
            {
                return cacheableObject.Value;
            } 
            throw new ArgumentException(String.Format("The value type [{0}] is not CacheableObject", cacheableValue.GetType()));
        }

        public override void Remove(object key)
        {
            string coKey = GetCachableKey(key);
            region.Destroy(coKey);
        }

        public override ICollection Keys
        {
            get
            {
                ICacheableKey[] keyArray = region.GetKeys();
                ArrayList arrayList = new ArrayList();
                foreach (var key in keyArray)
                {
                    if (key is CacheableString)
                    {
                        CacheableString cs = (CacheableString) key;
                        arrayList.Add(cs.Value);
                    }
                }
                return arrayList;
            }
        }

        protected override void DoInsert(object key, object value, TimeSpan timeToLive)
        {
            
            /*
            if (key.GetType().IsSerializable == false)
            {
                throw new ArgumentException(String.Format("The key type [{0}] is not Serializable", key.GetType()));
            }*/
            string coKey = GetCachableKey(key);
            if (value.GetType().IsSerializable == false)
            {
                throw new ArgumentException(String.Format("The value type [{0}] is not Serializable", value.GetType()));                
            }
            
            CacheableObject coValue = CacheableObject.Create(value);
            region.Put(coKey, coValue);
            
        }

        private string GetCachableKey(object key)
        {
            String coKey = null;
            if (key is String)
            {
                coKey = (string) key;
            } else
            {
                throw new ArgumentException(String.Format("The key type [{0}] is not of type String", key.GetType()));
            }
            return coKey;
        }

        #endregion
    }

}