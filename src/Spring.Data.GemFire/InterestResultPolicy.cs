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
    /// Enumeration for the policy of what to load on region initialization when registering for interest in cached data
    /// </summary>
    /// <author>Mark Pollack</author>
    public enum InterestResultPolicy
    {

        /// <summary>
        /// The client receives a bulk load of all available keys and values matching the interest registration criteria. This is the default interest result policy. 
        /// </summary>
        KeysAndValues,
        /// <summary>
        /// The client receives a bulk load of all available keys matching the interest registration criteria.
        /// </summary>
        Keys,
        /// <summary>
        /// The client does not receive any immediate bulk loading. 
        /// </summary>
        None

    }
}