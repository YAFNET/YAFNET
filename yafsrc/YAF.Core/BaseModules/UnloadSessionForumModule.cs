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

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Core.Services;
  using YAF.Core.Services.Startup;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The unload session module.
  /// </summary>
  [YafModule("Unload Session Module", "Tiny Gecko", 1)]
  public class UnloadSessionForumModule : BaseForumModule
  {
    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether UnloadSession.
    /// </summary>
    public bool UnloadSession { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
      (this.ForumControlObj as Control).Unload += this.UnloadSessionModule_Unload;
    }

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
      if (!this.Get<StartupInitializeDb>().Initialized)
      {
        return;
      }

      if (YafContext.Current.BoardSettings.AbandonSessionsForDontTrack &&
          (YafContext.Current.Vars.AsBoolean("DontTrack") ?? false) && this.Get<HttpSessionStateBase>().IsNewSession)
      {
        // remove session
        this.Get<HttpSessionStateBase>().Abandon();
      }
    }

    #endregion
  }
}