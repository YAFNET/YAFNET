
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

namespace DNA.UI
{
    /// <summary>
    /// ScriptBuilder is a helper class to build script string
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ScriptBuilder
    {
        protected StringBuilder scripts = new StringBuilder();
       
        /// <summary>
        /// Gets StringBuilder that store the script result
        /// </summary>
        public StringBuilder Scripts
        {
            get { return scripts; }
        }

        /// <summary>
        /// Add the script as anonymouse function to Sys.Application.Load event
        /// </summary>
        /// <remarks>
        /// This method will auto append "function(){...}" to wrap the script
        /// </remarks>
        /// <param name="script">script body which need to register</param>
        /// <param name="anonymousFunction">Whether generate the "function()" wrapper</param>
        public void AppendSysLoadScript(string script, bool anonymousFunction)
        {
            if (anonymousFunction)
                AppendSysLoadScript("function(){" + script + "}");
            else
                AppendSysLoadScript(script);
        }

        /// <summary>
        /// Add the script to Sys.Application.Load event
        /// </summary>
        /// <param name="script">script body which need to register</param>
        public void AppendSysLoadScript(string script)
        {
            scripts.Append("Sys.Application.add_load(");
            scripts.Append(script);
            scripts.Append(");");
        }

        /// <summary>
        /// Add the script as anonymouse function to Sys.Application.Unload event
        /// </summary>
        /// <remarks>
        /// This method will auto append "function(){...}" to wrap the script
        /// </remarks>
        /// <param name="script">script body which need to register</param>
        /// <param name="anonymousFunction">Whether generate the "function()" wrapper</param>
        public void AppendSysUnloadScript(string script, bool anonymousFunction)
        {
            if (anonymousFunction)
                AppendSysUnloadScript("function(){" + script + "}");
            else
                AppendSysUnloadScript(script);
        }

        /// <summary>
        /// Add the script to Sys.Application.Unload event
        /// </summary>
        /// <param name="script">script body which need to register</param>
        public void AppendSysUnloadScript(string script)
        {
            scripts.Append("Sys.Application.add_unload(");
            scripts.Append(script);
            scripts.Append(");");
        }

        /// <summary>
        /// Add the script as anonymouse function to Sys.Application.Init event
        /// </summary>
        /// <remarks>
        /// This method will auto append "function(){...}" to wrap the script
        /// </remarks>
        /// <param name="script">script body which need to register</param>
        /// <param name="anonymousFunction">Whether generate the "function()" wrapper</param>
        public void AppendSysInitScript(string script, bool anonymousFunction)
        {
            if (anonymousFunction)
                AppendSysInitScript("function(){" + script + "}");
            else
                AppendSysInitScript(script);
        }

        /// <summary>
        /// Add the script to Sys.Application.Init event
        /// </summary>
        /// <param name="script"></param>
        public void AppendSysInitScript(string script)
        {
            scripts.Append("Sys.Application.add_init(");
            scripts.Append(script);
            scripts.Append(");");
        }

        /// <summary>
        /// Add the script to document.ready event
        /// </summary>
        /// <param name="script">script body which need to register</param>
        public void AppendDocumentReadyScript(string script)
        {
            scripts.Append("jQuery().ready(function() {");
            scripts.Append(script);
            scripts.Append("});");
        }


        /// <summary>
        ///  Generate the "$get" shortcut for the control
        /// </summary>
        /// <param name="control">Control instance</param>
        public ScriptBuilder AppendGetShortCut(Control control)
        {
            return AppendGetShortCut(control.ClientID);
        }

        public ScriptBuilder AppendGetShortCut(string id)
        {
            scripts.Append("$get('" +id + "')");
            return this;
        }

        public virtual ScriptBuilder AppendInvokeElementMethod(Control control, string method, params string[] paramValues)
        {
           return AppendInvokeElementMethod(control.ClientID, method, paramValues);
        }

        public virtual ScriptBuilder AppendInvokeElementMethod(string id, string method, params string[] paramValues)
        {
            AppendGetShortCut(id).AppendDot();
            scripts.Append(method+"("+string.Join(",",paramValues)+");");
            return this;
        }

        public void AppendSetValue(Control control, string value)
        {
            AppendSetValue(control.ClientID, value);
        }

        public void AppendSetValue(string id,string value) 
        {
            AppendGetShortCut(id);
            scripts.Append(".value="+value+";");
        }

        /// <summary>
        /// Generate the "$find" shortcut for the control
        /// </summary>
        /// <param name="control">Control instance</param>
        public ScriptBuilder AppendFindShortCut(Control control)
        {
            scripts.Append("$find('" + control.ClientID + "')");
            return this;
        }


        #region AppendFunctionWrapper

        /// <summary>
        /// Add the function wrapper to the script
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="script"></param>
        public void AppendFunctionWrapper(string functionName, string script)
        {
            AppendFunctionWrapper(functionName, null, script);
        }

        /// <summary>
        /// Add the function wrapper to the script
        /// </summary>
        /// <param name="script"></param>
        public void AppendFunctionWrapper(string script)
        {
            AppendFunctionWrapper(null, null, script);
        }

        public ScriptBuilder AppendDot()
        {
            scripts.Append(".");
            return this;
        }

        /// <summary>
        /// Add the function wrapper to the script
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="functionParams"></param>
        /// <param name="script"></param>
        public void AppendFunctionWrapper(string functionName, string[] functionParams, string script)
        {
            scripts.Append("function");
            if (!string.IsNullOrEmpty(functionName))
                scripts.Append(" " + functionName);
            scripts.Append("(");
            if (functionParams != null)
            {
                if (functionParams.Length > 0)
                    scripts.Append(string.Join(",", functionParams));
            }
            scripts.Append(")");
            scripts.Append("{" + script + "}");
        }
        #endregion


        /// <summary>
        /// Get the script's result
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return scripts.ToString();
        }
    }
}
