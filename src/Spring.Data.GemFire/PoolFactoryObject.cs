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
using System.Collections;
using Common.Logging;
using Spring.Objects.Factory;
using Spring.Util;

#endregion

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Factory object for easy declaration and configuration of a GemFire pool. 
    /// </summary>
    /// <remarks>
    /// If a new pool is created, its life-cycle is bound to that of the declaring container.
    /// <para>
    /// Note that if the pool already exists, it will be returned as is, without any
    /// modifications and its life cycle untouched by this factory.
    /// </para>
    /// </remarks>
    /// <author>Mark Pollack</author>
    public class PoolFactoryObject : IDisposable, IFactoryObject, IInitializingObject, IObjectNameAware
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof (PoolFactoryObject));

        // whether the pool has been created internaly or not
        private bool internalPool = true;

        private GemStone.GemFire.Cache.Pool pool;


        // pool settings
        private String objectName;
        private String name;
        private IList locators;
        private IList servers;
        private bool keepAlive;


        private int freeConnectionTimeout = PoolFactoryDefaults.DEFAULT_FREE_CONNECTION_TIMEOUT;
        private int idleTimeout = PoolFactoryDefaults.DEFAULT_IDLE_TIMEOUT;
        private int loadConditioningInterval = PoolFactoryDefaults.DEFAULT_LOAD_CONDITIONING_INTERVAL;
        private int maxConnections = PoolFactoryDefaults.DEFAULT_MAX_CONNECTIONS;
        private int minConnections = PoolFactoryDefaults.DEFAULT_MIN_CONNECTIONS;
        private int pingInterval = PoolFactoryDefaults.DEFAULT_PING_INTERVAL;
        private int readTimeout = PoolFactoryDefaults.DEFAULT_READ_TIMEOUT;
        private int retryAttempts = PoolFactoryDefaults.DEFAULT_RETRY_ATTEMPTS;
        private String serverGroup = PoolFactoryDefaults.DEFAULT_SERVER_GROUP;
        private int socketBufferSize = PoolFactoryDefaults.DEFAULT_SOCKET_BUFFER_SIZE;
        private int statisticInterval = PoolFactoryDefaults.DEFAULT_STATISTIC_INTERVAL;
        private int subscriptionAckInterval = PoolFactoryDefaults.DEFAULT_SUBSCRIPTION_ACK_INTERVAL;
        private bool subscriptionEnabled = PoolFactoryDefaults.DEFAULT_SUBSCRIPTION_ENABLED;

        private int subscriptionMessageTrackingTimeout =
            PoolFactoryDefaults.DEFAULT_SUBSCRIPTION_MESSAGE_TRACKING_TIMEOUT;

        private int subscriptionRedundancy = PoolFactoryDefaults.DEFAULT_SUBSCRIPTION_REDUNDANCY;


        public void Dispose()
        {
            if (internalPool && pool != null)
            {
                if (!pool.Destroyed)
                {
                    pool.Destroy(keepAlive);
                    if (log.IsDebugEnabled)
                        log.Debug("Destroyed pool '" + name + "'...");
                }
            }
        }

        public object GetObject()
        {
            return pool;
        }

        public Type ObjectType
        {
            get { return typeof (GemStone.GemFire.Cache.Pool); }
        }

        public bool IsSingleton
        {
            get { return true; }
        }

        public void AfterPropertiesSet()
        {
            if (!StringUtils.HasText(name))
            {
                AssertUtils.ArgumentHasText(objectName, "the pool name is required");
                name = objectName;
            }

            GemStone.GemFire.Cache.Pool existingPool = GemStone.GemFire.Cache.PoolManager.Find(name);
            if (existingPool != null)
            {
                pool = existingPool;
                internalPool = false;
                if (log.IsDebugEnabled)
                    log.Debug("Pool '" + name + " already exists; using found instance...");
            }
            else
            {
                if (log.IsDebugEnabled)
                    log.Debug("No pool named '" + name + "' found. Creating a new once...");

                if (CollectionUtils.IsEmpty(locators) && CollectionUtils.IsEmpty(servers))
                {
                    throw new ArgumentException("at least one locator or server is required");
                }

                internalPool = true;

                GemStone.GemFire.Cache.PoolFactory poolFactory = GemStone.GemFire.Cache.PoolManager.CreateFactory();
                if (!CollectionUtils.IsEmpty(locators))
                {
                    foreach (PoolConnection connection in locators)
                    {
                        poolFactory.AddLocator(connection.Host, connection.Port);
                    }
                }

                if (!CollectionUtils.IsEmpty(servers))
                {
                    foreach (PoolConnection connection in servers)
                    {
                        poolFactory.AddServer(connection.Host, connection.Port);
                    }
                }

                poolFactory.SetFreeConnectionTimeout(freeConnectionTimeout);
                poolFactory.SetIdleTimeout(idleTimeout);
                poolFactory.SetLoadConditioningInterval(loadConditioningInterval);
                poolFactory.SetMaxConnections(maxConnections);
                poolFactory.SetMinConnections(minConnections);
                poolFactory.SetPingInterval(pingInterval);
                poolFactory.SetReadTimeout(readTimeout);
                poolFactory.SetRetryAttempts(retryAttempts);
                poolFactory.SetServerGroup(serverGroup);
                poolFactory.SetSocketBufferSize(socketBufferSize);
                poolFactory.SetStatisticInterval(statisticInterval);
                poolFactory.SetSubscriptionEnabled(subscriptionEnabled);
                poolFactory.SetSubscriptionAckInterval(subscriptionAckInterval);
                poolFactory.SetSubscriptionMessageTrackingTimeout(subscriptionMessageTrackingTimeout);
                poolFactory.SetSubscriptionRedundancy(subscriptionRedundancy);
                //NOTE thread local connections present in Java API
                // PoolFactoryDefaults.SetThreadLocalConnections(threadLocalConnections);

                pool = poolFactory.Create(name);
            }
        }

        public string ObjectName
        {
            set { this.objectName = value; }
        }

        /// <summary>
        /// Sets the pool.
        /// </summary>
        /// <value>The pool.</value>
        public GemStone.GemFire.Cache.Pool Pool
        {
            set { pool = value; }
        }

        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            set { name = value; }
        }

        /// <summary>
        /// Sets the locators.
        /// </summary>
        /// <value>The locators.</value>
        public IList Locators
        {
            set { locators = value; }
        }

        /// <summary>
        /// Sets the servers.
        /// </summary>
        /// <value>The servers.</value>
        public IList Servers
        {
            set { servers = value; }
        }

        /// <summary>
        /// Sets a value indicating whether to keep alive
        /// </summary>
        /// <value><c>true</c> if keep alive; otherwise, <c>false</c>.</value>
        public bool KeepAlive
        {
            set { keepAlive = value; }
        }

        public int FreeConnectionTimeout
        {
            set { freeConnectionTimeout = value; }
        }

        public int IdleTimeout
        {
            set { idleTimeout = value; }
        }

        public int LoadConditioningInterval
        {
            set { loadConditioningInterval = value; }
        }

        public int MaxConnections
        {
            set { maxConnections = value; }
        }

        public int MinConnections
        {
            set { minConnections = value; }
        }

        public int PingInterval
        {
            set { pingInterval = value; }
        }

        public int ReadTimeout
        {
            set { readTimeout = value; }
        }

        public int RetryAttempts
        {
            set { retryAttempts = value; }
        }

        public string ServerGroup
        {
            set { serverGroup = value; }
        }

        public int SocketBufferSize
        {
            set { socketBufferSize = value; }
        }

        public int StatisticInterval
        {
            set { statisticInterval = value; }
        }

        public int SubscriptionAckInterval
        {
            set { subscriptionAckInterval = value; }
        }

        public bool SubscriptionEnabled
        {
            set { subscriptionEnabled = value; }
        }

        public int SubscriptionMessageTrackingTimeout
        {
            set { subscriptionMessageTrackingTimeout = value; }
        }

        public int SubscriptionRedundancy
        {
            set { subscriptionRedundancy = value; }
        }
    }
}