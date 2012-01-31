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

namespace YAF.DotNetNuke
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Messages List
  /// </summary>
  public class Messages
  {
    #region Constants and Fields

    /// <summary>
      ///   Gets or sets Message Posted at
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    ///  Gets or sets The Message Id
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    ///  Gets or sets The Topic Id
    /// </summary>
    public int TopicId { get; set; }

    /// <summary>
    ///  Gets or sets The Complete Message of a Post
    /// </summary>
    public string Message { get; set; }

    #endregion
  }
}