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
  using System.Collections;

  /// <summary>
  /// The mime part.
  /// </summary>
  public class MIMEPart
  {
    /// <summary>
    /// The binary data.
    /// </summary>
    private byte[] binaryData;

    /// <summary>
    /// The boundary.
    /// </summary>
    private string boundary;

    /// <summary>
    /// The charset.
    /// </summary>
    private string charset;

    /// <summary>
    /// The content transfer encoding.
    /// </summary>
    private string contentTransferEncoding;

    /// <summary>
    /// The content type.
    /// </summary>
    private string contentType;

    /// <summary>
    /// The embedded part list.
    /// </summary>
    private ArrayList embeddedPartList;

    /// <summary>
    /// The filename.
    /// </summary>
    private string filename;

    /// <summary>
    /// The text.
    /// </summary>
    private string text;

    /// <summary>
    /// Gets or sets BinaryData.
    /// </summary>
    public byte[] BinaryData
    {
      get
      {
        return this.binaryData;
      }

      set
      {
        this.binaryData = value;
      }
    }

    /// <summary>
    /// Gets or sets Boundary.
    /// </summary>
    public string Boundary
    {
      get
      {
        return this.boundary;
      }

      set
      {
        this.boundary = value;
      }
    }

    /// <summary>
    /// Gets or sets ContentType.
    /// </summary>
    public string ContentType
    {
      get
      {
        return this.contentType;
      }

      set
      {
        this.contentType = value;
      }
    }

    /// <summary>
    /// Gets or sets ContentTransferEncoding.
    /// </summary>
    public string ContentTransferEncoding
    {
      get
      {
        return this.contentTransferEncoding;
      }

      set
      {
        this.contentTransferEncoding = value;
      }
    }

    /// <summary>
    /// Gets or sets Charset.
    /// </summary>
    public string Charset
    {
      get
      {
        return this.charset;
      }

      set
      {
        this.charset = value;
      }
    }

    /// <summary>
    /// Gets or sets Filename.
    /// </summary>
    public string Filename
    {
      get
      {
        return this.filename;
      }

      set
      {
        this.filename = value;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public string Text
    {
      get
      {
        return this.text;
      }

      set
      {
        this.text = value;
      }
    }

    /// <summary>
    /// Gets or sets EmbeddedPartList.
    /// </summary>
    public ArrayList EmbeddedPartList
    {
      get
      {
        return this.embeddedPartList;
      }

      set
      {
        this.embeddedPartList = value;
      }
    }
  }
}