/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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