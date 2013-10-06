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
namespace YAF.Core
{
  #region Using

  using System.Web;

  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The current task module provider.
  /// </summary>
  public class CurrentTaskModuleProvider : IReadWriteProvider<ITaskModuleManager>
  {
    #region Constants and Fields

    /// <summary>
    /// The _http application state.
    /// </summary>
    private readonly HttpApplicationStateBase _httpApplicationState;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentTaskModuleProvider"/> class.
    /// </summary>
    /// <param name="httpApplicationState">
    /// The http application state.
    /// </param>
    public CurrentTaskModuleProvider([NotNull] HttpApplicationStateBase httpApplicationState)
    {
      CodeContracts.VerifyNotNull(httpApplicationState, "httpApplicationState");

      this._httpApplicationState = httpApplicationState;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   The create.
    /// </summary>
    /// <returns>
    /// </returns>
    [CanBeNull]
    public ITaskModuleManager Instance
    {
      get
      {
        // Note: not treated with "BoardID" at all -- only one instance per application.
        return this._httpApplicationState[Constants.Cache.TaskModule] as ITaskModuleManager;
      }

      set
      {
        CodeContracts.VerifyNotNull(value, "value");

        this._httpApplicationState[Constants.Cache.TaskModule] = value;
      }
    }

    #endregion
  }
}