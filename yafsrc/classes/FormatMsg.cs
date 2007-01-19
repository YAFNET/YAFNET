/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;
using YAF.Classes.Utils;

namespace YAF
{
	/// <summary>
	/// Summary description for FormatMsg.
	/// </summary>
	public class FormatMsg
	{
		/// <summary>
		/// Formats message by converting "Forum Code" to HTML.
		/// </summary>
		/// <param name="basePage">Forum Page</param>
		/// <param name="Message">Message to Convert</param>
		/// <returns>Converted Message Text</returns>
		static protected string iConvertForumCode( string Message )
		{
			string tmp = "";
			bool bInCode = false;
			for ( int i = 0; i < Message.Length; i++ )
			{
				if ( Message [i] == '[' )
				{
					int e1 = Message.IndexOf( ']', i );
					int e2 = Message.IndexOf( '=', i );
					if ( e1 > 0 )
					{
						bool bNone = false;
						string cmd, arg = null;
						if ( e2 < 0 || e2 > e1 )
						{
							cmd = Message.Substring( i + 1, e1 - i - 1 );
							arg = null;
						}
						else
						{
							cmd = Message.Substring( i + 1, e2 - i - 1 );
							arg = Message.Substring( e2 + 1, e1 - e2 - 1 );

							arg = arg.Trim();

							arg = HttpContext.Current.Server.HtmlDecode( arg );
							if ( arg.Length > 2 && arg [0] == '"' && arg [arg.Length - 1] == '"' )
								arg = arg.Substring( 1, arg.Length - 2 );
						}

						cmd = cmd.ToLower();
						if ( !bInCode || cmd == "/code" )
						{
							switch ( cmd )
							{
								case "b":
									tmp += "<b>";
									break;
								case "/b":
									tmp += "</b>";
									break;
								case "i":
									tmp += "<em>";
									break;
								case "/i":
									tmp += "</em>";
									break;
								case "u":
									tmp += "<u>";
									break;
								case "/u":
									tmp += "</u>";
									break;
								case "url":
									if ( arg != null )
									{
										if ( yaf_Context.Current.BoardSettings.BlankLinks )
											tmp += String.Format( "<a target=\"_blank\" href=\"{0}\">", arg );
										else
											tmp += String.Format( "<a target=\"_top\" href=\"{0}\">", arg );
									}
									else
										tmp += "<a>";
									break;
								case "/url":
									tmp += "</a>";
									break;
								case "img":
									tmp += "<img src=\"";
									break;
								case "/img":
									tmp += "\"/>";
									break;
								case "color":
									if ( arg != null )
										tmp += String.Format( "<span style=\"color:{0}\">", arg );
									else
										tmp += "<span>";
									break;
								case "/color":
									tmp += "</span>";
									break;
								case "code":
									tmp += "<pre>";
									bInCode = true;
									break;
								case "/code":
									tmp += "</pre>";
									bInCode = false;
									break;
								default:
									bNone = true;
									break;
							}
						}
						else
						{
							bNone = true;
						}
						if ( !bNone )
						{
							i = e1;
							continue;
						}
					}
				}
				tmp += Message [i];
			}

			return tmp;
		}

		/// <summary>
		/// Adds smiles into the code.
		/// </summary>
		/// <param name="basePagee">Forum base page</param>
		/// <param name="Message">Text to add smiles to.</param>
		/// <returns>Processed text with smiles added.</returns>
		static public string iAddSmiles( string Message )
		{
			DataTable dtSmileys = GetSmilies();
			string strTemp = Message;

			foreach ( DataRow row in dtSmileys.Rows )
			{
				string code = row ["Code"].ToString();

				code = code.Replace( "&", "&amp;" );
				code = code.Replace( "\"", "&quot;" );
				// some symbols in html source becomes smylies
				// so prevent this
				strTemp = strTemp.Replace( "&amp;", "&amp%3B" );
				strTemp = strTemp.Replace( "&quot;", "&quot%3B" );
				strTemp = strTemp.Replace( "mailto:", "mailto%3A" );
				strTemp = strTemp.Replace( "color:", "color%3A" );

				strTemp = strTemp.Replace( code.ToLower(), String.Format( "<img src=\"{0}\" alt=\"{1}\">", yaf_BuildLink.Smiley( Convert.ToString( row ["Icon"] ) ), HttpContext.Current.Server.HtmlEncode( row ["Emoticon"].ToString() ) ) );
				strTemp = strTemp.Replace( code.ToUpper(), String.Format( "<img src=\"{0}\" alt=\"{1}\">", yaf_BuildLink.Smiley( Convert.ToString( row ["Icon"] ) ), HttpContext.Current.Server.HtmlEncode( row ["Emoticon"].ToString() ) ) );

				// restore html source

				strTemp = strTemp.Replace( "&amp%3B", "&amp;" );
				strTemp = strTemp.Replace( "&quot%3B", "&quot;" );
				strTemp = strTemp.Replace( "mailto%3A", "mailto:" );
				strTemp = strTemp.Replace( "color%3A", "color:" );
			}

			return strTemp;
		}

