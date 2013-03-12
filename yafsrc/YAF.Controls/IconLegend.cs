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

    using System;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;

    #endregion

    /// <summary>
    /// Icon Legend Control to Render Topic Icons
    /// </summary>
    public class IconLegend : BaseControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "IconLegend" /> class.
        /// </summary>
        public IconLegend()
        {
            Load += this.IconLegend_Load;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        /// <summary>
        /// The icon legend_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void IconLegend_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            string[] themeImageTags = {
                                          "TOPIC_NEW", "TOPIC", "TOPIC_HOT_NEW", "TOPIC_HOT", "TOPIC_NEW_LOCKED", "TOPIC_LOCKED", "TOPIC_ANNOUNCEMENT_NEW", "TOPIC_ANNOUNCEMENT",
                                          "TOPIC_STICKY_NEW", "TOPIC_STICKY", "TOPIC_POLL_NEW", "TOPIC_POLL", "TOPIC_MOVED"
                                      };

            string[] localizedTags = {
                                         "NEW_POSTS", "NO_NEW_POSTS", "HOT_NEW_POSTS", "HOT_NO_NEW_POSTS", "NEW_POSTS_LOCKED", "NO_NEW_POSTS_LOCKED",
                                         "ANNOUNCEMENT_NEW", "ANNOUNCEMENT", "STICKY_NEW", "STICKY", "POLL_NEW",  "POLL", "MOVED"
                                     };

            HtmlTableRow tr = null;

            // add a table control
            var table = new HtmlTable();
            table.Attributes.Add("class", "iconlegend");
            Controls.Add(table);

            for (int i = 0; i < themeImageTags.Length; i++)
            {
                if ((i % 2) == 0 || tr == null)
                {
                    // add <tr>
                    tr = new HtmlTableRow();
                    table.Controls.Add(tr);
                }

                // add this to the tr...
                HtmlTableCell td = new HtmlTableCell();
                tr.Controls.Add(td);

                // add the themed icons
                var themeImage = new ThemeImage { ThemeTag = themeImageTags[i] };
                td.Controls.Add(themeImage);

                // space
                var space = new Literal { Text = " " };
                td.Controls.Add(space);

                // localized text describing the image
                var localLabel = new LocalizedLabel { LocalizedTag = localizedTags[i] };
                td.Controls.Add(localLabel);
            }
        }

        #endregion
    }
}