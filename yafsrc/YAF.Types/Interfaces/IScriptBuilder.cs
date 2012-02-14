﻿/* Yet Another Forum.NET
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

namespace YAF.Types.Interfaces
{
  #region Using

  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// The i script builder.
  /// </summary>
  public interface IScriptBuilder
  {
    #region Properties

    /// <summary>
    ///   Gets the ScriptSelector.
    /// </summary>
    string ScriptSelector { get; }

    /// <summary>
    ///   Gets Statements.
    /// </summary>
    IList<IScriptStatement> Statements { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The build.
    /// </summary>
    /// <returns>
    /// The build.
    /// </returns>
    string Build();

    #endregion
  }
}