namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.UI;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The email_digest.
  /// </summary>
  public partial class emaildigest : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _combined user data.
    /// </summary>
    private CombinedUserDataHelper _combinedUserData;

    /// <summary>
    ///   The _forum data.
    /// </summary>
    private List<SimpleForum> _forumData;

    /// <summary>
    ///   The _language file.
    /// </summary>
    private string _languageFile;

    /// <summary>
    ///   The _theme.
    /// </summary>
    private YafTheme _theme;

    /// <summary>
    ///   Numbers of hours to compute digest for...
    /// </summary>
    private int _topicHours = -24;

    /// <summary>
    ///   The _yaf localization.
    /// </summary>
    private YafLocalization _yafLocalization;

    #endregion

    #region Properties

    /// <summary>
    /// Gets ActiveTopics.
    /// </summary>
    public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> ActiveTopics
    {
      get
      {
        // flatten...
        var topicsFlattened = this._forumData.SelectMany(x => x.Topics);

        return
          topicsFlattened.Where(
            t =>
            t.LastPostDate > DateTime.Now.AddHours(this._topicHours) &&
            t.CreatedDate < DateTime.Now.AddHours(this._topicHours)).GroupBy(x => x.Forum);
      }
    }

    /// <summary>
    ///   Gets or sets BoardID.
    /// </summary>
    public int BoardID { get; set; }

    /// <summary>
    ///   Gets or sets CurrentUserID.
    /// </summary>
    public int CurrentUserID { get; set; }

    /// <summary>
    ///   Gets NewTopics.
    /// </summary>
    public IEnumerable<IGrouping<SimpleForum, SimpleTopic>> NewTopics
    {
      get
      {
        // flatten...
        var topicsFlattened = this._forumData.SelectMany(x => x.Topics);

        return topicsFlattened.Where(t => t.CreatedDate > DateTime.Now.AddHours(this._topicHours)).GroupBy(x => x.Forum);
      }
    }

    /// <summary>
    ///   Gets UserData.
    /// </summary>
    public CombinedUserDataHelper UserData
    {
      get
      {
        if (this._combinedUserData == null)
        {
          this._combinedUserData = new CombinedUserDataHelper(this.CurrentUserID);
        }

        return this._combinedUserData;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The get text.
    /// </summary>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public string GetText(string tag)
    {
      if (this._languageFile.IsSet() && this._yafLocalization == null)
      {
        this._yafLocalization = new YafLocalization();
        this._yafLocalization.LoadTranslation(this._languageFile);
      }
      else if (this._yafLocalization == null)
      {
        this._yafLocalization = this.PageContext.Localization;
      }

      return this._yafLocalization.GetText("DIGEST", tag);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get message formatted and truncated.
    /// </summary>
    /// <param name="lastMessage">
    /// The last message.
    /// </param>
    /// <param name="maxlength">
    /// The maxlength.
    /// </param>
    /// <returns>
    /// The get message formatted and truncated.
    /// </returns>
    protected string GetMessageFormattedAndTruncated(string lastMessage, int maxlength)
    {
      return
        StringHelper.Truncate(
          StringHelper.RemoveMultipleWhitespace(
            BBCodeHelper.StripBBCode(HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(lastMessage)))), 
          maxlength);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      YafServices.InitializeDb.Run();

      var token = this.Request.QueryString.GetFirstOrDefault("token");

      if (token.IsNotSet() || !token.Equals(YafContext.Current.BoardSettings.WebServiceToken))
      {
        this.Response.End();
        return;
      }

      if (this.CurrentUserID == 0)
      {
        this.CurrentUserID = this.Request.QueryString.GetFirstOrDefault("UserID").ToType<int>();
      }

      if (this.BoardID == 0)
      {
        this.BoardID = this.Request.QueryString.GetFirstOrDefault("BoardID").ToType<int>();
      }

      this._forumData = YafServices.DBBroker.GetSimpleForumTopic(
        this.BoardID, this.CurrentUserID, DateTime.Now.AddHours(this._topicHours), 9999);

      if (!this.NewTopics.Any() && !this.ActiveTopics.Any())
      {
        this.Response.End();
        return;
      }

      this._languageFile = UserHelper.GetUserLanguageFile(this.CurrentUserID);
      this._theme = new YafTheme(UserHelper.GetUserThemeFile(this.CurrentUserID));

      string subject = this.GetText("SUBJECT").FormatWith(YafContext.Current.BoardSettings.Name);

      string digestHead = this._theme.GetItem("THEME", "DIGESTHEAD", null);

      if (digestHead.IsSet())
      {
        this.YafHead.Controls.Add(new LiteralControl(digestHead));
      }

      if (subject.IsSet())
      {
        this.YafHead.Title = subject;
      }
    }

    #endregion
  }
}