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
namespace YAF.Types
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The event converter args.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class EventConverterArgs<T> : EventArgs
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EventConverterArgs{T}"/> class.
    /// </summary>
    /// <param name="eventData">
    /// The event data.
    /// </param>
    public EventConverterArgs(T eventData)
    {
      this.EventData = eventData;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets EventData.
    /// </summary>
    public T EventData { get; set; }

    #endregion
  }
}