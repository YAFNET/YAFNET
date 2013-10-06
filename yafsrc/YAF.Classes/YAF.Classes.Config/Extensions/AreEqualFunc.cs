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
namespace YAF.Utils
{
  #region Using

  using System;
  using System.Collections.Generic;

  using YAF.Types;

  #endregion

  /// <summary>
  /// The are equal func.
  /// </summary>
  /// <typeparam name="T">
  /// </typeparam>
  public class AreEqualFunc<T> : IEqualityComparer<T>
  {
    #region Constants and Fields

    /// <summary>
    /// The _comparer.
    /// </summary>
    private readonly Func<T, T, bool> _comparer;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AreEqualFunc{T}"/> class.
    /// </summary>
    /// <param name="comparer">
    /// The comparer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    public AreEqualFunc([NotNull] Func<T, T, bool> comparer)
    {
      CodeContracts.VerifyNotNull(comparer, "comparer");

      this._comparer = comparer;
    }

    #endregion

    #region Implemented Interfaces

    #region IEqualityComparer<T>

    /// <summary>
    /// The equals.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The equals.
    /// </returns>
    public bool Equals(T x, T y)
    {
      return this._comparer(x, y);
    }

    /// <summary>
    /// The get hash code.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <returns>
    /// The get hash code.
    /// </returns>
    public int GetHashCode(T obj)
    {
      return obj.ToString().ToLower().GetHashCode();
    }

    #endregion

    #endregion
  }
}