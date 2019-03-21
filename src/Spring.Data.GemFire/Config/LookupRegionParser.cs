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

using System;
using System.Xml;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

namespace Spring.Data.GemFire.Config
{
    /// <summary>
    /// Parser for <lookup-region/> definitions.
    /// </summary>
    /// <author>Costin Leau</author> 
    /// <author>Mark Pollack (.NET)</author>
    public class LookupRegionParser : AbstractSingleObjectDefinitionParser
    {
        protected override Type GetObjectType(XmlElement element)
        {
            return typeof (RegionLookupFactoryObject);
        }

        protected override void DoParse(XmlElement element, ObjectDefinitionBuilder builder)
        {
            base.DoParse(element, builder);
            ParsingUtils.SetPropertyValue(element, builder, "name", "name");
            String attr = element.GetAttribute("cache-ref");
            // add cache reference (fallback to default if nothing is specified)
            builder.AddPropertyReference("cache", (StringUtils.HasText(attr) ? attr : "gemfire-cache"));            
        }
    }

}