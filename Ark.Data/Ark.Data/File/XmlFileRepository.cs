using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Ark.Data
{
    /// <summary>
    /// This is the base class for all the manipulation on a Xml content. 
    /// </summary>
    public class XmlFileRepository
    {
        /// <summary>
        /// Serialize a [Serializable] object of type T into an XML-formatted string using XmlSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">any object with SerializeAttribute to serialize to XML.</param>
        /// <param name="namespacesDictionary">The namespaces to add for the serialization</param>
        /// <param name="removeEmptyNodes">Whether the empty nodes should be removed.</param>
        /// <returns>
        /// Success : The serialized XML is returned.
        /// BadParameters : The value is null.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<string>> SerializeToXmlString<T>(T value, Dictionary<string, string> namespacesDictionary = null, bool removeEmptyNodes = false) => Task.Run(() =>
        {
            if (value == null)
                return new Result<string>(ResultStatus.BadParameters).WithReason("The value given is null");

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespacesDictionary?.ForEach(n => namespaces.Add(n.Key, n.Value));

                    xmlSerializer.Serialize(writer, value, namespaces);
                    var xml = stringWriter.ToString();

                    if (removeEmptyNodes)
                    {
                        var xDoc = XDocument.Parse(xml);
                        xDoc.Descendants().Where(child => string.IsNullOrEmpty(child.Value)).Remove();
                        xDoc.Descendants().Reverse().Where(child => !child.HasElements && child.Value.IsNullOrEmpty() && !child.HasAttributes).Remove();
                        xml = xDoc.ToString(SaveOptions.DisableFormatting);
                    }

                    return new Result<string>(xml);
                }
            }
            catch (Exception exception)
            {
                return new Result<string>(exception);
            }
        });

        /// <summary>
        /// Serialize a [Serializable] object of type T into an XML-formatted byte[] using XmlSerializer
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">any object with SerializeAttribute to serialize to XML.</param>
        /// <param name="namespacesDictionary">The namespaces to add for the serialization</param>
        /// <returns>
        /// Success : The serialized XML is returned.
        /// BadParameters : The value is null.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<byte[]>> SerializeToXmlByte<T>(T value, Dictionary<string, string> namespacesDictionary = null) => Task.Run(() =>
        {
            if (value == null)
                return new Result<byte[]>(ResultStatus.BadParameters).WithReason("Parameter is null");

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));

                using (var memoryStream = new MemoryStream())
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespacesDictionary?.ForEach(n => namespacesDictionary.Add(n.Key, n.Value));

                    xmlSerializer.Serialize(memoryStream, value, namespaces);
                    return new Result<byte[]>(memoryStream.ToArray());
                }
            }
            catch (Exception exception)
            {
                return new Result<byte[]>(exception);
            }
        });


        /// <summary>
        /// De-serialize a [Serializable] object of type T into an XML-formatted string using XmlSerializer
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <returns>
        /// Success : The deserialized T object is returned.
        /// BadParameters : The xml is null or empty.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result<T>> DeserializeFromXml<T>(string xml) => Task.Run(() =>
        {
            if (xml.IsNullOrEmpty())
                return new Result<T>(ResultStatus.BadParameters).WithReason("Parameter is null or empty");
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                var stringReader = new StringReader(xml);
                using (var reader = XmlReader.Create(stringReader))
                {
                    var obj = (T)xmlSerializer.Deserialize(reader);
                    return new Result<T>(obj);
                }
            }
            catch (Exception exception)
            {
                return new Result<T>(exception);
            }
        });

        /// <summary>
        /// Extracts the XML namespaces from a XML content.
        /// It searches all the xmlns: text in the XML and create and namespace dictionary with the full namespace and the shortcut.
        /// </summary>
        /// <param name="xml">The XML to extract the namespaces from.</param>
        /// <returns>The namespace dictionary with the full namespace and the shortcut.</returns>
        public XmlNamespaceManager ExtractNamespacesFromXml(string xml)
        {
            var index = xml.IndexOf("xmlns:", StringComparison.InvariantCulture);
            var namespaces = new XmlNamespaceManager(new NameTable());
            while (index > 0)
            {
                index += 6;
                var prefix = xml.Substring(index, xml.IndexOf('=', index) - index);
                index += prefix.Length + 2;
                var uri = xml.Substring(index, xml.IndexOfAny(new[] { '"', '\'' }, index) - index);

                if (!namespaces.HasNamespace(prefix))
                    namespaces.AddNamespace(prefix, uri);

                index = xml.IndexOf("xmlns:", index, StringComparison.InvariantCulture);
            }

            return namespaces;
        }
    }
}