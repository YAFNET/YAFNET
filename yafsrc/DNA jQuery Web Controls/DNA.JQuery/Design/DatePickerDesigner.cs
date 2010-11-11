//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

namespace DNA.UI.JQuery.Design
{
  #region Using

  using System;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.IO;
  using System.Text;
  using System.Web.UI;
  using System.Web.UI.Design;

  #endregion

  /// <summary>
  /// The date picker designer.
  /// </summary>
  public class DatePickerDesigner : ControlDesigner
  {
    #region Constants and Fields

    /// <summary>
    /// The date picker.
    /// </summary>
    private DatePicker datePicker;

    #endregion

    #region Public Methods

    /// <summary>
    /// The get design time html.
    /// </summary>
    /// <returns>
    /// The get design time html.
    /// </returns>
    public override string GetDesignTimeHtml()
    {
      var stringBuilder = new StringBuilder();
      var stringWriter = new StringWriter(stringBuilder);
      var writer = new HtmlTextWriter(stringWriter);
      if (this.datePicker.DisplayMode == DatePickerDisplayModes.Picker)
      {
        if (this.datePicker.ShowIconMode == DateIconShowModes.Focus)
        {
          this.datePicker.RenderControl(writer);
        }
        else
        {
          writer.Write("<table><tr><td>");
          this.datePicker.RenderControl(writer);
          writer.Write("</td><td>");

          string imageUrl = this.datePicker.ButtonImageUrl;
          if (string.IsNullOrEmpty(imageUrl))
          {
            imageUrl = this.datePicker.Page.ClientScript.GetWebResourceUrl(
              this.datePicker.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif");
          }
          else
          {
            imageUrl = this.datePicker.ResolveUrl(imageUrl);
          }

          if (this.datePicker.ShowButtonImageOnly)
          {
            writer.Write("<img src='" + imageUrl + "' />");
          }
          else
          {
            string pickerText = "...";

            if (!string.IsNullOrEmpty(this.datePicker.ButtonText))
            {
              pickerText = this.datePicker.ButtonText;
            }

            writer.Write("<button style='width:40px");

            if (this.datePicker.ShowDefaultButtonImage)
            {
              writer.Write("background-image:url('" + imageUrl + "');");
            }

            writer.Write("' >" + pickerText + "</button>");
          }

          writer.Write("</td></tr></table>");
        }
      }

      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "ui-datepicker ui-widget ui-widget-content");
        
        // ui-corner-all ui-helper-clearfix ui-helper-hidden-accessible
      writer.WriteAttribute("id", "ui-datepicker-div");
      writer.WriteAttribute("style", "display: block;"); // position: absolute; top: -14px; left: 10px;
      writer.Write(HtmlTextWriter.SelfClosingTagEnd);
      writer.WriteBeginTag("div");
      writer.WriteAttribute("class", "ui-datepicker-header ui-widget-header "); // ui-helper-clearfix ui-corner-all
      writer.WriteAttribute("style", "height:25px;");
      writer.Write(HtmlTextWriter.SelfClosingTagEnd);
      writer.WriteBeginTag("a");
      writer.WriteAttribute("title", "Prev");
      writer.WriteAttribute("class", "ui-datepicker-prev"); // ui-corner-all
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

      if (this.datePicker.ShowMonthAfterYear)
      {
        this.RenderMonth(writer);
        this.RenderYear(writer);
      }
      else
      {
        this.RenderYear(writer);
        this.RenderMonth(writer);
      }

      writer.WriteEndTag("div");
      writer.WriteEndTag("div");

      

      writer.Write("<table class=\"ui-datepicker-calendar\"><thead><tr>");
      this.RenderDayOfWeek(writer);
      writer.Write("</tr></thead>");
      writer.Write("<tbody><tr>");
      writer.Write(
        "<td class=\"ui-datepicker-week-end ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">26</td>");
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
      writer.Write(
        "</td></tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">17</a></td>");
      writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">18</a></td>");
      writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">19</a></td>");
      writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">20</a></td>");
      writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">21</a></td>");
      writer.Write("<td class=\"\"><a href=\"#\" class=\"ui-state-default\">22</a></td>");
      writer.Write("<td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">23</a>");
      writer.Write(
        "</td></tr><tr><td class=\"ui-datepicker-week-end\"><a href=\"#\" class=\"ui-state-default\">24</a></td>");
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
      writer.Write(
        "<td class=\"ui-datepicker-week-end ui-datepicker-other-month ui-datepicker-unselectable ui-state-disabled\">6</td>");
      writer.Write("</tr></tbody></table>");

      

      if (this.datePicker.ShowButtonPanel)
      {
        writer.WriteBeginTag("div");
        writer.WriteAttribute("class", "ui-datepicker-buttonpane ui-widget-content");
        writer.Write(HtmlTextWriter.SelfClosingTagEnd);

        writer.Write("<button class=\"ui-datepicker-current ui-state-default ui-priority-secondary\"  type=\"button\">");

        if (!string.IsNullOrEmpty(this.datePicker.TextForToday))
        {
          writer.Write(this.datePicker.TextForToday);
        }
        else
        {
          writer.Write("Today");
        }

        writer.Write("</button>");

        writer.Write("<button class=\"ui-datepicker-close ui-state-default ui-priority-primary\" type=\"button\">");
        if (!string.IsNullOrEmpty(this.datePicker.CloseButtonText))
        {
          writer.Write(this.datePicker.CloseButtonText);
        }
        else
        {
          writer.Write("Done");
        }

        writer.Write("</button>");
        writer.WriteEndTag("div");
      }

      writer.WriteEndTag("div");
      return stringBuilder.ToString();
    }

