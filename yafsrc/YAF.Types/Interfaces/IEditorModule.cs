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
namespace YAF.Types.Interfaces
{
  /// <summary>
  /// IEditorModule Interface for Editor classes.
  /// </summary>
  public interface IEditorModule : IModuleDefinition
  {
    #region Properties

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    string StyleSheet { get; set; }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    bool UsesBBCode { get; }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    bool UsesHTML { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Resolves the Url -- bad idea, needs to be replaced by an interface.
    /// </summary>
    /// <param name="relativeUrl">
    /// </param>
    /// <returns>
    /// The resolve url.
    /// </returns>
    string ResolveUrl([NotNull] string relativeUrl);

    #endregion
  }
}