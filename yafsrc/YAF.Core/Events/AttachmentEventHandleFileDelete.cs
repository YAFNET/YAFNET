/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2017 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core
{
    using System;
    using System.Linq;

    using YAF.Classes;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types.Attributes;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The attachment event handle file delete.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class AttachmentEventHandleFileDelete : IHandleEvent<RepositoryEvent<Attachment>>
    {
        #region Fields

        /// <summary>
        /// The _attachment repository.
        /// </summary>
        private readonly IRepository<Attachment> _attachmentRepository;

        /// <summary>
        /// The _board settings.
        /// </summary>
        private readonly YafBoardSettings _boardSettings;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentEventHandleFileDelete"/> class.
        /// </summary>
        /// <param name="boardSettings">
        /// The board settings.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="attachmentRepository">
        /// The attachment repository.
        /// </param>
        public AttachmentEventHandleFileDelete(YafBoardSettings boardSettings, ILogger logger, IRepository<Attachment> attachmentRepository)
        {
            this._boardSettings = boardSettings;
            this._logger = logger;
            this._attachmentRepository = attachmentRepository;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return 10000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(RepositoryEvent<Attachment> @event)
        {
            if (@event.RepositoryEventType != RepositoryEventType.Delete || this._boardSettings.UseFileTable)
            {
                return;
            }

            if (@event.Entity == null)
            {
                @event.Entity = this._attachmentRepository.ListTyped(attachmentID: @event.EntityId).FirstOrDefault();
            }

            if (@event.Entity == null)
            {
                return;
            }

            try
            {
                @event.Entity.DeleteFile();
            }
            catch (Exception e)
            {
                // error deleting that file... 
                this._logger.Warn(e, "Error Deleting Attachment");
            }
        }

        #endregion
    }
}