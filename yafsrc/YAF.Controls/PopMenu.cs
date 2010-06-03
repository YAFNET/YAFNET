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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumJump.
  /// </summary>
  public class PopMenu : BaseControl, IPostBackEventHandler
  {
    /// <summary>
    /// The _control.
    /// </summary>
    private string _control = string.Empty;

    /// <summary>
    /// The _items.
    /// </summary>
    private List<InternalPopMenuItem> _items = new List<InternalPopMenuItem>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PopMenu"/> class.
    /// </summary>
    public PopMenu()
      : base()
    {
      Init += new EventHandler(PopMenu_Init);
    }

    /// <summary>
    /// Gets or sets Control.
    /// </summary>
    public string Control
    {
      get
      {
        return this._control;
      }

      set
      {
        this._control = value;
      }
    }

    /// <summary>
    /// Gets ControlOnClick.
    /// </summary>
    public string ControlOnClick
    {
      get
      {
        return string.Format("yaf_popit('{0}')", ClientID);
      }
    }

    /// <summary>
    /// Gets ControlOnMouseOver.
    /// </summary>
    public string ControlOnMouseOver
    {
      get
      {
        return string.Format("yaf_mouseover('{0}')", ClientID);
      }
    }

    /// <summary>
    /// The pop menu_ init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PopMenu_Init(object sender, EventArgs e)
    {
      // init the necessary js...
      PageContext.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
    }

    /// <summary>
    /// The add post back item.
    /// </summary>
    /// <param name="argument">
    /// The argument.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    public void AddPostBackItem(string argument, string description)
    {
      this._items.Add(new InternalPopMenuItem(description, argument, null));
    }

    /// <summary>
    /// The remove post back item.
    /// </summary>
    /// <param name="argument">
    /// The argument.
    /// </param>
    public void RemovePostBackItem(string Argument)
    {
        foreach (InternalPopMenuItem item in this._items)
        {
            if (item.PostBackArgument == Argument)
            {
                this._items.Remove(item);
                break;
            }
        }
    }

    /// <summary>
    /// The add client script item.
    /// </summary>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="clientScript">
    /// The client script.
    /// </param>
    public void AddClientScriptItem(string description, string clientScript)
    {
      this._items.Add(new InternalPopMenuItem(description, null, clientScript));
    }

    /// <summary>
    /// Add a item with a client script and post back option. (Use {postbackcode} in the <paramref name="clientScript"/> code for the postback code.)
    /// </summary>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="argument">post back argument</param>
    /// <param name="clientScript">
    /// The client script.
    /// </param>
    public void AddClientScriptItemWithPostback(string description, string argument, string clientScript)
    {
      this._items.Add(new InternalPopMenuItem(description, argument, clientScript));
    }

    /// <summary>
    /// The attach.
    /// </summary>
    /// <param name="ctl">
    /// The ctl.
    /// </param>
    public void Attach(WebControl ctl)
    {
      ctl.Attributes["onclick"] = ControlOnClick;
      ctl.Attributes["onmouseover"] = ControlOnMouseOver;
    }

    /// <summary>
    /// The attach.
    /// </summary>
    /// <param name="userLinkControl">
    /// The user link control.
    /// </param>
    public void Attach(UserLink userLinkControl)
    {
      userLinkControl.OnClick = ControlOnClick;
      userLinkControl.OnMouseOver = ControlOnMouseOver;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      if (!Visible)
      {
        return;
      }

      var sb = new StringBuilder();
      sb.AppendFormat(@"<div class=""yafpopupmenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;display:none;"">", ClientID);
      sb.Append("<ul>");

      // add the items
      foreach (InternalPopMenuItem thisItem in this._items)
      {
        string onClick;

        if (!String.IsNullOrEmpty(thisItem.ClientScript))
        {
          // js style
          onClick = thisItem.ClientScript.Replace("{postbackcode}", Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument));
        }
        else
        {
          onClick = Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument);
        }

        sb.AppendFormat(
          @"<li class=""popupitem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{1}"" style=""white-space:nowrap"">{0}</li>", 
          thisItem.Description, 
          onClick);
      }

      sb.AppendFormat("</ul></div>");

      writer.WriteLine(sb.ToString());

      base.Render(writer);
    }

    #region IPostBackEventHandler

    /// <summary>
    /// The raise post back event.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    public void RaisePostBackEvent(string eventArgument)
    {
      if (ItemClick != null)
      {
        ItemClick(this, new PopEventArgs(eventArgument));
      }
    }

    /// <summary>
    /// The item click.
    /// </summary>
    public event PopEventHandler ItemClick;

    #endregion
  }

  /// <summary>
  /// The pop event args.
  /// </summary>
  public class PopEventArgs : EventArgs
  {
    /// <summary>
    /// The _item.
    /// </summary>
    private string _item;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopEventArgs"/> class.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    public PopEventArgs(string eventArgument)
    {
      this._item = eventArgument;
    }

    /// <summary>
    /// Gets Item.
    /// </summary>
    public string Item
    {
      get
      {
        return this._item;
      }
    }
  }

  /// <summary>
  /// The pop event handler.
  /// </summary>
  /// <param name="sender">
  /// The sender.
  /// </param>
  /// <param name="e">
  /// The e.
  /// </param>
  public delegate void PopEventHandler(object sender, PopEventArgs e);

  /// <summary>
  /// The internal pop menu item.
  /// </summary>
  public class InternalPopMenuItem
  {
    /// <summary>
    /// The _client script.
    /// </summary>
    private string _clientScript = null;

    /// <summary>
    /// The _description.
    /// </summary>
    private string _description = null;

    /// <summary>
    /// The _postback argument.
    /// </summary>
    private string _postbackArgument = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalPopMenuItem"/> class.
    /// </summary>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="postbackArgument">
    /// The postback argument.
    /// </param>
    /// <param name="clientScript">
    /// The client script.
    /// </param>
    public InternalPopMenuItem(string description, string postbackArgument, string clientScript)
    {
      this._description = description;
      this._postbackArgument = postbackArgument;
      this._clientScript = clientScript;
    }

    /// <summary>
    /// Gets or sets Description.
    /// </summary>
    public string Description
    {
      get
      {
        return this._description;
      }

      set
      {
        this._description = value;
      }
    }

    /// <summary>
    /// Gets or sets PostBackArgument.
    /// </summary>
    public string PostBackArgument
    {
      get
      {
        return this._postbackArgument;
      }

      set
      {
        this._postbackArgument = value;
      }
    }

    /// <summary>
    /// Gets or sets ClientScript.
    /// </summary>
    public string ClientScript
    {
      get
      {
        return this._clientScript;
      }

      set
      {
        this._clientScript = value;
      }
    }
  }
}