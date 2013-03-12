/* Yet Another Forum.net
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

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Control displaying list of user currently active on a forum.
    /// </summary>
    public class RoleRankStyles : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets styles data string to display in control.
        /// </summary>
        [CanBeNull]
        public string RawStyles { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Implements rendering of control to the client through use of <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            // we shall continue only if there is a style string available
            if (this.RawStyles.IsNotSet())
            {
                return;
            }

            // writes starting tag
            writer.WriteLine(@"<span class=""grouprankstyles"" id=""{0}"">".FormatWith(this.ClientID));

            int index = 1;

            var styles = this.RawStyles.Split('/').Where(s => s.IsSet()).Select(s => s.Trim().Split('!')).ToList();

            foreach (var styleElements in styles)
            {
                if (styleElements.Length >= 2)
                {
                    writer.WriteLine(
                        "<span style=\"{0}\">{1}{2}</span>".FormatWith(
                            styleElements[1], styleElements[0], (index++ == styles.Count) ? string.Empty : ", "));
                }

                // write ending tag
                writer.WriteLine("</span>");
            }
        }

        #endregion
    }
}