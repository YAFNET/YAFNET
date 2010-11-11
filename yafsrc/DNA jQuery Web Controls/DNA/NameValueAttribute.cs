//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;

  #endregion

  /// <summary>
  /// Name Value pair attribute object
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class NameValueAttribute : StateManagedObject
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NameValueAttribute"/> class.
    /// </summary>
    public NameValueAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NameValueAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public NameValueAttribute(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the name of the attribute
    /// </summary>
    [Description("Gets/Sets the name of the attribute")]
    [Category("Data")]
    public virtual string Name
    {
      get
      {
        object obj = this.ViewState["Name"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["Name"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the value of the attribute
    /// </summary>
    [Description("Gets/Sets the value of the attribute")]
    [Category("Data")]
    public string Value
    {
      get
      {
        object obj = this.ViewState["Value"];
        return (obj == null) ? String.Empty : (string)obj;
      }

      set
      {
        this.ViewState["Value"] = value;
      }
    }

    #endregion
  }
}