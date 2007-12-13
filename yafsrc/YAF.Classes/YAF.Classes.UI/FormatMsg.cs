/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using YAF.Classes.Data;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Summary description for FormatMsg.
	/// </summary>
	public class FormatMsg
	{
		/* Ederon : 6/16/2007 - conventions */

		/// <summary>
		/// Formats message by converting "Forum Code" to HTML.
		/// </summary>
		/// <param name="basePage">Forum Page</param>
		/// <param name="Message">Message to Convert</param>
		/// <returns>Converted Message Text</returns>
		static protected string ConvertForumCode( string message )
		{
			string tmp = "";
			bool inCode = false;

			for ( int i = 0; i < message.Length; i++ )
			{
				if ( message [i] == '[' )
				{
					int e1 = message.IndexOf( ']', i );
					int e2 = message.IndexOf( '=', i );

					if ( e1 > 0 )
					{
						bool none = false;
						string cmd, arg = null;

						if ( e2 < 0 || e2 > e1 )
						{
							cmd = message.Substring( i + 1, e1 - i - 1 );
							arg = null;
						}
						else
						{
							cmd = message.Substring( i + 1, e2 - i - 1 );
							arg = message.Substring( e2 + 1, e1 - e2 - 1 );

							arg = arg.Trim();

							arg = HttpContext.Current.Server.HtmlDecode( arg );
							if ( arg.Length > 2 && arg [0] == '"' && arg [arg.Length - 1] == '"' )
								arg = arg.Substring( 1, arg.Length - 2 );
						}

						cmd = cmd.ToLower();
						if ( !inCode || cmd == "/code" )
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
										if ( YafContext.Current.BoardSettings.BlankLinks )
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
									inCode = true;
									break;
								case "/code":
									tmp += "</pre>";
									inCode = false;
									break;
								default:
									none = true;
									break;
							}
						}
						else
						{
							none = true;
						}
						if ( !none )
						{
							i = e1;
							continue;
						}
					}
				}
				tmp += message [i];
			}

			return tmp;
		}

		/// <summary>
		/// Adds smiles replacement rules to the collection from the DB
		/// </summary>
		static public void AddSmiles( ref ReplaceRules rules )
		{
			DataTable dtSmileys = GetSmilies();

			foreach ( DataRow row in dtSmileys.Rows )
			{
				string code = row ["Code"].ToString();
				code = code.Replace( ">", "&gt;" );
				code = code.Replace( "<", "&lt;" );
				code = code.Replace( "&", "&amp;" );
				code = code.Replace( "\"", "&quot;" );

				// add new rules for smilies...
				rules.AddRule( new SimpleReplaceRule(	code.ToLower(),
																							String.Format( 
																								"<img src=\"{0}\" alt=\"{1}\">",
																								YafBuildLink.Smiley( Convert.ToString( row ["Icon"] ) ),
																								HttpContext.Current.Server.HtmlEncode( row ["Emoticon"].ToString() )
																								) ) );
				rules.AddRule( new SimpleReplaceRule( code.ToUpper(),
																							String.Format(
																								"<img src=\"{0}\" alt=\"{1}\">",
																								YafBuildLink.Smiley( Convert.ToString( row ["Icon"] ) ),
																								HttpContext.Current.Server.HtmlEncode( row ["Emoticon"].ToString() )
																								) ) );
			}
		}

		static public DataTable GetSmilies()
		{
			string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.Smilies );
			DataTable smiliesTable = YafCache.Current [cacheKey] as DataTable;

			// check if there is value cached
			if ( smiliesTable == null )
			{
				// get the smilies table from the db...
				smiliesTable = YAF.Classes.Data.DB.smiley_list( YafContext.Current.PageBoardID, null );
				// cache it for 60 minutes...
				YafCache.Current.Insert( cacheKey, smiliesTable, null, DateTime.Now.AddMinutes( 60 ), TimeSpan.Zero );
			}

			return smiliesTable;
		}

		static public string FormatMessage( string message, MessageFlags messageFlags )
		{
			return FormatMessage( message, messageFlags, true, false );
		}

		static public string FormatMessage( string message, MessageFlags messageFlags, bool isModeratorChanged )
		{
			return FormatMessage( message, messageFlags, isModeratorChanged, false );
		}

		// format message regex
		static private RegexOptions _options = RegexOptions.IgnoreCase;
		static private Regex _rgxEmail = new Regex( @"(?<before>^|[ ]|<br/>)(?<inner>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)", _options );
		static private Regex _rgxUrl1 = new Regex( @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,]*)?)", _options );
		static private Regex _rgxUrl2 = new Regex( @"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,#~$]*[^.<])?)", _options );
		static private Regex _rgxUrl3 = new Regex( @"(?<before>^|[ ]|<br/>)(?<!http://)(?<inner>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=;,]*)?)", _options );

		//if message was deleted then write that instead of real body
		static public string FormatMessage( string message, MessageFlags messageFlags, bool isModeratorChanged, bool targetBlankOverride )
		{
			if ( messageFlags.IsDeleted )
			{
				// TODO: Needs to be localized
				message = "Message was deleted";
				if ( isModeratorChanged ) { message += " by moderator."; } else { message += " by user."; };
				return message;
			}

			// do html damage control
			message = RepairHtml( message, messageFlags.IsHtml );

			// do BBCode and Smilies...
			message = BBCode.MakeHtml( message, messageFlags.IsBBCode, targetBlankOverride );

			//Email -- RegEx VS.NET
			BBCode.NestedReplace( ref message, _rgxEmail, "${before}<a href=\"mailto:${inner}\">${inner}</a>", new string [] { "before" } );

			// URLs
			if ( YafContext.Current.BoardSettings.BlankLinks || targetBlankOverride )
			{
				// target is blank...
				BBCode.NestedReplace( ref message, _rgxUrl1, "${before}<a target=\"_blank\" rel=\"nofollow\" href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
				BBCode.NestedReplace( ref message, _rgxUrl2, "${before}<a target=\"_blank\" rel=\"nofollow\" href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
				BBCode.NestedReplace( ref message, _rgxUrl3, "${before}<a target=\"_blank\" rel=\"nofollow\" href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
			}
			else
			{
				BBCode.NestedReplace( ref message, _rgxUrl1, "${before}<a rel=\"nofollow\" href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
				BBCode.NestedReplace( ref message, _rgxUrl2, "${before}<a rel=\"nofollow\" href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
				BBCode.NestedReplace( ref message, _rgxUrl3, "${before}<a rel=\"nofollow\" href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>", new string [] { "before" }, new string [] { "" }, 50 );
			}

			// jaben : moved word replace to reusable function in class utils
			message = General.BadWordReplace( message );

			return message;
		}

		static public string RemoveNestedQuotes( string body )
		{
			RegexOptions m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
			Regex quote = new Regex( @"\[quote(\=[^\]]*)?\](.*?)\[/quote\]", m_options );

			// remove quotes from old messages
			return quote.Replace( body, "" );
		}

		static private bool IsValidTag( string tag, string [] allowedTags )
		{
			if ( tag.IndexOf( "javascript" ) >= 0 ) return false;
			if ( tag.IndexOf( "vbscript" ) >= 0 ) return false;
			if ( tag.IndexOf( "onclick" ) >= 0 ) return false;

			char [] endchars = new char [] { ' ', '>', '/', '\t' };

			int pos = tag.IndexOfAny( endchars, 1 );
			if ( pos > 0 ) tag = tag.Substring( 0, pos );
			if ( tag [0] == '/' ) tag = tag.Substring( 1 );

			// check if it's a valid tag
			foreach ( string aTag in allowedTags )
			{
				if ( tag == aTag ) return true;
			}

			return false;
		}

		static public string RepairHtml( string html, bool allowHtml )
		{
			if ( !allowHtml )
			{
				html = BBCode.EncodeHTML( html );
			}
			else
			{
				// get allowable html tags
				string tStr = YafContext.Current.BoardSettings.AcceptedHTML;
				string [] allowedTags = tStr.Split( ',' );

				RegexOptions options = RegexOptions.IgnoreCase;

				MatchCollection m = Regex.Matches( html, "<.*?>", options );

				for ( int i = m.Count - 1; i >= 0; i-- )
				{
					string tag = html.Substring( m [i].Index + 1, m [i].Length - 1 ).Trim().ToLower();

					if ( !IsValidTag( tag, allowedTags ) )
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
}
