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

#region

using System;
using System.Collections.Generic;
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Objects.Factory;
using Spring.Util;

#endregion

namespace Spring.Data.GemFire
{
    /// <summary>
    /// FactoryBean for creating generic Gemfire {@link Region}s. Will try to first locate the region (by name)
    /// and, in case none if found, proceed to creating one using the given settings. 
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class RegionFactoryObject : IDisposable, IFactoryObject, IInitializingObject, IObjectNameAware
    {
        #region Fields

        protected static readonly ILog log = LogManager.GetLogger(typeof (RegionFactoryObject));

        private string objectName;
        private Cache cache;
        private string name;
        private bool destroy;

        // Callbacks
        private ICacheListener cacheListener;
        private ICacheLoader cacheLoader;
        private ICacheWriter cacheWriter;

        //expiration
        private uint regionTimeToLive;
        private ExpirationAction regionTimeToLiveAction = ExpirationAction.Invalidate;

        private uint regionIdleTimeout;
        private ExpirationAction regionIdleTimeoutAction = ExpirationAction.Invalidate;

        private uint entryTimeToLive;
        private ExpirationAction entryTimeToLiveAction = ExpirationAction.Invalidate;

        private uint entryIdleTimeout;
        private ExpirationAction entryIdleTimeoutAction = ExpirationAction.Invalidate;


        // Distribution
        // For client caches the ScopeType should only be used when you want a local scope.
        private bool localScope = false;        

        // storage;
        private int? initialCapacity;
        private float? loadFactor;
        private int? concurrencyLevel;
        private DiskPolicyType diskPolicy = DiskPolicyType.None;


        //Misc settings
        private bool? cachingEnabled = null;
        private bool? clientNotificationEnabled = null;
        private string endpoints;
        private uint? lruEntriesLimit = null;
        private string poolName;

        //Interests
        private IInterest[] interests;

        private RegionAttributes attributes;
        private Region region;


        #endregion

        public void AfterPropertiesSet()
        {
            AssertUtils.ArgumentNotNull(cache, "Cache Property must be set");
            name = (!StringUtils.HasText(name) ? objectName : name);
            AssertUtils.ArgumentHasText(name, "Name (or ObjectName) property must be set");

            //first get cache
            region = cache.GetRegion(name);
            if (region != null)
            {
                log.Info("Retrieved region [" + name + "] from cache");
            }
                // fall back to cache creation if one is not found
            else
            {
                AttributesFactory attributesFactory = (attributes != null
                                                           ? new AttributesFactory(attributes)
                                                           : new AttributesFactory());
                
                if (cachingEnabled != null) attributesFactory.SetCachingEnabled(cachingEnabled.Value);
                if (clientNotificationEnabled != null) attributesFactory.SetClientNotificationEnabled(clientNotificationEnabled.Value);
                if (endpoints != null) attributesFactory.SetEndpoints(endpoints);
                if (lruEntriesLimit != null) attributesFactory.SetLruEntriesLimit(lruEntriesLimit.Value);
                if (poolName != null) attributesFactory.SetPoolName(poolName);
                
                SetCallbacks(attributesFactory);
                SetDistributionProperties(attributesFactory);
                SetStorageProperties(attributesFactory);
                SetExpirationProperties(attributesFactory);
                PostProcessAttributes(attributesFactory);
                region = cache.CreateRegion(name, attributesFactory.CreateRegionAttributes());                
                log.Info("Created new cache region [" + name + "]");
                RegisterInterests();
            }            
            PostProcess(region);
        }

        protected virtual void RegisterInterests()
        {
            foreach (AllKeysInterest interest in interests)
            {
                if (interest is RegexInterest)
                {
                    RegexInterest regexInterest = (RegexInterest) interest;
                    if (interest.Policy == InterestResultPolicy.None)
                    {
                        region.RegisterRegex(regexInterest.Regex, interest.Durable);
                    }
                    else if (interest.Policy == InterestResultPolicy.Keys)
                    {
                        region.RegisterRegex(regexInterest.Regex, interest.Durable, new List<ICacheableKey>());
                    } 
                    else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                    {
                        //TODO should the list of keys be made accessible to client code, post an application context event?
                        region.RegisterRegex(regexInterest.Regex, interest.Durable, new List<ICacheableKey>(), true);
                    }                    
                } else if (interest is KeyInterest)
                {
                    KeyInterest keyInterest = (KeyInterest) interest;
                    if (keyInterest.Policy == InterestResultPolicy.None)
                    {
                        region.RegisterKeys(keyInterest.Keys);
                    } else if (interest.Policy == InterestResultPolicy.Keys)
                    {
                        region.RegisterKeys(keyInterest.Keys, keyInterest.Durable, false);
                    } else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                    {
                        region.RegisterKeys(keyInterest.Keys, keyInterest.Durable, true);
                    }
                } else
                {
                    if  (interest.Policy == InterestResultPolicy.None)
                    {
                        region.RegisterAllKeys(interest.Durable);

                    } 
                    else if (interest.Policy == InterestResultPolicy.Keys)
                    {
                        region.RegisterAllKeys(interest.Durable, new List<ICacheableKey>(), false);
                    }
                    else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                    {
                        try
                        {
                            region.RegisterAllKeys(interest.Durable, new List<ICacheableKey>(), true);
                        }
                        catch (Exception ex)
                        {
                            log.Warn("could not register interest in keys", ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indented for subclasses to override and provide additional configuration of the AttributesFactory,
        /// for example setting persistence manager option is not currenlty exposed as a public property on the
        /// RegionFactoryObject.
        /// </summary>
        /// <param name="attributesFactory">The attributes factory.</param>
        protected virtual void PostProcessAttributes(AttributesFactory attributesFactory)
        {
            // no-op
        }

        protected virtual void SetExpirationProperties(AttributesFactory attributesFactory)
        {
            attributesFactory.SetRegionTimeToLive(regionTimeToLiveAction, regionTimeToLive);
            attributesFactory.SetRegionIdleTimeout(regionIdleTimeoutAction, regionIdleTimeout);
            attributesFactory.SetEntryTimeToLive(entryTimeToLiveAction, entryTimeToLive);
            attributesFactory.SetEntryIdleTimeout(entryIdleTimeoutAction, entryIdleTimeout);
        }

        protected virtual void SetDistributionProperties(AttributesFactory attributesFactory)
        {
        }

        protected virtual void SetCallbacks(AttributesFactory attributesFactory)
        {
            if (cacheListener != null)
            {
                attributesFactory.SetCacheListener(cacheListener);
            }
            if (cacheLoader != null)
            {
                attributesFactory.SetCacheLoader(cacheLoader);
            }
            if (cacheWriter != null)
            {
                attributesFactory.SetCacheWriter(cacheWriter);
            }
        }

        protected virtual void SetStorageProperties(AttributesFactory attributesFactory)
        {
            attributesFactory.SetDiskPolicy(diskPolicy);
            if (initialCapacity != null)
            {
                attributesFactory.SetInitialCapacity(initialCapacity.Value);
            }
            if (loadFactor != null)
            {
                attributesFactory.SetLoadFactor(loadFactor.Value);
            }
            if (concurrencyLevel != null)
            {
                attributesFactory.SetConcurrencyLevel(concurrencyLevel.Value);
            }
        }

        /// <summary>
        /// Subclasses can override this method to further customize the Region configuration.
        /// </summary>
        /// <remarks>
        /// Post-process the region object for this factory bean during the initialization process.
        /// The object is already initialized and configured by the factory object before this method
        /// is invoked.
        /// </remarks>
        /// <param name="region">The region.</param>
        protected virtual void PostProcess(Region region)
        {
            // do nothing
        }

        public void Dispose()
        {
            try
            {
                if (region != null)
                {
                    foreach (AllKeysInterest interest in interests)
                    {
                        if (interest is RegexInterest)
                        {
                            RegexInterest regexInterest = (RegexInterest) interest;
                            region.UnregisterRegex(regexInterest.Regex);                            
                        }
                        else if (interest is KeyInterest)
                        {
                            KeyInterest keyInterest = (KeyInterest) interest;
                            region.UnregisterKeys(keyInterest.Keys);
                            
                        } else
                        {
                            region.UnregisterAllKeys();
                        }
                    }
                }
            } catch (Exception ex)
            {
                log.Warn("Cannot unregister cache interests", ex);
            }

            if (region != null && destroy)
            {
                region.DestroyRegion();
            }
            region = null;
        }

        public object GetObject()
        {
            return region;
        }

        public Type ObjectType
        {
            get
            {
                //return (region != null ? region.GetType() : typeof (Region));
                return typeof (Region);
            }
        }

        public bool IsSingleton
        {
            get { return true; }
        }

        public string ObjectName
        {
            set { objectName = value; }
        }

        /// <summary>
        /// Sets the cache used for creating the region
        /// </summary>
        /// <value>The cache to set</value>
        public Cache Cache
        {
            set { cache = value; }
        }

        /// <summary>
        /// Sets the name of the cache region. If no cache is found under
        /// the given name, a new one will be created.
        /// If no name is given, the objectName will be used.
        /// </summary>
        /// <value>The region name.</value>
        public string Name
        {
            set { name = value; }
        }

        /// <summary>
        /// Sets a value indicating whether the region created by this factory object will be
        /// destroyed on shutdown (default false)
        /// </summary>
        /// <value><c>true</c> if destroy region on shutdown; otherwise, <c>false</c>.</value>
        public bool Destroy
        {
            set { destroy = value; }
        }

        /// <summary>
        /// Sets the region attributes used for the region by this factory.  Allows maximum control in 
        /// specifying the region settings.  
        /// </summary>
        /// <value>The attributes.</value>
        /// <remarks>Used only when a new region is created.</remarks>
        public RegionAttributes Attributes
        {
            set { attributes = value; }
        }

        public IInterest[] Interests
        {
            get { return interests; }
            set { interests = value; }
        }

        #region Callbacks

        /// <summary>
        /// Gets or sets the cache listener.
        /// </summary>
        /// <value>The cache listener.</value>
        /// <remarks>
        /// Sets the cache listeners used for the region used by this factory.
        /// Used only when a new region is created.Overrides the settings
        /// specified through <see cref="Attributes"/>
        /// </remarks>
        public ICacheListener CacheListener
        {
            set { cacheListener = value; }
        }


        /// <summary>
        /// Sets the cache loader.
        /// </summary>
        /// <value>The cache loader to set on a newly created region</value>
        /// <remarks>
        /// Sets the cache loader used for the region used by this factory.
        /// Used only when a new region is created.  Overrides the settings
        /// specified through <see cref="Attributes"/>
        /// </remarks>
        public ICacheLoader CacheLoader
        {
            set { cacheLoader = value; }
        }

        /// <summary>
        /// Gets or sets the cache writer.
        /// </summary>
        /// <value>The cache writer to set on a newly created region</value>
        /// <remarks>
        /// Sets the cache writer used for the region used by this factory.
        /// Used only when a new region is created. Overrides the settings
        /// specified through <see cref="Attributes"/>
        /// </remarks>
        public ICacheWriter CacheWriter
        {
            set { cacheWriter = value; }
        }

        #endregion

        #region Distribution

        /// <summary>
        /// Sets a value indicating whether the region should be local scope, creating a private
        /// data set in the memory area where this region residers, invisible to other client caches in the system.
        /// </summary>
        /// <value><c>true</c> if creat a locally coped region; otherwise, <c>false</c>.</value>
        public bool LocalScope
        {
            set { localScope = value; }
        }

        #endregion

        #region Storage

        /// <summary>
        /// Sets the disk policy.  Only used when a new region is created.
        /// </summary>
        /// <remarks>Overrides the settings specified through <see cref="Attributes"/></remarks>
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
        /// Sets the endpoints for a client region. It is a comma separated list of host:port pairs.
        /// </summary>
        /// <value>The endpoints.</value>
        /// <remarks>If the endpoints are set then the region is taken to be a Thin-client region that interacts with the GemFire Java cacheserver. </remarks>
        public string Endpoints
        {
            set { endpoints = value; }
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
        /// Sets the name of the pool for a client region
        /// </summary>
        /// <value>The name of the pool to attach to this region.</value>
        /// <remarks>The pool with the name specified must be already created.</remarks>
        public string PoolName
        {
            set { poolName = value; }
        }
    }
}