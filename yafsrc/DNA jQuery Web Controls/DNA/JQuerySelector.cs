///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.Web.UI.Design;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace DNA.UI.JQuery
{
    /// <summary>
    /// JQuerySelector this a class that define the jQuery selector string build rules,this class can build jQuery selector
    /// string for html elements or write the selector string in Selector property directly. Or use TargetID,TargetIDs to 
    /// get the server controls client id for render.
    /// </summary>
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class JQuerySelector
    {
        string _selector = "";
        bool noConflict = false;
        string targetID;
        bool expressionOnly = false;

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(_selector) && (string.IsNullOrEmpty(TargetID)) && (TargetIDs == null));
            }
        }

        [NotifyParentProperty(true)]
        [TypeConverter(typeof(ControlIDConverter))]
        [Bindable(true)]
        public string TargetID
        {
            get { return targetID; }
            set { targetID = value; }
        }

        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] TargetIDs { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool NoConflict
        {
            get { return noConflict; }
            set { noConflict = value; }
        }

        /// <summary>
        ///  Gets/Sets whether the selector output append "jQuery()"
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ExpressionOnly
        {
            get { return expressionOnly; }
            set { expressionOnly = value; }
        }

        [NotifyParentProperty(true)]
        [Bindable(true)]
        public string Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }

        public JQuerySelector() { }

        public JQuerySelector(string selector)
        {
            _selector = selector;
        }

        public JQuerySelector(params string[] selectors)
        {
            _selector = string.Join(",", selectors);
        }

        public JQuerySelector(Control control)
        {
            _selector = "#" + control.ClientID;
        }

        /// <summary>
        /// Clear the selector and the IsEmpty will be return true.
        /// </summary>
        public void Clear()
        {
            _selector = "";
            targetID = "";
            TargetIDs = null;
        }

        /// <summary>
        /// Format the selector to string in jQuery format.
        /// </summary>
        /// <remarks>
        ///  this method will not transfer the targetid or targetids to ClientID
        /// </remarks>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsEmpty)
            {
                if (noConflict)
                    return "jQuery";
                else
                    return "$";
            }
            else
            {
                if (!string.IsNullOrEmpty(_selector))
                    return GetSelectorString(_selector);

                if (TargetIDs != null)
                {
                    string[] ts = new string[TargetIDs.Length];
                    for (int i = 0; i < ts.Length; i++)
                    {
                        if (!TargetIDs[i].StartsWith("#"))
                            ts[i] = "#" + TargetIDs[i];
                        else
                            ts[i] = TargetIDs[i];
                    }
                    return GetSelectorString(string.Join(",", ts));
                }

                return GetSelectorString("#" +targetID);
            }
        }

        /// <summary>
        /// Format the selector to string in jQuery format
        /// </summary>
        /// <remarks>
        ///  If targetID,targetIDs sets this method will get their ClientID into the output string.
        /// </remarks>
        /// <param name="page"></param>
        /// <returns></returns>
        public string ToString(Page page)
        {
            if (IsEmpty)
            {
                if (noConflict)
                    return "jQuery";
                else
                    return "$";
            }
            else
            {
                if (!string.IsNullOrEmpty(_selector))
                    return GetSelectorString(_selector);
                
                if (TargetIDs!=null)
                {
                    string[] ts = new string[TargetIDs.Length];
                    for (int i = 0; i < ts.Length; i++)
                        ts[i] ="#"+ FormattedServerSelector(page, TargetIDs[i]);
                    return GetSelectorString(string.Join(",", ts));
                }

               return GetSelectorString("#"+FormattedServerSelector(page,targetID));
            }
        }

        private string GetSelectorString(string _s)
        {
            if (expressionOnly)
            {
                return "'" + _s + "'";
            }
            else
            {
                if (noConflict)
                    return "jQuery(\"" + _s + "\")";
                else
                    return "jQuery(\"" + _s + "\")";
            }
        }

        private string FormattedServerSelector(Page page, string exp)
        {
            int index = SubExpIndex(exp);
            if (index == -1)
                return ClientScriptManager.GetControlClientID(page, exp);
            return ClientScriptManager.GetControlClientID(page, exp.Substring(0, index)) + exp.Substring(index, exp.Length - index);
        }

        private int SubExpIndex(string id)
        {
            if (id.IndexOf(" ") > -1)
                return id.IndexOf(" ");
            if (id.IndexOf(">") > -1)
                return id.IndexOf(">");
            if (id.IndexOf("+") > -1)
                return id.IndexOf("+");
            if (id.IndexOf(":") > -1)
                return id.IndexOf(":");
            if (id.IndexOf("[") > -1)
                return id.IndexOf("]");
            return -1;
        }
    }
}
