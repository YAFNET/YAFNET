/* Yet Another Forum.net
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
namespace YAF.Core.Tasks
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The mail sending module.
  /// </summary>
  [YafModule("Mail Queue Starting Module", "Tiny Gecko", 1)]
  public class MailSendingForumModule : BaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _key name.
    /// </summary>
    private const string _keyName = "MailSendTask";

    #endregion

    #region Public Methods

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
      // hook the page init for mail sending...
      YafContext.Current.AfterInit += this.Current_AfterInit;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The current_ after init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Current_AfterInit([NotNull] object sender, [NotNull] EventArgs e)
    {
      // add the mailing task if it's not already added...
      if (this.Get<ITaskModuleManager>() != null && !this.Get<ITaskModuleManager>().TaskExists(_keyName))
      {
        // start it...
        this.Get<ITaskModuleManager>().StartTask(_keyName, new MailSendTask());
      }
    }

    #endregion
  }
}