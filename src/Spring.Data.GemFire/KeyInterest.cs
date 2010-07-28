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
using Spring.Objects.Factory;
using Spring.Util;

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Basic holder class for registering a keys based interest in cached data.
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class KeyInterest : AllKeysInterest, IInitializingObject
    {
        private ICacheableKey[] keys;

        public KeyInterest()
        {
        }

        public KeyInterest(ICacheableKey[] keys)
            : this(keys, InterestResultPolicy.KeysAndValues, false)
        {
            
        }

        public KeyInterest(ICacheableKey[] keys, bool durable)
            : this(keys, InterestResultPolicy.KeysAndValues, durable)
        {
        }

        public KeyInterest(ICacheableKey[] keys, InterestResultPolicy policy)
            : this(keys, policy, false)
        {
            
        }

        public KeyInterest(ICacheableKey[] keys, InterestResultPolicy policy, bool durable)
        {
            this.keys = keys;
            this.policy = policy;
            this.durable = durable;
        }

        public ICacheableKey[] Keys
        {
            get { return keys; }
            set { keys = value; }
        }

        public override string ToString()
        {
            return string.Format("Keys: {0}, Policy: {1}, Durable: {2}", keys, policy, durable);
        }

        public virtual void AfterPropertiesSet()
        {
            AssertUtils.ArgumentNotNull(keys, "a non-null keys is required.");
        }
    }

}