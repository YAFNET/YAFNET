/* Yet Another Forum.NET
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

namespace YAF.Classes.Core
{
  #region Using

  using Autofac;

  using YAF.Classes.Pattern;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// The yaf service extensions.
  /// </summary>
  public static class ContainerExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get service -- basic service locator
    /// </summary>
    /// <param name="current">
    /// The current.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this YafContext current) where T : class
    {
      CodeContracts.ArgumentNotNull(current, "current");

      return current.ContextContainer.Resolve<T>();
    }

    /// <summary>
    /// The get service -- basic service locator
    /// </summary>
    /// <param name="forumPage">
    /// The forum Page.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this ForumPage forumPage) where T : class
    {
      CodeContracts.ArgumentNotNull(forumPage, "forumPage");

      return forumPage.PageContext.ContextContainer.Resolve<T>();
    }

    /// <summary>
    /// The get service -- basic service locator
    /// </summary>
    /// <param name="baseUserControl">
    /// The base User Control.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this BaseUserControl baseUserControl) where T : class
    {
      CodeContracts.ArgumentNotNull(baseUserControl, "baseUserControl");

      return baseUserControl.PageContext.ContextContainer.Resolve<T>();
    }

    /// <summary>
    /// The get service -- basic service locator
    /// </summary>
    /// <param name="baseUserControl">
    /// The base User Control.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    /// <returns>
    /// </returns>
    public static T Get<T>([NotNull] this BaseControl baseControl) where T : class
    {
      CodeContracts.ArgumentNotNull(baseControl, "baseControl");

      return baseControl.PageContext.ContextContainer.Resolve<T>();
    }

    #endregion
  }
}