/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Collections;
  using System.Data;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Web.UI;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Classes.Pattern;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The user box.
  /// </summary>
  public class UserBox : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The current data row.
    /// </summary>
    private DataRow _row;

    /// <summary>
    ///   Instance of the style transformation class
    /// </summary>
    private IStyleTransform _styleTransforum;

    /// <summary>
    ///   The _user profile.
    /// </summary>
    private YafUserProfile _userProfile;

    /// <summary>
    ///   The _message flags.
    /// </summary>
    private MessageFlags messageFlags;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets DataRow.
    /// </summary>
    public DataRow DataRow
    {
      get
      {
        return this._row;
      }

      set
      {
        this._row = value;
        this.messageFlags = (this._row != null) ? new MessageFlags(this._row["Flags"]) : new MessageFlags(0);
      }
    }

    /// <summary>
    ///   Gets or sets PageCache.
    /// </summary>
    public Hashtable PageCache { get; set; }

    /// <summary>
    ///   Gets or sets CachedUserBox.
    /// </summary>
    [CanBeNull]
    protected string CachedUserBox
    {
      get
      {
        if (this.PageCache != null && this.DataRow != null)
        {
          // get cache for user boxes
          object cache = this.PageCache[Constants.Cache.UserBoxes];

          // is it hashtable?
          if (cache != null && cache is Hashtable)
          {
            // get only record for user who made message being
            cache = ((Hashtable)cache)[this.UserId];

            // return from cache if there is something there
            if (cache != null && cache.ToString() != string.Empty)
            {
              return cache.ToString();
            }
          }
        }

        return null;
      }

      set
      {
        if (this.PageCache != null && this.DataRow != null)
        {
          // get cache for user boxes
          object cache = this.PageCache[Constants.Cache.UserBoxes];

          // is it hashtable?
          if (cache != null && cache is Hashtable)
          {
            // save userbox for user of this id to cache
            ((Hashtable)cache)[this.UserId] = value;
          }
          else
          {
            // create new hashtable for userbox caching
            cache = new Hashtable();

            // save userbox of this user
            ((Hashtable)cache)[this.UserId] = value;

            // save cache
            this.PageCache[Constants.Cache.UserBoxes] = cache;
          }
        }
      }
    }

    /// <summary>
    /// Gets UserBoxRegex.
    /// </summary>
    protected IThreadSafeDictionary<string, Regex> UserBoxRegex
    {
      get
      {
        return this.Get<IDataCache>().GetOrSet(
          "UserBoxRegexDictionary", () => new ThreadSafeDictionary<string, Regex>());
      }
    }

    /// <summary>
    ///   Gets UserId.
    /// </summary>
    protected int UserId
    {
      get
      {
        if (this.DataRow != null)
        {
          return this.DataRow["UserID"].ToType<int>();
        }

        return 0;
      }
    }

    /// <summary>
    ///   Gets UserProfile.
    /// </summary>
    protected YafUserProfile UserProfile
    {
      get
      {
        if (this._userProfile == null)
        {
          // setup instance of the user profile...
          this._userProfile = YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID(this.UserId));
        }

        return this._userProfile;
      }
    }

    /// <summary>
    ///   Gets a value indicating whether PostDeleted.
    /// </summary>
    private bool PostDeleted
    {
      get
      {
        return this.messageFlags.IsDeleted;
      }
    }

    /// <summary>
    ///   Refines style string from other skins info
    /// </summary>
    private IStyleTransform TransformStyle
    {
      get
      {
        if (this._styleTransforum == null)
        {
          this._styleTransforum = this.Get<IStyleTransform>();
        }

        return this._styleTransforum;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create user box.
    /// </summary>
    /// <returns>
    /// User box control string to display on a page.
    /// </returns>
    [NotNull]
    protected string CreateUserBox()
    {
      string userBox = this.PageContext.BoardSettings.UserBox;

      // Get styles table for user 
      // this should be called once for groups and for rank for each user/post.
      DataTable roleRankStyleTable = this.Get<IDataCache>().GetOrSet(
        Constants.Cache.GroupRankStyles, 
        () => LegacyDb.group_rank_style(YafContext.Current.PageBoardID), 
        TimeSpan.FromMinutes(YafContext.Current.BoardSettings.ForumStatisticsCacheTimeout));

      // Avatar
      userBox = this.MatchUserBoxAvatar(userBox);

      // User Medals     
      userBox = this.MatchUserBoxMedals(userBox);

      // Rank Image
      userBox = this.MatchUserBoxRankImages(userBox);

      // Rank     
      userBox = this.MatchUserBoxRank(userBox, roleRankStyleTable);

      // Groups
      userBox = this.MatchUserBoxGroups(userBox, roleRankStyleTable);

      // ThanksFrom
      userBox = this.MatchUserBoxThanksFrom(userBox);

      // ThanksTo
      userBox = this.MatchUserBoxThanksTo(userBox);

      if (!this.PostDeleted)
      {
        // Ederon : 02/24/2007
        // Joined Date
        userBox = this.MatchUserBoxJoinedDate(userBox);

        // Posts
        userBox = this.MatchUserBoxPostCount(userBox);

        // Points
        userBox = this.MatchUserBoxPoints(userBox);

        // Gender
        userBox = this.MatchUserBoxGender(userBox);

        // Location
        userBox = this.MatchUserBoxLocation(userBox);
      }
      else
      {
        userBox = this.MatchUserBoxClearAll(userBox);
      }

      // vzrus: to remove empty dividers  
      return this.RemoveEmptyDividers(userBox);
    }

    /// <summary>
    /// The get regex.
    /// </summary>
    /// <param name="search">
    /// The search.
    /// </param>
    /// <returns>
    /// </returns>
    protected Regex GetRegex([NotNull] string search)
    {
      Regex thisRegex;

      if (!this.UserBoxRegex.TryGetValue(search, out thisRegex))
      {
        thisRegex = new Regex(search, RegexOptions.Compiled);
        this.UserBoxRegex.MergeSafe(search, thisRegex);
      }

      return thisRegex;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
      output.WriteLine(@"<div class=""yafUserBox"" id=""{0}"">".FormatWith(this.ClientID));

      string userBox = this.CachedUserBox;

      if (string.IsNullOrEmpty(userBox))
      {
        userBox = this.CreateUserBox();

        // cache...
        this.CachedUserBox = userBox;
      }

      // output the user box info...
      output.WriteLine(userBox);

      output.WriteLine("</div>");
    }

    /// <summary>
    /// The match user box avatar.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box avatar.
    /// </returns>
    [NotNull]
    private string MatchUserBoxAvatar([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.Avatar);

      if (!this.PostDeleted)
      {
        string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(this.UserId);

        if (avatarUrl.IsSet())
        {
          filler =
            this.PageContext.BoardSettings.UserBoxAvatar.FormatWith(
              @"<a href=""{1}"" title=""{2}""><img class=""avatarimage"" src=""{0}"" alt=""{2}"" /></a>".FormatWith(
                avatarUrl, 
                YafBuildLink.GetLinkNotEscaped(ForumPages.profile, "u={0}", this.UserId), 
                UserMembershipHelper.GetUserNameFromID(this.UserId)));
        }
      }

      // replaces template placeholder with actual avatar
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box clear all.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box clear all.
    /// </returns>
    [NotNull]
    private string MatchUserBoxClearAll([NotNull] string userBox)
    {
      string filler = string.Empty;

      var rx = this.GetRegex(Constants.UserBox.JoinDate);
      userBox = rx.Replace(userBox, filler);
      rx = this.GetRegex(Constants.UserBox.Posts);
      userBox = rx.Replace(userBox, filler);
      rx = this.GetRegex(Constants.UserBox.Points);
      userBox = rx.Replace(userBox, filler);
      rx = this.GetRegex(Constants.UserBox.Location);
      userBox = rx.Replace(userBox, filler);
      rx = this.GetRegex(Constants.UserBox.ThanksFrom);
      userBox = rx.Replace(userBox, filler);
      rx = this.GetRegex(Constants.UserBox.ThanksTo);
      userBox = rx.Replace(userBox, filler);

      // vzrus: to remove empty dividers  
      return this.RemoveEmptyDividers(userBox);
    }

    /// <summary>
    /// The match user box gender.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box gender.
    /// </returns>
    [NotNull]
    private string MatchUserBoxGender([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.Gender);
      int userGender = this.UserProfile.Gender;
      string imagePath = string.Empty;
      string imageAlt = string.Empty;

      if (this.PageContext.BoardSettings.AllowGenderInUserBox)
      {
        if (userGender > 0)
        {
          if (userGender == 1)
          {
            imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_MALE", null);
            imageAlt = this.GetText("USERGENDER_MAS");
          }
          else if (userGender == 2)
          {
            imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_FEMALE", null);
            imageAlt = this.GetText("USERGENDER_FEM");
          }

          filler =
            this.PageContext.BoardSettings.UserBoxGender.FormatWith(
              @"<a><img src=""{0}"" alt=""{1}"" title=""{1}"" /></a>".FormatWith(imagePath, imageAlt));
        }
      }

      // replaces template placeholder with actual image
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box groups.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <param name="roleStyleTable">
    /// The role Style Table.
    /// </param>
    /// <returns>
    /// The match user box groups.
    /// </returns>
    [NotNull]
    private string MatchUserBoxGroups([NotNull] string userBox, [NotNull] DataTable roleStyleTable)
    {
      const string StyledNick = @"<span class=""YafGroup_{0}"" style=""{1}"">{0}</span>";

      string filler = string.Empty;

      Regex rx = this.GetRegex(Constants.UserBox.Groups);

      if (this.PageContext.BoardSettings.ShowGroups)
      {
        var groupsText = new StringBuilder(500);

        bool bFirst = true;
        string roleStyle = null;

        foreach (string role in RoleMembershipHelper.GetRolesForUser(this.DataRow["UserName"].ToString()))
        {
          foreach (DataRow drow in roleStyleTable.Rows)
          {
            // Groups for a user have LegendID = 1
            if (Convert.ToInt32(drow["LegendID"]) == 1 && drow["Style"] != null && drow["Name"].ToString() == role)
            {
              roleStyle = this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true);
              break;
            }
          }

          if (bFirst)
          {
            if (YafContext.Current.BoardSettings.UseStyledNicks)
            {
              groupsText.AppendLine(StyledNick.FormatWith(role, roleStyle));
            }
            else
            {
              groupsText.AppendLine(role);
            }

            bFirst = false;
          }
          else
          {
            if (YafContext.Current.BoardSettings.UseStyledNicks)
            {
              groupsText.AppendLine((@", " + StyledNick).FormatWith(role, roleStyle));
            }
            else
            {
              groupsText.AppendFormat(", {0}", role);
            }
          }

          roleStyle = null;
        }

        filler = this.PageContext.BoardSettings.UserBoxGroups.FormatWith(
          this.GetText("groups"), groupsText);

        // mddubs : 02/21/2009
        // Remove the space before the first comma when multiple groups exist.
        filler = filler.Replace("\r\n,", ",");
      }

      // replaces template placeholder with actual groups
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box joined date.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box joined date.
    /// </returns>
    [NotNull]
    private string MatchUserBoxJoinedDate([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.JoinDate);

      if (this.PageContext.BoardSettings.DisplayJoinDate)
      {
        filler =
          this.PageContext.BoardSettings.UserBoxJoinDate.FormatWith(
            this.GetText("joined"), 
            this.Get<IDateTime>().FormatDateShort((DateTime)this.DataRow["Joined"]));
      }

      // replaces template placeholder with actual join date
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box location.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box location.
    /// </returns>
    [NotNull]
    private string MatchUserBoxLocation([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.Location);

      if (this.UserProfile.Location.IsSet())
      {
        filler =
          this.PageContext.BoardSettings.UserBoxLocation.FormatWith(
            this.GetText("location"), 
            this.Get<IFormatMessage>().RepairHtml(this.UserProfile.Location, false));
      }

      // replaces template placeholder with actual location
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box medals.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box medals.
    /// </returns>
    [NotNull]
    private string MatchUserBoxMedals([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.Medals);

      if (this.PageContext.BoardSettings.ShowMedals)
      {
        DataTable dt = this.Get<IDBBroker>().UserMedals(this.UserId);

        // vzrus: If user doesn't have we shouldn't render this waisting resources
        if (dt.Rows.Count <= 0)
        {
          return rx.Replace(userBox, filler);
        }

        var ribbonBar = new StringBuilder(500);
        var medals = new StringBuilder(500);

        DataRow r;
        MedalFlags f;

        int i = 0;
        int inRow = 0;

        // do ribbon bar first
        while (dt.Rows.Count > i)
        {
          r = dt.Rows[i];
          f = new MedalFlags(r["Flags"]);

          // do only ribbon bar items first
          if (!SqlDataLayerConverter.VerifyBool(r["OnlyRibbon"]))
          {
            break;
          }

          // skip hidden medals
          if (!f.AllowHiding || !SqlDataLayerConverter.VerifyBool(r["Hide"]))
          {
            if (inRow == 3)
            {
              // add break - only three ribbons in a row
              ribbonBar.Append("<br />");
              inRow = 0;
            }

            ribbonBar.AppendFormat(
              "<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" />", 
              YafForumInfo.ForumClientFileRoot, 
              r["SmallRibbonURL"], 
              r["SmallRibbonWidth"], 
              r["SmallRibbonHeight"], 
              r["Name"], 
              f.ShowMessage ? ": {0}".FormatWith(r["Message"]) : string.Empty, 
              YafBoardFolders.Current.Medals);

            inRow++;
          }

          // move to next row
          i++;
        }

        // follow with the rest
        while (dt.Rows.Count > i)
        {
          r = dt.Rows[i];
          f = new MedalFlags(r["Flags"]);

          // skip hidden medals
          if (!f.AllowHiding || !SqlDataLayerConverter.VerifyBool(r["Hide"]))
          {
            medals.AppendFormat(
              "<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" title=\"{4}{5}\" />", 
              YafForumInfo.ForumClientFileRoot, 
              r["SmallMedalURL"], 
              r["SmallMedalWidth"], 
              r["SmallMedalHeight"], 
              r["Name"], 
              f.ShowMessage ? ": {0}".FormatWith(r["Message"]) : string.Empty, 
              YafBoardFolders.Current.Medals);
          }

          // move to next row
          i++;
        }

        filler = this.PageContext.BoardSettings.UserBoxMedals.FormatWith(
          this.GetText("MEDALS"), ribbonBar, medals);
      }

      // replaces template placeholder with actual medals
      userBox = rx.Replace(userBox, filler);

      return userBox;
    }

    /// <summary>
    /// The match user box points.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box points.
    /// </returns>
    [NotNull]
    private string MatchUserBoxPoints([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.Points);
      if (this.PageContext.BoardSettings.DisplayPoints)
      {
        filler = this.PageContext.BoardSettings.UserBoxPoints.FormatWith(
          this.GetText("points"), this.DataRow["Points"]);
      }

      // replaces template placeholder with actual points
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box post count.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box post count.
    /// </returns>
    [NotNull]
    private string MatchUserBoxPostCount([NotNull] string userBox)
    {
      var rx = this.GetRegex(Constants.UserBox.Posts);

      // vzrus: should not display posts count string if the user post only in a forum with no post count?
      // if ((int)this.DataRow["Posts"] > 0)
      // {
      string filler =
        this.PageContext.BoardSettings.UserBoxPosts.FormatWith(
          this.GetText("posts"), this.DataRow["Posts"]);

      // }

      // replaces template placeholder with actual post count
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box rank.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <param name="roleStyleTable">
    /// The role Style Table.
    /// </param>
    /// <returns>
    /// The match user box rank.
    /// </returns>
    [NotNull]
    private string MatchUserBoxRank([NotNull] string userBox, [NotNull] DataTable roleStyleTable)
    {
      string rankStyle = null;

      foreach (DataRow drow in roleStyleTable.Rows)
      {
        // Rank for a user has LegendID = 2
        if (Convert.ToInt32(drow["LegendID"]) == 2 && drow["Style"] != null &&
            drow["Name"].ToString() == this.DataRow["RankName"].ToString())
        {
          rankStyle = this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true);
          break;
        }
      }

      string filler = null;

      var rx = this.GetRegex(Constants.UserBox.Rank);
      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        filler = this.PageContext.BoardSettings.UserBoxRank.FormatWith(
          this.GetText("rank"), 
          @"<span class=""YafRank_{0}"" style=""{1}"">{0}</span>".FormatWith(this.DataRow["RankName"], rankStyle));
      }
      else
      {
        filler = this.PageContext.BoardSettings.UserBoxRank.FormatWith(
          this.GetText("rank"), this.DataRow["RankName"]);
      }

      // replaces template placeholder with actual rank
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box rank images.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box rank images.
    /// </returns>
    [NotNull]
    private string MatchUserBoxRankImages([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.RankImage);

      if (!this.DataRow["RankImage"].IsNullOrEmptyDBField())
      {
        filler =
          this.PageContext.BoardSettings.UserBoxRankImage.FormatWith(
            @"<img class=""rankimage"" src=""{0}{1}/{2}"" alt="""" />".FormatWith(
              YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Ranks, this.DataRow["RankImage"]));
      }

      // replaces template placeholder with actual rank image
      userBox = rx.Replace(userBox, filler);
      return userBox;
    }

    /// <summary>
    /// The match user box thanks from.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The match user box thanks from.
    /// </returns>
    [NotNull]
    private string MatchUserBoxThanksFrom([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.ThanksFrom);

      // vzrus: should not display if no thanks?
      if (this.PageContext.BoardSettings.EnableThanksMod)
      {
        if ((int)this.DataRow["ThanksFromUserNumber"] > 0)
        {
          filler =
            this.PageContext.BoardSettings.UserBoxThanksFrom.FormatWith(
              (this.UserProfile.Gender == 1
                 ? this.GetText("thanksfrom_musc")
                 : (this.UserProfile.Gender == 2
                      ? this.GetText("thanksfrom_fem")
                      : this.GetText("thanksfrom"))).FormatWith(
                        this.DataRow["ThanksFromUserNumber"]));
        }
      }

      // replaces template placeholder with actual thanks from
      userBox = rx.Replace(userBox, filler);

      return userBox;
    }

    /// <summary>
    /// The match user box thanks to.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// String with Thanks string added to UserBox .
    /// </returns>
    [NotNull]
    private string MatchUserBoxThanksTo([NotNull] string userBox)
    {
      string filler = string.Empty;
      var rx = this.GetRegex(Constants.UserBox.ThanksTo);

      // vzrus: should not display if no thanks?
      if (this.PageContext.BoardSettings.EnableThanksMod)
      {
        if ((int)this.DataRow["ThanksToUserNumber"] > 0 && (int)this.DataRow["ThanksToUserPostsNumber"] > 0)
        {
          filler =
            this.PageContext.BoardSettings.UserBoxThanksTo.FormatWith(
              this.GetText("thanksto").FormatWith(
                this.DataRow["ThanksToUserNumber"], this.DataRow["ThanksToUserPostsNumber"]));
        }
      }

      // replaces template placeholder with actual thanks from
      userBox = rx.Replace(userBox, filler);

      return userBox;
    }

    /// <summary>
    /// The remove empty dividers.
    /// </summary>
    /// <param name="userBox">
    /// The user box.
    /// </param>
    /// <returns>
    /// The remove empty dividers.
    /// </returns>
    [NotNull]
    private string RemoveEmptyDividers([NotNull] string userBox)
    {
      if (userBox.IndexOf(@"<div class=""section""></div>") > 0)
      {
        if (userBox.IndexOf(@"<div class=""section""></div><br />") > 0)
        {
          userBox = userBox.Replace(@"<div class=""section""></div><br />", String.Empty);
        }
        else
        {
          userBox = userBox.Replace(@"<div class=""section""></div>", String.Empty);
        }
      }

      return userBox;
    }

    #endregion
  }
}