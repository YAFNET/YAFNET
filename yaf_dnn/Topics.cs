/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.DotNetNuke
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Topics List
  /// </summary>
  public class Topics
  {
    #region Constants and Fields

    /// <summary>
    ///  Gets or sets The Creation Date 
    ///   of the Topic
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    ///   Gets or sets The Forum Id
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    ///  Gets or sets  The Topic Id
    /// </summary>
    public int TopicId { get; set; }

    /// <summary>
    ///   Gets or sets The Topic Name
    /// </summary>
    public string TopicName { get; set; }

    #endregion
  }
}