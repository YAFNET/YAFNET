//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// Handling the WebControl has more than one JQueryAttribute define
  /// </summary>
  public class MultiJQueryScriptBuilder : IScriptBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The naming builders.
    /// </summary>
    private readonly Dictionary<string, JQueryScriptBuilder> namingBuilders =
      new Dictionary<string, JQueryScriptBuilder>();

    /// <summary>
    /// The page.
    /// </summary>
    private readonly Page page;

    /// <summary>
    /// The target control.
    /// </summary>
    private readonly Control targetControl;

    /// <summary>
    /// The _selector.
    /// </summary>
    private JQuerySelector _selector;

    /// <summary>
    /// The doc ready script.
    /// </summary>
    private string docReadyScript = string.Empty;

    /// <summary>
    /// The init script.
    /// </summary>
    private string initScript = string.Empty;

    /// <summary>
    /// The is builded.
    /// </summary>
    private bool isBuilded;

    /// <summary>
    /// The is prepared.
    /// </summary>
    private bool isPrepared;

    /// <summary>
    /// The load script.
    /// </summary>
    private string loadScript = string.Empty;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiJQueryScriptBuilder"/> class.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public MultiJQueryScriptBuilder(Control control)
    {
      if (control == null)
      {
        throw new Exception("MultiJQueryScriptBuilder must be bind to a ServerControl the control could not be null");
      }

      this.targetControl = control;
      this.page = control.Page;
      this._selector = new JQuerySelector(control);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiJQueryScriptBuilder"/> class.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public MultiJQueryScriptBuilder(Control control, JQuerySelector selector)
    {
      if (control == null)
      {
        throw new Exception("MultiJQueryScriptBuilder must be bind to a ServerControl the control could not be null");
      }

      this.targetControl = control;
      this.page = control.Page;
      this._selector = selector;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsBuilded.
    /// </summary>
    public bool IsBuilded
    {
      get
      {
        return this.isBuilded;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsPrepared.
    /// </summary>
    public bool IsPrepared
    {
      get
      {
        return this.isPrepared;
      }

      set
      {
        this.isPrepared = value;
      }
    }

    /// <summary>
    /// Gets NamingBuilders.
    /// </summary>
    public Dictionary<string, JQueryScriptBuilder> NamingBuilders
    {
      get
      {
        return this.namingBuilders;
      }
    }

    /// <summary>
    /// Gets or sets Selector.
    /// </summary>
    public JQuerySelector Selector
    {
      get
      {
        return this._selector;
      }

      set
      {
        this._selector = value;
      }
    }

    /// <summary>
    /// Gets Page.
    /// </summary>
    protected Page Page
    {
      get
      {
        return this.page;
      }
    }

    /// <summary>
    /// Gets TargetControl.
    /// </summary>
    protected Control TargetControl
    {
      get
      {
        return this.targetControl;
      }
    }

    #endregion

    #region Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    public JQueryScriptBuilder this[string name]
    {
      get
      {
        return this.NamingBuilders[name];
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IScriptBuilder

    /// <summary>
    /// The build.
    /// </summary>
    public void Build()
    {
      if (this.isBuilded)
      {
        return;
      }

      foreach (string key in this.namingBuilders.Keys)
      {
        this[key].Build();
        this.initScript += this[key].GetApplicaitonInitScript();
        this.loadScript += this[key].GetApplicationLoadScript();
        this.docReadyScript += this[key].GetDocumentReadyScript();
      }

      this.isBuilded = true;
    }

    /// <summary>
    /// The get applicaiton init script.
    /// </summary>
    /// <returns>
    /// The get applicaiton init script.
    /// </returns>
    public string GetApplicaitonInitScript()
    {
      return this.initScript;
    }

    /// <summary>
    /// The get application load script.
    /// </summary>
    /// <returns>
    /// The get application load script.
    /// </returns>
    public string GetApplicationLoadScript()
    {
      return this.loadScript;
    }

    /// <summary>
    /// The get document ready script.
    /// </summary>
    /// <returns>
    /// The get document ready script.
    /// </returns>
    public string GetDocumentReadyScript()
    {
      return this.docReadyScript;
    }

    /// <summary>
    /// The prepare.
    /// </summary>
    public void Prepare()
    {
      if (this.isPrepared)
      {
        return;
      }

      Type controlType = this.TargetControl.GetType();
      object[] attrs = controlType.GetCustomAttributes(typeof(JQueryAttribute), true);

      foreach (JQueryAttribute jqueryAttr in attrs)
      {
        var jbuilder = new JQueryScriptBuilder(this.TargetControl, this.Selector);
        jbuilder.Prepare(jqueryAttr);
        this.namingBuilders.Add(jqueryAttr.Name, jbuilder);
      }

      this.isPrepared = true;
    }

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this.initScript = string.Empty;
      this.loadScript = string.Empty;
      this.docReadyScript = string.Empty;
      this.namingBuilders.Clear();
      this.isPrepared = false;
      this.isBuilded = false;
    }

    #endregion

    #endregion
  }
}