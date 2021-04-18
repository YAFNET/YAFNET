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

namespace YAF.Core.BasePages
{
    using System.Globalization;
    using System.Threading;
    using System.Web.UI;

    /// <summary>
    /// The base page.
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        /// The initialize culture.
        /// </summary>
        protected override void InitializeCulture()
        {
            var language = "en-US";

            // Check if PostBack is caused by Language DropDownList.
            if (this.Request.Form["__EVENTTARGET"] != null && this.Request.Form["__EVENTTARGET"].Contains("Languages"))
            {
                // Set the Language.
                language = this.Request.Form[this.Request.Form["__EVENTTARGET"]];
                this.Session["language"] = language;

                SetLanguageUsingThread(language);
            }
            else
            {
                if (this.Session["language"] != null)
                {
                    language = this.Session["language"].ToString();
                }
                else
                {
                    // Detect User's Language.
                    if (this.Request.UserLanguages != null)
                    {
                        // Set the Language.
                        language = this.Request.UserLanguages[0];
                    }
                }
            }

            SetLanguageUsingThread(language);
        }

        /// <summary>
        /// The set language using thread.
        /// </summary>
        /// <param name="selectedLanguage">
        /// The selected language.
        /// </param>
        private static void SetLanguageUsingThread(string selectedLanguage)
        {
            var info = CultureInfo.CreateSpecificCulture("en-US");

            try
            {
                info = CultureInfo.CreateSpecificCulture(selectedLanguage);
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = info;
                Thread.CurrentThread.CurrentCulture = info;
            }
        }
    }
}
