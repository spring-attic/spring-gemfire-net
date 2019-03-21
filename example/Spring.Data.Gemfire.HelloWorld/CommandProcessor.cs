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

#region

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Common.Logging;
using GemStone.GemFire.Cache;
using Spring.Util;

#endregion

namespace Spring.Data.Gemfire.HelloWorld
{
    /// <summary>
    /// Entity processing and interpreting shell commands. 
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class CommandProcessor
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (CommandProcessor));

        private static string regexString =
            "query|exit|help|size|clear|keys|values|map|containsKey|containsValue|get|remove|put";

        private Regex regex;

        private static string help = InitHelp();

        private bool threadActive;

        private Thread thread;


        private Region region;

        public Region Region
        {
            set { region = value; }
        }

        public string CommandLinePrefix
        {
            set { commandLinePrefix = value; }
        }

        private static string InitHelp()
        {
            return System.IO.File.ReadAllText("help.txt");
        }

        public CommandProcessor()
        {
            RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) |
                                    RegexOptions.IgnoreCase);
            regex = new Regex(regexString, options);
        }

        private static string EMPTY = "";
        private string commandLinePrefix;

        public void Start()
        {
            LOG.Info("Distributed System " + region.Cache.DistributedSystem.Name + " connecting to region [" +
                     region.Name + "]");

            if (thread == null)
            {
                threadActive = true;
                thread = new Thread(ProcessCommands);
                thread.Start();
            }
        }

        public void Stop()
        {
            LOG.Info("Distributed System " + region.Cache.DistributedSystem.Name + " disconnecting from region [" +
                     region.Name + "]");
            threadActive = false;
            thread.Join(3*100);
        }

        public void ProcessCommands()
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Want to interact with the world ? ...");
            Console.WriteLine(help);
            Console.Write(commandLinePrefix + ">");
            Console.Out.Flush();
            try
            {
                while (threadActive)
                {
                    string expr = Console.ReadLine();
                    if (expr == null)
                    {
                        break;
                    }
                    try
                    {
                        Console.WriteLine(Process(expr));
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error executing last command", e);
                    }
                    Console.Write(commandLinePrefix + ">");
                    Console.Out.Flush();
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception while processing commands.", ex);
            }
        }

        private string Process(string expr)
        {
            Match match = regex.Match(expr);
            if (match.Success)
            {
                string[] args = StringUtils.DelimitedListToStringArray(expr, " ");
                string command = args[0];
                string arg1 = (args.Length >= 2 ? args[1] : null);
                string arg2 = (args.Length == 3 ? args[2] : null);

                if (IsMatch("query", command))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");
                    string query = expr.Trim().Substring(command.Length);
                    ISelectResults resultSet = region.Query(query);


                    for (uint i = 0; i < resultSet.Size; i++)
                    {
                        sb.Append(resultSet[i].ToString());
                        if (i != resultSet.Size - 1) sb.Append(",");
                    }
                    sb.Append("]");
                    return sb.ToString();                   
                    
                }

                // parse commands w/o arguments
                if (IsMatch("exit", command))
                {
                    threadActive = false;
                    return "Node exiting...";
                }
                if (IsMatch("help", command))
                {
                    return help;
                }
                if (IsMatch("size", command))
                {
                    return "" + region.Size;
                }
                if (IsMatch("clear", command))
                {
                    region.Clear();
                    return "Clearing grid...";
                }
                if (IsMatch("keys", command))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");
                    ICacheableKey[] keys = region.GetKeys();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        sb.Append(keys[i]);
                        if (i != keys.Length - 1) sb.Append(",");
                    }
                    sb.Append("]");
                    return sb.ToString();
                }
                if (IsMatch("values", command))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");
                    IGFSerializable[] values = region.GetValues();
                    for (int i = 0; i < values.Length; i++)
                    {
                        sb.Append(values[i]);
                        if (i != values.Length - 1) sb.Append(",");
                    }
                    sb.Append("]");
                    return sb.ToString();
                }
                if (IsMatch("map", command))
                {
                    RegionEntry[] regionEntries = region.GetEntries(false);
                    if (regionEntries.Length == 0)
                    {
                        return "[]";
                    }
                    StringBuilder sb = new StringBuilder();
                    foreach (RegionEntry regionEntry in regionEntries)
                    {
                        sb.Append("[").Append(regionEntry.Key.ToString()).Append("=").Append(
                            regionEntry.Value.ToString()).Append("]");
                    }
                    return sb.ToString();
                }


                //commands w/ 1 arg
                if (IsMatch("containsKey", command))
                {
                    return "" + region.ContainsKey(arg1);
                }
                if (IsMatch("containsValue", command))
                {
                    return "not yet implemented";
                    //return "" + region.ExistsValue(arg1);
                }
                if (IsMatch("get", command))
                {
                    IGFSerializable cValue = region.Get(arg1);
                    if (cValue == null)
                    {
                        return "null";
                    }
                    return cValue.ToString();
                }
                if (IsMatch("remove", command))
                {
                    return "not yet implemented";
                }

                // commands w/ 2 args
                if (IsMatch("put", command))
                {
                    IGFSerializable oldValue = region.Get(arg1);
                    region.Put(arg1, arg2);
                    if (oldValue == null)
                    {
                        return "null";
                    }
                    return "old value = [" + oldValue.ToString() + "]";
                }
                return "unknown command [" + command + "] - type 'help' for available commands";
            }
            return "unknown command [" + expr + "] -  type 'help' for available commands";
        }

        private bool IsMatch(string command, string userText)
        {
            if (String.Compare(command, userText, true) == 0) return true;
            return false;
        }

        public void AwaitCommands()
        {
            thread.Join();
        }
    }
}