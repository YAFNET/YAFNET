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
  /// The typed topic simple list.
  /// </summary>
  [Serializable]
  public class TypedTopicSimpleList
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedTopicSimpleList"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedTopicSimpleList([NotNull] DataRow row)
    {
      this.TopicID = row.Field<int?>("TopicID");
      this.Topic = row.Field<string>("Topic");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedTopicSimpleList"/> class.
    /// </summary>
    /// <param name="topicid">
    /// The topicid.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    public TypedTopicSimpleList(int? topicid, [NotNull] string topic)
    {
      this.TopicID = topicid;
      this.Topic = topic;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Topic.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets TopicID.
    /// </summary>
    public int? TopicID { get; set; }

    #endregion
  }
}