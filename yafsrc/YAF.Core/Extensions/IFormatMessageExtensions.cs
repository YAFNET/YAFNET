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
namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i format message extensions.
  /// </summary>
  public static class IFormatMessageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="formatMessage">
    /// The format Message.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <returns>
    /// The formatted message.
    /// </returns>
    public static string FormatMessage([NotNull] this IFormatMessage formatMessage, [NotNull] string message, [NotNull] MessageFlags messageFlags)
    {
      return formatMessage.FormatMessage(message, messageFlags, false, DateTime.UtcNow);
    }

    /// <summary>
    /// The format message.
    /// </summary>
    /// <param name="formatMessage">
    /// The format Message.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="messageFlags">
    /// The message flags.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target blank override.
    /// </param>
    /// <returns>
    /// The formated message.
    /// </returns>
    public static string FormatMessage([NotNull] this IFormatMessage formatMessage, 
      [NotNull] string message, 
      [NotNull] MessageFlags messageFlags, 
      bool targetBlankOverride)
    {
      return formatMessage.FormatMessage(message, messageFlags, targetBlankOverride, DateTime.UtcNow);
    }

    #endregion
  }
}