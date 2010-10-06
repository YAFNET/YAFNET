
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;
using System.Drawing.Design;
using System.Reflection;
using System.Linq;

namespace DNA.UI.JQuery
{
    /// <summary>
    ///  JQueryScriptBuilder provides the methods to builder the jQuery scripts
    /// </summary>
    /// <remarks>
    /// Roles: One control has one builder
    /// </remarks>
    /// <example>
    ///    JQueryScriptBuilder jBuilder=new JQueryScriptBuilder(this);
    ///    jQueryScript.AddOption("range",true);
    ///    .....
    ///    ClientScriptManager.RegisterJQueryControl(ths,jBuilder);
    /// </example>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class JQueryScriptBuilder : ScriptBuilder,IScriptBuilder
    {
        private Dictionary<string, string> options = new Dictionary<string, string>();
        private JQuerySelector _selector;
        private Control targetControl;
        private Page page;
        private bool isPrepared = false;
        private bool isBuilded = false;

        public bool IsBuilded
        {
            get { return isBuilded; }
        }

        public bool IsPrepared
        {
            get { return isPrepared; }
        }

        private ClientRegisterEvents registerEvent=ClientRegisterEvents.ApplicationLoad;

        public ClientRegisterEvents RegisterEvent
        {
            get { return registerEvent; }
            set { registerEvent = value; }
        }

        protected Page Page
        {
            get { return page; }
        }

        protected Control TargetControl
        {
            get { return targetControl; }
        }

        public JQueryScriptBuilder(Control control)
        {
            if (control == null)
                throw new Exception("JQueryScriptBuilder must be bind to a ServerControl the control could not be null");
            targetControl = control;
            page = control.Page;
            _selector = new JQuerySelector(control);
        }

        public JQueryScriptBuilder(Control control,JQuerySelector selector)
        {
            if (control == null)
                throw new Exception("JQueryScriptBuilder must be bind to a ServerControl the control could not be null");
            targetControl = control;
            page = control.Page;
            _selector = selector;
        }

        public JQuerySelector Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        #region AppendSelector
        /// <summary>
        /// Append the jQuery selector string of Control
        /// </summary>
        /// <remarks>
        ///  this function will get the control's clientId to build the selector string.
        /// </remarks>
        /// <example>
        ///  AppendJQuerySelector(TextBox1);
        ///  //Result: $('#TextBox1')
        /// </example>
        /// <param name="control"></param>
        public JQueryScriptBuilder AppendSelector(Control control)
        {
            return AppendSelector(control.ClientID);
        }

        public JQueryScriptBuilder AppendSelector(string clientID)
        {
            scripts.Append("jQuery('#" + clientID + "')");
            return this;
        }

        public JQueryScriptBuilder AppendSelector(JQuerySelector selector)
        {
           scripts.Append(selector.ToString(page));
           return this;
        }

        public JQueryScriptBuilder AppendSelector()
        {
           scripts.Append(Selector.ToString(Page));
           return this;
        }

        public JQueryScriptBuilder AppendInvokeMethodWithOptions(string methodName)
        {
            return AppendInvokeMethodWithOptions(methodName, null);
        }

        public JQueryScriptBuilder AppendInvokeMethodWithOptions(string methodName, params string[] optionParams)
        {
            return AppendInvokeMethodWithOptions(Selector, methodName, optionParams);
        }

        public JQueryScriptBuilder AppendInvokeMethodWithOptions(JQuerySelector selector,string methodName,params string[] optionParams)
        {
            AppendSelector(selector).AppendDot().Scripts.Append(methodName).Append("(");
            if (optionParams != null)
            {
                if (optionParams.Length > 0)
                    scripts.Append("{"+string.Join(",",optionParams)+"}");
            }
            scripts.Append(");");
            return this;
        }
       
        #endregion

        /// <summary>
        /// Append all option to the result string and the option cache will be clear.
        /// </summary>
        public void AppendOptionsToResult()
        {
            if (options.Count > 0)
            {
                scripts.Append("{");
                string[] optionArray = new string[options.Count];
                int i = 0;
                foreach (string key in options.Keys)
                {
                    optionArray[i++] = key + ":" + options[key];
                }
                scripts.Append(string.Join(",", optionArray));
                scripts.Append("}");
            }
            options.Clear();
        }

        #region AddOption

        public void AddOption(string name, object value)
        {
            if (value == null)
                return;

            Type t = value.GetType();
            if (t == typeof(string))
            {
                AddOption(name, value.ToString(), true);
                return;
            }

            if (t == typeof(string[]))
            {
                AddOption(name, (string[])value);
                return;
            }

            if (t == typeof(int))
            {
                AddOption(name, (int)value);
                return;
            }

            if (t == typeof(int[]))
            {
                AddOption(name, (int[])value);
                return;
            }

            if (t == typeof(float))
            {
                AddOption(name, (float)value);
                return;
            }

            if (t == typeof(float[]))
            {
                AddOption(name, (float[])value);
                return;
            }

            if (t == typeof(decimal))
            {
                AddOption(name, (decimal)value);
                return;
            }

            if (t == typeof(bool))
            {
                AddOption(name, (bool)value);
                return;
            }


            if (t.IsEnum)
            {
                AddOption(name, (Enum)value);
                return;
            }
        }

        public void AddSelectorOption(string name,JQuerySelector selector)
        {
            AddOption(name, selector.ToString(Page),true);
        }

        public void AddSelectorOption(string name, string controlID)
        {
            JQuerySelector s = new JQuerySelector();
            s.TargetID = controlID;
            AddOption(name, s);
        }

        public void AddFunctionOption(string name, string script, string[] functionParams)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function");

            sb.Append("(");
            if (functionParams != null)
            {
                if (functionParams.Length > 0)
                    sb.Append(string.Join(",", functionParams));
            }
            sb.Append(")");
            sb.Append("{" + script + "}");
            AddOption(name, sb.ToString());
        }

