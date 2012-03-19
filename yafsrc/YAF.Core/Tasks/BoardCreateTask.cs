
// vzrus
namespace YAF.Core.Tasks
{
    #region Using

    using System;

    using YAF.Types.Constants;
    using YAF.Classes.Data;
    using YAF.Types.Interfaces;
    using YAF.Utils; 

    #endregion
    
    /// <summary>
    ///  Run when we want to do migration of users in the background...
    ///  </summary>
  
    public class BoardCreateTask : LongBackgroundTask, ICriticalBackgroundTask, IBlockableTask
    {
        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _taskName = "BoardCreateTask";
        
        /// <summary>
        ///  Gets TaskName.
        ///  </summary>
      
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
        private static readonly string[] _blockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

      /// <summary>
      /// Gets Blocking Task Names.
      /// </summary>
      public static string[] BlockingTaskNames
      {
          get
          {
              return _blockingTaskNames;
          }
      }

      private static long _boardOut;
      /// <summary>
      /// Gets or sets ForumOut.
      /// </summary>
      public static long BoardOut
      {
          get
          {
              return _boardOut;
          }

          set
          {
              _boardOut = value;
          }
      }

      /// <summary>
      /// The _adminUserName.
      /// </summary>
      private object _adminUserName;

      /// <summary>
      /// Gets or sets AdminUserName.
      /// </summary>
      public object AdminUserName
      {
          get
          {
              return this._adminUserName;
          }

          set
          {
              this._adminUserName = value;
          }
      }

      /// <summary>
      /// The _adminUserEmail.
      /// </summary>
      private object _adminUserEmail;

      /// <summary>
      /// Gets or sets AdminUserEmail.
      /// </summary>
      public object AdminUserEmail
      {
          get
          {
              return this._adminUserEmail;
          }

          set
          {
              this._adminUserEmail = value;
          }
      }

      /// <summary>
      /// The _adminUserProviderUserKey.
      /// </summary>
      private object _adminUserProviderUserKey;

      /// <summary>
      /// Gets or sets AdminUserProviderUserKey.
      /// </summary>
      public object AdminUserProviderUserKey
      {
          get
          {
              return this._adminUserProviderUserKey;
          }

          set
          {
              this._adminUserProviderUserKey = value;
          }
      }

      private object _boardName;

      /// <summary>
      /// Gets or sets Board Name.
      /// </summary>
      public object BoardName
      {
          get
          {
              return this._boardName;
          }

          set
          {
              this._boardName = value;
          }
      }

      private object _culture;

      /// <summary>
      /// Gets or sets Culture.
      /// </summary>
      public object Culture
      {
          get
          {
              return this._culture;
;
          }

          set
          {
              this._culture = value;
          }
      }

      private object _languageFileName;

      /// <summary>
      /// Gets or sets LanguageFileName.
      /// </summary>
      public object LanguageFileName
      {
          get
          {
              return this._languageFileName;
              ;
          }

          set
          {
              this._languageFileName = value;
          }
      }

      private object _boardMembershipAppName;

      /// <summary>
      /// Gets or sets BoardMembershipAppName.
      /// </summary>
      public object BoardMembershipAppName
      {
          get
          {
              return this._boardMembershipAppName;
              ;
          }

          set
          {
              this._boardMembershipAppName = value;
          }
      }

      private object _boardRolesAppName;

      /// <summary>
      /// Gets or sets BoardRolesAppName.
      /// </summary>
      public object BoardRolesAppName
      {
          get
          {
              return this._boardRolesAppName;
              ;
          }

          set
          {
              this._boardRolesAppName = value;
          }
      }

      private object _rolePrefix;

      /// <summary>
      /// Gets or sets RolePrefix.
      /// </summary>
      public object RolePrefix
      {
          get
          {
              return this._rolePrefix;
              ;
          }

          set
          {
              this._rolePrefix = value;
          }
      }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardCreateTask"/> class.
    /// </summary>
    public BoardCreateTask()
    {
    }

      /// <summary>
      /// The start.
      /// </summary>
      /// <returns>
      /// The start.
      /// </returns>
    public static long Start(
        object adminUserName, 
        object admiUserEmail, 
        object adminUserProviderUserkey, 
        object boardName, 
        object culture, 
        object languageFileName,
        object boardMembershipAppName,
        object boardRolesAppName, 
        object rolePrefix, 
        out string failureMessage)
      {

      failureMessage = String.Empty;
      if (YafContext.Current.Get<ITaskModuleManager>() == null)
      {
        return 0;
      }
      
      if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
      {
          YafContext.Current.Get<ITaskModuleManager>().StartTask(
              TaskName, () => new BoardCreateTask
                                  {
                                      AdminUserName = adminUserName,
                                      AdminUserEmail = admiUserEmail,
                                      AdminUserProviderUserKey = adminUserProviderUserkey,
                                      BoardName = boardName,
                                      Culture = culture,
                                      LanguageFileName = languageFileName,
                                      BoardMembershipAppName = boardMembershipAppName,
                                      BoardRolesAppName = boardRolesAppName,
                                      RolePrefix = rolePrefix
                                  });
      }
      else
      {
          failureMessage = "You can't delete forum while blocking {0} tasks are running.".FormatWith(BlockingTaskNames.ToDelimitedString(","));
          _boardOut = -1;
      }
      return BoardOut;
    }

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
        try
        {
            this.Logger.Info(
                "Starting  Board Add Task for board {0}.", _boardOut);
                // Create Board
                _boardOut = LegacyDb.board_create(
                    AdminUserName,
                    AdminUserEmail,
                    AdminUserProviderUserKey,
                    BoardName,
                    Culture,
                    LanguageFileName,
                    BoardMembershipAppName,
                    BoardRolesAppName,
                    RolePrefix);
            this.Logger.Info("Board Add Task for board {0} completed.", _boardOut);
        }
        catch (Exception x)
        {
            _boardOut = -1;
        }
    }
  }
}