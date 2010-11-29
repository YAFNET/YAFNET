/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;

  /// <summary>
  /// Summary description for SimpleBaseModule
  /// </summary>
  public class SimpleBaseModule : IBaseModule
  {
    /// <summary>
    /// The _forum control obj.
    /// </summary>
    protected object _forumControlObj;

    /// <summary>
    /// The _forum page type.
    /// </summary>
    protected ForumPages _forumPageType;

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    /// <summary>
    /// Gets CurrentForumPage.
    /// </summary>
    public ForumPage CurrentForumPage
    {
      get
      {
        return PageContext.CurrentForumPage;
      }
    }

    /// <summary>
    /// Gets ForumControl.
    /// </summary>
    public Forum ForumControl
    {
      get
      {
        return (Forum) ForumControlObj;
      }
    }

    /// <summary>
    /// Gets ForumPageType.
    /// </summary>
    public ForumPages ForumPageType
    {
      get
      {
        return PageContext.ForumPageType;
      }
    }

    #region IBaseModule Members

    /// <summary>
    /// The init.
    /// </summary>
    public void Init()
    {
      ForumControl.BeforeForumPageLoad += new EventHandler<YafBeforeForumPageLoad>(ForumControl_BeforeForumPageLoad);
      ForumControl.AfterForumPageLoad += new EventHandler<YafAfterForumPageLoad>(ForumControl_AfterForumPageLoad);
      InitForum();
    }

    /// <summary>
    /// Gets or sets ForumControlObj.
    /// </summary>
    public object ForumControlObj
    {
      get
      {
        return this._forumControlObj;
      }

      set
      {
        this._forumControlObj = value;
      }
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public virtual void Dispose()
    {
    }

    #endregion

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

    /// <summary>
    /// The forum control_ after forum page load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_AfterForumPageLoad(object sender, YafAfterForumPageLoad e)
    {
      InitAfterPage();
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
    private void ForumControl_BeforeForumPageLoad(object sender, YafBeforeForumPageLoad e)
    {
      InitBeforePage();
    }
  }
}