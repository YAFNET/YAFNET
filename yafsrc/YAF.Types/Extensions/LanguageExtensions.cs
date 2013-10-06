/* Yet Another Forum.net
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
namespace YAF.Types.Extensions
{
  #region Using

    using System;

    using YAF.Types;

    #endregion

  /// <summary>
  /// The using extensions.
  /// </summary>
  public static class LanguageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The using.
    /// </summary>
    /// <param name="anyObj">
    /// The any obj.
    /// </param>
    /// <param name="makeUsing1">
    /// The make using 1.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    /// <typeparam name="TAny">
    /// </typeparam>
    /// <typeparam name="T1">
    /// </typeparam>
    public static void Using<TAny, T1>(this TAny anyObj, [NotNull] Func<T1> makeUsing1, [NotNull] Action<T1> action)
      where T1 : IDisposable
    {
      CodeContracts.VerifyNotNull(makeUsing1, "makeUsing1");
      CodeContracts.VerifyNotNull(action, "action");

      using (var item1 = makeUsing1())
      {
        action(item1);
      }
    }

    /// <summary>
    /// The using.
    /// </summary>
    /// <param name="anyObj">
    /// The any obj.
    /// </param>
    /// <param name="makeUsing1">
    /// The make using 1.
    /// </param>
    /// <param name="makeUsing2">
    /// The make using 2.
    /// </param>
    /// <param name="action">
    /// The action.
    /// </param>
    /// <typeparam name="TAny">
    /// </typeparam>
    /// <typeparam name="T1">
    /// </typeparam>
    /// <typeparam name="T2">
    /// </typeparam>
    public static void Using<TAny, T1, T2>(
      this TAny anyObj, [NotNull] Func<T1> makeUsing1, [NotNull] Func<T2> makeUsing2, [NotNull] Action<T1, T2> action)
      where T1 : IDisposable where T2 : IDisposable
    {
      CodeContracts.VerifyNotNull(makeUsing1, "makeUsing1");
      CodeContracts.VerifyNotNull(makeUsing2, "makeUsing2");
      CodeContracts.VerifyNotNull(action, "action");

      using (var item1 = makeUsing1())
      {
        using (var item2 = makeUsing2())
        {
          action(item1, item2);
        }
      }
    }

    #endregion
  }
}