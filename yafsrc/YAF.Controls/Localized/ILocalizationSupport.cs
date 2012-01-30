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

  

  #endregion

  /// <summary>
  /// The localization support interface
  /// </summary>
  public interface ILocalizationSupport
  {
    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether EnableBBCode.
    /// </summary>
    bool EnableBBCode { get; set; }

    /// <summary>
    /// Gets or sets LocalizedPage.
    /// </summary>
    string LocalizedPage { get; set; }

    /// <summary>
    /// Gets or sets LocalizedTag.
    /// </summary>
    string LocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets Param0.
    /// </summary>
    string Param0 { get; set; }

    /// <summary>
    /// Gets or sets Param1.
    /// </summary>
    string Param1 { get; set; }

    /// <summary>
    /// Gets or sets Param2.
    /// </summary>
    string Param2 { get; set; }

    #endregion
  }
}