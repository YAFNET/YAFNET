// Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The client property attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ClientPropertyAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The _name.
    /// </summary>
    private string _name = string.Empty;

    /// <summary>
    /// The default value.
    /// </summary>
    private object defaultValue;

    /// <summary>
    /// The is url.
    /// </summary>
    private bool isUrl;

    /// <summary>
    /// The regist null value.
    /// </summary>
    private bool registNullValue;

    /// <summary>
    /// The type.
    /// </summary>
    private ClientPropertyTypes type = ClientPropertyTypes.Property;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientPropertyAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public ClientPropertyAttribute(string name)
    {
      this._name = name;
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
    /// Gets or sets a value indicating whether IsUrl.
    /// </summary>
    public bool IsUrl
    {
      get
      {
        return this.isUrl;
      }

      set
      {
        this.isUrl = value;
      }
    }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name
    {
      get
      {
        return this._name;
      }

      set
      {
        this._name = value;
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
    /// Gets or sets Type.
    /// </summary>
    public ClientPropertyTypes Type
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