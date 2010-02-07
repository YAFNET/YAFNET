/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Provides a basic "profile link" for a YAF User
  /// </summary>
  public class UserLink : UserLabel
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLink"/> class.
    /// </summary>
    public UserLink()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Make the link target "blank" to open in a new window.
    /// </summary>
    public bool BlankTarget
    {
      get
      {
        if (this.ViewState["BlankTarget"] != null)
        {
          return Convert.ToBoolean(this.ViewState["BlankTarget"]);
        }

        return false;
      }

      set
      {
        this.ViewState["BlankTarget"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render(HtmlTextWriter output)
    {
      string displayName = PageContext.UserDisplayName.GetName(this.UserID);

      if (this.UserID != -1 && !displayName.IsNullOrEmptyTrimmed())
      {
        // is this the guest user? If so, guest's don't have a profile.
        bool isGuest = UserMembershipHelper.IsGuestUser(this.UserID);

        output.BeginRender();

        if (!isGuest)
        {
          output.WriteBeginTag("a");

          output.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));

          output.WriteAttribute("title", this.HtmlEncode(displayName));

          if (this.BlankTarget)
          {
            output.WriteAttribute("target", "_blank");
          }
        }
        else
        {
          output.WriteBeginTag("span");
        }

        RenderMainTagAttributes(output);

        output.Write(HtmlTextWriter.TagRightChar);
        
        output.WriteEncodedText(displayName);

        if (!isGuest)
        {
          output.WriteEndTag("a");
        }
        else
        {
          output.WriteEndTag("span");
        }

        if (!String.IsNullOrEmpty(this.PostfixText))
        {
          output.Write(this.PostfixText);
        }

        output.EndRender();
      }
    }

    #endregion
  }
}