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
namespace YAF.Core.Tasks
{
    #region Using

    using System;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Update Search Index Rask
    /// </summary>
    /// <seealso cref="YAF.Core.BaseForumModule" />
    [YafModule("Update Search Index Task Forum Module", "Tiny Gecko", 1)]
    public class UpdateSearchIndexTaskForumModule : BaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init.
        /// </summary>
        public override void Init()
        {
            // hook the page init for mail sending...
            this.PageContext.AfterInit += this.CurrentAfterInit;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Currents the after initialize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentAfterInit([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<ITaskModuleManager>().StartTask(UpdateSearchIndexTask.TaskName, () => new UpdateSearchIndexTask());
        }

        #endregion
    }
}