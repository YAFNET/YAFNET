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
namespace YAF.Types.Interfaces
{
    /// <summary>
    /// The digest interface
    /// </summary>
    public interface IDigest
    {
        #region Public Methods

        /// <summary>
        /// Gets the digest HTML.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
        /// <returns>
        /// The get digest html.
        /// </returns>
        string GetDigestHtml(int userId, object boardSettings, bool showErrors = false);

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        string GetDigestUrl(int userId, object boardSettings);

        /// <summary>
        /// Gets the digest URL.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="boardSettings">The board settings.</param>
        /// <param name="showErrors">Show digest generation errors</param>
        /// <returns>
        /// The get digest url.
        /// </returns>
        string GetDigestUrl(int userId, object boardSettings, bool showErrors);

        /// <summary>
        /// Sends the digest html to the email/name specified.
        /// </summary>
        /// <param name="digestHtml">The digest html.</param>
        /// <param name="forumName">The forum name.</param>
        /// <param name="forumEmail">The forum email.</param>
        /// <param name="toEmail">The to email.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="sendQueued">The send queued.</param>
        void SendDigest(
            [NotNull] string digestHtml,
            [NotNull] string forumName,
            [NotNull] string forumEmail,
            [NotNull] string toEmail,
            [CanBeNull] string toName,
            bool sendQueued);

        #endregion
    }
}