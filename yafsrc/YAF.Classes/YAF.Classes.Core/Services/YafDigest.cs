namespace YAF.Classes.Core
{
  #region Using

  using System.Net;
  using System.Text.RegularExpressions;

  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;
  using YAF.Classes.Utils.Extensions;

  #endregion

  /// <summary>
  /// The yaf digest.
  /// </summary>
  public class YafDigest
  {
    #region Public Methods

    /// <summary>
    /// The get digest html.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// The get digest html.
    /// </returns>
    public string GetDigestHtml(int userId, int boardId)
    {
      var request = (HttpWebRequest)WebRequest.Create(this.GetDigestUrl(userId, boardId));

      string digestHtml = string.Empty;

      // set timeout to max 10 seconds
      request.Timeout = 10 * 1000;
      var response = request.GetResponse().ToClass<HttpWebResponse>();

      if (response.StatusCode == HttpStatusCode.OK)
      {
        digestHtml = response.GetResponseStream().AsString();
      }

      return digestHtml;
    }

    /// <summary>
    /// The get digest url.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// The get digest url.
    /// </returns>
    public string GetDigestUrl(int userId, int boardId)
    {
      return "{0}{1}{2}?{3}".FormatWith(
        BaseUrlBuilder.BaseUrl, 
        BaseUrlBuilder.AppPath, 
        "digest.aspx", 
        "token={0}&userid={1}&boardid={2}".FormatWith(YafContext.Current.BoardSettings.WebServiceToken, userId, boardId));
    }

    /// <summary>
    /// Sends the digest html to the email/name specified.
    /// </summary>
    /// <param name="digestHtml">
    /// The digest html.
    /// </param>
    /// <param name="forumName">
    /// The forum name.
    /// </param>
    /// <param name="toEmail">
    /// The to email.
    /// </param>
    /// <param name="toName">
    /// The to name.
    /// </param>
    /// <param name="sendQueued">
    /// The send queued.
    /// </param>
    public void SendDigest(
      [NotNull] string digestHtml, 
      [NotNull] string forumName, 
      [NotNull] string toEmail, 
      [CanBeNull] string toName, 
      bool sendQueued)
    {
      CodeContracts.ArgumentNotNull(digestHtml, "digestHtml");
      CodeContracts.ArgumentNotNull(forumName, "forumName");
      CodeContracts.ArgumentNotNull(toEmail, "toEmail");

      string subject = "Active Topics and New Topics on {0}".FormatWith(forumName);
      var match = Regex.Match(
        digestHtml, @"\<title\>(?<inner>(.*?))\<\/title\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

      if (match.Groups["inner"] != null)
      {
        subject = match.Groups["inner"].Value.Trim();
      }

      if (sendQueued)
      {
        // queue to send...
        YafContext.Current.Get<YafSendMail>().Queue(
          YafContext.Current.BoardSettings.ForumEmail, 
          YafContext.Current.BoardSettings.Name, 
          toEmail, 
          toName, 
          subject, 
          "You must have HTML Email Viewer to View.", 
          digestHtml);
      }
      else
      {
        // send direct...
        YafContext.Current.Get<YafSendMail>().Send(
          YafContext.Current.BoardSettings.ForumEmail, 
          YafContext.Current.BoardSettings.Name, 
          toEmail, 
          toName, 
          subject, 
          "You must have HTML Email Viewer to View.", 
          digestHtml);
      }
    }

    #endregion
  }
}