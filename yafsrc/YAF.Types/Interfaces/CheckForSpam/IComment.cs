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

    #endregion

    /// <summary>
    /// Defines the base information about a comment submitted to the client.
    /// </summary>
    public interface IComment
    {
        #region Properties

        /// <summary>
        ///   Gets the name submitted with the comment.
        /// </summary>
        string Author { get; }

        /// <summary>
        ///   Gets the email submitted with the comment.
        /// </summary>
        string AuthorEmail { get; }

        /// <summary>
        ///   Gets the url submitted if provided.
        /// </summary>
        Uri AuthorUrl { get; }

        /// <summary>
        ///   May be one of the following: {blank}, "comment", "trackback", "pingback", or a made-up value 
        ///   like "registration".
        /// </summary>
        string CommentType { get; }

        /// <summary>
        ///   Gets the Content of the comment
        /// </summary>
        string Content { get; }

        /// <summary>
        ///   Gets the IPAddress of the submitter
        /// </summary>
        IPAddress IPAddress { get; }

        /// <summary>
        ///   Permanent location of the entry the comment was 
        ///   submitted to.
        /// </summary>
        Uri Permalink { get; }

        /// <summary>
        ///   The HTTP_REFERER header value of the 
        ///   originating comment.
        /// </summary>
        string Referrer { get; }

        /// <summary>
        ///   Optional collection of various server environment variables.
        /// </summary>
        NameValueCollection ServerEnvironmentVariables { get; }

        /// <summary>
        ///   User agent of the requester. (Required)
        /// </summary>
        string UserAgent { get; }

        #endregion
    }
}