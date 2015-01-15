using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Glueware.KlikAanKlikUit.Client
{
    public static class XmlExtensions
    {
        public static byte[] GetBytes(this XDocument doc, Encoding enc)
        {
            using (var ms = new MemoryStream())
            {
                doc.Save(ms, SaveOptions.OmitDuplicateNamespaces);
                return ms.ToArray();
            }
        }
    }
}