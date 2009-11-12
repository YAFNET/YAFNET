namespace YAF.Classes.Data
{
  /// <summary>
  /// The moderator.
  /// </summary>
  public class Moderator
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Moderator"/> class.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="moderatorID">
    /// The moderator id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="isGroup">
    /// The is group.
    /// </param>
    public Moderator(long forumID, long moderatorID, string name, bool isGroup)
    {
      ForumID = forumID;
      ModeratorID = moderatorID;
      Name = name;
      IsGroup = isGroup;
    }

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public long ForumID
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets ModeratorID.
    /// </summary>
    public long ModeratorID
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsGroup.
    /// </summary>
    public bool IsGroup
    {
      get;
      set;
    }
  }
}