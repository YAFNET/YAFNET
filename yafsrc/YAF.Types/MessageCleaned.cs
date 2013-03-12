/* YetAnotherForum.NET
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
namespace YAF.Core.Services
{
  using System;
  using System.Collections.Generic;

  using YAF.Types;

  /// <summary>
  /// The message cleaned class (internal)
  /// </summary>
  [Serializable]
  public class MessageCleaned
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "MessageCleaned" /> class.
    /// </summary>
    public MessageCleaned()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageCleaned"/> class.
    /// </summary>
    /// <param name="messageTruncated">
    /// The message truncated.
    /// </param>
    /// <param name="messageKeywords">
    /// The message keywords.
    /// </param>
    public MessageCleaned([NotNull] string messageTruncated, [NotNull] List<string> messageKeywords)
    {
      this.MessageTruncated = messageTruncated;
      this.MessageKeywords = messageKeywords;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets MessageKeywords.
    /// </summary>
    public List<string> MessageKeywords { get; set; }

    /// <summary>
    ///   Gets or sets MessageTruncated.
    /// </summary>
    public string MessageTruncated { get; set; }

    #endregion
  }
}