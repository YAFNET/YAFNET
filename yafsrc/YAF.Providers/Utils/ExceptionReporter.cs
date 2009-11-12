/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Configuration.Provider;
using System.Web;
using System.Xml;
using YAF.Classes;
using YAF.Classes.Utils;

namespace YAF.Providers.Utils
{
  /// <summary>
  /// The exception reporter.
  /// </summary>
  public static class ExceptionReporter
  {
    /// <summary>
    /// Get Exception XML File Name from AppSettings
    /// </summary>
    private static string ProviderExceptionFile
    {
      get
      {
        return Config.GetConfigValueAsString("YAF.ProviderExceptionXML") ?? "ProviderExceptions.xml";
      }
    }

    /// <summary>
    /// Return XMLDocument containing text for the Exceptions
    /// </summary>
    private static XmlDocument ExceptionXML()
    {
      if (String.IsNullOrEmpty(ProviderExceptionFile))
      {
        throw new ApplicationException("Exceptionfile cannot be null or empty!");
      }

      var exceptionXmlDoc = new XmlDocument();
      exceptionXmlDoc.Load(HttpContext.Current.Server.MapPath(String.Format("{0}resources/{1}", YafForumInfo.ForumFileRoot, ProviderExceptionFile)));

      return exceptionXmlDoc;
    }

    /// <summary>
    /// Get Exception String
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get report.
    /// </returns>
    public static string GetReport(string providerSection, string tag)
    {
      string select = string.Format("//provider[@name='{0}']/Resource[@tag='{1}']", providerSection.ToUpper(), tag.ToUpper());
      XmlNode node = ExceptionXML().SelectSingleNode(select);

      if (node != null)
      {
        return node.InnerText;
      }
      else
      {
        return String.Format("Exception({1}:{0}) cannot be found in Exception file!", tag, providerSection);
      }
    }

    /// <summary>
    /// Throw Exception
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The throw.
    /// </returns>
    public static string Throw(string providerSection, string tag)
    {
      throw new ApplicationException(GetReport(providerSection, tag));
    }

    /// <summary>
    /// Throw ArgumentException
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The throw argument.
    /// </returns>
    public static string ThrowArgument(string providerSection, string tag)
    {
      throw new ArgumentException(GetReport(providerSection, tag));
    }

    /// <summary>
    /// Throw ArgumentNullException
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The throw argument null.
    /// </returns>
    public static string ThrowArgumentNull(string providerSection, string tag)
    {
      throw new ArgumentNullException(GetReport(providerSection, tag));
    }

    /// <summary>
    /// Throw NotSupportedException
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The throw not supported.
    /// </returns>
    public static string ThrowNotSupported(string providerSection, string tag)
    {
      throw new NotSupportedException(GetReport(providerSection, tag));
    }

    /// <summary>
    /// Throw ProviderException
    /// </summary>
    /// <param name="providerSection">
    /// The provider Section.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The throw provider.
    /// </returns>
    public static string ThrowProvider(string providerSection, string tag)
    {
      throw new ProviderException(GetReport(providerSection, tag));
    }
  }
}