
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.IO;
using DNA.UI.JQuery;
using jQueryNet;

namespace DNA.UI
{
    public sealed class ClientScriptManager
    {
        public static string AddComfirScript(string confirmText, string script)
        {
            if (!string.IsNullOrEmpty(confirmText))
                return "if (!confirm('" + confirmText + "')) {self.event.returnValue=false;void(0);} else {" + script + "}";
            else
                return script;
        }

        public static Control CreateStyleLink(string href)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes.Add("rel", "Stylesheet");
            link.Attributes.Add("type", "text/css");
            link.Attributes.Add("href", href);
            return link;
        }

        public static string GetControlResourceWebUrl(Control control, string scriptResource)
        {
            return control.Page.ClientScript.GetWebResourceUrl(control.GetType(), scriptResource);
        }

        private static ScriptManager sm = null;

        private static ScriptManager GetSM(Control control)
        {
            if (sm != null)
                return sm;
            else
                return ScriptManager.GetCurrent(control.Page);
        }

        /// <summary>
        /// This method will add the scripts to ScriptMaanger.CompositeScript
        /// </summary>
        /// <param name="control"></param>
        /// <param name="assembly"></param>
        /// <param name="scriptResource"></param>
        public static void AddCompositeScript(Control control, string name, string assembly)
        {
            AddCompositeScript(control, new ScriptReference(name, assembly));
        }

        public static void AddCompositeScript(Control control, string path)
        {
            AddCompositeScript(control, new ScriptReference(path));
        }

        private static bool ReferenceIsExists(ScriptManager sm, ScriptReference scriptReference)
        {
            if (!string.IsNullOrEmpty(scriptReference.Path))
            {
                var scripts = from s in sm.CompositeScript.Scripts
                              where s.Path.ToLower() == scriptReference.Path.ToLower()
                              select s;
                return scripts.Count() > 0;
            }

            var typedScripts = from ts in sm.CompositeScript.Scripts
                               where ts.Assembly.ToLower() == scriptReference.Assembly.ToLower() &&
                               ts.Name.ToLower() == scriptReference.Name.ToLower()
                               select ts;

            return typedScripts.Count() > 0;
        }

        public static void AddCompositeScript(Control control, ScriptReference scriptReference)
        {
            if (!ReferenceIsExists(GetSM(control), scriptReference))
                GetSM(control).CompositeScript.Scripts.Add(scriptReference);
        }

        public static void AddScriptReference(Control control, string scriptResource)
        {
            AddRefToContext(GetControlResourceWebUrl(control, scriptResource));
        }

        static void AddRefToContext(string refUrl)
        {
            string key = "JQuery-Javascripts";
            if (HttpContext.Current.Items[key] == null)
                HttpContext.Current.Items[key] = new System.Collections.Specialized.StringCollection();

            System.Collections.Specialized.StringCollection refs = (HttpContext.Current.Items[key] as System.Collections.Specialized.StringCollection);
            if (!refs.Contains(refUrl))
                refs.Add(refUrl);
        }

        public static void RegisterJQuery(Control ctrl)
        {
            if (HttpContext.Current.Items["AddJQueryRegistedHandler"] == null)
            {
                ctrl.Page.PreRenderComplete += new EventHandler(RegisterScriptsOnPagePreRenderComplete);
                HttpContext.Current.Items["AddJQueryRegistedHandler"] = true;
                string jqueryWebUrl = ctrl.Page.ClientScript.GetWebResourceUrl(typeof(Res), "jQueryNet.core.js");
                AddRefToContext(jqueryWebUrl);

                //Fix png v1.0.1               
                if (HttpContext.Current.Request.Browser.Browser == "IE")
                {
                    if (HttpContext.Current.Request.Browser.MajorVersion < 7)
                    {
                        AddRefToContext(ctrl.Page.ClientScript.GetWebResourceUrl(typeof(Res), "jQueryNet.plugins.pngFix.js"));
                        RegisterDocumentReadyScript(ctrl, "jQuery(document).pngFix();");
                    }
                }
            }
        }

        static void RegisterScriptsOnPagePreRenderComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;
            string key = "JQuery-Javascripts";
            if (HttpContext.Current.Items[key] == null)
                HttpContext.Current.Items[key] = new System.Collections.Specialized.StringCollection();
            System.Collections.Specialized.StringCollection refs = (HttpContext.Current.Items[key] as System.Collections.Specialized.StringCollection);
            foreach (string url in refs)
                page.Header.Controls.Add(new LiteralControl("<script type='text/javascript' src='" + url + "'></script>"));
        }

        public static void RegisterClientApplicationLoadScript(Control control, string script)
        {
            RegisterClientApplicationLoadScript(control, control.ClientID + "_LoadScript", script);
        }

        public static void RegisterClientApplicationLoadScript(Control control, string key, string script)
        {
            StringBuilder scripts = new StringBuilder();
            //scripts.Append("<script type='text/javascript'>");
            // scripts.Append("Sys.Application.remove_load(" + key + ");");
            scripts.Append("Sys.Application.add_load(" + key + ");");

            //Debug
            //scripts.Append("Sys.Debug.trace('"+control.ClientID+" is execute loadScript key:"+key+"');");
            scripts.Append("function " + key + "(){");
            scripts.Append(script);
            scripts.Append("Sys.Application.remove_load(" + key + ");");
            scripts.Append("}");
            // scripts.Append("</script>");

            ScriptManager.RegisterStartupScript(control, control.GetType(), key, scripts.ToString(), true);
            //if using the code below it cuse the control could not update the load script by postback when the control in UpdatePanel
            //control.Page.ClientScript.RegisterStartupScript(control.GetType(), key, scripts.ToString());
        }

        public static void RegisterClientApplicationUnLoadScript(Control control, string script)
        {
            RegisterClientApplicationUnLoadScript(control, control.ClientID + "_unload", script);
        }

        public static void RegisterClientApplicationUnLoadScript(Control control, string key, string script)
        {
            StringBuilder scripts = new StringBuilder();
            scripts.Append("<script type='text/javascript'>");
            scripts.Append("Sys.Application.add_unload(function(){");
            scripts.Append(script);
            scripts.Append("});");
            scripts.Append("</script>");
            control.Page.ClientScript.RegisterStartupScript(control.GetType(), key, scripts.ToString());
        }

        public static void RegisterClientApplicationInitScript(Control contorl, string script)
        {
            RegisterClientApplicationInitScript(contorl, contorl.ClientID + "_InitScript", script);
        }

        public static void RegisterClientApplicationInitScript(Control control, string key, string script)
        {
            StringBuilder scripts = new StringBuilder();
            scripts.Append("<script type='text/javascript'>");
            scripts.Append("Sys.Application.add_init(function(){");
            scripts.Append(script);
            scripts.Append("});");
            scripts.Append("</script>");
            control.Page.ClientScript.RegisterStartupScript(control.GetType(), key, scripts.ToString());
        }

        public static void RegisterDocumentReadyScript(Control control, string script)
        {
            StringBuilder scripts = new StringBuilder();
            scripts.Append("<script type='text/javascript'>");
            scripts.Append("jQuery().ready(function() {");
            scripts.Append(script);
            scripts.Append("});");
            scripts.Append("</script>");
            control.Page.ClientScript.RegisterStartupScript(control.GetType(), control.ClientID + "_ReadyScript", scripts.ToString());
        }

        public static string ExecuteCallBackMethod(Control control, string eventArgument)
        {
            if (string.IsNullOrEmpty(eventArgument))
                return "";

            JavaScriptSerializer ser = new JavaScriptSerializer();
            Dictionary<string, object> objArgs = (Dictionary<string, object>)ser.DeserializeObject(eventArgument);

            if (!objArgs.ContainsKey("method"))
                return "";

            string methodName = (string)objArgs["method"];

            if (string.IsNullOrEmpty(methodName))
                return "";

            //string paramsString = "";
            //if (objArgs.ContainsKey("params"))
            //    paramsString = objArgs["params"].ToString();

            string returnValue = "";
            Type cattr = typeof(CallBackMethodAttribute);
            //Refaction
            MethodInfo mi = (from MethodInfo m in control.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                             where (Attribute.GetCustomAttribute(m, cattr, true) == null) ? false :
                             ((m.Name == methodName) ||
                             (((CallBackMethodAttribute)Attribute.GetCustomAttribute(m, cattr, true)).FriendlyName == methodName))
                             select m).SingleOrDefault();

            if (mi != null)
            {
                //bool hasCallBackResult = ((CallBackMethodAttribute)Attribute.GetCustomAttribute(mi, cattr)).HasCallbackResult;
                object[] paramsObjs = null;
                if (objArgs["params"] != null)
                {
                    //Dictionary<string, object> paramdic = (Dictionary<string, object>)ser.DeserializeObject(paramsString);
                    //paramsObjs = paramdic.Values.ToArray<object>();
                    paramsObjs = objArgs["params"] as object[];
                }

                if (mi.ReturnType != null)
                {
                    object returnObject = mi.Invoke(control, paramsObjs);
                    if (mi.ReturnType == typeof(string))
                        returnValue = returnObject as string;
                    if (mi.ReturnType == typeof(bool))
                        returnValue = returnObject == null ? "false" : returnObject.ToString().ToLower();
                    if ((mi.ReturnType == typeof(int)) ||
                        (mi.ReturnType == typeof(float)) ||
                        (mi.ReturnType == typeof(decimal)))
                        returnValue = returnObject.ToString();
                    returnValue = ser.Serialize(returnObject);
                }
                else
                    mi.Invoke(control, paramsObjs);
            }
            else
                throw new MissingMethodException();

            if (control.EnableViewState)
            {
                //Update viewstates
                MethodInfo pmi = typeof(System.Web.UI.Page).GetMethod("SaveAllState", BindingFlags.Instance | BindingFlags.NonPublic);
                pmi.Invoke(control.Page, null);
                PropertyInfo statePro = control.Page.GetType().GetProperty("ClientState", BindingFlags.Instance | BindingFlags.NonPublic);
                string state = statePro.GetValue(control.Page, null).ToString();
                return state + ";" + returnValue;
            }
            else
                return returnValue;

        }

        public static ScriptDescriptor GetControlDescriptors(Control control, string controlType)
        {
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor(controlType, control.ClientID);

            Type TAttr = typeof(ClientPropertyAttribute);
            var properties = from PropertyInfo p in control.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                             where (Attribute.GetCustomAttribute(p, TAttr, true) != null)
                             select p;
            foreach (PropertyInfo property in properties)
            {
                ClientPropertyAttribute cpt = Attribute.GetCustomAttribute(property, TAttr, true) as ClientPropertyAttribute;
                object propertyValue = property.GetValue(control, null);
                if (propertyValue == null)
                {
                    if (cpt.DefaultValue != null)
                        propertyValue = cpt.DefaultValue;
                    else
                        if (!cpt.RegistNullValue)
                            continue;
                }
                switch (cpt.Type)
                {
                    case ClientPropertyTypes.ElementProperty:
                        descriptor.AddElementProperty(cpt.Name, control.ClientID);
                        break;
                    case ClientPropertyTypes.Event:
                        if (propertyValue != null)
                            descriptor.AddEvent(cpt.Name, propertyValue.ToString());
                        break;
                    case ClientPropertyTypes.ScriptProperty:
                        if (propertyValue != null)
                            descriptor.AddScriptProperty(cpt.Name, propertyValue.ToString());
                        break;
                    case ClientPropertyTypes.ComponentProperty:
                        descriptor.AddComponentProperty(cpt.Name, control.ClientID);
                        break;
                    default:
                        if (property.PropertyType == typeof(string))
                        {
                            string relativeUrl = propertyValue as string;
                            if (cpt.IsUrl)
                            {
                                //if (control.EnableTheming)
                                //{
                                //    if (!string.IsNullOrEmpty(relativeUrl))
                                //    {
                                //        if (VirtualPathUtility.IsAppRelative(relativeUrl))
                                //        {
                                //            if (!relativeUrl.ToLower().StartsWith("~/app_themes"))
                                //                relativeUrl = relativeUrl.Replace("~/", "~/app_themes/" + control.Page.Theme + "/");
                                //        }
                                //    }
                                //}
                                propertyValue = control.ResolveUrl(relativeUrl);
                            }
                        }
                        descriptor.AddProperty(cpt.Name, propertyValue);
                        break;
                }
            }
            return descriptor;
        }

        public static IEnumerable<ScriptReference> GetControlScriptReferences(Control control)
        {
            //List<ScriptReference> scriptRefs = new List<ScriptReference>();
            SortedList<int, ScriptReference> scriptRefs = new SortedList<int, ScriptReference>();

            object[] attrs = control.GetType().GetCustomAttributes(typeof(ScriptReferenceAttribute), true);
            if (attrs != null)
            {
                if (attrs.Length > 0)
                {
                    foreach (ScriptReferenceAttribute srefAttr in attrs)
                    {
                        int order = srefAttr.LoadOrder;

                        while (scriptRefs.Keys.Contains(order))
                            order++;
                        string assembly = srefAttr.Assembly;
                        if (string.IsNullOrEmpty(srefAttr.Assembly))
                            assembly = control.GetType().Assembly.FullName;
                        scriptRefs.Add(order, new ScriptReference(srefAttr.Name, assembly));
                    }
                }
            }
            return scriptRefs.Values.ToArray<ScriptReference>();
        }

        public static void RegisterJQueryControl(Control control)
        {
            MultiJQueryScriptBuilder jBuilder = new MultiJQueryScriptBuilder(control);
            RegisterJQueryControl(control, jBuilder);
        }

        public static void RegisterJQueryControl(Control control, IScriptBuilder builder)
        {
            //Step 1 Register Script References
            RegisterJQueryScriptReferences(control);

            if (!builder.IsPrepared)
                builder.Prepare();

            //Step 2 using builder to build scripts
            if (!builder.IsBuilded)
                builder.Build();

            string loadScript = builder.GetApplicationLoadScript();
            string initScript = builder.GetApplicaitonInitScript();
            string docReadyScript = builder.GetDocumentReadyScript();

            if (!string.IsNullOrEmpty(loadScript))
                RegisterClientApplicationLoadScript(control, loadScript);

            if (!string.IsNullOrEmpty(initScript))
                RegisterClientApplicationInitScript(control, initScript);

            if (!string.IsNullOrEmpty(docReadyScript))
                RegisterDocumentReadyScript(control, docReadyScript);

        }

        //[Obsolete]
        //public static void RegisterJQueryControl(Control control, Dictionary<string, string> options)
        //{
        //    ScriptManager sm = ScriptManager.GetCurrent(control.Page);
        //    if (sm == null)
        //        throw new Exception("You need place the ScriptManager in WebForm");

        //    RegisterJQuery(control);

        //    //JQuery.RegisterJQuery(control.Page);
        //    Type controlType = control.GetType();
        //    object[] attrs = controlType.GetCustomAttributes(typeof(JQuery.JQueryAttribute), true);

        //    if (attrs != null)
        //    {
        //        foreach (JQueryAttribute jqueryAttr in attrs)
        //        {
        //            string name = jqueryAttr.Name;
        //            StringBuilder scripts = new StringBuilder();
        //            scripts.Append("jQuery('#" + control.ClientID + "')." + name + "(");

        //            RegisterJQueryScriptReferences(control, sm, jqueryAttr);

        //            var properties = from PropertyInfo p in controlType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
        //                             where (Attribute.GetCustomAttribute(p, typeof(JQueryOptionAttribute), true) != null)
        //                             select p;

        //            System.Collections.ArrayList optionArray = new System.Collections.ArrayList();

        //            if (options != null)
        //            {
        //                foreach (string key in options.Keys)
        //                    optionArray.Add(key + ":" + options[key]);
        //            }

        //            foreach (PropertyInfo pi in properties)
        //            {
        //                JQueryOptionAttribute option = (JQueryOptionAttribute)(Attribute.GetCustomAttribute(pi, typeof(JQueryOptionAttribute)));
        //                if (!string.IsNullOrEmpty(option.Target))
        //                {
        //                    if (option.Target != jqueryAttr.Name)
        //                        continue;
        //                }

        //                object pValue = pi.GetValue(control, null);
        //                if (pValue == null)
        //                {
        //                    if (option.DefaultValue != null)
        //                        pValue = option.DefaultValue;
        //                    if (pValue == null)
        //                    {
        //                        if (!option.RegistNullValue)
        //                            continue;
        //                    }
        //                }

        //                if (IsIgnoreValue(option.IgnoreValue, pValue, pi.PropertyType))
        //                    continue;

        //                string value = "";
        //                value = option.Name + ":";

        //                #region Value type formats
        //                switch (option.Type)
        //                {
        //                    case JQueryOptionTypes.Value:
        //                        string fv = pValue.ToString();
        //                        if (pi.PropertyType == typeof(string))
        //                            fv = "'" + fv + "'";

        //                        if (pi.PropertyType == typeof(bool))
        //                            fv = fv.ToLower();

        //                        //v1.0.1 Updated:the script builder will convert the unit to number in internal
        //                        if (pi.PropertyType == typeof(System.Web.UI.WebControls.Unit))
        //                        {
        //                            System.Web.UI.WebControls.Unit unit =(System.Web.UI.WebControls.Unit)pValue;
        //                            if (unit.IsEmpty)
        //                                continue;
        //                            fv = unit.Value.ToString();
        //                        }

        //                        if ((pi.PropertyType == typeof(JQueryEffects?)) || (pi.PropertyType == typeof(JQueryEffects)))
        //                        {
        //                            if (fv.ToLower() == "none")
        //                                continue;
        //                            fv = "'" + fv.ToLower() + "'";
        //                        }
        //                        else
        //                        {
        //                            if (pi.PropertyType.IsEnum)
        //                                fv = "'" + fv.ToLower() + "'";
        //                        }
        //                        //if ((pi.PropertyType == typeof(Orientation)))
        //                        //    fv = "'" + fv.ToLower() + "'";
        //                        value += fv;
        //                        break;
        //                    case JQueryOptionTypes.JSONObject:
        //                        if (pi.PropertyType == typeof(string))
        //                            value += pValue;
        //                        else
        //                            value += (new JavaScriptSerializer()).Serialize(pValue);
        //                        break;
        //                    case JQueryOptionTypes.Function:
        //                        string fV = pValue.ToString();
        //                        if (!fV.StartsWith("function"))
        //                        {
        //                            if (option.FunctionParams != null)
        //                            {
        //                                fV = "function(" + String.Join(",", option.FunctionParams) + "){" + fV + "}";
        //                            }
        //                            else
        //                                fV = "function(){" + fV + "}";
        //                        }
        //                        value += fV;
        //                        break;
        //                    default:
        //                        value += pValue.ToString();
        //                        break;
        //                }
        //                optionArray.Add(value);
        //            }
        //                #endregion

        //            if (optionArray.Count > 0)
        //            {
        //                scripts.Append("{");
        //                scripts.Append(String.Join(",", (string[])optionArray.ToArray(typeof(string)))); scripts.Append("}");
        //            }

        //            scripts.Append(");");
        //            switch (jqueryAttr.StartEvent)
        //            {
        //                case ClientRegisterEvents.ApplicaitonInit:
        //                    RegisterClientApplicationInitScript(control, scripts.ToString());
        //                    break;
        //                case ClientRegisterEvents.ApplicationLoad:
        //                    RegisterClientApplicationLoadScript(control, scripts.ToString());
        //                    break;
        //                case ClientRegisterEvents.DocumentReady:
        //                    RegisterDocumentReadyScript(control, scripts.ToString());
        //                    break;
        //            }

        //            //New for v1.0.1
        //            //if (!string.IsNullOrEmpty(jqueryAttr.DisposeMethod))
        //           //     RegisterClientApplicationUnLoadScript(control, "jQuery('#"+control.ClientID+"')."+jqueryAttr.Name+"('"+jqueryAttr.DisposeMethod+"');");
        //        }
        //    }
        //}

        private static bool IsIgnoreValue(object compareValue, object value, Type valueType)
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
                    if ((float)compareValue == (float)value)
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

        public static void RegisterJQueryScriptReferences(Control control)
        {
            RegisterJQuery(control);
            Type controlType = control.GetType();
            object[] attrs = controlType.GetCustomAttributes(typeof(JQueryAttribute), true);

            if (attrs != null)
            {
                foreach (JQueryAttribute jqueryAttr in attrs)
                    RegisterJQueryScriptReferences(control, sm, jqueryAttr);
            }
        }

        public static void RegisterJQueryScriptReferences(Control control, ScriptManager sm, JQueryAttribute jqueryAttr)
        {
            if (jqueryAttr.ScriptResources != null)
            {
                string assembly = control.GetType().Assembly.FullName;
                if (!string.IsNullOrEmpty(jqueryAttr.Assembly))
                    assembly = jqueryAttr.Assembly;

                foreach (string scriptRef in jqueryAttr.ScriptResources)
                {
                    string script = scriptRef;
                    if (!string.IsNullOrEmpty(jqueryAttr.ScriptResourceBaseName))
                        script = jqueryAttr.ScriptResourceBaseName + scriptRef;
                    AddCompositeScript(control, script, assembly);
                    //sm.Scripts.Add(new ScriptReference(script, assembly));
                }
            }
        }

        /// <summary>
        /// Find the control instance in page
        /// </summary>
        /// <param name="container"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Control FindControl(Control container, string id)
        {
            Control ctrl = container.FindControl(id);

            if (ctrl == null)
            {
                for (int i = 0; i < container.Controls.Count; i++)
                {
                    ctrl = FindControl(container.Controls[i], id);
                    if (ctrl != null)
                        break;
                }
            }
            return ctrl;
        }

        public static string GetControlClientID(Page page, string id)
        {
            string realID = id;
            if (!string.IsNullOrEmpty(id))
            {
                Control ctrl = FindControl(page.Form, id);
                if (ctrl != null)
                    realID = ctrl.ClientID;
            }
            return realID;
        }

        public static string RenderControlToHTML(Control control)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            control.RenderControl(htmlWriter);
            return stringBuilder.ToString();
        }

        public static string FormatFunctionString(string value)
        {
            return FormatFunctionString(value, null);
        }

        public static string FormatFunctionString(string value, string[] _params)
        {
            string formatted = value;
            if (!value.StartsWith("function"))
            {
                if (_params != null)
                {
                    formatted = "function(" + String.Join(",", _params) + "){" + value + "}";
                }
                else
                    formatted = "function(){" + formatted + "}";
            }
            return formatted;
        }
    }
}
