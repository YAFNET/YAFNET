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
using System;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Provides a basic "profile link" for a YAF User
  /// </summary>
  public class UserLink : BaseControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="UserLink"/> class.
    /// </summary>
    public UserLink()
    {
    }

    /// <summary>
    /// Gets or sets CssClass.
    /// </summary>
    public string CssClass
    {
      get
      {
        if (ViewState["CssClass"] != null)
        {
          return ViewState["CssClass"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["CssClass"] = value;
      }
    }

    /// <summary>
    /// The onclick value for the profile link
    /// </summary>
    public string OnClick
    {
      get
      {
        if (ViewState["OnClick"] != null)
        {
          return ViewState["OnClick"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["OnClick"] = value;
      }
    }

    /// <summary>
    /// The onmouseover value for the profile link
    /// </summary>
    public string OnMouseOver
    {
      get
      {
        if (ViewState["OnMouseOver"] != null)
        {
          return ViewState["OnMouseOver"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["OnMouseOver"] = value;
      }
    }

    /// <summary>
    /// The name of the user for this profile link
    /// </summary>
    public string UserName
    {
      get
      {
        if (ViewState["UserName"] != null)
        {
          return ViewState["UserName"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["UserName"] = value;
      }
    }

    /// <summary>
    /// The name of the user for this profile link
    /// </summary>
    public string PostfixText
    {
      get
      {
        if (ViewState["PostfixText"] != null)
        {
          return ViewState["PostfixText"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["PostfixText"] = value;
      }
    }

    /// <summary>
    /// The userid of this user for the link
    /// </summary>
    public int UserID
    {
      get
      {
        if (ViewState["UserID"] != null)
        {
          return Convert.ToInt32(ViewState["UserID"]);
        }

        return -1;
      }

      set
      {
        ViewState["UserID"] = value;
      }
    }

    /// <summary>
    /// Make the link target "blank" to open in a new window.
    /// </summary>
    public bool BlankTarget
    {
      get
      {
        if (ViewState["BlankTarget"] != null)
        {
          return Convert.ToBoolean(ViewState["BlankTarget"]);
        }

        return false;
      }

      set
      {
        ViewState["BlankTarget"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Style.
    /// </summary>
    public string Style
    {
      get
      {
        if (ViewState["Style"] != null)
        {
          return ViewState["Style"].ToString();
        }

        return string.Empty;
      }

      set
      {
        ViewState["Style"] = value;
      }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render(HtmlTextWriter output)
    {
      if (UserID != -1 && !String.IsNullOrEmpty(UserName))
      {
        // is this the guest user? If so, guest's don't have a profile.
        bool isGuest = UserMembershipHelper.IsGuestUser(UserID);

        output.BeginRender();

        if (!isGuest)
        {
          output.WriteBeginTag("a");
          if (!String.IsNullOrEmpty(ClientID))
          {
            output.WriteAttribute("id", ClientID);
          }

          output.WriteAttribute("href", YafBuildLink.GetLink(ForumPages.profile, "u={0}", UserID));
          output.WriteAttribute("title", HtmlEncode(UserName));
          output.WriteAttribute("style", HtmlEncode(Style));
          if (BlankTarget)
          {
            output.WriteAttribute("target", "_blank");
          }

          if (!String.IsNullOrEmpty(OnClick))
          {
            output.WriteAttribute("onclick", OnClick);
          }

          if (!String.IsNullOrEmpty(OnMouseOver))
          {
            output.WriteAttribute("onmouseover", OnMouseOver);
          }

          if (!String.IsNullOrEmpty(CssClass))
          {
            output.WriteAttribute("class", CssClass);
          }

          // if (!String.IsNullOrEmpty( Style )) output.WriteAttribute("style", Style);
          output.Write(HtmlTextWriter.TagRightChar);
        }

        output.WriteEncodedText(UserName);

        if (!isGuest)
        {
          output.WriteEndTag("a");
        }

        if (!String.IsNullOrEmpty(PostfixText))
        {
          output.Write(PostfixText);
        }

        output.EndRender();
      }
    }
  }
}