/* YetAnotherForum.NET
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
    using YAF.Core.BBCode;
    using YAF.Types;
  using YAF.Types.Interfaces;

  /// <summary>
  /// The ibb code extensions.
  /// </summary>
  public static class IBBCodeExtensions
  {
    #region Public Methods

    /// <summary>
    /// For backwards compatibility
    /// </summary>
    /// <param name="bbCode">
    /// The bb Code.
    /// </param>
    /// <param name="message">
    /// the message to add smiles to.
    /// </param>
    /// <returns>
    /// The add smiles.
    /// </returns>
    public static string AddSmiles([NotNull] this IBBCode bbCode, [NotNull] string message)
    {
      var layers = new ProcessReplaceRules();
      bbCode.AddSmiles(layers);

      // apply...
      layers.Process(ref message);
      return message;
    }

    /// <summary>
    /// Creates the rules that convert <see cref="YafBBCode"/> to HTML
    /// </summary>
    /// <param name="ruleEngine">
    /// The rule Engine.
    /// </param>
    /// <param name="doFormatting">
    /// The do Formatting.
    /// </param>
    /// <param name="targetBlankOverride">
    /// The target Blank Override.
    /// </param>
    /// <param name="useNoFollow">
    /// The use No Follow.
    /// </param>
    public static void CreateBBCodeRules(this IBBCode bbcode, IProcessReplaceRules ruleEngine, bool doFormatting, bool targetBlankOverride, bool useNoFollow)
    {
      bbcode.CreateBBCodeRules(ruleEngine, doFormatting, targetBlankOverride, useNoFollow, true);
    }

    #endregion
  }
}