    /// <summary>
    /// The get persist inner html.
    /// </summary>
    /// <returns>
    /// The get persist inner html.
    /// </returns>
    public override string GetPersistInnerHtml()
    {
      var host = (IDesignerHost)this.Component.Site.GetService(typeof(IDesignerHost));
      if (host != null)
      {
        return ControlPersister.PersistInnerProperties(this.datePicker, host);
      }

      return String.Empty;
    }

    /// <summary>
    /// Initialize the NonUIControlDesigner
    /// </summary>
    /// <param name="component">
    /// </param>
    public override void Initialize(IComponent component)
    {
      this.datePicker = component as DatePicker;
      base.Initialize(component);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render day of week.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderDayOfWeek(HtmlTextWriter writer)
    {
      if ((this.datePicker.ShortDayNames != null) && (this.datePicker.ShortDayNames.Length == 7))
      {
        writer.Write(
          "<th class=\"ui-datepicker-week-end\"><span title=\"Sunday\">" + this.datePicker.ShortDayNames[0] +
          "</span></th>");
        writer.Write("<th><span title=\"Monday\">" + this.datePicker.ShortDayNames[1] + "</span></th>");
        writer.Write("<th><span title=\"Tuesday\">" + this.datePicker.ShortDayNames[2] + "</span></th>");
        writer.Write("<th><span title=\"Wednesday\">" + this.datePicker.ShortDayNames[3] + "</span></th>");
        writer.Write("<th><span title=\"Thursday\">" + this.datePicker.ShortDayNames[4] + "</span></th>");
        writer.Write("<th><span title=\"Friday\">" + this.datePicker.ShortDayNames[5] + "</span></th>");
        writer.Write(
          "<th class=\"ui-datepicker-week-end\"><span title=\"Saturday\">" + this.datePicker.ShortDayNames[6] +
          "</span></th>");
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

    /// <summary>
    /// The render month.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderMonth(HtmlTextWriter writer)
    {
      string monthString = this.datePicker.Value.ToString("MMM");
      if (this.datePicker.AllowChangeMonth)
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

    /// <summary>
    /// The render year.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    private void RenderYear(HtmlTextWriter writer)
    {
      if (this.datePicker.AllowChangeYear)
      {
        writer.Write(
          "<select class=\"ui-datepicker-year\" style='width:60px'><option selected=\"selected\" value=\"2009\">" +
          this.datePicker.Value.Year + "</option></select>");
      }
      else
      {
        writer.Write("<span class=\"ui-datepicker-year\">" + this.datePicker.Value.Year + "</span>");
      }
    }

    #endregion
  }
}