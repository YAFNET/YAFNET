/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Providers.Utils
{
    #region Using

    using System;
    using System.Configuration.Provider;
    using System.Web;
    using System.Xml;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The exception reporter.
    /// </summary>
    public static class ExceptionReporter
    {
        #region Properties

        /// <summary>
        ///   Gets the Exception XML File Name from AppSettings
        /// </summary>
        [NotNull]
        private static string ProviderExceptionFile
        {
            get
            {
                return Config.GetConfigValueAsString("YAF.ProviderExceptionXML") ?? "ProviderExceptions.xml";
            }
        }

        #endregion

        #region Public Methods

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
        public static string GetReport([NotNull] string providerSection, [NotNull] string tag)
        {
            string select = "//provider[@name='{0}']/Resource[@tag='{1}']".FormatWith(
                providerSection.ToUpper(),
                tag.ToUpper());
            XmlNode node = ExceptionXML().SelectSingleNode(select);

            return node != null
                       ? node.InnerText
                       : "Exception({1}:{0}) cannot be found in Exception file!".FormatWith(tag, providerSection);
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
        public static string Throw([NotNull] string providerSection, [NotNull] string tag)
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
        public static string ThrowArgument([NotNull] string providerSection, [NotNull] string tag)
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
        public static string ThrowArgumentNull([NotNull] string providerSection, [NotNull] string tag)
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
        public static string ThrowNotSupported([NotNull] string providerSection, [NotNull] string tag)
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
        public static string ThrowProvider([NotNull] string providerSection, [NotNull] string tag)
        {
            throw new ProviderException(GetReport(providerSection, tag));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Return XMLDocument containing text for the Exceptions
        /// </summary>
        /// <returns> Returns the XMLDocument containing text for the Exceptions</returns>
        /// <exception cref="System.ApplicationException">Exception file cannot be null or empty!</exception>
        [NotNull]
        private static XmlDocument ExceptionXML()
        {
            if (ProviderExceptionFile.IsNotSet())
            {
                throw new ApplicationException("Exceptionfile cannot be null or empty!");
            }

            var exceptionXmlDoc = new XmlDocument();
            exceptionXmlDoc.Load(
                HttpContext.Current.Server.MapPath(
                    "{0}{1}resources/{1}".FormatWith(
                        Config.ServerFileRoot,
                        Config.ServerFileRoot.EndsWith("/") ? string.Empty : "/",
                        ProviderExceptionFile)));

            return exceptionXmlDoc;
        }

        #endregion
    }
}