/* YetAnotherForum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
namespace YAF.Core.Services
{
  #region Using

  using System.Web;

  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The validate task module running startup.
  /// </summary>
  public class ValidateTaskModuleRunningStartup : BaseStartupService, ICriticalStartupService
  {
    #region Properties

    /// <summary>
    /// Gets Priority.
    /// </summary>
    public override int Priority
    {
      get
      {
        return 100;
      }
    }

    /// <summary>
    ///   Gets InitVarName.
    /// </summary>
    [NotNull]
    protected override string InitVarName
    {
      get
      {
        return "TaskModuleRunning";
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The run service.
    /// </returns>
    /// <exception cref="YafTaskModuleNotRegisteredException">
    /// </exception>
    protected override bool RunService()
    {
      bool debugging = false;

#if DEBUG
      debugging = true;
#endif
      if (YafTaskModule.Current == null)
      {
        if (!debugging)
        {
          // YAF is not setup properly...
          YafContext.Current.Get<HttpSessionStateBase>()["StartupException"] =
            @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.";

          // go immediately to the error page.
          YafContext.Current.Get<HttpResponseBase>().Redirect(YafForumInfo.ForumClientFileRoot + "error.aspx");
        }
        else
        {
          throw new YafTaskModuleNotRegisteredException(
            @"YAF.NET is not setup properly. Please add the <add name=""YafTaskModule"" type=""YAF.Core.YafTaskModule, YAF.Core"" /> to the <modules> section of your web.config file.");
        }
      }

      return true;
    }

    #endregion
  }
}