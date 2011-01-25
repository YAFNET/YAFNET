/* YetAnotherForum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Classes
{
  #region Using

  using System;
  using System.Data;
  using System.Globalization;
  using System.Linq;
  using System.Text;
  using System.Web;
  using System.Web.Script.Services;
  using System.Web.Security;
  using System.Web.Services;

  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Class for JS jQuery  Ajax Methods
  /// </summary>
  [WebService(Namespace = "http://yetanotherforum.net/yafajax")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ScriptService]
  public class YafAjax : WebService, IHaveServiceLocator
  {
    #region Properties

    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator
    {
      get
      {
        return YafContext.Current.ServiceLocator;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add favorite topic.
    /// </summary>
    /// <param name="topicId">
    /// The topic ID.
    /// </param>
    /// <returns>
    /// The add favorite topic.
    /// </returns>
    [WebMethod(EnableSession = true)]
    public int AddFavoriteTopic(int topicId)
    {
      return this.Get<IFavoriteTopic>().AddFavoriteTopic(topicId);
    }

    /// <summary>
    /// Add Thanks to post
    /// </summary>
    /// <param name="msgID">
    /// The msg id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    [CanBeNull, WebMethod(EnableSession = true)]
    public ThankYouInfo AddThanks([NotNull] object msgID)
    {
      var messageID = msgID.ToType<int>();

      string username =
        LegacyDb.message_AddThanks(
          UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

      // if the user is empty, return a null object...
      return username.IsNotSet()
               ? null
               : this.CreateThankYou(username, "BUTTON_THANKSDELETE", "BUTTON_THANKSDELETE_TT", messageID);
    }

    /// <summary>
    /// The change album title.
    /// </summary>
    /// <param name="albumID">
    /// The album id.
    /// </param>
    /// <param name="newTitle">
    /// The New title.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    [WebMethod(EnableSession = true)]
    public YafAlbum.ReturnClass ChangeAlbumTitle(int albumID, [NotNull] string newTitle)
    {
      return YafAlbum.ChangeAlbumTitle(albumID, newTitle);
    }

    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="imageID">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    [WebMethod(EnableSession = true)]
    public YafAlbum.ReturnClass ChangeImageCaption(int imageID, [NotNull] string newCaption)
    {
      return YafAlbum.ChangeImageCaption(imageID, newCaption);
    }

    /// <summary>
    /// This method returns a string containing the HTML code for
    ///   showing the the post footer. the HTML content is the name of users
    ///   who thanked the post and the date they thanked.
    /// </summary>
    /// <param name="messageId">
    /// The msg ID.
    /// </param>
    /// <returns>
    /// The get thanks.
    /// </returns>
    [NotNull]
    public string GetThanks([NotNull] int messageId)
    {
      var filler = new StringBuilder();

      using (DataTable dt = LegacyDb.message_GetThanks(messageId))
      {
        foreach (DataRow dr in dt.Rows)
        {
          if (filler.Length > 0)
          {
            filler.Append(",&nbsp;");
          }

          // vzrus: quick fix for the incorrect link. URL rewriting don't work :(
          filler.AppendFormat(
            @"<a id=""{0}"" href=""{1}""><u>{2}</u></a>", 
            dr["UserID"], 
            YafBuildLink.GetLink(ForumPages.profile, "u={0}", dr["UserID"]), 
            dr["DisplayName"] != DBNull.Value
              ? this.Get<HttpServerUtilityBase>().HtmlEncode(dr["DisplayName"].ToString())
              : this.Get<HttpServerUtilityBase>().HtmlEncode(dr["Name"].ToString()));

          if (YafContext.Current.BoardSettings.ShowThanksDate)
          {
            filler.AppendFormat(
              @" {0}", 
              this.Get<ILocalization>().GetText("DEFAULT", "ONDATE").FormatWith(
                this.Get<IDateTime>().FormatDateShort(dr["ThanksDate"])));
          }
        }
      }

      return filler.ToString();
    }

    /// <summary>
    /// The refresh shout box.
    /// </summary>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="lastCheck">
    /// The last check.
    /// </param>
    /// <returns>
    /// The refresh shout box.
    /// </returns>
    [WebMethod]
    public int RefreshShoutBox(int boardId)
    {
      var messages = YafContext.Current.Cache.GetItem(
        YafCache.GetBoardCacheKey(Constants.Cache.Shoutbox + "_basic"),
        (double)1000,
        () => LegacyDb.shoutbox_getmessages(boardId, 1, false).AsEnumerable());

      var message = messages.FirstOrDefault();

      if (message != null)
      {
        return message.Field<int>("ShoutBoxMessageID");
      }

      return 0;
    }

    /// <summary>
    /// The remove favorite topic.
    /// </summary>
    /// <param name="topicId">
    /// The favorite topic id.
    /// </param>
    /// <returns>
    /// The remove favorite topic.
    /// </returns>
    [WebMethod(EnableSession = true)]
    public int RemoveFavoriteTopic(int topicId)
    {
      return this.Get<IFavoriteTopic>().RemoveFavoriteTopic(topicId);
    }

    /// <summary>
    /// This method is called asynchronously when the user clicks on "Remove Thank" button.
    /// </summary>
    /// <param name="msgID">
    /// Message Id
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    [NotNull, WebMethod(EnableSession = true)]
    public ThankYouInfo RemoveThanks([NotNull] object msgID)
    {
      var messageID = msgID.ToType<int>();

      string username =
        LegacyDb.message_RemoveThanks(
          UserMembershipHelper.GetUserIDFromProviderUserKey(Membership.GetUser().ProviderUserKey), messageID);

      return this.CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageID);
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method returns a string which shows how many times users have
    ///   thanked the message with the provided messageID. Returns an empty string.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="messageID">
    /// The Message ID.
    /// </param>
    /// <returns>
    /// The thanks number.
    /// </returns>
    protected string ThanksNumber([NotNull] string username, int messageID)
    {
      int thanksNumber = LegacyDb.message_ThanksNumber(messageID);

      // get the user's display name.
      string displayName =
        this.Get<IUserDisplayName>().GetName(
          UserMembershipHelper.GetUserIDFromProviderUserKey(
            UserMembershipHelper.GetMembershipUserByName(username).ProviderUserKey));

      // if displayname is enabled in admin section, and the user has a display name, use it instead of username.
      displayName = (displayName != string.Empty && YafContext.Current.BoardSettings.EnableDisplayName)
                      ? displayName
                      : username;

      switch (thanksNumber)
      {
        case 0:
          return String.Empty;
        case 1:
          return this.Get<ILocalization>().GetText("POSTS", "THANKSINFOSINGLE").FormatWith(displayName);
      }

      return this.Get<ILocalization>().GetText("POSTS", "THANKSINFO").FormatWith(thanksNumber, displayName);
    }

    /// <summary>
    /// Creates an instance of the thank you object from the current information.
    /// </summary>
    /// <param name="username">
    /// The Current Username
    /// </param>
    /// <param name="textTag">
    /// Button Text
    /// </param>
    /// <param name="titleTag">
    /// Button  Title
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    [NotNull]
    private ThankYouInfo CreateThankYou(
      [NotNull] string username, [NotNull] string textTag, [NotNull] string titleTag, int messageId)
    {
      return new ThankYouInfo
        {
          MessageID = messageId, 
          ThanksInfo = this.ThanksNumber(username, messageId), 
          Thanks = this.GetThanks(messageId), 
          Text = this.Get<ILocalization>().GetText("BUTTON", textTag), 
          Title = this.Get<ILocalization>().GetText("BUTTON", titleTag)
        };
    }

    #endregion
  }
}