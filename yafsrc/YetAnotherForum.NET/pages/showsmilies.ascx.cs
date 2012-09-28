/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Types.Interfaces.Extensions;
  using YAF.Types.Models;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The showsmilies.
  /// </summary>
  public partial class showsmilies : ForumPage
  {
    // constructor
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "showsmilies" /> class.
    /// </summary>
    public showsmilies()
      : base("SHOWSMILIES")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The get smiley script.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <returns>
    /// The get smiley script.
    /// </returns>
    protected string GetSmileyScript([NotNull] string code, [NotNull] string icon)
    {
      code = code.ToLower();
      code = code.Replace("&", "&amp;");
      code = code.Replace("\"", "&quot;");
      code = code.Replace("'", "\\'");

      return "javascript:{0}('{1} ','{3}{4}/{2}');".FormatWith(
        "insertsmiley", code, icon, YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.ShowToolBar = false;
      this.ShowFooter = false;

      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = this.GetRepository<Smiley>().ListUnique(this.PageContext.PageBoardID);
      this.DataBind();
    }

    #endregion
  }
}