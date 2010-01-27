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
  public class UserLink : BaseControl
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

    /// <summary>
    /// Gets or sets CssClass.
    /// </summary>
    public string CssClass
    {
      get
      {
        if (this.ViewState["CssClass"] != null)
        {
          return this.ViewState["CssClass"].ToString();
        }

        return string.Empty;
      }

      set
      {
        this.ViewState["CssClass"] = value;
      }
    }

    /// <summary>
    /// The onclick value for the profile link
    /// </summary>
    public string OnClick
    {
      get
      {
        if (this.ViewState["OnClick"] != null)
        {
          return this.ViewState["OnClick"].ToString();
        }

        return string.Empty;
      }

      set
      {
        this.ViewState["OnClick"] = value;
      }
    }

    /// <summary>
    /// The onmouseover value for the profile link
    /// </summary>
    public string OnMouseOver
    {
      get
      {
        if (this.ViewState["OnMouseOver"] != null)
        {
          return this.ViewState["OnMouseOver"].ToString();
        }

        return string.Empty;
      }

      set
      {
        this.ViewState["OnMouseOver"] = value;
      }
    }

    /// <summary>
    /// The name of the user for this profile link
    /// </summary>
    public string PostfixText
    {
      get
      {
        if (this.ViewState["PostfixText"] != null)
        {
          return this.ViewState["PostfixText"].ToString();
        }

        return string.Empty;
      }

      set
      {
        this.ViewState["PostfixText"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Style.
    /// </summary>
    public string Style
    {
      get
      {
        if (this.ViewState["Style"] != null)
        {
          return this.ViewState["Style"].ToString();
        }

        return string.Empty;
      }

      set
      {
        this.ViewState["Style"] = value;
      }
    }

    /// <summary>
    /// The userid of this user for the link
    /// </summary>
    public int UserID
    {
      get
      {
        if (this.ViewState["UserID"] != null)
        {
          return Convert.ToInt32(this.ViewState["UserID"]);
        }

        return -1;
      }

      set
      {
        this.ViewState["UserID"] = value;
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
      string displayName = YafProvider.UserDisplayName.GetName(this.UserID);

      if (this.UserID != -1 && !displayName.IsNullOrEmptyTrimmed())
      {
        // is this the guest user? If so, guest's don't have a profile.
        bool isGuest = UserMembershipHelper.IsGuestUser(this.UserID);

        output.BeginRender();

        if (!isGuest)
        {
          output.WriteBeginTag("a");
          if (!String.IsNullOrEmpty(this.ClientID))
          {
            output.WriteAttribute("id", this.ClientID);
          }

          output.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));
          output.WriteAttribute("title", this.HtmlEncode(displayName));
          output.WriteAttribute("style", this.HtmlEncode(this.Style));
          if (this.BlankTarget)
          {
            output.WriteAttribute("target", "_blank");
          }

          if (!String.IsNullOrEmpty(this.OnClick))
          {
            output.WriteAttribute("onclick", this.OnClick);
          }

          if (!String.IsNullOrEmpty(this.OnMouseOver))
          {
            output.WriteAttribute("onmouseover", this.OnMouseOver);
          }

          if (!String.IsNullOrEmpty(this.CssClass))
          {
            output.WriteAttribute("class", this.CssClass);
          }

          // if (!String.IsNullOrEmpty( Style )) output.WriteAttribute("style", Style);
          output.Write(HtmlTextWriter.TagRightChar);
        }

        output.WriteEncodedText(displayName);

        if (!isGuest)
        {
          output.WriteEndTag("a");
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