        /// <summary>
        /// Append the name and string value to option string
        /// </summary>
        /// <remarks>
        /// the value will be convert to 'value' format
        /// </remarks>
        /// <param name="name">option name</param>
        /// <param name="value">option value of string</param>
        public void AddOption(string name, string value)
        {
            options.Add(name, value);
        }

        public void AddOption(string name, string value, bool format)
        {
            if (format)
                AddOption(name, "'" + value + "'");
            else
                AddOption(name, value);
        }

        /// <summary>
        ///  Append the name and int value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of integer</param>
        public void AddOption(string name, int value)
        {
            options.Add(name, value.ToString());
        }

        /// <summary>
        ///  Append the name and float value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of float</param>
        public void AddOption(string name, float value)
        {
            options.Add(name, value.ToString());
        }

        /// <summary>
        ///  Append the name and decimal value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of decimal</param>
        public void AddOption(string name, decimal value)
        {
            options.Add(name, value.ToString());
        }

        /// <summary>
        ///  Append the name and bool value to option string
        /// </summary>
        /// <remarks>
        ///  the value will be convert to low case string.
        /// </remarks>
        /// <param name="name">option name</param>
        /// <param name="value">option value of bool</param>
        public void AddOption(string name, bool value)
        {
            options.Add(name, value.ToString().ToLower());
        }

        /// <summary>
        ///  Append the name and string array value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of string array</param>
        public void AddOption(string name, string[] value)
        {
            string[] formatted = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
                formatted[i] = "'" + value[i] + "'";
            options.Add(name, "[" + string.Join(",", value) + "]");
        }

        /// <summary>
        ///  Append the name and int array value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of int array</param>
        public void AddOption(string name, int[] value)
        {
            string[] strValues = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
                strValues[i] = value[i].ToString();
            options.Add(name, "[" + string.Join(",",strValues) + "]");
        }

        /// <summary>
        ///  Append the name and float array value to option string
        /// </summary>
        /// <param name="name">option name</param>
        /// <param name="value">option value of float array</param>
        public void AddOption(string name, float[] value)
        {
            string[] strValues = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
                strValues[i] = value[i].ToString();
            options.Add(name, "[" + string.Join(",", strValues) + "]");
        }

