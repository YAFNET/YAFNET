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
  using System;
  using System.Web.UI;

  using Classes;
  using Classes.Core;
  using Classes.Utils;

  public class UserLabel : BaseControl
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UserLink"/> class.
    /// </summary>
    public UserLabel()
    {
    }

    #endregion

    #region Properties

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
      string displayName = PageContext.UserDisplayName.GetName(this.UserID);

      if (this.UserID != -1 && !displayName.IsNullOrEmptyTrimmed())
      {
        output.BeginRender();

        output.WriteBeginTag("span");

        this.RenderMainTagAttributes(output);

        output.Write(HtmlTextWriter.TagRightChar);

        output.WriteEncodedText(displayName);

        output.WriteEndTag("span");

        if (!String.IsNullOrEmpty(this.PostfixText))
        {
          output.Write(this.PostfixText);
        }

        output.EndRender();
      }
    }

    /// <summary>
    /// Renders "id", "style", "onclick", "onmouseover" and "class"
    /// </summary>
    /// <param name="output"></param>
    protected void RenderMainTagAttributes(HtmlTextWriter output)
    {
      if (!String.IsNullOrEmpty(this.ClientID))
      {
        output.WriteAttribute("id", this.ClientID);
      }

      if (!String.IsNullOrEmpty(this.Style))
      {
        output.WriteAttribute("style", this.HtmlEncode(this.Style));
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
    }

    #endregion    
  }
}