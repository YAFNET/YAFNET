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
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The current http application provider.
  /// </summary>
  public class CurrentHttpApplicationStateBaseProvider : IReadWriteProvider<HttpApplicationStateBase>
  {
    #region Constants and Fields

    /// <summary>
    /// The _application.
    /// </summary>
    protected HttpApplicationStateBase _application;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets the Instance.
    /// </summary>
    [CanBeNull]
    public HttpApplicationStateBase Instance
    {
      get
      {
        if (this._application == null && HttpContext.Current != null)
        {
          this._application = new HttpApplicationStateWrapper(HttpContext.Current.Application);
        }

        return this._application;
      }

      set
      {
        CodeContracts.VerifyNotNull(value, "value");

        this._application = value;
      }
    }

    #endregion
  }
}