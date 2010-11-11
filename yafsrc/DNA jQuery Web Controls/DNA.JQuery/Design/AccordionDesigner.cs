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
  /// The Control Designer for Accordion
  /// </summary>
  public class AccordionDesigner : CompositeControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The _accordion.
    /// </summary>
    private Accordion _accordion;

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
        lists.Add(new AccordionDesignerActionList(this.Component));
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
      if (this._accordion.Views.Count == 0)
      {
        return this.GetEmptyDesignTimeHtml();
      }

      var stringBuilder = new StringBuilder();
      var stringWriter = new StringWriter(stringBuilder);
      var htmlWriter = new HtmlTextWriter(stringWriter);
      this._accordion.RenderBeginTag(htmlWriter);
      for (int i = 0; i < this._accordion.Views.Count; i++)
      {
        htmlWriter.WriteBeginTag("div");
        View view = this._accordion.Views[i];

        // if (_accordion.SelectedIndex == view.Index)
        if (view.IsSelected)
        {
          regions.Add(new EditableDesignerRegion(this, "view" + i, false));

          // if (string.IsNullOrEmpty(view.HeaderCssClass))
          // view.HeaderCssClass = "ui-accordion-header ui-helper-reset ui-state-active ui-corner-top";
          htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());
          htmlWriter.Write(HtmlTextWriter.TagRightChar);
          htmlWriter.WriteEndTag("div");
        }
        else
        {
          regions.Add(new DesignerRegion(this, "view" + i, true));

          // if (string.IsNullOrEmpty(view.HeaderCssClass))
          // view.HeaderCssClass = "ui-accordion-header ui-helper-reset ui-state-default ui-corner-all";
          htmlWriter.WriteAttribute(DesignerRegion.DesignerRegionAttributeName, i.ToString());
          htmlWriter.Write(HtmlTextWriter.TagRightChar);
          view.RenderHeader(htmlWriter);
          htmlWriter.WriteEndTag("div");
        }
      }

      this._accordion.RenderEndTag(htmlWriter);
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
      if (this._accordion.Views.Count == 0)
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
        int index = int.Parse(region.Name.Substring(4));
        View view = this._accordion.Views[index];
        view.ShowHeader = true;

        // TODO: don't do this.in design time do not set the css class property
        if (string.IsNullOrEmpty(view.CssClass))
        {
          view.CssClass =
            "ui-accordion-content ui-helper-reset ui-widget-content ui-corner-bottom ui-accordion-content-active";
        }

        return ControlPersister.PersistControl(view);
      }

      return String.Empty;
    }

    /// <summary>
    /// Initialize the AccordionDesigner
    /// </summary>
    /// <param name="component">
    /// </param>
    public override void Initialize(IComponent component)
    {
      this._accordion = component as Accordion;
      if (this._accordion == null)
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
        var view = ControlParser.ParseControl(host, content) as View;
        int selectedIndex = int.Parse(region.Name.Substring(4));
        this._accordion.Views.RemoveAt(selectedIndex);
        this._accordion.Views.AddAt(selectedIndex, view);
        this._accordion.SelectedIndex = selectedIndex;
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
      return this.CreatePlaceHolderDesignTimeHtml("Please add View or NavView inside this Accordion");
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

      int index = int.Parse(e.Region.Name.Substring(4));

      if (index != this._accordion.SelectedIndex)
      {
        this._accordion.SelectedIndex = index;
        this.UpdateDesignTimeHtml();
      }
    }

    #endregion
  }
}