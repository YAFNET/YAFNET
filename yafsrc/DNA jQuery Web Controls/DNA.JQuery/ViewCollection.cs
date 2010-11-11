//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.Collections;
  using System.Security.Permissions;
  using System.Web;

  #endregion

  /// <summary>
  /// The view collection.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class ViewCollection : CollectionBase
  {
    #region Constants and Fields

    /// <summary>
    /// The _parent.
    /// </summary>
    private readonly JQueryMultiViewControl _parent;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewCollection"/> class.
    /// </summary>
    /// <param name="parent">
    /// The parent.
    /// </param>
    public ViewCollection(JQueryMultiViewControl parent)
    {
      this._parent = parent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets Parent.
    /// </summary>
    public JQueryMultiViewControl Parent
    {
      get
      {
        return this._parent;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public View this[int index]
    {
      get
      {
        if (this.InnerList.Count <= index)
        {
          return null;
        }

        return this.InnerList[index] as View;
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="viewID">
    /// The view id.
    /// </param>
    public View this[string viewID]
    {
      get
      {
        foreach (View view in this.InnerList)
        {
          if ((view.ID == viewID) || (view.ClientID == viewID))
          {
            return view;
          }
        }

        return null;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The add.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public void Add(View view)
    {
      view.Index = this.InnerList.Count;
      view.ParentContainer = this._parent;
      this.InnerList.Add(view);
      this.Parent.Controls.Add(view);
    }

    /// <summary>
    /// The add at.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="view">
    /// The view.
    /// </param>
    public void AddAt(int index, View view)
    {
      view.ParentContainer = this._parent;
      this.InnerList.Insert(index, view);
      this.Reindex();
      this.Parent.Controls.AddAt(index, view);
    }

    /// <summary>
    /// The remove.
    /// </summary>
    /// <param name="view">
    /// The view.
    /// </param>
    public void Remove(View view)
    {
      this.InnerList.Remove(view);
      this.Parent.Controls.Remove(view);
      this.Reindex();
    }

    /// <summary>
    /// The remove at.
    /// </summary>
    /// <param name="index">
    /// The index.
    /// </param>
    public void RemoveAt(int index)
    {
      this.InnerList.RemoveAt(index);
      this.Parent.Controls.RemoveAt(index);
      this.Reindex();
    }

    #endregion

    #region Methods

    /// <summary>
    /// The reindex.
    /// </summary>
    internal void Reindex()
    {
      int i = 0;
      foreach (View view in this.InnerList)
      {
        view.Index = i++;
      }
    }

    #endregion
  }
}