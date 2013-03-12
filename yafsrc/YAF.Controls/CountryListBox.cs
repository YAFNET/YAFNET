/* Yet Another Forum.NET
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

namespace YAF.Controls
{
    #region Using

    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Custom DropDown List Controls with Images
    /// </summary>
    public class CountryListBox : DropDownList
    {
        #region Methods

        /// <summary>
        /// Add Flag Images to Items
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render(HtmlTextWriter writer)
        {
            foreach (ListItem item in this.Items.Cast<ListItem>().Where(item => item.Value.IsSet()))
            {
                item.Attributes.Add(
                    "title", "{1}resources/images/flags/{0}.png".FormatWith(item.Value, YafForumInfo.ForumClientFileRoot));
            }

            base.Render(writer);
        }

        #endregion
    }
}