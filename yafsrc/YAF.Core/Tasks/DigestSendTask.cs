/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Net;
  using System.Text.RegularExpressions;

  using YAF.Classes;
  using YAF.Core;
  using YAF.Core.Data;
  using YAF.Core.Services;
  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Extensions;
  using YAF.Utils.Helpers.StringUtils;
  using YAF.Types.Interfaces;
  using YAF.Types.Objects;

  #endregion

  /// <summary>
  /// The digest send task.
  /// </summary>
  public class DigestSendTask : IntermittentBackgroundTask
  {
    #region Constants and Fields

    /// <summary>
    ///   The _task name.
    /// </summary>
    private const string _taskName = "DigestSendTask";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "DigestSendTask" /> class.
    /// </summary>
    public DigestSendTask()
    {
      this.RunPeriodMs = 300 * 1000;
      this.StartDelayMs = 30 * 1000;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets TaskName.
    /// </summary>
    public static string TaskName
    {
      get
      {
        return _taskName;
      }
    }

		[Inject]
  	public IDbFunction DbFunction { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
      //// validate DB run...
      //this.Get<StartupInitializeDb>().Run();

      this.SendDigest();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The is time to send digest for board.
    /// </summary>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    /// <returns>
    /// The is time to send digest for board.
    /// </returns>
    private bool IsTimeToSendDigestForBoard([NotNull] YafLoadBoardSettings boardSettings)
    {
      CodeContracts.ArgumentNotNull(boardSettings, "boardSettings");

      if (boardSettings.AllowDigestEmail)
      {
        DateTime lastSend = DateTime.MinValue;
        bool sendDigest = false;
        int sendEveryXHours = boardSettings.DigestSendEveryXHours;

        if (boardSettings.LastDigestSend.IsSet())
        {
          lastSend = Convert.ToDateTime(boardSettings.LastDigestSend);
        }

#if (DEBUG)
        // haven't sent in X hours or more and it's 12 to 5 am.
        sendDigest = lastSend < DateTime.Now.AddHours(-sendEveryXHours);      
#else
        // haven't sent in X hours or more and it's 12 to 5 am.
        sendDigest = lastSend < DateTime.Now.AddHours(-sendEveryXHours) && DateTime.Now < DateTime.Today.AddHours(6);
#endif
        if (sendDigest || boardSettings.ForceDigestSend)
        {
          // && DateTime.Now < DateTime.Today.AddHours(5))
          // we're good to send -- update latest send so no duplication...
          boardSettings.LastDigestSend = DateTime.Now.ToString();
          boardSettings.ForceDigestSend = false;
          boardSettings.SaveRegistry();

          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// The send digest.
    /// </summary>
    private void SendDigest()
    {
      try
      {
        var boardIds = ((DataTable)this.DbFunction.GetData.board_list(null)).AsEnumerable().Select(b => b.Field<int>("BoardID"));

        foreach (var boardId in boardIds)
        {
          var boardSettings = new YafLoadBoardSettings(boardId, this.DbFunction);

          if (!this.IsTimeToSendDigestForBoard(boardSettings))
          {
            continue;
          }

          if (Config.BaseUrlMask.IsNotSet())
          {
            // fail...
						this.Logger.Error("Failed to send digest because BaseUrlMask value is not set in your appSettings.");
            return;
          }

          // get users with digest enabled...
          var usersWithDigest = ((DataTable)this.DbFunction.GetData.user_find(boardId, false, null, null, null, null, true)).Typed<TypedUserFind>().ToList().Where(x => !x.IsGuest && (x.IsApproved ?? false));

          if (usersWithDigest.Any())
          {
            // start sending...
            this.SendDigestToUsers(usersWithDigest, boardId, boardSettings.Name);
          }
        }
      }
      catch (Exception ex)
      {
				this.Logger.Error(ex, "Error In {0} Task", TaskName);
      }
    }

    /// <summary>
    /// The send digest to users.
    /// </summary>
    /// <param name="usersWithDigest">
    ///   The users with digest.
    /// </param>
    /// <param name="boardId"></param>
    /// <param name="forumName"></param>
    private void SendDigestToUsers(IEnumerable<TypedUserFind> usersWithDigest, int boardId, string forumName)
    {
      foreach (var user in usersWithDigest)
      {
        string digestHtml = string.Empty;

        try
        {
          digestHtml = this.Get<IDigest>().GetDigestHtml(user.UserID ?? 0, boardId);
        }
        catch (Exception e)
        {
					this.Logger.Error(e, "Error In Creating Digest for User {0}".FormatWith(user.UserID));
        }

        if (digestHtml.IsSet())
        {
          if (user.ProviderUserKey == null)
          {
            continue;
          }

          var membershipUser = UserMembershipHelper.GetUser(user.ProviderUserKey);

          if (membershipUser == null || membershipUser.Email.IsNotSet())
          {
            continue;
          }

          // send the digest...
          this.Get<IDigest>().SendDigest(digestHtml, forumName, membershipUser.Email, user.DisplayName, true);
        }
      }
    }

    #endregion
  }
}