/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// The load message.
    /// </summary>
    public class LoadMessage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LoadMessage" /> class.
        /// </summary>
        public LoadMessage()
        {
            if (this.SessionLoadString.Any())
            {
                this.LoadStringList.AddRange(this.SessionLoadString);

                // session load string no longer needed
                this.SessionLoadString.Clear();
            }

            BoardContext.Current.Unload += this.CurrentUnload;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets LoadStringList.
        /// </summary>
        [NotNull]
        public List<MessageNotification> LoadStringList { get; } = new List<MessageNotification>();

        /// <summary>
        /// Gets the session load string.
        /// </summary>
        protected List<MessageNotification> SessionLoadString
        {
            get
            {
                BoardContext.Current.Get<HttpSessionStateBase>()["LoadStringList"] ??= new List<MessageNotification>();

                return BoardContext.Current.Get<HttpSessionStateBase>()["LoadStringList"] as List<MessageNotification>;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// AddLoadMessage creates a message that will be returned on the next page load.
        /// </summary>
        /// <param name="message">The message you wish to display.</param>
        /// <param name="messageType">Type of the message.</param>
        public void Add([NotNull] string message, MessageTypes messageType)
        {
            this.LoadStringList.Add(new MessageNotification(message, messageType));
        }

        /// <summary>
        /// AddLoadMessage creates a message that will be returned on the next page load.
        /// </summary>
        /// <param name="message">
        /// The message you wish to display.
        /// </param>
        /// <param name="messageType">
        /// Type of the message.
        /// </param>
        /// <param name="script">
        /// The script.
        /// </param>
        public void Add([NotNull] string message, MessageTypes messageType, string script)
        {
            this.LoadStringList.Add(new MessageNotification(message, messageType, script));
        }

        /// <summary>
        /// AddLoadMessageSession creates a message that will be returned on the next page.
        /// </summary>
        /// <param name="message">The message you wish to display.</param>
        /// <param name="messageType">Type of the message.</param>
        public void AddSession([NotNull] string message, MessageTypes messageType)
        {
            // add it too the session list...
            this.SessionLoadString.Add(new MessageNotification(message, messageType));
        }

        /// <summary>
        /// Clear the Load String (error) List
        /// </summary>
        public void Clear()
        {
            this.LoadStringList.Clear();
        }

       /// <summary>
        /// Gets the message.
        /// </summary>
        /// <returns>Returns the Current Message</returns>
        public MessageNotification GetMessage()
        {
            return !this.LoadStringList.Any()
                       ? null
                       : this.LoadStringList.First();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the load message...
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CurrentUnload([NotNull] object sender, [NotNull] EventArgs e)
        {
            // clear the load message...
            this.Clear();
        }

        #endregion
    }
}