/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Classes.Core;
  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The unload session module.
  /// </summary>
  [YafModule("Unload Session Module", "Tiny Gecko", 1)]
  public class UnloadSessionModule : IBaseModule
  {
    #region Properties

    /// <summary>
    ///   Gets or sets ForumControlObj.
    /// </summary>
    public object ForumControlObj { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether UnloadSession.
    /// </summary>
    public bool UnloadSession { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IBaseModule

    /// <summary>
    /// The init.
    /// </summary>
    public void Init()
    {
      (this.ForumControlObj as Control).Unload += this.UnloadSessionModule_Unload;
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The unload session module_ unload.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void UnloadSessionModule_Unload([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (YafContext.Current.BoardSettings.AbandonSessionsForDontTrack && (YafContext.Current.Vars.AsBoolean("DontTrack") ?? false) && HttpContext.Current.Session.IsNewSession)
      {
        // remove session
        HttpContext.Current.Session.Abandon();
      }
    }

    #endregion
  }
}