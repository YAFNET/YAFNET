/* Yet Another Forum.NET
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

namespace YAF.Controls
{
  #region Using

    using System;
    using System.Web.UI;
    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

  /// <summary>
  /// Provides a basic "profile link" for a YAF User
  /// </summary>
  public class UserLink : UserLabel
  {
    #region Properties

      /// <summary>
      /// Gets or sets a value indicating whether 
      /// Make the link target "blank" to open in a new window.
      /// </summary>
      public bool BlankTarget
    {
      get
      {
          return this.ViewState["BlankTarget"] != null && Convert.ToBoolean(this.ViewState["BlankTarget"]);
      }

        set
      {
        this.ViewState["BlankTarget"] = value;
      }
    }

      /// <summary>
      ///   Gets or sets a Replace Name
      /// </summary>
      [CanBeNull]
      public string ReplaceName
      {
          get
          {
              return this.ViewState["ReplaceName"] != null ? this.ViewState["ReplaceName"].ToString() : string.Empty;
          }

          set
          {
              this.ViewState["ReplaceName"] = value;
          }
      }

    #endregion

    #region Methods

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {

    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
       string displayName = this.Get<IUserDisplayName>().GetName(this.UserID);

        if (this.UserID == -1 || !displayName.IsSet())
        {
            return;
        }

        // is this the guest user? If so, guest's don't have a profile.
        bool isGuest = UserMembershipHelper.IsGuestUser(this.UserID);

        output.BeginRender();

        if (!isGuest)
        {
            output.WriteBeginTag("a");

            output.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));

            if (this.Get<YafBoardSettings>().UseNoFollowLinks)
            {
                output.WriteAttribute("rel", "nofollow");
            }

            output.WriteAttribute("title", this.GetText("COMMON", "VIEW_USRPROFILE"));

            if (this.BlankTarget)
            {
                output.WriteAttribute("target", "_blank");
            }
        }
        else
        {
            output.WriteBeginTag("span");
        }

        this.RenderMainTagAttributes(output);

        output.Write(HtmlTextWriter.TagRightChar);

        // Replace Name with Crawler Name if Set, otherwise use regular display name or Replace Name if set
        if (this.CrawlerName.IsSet())
        {
            output.WriteEncodedText(this.CrawlerName);
        }
        else if (!this.CrawlerName.IsSet() && this.ReplaceName.IsSet() && isGuest)
        {
            output.WriteEncodedText(this.ReplaceName);
        }
        else
        {
            output.WriteEncodedText(displayName);
        }

        output.WriteEndTag(!isGuest ? "a" : "span");

        if (this.PostfixText.IsSet())
        {
            output.Write(this.PostfixText);
        }

        output.EndRender();
    }

    #endregion
  }
}