/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

  #endregion

  /// <summary>
  /// The simple topic.
  /// </summary>
  [Serializable]
  public class SimpleTopic
  {
    #region Properties

    /// <summary>
    /// Gets or sets CreatedDate.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets FirstMessage.
    /// </summary>
    public string FirstMessage { get; set; }

    /// <summary>
    /// Gets or sets Forum.
    /// </summary>
    public SimpleForum Forum { get; set; }

    /// <summary>
    /// Gets or sets LastMessage.
    /// </summary>
    public string LastMessage { get; set; }

    /// <summary>
    /// Gets or sets LastMessageID.
    /// </summary>
    public int LastMessageID { get; set; }

    /// <summary>
    /// Gets or sets LastPostDate.
    /// </summary>
    public DateTime LastPostDate { get; set; }

    /// <summary>
    /// Gets or sets LastUserID.
    /// </summary>
    public int LastUserID { get; set; }

    /// <summary>
    /// Gets or sets LastUserName.
    /// </summary>
    public string LastUserName { get; set; }

    /// <summary>
    /// Gets or sets Replies.
    /// </summary>
    public int Replies { get; set; }

    /// <summary>
    /// Gets or sets StartedUserID.
    /// </summary>
    public int StartedUserID { get; set; }

    /// <summary>
    /// Gets or sets StartedUserName.
    /// </summary>
    public string StartedUserName { get; set; }

    /// <summary>
    /// Gets or sets Subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets TopicID.
    /// </summary>
    public int TopicID { get; set; }

    #endregion
  }
}