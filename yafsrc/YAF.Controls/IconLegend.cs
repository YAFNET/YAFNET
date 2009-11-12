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
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class IconLegend : BaseControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="IconLegend"/> class.
    /// </summary>
    public IconLegend()
    {
      Load += new EventHandler(IconLegend_Load);
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
    private void IconLegend_Load(object sender, EventArgs e)
    {
      string[] themeImageTags = {
                                  "TOPIC_NEW", "TOPIC", "TOPIC_NEW_LOCKED", "TOPIC_LOCKED", "TOPIC_ANNOUNCEMENT", "TOPIC_STICKY", "TOPIC_MOVED", "TOPIC_POLL"
                                };
      string[] localizedTags = {
                                 "NEW_POSTS", "NO_NEW_POSTS", "NEW_POSTS_LOCKED", "NO_NEW_POSTS_LOCKED", "ANNOUNCEMENT", "STICKY", "MOVED", "POLL"
                               };

      HtmlTableRow tr = null;
      HtmlTableCell td = null;

      // add a table control
      var table = new HtmlTable();
      table.Attributes.Add("class", "iconlegend");
      Controls.Add(table);

      for (int i = 0; i < themeImageTags.Length; i++)
      {
        if ((i%2) == 0 || tr == null)
        {
          // add <tr>
          tr = new HtmlTableRow();
          table.Controls.Add(tr);
        }

        // add this to the tr...
        td = new HtmlTableCell();
        tr.Controls.Add(td);

        // add the themed icons
        var themeImage = new ThemeImage();
        themeImage.ThemeTag = themeImageTags[i];
        td.Controls.Add(themeImage);

        // space
        var space = new Literal();
        space.Text = " ";
        td.Controls.Add(space);

        // localized text describing the image
        var localLabel = new LocalizedLabel();
        localLabel.LocalizedTag = localizedTags[i];
        td.Controls.Add(localLabel);
      }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      base.Render(writer);
    }
  }
}