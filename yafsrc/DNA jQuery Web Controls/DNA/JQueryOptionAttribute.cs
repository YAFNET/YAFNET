//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The j query option attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class JQueryOptionAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The default value.
    /// </summary>
    private object defaultValue;

    /// <summary>
    /// The function params.
    /// </summary>
    private string[] functionParams;

    /// <summary>
    /// The ignore value.
    /// </summary>
    private object ignoreValue;

    /// <summary>
    /// The name.
    /// </summary>
    private string name;

    /// <summary>
    /// The regist null value.
    /// </summary>
    private bool registNullValue;

    /// <summary>
    /// The target.
    /// </summary>
    private string target = string.Empty;

    /// <summary>
    /// The type.
    /// </summary>
    private JQueryOptionTypes type = JQueryOptionTypes.Value;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryOptionAttribute"/> class.
    /// </summary>
    public JQueryOptionAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryOptionAttribute"/> class.
    /// </summary>
    /// <param name="option">
    /// The option.
    /// </param>
    public JQueryOptionAttribute(string option)
    {
      this.name = option;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryOptionAttribute"/> class.
    /// </summary>
    /// <param name="option">
    /// The option.
    /// </param>
    /// <param name="valueType">
    /// The value type.
    /// </param>
    public JQueryOptionAttribute(string option, JQueryOptionTypes valueType)
    {
      this.name = option;
      this.type = valueType;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets DefaultValue.
    /// </summary>
    public object DefaultValue
    {
      get
      {
        return this.defaultValue;
      }

      set
      {
        this.defaultValue = value;
      }
    }

    /// <summary>
    /// Gets or sets FunctionParams.
    /// </summary>
    public string[] FunctionParams
    {
      get
      {
        return this.functionParams;
      }

      set
      {
        this.functionParams = value;
      }
    }

    /// <summary>
    /// Gets or sets IgnoreValue.
    /// </summary>
    public object IgnoreValue
    {
      get
      {
        return this.ignoreValue;
      }

      set
      {
        this.ignoreValue = value;
      }
    }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name
    {
      get
      {
        return this.name;
      }

      set
      {
        this.name = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether RegistNullValue.
    /// </summary>
    public bool RegistNullValue
    {
      get
      {
        return this.registNullValue;
      }

      set
      {
        this.registNullValue = value;
      }
    }

    /// <summary>
    /// Gets or sets Target.
    /// </summary>
    public string Target
    {
      get
      {
        return this.target;
      }

      set
      {
        this.target = value;
      }
    }

    /// <summary>
    /// Gets or sets Type.
    /// </summary>
    public JQueryOptionTypes Type
    {
      get
      {
        return this.type;
      }

      set
      {
        this.type = value;
      }
    }

    #endregion
  }
}