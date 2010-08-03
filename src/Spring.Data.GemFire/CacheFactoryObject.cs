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
using System.Collections.Specialized;
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Dao;
using Spring.Dao.Support;
using Spring.Objects.Factory;
using Spring.Util;
using Properties=GemStone.GemFire.Cache.Properties;

#endregion

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Factory used for configuring a Gemfire Cache manager. Allows either retrieval of an existing, opened cache 
    /// or the creation of a new one.
    /// </summary>
    /// <author>Mark Pollack</author>
    public class CacheFactoryObject : IDisposable, IInitializingObject, IObjectNameAware,
                                      IFactoryObject, IPersistenceExceptionTranslator
    {
        #region Fields

        protected static readonly ILog log = LogManager.GetLogger(typeof (CacheFactoryObject));

        private static readonly string DEFAULT_DISTRIBUTED_SYSTEM_NAME = "DistributedSystemDotNet";

        private static readonly string DEFAULT_CACHE_NAME = "";

        private Cache cache;
        private string distributedSystemName = DEFAULT_DISTRIBUTED_SYSTEM_NAME;

        private string name = DEFAULT_CACHE_NAME;

        private bool disconnectOnClose = true;

        private DistributedSystem system;
        private NameValueCollection properties;
        private string cacheXml;

        #endregion

        #region Properties

        /// <summary>
        /// Sets the name of the distributed system.
        /// </summary>
        /// <value>The name of the distributed system.</value>
        public string DistributedSystemName
        {
            set { distributedSystemName = value; }
        }

        /// <summary>
        /// Sets the properties used to configure the Gemfire Cache. These are copied into 
        /// a GemStone.GemFire.Cache.Properties object and used to initialize the DistributedSystem.
        /// </summary>
        /// <value>The properties used to configure the cache..</value>
        public NameValueCollection Properties
        {
            set { properties = value; }
        }

        /// <summary>
        /// Sets the name of the cache XML that can be used to configure the cache.
        /// </summary>
        /// <value>The cache XML.</value>
        public string CacheXml
        {
            set { cacheXml = value; }
        }

        /// <summary>
        /// Sets a value indicating whether to call DistributedSystem.Disconnect when this object is 
        /// disposed.  There is a but in the 3.0.0.9 client that may hang calls to close.  The default is
        /// true, set to false if you experience a hang in the application.
        /// </summary>
        /// <value><c>true</c> to call DistributedSystem.Disconnect when this object is dispose; otherwise, <c>false</c>.</value>
        public bool DisconnectOnClose
        {
            set { disconnectOnClose = value; }
        }

        #endregion

        /// <summary>
        /// Initialization callback called by Spring.  Responsible for connecting to the distributed system
        /// and creating the cache.
        /// </summary>
        public void AfterPropertiesSet()
        {
            AssertUtils.ArgumentNotNull("name", name, "Cache name can not be null");
            Properties gemfirePropertes = MergePropertes();
            system = DistributedSystem.Connect(distributedSystemName, gemfirePropertes);

            log.Info("Connected to Distributed System [" + system.Name + "]");

            // first look for open caches
            String msg = null;
            try
            {
                cache = CacheFactory.GetInstance(system);
                msg = "Retrieved existing";
            }
            catch (Exception)
            {
                if (cacheXml == null)
                {
                    cache = CacheFactory.Create(name, system);
                }
                else
                {
                    //TODO call Create method that takes CacheAttributes
                    cache = CacheFactory.Create(name, system, cacheXml);
                    log.Debug("Initialized cache from " + cacheXml);
                }
                //TODO call Create method that takes CacheAttributes
                msg = "Created";
            }
            
            log.Info(msg + " GemFire v." + CacheFactory.Version + " Cache ['" + cache.Name + "']");
        }

        private Properties MergePropertes()
        {
            GemStone.GemFire.Cache.Properties gemfirePropertes = GemStone.GemFire.Cache.Properties.Create();
            if (properties != null)
            {
                foreach (string key in properties.Keys)
                {
                    gemfirePropertes.Insert(key, properties[key]);
                    if (key.Equals("name"))
                    {
                        this.name = properties[key];
                    }
                }
            }
            if (StringUtils.HasText(name))
            {
                gemfirePropertes.Insert("name", name.Trim());
            }
            return gemfirePropertes;
        }

        /// <summary>
        /// Closes the cache and disconnects from the distributed system.
        /// </summary>
        public void Dispose()
        {
            if (cache != null && !cache.IsClosed)
            {
                cache.Close();
            }
            cache = null;

            if (disconnectOnClose)
            {
                if (system != null && DistributedSystem.IsConnected)
                {
                    DistributedSystem.Disconnect();
                }
                system = null;
            }
        }

        /// <summary>
        /// Translate the given exception thrown by a persistence framework to a
        /// corresponding exception from Spring's generic DataAccessException hierarchy,
        /// if possible.
        /// </summary>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>
        /// the corresponding DataAccessException (or <code>null</code> if the
        /// exception could not be translated, as in this case it may result from
        /// user code rather than an actual persistence problem)
        /// </returns>
        /// <remarks>
        /// 	<para>
        /// Do not translate exceptions that are not understand by this translator:
        /// for example, if coming from another persistence framework, or resulting
        /// from user code and unrelated to persistence.
        /// </para>
        /// 	<para>
        /// Of particular importance is the correct translation to <see cref="T:Spring.Dao.DataIntegrityViolationException"/>
        /// for example on constraint violation.  Implementations may use Spring ADO.NET Framework's
        /// sophisticated exception translation to provide further information in the event of SQLException as a root cause.
        /// </para>
        /// </remarks>
        /// <seealso cref="T:Spring.Dao.DataIntegrityViolationException"/>
        /// <seealso cref="T:Spring.Data.Support.ErrorCodeExceptionTranslator"/>
        /// <author>Rod Johnson</author>
        /// <author>Juergen Hoeller</author>
        /// <author>Mark Pollack (.NET)</author>
        public DataAccessException TranslateExceptionIfPossible(Exception ex)
        {
            if (ex is GemFireException)
            {
                return GemFireCacheUtils.ConvertGemFireAccessException((GemFireException) ex);
            }
            log.Info("Could not translate exception of type " + ex.GetType());
            return null;
        }

        /// <summary>
        /// Returns the cache object
        /// </summary>
        /// <returns>The cache object</returns>
        public object GetObject()
        {
            return cache;
        }

        /// <summary>
        /// Returns Gemstone.GemFire.Cache.Cache 
        /// </summary>
        public Type ObjectType
        {
            get { return typeof (Cache); }
        }

        /// <summary>
        /// Returns true
        /// </summary>
        public bool IsSingleton
        {
            get { return true; }
        }

        /// <summary>
        /// Sets the name of the cache to the name of the Spring object definition.  Can be overrided by 
        /// specifying 'name' as the key in the Properties collection.
        /// </summary>
        public string ObjectName
        {
            set {
                this.name = value;
            }
        }
    }
}