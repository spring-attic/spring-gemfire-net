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
    public class ClientRegionParser : AbstractSingleObjectDefinitionParser
    {
        protected override Type GetObjectType(XmlElement element)
        {
            return typeof (ClientRegionFactoryObject);
        }


        protected override void DoParse(XmlElement element, ParserContext parserContext, ObjectDefinitionBuilder builder)
        {
            base.DoParse(element, builder);

            //TODO investigate setting of scope on client cache
            //builder.AddPropertyValue("Scope", "some value");

            //TODO check if .NET client has any 'data policy' settings.

            ParsingUtils.SetPropertyValue(element, builder, "name", "name");
            ParsingUtils.SetPropertyValue(element, builder, "pool-name", "poolName");

            String cacheRef = element.GetAttribute("cache-ref");
            // add cache reference (fallback to default if nothing is specified)
            builder.AddPropertyReference("cache", (StringUtils.HasText(cacheRef) ? cacheRef : "gemfire-cache"));

            
		    // client region attributes
            String regionAttributesRef = element.GetAttribute("attributes-ref");
            if (StringUtils.HasText(regionAttributesRef))
            {
                ObjectDefinitionBuilder attrBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(RegionAttributesFactoryObject));
                builder.AddPropertyReference("attributes", regionAttributesRef);
            }
		    
            ManagedList interests = new ManagedList();
            XmlNodeList subElements = element.ChildNodes;
            for (int i = 0; i < subElements.Count; i++)
            {
                XmlNode subElement = subElements.Item(i);
                if (subElement.NodeType == XmlNodeType.Element)
                {
                    string name = subElement.LocalName;
                    if ("cache-listener".Equals(name))
                    {
                        builder.AddPropertyValue("cacheListeners", ParseCacheListener(parserContext, (XmlElement)subElement, builder));
                    }
                    else if ("regex-interest".Equals(name))
                    {
                        interests.Add(ParseRegexInterest(parserContext, (XmlElement)subElement));
                    }                       
                    else if ("key-interest".Equals(name))
                    {
                        interests.Add(ParseKeyInterest(parserContext, (XmlElement)subElement));
                    }
                    else if ("all-keys-interest".Equals(name))
                    {
                        interests.Add(ParseAllKeysInterest(parserContext, (XmlElement) subElement));
                    }

                    
                }
            }

            if (subElements.Count > 0)
            {
                builder.AddPropertyValue("interests", interests); 
            }
            
        }

        private object ParseCacheListener(ParserContext parserContext, XmlElement element, ObjectDefinitionBuilder builder)
        {
            return ParsingUtils.ParseRefOrNestedObjectDeclaration(parserContext, element, builder);
        }

        private object ParseKeyInterest(ParserContext parserContext, XmlElement subElement)
        {
		    ObjectDefinitionBuilder keyInterestBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(KeyInterest));
		    ParseCommonInterestAttr(subElement, keyInterestBuilder);         

		    Object key = ParsingUtils.ParseRefOrNestedObjectDeclaration(parserContext, subElement, keyInterestBuilder, "key-ref");
		    keyInterestBuilder.AddConstructorArg(key);
		    return keyInterestBuilder.ObjectDefinition;
        }

        private object ParseAllKeysInterest(ParserContext parserContext, XmlElement subElement)
        {
            ObjectDefinitionBuilder keyInterestBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(AllKeysInterest));
            ParseCommonInterestAttr(subElement, keyInterestBuilder);
            return keyInterestBuilder.ObjectDefinition;
        }


        private object ParseRegexInterest(ParserContext parserContext, XmlElement subElement)
        {
		    ObjectDefinitionBuilder regexInterestBuilder = ObjectDefinitionBuilder.GenericObjectDefinition(typeof(RegexInterest));

		    ParseCommonInterestAttr(subElement, regexInterestBuilder);
		    ParsingUtils.SetPropertyValue(subElement, regexInterestBuilder, "pattern", "regex");

            return regexInterestBuilder.ObjectDefinition;
        }


        private void ParseCommonInterestAttr(XmlElement element, ObjectDefinitionBuilder builder)
        {
            ParsingUtils.SetPropertyValue(element, builder, "durable", "durable");
            ParsingUtils.SetPropertyValue(element, builder, "result-policy", "policy");
        }
    }

}