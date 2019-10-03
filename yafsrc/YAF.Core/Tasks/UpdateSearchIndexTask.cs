/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Globalization;
    using System.Threading;

    using YAF.Configuration;
    using YAF.Core.Model;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The Update Search Index task.
    /// </summary>
    public class UpdateSearchIndexTask : LongBackgroundTask
    {
        #region Constants and Fields

        /// <summary>
        ///   The task name.
        /// </summary>
        private const string _TaskName = "UpdateSearchIndexTask";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UpdateSearchIndexTask" /> class.
        /// </summary>
        public UpdateSearchIndexTask()
        {
            // set interval values...
            this.RunPeriodMs = 3600000;
            this.StartDelayMs = 30000;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets TaskName.
        /// </summary>
        public static string TaskName => _TaskName;

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                Thread.BeginCriticalRegion();

                if (YafContext.Current == null)
                {
                    return;
                }

                if (!this.IsTimeToUpdateSearchIndex())
                {
                    return;
                }

                var messages = this.GetRepository<Message>().GetAllMessagesByBoard(YafContext.Current.PageBoardID);

                this.Get<ISearch>().AddSearchIndex(messages);
            }
            catch (Exception x)
            {
                if (!(x is ThreadAbortException))
                {
                    //this.Logger.Error(x, $"Error In {TaskName} Task");
                }
            }
            finally
            {
                this.Logger.Info($"search index updated");
                Thread.EndCriticalRegion();
            }
        }

        /// <summary>
        /// The is time to update search index.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsTimeToUpdateSearchIndex()
        {
            var boardSettings = YafContext.Current.Get<YafBoardSettings>();
            var lastSend = DateTime.MinValue;
            var sendEveryXHours = boardSettings.UpdateSearchIndexEveryXHours;

            if (boardSettings.LastSearchIndexUpdated.IsSet())
            {
                try
                {
                    lastSend = Convert.ToDateTime(
                        boardSettings.LastSearchIndexUpdated,
                        CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    lastSend = DateTime.MinValue;
                }
            }

            var updateIndex = lastSend < DateTime.Now.AddHours(-sendEveryXHours);

            if (!updateIndex)
            {
                return false;
            }

            this.GetRepository<Registry>().Save(
                "lastsearchindexupdated",
                DateTime.Now.ToString(CultureInfo.InvariantCulture));

            return true;
        }

        #endregion
    }
}