/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core.Tasks
{
  #region Using

  using System;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Run when we want to do migration of users in the background...
  /// </summary>
  public class SyncMembershipUsersTask : LongBackgroundTask
  {
    #region Constants and Fields

    /// <summary>
    ///   The _task name.
    /// </summary>
    private const string _taskName = "SyncMembershipUsersTask";

    #endregion

    #region Properties

    /// <summary>
    ///   Gets TaskName.
    /// </summary>
    [NotNull]
    public static string TaskName
    {
      get
      {
        return _taskName;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// The start.
    /// </returns>
    public static bool Start(int boardId)
    {
      if (YafContext.Current.Get<ITaskModuleManager>() == null)
      {
        return false;
      }

      YafContext.Current.Get<ITaskModuleManager>().StartTask(TaskName, () => new SyncMembershipUsersTask { Data = boardId });

      return true;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      try
      {
        // attempt to run the sync code...
        RoleMembershipHelper.SyncAllMembershipUsers((int)this.Data);
      }
      catch (Exception x)
      {
          this.Logger.Error(x, "Error In {0} Task".FormatWith(TaskName));
      }
    }

    #endregion
  }
}