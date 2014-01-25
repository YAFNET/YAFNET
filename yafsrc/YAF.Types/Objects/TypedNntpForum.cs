namespace YAF.Types.Objects
{
  #region Using

  using System;
  using System.Data;

  #endregion

  /// <summary>
  /// The typed nntp forum.
  /// </summary>
  public class TypedNntpForum
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedNntpForum"/> class.
    /// </summary>
    /// <param name="row">
    /// The row.
    /// </param>
    public TypedNntpForum([NotNull] DataRow row)
    {
      this.Name = row.Field<string>("Name");
      this.Address = row.Field<string>("Address");
      this.Port = row.Field<int?>("Port");
      this.UserName = row.Field<string>("UserName");
      this.UserPass = row.Field<string>("UserPass");
      this.NntpServerID = row.Field<int?>("NntpServerID");
      this.NntpForumID = row.Field<int?>("NntpForumID");
      this.GroupName = row.Field<string>("GroupName");
      this.ForumID = row.Field<int?>("ForumID");
      this.LastMessageNo = row.Field<int?>("LastMessageNo");
      this.LastUpdate = row.Field<DateTime?>("LastUpdate");
      this.Active = row.Field<bool?>("Active");
      this.DateCutOff = row.Field<DateTime?>("DateCutOff");
      this.ForumName = row.Field<string>("ForumName");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypedNntpForum"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="address">
    /// The address.
    /// </param>
    /// <param name="port">
    /// The port.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="userpass">
    /// The userpass.
    /// </param>
    /// <param name="nntpserverid">
    /// The nntpserverid.
    /// </param>
    /// <param name="nntpforumid">
    /// The nntpforumid.
    /// </param>
    /// <param name="groupname">
    /// The groupname.
    /// </param>
    /// <param name="forumid">
    /// The forumid.
    /// </param>
    /// <param name="lastmessageno">
    /// The lastmessageno.
    /// </param>
    /// <param name="lastupdate">
    /// The lastupdate.
    /// </param>
    /// <param name="active">
    /// The active.
    /// </param>
    /// <param name="datecutoff">
    /// The datecutoff.
    /// </param>
    /// <param name="forumname">
    /// The forumname.
    /// </param>
    public TypedNntpForum([NotNull] string name, [NotNull] string address, 
                          int? port, [NotNull] string username, [NotNull] string userpass, 
                          int? nntpserverid, 
                          int? nntpforumid, [NotNull] string groupname, 
                          int? forumid, 
                          int? lastmessageno, 
                          DateTime? lastupdate, 
                          bool? active, 
                          DateTime? datecutoff, [NotNull] string forumname)
    {
      this.Name = name;
      this.Address = address;
      this.Port = port;
      this.UserName = username;
      this.UserPass = userpass;
      this.NntpServerID = nntpserverid;
      this.NntpForumID = nntpforumid;
      this.GroupName = groupname;
      this.ForumID = forumid;
      this.LastMessageNo = lastmessageno;
      this.LastUpdate = lastupdate;
      this.Active = active;
      this.DateCutOff = datecutoff;
      this.ForumName = forumname;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Active.
    /// </summary>
    public bool? Active { get; set; }

    /// <summary>
    /// Gets or sets Address.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets DateCutOff.
    /// </summary>
    public DateTime? DateCutOff { get; set; }

    /// <summary>
    /// Gets or sets ForumID.
    /// </summary>
    public int? ForumID { get; set; }

    /// <summary>
    /// Gets or sets ForumName.
    /// </summary>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets GroupName.
    /// </summary>
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets LastMessageNo.
    /// </summary>
    public int? LastMessageNo { get; set; }

    /// <summary>
    /// Gets or sets LastUpdate.
    /// </summary>
    public DateTime? LastUpdate { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets NntpForumID.
    /// </summary>
    public int? NntpForumID { get; set; }

    /// <summary>
    /// Gets or sets NntpServerID.
    /// </summary>
    public int? NntpServerID { get; set; }

    /// <summary>
    /// Gets or sets Port.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets UserName.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets UserPass.
    /// </summary>
    public string UserPass { get; set; }

    #endregion
  }
}