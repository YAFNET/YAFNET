/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Controls
{
  using System;
  using YAF.Classes.Core;

  /// <summary>
  /// Summary description for DisplayAd.
  /// </summary>
  public partial class DisplayAd : BaseUserControl
  {
    /// <summary>
    /// Gets or sets a value indicating whether IsAlt.
    /// </summary>
    public bool IsAlt
    {
      get;

      set;
    }


    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      this.AdMessage.Message = PageContext.BoardSettings.AdPost;
      this.AdMessage.Signature = PageContext.Localization.GetText("AD_SIGNATURE");

      this.AdMessage.MessageFlags.IsLocked = true;
      this.AdMessage.MessageFlags.NotFormatted = true;
    }

    /// <summary>
    /// The get post class.
    /// </summary>
    /// <returns>
    /// The get post class.
    /// </returns>
    protected string GetPostClass()
    {
      return this.IsAlt ? "post_alt" : "post";
    }
  }
}