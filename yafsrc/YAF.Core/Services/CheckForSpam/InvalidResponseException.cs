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
    using System.Net;
    using System.Runtime.Serialization;

    using YAF.Types;

    #endregion

    /// <summary>
    /// Exception thrown when a response other than 200 is returned.
    /// </summary>
    /// <remarks>
    /// This exception does not have any custom properties, 
    ///   thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    public sealed class InvalidResponseException : Exception
    {
        #region Constants and Fields

        /// <summary>
        /// The status.
        /// </summary>
        private readonly HttpStatusCode status = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "InvalidResponseException" /> class.
        /// </summary>
        public InvalidResponseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InvalidResponseException([NotNull] string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public InvalidResponseException([NotNull] string message, [NotNull] Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        public InvalidResponseException([NotNull] string message, HttpStatusCode status)
            : base(message)
        {
            this.status = status;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidResponseException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private InvalidResponseException([NotNull] SerializationInfo info, StreamingContext context)
        {
            this.status = (HttpStatusCode)info.GetValue("Status", typeof(HttpStatusCode));
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the HTTP status returned by the service.
        /// </summary>
        /// <value>The HTTP status.</value>
        public HttpStatusCode HttpStatus
        {
            get
            {
                return this.status;
            }
        }

        #endregion
    }
}