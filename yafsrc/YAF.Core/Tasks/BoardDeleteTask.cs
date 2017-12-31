
// vzrus
namespace YAF.Core.Tasks
{
    #region Using

    using System;

    using YAF.Core.Extensions;
    using YAF.Types.Constants;
    using YAF.Classes.Data;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The forum delete task.
    /// </summary>
    public class BoardDeleteTask : LongBackgroundTask, ICriticalBackgroundTask
    {
        #region Constants and Fields

        /// <summary>
        /// The _task name.
        /// </summary>
        private const string _TaskName = "BoardDeleteTask";

        /// <summary>
        /// The Blocking Task Names.
        /// </summary>
        private static readonly string[] BlockingTaskNames = Constants.ForumRebuild.BlockingTaskNames;

        #endregion

        #region Properties

        /// <summary>
        /// Gets TaskName.
        /// </summary>
        public static string TaskName
        {
            get
            {
                return _TaskName;
            }
        }

        /// <summary>
        /// Gets or sets BoardIdToDelete.
        /// </summary>
        public int BoardIdToDelete { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the Board Delete Task
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="failureMessage"> 
        /// The failure message - is empty if task is launched successfully.
        /// </param>
        /// <returns>
        /// Returns if Task was Successfull
        /// </returns>
        public static bool Start(int boardId, out string failureMessage)
        {
            failureMessage = string.Empty;
            
            if (YafContext.Current.Get<ITaskModuleManager>() == null)
            {
                return false;
            }

            if (!YafContext.Current.Get<ITaskModuleManager>().AreTasksRunning(BlockingTaskNames))
            {
                YafContext.Current.Get<ITaskModuleManager>()
                    .StartTask(TaskName, () => new BoardDeleteTask { BoardIdToDelete = boardId });
            }
            else
            {
                failureMessage =
                    "You can't delete the board while some of the blocking {0} tasks are running.".FormatWith(
                        BlockingTaskNames.ToDelimitedString(","));

            }

            return true;
        }

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            try
            {
                this.Logger.Info("Starting Board delete task for BoardId {0} delete task.", this.BoardIdToDelete);

                this.GetRepository<Board>().DeleteById(this.BoardIdToDelete);
                this.Logger.Info("Board delete task for BoardId {0} delete task is completed.", this.BoardIdToDelete);
            }
            catch (Exception x)
            {
                this.Logger.Error(
                    x,
                    "Error In Board (ID: {0}) Delete Task: {1}".FormatWith(this.BoardIdToDelete),
                    TaskName);
            }
        }

        #endregion
    }
}