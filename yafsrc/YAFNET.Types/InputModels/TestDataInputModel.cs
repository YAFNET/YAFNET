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

namespace YAF.Types.InputModels;

/// <summary>
/// The input model.
/// </summary>
public class TestDataInputModel
{
    /// <summary>
    /// Gets or sets the forum prefix tb.
    /// </summary>
    /// <value>The forum prefix tb.</value>
    public string ForumPrefixTB { get; set; } = "frm-";

    /// <summary>
    /// Gets or sets the topic prefix tb.
    /// </summary>
    /// <value>The topic prefix tb.</value>
    public string TopicPrefixTB { get; set; } = "topic-";

    /// <summary>
    /// Gets or sets the message content prefix tb.
    /// </summary>
    /// <value>The message content prefix tb.</value>
    public string MessageContentPrefixTB { get; set; } = "msg-";

    /// <summary>
    /// Gets or sets the category prefix tb.
    /// </summary>
    /// <value>The category prefix tb.</value>
    public string CategoryPrefixTB { get; set; } = "cat-";

    /// <summary>
    /// Gets or sets the board prefix tb.
    /// </summary>
    /// <value>The board prefix tb.</value>
    public string BoardPrefixTB { get; set; } = "brd-";

    /// <summary>
    /// Gets or sets the user prefix tb.
    /// </summary>
    /// <value>The user prefix tb.</value>
    public string UserPrefixTB { get; set; } = "usr-";

    /// <summary>
    /// Gets or sets the last tab identifier.
    /// </summary>
    /// <value>The last tab identifier.</value>
    public string LastTabId { get; set; } = "View1";

    /// <summary>
    /// Gets or sets the users number.
    /// </summary>
    /// <value>The users number.</value>
    public int UsersNumber { get; set; }

