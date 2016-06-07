/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Summary description for ForumUsers.
    /// </summary>
    public class ForumUsers : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _active users.
        /// </summary>
        private readonly ActiveUsers _activeUsers = new ActiveUsers();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForumUsers" /> class.
        /// </summary>
        public ForumUsers()
        {
            this._activeUsers.ID = this.GetUniqueID("ActiveUsers");
            this.Load += this.ForumUsers_Load;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether TreatGuestAsHidden.
        /// </summary>
        public bool TreatGuestAsHidden
        {
            get
            {
                return this._activeUsers.TreatGuestAsHidden;
            }

            set
            {
                this._activeUsers.TreatGuestAsHidden = value;
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
            // Ederon : 07/14/2007
            if (!this.Get<YafBoardSettings>().ShowBrowsingUsers)
            {
                return;
            }

            bool bTopic = this.PageContext.PageTopicID > 0;

            if (bTopic)
            {
                writer.WriteLine(@"<tr id=""{0}"" class=""header2"">".FormatWith(this.ClientID));
                writer.WriteLine(
                  "<td colspan=\"3\">{0}</td>".FormatWith(this.GetText("TOPICBROWSERS")));
                writer.WriteLine("</tr>");
                writer.WriteLine("<tr class=\"post\">");
                writer.WriteLine("<td colspan=\"3\">");
            }
            else
            {
                writer.WriteLine(@"<tr id=""{0}"" class=""header2"">".FormatWith(this.ClientID));
                writer.WriteLine("<td colspan=\"6\">{0}</td>".FormatWith(this.GetText("FORUMUSERS")));
                writer.WriteLine("</tr>");
                writer.WriteLine("<tr class=\"post\">");
                writer.WriteLine("<td colspan=\"6\">");
            }

            base.Render(writer);

            writer.WriteLine("</td>");
            writer.WriteLine("</tr>");
        }

        /// <summary>
        /// The forum users_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumUsers_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            bool inTopic = this.PageContext.PageTopicID > 0;

            if (this._activeUsers.ActiveUserTable == null)
            {
                bool useStyledNicks = this.Get<YafBoardSettings>().UseStyledNicks;

                this._activeUsers.ActiveUserTable =
                    inTopic
                        ? this.GetRepository<Active>().ListTopic(this.PageContext.PageTopicID, useStyledNicks)
                        : this.GetRepository<Active>().ListForum(this.PageContext.PageForumID, useStyledNicks);
            }

            // add it...
            this.Controls.Add(this._activeUsers);
        }

        #endregion
    }
}