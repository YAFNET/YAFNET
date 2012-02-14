/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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