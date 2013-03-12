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
namespace YAF.Modules
{
  #region Using

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;

  #endregion

  /// <summary>
  /// Summary description for SimpleBaseModule
  /// </summary>
  public class SimpleBaseForumModule : BaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _forum page type.
    /// </summary>
    protected ForumPages _forumPageType;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets CurrentForumPage.
    /// </summary>
    public ForumPage CurrentForumPage
    {
      get
      {
        return this.PageContext.CurrentForumPage;
      }
    }

    /// <summary>
    ///   Gets ForumControl.
    /// </summary>
    public Forum ForumControl
    {
      get
      {
        return (Forum)this.ForumControlObj;
      }
    }

    /// <summary>
    ///   Gets ForumPageType.
    /// </summary>
    public ForumPages ForumPageType
    {
      get
      {
        return this.PageContext.ForumPageType;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The init.
    /// </summary>
    public override void Init()
    {
      this.ForumControl.BeforeForumPageLoad += this.ForumControl_BeforeForumPageLoad;
      this.ForumControl.AfterForumPageLoad += this.ForumControl_AfterForumPageLoad;
      this.InitForum();
    }

    /// <summary>
    /// The init after page.
    /// </summary>
    public virtual void InitAfterPage()
    {
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public virtual void InitBeforePage()
    {
    }

    /// <summary>
    /// The init forum.
    /// </summary>
    public virtual void InitForum()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum control_ after forum page load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_AfterForumPageLoad([NotNull] object sender, [NotNull] YafAfterForumPageLoad e)
    {
      this.InitAfterPage();
    }

    /// <summary>
    /// The forum control_ before forum page load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_BeforeForumPageLoad([NotNull] object sender, [NotNull] YafBeforeForumPageLoad e)
    {
      this.InitBeforePage();
    }

    #endregion
  }
}