///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.Design;
using System.IO;

namespace DNA.UI.JQuery.Design
{
    /// <summary>
    /// The Designer for Dialog
    /// </summary>
    public class DialogDesigner : CompositeControlDesigner
    {
        private Dialog dialog;

        /// <summary>
        /// Gets whether the Dialog can resize
        /// </summary>
        public override bool AllowResize
        {
            get
            {
                if (dialog != null)
                    return dialog.IsResizable;
                return false;
            }
        }

        /// <summary>
        /// Initialize the DialogDesigner
        /// </summary>
        /// <param name="component">Target component</param>
        public override void Initialize(IComponent component)
        {
            dialog = component as Dialog;
            if (dialog == null)
                throw new ArgumentException("Component must be an Accordion control", "component");
            base.Initialize(component);
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            int width =Convert.ToInt16( dialog.Width.Value);
            int height =Convert.ToInt16(dialog.Height.Value);
            
            if (width == 0)
                width = 300;

            if (dialog.MinHeight != 0)
                if (height < dialog.MinHeight)
                    height = dialog.MinHeight;

            if (dialog.MaxHeight != 0)
                if (height > dialog.MaxHeight)
                    height = dialog.MaxHeight;

            if (dialog.MinWidth != 0)
                if (width < dialog.MinWidth)
                    width = dialog.MinWidth;

            if (dialog.MaxWidth != 0)
                if (width > dialog.MaxWidth)
                    width = dialog.MaxWidth;

            htmlWriter.WriteBeginTag("div");
            htmlWriter.WriteAttribute("id", dialog.ID);
            htmlWriter.WriteAttribute("class", "ui-dialog ui-widget ui-widget-content ui-corner-all ui-draggable ui-resizable");
            string style = "width:" + width.ToString() + "px;border:solid 1px #333333;overflow: hidden; display: block;z-index: 1002";

            string contentHeight = "";


            if (height != 0)
            {
                if (dialog.Buttons.Count == 0)
                    style = style + ";height:" + (height + 25).ToString() + "px;";
                else
                    style = style + ";height:" + (height +95).ToString() + "px;";
                contentHeight = "height:" + height.ToString() + "px;";
            }
                //htmlWriter.AddStyleAttribute("height", height.ToString() + "px");
            //htmlWriter.AddStyleAttribute("width", width.ToString() + "px");
            //htmlWriter.AddStyleAttribute("border", "solid 1px #333333");
            htmlWriter.WriteAttribute("style", style);
            htmlWriter.Write(HtmlTextWriter.TagRightChar);

            //Dialog header
            htmlWriter.WriteBeginTag("div");
            htmlWriter.WriteAttribute("class", "ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix");
            htmlWriter.WriteAttribute("style", "padding:5px;display:block;height:25px;");//background-color:#c9c9c9;color:#000000
            htmlWriter.Write(HtmlTextWriter.TagRightChar);

            htmlWriter.WriteBeginTag("span");
            htmlWriter.WriteAttribute("class","ui-dialog-title") ;
            htmlWriter.Write(HtmlTextWriter.TagRightChar);
             htmlWriter.Write(dialog.Title);
             htmlWriter.WriteEndTag("span");

            htmlWriter.WriteBeginTag("a");
            htmlWriter.WriteAttribute("href","#");
            htmlWriter.WriteAttribute("class","ui-dialog-titlebar-close ui-corner-all");
             htmlWriter.Write(HtmlTextWriter.TagRightChar);
             htmlWriter.WriteBeginTag("span");
            htmlWriter.WriteAttribute("class","ui-icon ui-icon-closethick");
             htmlWriter.Write(HtmlTextWriter.TagRightChar);
             htmlWriter.WriteEndTag("span");
             htmlWriter.WriteEndTag("a");
       
            htmlWriter.WriteEndTag("div");

            regions.Add(new EditableDesignerRegion(this, "dialogContent", false));
            htmlWriter.WriteBeginTag("div");
            htmlWriter.WriteAttribute(EditableDesignerRegion.DesignerRegionAttributeName, "0");
            htmlWriter.WriteAttribute("class", "ui-dialog-content ui-widget-content");
            htmlWriter.WriteAttribute("style", "overflow: hidden;padding:5px;display:block;" + contentHeight);
            htmlWriter.Write(HtmlTextWriter.TagRightChar);
            htmlWriter.WriteEndTag("div");

            //Write buttons
            if (dialog.Buttons.Count > 0)
            {
                htmlWriter.WriteBeginTag("div");
                htmlWriter.WriteAttribute("class", "ui-dialog-buttonpane ui-widget-content ui-helper-clearfix");
                htmlWriter.WriteAttribute("style", "padding:5px;display:block;text-align:right;height:28px");
                htmlWriter.Write(HtmlTextWriter.TagRightChar);
                foreach (DialogButton btn in dialog.Buttons)
                {
                    htmlWriter.WriteBeginTag("input");
                    htmlWriter.WriteAttribute("type","button");
                    htmlWriter.WriteAttribute("class", "ui-state-default ui-corner-all");
                    htmlWriter.WriteAttribute("style", "margin:2px;");
                    htmlWriter.WriteAttribute("value",btn.Text);
                    htmlWriter.Write(HtmlTextWriter.SelfClosingTagEnd);
                }
                htmlWriter.WriteEndTag("div");
            }
            htmlWriter.WriteEndTag("div");
            return stringBuilder.ToString();
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                if (dialog.BodyTemplate != null)
                    return ControlPersister.PersistTemplate(dialog.BodyTemplate, host);
            }
            return String.Empty;
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                dialog.BodyTemplate = ControlParser.ParseTemplate(host, content);
            }
        }

    }
}
