namespace YAF.DotNetNuke
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// Messages List
  /// </summary>
  public class Messages
  {
    #region Constants and Fields

    /// <summary>
    ///   Message Posted at
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    ///   The Message Id
    /// </summary>
    public int MessageId { get; set; }

    /// <summary>
    ///   The Topic Id
    /// </summary>
    public int TopicId { get; set; }

    /// <summary>
    ///   The Complete Message of a Post
    /// </summary>
    public string Message { get; set; }

    #endregion
  }
}