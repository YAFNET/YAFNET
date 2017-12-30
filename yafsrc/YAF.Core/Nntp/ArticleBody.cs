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
  /// <summary>
  /// The article body.
  /// </summary>
  public class ArticleBody
  {
    /// <summary>
    /// The _attachments.
    /// </summary>
    private Attachment[] _attachments;

    /// <summary>
    /// The _is html.
    /// </summary>
    private bool _isHtml;

    /// <summary>
    /// The _text.
    /// </summary>
    private string _text;

    /// <summary>
    /// Gets or sets a value indicating whether IsHtml.
    /// </summary>
    public bool IsHtml
    {
      get
      {
        return this._isHtml;
      }

      set
      {
        this._isHtml = value;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public string Text
    {
      get
      {
        return this._text;
      }

      set
      {
        this._text = value;
      }
    }

    /// <summary>
    /// Gets or sets Attachments.
    /// </summary>
    public Attachment[] Attachments
    {
      get
      {
        return this._attachments;
      }

      set
      {
        this._attachments = value;
      }
    }
  }
}