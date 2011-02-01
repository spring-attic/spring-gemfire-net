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
    /// FactoryObject for creating generic Gemfire Region. Will try to first locate the region (by name)
    /// and, in case none if found, proceed to creating one using the given settings. 
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class ClientRegionFactoryObject : RegionLookupFactoryObject, IDisposable, IObjectFactoryAware
    {
        #region Fields

        private static readonly ILog log = LogManager.GetLogger(typeof(ClientRegionFactoryObject));

        private bool destroy;

        private IObjectFactory objectFactory;

        // Callbacks
        private ICacheListener[] cacheListener;
        private ICacheLoader cacheLoader;
        private ICacheWriter cacheWriter;

        // Data Policy in .NET

        //Interests
        // TODO where did these go in Spring Gemfire Java?
        private IInterest[] interests;

        private RegionAttributes attributes;

        //TODO this seems essential to have been set when using local region , seems like should be set via RegionAttributes
        private string endpoints;

        private string poolName;


        #endregion

        public override void AfterPropertiesSet()
        {
            base.AfterPropertiesSet();
            PostProcess(region);
        }

        protected override Region LookupFallback(Cache cacheObject, string regionName)
        {
            AttributesFactory attributesFactory = (attributes != null
                                                       ? new AttributesFactory(attributes)
                                                       : new AttributesFactory());


            if (endpoints != null) attributesFactory.SetEndpoints(endpoints);
            if (poolName != null) attributesFactory.SetPoolName(poolName);
            SetCallbacks(attributesFactory);
            SetDistributionProperties(attributesFactory);

            PostProcessAttributes(attributesFactory);
                      
            Region reg = cacheObject.CreateRegion(regionName, attributesFactory.CreateRegionAttributes());
            log.Info("Created new cache region [" + regionName + "]");           
            return reg;
        }

        protected virtual void RegisterInterests()
        {
            if (interests == null)
            {
                return;
            }
            foreach (AllKeysInterest interest in interests)
            {
                if (interest is RegexInterest)
                {
                    try
                    {
                        RegexInterest regexInterest = (RegexInterest) interest;
                        if (interest.Policy == InterestResultPolicy.None)
                        {
                            region.RegisterRegex(regexInterest.Regex, interest.Durable);
                        }
                        else if (interest.Policy == InterestResultPolicy.Keys)
                        {
                            region.RegisterRegex(regexInterest.Regex, interest.Durable, null, false);                                                          
                        }
                        else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                        {
                            //TODO How to make the list of keys be made accessible to client code..?
                            //     Have the List<ICacheableKey> come from the container.                            
                            region.RegisterRegex(regexInterest.Regex, interest.Durable, new List<ICacheableKey>());                                                       
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Couldn't register regex interest [" + interest + "]", e);
                        throw;
                    }
                }
                else if (interest is KeyInterest)
                {
                    try
                    {
                        KeyInterest keyInterest = (KeyInterest) interest;
                        if (keyInterest.Policy == InterestResultPolicy.None)
                        {
                            region.RegisterKeys(keyInterest.Keys);
                        }
                        else if (interest.Policy == InterestResultPolicy.Keys)
                        {
                            region.RegisterKeys(keyInterest.Keys, keyInterest.Durable, false);
                        }
                        else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                        {
                            region.RegisterKeys(keyInterest.Keys, keyInterest.Durable, true);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Couldn't register key interest [" + interest + "]", e);
                        throw;
                    }
                }
                else // AllKeysInterest
                {
                    try
                    {
                        if (interest.Policy == InterestResultPolicy.None)
                        {
                            region.RegisterAllKeys(interest.Durable);
                        }
                        else if (interest.Policy == InterestResultPolicy.Keys)
                        {
                            //TODO have the List<ICacheableKey> come from the container
                            region.RegisterAllKeys(interest.Durable, new List<ICacheableKey>(), false);
                        }
                        else if (interest.Policy == InterestResultPolicy.KeysAndValues)
                        {
                            region.RegisterAllKeys(interest.Durable, new List<ICacheableKey>(), true);                                                       
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("Couldn't register all keys interest [" + interest + "]", e);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Indented for subclasses to override and provide additional configuration of the AttributesFactory,
        /// for example setting persistence manager option is not currenlty exposed as a public property on the
        /// ClientRegionFactoryObject.
        /// </summary>
        /// <param name="attrFactory">The attributes factory.</param>
        protected virtual void PostProcessAttributes(AttributesFactory attrFactory)
        {
		    // try to eagerly initialize the pool name, if defined as an object            
		    if (poolName != null && objectFactory.IsTypeMatch(poolName, typeof(GemStone.GemFire.Cache.Pool))) {
			    if (log.IsDebugEnabled) {
				    log.Debug("Found object definition for pool '" + poolName + "'. Eagerly initializing it...");
			    }
			    objectFactory.GetObject(poolName, typeof(GemStone.GemFire.Cache.Pool));
		    }

		    attrFactory.SetPoolName(poolName);
        }



        protected virtual void SetDistributionProperties(AttributesFactory attributesFactory)
        {
        }

        protected virtual void SetCallbacks(AttributesFactory attributesFactory)
        {

            if (cacheListener != null)
            {
                foreach (ICacheListener listener in cacheListener)
                {
                    attributesFactory.SetCacheListener(listener);
                }               
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


        /// <summary>
        /// Subclasses can override this method to further customize the Region configuration.
        /// </summary>
        /// <remarks>
        /// Post-process the region object for this factory object during the initialization process.
        /// The object is already initialized and configured by the factory object before this method
        /// is invoked.
        /// </remarks>
        /// <param name="region">The region.</param>
        protected virtual void PostProcess(Region region)
        {
            RegisterInterests();
        }

        public void Dispose()
        {
            try
            {
                if (region != null && !CollectionUtils.IsEmpty(interests))
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
                        }
                        else
                        {
                            region.UnregisterAllKeys();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Cannot unregister cache interests", ex);
            }

            if (region != null && destroy)
            {
                region.DestroyRegion();
            }
            region = null;
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
        /// Sets the name of the pool for a client region
        /// </summary>
        /// <value>The name of the pool to attach to this region.</value>
        /// <remarks>The pool with the name specified must be already created.</remarks>
        public string PoolName
        {
            set { poolName = value; }
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
        public ICacheListener[] CacheListeners
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

        public IObjectFactory ObjectFactory
        {
            set { this.objectFactory = value; }
        }
    }
}