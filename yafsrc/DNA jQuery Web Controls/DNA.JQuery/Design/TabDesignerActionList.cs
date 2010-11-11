//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery.Design
{
  #region Using

  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Web.UI.Design;

  #endregion

  /// <summary>
  /// The tab designer action list.
  /// </summary>
  internal class TabDesignerActionList : DesignerActionList
  {
    #region Constants and Fields

    /// <summary>
    /// The items.
    /// </summary>
    protected DesignerActionItemCollection items;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TabDesignerActionList"/> class.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    public TabDesignerActionList(IComponent component)
      : base(component)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add view.
    /// </summary>
    public void AddView()
    {
      TransactedChangeCallback addNewCallback = this.DoAddView;
      ControlDesigner.InvokeTransactedChange(this.Component, addNewCallback, "AddView", "Add new View");
    }

    /// <summary>
    /// The do add view.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// The do add view.
    /// </returns>
    public bool DoAddView(object args)
    {
      var viewControl = this.Component as Tabs;
      var newView = new View();
      newView.Text = "View" + viewControl.Views.Count;
      newView.ID = "View" + viewControl.Views.Count;
      viewControl.Views.Add(newView);
      return true;
    }

    /// <summary>
    /// The get sorted action items.
    /// </summary>
    /// <returns>
    /// </returns>
    public override DesignerActionItemCollection GetSortedActionItems()
    {
      if (this.items == null)
      {
        this.items = new DesignerActionItemCollection();
        this.items.Add(new DesignerActionMethodItem(this, "AddView", "Add View", "Behavior"));
      }

      return this.items;
    }

    #endregion
  }
}