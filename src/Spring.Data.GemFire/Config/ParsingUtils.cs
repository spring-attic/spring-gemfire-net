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
using System.Xml;
using Spring.Objects.Factory.Config;
using Spring.Objects.Factory.Support;
using Spring.Objects.Factory.Xml;
using Spring.Util;

#endregion

namespace Spring.Data.GemFire.Config
{
    /// <summary>
    /// Various minor utility used by the parser.
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    internal abstract class ParsingUtils
    {
        public static void SetPropertyValue(XmlElement element, ObjectDefinitionBuilder builder, string attrName,
                                            string propertyName)
        {
            string attr = element.GetAttribute(attrName);
            if (StringUtils.HasText(attr))
            {
                builder.AddPropertyValue(propertyName, attr);
            }
        }

        public static void SetPropertyReference(XmlElement element, ObjectDefinitionBuilder builder, string attrName,
                                                string propertyName)
        {
            string attr = element.GetAttribute(attrName);
            if (StringUtils.HasText(attr))
            {
                builder.AddPropertyReference(propertyName, attr);
            }
        }

        // Utility method handling parsing of nested definition of the type:
        // <pre>
        //   <tag ref="someObject"/>
        // </pre>
        // or 
        // <pre>
        //   <tag>
        //     <object .... />
        //     <ref = .... />
        //   </tag>
        // </pre>
        public static object ParseRefOrNestedObjectDeclaration(ParserContext parserContext, XmlElement element,
                                                               ObjectDefinitionBuilder builder)
        {
            return ParseRefOrNestedObjectDeclaration(parserContext, element, builder, "ref");
        }

        public static object ParseRefOrNestedObjectDeclaration(ParserContext parserContext, XmlElement element,
                                                                ObjectDefinitionBuilder builder, string refAttrName)
        {
            String attr = element.GetAttribute(refAttrName);
            bool hasRef = StringUtils.HasText(attr);


            XmlNodeList childNodes = element.ChildNodes;
            if (hasRef)
            {
                if (childNodes.Count > 0)
                {
                    //"either use the '" + refAttrName + "' attribute or a nested object declaration for '"
                    //+ element.getLocalName() + "' element, but not both", element);
                    parserContext.ReaderContext.ReportException(element, element.LocalName,
                                                                "either use the '" + refAttrName +
                                                                "' attribute or a nested object declaration for '"
                                                                + element.LocalName + "' element, but not both");
                }
                return new RuntimeObjectReference(attr);
            }

            if (childNodes.Count == 0)
            {
                parserContext.ReaderContext.ReportException(element, element.LocalName,
                                                            "specify either '" + refAttrName +
                                                            "' attribute or a nested object declaration for '"
                                                            + element.LocalName + "' element");
            }
            // nested parse nested object definition
            if (childNodes.Count == 1)
            {
                if (childNodes[0].NodeType == XmlNodeType.Element)
                {
                    XmlElement childElement = (XmlElement) childNodes[0];
                    return ParsePropertySubElement(childElement, builder.RawObjectDefinition, parserContext);
                }
            }


            ManagedList list = new ManagedList();

            for (int i = 0; i < childNodes.Count; i++)
            {
                XmlNode childNode = childNodes.Item(i);
                if (childNode != null && childNode.NodeType == XmlNodeType.Element)
                {
                    list.Add(ParsePropertySubElement((XmlElement) childNode, builder.RawObjectDefinition, parserContext));
                }
            }

            return list;
        }

        private static object ParsePropertySubElement(XmlElement childElement,
                                                      AbstractObjectDefinition rawObjectDefinition,
                                                      ParserContext parserContext)
        {
            string localName = childElement.LocalName;
            if ("object".Equals(localName))
            {
                ObjectDefinitionHolder holder = parserContext.ParserHelper.ParseObjectDefinitionElement(childElement);

                return holder;
            }
            if ("ref".Equals(localName))
            {
                string reference = childElement.GetAttribute("object");
                return new RuntimeObjectReference(reference);
            }
            parserContext.ReaderContext.ReportException(childElement, localName, "unsupported element");
            return null;
            
        }
    }
}