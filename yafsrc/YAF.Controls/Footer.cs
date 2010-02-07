/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System;
using System.Text;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for Footer.
  /// </summary>
  public class Footer : BaseControl, IYafFooter
  {
    /// <summary>
    /// The _simple render.
    /// </summary>
    private bool _simpleRender = false;

    #region IYafFooter Members

    /// <summary>
    /// Gets or sets a value indicating whether SimpleRender.
    /// </summary>
    public bool SimpleRender
    {
      get
      {
        return this._simpleRender;
      }

      set
      {
        this._simpleRender = value;
      }
    }

    /// <summary>
    /// Gets ThisControl.
    /// </summary>
    public Control ThisControl
    {
      get
      {
        return this;
      }
    }

    #endregion

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      if (!SimpleRender)
      {
        RenderRegular(ref writer);
      }

      base.Render(writer);
    }

    /// <summary>
    /// The render regular.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderRegular(ref HtmlTextWriter writer)
    {
      // BEGIN FOOTER
      var footer = new StringBuilder();

      // get the theme credit info from the theme file
      // it's not really an error if it doesn't exist
      string themeCredit = PageContext.Theme.GetItem("THEME", "CREDIT", null);

      YafServices.StopWatch.Stop();

      footer.Append(@"<br/><div class=""content"" style=""text-align:right;font-size:7pt"">");

      bool br = false;

      // append theme Credit if it exists...
      if (!String.IsNullOrEmpty(themeCredit))
      {
        footer.AppendFormat(@"<span id=""themecredit"" style=""color:#999999"">{0}</span>", themeCredit);
        br = true;
      }

      if (PageContext.CurrentForumPage.IsAdminPage)
      {
        if (br)
        {
          footer.Append(" | ");
        }

        // show admin icons license...
        footer.Append(
          @"<span style=""color:#999999""><a target=""_blank"" href=""http://www.pinvoke.com/"">Fugue Icons</a> &copy; 2009 Yusuke Kamiyamane</span>");
        br = true;
      }

      if (br)
      {
        footer.Append("<br/>");
      }

      footer.Append(@"<a target=""_top"" title=""YetAnotherForum.NET"" href=""http://www.yetanotherforum.net"">");
      footer.Append(PageContext.Localization.GetText("COMMON", "POWERED_BY"));
      footer.Append(@" YAF");

      if (PageContext.BoardSettings.ShowYAFVersion)
      {
        footer.AppendFormat(" {0} ", YafForumInfo.AppVersionName);
        if (Config.IsDotNetNuke)
        {
          footer.Append(" Under DNN ");
        }
        else if (Config.IsRainbow)
        {
          footer.Append(" Under Rainbow ");
        }
        else if (Config.IsMojoPortal)
        {
          footer.Append(" Under MojoPortal ");
        }
        else if (Config.IsPortalomatic)
        {
          footer.Append(" Under Portalomatic ");
        }
      }

      footer.AppendFormat(
        @"</a> | <a target=""_top"" title=""{0}"" href=""{1}"">YAF &copy; 2003-2010, Yet Another Forum.NET</a>", 
        "YetAnotherForum.NET", 
        "http://www.yetanotherforum.net");

      if (PageContext.BoardSettings.ShowPageGenerationTime)
      {
        footer.Append("<br/>");
        footer.AppendFormat(PageContext.Localization.GetText("COMMON", "GENERATED"), YafServices.StopWatch.Duration);
      }

      footer.Append(@"</div>");

#if DEBUG
      if (PageContext.IsAdmin)
      {
        footer.AppendFormat(
          @"<br/><br/><div style=""width:350px;margin:auto;padding:5px;text-align:right;font-size:7pt;""><span style=""color:#990000"">YAF Compiled in <b>DEBUG MODE</b></span>.<br/>Recompile in <b>RELEASE MODE</b> to remove this information:");
        footer.Append(@"<br></br><a href=""http://validator.w3.org/check?uri=referer"" >XHTML</a> | ");
        footer.Append(@"<a href=""http://jigsaw.w3.org/css-validator/check/referer"" >CSS</a><br></br>");
        footer.AppendFormat(
          @"<br></br>{0} sql queries ({1:N3} seconds, {2:N2}%).<br></br>{3}", 
          QueryCounter.Count, 
          QueryCounter.Duration, 
          (100*QueryCounter.Duration)/YafServices.StopWatch.Duration, 
          QueryCounter.Commands);
        footer.Append("</div>");
      }

#endif

      // write CSS, Refresh, then header...
      writer.Write(footer);
    }
  }
}