///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.ComponentModel.Design;
using System.Web.UI.Design.WebControls;
using System.Web.UI.Design;
using System.IO;

namespace DNA.UI.JQuery.Design
{
    public class DatePickerDesigner : ControlDesigner
    {
        private DatePicker datePicker;

        /// <summary>
        /// Initialize the NonUIControlDesigner
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(IComponent component)
        {
            datePicker = component as DatePicker;
            base.Initialize(component);
        }

        public override string GetDesignTimeHtml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            HtmlTextWriter writer = new HtmlTextWriter(stringWriter);
            if (datePicker.DisplayMode == DatePickerDisplayModes.Picker)
            {
                if (datePicker.ShowIconMode == DateIconShowModes.Focus)
                    datePicker.RenderControl(writer);
                else
                {
                    writer.Write("<table><tr><td>");
                    datePicker.RenderControl(writer);
                    writer.Write("</td><td>");

                    string imageUrl = datePicker.ButtonImageUrl;
                    if (string.IsNullOrEmpty(imageUrl))
                        imageUrl = datePicker.Page.ClientScript.GetWebResourceUrl(datePicker.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif");
                    else
                        imageUrl = datePicker.ResolveUrl(imageUrl);

                    if (datePicker.ShowButtonImageOnly)
                    {
                        writer.Write("<img src='" + imageUrl + "' />");
                    }
                    else
                    {
                        string pickerText = "...";

                        if (!string.IsNullOrEmpty(datePicker.ButtonText))
                            pickerText = datePicker.ButtonText;

                        writer.Write("<button style='width:40px");

                        if (datePicker.ShowDefaultButtonImage)
                            writer.Write("background-image:url('" + imageUrl + "');");

                        writer.Write("' >" + pickerText + "</button>");
                    }
                    writer.Write("</td></tr></table>");
                }
            }
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "ui-datepicker ui-widget ui-widget-content");// ui-corner-all ui-helper-clearfix ui-helper-hidden-accessible
            writer.WriteAttribute("id", "ui-datepicker-div");
            writer.WriteAttribute("style", "display: block;");//position: absolute; top: -14px; left: 10px;
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "ui-datepicker-header ui-widget-header ");// ui-helper-clearfix ui-corner-all
            writer.WriteAttribute("style", "height:25px;");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.WriteBeginTag("a");
            writer.WriteAttribute("title", "Prev");
            writer.WriteAttribute("class", "ui-datepicker-prev");// ui-corner-all
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.WriteBeginTag("span");
            writer.WriteAttribute("class", "ui-icon ui-icon-circle-triangle-w");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.Write("Prev");
            writer.WriteEndTag("span");
            writer.WriteEndTag("a");
            writer.WriteBeginTag("a");
            writer.WriteAttribute("title", "Next");
            writer.WriteAttribute("class", "ui-datepicker-next ui-corner-all");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.WriteBeginTag("span");
            writer.WriteAttribute("class", "ui-icon ui-icon-circle-triangle-e");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            writer.Write("Next");
            writer.WriteEndTag("span");
            writer.WriteEndTag("a");
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", "ui-datepicker-title");
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);

            if (datePicker.ShowMonthAfterYear)
            {
                RenderMonth(writer);
                RenderYear(writer);
            }
            else
            {
                RenderYear(writer);
                RenderMonth(writer);
            }

            writer.WriteEndTag("div");
            writer.WriteEndTag("div");

            #region Table area
            writer.Write("<table class=\"ui-datepicker-calendar\"><thead><tr>");
            RenderDayOfWeek(writer);
            writer.Write("</tr></thead>");
            writer.Write("<tbody><tr>");
            writer.Write("<td class=\"ui-datepicker-week-end ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">26</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">27</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">28</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">29</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">30</td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">1</a></td>");
            writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">2</a></td></tr>");
            writer.Write("<tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">3</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">4</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">5</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">6</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">7</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">8</a></td>");
            writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">9</a></td>");
            writer.Write("</tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">10</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">11</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">12</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">13</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">14</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">15</a></td>");
            writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">16</a>");
            writer.Write("</td></tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">17</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">18</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">19</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">20</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">21</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">22</a></td>");
            writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">23</a>");
            writer.Write("</td></tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">24</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">25</a></td>");
            writer.Write("<td class=\"ui-datepicker-days-cell-over ui-datepicker-current-day ui-datepicker-today\">");
            writer.Write("<a href=\"#\" class=\"ui-state-default ui-state-highlight ui-state-active\">26</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">27</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">28</a></td>");
            writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">29</a></td>");
            writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">30</a></td>");
            writer.Write("</tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">31</a></td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">1</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">2</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">3</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">4</td>");
            writer.Write("<td class=\"ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">5</td>");
            writer.Write("<td class=\"ui-datepicker-week-end ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">6</td>");
            writer.Write("</tr></tbody></table>");
            #endregion

            if (datePicker.ShowButtonPanel)
            {
                writer.WriteBeginTag("div");
                writer.WriteAttribute("class", "ui-datepicker-buttonpane ui-widget-content");
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);

                writer.Write("<button class=\"ui-datepicker-current ui-state-default ui-priority-secondary\"  type=\"button\">");

                if (!string.IsNullOrEmpty(datePicker.TextForToday))
                    writer.Write(datePicker.TextForToday);
                else
                    writer.Write("Today");

                writer.Write("</button>");

                writer.Write("<button class=\"ui-datepicker-close ui-state-default ui-priority-primary\" type=\"button\">");
                if (!string.IsNullOrEmpty(datePicker.CloseButtonText))
                    writer.Write(datePicker.CloseButtonText);
                else
                    writer.Write("Done");

                writer.Write("</button>");
                writer.WriteEndTag("div");
            }
            writer.WriteEndTag("div");
            return stringBuilder.ToString();
        }

        private void RenderMonth(HtmlTextWriter writer)
        {
            string monthString = datePicker.Value.ToString("MMM");
            if (datePicker.AllowChangeMonth)
            {
                writer.WriteBeginTag("select");
                writer.WriteAttribute("class", "ui-datepicker-month");
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                writer.Write("<option selected=\"selected\" value=\"4\" style='width:60px'>" + monthString + "</option>");
                writer.WriteEndTag("select");
            }
            else
            {
                writer.Write("<span class=\"ui-datepicker-month\">" + monthString + "</span>");
            }
        }

        private void RenderYear(HtmlTextWriter writer)
        {
            if (datePicker.AllowChangeYear)
                writer.Write("<select class=\"ui-datepicker-year\" style='width:60px'><option selected=\"selected\" value=\"2009\">" + datePicker.Value.Year.ToString() + "</option></select>");
            else
                writer.Write("<span class=\"ui-datepicker-year\">" + datePicker.Value.Year.ToString() + "</span>");
        }

        private void RenderDayOfWeek(HtmlTextWriter writer)
        {
            if ((datePicker.ShortDayNames != null) && (datePicker.ShortDayNames.Length == 7))
            {

                writer.Write("<th class=\"ui-datepicker-week-end\"><span title=\"Sunday\">" + datePicker.ShortDayNames[0] + "</span></th>");
                writer.Write("<th><span title=\"Monday\">" + datePicker.ShortDayNames[1] + "</span></th>");
                writer.Write("<th><span title=\"Tuesday\">" + datePicker.ShortDayNames[2] + "</span></th>");
                writer.Write("<th><span title=\"Wednesday\">" + datePicker.ShortDayNames[3] + "</span></th>");
                writer.Write("<th><span title=\"Thursday\">" + datePicker.ShortDayNames[4] + "</span></th>");
                writer.Write("<th><span title=\"Friday\">" + datePicker.ShortDayNames[5] + "</span></th>");
                writer.Write("<th class=\"ui-datepicker-week-end\"><span title=\"Saturday\">" + datePicker.ShortDayNames[6] + "</span></th>");

            }
            else
            {
                writer.Write("<th class=\"ui-datepicker-week-end\"><span title=\"Sunday\">Su</span></th>");
                writer.Write("<th><span title=\"Monday\">Mo</span></th>");
                writer.Write("<th><span title=\"Tuesday\">Tu</span></th>");
                writer.Write("<th><span title=\"Wednesday\">We</span></th>");
                writer.Write("<th><span title=\"Thursday\">Th</span></th>");
                writer.Write("<th><span title=\"Friday\">Fr</span></th>");
                writer.Write("<th class=\"ui-datepicker-week-end\"><span title=\"Saturday\">Sa</span></th>");
            }
        }

        public override string GetPersistInnerHtml()
        {
            IDesignerHost host = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            if (host != null)
                return ControlPersister.PersistInnerProperties(datePicker, host);
            return String.Empty;
        }
    }
}
