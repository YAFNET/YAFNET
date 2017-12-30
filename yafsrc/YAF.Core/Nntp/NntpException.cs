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
namespace YAF.Core.Nntp
{
  using System;

  /// <summary>
  /// The nntp exception.
  /// </summary>
  public class NntpException : Exception
  {
    /// <summary>
    /// The _error code.
    /// </summary>
    private int _errorCode;

    /// <summary>
    /// The _message.
    /// </summary>
    private string _message;

    /// <summary>
    /// The _request.
    /// </summary>
    private string _request;

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
      this._errorCode = 999;
      this._request = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NntpException"/> class.
    /// </summary>
    /// <param name="errorCode">
    /// The error code.
    /// </param>
    public NntpException(int errorCode)
      : base()
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
      : base()
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
      : base()
    {
      this._message = response;
      this._errorCode = 999;
      this._request = request;
    }

    /// <summary>
    /// Gets ErrorCode.
    /// </summary>
    public int ErrorCode
    {
      get
      {
        return this._errorCode;
      }
    }

    /// <summary>
    /// Gets Request.
    /// </summary>
    public string Request
    {
      get
      {
        return this._request;
      }
    }

    /// <summary>
    /// Gets Message.
    /// </summary>
    public override string Message
    {
      get
      {
        return this._message;
      }
    }

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
      this._errorCode = errorCode;
      this._request = request;
      switch (errorCode)
      {
        case 281:
          this._message = "Authentication accepted.";
          break;
        case 288:
          this._message = "Binary data to follow.";
          break;
        case 381:
          this._message = "More authentication information required.";
          break;
        case 400:
          this._message = "Service disconnected.";
          break;
        case 411:
          this._message = "No such newsgroup.";
          break;
        case 412:
          this._message = "No newsgroup current selected.";
          break;
        case 420:
          this._message = "No current article has been selected.";
          break;
        case 423:
          this._message = "No such article number in this group.";
          break;
        case 430:
          this._message = "No such article found.";
          break;
        case 436:
          this._message = "Transfer failed - try again later.";
          break;
        case 440:
          this._message = "Posting not allowed.";
          break;
        case 441:
          this._message = "Posting failed.";
          break;
        case 480:
          this._message = "Authentication required.";
          break;
        case 481:
          this._message = "More authentication information required.";
          break;
        case 482:
          this._message = "Authentication rejected.";
          break;
        case 500:
          this._message = "Command not understood.";
          break;
        case 501:
          this._message = "Command syntax error.";
          break;
        case 502:
          this._message = "No permission.";
          break;
        case 503:
          this._message = "Program error, function not performed.";
          break;
        default:
          this._message = "Unknown error.";
          break;
      }
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
      if (this.InnerException != null)
      {
        return "Nntp:NntpException: [Request: " + this._request + "][Response: " + this._errorCode.ToString() + " " + this._message + "]\n" +
               this.InnerException.ToString() + "\n" + this.StackTrace;
      }
      else
      {
        return "Nntp:NntpException: [Request: " + this._request + "][Response: " + this._errorCode.ToString() + " " + this._message + "]\n" + this.StackTrace;
      }
    }
  }
}