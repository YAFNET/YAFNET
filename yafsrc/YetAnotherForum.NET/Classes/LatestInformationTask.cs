/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
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