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

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Basic holder interface for registering an interest in cached data. 
    /// </summary>
    /// <author>Mark Pollack</author>
    public interface IInterest
    {
        /// <summary>
        /// Gets or sets the policy.
        /// </summary>
        /// <value>The policy.</value>
        InterestResultPolicy Policy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AllKeysInterest"/> is durable.
        /// </summary>
        /// <value><c>true</c> if durable; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The client can register any of its interest lists and continuous queries as durable. 
        /// Durable interest remains even if the client disconnects for a period of time. 
        /// During the client’s down time, the server maintains its durable subscriptions and then, 
        /// when the client reconnects, plays them back to the client.
        /// </remarks>
        bool Durable { get; set; }
    }
}