    /// <summary>
    /// Gets or sets the users boards list.
    /// </summary>
    /// <value>The users boards list.</value>
    public int UsersBoardsList { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>The password.</value>
    public string Password { get; set; } = "TestUser1234?";

    /// <summary>
    /// Gets or sets the password2.
    /// </summary>
    /// <value>The password2.</value>
    public string Password2 { get; set; } = "TestUser1234?";

    /// <summary>
    /// Gets or sets the question.
    /// </summary>
    /// <value>The question.</value>
    public string Question { get; set; }

    /// <summary>
    /// Gets or sets the answer.
    /// </summary>
    /// <value>The answer.</value>
    public string Answer { get; set; }

    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the home page.
    /// </summary>
    /// <value>The home page.</value>
    public string HomePage { get; set; }

    /// <summary>
    /// Gets or sets the board number.
    /// </summary>
    /// <value>The board number.</value>
    public int BoardNumber { get; set; }

    /// <summary>
    /// Gets or sets the boards users number.
    /// </summary>
    /// <value>The boards users number.</value>
    public int BoardsUsersNumber { get; set; }

    /// <summary>
    /// Gets or sets the categories number.
    /// </summary>
    /// <value>The categories number.</value>
    public int CategoriesNumber { get; set; }

    /// <summary>
    /// Gets or sets the categories boards list.
    /// </summary>
    /// <value>The categories boards list.</value>
    public int CategoriesBoardsList { get; set; }

    /// <summary>
    /// Gets or sets the categories forums number.
    /// </summary>
    /// <value>The categories forums number.</value>
    public int CategoriesForumsNumber { get; set; }

    /// <summary>
    /// Gets or sets the categories topics number.
    /// </summary>
    /// <value>The categories topics number.</value>
    public int CategoriesTopicsNumber { get; set; }

    /// <summary>
    /// Gets or sets the categories messages number.
    /// </summary>
    /// <value>The categories messages number.</value>
    public int CategoriesMessagesNumber { get; set; }

    /// <summary>
    /// Gets or sets the forums number.
    /// </summary>
    /// <value>The forums number.</value>
    public int ForumsNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [forums count messages].
    /// </summary>
    /// <value><c>true</c> if [forums count messages]; otherwise, <c>false</c>.</value>
    public bool ForumsCountMessages { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether [forums hide no access].
    /// </summary>
    /// <value><c>true</c> if [forums hide no access]; otherwise, <c>false</c>.</value>
    public bool ForumsHideNoAccess { get; set; } = true;

    /// <summary>
    /// Gets or sets the forums start mask.
    /// </summary>
    /// <value>The forums start mask.</value>
    public int ForumsStartMask { get; set; }

    /// <summary>
    /// Gets or sets the forums category.
    /// </summary>
    /// <value>The forums category.</value>
    public int ForumsCategory { get; set; }

    /// <summary>
    /// Gets or sets the forums parent.
    /// </summary>
    /// <value>The forums parent.</value>
    public int ForumsParent { get; set; }

    /// <summary>
    /// Gets or sets the forums topics number.
    /// </summary>
    /// <value>The forums topics number.</value>
    public int ForumsTopicsNumber { get; set; }

    /// <summary>
    /// Gets or sets the forums messages number.
    /// </summary>
    /// <value>The forums messages number.</value>
    public int ForumsMessagesNumber { get; set; }

    /// <summary>
    /// Gets or sets the topics number.
    /// </summary>
    /// <value>The topics number.</value>
    public int TopicsNumber { get; set; }

    /// <summary>
    /// Gets or sets the topics priority list.
    /// </summary>
    /// <value>The topics priority list.</value>
    public int TopicsPriorityList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [poll create].
    /// </summary>
    /// <value><c>true</c> if [poll create]; otherwise, <c>false</c>.</value>
    public bool PollCreate { get; set; }

    /// <summary>
    /// Gets or sets the topics category.
    /// </summary>
    /// <value>The topics category.</value>
    public int TopicsCategory { get; set; }

    /// <summary>
    /// Gets or sets the topics forum.
    /// </summary>
    /// <value>The topics forum.</value>
    public int TopicsForum { get; set; }

    /// <summary>
    /// Gets or sets the topics messages number.
    /// </summary>
    /// <value>The topics messages number.</value>
    public int TopicsMessagesNumber { get; set; }

    /// <summary>
    /// Gets or sets the posts number.
    /// </summary>
    /// <value>The posts number.</value>
    public int PostsNumber { get; set; }

    /// <summary>
    /// Gets or sets the posts category.
    /// </summary>
    /// <value>The posts category.</value>
    public int PostsCategory { get; set; }

    /// <summary>
    /// Gets or sets the posts forum.
    /// </summary>
    /// <value>The posts forum.</value>
    public int PostsForum { get; set; }

    /// <summary>
    /// Gets or sets the posts topic.
    /// </summary>
    /// <value>The posts topic.</value>
    public int PostsTopic { get; set; }

    /// <summary>
    /// Gets or sets my message.
    /// </summary>
    /// <value>My message.</value>
    public string MyMessage { get; set; }

    /// <summary>
    /// Gets or sets the p messages number.
    /// </summary>
    /// <value>The p messages number.</value>
    public int PMessagesNumber { get; set; }

    /// <summary>
    /// Gets or sets the p messages boards list.
    /// </summary>
    /// <value>The p messages boards list.</value>
    public int PMessagesBoardsList { get; set; }

    /// <summary>
    /// Gets or sets from.
    /// </summary>
    /// <value>From.</value>
    public string From { get; set; }

    /// <summary>
    /// Gets or sets to.
    /// </summary>
    /// <value>To.</value>
    public string To { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [p messages to all].
    /// </summary>
    /// <value><c>true</c> if [p messages to all]; otherwise, <c>false</c>.</value>
    public bool PMessagesToAll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [mark read].
    /// </summary>
    /// <value><c>true</c> if [mark read]; otherwise, <c>false</c>.</value>
    public bool MarkRead { get; set; } = true;

    /// <summary>
    /// Gets or sets the p message text.
    /// </summary>
    /// <value>The p message text.</value>
    public string PMessageText { get; set; }
}