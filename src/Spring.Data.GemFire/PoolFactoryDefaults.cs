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

namespace Spring.Data.GemFire
{
    /// <summary>
    /// Static value for default values of the Pool 
    /// </summary>
    /// <author>Mark Pollack</author>
    public abstract class PoolFactoryDefaults
    {
        public static int DEFAULT_FREE_CONNECTION_TIMEOUT = 10000;

        public static int DEFAULT_LOAD_CONDITIONING_INTERVAL = 300000;

        public static int DEFAULT_SOCKET_BUFFER_SIZE = 32768;

        public static int DEFAULT_READ_TIMEOUT = 10000;

        public static int DEFAULT_MIN_CONNECTIONS = 1;

        public static int DEFAULT_MAX_CONNECTIONS = -1;

        //NOTE is long in java
        public static int DEFAULT_IDLE_TIMEOUT = 5000;

        public static int DEFAULT_RETRY_ATTEMPTS = -1;
        
        //NOTE is long in java
        public static int DEFAULT_PING_INTERVAL = 10000;

        public static int DEFAULT_STATISTIC_INTERVAL = -1;

        public static bool DEFAULT_THREAD_LOCAL_CONNECTIONS = false;

        public static bool DEFAULT_SUBSCRIPTION_ENABLED = false;

        public static int DEFAULT_SUBSCRIPTION_REDUNDANCY;

        public static int DEFAULT_SUBSCRIPTION_MESSAGE_TRACKING_TIMEOUT = 900000;

        public static int DEFAULT_SUBSCRIPTION_ACK_INTERVAL = 100;

        public static string DEFAULT_SERVER_GROUP = "";

        public static bool DEFAULT_PR_SINGLE_HOP_ENABLED = true;

        public static bool DEFAULT_MULTIUSER_AUTHENTICATION = false;
    }
}