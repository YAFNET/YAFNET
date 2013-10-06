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
  /// The i have localization extensions.
  /// </summary>
  public static class IHaveLocalizationExtensions
  {
    #region Public Methods

    /// <summary>
    /// Gets a text localization using the page and tag name.
    /// </summary>
    /// <param name="haveLocalization">
    /// The have localization.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public static string GetText(
      [NotNull] this IHaveLocalization haveLocalization, [NotNull] string page, [NotNull] string tag)
    {
      CodeContracts.VerifyNotNull(haveLocalization, "haveLocalization");
      CodeContracts.VerifyNotNull(page, "page");
      CodeContracts.VerifyNotNull(tag, "tag");

      return haveLocalization.Localization.GetText(page, tag);
    }

    /// <summary>
    /// Gets a text localization.
    /// </summary>
    /// <param name="haveLocalization">
    /// The have localization.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <returns>
    /// The get text.
    /// </returns>
    public static string GetText([NotNull] this IHaveLocalization haveLocalization, [NotNull] string tag)
    {
      CodeContracts.VerifyNotNull(haveLocalization, "haveLocalization");
      CodeContracts.VerifyNotNull(tag, "tag");

      return haveLocalization.Localization.GetText(tag);
    }

    /// <summary>
    /// Gets a text localization using formatting.
    /// </summary>
    /// <param name="haveLocalization">
    /// The have localization.
    /// </param>
    /// <param name="tag">
    /// The tag.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    /// <returns>
    /// The get text formatted.
    /// </returns>
    public static string GetTextFormatted(
      [NotNull] this IHaveLocalization haveLocalization, [NotNull] string tag, [CanBeNull] params object[] args)
    {
      CodeContracts.VerifyNotNull(haveLocalization, "haveLocalization");
      CodeContracts.VerifyNotNull(tag, "tag");

      return haveLocalization.Localization.GetTextFormatted(tag, args);
    }

    #endregion
  }
}