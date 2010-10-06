///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Xml;
using System.Security.Permissions;
using System.Drawing.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

namespace DNA.UI
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [ToolboxData("<{0}:SimpleButton runat=\"server\" id=\"SimpleButton1\"></{0}:SimpleButton>")]
    public class SimpleButton:WebControl,INamingContainer,IPostBackEventHandler
    {
        public event EventHandler Click;
        private string iconCssClass = "ui-icon ui-icon-newwin";

        public string IconCssClass
        {
            get { return iconCssClass; }
            set { iconCssClass = value; }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.A;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(CssClass))
                CssClass = "ui-state-default ui-corner-all";
            if (Click == null)
                Attributes.Add("href", "#");
            else
                Attributes.Add("href", Page.ClientScript.GetPostBackClientHyperlink(this,""));

            Style.Add("position", "relative");
            Style.Add("padding", "0.4em 1em 0.4em 20px");
            Style.Add("text-decoration", "none");
            Attributes.Add("onmouseover","jQuery(this).addClass('ui-state-hover');");
            Attributes.Add("onmouseout", "jQuery(this).removeClass('ui-state-hover');");
            base.RenderBeginTag(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<span style='left:0.2em;margin:-8px 5px 0 0;position:absolute;top:50%;' class=\""+IconCssClass+"\"></span>");
            writer.Write(Text);
        }

        #region IPostBackEventHandler Members

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (Click != null)
                Click(this, EventArgs.Empty);
        }

        #endregion
    }
}
