//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery.Design
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.Drawing.Design;
  using System.Windows.Forms;
  using System.Windows.Forms.Design;

  #endregion

  /// <summary>
  /// The animation for show editor.
  /// </summary>
  public class AnimationForShowEditor : UITypeEditor
  {
    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsDropDownResizable.
    /// </summary>
    public override bool IsDropDownResizable
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The edit value.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="provider">
    /// The provider.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// The edit value.
    /// </returns>
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      if (provider != null)
      {
        // This service is in charge of popping our dropdown.
        var service1 = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

        if (service1 != null)
        {
          var animDropDown = new AnimationsListBox();

          // Drop the list control.
          service1.DropDownControl(animDropDown);

          if (animDropDown.SelectedItem != null)
          {
            value = animDropDown.SelectedItem.ToString();
          }

          // Close the list control after selection.
          service1.CloseDropDown();
        }
      }

      return value;
    }

    /// <summary>
    /// The get edit style.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// </returns>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    #endregion

    /// <summary>
    /// The animations list box.
    /// </summary>
    private class AnimationsListBox : ListBox
    {
      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="AnimationsListBox"/> class.
      /// </summary>
      public AnimationsListBox()
      {
        this.BorderStyle = BorderStyle.None;
        this.Items.AddRange(new object[] { "slideDown", "show", "toggle", "fadeIn" });
      }

      #endregion
    }
  }
}