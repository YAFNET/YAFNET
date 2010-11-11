//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
 ///<notes> Updates Notes:
/// Date:2009/05/17
///  1.Inhert from the CompositeControl not from Control to support the designer
///  2.Remove "ContentStyle" and "ContentCss" use "Style" and "CssClass" from CompositeControl
///  3.Remove Render Override , RenderContent
///  3.Override RenderBeginTag
///</notes>

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.Design;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery.Design;

  #endregion

  /// <summary>
  /// View Control is a Container Control can be use inside Accordion or Tab Control.
  /// </summary>
  [PersistChildren(false)]
  [ParseChildren(false)]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxData("<{0}:View runat=\"server\" ID=\"View1\" Text=\"View\"></{0}:View>")]
  [ToolboxBitmap(typeof(View), "View.ico")]
  [Designer(typeof(ViewDesigner))]
  public class View : CompositeControl
  {
    #region Constants and Fields

    /// <summary>
    /// The enabled.
    /// </summary>
    private bool enabled = true;

    /// <summary>
    /// The show header.
    /// </summary>
    private bool showHeader = true;

    #endregion

    #region Events

    /// <summary>
    /// The active.
    /// </summary>
    public event EventHandler Active;

    /// <summary>
    /// The deactive.
    /// </summary>
    public event EventHandler Deactive;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the View Header's css class
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the View Header's css class")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Bindable(true)]
    [Themeable(true)]
    [CssClassProperty]
    [NotifyParentProperty(true)]
    public string HeaderCssClass { get; set; }

    /// <summary>
    ///   Gets/Sets the view header css style string.The ShowHeader Property must be set to true.
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the the view header css style string")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Bindable(true)]
    [NotifyParentProperty(true)]
    [Themeable(true)]
    public string HeaderStyle { get; set; }

    /// <summary>
    /// Gets or sets Height.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public override Unit Height
    {
      get
      {
        return base.Height;
      }

      set
      {
        base.Height = value;
      }
    }

    /// <summary>
    ///   Gets whether the view is selected
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [NotifyParentProperty(true)]
    public bool IsSelected
    {
      get
      {
        return this.Index == this.ParentContainer.SelectedIndex;
      }
    }

    /// <summary>
    ///   Gets/Sets when set the view header naviage url
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets when set the view header naviage url")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Bindable(true)]
    [NotifyParentProperty(true)]
    [UrlProperty]
    [Editor(typeof(UrlEditor), typeof(UITypeEditor))]
    public string NavigateUrl { get; set; }

    /// <summary>
    ///   Gets the View 's Container Control
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public JQueryMultiViewControl ParentContainer { get; internal set; }

    /// <summary>
    ///   Gets/Sets if the view show's it's header
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets if the view show's it's header")]
    [Bindable(true)]
    [NotifyParentProperty(true)]
    [Themeable(true)]
    public bool ShowHeader
    {
      get
      {
        return this.showHeader;
      }

      set
      {
        this.showHeader = value;
      }
    }

    /// <summary>
    ///   Gets/Sets when click the view header which the window open to
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets when click the view header which the window open to")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [Bindable(true)]
    [TypeConverter(typeof(TargetConverter))]
    public string Target { get; set; }

    /// <summary>
    ///   Gets/Sets the view's header title text
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the view's header title text")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [Localizable(true)]
    [Bindable(true)]
    [NotifyParentProperty(true)]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets Width.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public override Unit Width
    {
      get
      {
        return base.Width;
      }

      set
      {
        base.Width = value;
      }
    }

    /// <summary>
    /// Gets or sets Index.
    /// </summary>
    internal int Index { get; set; }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    [Browsable(false)]
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        return HtmlTextWriterTag.Div;
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
      if (this.ShowHeader)
      {
        this.RenderHeader(writer);
      }

      if (this.DesignMode)
      {
        // if (string.IsNullOrEmpty(CssClass))
        // Attributes.Add("class", "ui-accordion-content ui-helper-reset ui-widget-content ui-accordion-content-active");
        
        // COMMENTED OUT BY JABEN 11/11: Causing security exceptions in Medium Trust.
        //this.Attributes.Add(DesignerRegion.DesignerRegionAttributeName, "viewContent");
      }

      base.RenderBeginTag(writer);
    }

    /// <summary>
    /// The render header.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public virtual void RenderHeader(HtmlTextWriter writer)
    {
      string url = "#";
      if (!string.IsNullOrEmpty(this.NavigateUrl))
      {
        url = this.ResolveUrl(url);
      }

      writer.WriteBeginTag("h3");

      if (!string.IsNullOrEmpty(this.ToolTip))
      {
        writer.WriteAttribute("title", this.ToolTip);
      }

      if (!string.IsNullOrEmpty(this.HeaderStyle))
      {
        writer.WriteAttribute("style", this.HeaderStyle);
      }

      if (!string.IsNullOrEmpty(this.HeaderCssClass))
      {
        writer.WriteAttribute("class", this.HeaderCssClass);
      }
      else
      {
        // UNDONE: when drawing EditableDesignerRegion all property will be set to null ???
        if (this.DesignMode)
        {
          // This default header style just use in accrodion
          if (this.ParentContainer != null)
          {
            writer.WriteAttribute("class", "ui-accordion-header ui-helper-reset ui-state-default");
          }
          else
          {
            writer.WriteAttribute("class", "ui-accordion-header ui-helper-reset ui-state-active");
          }
        }
      }

      writer.Write(HtmlTextWriter.TagRightChar);

      if (this.DesignMode)
      {
        writer.WriteBeginTag("span");
        writer.WriteAttribute("class", "ui-icon ui-icon-triangle-1-e");
        writer.Write(HtmlTextWriter.TagRightChar);
        writer.WriteEndTag("span");
      }

      writer.WriteBeginTag("a");

      writer.WriteAttribute("href", url);
      if (!string.IsNullOrEmpty(this.Target))
      {
        writer.WriteAttribute("target", this.Target);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(this.Text);
      writer.WriteEndTag("a");
      writer.WriteEndTag("h3");
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on active.
    /// </summary>
    internal void OnActive()
    {
      if (this.Active != null)
      {
        this.Active(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// The on deactive.
    /// </summary>
    internal void OnDeactive()
    {
      if (this.Deactive != null)
      {
        this.Deactive(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// The set visible.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    internal void SetVisible(bool value)
    {
      foreach (Control ctrl in this.Controls)
      {
        ctrl.Visible = value;
      }
    }

    #endregion
  }
}