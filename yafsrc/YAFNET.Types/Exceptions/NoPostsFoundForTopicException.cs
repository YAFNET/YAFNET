/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Types.Exceptions;

/// <summary>
/// No Posts Found For Topic Exception
/// </summary>
public class NoPostsFoundForTopicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoPostsFoundForTopicException"/> class.
    /// </summary>
    /// <param name="topicId">
    /// The topic identifier.
    /// </param>
    /// <param name="userId">
    /// The current user identifier.
    /// </param>
    /// <param name="updateViewCount">
    /// The update view count.
    /// </param>
    /// <param name="showDeleted">
    /// if set to <c>true</c> [show deleted].
    /// </param>
    /// <param name="sincePostedDate">
    /// The since posted date.
    /// </param>
    /// <param name="toPostedDate">
    /// To posted date.
    /// </param>
    /// <param name="pageIndex">
    /// Index of the page.
    /// </param>
    /// <param name="pageSize">
    /// Size of the page.
    /// </param>
    /// <param name="messagePosition">
    /// The message position.
    /// </param>
    public NoPostsFoundForTopicException(
        int topicId,
        int userId,
        bool updateViewCount,
        bool showDeleted,
        DateTime sincePostedDate,
        DateTime toPostedDate,
        int pageIndex,
        int pageSize,
        int messagePosition)
        : base(
            $"""
             No posts were found for topic [
                                         topicId:{topicId}, userId:{userId}, updateViewCount:{updateViewCount},
                                         showDeleted:{showDeleted}, sincePostedDate:{sincePostedDate}, toPostedDate:{toPostedDate},
                                         pageIndex:{pageIndex}, pageSize:{pageSize}, messagePosition:{messagePosition}]
             """)
    {
    }
}