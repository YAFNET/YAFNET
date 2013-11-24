/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Display Ad Control
    /// </summary>
    public partial class DisplayAd : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether Is Alt. Row
        /// </summary>
        public bool IsAlt { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the post class.
        /// </summary>
        /// <returns>
        /// Returns the post class.
        /// </returns>
        [NotNull]
        protected string GetPostClass()
        {
            return this.IsAlt ? "post_alt" : "post";
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AdMessage.Message = this.Get<YafBoardSettings>().AdPost;
            this.AdMessage.Signature = this.GetText("AD_SIGNATURE");

            this.AdMessage.MessageFlags = new MessageFlags { IsLocked = true, NotFormatted = true };
        }

        #endregion
    }
}