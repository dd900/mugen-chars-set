using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MUGENCharsSet
{
    /// <summary>
    /// XML configuration class
    /// </summary>
    public class XmlConfig
    {
        /// <summary>Default root element name</summary>
        public const string DefaultRootName = "config";

        private readonly string _configPath;
        private readonly string _rootName;
        private readonly XDocument _document = null;

        /// <summary>
        /// Get the absolute path of the current configuration file
        /// </summary>
        public string ConfigPath
        {
            get { return _configPath; }
        }

        /// <summary>
        /// Get the current configuration file<see cref="XDocument"/>Object
        /// </summary>
        public XDocument Document
        {
            get { return _document; }
        }

        /// <summary>
        /// Get root elements
        /// </summary>
        public XElement Root
        {
            get { return Document.Root; }
        }

        /// <summary>
        /// Get the name of the root element
        /// </summary>
        public string RootName
        {
            get { return _rootName; }
        }

        /// <summary>
        /// Create according to the specified configuration file path and root element name<see cref="XmlConfig"/>Class new instance, can indicate whether to create a new configuration file when loading fails
        /// </summary>
        /// <param name="configPath">Profile absolute path</param>
        /// <param name="isOverWrite">Whether to create a new configuration file when loading fails</param>
        /// <param name="rootName">Root element name</param>
        /// <exception cref="System.ApplicationException"></exception>
        public XmlConfig(string configPath, bool isOverWrite = false, string rootName = DefaultRootName)
        {
            _configPath = configPath;
            _rootName = rootName;
            try
            {
                _document = XDocument.Load(ConfigPath);
            }
            catch (Exception)
            {
                if (isOverWrite)
                {
                    if (!CreateConfigFile()) throw new ApplicationException("Create a configuration file failed！");
                    try
                    {
                        _document = XDocument.Load(ConfigPath);
                    }
                    catch (Exception)
                    {
                        throw new ApplicationException("Loading the configuration file failed！");
                    }
                }
                else
                {
                    throw new ApplicationException("Loading the configuration file failed！");
                }
            }
        }

        /// <summary>
        /// Save the configuration file
        /// </summary>
        /// <returns>Whether saving success</returns>
        public bool Save()
        {
            try
            {
                Document.Save(ConfigPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create a configuration file
        /// </summary>
        /// <returns>Whether to create success</returns>
        private bool CreateConfigFile()
        {
            try
            {
                XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(RootName));
                document.Save(ConfigPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Element value</returns>
        public string GetValue(string elementName, string defaultValue)
        {
            try
            {
                XElement element = Root.Element(elementName);
                if (element == null) return defaultValue;
                else return element.Value.Trim();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Element value</returns>
        public int GetValue(string elementName, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(GetValue(elementName, defaultValue.ToString()));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Element value</returns>
        public bool GetValue(string elementName, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(GetValue(elementName, defaultValue.ToString()));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="value">Element value</param>
        /// <returns>Whether to set success</returns>
        public bool SetValue(string elementName, string value)
        {
            try
            {
                XElement element = Root.Element(elementName);
                if (element != null)
                {
                    element.SetValue(value);
                }
                else
                {
                    element = new XElement(elementName, value);
                    Root.Add(element);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="value">Element value</param>
        /// <returns>Whether to set success</returns>
        public bool SetValue(string elementName, int value)
        {
            return SetValue(elementName, value.ToString());
        }

        /// <summary>
        /// Set the value of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="value">Element value</param>
        /// <returns>Whether to set success</returns>
        public bool SetValue(string elementName, bool value)
        {
            return SetValue(elementName, value.ToString());
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public string GetAttributeValue(string elementName, string attributeName, string defaultValue)
        {
            return GetAttributeValue(Root.Element(elementName), attributeName, defaultValue);
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public int GetAttributeValue(string elementName, string attributeName, int defaultValue)
        {
            return GetAttributeValue(Root.Element(elementName), attributeName, defaultValue);
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public bool GetAttributeValue(string elementName, string attributeName, bool defaultValue)
        {
            return GetAttributeValue(Root.Element(elementName), attributeName, defaultValue);
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public string GetAttributeValue(XElement element, string attributeName, string defaultValue)
        {
            try
            {
                if (element != null)
                {
                    XAttribute attribute = element.Attribute(attributeName);
                    if (attribute != null) return attribute.Value;
                    else return defaultValue;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="element">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public int GetAttributeValue(XElement element, string attributeName, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(GetAttributeValue(element, attributeName, defaultValue.ToString()));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the value of the specified properties of the specified element
        /// </summary>
        /// <param name="element">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">Defaults</param>
        /// <returns>Value of the attribute</returns>
        public bool GetAttributeValue(XElement element, string attributeName, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(GetAttributeValue(element, attributeName, defaultValue.ToString()));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Set the value of the specified attribute of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>Whether to set success</returns>
        public bool SetAttributeValue(string elementName, string attributeName, string value)
        {
            try
            {
                XElement element = Root.Element(elementName);
                if (element != null)
                {
                    element.SetAttributeValue(attributeName, value);
                }
                else
                {
                    element = new XElement(elementName, new XAttribute(attributeName, value));
                    Root.Add(element);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set the value of the specified attribute of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>Whether to set success</returns>
        public bool SetAttributeValue(string elementName, string attributeName, int value)
        {
            return SetAttributeValue(elementName, attributeName, value.ToString());
        }

        /// <summary>
        /// Set the value of the specified attribute of the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns>Whether to set success</returns>
        public bool SetAttributeValue(string elementName, string attributeName, bool value)
        {
            return SetAttributeValue(elementName, attributeName, value.ToString());
        }

        /// <summary>
        /// Gets a collection of filtered child elements for specified elements
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="subElementName">Child element name</param>
        /// <returns>Child elements collection</returns>
        public IEnumerable<XElement> GetElements(string elementName, string subElementName)
        {
            try
            {
                return Root.Element(elementName).Elements(subElementName);
            }
            catch (Exception)
            {
                return new List<XElement>();
            }
        }

        /// <summary>
        /// Get all sub-elements collection of specified elements
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <returns>Child elements collection</returns>
        public IEnumerable<XElement> GetElements(string elementName)
        {
            try
            {
                return Root.Element(elementName).Elements();
            }
            catch (Exception)
            {
                return new List<XElement>();
            }
        }

        /// <summary>
        /// Gets the filtered property set for the specified element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Attribute collection</returns>
        public IEnumerable<XAttribute> GetAttributes(string elementName, string attributeName)
        {
            try
            {
                return Root.Element(elementName).Attributes(attributeName);
            }
            catch (Exception)
            {
                return new List<XAttribute>();
            }
        }

        /// <summary>
        /// Get all property sets for specified elements
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <returns>Attribute collection</returns>
        public IEnumerable<XAttribute> GetAttributes(string elementName)
        {
            try
            {
                return Root.Element(elementName).Attributes();
            }
            catch (Exception)
            {
                return new List<XAttribute>();
            }
        }
    }
}