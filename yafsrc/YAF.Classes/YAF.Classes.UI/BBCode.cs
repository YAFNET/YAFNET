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
using System.Web;
using System.Text.RegularExpressions;
using YAF.Classes.Utils;

namespace YAF.Classes.UI
{
	/// <summary>
	/// Summary description for BBCode.
	/// </summary>
	public class BBCode
	{
		/* Ederon : 6/16/2007 - conventions */

		private BBCode() {}

		static private int GetNumber( string input )
		{
			try
			{
				return int.Parse( input );
			}
			catch ( FormatException )
			{
				return -1;
			}
		}

		static private string GetFontSize( int input )
		{
			switch ( input )
			{
				case 1:
					return "50%";
				case 2:
					return "70%";
				case 3:
					return "80%";
				case 4:
					return "90%";
				case 5:
				default:
					return "100%";
				case 6:
					return "120%";
				case 7:
					return "140%";
				case 8:
					return "160%";
				case 9:
					return "180%";
			}
			///return string.Format("{0}pt",input*2);
		}

		static private RegexOptions _options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
		static private Regex _rgxCode2 = new Regex( @"\[code=(?<language>[^\]]*)\](?<inner>(.*?))\[/code\]", _options );
		static private Regex _rgxCode1 = new Regex( @"\[code\](?<inner>(.*?))\[/code\]", _options );
		static private Regex _rgxSize = new Regex( @"\[size=(?<size>([1-9]))\](?<inner>(.*?))\[/size\]", _options );
		static private Regex _rgxBold = new Regex( @"\[B\](?<inner>(.*?))\[/B\]", _options );
		static private Regex _rgxStrike = new Regex( @"\[S\](?<inner>(.*?))\[/S\]", _options );
		static private Regex _rgxItalic = new Regex( @"\[I\](?<inner>(.*?))\[/I\]", _options );
		static private Regex _rgxUnderline = new Regex( @"\[U\](?<inner>(.*?))\[/U\]", _options );
		static private Regex _rgxEmail2 = new Regex( @"\[email=(?<email>[^\]]*)\](?<inner>(.*?))\[/email\]", _options );
		static private Regex _rgxEmail1 = new Regex( @"\[email[^\]]*\](?<inner>(.*?))\[/email\]", _options );
		static private Regex _rgxUrl1 = new Regex( @"\[url\]( ?<http>( skype:)|( http://)|( https://)| (ftp://)|( ftps://))? (?<inner>( .*?))\[/ur l\]", _options );
		static private Regex _rgxUrl2 = new Regex( @"\[url\]( ?<http>( skype:)|( http://)|( https://)| (ftp://)|( ftps://))? (?<inner>( .*?))\[/ur l\]", _options );
		static private Regex _rgxFont = new Regex( @"\[font=(?<font>([-a-z0-9, ]*))\](?<inner>(.*?))\[/font\]", _options );
		static private Regex _rgxColor = new Regex( @"\[color=(?<color>(\#?[-a-z0-9]*))\](?<inner>(.*?))\[/color\]", _options );
		static private Regex _rgxBullet = new Regex( @"\[\*\]", _options );
		static private Regex _rgxList4 = new Regex( @"\[list=i\](?<inner>(.*?))\[/list\]", _options );
		static private Regex _rgxList3 = new Regex( @"\[list=a\](?<inner>(.*?))\[/list\]", _options );
		static private Regex _rgxList2 = new Regex( @"\[list=1\](?<inner>(.*?))\[/list\]", _options );
		static private Regex _rgxList1 = new Regex( @"\[list\](?<inner>(.*?))\[/list\]", _options );
		static private Regex _rgxCenter = new Regex( @"\[center\](?<inner>(.*?))\[/center\]", _options );
		static private Regex _rgxLeft = new Regex( @"\[left\](?<inner>(.*?))\[/left\]", _options );
		static private Regex _rgxRight = new Regex( @"\[right\](?<inner>(.*?))\[/right\]", _options );
		static private Regex _rgxQuote2 = new Regex( @"\[quote=(?<quote>[^\]]*)\](?<inner>(.*?))\[/quote\]", _options );
		static private Regex _rgxQuote1 = new Regex( @"\[quote\](?<inner>(.*?))\[/quote\]", _options );
		static private Regex _rgxHr = new Regex( "^[-][-][-][-][-]*[\r]?[\n]", _options );
		static private Regex _rgxBr = new Regex( "[\r]?\n", _options );
		static private Regex _rgxPost = new Regex( @"\[post=(?<post>[^\]]*)\](?<inner>(.*?))\[/post\]", _options );
		static private Regex _rgxTopic = new Regex( @"\[topic=(?<topic>[^\]]*)\](?<inner>(.*?))\[/topic\]", _options );
		static private Regex _rgxImg = new Regex( @"\[img\](?<http>(http://)|(https://)|(ftp://)|(ftps://))?(?<inner>(.*?))\[/img\]", _options );
		static private Regex _rgxYoutube = new Regex( @"\[youtube\](?<inner>http://(www\.)?youtube.com/watch\?v=(?<id>[0-9A-Za-z-_]{11})[^[]*)\[/youtube\]", _options);

