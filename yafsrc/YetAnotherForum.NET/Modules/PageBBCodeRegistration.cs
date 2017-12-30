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
namespace YAF.Modules
{
  #region Using

    using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page BBCode Registration Module", "Tiny Gecko", 1)]
  public class PageBBCodeRegistration : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      switch (this.PageContext.ForumPageType)
      {
        case ForumPages.cp_message:
        case ForumPages.search:
        case ForumPages.lastposts:
        case ForumPages.posts:
        case ForumPages.profile:
          this.Get<IBBCode>().RegisterCustomBBCodePageElements(
            this.PageContext.CurrentForumPage.Page, this.PageContext.CurrentForumPage.GetType());
          break;
      }
    }

    #endregion
  }
}