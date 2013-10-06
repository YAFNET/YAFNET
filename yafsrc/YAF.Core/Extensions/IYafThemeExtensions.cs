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
namespace YAF.Core
{
  #region Using

  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The i yaf theme extensions.
  /// </summary>
  public static class IYafThemeExtensions
  {
    #region Public Methods

    /// <summary>
    /// Gets the collapsible panel image url (expanded or collapsed).
    /// </summary>
    /// <param name="theme">
    /// The theme.
    /// </param>
    /// <param name="panelID">
    /// ID of collapsible panel
    /// </param>
    /// <param name="defaultState">
    /// Default Panel State
    /// </param>
    /// <returns>
    /// Image URL
    /// </returns>
    public static string GetCollapsiblePanelImageURL(
      [NotNull] this ITheme theme, [NotNull] string panelID, CollapsiblePanelState defaultState)
    {
      CodeContracts.VerifyNotNull(theme, "theme");
      CodeContracts.VerifyNotNull(panelID, "panelID");

      CollapsiblePanelState stateValue = YafContext.Current.Get<IYafSession>().PanelState[panelID];
      if (stateValue == CollapsiblePanelState.None)
      {
        stateValue = defaultState;
        YafContext.Current.Get<IYafSession>().PanelState[panelID] = defaultState;
      }

      return theme.GetItem("ICONS", stateValue == CollapsiblePanelState.Expanded ? "PANEL_COLLAPSE" : "PANEL_EXPAND");
    }

    #endregion
  }
}