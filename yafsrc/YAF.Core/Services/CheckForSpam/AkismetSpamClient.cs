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

    using YAF.Types;

    #endregion

    /// <summary>
    /// The Akismet spam client.
    /// </summary>
    public class AkismetSpamClient : CheckForSpamClientBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AkismetSpamClient"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The api key.
        /// </param>
        /// <param name="rootUrl">
        /// The root url.
        /// </param>
        public AkismetSpamClient([NotNull] string apiKey, [NotNull] Uri rootUrl)
            : base(apiKey, rootUrl)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets CheckUrlFormat.
        /// </summary>
        [NotNull]
        protected override string CheckUrlFormat => "http://{0}.rest.akismet.com/1.1/comment-check";

        /// <summary>
        /// Gets SubmitHamUrlFormat.
        /// </summary>
        [NotNull]
        protected override string SubmitHamUrlFormat => "http://{0}.rest.akismet.com/1.1/submit-ham";

        /// <summary>
        /// Gets SubmitSpamUrlFormat.
        /// </summary>
        [NotNull]
        protected override string SubmitSpamUrlFormat => "http://{0}.rest.akismet.com/1.1/submit-spam";

        /// <summary>
        /// Gets SubmitVerifyKeyFormat.
        /// </summary>
        [NotNull]
        protected override string SubmitVerifyKeyFormat => "http://rest.akismet.com/1.1/verify-key";

        #endregion
    }
}