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
using Spring.Objects.Factory;
using Spring.Util;

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
    
        public RegexInterest()
        {
        }

        public RegexInterest(string regex)
            : this(regex, InterestResultPolicy.KeysAndValues, false)
        {
            
        }

        public RegexInterest(string regex, bool durable)
            : this(regex, InterestResultPolicy.KeysAndValues, durable)
        {
        }

        public RegexInterest(string regex, InterestResultPolicy policy)
            : this(regex, policy, false)
        {
            
        }

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
            set { this.regex = value;}
        }

        public override string ToString()
        {
            return string.Format("Regex: {0}, Policy: {1}, Durable: {2}", regex, policy, durable);
        }

        public void AfterPropertiesSet()
        {
            AssertUtils.ArgumentHasText(regex, "A non-empty regex is required");
        }

    }

}