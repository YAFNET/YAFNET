/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Core.BaseModules
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;

    using YAF.Core.Context;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The unload session module.
    /// </summary>
    [Module("Unload Session Module", "Tiny Gecko", 1)]
    public class UnloadSessionForumModule : BaseForumModule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether UnloadSession.
        /// </summary>
        public bool UnloadSession { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// The init.
        /// </summary>
        public override void Init()
        {
            ((Control)this.ForumControlObj).Unload += this.UnloadSessionModule_Unload;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The unload session module_ unload.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UnloadSessionModule_Unload([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<StartupInitializeDb>().Initialized)
            {
                return;
            }

            if (BoardContext.Current.BoardSettings.AbandonSessionsForDontTrack
                && (BoardContext.Current.Vars.AsBoolean("DontTrack") ?? false)
                && this.Get<HttpSessionStateBase>().IsNewSession)
            {
                // remove session
                this.Get<HttpSessionStateBase>().Abandon();
            }
        }

        #endregion
    }
}