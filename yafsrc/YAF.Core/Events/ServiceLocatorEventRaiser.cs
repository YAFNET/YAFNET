/* Yet Another Forum.NET
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
namespace YAF.Core
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The service locator event raiser.
    /// </summary>
    public class ServiceLocatorEventRaiser : IRaiseEvent
    {
        #region Fields

        /// <summary>
        ///     The _service locator.
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorEventRaiser"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service Locator.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ServiceLocatorEventRaiser([NotNull] IServiceLocator serviceLocator, ILogger logger)
        {
            this.Logger = logger;
            this._serviceLocator = serviceLocator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The event raiser.
        /// </summary>
        /// <param name="eventObject">
        /// The event object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public void Raise<T>(T eventObject) where T : IAmEvent
        {
            foreach (var x in this.GetAggregatedAndOrderedEventHandlers<T>())
            {
                x.Handle(eventObject);
            }
        }

        /// <summary>
        /// Raise all events using try/catch block.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="eventObject">
        /// </param>
        /// <param name="logExceptionAction">
        /// </param>
        public void RaiseIssolated<T>(T eventObject, [CanBeNull] Action<string, Exception> logExceptionAction)
            where T : IAmEvent
        {
            foreach (var theHandler in this.GetAggregatedAndOrderedEventHandlers<T>())
            {
                try
                {
                    theHandler.Handle(eventObject);
                }
                catch (Exception ex)
                {
                    if (logExceptionAction != null)
                    {
                        logExceptionAction(theHandler.GetType().Name, ex);
                    }
                    else
                    {
                        this.Logger.Error(ex, "Exception Raising Event to Handler: {0}", theHandler.GetType().Name);
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The get event handlers aggregated and ordered.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IList" />.
        /// </returns>
        private IList<IHandleEvent<T>> GetAggregatedAndOrderedEventHandlers<T>() where T : IAmEvent
        {
            return this._serviceLocator.Get<IEnumerable<IHandleEvent<T>>>()
                       .Concat(this._serviceLocator.Get<IEnumerable<IFireEvent<T>>>())
                       .OrderBy(x => x.Order)
                       .ToList();
        }

        #endregion
    }
}