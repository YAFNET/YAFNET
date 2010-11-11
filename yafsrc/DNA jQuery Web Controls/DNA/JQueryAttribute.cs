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
  /// JQueryAttribute for jQuery Web Control
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class JQueryAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The _assembly.
    /// </summary>
    private string _assembly = string.Empty;

    /// <summary>
    /// The _dispose method.
    /// </summary>
    private string _disposeMethod = string.Empty;

    /// <summary>
    /// The name.
    /// </summary>
    private string name = string.Empty;

    /// <summary>
    /// The script resource base name.
    /// </summary>
    private string scriptResourceBaseName = "jQueryNet.";

    /// <summary>
    /// The start event.
    /// </summary>
    private ClientRegisterEvents startEvent = ClientRegisterEvents.ApplicaitonInit;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the script resource 's assembly name
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
    ///   Gets/Sets the jQuery dispose method name.
    /// </summary>
    /// <remarks>
    ///   if this property sets the dispose method for jquery will invoke when application.unload.
    /// </remarks>
    public string DisposeMethod
    {
      get
      {
        return this._disposeMethod;
      }

      set
      {
        this._disposeMethod = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the jQuery (plugins/widgets) 's name
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
    ///   Gets/Sets the jQuery scriptResource file base name
    /// </summary>
    public string ScriptResourceBaseName
    {
      get
      {
        return this.scriptResourceBaseName;
      }

      set
      {
        this.scriptResourceBaseName = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the script resource name array
    /// </summary>
    public string[] ScriptResources { get; set; }

    /// <summary>
    ///   Gets/Sets the client register event for jQuery
    /// </summary>
    public ClientRegisterEvents StartEvent
    {
      get
      {
        return this.startEvent;
      }

      set
      {
        this.startEvent = value;
      }
    }

    #endregion
  }
}