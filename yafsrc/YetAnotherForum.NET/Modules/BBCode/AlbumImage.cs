/* YetAnotherForum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Modules.BBCode
{
    using System.Text;
    using System.Web.UI;

    using YAF.Controls;
    using YAF.Utils;

    /// <summary>
    /// The Album Image BB Code Module.
    /// </summary>
    public class AlbumImage : YafBBCodeControl
    {
        /// <summary>
        /// Render The Album Image as Link with Image
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();
            
            sb.AppendFormat(
                @"<a href=""{0}resource.ashx?image={1}"" class=""ceebox"">",
                YafForumInfo.ForumClientFileRoot,
                Parameters["inner"]);

            sb.AppendFormat(
                @"<img src=""{0}resource.ashx?imgprv={1}"" />",
                YafForumInfo.ForumClientFileRoot,
                Parameters["inner"]);

            sb.Append("</a>");

            writer.Write(sb.ToString());
        }
    }
}