		static public string MakeHtml( string bbcode, bool doFormatting )
		{
			System.Collections.ArrayList codes = new System.Collections.ArrayList();
			const string codeFormat = ".code@{0}.";

			string localQuoteStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_QUOTE" );
			string localCodeStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_CODE" );
			string localQuoteWroteStr = yaf_Context.Current.Localization.GetText( "COMMON", "BBCODE_QUOTEWROTE" );

			Match m = _rgxCode2.Match( bbcode );
			int nCodes = 0;

			while ( m.Success )
			{
				string before_replace = m.Groups [0].Value;
				string after_replace = m.Groups ["inner"].Value;

				try
				{
					HighLighter hl = new HighLighter();
					hl.ReplaceEnter = true;
					after_replace = hl.ColorText( after_replace, HttpContext.Current.Server.MapPath( yaf_ForumInfo.ForumRoot + "defs/" ), m.Groups ["language"].Value );
				}
				catch ( Exception x )
				{
					if ( yaf_Context.Current.IsAdmin )
						yaf_Context.Current.AddLoadMessage( x.Message );
					after_replace = FixCode( after_replace );
				}

				bbcode = bbcode.Replace( before_replace, string.Format( codeFormat, nCodes++ ) );
				codes.Add( string.Format( @"<div class=""code""><b>{1}</b><div class=""innercode"">{0}</div></div>", after_replace, localCodeStr ) );
				m = _rgxCode2.Match( bbcode );

			}

			m = _rgxCode1.Match( bbcode );
			while ( m.Success )
			{
				string before_replace = m.Groups [0].Value;
				string after_replace = FixCode( m.Groups ["inner"].Value );
				bbcode = bbcode.Replace( before_replace, string.Format( codeFormat, nCodes++ ) );
				codes.Add( string.Format( @"<div class=""code""><b>{1}</b><div class=""innercode"">{0}</div></div>", after_replace, localCodeStr ) );
				m = _rgxCode1.Match( bbcode );
			}

			m = _rgxSize.Match( bbcode );

			while ( m.Success )
			{
				///Console.WriteLine("{0}",m.Groups["size"]);
				int i = GetNumber( m.Groups ["size"].Value );
				string tmp;
				if ( i < 1 )
					tmp = m.Groups ["inner"].Value;
				else if ( i > 9 )
					tmp = string.Format( "<span style=\"font-size:{1}\">{0}</span>", m.Groups ["inner"].Value, GetFontSize( 9 ) );
				else
					tmp = string.Format( "<span style=\"font-size:{1}\">{0}</span>", m.Groups ["inner"].Value, GetFontSize( i ) );
				bbcode = bbcode.Substring( 0, m.Groups [0].Index ) + tmp + bbcode.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = _rgxSize.Match( bbcode );
			}

			if ( doFormatting )
			{
				NestedReplace( ref bbcode, _rgxBold, "<b>${inner}</b>" );
				NestedReplace( ref bbcode, _rgxStrike, "<s>${inner}</s>" );
				NestedReplace( ref bbcode, _rgxItalic, "<i>${inner}</i>" );
				NestedReplace( ref bbcode, _rgxUnderline, "<u>${inner}</u>" );
				// e-mails
				NestedReplace( ref bbcode, _rgxEmail2, "<a href=\"mailto:${email}\">${inner}</a>", new string [] { "email" } );
				NestedReplace( ref bbcode, _rgxEmail1, "<a href=\"mailto:${inner}\">${inner}</a>" );
				// urls
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
				{
					NestedReplace( ref bbcode, _rgxUrl2, "<a target=\"_blank\" rel=\"nofollow\" href=\"${http}${url}\">${inner}</a>", new string [] { "url", "http" }, new string [] { "", "http://" } );
					NestedReplace( ref bbcode, _rgxUrl1, "<a target=\"_blank\" rel=\"nofollow\" href=\"${http}${inner}\">${http}${inner}</a>", new string [] { "http" }, new string [] { "http://" } );
				}
				else
				{
					NestedReplace( ref bbcode, _rgxUrl2, "<a rel=\"nofollow\" href=\"${http}${url}\">${inner}</a>", new string [] { "url", "http" }, new string [] { "", "http://" } );
					NestedReplace( ref bbcode, _rgxUrl1, "<a rel=\"nofollow\" href=\"${http}${inner}\">${http}${inner}</a>", new string [] { "http" }, new string [] { "http://" } );
				}
				// font
				NestedReplace( ref bbcode, _rgxFont, "<span style=\"font-family:${font}\">${inner}</span>", new string [] { "font" } );
				NestedReplace( ref bbcode, _rgxColor, "<span style=\"color:${color}\">${inner}</span>", new string [] { "color" } );
				// bullets
				bbcode = _rgxBullet.Replace( bbcode, "<li>" );
				NestedReplace( ref bbcode, _rgxList4, "<ol type=\"i\">${inner}</ol>" );
				NestedReplace( ref bbcode, _rgxList3, "<ol type=\"a\">${inner}</ol>" );
				NestedReplace( ref bbcode, _rgxList2, "<ol>${inner}</ol>" );
				NestedReplace( ref bbcode, _rgxList1, "<ul>${inner}</ul>" );
				// alignment
				NestedReplace( ref bbcode, _rgxCenter, "<div align=\"center\">${inner}</div>" );
				NestedReplace( ref bbcode, _rgxLeft, "<div align=\"left\">${inner}</div>" );
				NestedReplace( ref bbcode, _rgxRight, "<div align=\"right\">${inner}</div>" );
				// image
				NestedReplace( ref bbcode, _rgxImg, "<img src=\"${http}${inner}\"/>", new string [] { "http" }, new string [] { "http://" } );
				// youtube
				NestedReplace( ref bbcode, _rgxYoutube, @"<!-- BEGIN youtube --><object width=""425"" height=""350""><param name=""movie"" value=""http://www.youtube.com/v/${id}""></param><embed src=""http://www.youtube.com/v/${id}"" type=""application/x-shockwave-flash"" width=""425"" height=""350""></embed></object><br /><a href=""http://youtube.com/watch?v=${id}"" target=""_blank"">${inner}</a><br /><!-- END youtube -->", new string [] { "id" } );

				bbcode = _rgxHr.Replace( bbcode, "<hr noshade/>" );
				bbcode = _rgxBr.Replace( bbcode, "<br/>" );
			}

			bbcode = FormatMsg.AddSmiles( bbcode );

			string tmpReplaceStr;

			tmpReplaceStr = string.Format( @"<div class=""quote""><b>{0}</b><div class=""innerquote"">{1}</div></div>", localQuoteWroteStr.Replace( "{0}", "${quote}" ), "${inner}" );

			while ( _rgxQuote2.IsMatch( bbcode ) )
				bbcode = _rgxQuote2.Replace( bbcode, tmpReplaceStr );

			tmpReplaceStr = string.Format( @"<div class=""quote""><b>{0}</b><div class=""innerquote"">{1}</div></div>", localQuoteStr, "${inner}" );
						
			while ( _rgxQuote1.IsMatch( bbcode ) )
				bbcode = _rgxQuote1.Replace( bbcode, tmpReplaceStr );

			m = _rgxPost.Match( bbcode );
			while ( m.Success )
			{
				string link = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", m.Groups ["post"] );
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a target=\"_blank\" href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				else
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				m = _rgxPost.Match( bbcode );
			}

			m = _rgxTopic.Match( bbcode );
			while ( m.Success )
			{
				string link = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", m.Groups ["topic"] );
				if ( yaf_Context.Current.BoardSettings.BlankLinks )
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a target=\"_blank\" href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				else
					bbcode = bbcode.Replace( m.Groups [0].ToString(), string.Format( "<a href=\"{0}\">{1}</a>", link, m.Groups ["inner"] ) );
				m = _rgxTopic.Match( bbcode );
			}

			while ( nCodes > 0 )
			{
				bbcode = bbcode.Replace( string.Format( codeFormat, --nCodes ), codes [nCodes].ToString() );
			}

			return bbcode;
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace, string [] variables, string [] varDefaults )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace;
				int i = 0;

