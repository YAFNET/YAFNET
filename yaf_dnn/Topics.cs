namespace YAF.DotNetNuke
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Topics List
  /// </summary>
  public class Topics
  {
    #region Constants and Fields

    /// <summary>
    ///   The Creation Date 
    ///   of the Topic
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    ///   The Forum Id
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    ///   The Topic Id
    /// </summary>
    public int TopicId { get; set; }

    /// <summary>
    ///   The Topic Name
    /// </summary>
    public string TopicName { get; set; }

    #endregion
  }
}