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
    using System;

    using YAF.Classes.Data;
    using YAF.Types.Extensions;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    /// <summary>
    /// Run when we want to do migration of users in the background...
    /// </summary>
    public class ForumSaveTask : LongBackgroundTask, ICriticalBackgroundTask
    {
        /// <summary>
        /// Gets or sets ForumId.
        /// </summary>
        public object ForumId { get; set; }

        /// <summary>
        /// Gets or sets CategoryId.
        /// </summary>
        public object CategoryId { get; set; }

        /// <summary>
        /// Gets or sets ParentId.
        /// </summary>
        public object ParentId { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public object Description { get; set; }

        /// <summary>
        /// Gets or sets SortOrder.
        /// </summary>
        public object SortOrder { get; set; }

        /// <summary>
        /// Gets or sets Locked.
        /// </summary>
        public object Locked { get; set; }

        /// <summary>
        /// Gets or sets Hidden.
        /// </summary>
        public object Hidden { get; set; }

        /// <summary>
        /// Gets or sets IsTest.
        /// </summary>
        public object IsTest { get; set; }

        /// <summary>
        /// Gets or sets Moderated.
        /// </summary>
        public object Moderated { get; set; }

        /// <summary>
        /// Gets or sets the moderated post count.
        /// </summary>
        /// <value>
        /// The moderated post count.
        /// </value>
        public object ModeratedPostCount { get; set; }

        /// <summary>
        /// Gets or sets the is moderated new topic only.
        /// </summary>
        /// <value>
        /// The is moderated new topic only.
        /// </value>
        public object IsModeratedNewTopicOnly { get; set; }

        /// <summary>
        /// Gets or sets AccessMaskId.
        /// </summary>
        public object AccessMaskId { get; set; }

        /// <summary>
        /// Gets or sets RemoteURL.
        /// </summary>
        public object RemoteURL { get; set; }

        /// <summary>
        /// Gets or sets ThemeURL.
        /// </summary>
        public object ThemeURL { get; set; }

        /// <summary>
        /// Gets or sets ImageURL.
        /// </summary>
        public object ImageURL { get; set; }

        /// <summary>
        /// Gets or sets Styles.
        /// </summary>
        public object Styles { get; set; }

        /// <summary>
        /// Gets or sets ForumOut.
        /// </summary>
        public static long ForumOut { get; set; }

        /// <summary>
        /// Gets or sets Dummy.
        /// </summary>
        public bool Dummy { get; set; }

        /// <summary>
        /// Gets TaskName.
        /// </summary>
        public static string TaskName { get; private set; }

        /// <summary>
        /// The Blocking Task Names.
        /// </summary>
        private static readonly string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;


        /// <summary>
        /// Initializes a new instance of the <see cref="ForumSaveTask"/> class.
        /// </summary>
        static ForumSaveTask()
        {
            TaskName = "ForumSaveTask";
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="forumId">The forum Id.</param>
        /// <param name="categoryId">The category Id.</param>
        /// <param name="parentId">The parent Id.</param>
        /// <param name="name">The forum name.</param>
        /// <param name="description">The description.</param>
        /// <param name="sortOrder">The sort Order.</param>
        /// <param name="locked">The locked.</param>
        /// <param name="hidden">The hidden.</param>
        /// <param name="isTest">The is test.</param>
        /// <param name="moderated">The moderated.</param>
        /// <param name="moderatedPostCount">The moderated post count.</param>
        /// <param name="isModeratedNewTopicOnly">The is moderated new topic only.</param>
        /// <param name="accessMaskID">The access mask identifier.</param>
        /// <param name="remoteURL">The remote URL.</param>
        /// <param name="themeURL">The theme URL.</param>
        /// <param name="imageURL">The image URL.</param>
        /// <param name="styles">The styles.</param>
        /// <param name="dummy">if set to <c>true</c> [dummy].</param>
        /// <param name="failureMessage">The failure message.</param>
        /// <returns>
        /// The start.
        /// </returns>
        public static long Start(
            object forumId,
            object categoryId,
            object parentId,
            object name,
            object description,
            object sortOrder,
            object locked,
            object hidden,
            object isTest,
            object moderated,
            object moderatedPostCount,
            object isModeratedNewTopicOnly,
            object accessMaskID,
            object remoteURL,
            object themeURL,
            object imageURL,
            object styles,
            bool dummy,
            out string failureMessage)
        {

            failureMessage = string.Empty;
            if (YafContext.Current.Get<ITaskModuleManager>() == null)
            {
                return 0;
            }

            ////long newForumId = forumId.ToType<int>();

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
            {
                YafContext.Current.Get<ITaskModuleManager>()
                    .StartTask(
                        TaskName,
                        () =>
                        new ForumSaveTask
                            {
                                ForumId = forumId,
                                CategoryId = categoryId,
                                ParentId = parentId,
                                Name = name,
                                Description = description,
                                SortOrder = sortOrder,
                                Locked = locked,
                                Hidden = hidden,
                                IsTest = isTest,
                                Moderated = moderated,
                                ModeratedPostCount = moderatedPostCount,
                                IsModeratedNewTopicOnly = isModeratedNewTopicOnly,
                                AccessMaskId = accessMaskID,
                                RemoteURL = remoteURL,
                                ThemeURL = themeURL,
                                ImageURL = imageURL,
                                Styles = styles,
                                Dummy = dummy
                            });
            }
            else
            {
                failureMessage =
                    "You can't delete forum while blocking {0} tasks are running.".FormatWith(
                        BlockingTaskNames.ToDelimitedString(","));
                ForumOut = -1;
            }
            return ForumOut;
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {

            try
            {
                this.Logger.Info(
                    "Starting Forum Update||Add Task for ForumID {0}, {1} CategoryID, ParentID {2}.",
                    this.ForumId,
                    this.CategoryId,
                    this.ParentId);

                ForumOut = LegacyDb.forum_save(
                    this.ForumId,
                    this.CategoryId,
                    this.ParentId,
                    this.Name,
                    this.Description,
                    this.SortOrder,
                    this.Locked,
                    this.Hidden,
                    this.IsTest,
                    this.Moderated,
                    this.ModeratedPostCount,
                    this.IsModeratedNewTopicOnly,
                    this.AccessMaskId,
                    this.RemoteURL,
                    this.ThemeURL,
                    this.ImageURL,
                    this.Styles,
                    this.Dummy);
                this.Logger.Info("Forum Update||Add Task is completed. Handled forum {0}.", ForumOut);
            }
            catch (Exception x)
            {
                ForumOut = -1;
                this.Logger.Error(x, "Error In Forum Update||Add Task: {0}", x);
            }
        }
    }
}