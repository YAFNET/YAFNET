/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Helpers
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.Utilities.Helpers.StringUtils;
    using YAF.Types;
    using YAF.Types.Extensions;

    /// <summary>
    /// URL Rewriter Helper Class
    /// </summary>
    public static class UrlRewriteHelper
    {
        #region Methods

        /// <summary>
        /// Cleans the string for URL.
        /// </summary>
        /// <param name="inputString">The input String.</param>
        /// <returns>
        /// The clean string for url.
        /// </returns>
        public static string CleanStringForURL([NotNull]string inputString)
        {
            CodeContracts.VerifyNotNull(inputString, nameof(inputString));

            var sb = new StringBuilder();

            // fix ampersand...
            inputString = inputString.Replace(" & ", "and").Replace("ـ", string.Empty);

            // trim...
            inputString = Config.UrlRewritingMode == "Unicode"
                      ? HttpUtility.UrlDecode(inputString.Trim())
                      : HttpContext.Current.Server.HtmlDecode(inputString.Trim());

            inputString = Regex.Replace(inputString, @"\p{Cs}", string.Empty);

            // normalize the Unicode
            inputString = inputString.Normalize(NormalizationForm.FormD);

            switch (Config.UrlRewritingMode)
            {
                case "Unicode":
                    {
                        inputString.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar))
                                    {
                                        sb.Append(currentChar);
                                    }
                                });
                        
                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : HttpUtility.UrlEncode(strNew);
                    }

                case "Translit":
                    {
                        string uniDecode;

                        try
                        {
                            uniDecode = inputString.Unidecode().Replace(" ", "-");
                        }
                        catch (Exception)
                        {
                            uniDecode = inputString;
                        }

                        uniDecode.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar))
                                    {
                                        sb.Append(currentChar);
                                    }
                                });

                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }

                default:
                    {
                        inputString.ForEach(
                            currentChar =>
                                {
                                    if (char.IsWhiteSpace(currentChar) || char.IsPunctuation(currentChar))
                                    {
                                        sb.Append('-');
                                    }
                                    else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark
                                             && !char.IsSymbol(currentChar) && currentChar < 128)
                                    {
                                        sb.Append(currentChar);
                                    }
                                });

                        var strNew = sb.ToString();

                        while (strNew.EndsWith("-"))
                        {
                            strNew = strNew.Remove(strNew.Length - 1, 1);
                        }

                        return strNew.Length.Equals(0) ? "Default" : strNew;
                    }
            }
        }

        #endregion 
    }
}