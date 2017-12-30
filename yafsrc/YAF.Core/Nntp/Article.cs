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
  using System.Collections;

  /// <summary>
  /// The article.
  /// </summary>
  public class Article
  {
    /// <summary>
    /// The article id.
    /// </summary>
    private int articleId;

    /// <summary>
    /// The body.
    /// </summary>
    private ArticleBody body;

    /// <summary>
    /// The children.
    /// </summary>
    private ArrayList children;

    /// <summary>
    /// The header.
    /// </summary>
    private ArticleHeader header;

    /// <summary>
    /// The last reply.
    /// </summary>
    private DateTime lastReply;

    /// <summary>
    /// The message id.
    /// </summary>
    private string messageId;

    /// <summary>
    /// Gets or sets MessageId.
    /// </summary>
    public string MessageId
    {
      get
      {
        return this.messageId;
      }

      set
      {
        this.messageId = value;
      }
    }

    /// <summary>
    /// Gets or sets ArticleId.
    /// </summary>
    public int ArticleId
    {
      get
      {
        return this.articleId;
      }

      set
      {
        this.articleId = value;
      }
    }

    /// <summary>
    /// Gets or sets Header.
    /// </summary>
    public ArticleHeader Header
    {
      get
      {
        return this.header;
      }

      set
      {
        this.header = value;
      }
    }

    /// <summary>
    /// Gets or sets Body.
    /// </summary>
    public ArticleBody Body
    {
      get
      {
        return this.body;
      }

      set
      {
        this.body = value;
      }
    }

    /// <summary>
    /// Gets or sets LastReply.
    /// </summary>
    public DateTime LastReply
    {
      get
      {
        return this.lastReply;
      }

      set
      {
        this.lastReply = value;
      }
    }

    /// <summary>
    /// Gets or sets Children.
    /// </summary>
    public ArrayList Children
    {
      get
      {
        return this.children;
      }

      set
      {
        this.children = value;
      }
    }

    public MIMEPart MimePart { get; set; }
  }
}