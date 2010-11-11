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

  #endregion

  /// <summary>
  /// Designer of Tabs
  /// </summary>
  public class TabsDesigner : ControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The tabs.
    /// </summary>
    private Tabs tabs;

    #endregion

    #region Properties

    /// <summary>
    /// Gets ActionLists.
    /// </summary>
    public override DesignerActionListCollection ActionLists
    {
      get
      {
        var lists = new DesignerActionListCollection();
        lists.AddRange(base.ActionLists);
        lists.Add(new TabDesignerActionList(this.Component));
        return lists;
      }
    }

    /// <summary>
    /// Gets a value indicating whether AllowResize.
    /// </summary>
    public override bool AllowResize
    {
      get
      {
        return true;
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
      if (this.tabs.Views.Count < 0)
      {
        return this.GetEmptyDesignTimeHtml();
      }

      var stringBuilder = new StringBuilder();
      var stringWriter = new StringWriter(stringBuilder);
      var htmlWriter = new HtmlTextWriter(stringWriter);
      if (string.IsNullOrEmpty(this.tabs.CssClass))
      {
        this.tabs.CssClass = "ui-tabs ui-widget ui-widget-content ui-corner-all";
      }

      this.tabs.RenderBeginTag(htmlWriter);
      htmlWriter.WriteBeginTag("ul");
      htmlWriter.WriteAttribute(
        "class", "ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all");
      htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
      for (int i = 0; i < this.tabs.Views.Count; i++)
      {
        View view = this.tabs.Views[i];
        regions.Add(new DesignerRegion(this, "view" + i, false));
        htmlWriter.WriteBeginTag("li");
        htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());

        if (view.IsSelected)
        {
          htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-top ui-tabs-selected ui-state-active");
        }
        else
        {
          htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-top");
        }

        htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
        htmlWriter.WriteBeginTag("a");
        htmlWriter.WriteAttribute("href", "#");
        htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
        htmlWriter.Write(view.Text);
        htmlWriter.WriteEndTag("a");
        htmlWriter.WriteEndTag("li");
      }

      htmlWriter.WriteEndTag("ul");
      regions.Add(new EditableDesignerRegion(this, "content", false));
      htmlWriter.WriteBeginTag("div");

      // htmlWriter.WriteAttribute("style","position:;");
      htmlWriter.WriteAttribute("class", "ui-tabs-panel ui-widget-content ui-corner-bottom");
      htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, this.tabs.Views.Count.ToString());
      htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
      htmlWriter.WriteEndTag("div");
      this.tabs.RenderEndTag(htmlWriter);
      return stringBuilder.ToString();
    }

    /// <summary>
    /// The get design time html.
    /// </summary>
    /// <returns>
    /// The get design time html.
    /// </returns>
    public override string GetDesignTimeHtml()
    {
      if (this.tabs.Views.Count < 1)
      {
        return this.GetEmptyDesignTimeHtml();
      }

      return base.GetDesignTimeHtml();
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
        // int index = int.Parse(region.Name.Substring(4));
        View view = this.tabs.Views[this.tabs.SelectedIndex];
        view.CssClass = "ui-tabs-panel ui-widget-content ui-corner-bottom";
        view.ShowHeader = false;
        return ControlPersister.PersistControl(view);
      }

      return String.Empty;
    }

    /// <summary>
    /// Initialize the TabsDesigner
    /// </summary>
    /// <param name="component">
    /// </param>
    public override void Initialize(IComponent component)
    {
      this.tabs = component as Tabs;
      if (this.tabs == null)
      {
        throw new ArgumentException("Component must be an Tabs control", "component");
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
        var view = ControlParser.ParseControl(host, content) as View;
        int selectedIndex = int.Parse(region.Name.Substring(4));
        this.tabs.Views.RemoveAt(selectedIndex);
        this.tabs.Views.AddAt(selectedIndex, view);
        this.tabs.SelectedIndex = selectedIndex;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get empty design time html.
    /// </summary>
    /// <returns>
    /// The get empty design time html.
    /// </returns>
    protected override string GetEmptyDesignTimeHtml()
    {
      return this.CreatePlaceHolderDesignTimeHtml("Please add View inside Tabs");
    }

    /// <summary>
    /// The on click.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnClick(DesignerRegionMouseEventArgs e)
    {
      if (e.Region == null)
      {
        return;
      }

      if (e.Region.Name.StartsWith("content"))
      {
        return;
      }

      int index = int.Parse(e.Region.Name.Substring(4));

      if (index != this.tabs.SelectedIndex)
      {
        this.tabs.SelectedIndex = index;
        this.UpdateDesignTimeHtml();
      }
    }

    #endregion
  }
}