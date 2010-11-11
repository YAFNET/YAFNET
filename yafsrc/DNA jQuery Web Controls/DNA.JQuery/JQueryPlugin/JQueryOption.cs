//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;

  #endregion

  /// <summary>
  /// Define the jQuery option parameters
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class JQueryOption : StateManagedObject
  {
    // private string name;
    // private string _value = null;
    // private JQueryOptionTypes type = JQueryOptionTypes.Value;
    #region Properties

    /// <summary>
    ///   Gets/Sets the option name using in script
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the option name using in script")]
    public string Name
    {
      get
      {
        object obj = this.ViewState["Name"];
        return (obj == null) ? string.Empty : (string)obj;
      }

      set
      {
        this.ViewState["Name"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the option value data type
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the option value data type")]
    public JavaScriptTypes Type
    {
      get
      {
        object obj = this.ViewState["Type"];
        return (obj == null) ? JavaScriptTypes.String : (JavaScriptTypes)obj;
      }

      set
      {
        this.ViewState["Type"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the option value
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets the option value ")]
    public string Value
    {
      get
      {
        object obj = this.ViewState["Value"];
        return (obj == null) ? string.Empty : (string)obj;
      }

      set
      {
        this.ViewState["Value"] = value;
      }
    }

    #endregion
  }
}