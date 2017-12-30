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