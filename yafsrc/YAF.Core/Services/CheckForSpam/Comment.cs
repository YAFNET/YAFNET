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

namespace YAF.Core.Services.CheckForSpam
{
    #region Using

    using System;
    using System.Collections.Specialized;
    using System.Net;

    using YAF.Types;

    #endregion

    /// <summary>
    /// The comment.
    /// </summary>
    public class Comment : IComment
    {
        #region Constants and Fields

        /// <summary>
        /// The ip address.
        /// </summary>
        private readonly IPAddress ipAddress;

        /// <summary>
        /// The server environment variables.
        /// </summary>
        private readonly NameValueCollection serverEnvironmentVariables = new NameValueCollection();

        /// <summary>
        /// The user agent.
        /// </summary>
        private readonly string userAgent;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="authorIpAddress">
        /// The author ip address.
        /// </param>
        /// <param name="authorUserAgent">
        /// The author user agent.
        /// </param>
        public Comment([NotNull] IPAddress authorIpAddress, [NotNull] string authorUserAgent)
        {
            this.ipAddress = authorIpAddress;
            this.userAgent = authorUserAgent;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The name submitted with the comment.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///   The email submitted with the comment.
        /// </summary>
        public string AuthorEmail { get; set; }

        /// <summary>
        ///   The url submitted if provided.
        /// </summary>
        public Uri AuthorUrl { get; set; }

        /// <summary>
        ///   May be one of the following: {blank}, "comment", "trackback", "pingback", or a made-up value 
        ///   like "registration".
        /// </summary>
        public string CommentType { get; set; }

        /// <summary>
        ///   Content of the comment
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///   IPAddress of the submitter
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                return this.ipAddress;
            }
        }

        /// <summary>
        ///   Permanent location of the entry the comment was 
        ///   submitted to.
        /// </summary>
        public Uri Permalink { get; set; }

        /// <summary>
        ///   The HTTP_REFERER header value of the 
        ///   originating comment.
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        ///   Optional collection of various server environment variables.
        /// </summary>
        public NameValueCollection ServerEnvironmentVariables
        {
            get
            {
                return this.serverEnvironmentVariables;
            }
        }

        /// <summary>
        ///   User agent of the requester. (Required)
        /// </summary>
        public string UserAgent
        {
            get
            {
                return this.userAgent;
            }
        }

        #endregion
    }
}