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
using System.Threading;
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Objects.Factory;

namespace Spring.Data.Gemfire.HelloWorld
{
    /// <summary>
    /// Main object for interacting with the cache from the console. 
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class HelloWorld : IInitializingObject , IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HelloWorld));

        private CommandProcessor processor; 
        
        public CommandProcessor CommandProcessor
        {
            set { processor = value; }
        }

        public void AfterPropertiesSet()
        {
            processor.Start();
        }


        public void Dispose()
        {
            processor.Stop();
        }

        public void GreetWorld(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    processor.CommandLinePrefix = args[0];
                }
                processor.AwaitCommands();
            } catch  (Exception ex)
            {
                throw new IllegalStateException("Cannot greet world", ex);
            }
        }
    }

}