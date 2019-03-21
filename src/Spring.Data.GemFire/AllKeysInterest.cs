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

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Basic holder class for registering an all-key based interest in cached data.
    /// </summary>
    /// <author>Mark Pollack</author>
    public class AllKeysInterest : IInterest
    {
        #region Fields

        protected InterestResultPolicy policy = InterestResultPolicy.KeysAndValues;
        protected bool durable;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AllKeysInterest"/> class.
        /// </summary>
        public AllKeysInterest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllKeysInterest"/> class.
        /// </summary>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public AllKeysInterest(bool durable)
        {
            this.durable = durable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllKeysInterest"/> class.
        /// </summary>
        /// <param name="policy">The policy.</param>
        public AllKeysInterest(InterestResultPolicy policy)
        {
            this.policy = policy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllKeysInterest"/> class.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="durable">if set to <c>true</c> register durable interest.</param>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public AllKeysInterest(InterestResultPolicy policy, bool durable)
        {
            this.policy = policy;
            this.durable = durable;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>The policy.</value>
        public InterestResultPolicy Policy
        {
            get { return policy; }
            set { policy = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AllKeysInterest"/> is durable.
        /// </summary>
        /// <value><c>true</c> if durable; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client�s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        public bool Durable
        {
            get { return durable; }
            set { durable = value; }
        }

        #endregion


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Policy: {0}, Durable: {1}", policy, durable);
        }
    }
}