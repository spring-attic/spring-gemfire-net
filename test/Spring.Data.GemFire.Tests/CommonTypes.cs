#region License

/*
 * Copyright 2002-2004 the original author or authors.
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

using System;
using System.Collections;
using System.Collections.Specialized;
using Spring.Caching;
using Spring.Stereotype;

namespace Spring.Data.GemFire.Tests
{
    [Serializable]
	public class Inventor
    {
        public string Name;
        public string Nationality;
        public string[] Inventions;
#if NET_2_0
        public DateTime? DateOfGraduation;
#endif
        private DateTime dob;
        private Place pob;

        public Inventor() : this(null, DateTime.MinValue, null)
        {}

        public Inventor(string name, DateTime dateOfBirth, string nationality)
        {
            this.Name = name;
            this.dob = dateOfBirth;
            this.Nationality = nationality;
            this.pob = new Place();
        }

        public DateTime DOB
        {
            get { return dob; }
            set { dob = value; }
        }

        /// <summary>
        /// R/W PlaceOfBirth property
        /// </summary>
        public Place POB
        {
            get { return pob; }
            set { pob = value; }
        }

        /// <summary>
        /// Readonly
        /// </summary>
        public Place PlaceOfBirth
        {
            get { return pob; }
        }

        public int GetAge(DateTime on)
        {
            // not very accurate, but it will do the job ;-)
            return on.Year - dob.Year;
        }
    }

    [Serializable]
    public class Place
    {
        public string City;
        public string Country;
    }

    [Serializable]
    public class Society
    {
        public string Name = "League of Extraordinary Gentlemen";
        public static string Advisors = "advisors";
        public static string President = "president";

        private IList members = new ArrayList();
        private IDictionary officers = new Hashtable();

        public IList Members
        {
            get { return members; }
        }

        public IDictionary Officers
        {
            get { return officers; }
        }

        public bool IsMember(string name)
        {
            bool found = false;
            foreach (Inventor inventor in members)
            {
                if (inventor.Name == name)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }

    public interface IInventorRepository
    {
        IList GetAll();
        IList GetAllNoCacheKey();
        Inventor Load(string name);
        void Save(Inventor inventor);
        void Delete(Inventor inventor);
        void DeleteAll();
    }

    //NOTE Serializable added for Gemfire
    [Serializable]
    [Repository]
    public sealed class InventorRepository : IInventorRepository
    {
        private IDictionary inventors = new ListDictionary();

        public InventorRepository()
        {
            Inventor tesla = new Inventor("Nikola Tesla", new DateTime(1856, 7, 9), null);
            Inventor pupin = new Inventor("Mihajlo Pupin", new DateTime(1854, 10, 9), null);
            inventors.Add("Nikola Tesla", tesla);
            inventors.Add("Mihajlo Pupin", pupin);
        }

        [CacheResultItems("inventors", "Name")]
        public IList GetAll()
        {
            return new ArrayList(inventors.Values);
        }


        [CacheResult(CacheName = "inventors")]
        public IList GetAllNoCacheKey()
        {
            return new ArrayList(inventors.Values);
        }

        [CacheResult("inventors", "#name")]
        public Inventor Load(string name)
        {
            return (Inventor)inventors[name];
        }

        public void Save([CacheParameter("inventors", "Name")] Inventor inventor)
        {
            inventor.Nationality = "Serbian";
        }

        [InvalidateCache("inventors", Keys = "#inventor.Name")]
        public void Delete(Inventor inventor)
        {
        }

        [InvalidateCache("inventors")]
        public void DeleteAll()
        {
        }
    }


}