				foreach ( string tVar in variables )
				{
					string tValue = m.Groups [tVar].Value;
					if ( tValue.Length == 0 )
					{
						// use default instead
						tValue = varDefaults [i];
					}
					tStr = tStr.Replace( "${" + tVar + "}", tValue );
					i++;
				}

				tStr = tStr.Replace( "${inner}", m.Groups ["inner"].Value );

				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace, string [] variables )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace;

				foreach ( string tVar in variables )
				{
					tStr = tStr.Replace( "${" + tVar + "}", m.Groups [tVar].Value );
				}

				tStr = tStr.Replace( "${inner}", m.Groups ["inner"].Value );

				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static protected void NestedReplace( ref string refText, Regex regexMatch, string strReplace )
		{
			Match m = regexMatch.Match( refText );
			while ( m.Success )
			{
				string tStr = strReplace.Replace( "${inner}", m.Groups ["inner"].Value );
				refText = refText.Substring( 0, m.Groups [0].Index ) + tStr + refText.Substring( m.Groups [0].Index + m.Groups [0].Length );
				m = regexMatch.Match( refText );
			}
		}

		static public string EncodeHTML( string html )
		{
			return System.Web.HttpContext.Current.Server.HtmlEncode( html );
		}

		static public string DecodeHTML( string text )
		{
			return System.Web.HttpContext.Current.Server.HtmlDecode( text );
		}

		static private string FixCode( string html )
		{
			html = html.Replace( "  ", "&nbsp; " );
			html = html.Replace( "  ", " &nbsp;" );
			html = html.Replace( "\t", "&nbsp; &nbsp;&nbsp;" );
			html = html.Replace( "[", "&#91;" );
			html = html.Replace( "]", "&#93;" );
			html = html.Replace( "<", "&lt;" );
			html = html.Replace( ">", "&gt;" );
			html = html.Replace( "\r\n", "<br/>" );
			return html;
		}
	}
}