		/// <summary>
		/// Supposed to convert HTML to BBCode -- Doesn't function
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		[Obsolete( "Doesn't work" )]
		static public string HtmlToForumCode( string html )
		{
#if true
			return html;
#else
			html = html.Replace("<ul>","[list]");	// TODO
			html = html.Replace("</ul>","[/list]");	// TODO
			html = html.Replace("<ol>","[list]");	// TODO
			html = html.Replace("</ol>","[/list]");	// TODO
			html = html.Replace("<li>","[*]");		// TODO
			html = html.Replace("</li>","");		// TODO
			
			RegexOptions options = RegexOptions.IgnoreCase;
			html = Regex.Replace(html,"<a href=\"(.*)\">(.*)</a>","[url=\"$1\"]$2[/url]",options);
			html = Regex.Replace(html,"<img src=\"(.*)\">","[img]$1[/img]",options);
			html = Regex.Replace(html,"<p(.*?)>","",options);
			html = Regex.Replace(html,"</p>","<br /><br />",options);
			html = Regex.Replace(html,"<br />","\n",options);
			html = Regex.Replace(html,"<b>","[b]",options);
			html = Regex.Replace(html,"</b>","[/b]",options);
			html = Regex.Replace(html,"<strong>","[b]",options);
			html = Regex.Replace(html,"</strong>","[/b]",options);
			html = Regex.Replace(html,"<i>","[i]",options);
			html = Regex.Replace(html,"</i>","[/i]",options);
			html = Regex.Replace(html,"<em>","[i]",options);
			html = Regex.Replace(html,"</em>","[/i]",options);
			html = Regex.Replace(html,"<u>","[u]",options);
			html = Regex.Replace(html,"</u>","[/u]",options);
			html = Regex.Replace(html,"<blockquote(.*)>","[block]",options);			
			html = Regex.Replace(html,"</blockquote>","[/block]",options);			

			html = Regex.Replace(html,"<","&lt;",options);
			html = Regex.Replace(html,">","&gt;",options);
//			if(html.IndexOf('<')>=0 || html.IndexOf('>')>=0)
//				html += "\n\nINVALID";

			return html;
#endif
		}

		static public DataTable GetSmilies()
		{
			DataTable dt = ( DataTable ) System.Web.HttpContext.Current.Cache ["Smilies"];
			if ( dt == null )
			{
				dt = YAF.Classes.Data.DB.smiley_list( yaf_Context.Current.PageBoardID, null );
				System.Web.HttpContext.Current.Cache.Insert( "Smilies", dt, null, DateTime.Now.AddMinutes( 60 ), TimeSpan.Zero );
			}
			return dt;
		}

		static public string FormatMessage( string Message, MessageFlags mFlags )
		{
			return FormatMessage( Message, mFlags, true );
		}

		//if message was deleted then write that instead of real body
		static public string FormatMessage( string Message, MessageFlags mFlags, bool isModeratorChanged )
		{
			if ( mFlags.IsDeleted )
			{
				Message = "Message was deleted";
				if ( isModeratorChanged ) { Message += " by moderator."; } else { Message += " by user."; };
				return Message;
			}

			// do html damage control
			Message = RepairHtml( Message, mFlags.IsHTML );

			// convert spaces if bbcode (causes too many problems)
			/*if (mFlags.IsBBCode)
			{
				Message = Message.Replace(" ","&nbsp;");
			}*/

			// do BBCode and Smilies...
			Message = BBCode.MakeHtml( Message, mFlags.IsBBCode );

			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;

			//Email -- RegEx VS.NET
			Message = Regex.Replace( Message, @"(?<before>^|[ ]|<br/>)(?<email>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", "${before}<a href=\"mailto:${email}\">${email}</a>", options );

			//URL (http://) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
			Message = Regex.Replace( Message, "(?<before>^|[ ]|<br/>)(?<!href=\")(?<!src=\")(?<url>(http://|https://|ftp://)(?:[\\w-]+\\.)+[\\w-]+(?:/[\\w-./?%&=;,]*)?)", "${before}<a rel=\"nofollow\" href=\"${url}\">${url}</a>", options );

			// Demonixed : addition
			Message = Regex.Replace( Message, "(?<before>^|[ ]|<br/>)(?<!href=\")(?<!src=\")(?<url>(http://|https://|ftp://)(?:[\\w-]+\\.)+[\\w-]+(?:/[\\w-./?%&=;,#~$]*[^.<])?)", "${before}<a rel=\"nofollow\" href=\"${url}\">${url}</a>", options );


			//URL (www) -- RegEx http://www.dotnet247.com/247reference/msgs/2/10022.aspx
			Message = Regex.Replace( Message, @"(?<before>^|[ ]|<br/>)(?<!http://)(?<url>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,]*)?)", "${before}<a rel=\"nofollow\" href=\"http://${url}\">${url}</a>", options );

			// jaben : moved word replace to reusable function in class utils
			Message = General.BadWordReplace( Message );

			return Message;
		}

