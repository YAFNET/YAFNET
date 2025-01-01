/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Types.EventProxies;

using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The page load event.
/// </summary>
public class InitPageLoadEvent : IAmEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitPageLoadEvent"/> class.
    /// </summary>
    public InitPageLoadEvent()
    {
        this.UserRequestData = new UserRequestData();
        this.UserLazyData = new UserLazyData();
        this.PageQueryData = new PageQueryData();
    }

    /// <summary>
    /// Gets or sets the user request data.
    /// </summary>
    /// <value>The user request data.</value>
    public UserRequestData UserRequestData { get; set; }

    /// <summary>
    /// Gets or sets the page load data.
    /// </summary>
    /// <value>The page load data.</value>
    public Tuple<PageLoad, User, Category, Forum, Topic, Message> PageLoadData { get; set; }

    /// <summary>
    /// Gets or sets the page query data.
    /// </summary>
    /// <value>The page query data.</value>
    public PageQueryData PageQueryData { get; set; }

    /// <summary>
    /// Gets or sets the user lazy data.
    /// </summary>
    /// <value>The user lazy data.</value>
    public UserLazyData UserLazyData { get; set; }

    /// <summary>
    /// The page data.
    /// </summary>
    public Tuple<UserRequestData, Tuple<PageLoad, User, Category, Forum, Topic, Message>, UserLazyData, PageQueryData> PageData =>
        new(
            this.UserRequestData,
            this.PageLoadData,
            this.UserLazyData,
            this.PageQueryData);
}