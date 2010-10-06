///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// Handling the WebControl has more than one JQueryAttribute define 
    /// </summary>
    public class MultiJQueryScriptBuilder:IScriptBuilder
    {
        private JQuerySelector _selector;
        private Control targetControl;
        private Page page;
        private Dictionary<string, JQueryScriptBuilder> namingBuilders = new Dictionary<string, JQueryScriptBuilder>();
        private string initScript="";
        private string loadScript="";
        private string docReadyScript="";
        private bool isPrepared = false;
        private bool isBuilded = false;

        public bool IsBuilded
        {
            get { return isBuilded; }
        }

        public bool IsPrepared
        {
            get { return isPrepared; }
            set { isPrepared = value; }
        }

        protected Page Page
        {
            get { return page; }
        }

        protected Control TargetControl
        {
            get { return targetControl; }
        }

        public JQuerySelector Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        public MultiJQueryScriptBuilder(Control control)
        {
            if (control == null)
                throw new Exception("MultiJQueryScriptBuilder must be bind to a ServerControl the control could not be null");
            targetControl = control;
            page = control.Page;
            _selector = new JQuerySelector(control);
        }

        public MultiJQueryScriptBuilder(Control control, JQuerySelector selector)
        {
            if (control == null)
                throw new Exception("MultiJQueryScriptBuilder must be bind to a ServerControl the control could not be null");
           
            targetControl = control;
            page = control.Page;
            _selector = selector;
        }

        public Dictionary<string, JQueryScriptBuilder> NamingBuilders
        {
            get { return namingBuilders; }
        }

        public JQueryScriptBuilder this[string name]
        {
            get
            {
                return NamingBuilders[name];
            }
        }

        #region IScriptBuilder Members

        public void Prepare()
        {
            if (isPrepared)
                return;

            Type controlType = TargetControl.GetType();
            object[] attrs = controlType.GetCustomAttributes(typeof(JQueryAttribute), true);

            foreach (JQueryAttribute jqueryAttr in attrs)
            {
                JQueryScriptBuilder jbuilder = new JQueryScriptBuilder(TargetControl, Selector);
                jbuilder.Prepare(jqueryAttr);
                namingBuilders.Add(jqueryAttr.Name, jbuilder);
            }
            isPrepared = true;
        }

        public void Reset()
        {
            initScript = "";
            loadScript = "";
            docReadyScript = "";
            namingBuilders.Clear();
            isPrepared = false;
            isBuilded = false;
        }

        public void Build()
        {
            if (isBuilded)
                return;

            foreach (string key in namingBuilders.Keys)
            {
                this[key].Build();
                initScript += this[key].GetApplicaitonInitScript();
                loadScript += this[key].GetApplicationLoadScript();
                docReadyScript += this[key].GetDocumentReadyScript();
            }

            isBuilded = true;
        }

        public string GetApplicationLoadScript()
        {
            return loadScript;
        }

        public string GetApplicaitonInitScript()
        {
            return initScript;
        }

        public string GetDocumentReadyScript()
        {
            return docReadyScript;
        }

        #endregion
    }
}
