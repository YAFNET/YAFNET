/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.IO;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace YAF.Classes.Utils.Nntp
{
	public class NntpException : Exception
	{
		private int errorCode;
		private string request;
		private string message;
		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}
		public string Request
		{
			get
			{
				return this.request;
			}
		}
		public override string Message
		{
			get
			{
				return this.message;
			}
		}
		private void BuildNntpException( int errorCode, string request )
		{
			this.errorCode = errorCode;
			this.request = request;
			switch ( errorCode )
			{
				case 281:
					this.message = "Authentication accepted.";
					break;
				case 288:
					this.message = "Binary data to follow.";
					break;
				case 381:
					this.message = "More authentication information required.";
					break;
				case 400:
					this.message = "Service disconnected.";
					break;
				case 411:
					this.message = "No such newsgroup.";
					break;
				case 412:
					this.message = "No newsgroup current selected.";
					break;
				case 420:
					this.message = "No current article has been selected.";
					break;
				case 423:
					this.message = "No such article number in this group.";
					break;
				case 430:
					this.message = "No such article found.";
					break;
				case 436:
					this.message = "Transfer failed - try again later.";
					break;
				case 440:
					this.message = "Posting not allowed.";
					break;
				case 441:
					this.message = "Posting failed.";
					break;
				case 480:
					this.message = "Authentication required.";
					break;
				case 481:
					this.message = "More authentication information required.";
					break;
				case 482:
					this.message = "Authentication rejected.";
					break;
				case 500:
					this.message = "Command not understood.";
					break;
				case 501:
					this.message = "Command syntax error.";
					break;
				case 502:
					this.message = "No permission.";
					break;
				case 503:
					this.message = "Program error, function not performed.";
					break;
				default:
					this.message = "Unknown error.";
					break;
			}
		}
		public NntpException( String message )
			: base( message )
		{
			this.message = message;
			this.errorCode = 999;
			this.request = null;
		}
		public NntpException( int errorCode )
			: base()
		{
			this.BuildNntpException( errorCode, null );
		}
		public NntpException( int errorCode, string request )
			: base()
		{
			this.BuildNntpException( errorCode, request );
		}
		public NntpException( string response, string request )
			: base()
		{
			this.message = response;
			this.errorCode = 999;
			this.request = request;
		}
		public override string ToString()
		{
			if ( this.InnerException != null )
				return "Nntp:NntpException: [Request: " + this.request + "][Response: " + this.errorCode.ToString() + " " + this.message + "]\n" + this.InnerException.ToString() + "\n" + this.StackTrace;
			else
				return "Nntp:NntpException: [Request: " + this.request + "][Response: " + this.errorCode.ToString() + " " + this.message + "]\n" + this.StackTrace;
		}
	}

	public class ArticleBody
	{
		private bool isHtml;
		private string text;
		private Attachment [] attachments;

		public bool IsHtml
		{
			get
			{
				return isHtml;
			}
			set
			{
				isHtml = value;
			}
		}
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}
		public Attachment [] Attachments
		{
			get
			{
				return attachments;
			}
			set
			{
				attachments = value;
			}
		}
	}

	public class ArticleHeader
	{
		private string [] referenceIds;
		private string subject;
		private DateTime date;
		private string from;
		private string sender;
		private string postingHost;
		private int lineCount;

		public string [] ReferenceIds
		{
			get
			{
				return referenceIds;
			}
			set
			{
				referenceIds = value;
			}
		}
		public string Subject
		{
			get
			{
				return subject;
			}
			set
			{
				subject = value;
			}
		}
		public DateTime Date
		{
			get
			{
				return date;
			}
			set
			{
				date = value;
			}
		}
		public string From
		{
			get
			{
				return from;
			}
			set
			{
				from = value;
			}
		}
		public string Sender
		{
			get
			{
				return sender;
			}
			set
			{
				sender = value;
			}
		}
		public string PostingHost
		{
			get
			{
				return postingHost;
			}
			set
			{
				postingHost = value;
			}
		}
		public int LineCount
		{
			get
			{
				return lineCount;
			}
			set
			{
				lineCount = value;
			}
		}
	}

	public class Article
	{
		private string messageId;
		private int articleId;
		private ArticleHeader header;
		private ArticleBody body;
		private DateTime lastReply;
		private ArrayList children;

		public string MessageId
		{
			get
			{
				return messageId;
			}
			set
			{
				messageId = value;
			}
		}
		public int ArticleId
		{
			get
			{
				return articleId;
			}
			set
			{
				articleId = value;
			}
		}
		public ArticleHeader Header
		{
			get
			{
				return header;
			}
			set
			{
				header = value;
			}
		}
		public ArticleBody Body
		{
			get
			{
				return body;
			}
			set
			{
				body = value;
			}
		}
		public DateTime LastReply
		{
			get
			{
				return lastReply;
			}
			set
			{
				lastReply = value;
			}
		}
		public ArrayList Children
		{
			get
			{
				return children;
			}
			set
			{
				children = value;
			}
		}
	}

	public class Attachment
	{
		private string id;
		private string filename;
		private byte [] binaryData;

		public string Id
		{
			get
			{
				return id;
			}
		}
		public string Filename
		{
			get
			{
				return filename;
			}
		}
		public byte [] BinaryData
		{
			get
			{
				return binaryData;
			}
		}
		public Attachment( string id, string filename, byte [] binaryData )
		{
			this.id = id;
			this.filename = filename;
			this.binaryData = binaryData;
		}
		public void SaveAs( string path )
		{
			this.SaveAs( path, false );
		}
		public void SaveAs( string path, bool isOverwrite )
		{
			FileStream fs = null;
			if ( isOverwrite )
				fs = new FileStream( path, FileMode.Create );
			else
				fs = new FileStream( path, FileMode.CreateNew );
			fs.Write( binaryData, 0, binaryData.Length );
			fs.Close();
		}
	}

	public class MIMEPart
	{
		private byte [] binaryData;
		private string boundary;
		private string contentType;
		private string contentTransferEncoding;
		private string charset;
		private string filename;
		private string text;
		private ArrayList embeddedPartList;

		public byte [] BinaryData
		{
			get
			{
				return binaryData;
			}
			set
			{
				binaryData = value;
			}
		}
		public string Boundary
		{
			get
			{
				return boundary;
			}
			set
			{
				boundary = value;
			}
		}
		public string ContentType
		{
			get
			{
				return contentType;
			}
			set
			{
				contentType = value;
			}
		}
		public string ContentTransferEncoding
		{
			get
			{
				return contentTransferEncoding;
			}
			set
			{
				contentTransferEncoding = value;
			}
		}
		public string Charset
		{
			get
			{
				return charset;
			}
			set
			{
				charset = value;
			}
		}
		public string Filename
		{
			get
			{
				return filename;
			}
			set
			{
				filename = value;
			}
		}
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}
		public ArrayList EmbeddedPartList
		{
			get
			{
				return embeddedPartList;
			}
			set
			{
				embeddedPartList = value;
			}
		}
	}

	public class Newsgroup : IComparable
	{
		protected string group;
		protected int low;
		protected int high;

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}
		public int Low
		{
			get
			{
				return low;
			}
			set
			{
				low = value;
			}
		}
		public int High
		{
			get
			{
				return high;
			}
			set
			{
				high = value;
			}
		}
		public Newsgroup()
		{
		}
		public Newsgroup( string group, int low, int high )
		{
			this.group = group;
			this.low = low;
			this.high = high;
		}
		public int CompareTo( object r )
		{
			return this.Group.CompareTo( ( ( Newsgroup ) r ).Group );
		}
	}

	public class NntpUtil
	{
		private static int [] hexValue;
		private static char [] base64PemCode = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
			    									, 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
												, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '/' };
		private static byte [] base64PemConvertCode;

		static NntpUtil()
		{
			hexValue = new int [128];
			for ( int i = 0; i <= 9; i++ )
				hexValue [i + '0'] = i;
			for ( int i = 0; i < 6; i++ )
				hexValue [i + 'A'] = i + 10;

			base64PemConvertCode = new byte [256];
			for ( int i = 0; i < 255; i++ )
				base64PemConvertCode [i] = ( byte ) 255;
			for ( int i = 0; i < base64PemCode.Length; i++ )
				base64PemConvertCode [base64PemCode [i]] = ( byte ) i;
		}

		public static int UUDecode( string line, Stream outputStream )
		{
			return UUDecode( line.ToCharArray(), outputStream );
		}

		public static int UUDecode( char [] line, Stream outputStream )
		{
			if ( line.Length < 1 )
				throw new InvalidOperationException( "Invalid line: " + new String( line ) + "." );
			if ( line [0] == '`' )
				return 0;
			uint [] line2 = new uint [line.Length];
			for ( int ii = 0; ii < line.Length; ii++ )
				line2 [ii] = ( uint ) line [ii] - 32 & 0x3f;
			int length = ( int ) line2 [0];
			if ( ( int ) ( length / 3.0 + 0.999999999 ) * 4 > line.Length - 1 )
				throw new InvalidOperationException( "Invalid length(" + length + ") with line: " + new String( line ) + "." );

			int i = 1;
			int j = 0;
			while ( length > j + 3 )
			{
				outputStream.WriteByte( ( byte ) ( ( line2 [i] << 2 & 0xfc | line2 [i + 1] >> 4 & 0x3 ) & 0xff ) );
				outputStream.WriteByte( ( byte ) ( ( line2 [i + 1] << 4 & 0xf0 | line2 [i + 2] >> 2 & 0xf ) & 0xff ) );
				outputStream.WriteByte( ( byte ) ( ( line2 [i + 2] << 6 & 0xc0 | line2 [i + 3] & 0x3f ) & 0xff ) );
				i += 4;
				j += 3;
			}
			if ( length > j )
				outputStream.WriteByte( ( byte ) ( ( line2 [i] << 2 & 0xfc | line2 [i + 1] >> 4 & 0x3 ) & 0xff ) );
			if ( length > j + 1 )
				outputStream.WriteByte( ( byte ) ( ( line2 [i + 1] << 4 & 0xf0 | line2 [i + 2] >> 2 & 0xf ) & 0xff ) );
			if ( length > j + 2 )
				outputStream.WriteByte( ( byte ) ( ( line2 [i + 2] << 6 & 0xc0 | line2 [i + 3] & 0x3f ) & 0xff ) );
			return length;
		}

		public static int Base64Decode( string line, Stream outputStream )
		{
			return Base64Decode( line.ToCharArray(), outputStream );
		}

		public static int Base64Decode( char [] line, Stream outputStream )
		{
			if ( line.Length < 2 )
				throw new InvalidOperationException( "Invalid line: " + new String( line ) + "." );
			uint [] line2 = new uint [line.Length];
			for ( int ii = 0; ii < line.Length && line [ii] != '='; ii++ )
				line2 [ii] = ( uint ) base64PemConvertCode [line [ii] & 0xff];

			int length;
			for ( length = line2.Length - 1; line [length] == '=' && length >= 0; length-- ) ;
			length++;
			int i = 0;
			int j = 0;
			while ( length - i >= 4 )
			{
				outputStream.WriteByte( ( byte ) ( line2 [i] << 2 & 0xfc | line2 [i + 1] >> 4 & 0x3 ) );
				outputStream.WriteByte( ( byte ) ( line2 [i + 1] << 4 & 0xf0 | line2 [i + 2] >> 2 & 0xf ) );
				outputStream.WriteByte( ( byte ) ( line2 [i + 2] << 6 & 0xc0 | line2 [i + 3] & 0x3f ) );
				i += 4;
				j += 3;
			}
			switch ( length - i )
			{
				case 2:
					outputStream.WriteByte( ( byte ) ( line2 [i] << 2 & 0xfc | line2 [i + 1] >> 4 & 0x3 ) );
					return j + 1;
				case 3:
					outputStream.WriteByte( ( byte ) ( line2 [i] << 2 & 0xfc | line2 [i + 1] >> 4 & 0x3 ) );
					outputStream.WriteByte( ( byte ) ( line2 [i + 1] << 4 & 0xf0 | line2 [i + 2] >> 2 & 0xf ) );
					return j + 2;
				default:
					return j;
			}
		}

		public static int QuotedPrintableDecode( string line, Stream outputStream )
		{
			return QuotedPrintableDecode( line.ToCharArray(), outputStream );
		}

		public static int QuotedPrintableDecode( char [] line, Stream outputStream )
		{
			int length = line.Length;
			int i = 0, j = 0;
			while ( i < length )
			{
				if ( line [i] == '=' )
				{
					if ( i + 2 < length )
					{
						outputStream.WriteByte( ( byte ) ( hexValue [( int ) line [i + 1]] << 4 | hexValue [( int ) line [i + 2]] ) );
						i += 3;
					}
					else
						i++;
				}
				else
				{
					outputStream.WriteByte( ( byte ) ( line [i] ) );
					i++;
				}
				j++;
			}
			if ( line [length - 1] != '=' )
				outputStream.WriteByte( ( byte ) ( '\n' ) );
			return j;
		}
		public static MIMEPart DispatchMIMEContent( StreamReader sr, MIMEPart part, string seperator )
		{
			string line = null;
			Match m = null;
			MemoryStream ms;
			byte [] bytes;
			switch ( part.ContentType.Substring( 0, part.ContentType.IndexOf( '/' ) ).ToUpper() )
			{
				case "MULTIPART":
					MIMEPart newPart = null;
					while ( ( line = sr.ReadLine() ) != null && line != seperator && line != seperator + "--" )
					{
						m = Regex.Match( line, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase );
						if ( !m.Success )
						{
							continue;
						}
						newPart = new MIMEPart();
						newPart.ContentType = m.Groups [1].ToString();
						newPart.Charset = "US-ASCII";
						newPart.ContentTransferEncoding = "7BIT";
						while ( line != "" )
						{
							m = Regex.Match( line, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								newPart.Boundary = m.Groups [1].ToString();
								newPart.EmbeddedPartList = new ArrayList();
							}
							m = Regex.Match( line, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								newPart.Charset = m.Groups [1].ToString();
							}
							m = Regex.Match( line, @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								newPart.ContentTransferEncoding = m.Groups [1].ToString();
							}
							m = Regex.Match( line, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								newPart.Filename = Base64HeaderDecode( m.Groups [1].ToString() );
								newPart.Filename = newPart.Filename.Substring( newPart.Filename.LastIndexOfAny( new char [] { '\\', '/' } ) + 1 );
							}
							line = sr.ReadLine();
						}
						part.EmbeddedPartList.Add( DispatchMIMEContent( sr, newPart, "--" + part.Boundary ) );
					}
					break;
				case "TEXT":
					ms = new MemoryStream();
					bytes = null;
					long pos;
					StreamReader msr = new StreamReader( ms, Encoding.GetEncoding( part.Charset ) );
					StringBuilder sb = new StringBuilder();
					while ( ( line = sr.ReadLine() ) != null && line != seperator && line != seperator + "--" )
					{
						pos = ms.Position;
						if ( line != "" )
						{
							switch ( part.ContentTransferEncoding.ToUpper() )
							{
								case "QUOTED-PRINTABLE":
									NntpUtil.QuotedPrintableDecode( line, ms );
									break;
								case "BASE64":
									if ( line != null && line != "" )
										NntpUtil.Base64Decode( line, ms );
									break;
								case "UU":
									if ( line != null && line != "" )
										NntpUtil.UUDecode( line, ms );
									break;
								case "7BIT":
									bytes = Encoding.ASCII.GetBytes( line );
									ms.Write( bytes, 0, bytes.Length );
									ms.WriteByte( ( byte ) '\n' );
									break;
								default:
									bytes = Encoding.ASCII.GetBytes( line );
									ms.Write( bytes, 0, bytes.Length );
									ms.WriteByte( ( byte ) '\n' );
									break;
							}
						}
						ms.Position = pos;
						if ( part.ContentType.ToUpper() == "TEXT/HTML" )
						{
							sb.Append( msr.ReadToEnd() );
						}
						else
						{
							sb.Append( HttpUtility.HtmlEncode( msr.ReadToEnd() ).Replace( "\n", "<br>\n" ) );
						}
					}
					part.Text = sb.ToString();
					break;
				default:
					ms = new MemoryStream();
					bytes = null;
					while ( ( line = sr.ReadLine() ) != null && line != seperator && line != seperator + "--" )
					{
						if ( line != "" )
						{
							switch ( part.ContentTransferEncoding.ToUpper() )
							{
								case "QUOTED-PRINTABLE":
									NntpUtil.QuotedPrintableDecode( line, ms );
									break;
								case "BASE64":
									if ( line != null && line != "" )
										NntpUtil.Base64Decode( line, ms );
									break;
								case "UU":
									if ( line != null && line != "" )
										NntpUtil.UUDecode( line, ms );
									break;
								default:
									bytes = Encoding.ASCII.GetBytes( line );
									ms.Write( bytes, 0, bytes.Length );
									break;
							}
						}
					}
					ms.Seek( 0, SeekOrigin.Begin );
					part.BinaryData = new byte [ms.Length];
					ms.Read( part.BinaryData, 0, ( int ) ms.Length );
					break;
			}

			return part;
		}

		public static string Base64HeaderDecode( string line )
		{
			MemoryStream ms = null;
			byte [] bytes = null;
			string oStr = null;
			string code = null;
			string content = null;
			Match m = Regex.Match( line, @"=\?([^?]+)\?[^?]+\?([^?]+)\?=" );
			while ( m.Success )
			{
				ms = new MemoryStream();
				oStr = m.Groups [0].ToString();
				code = m.Groups [1].ToString();
				content = m.Groups [2].ToString();
				NntpUtil.Base64Decode( content, ms );
				ms.Seek( 0, SeekOrigin.Begin );
				bytes = new byte [ms.Length];
				ms.Read( bytes, 0, bytes.Length );
				line = line.Replace( oStr, Encoding.GetEncoding( code ).GetString( bytes ) );
				m = m.NextMatch();
			}
			return line;
		}

		public static ArrayList ConvertListToTree( ArrayList list )
		{
			Hashtable hash = new Hashtable( list.Count );
			ArrayList treeList = new ArrayList();
			int len;
			bool isTop;
			foreach ( Article article in list )
			{
				isTop = true;
				hash [article.MessageId] = article;
				article.LastReply = article.Header.Date;
				article.Children = new ArrayList();
				len = article.Header.ReferenceIds.Length;
				for ( int i = 0; i < len; i++ )
				{
					if ( hash.ContainsKey( article.Header.ReferenceIds [i] ) )
					{
						( ( Article ) hash [article.Header.ReferenceIds [i]] ).LastReply = article.LastReply;
						break;
					}
				}
				for ( int i = len - 1; i >= 0; i-- )
				{
					if ( hash.ContainsKey( article.Header.ReferenceIds [i] ) )
					{
						isTop = false;
						( ( Article ) hash [article.Header.ReferenceIds [i]] ).Children.Add( article );
						break;
					}
				}
				if ( isTop )
					treeList.Add( article );
			}
			return treeList;
		}
	}

	public delegate void OnRequestDelegate( string msg );

	public class NntpConnection
	{
		#region Private variables
		private TcpClient tcpClient = null;
		private StreamReader sr;
		private StreamWriter sw;

		private int timeout;
		private string connectedServer;
		private Newsgroup connectedGroup;
		private int port;
		private event OnRequestDelegate onRequest = null;

		private string username = null;
		private string password = null;
		#endregion

		#region Public accessors
		public int Timeout
		{
			get
			{
				return timeout;
			}
			[MethodImpl( MethodImplOptions.Synchronized )]
			set
			{
				timeout = value;
				tcpClient.SendTimeout = timeout;
				tcpClient.ReceiveTimeout = timeout;
			}
		}
		public string ConnectedServer
		{
			get
			{
				return connectedServer;
			}
		}
		public Newsgroup ConnectedGroup
		{
			get
			{
				return connectedGroup;
			}
		}
		public int Port
		{
			get
			{
				return port;
			}
		}
		public event OnRequestDelegate OnRequest
		{
			[MethodImpl( MethodImplOptions.Synchronized )]
			add
			{
				onRequest += value;
			}
			[MethodImpl( MethodImplOptions.Synchronized )]
			remove
			{
				onRequest -= value;
			}
		}
		#endregion

		#region Private methods
		private void Reset()
		{
			this.connectedServer = null;
			this.connectedGroup = null;
			this.username = null;
			this.password = null;
			if ( this.tcpClient != null )
				try
				{
					this.sw.Close();
					this.sr.Close();
					this.tcpClient.Close();
				}
				catch
				{
				}
			this.tcpClient = new TcpClient();
			this.tcpClient.SendTimeout = timeout;
			this.tcpClient.ReceiveTimeout = timeout;
		}
		private Response MakeRequest( string request )
		{
			if ( request != null )
			{
				sw.WriteLine( request );
				if ( onRequest != null )
					onRequest( "SEND: " + request );
			}
			string line = null;
			int code = 0;
			line = sr.ReadLine();
			if ( onRequest != null && line != null )
				onRequest( "RECEIVE: " + line );
			try
			{
				code = int.Parse( line.Substring( 0, 3 ) );
			}
			catch ( NullReferenceException )
			{
				this.Reset();
				throw new NntpException( line, request );
			}
			catch ( ArgumentOutOfRangeException )
			{
				this.Reset();
				throw new NntpException( line, request );
			}
			catch ( ArgumentNullException )
			{
				this.Reset();
				throw new NntpException( line, request );
			}
			catch ( FormatException )
			{
				this.Reset();
				throw new NntpException( line, request );
			}
			if ( code == 480 )
				if ( this.SendIdentity() )
					return MakeRequest( request );
			return new Response( code, ( line.Length >= 5 ? line.Substring( 4 ) : null ), request );
		}
		private ArticleHeader GetHeader( string messageId, out MIMEPart part )
		{
			string response = null;
			ArticleHeader header = new ArticleHeader();
			string name = null;
			string value = null;
			header.ReferenceIds = new string [0];
			string [] values = null;
			string [] values2 = null;
			Match m = null;
			part = null;
			int i = -1;
			while ( ( response = sr.ReadLine() ) != null && response != "" )
			{
				m = Regex.Match( response, @"^\s+(\S+)$" );
				if ( m.Success )
				{
					value = m.Groups [1].ToString();
				}
				else
				{
					i = response.IndexOf( ':' );
					if ( i == -1 )
					{
						continue;
					}
					name = response.Substring( 0, i ).ToUpper();
					value = response.Substring( i + 1 );
				}
				switch ( name )
				{
					case "REFERENCES":
						values = value.Split( ' ' );
						values2 = header.ReferenceIds;
						header.ReferenceIds = new string [values.Length + values2.Length];
						values.CopyTo( header.ReferenceIds, 0 );
						values2.CopyTo( header.ReferenceIds, values.Length );
						break;
					case "SUBJECT":
						header.Subject += NntpUtil.Base64HeaderDecode( value );
						break;
					case "DATE":
						i = value.IndexOf( ',' );
						header.Date = DateTime.Parse( value.Substring( i + 1, value.Length - 7 - i ) );
						break;
					case "FROM":
						header.From += NntpUtil.Base64HeaderDecode( value );
						break;
					case "NNTP-POSTING-HOST":
						header.PostingHost += value;
						break;
					case "LINES":
						header.LineCount = int.Parse( value );
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
						if ( part != null )
						{
							m = Regex.Match( response, @"CONTENT-TYPE: ""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								part.ContentType = m.Groups [1].ToString();
							}
							m = Regex.Match( response, @"BOUNDARY=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								part.Boundary = m.Groups [1].ToString();
								part.EmbeddedPartList = new ArrayList();
							}
							m = Regex.Match( response, @"CHARSET=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								part.Charset = m.Groups [1].ToString();
							}
							m = Regex.Match( response, @"NAME=""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								part.Filename = m.Groups [1].ToString();
							}
						}
						break;
					case "CONTENT-TRANSFER-ENCODING":
						if ( part != null )
						{
							m = Regex.Match( response, @"CONTENT-TRANSFER-ENCODING: ""?([^""\s;]+)", RegexOptions.IgnoreCase );
							if ( m.Success )
							{
								part.ContentTransferEncoding = m.Groups [1].ToString();
							}
						}
						break;
				}
			}
			return header;
		}
		private ArticleBody GetNormalBody( string messageId )
		{
			char [] buff = new char [1];
			string response = null;
			ArrayList list = new ArrayList();
			StringBuilder sb = new StringBuilder();
			Attachment attach = null;
			MemoryStream ms = null;
			sr.Read( buff, 0, 1 );
			int i = 0;
			byte [] bytes = null;
			Match m = null;
			while ( ( response = sr.ReadLine() ) != null )
			{
				if ( buff [0] == '.' )
				{
					if ( response == "" )
						break;
					else
						sb.Append( response );
				}
				else
				{
					if ( ( buff [0] == 'B' || buff [0] == 'b' ) && ( m = Regex.Match( response, @"^EGIN \d\d\d (.+)$", RegexOptions.IgnoreCase ) ).Success )
					{
						ms = new MemoryStream();
						while ( ( response = sr.ReadLine() ) != null && ( response.Length != 3 || response.ToUpper() != "END" ) )
						{
							NntpUtil.UUDecode( response, ms );
						}
						ms.Seek( 0, SeekOrigin.Begin );
						bytes = new byte [ms.Length];
						ms.Read( bytes, 0, ( int ) ms.Length );
						attach = new Attachment( messageId + " - " + m.Groups [1].ToString(), m.Groups [1].ToString(), bytes );
						list.Add( attach );
						ms.Close();
						i++;
					}
					else
					{
						sb.Append( buff [0] );
						sb.Append( response );
					}
				}
				sb.Append( '\n' );
				sr.Read( buff, 0, 1 );
			}
			ArticleBody ab = new ArticleBody();
			ab.IsHtml = false;
			ab.Text = sb.ToString();
			ab.Attachments = ( Attachment [] ) list.ToArray( typeof( Attachment ) );
			return ab;
		}
		private ArticleBody GetMIMEBody( string messageId, MIMEPart part )
		{
			string line = null;
			ArticleBody body = null;
			StringBuilder sb = null;
			ArrayList attachmentList = new ArrayList();
			try
			{
				NntpUtil.DispatchMIMEContent( sr, part, "." );
				sb = new StringBuilder();
				attachmentList = new ArrayList();
				body = new ArticleBody();
				body.IsHtml = true;
				this.ConvertMIMEContent( messageId, part, sb, attachmentList );
				body.Text = sb.ToString();
				body.Attachments = ( Attachment [] ) attachmentList.ToArray( typeof( Attachment ) );
			}
			finally
			{
				if ( ( ( NetworkStream ) sr.BaseStream ).DataAvailable )
					while ( ( line = sr.ReadLine() ) != null && line != "." ) ;
			}
			return body;
		}
		private void ConvertMIMEContent( string messageId, MIMEPart part, StringBuilder sb, ArrayList attachmentList )
		{
			Match m = null;
			m = Regex.Match( part.ContentType, @"MULTIPART", RegexOptions.IgnoreCase );
			if ( m.Success )
			{
				foreach ( MIMEPart subPart in part.EmbeddedPartList )
					this.ConvertMIMEContent( messageId, subPart, sb, attachmentList );
				return;
			}
			m = Regex.Match( part.ContentType, @"TEXT", RegexOptions.IgnoreCase );
			if ( m.Success )
			{
				sb.Append( part.Text );
				sb.Append( "<hr>" );
				return;
			}
			Attachment attachment = new Attachment( messageId + " - " + part.Filename, part.Filename, part.BinaryData );
			attachmentList.Add( attachment );
		}
		#endregion

		#region Public methods
		[MethodImpl( MethodImplOptions.Synchronized )]
		public NntpConnection()
		{
			this.timeout = 5000;
			this.Reset();
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void ConnectServer( string server, int port )
		{
			if ( this.connectedServer != null && this.connectedServer != server )
			{
				this.Disconnect();
			}
			if ( this.connectedServer != server )
			{
				tcpClient.Connect( server, port );
				NetworkStream stream = tcpClient.GetStream();
				if ( stream == null )
					throw new NntpException( "Fail to setup connection." );
				this.sr = new StreamReader( stream, Encoding.Default );
				this.sw = new StreamWriter( stream, Encoding.ASCII );
				this.sw.AutoFlush = true;
				Response res = MakeRequest( null );
				if ( res.Code != 200 && res.Code != 201 )
				{
					this.Reset();
					throw new NntpException( res.Code );
				}
				this.connectedServer = server;
				this.port = port;
			}
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void ProvideIdentity( string username, string password )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			this.username = username;
			this.password = password;
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public bool SendIdentity()
		{
			if ( this.username == null )
				return false;
			Response res = MakeRequest( "AUTHINFO USER " + this.username );
			if ( res.Code == 381 )
			{
				res = MakeRequest( "AUTHINFO PASS " + this.password );
			}
			if ( res.Code != 281 )
			{
				this.Reset();
				throw new NntpException( res.Code, "AUTHINFO PASS ******" );
			}
			return true;
		}

		[MethodImpl( MethodImplOptions.Synchronized )]
		public Newsgroup ConnectGroup( string group )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null || this.connectedGroup.Group != group )
			{
				Response res = MakeRequest( "GROUP " + group );
				if ( res.Code != 211 )
				{
					this.connectedGroup = null;
					throw new NntpException( res.Code, res.Request );
				}
				string [] values = res.Message.Split( ' ' );
				this.connectedGroup = new Newsgroup( group, int.Parse( values [1] ), int.Parse( values [2] ) );
			}
			return this.connectedGroup;
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public ArrayList GetGroupList()
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			Response res = MakeRequest( "LIST" );
			if ( res.Code != 215 )
				throw new NntpException( res.Code, res.Request );
			ArrayList list = new ArrayList();
			string response = null;
			string [] values;
			while ( ( response = sr.ReadLine() ) != null && response != "." )
			{
				values = response.Split( ' ' );
				list.Add( new Newsgroup( values [0], int.Parse( values [2] ), int.Parse( values [1] ) ) );
			}
			return list;
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public string GetMessageId( int articleId )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null )
				throw new NntpException( "No connecting newsgroup." );
			Response res = MakeRequest( "STAT " + articleId );
			if ( res.Code != 223 )
				throw new NntpException( res.Code, res.Request );
			int i = res.Message.IndexOf( '<' );
			int j = res.Message.IndexOf( '>' );
			return res.Message.Substring( i, j - i + 1 );
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public int GetArticleId( string messageId )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null )
				throw new NntpException( "No connecting newsgroup." );
			Response res = MakeRequest( "STAT " + messageId );
			if ( res.Code != 223 )
				throw new NntpException( res.Code, res.Request );
			int i = res.Message.IndexOf( ' ' );
			return int.Parse( res.Message.Substring( 0, i ) );
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public ArrayList GetArticleList( int low, int high )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null )
				throw new NntpException( "No connecting newsgroup." );
			Response res = MakeRequest( "XOVER " + low + "-" + high );
			if ( res.Code != 224 )
				throw new NntpException( res.Code, res.Request );
			ArrayList list = new ArrayList();
			Article article = null;
			string [] values = null;
			int i;
			string response = null;
			while ( ( response = sr.ReadLine() ) != null && response != "." )
			{
				try
				{
					article = new Article();
					article.Header = new ArticleHeader();
					values = response.Split( '\t' );
					article.ArticleId = int.Parse( values [0] );
					article.Header.Subject = NntpUtil.Base64HeaderDecode( values [1] );
					article.Header.From = NntpUtil.Base64HeaderDecode( values [2] );
					i = values [3].IndexOf( ',' );
					article.Header.Date = DateTime.Parse( values [3].Substring( i + 1, values [3].Length - 7 - i ) );
					article.MessageId = values [4];
					if ( values [5].Trim().Length == 0 )
						article.Header.ReferenceIds = new string [0];
					else
						article.Header.ReferenceIds = values [5].Split( ' ' );
					if ( values.Length < 8 || values [7] == null || values [7].Trim() == "" )
						article.Header.LineCount = 0;
					else
						article.Header.LineCount = int.Parse( values [7] );

					article.Body = null;
				}
				catch ( Exception e )
				{
					throw new Exception( response, e );
				}
				list.Add( article );
			}
			return list;
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public Article GetArticle( int articleId )
		{
			return GetArticle( articleId.ToString() );
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public Article GetArticle( string messageId )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null )
				throw new NntpException( "No connecting newsgroup." );
			Article article = new Article();
			Response res = MakeRequest( "Article " + messageId );
			if ( res.Code != 220 )
				throw new NntpException( res.Code );
			int i = res.Message.IndexOf( ' ' );
			article.ArticleId = int.Parse( res.Message.Substring( 0, i ) );
			int end = res.Message.IndexOf( ' ', i + 1 );
			if ( end == -1 ) end = res.Message.Length - ( i + 1 );
			article.MessageId = res.Message.Substring( i + 1, end );
			MIMEPart part = null;
			article.Header = this.GetHeader( messageId, out part );
			if ( part == null )
				article.Body = this.GetNormalBody( messageId );
			else
				article.Body = this.GetMIMEBody( messageId, part );
			return article;
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void PostArticle( Article article )
		{
			if ( this.connectedServer == null )
				throw new NntpException( "No connecting newsserver." );
			if ( this.connectedGroup == null )
				throw new NntpException( "No connecting newsgroup." );
			Response res = MakeRequest( "POST" );
			if ( res.Code != 340 )
				throw new NntpException( res.Code, res.Request );
			StringBuilder sb = new StringBuilder();
			sb.Append( "From: " );
			sb.Append( article.Header.From );
			sb.Append( "\r\nNewsgroup: " );
			sb.Append( this.connectedGroup );
			if ( article.Header.ReferenceIds != null && article.Header.ReferenceIds.Length != 0 )
			{
				sb.Append( "\r\nReference: " );
				sb.Append( string.Join( " ", article.Header.ReferenceIds ) );
			}
			sb.Append( "\r\nSubject: " );
			sb.Append( article.Header.Subject );
			sb.Append( "\r\n\r\n" );
			sb.Append( article.Body.Text.Replace( "\n.", "\n.." ) );
			sb.Append( "\r\n.\r\n" );
			res = MakeRequest( sb.ToString() );
			if ( res.Code != 240 )
				throw new NntpException( res.Code, res.Request );
		}
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void Disconnect()
		{
			if ( this.connectedServer != null )
			{
				string response = null;
				if ( ( ( NetworkStream ) sr.BaseStream ).DataAvailable )
				{
					while ( ( response = sr.ReadLine() ) != null && response != "." ) ;
				}
				Response res = MakeRequest( "QUIT" );
				if ( res.Code != 205 )
					throw new NntpException( res.Code, res.Request );
			}
			this.Reset();
		}
		#endregion

		private class Response
		{
			private int code;
			private string message;
			private string request;

			public Response( int code, string message, string request )
			{
				this.code = code;
				this.message = message;
				this.request = request;
			}
			public int Code
			{
				get
				{
					return code;
				}
				set
				{
					code = value;
				}
			}
			public string Message
			{
				get
				{
					return message;
				}
				set
				{
					message = value;
				}
			}
			public string Request
			{
				get
				{
					return request;
				}
				set
				{
					request = value;
				}
			}
		}

	}

	public static class YafNntp
	{
		static public int ReadArticles( object boardID, int nLastUpdate, int nTimeToRun, bool bCreateUsers )
		{
			int nUserID = YAF.Classes.Data.DB.user_guest( boardID );	// Use guests user-id
			string sHostAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
			DataTable dtSystem = YAF.Classes.Data.DB.registry_list( "TimeZone" );
			TimeSpan tsLocal = new TimeSpan( 0, Convert.ToInt32( dtSystem.Rows [0] ["Value"] ), 0 );
			DateTime dtStart = DateTime.Now;
			int nArticleCount = 0;

			string nntpHostName = string.Empty;
			int nntpPort = 119;

			NntpConnection nntpConnection = new NntpConnection();

			try
			{
				// Only those not updated in the last 30 minutes
				using ( DataTable dtForums = YAF.Classes.Data.DB.nntpforum_list( boardID, nLastUpdate, null, true ) )
				{
					foreach ( DataRow drForum in dtForums.Rows )
					{
						if ( nntpHostName != drForum ["Address"].ToString().ToLower() || nntpPort != ( int ) drForum ["Port"] )
						{
							if ( nntpConnection != null )
							{
								nntpConnection.Disconnect();
							}

							nntpHostName = drForum ["Address"].ToString().ToLower();
							nntpPort = Convert.ToInt32( drForum ["Port"] );

							// call connect server
							nntpConnection.ConnectServer( nntpHostName, nntpPort );

							// provide authentication if required...
							if ( drForum ["UserName"] != DBNull.Value && drForum ["UserPass"] != DBNull.Value )
							{
								nntpConnection.ProvideIdentity( drForum ["UserName"].ToString(), drForum ["UserPass"].ToString() );
								nntpConnection.SendIdentity();
							}
						}

						Newsgroup group = nntpConnection.ConnectGroup( drForum ["GroupName"].ToString() );

						int nLastMessageNo = ( int ) drForum ["LastMessageNo"];
						int nCurrentMessage = nLastMessageNo;
						// If this is first retrieve for this group, only fetch last 50
						if ( nCurrentMessage == 0 )
							nCurrentMessage = group.High - 50;

						nCurrentMessage++;

						int nForumID = ( int ) drForum ["ForumID"];

						for ( ; nCurrentMessage <= group.High; nCurrentMessage++ )
						{
							try
							{
								Article article = nntpConnection.GetArticle( nCurrentMessage );

								string sBody = article.Body.Text;
								string sSubject = article.Header.Subject;
								string sFrom = article.Header.From;
								string sThread = article.ArticleId.ToString();
								DateTime dtDate = article.Header.Date - tsLocal;

								if ( dtDate.Year < 1950 || dtDate > DateTime.Now )
									dtDate = DateTime.Now;

								sBody = String.Format( "Date: {0}\r\n\r\n", article.Header.Date ) + sBody;
								sBody = String.Format( "Date parsed: {0}\r\n", dtDate ) + sBody;

								if ( bCreateUsers )
									nUserID = YAF.Classes.Data.DB.user_nntp( boardID, sFrom, "" );

								sBody = System.Web.HttpContext.Current.Server.HtmlEncode( sBody );
								YAF.Classes.Data.DB.nntptopic_savemessage( drForum ["NntpForumID"], sSubject, sBody, nUserID, sFrom, sHostAddress, dtDate, sThread );
								nLastMessageNo = nCurrentMessage;
								nArticleCount++;
								// We don't wanna retrieve articles forever...
								// Total time x seconds for all groups
								if ( ( DateTime.Now - dtStart ).TotalSeconds > nTimeToRun )
									break;
							}
							catch ( NntpException )
							{
							}
						}
						YAF.Classes.Data.DB.nntpforum_update( drForum ["NntpForumID"], nLastMessageNo, nUserID );
						// Total time x seconds for all groups
						if ( ( DateTime.Now - dtStart ).TotalSeconds > nTimeToRun )
							break;
					}
				}
			}
			finally
			{
				if ( nntpConnection != null )
				{
					nntpConnection.Disconnect();
				}
			}
			return nArticleCount;
		}
	}
}
