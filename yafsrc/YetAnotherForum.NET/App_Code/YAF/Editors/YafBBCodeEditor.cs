/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Editors
{
  using System;
  using System.Data;
  using System.Web.UI;
  using YAF.Classes.Core;
  using YAF.Classes.UI;
  using YAF.Controls;

  /// <summary>
  /// The bb code editor.
  /// </summary>
  public class BBCodeEditor : TextEditor
  {
    /// <summary>
    /// The _pop menu.
    /// </summary>
    private PopMenu _popMenu = null;

    /// <summary>
    /// The render button.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="cmd">
    /// The cmd.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="image">
    /// The image.
    /// </param>
    private void RenderButton(HtmlTextWriter writer, string id, string cmd, string title, string image)
    {
      // writer.WriteLine("		<td><img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"{1}.{3}\"></td><td>&nbsp;</td>",ResolveUrl(image),SafeID,title,cmd,id);
      writer.WriteLine(
        @"<img id=""{1}_{4}"" onload=""Button_Load(this)"" src=""{0}"" width=""21"" height=""20"" alt=""{2}"" title=""{2}"" onclick=""setStyle('{4}','')"" />", 
        ResolveUrl(image), 
        SafeID, 
        title, 
        cmd, 
        id);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      writer.WriteLine("<table border='0' cellpadding='0' cellspacing='2' width='100%'>");
      writer.WriteLine("<tr><td valign='top'>");
      writer.WriteLine("	<table border='0' cellpadding='1' cellspacing='2' id='bbcodeFeatures'>");
      writer.WriteLine("	<tr><td valign='middle'>");

      RenderButton(writer, "bold", "FormatText('bold','')", YafContext.Current.Localization.GetText("COMMON", "TT_BOLD"), "yafEditor/bold.gif");
      RenderButton(writer, "italic", "FormatText('italic','')", YafContext.Current.Localization.GetText("COMMON", "TT_ITALIC"), "yafEditor/italic.gif");
      RenderButton(
        writer, "underline", "FormatText('underline','')", YafContext.Current.Localization.GetText("COMMON", "TT_UNDERLINE"), "yafEditor/underline.gif");

      writer.WriteLine("&nbsp;");

      RenderButton(writer, "quote", "FormatText('quote','')", YafContext.Current.Localization.GetText("COMMON", "TT_QUOTE"), "yafEditor/quote.gif");
      RenderButton(writer, "code", "FormatText('code','')", YafContext.Current.Localization.GetText("COMMON", "TT_CODE"), "yafEditor/code.gif");
      RenderButton(writer, "img", "FormatText('img','')", YafContext.Current.Localization.GetText("COMMON", "TT_IMAGE"), "yafEditor/image.gif");
      RenderButton(
        writer, "createlink", "FormatText('createlink','')", YafContext.Current.Localization.GetText("COMMON", "TT_CREATELINK"), "yafEditor/link.gif");

      writer.WriteLine("&nbsp;");

      RenderButton(
        writer, "justifyleft", "FormatText('justifyleft','')", YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNLEFT"), "yafEditor/justifyleft.gif");
      RenderButton(
        writer, 
        "justifycenter", 
        "FormatText('justifycenter','')", 
        YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNCENTER"), 
        "yafEditor/justifycenter.gif");
      RenderButton(
        writer, 
        "justifyright", 
        "FormatText('justifyright','')", 
        YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNRIGHT"), 
        "yafEditor/justifyright.gif");

      DataTable bbCodeTable = YafBBCode.GetCustomBBCode();

      if (bbCodeTable.Rows.Count > 0)
      {
        writer.WriteLine("&nbsp;");

        // add drop down for optional "extra" codes...
        writer.WriteLine(
          String.Format(
            @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />", 
            this._popMenu.ControlOnClick, 
            this._popMenu.ControlOnMouseOver, 
            YafContext.Current.Localization.GetText("COMMON", "CUSTOM_BBCODE"), 
            ClientID + "_bbcode_popMenu", 
            YafContext.Current.Localization.GetText("COMMON", "TT_CUSTOMBBCODE"), 
            ResolveUrl("yafEditor/bbcode.gif")));

        foreach (DataRow row in bbCodeTable.Rows)
        {
          string name = row["Name"].ToString();

          if (row["Description"] != DBNull.Value && !String.IsNullOrEmpty(row["Description"].ToString()))
          {
            // use the description as the option "name"
            name = YafBBCode.LocalizeCustomBBCodeElement(row["Description"].ToString());
          }

          string onclickJS = string.Empty;

          if (row["OnClickJS"] != DBNull.Value && !String.IsNullOrEmpty(row["OnClickJS"].ToString()))
          {
            onclickJS = row["OnClickJS"].ToString();
          }
          else
          {
            // assume the bbcode is just the name... 
            onclickJS = string.Format("setStyle('{0}','')", row["Name"].ToString().Trim());
          }

          this._popMenu.AddClientScriptItem(name, onclickJS);
        }
      }

      writer.WriteLine("	</td></tr>");
      writer.WriteLine("	<tr><td valign='middle'>");

      // TODO: Convert to a control...
      writer.WriteLine(YafContext.Current.Localization.GetText("COMMON", "FONT_COLOR"));
      writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">", SafeID);
      writer.WriteLine("<option value=\"\">Default</option>");

      string[] Colors = {
                          "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue", "Dark Blue", "Indigo", "Violet", "White", "Black"
                        };
      foreach (string color in Colors)
      {
        string tValue = color.Replace(" ", string.Empty).ToLower();
        writer.WriteLine(string.Format("<option style=\"color:{0}\" value=\"{0}\">{1}</option>", tValue, color));
      }

      writer.WriteLine("</select>");

      // TODO: Just convert to a drop down control...
      writer.WriteLine(YafContext.Current.Localization.GetText("COMMON", "FONT_SIZE"));
      writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">", SafeID);
      writer.WriteLine("<option value=\"1\">1</option>");
      writer.WriteLine("<option value=\"2\">2</option>");
      writer.WriteLine("<option value=\"3\">3</option>");
      writer.WriteLine("<option value=\"4\">4</option>");
      writer.WriteLine("<option selected=\"selected\" value=\"5\">Default</option>");
      writer.WriteLine("<option value=\"6\">6</option>");
      writer.WriteLine("<option value=\"7\">7</option>");
      writer.WriteLine("<option value=\"8\">8</option>");
      writer.WriteLine("<option value=\"9\">9</option>");
      writer.WriteLine("</select>");

      writer.WriteLine("	</td></tr>");
      writer.WriteLine("	</table>");
      writer.WriteLine(@"</td></tr><tr><td height=""99%"">");

      this._textCtl.RenderControl(writer);

      writer.WriteLine("</td></tr></table>");

      this._popMenu.RenderControl(writer);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      base.OnInit(e);
      this._textCtl.Attributes.Add("class", "BBCodeEditor");

      // add popmenu to this mix...
      this._popMenu = new PopMenu();
      Controls.Add(this._popMenu);
    }

    /// <summary>
    /// The editor_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void Editor_Load(object sender, EventArgs e)
    {
      base.Editor_Load(sender, e);

      // register custom YafBBCode javascript (if there is any)
      // this call is supposed to be after editor load since it may use
      // JS variables created in editor_load...
      YafBBCode.RegisterCustomBBCodePageElements(Page, GetType(), SafeID);
      YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
    }

    #region Properties

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "YAF Standard YafBBCode Editor";
      }
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override int ModuleId
    {
      get
      {
        // backward compatibility...
        return 1;
      }
    }

    #endregion
  }
}