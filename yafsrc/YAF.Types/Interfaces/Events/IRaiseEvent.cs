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
namespace YAF.Types.Interfaces
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The event raiser interface
  /// </summary>
  public interface IRaiseEvent
  {
    #region Public Methods

    /// <summary>
    /// The raise event.
    /// </summary>
    /// <param name="eventObject">
    /// The event object.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    void Raise<T>(T eventObject) where T : IAmEvent;

    /// <summary>
    /// Raise all events using try/catch block.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="eventObject">
    /// </param>
    /// <param name="logExceptionAction">
    /// </param>
    void RaiseIssolated<T>(T eventObject, [CanBeNull] Action<string, Exception> logExceptionAction) where T : IAmEvent;

    #endregion
  }
}