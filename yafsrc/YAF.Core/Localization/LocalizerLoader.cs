﻿/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core
{
  #region Using

  using System;
  using System.IO;
  using System.Text;
  using System.Web;
  using System.Web.Caching;
  using System.Xml;
  using System.Xml.Serialization;

  using YAF.Utils;
  using YAF.Utils.Helpers.StringUtils;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The localizer loader.
  /// </summary>
  public class LocalizerLoader
  {
    #region Public Methods

    /// <summary>
    /// The attempt load language file.
    /// </summary>
    /// <param name="languageFileName">
    /// The language File Name.
    /// </param>
    /// <param name="cacheName">
    /// The cache Name.
    /// </param>
    /// <returns>
    /// </returns>
    public LanguageResources LoadLanguageFile(string languageFileName, string cacheName)
    {
      var file = HttpRuntime.Cache.Get(cacheName) as LanguageResources;

      if (file != null)
      {
        return file;
      }

      if (languageFileName.IsSet() && File.Exists(languageFileName))
      {
        lock (this)
        {
          var serializer = new XmlSerializer(typeof(LanguageResources));
          var sourceEncoding = this.GetEncodingForXmlFile(languageFileName);

          using (var sourceReader = new StreamReader(languageFileName, sourceEncoding))
          {
            var resources = (LanguageResources)serializer.Deserialize(sourceReader);

            // transform the page and tag name ToUpper...
            resources.page.ForEach(p => p.name = p.name.ToUpper());
            resources.page.ForEach(p => p.Resource.ForEach(i => i.tag = i.tag.ToUpper()));

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

    #endregion

    #region Methods

    /// <summary>
    /// The get encoding for xml file.
    /// </summary>
    /// <param name="xmlFileName">
    /// The xml file name.
    /// </param>
    /// <returns>
    /// </returns>
    private Encoding GetEncodingForXmlFile(string xmlFileName)
    {
      var doc = new XmlDocument();

      doc.Load(xmlFileName);

      // The first child of a standard XML document is the XML declaration.
      // The following code assumes and reads the first child as the XmlDeclaration.
      if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
      {
        // Get the encoding declaration.
        var decl = (XmlDeclaration)doc.FirstChild;
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

    #endregion
  }
}