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

using System;
using GemStone.GemFire.Cache;
using Spring.Objects.Factory;

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Create a Gemstone.Gemfire.Cache.RegionsAttribute using standard .NET setter properties.
    /// </summary>
    /// <author>Mark Pollack</author>
    public class RegionAttributesFactoryObject : IFactoryObject, IInitializingObject
    {

        //TODO add support for the BDB persistence manager

        //This class is sealed and also doesn't follow standard .NET property setter style.
        private readonly AttributesFactory attributesFactory = new AttributesFactory();
        private RegionAttributes gemfireRegionAttributes;

        // Expiration
        private uint regionTimeToLive;
        private ExpirationAction regionTimeToLiveAction = ExpirationAction.Invalidate;

        private uint regionIdleTimeout;
        private ExpirationAction regionIdleTimeoutAction = ExpirationAction.Invalidate;

        private uint entryTimeToLive;
        private ExpirationAction entryTimeToLiveAction = ExpirationAction.Invalidate;

        private uint entryIdleTimeout;
        private ExpirationAction entryIdleTimeoutAction = ExpirationAction.Invalidate;

        // storage;
        private int? initialCapacity;
        private float? loadFactor;
        private int? concurrencyLevel;
        private DiskPolicyType diskPolicy = DiskPolicyType.None;

        // Distribution
        // For client caches the ScopeType should only be used when you want a local scope.
        // TODO check on using StypeType - related to use of CachingEnabled property in RegionAttributes.        
        private bool? localScope;


        //Misc settings
        private bool? cachingEnabled;
        
        private bool? clientNotificationEnabled;

        private uint? lruEntriesLimit;

        private bool? cloningEnabled;

        /// <summary>
        /// Sets the caching enabled flag for this region.  
        /// </summary>
        /// <value>The caching is enabled.</value>
        /// <remarks>If true, cache data for this region in this process. 
        /// If set to false, then no data is stored in the local process, 
        /// but events and distributions will still occur, and the region can still be used to put and remove, etc... 
        /// The default if not set is 'true', 'false' is illegal for regions of ScopeType.Local scope. <see cref="LocalScope"/>
        /// </remarks>
        public bool? CachingEnabled
        {
            set { cachingEnabled = value; }
        }

        /// <summary>
        /// Sets the client notification to be enabled/disabled.
        /// </summary>
        /// <value>The client notification enabled.</value>
        /// <remarks>
        ///Ttrue if client notifications have to be enabled; false otherwise 
        /// </remarks>
        public bool? ClientNotification
        {
            set { clientNotificationEnabled = value; }
        }



        /// <summary>
        /// Sets a limit on the number of entries that will be held in the cache. If a new entry is added while at the limit, the cache will evict the least recently used entry. 
        /// </summary>
        /// <value>The limit of the number of entries before eviction starts. Defaults to 0, meaning no LRU actions will used.</value>
        public uint? LruEntriesLimit
        {
            set { lruEntriesLimit = value; }
        }

        /// <summary>
        /// Gets or sets the cloning enabled.
        /// </summary>
        /// <value>The cloning enabled.</value>
        public bool? CloningEnabled
        {
            set { cloningEnabled = value; }
        }

        #region Distribution

        /// <summary>
        /// Sets a value indicating whether the region should be local scope, creating a private
        /// data set in the memory area where this region residers, invisible to other client caches in the system.
        /// </summary>
        /// <value><c>true</c> if create a locally coped region; otherwise, <c>false</c>.</value>
        public bool LocalScope
        {
            set { localScope = value; }
        }

        #endregion


        #region Storage

        /// <summary>
        /// Sets the disk policy.  Only used when a new region is created.
        /// </summary>
        /// <value>The region disk policy.</value>
        public DiskPolicyType DiskPolicy
        {
            set { diskPolicy = value; }
        }

        /// <summary>
        /// Sets the initial capacity of the map used for storing the entries.  Default is 16
        /// </summary>
        /// <value>The initial capacity.</value>
        public int InitialCapacity
        {
            set { initialCapacity = value; }
        }

        /// <summary>
        /// Sets the load factor of the map used for storing the entries.  Default 0.75
        /// </summary>
        /// <value>The load factor.</value>
        public float LoadFactor
        {
            set { loadFactor = value; }
        }

        /// <summary>
        /// Sets the allowed concurrency among updates to values in the region is guided by the concurrencyLevel, 
        /// which is used as a hint for internal sizing.  Default 16.
        /// </summary>
        /// <value>The concurrency level.</value>
        public int ConcurrencyLevel
        {
            set { concurrencyLevel = value; }
        }

        #endregion

        #region Expiration

        /// <summary>
        /// Sets the region time to live in seconds for the region as a whole
        /// </summary>
        /// <value>The region time to live.</value>
        public uint RegionTimeToLive
        {
            set { regionTimeToLive = value; }
        }

        /// <summary>
        /// Sets the region time to live expiration action.
        /// </summary>
        /// <value>The region time to live action.</value>
        public ExpirationAction RegionTimeToLiveAction
        {
            set { regionTimeToLiveAction = value; }
        }

        /// <summary>
        /// Sets the region idle timeout in seconds
        /// </summary>
        /// <value>The region idle timeout.</value>
        public uint RegionIdleTimeout
        {
            set { regionIdleTimeout = value; }
        }

        /// <summary>
        /// Sets the region idle timeout expiration action.
        /// </summary>
        /// <value>The region idle timeout action.</value>
        public ExpirationAction RegionIdleTimeoutAction
        {
            set { regionIdleTimeoutAction = value; }
        }

        /// <summary>
        /// Sets the entry time to live in seconds
        /// </summary>
        /// <value>The entry time to live.</value>
        public uint EntryTimeToLive
        {
            set { entryTimeToLive = value; }
        }

        /// <summary>
        /// Sets the entry time to live expiration action.
        /// </summary>
        /// <value>The entry time to live action.</value>
        public ExpirationAction EntryTimeToLiveAction
        {
            set { entryTimeToLiveAction = value; }
        }

        /// <summary>
        /// Sets the entry idle timeout in seconds
        /// </summary>
        /// <value>The entry idle timeout.</value>
        public uint EntryIdleTimeout
        {
            set { entryIdleTimeout = value; }
        }

        /// <summary>
        /// Sets the entry idle timeout expiration action.
        /// </summary>
        /// <value>The entry idle timeout action.</value>
        public ExpirationAction EntryIdleTimeoutAction
        {
            set { entryIdleTimeoutAction = value; }
        }

        #endregion

        public object GetObject()
        {
            return gemfireRegionAttributes;
        }

        public Type ObjectType
        {
            get { return typeof (RegionAttributes); }
        }

        public bool IsSingleton
        {
            get { return true; }
        }

        public void AfterPropertiesSet()
        {
            if (cachingEnabled != null) attributesFactory.SetCachingEnabled(cachingEnabled.Value);
            if (clientNotificationEnabled != null)
                attributesFactory.SetClientNotificationEnabled(clientNotificationEnabled.Value);
            
            if (lruEntriesLimit != null) attributesFactory.SetLruEntriesLimit(lruEntriesLimit.Value);

            if (cloningEnabled != null) attributesFactory.SetCloningEnabled(cloningEnabled.Value);

            SetStorageProperties(attributesFactory);
            SetExpirationProperties(attributesFactory);
            
            gemfireRegionAttributes = attributesFactory.CreateRegionAttributes();
        }

        protected virtual void SetStorageProperties(AttributesFactory attrFactory)
        {
            attrFactory.SetDiskPolicy(diskPolicy);
            if (initialCapacity != null)
            {
                attrFactory.SetInitialCapacity(initialCapacity.Value);
            }
            if (loadFactor != null)
            {
                attrFactory.SetLoadFactor(loadFactor.Value);
            }
            if (concurrencyLevel != null)
            {
                attrFactory.SetConcurrencyLevel(concurrencyLevel.Value);
            }
        }
        protected virtual void SetExpirationProperties(AttributesFactory attrFactory)
        {
            attrFactory.SetRegionTimeToLive(regionTimeToLiveAction, regionTimeToLive);
            attrFactory.SetRegionIdleTimeout(regionIdleTimeoutAction, regionIdleTimeout);
            attrFactory.SetEntryTimeToLive(entryTimeToLiveAction, entryTimeToLive);
            attrFactory.SetEntryIdleTimeout(entryIdleTimeoutAction, entryIdleTimeout);
        }
    }

}