        /// <summary>
        /// Append the name and Enum object value to option string
        /// </summary>
        /// <remarks>
        /// the Enum object will be convert to low case string.
        /// </remarks>
        /// <param name="name">option name</param>
        /// <param name="value">option value of Enum object</param>
        public void AddOption(string name, Enum value)
        {
            options.Add(name, "'" + value.ToString().ToLower() + "'");
        }

        public void AddOption(string name, Unit value, bool isStringValue)
        {
            if (isStringValue)
                AddOption(name, value.ToString(), true);
            else
                AddOption(name, value.Value);
        }

        #endregion

        #region AppendCssStyle

        public void AppendCssStyle(params string[] styles)
        {
            AppendCssStyle(Selector, styles);
        }

        public void AppendCssStyle(JQuerySelector selector, params string[] styles)
        {
            AppendSelector(selector);
            scripts.Append(".css({" + string.Join(",", styles) + "});");
        }

        /// <summary>
        /// Generate then css style for jQuery object
        /// </summary>
        /// <param name="control"></param>
        /// <param name="styles">
        ///  sets the style string to jQuery object. this param must be using 'key':'name' format;
        /// </param>
        public void AppendCssStyle(Control control, params string[] styles)
        {
            AppendSelector(control);
            scripts.Append(".css({" + string.Join(",", styles) + "});");
        }

        #endregion

        #region AppendAttr

        public void AppendAttr(string name, string value)
        {
            AppendAttr(Selector, name, value);
        }

        public void AppendAttr(Control control, string name, string value)
        {
            AppendAttr(new JQuerySelector(control), name, value);
        }

        public void AppendAttr(JQuerySelector selector, string name, string value)
        {
            AppendSelector(selector);
            scripts.Append(".attr('" + name + "'," + value + ");");
        }

        #endregion

        public void AppendEventHandler(JQuerySelector selector,string functionName, string eventName, string[] functionParams, string script)
        {
            AppendSelector(selector);
            scripts.Append("." + functionName + "('" + eventName + "',");
            AppendFunctionWrapper(null, functionParams, script);
            scripts.Append(");");
        }

        #region AppendBindFunction

        public void AppendBindFunction(string eventName, string script)
        {
            AppendBindFunction(Selector, eventName, script);
        }

        public void AppendBindFunction(string eventName, string[] functionParams, string script)
        {
            AppendBindFunction(Selector,eventName,functionParams,script);
        }

        public void AppendBindFunction(Control control, string eventName, string script)
        {
            AppendBindFunction(control, eventName, null, script);
        }

        public void AppendBindFunction(JQuerySelector selector, string eventName, string script)
        {
            AppendBindFunction(selector, eventName, null, script);
        }

        public void AppendBindFunction(Control control, string eventName, string[] functionParams, string script)
        {
            AppendBindFunction(new JQuerySelector(control), eventName, functionParams, script);
        }

        public void AppendBindFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
        {
            AppendEventHandler(selector, "bind", eventName, functionParams, script);
        }
        #endregion

        #region AppendUnbindFunction

        public void AppendUnbindFunction(JQuerySelector selector)
        {
            AppendSelector(selector);
            scripts.Append(".unbind();");
        }

        public void AppendUnbindFunction(Control control)
        {
            AppendUnbindFunction(new JQuerySelector(control));
        }

        public void AppendUnbindFunction()
        {
            AppendUnbindFunction(Selector);
        }

        #endregion

        #region AppenOneFunction

        public void AppendOneFunction(Control control, string eventName, string script)
        {
            AppendOneFunction(control, eventName, null, script);
        }

        public void AppendOneFunction(Control control, string eventName, string[] functionParams, string script)
        {
            AppendOneFunction(new JQuerySelector(control), eventName, functionParams, script);
        }

        public void AppendOneFunction(string eventName, string script) 
        {
            AppendOneFunction(Selector, eventName, script);
        }

        public void AppendOneFunction(JQuerySelector selector, string eventName, string script)
        {
            AppendOneFunction(Selector, eventName, null,script);
        }

