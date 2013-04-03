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

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;

    using EventLog = YAF.Types.Models.EventLog;

    #endregion

    /// <summary>
    ///     The yaf db logger.
    /// </summary>
    public class YafDbLogger : ILogger, IHaveServiceLocator
    {
        /// <summary>
        ///     The _event log repository.
        /// </summary>
        public IRepository<EventLog> EventLogRepository
        {
            get
            {
                return this.GetRepository<EventLog>();
            }
        }

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YafDbLogger"/> class.
        /// </summary>
        /// <param name="logType">
        /// The log type.
        /// </param>
        public YafDbLogger([CanBeNull] Type logType)
        {
            this.Type = logType;
        }

#if (DEBUG)

        /// <summary>
        ///     The _is debug.
        /// </summary>
        private bool _isDebug = true;
#else
    private bool _isDebug = false;
#endif

        /// <summary>
        ///     Gets a value indicating the logging type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        ///     The _event log type lookup.
        /// </summary>
        private Dictionary<EventLogTypes, bool> _eventLogTypeLookup;

        /// <summary>
        ///     The init lookup.
        /// </summary>
        private void InitLookup()
        {
            this._eventLogTypeLookup = new Dictionary<EventLogTypes, bool> { };

            foreach (var logType in EnumHelper.EnumToList<EventLogTypes>())
            {
                this._eventLogTypeLookup.Add(logType, true);
            }

            this._eventLogTypeLookup.AddOrUpdate(EventLogTypes.Debug, this._isDebug);
            this._eventLogTypeLookup.AddOrUpdate(EventLogTypes.Trace, this._isDebug);

            this._eventLogTypeLookup.AddOrUpdate(EventLogTypes.IpBanLifted, true);
            this._eventLogTypeLookup.AddOrUpdate(EventLogTypes.IpBanSet, true);
        }

        /// <summary>
        /// The is log type enabled.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsLogTypeEnabled(EventLogTypes type)
        {
            if (this._eventLogTypeLookup == null)
            {
                // create it...
                this.InitLookup();
            }

            return this._eventLogTypeLookup[type];
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Log(
            string message, EventLogTypes eventType = EventLogTypes.Error, string username = null, string source = null, Exception exception = null)
        {
            if (!this.IsLogTypeEnabled(eventType))
            {
                return;
            }

            var exceptionDescription = string.Empty;

            if (exception != null)
            {
                exceptionDescription = exception.ToString();
            }

            var formattedDescription = message + "\r\n" + exceptionDescription;

            if (eventType == EventLogTypes.Debug)
            {
                Debug.WriteLine(formattedDescription, source);
            }
            else if (eventType == EventLogTypes.Trace)
            {
                Trace.TraceInformation(formattedDescription);
                if (exception != null)
                {
                    Trace.TraceError(exception.ToString());
                }
            }
            else
            {
                var log = new EventLog
                              {
                                  Type = eventType, 
                                  UserName = username, 
                                  Description = formattedDescription, 
                                  Source = source ?? this.Type.FullName, 
                                  EventTime = DateTime.UtcNow
                              };

                this.EventLogRepository.Insert(log);
            }
        }
    }
}