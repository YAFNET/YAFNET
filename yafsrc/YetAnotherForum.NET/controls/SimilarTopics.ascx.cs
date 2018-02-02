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
    using System.Linq;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Similar Topics Control
    /// </summary>
    public partial class SimilarTopics : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets TopicId
        /// </summary>
        public string Topic { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<YafBoardSettings>().ShowSimilarTopics)
            {
                this.SimilarTopicsHolder.Visible = false;
                return;
            }
            
            this.BindData();
        }

        /// <summary>
        /// Binds the similar topics list.
        /// </summary>
        private void BindData()
        {
            var topicsList = this.GetRepository<Topic>().GetSimilarTopics(this.PageContext.PageUserID, this.Topic)
                .Take(5);

            if (!topicsList.Any())
            {
                this.SimilarTopicsHolder.Visible = false;
                return;
            }

            this.Topics.DataSource = topicsList;
            this.Topics.DataBind();

            this.DataBind();
        }

        #endregion
    }
}