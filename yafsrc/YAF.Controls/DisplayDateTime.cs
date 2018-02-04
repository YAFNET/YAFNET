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
    using System.Globalization;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The display date time.
    /// </summary>
    public class DisplayDateTime : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _controlHtml.
        /// </summary>
        private readonly string controlHtml = @"<abbr class=""timeago"" title=""{1}"">{0}</abbr>";

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DateTime.
        /// </summary>
        public object DateTime
        {
            get
            {
                return this.ViewState["DateTime"];
            }

            set
            {
                this.ViewState["DateTime"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets Format.
        /// </summary>
        public DateTimeFormat Format
        {
            get
            {
                return this.ViewState["Format"]?.ToEnum<DateTimeFormat>() ?? DateTimeFormat.Both;
            }

            set
            {
                this.ViewState["Format"] = value;
            }
        }

        /// <summary>
        ///   Gets AsDateTime.
        /// </summary>
        protected DateTime AsDateTime
        {
            get
            {
                if (this.DateTime == null)
                {
                    return DateTimeHelper.SqlDbMinTime();
                }

                try
                {
                    return Convert.ToDateTime(this.DateTime);
                }
                catch (InvalidCastException)
                {
                    // not useable...            
                }

                return DateTimeHelper.SqlDbMinTime();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (!this.Visible || this.DateTime == null)
            {
                return;
            }

            writer.Write(
                this.controlHtml.FormatWith(
                    this.AsDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                    this.Get<IDateTime>().Format(this.Format, this.DateTime)));
            writer.WriteLine();
        }

        #endregion
    }
}
