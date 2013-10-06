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
namespace YAF.Controls
{
    using System.Web.UI;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  /// <summary>
  /// The localization support extensions.
  /// </summary>
  public static class LocalizationSupportExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get current item.
    /// </summary>
    /// <param name="supportItem">
    /// The support Item.
    /// </param>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <returns>
    /// The get current item.
    /// </returns>
    public static string Localize([NotNull] this ILocalizationSupport supportItem, [NotNull] Control currentControl)
    {
      CodeContracts.VerifyNotNull(supportItem, "supportItem");
      CodeContracts.VerifyNotNull(currentControl, "currentControl");

      if (currentControl.Site != null && currentControl.Site.DesignMode == true)
      {
        return "[PAGE:{0}|TAG:{1}]".FormatWith(supportItem.LocalizedPage, supportItem.LocalizedTag);
      }
      else if (supportItem.LocalizedPage.IsSet() && supportItem.LocalizedTag.IsSet())
      {
        return YafContext.Current.Get<ILocalization>().GetText(supportItem.LocalizedPage, supportItem.LocalizedTag);
      }
      else if (supportItem.LocalizedTag.IsSet())
      {
        return YafContext.Current.Get<ILocalization>().GetText(supportItem.LocalizedTag);
      }

      return null;
    }

    /// <summary>
    /// The localize and render.
    /// </summary>
    /// <param name="supportedItem">
    /// The supported item.
    /// </param>
    /// <param name="currentControl">
    /// The current control.
    /// </param>
    /// <returns>
    /// The localize and render.
    /// </returns>
    public static string LocalizeAndRender([NotNull] this ILocalizationSupport supportedItem, [NotNull] BaseControl currentControl)
    {
      CodeContracts.VerifyNotNull(supportedItem, "supportedItem");
      CodeContracts.VerifyNotNull(currentControl, "currentControl");

      string localizedItem = supportedItem.Localize(currentControl);

      // convert from YafBBCode to HTML
      if (supportedItem.EnableBBCode)
      {
        localizedItem = currentControl.Get<IBBCode>().MakeHtml(localizedItem, true, false);
      }

      return localizedItem.FormatWith(supportedItem.Param0, supportedItem.Param1, supportedItem.Param2);
    }

    #endregion
  }
}