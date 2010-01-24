/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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
  /// <summary>
  /// For globally or multiple times used constants
  /// </summary>
  public static class Constants
  {
    #region Nested type: Cache

    /// <summary>
    /// Cache key constants
    /// </summary>
    public struct Cache
    {
      /// <summary>
      /// The active discussions.
      /// </summary>
      public const string ActiveDiscussions = "ActiveDiscussions";

      /// <summary>
      /// The banned ip.
      /// </summary>
      public const string BannedIP = "BannedIP";

      /// <summary>
      /// The board settings.
      /// </summary>
      public const string BoardSettings = "BoardSettings";

      /// <summary>
      /// The board stats.
      /// </summary>
      public const string BoardStats = "BoardStats";

      /// <summary>
      /// The custom bb code.
      /// </summary>
      public const string CustomBBCode = "CustomBBCode";

      /// <summary>
      /// The first post cleaned.
      /// </summary>
      public const string FirstPostCleaned = "FirstPostCleaned{0}{1}";

      /// <summary>
      /// The forum active discussions.
      /// </summary>
      public const string ForumActiveDiscussions = "ForumActiveDiscussions";

      /// <summary>
      /// The forum category.
      /// </summary>
      public const string ForumCategory = "ForumCategory";

      /// <summary>
      /// The forum jump.
      /// </summary>
      public const string ForumJump = "ForumJump{0}";

      /// <summary>
      /// The forum moderators.
      /// </summary>
      public const string ForumModerators = "ForumModerators";

      /// <summary>
      /// The group rank styles.
      /// </summary>
      public const string GroupRankStyles = "GroupRankStyles";

      /// <summary>
      /// The guest user id.
      /// </summary>
      public const string GuestUserID = "GuestUserID";

      /// <summary>
      /// The most active users.
      /// </summary>
      public const string MostActiveUsers = "MostActiveUsers";

      /// <summary>
      /// The replace rules.
      /// </summary>
      public const string ReplaceRules = "ReplaceRules{0}";

      /// <summary>
      /// The replace words.
      /// </summary>
      public const string ReplaceWords = "ReplaceWords";

      /// <summary>
      /// The shoutbox.
      /// </summary>
      public const string Shoutbox = "Shoutbox";

      /// <summary>
      /// The smilies.
      /// </summary>
      public const string Smilies = "Smilies";

      /// <summary>
      /// The user boxes.
      /// </summary>
      public const string UserBoxes = "UserBoxes";

      /// <summary>
      /// The user ignore list.
      /// </summary>
      public const string UserIgnoreList = "UserIgnoreList{0}";

      /// <summary>
      /// The user buddies.
      /// </summary>
      public const string UserBuddies = "UserBuddies{0}";

      /// <summary>
      /// The favorite topic list.
      /// </summary>
      public const string FavoriteTopicList = "FavoriteTopicList{0}";

      /// <summary>
      /// The user medals.
      /// </summary>
      public const string UserMedals = "UserMedals{0}";

      /// <summary>
      /// The users online status.
      /// </summary>
      public const string UsersOnlineStatus = "UsersOnlineStatus";

      /// <summary>
      /// The users display name collection.
      /// </summary>
      public const string UsersDisplayNameCollection = "UsersDisplayNameCollection";
    }

    #endregion

    #region Nested type: UserBox

    /// <summary>
    /// Constants for UserBox templating
    /// </summary>
    public struct UserBox
    {
      /// <summary>
      /// The avatar.
      /// </summary>
      public const string Avatar = @"<yaf:avatar\s*/>";

      /// <summary>
      /// The display template default.
      /// </summary>
      public const string DisplayTemplateDefault =
        @"<yaf:avatar /><div class=""section""><yaf:rankimage /><yaf:rank /></div><br /><yaf:medals /><div class=""section""><yaf:groups /><yaf:joindate /><yaf:posts /><yaf:points /><yaf:location /></div><br/><div class=""section""><yaf:thanksfrom /><yaf:thanksto /></div>";

      /// <summary>
      /// The groups.
      /// </summary>
      public const string Groups = @"<yaf:groups\s*/>";

      /// <summary>
      /// The join date.
      /// </summary>
      public const string JoinDate = @"<yaf:joindate\s*/>";

      /// <summary>
      /// The location.
      /// </summary>
      public const string Location = @"<yaf:location\s*/>";

      /// <summary>
      /// The medals.
      /// </summary>
      public const string Medals = @"<yaf:medals\s*/>";

      /// <summary>
      /// The points.
      /// </summary>
      public const string Points = @"<yaf:points\s*/>";

      /// <summary>
      /// The posts.
      /// </summary>
      public const string Posts = @"<yaf:posts\s*/>";

      /// <summary>
      /// The rank.
      /// </summary>
      public const string Rank = @"<yaf:rank\s*/>";

      /// <summary>
      /// The rank image.
      /// </summary>
      public const string RankImage = @"<yaf:rankimage\s*/>";

      /// <summary>
      /// The thanks from.
      /// </summary>
      public const string ThanksFrom = @"<yaf:thanksfrom\s*/>";

      /// <summary>
      /// The thanks to.
      /// </summary>
      public const string ThanksTo = @"<yaf:thanksto\s*/>";
    }

    #endregion
  }
}