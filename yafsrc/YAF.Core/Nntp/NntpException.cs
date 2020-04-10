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
namespace YAF.Core.Nntp
{
    using System;

    /// <summary>
    /// The nntp exception.
    /// </summary>
    public class NntpException : Exception
    {
        /// <summary>
        /// The _message.
        /// </summary>
        private string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public NntpException(string message)
            : base(message)
        {
            this._message = message;
            this.ErrorCode = 999;
            this.Request = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        public NntpException(int errorCode)
        {
            this.BuildNntpException(errorCode, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public NntpException(int errorCode, string request)
        {
            this.BuildNntpException(errorCode, request);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NntpException"/> class.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public NntpException(string response, string request)
        {
            this._message = response;
            this.ErrorCode = 999;
            this.Request = request;
        }

        /// <summary>
        /// Gets ErrorCode.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets Request.
        /// </summary>
        public string Request { get; private set; }

        /// <summary>
        /// Gets Message.
        /// </summary>
        public override string Message => this._message;

        /// <summary>
        /// The build nntp exception.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        private void BuildNntpException(int errorCode, string request)
        {
            this.ErrorCode = errorCode;
            this.Request = request;

            this._message = errorCode switch
                {
                    281 => "Authentication accepted.",
                    288 => "Binary data to follow.",
                    381 => "More authentication information required.",
                    400 => "Service disconnected.",
                    411 => "No such newsgroup.",
                    412 => "No newsgroup current selected.",
                    420 => "No current article has been selected.",
                    423 => "No such article number in this group.",
                    430 => "No such article found.",
                    436 => "Transfer failed - try again later.",
                    440 => "Posting not allowed.",
                    441 => "Posting failed.",
                    480 => "Authentication required.",
                    481 => "More authentication information required.",
                    482 => "Authentication rejected.",
                    500 => "Command not understood.",
                    501 => "Command syntax error.",
                    502 => "No permission.",
                    503 => "Program error, function not performed.",
                    _ => "Unknown error."
                };
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return this.InnerException != null
                       ? $"Nntp:NntpException: [Request: {this.Request}][Response: {this.ErrorCode} {this._message}]\n{this.InnerException}\n{this.StackTrace}"
                       : $"Nntp:NntpException: [Request: {this.Request}][Response: {this.ErrorCode} {this._message}]\n{this.StackTrace}";
        }
    }
}