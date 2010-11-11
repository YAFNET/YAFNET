//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.Script.Serialization;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  #endregion

  /// <summary>
  /// JQueryScriptBuilder provides the methods to builder the jQuery scripts
  /// </summary>
  /// <remarks>
  /// Roles: One control has one builder
  /// </remarks>
  /// <example>
  /// JQueryScriptBuilder jBuilder=new JQueryScriptBuilder(this);
  ///   jQueryScript.AddOption("range",true);
  ///   .....
  ///   ClientScriptManager.RegisterJQueryControl(ths,jBuilder);
  /// </example>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class JQueryScriptBuilder : ScriptBuilder, IScriptBuilder
  {
    #region Constants and Fields

    /// <summary>
    /// The options.
    /// </summary>
    private readonly Dictionary<string, string> options = new Dictionary<string, string>();

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
    /// The is builded.
    /// </summary>
    private bool isBuilded;

    /// <summary>
    /// The is prepared.
    /// </summary>
    private bool isPrepared;

    /// <summary>
    /// The register event.
    /// </summary>
    private ClientRegisterEvents registerEvent = ClientRegisterEvents.ApplicationLoad;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryScriptBuilder"/> class.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public JQueryScriptBuilder(Control control)
    {
      if (control == null)
      {
        throw new Exception("JQueryScriptBuilder must be bind to a ServerControl the control could not be null");
      }

      this.targetControl = control;
      this.page = control.Page;
      this._selector = new JQuerySelector(control);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQueryScriptBuilder"/> class.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public JQueryScriptBuilder(Control control, JQuerySelector selector)
    {
      if (control == null)
      {
        throw new Exception("JQueryScriptBuilder must be bind to a ServerControl the control could not be null");
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
    /// Gets a value indicating whether IsPrepared.
    /// </summary>
    public bool IsPrepared
    {
      get
      {
        return this.isPrepared;
      }
    }

    /// <summary>
    /// Gets or sets RegisterEvent.
    /// </summary>
    public ClientRegisterEvents RegisterEvent
    {
      get
      {
        return this.registerEvent;
      }

      set
      {
        this.registerEvent = value;
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

    #region Public Methods

    /// <summary>
    /// The add function option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    public void AddFunctionOption(string name, string script, string[] functionParams)
    {
      var sb = new StringBuilder();
      sb.Append("function");

      sb.Append("(");
      if (functionParams != null)
      {
        if (functionParams.Length > 0)
        {
          sb.Append(string.Join(",", functionParams));
        }
      }

      sb.Append(")");
      sb.Append("{" + script + "}");
      AddOption(name, sb.ToString());
    }

    /// <summary>
    /// The add option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AddOption(string name, object value)
    {
      if (value == null)
      {
        return;
      }

      Type t = value.GetType();
      if (t == typeof(string))
      {
        AddOption(name, value.ToString(), true);
        return;
      }

      if (t == typeof(string[]))
      {
        this.AddOption(name, (string[])value);
        return;
      }

      if (t == typeof(int))
      {
        this.AddOption(name, (int)value);
        return;
      }

      if (t == typeof(int[]))
      {
        this.AddOption(name, (int[])value);
        return;
      }

      if (t == typeof(float))
      {
        this.AddOption(name, (float)value);
        return;
      }

      if (t == typeof(float[]))
      {
        this.AddOption(name, (float[])value);
        return;
      }

      if (t == typeof(decimal))
      {
        this.AddOption(name, (decimal)value);
        return;
      }

      if (t == typeof(bool))
      {
        this.AddOption(name, (bool)value);
        return;
      }

      if (t.IsEnum)
      {
        this.AddOption(name, (Enum)value);
        return;
      }
    }

    /// <summary>
    /// Append the name and string value to option string
    /// </summary>
    /// <remarks>
    /// the value will be convert to 'value' format
    /// </remarks>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of string
    /// </param>
    public void AddOption(string name, string value)
    {
      this.options.Add(name, value);
    }

    /// <summary>
    /// The add option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="format">
    /// The format.
    /// </param>
    public void AddOption(string name, string value, bool format)
    {
      if (format)
      {
        this.AddOption(name, "'" + value + "'");
      }
      else
      {
        AddOption(name, value);
      }
    }

    /// <summary>
    /// Append the name and int value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of integer
    /// </param>
    public void AddOption(string name, int value)
    {
      this.options.Add(name, value.ToString());
    }

    /// <summary>
    /// Append the name and float value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of float
    /// </param>
    public void AddOption(string name, float value)
    {
      this.options.Add(name, value.ToString());
    }

    /// <summary>
    /// Append the name and decimal value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of decimal
    /// </param>
    public void AddOption(string name, decimal value)
    {
      this.options.Add(name, value.ToString());
    }

    /// <summary>
    /// Append the name and bool value to option string
    /// </summary>
    /// <remarks>
    /// the value will be convert to low case string.
    /// </remarks>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of bool
    /// </param>
    public void AddOption(string name, bool value)
    {
      this.options.Add(name, value.ToString().ToLower());
    }

    /// <summary>
    /// Append the name and string array value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of string array
    /// </param>
    public void AddOption(string name, string[] value)
    {
      var formatted = new string[value.Length];
      for (int i = 0; i < value.Length; i++)
      {
        formatted[i] = "'" + value[i] + "'";
      }

      this.options.Add(name, "[" + string.Join(",", value) + "]");
    }

    /// <summary>
    /// Append the name and int array value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of int array
    /// </param>
    public void AddOption(string name, int[] value)
    {
      var strValues = new string[value.Length];
      for (int i = 0; i < value.Length; i++)
      {
        strValues[i] = value[i].ToString();
      }

      this.options.Add(name, "[" + string.Join(",", strValues) + "]");
    }

    /// <summary>
    /// Append the name and float array value to option string
    /// </summary>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of float array
    /// </param>
    public void AddOption(string name, float[] value)
    {
      var strValues = new string[value.Length];
      for (int i = 0; i < value.Length; i++)
      {
        strValues[i] = value[i].ToString();
      }

      this.options.Add(name, "[" + string.Join(",", strValues) + "]");
    }

    /// <summary>
    /// Append the name and Enum object value to option string
    /// </summary>
    /// <remarks>
    /// the Enum object will be convert to low case string.
    /// </remarks>
    /// <param name="name">
    /// option name
    /// </param>
    /// <param name="value">
    /// option value of Enum object
    /// </param>
    public void AddOption(string name, Enum value)
    {
      this.options.Add(name, "'" + value.ToString().ToLower() + "'");
    }

    /// <summary>
    /// The add option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="isStringValue">
    /// The is string value.
    /// </param>
    public void AddOption(string name, Unit value, bool isStringValue)
    {
      if (isStringValue)
      {
        AddOption(name, value.ToString(), true);
      }
      else
      {
        AddOption(name, value.Value);
      }
    }

    /// <summary>
    /// The add selector option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="selector">
    /// The selector.
    /// </param>
    public void AddSelectorOption(string name, JQuerySelector selector)
    {
      AddOption(name, selector.ToString(this.Page), true);
    }

    /// <summary>
    /// The add selector option.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="controlID">
    /// The control id.
    /// </param>
    public void AddSelectorOption(string name, string controlID)
    {
      var s = new JQuerySelector();
      s.TargetID = controlID;
      AddOption(name, s);
    }

    /// <summary>
    /// The append attr.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AppendAttr(string name, string value)
    {
      this.AppendAttr(this.Selector, name, value);
    }

    /// <summary>
    /// The append attr.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AppendAttr(Control control, string name, string value)
    {
      this.AppendAttr(new JQuerySelector(control), name, value);
    }

    /// <summary>
    /// The append attr.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public void AppendAttr(JQuerySelector selector, string name, string value)
    {
      AppendSelector(selector);
      this.scripts.Append(".attr('" + name + "'," + value + ");");
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(string eventName, string script)
    {
      this.AppendBindFunction(this.Selector, eventName, script);
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(string eventName, string[] functionParams, string script)
    {
      this.AppendBindFunction(this.Selector, eventName, functionParams, script);
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(Control control, string eventName, string script)
    {
      AppendBindFunction(control, eventName, null, script);
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(JQuerySelector selector, string eventName, string script)
    {
      AppendBindFunction(selector, eventName, null, script);
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(Control control, string eventName, string[] functionParams, string script)
    {
      this.AppendBindFunction(new JQuerySelector(control), eventName, functionParams, script);
    }

    /// <summary>
    /// The append bind function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendBindFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
    {
      this.AppendEventHandler(selector, "bind", eventName, functionParams, script);
    }

    /// <summary>
    /// The append css style.
    /// </summary>
    /// <param name="styles">
    /// The styles.
    /// </param>
    public void AppendCssStyle(params string[] styles)
    {
      this.AppendCssStyle(this.Selector, styles);
    }

    /// <summary>
    /// The append css style.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="styles">
    /// The styles.
    /// </param>
    public void AppendCssStyle(JQuerySelector selector, params string[] styles)
    {
      AppendSelector(selector);
      this.scripts.Append(".css({" + string.Join(",", styles) + "});");
    }

    /// <summary>
    /// Generate then css style for jQuery object
    /// </summary>
    /// <param name="control">
    /// </param>
    /// <param name="styles">
    /// sets the style string to jQuery object. this param must be using 'key':'name' format;
    /// </param>
    public void AppendCssStyle(Control control, params string[] styles)
    {
      AppendSelector(control);
      this.scripts.Append(".css({" + string.Join(",", styles) + "});");
    }

    /// <summary>
    /// The append die function.
    /// </summary>
    public void AppendDieFunction()
    {
      this.AppendDieFunction(this.Selector);
    }

    /// <summary>
    /// The append die function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    public void AppendDieFunction(Control control)
    {
      this.AppendDieFunction(new JQuerySelector(control));
    }

    /// <summary>
    /// The append die function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    public void AppendDieFunction(JQuerySelector selector)
    {
      AppendSelector(selector);
      this.scripts.Append(".die();");
    }

    /// <summary>
    /// The append event handler.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="functionName">
    /// The function name.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendEventHandler(
      JQuerySelector selector, string functionName, string eventName, string[] functionParams, string script)
    {
      AppendSelector(selector);
      this.scripts.Append("." + functionName + "('" + eventName + "',");
      this.AppendFunctionWrapper(null, functionParams, script);
      this.scripts.Append(");");
    }

    /// <summary>
    /// The append invoke method with options.
    /// </summary>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendInvokeMethodWithOptions(string methodName)
    {
      return AppendInvokeMethodWithOptions(methodName, null);
    }

    /// <summary>
    /// The append invoke method with options.
    /// </summary>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <param name="optionParams">
    /// The option params.
    /// </param>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendInvokeMethodWithOptions(string methodName, params string[] optionParams)
    {
      return this.AppendInvokeMethodWithOptions(this.Selector, methodName, optionParams);
    }

    /// <summary>
    /// The append invoke method with options.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="methodName">
    /// The method name.
    /// </param>
    /// <param name="optionParams">
    /// The option params.
    /// </param>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendInvokeMethodWithOptions(
      JQuerySelector selector, string methodName, params string[] optionParams)
    {
      AppendSelector(selector).AppendDot().Scripts.Append(methodName).Append("(");
      if (optionParams != null)
      {
        if (optionParams.Length > 0)
        {
          this.scripts.Append("{" + string.Join(",", optionParams) + "}");
        }
      }

      this.scripts.Append(");");
      return this;
    }

    /// <summary>
    /// The append live function.
    /// </summary>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendLiveFunction(string eventName, string script)
    {
      this.AppendLiveFunction(this.Selector, eventName, script);
    }

    /// <summary>
    /// The append live function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendLiveFunction(JQuerySelector selector, string eventName, string script)
    {
      AppendLiveFunction(selector, eventName, null, script);
    }

    /// <summary>
    /// The append live function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendLiveFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
    {
      this.AppendEventHandler(selector, "live", eventName, functionParams, script);
    }

    /// <summary>
    /// The append live function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendLiveFunction(Control control, string eventName, string script)
    {
      AppendLiveFunction(control, eventName, null, script);
    }

    /// <summary>
    /// The append live function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendLiveFunction(Control control, string eventName, string[] functionParams, string script)
    {
      this.AppendLiveFunction(new JQuerySelector(control), eventName, functionParams, script);
    }

    /// <summary>
    /// The append one function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendOneFunction(Control control, string eventName, string script)
    {
      AppendOneFunction(control, eventName, null, script);
    }

    /// <summary>
    /// The append one function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendOneFunction(Control control, string eventName, string[] functionParams, string script)
    {
      this.AppendOneFunction(new JQuerySelector(control), eventName, functionParams, script);
    }

    /// <summary>
    /// The append one function.
    /// </summary>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendOneFunction(string eventName, string script)
    {
      this.AppendOneFunction(this.Selector, eventName, script);
    }

    /// <summary>
    /// The append one function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendOneFunction(JQuerySelector selector, string eventName, string script)
    {
      this.AppendOneFunction(this.Selector, eventName, null, script);
    }

    /// <summary>
    /// The append one function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <param name="eventName">
    /// The event name.
    /// </param>
    /// <param name="functionParams">
    /// The function params.
    /// </param>
    /// <param name="script">
    /// The script.
    /// </param>
    public void AppendOneFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
    {
      this.AppendEventHandler(selector, "one", eventName, functionParams, script);
    }

    /// <summary>
    /// Append all option to the result string and the option cache will be clear.
    /// </summary>
    public void AppendOptionsToResult()
    {
      if (this.options.Count > 0)
      {
        this.scripts.Append("{");
        var optionArray = new string[this.options.Count];
        int i = 0;
        foreach (string key in this.options.Keys)
        {
          optionArray[i++] = key + ":" + this.options[key];
        }

        this.scripts.Append(string.Join(",", optionArray));
        this.scripts.Append("}");
      }

      this.options.Clear();
    }

    /// <summary>
    /// Append the jQuery selector string of Control
    /// </summary>
    /// <remarks>
    /// this function will get the control's clientId to build the selector string.
    /// </remarks>
    /// <example>
    /// AppendJQuerySelector(TextBox1);
    ///   //Result: $('#TextBox1')
    /// </example>
    /// <param name="control">
    /// </param>
    public JQueryScriptBuilder AppendSelector(Control control)
    {
      return AppendSelector(control.ClientID);
    }

    /// <summary>
    /// The append selector.
    /// </summary>
    /// <param name="clientID">
    /// The client id.
    /// </param>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendSelector(string clientID)
    {
      this.scripts.Append("$('#" + clientID + "')");
      return this;
    }

    /// <summary>
    /// The append selector.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendSelector(JQuerySelector selector)
    {
      this.scripts.Append(selector.ToString(this.page));
      return this;
    }

    /// <summary>
    /// The append selector.
    /// </summary>
    /// <returns>
    /// </returns>
    public JQueryScriptBuilder AppendSelector()
    {
      this.scripts.Append(this.Selector.ToString(this.Page));
      return this;
    }

    /// <summary>
    /// The append unbind function.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    public void AppendUnbindFunction(JQuerySelector selector)
    {
      AppendSelector(selector);
      this.scripts.Append(".unbind();");
    }

    /// <summary>
    /// The append unbind function.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    public void AppendUnbindFunction(Control control)
    {
      this.AppendUnbindFunction(new JQuerySelector(control));
    }

    /// <summary>
    /// The append unbind function.
    /// </summary>
    public void AppendUnbindFunction()
    {
      this.AppendUnbindFunction(this.Selector);
    }

    /// <summary>
    /// The prepare.
    /// </summary>
    /// <param name="jQueryAttr">
    /// The j query attr.
    /// </param>
    public void Prepare(JQueryAttribute jQueryAttr)
    {
      if (this.isPrepared)
      {
        return;
      }

      if (jQueryAttr != null)
      {
        string name = jQueryAttr.Name;
        this.registerEvent = jQueryAttr.StartEvent;
        Type controlType = this.targetControl.GetType();

        this.AppendSelector(this.Selector);

        this.scripts.Append("." + name);

        var properties =
          from PropertyInfo p in
            controlType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          where Attribute.GetCustomAttribute(p, typeof(JQueryOptionAttribute), true) != null
          select p;

        foreach (PropertyInfo pi in properties)
        {
          var option = (JQueryOptionAttribute)Attribute.GetCustomAttribute(pi, typeof(JQueryOptionAttribute));
          if (!string.IsNullOrEmpty(option.Target))
          {
            if (option.Target != jQueryAttr.Name)
            {
              continue;
            }
          }

          object pValue = pi.GetValue(this.targetControl, null);
          if (pValue == null)
          {
            if (option.DefaultValue != null)
            {
              pValue = option.DefaultValue;
            }

            if (pValue == null)
            {
              if (!option.RegistNullValue)
              {
                continue;
              }
            }
          }

          if (this.IsIgnoreValue(option.IgnoreValue, pValue, pi.PropertyType))
          {
            continue;
          }

          

          switch (option.Type)
          {
            case JQueryOptionTypes.Value:

              if (pi.PropertyType == typeof(JQuerySelector))
              {
                var _s = pValue as JQuerySelector;
                if (!_s.IsEmpty)
                {
                  AddOption(option.Name, _s.ToString(this.Page));
                }

                continue;
              }

              if (pi.PropertyType == typeof(Position))
              {
                string pos = pValue.ToString();
                if (!string.IsNullOrEmpty(pos))
                {
                  AddOption(option.Name, pos);
                }

                continue;
              }

              if (pi.PropertyType == typeof(Unit))
              {
                var unit = (Unit)pValue;
                if (unit.IsEmpty)
                {
                  continue;
                }

                AddOption(option.Name, unit, true);
              }

              if (pi.PropertyType == typeof(JQueryEffects))
              {
                if (pValue.ToString().ToLower() == "none")
                {
                  continue;
                }

                AddOption(option.Name, pValue.ToString().ToLower());
                continue;
              }

              AddOption(option.Name, pValue);
              break;

            case JQueryOptionTypes.JSONObject:
              if (pi.PropertyType == typeof(string))
              {
                this.AddOption(option.Name, (string)pValue);
              }
              else
              {
                this.AddOption(option.Name, (new JavaScriptSerializer()).Serialize(pValue));
              }

              break;

            case JQueryOptionTypes.Function:
              this.AddFunctionOption(option.Name, pValue.ToString(), option.FunctionParams);
              break;
            default:
              AddOption(option.Name, pValue);
              break;
          }
        }

        
      }

      this.isPrepared = true;
    }

    #endregion

    #region Implemented Interfaces

    #region IScriptBuilder

    /// <summary>
    /// The build.
    /// </summary>
    public void Build()
    {
      if (this.isBuilded == false)
      {
        this.scripts.Append("(");
        this.AppendOptionsToResult();
        this.scripts.Append(");");
        this.isBuilded = true;
      }
    }

    /// <summary>
    /// The get applicaiton init script.
    /// </summary>
    /// <returns>
    /// The get applicaiton init script.
    /// </returns>
    public string GetApplicaitonInitScript()
    {
      if (this.registerEvent == ClientRegisterEvents.ApplicaitonInit)
      {
        return this.ToString();
      }

      return string.Empty;
    }

    /// <summary>
    /// The get application load script.
    /// </summary>
    /// <returns>
    /// The get application load script.
    /// </returns>
    public string GetApplicationLoadScript()
    {
      if (this.registerEvent == ClientRegisterEvents.ApplicationLoad)
      {
        return this.ToString();
      }

      return string.Empty;
    }

    /// <summary>
    /// The get document ready script.
    /// </summary>
    /// <returns>
    /// The get document ready script.
    /// </returns>
    public string GetDocumentReadyScript()
    {
      if (this.registerEvent == ClientRegisterEvents.DocumentReady)
      {
        return this.ToString();
      }

      return string.Empty;
    }

    /// <summary>
    /// The prepare.
    /// </summary>
    /// <exception cref="Exception">
    /// </exception>
    public void Prepare()
    {
      Type controlType = this.targetControl.GetType();
      object[] attrs = controlType.GetCustomAttributes(typeof(JQueryAttribute), true);

      if (attrs == null)
      {
        throw new Exception(
          "Could not build this control's jQuery script. JQueryAttribute declare not found in this control.");
      }

      var jQueryAttr = attrs[0] as JQueryAttribute;
      this.Prepare(jQueryAttr);
    }

    /// <summary>
    /// The reset.
    /// </summary>
    public void Reset()
    {
      this.scripts = new StringBuilder();
      this.options.Clear();
      this.isPrepared = false;
      this.isBuilded = false;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The is ignore value.
    /// </summary>
    /// <param name="compareValue">
    /// The compare value.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="valueType">
    /// The value type.
    /// </param>
    /// <returns>
    /// The is ignore value.
    /// </returns>
    private bool IsIgnoreValue(object compareValue, object value, Type valueType)
    {
      // Ignore value
      if (compareValue != null)
      {
        if (valueType == typeof(bool))
        {
          if ((bool)compareValue == (bool)value)
          {
            return true;
          }
        }

        if (valueType == typeof(int))
        {
          if ((int)compareValue == (int)value)
          {
            return true;
          }
        }

        if (valueType == typeof(float))
        {
          if (Convert.ToDecimal(compareValue) == Convert.ToDecimal(value))
          {
            return true;
          }
        }

        if (valueType == typeof(string))
        {
          if (compareValue.ToString() == valueType.ToString())
          {
            return true;
          }
        }

        if (compareValue == value)
        {
          return true;
        }
      }

      return false;
    }

    #endregion
  }
}