/* YetAnotherForum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
namespace YAF.Modules
{
  using System.Text;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Controls;
  using YAF.Types.Constants;

  /// <summary>
  /// The spoiler bb code module.
  /// </summary>
  public class SpoilerBBCodeModule : YafBBCodeControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SpoilerBBCodeModule"/> class.
    /// </summary>
    public SpoilerBBCodeModule()
      : base()
    {
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      var sb = new StringBuilder();

      string spoilerTitle = this.HtmlEncode(LocalizedString("SPOILERMOD_TOOLTIP", "Click here to show or hide the hidden text (also known as a spoiler)"));

      sb.AppendLine("<!-- BEGIN spoiler -->");
      sb.AppendLine(@"<div class=""spoilertitle"">");
      sb.AppendFormat(
        @"<input type=""button"" value=""{2}"" class=""spoilerbutton"" name=""{0}"" onclick='toggleSpoiler(this,""{1}"");' title=""{3}"" /></div><div class=""spoilerbox"" id=""{1}"" style=""display:none"">", 
        this.GetUniqueID("spoilerBtn"), 
        this.GetUniqueID("spoil_"), 
        this.HtmlEncode(LocalizedString("SPOILERMOD_SHOW", "Show Spoiler")), 
        spoilerTitle);
      sb.AppendLine(Parameters["inner"]);
      sb.AppendLine("</div>");
      sb.AppendLine("<!-- END spoiler -->");

      writer.Write(sb.ToString());
    }
  }
}