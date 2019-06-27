﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Utils.NET.Utils;

namespace Utils.NET.IO.Xml
{
    public class XmlParser
    {
        /// <summary>
        /// The xml node to parse
        /// </summary>
        private XElement xml;

        public XmlParser(XElement xml)
        {
            this.xml = xml;
        }

        /// <summary>
        /// Returns all children elements with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<XElement> Elements(string name)
        {
            return xml.Elements(name);
        }

        /// <summary>
        /// Attempts to return the element with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out XElement value)
        {
            value = xml.Element(name);
            return value != null;
        }

        /// <summary>
        /// Returns the string value of the node
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string String(string name, string defaultValue = "")
        {
            if (!TryGetValue(name, out var value)) return defaultValue;
            return value.Value;
        }

        /// <summary>
        /// Returns an integer representation of the node's value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int Int(string name, int defaultValue = 0)
        {
            if (!TryGetValue(name, out var value)) return defaultValue;
            return Convert.ToInt32(value.Value);
        }

        /// <summary>
        /// Returns the integer value of a hexadecimal string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public uint Hex(string name, uint defaultValue = 0)
        {
            if (!TryGetValue(name, out var value)) return defaultValue;
            return StringUtils.ParseHex(value.Value);
        }

        /// <summary>
        /// Returns true if the node exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            return xml.Element(name) != null;
        }

        /// <summary>
        /// Attempts to return the element with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAttribute(string name, out XAttribute value)
        {
            value = xml.Attribute(name);
            return value != null;
        }

        /// <summary>
        /// Returns the string value of the attribute
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string AtrString(string name, string defaultValue = "")
        {
            if (!TryGetAttribute(name, out var value)) return defaultValue;
            return value.Value;
        }

        /// <summary>
        /// Returns an integer representation of the attribute's value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int AtrInt(string name, int defaultValue = 0)
        {
            if (!TryGetAttribute(name, out var value)) return defaultValue;
            return Convert.ToInt32(value.Value);
        }

        /// <summary>
        /// Returns the integer value of a hexadecimal attribute value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public uint AtrHex(string name, uint defaultValue = 0)
        {
            if (!TryGetAttribute(name, out var value)) return defaultValue;
            return StringUtils.ParseHex(value.Value);
        }

        /// <summary>
        /// Returns true if the attribute exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AtrExists(string name)
        {
            return xml.Attribute(name) != null;
        }
    }
}
