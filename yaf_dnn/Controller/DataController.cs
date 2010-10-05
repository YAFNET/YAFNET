namespace YAF.DotNetNuke.Controller
{
  #region Using

  using System.Data;
  using System.Data.SqlClient;

  using YAF.Classes.Data;

  #endregion

  /// <summary>
  /// Module Data Controller to Handle SQL Stuff
  /// </summary>
  public class DataController
  {
    #region Public Methods

    /// <summary>
    /// Get The Latest Post from SQL
    /// </summary>
    /// <param name="boardId">
    /// The Board Id of the Board
    /// </param>
    /// <param name="numOfPostsToRetrieve">
    /// How many post should been retrieved
    /// </param>
    /// <param name="userId">
    /// Current Users Id
    /// </param>
    /// <param name="useStyledNicks">
    /// </param>
    /// <param name="showNoCountPosts">
    /// </param>
    /// <returns>
    /// Data Table of Latest Posts
    /// </returns>
    public static DataTable TopicLatest(
      object boardId, object numOfPostsToRetrieve, object userId, bool useStyledNicks, bool showNoCountPosts)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_latest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
        cmd.Parameters.AddWithValue("UserID", userId);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion
  }
}