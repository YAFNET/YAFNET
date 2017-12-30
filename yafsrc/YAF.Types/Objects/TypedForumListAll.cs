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
namespace YAF.Types.Objects
{
  #region Using

  using System;
  using System.Data;

  #endregion

  /// <summary>
  /// The typed forum list all.
  /// </summary>
  [Serializable]
  public class TypedForumListAll
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedForumListAll"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedForumListAll([NotNull] DataRow row)
    {
      this.CategoryID = row.Field<int?>("CategoryID");
      this.Category = row.Field<string>("Category");
      this.ForumID = row.Field<int?>("ForumID");
      this.Forum = row.Field<string>("Forum");
      this.Indent = row.Field<int?>("Indent");
      this.ParentID = row.Field<int?>("ParentID");
      this.PollGroupID = row.Field<int?>("PollGroupID");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedForumListAll"/> class.
    /// </summary>
    /// <param name="categoryid">
    /// The categoryid.
    /// </param>
    /// <param name="category">
    /// The category.
    /// </param>
    /// <param name="forumid">
    /// The forumid.
    /// </param>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <param name="indent">
    /// The indent.
    /// </param>
    /// <param name="parentid">
    /// The parentid.
    /// </param>
    /// <param name="pollgroupid">
    /// The pollgroupid.
    /// </param>
    public TypedForumListAll(
      int? categoryid, [NotNull] string category, int? forumid, [NotNull] string forum, int? indent, int? parentid, int? pollgroupid)
    {
      this.CategoryID = categoryid;
      this.Category = category;
      this.ForumID = forumid;
      this.Forum = forum;
      this.Indent = indent;
      this.ParentID = parentid;
      this.PollGroupID = pollgroupid;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets CategoryID.
    /// </summary>
    public int? CategoryID { get; set; }

    /// <summary>
    /// Gets or sets Forum.
    /// </summary>
    public string Forum { get; set; }

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public int? ForumID { get; set; }

    /// <summary>
    /// Gets or sets Indent.
    /// </summary>
    public int? Indent { get; set; }

    /// <summary>
    /// Gets or sets ParentID.
    /// </summary>
    public int? ParentID { get; set; }

    /// <summary>
    /// Gets or sets PollGroupID.
    /// </summary>
    public int? PollGroupID { get; set; }

    #endregion
  }
}