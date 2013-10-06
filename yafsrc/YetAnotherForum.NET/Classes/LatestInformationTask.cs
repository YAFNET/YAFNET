/* YetAnotherForum.NET
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
namespace YAF.Classes
{
  #region Using

  using System;
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Core.Tasks;
  using YAF.RegisterV2;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Gets the latest information from YAF headquarters
  /// </summary>
  [ExportService(ServiceLifetimeScope.Singleton)]
  public class LatestInformationTask : LongBackgroundTask, IStartTasks
  {
    #region Constants and Fields

    /// <summary>
    ///   The _task name.
    /// </summary>
    private const string _taskName = "LatestInformationTask";

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Culture.
    /// </summary>
    public string Culture { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      try
      {
        using (var reg = new RegisterV2())
        {
          reg.Timeout = 30000;

          // load the latest info -- but only provide the current version information and the user's two-letter language information. Nothing trackable.))
          var latestInfo = reg.LatestInfo(YafForumInfo.AppVersionCode, this.Culture);

          if (latestInfo != null)
          {
              this.Get<HttpApplicationStateBase>().Set("YafRegistrationLatestInformation", latestInfo);
          }
        }
      }
      catch (Exception x)
      {
#if DEBUG
          this.Logger.Error(x, "Exception In {0}", _taskName);
#endif
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IStartTasks

    /// <summary>
    /// Start various tasks
    /// </summary>
    /// <param name="manager">
    /// </param>
    public void Start([NotNull] ITaskModuleManager manager)
    {
      CodeContracts.VerifyNotNull(manager, "manager");

      this.Culture = "US";

      manager.StartTask(_taskName, () => this);
    }

    #endregion

    #endregion
  }
}