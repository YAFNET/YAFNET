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
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  using YAF.Classes.Core;

  /// <summary>
  /// Control displaying list of letters and/or characters for filtering list of members.
  /// </summary>
  public class AlphaSort : BaseControl
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AlphaSort"/> class. 
    /// Default constructor
    /// </summary>
    public AlphaSort()
    {
    }

    #endregion

    #region Page Arguments

    /// <summary>
    /// Gets actually selected letter.
    /// </summary>
    public char CurrentLetter
    {
      get
      {
        char currentLetter = char.MinValue;

        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("letter") != null)
        {
          // try to convert to char
          char.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("letter"), out currentLetter);

          // since we cannot use '#' in URL, we use '_' instead, this is to give it the right meaning
          if (currentLetter == '_')
          {
            currentLetter = '#';
          }
        }

        return currentLetter;
      }
    }

    #endregion

    #region Control Load & Rendering

    /// <summary>
    /// Raises the Load event.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      // IMPORTANT: call base implementation - calls event handlers
      base.OnLoad(e);

      // it's gonna be a table containing those letters in cells
      var table = new HtmlTable();

      // define table attributes
      table.Attributes.Add("class", "content");
      table.Width = "100%";

      // add table to our control so it can be rendered
      Controls.Add(table);

      // get the localized character set
      string[] charSet = PageContext.Localization.GetText("LANGUAGE", "CHARSET").Split('/');

      for (int i = 0; i < charSet.Length; i++)
      {
      // create row for letters and attach it to the table
      var tr = new HtmlTableRow();
      table.Controls.Add(tr);      
     
          // get the current selected character (if there is one)
          char selectedLetter = CurrentLetter;

          // go through all letters in a set
          foreach (char letter in charSet[i])
          {
              // create cell for the letter and define its properties
              var cell = new HtmlTableCell();
              cell.Align = "center";

              // is letter selected?
              if (selectedLetter != char.MinValue && selectedLetter == letter)
              {
                  // current letter is selected, use specified style
                  cell.Attributes["class"] = "postheader";
              }
              else
              {
                  // regular non-selected letter
                  cell.Attributes["class"] = "post";
              }

              // create a link to this letter
              var link = new HyperLink();
              link.Text = letter.ToString();
              link.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.members, "letter={0}", letter == '#' ? '_' : letter);

              // add link to the cell
              cell.Controls.Add(link);

              // add this cell to the row
              tr.Cells.Add(cell);
          }
      }
  
    }

    #endregion

    /* Construction & Destruction */

    /* Properties */

    /* Control Processing Methods */
  }
}