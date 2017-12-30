/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
namespace YAF.Core.Tasks
{
    #region Using

    using System;
    using System.Diagnostics;

    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Sends Email in the background.
    /// </summary>
    public class MailSendTask : IntermittentBackgroundTask
    {
        /// <summary>
        /// The _send mail threaded
        /// </summary>
        private ISendMailThreaded _sendMailThreaded;

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MailSendTask" /> class.
        /// </summary>
        public MailSendTask()
        {
            // set the unique value...
            var rand = new Random();

            // set interval values...
            this.RunPeriodMs = (rand.Next(10) + 5) * 1000;
            this.StartDelayMs = (rand.Next(30) + 15) * 1000;
        }

        #endregion

        #region Constants and Fields


        /// <summary>
        /// Gets or sets the ServiceLocator.
        /// </summary>
        [Inject]
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets the send mail threaded.
        /// </summary>
        /// <value>
        /// The send mail threaded.
        /// </value>
        public ISendMailThreaded SendMailThreaded => this._sendMailThreaded ?? (this._sendMailThreaded = this.ServiceLocator.Get<ISendMailThreaded>());

        #endregion

        #region Public Methods

        /// <summary>
        /// The run once.
        /// </summary>
        public override void RunOnce()
        {
            this.Logger.Debug("Running Send Mail Thread....");
            this.SendMailThreaded.SendThreaded();
        }

        #endregion
    }
}