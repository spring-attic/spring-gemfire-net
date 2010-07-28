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

using Spring.Objects.Factory;

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Basic holder class for registering an all-key based interest in cached data.
    /// </summary>
    /// <author>Mark Pollack</author>
    public class AllKeysInterest : IInterest
    {
        protected InterestResultPolicy policy = InterestResultPolicy.KeysAndValues;
        protected bool durable = false;

        public AllKeysInterest()
        {
        }

        public AllKeysInterest(bool durable)
        {
            this.durable = durable;
        }

        public AllKeysInterest(InterestResultPolicy policy)
        {
            this.policy = policy;
        }

        public AllKeysInterest(InterestResultPolicy policy, bool durable)
        {
            this.policy = policy;
            this.durable = durable;
        }


        public InterestResultPolicy Policy
        {
            get { return policy; }
            set { policy = value; }
        }

        public bool Durable
        {
            get { return durable; }
            set { durable = value; }
        }


        public override string ToString()
        {
            return string.Format("Policy: {0}, Durable: {1}", policy, durable);
        }

    }

}