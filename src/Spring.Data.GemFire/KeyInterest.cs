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

#region

using GemStone.GemFire.Cache;
using Spring.Objects.Factory;
using Spring.Util;

#endregion

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInterest"/> class.
        /// </summary>
        public KeyInterest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInterest"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        public KeyInterest(ICacheableKey[] keys)
            : this(keys, InterestResultPolicy.KeysAndValues, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInterest"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public KeyInterest(ICacheableKey[] keys, bool durable)
            : this(keys, InterestResultPolicy.KeysAndValues, durable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInterest"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="policy">The policy.</param>
        public KeyInterest(ICacheableKey[] keys, InterestResultPolicy policy)
            : this(keys, policy, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyInterest"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public KeyInterest(ICacheableKey[] keys, InterestResultPolicy policy, bool durable)
        {
            this.keys = keys;
            this.policy = policy;
            this.durable = durable;
        }

        #endregion

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public ICacheableKey[] Keys
        {
            get { return keys; }
            set { keys = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
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