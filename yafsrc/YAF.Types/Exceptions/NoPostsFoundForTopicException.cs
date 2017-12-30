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

namespace YAF.Types.Exceptions
{
    using System;

    using YAF.Types.Extensions;

    /// <summary>
    /// No Posts Found For Topic Exception
    /// </summary>
    public class NoPostsFoundForTopicException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoPostsFoundForTopicException"/> class.
        /// </summary>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="currentUserID">The current user identifier.</param>
        /// <param name="authorUserID">The author user identifier.</param>
        /// <param name="updateViewCount">The update view count.</param>
        /// <param name="showDeleted">if set to <c>true</c> [show deleted].</param>
        /// <param name="styledNicks">if set to <c>true</c> [styled nicks].</param>
        /// <param name="showReputation">if set to <c>true</c> [show reputation].</param>
        /// <param name="sincePostedDate">The since posted date.</param>
        /// <param name="toPostedDate">To posted date.</param>
        /// <param name="sinceEditedDate">The since edited date.</param>
        /// <param name="toEditedDate">To edited date.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortPosted">The sort posted.</param>
        /// <param name="sortEdited">The sort edited.</param>
        /// <param name="sortPosition">The sort position.</param>
        /// <param name="showThanks">if set to <c>true</c> [show thanks].</param>
        /// <param name="messagePosition">The message position.</param>
        public NoPostsFoundForTopicException(
            int topicId,
            object currentUserID,
            [NotNull] object authorUserID,
            [NotNull] object updateViewCount,
            bool showDeleted,
            bool styledNicks,
            bool showReputation,
            DateTime sincePostedDate,
            DateTime toPostedDate,
            DateTime sinceEditedDate,
            DateTime toEditedDate,
            int pageIndex,
            int pageSize,
            int sortPosted,
            int sortEdited,
            int sortPosition,
            bool showThanks,
            int messagePosition)
            : base(
                "No posts were found for topic [topicId:{0}, currentUserID,:{1}, authorUserID:{2}, updateViewCount:{3}, showDeleted:{4}, styledNicks:{5}, showReputation:{6}, sincePostedDate:{7}, toPostedDate:{8}, sinceEditedDate:{9}, toEditedDate:{10}, pageIndex:{11}, pageSize:{12}, sortPosted:{13}, sortEdited:{14}, sortPosition:{15}, showThanks:{16}, messagePosition: {17}]"
                    .FormatWith(
                        topicId,
                        currentUserID,
                        authorUserID,
                        updateViewCount,
                        showDeleted,
                        styledNicks,
                        showReputation,
                        sincePostedDate,
                        toPostedDate,
                        sinceEditedDate,
                        toEditedDate,
                        pageIndex,
                        pageSize,
                        sortPosted,
                        sortEdited,
                        sortPosition,
                        showThanks,
                        messagePosition))
        {
        }
    }
}