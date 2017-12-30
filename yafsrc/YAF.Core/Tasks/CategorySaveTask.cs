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

using YAF.Types.Constants;

namespace YAF.Core.Tasks
{
  using System;

  using YAF.Classes.Data;
  using YAF.Core.Model;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Models;
  using YAF.Utils;

  /// <summary>
  /// Run when we want to do migration of users in the background...
  /// </summary>
    public class CategorySaveTask : LongBackgroundTask, ICriticalBackgroundTask
  {

      /// <summary>
      /// The _board id.
      /// </summary>
      private object _boardIdToSave;

      /// <summary>
      /// Gets or sets BoardIdToSave.
      /// </summary>
      public object BoardIdToSave
      {
          get
          {
              return this._boardIdToSave;
          }

          set
          {
              this._boardIdToSave = value;
          }
      }

      /// <summary>
      /// The _category Id.
      /// </summary>
      private object _categoryId;

      /// <summary>
      /// Gets or sets CategoryId.
      /// </summary>
      public object CategoryId
      {
          get
          {
              return this._categoryId;
          }

          set
          {
              this._categoryId = value;
          }
      }

      private object _categoryName;

      /// <summary>
      /// Gets or sets CategoryName.
      /// </summary>
      public object CategoryName
      {
          get
          {
              return this._categoryName;
          }

          set
          {
              this._categoryName = value;
          }
      }

      private object _categoryImage;

      /// <summary>
      /// Gets or sets CategoryImage.
      /// </summary>
      public object CategoryImage
      {
          get
          {
              return this._categoryImage;
          }

          set
          {
              this._categoryImage = value;
          }
      }

      private object _sortOrder;

      /// <summary>
      /// Gets or sets SortOrder.
      /// </summary>
      public object SortOrder
      {
          get
          {
              return this._sortOrder;
;
          }

          set
          {
              this._sortOrder = value;
          }
      }

      private  static long _categoryOut;
      /// <summary>
      /// Gets or sets CategoryOut.
      /// </summary>
      public static long CategoryOut
      {
          get
          {
              return _categoryOut;
          }

          set
          {
              _categoryOut = value;
          }
      }

   

    /// <summary>
    /// The _task name.
    /// </summary>
    private const string _taskName = "CategorySaveTask";

    /// <summary>
    /// Gets TaskName.
    /// </summary>
    public static string TaskName
    {
        get
        {
            return _taskName;
        }
    }

    /// <summary>
    /// The Blocking Task Names.
    /// </summary>
    private static readonly string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;
    

    /// <summary>
    /// Initializes a new instance of the <see cref="CategorySaveTask"/> class.
    /// </summary>
    public CategorySaveTask()
    {
    }

      /// <summary>
      ///  The start.
      ///  </summary>
      /// <param name="boardId"> The board Id.</param>
      /// <param name="categoryId"> The category Id.</param>
      /// <param name="categoryName"> The category Name.</param>
      /// <param name="categoryImage"> The category Image.</param>
      /// <param name="sortOrder"> The category sort order.</param>
      /// <param name="failureMessage"> The failure message to return.</param>
      /// <returns>
      ///  The start.
      ///  </returns>
      public static void Start(object boardId, object categoryId, object categoryName, object categoryImage, object sortOrder, out string failureMessage)
      {

      failureMessage = String.Empty;
      if (YafContext.Current.Get<ITaskModuleManager>() == null)
      {
        return;
      }
     
      if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
      {
          YafContext.Current.Get<ITaskModuleManager>().StartTask(
              TaskName, () => new CategorySaveTask
                                  {
                                      BoardIdToSave = boardId,
                                      CategoryId = categoryId,
                                      CategoryName = categoryName,
                                      CategoryImage = categoryImage,
                                      SortOrder = sortOrder
                                  });
      }
      else
      {
          failureMessage = "You can't save the category while some of the blocking {0} tasks are running.".FormatWith(BlockingTaskNames.ToDelimitedString(","));
         
      }
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
        try
        {
            this.Logger.Info("Starting Category Save Task for CategoryID {0}.",this.CategoryId);
            this.GetRepository<Category>()
                .Save((int?)this.CategoryId, (string)this.CategoryName, (string)this.CategoryImage, (short)this.SortOrder, (int)this.BoardIdToSave);
            this.Logger.Info("Category Save Task for CategoryID {0} is completed.", CategoryId);
        }
        catch (Exception x)
        {
            this.Logger.Error(x, "Error In Category Save Task: {0}", x);
        }
    }
  }
}