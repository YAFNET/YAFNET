/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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
                    "{0}{1}Resources/{2}".FormatWith(
                        Config.ServerFileRoot,
                        Config.ServerFileRoot.EndsWith("/") ? string.Empty : "/",
                        ProviderExceptionFile)));

            return exceptionXmlDoc;
        }

        #endregion
    }
}