//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery.Design
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.IO;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.Design;
  using System.Web.UI.Design.WebControls;

  #endregion

  /// <summary>
  /// The Designer for Dialog
  /// </summary>
  public class DialogDesigner : CompositeControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The dialog.
    /// </summary>
    private Dialog dialog;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets whether the Dialog can resize
    /// </summary>
    public override bool AllowResize
    {
      get
      {
        if (this.dialog != null)
        {
          return this.dialog.IsResizable;
        }

        return false;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The get design time html.
    /// </summary>
    /// <param name="regions">
    /// The regions.
    /// </param>
    /// <returns>
    /// The get design time html.
    /// </returns>
    public override string GetDesignTimeHtml(DesignerRegionCollection regions)
    {
      var stringBuilder = new StringBuilder();
      var stringWriter = new StringWriter(stringBuilder);
      var htmlWriter = new HtmlTextWriter(stringWriter);
      int width = Convert.ToInt16(this.dialog.Width.Value);
      int height = Convert.ToInt16(this.dialog.Height.Value);

      if (width == 0)
      {
        width = 300;
      }

      if (this.dialog.MinHeight != 0)
      {
        if (height < this.dialog.MinHeight)
        {
          height = this.dialog.MinHeight;
        }
      }

      if (this.dialog.MaxHeight != 0)
      {
        if (height > this.dialog.MaxHeight)
        {
          height = this.dialog.MaxHeight;
        }
      }

      if (this.dialog.MinWidth != 0)
      {
        if (width < this.dialog.MinWidth)
        {
          width = this.dialog.MinWidth;
        }
      }

      if (this.dialog.MaxWidth != 0)
      {
        if (width > this.dialog.MaxWidth)
        {
          width = this.dialog.MaxWidth;
        }
      }

      htmlWriter.WriteBeginTag("div");
      htmlWriter.WriteAttribute("id", this.dialog.ID);
      htmlWriter.WriteAttribute(
        "class", "ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable");
      string style = "width:" + width + "px;border:solid 1px #333333;overflow: hidden; display: block;z-index: 1002";

      string contentHeight = string.Empty;

      if (height != 0)
      {
        if (this.dialog.Buttons.Count == 0)
        {
          style = style + ";height:" + (height + 25) + "px;";
        }
        else
        {
          style = style + ";height:" + (height + 95) + "px;";
        }

        contentHeight = "height:" + height + "px;";
      }

      // htmlWriter.AddStyleAttribute("height", height.ToString() + "px");
      // htmlWriter.AddStyleAttribute("width", width.ToString() + "px");
      // htmlWriter.AddStyleAttribute("border", "solid 1px #333333");
      htmlWriter.WriteAttribute("style", style);
      htmlWriter.Write(HtmlTextWriter.TagRightChar);

      // Dialog header
      htmlWriter.WriteBeginTag("div");
      htmlWriter.WriteAttribute("class", "ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix");
      htmlWriter.WriteAttribute("style", "padding:5px;display:block;height:25px;");
        
        // background-color:#c9c9c9;color:#000000
      htmlWriter.Write(HtmlTextWriter.TagRightChar);

      htmlWriter.WriteBeginTag("span");
      htmlWriter.WriteAttribute("class", "ui-dialog-title");
      htmlWriter.Write(HtmlTextWriter.TagRightChar);
      htmlWriter.Write(this.dialog.Title);
      htmlWriter.WriteEndTag("span");

      htmlWriter.WriteBeginTag("a");
      htmlWriter.WriteAttribute("href", "#");
      htmlWriter.WriteAttribute("class", "ui-dialog-titlebar-close ui-corner-all");
      htmlWriter.Write(HtmlTextWriter.TagRightChar);
      htmlWriter.WriteBeginTag("span");
      htmlWriter.WriteAttribute("class", "ui-icon ui-icon-closethick");
      htmlWriter.Write(HtmlTextWriter.TagRightChar);
      htmlWriter.WriteEndTag("span");
      htmlWriter.WriteEndTag("a");

      htmlWriter.WriteEndTag("div");

      regions.Add(new EditableDesignerRegion(this, "dialogContent", false));
      htmlWriter.WriteBeginTag("div");
      htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, "0");
      htmlWriter.WriteAttribute("class", "ui-dialog-content ui-widget-content");
      htmlWriter.WriteAttribute("style", "overflow: hidden;padding:5px;display:block;" + contentHeight);
      htmlWriter.Write(HtmlTextWriter.TagRightChar);
      htmlWriter.WriteEndTag("div");

      // Write buttons
      if (this.dialog.Buttons.Count > 0)
      {
        htmlWriter.WriteBeginTag("div");
        htmlWriter.WriteAttribute("class", "ui-dialog-buttonpane ui-widget-content ui-helper-clearfix");
        htmlWriter.WriteAttribute("style", "padding:5px;display:block;text-align:right;height:28px");
        htmlWriter.Write(HtmlTextWriter.TagRightChar);
        foreach (DialogButton btn in this.dialog.Buttons)
        {
          htmlWriter.WriteBeginTag("input");
          htmlWriter.WriteAttribute("type", "button");
          htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-all");
          htmlWriter.WriteAttribute("style", "margin:2px;");
          htmlWriter.WriteAttribute("value", btn.Text);
          htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        htmlWriter.WriteEndTag("div");
      }

      htmlWriter.WriteEndTag("div");
      return stringBuilder.ToString();
    }

    /// <summary>
    /// The get editable designer region content.
    /// </summary>
    /// <param name="region">
    /// The region.
    /// </param>
    /// <returns>
    /// The get editable designer region content.
    /// </returns>
    public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
    {
      var host = (IDesignerHost)this.Component.Site.GetService(typeof(IDesignerHost));
      if (host != null)
      {
        if (this.dialog.BodyTemplate != null)
        {
          return ControlPersister.PersistTemplate(this.dialog.BodyTemplate, host);
        }
      }

      return String.Empty;
    }

    /// <summary>
    /// Initialize the DialogDesigner
    /// </summary>
    /// <param name="component">
    /// Target component
    /// </param>
    public override void Initialize(IComponent component)
    {
      this.dialog = component as Dialog;
      if (this.dialog == null)
      {
        throw new ArgumentException("Component must be an Accordion control", "component");
      }

      base.Initialize(component);
    }

    /// <summary>
    /// The set editable designer region content.
    /// </summary>
    /// <param name="region">
    /// The region.
    /// </param>
    /// <param name="content">
    /// The content.
    /// </param>
    public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
    {
      if (string.IsNullOrEmpty(content))
      {
        return;
      }

      var host = (IDesignerHost)this.Component.Site.GetService(typeof(IDesignerHost));
      if (host != null)
      {
        this.dialog.BodyTemplate = ControlParser.ParseTemplate(host, content);
      }
    }

    #endregion
  }
}