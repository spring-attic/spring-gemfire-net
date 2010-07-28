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
using System.Collections.Specialized;
using System.Threading;
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Dao;
using Spring.Dao.Support;
using Spring.Objects.Factory;
using Spring.Util;
using Properties=GemStone.GemFire.Cache.Properties;

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Factory used for configuring a Gemfire Cache manager. Allows either retrieval of an existing, opened cache 
    /// or the creation of a new one.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <author>Mark Pollack</author>
    public class CacheFactoryObject : IObjectNameAware, IObjectFactoryAware, IDisposable, IInitializingObject, IFactoryObject, IPersistenceExceptionTranslator
    {

        protected static readonly ILog log = LogManager.GetLogger(typeof(CacheFactoryObject));

        private static readonly string DEFAULT_DISTRIBUTED_SYSTEM_NAME = "DistributedSystemDotNet";

        private static readonly string DEFAULT_CACHE_NAME = "";

        private Cache cache;
        private string distributedSystemName = DEFAULT_DISTRIBUTED_SYSTEM_NAME;

        private string name = DEFAULT_CACHE_NAME;

        private DistributedSystem system;
        private NameValueCollection properties;
        private string cacheXml;

        private string objectName;
        private IObjectFactory objectFactory;


        public string DistributedSystemName
        {          
            set { distributedSystemName = value; }
        }

        public NameValueCollection Properties
        {
            set { properties = value; }
        }

        public string CacheXml
        {
            set { cacheXml = value; }
        }

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
            } catch (Exception)
            {
                if (cacheXml == null)
                {
                    cache = CacheFactory.Create(name, system);
                } else
                {
                    //TODO call Create method that takes CacheAttributes
                    cache = CacheFactory.Create(name, system, cacheXml);                    
                    log.Debug("Initialized cache from " + cacheXml);
                }
                //TODO call Create method that takes CacheAttributes
                msg = "Created";
            }
            log.Info(msg + " GemFire Cache ['" + cache.Name + "'] v. " + CacheFactory.Version);
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

        public void Dispose()
        {
            
            if (cache != null && !cache.IsClosed)
            {
                cache.Close();                
            }
            cache = null;
            /* TODO not working on my machine, call hangs
            
            if (system != null && DistributedSystem.IsConnected)
            {               
                DistributedSystem.Disconnect();
            }
            system = null;*/
        }

        public DataAccessException TranslateExceptionIfPossible(Exception ex)
        {
            if (ex is GemFireException)
            {
                return GemFireCacheUtils.ConvertGemFireAccessException((GemFireException) ex);
            }
            log.Info("Could not translate exception of type " +  ex.GetType());
            return null;
        }

        public object GetObject()
        {
            return cache;                    
        }

        public Type ObjectType
        {
            get { return typeof (Cache); }
        }

        public bool IsSingleton
        {
            get { return true; }
        }

        public string ObjectName
        {
            set { this.objectName = value; }
        }

        public IObjectFactory ObjectFactory
        {
            set { this.objectFactory = value; }
        }
    }
}