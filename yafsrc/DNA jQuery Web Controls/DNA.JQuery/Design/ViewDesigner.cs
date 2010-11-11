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
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.Design;

  #endregion

  /// <summary>
  /// The view designer.
  /// </summary>
  public class ViewDesigner : ControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The _view.
    /// </summary>
    private View _view;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether AllowResize.
    /// </summary>
    public override bool AllowResize
    {
      get
      {
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
      var region = new EditableDesignerRegion(this, "viewContent", false);
      region.EnsureSize = true;
      region.Selectable = true;
      regions.Add(region);
      return base.GetDesignTimeHtml(regions);
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
        var html = new StringBuilder();
        for (int i = 0; i < this._view.Controls.Count; i++)
        {
          html.Append(ControlPersister.PersistControl(this._view.Controls[i], host));
        }

        return html.ToString();
      }

      return String.Empty;
    }

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <exception cref="ArgumentException">
    /// </exception>
    public override void Initialize(IComponent component)
    {
      // 判断父类控件component.Site.Container
      this._view = component as View;
      if (this._view == null)
      {
        throw new ArgumentException("Component must be an View Control", "component");
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
        Control[] controls = ControlParser.ParseControls(host, content);

        this._view.Controls.Clear();
        foreach (Control ctrl in controls)
        {
          this._view.Controls.Add(ctrl);
        }
      }
    }

    #endregion

    #region Methods

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

      if (e.Region.Name.EndsWith("viewHeader"))
      {
        return;
      }

      e.Region.Highlight = true;
      e.Region.Selected = true;
      this.UpdateDesignTimeHtml();
    }

    #endregion
  }
}