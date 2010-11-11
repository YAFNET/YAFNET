//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// The simple button.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:SimpleButton runat=\"server\" id=\"SimpleButton1\"></{0}:SimpleButton>")]
  public class SimpleButton : WebControl, INamingContainer, IPostBackEventHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The icon css class.
    /// </summary>
    private string iconCssClass = "ui-icon ui-icon-newwin";

    /// <summary>
    /// The text.
    /// </summary>
    private string text;

    #endregion

    #region Events

    /// <summary>
    /// The click.
    /// </summary>
    public event EventHandler Click;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets IconCssClass.
    /// </summary>
    public string IconCssClass
    {
      get
      {
        return this.iconCssClass;
      }

      set
      {
        this.iconCssClass = value;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public string Text
    {
      get
      {
        return this.text;
      }

      set
      {
        this.text = value;
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.A;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The render begin tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty(this.CssClass))
      {
        this.CssClass = "ui-state-default ui-corner-all";
      }

      if (this.Click == null)
      {
        this.Attributes.Add("href", "#");
      }
      else
      {
        this.Attributes.Add("href", this.Page.ClientScript.GetPostBackClientHyperlink(this, string.Empty));
      }

      this.Style.Add("position", "relative");
      this.Style.Add("padding", "0.4em 1em 0.4em 20px");
      this.Style.Add("text-decoration", "none");
      this.Attributes.Add("onmouseover", "$(this).addClass('ui-state-hover');");
      this.Attributes.Add("onmouseout", "$(this).removeClass('ui-state-hover');");
      base.RenderBeginTag(writer);
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackEventHandler

    /// <summary>
    /// The raise post back event.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
      if (this.Click != null)
      {
        this.Click(this, EventArgs.Empty);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The render contents.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void RenderContents(HtmlTextWriter writer)
    {
      writer.Write(
        "<span style='left:0.2em;margin:-8px 5px 0 0;position:absolute;top:50%;' class=\"" + this.IconCssClass +
        "\"></span>");
      writer.Write(this.Text);
    }

    #endregion
  }
}