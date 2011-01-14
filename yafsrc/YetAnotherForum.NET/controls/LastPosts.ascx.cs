/* Yet Another Forum.NET
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Utilities;

  #endregion

  /// <summary>
  /// The last posts.
  /// </summary>
  public partial class LastPosts : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   Gets or sets TopicID.
    /// </summary>
    public long? TopicID
    {
      get
      {
        if (this.ViewState["TopicID"] != null)
        {
          return Convert.ToInt32(this.ViewState["TopicID"]);
        }

        return null;
      }

      set
      {
        this.ViewState["TopicID"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The last post update timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void LastPostUpdateTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      YafContext.Current.PageElements.RegisterJsBlockStartup(
        this.LastPostUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

      base.OnPreRender(e);
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
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.repLastPosts.DataSource = this.TopicID.HasValue
                                       ? LegacyDb.post_list_reverse10(this.TopicID.Value).AsEnumerable()
                                       : null;
      this.repLastPosts.DataBind();
    }

    #endregion
  }
}