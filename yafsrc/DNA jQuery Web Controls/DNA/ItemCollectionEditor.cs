//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.Design
{
  #region Using

  using System;
  using System.Collections;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing;
  using System.Windows.Forms;

  #endregion

  /// <summary>
  /// Designer editor for Item collections.
  /// </summary>
  public class ItemCollectionEditor : CollectionEditor
  {
    #region Constants and Fields

    /// <summary>
    /// The _ edit value.
    /// </summary>
    private object _EditValue;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the ItemCollectionEditor class.
    /// </summary>
    /// <param name="type">
    /// The type of collection this object is to edit.
    /// </param>
    public ItemCollectionEditor(Type type)
      : base(type)
    {
      this._EditValue = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Edits the value of the specified object using the specified service provider and context.
    /// </summary>
    /// <param name="context">
    /// A type descriptor context that can be used to gain additional context information.
    /// </param>
    /// <param name="provider">
    /// A service provider object through which editing services may be obtained.
    /// </param>
    /// <param name="value">
    /// The object to edit the value of.
    /// </param>
    /// <returns>
    /// The new value of the object. If the value of the object hasn't changed, this should return the same object it was passed.
    /// </returns>
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      this._EditValue = value;
      return base.EditValue(context, provider, value);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new form to show the current collection.
    /// </summary>
    /// <returns>
    /// The form.
    /// </returns>
    protected override CollectionForm CreateCollectionForm()
    {
      CollectionForm form = base.CreateCollectionForm();
      this.InitializeForm(form);

      return form;
    }

    /// <summary>
    /// Destroys the specified instance of the object.
    /// </summary>
    /// <param name="instance">
    /// The object to destroy.
    /// </param>
    protected override void DestroyInstance(object instance)
    {
      // WORKAROUND: The base class tries to remove items twice, so
      // this will prevent them from throwing an error if the object
      // is not found.
      if (this._EditValue is IList)
      {
        if (((IList)this._EditValue).IndexOf(instance) < 0)
        {
          return;
        }
      }

      base.DestroyInstance(instance);
    }

    /// <summary>
    /// Initialize the CollectionForm. Called from CreateCollectionForm().
    /// </summary>
    /// <param name="form">
    /// The CollectionForm to initialize.
    /// </param>
    protected virtual void InitializeForm(CollectionForm form)
    {
      foreach (Control ctrl in form.Controls)
      {
        if (ctrl is PropertyGrid)
        {
          this.InitializePropertyGrid((PropertyGrid)ctrl);
          break;
        }
      }
    }

    /// <summary>
    /// Initialize the PropertyGrid. Called from InitializeForm().
    /// </summary>
    /// <param name="propGrid">
    /// The PropertyGrid.
    /// </param>
    protected virtual void InitializePropertyGrid(PropertyGrid propGrid)
    {
      propGrid.BackColor = SystemColors.Control;
      propGrid.HelpVisible = true;
      propGrid.ToolbarVisible = true;
    }

    #endregion
  }
}