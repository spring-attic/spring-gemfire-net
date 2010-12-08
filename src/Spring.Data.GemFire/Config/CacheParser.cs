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
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace Spring.Data.GemFire.Config
{
    /// <summary>
    /// Parser for <cache/> definitions.
    /// </summary>
    /// <author>Costin Leau</author> 
    /// <author>Mark Pollack (.NET)</author>
    public class CacheParser : AbstractSingleObjectDefinitionParser
    {
        protected override Type GetObjectType(XmlElement element)
        {
            return typeof (CacheFactoryObject);
        }

        protected override void DoParse(XmlElement element, ObjectDefinitionBuilder builder)
        {
            base.DoParse(element, builder);
            ParsingUtils.SetPropertyValue(element, builder, "cache-xml-location", "CacheXml");
            ParsingUtils.SetPropertyValue(element, builder, "disconnect-on-close", "DisconnectOnClose");
            ParsingUtils.SetPropertyValue(element, builder, "keepalive-on-close", "KeepAliveOnClose");
            ParsingUtils.SetPropertyValue(element, builder, "distributed-system-name", "DistributedSystemName");
            ParsingUtils.SetPropertyReference(element, builder, "properties-ref", "Properties");
        }

        protected override string ResolveId(XmlElement element, AbstractObjectDefinition definition, ParserContext parserContext)
        {
            String name = base.ResolveId(element, definition, parserContext);
            if (!StringUtils.HasText(name))
            {
                name = "gemfire-cache";
            }
            return name;
        }
    }

}