        public void AppendOneFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
        {
            AppendEventHandler(selector, "one", eventName, functionParams, script);
        }

        #endregion

        #region AppendLiveFunction
        
        public void AppendLiveFunction(string eventName, string script)
        {
            AppendLiveFunction(Selector, eventName, script);
        }

        public void AppendLiveFunction(JQuerySelector selector, string eventName, string script)
        {
            AppendLiveFunction(selector, eventName, null, script);
        }
        
        public void AppendLiveFunction(JQuerySelector selector, string eventName, string[] functionParams, string script)
        {
            AppendEventHandler(selector, "live", eventName, functionParams, script);
        }

        public void AppendLiveFunction(Control control, string eventName, string script)
        {
            AppendLiveFunction(control, eventName, null, script);
        }

        public void AppendLiveFunction(Control control, string eventName, string[] functionParams, string script)
        {
            AppendLiveFunction(new JQuerySelector(control), eventName, functionParams, script);
        }

        #endregion 

        #region AppendDieFunction

        public void AppendDieFunction()
        {
            AppendDieFunction(Selector);
        }

        public void AppendDieFunction(Control control)
        {
            AppendDieFunction(new JQuerySelector(control));
        }

        public void AppendDieFunction(JQuerySelector selector)
        {
            AppendSelector(selector);
            scripts.Append(".die();");
        }

        #endregion 

        private bool IsIgnoreValue(object compareValue, object value, Type valueType)
        {
            //Ignore value
            if (compareValue != null)
            {
                if (valueType == typeof(bool))
                {
                    if ((bool)compareValue == (bool)value)
                        return true;
                }

                if (valueType == typeof(int))
                {
                    if ((int)compareValue == (int)value)
                        return true;
                }

                if (valueType == typeof(float))
                {
                    if (Convert.ToDecimal(compareValue) ==Convert.ToDecimal(value))
                        return true;
                }

                if (valueType == typeof(string))
                {
                    if (compareValue.ToString() == valueType.ToString())
                        return true;
                }


                if (compareValue == value)
                    return true;
            }

            return false;
        }

        //#region BeginBuildScript

        //public void BeginBuildScript(Control control, string jQueryAttributeName)
        //{
        //    BeginBuildScript(control, null, jQueryAttributeName);
        //}

        //public void BeginBuildScript(Control control, string targetControlID, string jQueryAttributeName)
        //{
        //    object[] attrs = control.GetType().GetCustomAttributes(typeof(JQueryAttribute), true);

        //    foreach (JQueryAttribute attr in attrs)
        //    {
        //        if (attr.Name == jQueryAttributeName)
        //        {
        //            BeginBuildScript(control, targetControlID, attr);
        //            break;
        //        }
        //    }
        //}

        //public void BeginBuildScript(Control control, JQueryAttribute jQueryAttr)
        //{
        //    BeginBuildScript(control, null, jQueryAttr);
        //}

        ///// <summary>
        ///// This method will generte the scripts for jQuery of the control
        ///// </summary>
        ///// <remarks>
        ///// The Control must be have JQueryAttribute.
        ///// </remarks>
        ///// <param name="control">The Control need to be build</param>
        //public void BeginBuildScript(Control control, string targetControlID, JQueryAttribute jQueryAttr)
        //{
        //    if (isBuildingScript)
        //        throw new Exception("Error to invoke BeginBuildScript Method,BeginBuildScript already invoked once could not invoke again before EndBuildScript invoke");

        //    //}
        //    isBuildingScript = true;
        //}

        //#endregion

        ///// <summary>
        ///// Generate all options added to option string and append to the result then clear the option cache.
        ///// </summary>
        //public void EndBuildScript()
        //{
        //    if (!isBuildingScript)
        //        throw new Exception("No on progress script builds please invoke BeginBuildScript first");


        //    isBuildingScript = false;
        //}

        #region IScriptBuilder Members
        
