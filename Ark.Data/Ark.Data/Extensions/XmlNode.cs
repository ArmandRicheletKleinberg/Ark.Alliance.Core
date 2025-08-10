using System.Linq;
using System.Xml;

namespace Ark.Data
{
    /// <summary>
    /// This class extends the <see cref="XmlNode"/> class.
    /// </summary>
    public static class XmlNodeExtensions
    {
        #region Methods (Public)

        /// <summary>
        /// Extracts the namespace manager used for XML query from the XML document attributes.
        /// </summary>
        /// <param name="xmlNode">The xml node which can be also a XmlDocument.</param>
        /// <returns>The XML namespace manager filled with the XML document namespaces attributes.</returns>
        public static XmlNamespaceManager ExtractNamespaceManagerFromDocument(this XmlNode xmlNode)
        {
            var xmlDocument = xmlNode as XmlDocument ?? xmlNode?.OwnerDocument;
            var xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlDocument?.DocumentElement?.Attributes.OfType<XmlAttribute>()
                .Where(a => a.Name.StartsWith("xmlns:"))
                .ForEach(a => xmlNamespaceManager.AddNamespace(a.Name.Substring(6), a.Value));
            return xmlNamespaceManager;
        }

        #endregion Methods (Public)
    }
}
