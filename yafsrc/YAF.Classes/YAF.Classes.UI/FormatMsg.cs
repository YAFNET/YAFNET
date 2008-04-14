/* Yet Another Forum.net
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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

		// format message regex
		static private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

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
		/// For backwards compatibility
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		static public string AddSmiles( string message )
		{
			ReplaceRules layers = new ReplaceRules();
			FormatMsg.AddSmiles( ref layers );
			// apply...
			layers.Process( ref message );
			return message;
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
				code = code.Replace( "&", "&amp;" );
				code = code.Replace( ">", "&gt;" );
				code = code.Replace( "<", "&lt;" );				
				code = code.Replace( "\"", "&quot;" );

				// add new rules for smilies...
				rules.AddRule( new SimpleReplaceRule( code.ToLower(),
																							String.Format(
																								"<img src=\"{0}\" alt=\"{1}\" />",
																								YafBuildLink.Smiley( Convert.ToString( row ["Icon"] ) ),
																								HttpContext.Current.Server.HtmlEncode( row ["Emoticon"].ToString() )
																								) ) );
				rules.AddRule( new SimpleReplaceRule( code.ToUpper(),
																							String.Format(
																								"<img src=\"{0}\" alt=\"{1}\" />",
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
			return FormatMessage( message, messageFlags, false );
		}
		static public string FormatMessage( string message, MessageFlags messageFlags, bool targetBlankOverride )
		{
			return FormatMessage( message, messageFlags, targetBlankOverride, DateTime.Now );
		}		
		static public string FormatMessage( string message, MessageFlags messageFlags, bool targetBlankOverride, DateTime messageLastEdited )
		{
			bool useNoFollow = YafContext.Current.BoardSettings.UseNoFollowLinks;

			// check to see if no follow should be disabled since the message is properly aged
			if ( useNoFollow && YafContext.Current.BoardSettings.DisableNoFollowLinksAfterDay > 0 )
			{
				TimeSpan messageAge = messageLastEdited - DateTime.Now;
				if ( messageAge.Days > YafContext.Current.BoardSettings.DisableNoFollowLinksAfterDay )
				{
					// disable no follow
					useNoFollow = false;
				}				
			}

			// do html damage control
			message = RepairHtml( message, messageFlags.IsHtml );

			// get the rules engine from the creator...
			ReplaceRules ruleEngine = ReplaceRulesCreator.GetInstance( new bool [] { messageFlags.IsBBCode, targetBlankOverride, useNoFollow } );

			// see if the rules are already populated...
			if ( ruleEngine.RulesList.Count == 0 )
			{
				// populate

				// get rules for BBCode and Smilies
				BBCode.CreateBBCodeRules( ref ruleEngine, messageFlags.IsBBCode, targetBlankOverride, useNoFollow );

				// add email rule
				VariableRegexReplaceRule email =
					new VariableRegexReplaceRule(
						@"(?<before>^|[ ]|<br/>)(?<inner>\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)",
						"${before}<a href=\"mailto:${inner}\">${inner}</a>",
						_options,
						new string [] { "before" }
					);
				email.RuleRank = 10;

				ruleEngine.AddRule( email );

				// URLs Rules
				string target = ( YafContext.Current.BoardSettings.BlankLinks || targetBlankOverride ) ? "target=\"_blank\"" : "";
				string nofollow = ( useNoFollow ) ? "rel=\"nofollow\"" : "";

				VariableRegexReplaceRule url =
					new VariableRegexReplaceRule(
						@"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?+%#&=;,]*)?)",
						"${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace( "{0}", target ).Replace( "{1}", nofollow ),
						_options,
						new string [] { "before" },
						new string [] { "" },
						50
					);

				url.RuleRank = 10;
				ruleEngine.AddRule( url );

				url =
					new VariableRegexReplaceRule(
						@"(?<before>^|[ ]|<br/>)(?<!href="")(?<!src="")(?<inner>(http://|https://|ftp://)(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%&=+;,#~$]*[^.<])?)",
						"${before}<a {0} {1} href=\"${inner}\" title=\"${inner}\">${innertrunc}</a>".Replace( "{0}", target ).Replace( "{1}", nofollow ),
						_options,
						new string [] { "before" },
						new string [] { "" },
						50
					);
				url.RuleRank = 10;
				ruleEngine.AddRule( url );

				url =
					new VariableRegexReplaceRule(
						@"(?<before>^|[ ]|<br/>)(?<!http://)(?<inner>www\.(?:[\w-]+\.)+[\w-]+(?:/[\w-./?%+#&=;,]*)?)",
						"${before}<a {0} {1} href=\"http://${inner}\" title=\"http://${inner}\">${innertrunc}</a>".Replace( "{0}", target ).Replace( "{1}", nofollow ),
						_options,
						new string [] { "before" },
						new string [] { "" },
						50
					);
				url.RuleRank = 10;
				ruleEngine.AddRule( url );
			}

			// process...
			ruleEngine.Process( ref message );

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
