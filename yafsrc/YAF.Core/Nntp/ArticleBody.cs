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