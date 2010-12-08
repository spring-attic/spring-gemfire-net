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
using System.Xml;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace Spring.Data.GemFire.Config
{
    /// <summary>
    /// Parser for <client-region/> definitions.
    /// </summary>
    /// <author>Costin Leau</author> 
    /// <author>Mark Pollack (.NET)</author>
    public class PoolParser : AbstractSimpleObjectDefinitionParser
    {
        protected override Type GetObjectType(XmlElement element)
        {
            return typeof(PoolFactoryObject);
        }


        protected override void DoParse(XmlElement element, ObjectDefinitionBuilder builder)
        {
            base.DoParse(element, builder);
            ParseSimpleProperties(element, builder);

            XmlNodeList subElements = element.ChildNodes;
            ManagedList locators = new ManagedList(subElements.Count);
            ManagedList servers = new ManagedList(subElements.Count);
            for (int i = 0; i < subElements.Count; i++) {
                XmlNode subElement = subElements.Item(i);
                if (subElement != null && subElement.NodeType == XmlNodeType.Element)
                {                    
                    if ("locator".Equals(subElement.LocalName))
                    {
                        locators.Add(ParseLocator((XmlElement)subElement));
                    }
                    if ("server".Equals(subElement.LocalName))
                    {
                        servers.Add(ParseServer((XmlElement)subElement));
                    }
                }
            }

            if (locators.Count > 0)
            {
                builder.AddPropertyValue("Locators", locators);
            }
            if (servers.Count > 0)
            {
                builder.AddPropertyValue("Servers", servers);
            }


        }

        private void ParseSimpleProperties(XmlElement element, ObjectDefinitionBuilder builder)
        {
            ParsingUtils.SetPropertyValue(element, builder, "free-connection-timeout", "FreeConnectionTimeout");
            ParsingUtils.SetPropertyValue(element, builder, "idle-timeout", "IdleTimeout");
            ParsingUtils.SetPropertyValue(element, builder, "load-conditioning-interval", "LoadConditioningInterval");
            ParsingUtils.SetPropertyValue(element, builder, "max-connections", "MaxConnections");
            ParsingUtils.SetPropertyValue(element, builder, "min-connections", "MinConnections");
            ParsingUtils.SetPropertyValue(element, builder, "ping-interval", "PingInterval");
            ParsingUtils.SetPropertyValue(element, builder, "read-timeout", "ReadTimeout");
            ParsingUtils.SetPropertyValue(element, builder, "retry-attempts", "RetryAttempts");
            ParsingUtils.SetPropertyValue(element, builder, "server-group", "ServerGroup");
            ParsingUtils.SetPropertyValue(element, builder, "socket-buffer-size", "SocketBufferSize");
            ParsingUtils.SetPropertyValue(element, builder, "statistic-interval", "StatisticInterval");
            ParsingUtils.SetPropertyValue(element, builder, "subscription-ack-interval", "SubscriptionAckInterval");
            ParsingUtils.SetPropertyValue(element, builder, "subscription-enabled", "SubscriptionEnabled");
            ParsingUtils.SetPropertyValue(element, builder, "subscription-message-tracking-timeout", "SubscriptionMessageTrackingTimeout");
            ParsingUtils.SetPropertyValue(element, builder, "subscription-redundancy", "SubscriptionRedundancy");
        }

        private object ParseServer(XmlElement subElement)
        {
            return ParseConnection(subElement);
        }

        private object ParseLocator(XmlElement subElement)
        {
            return ParseConnection(subElement);
        }

        private object ParseConnection(XmlElement subElement)
        {
            ObjectDefinitionBuilder definitionBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof (PoolConnection));
            ParsingUtils.SetPropertyValue(subElement, definitionBuilder, "host", "host");
            ParsingUtils.SetPropertyValue(subElement, definitionBuilder, "port", "port");
            return definitionBuilder.ObjectDefinition;
        }


        protected override string ResolveId(XmlElement element, AbstractObjectDefinition definition, ParserContext parserContext)
        {
            String name = base.ResolveId(element, definition, parserContext);
            if (!StringUtils.HasText(name))
            {
                name = "gemfire-pool";
            }
            return name;
        }
    }

}