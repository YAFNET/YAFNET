namespace YAF.Classes.Core
{
  using System;
  using System.IO;
  using System.Text;
  using System.Web;
  using System.Web.Caching;
  using System.Xml;
  using System.Xml.Serialization;

  using YAF.Classes.Utils;

  public class LocalizerLoader
  {
    /// <summary>
    /// The attempt load language file.
    /// </summary>
    /// <returns>
    /// </returns>
    public Resources LoadSiteFile(string languageFileName, string cacheName)
    {
      var file = HttpRuntime.Cache.Get(cacheName) as Resources;

      if (file != null)
      {
        return file;
      }

      if (languageFileName.IsSet() && File.Exists(languageFileName))
      {
        lock (this)
        {
          var serializer = new XmlSerializer(typeof(Resources));
          var sourceEncoding = this.GetEncodingForXmlFile(languageFileName);

          using (var sourceReader = new StreamReader(languageFileName, sourceEncoding))
          {
            var resources = (Resources)serializer.Deserialize(sourceReader);

            if (cacheName.IsSet())
            {
              var fileDependency = new CacheDependency(languageFileName);
              HttpRuntime.Cache.Add(
                cacheName,
                resources,
                fileDependency,
                DateTime.UtcNow.AddHours(1.0),
                TimeSpan.Zero,
                CacheItemPriority.Default,
                null);
            }

            return resources;
          }
        }
      }

      return null;
    }

    private Encoding GetEncodingForXmlFile(string xmlFileName)
    {
      XmlDocument doc = new XmlDocument();

      doc.Load(xmlFileName);

      // The first child of a standard XML document is the XML declaration.
      // The following code assumes and reads the first child as the XmlDeclaration.
      if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
      {
        // Get the encoding declaration.
        XmlDeclaration decl = (XmlDeclaration)doc.FirstChild;
        Encoding currentEncoding;
        try
        {
          currentEncoding = Encoding.GetEncoding(decl.Encoding);
          return currentEncoding;
        }
        catch
        {
          // use default...
        }
      }

      return Encoding.UTF8;
    }
  }
}