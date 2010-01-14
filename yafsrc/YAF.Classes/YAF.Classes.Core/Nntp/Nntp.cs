/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using YAF.Classes.Data;

namespace YAF.Classes.Core.Nntp
{
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
      BuildNntpException(errorCode, null);
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
      BuildNntpException(errorCode, request);
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
      if (InnerException != null)
      {
        return "Nntp:NntpException: [Request: " + this._request + "][Response: " + this._errorCode.ToString() + " " + this._message + "]\n" +
               InnerException.ToString() + "\n" + StackTrace;
      }
      else
      {
        return "Nntp:NntpException: [Request: " + this._request + "][Response: " + this._errorCode.ToString() + " " + this._message + "]\n" + StackTrace;
      }
    }
  }

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
  }

  /// <summary>
  /// The attachment.
  /// </summary>
  public class Attachment
  {
    /// <summary>
    /// The binary data.
    /// </summary>
    private byte[] binaryData;

    /// <summary>
    /// The filename.
    /// </summary>
    private string filename;

    /// <summary>
    /// The id.
    /// </summary>
    private string id;

    /// <summary>
    /// Initializes a new instance of the <see cref="Attachment"/> class.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="filename">
    /// The filename.
    /// </param>
    /// <param name="binaryData">
    /// The binary data.
    /// </param>
    public Attachment(string id, string filename, byte[] binaryData)
    {
      this.id = id;
      this.filename = filename;
      this.binaryData = binaryData;
    }

    /// <summary>
    /// Gets Id.
    /// </summary>
    public string Id
    {
      get
      {
        return this.id;
      }
    }

    /// <summary>
    /// Gets Filename.
    /// </summary>
    public string Filename
    {
      get
      {
        return this.filename;
      }
    }

    /// <summary>
    /// Gets BinaryData.
    /// </summary>
    public byte[] BinaryData
    {
      get
      {
        return this.binaryData;
      }
    }

    /// <summary>
    /// The save as.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    public void SaveAs(string path)
    {
      SaveAs(path, false);
    }

    /// <summary>
    /// The save as.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <param name="isOverwrite">
    /// The is overwrite.
    /// </param>
    public void SaveAs(string path, bool isOverwrite)
    {
      FileStream fs = null;
      if (isOverwrite)
      {
        fs = new FileStream(path, FileMode.Create);
      }
      else
      {
        fs = new FileStream(path, FileMode.CreateNew);
      }

      fs.Write(this.binaryData, 0, this.binaryData.Length);
      fs.Close();
    }
  }

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

  /// <summary>
  /// The newsgroup.
  /// </summary>
  public class Newsgroup : IComparable
  {
    /// <summary>
    /// The group.
    /// </summary>
    protected string group;

    /// <summary>
    /// The high.
    /// </summary>
    protected int high;

    /// <summary>
    /// The low.
    /// </summary>
    protected int low;

    /// <summary>
    /// Initializes a new instance of the <see cref="Newsgroup"/> class.
    /// </summary>
    public Newsgroup()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Newsgroup"/> class.
    /// </summary>
    /// <param name="group">
    /// The group.
    /// </param>
    /// <param name="low">
    /// The low.
    /// </param>
    /// <param name="high">
    /// The high.
    /// </param>
    public Newsgroup(string group, int low, int high)
    {
      this.group = group;
      this.low = low;
      this.high = high;
    }

    /// <summary>
    /// Gets or sets Group.
    /// </summary>
    public string Group
    {
      get
      {
        return this.group;
      }

      set
      {
        this.group = value;
      }
    }

    /// <summary>
    /// Gets or sets Low.
    /// </summary>
    public int Low
    {
      get
      {
        return this.low;
      }

      set
      {
        this.low = value;
      }
    }

    /// <summary>
    /// Gets or sets High.
    /// </summary>
    public int High
    {
      get
      {
        return this.high;
      }

      set
      {
        this.high = value;
      }
    }

    #region IComparable Members

    /// <summary>
    /// The compare to.
    /// </summary>
    /// <param name="r">
    /// The r.
    /// </param>
    /// <returns>
    /// The compare to.
    /// </returns>
    public int CompareTo(object r)
    {
      return Group.CompareTo(((Newsgroup) r).Group);
    }

    #endregion
  }

  /// <summary>
  /// The nntp util.
  /// </summary>
  public class NntpUtil
  {
    /// <summary>
    /// The base 64 pem code.
    /// </summary>
    private static char[] base64PemCode = {
                                            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 
                                            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 
                                            'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/'
                                          };

    /// <summary>
    /// The base 64 pem convert code.
    /// </summary>
    private static byte[] base64PemConvertCode;

    /// <summary>
    /// The hex value.
    /// </summary>
    private static int[] hexValue;

    /// <summary>
    /// Initializes static members of the <see cref="NntpUtil"/> class.
    /// </summary>
    static NntpUtil()
    {
      hexValue = new int[128];
      for (int i = 0; i <= 9; i++)
      {
        hexValue[i + '0'] = i;
      }

      for (int i = 0; i < 6; i++)
      {
        hexValue[i + 'A'] = i + 10;
      }

      base64PemConvertCode = new byte[256];
      for (int i = 0; i < 255; i++)
      {
        base64PemConvertCode[i] = (byte) 255;
      }

      for (int i = 0; i < base64PemCode.Length; i++)
      {
        base64PemConvertCode[base64PemCode[i]] = (byte) i;
      }
    }

    /// <summary>
    /// The uu decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The uu decode.
    /// </returns>
    public static int UUDecode(string line, Stream outputStream)
    {
      return UUDecode(line.ToCharArray(), outputStream);
    }

    /// <summary>
    /// The uu decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The uu decode.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public static int UUDecode(char[] line, Stream outputStream)
    {
      if (line.Length < 1)
      {
        throw new InvalidOperationException("Invalid line: " + new string(line) + ".");
      }

      if (line[0] == '`')
      {
        return 0;
      }

      var line2 = new uint[line.Length];
      for (int ii = 0; ii < line.Length; ii++)
      {
        line2[ii] = (uint) line[ii] - 32 & 0x3f;
      }

      var length = (int) line2[0];
      if ((int) (length/3.0 + 0.999999999)*4 > line.Length - 1)
      {
        throw new InvalidOperationException("Invalid length(" + length + ") with line: " + new string(line) + ".");
      }

      int i = 1;
      int j = 0;
      while (length > j + 3)
      {
        outputStream.WriteByte((byte) ((line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3) & 0xff));
        outputStream.WriteByte((byte) ((line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf) & 0xff));
        outputStream.WriteByte((byte) ((line2[i + 2] << 6 & 0xc0 | line2[i + 3] & 0x3f) & 0xff));
        i += 4;
        j += 3;
      }

      if (length > j)
      {
        outputStream.WriteByte((byte) ((line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3) & 0xff));
      }

      if (length > j + 1)
      {
        outputStream.WriteByte((byte) ((line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf) & 0xff));
      }

      if (length > j + 2)
      {
        outputStream.WriteByte((byte) ((line2[i + 2] << 6 & 0xc0 | line2[i + 3] & 0x3f) & 0xff));
      }

      return length;
    }

    /// <summary>
    /// The base 64 decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The base 64 decode.
    /// </returns>
    public static int Base64Decode(string line, Stream outputStream)
    {
      return Base64Decode(line.ToCharArray(), outputStream);
    }

    /// <summary>
    /// The base 64 decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The base 64 decode.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    public static int Base64Decode(char[] line, Stream outputStream)
    {
      if (line.Length < 2)
      {
        throw new InvalidOperationException("Invalid line: " + new string(line) + ".");
      }

      var line2 = new uint[line.Length];
      for (int ii = 0; ii < line.Length && line[ii] != '='; ii++)
      {
        line2[ii] = (uint) base64PemConvertCode[line[ii] & 0xff];
      }

      int length;
      for (length = line2.Length - 1; line[length] == '=' && length >= 0; length--)
      {
        ;
      }

      length++;
      int i = 0;
      int j = 0;
      while (length - i >= 4)
      {
        outputStream.WriteByte((byte) (line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3));
        outputStream.WriteByte((byte) (line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf));
        outputStream.WriteByte((byte) (line2[i + 2] << 6 & 0xc0 | line2[i + 3] & 0x3f));
        i += 4;
        j += 3;
      }

      switch (length - i)
      {
        case 2:
          outputStream.WriteByte((byte) (line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3));
          return j + 1;
        case 3:
          outputStream.WriteByte((byte) (line2[i] << 2 & 0xfc | line2[i + 1] >> 4 & 0x3));
          outputStream.WriteByte((byte) (line2[i + 1] << 4 & 0xf0 | line2[i + 2] >> 2 & 0xf));
          return j + 2;
        default:
          return j;
      }
    }

    /// <summary>
    /// The quoted printable decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The quoted printable decode.
    /// </returns>
    public static int QuotedPrintableDecode(string line, Stream outputStream)
    {
      return QuotedPrintableDecode(line.ToCharArray(), outputStream);
    }

    /// <summary>
    /// The quoted printable decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <param name="outputStream">
    /// The output stream.
    /// </param>
    /// <returns>
    /// The quoted printable decode.
    /// </returns>
    public static int QuotedPrintableDecode(char[] line, Stream outputStream)
    {
      int length = line.Length;
      int i = 0, j = 0;
      while (i < length)
      {
        if (line[i] == '=')
        {
          if (i + 2 < length)
          {
            outputStream.WriteByte((byte) (hexValue[(int) line[i + 1]] << 4 | hexValue[(int) line[i + 2]]));
            i += 3;
          }
          else
          {
            i++;
          }
        }
        else
        {
          outputStream.WriteByte((byte) line[i]);
          i++;
        }

        j++;
      }

      if (line[length - 1] != '=')
      {
        outputStream.WriteByte((byte) '\n');
      }

      return j;
    }

    /// <summary>
    /// The dispatch mime content.
    /// </summary>
    /// <param name="sr">
    /// The sr.
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <param name="seperator">
    /// The seperator.
    /// </param>
    /// <returns>
    /// </returns>
    public static MIMEPart DispatchMIMEContent(StreamReader sr, MIMEPart part, string seperator)
    {
      string line = null;
      Match m = null;
      MemoryStream ms;
      byte[] bytes;
      switch (part.ContentType.Substring(0, part.ContentType.IndexOf('/')).ToUpper())
      {
        case "MULTIPART":
          MIMEPart newPart = null;
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            m = Regex.Match(line, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
            if (!m.Success)
            {
              continue;
            }

            newPart = new MIMEPart();
            newPart.ContentType = m.Groups[1].ToString();
            newPart.Charset = "US-ASCII";
            newPart.ContentTransferEncoding = "7BIT";
            while (line != string.Empty)
            {
              m = Regex.Match(line, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Boundary = m.Groups[1].ToString();
                newPart.EmbeddedPartList = new ArrayList();
              }

              m = Regex.Match(line, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Charset = m.Groups[1].ToString();
              }

              m = Regex.Match(line, @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.ContentTransferEncoding = m.Groups[1].ToString();
              }

              m = Regex.Match(line, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                newPart.Filename = Base64HeaderDecode(m.Groups[1].ToString());
                newPart.Filename = newPart.Filename.Substring(
                  newPart.Filename.LastIndexOfAny(
                    new[]
                      {
                        '\\', '/'
                      }) + 1);
              }

              line = sr.ReadLine();
            }

            part.EmbeddedPartList.Add(DispatchMIMEContent(sr, newPart, "--" + part.Boundary));
          }

          break;
        case "TEXT":
          ms = new MemoryStream();
          bytes = null;
          long pos;
          var msr = new StreamReader(ms, Encoding.GetEncoding(part.Charset));
          var sb = new StringBuilder();
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            pos = ms.Position;
            if (line != string.Empty)
            {
              switch (part.ContentTransferEncoding.ToUpper())
              {
                case "QUOTED-PRINTABLE":
                  QuotedPrintableDecode(line, ms);
                  break;
                case "BASE64":
                  if (line != null && line != string.Empty)
                  {
                    Base64Decode(line, ms);
                  }

                  break;
                case "UU":
                  if (line != null && line != string.Empty)
                  {
                    UUDecode(line, ms);
                  }

                  break;
                case "7BIT":
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  ms.WriteByte((byte) '\n');
                  break;
                default:
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  ms.WriteByte((byte) '\n');
                  break;
              }
            }

            ms.Position = pos;
            if (part.ContentType.ToUpper() == "TEXT/HTML")
            {
              sb.Append(msr.ReadToEnd());
            }
            else
            {
              sb.Append(HttpUtility.HtmlEncode(msr.ReadToEnd()).Replace("\n", "<br>\n"));
            }
          }

          part.Text = sb.ToString();
          break;
        default:
          ms = new MemoryStream();
          bytes = null;
          while ((line = sr.ReadLine()) != null && line != seperator && line != seperator + "--")
          {
            if (line != string.Empty)
            {
              switch (part.ContentTransferEncoding.ToUpper())
              {
                case "QUOTED-PRINTABLE":
                  QuotedPrintableDecode(line, ms);
                  break;
                case "BASE64":
                  if (line != null && line != string.Empty)
                  {
                    Base64Decode(line, ms);
                  }

                  break;
                case "UU":
                  if (line != null && line != string.Empty)
                  {
                    UUDecode(line, ms);
                  }

                  break;
                default:
                  bytes = Encoding.ASCII.GetBytes(line);
                  ms.Write(bytes, 0, bytes.Length);
                  break;
              }
            }
          }

          ms.Seek(0, SeekOrigin.Begin);
          part.BinaryData = new byte[ms.Length];
          ms.Read(part.BinaryData, 0, (int) ms.Length);
          break;
      }

      return part;
    }

    /// <summary>
    /// The base 64 header decode.
    /// </summary>
    /// <param name="line">
    /// The line.
    /// </param>
    /// <returns>
    /// The base 64 header decode.
    /// </returns>
    public static string Base64HeaderDecode(string line)
    {
      MemoryStream ms = null;
      byte[] bytes = null;
      string oStr = null;
      string code = null;
      string content = null;
      Match m = Regex.Match(line, @"=\?([^?]+)\?[^?]+\?([^?]+)\?=");
      while (m.Success)
      {
        ms = new MemoryStream();
        oStr = m.Groups[0].ToString();
        code = m.Groups[1].ToString();
        content = m.Groups[2].ToString();
        Base64Decode(content, ms);
        ms.Seek(0, SeekOrigin.Begin);
        bytes = new byte[ms.Length];
        ms.Read(bytes, 0, bytes.Length);
        line = line.Replace(oStr, Encoding.GetEncoding(code).GetString(bytes));
        m = m.NextMatch();
      }

      return line;
    }

    /// <summary>
    /// The convert list to tree.
    /// </summary>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <returns>
    /// </returns>
    public static ArrayList ConvertListToTree(ArrayList list)
    {
      var hash = new Hashtable(list.Count);
      var treeList = new ArrayList();
      int len;
      bool isTop;
      foreach (Article article in list)
      {
        isTop = true;
        hash[article.MessageId] = article;
        article.LastReply = article.Header.Date;
        article.Children = new ArrayList();
        len = article.Header.ReferenceIds.Length;
        for (int i = 0; i < len; i++)
        {
          if (hash.ContainsKey(article.Header.ReferenceIds[i]))
          {
            ((Article) hash[article.Header.ReferenceIds[i]]).LastReply = article.LastReply;
            break;
          }
        }

        for (int i = len - 1; i >= 0; i--)
        {
          if (hash.ContainsKey(article.Header.ReferenceIds[i]))
          {
            isTop = false;
            ((Article) hash[article.Header.ReferenceIds[i]]).Children.Add(article);
            break;
          }
        }

        if (isTop)
        {
          treeList.Add(article);
        }
      }

      return treeList;
    }
  }

  /// <summary>
  /// The on request delegate.
  /// </summary>
  /// <param name="msg">
  /// The msg.
  /// </param>
  public delegate void OnRequestDelegate(string msg);

  /// <summary>
  /// The nntp connection.
  /// </summary>
  public class NntpConnection
  {
    #region Private variables

    /// <summary>
    /// The connected group.
    /// </summary>
    private Newsgroup connectedGroup;

    /// <summary>
    /// The connected server.
    /// </summary>
    private string connectedServer;

    /// <summary>
    /// The password.
    /// </summary>
    private string password = null;

    /// <summary>
    /// The port.
    /// </summary>
    private int port;

    /// <summary>
    /// The sr.
    /// </summary>
    private StreamReader sr;

    /// <summary>
    /// The sw.
    /// </summary>
    private StreamWriter sw;

    /// <summary>
    /// The tcp client.
    /// </summary>
    private TcpClient tcpClient = null;

    /// <summary>
    /// The timeout.
    /// </summary>
    private int timeout;

    /// <summary>
    /// The username.
    /// </summary>
    private string username = null;

    /// <summary>
    /// The on request.
    /// </summary>
    private event OnRequestDelegate onRequest = null;

    #endregion

    #region Public accessors

    /// <summary>
    /// Gets or sets Timeout.
    /// </summary>
    public int Timeout
    {
      get
      {
        return this.timeout;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set
      {
        this.timeout = value;
        this.tcpClient.SendTimeout = this.timeout;
        this.tcpClient.ReceiveTimeout = this.timeout;
      }
    }

    /// <summary>
    /// Gets ConnectedServer.
    /// </summary>
    public string ConnectedServer
    {
      get
      {
        return this.connectedServer;
      }
    }

    /// <summary>
    /// Gets ConnectedGroup.
    /// </summary>
    public Newsgroup ConnectedGroup
    {
      get
      {
        return this.connectedGroup;
      }
    }

    /// <summary>
    /// Gets Port.
    /// </summary>
    public int Port
    {
      get
      {
        return this.port;
      }
    }

    /// <summary>
    /// The on request.
    /// </summary>
    public event OnRequestDelegate OnRequest
    {
      [MethodImpl(MethodImplOptions.Synchronized)]
      add
      {
        onRequest += value;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      remove
      {
        onRequest -= value;
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// The reset.
    /// </summary>
    private void Reset()
    {
      this.connectedServer = null;
      this.connectedGroup = null;
      this.username = null;
      this.password = null;
      if (this.tcpClient != null)
      {
        try
        {
          this.sw.Close();
          this.sr.Close();
          this.tcpClient.Close();
        }
        catch
        {
        }
      }

      this.tcpClient = new TcpClient();
      this.tcpClient.SendTimeout = this.timeout;
      this.tcpClient.ReceiveTimeout = this.timeout;
    }

    /// <summary>
    /// The make request.
    /// </summary>
    /// <param name="request">
    /// The request.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    private Response MakeRequest(string request)
    {
      if (request != null)
      {
        this.sw.WriteLine(request);
        if (onRequest != null)
        {
          onRequest("SEND: " + request);
        }
      }

      string line = null;
      int code = 0;
      line = this.sr.ReadLine();
      if (onRequest != null && line != null)
      {
        onRequest("RECEIVE: " + line);
      }

      try
      {
        code = int.Parse(line.Substring(0, 3));
      }
      catch (NullReferenceException)
      {
        Reset();
        throw new NntpException(line, request);
      }
      catch (ArgumentOutOfRangeException)
      {
        Reset();
        throw new NntpException(line, request);
      }
      catch (ArgumentNullException)
      {
        Reset();
        throw new NntpException(line, request);
      }
      catch (FormatException)
      {
        Reset();
        throw new NntpException(line, request);
      }

      if (code == 480)
      {
        if (SendIdentity())
        {
          return MakeRequest(request);
        }
      }

      return new Response(code, line.Length >= 5 ? line.Substring(4) : null, request);
    }

    /// <summary>
    /// The get header.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <returns>
    /// </returns>
    private ArticleHeader GetHeader(string messageId, out MIMEPart part)
    {
      string response = null;
      var header = new ArticleHeader();
      string name = null;
      string value = null;
      header.ReferenceIds = new string[0];
      string[] values = null;
      string[] values2 = null;
      Match m = null;
      part = null;
      int i = -1;
      while ((response = this.sr.ReadLine()) != null && response != string.Empty)
      {
        m = Regex.Match(response, @"^\s+(\S+)$");
        if (m.Success)
        {
          value = m.Groups[1].ToString();
        }
        else
        {
          i = response.IndexOf(':');
          if (i == -1)
          {
            continue;
          }

          name = response.Substring(0, i).ToUpper();
          value = response.Substring(i + 1);
        }

        switch (name)
        {
          case "REFERENCES":
            values = value.Split(' ');
            values2 = header.ReferenceIds;
            header.ReferenceIds = new string[values.Length + values2.Length];
            values.CopyTo(header.ReferenceIds, 0);
            values2.CopyTo(header.ReferenceIds, values.Length);
            break;
          case "SUBJECT":
            header.Subject += NntpUtil.Base64HeaderDecode(value);
            break;
          case "DATE":
            i = value.IndexOf(',');
            header.Date = DateTime.Parse(value.Substring(i + 1, value.Length - 7 - i));
            break;
          case "FROM":
            header.From += NntpUtil.Base64HeaderDecode(value);
            break;
          case "NNTP-POSTING-HOST":
            header.PostingHost += value;
            break;
          case "LINES":
            header.LineCount = int.Parse(value);
            break;
          case "MIME-VERSION":
            part = new MIMEPart();
            part.ContentType = "TEXT/PLAIN";
            part.Charset = "US-ASCII";
            part.ContentTransferEncoding = "7BIT";
            part.Filename = null;
            part.Boundary = null;
            break;
          case "CONTENT-TYPE":
            if (part != null)
            {
              m = Regex.Match(response, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                part.ContentType = m.Groups[1].ToString();
              }

              m = Regex.Match(response, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                part.Boundary = m.Groups[1].ToString();
                part.EmbeddedPartList = new ArrayList();
              }

              m = Regex.Match(response, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                part.Charset = m.Groups[1].ToString();
              }

              m = Regex.Match(response, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                part.Filename = m.Groups[1].ToString();
              }
            }

            break;
          case "CONTENT-TRANSFER-ENCODING":
            if (part != null)
            {
              m = Regex.Match(response, @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)", RegexOptions.IgnoreCase);
              if (m.Success)
              {
                part.ContentTransferEncoding = m.Groups[1].ToString();
              }
            }

            break;
        }
      }

      return header;
    }

    /// <summary>
    /// The get normal body.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    private ArticleBody GetNormalBody(string messageId)
    {
      var buff = new char[1];
      string response = null;
      var list = new ArrayList();
      var sb = new StringBuilder();
      Attachment attach = null;
      MemoryStream ms = null;
      this.sr.Read(buff, 0, 1);
      int i = 0;
      byte[] bytes = null;
      Match m = null;
      while ((response = this.sr.ReadLine()) != null)
      {
        if (buff[0] == '.')
        {
          if (response == string.Empty)
          {
            break;
          }
          else
          {
            sb.Append(response);
          }
        }
        else
        {
          if ((buff[0] == 'B' || buff[0] == 'b') && (m = Regex.Match(response, @"^EGIN \d\d\d (.+)$", RegexOptions.IgnoreCase)).Success)
          {
            ms = new MemoryStream();
            while ((response = this.sr.ReadLine()) != null && (response.Length != 3 || response.ToUpper() != "END"))
            {
              NntpUtil.UUDecode(response, ms);
            }

            ms.Seek(0, SeekOrigin.Begin);
            bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int) ms.Length);
            attach = new Attachment(messageId + " - " + m.Groups[1].ToString(), m.Groups[1].ToString(), bytes);
            list.Add(attach);
            ms.Close();
            i++;
          }
          else
          {
            sb.Append(buff[0]);
            sb.Append(response);
          }
        }

        sb.Append('\n');
        this.sr.Read(buff, 0, 1);
      }

      var ab = new ArticleBody();
      ab.IsHtml = false;
      ab.Text = sb.ToString();
      ab.Attachments = (Attachment[]) list.ToArray(typeof (Attachment));
      return ab;
    }

    /// <summary>
    /// The get mime body.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <returns>
    /// </returns>
    private ArticleBody GetMIMEBody(string messageId, MIMEPart part)
    {
      string line = null;
      ArticleBody body = null;
      StringBuilder sb = null;
      var attachmentList = new ArrayList();
      try
      {
        NntpUtil.DispatchMIMEContent(this.sr, part, ".");
        sb = new StringBuilder();
        attachmentList = new ArrayList();
        body = new ArticleBody();
        body.IsHtml = true;
        ConvertMIMEContent(messageId, part, sb, attachmentList);
        body.Text = sb.ToString();
        body.Attachments = (Attachment[]) attachmentList.ToArray(typeof (Attachment));
      }
      finally
      {
        if (((NetworkStream) this.sr.BaseStream).DataAvailable)
        {
          while ((line = this.sr.ReadLine()) != null && line != ".")
          {
            ;
          }
        }
      }

      return body;
    }

    /// <summary>
    /// The convert mime content.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <param name="part">
    /// The part.
    /// </param>
    /// <param name="sb">
    /// The sb.
    /// </param>
    /// <param name="attachmentList">
    /// The attachment list.
    /// </param>
    private void ConvertMIMEContent(string messageId, MIMEPart part, StringBuilder sb, ArrayList attachmentList)
    {
      Match m = null;
      m = Regex.Match(part.ContentType, @"MULTIPART", RegexOptions.IgnoreCase);
      if (m.Success)
      {
        foreach (MIMEPart subPart in part.EmbeddedPartList)
        {
          ConvertMIMEContent(messageId, subPart, sb, attachmentList);
        }

        return;
      }

      m = Regex.Match(part.ContentType, @"TEXT", RegexOptions.IgnoreCase);
      if (m.Success)
      {
        sb.Append(part.Text);
        sb.Append("<hr>");
        return;
      }

      var attachment = new Attachment(messageId + " - " + part.Filename, part.Filename, part.BinaryData);
      attachmentList.Add(attachment);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Initializes a new instance of the <see cref="NntpConnection"/> class.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public NntpConnection()
    {
      this.timeout = 5000;
      Reset();
    }

    /// <summary>
    /// The connect server.
    /// </summary>
    /// <param name="server">
    /// The server.
    /// </param>
    /// <param name="port">
    /// The port.
    /// </param>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ConnectServer(string server, int port)
    {
      if (this.connectedServer != null && this.connectedServer != server)
      {
        Disconnect();
      }

      if (this.connectedServer != server)
      {
        this.tcpClient.Connect(server, port);
        NetworkStream stream = this.tcpClient.GetStream();
        if (stream == null)
        {
          throw new NntpException("Fail to setup connection.");
        }

        this.sr = new StreamReader(stream, Encoding.Default);
        this.sw = new StreamWriter(stream, Encoding.ASCII);
        this.sw.AutoFlush = true;
        Response res = MakeRequest(null);
        if (res.Code != 200 && res.Code != 201)
        {
          Reset();
          throw new NntpException(res.Code);
        }

        this.connectedServer = server;
        this.port = port;
      }
    }

    /// <summary>
    /// The provide identity.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ProvideIdentity(string username, string password)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      this.username = username;
      this.password = password;
    }

    /// <summary>
    /// The send identity.
    /// </summary>
    /// <returns>
    /// The send identity.
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool SendIdentity()
    {
      if (this.username == null)
      {
        return false;
      }

      Response res = MakeRequest("AUTHINFO USER " + this.username);
      if (res.Code == 381)
      {
        res = MakeRequest("AUTHINFO PASS " + this.password);
      }

      if (res.Code != 281)
      {
        Reset();
        throw new NntpException(res.Code, "AUTHINFO PASS ******");
      }

      return true;
    }

    /// <summary>
    /// The connect group.
    /// </summary>
    /// <param name="group">
    /// The group.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Newsgroup ConnectGroup(string group)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null || this.connectedGroup.Group != group)
      {
        Response res = MakeRequest("GROUP " + group);
        if (res.Code != 211)
        {
          this.connectedGroup = null;
          throw new NntpException(res.Code, res.Request);
        }

        string[] values = res.Message.Split(' ');
        this.connectedGroup = new Newsgroup(group, int.Parse(values[1]), int.Parse(values[2]));
      }

      return this.connectedGroup;
    }

    /// <summary>
    /// The get group list.
    /// </summary>
    /// <returns>
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ArrayList GetGroupList()
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      Response res = MakeRequest("LIST");
      if (res.Code != 215)
      {
        throw new NntpException(res.Code, res.Request);
      }

      var list = new ArrayList();
      string response = null;
      string[] values;
      while ((response = this.sr.ReadLine()) != null && response != ".")
      {
        values = response.Split(' ');
        list.Add(new Newsgroup(values[0], int.Parse(values[2]), int.Parse(values[1])));
      }

      return list;
    }

    /// <summary>
    /// The get message id.
    /// </summary>
    /// <param name="articleId">
    /// The article id.
    /// </param>
    /// <returns>
    /// The get message id.
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public string GetMessageId(int articleId)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null)
      {
        throw new NntpException("No connecting newsgroup.");
      }

      Response res = MakeRequest("STAT " + articleId);
      if (res.Code != 223)
      {
        throw new NntpException(res.Code, res.Request);
      }

      int i = res.Message.IndexOf('<');
      int j = res.Message.IndexOf('>');
      return res.Message.Substring(i, j - i + 1);
    }

    /// <summary>
    /// The get article id.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The get article id.
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public int GetArticleId(string messageId)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null)
      {
        throw new NntpException("No connecting newsgroup.");
      }

      Response res = MakeRequest("STAT " + messageId);
      if (res.Code != 223)
      {
        throw new NntpException(res.Code, res.Request);
      }

      int i = res.Message.IndexOf(' ');
      return int.Parse(res.Message.Substring(0, i));
    }

    /// <summary>
    /// The get article list.
    /// </summary>
    /// <param name="low">
    /// The low.
    /// </param>
    /// <param name="high">
    /// The high.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    /// <exception cref="Exception">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ArrayList GetArticleList(int low, int high)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null)
      {
        throw new NntpException("No connecting newsgroup.");
      }

      Response res = MakeRequest("XOVER " + low + "-" + high);
      if (res.Code != 224)
      {
        throw new NntpException(res.Code, res.Request);
      }

      var list = new ArrayList();
      Article article = null;
      string[] values = null;
      int i;
      string response = null;
      while ((response = this.sr.ReadLine()) != null && response != ".")
      {
        try
        {
          article = new Article();
          article.Header = new ArticleHeader();
          values = response.Split('\t');
          article.ArticleId = int.Parse(values[0]);
          article.Header.Subject = NntpUtil.Base64HeaderDecode(values[1]);
          article.Header.From = NntpUtil.Base64HeaderDecode(values[2]);
          i = values[3].IndexOf(',');
          article.Header.Date = DateTime.Parse(values[3].Substring(i + 1, values[3].Length - 7 - i));
          article.MessageId = values[4];
          if (values[5].Trim().Length == 0)
          {
            article.Header.ReferenceIds = new string[0];
          }
          else
          {
            article.Header.ReferenceIds = values[5].Split(' ');
          }

          if (values.Length < 8 || values[7] == null || values[7].Trim() == string.Empty)
          {
            article.Header.LineCount = 0;
          }
          else
          {
            article.Header.LineCount = int.Parse(values[7]);
          }

          article.Body = null;
        }
        catch (Exception e)
        {
          throw new Exception(response, e);
        }

        list.Add(article);
      }

      return list;
    }

    /// <summary>
    /// The get article.
    /// </summary>
    /// <param name="articleId">
    /// The article id.
    /// </param>
    /// <returns>
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Article GetArticle(int articleId)
    {
      return GetArticle(articleId.ToString());
    }

    /// <summary>
    /// The get article.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Article GetArticle(string messageId)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null)
      {
        throw new NntpException("No connecting newsgroup.");
      }

      var article = new Article();
      Response res = MakeRequest("Article " + messageId);
      if (res.Code != 220)
      {
        throw new NntpException(res.Code);
      }

      int i = res.Message.IndexOf(' ');
      article.ArticleId = int.Parse(res.Message.Substring(0, i));
      int end = res.Message.IndexOf(' ', i + 1);
      if (end == -1)
      {
        end = res.Message.Length - (i + 1);
      }

      article.MessageId = res.Message.Substring(i + 1, end);
      MIMEPart part = null;
      article.Header = GetHeader(messageId, out part);
      if (part == null)
      {
        article.Body = GetNormalBody(messageId);
      }
      else
      {
        article.Body = GetMIMEBody(messageId, part);
      }

      return article;
    }

    /// <summary>
    /// The post article.
    /// </summary>
    /// <param name="article">
    /// The article.
    /// </param>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void PostArticle(Article article)
    {
      if (this.connectedServer == null)
      {
        throw new NntpException("No connecting newsserver.");
      }

      if (this.connectedGroup == null)
      {
        throw new NntpException("No connecting newsgroup.");
      }

      Response res = MakeRequest("POST");
      if (res.Code != 340)
      {
        throw new NntpException(res.Code, res.Request);
      }

      var sb = new StringBuilder();
      sb.Append("From: ");
      sb.Append(article.Header.From);
      sb.Append("\r\nNewsgroup: ");
      sb.Append(this.connectedGroup);
      if (article.Header.ReferenceIds != null && article.Header.ReferenceIds.Length != 0)
      {
        sb.Append("\r\nReference: ");
        sb.Append(string.Join(" ", article.Header.ReferenceIds));
      }

      sb.Append("\r\nSubject: ");
      sb.Append(article.Header.Subject);
      sb.Append("\r\n\r\n");
      sb.Append(article.Body.Text.Replace("\n.", "\n.."));
      sb.Append("\r\n.\r\n");
      res = MakeRequest(sb.ToString());
      if (res.Code != 240)
      {
        throw new NntpException(res.Code, res.Request);
      }
    }

    /// <summary>
    /// The disconnect.
    /// </summary>
    /// <exception cref="NntpException">
    /// </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Disconnect()
    {
      if (this.connectedServer != null)
      {
        string response = null;
        if (((NetworkStream) this.sr.BaseStream).DataAvailable)
        {
          while ((response = this.sr.ReadLine()) != null && response != ".")
          {
            ;
          }
        }

        Response res = MakeRequest("QUIT");
        if (res.Code != 205)
        {
          throw new NntpException(res.Code, res.Request);
        }
      }

      Reset();
    }

    #endregion

    #region Nested type: Response

    /// <summary>
    /// The response.
    /// </summary>
    private class Response
    {
      /// <summary>
      /// The code.
      /// </summary>
      private int code;

      /// <summary>
      /// The message.
      /// </summary>
      private string message;

      /// <summary>
      /// The request.
      /// </summary>
      private string request;

      /// <summary>
      /// Initializes a new instance of the <see cref="Response"/> class.
      /// </summary>
      /// <param name="code">
      /// The code.
      /// </param>
      /// <param name="message">
      /// The message.
      /// </param>
      /// <param name="request">
      /// The request.
      /// </param>
      public Response(int code, string message, string request)
      {
        this.code = code;
        this.message = message;
        this.request = request;
      }

      /// <summary>
      /// Gets or sets Code.
      /// </summary>
      public int Code
      {
        get
        {
          return this.code;
        }

        set
        {
          this.code = value;
        }
      }

      /// <summary>
      /// Gets or sets Message.
      /// </summary>
      public string Message
      {
        get
        {
          return this.message;
        }

        set
        {
          this.message = value;
        }
      }

      /// <summary>
      /// Gets or sets Request.
      /// </summary>
      public string Request
      {
        get
        {
          return this.request;
        }

        set
        {
          this.request = value;
        }
      }
    }

    #endregion
  }

  /// <summary>
  /// The yaf nntp.
  /// </summary>
  public static class YafNntp
  {
    /// <summary>
    /// The read articles.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="nLastUpdate">
    /// The n last update.
    /// </param>
    /// <param name="nTimeToRun">
    /// The n time to run.
    /// </param>
    /// <param name="bCreateUsers">
    /// The b create users.
    /// </param>
    /// <returns>
    /// The read articles.
    /// </returns>
    public static int ReadArticles(object boardID, int nLastUpdate, int nTimeToRun, bool bCreateUsers)
    {
      int guestUserId = DB.user_guest(boardID); // Use guests user-id
      string sHostAddress = HttpContext.Current.Request.UserHostAddress;
      TimeSpan tsLocal = YafContext.Current.BoardSettings.TimeZone;
      DateTime dtStart = DateTime.Now;
      int nArticleCount = 0;

      string nntpHostName = string.Empty;
      int nntpPort = 119;

      var nntpConnection = new NntpConnection();

      try
      {
        // Only those not updated in the last 30 minutes
        using (DataTable dtForums = DB.nntpforum_list(boardID, nLastUpdate, null, true))
        {
          foreach (DataRow drForum in dtForums.Rows)
          {
            if (nntpHostName != drForum["Address"].ToString().ToLower() || nntpPort != (int) drForum["Port"])
            {
              if (nntpConnection != null)
              {
                nntpConnection.Disconnect();
              }

              nntpHostName = drForum["Address"].ToString().ToLower();
              nntpPort = Convert.ToInt32(drForum["Port"]);

              // call connect server
              nntpConnection.ConnectServer(nntpHostName, nntpPort);

              // provide authentication if required...
              if (drForum["UserName"] != DBNull.Value && drForum["UserPass"] != DBNull.Value)
              {
                nntpConnection.ProvideIdentity(drForum["UserName"].ToString(), drForum["UserPass"].ToString());
                nntpConnection.SendIdentity();
              }
            }

            Newsgroup group = nntpConnection.ConnectGroup(drForum["GroupName"].ToString());

            var nLastMessageNo = (int) drForum["LastMessageNo"];
            int nCurrentMessage = nLastMessageNo;

            // If this is first retrieve for this group, only fetch last 50
            if (nCurrentMessage == 0)
            {
              nCurrentMessage = group.High - 50;
            }

            nCurrentMessage++;

            var nForumID = (int) drForum["ForumID"];

            for (; nCurrentMessage <= group.High; nCurrentMessage++)
            {
              try
              {
                Article article = nntpConnection.GetArticle(nCurrentMessage);

                string sBody = article.Body.Text;
                string sSubject = article.Header.Subject;
                string sFrom = article.Header.From;
                string sThread = article.ArticleId.ToString();
                DateTime dtDate = article.Header.Date - tsLocal;

                if (dtDate.Year < 1950 || dtDate > DateTime.Now)
                {
                  dtDate = DateTime.Now;
                }

                sBody = String.Format("Date: {0}\r\n\r\n", article.Header.Date) + sBody;
                sBody = String.Format("Date parsed: {0}\r\n", dtDate) + sBody;

                if (bCreateUsers)
                {
                  guestUserId = DB.user_nntp(boardID, sFrom, string.Empty);
                }

                sBody = HttpContext.Current.Server.HtmlEncode(sBody);
                DB.nntptopic_savemessage(drForum["NntpForumID"], sSubject, sBody, guestUserId, sFrom, sHostAddress, dtDate, sThread);
                nLastMessageNo = nCurrentMessage;
                nArticleCount++;

                // We don't wanna retrieve articles forever...
                // Total time x seconds for all groups
                if ((DateTime.Now - dtStart).TotalSeconds > nTimeToRun)
                {
                  break;
                }
              }
              catch (NntpException)
              {
              }
            }

            DB.nntpforum_update(drForum["NntpForumID"], nLastMessageNo, guestUserId);

            // Total time x seconds for all groups
            if ((DateTime.Now - dtStart).TotalSeconds > nTimeToRun)
            {
              break;
            }
          }
        }
      }
      finally
      {
        if (nntpConnection != null)
        {
          nntpConnection.Disconnect();
        }
      }

      return nArticleCount;
    }
  }
}