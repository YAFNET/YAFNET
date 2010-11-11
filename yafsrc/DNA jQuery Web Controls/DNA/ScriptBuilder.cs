//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI
{
  #region Using

  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// ScriptBuilder is a helper class to build script string
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class ScriptBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The scripts.
    /// </summary>
    protected StringBuilder scripts = new StringBuilder();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets StringBuilder that store the script result
    /// </summary>
    public StringBuilder Scripts
    {
      get
      {
        return this.scripts;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Add the script to document.ready event
    /// </summary>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    public void AppendDocumentReadyScript(string script)
    {
      this.scripts.Append("jQuery().ready(function() {");
      this.scripts.Append(script);
      this.scripts.Append("});");
    }

    /// <summary>
    /// The append dot.
    /// </summary>
    /// <returns>
    /// </returns>
    public ScriptBuilder AppendDot()
    {
      this.scripts.Append(".");
      return this;
    }

    /// <summary>
    /// Generate the "$find" shortcut for the control
    /// </summary>
    /// <param name="control">
    /// Control instance
    /// </param>
    public ScriptBuilder AppendFindShortCut(Control control)
    {
      this.scripts.Append("$find('" + control.ClientID + "')");
      return this;
    }

    /// <summary>
    /// Add the function wrapper to the script
    /// </summary>
    /// <param name="functionName">
    /// </param>
    /// <param name="script">
    /// </param>
    public void AppendFunctionWrapper(string functionName, string script)
    {
      this.AppendFunctionWrapper(functionName, null, script);
    }

    /// <summary>
    /// Add the function wrapper to the script
    /// </summary>
    /// <param name="script">
    /// </param>
    public void AppendFunctionWrapper(string script)
    {
      this.AppendFunctionWrapper(null, null, script);
    }

    /// <summary>
    /// Add the function wrapper to the script
    /// </summary>
    /// <param name="functionName">
    /// </param>
    /// <param name="functionParams">
    /// </param>
    /// <param name="script">
    /// </param>
    public void AppendFunctionWrapper(string functionName, string[] functionParams, string script)
    {
      this.scripts.Append("function");
      if (!string.IsNullOrEmpty(functionName))
      {
        this.scripts.Append(" " + functionName);
      }

      this.scripts.Append("(");
      if (functionParams != null)
      {
        if (functionParams.Length > 0)
        {
          this.scripts.Append(string.Join(",", functionParams));
        }
      }

      this.scripts.Append(")");
      this.scripts.Append("{" + script + "}");
    }

    /// <summary>
    /// Generate the "$get" shortcut for the control
    /// </summary>
    /// <param name="control">
    /// Control instance
    /// </param>
    public ScriptBuilder AppendGetShortCut(Control control)
    {
      return AppendGetShortCut(control.ClientID);
    }

    /// <summary>
    /// The append get short cut.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// </returns>
    public ScriptBuilder AppendGetShortCut(string id)
    {
      this.scripts.Append("$get('" + id + "')");
      return this;
    }

    /// <summary>
    /// The append invoke element method.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="method">
    /// The method.
    /// </param>
    /// <param name="paramValues">
    /// The param values.
    /// </param>
    /// <returns>
    /// </returns>
    public virtual ScriptBuilder AppendInvokeElementMethod(Control control, string method, params string[] paramValues)
    {
      return AppendInvokeElementMethod(control.ClientID, method, paramValues);
    }

    /// <summary>
    /// The append invoke element method.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="method">
    /// The method.
    /// </param>
    /// <param name="paramValues">
    /// The param values.
    /// </param>
    /// <returns>
    /// </returns>
    public virtual ScriptBuilder AppendInvokeElementMethod(string id, string method, params string[] paramValues)
    {
      AppendGetShortCut(id).AppendDot();
      this.scripts.Append(method + "(" + string.Join(",", paramValues) + ");");
      return this;
    }

    /// <summary>
    /// The append set value.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AppendSetValue(Control control, string value)
    {
      AppendSetValue(control.ClientID, value);
    }

    /// <summary>
    /// The append set value.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AppendSetValue(string id, string value)
    {
      AppendGetShortCut(id);
      this.scripts.Append(".value=" + value + ";");
    }

    /// <summary>
    /// Add the script as anonymouse function to Sys.Application.Init event
    /// </summary>
    /// <remarks>
    /// This method will auto append "function(){...}" to wrap the script
    /// </remarks>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    /// <param name="anonymousFunction">
    /// Whether generate the "function()" wrapper
    /// </param>
    public void AppendSysInitScript(string script, bool anonymousFunction)
    {
      if (anonymousFunction)
      {
        this.AppendSysInitScript("function(){" + script + "}");
      }
      else
      {
        this.AppendSysInitScript(script);
      }
    }

    /// <summary>
    /// Add the script to Sys.Application.Init event
    /// </summary>
    /// <param name="script">
    /// </param>
    public void AppendSysInitScript(string script)
    {
      this.scripts.Append("Sys.Application.add_init(");
      this.scripts.Append(script);
      this.scripts.Append(");");
    }

    /// <summary>
    /// Add the script as anonymouse function to Sys.Application.Load event
    /// </summary>
    /// <remarks>
    /// This method will auto append "function(){...}" to wrap the script
    /// </remarks>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    /// <param name="anonymousFunction">
    /// Whether generate the "function()" wrapper
    /// </param>
    public void AppendSysLoadScript(string script, bool anonymousFunction)
    {
      if (anonymousFunction)
      {
        this.AppendSysLoadScript("function(){" + script + "}");
      }
      else
      {
        this.AppendSysLoadScript(script);
      }
    }

    /// <summary>
    /// Add the script to Sys.Application.Load event
    /// </summary>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    public void AppendSysLoadScript(string script)
    {
      this.scripts.Append("Sys.Application.add_load(");
      this.scripts.Append(script);
      this.scripts.Append(");");
    }

    /// <summary>
    /// Add the script as anonymouse function to Sys.Application.Unload event
    /// </summary>
    /// <remarks>
    /// This method will auto append "function(){...}" to wrap the script
    /// </remarks>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    /// <param name="anonymousFunction">
    /// Whether generate the "function()" wrapper
    /// </param>
    public void AppendSysUnloadScript(string script, bool anonymousFunction)
    {
      if (anonymousFunction)
      {
        this.AppendSysUnloadScript("function(){" + script + "}");
      }
      else
      {
        this.AppendSysUnloadScript(script);
      }
    }

    /// <summary>
    /// Add the script to Sys.Application.Unload event
    /// </summary>
    /// <param name="script">
    /// script body which need to register
    /// </param>
    public void AppendSysUnloadScript(string script)
    {
      this.scripts.Append("Sys.Application.add_unload(");
      this.scripts.Append(script);
      this.scripts.Append(");");
    }

    /// <summary>
    /// Get the script's result
    /// </summary>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
      return this.scripts.ToString();
    }

    #endregion
  }
}