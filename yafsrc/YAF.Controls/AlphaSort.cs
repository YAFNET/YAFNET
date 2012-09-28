/* Yet Another Forum.NET
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

namespace YAF.Controls
{
  #region Using

    using System;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Extensions;
    using YAF.Utils;

  #endregion

    /// <summary>
  /// Control displaying list of letters and/or characters for filtering list of members.
  /// </summary>
  public class AlphaSort : BaseControl
  {
    #region Properties

    /// <summary>
    ///   Gets actually selected letter.
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

    #region Methods

    /// <summary>
    /// Raises the Load event.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad([NotNull] EventArgs e)
    {
      // IMPORTANT: call base implementation - calls event handlers
      base.OnLoad(e);

      var alphaSortDefList = new HtmlGenericControl("dl");
      alphaSortDefList.Attributes.Add("class", "AlphaSort");
      this.Controls.Add(alphaSortDefList);

      var headerTitle = new HtmlGenericControl("dt") { InnerText = this.GetText("ALPHABET_FILTER") };
      headerTitle.Attributes.Add("class", "header1");
      alphaSortDefList.Controls.Add(headerTitle);

      // get the localized character set
      string[] charSet = this.GetText("LANGUAGE", "CHARSET").Split('/');

      foreach (string t in charSet)
      {
          // get the current selected character (if there is one)
          char selectedLetter = this.CurrentLetter;

          // go through all letters in a set
          foreach (char letter in t)
          {
              var alphaListItem = new HtmlGenericControl("dd");

              // is letter selected?
              if (selectedLetter != char.MinValue && selectedLetter == letter)
              {
                  // current letter is selected, use specified style
                  alphaListItem.Attributes.Add("class", "SelectedLetter");
              }

              // create a link to this letter
              var link = new HyperLink
                  {
                      ToolTip = this.GetTextFormatted("ALPHABET_FILTER_BY", letter.ToString()),
                      Text = letter.ToString(),
                      NavigateUrl =
                          YafBuildLink.GetLinkNotEscaped(ForumPages.members, "letter={0}", letter == '#' ? '_' : letter)
                  };

              alphaListItem.Controls.Add(link);

              alphaSortDefList.Controls.Add(alphaListItem);
          }
      }
    }

    #endregion
  }
}