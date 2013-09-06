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
namespace YAF.Core.Services.Logger
{
  #region Using

    using System;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// The yaf db logger provider.
  /// </summary>
  public class YafDbLoggerProvider : ILoggerProvider
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="YafDbLoggerProvider"/> class.
    /// </summary>
    /// <param name="injectServices">
    /// The inject services.
    /// </param>
    public YafDbLoggerProvider([NotNull] IInjectServices injectServices)
    {
      this.InjectServices = injectServices;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets InjectServices.
    /// </summary>
    public IInjectServices InjectServices { get; set; }

    #endregion

    #region Implemented Interfaces

    #region ILoggerProvider

    /// <summary>
    /// The create.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public ILogger Create([CanBeNull] Type type)
    {
      var logger = new YafDbLogger(type);
      this.InjectServices.Inject(logger);

      return logger;
    }

    #endregion

    #endregion
  }
}