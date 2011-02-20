/* Yet Another Forum.net
 * Copyright (C) 2006-2011 Jaben Cargman
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
using System.Web.UI.WebControls;

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Linq;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Control displaying list of user currently active on a forum.
  /// </summary>
    public class RoleRankStyles : BaseControl
    {
        // data about active users
        #region Constants and Fields

        /// <summary>
        ///   The _active user table.
        /// </summary>
        private string _rawStyles;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets styles data string to display in control.
        /// </summary>
        [CanBeNull]
        public string RawStyles
        {
            get
            {
                // return styles string
                return this._rawStyles;
            }

            set
            {
                this._rawStyles = value;
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Raises PreRender event.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // IMPORTANT : call base implementation, raises PreRender event
            base.OnPreRender(e);
        }

        /// <summary>
        /// Implements rendering of control to the client through use of <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            // we shall continue only if there is a style string available
            if (this.RawStyles.IsSet())
            {
                int counter = 0;
                string[] styles = this.RawStyles.Split('/');

                // writes starting tag
                writer.WriteLine(@"<span class=""grouprankstyles"" id=""{0}"">".FormatWith(this.ClientID));

                // indicates whether we are processing first style
                bool isLast = false;
              
                foreach (string row in styles)
                {
                    {
                        if (counter == styles.GetLength(0)-1)
                        {
                            // we are past first link
                            isLast = true;
                        }
                        writer.WriteLine("<span style=\"{0}\">{1}{2}</span>".FormatWith(row.Split('!')[1], row.Split('!')[0], !isLast ? ", " : string.Empty));
                        
                    }
                    counter++;
                    // write ending tag
                    writer.WriteLine("</span>");
                }
            }

        #endregion
        }
    }
  }