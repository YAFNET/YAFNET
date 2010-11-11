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
  /// The date format editor.
  /// </summary>
  public class DateFormatEditor : UITypeEditor
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
          var formatDropDown = new DateFormatListBox();

          // Drop the list control.
          service1.DropDownControl(formatDropDown);

          if (formatDropDown.SelectedItem != null)
          {
            string[] values = formatDropDown.SelectedItem.ToString().Split(new[] { '|' });
            value = values[1].Trim();
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
    /// The date format list box.
    /// </summary>
    private class DateFormatListBox : ListBox
    {
      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="DateFormatListBox"/> class.
      /// </summary>
      public DateFormatListBox()
      {
        this.BorderStyle = BorderStyle.None;
        this.Items.AddRange(
          new object[]
            {
              "Default - MM/dd/yyyy | MM/dd/yyyy", "ISO 8601 - yy-mm-dd | yyyy-MM-dd", "Short - d MMM yy | d MMM, yy", 
              "Medium - d MMMM, yy | d MMMM, yy", "Full - dddd, d MMMM, yyyy | dddd, d MMMM, yyyy"
            });
      }

      #endregion
    }
  }
}