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
  /// The accordion designer action list.
  /// </summary>
  internal class AccordionDesignerActionList : DesignerActionList
  {
    #region Constants and Fields

    /// <summary>
    /// The items.
    /// </summary>
    protected DesignerActionItemCollection items;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AccordionDesignerActionList"/> class.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    public AccordionDesignerActionList(IComponent component)
      : base(component)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add nav view.
    /// </summary>
    public void AddNavView()
    {
      TransactedChangeCallback addNewCallback = this.DoAddNavView;
      ControlDesigner.InvokeTransactedChange(this.Component, addNewCallback, "AddNavView", "Add new NavView");
    }

    /// <summary>
    /// The add view.
    /// </summary>
    public void AddView()
    {
      TransactedChangeCallback addNewCallback = this.DoAddView;
      ControlDesigner.InvokeTransactedChange(this.Component, addNewCallback, "AddView", "Add new View");
    }

    /// <summary>
    /// The do add nav view.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// The do add nav view.
    /// </returns>
    public bool DoAddNavView(object args)
    {
      var accordion = this.Component as Accordion;
      var newView = new NavView();
      newView.Text = "NavView" + accordion.Views.Count;
      newView.ID = "NavView" + accordion.Views.Count;
      accordion.Views.Add(newView);
      return true;
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
      var viewControl = this.Component as Accordion;
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
        this.items.Add(new DesignerActionMethodItem(this, "AddNavView", "Add NavView", "Behavior"));
      }

      return this.items;
    }

    #endregion
  }
}