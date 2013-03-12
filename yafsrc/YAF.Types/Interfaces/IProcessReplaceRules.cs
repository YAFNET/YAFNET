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
  /// Replace Rules Interface
  /// </summary>
  public interface IProcessReplaceRules
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether any rules have been added.
    /// </summary>
    bool HasRules { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add rule.
    /// </summary>
    /// <param name="newRule">
    /// The new rule.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    void AddRule([NotNull] IReplaceRule newRule);

    /// <summary>
    /// Process text using the rules.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    void Process([NotNull] ref string text);

    #endregion
  }
}