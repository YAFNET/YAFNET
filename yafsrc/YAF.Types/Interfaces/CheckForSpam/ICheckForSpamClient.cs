/* Based on "Subkismet - The Cure For Comment Spam" v1.0: http://subkismet.codeplex.com/
 * 
 * License: New BSD License
 * -------------------------------------
 * Copyright (c) 2007-2008, Phil Haack
 * All rights reserved. 
 * Modified by Jaben Cargman for YAF in 2010 
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 * 
 * Neither the name of Subkismet nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.* 
*/

namespace YAF.Types.Interfaces.CheckForSpam
{
    #region Using

    using System;
    using System.Net;

    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;

    #endregion

    /// <summary>
    /// Interface that communicates with a spam client.
    /// </summary>
    public interface ICheckForSpamClient
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the Akismet API key.
        /// </summary>
        /// <value>The API key.</value>
        string ApiKey { get; set; }

        /// <summary>
        ///   Gets or sets the root URL to the blog.
        /// </summary>
        /// <value>The blog URL.</value>
        Uri RootUrl { get; set; }

        /// <summary>
        ///   Gets or sets the timeout in milliseconds for the http request to the client.
        /// </summary>
        /// <value>The timeout.</value>
        int Timeout { get; set; }

        /// <summary>
        ///   Gets or sets the User Agent for the Client.  
        ///   Do not confuse this with the user agent for the comment 
        ///   being checked.
        /// </summary>
        string UserAgent { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks the comment and returns true if it is spam, otherwise false.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The check comment for spam.
        /// </returns>
        bool CheckCommentForSpam([NotNull] IComment comment, out string result);

        /// <summary>
        /// Submits a comment to the client that should not have been 
        ///   flagged as SPAM (a false positive).
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        void SubmitHam([NotNull] IComment comment);

        /// <summary>
        /// Submits a comment to the client that should have been 
        ///   flagged as SPAM, but was not flagged.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        void SubmitSpam([NotNull] IComment comment);

        #endregion
    }
}