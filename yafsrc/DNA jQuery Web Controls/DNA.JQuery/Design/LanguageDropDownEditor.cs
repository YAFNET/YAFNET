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
  /// The language drop down editor.
  /// </summary>
  public class LanguageDropDownEditor : UITypeEditor
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
          var langDropDown = new LanguageListBox();

          // Drop the list control.
          service1.DropDownControl(langDropDown);

          if (langDropDown.SelectedItem != null)
          {
            string[] values = langDropDown.SelectedItem.ToString().Split(new[] { ',' });
            value = values[1];
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
    /// The language list box.
    /// </summary>
    private class LanguageListBox : ListBox
    {
      #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="LanguageListBox"/> class.
      /// </summary>
      public LanguageListBox()
      {
        this.BorderStyle = BorderStyle.None;
        this.Items.AddRange(
          new object[]
            {
              "Arabic(لعربي),ar", "Armenian(Հայերեն),hy", "Albanian (Gjuha shqipe),sq", "Bulgarian(български език),bg", 
              "Catalan(Català),ca", "Chinese Simplified (简体中文),zh-CN", "Chinese Traditional (繁体中文),zh-TW", 
              "Croatian (Hrvatski jezik),hr", "Czech (Ceötina),cs", "Danish (Dansk),da", "Dutch (Nederlands),nl", 
              "Esperanto,eo", "Farsi/Persian (فارسی),fa", "Finnish (suomi),fi", "French (Français),fr", 
              "German (Deutsch),de", "Greek (Ελληνικά),el", "Hebrew (עברית),he", "Hungarian (Magyar),hu", 
              "Icelandic (Õslenska),is", "Indonesian (Bahasa Indonesia),id", "Italian (Italiano),it", 
              "Japanese (日本語),ja", "Korean (한국어),ko", "Latvian (Latvieöu Valoda),lv", "Lithuanian (lietuviu kalba),lt", 
              "Malaysian (Bahasa Malaysia),ms", "Norwegian (Norsk),no", "Polish (Polski),pl", 
              "Portuguese/Brazilian (Português),pt-BR", "Romanian (Română),ro", "Russian (Русский),ru", 
              "Serbian (српски језик),sr", "Serbian (srpski jezik),sr-SR", "Slovak (Slovencina),sk", 
              "Slovenian (Slovenski Jezik),sl", "Spanish (Español),es", "Swedish (Svenska),sv", "Thai (ภาษาไทย),th", 
              "Turkish (Türkçe),tr", "Ukranian (Українська),uk"
            });
      }

      #endregion
    }
  }
}