/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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