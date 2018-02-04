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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Control displaying list of letters and/or characters for filtering list of members.
    /// </summary>
    public class AlphaSort : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets actually selected letter.
        /// </summary>
        public char CurrentLetter
        {
            get
            {
                var currentLetter = char.MinValue;

                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("letter") != null)
                {
                    // try to convert to char
                    char.TryParse(
                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("letter"),
                        out currentLetter);

                    // since we cannot use '#' in URL, we use '_' instead, this is to give it the right meaning
                    if (currentLetter == '_')
                    {
                        currentLetter = '#';
                    }
                }

                return currentLetter;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the Load event.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnLoad([NotNull] EventArgs e)
        {
            base.OnLoad(e);

            var buttonGroup = new HtmlGenericControl("div");
            buttonGroup.Attributes.Add(HtmlTextWriterAttribute.Class.ToString(), "btn-group mb-3");

            this.Controls.Add(buttonGroup);

            // get the localized character set
            var charSet = this.GetText("LANGUAGE", "CHARSET").Split('/');

            foreach (var t in charSet)
            {
                // get the current selected character (if there is one)
                var selectedLetter = this.CurrentLetter;

                // go through all letters in a set
                foreach (var letter in t)
                {
                    // create a link to this letter
                    var link = new HyperLink
                                   {
                                       ToolTip =
                                           this.GetTextFormatted("ALPHABET_FILTER_BY", letter.ToString()),
                                       Text = letter.ToString(),
                                       NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                                           ForumPages.members,
                                           "letter={0}",
                                           letter == '#' ? '_' : letter)
                                   };

                    if (selectedLetter != char.MinValue && selectedLetter == letter)
                    {
                        // current letter is selected, use specified style
                        link.CssClass = "btn btn-secondary active";
                    }
                    else
                    {
                        link.CssClass = "btn btn-secondary";
                    }

                    buttonGroup.Controls.Add(link);
                }
            }
        }

        #endregion
    }
}