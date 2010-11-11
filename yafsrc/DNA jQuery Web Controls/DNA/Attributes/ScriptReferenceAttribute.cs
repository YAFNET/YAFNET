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
  /// The script reference attribute.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ScriptReferenceAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The _assembly.
    /// </summary>
    private string _assembly = string.Empty;

    /// <summary>
    /// The _name.
    /// </summary>
    private string _name = string.Empty;

    /// <summary>
    /// The load order.
    /// </summary>
    private int loadOrder = 1;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptReferenceAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public ScriptReferenceAttribute(string name)
    {
      this._name = name;
      this._assembly = this.GetType().Assembly.FullName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptReferenceAttribute"/> class.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="assembly">
    /// The assembly.
    /// </param>
    public ScriptReferenceAttribute(string name, string assembly)
    {
      this._name = name;
      this._assembly = assembly;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets Assembly.
    /// </summary>
    public string Assembly
    {
      get
      {
        return this._assembly;
      }

      set
      {
        this._assembly = value;
      }
    }

    /// <summary>
    /// Gets or sets LoadOrder.
    /// </summary>
    public int LoadOrder
    {
      get
      {
        return this.loadOrder;
      }

      set
      {
        this.loadOrder = value;
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

    #endregion
  }
}