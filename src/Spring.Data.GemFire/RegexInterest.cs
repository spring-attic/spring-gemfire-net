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

using Spring.Objects.Factory;
using Spring.Util;

#endregion

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Cache interest based on regular expression rather then individual key types.  
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class RegexInterest : AllKeysInterest, IInitializingObject
    {
        private string regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInterest"/> class.
        /// </summary>
        public RegexInterest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInterest"/> class.
        /// </summary>
        /// <param name="regex">The regex.</param>
        public RegexInterest(string regex)
            : this(regex, InterestResultPolicy.KeysAndValues, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInterest"/> class.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public RegexInterest(string regex, bool durable)
            : this(regex, InterestResultPolicy.KeysAndValues, durable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInterest"/> class.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="policy">The policy.</param>
        public RegexInterest(string regex, InterestResultPolicy policy)
            : this(regex, policy, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexInterest"/> class.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public RegexInterest(string regex, InterestResultPolicy policy, bool durable)
        {
            this.regex = regex;
            this.policy = policy;
            this.durable = durable;
        }

        /// <summary>
        /// Gets the regex backing this interest
        /// </summary>
        /// <value>The regex.</value>
        public string Regex
        {
            get { return regex; }
            set { regex = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Regex: {0}, Policy: {1}, Durable: {2}", regex, policy, durable);
        }

        /// <summary>
        /// Ensures the regex property has been set.
        /// </summary>
        public void AfterPropertiesSet()
        {
            AssertUtils.ArgumentHasText(regex, "A non-empty regex is required");
        }
    }
}