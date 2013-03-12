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
namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The yaf task module not registered exception.
  /// </summary>
  public class YafTaskModuleNotRegisteredException : Exception
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafTaskModuleNotRegisteredException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public YafTaskModuleNotRegisteredException([NotNull] string message)
      : base(message)
    {
    }

    #endregion
  }
}