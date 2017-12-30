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
  /// The article header.
  /// </summary>
  public class ArticleHeader
  {
    /// <summary>
    /// The _date.
    /// </summary>
    private DateTime _date;

    /// <summary>
    /// The _timeZoneOffset to hold an article timezone offset to UTC in minutes.
    /// </summary>
    private int _timeZoneOffset;

    /// <summary>
    /// The _from.
    /// </summary>
    private string _from;

    /// <summary>
    /// The _line count.
    /// </summary>
    private int _lineCount;

    /// <summary>
    /// The _posting host.
    /// </summary>
    private string _postingHost;

    /// <summary>
    /// The _reference ids.
    /// </summary>
    private string[] _referenceIds;

    /// <summary>
    /// The _sender.
    /// </summary>
    private string _sender;

    /// <summary>
    /// The _subject.
    /// </summary>
    private string _subject;

    /// <summary>
    /// Gets or sets ReferenceIds.
    /// </summary>
    public string[] ReferenceIds
    {
      get
      {
        return this._referenceIds;
      }

      set
      {
        this._referenceIds = value;
      }
    }

    /// <summary>
    /// Gets or sets an Article Time Zone offset to UTC.
    /// </summary>
    public int TimeZoneOffset
    {
      get
      {
        return this._timeZoneOffset;
      }

      set
      {
        this._timeZoneOffset = value;
      }
    }     

    /// <summary>
    /// Gets or sets Subject.
    /// </summary>
    public string Subject
    {
      get
      {
        return this._subject;
      }

      set
      {
        this._subject = value;
      }
    }

    /// <summary>
    /// Gets or sets Date.
    /// </summary>
    public DateTime Date
    {
      get
      {
        return this._date;
      }

      set
      {
        this._date = value;
      }
    }

    /// <summary>
    /// Gets or sets From.
    /// </summary>
    public string From
    {
      get
      {
        return this._from;
      }

      set
      {
        this._from = value;
      }
    }

    /// <summary>
    /// Gets or sets Sender.
    /// </summary>
    public string Sender
    {
      get
      {
        return this._sender;
      }

      set
      {
        this._sender = value;
      }
    }

    /// <summary>
    /// Gets or sets PostingHost.
    /// </summary>
    public string PostingHost
    {
      get
      {
        return this._postingHost;
      }

      set
      {
        this._postingHost = value;
      }
    }

    /// <summary>
    /// Gets or sets LineCount.
    /// </summary>
    public int LineCount
    {
      get
      {
        return this._lineCount;
      }

      set
      {
        this._lineCount = value;
      }
    }
  }
}