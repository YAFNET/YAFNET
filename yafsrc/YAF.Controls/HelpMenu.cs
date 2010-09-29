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
using System.Text;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for HelpMenu.
  /// </summary>
  public class HelpMenu : BasePanel
  {
    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
        var html = new StringBuilder(2000);

        html.Append(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafhelpmenu"">");

        html.Append(@"<tr><td class=""header1"">Navigation</td></tr>");

        html.Append(@"<tr class=""header2""><td>Index</td></tr>");

        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpindex"">");

        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.help_index, "faq=index"), PageContext.Localization.GetText("HELP_INDEX", "SEARCHHELP"));

        html.AppendFormat(@"</ul></td></tr>");

        ///////////////

        html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("HELP_INDEX", "GENERALTOPICS"));


        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpgeneral"">");

        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.help_index, "faq=anounce"), PageContext.Localization.GetText("HELP_INDEX", "ANOUNCETITLE"));
        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.help_index, "faq=recover"), PageContext.Localization.GetText("HELP_INDEX", "RECOVERTITLE"));
        
        html.AppendFormat(@"</ul></td></tr>");

        ///////////////

       /* html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("HELP_INDEX", "GENERALTOPICS"));
        

        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpsettings"">");

        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.help_index, "faq=anounce"), PageContext.Localization.GetText("HELP_INDEX", "ANOUNCETITLE"));

        html.AppendFormat(@"</ul></td></tr>");
        */
        //////////////

/*        html.AppendFormat(@"<tr class=""header2""><td>{0}</td></tr>", PageContext.Localization.GetText("HELP_INDEX", "GENERALTOPICS"));

        html.AppendFormat(@"<tr><td class=""post""><ul id=""yafhelpreading"">");

        html.AppendFormat(@"<li><a href=""{0}"">{1}</a></li>", YafBuildLink.GetLink(ForumPages.help_index, "faq=anounce"), PageContext.Localization.GetText("HELP_INDEX", "ANOUNCETITLE"));

        html.AppendFormat(@"</ul></td></tr>");
        */
        //////////////
        
        html.AppendFormat(@"</ul></td></tr>");
        html.Append(@"</table>");

        writer.BeginRender();

        // render the contents of the help menu....
        writer.WriteLine(@"<div id=""{0}"">".FormatWith(this.ClientID));
        writer.WriteLine(@"<table class=""adminContainer""><tr>");
        writer.WriteLine(@"<td class=""adminMenu"" valign=""top"">");

        writer.Write(html.ToString());

        writer.WriteLine(@"</td>");

        // contents of the help pages...
        writer.WriteLine(@"<td class=""adminContent"" valign=""top"">");

        this.RenderChildren(writer);

        writer.WriteLine(@"</td></tr></table>");
        writer.WriteLine("</div>");

        writer.EndRender();
    }
  }
}