        public void Prepare()
        {            
            Type controlType =targetControl.GetType();
             object[] attrs = controlType.GetCustomAttributes(typeof(JQueryAttribute), true);

             if (attrs == null)
                 throw new Exception("Could not build this control's jQuery script. JQueryAttribute declare not found in this control.");
            
            JQueryAttribute jQueryAttr = attrs[0] as JQueryAttribute;
            Prepare(jQueryAttr);
        }

        public void Prepare(JQueryAttribute jQueryAttr)
        {
            if (isPrepared)
                return;

            if (jQueryAttr != null)
            {
                string name = jQueryAttr.Name;
                registerEvent = jQueryAttr.StartEvent;
                Type controlType = targetControl.GetType();

                AppendSelector(Selector);

                scripts.Append("." + name);


                var properties = from PropertyInfo p in controlType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                                 where (Attribute.GetCustomAttribute(p, typeof(JQueryOptionAttribute), true) != null)
                                 select p;

                foreach (PropertyInfo pi in properties)
                {
                    JQueryOptionAttribute option = (JQueryOptionAttribute)(Attribute.GetCustomAttribute(pi, typeof(JQueryOptionAttribute)));
                    if (!string.IsNullOrEmpty(option.Target))
                    {
                        if (option.Target != jQueryAttr.Name)
                            continue;
                    }

                    object pValue = pi.GetValue(targetControl, null);
                    if (pValue == null)
                    {
                        if (option.DefaultValue != null)
                            pValue = option.DefaultValue;
                        if (pValue == null)
                        {
                            if (!option.RegistNullValue)
                                continue;
                        }
                    }

                    if (IsIgnoreValue(option.IgnoreValue, pValue, pi.PropertyType))
                        continue;

                    #region Value type formats

                    switch (option.Type)
                    {
                        case JQueryOptionTypes.Value:

                            if (pi.PropertyType == typeof(JQuerySelector))
                            {
                                JQuerySelector _s = pValue as JQuerySelector;
                                if (!_s.IsEmpty)
                                    AddOption(option.Name, _s.ToString(Page));
                                continue;
                            }

                            if (pi.PropertyType == typeof(Position))
                            {
                                string pos = ((Position)pValue).ToString();
                                if (!string.IsNullOrEmpty(pos))
                                    AddOption(option.Name,pos);
                                continue;
                            }

                            if (pi.PropertyType == typeof(System.Web.UI.WebControls.Unit))
                            {
                                Unit unit = (Unit)pValue;
                                if (unit.IsEmpty)
                                    continue;
                                AddOption(option.Name, unit, true);
                            }

                            if (pi.PropertyType == typeof(JQueryEffects))
                            {
                                if (pValue.ToString().ToLower() == "none")
                                    continue;
                                AddOption(option.Name, pValue.ToString().ToLower());
                                continue;
                            }

                            AddOption(option.Name, pValue);
                            break;

                        case JQueryOptionTypes.JSONObject:
                            if (pi.PropertyType == typeof(string))
                                AddOption(option.Name, (string)pValue);
                            else
                                AddOption(option.Name, (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(pValue));
                            break;

                        case JQueryOptionTypes.Function:
                            AddFunctionOption(option.Name, pValue.ToString(), option.FunctionParams);
                            break;
                        default:
                            AddOption(option.Name, pValue);
                            break;
                    }
                }
                    #endregion
            }
            isPrepared = true;
        }

        public void Build()
        {
            if (isBuilded == false)
            {
                scripts.Append("(");
                AppendOptionsToResult();
                scripts.Append(");");
                isBuilded = true;
            }
        }

        public void Reset()
        {
            scripts = new StringBuilder();
            options.Clear();
            isPrepared = false;
            isBuilded = false;
        }

        public string GetApplicationLoadScript()
        {
            if (registerEvent == ClientRegisterEvents.ApplicationLoad)
                return this.ToString();
            return "";
        }

        public string GetApplicaitonInitScript()
        {
            if (registerEvent == ClientRegisterEvents.ApplicaitonInit)
                return this.ToString();
            return "";
        }

        public string GetDocumentReadyScript()
        {
            if (registerEvent == ClientRegisterEvents.DocumentReady)
                return this.ToString();
            return "";
        }

        #endregion
    }
}