		static private bool IsValidTag( string tag, string [] AllowedTags )
		{
			if ( tag.IndexOf( "javascript" ) >= 0 ) return false;
			if ( tag.IndexOf( "vbscript" ) >= 0 ) return false;
			if ( tag.IndexOf( "onclick" ) >= 0 ) return false;

			char [] endchars = new char [] { ' ', '>', '/', '\t' };

			int pos = tag.IndexOfAny( endchars, 1 );
			if ( pos > 0 ) tag = tag.Substring( 0, pos );
			if ( tag [0] == '/' ) tag = tag.Substring( 1 );

			// check if it's a valid tag
			foreach ( string aTag in AllowedTags )
			{
				if ( tag == aTag ) return true;
			}

			return false;
		}

		static public string RepairHtml( string html, bool bAllowHtml )
		{
			if ( !bAllowHtml )
			{
				html = BBCode.EncodeHTML( html );
			}
			else
			{
				// get allowable html tags
				string tStr = yaf_Context.Current.BoardSettings.AcceptedHTML;
				string [] AllowedTags = tStr.Split( ',' );

				RegexOptions options = RegexOptions.IgnoreCase;

				MatchCollection m = Regex.Matches( html, "<.*?>", options );

				for ( int i = m.Count - 1; i >= 0; i-- )
				{
					string tag = html.Substring( m [i].Index + 1, m [i].Length - 1 ).Trim().ToLower();

					if ( !IsValidTag( tag, AllowedTags ) )
					{
						html = html.Remove( m [i].Index, m [i].Length );
						// just don't show this tag for now

						//string tmp = System.Web.HttpContext.Current.Server.HtmlEncode(html.Substring(m[i].Index,m[i].Length));
						//html = html.Insert(m[i].Index,tmp);
					}
				}
			}
			return html;
		}
	}

	public class MessageFlags
	{
		int FBitValue;

		public MessageFlags()
			: this( 23 )
		{

		}

		public MessageFlags( int bitValue )
		{
			FBitValue = bitValue;
		}

		static public bool GetBitAsBool( int bitValue, int bitShift )
		{
			if ( bitShift > 31 ) bitShift %= 31;
			if ( ( ( bitValue >> bitShift ) & 0x00000001 ) == 1 ) return true;
			return false;
		}

		static public int SetBitFromBool( int bitValue, int bitShift, bool bValue )
		{
			if ( bitShift > 31 ) bitShift %= 31;

			if ( GetBitAsBool( bitValue, bitShift ) != bValue )
			{
				// toggle that value using XOR
				int tV = 0x00000001 << bitShift;
				bitValue ^= tV;
			}
			return bitValue;
		}

		public static implicit operator MessageFlags( int newBitValue )
		{
			MessageFlags mf = new MessageFlags( newBitValue );
			return mf;
		}

		public int BitValue
		{
			get { return FBitValue; }
			set { FBitValue = value; }
		}

		public bool this [int index]
		{
			get { return GetBitAsBool( FBitValue, index ); }
			set { FBitValue = SetBitFromBool( FBitValue, index, value ); }
		}

		// actual flags here -- can be a total of 31
		public bool IsHTML
		{
			get { return GetBitAsBool( FBitValue, 0 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 0, value ); }
		}

		public bool IsBBCode
		{
			get { return GetBitAsBool( FBitValue, 1 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 1, value ); }
		}

		public bool IsSmilies
		{
			get { return GetBitAsBool( FBitValue, 2 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 2, value ); }
		}

		public bool IsDeleted
		{
			get { return GetBitAsBool( FBitValue, 3 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 3, value ); }
		}

		public bool IsApproved
		{
			get { return GetBitAsBool( FBitValue, 4 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 4, value ); }
		}

		/// <summary>
		/// This post is locked -- nothing can be done to it
		/// </summary>
		public bool IsLocked
		{
			get { return GetBitAsBool( FBitValue, 5 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 5, value ); }
		}

		/// <summary>
		/// Setting so that the message isn't formatted at all
		/// </summary>
		public bool NotFormatted
		{
			get { return GetBitAsBool( FBitValue, 6 ); }
			set { FBitValue = SetBitFromBool( FBitValue, 6, value ); }
		}
	}
}
